using Godot;
using System;

public partial class Player : CharacterBody2D
{
	// Seção de Variáveis
	// Vida
	public double maxHealth = 50;
	[Export] public double health = 50.0;
	bool isAlive = true;

	// Colisões
	private CollisionShape2D hitboxShape;
	private Area2D enemyDetector;

	// Knockback
	Vector2 knockback;
	float knockbackTimer = 0;
	[Signal] public delegate void PlayerDiedEventHandler();
	[Signal] public delegate void HealthChangedEventHandler();

	// Animação
	private AnimatedSprite2D sprite;
	private AnimationPlayer animationPlayer;

	// Movimento
	double walkSpeed = 175.0;
	double acceleration = 0.1; //até 1
	double deceleration = 0.075; //até 1
	float direction = 1;

	// Variaveis de pulo
	bool canJump = true;
	double coyoteTime = 0.2;
	double jumpForce = -500.0;
	double decelerateOnJumpRelease = 0.5; //até 1
	public double gravity = (double)ProjectSettings.GetSetting("physics/2d/default_gravity");

	// Variaveis de dash
	double dashSpeed = 100.0;
	double dashMaxDistance = 100.0;
	bool backDash = false;
	float lastDirection = 1;
	[Export] public Curve dashCurve;
	public double dashCooldown = 1.0;
	public bool isDashing = false;
	double dashStartPosition = 0;
	double dashDirection = 0;
	public double dashTimer = 0;

	// Tempo de Imunidade e Combate
	public double immunityTime = 2;
	public bool isAttacking = false;
	public bool isInsideEnemy = false;

	// Combo
	public int comboStep = 0; // Qual ataque da sequência.
	public float attackTimer = 0f; // Tempo para o próximo ataque.
	public const float attackCooldown = 0.5f; // Tempo de recarga do ataque.

	// Camera
	public Camera2D camera;
	
	AudioStreamPlayer2D audio;

	// Itens
	public int wallet { get; private set; } = 0;
	public int lifeRefill { get; private set; } = 0;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		enemyDetector = GetNode<Area2D>("EnemyDetector");
		hitboxShape = GetNode<CollisionShape2D>("Hitbox/CollisionShape2D");
		audio = GetNode<AudioStreamPlayer2D>("Sounds/Collect");
		camera = GetNode<Camera2D>("Camera2D");

		// Desativa a Hitbox por padrão.
		hitboxShape.Disabled = true;

		GD.Print($"{Name} ready with health: {health} and max health: {maxHealth}");

		// Conecta os delegates (event handlers) para os sinais
		animationPlayer.AnimationFinished += OnAnimationFinished;
		enemyDetector.AreaEntered += OnAreaEntered;
		enemyDetector.AreaExited += OnAreaExited;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!isAlive) { return; }

		// Aplica Gravidade e Coyote Time.
		if (!IsOnFloor())
		{
			Velocity = new Vector2(Velocity.X, (float)(Velocity.Y + gravity * delta));
			coyoteTime -= (float)delta;
			if (coyoteTime <= 0)
			{
				canJump = false;
			}
		}
		else
		{
			coyoteTime = 0.2f; // Reseta o coyote time quando está no chão.
			canJump = true;
		}

		// * Aqui é onde as ações do jogador serão processadas
		Attack();
		Animations();
		CameraZoom();

		if (knockbackTimer > 0)
		{
			Velocity = new Vector2(knockback.X, knockback.Y);
			knockbackTimer -= (float)delta;
			if (knockbackTimer <= 0)
			{
				knockback = Vector2.Zero; // Reseta o knockback quando o tempo acaba;

				// Volta para a cor padrão depois de ser atingido.
				if (immunityTime > 0)
				{
					sprite.Modulate = new Color(1, 1, 1, 1);
				}
			}
		}
		else
		{
			if (!isAttacking)
			{
				Movement(delta);
			}
		}

		// ! Só diminui o contador se for maior que zero
		if (immunityTime > 0)
		{
			immunityTime -= delta;
		}

		if (attackTimer > 0)
		{
			attackTimer -= (float)delta;
		}

		MoveAndSlide();
	}

	// ? Ele verifica se tá dentro ou não, mas e agora?
	private void OnAreaEntered(Area2D area)
	{
		if (area.IsInGroup("enemyHitbox"))
		{
			isInsideEnemy = true;
		}
	}

	private void OnAreaExited(Area2D area)
	{
		if (area.IsInGroup("enemyHitbox"))
		{
			isInsideEnemy = false;
		}
	}

	public void CollectItens(int value, string itemType)
	{
		GD.Print($"Collected {itemType}: " + value);

		if (itemType == "money")
		{
			audio.Play();
			wallet += value;
		}
		else if (itemType == "lifeRefill")
		{
			lifeRefill += value;
		}
	}

	private void CameraZoom()
	{
		if (Input.IsActionJustPressed("zoomIn") && camera.Zoom.X > 0)
		{
			camera.Zoom += new Vector2(0.1f, 0.1f);
		}
		else if (Input.IsActionJustPressed("zoomOut") && camera.Zoom.X < 0.8)
		{
			camera.Zoom -= new Vector2(0.1f, 0.1f);
		}
	}

	private void OnAnimationFinished(StringName animName)
	{
		if (animName == "punch1")
		{
			if (comboStep > 1)
			{
				animationPlayer.Play("punch2");
			}
			else
			{
				StopAttack();
			}
		}
		else if (animName == "punch2")
		{
			StopAttack();
		}
	}

	public void Attack()
	{
		if (Input.IsActionJustPressed("attack") && !isDashing)
		{
			if (attackTimer <= 0)
			{
				Velocity = new Vector2(lastDirection < 0 ? -2.5f : 2.5f, Velocity.Y * 0.75f);

				isAttacking = true;
				hitboxShape.Disabled = false;

				// Acrescenta a sequência de combos.
				comboStep++;
			}
		}
	}

	public void StopAttack()
	{
		sprite.Offset = new Vector2(0f, 0f);

		comboStep = 0;
		hitboxShape.Disabled = true;
		isAttacking = false;
		attackTimer = attackCooldown;
	}

	// Movimentação do jogador
	public void Movement(double delta)
	{
		// *Pega a última direção
		if (direction != 0)
		{
			lastDirection = direction;
		}

		// * Pulo
		if (Input.IsActionJustPressed("jump") && canJump == true)
		{
			canJump = false;
			Velocity = new Vector2(Velocity.X, (float)jumpForce);
		}
		if (Input.IsActionJustReleased("jump") && Velocity.Y < 0)
		{
			Velocity = new Vector2(Velocity.X, (float)(Velocity.Y * decelerateOnJumpRelease));
		}

		// * Movimento
		if (Input.IsActionPressed("left") && Input.IsActionPressed("right"))
		{
			direction = 0;
			Velocity = new Vector2(Mathf.MoveToward(Velocity.X, 0, (float)(walkSpeed * deceleration)), Velocity.Y);
		}
		else if (Input.IsActionPressed("left") && !IsOnWall())
		{
			hitboxShape.Transform = new Transform2D(0, new Vector2(-24, 0));
			direction = -1;
			sprite.FlipH = true;

			Velocity = new Vector2(Mathf.MoveToward(Velocity.X, (float)(direction * walkSpeed), (float)(walkSpeed * acceleration)), Velocity.Y);
		}
		else if (Input.IsActionPressed("right") && !IsOnWall())
		{
			hitboxShape.Transform = new Transform2D(0, new Vector2(24, 0));
			direction = 1;
			sprite.FlipH = false;

			Velocity = new Vector2(Mathf.MoveToward(Velocity.X, (float)(direction * walkSpeed), (float)(walkSpeed * acceleration)), Velocity.Y);
		}
		else
		{
			direction = 0;
			Velocity = new Vector2(Mathf.MoveToward(Velocity.X, 0, (float)(walkSpeed * deceleration)), Velocity.Y);
		}

		// * Sistema de Dash
		if (Input.IsActionJustPressed("dash") && dashTimer <= 0)
		{
			isDashing = true;
			dashStartPosition = Position.X;

			if (direction != 0)
			{
				dashDirection = direction;
			}
			else
			{
				backDash = true;
				dashDirection = -lastDirection;
			}

			dashTimer = dashCooldown;
		}

		if (isDashing)
		{
			double currentDistance = Math.Abs(Position.X - dashStartPosition);

			if (currentDistance >= dashMaxDistance || IsOnWall())
			{
				isDashing = false;
				backDash = false;
			}
			else
			{
				immunityTime = 0.45;

				// * Mover o personagem
				double curveFactor = dashCurve.Sample((float)Math.Abs(currentDistance / dashMaxDistance));
				Velocity = new Vector2((float)(Velocity.X + dashDirection * dashSpeed * curveFactor), Velocity.Y);
				Velocity = new Vector2(Velocity.X, 0);
			}
		}

		// * Cooldown do dash
		if (dashTimer > 0)
		{
			dashTimer -= delta;
		}
	}

	public void Animations()
	{
		if (isAttacking)
		{
			if (attackTimer <= 0)
			{
				if (lastDirection < 0)
				{
					sprite.Offset = new Vector2(-12, 0);
				}
				else
				{
					sprite.Offset = new Vector2(12, 0);
				}

				if (comboStep == 1 && animationPlayer.CurrentAnimation != "punch1")
				{
					animationPlayer.Play("punch1");
				}
			}
		}
		else
		{
			if (!IsOnFloor())
			{
				if (Velocity.Y < 0)
				{
					if (animationPlayer.CurrentAnimation != "jump")
					{
						animationPlayer.Play("jump");
					}
				}
				else
				{
					if (animationPlayer.CurrentAnimation != "fall")
					{
						animationPlayer.Play("fall");
					}
				}
			}

			if (direction != 0 && IsOnFloor() && !IsOnWall())
			{
				if (animationPlayer.CurrentAnimation != "walk")
				{
					animationPlayer.Play("walk");
				}
			}
			else if (direction == 0 && IsOnFloor() && !isAttacking)
			{
				if (animationPlayer.CurrentAnimation != "idle")
				{
					animationPlayer.Play("idle");
				}
			}

			if (isDashing)
			{
				if (!backDash)
				{
					animationPlayer.Play("dash");
				}
				else
				{
					animationPlayer.Play("backDash");
				}
			}

			if (health <= 0)
			{
				animationPlayer.Play("death");
			}
		}
	}

	// Função para receber dano
	public void Hurt(double damage, Vector2 hitboxLocation, float knockbackForce)
	{
		// Verifica se o jogador está imune a ataques
		if (immunityTime <= 0)
		{
			// Pisca o sprite
			sprite.Modulate = new Color(5f, 1, 1, 0.75f);

			health -= damage;
			GD.Print($"{Name} recebeu {damage} de dano, vida restante: {health}");

			if (health <= 0)
			{
				isAlive = false;
				EmitSignal(nameof(PlayerDied));
				GetTree().CreateTimer(0.01).Timeout += () => QueueFree();
				GD.Print("Jogador morreu");
			}
			else
			{
				isDashing = false;
				knockback = (GlobalPosition - hitboxLocation).Normalized() * new Vector2(knockbackForce, 100);
				knockbackTimer = 0.15f;
				EmitSignal(nameof(HealthChanged));
				immunityTime += 0.5;
			}
		}
	}
}

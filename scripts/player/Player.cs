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
	double deceleration = 0.1; //até 1
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

	// Camera
	public Camera2D camera;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		enemyDetector = GetNode<Area2D>("EnemyDetector");
		hitboxShape = GetNode<CollisionShape2D>("Hitbox/CollisionShape2D");
		camera = GetNode<Camera2D>("Camera2D");

		// Desativa a Hitbox por padrão.
		hitboxShape.Disabled = true;

		GD.Print($"{Name} ready with health: {health} and max health: {maxHealth}");

		// Conecta os delegates (event handlers) para os sinais
		animationPlayer.AnimationFinished += OnAnimationFinished;
		enemyDetector.AreaEntered += OnAreaEntered;
		enemyDetector.AreaExited += OnAreaExited;
	}

	// Processamento de fisica do jogador
	public override void _PhysicsProcess(double delta)
	{
		if (!IsInsideTree()) return;
		if (!isAlive) return;

		if (Input.IsActionJustPressed("menu"))
		{
			GetTree().ChangeSceneToFile("res://scenes/main/Main.tscn");
		}

		// Aplica Gravidade e Coyote Time
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
			coyoteTime = 0.2f; // Reseta o coyote time quando está no chão;
			canJump = true;
		}

		// Funções Diversas
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
			}
		}
		else
		{
			if (!isAttacking)
			{
				Movement(delta);
			}
		}

		// Só diminui o contador se for maior que zero
		if (immunityTime > 0)
		{
			immunityTime -= delta;
		}

		// Processa o Movimento
		MoveAndSlide();
	}

	// !! Ele verifica se tá dentro ou não, mas e agora?
	private void OnAreaEntered(Area2D area)
	{
		if (area.IsInGroup("enemyHitbox"))
		{
			isInsideEnemy = true;
			GD.Print($"Entrou de: {area.Name}");
		}
	}

	private void OnAreaExited(Area2D area)
	{
		if (area.IsInGroup("enemyHitbox"))
		{
			isInsideEnemy = false;
			GD.Print($"Saiu de: {area.Name}");
		}
	}

	private void CameraZoom()
	{
		if (Input.IsActionJustPressed("zoomIn") && camera.Zoom.X > 0)
		{
			camera.Zoom += new Vector2(0.1f, 0.1f);
		}
		else if (Input.IsActionJustPressed("zoomOut"))
		{
			camera.Zoom -= new Vector2(0.1f, 0.1f);
		}
	}

	private void OnAnimationFinished(StringName animName)
	{
		if (animName == "punch1")
		{
			isAttacking = false;
			hitboxShape.Disabled = true;
			sprite.Offset = new Vector2(0, 0);
		}
	}

	// ? Eu deveria fazer um handler para isso?
	// TODO: Um dia, eu faço combos, um dia...
	public void Attack()
	{
		if (!isAttacking && Input.IsActionJustPressed("attack"))
		{
			Velocity = Vector2.Zero;
			hitboxShape.Disabled = false;
			isAttacking = true;
		}
	}

	// Movimentação do jogador
	public void Movement(double delta)
	{
		// Pega a última direção
		if (direction != 0)
		{
			lastDirection = direction;
		}

		// Pulo
		if (Input.IsActionJustPressed("jump") && canJump == true)
		{
			canJump = false;
			Velocity = new Vector2(Velocity.X, (float)jumpForce);
		}

		if (Input.IsActionJustReleased("jump") && Velocity.Y < 0)
		{
			Velocity = new Vector2(Velocity.X, (float)(Velocity.Y * decelerateOnJumpRelease));
		}

		// Andar
		if (Input.IsActionPressed("left") && Input.IsActionPressed("right"))
		{
			direction = 0;
			Velocity = new Vector2(Mathf.MoveToward(Velocity.X, 0, (float)(walkSpeed * deceleration)), Velocity.Y);
		}
		else if (Input.IsActionPressed("left"))
		{
			hitboxShape.Transform = new Transform2D(0, new Vector2(-24, 0));
			direction = -1;
			sprite.FlipH = true;

			Velocity = new Vector2(Mathf.MoveToward(Velocity.X, (float)(direction * walkSpeed), (float)(walkSpeed * acceleration)), Velocity.Y);
		}
		else if (Input.IsActionPressed("right"))
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

		// Sistema de Dash
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

				// Mover o personagem
				double curveFactor = dashCurve.Sample((float)Math.Abs(currentDistance / dashMaxDistance));
				Velocity = new Vector2((float)(Velocity.X + dashDirection * dashSpeed * curveFactor), Velocity.Y);
				Velocity = new Vector2(Velocity.X, 0);
			}
		}

		// Cooldown do dash
		if (dashTimer > 0)
		{
			dashTimer -= delta;
		}
	}

	public void Animations()
	{
		// Volta para a cor padrão.
		if (immunityTime > 0)
		{
			sprite.Modulate = new Color(1, 1, 1, 1);
		}

		if (Input.IsActionJustPressed("attack"))
		{
			if (animationPlayer.CurrentAnimation != "punch1")
			{
				animationPlayer.Play("punch1");

				if (lastDirection < 0)
				{
					sprite.Offset = new Vector2(-12, 0);
				}
				else
				{
					sprite.Offset = new Vector2(12, 0);
				}
			}
		}

		if (!isAttacking)
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

			if (direction != 0 && IsOnFloor())
			{
				if (animationPlayer.CurrentAnimation != "walk")
				{
					animationPlayer.Play("walk");
				}
			}
			else if (direction == 0 && IsOnFloor())
			{
				if (animationPlayer.CurrentAnimation != "idle" && !isAttacking)
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
			GD.Print($"Jogador {Name} recebeu {damage} de dano, vida restante: {health}");

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
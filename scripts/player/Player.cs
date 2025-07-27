using Godot;
using System;

public partial class Player : CharacterBody2D
{
	// Variaveis do combate
	double maxHealth = 50;
	[Export] public double health = 50.0;
	bool isAlive = true;


	//knockback
	Vector2 knockback;
	float knockbackTimer = 0;
	[Signal] public delegate void PlayerDiedEventHandler();
	[Signal] public delegate void HealthChangedEventHandler();


	// Variaveis de animação
	private AnimatedSprite2D sprite;


	// Variaveis de movimento
	double walkSpeed = 175.0;
	double acceleration = 0.1; //até 1
	double deceleration = 0.1; //até 1
	float direction = 1;


	// Variaveis de pulo
	bool canJump = true;
	double coyoteTime = 0.2;
	double jumpForce = -500.0;
	double decelerateOnJumpRelease = 0.5; //até 1
	double gravity = (double)ProjectSettings.GetSetting("physics/2d/default_gravity");


	// Variaveis de dash
	double dashSpeed = 100.0;
	double dashMaxDistance = 100.0;
	[Export] public Curve dashCurve;
	double dashCooldown = 1.0;

	

	bool isDashing = false;
	double dashStartPosition = 0;
	double dashDirection = 0;
	double dashTimer = 0;

	double immunityTime = 0.5;



	public override void _Ready()
	{

		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		GD.Print($"Player {Name} ready with health: {health} and max health: {maxHealth}");

	}

	// Processamento de fisica do jogador
	public override void _PhysicsProcess(double delta)
	{
		if (!isAlive)
			return;

		//Botão para pausar o jogo
		if (Input.IsActionJustPressed("menu"))
		{
			GetTree().ChangeSceneToFile("res://scenes/main/Main.tscn");
			return;
		}

		// Adiciona a gravidade e coyote time
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
			coyoteTime = 0.3f; // Resetando o coyote time quando está no chão
			canJump = true;
		}

		if (knockbackTimer > 0)
		{
			Velocity = new Vector2(knockback.X, knockback.Y);
			knockbackTimer -= (float)delta;
			if (knockbackTimer <= 0)
			{
				knockback = Vector2.Zero; // Reseta o knockback quando o tempo acaba
			}
		}
		else
		{
			// Movimento do jogador
			Movement(delta);
		}



		immunityTime -= delta;
		MoveAndSlide();
	}



	public void Movement(double delta)
	{
		// Pulo
		if (Input.IsActionJustPressed("jump") && canJump == true)
		{
			canJump = false;
			Velocity = new Vector2(Velocity.X, (float)jumpForce);
			// GD.Print("Pulando");
		}

		if (Input.IsActionJustReleased("jump") && Velocity.Y < 0)
		{
			Velocity = new Vector2(Velocity.X, (float)(Velocity.Y * decelerateOnJumpRelease));
		}

		//Apenas para visual enquanto prototipo
		if (!isDashing && sprite.RotationDegrees != 0)
		{
			sprite.RotationDegrees = 0;
		}


		// Andar
		if (Input.IsActionPressed("left") && Input.IsActionPressed("right"))
		{
			direction = 0;
			Velocity = new Vector2(Mathf.MoveToward(Velocity.X, 0, (float)(walkSpeed * deceleration)), Velocity.Y);
		}
		else if (Input.IsActionPressed("left"))
		{
			direction = -1;
			sprite.FlipH = true;
			Velocity = new Vector2(Mathf.MoveToward(Velocity.X, (float)(direction * walkSpeed), (float)(walkSpeed * acceleration)), Velocity.Y);
		}
		else if (Input.IsActionPressed("right"))
		{
			direction = 1;
			sprite.FlipH = false;
			Velocity = new Vector2(Mathf.MoveToward(Velocity.X, (float)(direction * walkSpeed), (float)(walkSpeed * acceleration)), Velocity.Y);
		}
		else
		{
			direction = 0;
			Velocity = new Vector2(Mathf.MoveToward(Velocity.X, 0, (float)(walkSpeed * deceleration)), Velocity.Y);
		}
		


		// Codigo para o dash
		if (Input.IsActionJustPressed("dash") && direction != 0 && dashTimer <= 0)
		{
			isDashing = true;
			dashStartPosition = Position.X;
			dashDirection = direction;
			dashTimer = dashCooldown;

			//Apenas para visual enquanto prototipo
			sprite.RotationDegrees = 25 * direction;
		}

		// Dash
		if (isDashing)
		{
			double currentDistance = Math.Abs(Position.X - dashStartPosition);

			if (currentDistance >= dashMaxDistance || IsOnWall())
			{
				isDashing = false;
				RotationDegrees = 0;
			}
			// Mover o personagem
			else
			{
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

	// Função para receber dano
	public void Hurt(double damage, Vector2 hitboxLocation, float knockbackForce)
	{
		//verifica se o jogador está imune a ataques
		
		if (immunityTime <= 0)
		{
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



using Godot;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

public partial class Player : CharacterBody2D
{
	// Variaveis do combate
	double max_health = 0;
	[Export] public double health = 50.0;
	bool is_alive = true;
	bool enemy_attack_cooldown = false;

	//knockback
	Vector2 knockback;
	float knockback_timer = 0;
	[Signal] public delegate void PlayerDiedEventHandler();
	[Signal] public delegate void HealthChangedEventHandler();



	// Variaveis de animação
	private AnimatedSprite2D sprite;

	// Variaveis de movimento
	double walk_speed = 150.0;
	double acceleration = 0.1; //até 1
	double deceleration = 0.1; //até 1
	float direction = 1;

	// Variaveis de pulo
	bool can_jump = true;
	double coyote_time = 0.2;
	double jump_force = -500.0;
	double decelerate_on_jump_release = 0.5; //até 1

	// Variaveis de dash
	double dash_speed = 100.0;
	double dash_max_distance = 100.0;
	[Export] public Curve dash_curve;
	double dash_cooldown = 1.0;

	double gravity = (double)ProjectSettings.GetSetting("physics/2d/default_gravity");

	// Definição para o Dash
	bool is_dashing = false;
	double dash_start_position = 0;
	double dash_direction = 0;
	double dash_timer = 0;



	//Inicialização do jogador
	public override void _Ready()
	{
		// Definindo a vida do jogador
		max_health = Global.PlayerMaxHealth;
		health = max_health;
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

	}

	// Processamento de fisica do jogador
	public override void _PhysicsProcess(double delta)
	{
		if (!is_alive)
		{ return; }

		// Adiciona a gravidade
		if (!IsOnFloor())
		{
			Velocity = new Vector2(Velocity.X, (float)(Velocity.Y + gravity * delta));
			coyote_time -= (float)delta;
			if (coyote_time <= 0)
			{
				can_jump = false;
			}
		}
		else
		{
			coyote_time = 0.3f; // Resetando o coyote time quando está no chão
			can_jump = true;
		}

		if (knockback_timer > 0)
		{
			Velocity = new Vector2(knockback.X, knockback.Y);
			knockback_timer -= (float)delta;
			if (knockback_timer <= 0)
			{
				knockback = Vector2.Zero;
			}
		}
		else
		{
			// Movimento do jogador
			Movement();
		}



		// Ativação do Dash
		if (Input.IsActionJustPressed("dash") && direction != 0 && !is_dashing && dash_timer <= 0)
		{
			is_dashing = true;
			dash_start_position = Position.X;
			dash_direction = direction;
			dash_timer = dash_cooldown;
			// GD.Print("Dash");

			sprite.RotationDegrees = 25 * direction;
		}

		// Dash
		if (is_dashing)
		{
			double current_distance = Math.Abs(Position.X - dash_start_position);

			if (current_distance >= dash_max_distance || IsOnWall())
			{
				is_dashing = false;
				RotationDegrees = 0;
			}
			// Mover o personagem
			else
			{
				double curve_factor = dash_curve.Sample((float)Math.Abs(current_distance / dash_max_distance));
				Velocity = new Vector2((float)(Velocity.X + dash_direction * dash_speed * curve_factor), Velocity.Y);
				Velocity = new Vector2(Velocity.X, 0);

			}
		}

		// Cooldown do dash
		if (dash_timer > 0)
		{
			dash_timer -= delta;
		}


		if (Input.IsActionJustPressed("menu"))
		{
			GD.Print("Voltando ao menu");
			GetTree().ChangeSceneToFile("res://scenes/main/Main.tscn");
			return;
		}
		

		MoveAndSlide();
	}



	public void Movement()
	{
		// Pulo
		if (Input.IsActionJustPressed("jump") && can_jump == true)
		{
			can_jump = false;
			Velocity = new Vector2(Velocity.X, (float)jump_force);
			// GD.Print("Pulando");
		}

		if (Input.IsActionJustReleased("jump") && Velocity.Y < 0)
		{
			Velocity = new Vector2(Velocity.X, (float)(Velocity.Y * decelerate_on_jump_release));
		}

		if (!is_dashing && sprite.RotationDegrees != 0)
		{
			sprite.RotationDegrees = 0;
		}

		// Direção
		if (Input.IsActionJustPressed("left"))
		{
			direction = -1;
		}
		else if (Input.IsActionJustPressed("right"))
		{
			direction = 1;
		}


		if (Input.IsActionPressed("left") || Input.IsActionPressed("right"))
		{
			if (direction > 0)
			{
				sprite.FlipH = false;
				Velocity = new Vector2(Mathf.MoveToward(Velocity.X, (float)(direction * walk_speed), (float)(walk_speed * acceleration)), Velocity.Y);
			}
			else if (direction < 0)
			{
				sprite.FlipH = true;
				Velocity = new Vector2(Mathf.MoveToward(Velocity.X, (float)(direction * walk_speed), (float)(walk_speed * acceleration)), Velocity.Y);
			}
		}
		else
		{
			Velocity = new Vector2(Mathf.MoveToward(Velocity.X, 0, (float)(walk_speed * deceleration)), Velocity.Y);
		}
	}





	// Função para receber dano
	public void Hurt(double damage, Vector2 hitbox_location)
	{
		if (enemy_attack_cooldown == false)
		{
			health -= damage;
			GD.Print($"Jogador recebeu {damage} de dano. Vida restante: {health}");
			if (health <= 0)
			{
				is_alive = false;
				EmitSignal(nameof(PlayerDied));
				GetTree().CreateTimer(0.01).Timeout += () => QueueFree();
				GD.Print("Jogador morreu");
			}
			else
			{
				is_dashing = false;
				var hit_direction = (GlobalPosition - hitbox_location).Normalized();
				Apply_Knockback(new Vector2(275, 100), hit_direction, 0.15f);
				EmitSignal(nameof(HealthChanged));
				enemy_attack_cooldown = true;
				GetTree().CreateTimer(0.4).Timeout += ResetEnemyAttackCooldown;
			}
		}
	}

	private void ResetEnemyAttackCooldown()
	{
		enemy_attack_cooldown = false;
	}



	public void Apply_Knockback(Vector2 knockbackForce, Vector2 direction, float knockback_duration)
	{
		knockback = direction * knockbackForce;
		knockback_timer = knockback_duration;
	}
}



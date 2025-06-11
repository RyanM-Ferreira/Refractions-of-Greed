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

	[Signal] public delegate void PlayerDiedEventHandler();



	// Variaveis de animação
	private AnimatedSprite2D sprite;

	// Variaveis de movimento
	double walk_speed = 150.0;
	double acceleration = 0.1; //até 1
	double deceleration = 0.1; //até 1

	// Variaveis de pulo
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


	public override void _Ready()
	{
		// Definindo a vida do jogador
		max_health = Global.PlayerMaxHealth;
		health = max_health;
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		Debug.Assert(sprite != null, "Sprite não encontrado.");

	}

	public override void _PhysicsProcess(double delta)
	{
		if (!is_alive)
		{ return; }

		// Adiciona a gravidade
		if (!IsOnFloor())
		{
			Velocity = new Vector2(Velocity.X, (float)(Velocity.Y + gravity * delta));
		}


		// Pulo
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			Velocity = new Vector2(Velocity.X, (float)jump_force);
			// GD.Print("Pulando");
		}

		if (Input.IsActionJustReleased("jump") && Velocity.Y < 0)
		{
			Velocity = new Vector2(Velocity.X, (float)(Velocity.Y * decelerate_on_jump_release));
		}


		// Direção
		var direction = Input.GetAxis("left", "right");

		if (!is_dashing && RotationDegrees != 0)
		{
			RotationDegrees = 0;
		}

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
		else
		{
			Velocity = new Vector2(Mathf.MoveToward(Velocity.X, 0, (float)(walk_speed * deceleration)), Velocity.Y);
		}


		// Ativação do Dash
		if (Input.IsActionJustPressed("dash") && direction != 0 && !is_dashing && dash_timer <= 0)
		{
			is_dashing = true;
			dash_start_position = Position.X;
			dash_direction = direction;
			dash_timer = dash_cooldown;
			// GD.Print("Dash");

			RotationDegrees = 25 * direction;
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





	// Função para receber dano
	public void Hurt(double damage)
	{
		if (enemy_attack_cooldown == false)
		{
			GD.Print("Dano recebido: " + damage);
			health -= damage;
			GD.Print("vida: " + health);
			if (health <= 0)
			{
				is_alive = false;
				EmitSignal(nameof(PlayerDied));
				GetTree().CreateTimer(0.01).Timeout += () => QueueFree();
				GD.Print("Jogador morreu");
			}
			else
			{
				enemy_attack_cooldown = true;
				GetTree().CreateTimer(1.0).Timeout += ResetEnemyAttackCooldown;
			}
		}
	}

	private void ResetEnemyAttackCooldown()
	{
		enemy_attack_cooldown = false;
	}
}

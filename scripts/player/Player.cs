using Godot;
using System;
using System.Diagnostics;

public partial class Player : CharacterBody2D
{
	// Os @export são para permitir a edição das variaveis pelo editor e outros arquivos
	[Export] float walk_speed = 150.0f;
	[Export(PropertyHint.Range, "0,1")] float acceleration = 0.1f;
	[Export(PropertyHint.Range, "0,1")] float deceleration = 0.1f;

	[Export] float jump_force = -400.0f;
	[Export(PropertyHint.Range, "0,1")] float decelerate_on_jump_release = 0.5f;

	[Export] float dash_speed = 1000.0f;
	[Export] float dash_max_distance = 300.0f;
	[Export] public Curve dash_curve;

	[Export] float dash_cooldown = 1.0f;

	float gravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");

	// Definição para o Dash
	bool is_dashing = false;
	float dash_start_position = 0; // para descobrir se atravesou a distancia maxima
	float dash_direction = 0;
	float dash_timer = 0;

	public override void _PhysicsProcess(double delta)
	{
		// Estados do jogador
		// var is_falling := velocity.y > 0.0 and not is_on_floor()
		// var is_jumping := Input.is_action_just_pressed("jump") and is_on_floor()
		// var is_idling := is_on_floor() and is_zero_approx(velocity.x)
		// var is_walking := is_on_floor() and not is_zero_approx(velocity.x)

		var sprite = GetNode<Sprite2D>("Sprite2D");
		Debug.Assert(sprite != null, "Esta bosta não foi encontrada.");

		// Adiciona a gravidade
		if (!IsOnFloor())
		{
			Velocity = new Vector2(Velocity.X, Velocity.Y + gravity * (float)delta);
		}
		if (IsOnFloor())
		{
			// GD.Print("Chão");
		}

		// Pulo
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			Velocity = new Vector2(Velocity.X, jump_force);
			// GD.Print("Pulando");
		}

		if (Input.IsActionJustReleased("jump") && Velocity.Y < 0)
		{
			Velocity = new Vector2(Velocity.X, Velocity.Y * decelerate_on_jump_release);
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
		}
		else if (direction < 0)
		{
			sprite.FlipH = true;
		}

		// Andar
		if (direction != 0)
		{
			GD.Print("Moving");
			GD.Print(direction);
			Velocity = new Vector2(Mathf.MoveToward(Velocity.X, direction * walk_speed, (float)(walk_speed * acceleration)), Velocity.Y);
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
			GD.Print("Dash");

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
				double t = Math.Abs(current_distance / dash_max_distance);
				Velocity = new Vector2(Velocity.X + dash_direction * dash_speed * dash_curve.Sample((float)t), Velocity.Y);
				Velocity = new Vector2(Velocity.X, 0);

				is_dashing = false;
			}
		}

		// Cooldown do dash
		if (dash_timer > 0)
		{
			dash_timer -= (float)delta;
		}

		MoveAndSlide();
	}
}
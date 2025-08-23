using Godot;
using System;
public partial class Enemy : CharacterBody2D
{
	// Sessão de variáveis

	// Movimento
	public double Speed = 20.0;
	public double Gravity = 100.0;
	public float direction = -1.0f;

	// Combate
	public double max_health = 50.0;
	public double health = 50.0;
	public bool is_alive = true;
	public double immunityTime = 0.5;
	public Vector2 knockback;
	public float knockbackTimer = 0;

	[Signal] public delegate void HealthChangedEventHandler();

	public void Hurt(double damage, Vector2 hitboxLocation, float knockbackForce)
	{
		if (immunityTime <= 0)
		{
			health -= damage;
			GD.Print($"{Name} recebeu {damage} de dano, vida restante: {health}");
			if (health <= 0)
			{
				GetTree().CreateTimer(0.01).Timeout += () => QueueFree();
				GD.Print($"{Name} morreu");
			}
			else
			{
				knockback = (GlobalPosition - hitboxLocation).Normalized() * new Vector2(knockbackForce, 100);
				knockbackTimer = 0.15f;
				EmitSignal(nameof(HealthChanged));
				immunityTime += 0.5;
			}
		}
	}

	public void GroundEnemy(double delta, double Gravity, double direction, double Speed)
	{
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
			// Gravidade
			if (!IsOnFloor())
			{
				Velocity = new Vector2(Velocity.X, Velocity.Y + (float)Gravity * (float)delta);
			}

			// Mover o inimigo para frente
			Velocity = new Vector2(Mathf.MoveToward(Velocity.X, (float)(Speed * direction), (float)(Speed * delta)), Velocity.Y);
		}

		immunityTime -= delta;

		MoveAndSlide();
	}

	public void FlyingEnemy(double delta, double direction, double Speed)
	{
		// TODO: Complicado isso aí...
		GD.Print("FlyingEnemy");
	}
}
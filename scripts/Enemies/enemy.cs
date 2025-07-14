using Godot;
using System;
public partial class Enemy : CharacterBody2D
{
	public double Speed = 20.0;
	public double Gravity = 100.0;
	public double direction = -1.0;

	//combate
	public double max_health = 50.0;
	public double health = 50.0;
	public bool is_alive = true;

	double immunityTime = 0.5; 


	public void Hurt(double damage)
	{
		if (immunityTime <=0)
		{
			health -= damage;
			GD.Print($"Enemy {Name} took {damage} damage, remaining health: {health}");
			if (health <= 0)
			{
				is_alive = false;
				GetTree().QueueDelete(this);
				GD.Print($"Inimigo morto: " + Name);
			}
			else
			{
				immunityTime = 0.5;
			}
		}
	}



	public void GroundEnemy(double delta, double Gravity, double direction, double Speed)
	{
		//Gravidade
		if (!IsOnFloor())
		{
			Velocity = new Vector2(Velocity.X, Velocity.Y + (float)Gravity * (float)delta);
		}

		// Mover o inimigo para frente
		Velocity = new Vector2(Mathf.MoveToward(Velocity.X, (float)(Speed * direction), (float)(Speed * delta)), Velocity.Y);

		immunityTime -= delta;

		MoveAndSlide();
	}



	public void FlyingEnemy(double delta, double direction, double Speed)
	{
		GD.Print("FlyingEnemy");
	}
}

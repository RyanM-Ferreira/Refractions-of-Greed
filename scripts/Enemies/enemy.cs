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
	public bool player_attack_cooldown = false;


	public void Damage(double damage)
	{
		if (player_attack_cooldown == false)
		{
			health -= damage;
			if (health <= 0)
			{
				is_alive = false;
				GetTree().QueueDelete(this);
			}
			else
			{
				player_attack_cooldown = true;
				GetTree().CreateTimer(1.0).Timeout += ResetPlayerAttackCooldown;
			}
		}
	}

	private void ResetPlayerAttackCooldown()
	{
		player_attack_cooldown = false;
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

		MoveAndSlide();


	}



	public void FlyingEnemy(double delta, double direction, double Speed)
	{
		GD.Print("FlyingEnemy");
	}
}

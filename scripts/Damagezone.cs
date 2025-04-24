using Godot;
using System;


public partial class Damagezone : Area2D
{
	[Export] int Damage = 10;

	public void _on_body_entered(Node2D body)
	{
		GD.Print("you got hit!");
		if (body is Player player)
		{
		
			player.health -= Damage;
			GD.Print("Player health: " + player.health);
		}
	}
	
}

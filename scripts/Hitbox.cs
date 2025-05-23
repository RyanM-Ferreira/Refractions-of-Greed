using Godot;
using System;


public partial class Hitbox : Area2D
{
	[Export] int Damage = 10;

	public void _on_body_entered(Node2D body)
	{

		if (body.HasMethod("Damage"))
		{
			body.Call("Damage", Damage);
		}
	}

}

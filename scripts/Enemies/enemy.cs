using Godot;
using System;

public partial class Enemy : Node2D
{
	[Export] double SPEED = 100.0;
	public double direction = -1.0;
	
	[Export] double gravity = 9.8;

	public override void _Process(double delta)
	{
		// Mover o inimigo para frente 
		Position = new Vector2(Position.X + (float)(SPEED * direction * delta), Position.Y);
	}
}

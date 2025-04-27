using Godot;
using System;

public partial class Enemy : Node2D
{
	private RayCast2D ray_cast_right;
	private RayCast2D ray_cast_left;
	private AnimatedSprite2D animated_sprite;
	
	//public _Ready(){
		// Inicializa os raycasts
	//	ray_cast_right = GetNode<RayCast2D>("RayCastRight");
	//	ray_cast_left = GetNode<RayCast2D>("RayCastLeft");
	//	animated_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	//}


	const double SPEED = 100.0;
	private double direction = 1.0;

	//tem que fazer o @onready dos 2 raycast que são utilizados para verificar se o inimigo vai virar de diração

	//@onready do animated sprite

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (ray_cast_right.IsColliding())
		{
			direction = -1;
			animated_sprite.FlipH = true;
		}
		if (ray_cast_left.IsColliding())
		{
			direction = 1;
			animated_sprite.FlipH = 	 false;
		}
		// Mover o inimigo
		// esse tipo de inimigo só anda pra frente e pra trás e troca de direção quando colide com algo
		Position = new Vector2(Position.X + (float)(SPEED * direction * delta), Position.Y);
	}
}

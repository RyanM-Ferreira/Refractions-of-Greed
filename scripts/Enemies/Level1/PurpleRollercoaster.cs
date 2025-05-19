using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;


public partial class PurpleRollercoaster : Enemy
{
	//import AnimatedSprite2D
	private AnimatedSprite2D Sprite;
	private AnimationTree animationTree;
	private AnimationNodeStateMachinePlayback AnimationPlayback;

	//import hitbox
	private Area2D hitbox;


	//import RayCast2D
	private RayCast2D raycast_Left;
	private RayCast2D raycast_Right;
	private RayCast2D raycast_DownLeft;
	private RayCast2D raycast_DownRight;

	private bool is_attacking = false;

	public override void _Ready()
	{
		Sprite = GetNode<AnimatedSprite2D>("sprite");
		raycast_Right = GetNode<RayCast2D>("RayCast_Right");
		raycast_Left = GetNode<RayCast2D>("RayCast_Left");
		raycast_DownLeft = GetNode<RayCast2D>("RayCast_DownLeft");
		raycast_DownRight = GetNode<RayCast2D>("RayCast_DownRight");
		animationTree = GetNode<AnimationTree>("AnimationTree");
		hitbox = GetNode<Area2D>("sprite/Damagezone");

		AnimationPlayback = animationTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
	}

	public override void _Process(double delta)
	{
		//raycast 
		if (raycast_Left.IsColliding())
		{
			// Muda a direção do inimigo
			direction = 1;
		}
		if (raycast_Right.IsColliding())
		{
			// Muda a direção do inimigo
			direction = -1;
		}
		if (!raycast_DownLeft.IsColliding() && raycast_DownRight.IsColliding())
		{
			// Muda a direção do inimigo
			direction = -1;
		}
		if (!raycast_DownRight.IsColliding() && raycast_DownLeft.IsColliding()){
			direction = 1;
		}


		//gravidade
		if (!raycast_DownLeft.IsColliding() && !raycast_DownRight.IsColliding())
		{
			Position = new Vector2(Position.X, Position.Y + (float)(gravity * delta));
		}

		Position = new Vector2(Position.X + (float)(SPEED * direction * delta), Position.Y);

		// Verifica se o inimigo deve mudar de direção
		if (direction > 0)
		{
			Sprite.FlipH = true;
			hitbox.Scale = new Vector2(-1, 1);
		}
		else if (direction < 0)
		{
			Sprite.FlipH = false;
			hitbox.Scale = new Vector2(1, 1);
		}
	}
}

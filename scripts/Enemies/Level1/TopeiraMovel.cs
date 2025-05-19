using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;



public partial class TopeiraMovel : Enemy
{	//import AnimatedSprite2D
	private AnimatedSprite2D sprite;
	private AnimationTree animationTree;
	private AnimationNodeStateMachinePlayback AnimationPlayback;

	//import hitbox
	private Area2D hitbox;


	//import RayCast2D
	private RayCast2D raycast_left;
	private RayCast2D raycast_right;
	private RayCast2D raycast_Down;
	private RayCast2D playerdetect;

	private bool is_attacking = false;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("sprite");
		raycast_right = GetNode<RayCast2D>("RayCast_Right");
		raycast_left = GetNode<RayCast2D>("RayCast_Left");
		raycast_Down = GetNode<RayCast2D>("RayCast_Down");
		playerdetect = GetNode<RayCast2D>("Player_detection");
		animationTree = GetNode<AnimationTree>("AnimationTree");
		hitbox = GetNode<Area2D>("sprite/Damagezone");

		AnimationPlayback = animationTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
	}

	public override void _Process(double delta)
	{
		//raycast 
		if (raycast_left.IsColliding())
		{
			// Muda a direção do inimigo
			direction = 1;
		}
		if (raycast_right.IsColliding())
		{
			// Muda a direção do inimigo
			direction = -1;
		}
		if (playerdetect.IsColliding())
		{
			is_attacking = true;
		}



		//gravidade
		if (!raycast_Down.IsColliding())
		{
			Position = new Vector2(Position.X, Position.Y + (float)(gravity * delta));
		}

		Position = new Vector2(Position.X + (float)(SPEED * direction * delta), Position.Y);

		// Verifica se o inimigo deve mudar de direção
		if (direction > 0)
		{
			sprite.FlipH = true;
			playerdetect.Scale = new Vector2(-1, 1);
			hitbox.Scale = new Vector2(-1, 1);
		}
		else if (direction < 0)
		{
			sprite.FlipH = false;
			playerdetect.Scale = new Vector2(1, 1);
			hitbox.Scale = new Vector2(1, 1);
		}
		if (is_attacking == true)
		{
			animationTree.Set("parameters/conditions/attacking", true);

		}
		else
		{
			AnimationPlayback.Travel("moving");
		}
	}

	public void _on_animation_finished(string anim_name){
		if (anim_name == "attack")
		{
			is_attacking = false;
			animationTree.Set("parameters/conditions/attacking", false);
		}
		
		if (anim_name == "attack_end")
		{
			animationTree.Set("parameters/conditions/attacking", false);
		}
	}
}

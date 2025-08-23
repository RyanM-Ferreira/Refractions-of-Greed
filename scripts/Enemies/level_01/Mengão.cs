using Godot;
using System;
using System.Numerics;

public partial class Mengão : CharacterBody2D
{
	double health = 300.0;
	private AnimatedSprite2D sprite;
	private AnimationPlayer animationPlayer;
	private AnimationTree animationTree;
	private AnimationNodeStateMachinePlayback animationPlayback;
	private Label label;
	private Label label2;

	private Node2D player;



	private bool inRangedAttackRange = false;
	private bool inMeleeAttackRange = false;
	private bool isAttacking = false;
	private bool canAttack = false;
	private double attackCooldown = 0.5;
	private int direction;
	public double Gravity = 100.0;

	[Export] public string lastAttack;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animationTree = GetNode<AnimationTree>("AnimationTree");
		animationPlayback = animationTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
		label = GetNode<Label>("Label");
		label2 = GetNode<Label>("Label2");
		animationTree.Active = true;
		player = Global.player;
	}




	public override void _Process(double delta)
	{
		if (attackCooldown > 0)
		{
			canAttack = false;
			attackCooldown -= delta;
		}
		else
		{
			canAttack = true;
		}


		checkAttacking();


		label.Text = $"State: " + animationPlayback.GetCurrentNode();
		label2.Text = $"variables: " +
			$"\n inRangedAttackRange: {inRangedAttackRange}" +
			$"\n inMeleeAttackRange: {inMeleeAttackRange}" +
			$"\n isAttacking: {isAttacking}" +
			$"\n canAttack: {canAttack}" +
			$"\n lastAttack: {lastAttack}";


	}

	public override void _PhysicsProcess(double delta)
	{

		if (!IsOnFloor())
		{
			Velocity += new Godot.Vector2(Velocity.X, (float)Gravity * (float)delta);
		}

		int distanceToPlayer = (int)(player.GlobalPosition.X - GlobalPosition.X);
		if (distanceToPlayer < 0)
		{
			direction = -1;
			sprite.FlipH = true;
		}
		if (distanceToPlayer > 0)
		{
			direction = 1;
			sprite.FlipH = false;
		}

			Velocity = new Godot.Vector2((float)(direction * 35 * delta), Velocity.Y);
		





		MoveAndSlide();
		GD.Print(Velocity);
	}




	public void onAnimationEnded(string anim_name)
	{
		lastAttack = anim_name;
	}




	public void checkAttacking()
	{
		if (animationPlayback.GetCurrentNode() != "moving" &&
			animationPlayback.GetCurrentNode() != "death")
		{
			isAttacking = true;
		}
		if (animationPlayback.GetCurrentNode() == "moving" ||
			animationPlayback.GetCurrentNode() == "death")
		{
			isAttacking = false;
		}
	}

	public void resetAttackCooldown()
	{
		attackCooldown = 1;
	}








	private void ranged_area_entered(Node2D body)
	{
		inRangedAttackRange = true;
	}
	private void ranged_area_exited(Node2D body)
	{
		inRangedAttackRange = false;
	}

	private void melee_area_entered(Node2D body)
	{
		inMeleeAttackRange = true;
	}
	private void melee_area_exited(Node2D body)
	{
		inMeleeAttackRange = false;
	}

}

using System;
using Godot;

public partial class Mengão : CharacterBody2D
{
	[Export] double health = 300.0;
	private CharacterBody2D mengao;

	private AnimatedSprite2D sprite;
	private AnimatedSprite2D effect;
	private AnimationPlayer animationPlayer;
	private AnimationTree animationTree;
	private AnimationNodeStateMachinePlayback animationPlayback;
	private Label label;
	private Label label2;
	private Node2D player;
	private CollisionShape2D attackcollision;
	[Export] public double speed = 35;

	[Signal] public delegate void HealthChangedEventHandler();


	private bool inRangedAttackRange = false;
	private bool inMidAttackRange = false;
	private bool inMeleeAttackRange = false;
	private bool isAttacking = false;
	private bool canAttack = false;
	private double attackCooldown = 0.5;
	private int direction = 1;
	public double Gravity = 100.0;

	[Export] public string lastAttack;


	public double immunityTime = 0.5;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		effect = GetNode<AnimatedSprite2D>("effects");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animationTree = GetNode<AnimationTree>("AnimationTree");
		animationPlayback = animationTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
		label = GetNode<Label>("Label");
		label2 = GetNode<Label>("Label2");
		animationTree.Active = true;
		attackcollision = GetNode<CollisionShape2D>("attackHitbox/CollisionShape2D");
		Scale = new Vector2(1, 1);
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
			$"\n inMidAttackRange: {inMidAttackRange}" +
			$"\n inMeleeAttackRange: {inMeleeAttackRange}" +
			$"\n isAttacking: {isAttacking}" +
			$"\n velocity: {Velocity}" +
			$"\n lastAttack: {lastAttack}" +
			$"\n speed: {speed}";


	}

	public override void _PhysicsProcess(double delta)
	{
		
		if (player == null || !GodotObject.IsInstanceValid(player))
		{
			return;
		}
		
		int distanceToPlayer = (int)(player.GlobalPosition.X - GlobalPosition.X);


		if (!IsOnFloor())
			Velocity += new Vector2(Velocity.X, (float)(Gravity * delta));

		int newDirection = direction;
		if (distanceToPlayer > 0)
			newDirection = 1;
		else if (distanceToPlayer < 0)
			newDirection = -1;




		if (!isAttacking)
		{
			if (newDirection != direction)
			{
				direction = newDirection;
				Scale = new Vector2(direction, Scale.Y);
			}
			else if (direction != Scale.Y)
			{
				Scale = new Vector2(Scale.X * -1, Scale.Y);
			}
		}



		if (distanceToPlayer <= 48 && distanceToPlayer >= -48)
		{
			Velocity = new Vector2(0, Velocity.Y);
		}
		else
		{
			Velocity = new Vector2((float)(direction * speed), Velocity.Y);
		}

		immunityTime -= delta;
		MoveAndSlide();



	}

	public void VerticalAdd(double Y)
	{
		Velocity = new Vector2(Velocity.X, Velocity.Y + (float)Y);
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


	private void mid_area_entered(Node2D body)
	{
		inMidAttackRange = true;
	}
	private void mid_area_exited(Node2D body)
	{
		inMidAttackRange = false;
	}

	private void melee_area_entered(Node2D body)
	{
		inMeleeAttackRange = true;
	}
	private void melee_area_exited(Node2D body)
	{
		inMeleeAttackRange = false;
	}



	public void Hurt(double damage)
	{
		if (immunityTime <= 0)
		{
			health -= damage;
			GD.Print($"Jogador {Name} recebeu {damage} de dano, vida restante: {health}");

			EmitSignal(nameof(HealthChanged));
			immunityTime += 0.5;
		}
	}




}

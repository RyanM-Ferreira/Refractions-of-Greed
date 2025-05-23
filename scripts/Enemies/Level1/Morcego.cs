using Godot;
using System;
public partial class Morcego : Enemy
{
	public new double Speed = 50.0;

	private new (int, int) direction = (-1, 1);
	bool is_chassing;

	//import animation elements
	private AnimatedSprite2D sprite;
	private AnimationTree animationTree;
	private AnimationNodeStateMachinePlayback AnimationPlayback;
	//import hitbox
	private Area2D hitbox;


	//import timer
	private Timer timer;


	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("sprite");
		animationTree = GetNode<AnimationTree>("AnimationTree");
		AnimationPlayback = animationTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
		hitbox = GetNode<Area2D>("Hitbox");
		timer = GetNode<Timer>("Timer");
		is_chassing = false;
	}


	private void OnTimerTimeout()
	{
		timer.WaitTime = choose([1, 1.5]);
		if (!is_chassing)
		{
			direction = (choose([-1.0, 1.0, 0.0]), choose([-1.0, 1.0, 0.0]));

		}
	}

	private int choose(double[] options)
	{
		Random random = new Random();
		double index = random.Next(options.Length);
		return (int)options[(int)index];
	}

	public void Move(double delta)
	{
		
		if (!is_chassing)
		{

			Vector2 velocity = new Vector2(
				(float)(Speed *delta* direction.Item1),
				(float)(Speed *delta* direction.Item2)
			);
			Velocity = new Vector2(Velocity.X + velocity.X, Velocity.Y + velocity.Y);
		}
		MoveAndSlide();
	}




	public override void _PhysicsProcess(double delta)
	{
		if (direction.Item1 <= -1)
		{
			sprite.FlipH = false;
		}
		else if (direction.Item1 >= 1)
		{
			sprite.FlipH = true;
		}

		Move(delta);
	}

}

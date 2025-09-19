using Godot;
using System;
public partial class Mrocego : Enemy
{
	public new double Speed = 75.0;
	public new (int, int) direction = (-1, 1);
	bool is_chassing;

	// Import elementos de animação
	private AnimatedSprite2D sprite;
	private AnimationTree animationTree;
	private AnimationNodeStateMachinePlayback AnimationPlayback;

	// Import Timer
	private Timer timer;

	private Player player;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("sprite");
		animationTree = GetNode<AnimationTree>("AnimationTree");
		AnimationPlayback = animationTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
		timer = GetNode<Timer>("Timer");

		player = Global.player;

		is_chassing = false;
	}

	public void Player_Search_Entered(Node2D body)
	{
		if (body is Player)
		{
			is_chassing = true;
		}
	}
	public void Player_Search_Exited(Node2D body)
	{
		if (body is Player)
		{
			is_chassing = false;
		}
	}

	private void OnTimerTimeout()
	{
		timer.WaitTime = choose([1, 0.8]);
		if (!is_chassing)
		{
			direction = ((int)choose([-1.0, 1.0, 0.0]), (int)choose([-1.0, 1.0, 0.0]));
		}
	}

	private double choose(double[] options)
	{
		Random random = new Random();
		double index = random.Next(options.Length);
		return options[(int)index];
	}

	public void Move(double delta)
	{
		if (!is_chassing)
		{
			Vector2 velocity = new Vector2(
				(float)(Speed * delta * direction.Item1),
				(float)(Speed * delta * direction.Item2)
			);
			Velocity = new Vector2(Velocity.X + velocity.X, Velocity.Y + velocity.Y);
		}
		else
		{
			if (player != null)
			{
				Vector2 playerPosition = player.GlobalPosition;
				Vector2 directionToPlayer = (playerPosition - GlobalPosition).Normalized();
				direction = ((int)(MathF.Abs(directionToPlayer.X) / directionToPlayer.X), (int)(MathF.Abs(directionToPlayer.Y) / directionToPlayer.Y));
				Velocity = new Vector2(directionToPlayer.X * (float)Speed, directionToPlayer.Y * (float)Speed);
			}
			else
			{
				GD.PrintErr("Player node not found.");
			}
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

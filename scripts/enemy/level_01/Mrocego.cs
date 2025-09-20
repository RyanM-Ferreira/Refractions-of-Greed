using Godot;
using System;
public partial class Mrocego : Enemy
{
	public  double max_health = 50.0;
	public double health = 50.0;
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


	private Label label;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("sprite");
		animationTree = GetNode<AnimationTree>("AnimationTree");
		AnimationPlayback = animationTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
		label = GetNode<Label>("Label");
		timer = GetNode<Timer>("Timer");

		player = Global.player;

		is_chassing = false;
	}



	private void OnTimerTimeout()
	{
		timer.WaitTime = choose([1, 2]);
		if (!is_chassing)
		{
			direction = (choose([-1, 1, 0]), choose([-1, 1, 0]));
		}
	}

	private int choose(int[] options)
	{
		Random random = new Random();
		int index = random.Next(options.Length);
		return options[index];
	}




	public void Move(double delta)
	{
		if (is_chassing)
		{
			Vector2 directionToPlayer = (player.GlobalPosition - GlobalPosition).Normalized();
			direction = ((int)(MathF.Abs(directionToPlayer.X) / directionToPlayer.X), (int)(MathF.Abs(directionToPlayer.Y) / directionToPlayer.Y));
			Velocity = new Vector2(directionToPlayer.X * (float)Speed, directionToPlayer.Y * (float)Speed);
		}
		else
		{
			Velocity = new Vector2((float)(Speed * direction.Item1), (float)(Speed * direction.Item2));
		}

		MoveAndSlide();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (knockbackTimer > 0)
		{
			Velocity = new Vector2(knockback.X, knockback.Y);
			knockbackTimer -= (float)delta;
			if (knockbackTimer <= 0)
			{
				knockback = Vector2.Zero; // Reseta o knockback quando o tempo acaba
			}
		}




		if (direction.Item1 == -1)
		{
			sprite.FlipH = false;
		}
		else if (direction.Item1 == 1)
		{
			sprite.FlipH = true;
		}


		label.Text = $"variables: " +
		$"\n direction: {direction}" +
		$"\n isChassing: {is_chassing}";
		if (immunityTime > 0)
		{
			immunityTime -= delta;
		}
		Move(delta);
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

	public void Hurt(double damage, Vector2 hitboxLocation, float knockbackForce)
	{
		if (immunityTime <= 0)
		{
			health -= damage;
			GD.Print($"Jogador {Name} recebeu {damage} de dano, vida restante: {health}");
			if (health <= 0)
			{
				GetTree().CreateTimer(0.01).Timeout += () => QueueFree();
				GD.Print("Jogador morreu");
			}
			else
			{
				knockback = (GlobalPosition - hitboxLocation).Normalized() * new Vector2(knockbackForce, 100);
				knockbackTimer = 0.15f;
				EmitSignal(nameof(HealthChanged));
				immunityTime += 0.5;
			}
		}
	}
}

using Godot;

public partial class PurpleRollercoaster : Enemy
{
	// Import Animation elements
	private AnimatedSprite2D Sprite;
	private AnimationTree animationTree;
	private AnimationNodeStateMachinePlayback AnimationPlayback;

	// Import hitbox
	private Area2D hitbox;

	// Import RayCast2D
	private RayCast2D raycast_Left;
	private RayCast2D raycast_Right;
	private RayCast2D raycast_DownLeft;
	private RayCast2D raycast_DownRight;

	// Enemy properties
	public new double Speed = 40.0;
	public new double Gravity = 100;
	public new double direction = -1.0;
	public new double health = 20.0;

	public override void _Ready()
	{
		raycast_Right = GetNode<RayCast2D>("RayCast_Right");
		raycast_Left = GetNode<RayCast2D>("RayCast_Left");
		raycast_DownLeft = GetNode<RayCast2D>("RayCast_DownLeft");
		raycast_DownRight = GetNode<RayCast2D>("RayCast_DownRight");
		animationTree = GetNode<AnimationTree>("AnimationTree");
		hitbox = GetNode<Area2D>("Hitbox");
		Sprite = GetNode<AnimatedSprite2D>("sprite");
		AnimationPlayback = animationTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
	}

	public override void _Process(double delta)
	{
		// Muda a direção do inimigo de acordo com o RayCast
		if (raycast_Left.IsColliding())
		{
			direction = 1;
		}
		if (raycast_Right.IsColliding())
		{
			direction = -1;
		}
		if (!raycast_DownLeft.IsColliding() && raycast_DownRight.IsColliding() && IsOnFloor())
		{
			direction = -1;
		}
		if (!raycast_DownRight.IsColliding() && raycast_DownLeft.IsColliding() &&  IsOnFloor())
		{
			direction = 1;
		}

		// Verifica se o inimigo deve mudar de direção
		if (direction > 0)
		{
			hitbox.Scale = new Vector2(-1, 1);
			Sprite.FlipH = true;
		}
		else if (direction < 0)
		{
			hitbox.Scale = new Vector2(1, 1);
			Sprite.FlipH = false;
		}

		GroundEnemy(delta, Gravity, direction, Speed);
	}
}

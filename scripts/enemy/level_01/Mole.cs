using Godot;

public partial class Mole : Enemy
{
	// Import AnimatedSprite2D
	private AnimatedSprite2D sprite;
	private AnimationTree animationTree;
	private AnimationNodeStateMachinePlayback AnimationPlayback;

	// Import Hitbox
	private Area2D hitbox;

	// Import RayCast2D
	private RayCast2D raycast_Left;
	private RayCast2D raycast_Right;
	private RayCast2D playerdetect;

	[Export] public bool is_attacking;

	[Export] public new double Speed = 40.0;
	public new double direction = -1.0;
	public new double Gravity = 140;

	private RayCast2D raycast_DownLeft;
	private RayCast2D raycast_DownRight;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("sprite");
		raycast_Right = GetNode<RayCast2D>("RayCast_Right");
		raycast_Left = GetNode<RayCast2D>("RayCast_Left");
		playerdetect = GetNode<RayCast2D>("Player_detection");
		animationTree = GetNode<AnimationTree>("AnimationTree");
		raycast_DownLeft = GetNode<RayCast2D>("RayCast_DownLeft");
		raycast_DownRight = GetNode<RayCast2D>("RayCast_DownRight");

		hitbox = GetNode<Area2D>("Hitbox");

		AnimationPlayback = animationTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
	}

	public void _on_animation_finished(string anim_name)
	{
		if (anim_name == "attack")
		{
			is_attacking = false;
			animationTree.Set("parameters/conditions/attacking", false);
		}
		if (anim_name == "attack_end")
		{
			Velocity = new Vector2((float)(direction * 60), 0);
		}
	}

	public override void _Process(double delta)
	{
		// Raycast 
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
			direction = -1;
		}
		if (!raycast_DownRight.IsColliding() && raycast_DownLeft.IsColliding())
		{
			direction = 1;
		}
		if (playerdetect.IsColliding())
		{
			is_attacking = true;
			animationTree.Set("parameters/conditions/attacking", true);
		}
		else
		{
			AnimationPlayback.Travel("moving");
		}

		// Verifica direção e ajusta o sprite e hitbox
		if (direction > 0)
		{
			playerdetect.Scale = new Vector2(-1, 1);
			hitbox.Scale = new Vector2(-1, 1);
			sprite.FlipH = true;
		}
		else if (direction < 0)
		{
			playerdetect.Scale = new Vector2(1, 1);
			hitbox.Scale = new Vector2(1, 1);
			sprite.FlipH = false;
		}

		GroundEnemy(delta, Gravity, direction, Speed);
	}
}
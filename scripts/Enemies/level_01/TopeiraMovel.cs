using Godot;

public partial class TopeiraMovel : Enemy
{   //import AnimatedSprite2D
	private AnimatedSprite2D sprite;
	private AnimationTree animationTree;
	private AnimationNodeStateMachinePlayback AnimationPlayback;

	//import hitbox
	private Area2D hitbox;

	//import RayCast2D
	private RayCast2D raycast_Left;
	private RayCast2D raycast_Right;
	private RayCast2D playerdetect;

	[Export] public bool is_attacking;

	[Export] public double Speed = 40.0;
	public double direction = -1.0;
	public double Gravity = 140;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("sprite");
		raycast_Right = GetNode<RayCast2D>("RayCast_Right");
		raycast_Left = GetNode<RayCast2D>("RayCast_Left");
		playerdetect = GetNode<RayCast2D>("Player_detection");
		animationTree = GetNode<AnimationTree>("AnimationTree");
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
		if (playerdetect.IsColliding())
		{
			is_attacking = true;
			animationTree.Set("parameters/conditions/attacking", true);
		}
		else
		{
			AnimationPlayback.Travel("moving");
		}

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

		GroundEnemy(delta, Gravity, direction, Speed);
	}

}
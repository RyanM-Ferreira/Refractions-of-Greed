using Godot;

public partial class splashScreen : Node2D
{
	public override void _Ready()
	{
		Timer timer = GetNode<Timer>("timer");
		timer.Timeout += OnTimeout;
		timer.Start();
	}

	private void OnTimeout()
	{
		GetTree().ChangeSceneToFile("res://scenes/main/main.tscn");
	}

	public override void _Process(double delta)
	{
	}
}

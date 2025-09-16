using Godot;

public partial class SplashScreen : Node2D
{
	public override void _Ready()
	{
		Timer timer = GetNode<Timer>("timer");
		timer.Timeout += OnTimeout;
		timer.Start();

		ConfigManager.LoadConfig();
	}

	private void OnTimeout()
	{
		GoToMainScreen();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("menu"))
		{
			GoToMainScreen();
		}
	}

	public void GoToMainScreen()
	{
		GetTree().ChangeSceneToFile("res://scenes/main/main.tscn");
	}
}

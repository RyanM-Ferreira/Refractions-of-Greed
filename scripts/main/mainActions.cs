using Godot;

public partial class mainActions : Button
{
	public void QuitButtonPressed()
	{
		GetTree().Quit();
	}
	public void StartButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes//Levels/cave/Level_1.tscn");
	}

	public void OptionsMenuButtonPressed()
	{
		// Provavelmente
	}

	public void DebugButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes//debug/debug_level.tscn");
	}

	private Vector2 originalPosition;

	public override void _Ready()
	{
		originalPosition = Position;
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}

	private void OnMouseEntered()
	{
		Scale = new Vector2(1.125f, 1.125f);

		Position = originalPosition - ((Scale - new Vector2(1,1)) * Size) / 2;
	}

	private void OnMouseExited()
	{
		Scale = new Vector2(1,1);
		Position = originalPosition;
	}
}

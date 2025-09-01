using Godot;

// ? Devo usar List<T> ou Array?
public partial class MainActions : Button
{
	int selectedOption = -1;
	int maxOptions = 2;

	private Vector2 originalScale;
	private Vector2 scaled;
	private Vector2 originalPosition;
	private Vector2 scaledPosition;

	private Button[] buttons;
	private Vector2[] originalPositions;

	public override void _Ready()
	{
		originalScale = Scale;
		originalPosition = Position;

		buttons = new Button[]
		{
			GetParent().GetNode<Button>("StartButton"),
			GetParent().GetNode<Button>("OptionsButton"),
			GetParent().GetNode<Button>("QuitButton"),
		};

		originalPositions = new Vector2[buttons.Length];
		for (int i = 0; i < buttons.Length; i++)
		{
			originalPositions[i] = buttons[i].Position;
		}

		scaled = new Vector2(1.125f, 1.125f);
		scaledPosition = originalPosition - (scaled - originalScale) * Size / 2;

		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}

	public void StartButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/levels/cave/level_01.tscn");
	}

	public void OptionsMenuButtonPressed()
	{
		GD.PrintErr("Não implementado");
	}

	public void QuitButtonPressed() => GetTree().Quit();

	public void DebugButtonPressed() => GetTree().ChangeSceneToFile("res://scenes/debug/debugLevel.tscn");

	private void OnMouseEntered()
	{
		ResetAppearance();
		selectedOption = -1;
		Scale = scaled;
		Position = scaledPosition;
	}

	private void OnMouseExited()
	{
		Scale = originalScale;
		Position = originalPosition;
	}

	private void ResetAppearance()
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].Scale = originalScale;
			buttons[i].Position = originalPositions[i];
		}
	}

	private void InputAnimation()
	{
		ResetAppearance();

		var button = buttons[selectedOption];
		button.Scale = scaled;
		button.Position = originalPositions[selectedOption] - (scaled - originalScale) * button.Size / 2;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (Input.IsActionJustPressed("uiUp") && selectedOption > 0)
		{
			selectedOption--;
			InputAnimation();
		}
		else if (Input.IsActionJustPressed("uiDown") && selectedOption < maxOptions)
		{
			selectedOption++;
			InputAnimation();
		}

		if (Input.IsActionJustPressed("start"))
		{
			switch (selectedOption)
			{
				case 0: StartButtonPressed(); break;
				case 1: OptionsMenuButtonPressed(); break;
				case 2: QuitButtonPressed(); break;
			}
		}
	}
}

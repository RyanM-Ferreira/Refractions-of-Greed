using Godot;

// ! Esse código está uma porcaria!
// ? Devo usar List<T> ou um simples Array?

public partial class MainActions : Button
{
	int selectedOption = -1;
	int maxOptions = 2;

	private Button startButton;
	private Button optionsButton;
	private Button quitButton;

	private Vector2 originalScale;
	private Vector2 scaled;
	private Vector2 originalPosition;
	private Vector2 scaledPosition;

	private Vector2 startOriginalPos;
	private Vector2 optionsOriginalPos;
	private Vector2 quitOriginalPos;

	public override void _Ready()
	{
		originalScale = Scale;
		originalPosition = Position;

		startButton = GetParent().GetNode<Button>("StartButton");
		optionsButton = GetParent().GetNode<Button>("OptionsButton");
		quitButton = GetParent().GetNode<Button>("QuitButton");

		// TODO: Horroroso, depois eu refaço isso tudo usando List<T>, 
		startOriginalPos = startButton.Position;
		optionsOriginalPos = optionsButton.Position;
		quitOriginalPos = quitButton.Position;

		scaled = new Vector2(1.125f, 1.125f);
		scaledPosition = originalPosition - ((scaled - originalScale) * Size) / 2;

		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}


	public override void _Process(double delta)
	{
		InputHandle();
	}

	public void QuitButtonPressed()
	{
		GetTree().Quit();
	}
	public void StartButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/Levels/cave/level_01.tscn");
	}

	public void OptionsMenuButtonPressed()
	{
		// TODO: Complicado isso aí...
		GD.PrintErr("Não implementado");
	}

	public void DebugButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/debug/debugLevel.tscn");
	}

	private void OnMouseEntered()
	{
		Reset();

		selectedOption = -1;

		Scale = scaled;
		Position = scaledPosition;
	}

	private void OnMouseExited()
	{
		Scale = originalScale;
		Position = originalPosition;
	}

	private void Reset()
	{
		startButton.Scale = originalScale = optionsButton.Scale = quitButton.Scale = originalScale;

		startButton.Position = startOriginalPos;
		optionsButton.Position = optionsOriginalPos;
		quitButton.Position = quitOriginalPos;
	}

	private void AnimHandle()
	{
		Reset();

		switch (selectedOption)
		{
			case 0:
				startButton.Scale = scaled;
				startButton.Position = startOriginalPos - ((scaled - originalScale) * startButton.Size) / 2;
				break;

			case 1:
				optionsButton.Scale = scaled;
				optionsButton.Position = optionsOriginalPos - ((scaled - originalScale) * optionsButton.Size) / 2;
				break;

			case 2:
				quitButton.Scale = scaled;
				quitButton.Position = quitOriginalPos - ((scaled - originalScale) * quitButton.Size) / 2;
				break;
			default:
				break;
		}
	}

	private void InputHandle()
	{
		if (Input.IsActionJustPressed("uiUp") && selectedOption > 0)
		{
			selectedOption--;
			AnimHandle();
		}
		else if (Input.IsActionJustPressed("uiDown") && selectedOption < maxOptions)
		{
			selectedOption++;
			AnimHandle();
		}

		if (Input.IsActionJustPressed("start"))
		{
			switch (selectedOption)
			{
				case 0:
					StartButtonPressed();
					break;
				case 1:
					OptionsMenuButtonPressed();
					break;
				case 2:
					QuitButtonPressed();
					break;
				default:
					break;
			}
		}
	}
}
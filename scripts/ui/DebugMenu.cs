using System.Threading.Tasks;
using Godot;

public partial class DebugMenu : Label
{
	private Player player;

	private double currentFPS = 0;
	private double minFPS = 0;
	private double maxFPS = 0;

	bool _isDebugVisible = true;
	bool _isCollisionVisible = false;

	public override void _Ready()
	{
		player = GetParent().GetParent<Player>();
	}

	public override void _Process(double delta)
	{
		UpdateDebugDisplay();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (Input.IsActionJustPressed("F1"))
		{
			_isCollisionVisible = !_isCollisionVisible;
			OnCollisionVisibilityChanged();
		}

		if (Input.IsActionJustPressed("F3"))
		{
			_isDebugVisible = !_isDebugVisible;
		}
	}

	private void OnCollisionVisibilityChanged()
	{
		GetTree().DebugCollisionsHint = _isCollisionVisible;
		GD.Print("Colisões: " + (_isCollisionVisible ? "Ativadas" : "Desativadas"));
		GetTree().ReloadCurrentScene();
	}

	private void UpdateDebugDisplay()
	{
		if (!_isDebugVisible)
		{
			Text = null;
			return;
		}

		currentFPS = Engine.GetFramesPerSecond();

		if (currentFPS < minFPS || minFPS == 0)
		{
			minFPS = currentFPS;
		}
		else if (currentFPS > maxFPS)
		{
			maxFPS = currentFPS;
		}

		if (player != null)
		{
			Text = $"FPS: {currentFPS}\nMinFPS: {minFPS}\nMaxFPS: {maxFPS}" +
				$"\n\nHealth: {player.health}/{player.maxHealth}\nIsDashing: {player.isDashing}\nIsInsideEnemy: {player.isInsideEnemy}\nImmunityTime: {player.immunityTime:F2}. {(player.immunityTime > 0 ? "Immune" : "Not Immune")}\nCollisionView: {_isCollisionVisible}\nIsAttacking: {player.isAttacking}" +
				$"\n\nPlayerPosition: {player.GlobalPosition:F2}\nPlayerVelocity: {player.Velocity:F2}." +
				$"\n\nCameraZoom: {player.camera.Zoom:F2}" +
				$"\n\nAttackCooldown: {player.attackTimer:F2}. {(player.attackTimer > 0 ? "Reloading." : "Available.")}\nDashCooldown: {player.dashTimer:F2}. {(player.dashTimer > 0 ? "Reloading." : "Available.")}\nComboStep: {player.comboStep}";
		}
		else
		{
			Text = "ERROR: Look at the console for more information.";
			GD.PrintErr("Sei lá, player não encontrado. Faz o L!");
		}
	}
}

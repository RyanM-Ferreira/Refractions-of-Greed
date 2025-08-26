using Godot;

public partial class DebugMenu : Label
{
    private Player player;

    private double minFPS = 0;
    private double maxFPS = 0;

    bool isVisible = true;
    bool viewCollision = false;

    public override void _Ready()
    {
        player = GetParent().GetParent<Player>();
    }

    public override void _Process(double delta)
    {
        HandleInput();
        UpdateDebugDisplay();
    }

    private void HandleInput()
    {
        HandleCollisionToggle();
        HandleDebugMenuToggle();
    }

    private void HandleCollisionToggle()
    {
        if (Input.IsActionJustPressed("toggleDebugCollision"))
        {
            viewCollision = !viewCollision;

        }

        if (viewCollision)
        {
            GetTree().DebugCollisionsHint = true;
            GD.Print("Collision Shapes are visible!");

            GetTree().ReloadCurrentScene();
            GD.Print("Reloaded Scene!");

        }
        else
        {
            GetTree().DebugCollisionsHint = false;
        }
    }

    private void HandleDebugMenuToggle()
    {
        if (Input.IsActionJustPressed("F3"))
        {
            isVisible = !isVisible;
            GD.Print("Debug Menu is Active!");
        }
    }

    private void UpdateDebugDisplay()
    {
        if (!isVisible)
        {
            Text = "";
            return;
        }

        double currentFPS = Engine.GetFramesPerSecond();

        if (currentFPS < minFPS || minFPS == 0)
        {
            minFPS = currentFPS;
        }

        if (currentFPS > maxFPS)
        {
            maxFPS = currentFPS;
        }

        if (player != null)
        {
            Text = $"FPS: {currentFPS}\nMinFPS: {minFPS}\nMaxFPS: {maxFPS}" +
                $"\n\nHealth: {player.health}/{player.maxHealth}\nIsDashing: {player.isDashing}\nIsInsideEnemy: {player.isInsideEnemy}\nImmunityTime: {player.immunityTime:F2}. {(player.immunityTime > 0 ? "Immune" : "Not Immune")}\nCollisionView: {viewCollision}\nIsAttacking: {player.isAttacking}" +
                $"\n\nPlayerPosition: {player.GlobalPosition:F2}\nPlayerVelocity: {player.Velocity:F2}.\nDashCooldown: {player.dashTimer:F2}. {(player.dashTimer > 0 ? "Reloading." : "Available.")}" +
                $"\n\nCameraZoom: {player.camera.Zoom:F2}";
        }
        else
        {
            Text = "ERROR: Look at the console for more information.";
            GD.PrintErr("Sei lá, player não encontrado. Faz o L!");
        }
    }
}
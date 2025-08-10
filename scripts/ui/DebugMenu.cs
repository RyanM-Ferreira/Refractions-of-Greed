using Godot;

public partial class DebugMenu : Label
{
    private Player player;

    private double minFPS = 0;
    private double maxFPS = 0;

    bool isVisible = true;

    public override void _Ready()
    {
        player = GetParent().GetParent<Player>();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("F3"))
        {
            GD.Print("Debug Menu Ativo!");
            isVisible = !isVisible;
        }

        if (isVisible)
        {
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
                $"\n\nHealth: {player.health}/{player.maxHealth}\nisDashing: {player.isDashing}\nisInsideEnemy: {player.isInsideEnemy}\nImmunityTime: {player.immunityTime.ToString("F2")}. {(player.immunityTime > 0 ? "Immune" : "Not Immune")}" +
                $"\n\nPlayerPosition: {player.GlobalPosition.ToString("F2")}\nPlayerVelocity: {player.Velocity.ToString("F2")}.\nDashCooldown: {player.dashTimer.ToString("F2")}. {(player.dashTimer > 0 ? "Reloading." : "Available.")}";
            }
            else
            {
                Text = "Sei lá, player não encontrado. Faz o L!";
            }
        }
        else
        {
            Text = "";
        }
    }
}
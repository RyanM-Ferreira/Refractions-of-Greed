using Godot;
using System;
using System.Dynamic;

public partial class Global : Node2D
{
    // Variáveis globais do jogo
    public static double PlayerMaxHealth = 50.0;
    public static Player player;



    public override void _Ready()
    {
        player = GetTree().Root.FindChild("Player", true, false) as Player;
        player.PlayerDied += OnPlayerDied;
    }
    private void OnPlayerDied()
    {
        GetTree().CreateTimer(0.2).Timeout += () => GetTree().ReloadCurrentScene();
    }
}

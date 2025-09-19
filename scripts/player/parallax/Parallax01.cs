using Godot;
using System;

public partial class Parallax01 : Node2D
{
    private Player player;

    public override void _Ready()
    {
        // ajuste o caminho até o Player conforme sua cena
        player = GetParent().GetNode<Player>("Player");
    }

    public override void _Process(double delta)
    {
        if (player != null) // só pra garantir
        {
            GlobalPosition = new Vector2(GlobalPosition.X, player.GlobalPosition.Y);
        }
    }
}

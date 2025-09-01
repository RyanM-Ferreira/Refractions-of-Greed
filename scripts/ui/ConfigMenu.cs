using Godot;
using System;

public partial class ConfigMenu : CanvasLayer
{
    Button vsync;
    
    public override void _Ready()
    {
        CloseConfigMenu();
    }

    public void OpenConfigMenu()
    {
        Visible = true;
    }

    public void CloseConfigMenu()
    {
        Visible = false;
    }
}

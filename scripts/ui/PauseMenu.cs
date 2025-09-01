using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
    private bool _isMenuActive = false;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        Visible = false;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("menu"))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        _isMenuActive = !_isMenuActive;

        if (_isMenuActive)
        {
            Pause();
        }
        else
        {
            Unpause();
        }
    }

    private void Unpause()
    {
        Visible = false;
        GetTree().Paused = false;
    }

    private void Pause()
    {
        Visible = true;
        GetTree().Paused = true;
    }

    public void ResumeButton()
    {
        _isMenuActive = false;
        Visible = false;
        Unpause();
    }

    public void SaveButton()
    {
        GD.PrintErr("Não implementado ainda...");
    }

    public void BackButton()
    {
        Unpause();
        GetTree().ChangeSceneToFile("res://scenes/main/main.tscn");
    }
}

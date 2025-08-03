using Godot;
using System;

public partial class FpsCounter : Label
{
    private double min = 0;
    private double max = 0;

    public override void _Process(double delta)
    {
        double currentFPS = Engine.GetFramesPerSecond();

        if (currentFPS < min || min == 0)
        {
            min = currentFPS;
        }

        if (currentFPS > max)
        {
            max = currentFPS;
        }

        Text = $"FPS: {currentFPS}\nMin: {min}\nMax: {max}";
    }
}
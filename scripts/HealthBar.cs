using Godot;
using System;

public partial class HealthBar : ProgressBar
{
    Node parent;
    public override void _Ready()
    {

        parent = GetOwner();
        if (parent == null)
        {
            GD.PrintErr("Parent node is null. Cannot update health bar.");
            return;
        }


        if (parent.HasSignal("HealthChanged"))
        {
            parent.Connect("HealthChanged", new Callable(this, nameof(UpdateHealthBar)));
            UpdateHealthBar();
        }
        GD.Print($"Health bar ready with parent: {parent.Name}");
    }

    public void UpdateHealthBar()
    {
        MaxValue = (double)parent.Get("maxHealth");
        Value = (double)parent.Get("health");
        GD.Print($"Health bar updated: MaxValue = {MaxValue}, Value = {Value}");
    }
}

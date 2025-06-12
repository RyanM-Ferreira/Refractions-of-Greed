using Godot;
using System;

public partial class HealthBar : ProgressBar
{
    Node parent;
    Double health;
    Double maxHealth;
    public override void _Ready()
    {
        
        parent = GetParent();
        if (parent == null)
        {
            GD.PrintErr("Parent node is null. Cannot update health bar.");
            return;
        }
        health = (double)parent.Get("health");
        maxHealth = (double)parent.Get("max_health");


        GD.Print($"HealthBar ready for {parent.Name}. Current health: {health}, Max health: {maxHealth}");
        if (parent.HasSignal("HealthChanged"))
        {
            GD.Print($"Connecting to HealthChanged signal on {parent.Name}.");
            parent.Connect("HealthChanged", new Callable(this, nameof(UpdateHealthBar)));
            UpdateHealthBar();
        }
    }

    public void UpdateHealthBar()
    {
        maxHealth = (double)parent.Get("max_health");
        health = (double)parent.Get("health");
        GD.Print($"Updating health bar for {parent.Name}. Current health: {health}, Max health: {maxHealth}");
        MaxValue = maxHealth;
        Value = health;

        if ((double)parent.Get("health") <= 0)
        {
            GD.Print($"{parent.Name} is dead.");
            if (parent.HasMethod("Die"))
            {
                parent.Call("Die");
            }
            else
            {
                GD.PrintErr($"{parent.Name} does not have a Die method.");
            }
            return;
        }
        else if ((double)parent.Get("health") > 0)
        {
            health = (double)parent.Get("health");
            Value = health;
            return;
        }
    }
}

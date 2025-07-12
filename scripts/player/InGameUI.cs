using Godot;
using System;

public partial class InGameUI : CanvasLayer
{
	private Label hudLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		hudLabel = GetNode<Label>("hudLabel");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void RefreshLife(double vida)
	{
		hudLabel.Text = $"Vida: {vida}";
	}
}
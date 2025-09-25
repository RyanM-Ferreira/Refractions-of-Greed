using Godot;
using System;

public partial class Level01 : Node2D
{
	private Transition transition;

	public override void _Ready()
	{
		transition = GetNode<Transition>("Transition");
		transition.PlayFadeIn();
	}

	public override void _Process(double delta)
	{
	}
}

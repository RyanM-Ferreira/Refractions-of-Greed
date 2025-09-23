using Godot;
using System;

public partial class Intro : Node2D
{
	AnimationPlayer animation;
	Node2D confirm;
	[Export] public PackedScene Scene;

	int count = 0;

	public override void _Ready()
	{
		animation = GetNode<AnimationPlayer>("AnimationPlayer");
		confirm = GetNode<Node2D>("Confirm");
		confirm.Visible = false;

		animation.Play("Intro");
	}

	public override void _Process(double delta)
	{
		if (count >= 2)
		{
			ChangeScene();
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (Input.IsActionJustPressed("menu"))
		{
			confirm.Visible = true;
			count++;
		}
	}


	public void OnAnimationFinished(StringName AnimName)
	{
		if (AnimName == "Intro")
		{
			animation.Play("Scene");
		}
		else if (AnimName == "Scene")
		{
			animation.Play("Outro");
		}
		else if (AnimName == "Outro")
		{
			ChangeScene();
		}
	}

	public void ChangeScene()
	{
		GetTree().ChangeSceneToPacked(Scene);
	}
}

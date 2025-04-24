using Godot;
using System;

//odeio essa merda, não consigo fazer o timer funcionar

public partial class Killzone : Area2D
{
	private Timer _TimerNode;
	public override void _Ready(){
		_TimerNode = GetNode<Timer>("Timer");
	}

	public void _on_body_entered(Node2D body)
	{
		GD.Print("you died!");
		Engine.TimeScale = 0.5f;
		_TimerNode.Start();
	}

	public void _on_Timer_timeout()
	{
		GD.Print("Reloading scene...");
		GetTree().ReloadCurrentScene();
	}
	
}

using Godot;
using System;

public partial class mainActions : Button
{
	public void QuitButtonPressed()
	{
		GetTree().Quit();
	}
	public void StartButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes//Levels/cave/Level_1.tscn");
	}
	
	public void DebugButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes//debug/debug_level.tscn");
	}
}

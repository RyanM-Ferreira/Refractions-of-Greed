using Godot;
using System;

public partial class UIIndicator : Node2D
{
	public Label label;
	private Player player;
	
	public override void _Ready(){
		label = GetNode<Label>("Quantity");
		player = GetParent().GetParent<Player>();
	}
	
	public override void _Process(double Delta){
		if (this.Name == "Money"){
			label.Text = $"{player.wallet}$";
		}
		else if (this.Name == "Refill"){
			label.Text = $"{player.lifeRefill}";
		}
	}
}

using Godot;
using System;

public partial class Chest01 : Area2D
{
	[Export] public PackedScene ItemScene;
	[Export] public int ItemCount = 1;
	[Export] public int collectibleIndex = 1;

	bool _isPlayerInside = false;
	bool _isChestOpened = false;

	AnimatedSprite2D sprite;
	Node2D popUp;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		popUp = GetNode<Node2D>("PopUp");
		sprite.Play("default");
	}

	private void TogglePlayerInside(Node2D body, bool inside)
	{
		if (body is Player && !_isChestOpened)
		{
			popUp.Visible = inside;
			_isPlayerInside = inside;
		}
	}

	public void OnBodyEntered(Node2D body) => TogglePlayerInside(body, true);
	public void OnBodyExited(Node2D body) => TogglePlayerInside(body, false);

	public override void _Process(double delta)
	{
		if (_isPlayerInside && Input.IsActionPressed("interact")) { _isChestOpened = true; }

		if (_isChestOpened && sprite.Animation != "open") { sprite.Play("open"); }
	}

	private void OnAnimationFinished()
	{
		if (sprite.Animation == "open") { OpenChest(); popUp.Visible = false; }
	}

	public void OpenChest()
	{
		for (int i = 0; i < ItemCount; i++)
		{
			var itemInstance = ItemScene.Instantiate();

			if (itemInstance is Collectibles c)
			{ c.index = collectibleIndex; }

			AddChild(itemInstance);

			GD.Print($"Chest spawned an item! ({itemInstance.Name})");
		}
	}
}

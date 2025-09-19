using Godot;
using System;

public partial class Collectibles : Area2D
{
	[Export] public int index { get; set; } = 0;
	[Export] public int value { get; set; } = 1;

	AnimatedSprite2D animatedSprite;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;

		if (IsInGroup("Gem"))
		{
			animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

			switch (index)
			{
				case 1: animatedSprite.Play("green"); value = 5; break;
				case 2: animatedSprite.Play("pink"); value = 10; break;
				case 3: animatedSprite.Play("purple"); value = 20; break;
				case 4: animatedSprite.Play("red"); value = 50; break;
				case 5: animatedSprite.Play("white"); value = 100; break;
				case 6: animatedSprite.Play("yellow"); value = 200; break;
				default: animatedSprite.Play("cyan"); value = 1; break;
			}
		}
	}

	private void OnBodyEntered(Node body)
	{
		if (body is Player player)
		{
			if (IsInGroup("Life"))
			{
				player.CollectItens(value, "lifeRefill");
			}
			else if (IsInGroup("Energy"))
			{
				player.CollectItens(value, "energyRefill");
			}
			else if (IsInGroup("Coin") || IsInGroup("Gem"))
			{
				player.CollectItens(value, "money");
			}

			QueueFree();
		}
	}
}
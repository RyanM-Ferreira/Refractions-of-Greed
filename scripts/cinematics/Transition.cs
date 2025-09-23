using Godot;

public partial class Transition : CanvasLayer
{
	ColorRect color;
	AnimationPlayer animation;

	[Export] public bool IsFadeIn = true;
	public bool IsFinished { get; private set; } = false;

	public override void _Ready()
	{
		color = GetNode<ColorRect>("ColorRect");
		animation = GetNode<AnimationPlayer>("AnimationPlayer");

		animation.AnimationFinished += OnAnimationFinished;
	}

	public void PlayFadeIn()
	{
		Visible = true;
		IsFinished = false;
		animation.Play("FadeIn");
	}

	public void PlayFadeOut()
	{
		Visible = true;
		IsFinished = false;
		animation.Play("FadeOut");
	}

	private void OnAnimationFinished(StringName animName)
	{
		if (animName == "FadeIn" || animName == "FadeOut")
		{
			Visible = false;
			IsFinished = true;
		}
	}
}

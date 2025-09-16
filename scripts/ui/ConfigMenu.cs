using Godot;
using System;
using System.Threading;

public partial class ConfigMenu : Window
{
	CheckButton vsync;
	CheckBox fullscreen;
	Window confirmationDialog;

	public override void _Ready()
	{
		confirmationDialog = GetNode<AcceptDialog>("ConfirmationDialog");
		vsync = GetNode<CheckButton>("Panel/ScrollContainer/VBoxContainer/VSyncContainer/VSyncCheck");
		fullscreen = GetNode<CheckBox>("Panel/ScrollContainer/VBoxContainer/FullscreenContainer/FullscreenCheck");

		SaveAndInitialize();
	}

	public void SaveAndInitialize()
	{
		var config = ConfigManager.LoadConfig();
		vsync.ButtonPressed = config.vsync;
		fullscreen.ButtonPressed = config.fullscreen;
	}

	public void OnOpenRequest()
	{
		Visible = true;
	}
	public void OnCloseRequest()
	{
		confirmationDialog.Visible = true;
	}

	public void ConfirmationOnCancel()
	{
		Visible = false;
	}

	public void ConfirmationOnConfirm()
	{
		bool isVSyncEnabled = vsync.ButtonPressed;
		bool isFullscreenEnabled = fullscreen.ButtonPressed;
		ConfigManager.SaveConfig(isFullscreenEnabled, isVSyncEnabled);

		SaveAndInitialize();
		HideAll();
	}

	public void HideAll()
	{
		Visible = false;
		confirmationDialog.Visible = false;
	}
}
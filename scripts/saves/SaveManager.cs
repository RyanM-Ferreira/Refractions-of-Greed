using Godot;
using System;

public static class ConfigManager
{
    const string SAVEPATH = "user://config.cfg";

    public static void SaveConfig(bool fullscreen, bool vsync)
    {
        var config = new ConfigFile();
        config.SetValue("graphics", "fullscreen", fullscreen);
        config.SetValue("graphics", "vsync", vsync);
        config.Save(SAVEPATH);
    }

    public static (bool fullscreen, bool vsync) LoadConfig()
    {
        var config = new ConfigFile();

        if (config.Load(SAVEPATH) != Error.Ok)
        {
            GD.Print("No config found, using defaults.");
            return (false, false);
        }

        bool fullscreen = (bool)config.GetValue("graphics", "fullscreen", false);
        bool vsync = (bool)config.GetValue("graphics", "vsync", false);

        UpdateContent(fullscreen, vsync);
        
        return (fullscreen, vsync);
    }

    public static void UpdateContent(bool fullscreen, bool vsync)
    {
        DisplayServer.WindowSetMode(fullscreen ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Maximized);
        DisplayServer.WindowSetVsyncMode(vsync ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled);
    }
}

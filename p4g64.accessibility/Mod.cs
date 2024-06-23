﻿using DavyKager;
using p4g64.accessibility.Components;
using p4g64.accessibility.Configuration;
using p4g64.accessibility.Native;
using p4g64.accessibility.Native.Text;
using p4g64.accessibility.Template;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Memory.Streams;
using Reloaded.Mod.Interfaces;
using static p4g64.accessibility.Utils;

namespace p4g64.accessibility;
/// <summary>
/// Your mod logic goes here.
/// </summary>
public class Mod : ModBase // <= Do not Remove.
{
    /// <summary>
    /// Provides access to the mod loader API.
    /// </summary>
    private readonly IModLoader _modLoader;

    /// <summary>
    /// Provides access to the Reloaded.Hooks API.
    /// </summary>
    /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
    private readonly IReloadedHooks? _hooks;

    /// <summary>
    /// Provides access to the Reloaded logger.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Entry point into the mod, instance that created this class.
    /// </summary>
    private readonly IMod _owner;

    /// <summary>
    /// Provides access to this mod's configuration.
    /// </summary>
    private Config _configuration;

    /// <summary>
    /// The configuration of the currently executing mod.
    /// </summary>
    private readonly IModConfig _modConfig;

    private Dialogue _dialogue;
    private TitleBar _titleBar;
    private VibrationNavigation _vibrationNav;

    public Mod(ModContext context)
    {
        _modLoader = context.ModLoader;
        _hooks = context.Hooks;
        _logger = context.Logger;
        _owner = context.Owner;
        _configuration = context.Configuration;
        _modConfig = context.ModConfig;
        

        Initialise(_logger, _configuration, _modLoader);
        AtlusEncoding.Initiailse(_modLoader.GetDirectoryForModId(_modConfig.ModId));
        var modDir = _modLoader.GetDirectoryForModId(_modConfig.ModId);


        _vibrationNav = new VibrationNavigation(_logger, _hooks);

        // Add the mod's folder to the path so tolk will load screen reader dlls
        Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + modDir, EnvironmentVariableTarget.Process);

        Log("Loading tolk");
        Tolk.Load();



        if (!Tolk.IsLoaded())
        {
            LogError("Tolk failed to load, your mod files may be corrupted!");
            return;
        }
        _dialogue = new Dialogue(_hooks!);
        _titleBar = new TitleBar(_hooks!);
        
    }

    #region Standard Overrides
    public override void ConfigurationUpdated(Config configuration)
    {
        // Apply settings from configuration.
        // ... your code here.
        _configuration = configuration;
        _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
    }
    #endregion

    #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Mod() { }
#pragma warning restore CS8618
    #endregion
}
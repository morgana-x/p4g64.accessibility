﻿using p4g64.accessibility.Template.Configuration;
using System.ComponentModel;

namespace p4g64.accessibility.Configuration;
public class Config : Configurable<Config>
{
    [DisplayName("Debug Mode")]
    [Description("Logs additional information to the console that is useful for debugging.")]
    [DefaultValue(false)]
    public bool DebugEnabled { get; set; } = false;


    [DisplayName("Text to Speech Interact Prompt")]
    [Description("Enables saying when you can Interact with an object/npc.")]
    [DefaultValue(false)]
    public bool TextToSpeechInteractPrompt { get; set; } = false;
}

/// <summary>
/// Allows you to override certain aspects of the configuration creation process (e.g. create multiple configurations).
/// Override elements in <see cref="ConfiguratorMixinBase"/> for finer control.
/// </summary>
public class ConfiguratorMixin : ConfiguratorMixinBase
{
    // 
}
﻿using System;
using System.Collections.Generic;
using PluginConfig.API;

namespace Plugin.VRTRAKILL.ModSupport
{
    // Contains initializers for every mod
    internal static class Initializers
    {
        public static Dictionary<string, Action<object>> Mods = new Dictionary<string, Action<object>>
        {
            { "com.eternalUnion.pluginConfigurator", PluginConfigurator },
        };

        public static void PluginConfigurator(object o)
        {
            PluginConfig.API.PluginConfigurator PC = PluginConfig.API.PluginConfigurator.Create(Plugin.PLUGIN_NAME, Plugin.PLUGIN_GUID);
        }
    }
}
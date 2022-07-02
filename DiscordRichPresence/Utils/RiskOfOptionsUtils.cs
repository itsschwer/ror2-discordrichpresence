using BepInEx;
using RoR2;
using UnityEngine.SceneManagement;
using DiscordRPC;
using DiscordRPC.Message;
using DiscordRPC.Unity;
using UnityEngine;
using System;
using R2API.Utils;
using System.Collections.Generic;
using BepInEx.Configuration;
using BepInEx.Logging;
using DiscordRichPresence.Hooks;
using DiscordRichPresence.Utils;

namespace DiscordRichPresence.Utils
{
    public static class RiskOfOptionsUtils
    {
        public static bool IsEnabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");

        public static void AddCheckBoxOption(ConfigEntry<bool> entry)
        {
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(entry));
        }

        public static void AddMultiOption<T>(ConfigEntry<T> entry) where T : Enum
        {
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.ChoiceOption(entry));
        }

        public static void AddTextInputOption(ConfigEntry<string> entry)
        {
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.StringInputFieldOption(entry));
        }

        public static void SetModDescription(string description)
        {
            RiskOfOptions.ModSettingsManager.SetModDescription(description);
        }
    }
}
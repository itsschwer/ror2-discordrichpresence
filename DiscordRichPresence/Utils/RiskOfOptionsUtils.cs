using BepInEx.Bootstrap;
using BepInEx.Configuration;
using System;

namespace DiscordRichPresence.Utils
{
    public static class RiskOfOptionsUtils
    {
        public static bool IsEnabled => Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");

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
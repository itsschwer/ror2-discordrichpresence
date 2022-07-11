using System;
using System.Collections.Generic;

namespace DiscordRichPresence.Utils
{
    public static class InfoTextUtils
    {
        public static List<string> CharactersWithAssets = new List<string>()
        {
            "Acrid",
            "Artificer",
            "Bandit",
            "Captain",
            "Commando",
            "Engineer",
            "Heretic",
            "Huntress",
            "Loader",
            "MUL-T",
            "Mercenary",
            "REX",
            "Railgunner",
            "Void Fiend",
            "Enforcer",
            "CHEF",
            "Miner",
            "Paladin",
            "HAN-D",
            "Sniper",
            "Bomber",
            "Nemesis Enforcer",
            "Nemesis Commando",
            "Chirr",
            "Executioner",
            "An Arbiter",
            "Red Mist"
        };

        [Flags]
        public enum StyleTag : int
        {
            Damage = 1,
            Healing = 2,
            Utility = 4,
            Health = 8,
            Stack = 16,
            Mono = 32,
            Death = 64,
            UserSetting = 128,
            Artifact = 256,
            Sub = 512,
            Event = 1024,
            WorldEvent = 2048,
            KeywordName = 4096,
            Shrine = 8192
        }

        public static string GetCharacterInternalName(string name)
        {
            if (CharactersWithAssets.Contains(name))
            {
                return CharactersWithAssets.Find(c => c == name).ToLower().Replace(" ", "");
            }
            return "unknown";
        }

        public static string FormatTextStyleTag(string content, StyleTag styleTag)
        {
            string tagString;
            if (styleTag == StyleTag.Damage || styleTag == StyleTag.Healing || styleTag == StyleTag.Utility || styleTag == StyleTag.Health)
            {
                tagString = "cIs" + styleTag.ToString();
            }
            else
            {
                tagString = "c" + styleTag.ToString();
            }
            return $"<style={tagString}>{content}</style>";
        }
    }
}
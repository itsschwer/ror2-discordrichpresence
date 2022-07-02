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

namespace DiscordRichPresence.Utils
{
    public static class InfoTextUtils
    {
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

		public static string GetDifficultyString(DifficultyIndex difficultyIndex)
		{
			if ((int)difficultyIndex >= 3 && (int)difficultyIndex <= 10)
			{
				return "Eclipse " + ((int)difficultyIndex - 2);
			}
			switch (difficultyIndex)
			{
				case DifficultyIndex.Easy:
					return "Drizzle";
				case DifficultyIndex.Normal:
					return "Rainstorm";
				case DifficultyIndex.Hard:
					return "Monsoon";
				default:
					return "Unknown";
			}
		}

		public static bool GetCharacterInternalName(string name, out string formatted)
		{
			formatted = "Unknown";
			switch (name) // sigh...
			{
				case "Acrid":
					formatted = "croco";
					return true;
				case "Artificer":
					formatted = "mage";
					return true;
				case "Bandit":
					formatted = "bandit";
					return true;
				case "Captain":
					formatted = "captain";
					return true;
				case "Commando":
					formatted = "commando";
					return true;
				case "Engineer":
					formatted = "engi";
					return true;
				case "Heretic":
					formatted = "heretic";
					return true;
				case "Huntress":
					formatted = "huntress";
					return true;
				case "Loader":
					formatted = "loader";
					return true;
				case "MUL-T":
					formatted = "toolbot";
					return true;
				case "Mercenary":
					formatted = "merc";
					return true;
				case "REX":
					formatted = "treebot";
					return true;
				case "Railgunner":
					formatted = "railgunner";
					return true;
				case "Void Fiend":
					formatted = "voidsurvivor";
					return true;
			}
			return false;
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
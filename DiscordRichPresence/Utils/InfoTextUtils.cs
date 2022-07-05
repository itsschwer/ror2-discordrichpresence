using System;

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

		public static string GetCharacterInternalName(string name)
		{
			switch (name) // sigh...
			{
				case "Acrid":
					return "croco";
				case "Artificer":
					return "mage";
				case "Bandit":
					return "bandit";
				case "Captain":
					return "captain";
				case "Commando":
					return "commando";
				case "Engineer":
					return "engi";
				case "Heretic":
					return "heretic";
				case "Huntress":
					return "huntress";
				case "Loader":
					return "loader";
				case "MUL-T":
					return "toolbot";
				case "Mercenary":
					return "merc";
				case "REX":
					return "treebot";
				case "Railgunner":
					return "railgunner";
				case "Void Fiend":
					return "voidsurvivor";
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
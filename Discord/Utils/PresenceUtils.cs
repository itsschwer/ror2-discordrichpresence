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
    public static class PresenceUtils
    {
		public static void SetStagePresence(DiscordRpcClient client, RichPresence richPresence, SceneDef scene, Run run, bool includeRunTime, string whatToShow = "none")
		{
			richPresence.Assets.LargeImageKey = scene.baseSceneName;
			richPresence.Assets.LargeImageText = Language.GetString(scene.subtitleToken);

			richPresence.State = InfoTextUtils.GetDifficultyString(run.selectedDifficulty);
			if (whatToShow == "boss" && DiscordRichPresencePlugin.CurrentBoss != "None")
			{
				richPresence.State = "Fighting " + DiscordRichPresencePlugin.CurrentBoss + " | " + InfoTextUtils.GetDifficultyString(run.selectedDifficulty);
			}
			else if (whatToShow == "charge" && DiscordRichPresencePlugin.CurrentChargeLevel > 0)
            {
				richPresence.State = "Charging teleporter (" + DiscordRichPresencePlugin.CurrentChargeLevel * 100 + "%) | " + InfoTextUtils.GetDifficultyString(run.selectedDifficulty);
			}

			richPresence.Details = string.Format("Stage {0} - {1}", run.stageClearCount + 1, Language.GetString(scene.nameToken));

			richPresence.Timestamps = new Timestamps();
			if (scene.sceneType == SceneType.Stage && includeRunTime)
			{
				richPresence.Timestamps = new Timestamps()
				{
					StartUnixMilliseconds = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds() - ((ulong)run.GetRunStopwatch())
				};
			}

			DiscordRichPresencePlugin.RichPresence = richPresence;
			client.SetPresence(richPresence);
		}

		public static void SetMainMenuPresence(DiscordRpcClient client, RichPresence richPresence, string state = "")
		{
			richPresence.Assets = new Assets()
			{
				LargeImageKey = "riskofrain2", //lobby
				LargeImageText = "In Menu"
			};
			richPresence.Details = "In Menu";
			richPresence.State = DiscordRichPresencePlugin.PluginConfig.MainMenuIdleMessageEntry.Value;
			if (state != "")
            {
				richPresence.State = state;
            }
			richPresence.Timestamps = new Timestamps();
			richPresence.Secrets = new Secrets();
			richPresence.Party = new Party();

			DiscordRichPresencePlugin.RichPresence = richPresence;
			client.SetPresence(richPresence);
		}

		public static void SetLobbyPresence(DiscordRpcClient client, RichPresence richPresence, ulong lobbyID, Facepunch.Steamworks.Client faceClient)
		{
			richPresence.State = "In Lobby";
			richPresence.Details = "Preparing";
			richPresence.Assets = new Assets()
			{
				LargeImageKey = "riskofrain2", //lobby
				LargeImageText = "Join!",
			};
			richPresence.Party = new Party()
			{
				ID = faceClient.Username,
				Max = faceClient.Lobby.MaxMembers,
				Size = faceClient.Lobby.NumMembers
			};

			if (DiscordRichPresencePlugin.PluginConfig.AllowJoiningEntry.Value)
			{
				richPresence.Secrets = new Secrets()
				{
					JoinSecret = lobbyID.ToString()
				};
			}

			DiscordRichPresencePlugin.RichPresence = richPresence;
			client.SetPresence(richPresence);
		}
	}
}
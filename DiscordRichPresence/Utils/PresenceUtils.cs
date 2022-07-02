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

			richPresence.Details = InfoTextUtils.GetDifficultyString(run.selectedDifficulty);
			if (whatToShow == "boss" && DiscordRichPresencePlugin.CurrentBoss != "None")
			{
				richPresence.Details = "Fighting " + DiscordRichPresencePlugin.CurrentBoss + " | " + InfoTextUtils.GetDifficultyString(run.selectedDifficulty);
			}
			else if (whatToShow == "charge" && DiscordRichPresencePlugin.CurrentChargeLevel > 0)
            {
				richPresence.Details = "Charging teleporter (" + DiscordRichPresencePlugin.CurrentChargeLevel * 100 + "%) | " + InfoTextUtils.GetDifficultyString(run.selectedDifficulty);
			}

			richPresence.State = string.Format("Stage {0} - {1}", run.stageClearCount + 1, Language.GetString(scene.nameToken));

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

		public static void SetMainMenuPresence(DiscordRpcClient client, RichPresence richPresence, string details = "")
		{
			richPresence.Assets = new Assets()
			{
				LargeImageKey = "riskofrain2", //lobby
				LargeImageText = "In Menu"
			};
			richPresence.Details = DiscordRichPresencePlugin.PluginConfig.MainMenuIdleMessageEntry.Value;
			if (details != "")
            {
				richPresence.Details = details;
            }
			richPresence.Timestamps = new Timestamps();

			richPresence.State = "In Lobby";
			if (!Facepunch.Steamworks.Client.Instance.Lobby.IsValid)
            {
				richPresence.State = "In Menu";
				richPresence.Secrets = new Secrets();
				richPresence.Party = new Party();
			}

			DiscordRichPresencePlugin.RichPresence = richPresence;
			client.SetPresence(richPresence);
		}

		public static void SetLobbyPresence(DiscordRpcClient client, RichPresence richPresence, Facepunch.Steamworks.Client faceClient)
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
					JoinSecret = faceClient.Lobby.CurrentLobby.ToString()
				};
			}

			DiscordRichPresencePlugin.RichPresence = richPresence;
			client.SetPresence(richPresence);
		}
	}
}
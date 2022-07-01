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
		public static void SetStagePresence(DiscordRpcClient client, RichPresence richPresence, SceneDef scene, Run run, bool includeRunTime, bool showBoss = false)
		{
			richPresence.Assets.LargeImageKey = scene.baseSceneName;
			richPresence.Assets.LargeImageText = Language.GetString(scene.subtitleToken);

			richPresence.State = InfoTextUtils.GetDifficultyString(run.selectedDifficulty);
			if (showBoss && DiscordRichPresencePlugin.CurrentBoss != "None")
			{
				richPresence.State = "Fighting " + DiscordRichPresencePlugin.CurrentBoss + " | " + InfoTextUtils.GetDifficultyString(run.selectedDifficulty);
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

			client.SetPresence(richPresence);
		}

		public static void SetMainMenuPresence(DiscordRpcClient client, RichPresence richPresence)
		{
			richPresence.Assets = new Assets()
			{
				LargeImageKey = "riskofrain2", //lobby
				LargeImageText = "In Menu"
			};
			richPresence.State = "In Menu";
			richPresence.Details = DiscordRichPresencePlugin.MainMenuIdleMessageEntry.Value;
			richPresence.Timestamps = new Timestamps();
			richPresence.Secrets = new Secrets();
			richPresence.Party = new Party();

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

			if (DiscordRichPresencePlugin.AllowJoiningEntry.Value)
			{
				richPresence.Secrets = new Secrets()
				{
					JoinSecret = lobbyID.ToString()
				};
			}

			client.SetPresence(richPresence);
		}
	}
}
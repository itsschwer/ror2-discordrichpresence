using RoR2;
using DiscordRPC;
using System;
using static DiscordRichPresence.DiscordRichPresencePlugin;

namespace DiscordRichPresence.Utils
{
    public static class PresenceUtils
    {
		public static void SetStagePresence(DiscordRpcClient client, RichPresence richPresence, SceneDef scene, Run run, bool isPaused = false)
		{
			string sceneImageKey = scene.baseSceneName;
			if (sceneImageKey.StartsWith("it"))
            {
				sceneImageKey = sceneImageKey.Substring(2);
            }
			richPresence.Assets.LargeImageKey = sceneImageKey;
			richPresence.Assets.LargeImageText = "DiscordRichPresence v" + Instance.Info.Metadata.Version; //Language.GetString(scene.subtitleToken);

			richPresence.State = string.Format("Stage {0} - {1}", run.stageClearCount + 1, Language.GetString(scene.nameToken));
			if (run is InfiniteTowerRun infRun && infRun.waveIndex > 0)
            {
				richPresence.State = string.Format("Wave {0} - {1}", infRun.waveIndex, Language.GetString(scene.nameToken));
			}

			string currentDifficultyString = Language.GetString(DifficultyCatalog.GetDifficultyDef(run.selectedDifficulty).nameToken);
			if (MoonDetonationController == null)
            {
				richPresence.Details = currentDifficultyString;
				if (PluginConfig.TeleporterStatusEntry.Value == TeleporterStatus.Boss && CurrentBoss != "")
				{
					richPresence.Details = "Fighting " + CurrentBoss + " | " + currentDifficultyString;
				}
				else if (PluginConfig.TeleporterStatusEntry.Value == TeleporterStatus.Charge && CurrentChargeLevel > 0)
				{
					richPresence.Details = "Charging teleporter (" + CurrentChargeLevel * 100 + "%) | " + currentDifficultyString;
				}

				richPresence.Timestamps = new Timestamps(); // Clear timestamps
				if (scene.sceneType == SceneType.Stage && !isPaused)
				{
					richPresence.Timestamps.StartUnixMilliseconds = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds() - (ulong)run.GetRunStopwatch();
				}
			}
			else
            {
				richPresence.Details = "Escaping! | " + currentDifficultyString;
				richPresence.Timestamps.EndUnixMilliseconds = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds() + (ulong)MoonDetonationController.countdownDuration;
			}

			DiscordRichPresencePlugin.RichPresence = richPresence;
			client.SetPresence(richPresence);
		}

		public static void SetMainMenuPresence(DiscordRpcClient client, RichPresence richPresence, string details = "")
		{
            richPresence.Assets = new Assets
            {
                LargeImageKey = "riskofrain2", //lobby
                LargeImageText = "DiscordRichPresence v" + Instance.Info.Metadata.Version
            };

            richPresence.Details = PluginConfig.MainMenuIdleMessageEntry.Value;
			if (details != "")
            {
				richPresence.Details = details;
            }

			richPresence.Timestamps = new Timestamps(); // Clear timestamps

			richPresence.State = "In Menu";
			richPresence.Secrets = new Secrets();
			richPresence.Party = new Party(); // Clear secrets and party

			DiscordRichPresencePlugin.RichPresence = richPresence;
			client.SetPresence(richPresence);
		}

		public static void SetLobbyPresence(DiscordRpcClient client, RichPresence richPresence, Facepunch.Steamworks.Client faceClient, string details = "")
		{
			richPresence.State = "In Lobby";
			richPresence.Details = "Preparing";
			if (details != "")
			{
				richPresence.Details = details;
			}

			richPresence.Assets = new Assets
            {
                LargeImageKey = "riskofrain2", //lobby
                LargeImageText = "DiscordRichPresence v" + Instance.Info.Metadata.Version
            };
			richPresence.Timestamps = new Timestamps(); // Clear timestamps

			richPresence = UpdateParty(richPresence, faceClient);

			DiscordRichPresencePlugin.RichPresence = richPresence;
			client.SetPresence(richPresence);
		}

		public static void SetLobbyPresence(DiscordRpcClient client, RichPresence richPresence, EOSLobbyManager lobbyManager, string details = "")
		{
			richPresence.State = "In Lobby";
			richPresence.Details = "Preparing";
			if (details != "")
			{
				richPresence.Details = details;
			}

			richPresence.Assets = new Assets
			{
				LargeImageKey = "riskofrain2", //lobby
				LargeImageText = "DiscordRichPresence v" + Instance.Info.Metadata.Version
			};
			richPresence.Timestamps = new Timestamps(); // Clear timestamps

			richPresence = UpdateParty(richPresence, lobbyManager);

			DiscordRichPresencePlugin.RichPresence = richPresence;
			client.SetPresence(richPresence);
		}

		public static RichPresence UpdateParty(RichPresence richPresence, Facepunch.Steamworks.Client faceClient, bool includeJoinButton = true)
        {
			richPresence.Party.ID = faceClient.Username;
			richPresence.Party.Max = faceClient.Lobby.MaxMembers;
			richPresence.Party.Size = faceClient.Lobby.NumMembers;

			richPresence.Secrets = new Secrets();
			if (PluginConfig.AllowJoiningEntry.Value && includeJoinButton)
			{
				richPresence.Secrets.JoinSecret = faceClient.Lobby.CurrentLobby.ToString();
			}

			return richPresence;
		}

		public static RichPresence UpdateParty(RichPresence richPresence, EOSLobbyManager lobbyManager, bool includeJoinButton = true)
		{
			richPresence.Party.ID = lobbyManager.CurrentLobbyId;
			richPresence.Party.Max = lobbyManager.newestLobbyData.totalMaxPlayers;
			richPresence.Party.Size = lobbyManager.newestLobbyData.totalPlayerCount; // GetLobbyMembers().Length

			richPresence.Secrets = new Secrets();
			if (PluginConfig.AllowJoiningEntry.Value && includeJoinButton)
			{
				richPresence.Secrets.JoinSecret = lobbyManager.GetLobbyMembers()[0].ToString();
			}

			return richPresence;
		}
	}
}
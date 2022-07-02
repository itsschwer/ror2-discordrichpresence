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
using DiscordRichPresence.Utils;

namespace DiscordRichPresence.Hooks
{
    public static class SteamworksLobbyHooks
    {
        public static void Initialize()
        {
			On.RoR2.SteamworksLobbyManager.OnLobbyCreated += SteamworksLobbyManager_OnLobbyCreated;
			On.RoR2.SteamworksLobbyManager.OnLobbyJoined += SteamworksLobbyManager_OnLobbyJoined;
			On.RoR2.SteamworksLobbyManager.OnLobbyChanged += SteamworksLobbyManager_OnLobbyChanged;
			On.RoR2.SteamworksLobbyManager.LeaveLobby += SteamworksLobbyManager_LeaveLobby;
		}

		public static void Dispose()
        {
			On.RoR2.SteamworksLobbyManager.OnLobbyCreated -= SteamworksLobbyManager_OnLobbyCreated;
			On.RoR2.SteamworksLobbyManager.OnLobbyJoined -= SteamworksLobbyManager_OnLobbyJoined;
			On.RoR2.SteamworksLobbyManager.OnLobbyChanged -= SteamworksLobbyManager_OnLobbyChanged;
			On.RoR2.SteamworksLobbyManager.LeaveLobby -= SteamworksLobbyManager_LeaveLobby;
		}

		private static void SteamworksLobbyManager_OnLobbyCreated(On.RoR2.SteamworksLobbyManager.orig_OnLobbyCreated orig, SteamworksLobbyManager self, bool success)
		{
			orig(self, success);

			if (!success || Facepunch.Steamworks.Client.Instance == null)
			{
				return;
			}


			ulong lobbyID = Facepunch.Steamworks.Client.Instance.Lobby.CurrentLobby;

			DiscordRichPresencePlugin.LoggerEXT.LogInfo("Discord broadcasting new Steam lobby with ID " + lobbyID);

			PresenceUtils.SetLobbyPresence(DiscordRichPresencePlugin.Client, DiscordRichPresencePlugin.RichPresence, Facepunch.Steamworks.Client.Instance);
		}

		private static void SteamworksLobbyManager_OnLobbyJoined(On.RoR2.SteamworksLobbyManager.orig_OnLobbyJoined orig, SteamworksLobbyManager self, bool success)
		{
			orig(self, success);

			if (!success || Facepunch.Steamworks.Client.Instance == null)
			{
				return;
			}

			DiscordRichPresencePlugin.LoggerEXT.LogInfo("Discord join complete");

			PresenceUtils.SetLobbyPresence(DiscordRichPresencePlugin.Client, DiscordRichPresencePlugin.RichPresence, Facepunch.Steamworks.Client.Instance);
		}

		private static void SteamworksLobbyManager_OnLobbyChanged(On.RoR2.SteamworksLobbyManager.orig_OnLobbyChanged orig, SteamworksLobbyManager self)
		{
			orig(self);

			if (Facepunch.Steamworks.Client.Instance == null || !self.isInLobby)
			{
				return;
			}

			DiscordRichPresencePlugin.LoggerEXT.LogInfo("Discord re-broadcasting Steam Lobby");

			PresenceUtils.SetLobbyPresence(DiscordRichPresencePlugin.Client, DiscordRichPresencePlugin.RichPresence, Facepunch.Steamworks.Client.Instance);
		}

		private static void SteamworksLobbyManager_LeaveLobby(On.RoR2.SteamworksLobbyManager.orig_LeaveLobby orig, SteamworksLobbyManager self)
		{
			orig(self);

			if (DiscordRichPresencePlugin.Client != null && DiscordRichPresencePlugin.Client.IsInitialized)
			{
				PresenceUtils.SetMainMenuPresence(DiscordRichPresencePlugin.Client, DiscordRichPresencePlugin.RichPresence);
			}
		}
	}
}
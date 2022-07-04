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
	public static class EOSLobbyHooks
	{
		public static void Initialize()
		{
			On.RoR2.EOSLobbyManager.OnLobbyCreated += EOSLobbyManager_OnLobbyCreated;
            On.RoR2.EOSLobbyManager.OnLobbyJoined += EOSLobbyManager_OnLobbyJoined;
            // On.RoR2.EOSLobbyManager.OnLobbyChanged += EOSLobbyManager_OnLobbyChanged;
            On.RoR2.EOSLobbyManager.LeaveLobby += EOSLobbyManager_LeaveLobby;
		}

        public static void Dispose()
		{
			On.RoR2.EOSLobbyManager.OnLobbyCreated -= EOSLobbyManager_OnLobbyCreated;
			On.RoR2.EOSLobbyManager.OnLobbyJoined -= EOSLobbyManager_OnLobbyJoined;
			// On.RoR2.EOSLobbyManager.OnLobbyChanged -= EOSLobbyManager_OnLobbyChanged;
			On.RoR2.EOSLobbyManager.LeaveLobby -= EOSLobbyManager_LeaveLobby;
		}

		private static void EOSLobbyManager_OnLobbyCreated(On.RoR2.EOSLobbyManager.orig_OnLobbyCreated orig, EOSLobbyManager self, Epic.OnlineServices.Lobby.CreateLobbyCallbackInfo data)
		{
			orig(self, data);

			if (data.ResultCode != Epic.OnlineServices.Result.Success || self == null)
			{
				return;
			}

			DiscordRichPresencePlugin.LoggerEXT.LogInfo("Discord broadcasting new EOS lobby with ID " + self.CurrentLobbyId);

			DiscordRichPresencePlugin.CurrentEOSLobby = self;

			PresenceUtils.SetLobbyPresence(DiscordRichPresencePlugin.Client, DiscordRichPresencePlugin.RichPresence, self);
		}

		private static void EOSLobbyManager_OnLobbyJoined(On.RoR2.EOSLobbyManager.orig_OnLobbyJoined orig, EOSLobbyManager self, Epic.OnlineServices.Lobby.JoinLobbyCallbackInfo data)
		{
			orig(self, data);

			if (data.ResultCode != Epic.OnlineServices.Result.Success || self == null)
			{
				return;
			}

			DiscordRichPresencePlugin.LoggerEXT.LogInfo("Successfully joined EOS lobby");

			DiscordRichPresencePlugin.CurrentEOSLobby = self;

			PresenceUtils.SetLobbyPresence(DiscordRichPresencePlugin.Client, DiscordRichPresencePlugin.RichPresence, self);
		}

		private static void EOSLobbyManager_LeaveLobby(On.RoR2.EOSLobbyManager.orig_LeaveLobby orig, EOSLobbyManager self)
		{
			orig(self);

			if (DiscordRichPresencePlugin.Client == null || !DiscordRichPresencePlugin.Client.IsInitialized)
			{
				return;
			}

			DiscordRichPresencePlugin.CurrentEOSLobby = self;

			PresenceUtils.SetMainMenuPresence(DiscordRichPresencePlugin.Client, DiscordRichPresencePlugin.RichPresence);
		}
	}
}
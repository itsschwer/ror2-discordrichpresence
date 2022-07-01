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
using DiscordRichPresence.Hooks;
using DiscordRichPresence.Utils;

namespace DiscordRichPresence
{
	[BepInDependency("com.bepis.r2api")]

	[BepInPlugin("com.cuno.discord", "Discord Rich Presence", "1.0.1")]

    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync)] // Client-sided

	[R2APISubmoduleDependency("CommandHelper")]

	public class DiscordRichPresencePlugin : BaseUnityPlugin
	{
		internal static ManualLogSource LoggerEXT { get; private set; }

		public static DiscordRpcClient Client { get; set; }

		public static RichPresence RichPresence { get; set; }

		public static ConfigEntry<bool> AllowJoiningEntry { get; set; }

		public static ConfigEntry<bool> ShowCurrentBossEntry { get; set; }

		public static ConfigEntry<string> MainMenuIdleMessageEntry { get; set; }

		public static string CurrentBoss { get; set; } = "None";

		public void Awake()
		{
			LoggerEXT = Logger;
			Logger.LogInfo("Starting Discord Rich Presence...");
			UnityNamedPipe pipe = new UnityNamedPipe();
			Client = new DiscordRpcClient("992086428240580720", -1, null, true, pipe);
			Client.RegisterUriScheme("632360");

			RichPresence = new RichPresence()
			{
				State = "Starting game..."
			};

			Client.SetPresence(RichPresence);

			CommandHelper.AddToConsoleWhenReady(); // Init all commands

			Client.Initialize();

			AllowJoiningEntry = Config.Bind("Options", "AllowJoining", true, "Controls whether or not other users should be allowed to ask to join your game.");
			ShowCurrentBossEntry = Config.Bind("Options", "ShowCurrentBoss", true, "Controls whether or not the currently fought boss should be displayed.");
			MainMenuIdleMessageEntry = Config.Bind("Options", "MainMenuIdleMessage", "", "Allows you to choose a message to be displayed when idling in the main menu.");
		}

		private static void InitializeHooks()
        {
			DiscordClientHooks.Initialize(Client);
			PauseManagerHooks.Initialize();
			SteamworksLobbyHooks.Initialize();

			On.RoR2.Run.BeginStage += Run_BeginStage;
            On.RoR2.CharacterMaster.OnBodyStart += CharacterMaster_OnBodyStart;
            On.RoR2.CharacterMaster.OnBodyDestroyed += CharacterMaster_OnBodyDestroyed;

			SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
		}

        private static void CharacterMaster_OnBodyStart(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
        {
			if (InfoTextUtils.GetCharacterInternalName(body.GetDisplayName(), out string formatted) && body.isClient && body.isPlayerControlled) // TO-DO: Test if CharacterBody.isClient and CharacterBody.isPlayerControlled distinguishes on MP
			{
				RichPresence.Assets.SmallImageKey = formatted;
				RichPresence.Assets.SmallImageText = body.GetDisplayName();
				Client.SetPresence(RichPresence);
			}
			else if (body.isBoss)
            {
				CurrentBoss = body.GetDisplayName();
            }

			orig(self, body);
		}

		private static void CharacterMaster_OnBodyDestroyed(On.RoR2.CharacterMaster.orig_OnBodyDestroyed orig, CharacterMaster self, CharacterBody characterBody) // TO-DO: Fix boss death not triggering
		{
			LoggerEXT.LogInfo("DiedPreson:" + characterBody.GetDisplayName());
			LoggerEXT.LogInfo("IsBossWhoDied?:" + characterBody.isBoss);
			if (characterBody.isBoss && characterBody.GetDisplayName() == CurrentBoss)
			{
				CurrentBoss = "None";
			}
			orig(self, characterBody);
		}

		private static void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
		{
			SceneDef scene = SceneCatalog.GetSceneDefForCurrentScene();

			if (scene != null)
			{
				PresenceUtils.SetStagePresence(Client, RichPresence, scene, self, true, ShowCurrentBossEntry.Value);
			}
			orig(self);
		}

		private static void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
		{
			if (Client != null && Client.IsInitialized && (arg1.name == "title" || arg1.name == "lobby")) // TO-DO: Create separate presence for in menu but choosing character
			{
				PresenceUtils.SetMainMenuPresence(Client, RichPresence);
			}
		}

		public void OnEnable()
        {
			InitializeHooks();
        }

		public void OnDisable()
		{
			Dispose();
		}

		public static void Dispose()
        {
			DiscordClientHooks.Dispose(Client);
			PauseManagerHooks.Dispose();
			SteamworksLobbyHooks.Dispose();

			On.RoR2.Run.BeginStage -= Run_BeginStage;
			On.RoR2.CharacterMaster.OnBodyStart -= CharacterMaster_OnBodyStart;
			On.RoR2.CharacterMaster.OnBodyDestroyed -= CharacterMaster_OnBodyDestroyed;

			SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;

			Client.Dispose();
		}
    }
}
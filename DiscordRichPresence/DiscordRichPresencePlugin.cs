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

// Thanks to WhelanB (to which this repository originates from)
// and DarkKronicle (whose repository this is forked from)

namespace DiscordRichPresence
{
	[BepInDependency("com.bepis.r2api")]

    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]

	[BepInPlugin("com.cuno.discord", "Discord Rich Presence", "1.1.0")]

    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync)] // Client-sided

	public class DiscordRichPresencePlugin : BaseUnityPlugin
	{
		internal static ManualLogSource LoggerEXT { get; private set; }

		public static DiscordRpcClient Client { get; set; }

		public static RichPresence RichPresence { get; set; }

		public struct PluginConfig
        {
			public static ConfigEntry<bool> AllowJoiningEntry { get; set; }

			public static ConfigEntry<TeleporterStatus> TeleporterStatusEntry { get; set; }

			public static ConfigEntry<string> MainMenuIdleMessageEntry { get; set; }
		}

		public static float CurrentChargeLevel { get; set; }

		public static string CurrentBoss { get; set; } = "None";

		public static Button TestButton;

		public static DiscordRichPresencePlugin Instance { get; private set; }

        public static SceneDef CurrentScene => SceneCatalog.GetSceneDefForCurrentScene();

        public enum TeleporterStatus : byte
        {
            None = 0,
			Boss = 1,
			Charge = 2
        }

		public void Awake()
		{
			Instance = this;
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

			Client.Initialize();

			PluginConfig.AllowJoiningEntry = Config.Bind("Options", "Allow Joining", true, "Controls whether or not other users should be allowed to ask to join your game.");
			PluginConfig.TeleporterStatusEntry = Config.Bind("Options", "Teleporter Status", TeleporterStatus.None, "Controls whether the teleporter boss, teleporter charge status, or neither, should be shown alongside the current difficulty.");
			PluginConfig.MainMenuIdleMessageEntry = Config.Bind("Options", "Main Menu Idle Message", "", "Allows you to choose a message to be displayed when idling in the main menu.");

			if (RiskOfOptionsUtils.IsEnabled)
            {
				RiskOfOptionsUtils.SetModDescription("Adds Discord Rich Presence functionality to Risk of Rain 2");
				RiskOfOptionsUtils.AddCheckBoxOption(PluginConfig.AllowJoiningEntry);
				RiskOfOptionsUtils.AddMultiOption(PluginConfig.TeleporterStatusEntry);
				RiskOfOptionsUtils.AddTextInputOption(PluginConfig.MainMenuIdleMessageEntry);
			}
		}

		private static void InitializeHooks()
        {
			DiscordClientHooks.Initialize(Client);
			PauseManagerHooks.Initialize();
			SteamworksLobbyHooks.Initialize();

            On.RoR2.Run.OnServerBossAdded += Run_OnServerBossAdded;
            On.RoR2.Run.OnServerBossDefeated += Run_OnServerBossDefeated;
			On.RoR2.Run.BeginStage += Run_BeginStage;
            On.RoR2.CharacterMaster.OnBodyStart += CharacterMaster_OnBodyStart;
            On.RoR2.TeleporterInteraction.FixedUpdate += TeleporterInteraction_FixedUpdate;

			SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
		}

        public static void Dispose()
		{
			DiscordClientHooks.Dispose(Client);
			PauseManagerHooks.Dispose();
			SteamworksLobbyHooks.Dispose();

			On.RoR2.Run.OnServerBossAdded -= Run_OnServerBossAdded;
			On.RoR2.Run.OnServerBossDefeated -= Run_OnServerBossDefeated;
			On.RoR2.Run.BeginStage -= Run_BeginStage;
			On.RoR2.CharacterMaster.OnBodyStart -= CharacterMaster_OnBodyStart;
			On.RoR2.TeleporterInteraction.FixedUpdate -= TeleporterInteraction_FixedUpdate;

			SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;

			Client.Dispose();
		}

		public void OnEnable()
		{
			InitializeHooks();
		}

		public void OnDisable()
		{
			Dispose();
		}

		private static void Run_OnServerBossDefeated(On.RoR2.Run.orig_OnServerBossDefeated orig, Run self, BossGroup bossGroup)
        {
			CurrentBoss = "None";

			orig(self, bossGroup);
        }

        private static void Run_OnServerBossAdded(On.RoR2.Run.orig_OnServerBossAdded orig, Run self, BossGroup bossGroup, CharacterMaster characterMaster)
        {
			CurrentBoss = characterMaster.GetBody().GetDisplayName();

			orig(self, bossGroup, characterMaster);
        }

        private static void TeleporterInteraction_FixedUpdate(On.RoR2.TeleporterInteraction.orig_FixedUpdate orig, TeleporterInteraction self)
        {
            if (Math.Round(self.chargeFraction, 2) != CurrentChargeLevel && PluginConfig.TeleporterStatusEntry.Value == TeleporterStatus.Charge)
            {
				CurrentChargeLevel = (float)Math.Round(self.chargeFraction, 2);
				PresenceUtils.SetStagePresence(Client, RichPresence, CurrentScene, Run.instance, false, PluginConfig.TeleporterStatusEntry.Value);
			}

			orig(self);
        }

        private static void CharacterMaster_OnBodyStart(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
        {
			if (InfoTextUtils.GetCharacterInternalName(body.GetDisplayName(), out string formatted) && body == CharacterMaster.readOnlyInstancesList[0].GetBody())
			{
				RichPresence.Assets.SmallImageKey = formatted;
				RichPresence.Assets.SmallImageText = body.GetDisplayName();
				Client.SetPresence(RichPresence);
			}

			orig(self, body);
		}

		private static void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
		{
			CurrentChargeLevel = 0;

			if (CurrentScene != null)
			{
				PresenceUtils.SetStagePresence(Client, RichPresence, CurrentScene, self, false, PluginConfig.TeleporterStatusEntry.Value);
			}

			orig(self);
		}

		private static void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
		{
			if (Client == null || !Client.IsInitialized)
			{
				return;
			}

			if (arg1.name == "title" && !Facepunch.Steamworks.Client.Instance.Lobby.IsValid)
            {
				PresenceUtils.SetMainMenuPresence(Client, RichPresence);
			}
			else if (arg1.name == "title" && Facepunch.Steamworks.Client.Instance.Lobby.IsValid)
            {
				PresenceUtils.SetLobbyPresence(Client, RichPresence, Facepunch.Steamworks.Client.Instance);
			}
			if (arg1.name == "lobby")
            {
				PresenceUtils.SetMainMenuPresence(Client, RichPresence, "Choosing character");
			}
			if (arg1.name == "logbook")
            {
				PresenceUtils.SetMainMenuPresence(Client, RichPresence, "Reading logbook");
			}
		}
    }
}
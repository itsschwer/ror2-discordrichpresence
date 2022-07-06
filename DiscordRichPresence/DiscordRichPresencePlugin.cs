using BepInEx;
using RoR2;
using UnityEngine.SceneManagement;
using DiscordRPC;
using DiscordRPC.Unity;
using R2API.Utils;
using BepInEx.Configuration;
using BepInEx.Logging;
using DiscordRichPresence.Hooks;
using DiscordRichPresence.Utils;

// Thanks to WhelanB (to which this repository originates from)
// and DarkKronicle (whose repository this is forked from)

namespace DiscordRichPresence
{
	[BepInPlugin("com.cuno.discord", "Discord Rich Presence", "1.2.1")]

	[BepInDependency("com.bepis.r2api")]

    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]

    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync)] // Client-sided

	public class DiscordRichPresencePlugin : BaseUnityPlugin
	{
		internal static ManualLogSource LoggerEXT { get; private set; }

		public static DiscordRpcClient Client { get; set; }

		public static RichPresence RichPresence { get; set; }

		public static DiscordRichPresencePlugin Instance { get; private set; }

        public static SceneDef CurrentScene => SceneCatalog.GetSceneDefForCurrentScene();

		public static float CurrentChargeLevel { get; set; }

		public static float MoonCountdownTimer { get; set; }

		public static string CurrentBoss { get; set; }

		public struct PluginConfig
		{
			public static ConfigEntry<bool> AllowJoiningEntry { get; set; }

			public static ConfigEntry<TeleporterStatus> TeleporterStatusEntry { get; set; }

			public static ConfigEntry<string> MainMenuIdleMessageEntry { get; set; }
		}

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
			Client = new DiscordRpcClient("992086428240580720", client: pipe);
			Client.RegisterUriScheme("632360");

			RichPresence = new RichPresence()
			{
				State = "Starting game...",
				Assets = new Assets(),
				Secrets = new Secrets(),
				Timestamps = new Timestamps()
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
			DiscordClientHooks.AddHooks(Client);
			PauseManagerHooks.AddHooks();
			SteamworksLobbyHooks.AddHooks();
			EOSLobbyHooks.AddHooks();
			RoR2Hooks.AddHooks();

			SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
		}

        public static void Dispose()
		{
			DiscordClientHooks.RemoveHooks(Client);
			PauseManagerHooks.RemoveHooks();
			SteamworksLobbyHooks.RemoveHooks();
			EOSLobbyHooks.RemoveHooks();
			RoR2Hooks.RemoveHooks();

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

		private static void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
		{
			if (Client == null || !Client.IsInitialized)
			{
				return;
			}

			CurrentBoss = "";
			CurrentChargeLevel = 0;
			MoonCountdownTimer = 0;
			if (arg1.name == "title" && Facepunch.Steamworks.Client.Instance.Lobby.IsValid)
            {
				PresenceUtils.SetLobbyPresence(Client, RichPresence, Facepunch.Steamworks.Client.Instance);
			}
			else if (arg1.name == "title" && IsInEOSLobby(out EOSLobbyManager lobbyManager))
			{
				PresenceUtils.SetLobbyPresence(Client, RichPresence, lobbyManager);
			}
			if (arg1.name == "lobby" && !Facepunch.Steamworks.Client.Instance.Lobby.IsValid && !IsInEOSLobby(out EOSLobbyManager _))
            {
				PresenceUtils.SetMainMenuPresence(Client, RichPresence, "Choosing Character");
			}
			else if (arg1.name == "lobby" && Facepunch.Steamworks.Client.Instance.Lobby.IsValid)
			{
				PresenceUtils.SetLobbyPresence(Client, RichPresence, Facepunch.Steamworks.Client.Instance, "Choosing Character");
			}
			else if (arg1.name == "lobby" && IsInEOSLobby(out EOSLobbyManager lobbyManager))
			{
				PresenceUtils.SetLobbyPresence(Client, RichPresence, lobbyManager, "Choosing Character");
			}
			if (arg1.name == "logbook")
            {
				PresenceUtils.SetMainMenuPresence(Client, RichPresence, "Reading Logbook");
			}
			else if (Run.instance != null && CurrentScene != null && (Facepunch.Steamworks.Client.Instance.Lobby.IsValid || IsInEOSLobby(out EOSLobbyManager _)))
            {
				PresenceUtils.SetStagePresence(Client, RichPresence, CurrentScene, Run.instance);
			}
		}

		public static bool IsInEOSLobby(out EOSLobbyManager lobbyManager)
        {
			lobbyManager = EOSLobbyManager.GetFromPlatformSystems();
            return lobbyManager != null && lobbyManager.isInLobby;
        }
    }
}
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

namespace DiscordRichPresence
{
	[BepInDependency("com.bepis.r2api")]

	[BepInPlugin("com.whelanb.discord", "Discord Rich Presence", "3.0.1")]

	[R2APISubmoduleDependency("CommandHelper")]

	public class Discord : BaseUnityPlugin
	{
		enum PrivacyLevel
		{
			Disabled = 0,
			Presence = 1,
			Join = 2
		}

		public DiscordRpcClient Client { get; set; }

		public RichPresence RichPresence { get; set; }

		static PrivacyLevel currentPrivacyLevel;
		
		public void Awake()
		{
			Logger.LogInfo("Starting Discord Rich Presence....");
			UnityNamedPipe pipe = new UnityNamedPipe();
			// Get your own clientid!
			Client = new DiscordRpcClient("992086428240580720", -1, null, true, pipe);
			Client.RegisterUriScheme("632360");

			RichPresence = new RichPresence()
			{
				State = "Starting game..."
			};

			Client.SetPresence(RichPresence);

			currentPrivacyLevel = PrivacyLevel.Join;

			// Subscribe to join events
			Client.Subscribe(DiscordRPC.EventType.Join);
			Client.Subscribe(DiscordRPC.EventType.JoinRequest);

			// Setup Discord client hooks
			Client.OnReady += Client_OnReady;
			Client.OnError += Client_OnError;
			Client.OnJoinRequested += Client_OnJoinRequested;
			// client.OnJoin += Client_OnJoin;

			Client.Initialize();

			// When a new stage is entered, update stats
			On.RoR2.Run.BeginStage += Run_BeginStage;

			// Used to handle additional potential presence changes
			SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

			// Handle Presence when Lobby is created
			On.RoR2.SteamworksLobbyManager.OnLobbyCreated += SteamworksLobbyManager_OnLobbyCreated;
			// Handle Presence when Lobby is joined
			On.RoR2.SteamworksLobbyManager.OnLobbyJoined += SteamworksLobbyManager_OnLobbyJoined;
			// Handle Presence when Lobby changes
			On.RoR2.SteamworksLobbyManager.OnLobbyChanged += SteamworksLobbyManager_OnLobbyChanged;
			// Handle Presence when user leaves Lobby
			On.RoR2.SteamworksLobbyManager.LeaveLobby += SteamworksLobbyManager_LeaveLobby;

			On.RoR2.CharacterBody.Awake += CharacterBody_Awake;

			// Messy work around for hiding timer in Discord when user pauses the game during a run
			PauseManager.onPauseStartGlobal += OnGamePaused;

			// When the user un-pauses, re-broadcast run time to Discord
			PauseManager.onPauseEndGlobal += OnGameUnPaused;

			// Register console commands
			On.RoR2.Console.Awake += (orig, self) =>
			{
				CommandHelper.AddToConsoleWhenReady();
				orig(self);
			};
		}

		public void OnGamePaused()
		{
			if (Run.instance != null)
			{
				if (Client.CurrentPresence != null)
				{
					SceneDef scene = SceneCatalog.GetSceneDefForCurrentScene();
					if (scene != null)
                    {
						SetStagePresence(scene, Run.instance, false);
					}
				}
			}
		}

		public void OnGameUnPaused()
		{
			if (Run.instance != null)
			{
				if (Client.CurrentPresence != null)
				{
					SceneDef scene = SceneCatalog.GetSceneDefForCurrentScene();
					if (scene != null)
                    {
						SetStagePresence(scene, Run.instance, true);
					}
				}
			}
		}

		private bool CharacterInternalName(string name, out string formatted)
        {
			formatted = "Unknown";
			switch (name) // sigh...
            {
				case "Acrid":
					formatted = "croco";
					return true;
				case "Artificer":
					formatted = "mage";
					return true;
				case "Bandit":
					formatted = "bandit";
					return true;
				case "Captain":
					formatted = "captain";
					return true;
				case "Commando":
					formatted = "commando";
					return true;
				case "Engineer":
					formatted = "engi";
					return true;
				case "Heretic":
					formatted = "heretic";
					return true;
				case "Huntress":
					formatted = "huntress";
					return true;
				case "Loader":
					formatted = "loader";
					return true;
				case "MUL-T":
					formatted = "toolbot";
					return true;
				case "Mercenary":
					formatted = "merc";
					return true;
				case "REX":
					formatted = "treebot";
					return true;
				case "Railgunner":
					formatted = "railgunner";
					return true;
				case "Void Fiend":
					formatted = "voidsurvivor";
					return true;
			}
			return false;
        }

		private void CharacterBody_Awake(On.RoR2.CharacterBody.orig_Awake orig, CharacterBody self)
		{
			if (CharacterInternalName(self.GetDisplayName(), out string formatted)) // Can't figure out a way to detect player from CharacterBody, so going the nuclear option :(
			{
				RichPresence.Assets.SmallImageKey = formatted;
				RichPresence.Assets.SmallImageText = self.GetDisplayName();
				Client.SetPresence(RichPresence);
			}

			orig(self);
		}

		// Remove any lingering hooks and dispose of discord client connection
		public void Dispose()
		{
			On.RoR2.Run.BeginStage -= Run_BeginStage;

			On.RoR2.SteamworksLobbyManager.OnLobbyCreated -= SteamworksLobbyManager_OnLobbyCreated;
			On.RoR2.SteamworksLobbyManager.OnLobbyJoined -= SteamworksLobbyManager_OnLobbyJoined;
			On.RoR2.SteamworksLobbyManager.OnLobbyChanged -= SteamworksLobbyManager_OnLobbyChanged;
			On.RoR2.SteamworksLobbyManager.LeaveLobby -= SteamworksLobbyManager_LeaveLobby;

			PauseManager.onPauseStartGlobal -= OnGamePaused;
			PauseManager.onPauseEndGlobal -= OnGameUnPaused;

			Client.Unsubscribe(DiscordRPC.EventType.Join);
			Client.Unsubscribe(DiscordRPC.EventType.JoinRequest);

			Client.Dispose();
		}

		// Be kind, rewind!
		public void OnDisable()
		{
			Dispose();
		}

		private void Client_OnJoin(object sender, JoinMessage args)
		{
			Logger.LogInfo("Joining Game via Discord - Steam Lobby ID: " + args.Secret);
            ConCommandArgs conArgs = new ConCommandArgs
            {
                userArgs = new List<string>() { args.Secret }
            };
            // We use this so we don't have to use the Epic Games SDK
            SteamworksLobbyManager.GetFromPlatformSystems().JoinLobby(conArgs);
		}

		//This is mostly handled through the Discord overlay now, so we can always accept for now
		private void Client_OnJoinRequested(object sender, JoinRequestMessage args)
		{
			Logger.LogInfo(string.Format("User {0} asked to join lobby", args.User.Username));
			Chat.AddMessage(string.Format("Discord user {0} has asked to join your game!", args.User.Username));
			//Always let people into your game for now
			Client.Respond(args, true);
		}

		private void Client_OnError(object sender, ErrorMessage args)
		{
			Logger.LogError(args.Message);
			Dispose();
		}

		private void Client_OnReady(object sender, ReadyMessage args)
		{
			Logger.LogInfo("Discord Rich Presence Ready - User: " + args.User.Username);

			if (Client != null && Client.IsInitialized)
            {
				RichPresence = new RichPresence()
				{
					Assets = new Assets()
					{
						LargeImageKey = "riskofrain2",
						LargeImageText = "Risk of Rain 2"
					},
					State = "Starting game..."
				};

				Client.SetPresence(RichPresence);
			}
		}

		//Currently, presence isn't cleared on run/lobby exit - TODO
		private void SteamworksLobbyManager_LeaveLobby(On.RoR2.SteamworksLobbyManager.orig_LeaveLobby orig, SteamworksLobbyManager self)
		{
			if (Client != null && Client.IsInitialized)
            {
				SetMainMenuPresence();
			}

			orig(self);
		}

		//TODO - Refactor these as they all update the Presence with the same data
		private void SteamworksLobbyManager_OnLobbyChanged(On.RoR2.SteamworksLobbyManager.orig_OnLobbyChanged orig, SteamworksLobbyManager self)
		{
			orig(self);

			if (Facepunch.Steamworks.Client.Instance == null || !self.isInLobby)
            {
				return;
			}

			Logger.LogInfo("Discord re-broadcasting Steam Lobby");
			ulong lobbyID = Facepunch.Steamworks.Client.Instance.Lobby.CurrentLobby;
			SetLobbyPresence(lobbyID, Facepunch.Steamworks.Client.Instance);
		}

		private void SteamworksLobbyManager_OnLobbyJoined(On.RoR2.SteamworksLobbyManager.orig_OnLobbyJoined orig,SteamworksLobbyManager self,  bool success)
		{
			orig(self, success);

			if (!success || Facepunch.Steamworks.Client.Instance == null)
            {
				return;
			}

			Logger.LogInfo("Discord join complete");

			ulong lobbyID = Facepunch.Steamworks.Client.Instance.Lobby.CurrentLobby;

			SetLobbyPresence(lobbyID, Facepunch.Steamworks.Client.Instance);
		}

		private void SteamworksLobbyManager_OnLobbyCreated(On.RoR2.SteamworksLobbyManager.orig_OnLobbyCreated orig, SteamworksLobbyManager self, bool success)
		{
			orig(self, success);

			if (!success || Facepunch.Steamworks.Client.Instance == null)
            {
				return;
			}
				

			ulong lobbyID = Facepunch.Steamworks.Client.Instance.Lobby.CurrentLobby;

			Logger.LogInfo("Discord broadcasting new Steam lobby" + lobbyID);
			SetLobbyPresence(lobbyID, Facepunch.Steamworks.Client.Instance);
		}

		//If the scene being loaded is a menu scene, remove the presence
		private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
		{
			if (Client != null && Client.IsInitialized && arg1.name == "title")
            {
				SetMainMenuPresence();
			}
		}

		//When the game begins a new stage, update presence
		private void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
		{
			//Grab the run start time (elapsed time does not take into account timer freeze from intermissions yet)
			//Also runs a little fast - find a better hook point!
			if (currentPrivacyLevel != PrivacyLevel.Disabled)
			{
				SceneDef scene = SceneCatalog.GetSceneDefForCurrentScene();

				if (scene != null)
				{
					SetStagePresence(scene, self, true);
				}
			}
			orig(self);
		}

		private string GetDifficultyString(DifficultyIndex difficultyIndex)
        {
			if ((int)difficultyIndex >= 3 && (int)difficultyIndex <= 10)
            {
				return "Eclipse " + ((int)difficultyIndex - 2);
            }
			switch (difficultyIndex)
            {
				case DifficultyIndex.Easy:
					return "Drizzle";
				case DifficultyIndex.Normal:
					return "Rainstorm";
				case DifficultyIndex.Hard:
					return "Monsoon";
				default:
					return "Unknown";
            }
        }

		public void SetStagePresence(SceneDef scene, Run run, bool includeRunTime)
		{
			RichPresence.Assets = new Assets()
			{
				LargeImageKey = scene.baseSceneName,
				LargeImageText = Language.GetString(scene.subtitleToken)
			};
			RichPresence.State = GetDifficultyString(run.selectedDifficulty);
			RichPresence.Details = string.Format("Stage {0} - {1}", run.stageClearCount + 1, Language.GetString(scene.nameToken));

			if (scene.sceneType == SceneType.Stage && includeRunTime) //When in-game and paused, includeRunTime becomes false but sceneType is still SceneType.Stage
			{
				RichPresence.Timestamps = new Timestamps()
				{
					StartUnixMilliseconds = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds() - ((ulong)run.GetRunStopwatch())
				};
			}

			Client.SetPresence(RichPresence);
		}

        public void SetMainMenuPresence()
        {
			RichPresence.Assets = new Assets()
			{
				LargeImageKey = "lobby",
				LargeImageText = "In Menu"
			};
			RichPresence.State = "In Menu";
			RichPresence.Timestamps = new Timestamps();

			Client.SetPresence(RichPresence);
		}

		public void SetLobbyPresence(ulong lobbyID, Facepunch.Steamworks.Client client)
		{
			RichPresence.State = "In Lobby";
			RichPresence.Details = "Preparing";
			RichPresence.Assets = new Assets()
			{
				//LargeImageKey = "lobby",
				LargeImageText = "Join!",
			};
			RichPresence.Party = new Party()
			{
				ID = client.Username,
				Max = client.Lobby.MaxMembers,
				Size = client.Lobby.NumMembers
			};

			if (currentPrivacyLevel == PrivacyLevel.Join)
			{
				RichPresence.Secrets = new Secrets()
				{
					JoinSecret = lobbyID.ToString()
				};
			}

			Client.SetPresence(RichPresence);
		}

		// Temporarily disable concommand until we can port to latest R2API
		// [ConCommand(commandName = "discord_privacy_level", flags  = ConVarFlags.None, helpText = "Set the privacy level for Discord (0 is disabled, 1 is presence, 2 is presence + join)")]
		private static void SetPrivacyLevel(ConCommandArgs args)
		{
			if (args.Count != 1)
			{
				Debug.LogError("discord_privacy_level accepts 1 parameter only");
				return;
			}

			int level;
			bool parse = int.TryParse(args[0], out level);

			if (parse)
            {
				currentPrivacyLevel = (PrivacyLevel)level; //unchecked
				return;
			}
			
			Debug.LogError("Failed to parse arg - must be integer value");
			
			// TODO - if disabled, clear presence
		}
	}
}
using RoR2;
using System;
using DiscordRichPresence.Utils;
using static DiscordRichPresence.DiscordRichPresencePlugin;

namespace DiscordRichPresence.Hooks
{
	public static class RoR2Hooks
	{
		public static void AddHooks()
		{
			On.RoR2.Run.OnServerBossAdded += Run_OnServerBossAdded;
			On.RoR2.Run.OnServerBossDefeated += Run_OnServerBossDefeated;
			On.RoR2.Run.BeginStage += Run_BeginStage;
			On.RoR2.CharacterMaster.OnBodyStart += CharacterMaster_OnBodyStart;
			On.RoR2.TeleporterInteraction.FixedUpdate += TeleporterInteraction_FixedUpdate;
			On.RoR2.EscapeSequenceController.BeginEscapeSequence += EscapeSequenceController_BeginEscapeSequence;
			On.RoR2.InfiniteTowerRun.BeginNextWave += InfiniteTowerRun_BeginNextWave;
			On.RoR2.UI.MainMenu.BaseMainMenuScreen.OnEnter += BaseMainMenuScreen_OnEnter;
		}

        public static void RemoveHooks()
		{
			On.RoR2.Run.OnServerBossAdded -= Run_OnServerBossAdded;
			On.RoR2.Run.OnServerBossDefeated -= Run_OnServerBossDefeated;
			On.RoR2.Run.BeginStage -= Run_BeginStage;
			On.RoR2.CharacterMaster.OnBodyStart -= CharacterMaster_OnBodyStart;
			On.RoR2.TeleporterInteraction.FixedUpdate -= TeleporterInteraction_FixedUpdate;
			On.RoR2.EscapeSequenceController.BeginEscapeSequence -= EscapeSequenceController_BeginEscapeSequence;
			On.RoR2.InfiniteTowerRun.BeginNextWave -= InfiniteTowerRun_BeginNextWave;
			On.RoR2.UI.MainMenu.BaseMainMenuScreen.OnEnter -= BaseMainMenuScreen_OnEnter;
		}

		private static void Run_OnServerBossDefeated(On.RoR2.Run.orig_OnServerBossDefeated orig, Run self, BossGroup bossGroup)
		{
			CurrentBoss = "";
			PresenceUtils.SetStagePresence(Client, RichPresence, CurrentScene, Run.instance);

			orig(self, bossGroup);
		}

		private static void Run_OnServerBossAdded(On.RoR2.Run.orig_OnServerBossAdded orig, Run self, BossGroup bossGroup, CharacterMaster characterMaster)
		{
			CurrentBoss = characterMaster.GetBody().GetDisplayName();
			PresenceUtils.SetStagePresence(Client, RichPresence, CurrentScene, Run.instance);

			orig(self, bossGroup, characterMaster);
		}

		// We use this method because it provides a robust update system
		// Additionally, comparing with CurrentChargeLevel prevents unnecessary presence updates (which would lead to ratelimiting)
		private static void TeleporterInteraction_FixedUpdate(On.RoR2.TeleporterInteraction.orig_FixedUpdate orig, TeleporterInteraction self)
		{
			if (Math.Round(self.chargeFraction, 2) != CurrentChargeLevel && PluginConfig.TeleporterStatusEntry.Value == TeleporterStatus.Charge)
			{
				CurrentChargeLevel = (float)Math.Round(self.chargeFraction, 2);
				PresenceUtils.SetStagePresence(Client, RichPresence, CurrentScene, Run.instance);
			}

			orig(self);
		}

		private static void CharacterMaster_OnBodyStart(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
		{
			RichPresence.Assets.SmallImageKey = InfoTextUtils.GetCharacterInternalName(LocalUserManager.GetFirstLocalUser().cachedMasterController.master.GetBody().GetDisplayName());
			RichPresence.Assets.SmallImageText = LocalUserManager.GetFirstLocalUser().cachedMasterController.master.GetBody().GetDisplayName();
			Client.SetPresence(RichPresence);

			orig(self, body);
		}

		private static void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
		{
			CurrentChargeLevel = 0;

			if (CurrentScene != null)
			{
				PresenceUtils.SetStagePresence(Client, RichPresence, CurrentScene, self);
			}

			orig(self);
		}

		private static void EscapeSequenceController_BeginEscapeSequence(On.RoR2.EscapeSequenceController.orig_BeginEscapeSequence orig, EscapeSequenceController self)
		{
			MoonDetonationController = self;
			PresenceUtils.SetStagePresence(Client, RichPresence, CurrentScene, Run.instance);

			orig(self);
		}

		private static void InfiniteTowerRun_BeginNextWave(On.RoR2.InfiniteTowerRun.orig_BeginNextWave orig, InfiniteTowerRun self)
		{
			PresenceUtils.SetStagePresence(Client, RichPresence, CurrentScene, self);

			orig(self);
		}

		private static void BaseMainMenuScreen_OnEnter(On.RoR2.UI.MainMenu.BaseMainMenuScreen.orig_OnEnter orig, RoR2.UI.MainMenu.BaseMainMenuScreen self, RoR2.UI.MainMenu.MainMenuController mainMenuController)
		{
			if (Facepunch.Steamworks.Client.Instance.Lobby.IsValid) // Messy if-else, but the goal is that when exiting a multiplayer game to the menu, it will display the lobby presence instead of the main menu presence
			{
				PresenceUtils.SetLobbyPresence(Client, RichPresence, Facepunch.Steamworks.Client.Instance);
			}
			else if (IsInEOSLobby(out EOSLobbyManager lobbyManager))
			{
				PresenceUtils.SetLobbyPresence(Client, RichPresence, lobbyManager);
			}
			else
			{
				PresenceUtils.SetMainMenuPresence(Client, RichPresence);
			}

			orig(self, mainMenuController);
		}
	}
}
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
	public static class PauseManagerHooks
	{
		public static void Initialize()
		{
			PauseManager.onPauseStartGlobal += OnGamePaused; // Workaround to pause time on RPC when in pause menu
			PauseManager.onPauseEndGlobal += OnGameUnPaused;
		}

		public static void Dispose()
        {
			PauseManager.onPauseStartGlobal -= OnGamePaused;
			PauseManager.onPauseEndGlobal -= OnGameUnPaused;
		}

		private static void OnGamePaused()
		{
			if (Run.instance != null)
			{
				if (DiscordRichPresencePlugin.Client.CurrentPresence != null)
				{
					SceneDef scene = SceneCatalog.GetSceneDefForCurrentScene();
					if (scene != null)
					{
						PresenceUtils.SetStagePresence(DiscordRichPresencePlugin.Client, DiscordRichPresencePlugin.RichPresence, scene, Run.instance, false, DiscordRichPresencePlugin.ShowCurrentBossEntry.Value);
					}
				}
			}
		}

		private static void OnGameUnPaused()
		{
			if (Run.instance != null)
			{
				if (DiscordRichPresencePlugin.Client.CurrentPresence != null)
				{
					SceneDef scene = SceneCatalog.GetSceneDefForCurrentScene();
					if (scene != null)
					{
						PresenceUtils.SetStagePresence(DiscordRichPresencePlugin.Client, DiscordRichPresencePlugin.RichPresence, scene, Run.instance, true, DiscordRichPresencePlugin.ShowCurrentBossEntry.Value);
					}
				}
			}
		}
	}
}
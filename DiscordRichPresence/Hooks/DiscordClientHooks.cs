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
using static DiscordRichPresence.Utils.InfoTextUtils;

namespace DiscordRichPresence.Hooks
{
	public static class DiscordClientHooks
	{
		public static void Initialize(DiscordRpcClient client)
		{
			// Subscribe to join events
			client.Subscribe(DiscordRPC.EventType.Join);
			client.Subscribe(DiscordRPC.EventType.JoinRequest);

			client.OnReady += Client_OnReady;
			client.OnError += Client_OnError;
			client.OnJoinRequested += Client_OnJoinRequested;
            client.OnJoin += Client_OnJoin;
        }

        public static void Dispose(DiscordRpcClient client)
        {
			client.Unsubscribe(DiscordRPC.EventType.Join);
			client.Unsubscribe(DiscordRPC.EventType.JoinRequest);

			client.OnReady -= Client_OnReady;
			client.OnError -= Client_OnError;
			client.OnJoinRequested -= Client_OnJoinRequested;
			client.OnJoin -= Client_OnJoin;
		}

		private static void Client_OnReady(object sender, ReadyMessage args)
		{
			DiscordRichPresencePlugin.LoggerEXT.LogInfo("Discord Rich Presence Ready - User: " + args.User.Username);

			if (DiscordRichPresencePlugin.Client != null && DiscordRichPresencePlugin.Client.IsInitialized)
			{
				DiscordRichPresencePlugin.RichPresence = new RichPresence()
				{
					Assets = new Assets()
					{
						LargeImageKey = "riskofrain2",
						LargeImageText = "Risk of Rain 2"
					},
					State = "Starting game..."
				};

				DiscordRichPresencePlugin.Client.SetPresence(DiscordRichPresencePlugin.RichPresence);
			}
		}

		private static void Client_OnError(object sender, ErrorMessage args)
		{
			DiscordRichPresencePlugin.LoggerEXT.LogError(args.Message);
			DiscordRichPresencePlugin.Dispose();
		}

		private static void Client_OnJoinRequested(object sender, JoinRequestMessage args)
		{
			DiscordRichPresencePlugin.LoggerEXT.LogInfo(string.Format("User {0} asked to join lobby", args.User.Username));
			Chat.AddMessage(string.Format("Discord user {0} has asked to join your game!", FormatTextStyleTag(args.User.Username, StyleTag.Event)));
			// Always let people into your game for now
			DiscordRpcClient senderClient = (DiscordRpcClient)sender;
			senderClient.Respond(args, true);
		}

		private static void Client_OnJoin(object sender, JoinMessage args) // Works, but is finnicky...not sure what the trigger is. Needs testing
		{
			DiscordRichPresencePlugin.LoggerEXT.LogInfo("Joining Game via Discord - Steam Lobby ID: " + args.Secret);
			ConCommandArgs conArgs = new ConCommandArgs
			{
				userArgs = new List<string>() { args.Secret },
			};

			SteamworksLobbyManager.GetFromPlatformSystems().JoinLobby(conArgs);
		}
	}
}
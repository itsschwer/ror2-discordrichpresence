/*using Discord;
using RoR2;
using System.Collections.Generic;
using DiscordRichPresence.Utils;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;
using static DiscordRichPresence.DiscordRichPresencePlugin;
using static DiscordRichPresence.Utils.InfoTextUtils;

namespace DiscordRichPresence.Hooks
{
    public static class DiscordClientHooks
    {
        public static void AddHooks(Discord.Discord client)
        {
            // Subscribe to join events
            //client.GetLobbyManager();
            
            client.Subscribe(EventType.Join);// im not sure abt these ,,..
            client.Subscribe(EventType.JoinRequest);

            client.OnReady -= Client_OnReady; // https://discord.com/developers/docs/developer-tools/game-sdk#openactivityinvite
            client.OnError -= Client_OnError; // i dunno if this one has a docs
            client.OnJoinRequested -= Client_OnJoinRequested; // https://discord.com/developers/docs/developer-tools/game-sdk#onactivityjoinrequest
            client.OnJoin -= Client_OnJoin; // https://discord.com/developers/docs/developer-tools/game-sdk#onactivityjoin
            
        }

        public static void RemoveHooks(Discord.Discord client)
        {
            client.Unsubscribe(EventType.Join);
            client.Unsubscribe(EventType.JoinRequest);

            client.OnReady -= Client_OnReady;
            client.OnError -= Client_OnError;
            client.OnJoinRequested -= Client_OnJoinRequested;
            client.OnJoin -= Client_OnJoin;
        }

        private static void Client_OnReady(object sender, ReadyMessage args)
        {
            LoggerEXT.LogInfo("Discord Rich Presence Ready - User: " + args.User.Username);

            if (Client != null && Client.IsInitialized)
            {
                DiscordRichPresencePlugin.RichPresence = new RichPresence()
                {
                    Assets = new Assets()
                    {
                        LargeImageKey = "riskofrain2",
                        LargeImageText = "DiscordRichPresence v" + Instance.Info.Metadata.Version
                    },
                    State = "Starting game...",
                    Secrets = new Secrets(),
                    Timestamps = new Timestamps()
                };

                Client.SetPresence(DiscordRichPresencePlugin.RichPresence);
            }
        }

        private static void Client_OnError(object sender, ErrorMessage args)
        {
            LoggerEXT.LogError(args.Message);
            Dispose();
        }

        private static void Client_OnJoinRequested(object sender, JoinRequestMessage args)
        {
            LoggerEXT.LogInfo(string.Format("User {0} asked to join lobby", args.User.Username));
            Chat.AddMessage(string.Format("Discord user {0} has asked to join your game!", FormatTextStyleTag(args.User.Username, InfoTextUtils.StyleTag.Event)));
            // Always let people into your game for now
            DiscordRpcClient senderClient = (DiscordRpcClient)sender;
            senderClient.Respond(args, true);
        }

        private static void Client_OnJoin(object sender, JoinMessage args) // Works, but is finnicky...not sure what the trigger is. Needs testing
        {
            if (Facepunch.Steamworks.Client.Instance.Lobby.IsValid)
            {
                LoggerEXT.LogInfo("Joining Game via Discord - Steam Lobby ID: " + args.Secret);
                ConCommandArgs conArgs = new ConCommandArgs
                {
                    userArgs = new List<string>() { args.Secret },
                };

                SteamworksLobbyManager.GetFromPlatformSystems().JoinLobby(conArgs);
            }
            else
            {
                LoggerEXT.LogInfo("Joining Game via Discord - EOS User ID: " + args.Secret);
                UserID.TryParse(args.Secret, out UserID usId);
                EOSLobbyManager.GetFromPlatformSystems().JoinLobby(usId);
            }
            //LoggerEXT.LogInfo("Joining Game via Discord - Steam Lobby ID: " + args.Secret);
			//ConCommandArgs conArgs = new ConCommandArgs
			//{
			//	userArgs = new List<string>() { args.Secret },
			//};

			//if (Facepunch.Steamworks.Client.Instance.Lobby.IsValid)
            //{
			//	SteamworksLobbyManager.GetFromPlatformSystems().JoinLobby(conArgs);
			//}
			//else
            //{
			//	EOSLobbyManager.GetFromPlatformSystems().JoinLobby();
            //}
        }
    }
}*/
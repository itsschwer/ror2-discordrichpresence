Adds Discord Rich Presence functionality to Risk of Rain 2.

## Features
- Displays the current stage's icon, name, and subtitle, the currently selected difficulty, the currently selected character, and the current run time.
- Has a unique presence for different menu states (currently idling in the menu, choosing a character, and reading the logbook).
- Allows invitation of users via Discord (requires both the sender and receiver to have the mod for this to work).
- Other unique presences depending on the game status.

A plethora of options are provided to fine-tune the presence settings, such as whether or not to show the currently fought boss, the teleporter charge status, include a custom menu idle message, and more. [Risk of Options](https://thunderstore.io/package/Rune580/Risk_Of_Options/) provides a nice UI for interacting with this! (Otherwise, it can be found at `BepInEx/config/com.cuno.discord.cfg`)

**Mod creators**: if you would like to add your map or character's image to the presence, contact me at the user found below.

If you experience any issues or have any suggestions, please contact me! I can be found on the RoR2 Modding Discord server, as `mikhailmcraft`.

## Images
Selecting a character

![Selecting a character](https://cdn.discordapp.com/attachments/697901894999474308/992475444295237692/unknown.png?ex=65f47f49&is=65e20a49&hm=fdb46abb3814c9e2c7f6d74dbfabc9b40a716d27ffa4b6bfac659707b67e5943&)

Playing in a stage

![Playing in a stage](https://cdn.discordapp.com/attachments/697901894999474308/992475537584963735/unknown.png?ex=65f47f5f&is=65e20a5f&hm=3141ffbc15e0974eabcd12d7dc5ed387039dfb4fcb820415ca2eed78aad872f0&)

Idling in a lobby

![Idling in a lobby](https://cdn.discordapp.com/attachments/697901894999474308/992475648675303445/unknown.png?ex=65f47f79&is=65e20a79&hm=7c4c038e1440d9209c64855fdf6bd20beee509d79f4ca62a3fa78932596d2591&)

Invitation via Discord

![Invitation via Discord](https://cdn.discordapp.com/attachments/697901894999474308/992476608474644570/unknown.png?ex=65f4805e&is=65e20b5e&hm=502b9a40926714d01fa5ad9a5885de21733a4960224932045595c5fda4f6a8f3&)

## Changelog

1.2.1
- Added Nemesis Enforcer, Nemesis Commando, Chirr, Executioner, An Arbiter, and Red Mist character icons
- Fix lunar detonation presence
- Add outro (credits) presence
- Fixed an issue where multiplayer would be broken if the user was using EOS
  - This mod's EOS support is still in testing, please let me know if you encounter any game- or mod-breaking issues
  - If you still encounter issues with multiplayer, please make sure you are signed into your Epic Games account if you have enabled crossplay -- this is not a mod issue!
- Fixed an issue where multiplayer clients would have their stage number behind by one
- Fixed an issue where Steam lobbies would update presence incorrectly after a user joins
- Fixed a few behind-the-scenes errors that did not affect gameplay

1.2.0
- Added Epic Online Services (EOS) support
  - Ensures lobby presence will work if crossplay is ON
  - May have issues, please let me know if you encounter any!
  - Discord invites and joins may not work while crossplay is enabled
- Added new, higher-resolution images for stages, as well as 7 modded character images (thanks Zan!)
- Fixed display images for Scorched Acres, Abyssal Depths, Void Locus, Planetarium, and Simulacrum
  - Simulacrum runs will now display the image of the current environment being simulated
- Fixed presence not updating from lobby to in-game if the user is a multiplayer client
  - By extension, this streamlines the character selection process for the small image and fixes it entirely on multiplayer clients
- Added a unique presence for the lunar detonation sequence on Commencement
- Unknown/custom characters will now display a question mark and the name of the character (if the character does not have an image in the rich presence database)
- Unknown/custom difficulties will now display the name of the difficulty instead of "Unknown"
- Fixed an issue where bosses would only update on the presence after pausing after the boss is spawned
- Fixed an issue where dying while a boss was alive and starting a new run would not update the presence
- Fixed an issue where exiting to the menu from a multiplayer game would cause the main menu presence to display, rather than the lobby presence
- Fixed an issue where the presence would reset to lobby when another player leaves in a multiplayer game
- Fixed an issue where the user could still receive join requests and send game invites after the run has started

1.1.0
- Added Risk of Options support, as a soft dependency
- Added Discord join support (BOTH the host and the person joining need the mod for this to work, this is very finnicky and only works some of the time)
- Updated Teleporter Charge to use enums instead of bytes

1.0.1
- (Re)Release

## Credits

- Some stage photos provided by [The Risk of Rain 2 Fandom Wiki](https://riskofrain2.fandom.com/wiki/Risk_of_Rain_2_Wiki) under [CC BY-NC-SA 3.0](https://www.fandom.com/licensing). Higher quality photos provided by Zan#1601.
- Initial repository code taken from [DarkKronicle's fork](https://github.com/DarkKronicle/RoR2-Discord-RP) of [WhelanB's repository](https://github.com/WhelanB/RoR2-Discord-RP) (if you are either of these users and have issue with me using your code, please do contact me!).
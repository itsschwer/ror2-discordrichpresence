Adds Discord Rich Presence functionality to Risk of Rain 2.

## Features
- Displays the current stage's icon, name, and subtitle, the currently selected difficulty, the currently selected character, and the current run time.
- Has a unique presence for different menu states (currently idling in the menu, choosing a character, and reading the logbook).
- Allows invitation of users via Discord (requires both the sender and receiver to have the mod for this to work).
- Other unique presences depending on the game status.

A plethora of options are provided to fine-tune the presence settings, such as whether or not to show the currently fought boss, the teleporter charge status, include a custom menu idle message, and more. [Risk of Options](https://thunderstore.io/package/Rune580/Risk_Of_Options/) provides a nice UI for interacting with this! (Otherwise, it can be found at `BepInEx/config/com.cuno.discord.cfg`)

**Mod creators**: if you would like to add your map or character's image to the presence, contact me at the user found below.

If you experience any issues or have any suggestions, please contact me! I can be found on the RoR2 Modding Discord server, at Cuno#9958.

## Images
Selecting a character

![Selecting a character](https://cdn.discordapp.com/attachments/697901894999474308/992475444295237692/unknown.png)

Playing in a stage

![Playing in a stage](https://cdn.discordapp.com/attachments/697901894999474308/992475537584963735/unknown.png)

Idling in a lobby

![Idling in a lobby](https://cdn.discordapp.com/attachments/697901894999474308/992475648675303445/unknown.png)

Invitation via Discord

![Invitation via Discord](https://cdn.discordapp.com/attachments/697901894999474308/992476608474644570/unknown.png)

## Changelog

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

- Stage photos are provided by [The Risk of Rain 2 Fandom Wiki](https://riskofrain2.fandom.com/wiki/Risk_of_Rain_2_Wiki) under [CC BY-NC-SA 3.0](https://www.fandom.com/licensing), and Zan#1601.
- Bomber, Chef, Miner, Paladin, HAN-D, Sniper, and Enforcer character images provided by Zan#1601.
- Initial repository code taken from [DarkKronicle's fork](https://github.com/DarkKronicle/RoR2-Discord-RP) of [WhelanB's repository](https://github.com/WhelanB/RoR2-Discord-RP) (if you are either of these users and have issue with me using your code, please do contact me!).
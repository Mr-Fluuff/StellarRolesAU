
<p align="center">
This mod is not affiliated with Among Us or Innersloth LLC, and the content contained therein is not endorsed or otherwise sponsored by Innersloth LLC. Portions of the materials contained herein are property of Innersloth LLC. © Innersloth LLC.</p>


# StellarRoles

**StellarRoles** is a mod for [Among Us](https://store.steampowered.com/app/945360/Among_Us) which adds new roles, new [Settings](#settings) and new [Custom Hats](#custom-hats) to the game.
Even more roles are coming soon. o.o

| Impostors | Crewmates | Neutral | Neutral Killers |
|----------|-------------|-----------------|----------------|
| [Wraith](#wraith)| [Mayor](#mayor) | [Arsonist](#arsonist) | [Nightmare](#nightmare)|
| [Cultist](#cultist)| [Engineer](#engineer) | [Romantic](#romantic) | [Pyromaniac](#pyromaniac)  |
| [Morphling](#morphling) | [Sheriff](#sheriff) | [Executior](#executioner) | [Head Hunter](#head-hunter) |
| [Camouflager](#camouflager) | [Jailor](#jailor) | [Scavenger](#scavenger) | [Rogue Imposters](#rogue-imposters)|
| [Vampire](#vampire) |[Parity Cop](#parity-cop) | [Vengeful Romantic](#vengeful-romantic) | [Ruthless Romantic](#ruthless-romantic)|
| [Shade](#shade)| [Detective](#detective) | [Beloved](#beloved) | |
| [Undertaker](#undertaker) | [Administrator](#administrator) | [Refugee](#refugee) | |
| [Janitor](#janitor) | [Medic](#medic) | [Jester](#jester) | |
| [Warlock](#warlock) | [Swapper](#swapper) |
| [Bounty Hunter](#bounty-hunter) | [Watcher](#watcher) |  |
| [Miner](#miner)| [Investigator](#investigator) |  |  |
| [Changeling](#changeling)| [Tracker](#tracker) |  |  |
| [Assassin](#assassin)|[Trapper](#trapper) |  |  |
| [Bomber](#bomber)| [Spy](#spy) |  |  |
|  | [Guardian](#guardian) |  |  |
|  | [Medium](#medium) |  |  |
|  | [Vigilante](#guesser) |  |

# Releases
| Among Us - Version| Mod Version | Link |
|----------|-------------|-----------------|
| 2023.03.28| v1.3.0| [Download](https://github.com/Mr-Fluuff/StellarRolesAU/releases/download/v1.3.0/StellarRoles.zip)
| 2023.02.28| v1.2.4 Dev H| [Download](https://github.com/Mr-Fluuff/StellarRolesAU/releases/download/v1.2.4/StellarRoles.zip)
| 2022.6.21, 2022.7.12| v1.0.5| [Download](https://github.com/Mr-Fluuff/StellarRolesAU/releases/download/v1.0.5/StellarRoles.zip)

# Changelog
  <details>
    **<summary>Version 1.3.0</summary>**
  
Bug Fixes/Tweaks:
- Refugee now displays properly in the in-game guide book
- End game screen now plays the proper noise based on which alignment won
- Scavenger button art fixed
- Assassin count now properly spawns the correct number of assassins
- If the lobby setting requires a specific number of tasks to complete an objected (Ex: Mayor seeing vote colors) is higher than the total number of tasks in the lobby, then completing tasks will earn that objective instead
- All guesser menus will now force close when a jail is applied to a player.
- The mini crewmate pet no longer randomly displays the wrong color.
- Pyromaniac douses are no longer interrupted by another player entering the pyromaniac's target highlight
- Scavenger should convert to Refugee at the correct number of players remaining(this time for sure)
- Administrator, Watcher, and Medic batteries should properly pause their charging during meetings when tasks are complete.

Role Changes:
- Administrator, Watcher, and Medic now trigger a 5 second cooldown when their mobile info button is initiated
- Non-Crewmates who have been tracked by a tracker now get a flash notification on a configurable delay.
- Non-Crewmates who have killed a player monitored by a medic now get a flash notification on a configurable delay.
- Non-Crewmates who have tripped a Watcher's sensor now get a flash notification on a configurable delay.
- Watcher now has an optional setting to make their sensor arrow anonymous.
- Shuffle is now a default ability of the Headhunter and is no longer optional.
- Cultist kill notification flashes are now red instead of purple.
- Vigilante can no longer be set as the Executioner's target.
- Several role encyclopedia entries have been fixed.
- Impostors and Neutral Killers can no longer be the target of an Assassin(can still be shot by Vigilante).
- Neutral Killers can now have the option to either inherit the Assassin power after impostors have all died or at the start of the game. This option can also be disabled.

Other Changes:
- Built-in lobby presets have been reworked, and now include the following presets: Streamer Prox, Streamer Non-Prox, Beginner, Chaotic, Stell's Lobby
how does that look
  </details>

# Installation

## Windows Installation Steam
1. Download the newest [release](https://github.com/Mr-Fluuff/StellarRolesAU/releases/latest)
2. Find the folder of your game. You can right click on the game in your library, a menu will appear, click on properties, local data, browse
3. Go back one folder to common and make a copy of your Among Us game folder and paste it somewhere on the same drive.
4. Now unzip and drag or extract the files from the .zip into your Among Us game folder that you just copied, at the `.exe` level (just into the folder).
5. Run the game by starting the .exe from this folder (the first launch might take a while).

Not working? You might want to install the dependency [vc_redist](https://aka.ms/vs/16/release/vc_redist.x86.exe)

## Windows Installation Epic
1. Download the newest [release](https://github.com/Mr-Fluuff/StellarRolesAU/releases/latest)
2. Find the folder of your game. Should be stored in "Epic/AmongUs" (wherever you installed Epic on your PC)
3. Now unzip and drag or extract the files from the .zip into the original Epic Among Us game folder.
4. Run the game by starting the game in your Epic Games launcher (the first launch might take a while).

Not working? You might want to install the dependency [vc_redist](https://aka.ms/vs/16/release/vc_redist.x86.exe)

![Install](https://i.imgur.com/pvBAyZN.png)

## Linux Installation
1. Install Among Us via Steam
2. Download newest [release](https://github.com/Mr-Fluuff/StellarRolesAU/releases/latest) and extract it to ~/.steam/steam/steamapps/common/Among Us
3. Enable `winhttp.dll` via the proton winecfg (https://docs.bepinex.dev/articles/advanced/steam_interop.html#open-winecfg-for-the-target-game)
4. Launch the game via Steam

## Custom Servers
**A custom server is not necessary and official servers are working just fine with the mod, but in case you want to set up and host your own server, here's a guide for you to follow.**

**Setup the Server:**
1. Get the Impostor release (https://github.com/Impostor/Impostor)
2. Follow the steps (using the server release you just downloaded) on the official Impostor-Documentation (https://github.com/Impostor/Impostor/wiki/Running-the-server)
3. Make sure to set the following values to false in the config.json file:
```    ...
     'AntiCheat': {
       'Enabled': false,
      'BanIpFromGame': false
    }
```
4. Make sure to forward the right ports on the hosting machine.
5. Run the server and setup the client.
Setting up Server as Docker Container:
If you want to run the server as a docker container you'll need to use the image
aeonlucid/impostor:nightly

(Currently only the 'nightly' tag is starting a server supporting 2021.3.31 or later)
In addition to running it we need to set the environment variables to disable the AntiCheat feature.
IMPOSTOR_AntiCheatEnabled=false
IMPOSTOR_AntiCheatBanIpFromGame=false

Example to docker run command:
docker run -p 22023:22023/udp --env IMPOSTOR_AntiCheatEnabled=false --env IMPOSTOR_AntiCheatBanIpFromGame=false aeonlucid/impostor:nightly

Or use to run it in the background
docker run -d -p 22023:22023/udp --env IMPOSTOR_AntiCheatEnabled=false --env IMPOSTOR_AntiCheatBanIpFromGame=false aeonlucid/impostor:nightly


# Credits & Resources
[OxygenFilter](https://github.com/NuclearPowered/Reactor.OxygenFilter) - For all the version v2.3.0 to v2.6.1, we were using the OxygenFilter for automatic deobfuscation\
[Reactor](https://github.com/NuclearPowered/Reactor) - The framework used for all version before v2.0.0\
[BepInEx](https://github.com/BepInEx) - Used to hook game functions\
[Essentials](https://github.com/DorCoMaNdO/Reactor-Essentials) - Custom game options by **DorCoMaNdO**:
- Before v1.6: We used the default Essentials release
- v1.6-v1.8: We slightly changed the default Essentials. The changes can be found on this [branch](https://github.com/Eisbison/Reactor-Essentials/tree/feature/TheOtherRoles-Adaption) of our fork.
- v2.0.0 and later: As we're not using Reactor anymore, we are using our own implementation, inspired by the one from **DorCoMaNdO**

[Jackal and Sidekick](https://www.twitch.tv/dhalucard) - Original idea for the Jackal and Sidekick comes from **Dhalucard**\
[Among-Us-Love-Couple-Mod](https://github.com/Woodi-dev/Among-Us-Love-Couple-Mod) - Idea for the Lovers role comes from **Woodi-dev**\
[Jester](https://github.com/Maartii/Jester) - Idea for the Jester role comes from **Maartii**\
[ExtraRolesAmongUs](https://github.com/NotHunter101/ExtraRolesAmongUs) - Idea for the Engineer and Medic role comes from **NotHunter101**. Also some code snippets come of the implementation were used.\
[Among-Us-Sheriff-Mod](https://github.com/Woodi-dev/Among-Us-Sheriff-Mod) - Idea for the Sheriff role comes from **Woodi-dev**\
[TooManyRolesMods](https://github.com/Hardel-DW/TooManyRolesMods) - Idea for the Detective role comes from **Hardel-DW**. Also some code snippets of the implementation were used.\
[TownOfUs](https://github.com/slushiegoose/Town-Of-Us) - Idea for the Swapper, Arsonist, and a similar Mayor role come from **Slushiegoose**\
[Ottomated](https://twitter.com/ottomated_) - Idea for the Morphling, Snitch and Camouflager role come from **Ottomated**\
[Goose-Goose-Duck](https://store.steampowered.com/app/1568590/Goose_Goose_Duck) - Idea for the Vulture role come from **Slushiegoose**
[TheOtherRoles](https://github.com/TheOtherRolesAU/TheOtherRoles) - Using Custom Hats for now from them

# Settings
The mod adds a few new settings to Among Us (in addition to the role settings):

### Task Count Limits per map
You can configure:
- Up to 4 common tasks
- Up to 23 short tasks
- Up to 15 long tasks

Please note, that if the configured option exceeds the available number of tasks of a map, the tasks will be limited to that number of tasks. \
Example: If you configure 4 common tasks on Airship crewmates will only receive 2 common tasks, as airship doesn't offer more than 2 common tasks.

| Map | Common Tasks | Short Tasks | Long Tasks |
|----------|:-------------:|:-------------:|:-------------:|
| Skeld / Dleks | 2 | 19 | 8
| Mira HQ | 2 | 13 | 11
| Polus | 4 | 14 | 15
| Airship | 2 | 23 | 15
-----------------------


# Custom Hats
## Create and submit new hat designs
We're awaiting your creative hat designs and we'll integrate all the good ones in our mod.
Here are a few instructions, on how to create a custom hat:

- **Creation:** A hat consists of up to three textures. The aspect ratio of the textures has to be `4:5`, we recommend `300px:375px`:
  - `Main texture (required)`:
    - This is the main texture of your hat. It will usually be rendered in front of the player, if you set the `behind` parameter it will be rendered behind the player.
    - The name of the texture needs to follow the pattern *hatname.png*, but you can also set some additional parameters in the file name by adding `_parametername` to the file name (before the *.png*).
    - Parameter `bounce`: This parameter determines whether the hat will bounce while you're walking or not.
    - Parameter `adaptive`: If this parameter is set, the Among Us coloring shader will be applied (the shader that replaces some colors with the colors that your character is wearing in the game). The color red (#ff0000) will be replaced with the primary color of your player and the color blue (#0000ff) with the secondary color. Also other colors will be affected and changed, you can have a look at the texture of the [Crewmate Hat](https://static.wikia.nocookie.net/among-us-wiki/images/e/e0/Crewmate_hat.png) to see how this feature should be used.
    - Parameter `behind`: If this parameter is set, the main texture will be rendered behind the player.
  - `Flipped texture (optional)`:
    - This texture will be rendered instead of the Main texture, when facing the left.
    - The name of the texture needs to follow the pattern `hatname_flip.png`.
  - `Back texture (optional)`:
    - This texture will be rendered behind the player.
    - The name of the texture needs to follow the pattern `hatname_back.png`.
  - `Flipped Back texture (optional)`:
    - This texture will be rendered instead of the Back texture, when facing the left.
    - The name of the texture needs to follow the pattern `hatname_back_flip.png`.
  - `Climb texture (optional)`:
    - This texture will be rendered in front of the player, when they're climbing.
    - The name of the texture needs to follow the pattern `hatname_climb.png`.
- **Testing:** You can test your hat design by putting all the files in the `\TheOtherHats\Test` subfolder of your mod folder. Then whenever you start a Freeplay game, you and all the dummies will be wearing the new hat. You don't need to restart Among Us if you change the hat files, just exit and reenter the Freeplay mode.

- **Submission:** If you got a hat design, you can submit it on our [Discord](https://discord.gg/tKNVCXsP). We'll look at all the hats and add all the good ones to the game.

# Colors
![Stellar Colors](./Images/TOR_colors.jpg)

# Roles

## Role Assignment

First you need to choose how many special roles of each kind (Impostor/Neutral/Crewmate) you want in the game.
The count you set will only be reached, if there are enough Crewmates/Impostors in the game and if enough roles are set to be in the game (i.e. they are set to > 0%). The roles are then being distributed as follows:
- First all roles that are set to 100% are being assigned to arbitrary players.
- After that each role that has 10%-90% selected adds 1-9 tickets to a ticket pool (there exists a ticket pool for Crewmates, Neutrals and Impostors). Then the roles will be selected randomly from the pools as long it's possible (until the selected number is reached, until there are no more Crewmates/Impostors or until there are no more tickets). If a role is selected from the pool, obviously all the tickets of that role are being removed.

**Example:**\
Settings: 2 special Crewmate roles, Tracker: 100%, Watcher: 10%, Detective: 30%\
Result: Tracker is assigned, then one role out of the pool [Watcher, Detective, Detective, Detective] is being selected\
Note: Changing the settings to Watcher: 20%, Detective: 60% would statistically result in the same outcome .

## Changeling
### **Team: Imposters**
The Changeling is a 'choose your fighter'-style impostor role who behaves exactly like a vanilla impostor until they use their one-time use ability.\
\
**NOTE:**
- This ability will open up a menu with a list of all impostor roles with >0% spawn rate, excluding cultist and the role of their impostor partner(s). This menu will also respect roles that are exclusive of each other; for example: if the partner is a warlock, then both warlock and vampire will not be on the list. Selecting the role will permanently change the player into it. This can only be done once, and cannot be undone.
- The host can decide the power level of the Changeling through its ability to vent or increasing its kill cooldown before it changes.
- Ability cooldowns are set to 50% of their base cooldown upon changing.
- The kill cooldown will continue to count as normal upon changing.
- The Changeling can be guessed, but must be guessed as the proper role after they change.
- To prevent discouragement from late-game picks for roles that place objects: For every dead player in the game, miner vents and trickster boxes cooldowns are reduced by 15% (If this number is >=100%, objects will not have a cooldown). This effect is static at the time the changeling changes, meaning cooldowns are not reduced further after they change into the role.

### Game Options
| Name | Description |
|----------|:-------------:|
| Changeling Spawn Chance | -
-----------------------

## Camouflager
### **Team: Impostors**
The Camouflager is an Impostor which can additionally activate a camouflage mode.\
The camouflage mode lasts for 10s and while it is active, all player names/pets/hats\
are hidden and all players have the same color.\
\
**NOTE:**
- The Mini/Giant will look like all the other players
- The color of the footprints turns gray (also the ones that were already on the ground).
- The shield is not visible anymore
- Tracker arrows keep working

### Game Options
| Name | Description |
|----------|:-------------:|
| Camouflager Spawn Chance | -
| Camouflager Cooldown | -
| Camo Duration | Time players stay camouflaged
-----------------------

## Morphling
### **Team: Impostors**
The Morphling is an Impostor which can additionally scan the appearance of a player. After an arbitrary time they can take on that appearance for 10s.\
\
**NOTE:**
- They shrink to the size of the Mini and Grow to the size of the Giant when they copy their look.
- The Hacker sees the new color on the admin table.
- The color of the footprints changes accordingly (also the ones that were already on the ground).
- The other Impostor still sees that they are an Impostor (the name remains red).
- The shield indicator changes accordingly (the Morphling gains or loses the shield indicator).
- Tracker and Snitch arrows keep working.

### Game Options
| Name | Description |
|----------|:-------------:|
| Morphling Spawn Chance | -
| Morphling Cooldown | -
| Morph Duration | Time the Morphling stays morphed
-----------------------

## Vampire
### **Team: Impostors**
The Vampire is an Impostor, that can bite other player. Bitten players die after a configurable amount of time.\
If the Vampire spawn chance is greater 0 (even if there is no Vampire in the game), all players can place one garlic.\
If a victim is near a garlic, the "Bite Button" turns into the default "Kill Button" and the Vampire can only perform a normal kill.\
\
**NOTE:**
- If a bitten player is still alive when a meeting is being called, they die at the start of the meeting.
- The cooldown is the same as the default kill cooldown (+ the kill delay if the Vampire bites the target).
- If there is a Vampire in the game, there can't be a Warlock.

### Game Options
| Name | Description |
|----------|:-------------:|
| Vampire Spawn Chance | -
| Vampire Kill Delay | -
| Vampire Cooldown | Sets the kill/bite cooldown
| Vampire Can Kill Near Garlics | The Vampire can never bite when their victim is near a garlic. If this option is set to true, they can still perform a normal kill there.
-----------------------

## Janitor
### **Team: Impostors**
The Janitor is an Impostor who has the ability to clean up dead bodies.\
\
**NOTE:**
- The Kill and Clean cooldown are shared, preventing them from immediately cleaning their own kills.
- If there is a Janitor in the game, there can't be a Vulture.

### Game Options
| Name | Description |
|----------|:-------------:|
| Janitor Spawn Chance | -
| Clean Cooldown | Cooldown for cleaning dead bodies
-----------------------

## Warlock
### **Team: Impostors**
The Warlock is an Impostor, that can curse another player (the cursed player doesn't get notified).\
If the cursed person stands next to another player, the Warlock is able to kill that player (no matter how far away they are).\
Performing a kill with the help of a cursed player, will lift the curse and it will result in the Warlock being unable to move for a configurable amount of time.\
The Warlock can still perform normal kills, but the two buttons share the same cooldown.\
\
**NOTE:**
- The Warlock can always kill their Impostor mates (and even themself) using the "cursed kill"
- If there is a Warlock in the game, there can't be a Vampire
- Performing a normal kill, doesn't lift the curse

### Game Options
| Name | Description |
|----------|:-------------:|
| Warlock Spawn Chance | -
| Warlock Cooldown | Cooldown for using the Curse and curse Kill
| Warlock Root Time | Time the Warlock is rooted in place after killing using the curse
-----------------------

## Bounty Hunter
### **Team: Impostors**
\
The Bounty Hunter is an Impostor, that continuously get bounties (the targeted player doesn't get notified).\
The target of the Bounty Hunter swaps after every meeting and after a configurable amount of time.\
If the Bounty Hunter kills their target, their kill cooldown will be a lot less than usual.\
Killing a player that's not their current target results in an increased kill cooldown.\
Depending on the options, there'll be an arrow pointing towards the current target.\
\
**NOTE:**
- The target won't be an Impostor, a Spy or the Bounty Hunter's Lover.
- Killing the target resets the timer and a new target will be selected.

### Game Options
| Name | Description |
|----------|:-------------:|
| Bounty Hunter Spawn Chance | -
| Duration After Which Bounty Changes | -
| Cooldown After Killing Bounty | -
| Additional Cooldown After Killing Others | Time will be added to the normal impostor cooldown if the Bounty Hunter kills a not-bounty player
| Show Arrow Pointing Towards The Bounty | If set to true an arrow will appear (only visiable for the Bounty Hunter)
| Bounty Hunter Arrow Update Interval | Sets how often the position is being updated
-----------------------

## Undertaker
### **Team: Impostors**
The Undertaker is an Impostor, that can drag the bodies of Crewmates.\

**NOTE:**
- Placing a body at the edge of the map is still reportable for Everyone, but sometimes is not reportable for the Undertaker.
- Kill Cooldown will pause when dragging a body.

### Game Options
| Name | Description |
|----------|:-------------:|
| Undertaker Spawn Chance | -
| Undertaker Drag After Kill Cooldown | -
| Can Vent While Dragging Body | -
| Drag Cooldown | -
-----------------------

## Miner
### **Team: Impostors**
The Miner is an Impostor, that can place vents around the map.\

**NOTE:**
- Placing a vent will not be usable/visible until next round.

### Game Options
| Name | Description |
|----------|:-------------:|
| Miner Spawn Chance | -
| Mine Cooldown | -
-----------------------

## Wraith
### **Team: Impostors**
The Wraith is an impostor role with two abilities. The first ability, Dash, increases the movement speed of the Wraith for a few seconds on a cooldown. The second ability is a Lantern. The Wraith can place the Lantern on the ground, and reactivate it to teleport to it. The Lantern has a short, limited duration, and is only visible to the Wraith. Failing to reactivate a Lantern will leave a broken lantern on the ground permanently that is visible to all players.\
\
**Abilities**
- Phase: Increases player movement speed -potential balance check if needed: canceled on kill/vent use
- Lantern: Places a Lantern on the ground that can be teleported to later. Leaves permanent evidence if unused before expiration.
- Optional: Teleporting to the lantern grants temporary Invisibility.
### Game Options
| Name | Description |
|----------|:-------------:|
| Wraith Spawn Chance | -
| Dash Cooldown | -
| Dash Duration | - 
| Dash Speed Multiplier | -
| Lantern | Enable Lantern Ability
| Lantern Cooldown| -
| Latern Duration| -
| Invisibility| -
| Invisibility Duration | Time players stays invisible after using lantern
-----------------------

## Shade
### **Team: Impostors**
The Shade is an impostor who can temporarily turn invisible. Upon scoring X kills, The Shade gains a 'Blind' Ability. Evidence of when a stealth turns invisible, and when they reappear is configurable. Pressing stealth again prematurely will cancel the effect.\
\
### Game Options
| Name | Description |
|----------|:-------------:|
| Shade Spawn Chance | -
| Vanish Cooldown | -
| Vanish Duration | -
| Evidence Duration| -
| Blind Button | -
| Blind Duration |
-----------------------

## Cultist
### **Team: Impostors**
The Cultist is an Impostor, that can convert a crewmate to a Follower.\

**NOTE:**
- Cultist and Follower get a chat box mid round.
- Cultist gets an arrow pointing to their Follower at all times.
- Cultist gets a red flash when the Follower Kills.
- When a Cultist Spawns, it will spawn as a solo Impostor.

### Game Options
| Name | Description |
|----------|:-------------:|
| Cultist Spawn Chance | -
| Exe/Cultist | Executioner Can Spawn along side Cultist?
-----------------------

## Follower
### **Team: Impostors**
The Follower is an Impostor, that was converted by the Cultist.\

**NOTE:**
- Follower and Cultist get a chat box mid round.
- Follower gets an arrow pointing to the Cultist at all times.
- Follower gets a red flash when the Cultist Kills.
-----------------------

## Jester
### **Team: Neutral**
The Jester does not have any tasks. They win the game as a solo, if they get voted out during a meeting.

### Game Options
| Name | Description |
|----------|:-------------:|
| Jester Spawn Chance | -
| Jester Can Call Emergency Meeting | Option to disable the emergency button for the Jester
-----------------------

## Arsonist
### **Team: Neutral**
The Arsonist does not have any tasks, they have to win the game as a solo.\
The Arsonist can douse other players by pressing the douse button and remaining next to the player for a few seconds.\
If the player that the Arsonist douses walks out of range, the cooldown will reset to 0.\
After dousing everyone alive the Arsonist can ignite all the players which results in an Arsonist win.

### Game Options
| Name | Description |
|----------|:-------------:|
| Arsonist Spawn Chance | -
| Arsonist Countdown | -
| Arsonist Douse Duration | The time it takes to douse a player
-----------------------

## Executioner
### **Team: Neutral**
The Executioner is someone that is assigned a Crewmate. Their goal is to get that Crewmate voted out at all costs.\

### Game Options
| Name | Description |
|----------|:-------------:|
| Executioner Spawn Chance | -
| Executioner Becomes on Target Death | Jester/Pursuer
| Executioner Becomes on Target Conversion | Lawyer/Jester
-----------------------

## Scavenger
### **Team: Neutral**

The Scavenger does not have any tasks, they have to win the game as a solo.\
The Scavenger is a neutral role that must eat a specified number of corpses (depending on the options) in order to win.\
Depending on the options, when a player dies, the Scavenger gets an arrow pointing to the corpse.\
If there is a Scavenger in the game, there can't be a Janitor. If the Scavenger's win condition can no longer be met, they
become a Refugee.\
\
**Abilities:**
- Scavenge: Opens a time window where the Scavenger has arrows pointing to all corpses currently on the map.
- Eat: Consumes a corpse, causing it to disappear and no longer be reportable. The body will show as a disconnect on vitals, and will not be displayed on a report screen at the start of a meeting.

**NOTE**
- If the corpse is on a different floor on Submerged, the arrow will always point to the elevator

### Game Options
| Name | Description |
|----------|:-------------:|
| Scavenger Spawn Chance | -
| Eat Countdown | -
| Number Of Eats to Win | Corpes needed to be eaten to win the game
| Scavenger Can Use Vents | -
| Scavenge Cooldown | -
| Scavenge Duration | How long Scavenger will have arrows pointing to dead bodies
-----------------------

## Vigilante
### **Team: Crewmates**
The Vigilante can shoot players during the meeting, by guessing its role. If the guess is wrong, the Vigilante dies instead.\
You can select how many players can be shot per game and if multiple players can be shot during a single meeting.\
The guesses Impostor and Crewmate are only right, if the player is part of the corresponding team and has no special role.\
You can only shoot during the voting time.\

**NOTE:**
- If a player gets shot, you'll get back your votes
- Jester wins won't be triggered, if the Guesser shoots the Jester before the Jester gets voted out

### Game Options
| Name | Description |
|----------|:-------------:|
| Vigilante Spawn Chance | -
| Number Of Shots Per Game | -
| Can Shoot Multiple Times Per Meeting |  -
-----------------------

## Romantic
### **Team: Neutral**
The Romantic is role who Declares their love to any player. When they do this, their alignment becomes tied to them. They will win if their chosen target does, with a couple minor exceptions.
- The Romantic will win with the crewmates or with neutrals if they are killed or assassinated, and can still win with crew or neutrals if they are voted out(granted the proper alignment has won the game).\
\
**Abilities**
- The Romantic Declare button chooses your selected love. This button is replaced with Shield when a target is chosen. There is no indicator on the target's end that they are selected.
- Shield will protect your target for a short duration, causing the first attack made against them to fail and put the attacker's kill on a five second cooldown.
- The shield is ONLY visible to the Romantic, no one else.
-If at any point your selected target is killed or assassinated, your role will immediately change into the "Vengeful Romantic" role.

### Game Options
| Name | Description |
|----------|:-------------:|
| Romantic Spawn Chance | -
| Protect Countdown | -
| Protect Duration | -
| Romantic Knows Target Identity | -
| Romantic Target Knows they are selected | -
| Display Meeting Indicators | -
-----------------------

## Vengeful-Romantic
### **Team: Neutral**
The Vengeful Romantic was once a romantic to a crewmate. Now that their love has died they must take their revenge! Their protect button now becomes an avenge button. Be careful tho as it will only work on the one who took your love away from you.

-The Vengeful Romantic CAN be assassinated
-The Vengeful Romantic's 'Shield' button is replaced with a 'Kill' button.
-This kill button behaves exactly like a sheriff's kill button but instead of misfiring on crewmates, this kill will misfire on everyone but the person who killed or assassinated your love. It starts on cooldown, matching base impostor cooldown.

## Ruthless-Romantic
### **Team: Neutral Killer**
The Ruthless Romantic is a neutral killing role that was once the romantic of another neutral killer.
Their objective now is to kill all crew and be the last killer standing.

**Notes**
- The Lover of the Ruthless Romantic will win with them.

## Beloved
### **Team: Neutral**

## Head-Hunter
### **Team: Neutral Killer**
The headhunter is a neutral killing role who has three assigned kill targets every round.
These names are displayed to the headhunter in an alternate color during rounds and meetings.\
\
**Notes**
- The headhunter cannot target impostors or players who were dead first round of the previous game as its first three targets.
- The headhunter can never target more than one impostor at one time, and cannot target a solo impostor unless there are not enough players remaining or the impostor was already a target.
- If any of the headhunter's targets are killed, the headhunter will get new targets at the start of the next meeting.
- The headhunter has impostor vision and can vent. The Headhunter starts the game with a kill cooldown 5s higher than impostors.
- Every time the Headhunter kills a target, its kill cooldown is reduced by 5s. This effect is permanent.
- Every time the Headhunter kills a non-target, its kill cooldown is increased by 10s. This effect is permanent.
- The headhunter has a "Pursue" button to get arrows pointing to their targets for a set duration. This arrow will point to the targets death location in the event a target has been killed. The arrow will remain at the death location, and will not follow a corpse in the event the body is displaced.


### Game Options
| Name | Description |
|----------|:-------------:|
| Head Hunter Spawn Chance | -
| Pursue Countdown | -
| Pursue Duration | -
| Can Target Imposters Round One | -
| One Time Target Shuffle | -
-----------------------

## Nightmare
### **Team: Neutral Killers**
Nightmare plays into a players worst nightmares! They have two abilities, Blind and Paralyze! 

### Game Options
| Name | Description |
|----------|:-------------:|
| Nightmare Spawn Chance | -
| Paralyze Countdown | -
| Paralyze Duration | -
| Blind Cooldown | -
| Blind Duration | -
-----------------------

## Pyromaniac
### **Team: Neutral Killers**

### Game Options
| Name | Description |
|----------|:-------------:|
| Douse Cooldown | -
| Douse Duration | -
| Doused Kill Cooldown | -
| Douses Display In Meetings | -
-----------------------


## Rogue-Imposters
### **Team: Neutral Killers**

Option to turn any imposter role into a Neutral Killer Role. You can enable each role as a neutral killer, and 
it will use the spawn chance under the imposter settings. It cannot spawn as an imposter if enabled for Neutral Killer. 

## Mayor
### **Team: Crewmates**
The Mayor leads the Crewmates by having a vote that counts twice.\
The Mayor can always use their meeting, even if the maximum number of meetings was reached.\
The Mayor has a portable Meeting Button, depending on the options.\
The Mayor can see the vote colors after completing a configurable amount of tasks, depending on the options.

### Game Options
| Name | Description |
|----------|:-------------:|
| Mayor Spawn Chance | -
| Mayor Can See Vote Colors | -
| Completed Tasks Needed To See Vote Colors | -
-----------------------

## Parity-Cop
### **Team: Crewmates**
**WARNING: This role is one of the most complex roles in the mod, and should only be used in advanced lobbies!**
The Parity Cop is a role that can select two players during rounds. In meetings these players will be compared to one another, to see if they have the same alignment or not! Players who are killed will not be compared. The two most recently selected living players will be compared during the next meeting. The Parity Cop may use this button as much as they want per round.\
\
**Notes**
- There are two categories that all players fit into: Crewmates, and Non-Crewmates.
- In meeting, players with matching alignments will be displayed with checkmarks!
- Mismatched players will be displayed with X's. Dead players may also see these comparisons! If the Parity Cop is killed, their final comparison will still be made! 
- When any Non-Crewmate role is put in a comparison with another player, they will be made aware of it in meeting with a ? next to their name, and next to the name of the person they were compared to. Crewmate roles do not see these indicators! 
- The host may decide to increase the difficulty of this role. There are two settings to place Neutral-Evil roles in the Crewmate category, or Crewmate-Killing roles(Sheriff and Vigilante) in the Non-Crewmate category! This setting is generally used for more advanced lobbies, or as a power-check to this strong role!
- The host may also enable an ability called Fake Out, though this is also recommended for advanced lobbies. Fake out will force the Parity Cop to the top of their running comparison list. If the most recent person they have compared is a Non-Crewmate, they will see a standard comparison has been made to them with the parity cop!
- Fake Out provides no information to the Parity Cop. It is simply a way for the Parity Cop to keep their role from being singled out, as they cannot compare themselves normally.

### Game Options
| Name | Description |
|----------|:-------------:|
| Neutral Evil Groups Compared With Crew | -
| Crewmate Killers Grouped With Crew | -
| Compare Cooldown | -
| Enable Fakeout | -
-----------------------

## Mayor
### **Team: Crewmates**
The Mayor leads the Crewmates by having a vote that counts twice.\
The Mayor can always use their meeting, even if the maximum number of meetings was reached.\
The Mayor has a portable Meeting Button, depending on the options.\
The Mayor can see the vote colors after completing a configurable amount of tasks, depending on the options.

### Game Options
| Name | Description |
|----------|:-------------:|
| Mayor Spawn Chance | -
| Mayor Can See Vote Colors | -
| Completed Tasks Needed To See Vote Colors | -
| Mobile Emergency Button | -
-----------------------

## Administrator
### **Team: Crewmates**
The Administrator is another simple role with one power: It can access the Admin Table from anywhere on the map! 
Like the Medic and the Watcher, the Administrator uses a Task-Powered-Battery to power its mobile Admin Table! The Task Battery gains a host-configured amount of charge whenever the Administrator does tasks! It costs 1 second of charge to open your admin table, and the power of the battery drains over time while it is open.\
\
**Notes**
- When you complete your tasks, the charge of the battery generates on its own! The host can configure an internal cooldown for this charge increase, though the default cooldown is 20 seconds.
- The host may choose to disable battery charging and access to your mobile power in the first round of the game. This should not discourage you from doing tasks on round one, as when you finish tasks the battery will charge on its own anyway. 

### Game Options
| Name | Description |
|----------|:-------------:|
| Administrator Spawn Chance | -
| Initial Battery Time | -
| Battery Time Per Task | -
| Self Charging Battery Cooldown| -
| Self Charging Battery Duration | -
| Disable Round One Mobile Access | -
| Turn Off Administrator On Mira | -
-----------------------

## Watcher
### **Team: Crewmates**
The Watcher is a role with two abilities: Sensors and map-wide camera access!
Sensors have configurable charges. These charges are refreshed every round. Placed sensors are also cleared every round.
When the watcher places a sensor on the ground, the next player to stand next to it will alert the Watcher with a flash and an arrow pointing to which sensor was tripped. The arrow is the color of the player who tripped it by default, but the host can make this color anonymous.\
\
**Notes**
- Like the Medic and the Administrator, the Watcher uses a Task-Powered-Battery to power its mobile cameras! The Task Battery gains a host-configured amount of charge whenever the Watcher does tasks! It costs 1 second of charge to open your cameras, and the power of the battery drains over time while it is open.
- When you complete your tasks, the charge of the battery generates on its own! The host can configure an internal cooldown for this charge increase, though the default cooldown is 20 seconds.
- The host may choose to disable battery charging and access to your mobile power in the first round of the game. This should not discourage you from doing tasks on round one, as when you finish tasks the battery will charge on its own anyway.

### Game Options
| Name | Description |
|----------|:-------------:|
| Watcher Spawn Chance | -
| Number of Sensors Per Round | -
| Enable Meeting Overlay | -
| Anonymous Arrows | -
| Initial Battery Time | -
| Battery Time Per Task | -
| Self Charging Battery Cooldown| -
| Self Charging Battery Duration | -
| Disable Round One Mobile Access | -
| Turn Off Watcher On Skeld | -
-----------------------

## Trapper
### **Team: Crewmates**

The Trapper is a crewmate role that is centered around hindering the ability to use vents. It has two main abilities. Using one of these abilities will trigger the cooldown of the other. The first ability, Cover, is usable when the Trapper is near a vent. This will cover the vent after the next meeting. The second ability, Trap, is also usable when near a vent. This ability will trap the vent, rooting players who attempt to use it in place for a SHORT duration(intended to be ~3 seconds). When triggered, the trap is consumed and the trapper is notified with a flash. Traps are only visible to players who have been in close proximity to them for 3 seconds. Traps and Covers are NOT visible in fog of war. The trapper is aware of when a player is in a specific vent system with a red highlight(this includes the engineer/jester).\
\
**Abilities**
- Trap Vents - Roots people who use a trapped vent and notify the trapper. Traps are consumed when triggered. Traps are permanently visible after 3s of proximity. Placing a Trap triggers Cover cooldown. Traps are active as soon as they are placed. Rooted players are unable to move or take actions.
- Cover Vents - Permanently seals a vent. Placing a Cover triggers Trap cooldown. Covers are not visible in fog of war. Covers activate after the next meeting.

### Game Options
| Name | Description
|----------|:-------------:|
| Trapper Spawn Chance | -
| Number of Traps| -
| Number of Covers | -
| Trap/Cover Cooldown | -
| Trap Root Duration | -
-----------------------

## Refugee
### **Team: Crewmates**

The Refugee is a crewmate that was once another role that has lost its win condition but has gained a second chance to win. Refugee can only win with crew.

**Abilities**
- Refuge - Protects the Refugee from harm for a configurable duration.

### Game Options
| Name | Description
|----------|:-------------:|
| Refuge Cooldown | -
| Refuge Duration | -
-----------------------

## Detective
### **Team: Crewmates**
The Detective is a crewmate role who gathers information from corpses and 'Crime Scenes'\
\
A crime scene is only visible to the detective, and is depicted by a blood stain, shown where they were killed.
Crime Scenes are visible the round after a player has been killed.
If a body is displaced, a crime scene will still only display where they died and not where they end up. A crime scene will NOT appear if a body has been cleaned.
The detective has a chat window during rounds where it notes information from inspections. The detective may also use this chat window to take personal notes.\
\
The Detective has one ability: Inspect
- This ability has no cooldown, but has a limit on how many times it can be used per body and per crime scene.
- Inspect has a duration, and is intended to be usable multiple times per object.
- Inspecting a corpse will give more valuable information than inspecting a body first, and is intended to have a higher limit to number of possible inspections.
- Inspect will cause a chat popup in the detective's chat window with a bit of information every time it is used. Order of information is given is random and cannot repeat.
 
**Information(In order of power):** 
     1. Body Age Before Round ends(Crime Scene) / Body Age Before Inspect(Corpse)
     2. Whether the player was killed by an ability or a kill button(sheriff misfire, vampire bite, warlock, etc count as abilities)
     3. Dead Player Role
     4. Name of another player this person has killed
     5. Number of Players this killer has killed(if <=2, skip this)
     6. Killer's Alignment
     7. Direction the killer ran for up to 2s after killing(time stopped if vent is used)(N, S, E, W, NW, NE, SW, SE)
     8. Did killer use vent within 5s after killing
     
**Notes**
- Blood stain can only be seen within the detective's vision range(fog of war hides it)


### Game Options
| Name | Description
|----------|:-------------:|
| Detective Spawn Chance | -
| Inspect Duration | The time it takes to question a body
| Enable A Crime Scene | -
-----------------------

## Jailor
### **Team: Crewmates**
The Jailor is a crewmate-protective role with a meeting-based role-block ability. This role is meant to have a main focus on countering assassins. Their main ability {Jail} can be performed once per meeting, and is earned by completing tasks to build up charges(in a similar fashion to ToR's swapper). The Jailor can choose one person per meeting to jail. When jailed, a player’s name is displayed in dark gray. Players who can see a Jailed Target are configurable. A jailed player is immune to all role-based interactions, and cannot perform role-based interactions themselves. Players who attempt any of these actions will have charges/shots consumed where it applies, and get a gray flash on their screen when they use their action on this player. If a player is jailed and for some reason tries to use a role ability in a meeting, the same thing happens to them. The only interaction a Jailed target can make without repercussion is an attempt to assassinate the jailor.
\
**Notes**
- If an incorrect assassinate attempt on the jailor is made when a target is jailed, their shot is consumed, they do not die, and a charge of their assassin shots are used up. They are not able to guess again in the same meeting.
- If an attempt is made to assassinate a Jailed target, the shot will fail without killing the assassin, regardless of it being correct or not. They may not make other guesses in that meeting.
- The Jailor is able to send in game messages to their jailed target, under a gray bean named “Jailor”
**Abilities:**
Jail: Choose a player in a meeting to be jailed. This player is now immune to all role-based interactions, and cannot perform role-based interactions. Only an assassin attempt can be made on the Jailor without repercussions.
### Game Options
| Name | Description
|----------|:-------------:|
| Jailor Spawn Chance | -
| Initial Jail Charges| -
| Tasks Per Recharge | -
| Can Jail Self | -
-----------------------

## Engineer
### **Team: Crewmates**
The Engineer (if alive) can fix a certain amount of sabotages per game from anywhere on the map.\
The Engineer can use vents.\
If the Engineer is inside a vent, depending on the options the members of the team Jackal/Impostors will see a blue outline around all vents on the map (in order to warn them).\
Because of the vents the Engineer might not be able to start some tasks using the "Use" button, you can double-click on the tasks instead.

**NOTE:**
- The kill button of Impostors activates if they stand next to a vent where the Engineer is. They can also kill them there. No other action (e.g. Morphling sample, Shifter shift, ...) can affect players inside vents.

### Game Options
| Name | Description |
|----------|:-------------:|
| Engineer Spawn Chance | -
| Number Of Sabotage Fixes| -
| Impostors See Vents Highlighted | -
| Jackal and Sidekick See Vents Highlighted | -
-----------------------

## Sheriff
### **Team: Crewmates**
The Sheriff has the ability to kill Impostors, Neutral Killers, and Neutral roles if enabled.\
If they try to kill a Crewmate, they die instead.\
\
**NOTE:**
- If the Sheriff shoots the person the Guaridan shielded, the Sheriff and the shielded person **both remain unharmed**.

### Game Options
| Name | Description |
|----------|:-------------:|
| Sheriff Spawn Chance | -
| Sheriff Cooldown | -
| Sheriff Can Kill Neutrals | -
| Sheriff Has A Deputy | Deputy can not be in game without Sheriff
-----------------------

## Investigator
### **Team: Crewmates**
The Investigator can see footprints that other players leave behind.\
The Investigator's other feature shows when they report a corpse: they receive clues about the killer's identity. The type of information they get is based on the time it took them to find the corpse.

**NOTE:**
- When people change their colors (because of a morph or camouflage), all the footprints also change their colors (also the ones that were already on the ground). If the effects are over, all footprints switch back to the original color.
- The Investigator does not see footprints of players that sit in vents
- More information about the [colors](#colors)

### Game Options
| Name | Description |
|----------|:-------------:|
| Investigator Spawn Chance | -
| Anonymous Footprints | If set to true, all footprints will have the same color. Otherwise they will have the color of the respective player.
| Footprint Interval | The interval between two footprints
| Footprint Duration | Sets how long the footprints remain visible.
| Time Where Investigator Reports Will Have Name | The amount of time that the Investigator will have to report the body since death to get the killer's name.  |
| Time Where Investigator Reports Will Have Color Type| The amount of time that the Investigator will have to report the body since death to get the killer's color type. |
-----------------------

## Swapper
### **Team: Crewmates**
During meetings the Swapper can exchange votes that two people get (i.e. all votes
that player A got will be given to player B and vice versa).\
Because of the Swapper's strength in meetings, they might not start emergency meetings and can't fix lights and comms.\
The Swapper now has initial swap charges and can recharge those charges after completing a configurable amount of tasks.\
\
**NOTE:**
- The remaining charges will be displayed in brackets next to the players role while not in a meeting
- In a meeting the charges will appear next to the Confirm Swap button

### Game Options
| Name | Description
|----------|:-------------:|
| Swapper Spawn Chance | -
| Swapper can call emergency meeting | Option to disable the emergency button for the Swapper
| Swapper can only swap others | Sets whether the Swapper can swap themself or not
| Initial Swap Charges | -
| Number Of Tasks Needed For Recharging | -
-----------------------

## Tracker
### **Team: Crewmates**
The Tracker can select one player to track. Depending on the options the Tracker can track a different person after each meeting or the Tracker tracks the same person for the whole game.\
An arrow points to the last tracked position of the player.\
The arrow updates its position every few seconds (configurable).\
Depending on the options, the Tracker has another ability: They can track all corpses on the map for a set amount of time. They will keep tracking corpses, even if they were cleaned or eaten by the Vulture.

**NOTE**
- If the tracked player is on a different floor on Submerged, the arrow will always point to the elevator

### Game Options
| Name | Description
|----------|:-------------:|
| Tracker Spawn Chance | -
| Tracker Update Interval | Sets how often the position is being updated
| Tracker Reset Target After Meeting | -
| Tracker Can Track Corpses | -
| Corpses Tracking Cooldown | -
| Corpses Tracking Duration | -
-----------------------

## Spy
### **Team: Crewmates**
The Spy is a Crewmate, which has no special abilities.\
The Spy looks like an additional Impostor to the Impostors, they can't tell the difference.\
There are two possibilities (depending on the set options):
- The Impostors can't kill the Spy (because otherwise their kill button would reveal, who the Spy is)
- The Impostors can kill the Spy but they can also kill their Impostor partner (if they mistake another Impostor for the Spy)
You can set whether the Sheriff can kill the Spy or not (in order to keep the lie alive).

**NOTE:**
- If the Spy gets sidekicked, it still will appear red to the Impostors.

### Game Options
| Name | Description
|----------|:-------------:|
| Spy Spawn Chance |
| Spy Can Die To Sheriff |
| Impostors Can Kill Anyone If There Is A Spy | This allows the Impostors to kill both the Spy and their Impostor partners
| Spy Can Enter Vents | Allow the Spy to enter/exit vents (but not actually move to connected vents)
| Spy Has Impostor Vision | Give the Spy the same vision as the Impostors have
-----------------------

## Medium
### **Team: Crewmates**

The medium is a crewmate who can ask the souls of dead players for information. Like the Seer, it sees the places where the players have died (after the next meeting) and can question them. It then gets random information about the soul or the killer in the chat. The souls only stay for one round, i.e. until the next meeting. Depending on the options, the souls can only be questioned once and then disappear.\

Questions:
What is your Role?
What is your killer's color type?
When did you die?
What is your killers role?

### Game Options
| Name | Description
|----------|:-------------:|
| Medium Spawn Chance | -
| Medium Cooldown | -
| Medium Duration | The time it takes to question a soul
| Medium Each Soul Can Only Be Questioned Once | If set to true, souls can only be questioned once and then disappear
-----------------------

# Modifier
A modifier is an addition to your Impostor/Neutral/Crewmate role.
Some modifiers can be ingame more than once (Quantity option).

## Assassin
### **Impostor Modifier**
The Assassin can shoot players during the meeting, by guessing its role. If the guess is wrong, the Assassin dies instead.\
You can select how many players can be shot per game and if multiple players can be shot during a single meeting.\
The guesses Impostor and Crewmate are only right, if the player is part of the corresponding team and has no special role.\
You can only shoot during the voting time.\

**NOTE:**
- If a player gets shot, you'll get back your votes
- Jester wins won't be triggered, if the Guesser shoots the Jester before the Jester gets voted out

### Game Options
| Name | Description |
|----------|:-------------:|
| Number Of Assassins | 0-3
| Number Of Shots Per Game | -
| Can Shoot Multiple Times Per Meeting |  -
| Guesses Ignore The Medic Shield | -
| Can Guess The Spy | -
| Can't Guess Snitch When Tasks Completed | -

-----------------------

## Sleepwalker

The Sleepwalker Modifier prevents the player from getting teleported to the Meeting Table if a body gets reported or an Emergency Meeting is called.\
The player will start the round where the previous one ended (Emergency Meeting Call/Body Report).

### Game Options
| Name | Description |
|----------|:-------------:|
| Sleepwalker Spawn Chance | -
| Sleepwalker Quantity | -
-----------------------

## Mini

The Mini's character is smaller and hence visible to everyone in the game.\

**NOTE:**
- Mini Cannot be guessed, the guesser must know the primary role.

### Game Options
| Name | Description |
|----------|:-------------:|
| Mini Spawn Chance | -
| Mini Speed Modifier | =
-----------------------

## Giant

The Giant's character is larger and hence visible to everyone in the game.\

**NOTE:**
- Giant cannot be guessed, the guesser must know the primary role.

### Game Options
| Name | Description |
|----------|:-------------:|
| Giant Spawn Chance | -
| Giant Speed Modifier | -
-----------------------

# Source code
You can use parts of the code but don't copy paste the whole thing. Make sure you give credits to the other developers, because some parts of the code are based on theirs.

# Bugs, suggestions and requests
If you found any bugs, have an idea for a new role or any other request, join our [Discord](https://discord.gg/tKNVCXsP).


<p align="center">
This mod is not affiliated with Among Us or Innersloth LLC, and the content contained therein is not endorsed or otherwise sponsored by Innersloth LLC. Portions of the materials contained herein are property of Innersloth LLC. © Innersloth LLC.</p>


# StellarRoles

**StellarRoles** is a mod for [Among Us](https://store.steampowered.com/app/945360/Among_Us) which adds new roles, new [Settings](#settings) and new [Custom Hats](#custom-hats) to the game.

# Releases
| Among Us - Version| Mod Version | Link |
|----------|-------------|-----------------|
| 2022.12.14, 2022.7.12| v1.2.4 Dev A| [Download](https://github.com/Mr-Fluuff/StellarRolesAU/releases/download/v1.2.4/StellarRoles.zip)
| 2022.6.21, 2022.7.12| v1.0.5| [Download](https://github.com/Mr-Fluuff/StellarRolesAU/releases/download/v1.0.5/StellarRoles.zip)

# Changelog
<details>
  <summary>Click to show the Changelog</summary>
  
**Version 1.0.5**  

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
![TOR Colors](./Images/TOR_colors.jpg)

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
The Wraith is an Impostor, that has two abilities.
1. Phase - Speed up yourself for a short period of time.
2. Lantern - Place a lantern to later teleport back too.\

**NOTE:**
- If you do not return to the lantern before time is up or a meeting is called you will leave evidence behind.
- Option to turn Wraith invisible after return button is clicked.

### Game Options
| Name | Description |
|----------|:-------------:|
| Wraith Spawn Chance | -
| Phase Cooldown | -
| Phase Duration | -
| Lantern | On/Off
| Lantern Cooldown | -
| Lantern Duration | -
| Invisibility | Should Wraith turn invisible after return?
| Invisibility Duration | -
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
If there is a Scavenger in the game, there can't be a Janitor.

**NOTE**
- If the corpse is on a different floor on Submerged, the arrow will always point to the elevator

### Game Options
| Name | Description |
|----------|:-------------:|
| Scavenger Spawn Chance | -
| Scavenger Countdown | -
| Number Of Corpses Needed To Be Eaten | Corpes needed to be eaten to win the game
| Scavenger Can Use Vents | -
| Show Arrows Pointing Towards The Corpes | -
-----------------------

## Vigilante
### **Team: Crewmates**
The Vigilante can shoot players during the meeting, by guessing its role. If the guess is wrong, the Vigilante dies instead.\
You can select how many players can be shot per game and if multiple players can be shot during a single meeting.\
The guesses Impostor and Crewmate are only right, if the player is part of the corresponding team and has no special role.\
You can only shoot during the voting time.\
Depending on the options, the Vigilante can't guess the shielded player and depending on the Medic options the Medic/shielded player might be notified (no one will die, independently of what the Guesser guessed).\
\
**NOTE:**
- If a player gets shot, you'll get back your votes
- Jester wins won't be triggered, if the Guesser shoots the Jester before the Jester gets voted out

### Game Options
| Name | Description |
|----------|:-------------:|
| Vigilante Spawn Chance | -
| Number Of Shots Per Game | -
| Can Shoot Multiple Times Per Meeting |  -
| Guesses Visible In Ghost Chat | -
| Guesses Ignore The Medic Shield | -

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
The Sheriff has the ability to kill Impostors or Neutral roles if enabled.\
If they try to kill a Crewmate, they die instead.\
\
**NOTE:**
- If the Sheriff shoots the person the Medic shielded, the Sheriff and the shielded person **both remain unharmed**.

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
Depending on the options, the Assassin can't guess the shielded player and depending on the Medic options the Medic/shielded player might be notified (no one will die, independently of what the Guesser guessed).\
\
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

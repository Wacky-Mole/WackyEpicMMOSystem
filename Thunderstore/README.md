# Description:
This mod adds an RPG-like system of levels and attribute increases: - Wacky Branch 1.9.44


Support me!

<a href="https://www.buymeacoffee.com/WackyMole" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" height='36' style="height: 36px;" ></a>  [![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/H2H6LL5GA)

![https://wackymole.com/hosts/Attributes_basic.png](https://wackymole.com/hosts/Attributes_basic.png)

Features:
 - On screen XP bar
 - Usage for Mob Trophies with Potions and XP Orbs
 - Allows admins to adjust player progression
 - Custom mobs can be added for XP gain.
 - PVP XP, With Level, XP worth and Days since the player last died.
 - Non Combat XP earning.
 - Shared group XP. Outside of groups all XP awards go to the character who struck the last blow. Requires Group Mod
 - MMO-like friends list. -[Groups](https://valheim.thunderstore.io/package/Smoothbrain/Groups/)
 - Compatible with [ItemRequiresSkillLevel](https://thunderstore.io/c/valheim/p/WackyMole/WackyItemRequiresSkillLevel/) mod. Equipment can be limited by level or attribute.
 - Compatible with [Item Control](https://thunderstore.io/c/valheim/p/RustyMods/ItemControl/) mod. Control craft, equip, consume based on skill level or Almanac Classes or WackyEpicMMO
 - Compatible with [KGMarketplace mod](https://valheim.thunderstore.io/package/KGvalheim/Marketplace_And_Server_NPCs_Revamped/). Experience rewards can be added: <s>(EpicMMO_Exp:250) Quests can be limited by level (EpicMMO_Level: text, 20) </s> Read updated Marketplace docs
 
 ![https://wackymole.com/hosts/2nd%20image.png](https://wackymole.com/hosts/2nd%20image.png)

<details><summary>Attributes</summary>

	Strength: Physical Damage increase, Carry Weight Increase, Decreased Stamina Consumption, Critical Damage

	Dexterity: Player Attack/Usage Speed%,  Stamina consumption (running, jumping) decreased,

	Intellect: Elemental Damage increase, Eitr Regeneration increases,  Eitr Increase

	Endurance: Physical Armor increase, Flat Stamina, Stamina Regeneration

	Vigour: Flat Health Increase, Health Regeneration, Elemental Armor increase, 

	Specializing: Critical Damage Chance, Mining Damage, Construction Piece Health, Tree Cutting Damage

</details> 

<details><summary>Friends list</summary>

MMO-like friends list. - Groups MOD Group to earn XP, download requires Group mod for each client https://valheim.thunderstore.io/package/Smoothbrain/Groups/

Click the plus button at the bottom of the friends bar. Enter the name of the character you wish to add, starting with a capital letter. ****
   ![https://wackymole.com/hosts/3rd%20image.png](https://wackymole.com/hosts/3rd%20image.png)
The player will receive a friend request. Once accepted, the character will appear in your friends list. Group invites can be sent from the friends list. 

# Warning: 
- If you accept a friend request while the player who sent it is not logged in with the character, you will not be added to their friends list and they will need to resend the friend request.
- You cannot send friend requests to yourself or characters you have already added. If you need to send another friend request, remove the character from the list first.
- Friend requests that have been sent, but not accepted will be removed on logout. They must be accepted while both characters are online.
</details> 

<details><summary>Creature level control</summary>

This mod should assigns levels to all in-game monsters. Every star added adds +1 to the level of the mob.

![https://wackymole.com/hosts/creaturecontrol.png](https://wackymole.com/hosts/creaturecontrol.png)


</details>

<details><summary>Cyan, White, Red, and Yellow Mobs</summary>




	Higher level monsters will have their names appear in red. Monsters within your range will be white. Monsters below your level will be cyan.  By default it is +- 10 of your current level.

	If you are significantly higher level than a monster, your XP award will be reduced. Monsters that are significantly lower level than you will have their names appear in cyan.

	Monsters that are 1 level higher than the character + MaxLevelRange will curve XP.

	With defaults, starting exp req is 300 with a 1.05 multiplayer.  So first 5 levels of experience required will be: level 1 is 300, 2 is 615, 3 is 946, 4 is 1293, 5 is 1658

	Add LevelExperience on each level: disabled means that the levels will not add 300 each time: level 1 is 315, 2 is 330, 3 is 347, 4 is 365, 5 is 383. The jsons will all have to be reworked if this is disabled

	Below is an image of 1.04 +500 and with LevelExperience disabled, so no 300 added. The difference is a lot. Also 1.08 scaling is added just to show how it gets into the millions pretty quickly. 


	With Low_damage_level- Damage dealt to a higher level monster will be reduced by the difference in levels. E.g. (Character level 20/ Monster level 50 = 0.4. Damage dealt will be 0.4% of normal damage) 
	damageFactor = (float)(playerLevel + LowDamageConfig)/ monsterLevel; You can configure LowDamageConfig to adjust damage scaling up or down. Damage Factor will not go above 1 or below .1f

	All of these formulas functions can be configured in the settings file.

	Please note:
	When upgrading the mod to a newer version, new fields in the settings file will be created automatically. You will have to manually re-edit these values if you have changed them.
	If you have no custom settings in the configuration file, you should delete the file so that a fresh one can be created by the new version.

	Note for other Mods: This mod uses hit.toolTier to pass the Lvl of player and Player.m_localPlayer.m_knownTexts to store levels

	Yellow mobs are those that have a level 0 set in the Json. They are not restricted by level and will always drop their loot/give full exp and players can always do full damage. They show up as "???" yellow tags. 

![https://wackymole.com/hosts/epicmmolevelcalcs2.png](https://wackymole.com/hosts/epicmmolevelcalcs2.png)


</details>

<details><summary>ItemRequiresSkillLevel Mod</summary>

[ItemRequiresSkillLevel](https://thunderstore.io/c/valheim/p/WackyMole/WackyItemRequiresSkillLevel/)

Strength Agility Intellect Body Vigour  Special 

You can combine multiple Skills for one Requirement

		- PrefabName: AxeBronze
		  Requirements:
		  - Skill: Level
			Level: 15
			BlockEquip: true
			BlockCraft: false
			EpicMMO: true
			ExhibitionName: PlayerLevel
		  - Skill: Strength
			Level: 7
			BlockEquip: true
			BlockCraft: true
			EpicMMO: true
			ExhibitionName: Strength

</details>

<details><summary>Mob Data Included</summary>

	Mob's data (names, levels, exp) from other mods are included:

	Fantasy-Creatures, AirAnimals, Defaults(Ashlands), DoOrDieMonsters, LandAnimals, MonsterlabZ, Outsiders, SeaAnimals, Monstrum, Krumpac Mods, Teddy Bears, PungusSouls, JewelCrafting, Wizardy, NonCombat, Monstrum-DeepNorth, RTD mods Horrors, Monsters, Monstrum, Sea

	A folder listing all monsters and their levels is located in config/EpicMMOSystem/ Default is for vanilla mobs

	These jsons will get auto updated everytime the line below Version gets changed.

	A file called Version.txt is created in the folder. It contains the mod version that was used to create it. Replace it with "NO" to stop it from overwritting on a future update.

	Latest Update for Jsons config is <b> 1.9.38 </b>(Number will be updated when Jsons recieve an update)

	1.9.30 and 1.9.32 changed the jsons names, your old files will be in EpicMMOSystem_backup folder

</details>


<details><summary>PVP XP Mode</summary>

</br>
</br>

![https://wackymole.com/hosts/SEeffectsMMO.png](https://wackymole.com/hosts/pvphudmmo.png)

You can enable PVP XP in ( 8.PVP Combat XP)

Enable PVP XP - true or false

This mod does not change any aspect of vanilla PVP (pvp toggle)

I also added the ability for players to see other players Level, XP they are worth and Days alive. 

Total XP a player is worth is (Level * XP Per Level) + (XP Per Day Alive * Days).

You can control how much or little information is displayed and changed style and colors.

You can incentivize killing people that have been alive longer.

A player gains a day alive for each morning that they are on for. Everyone starts at 0. 

</br>
</details>

<details><summary>Potions and Magic Orbs</summary>

![https://wackymole.com/hosts/SEeffectsMMO.png](https://wackymole.com/hosts/SEeffectsMMO.png)


	8 Magic Orb Levels with Various XP given

	They have by default a 1% chance to drop from any mob and 100% to drop 1-4 from Bosses

	Orb levels depend on biome ( Extra biomes from Marketplace or Expanded world won't drop orbs)
	1 for Meadows
	2 for Blackforest, None
	3 for Swamps and Oceans
	4 for Mountains
	5 for Plains
	6 for Mistlands
	7 for Ashlands
	8 for Deep North


	3 Potions 
	XP Potion Minor: 30% extra XP for 10 min
	XP Potion Medium: 60%
	XP Potion Greator 100% 

	1 Magic Fermenator: Gold, FineWood, and Bronze
	It's colorful!

	Meads: are made from Mob Chunks
	Mob Chunks can be made from a variety of Trophies from mobs, you can add to the list. 
	Meads also require 1 or 2 Orbs depending on level
	Meads take the standard amount of time to ferment and drop 3 potions each
	Watch for the sky to light up with colors when fermentation is done.

	Orbs always have a chance to drop. 


</details>

<details><summary>Reset Skill Points</summary>
</br> </br>

There are configs for setting the Reset currency, default is Coins. You set the ammount per level.

There is also an Item called ResetTrophy that you can spawn or add to the builtin droplist that will allow any level reset with only 1 ResetTrophy.

The mod looks for your reset currency first and then ResetTrophies. Only consumes 1, so make this a very rare item. 

</details>

<details><summary>UI</summary>

![https://wackymole.com/hosts/hoverlarge.png](https://wackymole.com/hosts/hoverlarge.png)


Pretty much all of the UI can be scaled, hidden, dragged and remembers their location.

To make UI elements disappear type "none" in the respective elements color setting. 

	1HudPanelPosition: Main UI Background Panel Draggable, default color set by HudBackgroundCol, Type "none" to make it disappear

	HudBarScale: Scale this up or down to resize ALL MMO UI elements. - 1.0 Should cover all of your screen horizontally 

	2-5 UI elements have Position, Scale and Color: 
	 Scale (x, y, z)- z does not matter. - float
	 Color: #(6 digit Hex),  optional 7-8 Digit means alpha. #986100FF (FF -alpha of 1) or use without # red, cyan, blue, 
	 darkblue, lightblue, purple, yellow, lime, fuchsia, white, silver, grey, black, orange, brown, maroon, green, olive, navy, teal, aqua, magenta
	 set color to none, to hide element
	 
	 Can all be set to "none" to make individual elements disappear

	2ExpPanelPosition: ExP Bar, Dragable, Position, Scale and Color, Can be Hidden

	3StaminaPanelPosition: Dragable, Position, Scale and Color, Can be Hidden
	
	4HpPanelPosition: Dragable, Position, Scale and Color, Can be Hidden

	5EitrPanelPosition: Dragable, Position, Scale and Color, Can be Hidden. Will disappear and reappear when you have Eitr.

	DisabledHealthIcons: This disables the red Health Icon that is normal present under vanilla health bar

	To enable ONLY EXP bar , enable OldXPBar Bar Only and restart - not dragable in this mode, this is being slowly phased out.  No reason to use. 


</details> 

<details><summary>Translations</summary>

EpicMMO uses a built-in custom Translation Manager and the blaxx Translation Manager for Items

English, Russian, Chinese, Spanish, Korean, French, Polish, Portuguese, Brazilian Portuguese, Swedish, Ukrainian, Czech, Turkish, and German are currently implemented. 

</details> 

<details><summary>Console commands</summary>

Admin only commands: - Should work in singleplayer now
 - To set a character's level: `epicmmosystem level [value] [name]` 
 - To reset attribute points: `epicmmosystem reset_points [name]` 
 - To reset total points and level and current points: `epicmmosystem reset_totalpoints [name]` 
 - To recalc levels based on total experience: `epicmmosystem recalc [name]` 
 - Should work with spaces in names now or replace spaces with '&'
</details> 

<details><summary>Feedback</summary>


Wacky Git https://github.com/Wacky-Mole/WackyEpicMMOSystem

Original git - https://github.com/Single-sh/EpicMMOSystem

For questions or suggestions please join discord channel: [Odin Plus Team](https://discord.gg/odinplus) or my discord at [Wolf Den](https://discord.gg/uPjjH8y52j)

Support me at https://www.buymeacoffee.com/WackyMole  or https://ko-fi.com/wackymole

<a href="https://www.buymeacoffee.com/WackyMole" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" style="height: 60px !important;width: 217px !important;" ></a>

<a href='https://ko-fi.com/H2H6LL5GA' target='_blank'><img height='36' style='border:0px;height:36px;' src='https://storage.ko-fi.com/cdn/kofi3.png?v=3' border='0' alt='Buy Me a Coffee at ko-fi.com' /></a>

<img src="https://wackymole.com/hosts/bmc_qr.png" width="100"/>

Original Creator: LambaSun

</details> 

If you can't see the HUD BARs, try changing your Valheim Settings Display to Native Scaling Or Scale GUI to 100%
# Description:
This mod adds an RPG-like system of levels and attribute increases: - Wacky Branch 1.8.9.1


Support me!

<a href="https://www.buymeacoffee.com/WackyMole" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" height='36' style="height: 36px;" ></a>  [![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/H2H6LL5GA)

![https://wackymole.com/hosts/Attributes_basic.png](https://wackymole.com/hosts/Attributes_basic.png)

Features:
 - On screen XP bar
 - Usage for Mob Trophies with Potions and XP Orbs
 - Allows admins to adjust player progression
 - Custom mobs can be added for XP gain.
 - Shared group XP. Outside of groups all XP awards go to the character who struck the last blow. Requires Group Mod
 - MMO-like friends list. -[Groups](https://valheim.thunderstore.io/package/Smoothbrain/Groups/)
 - Compatible with [ItemRequiresSkillLevel](https://valheim.thunderstore.io/package/Detalhes/ItemRequiresSkillLevel/) mod. Equipment can be limited by level or attribute.
 - Compatible with [KGMarketplace mod](https://valheim.thunderstore.io/package/KGvalheim/Marketplace_And_Server_NPCs_Revamped/). Experience rewards can be added: (EpicMMO_Exp:250) Quests can be limited by level (EpicMMO_Level: text, 20)
 
 ![https://wackymole.com/hosts/2nd%20image.png](https://wackymole.com/hosts/2nd%20image.png)

<details><summary>Attributes</summary>

	Strength: Physical Damage increase, Carry Weight Increase, Decreased Black Stamina Consumption, Critical Damage

	Dexterity: Player Attack/Usage Speed**%,**  Stamina consumption (running, jumping) decreased,

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

[ItemRequiresSkillLevel](https://valheim.thunderstore.io/package/Detalhes/ItemRequiresSkillLevel/)

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

	Fantasy-Creatures, AirAnimals, Defaults, DoOrDieMonsters, LandAnimals, MonsterlabZ, Outsiders, SeaAnimals, Monstrum, Krumpac Mods, Teddy Bears, PungusSouls, JewelCrafting, RTDMonsters

	A folder listing all monsters and their levels is located in config/EpicMMOSystem/ Default is for vanilla mobs

	These jsons will get auto updated everytime the line below Version gets changed.

	A file called Version.txt is created in the folder. It contains the mod version that was used to create it. Replace it with "NO" to stop it from overwritting on a future update.

	Latest Update for Jsons config is <b> 1.8.8 </b>(Number will be updated when Jsons recieve an update)

</details>

<details><summary>Potions and Magic Orbs</summary>
****
![https://wackymole.com/hosts/SEeffectsMMO.png](https://wackymole.com/hosts/SEeffectsMMO.png)

	6 Magic Orb Levels with Various XP given

	They have by default a 1% chance to drop from any mob and 100% to drop 1-4 from Bosses

	Orb levels depend on biome ( Extra biomes from Marketplace or Expanded world won't drop orbs)
	1 for Meadows
	2 for Blackforest, None
	3 for Swamps and Oceans
	4 for Mountains
	5 for Plains
	6 for Mistlands, Ashlands, Deep North

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

English, Russian, Chinese, Spanish and German are currently implemented. 

</details> 

<details><summary>Console commands</summary>

Admin only commands: - Should work in singleplayer now
 - To set a character's level: `epicmmosystem level [value] [name]` 
 - To reset attribute points: `epicmmosystem reset_points [name]` 
 - To recalc levels based on total experience: `epicmmosystem recalc [name]` 
 - Should work with spaces in names now or replace spaces with '&'
</details> 

<details><summary>Feedback</summary>


Wacky Git https://github.com/Wacky-Mole/WackyEpicMMOSystem

Original git - https://github.com/Single-sh/EpicMMOSystem

For questions or suggestions please join discord channel: [Odin Plus Team](https://discord.gg/odinplus) or my discord at [Wolf Den](https://discord.gg/yPj7xjs3Xf)

Support me at https://www.buymeacoffee.com/WackyMole  or https://ko-fi.com/wackymole

<a href="https://www.buymeacoffee.com/WackyMole" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" style="height: 60px !important;width: 217px !important;" ></a>

<a href='https://ko-fi.com/H2H6LL5GA' target='_blank'><img height='36' style='border:0px;height:36px;' src='https://storage.ko-fi.com/cdn/kofi3.png?v=3' border='0' alt='Buy Me a Coffee at ko-fi.com' /></a>

<img src="https://wackymole.com/hosts/bmc_qr.png" width="100"/>

Original Creator: LambaSun or my [mod branch](https://discord.com/channels/826573164371902465/977656428670111794)

</details> 

<details>
  <summary><b><span style="color:aqua;font-weight:200;font-size:20px">
    ChangeLog
</span></b></summary>

| Version | Changes                                                                                                                                                                                                                                                                                                                                |
|----------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 1.8.9.1| - Add more level checks for some people getting rare errors. 
| 1.8.9  | - Add Korean****
| 1.8.8  | - Updated Monstrum.json for new mobs
| 1.8.7  | - Fix for header. </br> Updated Mobs, Default.json
| 1.8.6  | - Maybe fix for very rare directory error.
| 1.8.5  | - Fix for rare error resetting points. </br> Adjusting value default starting xp requirements and orbs to make leveling earlier a lot smoother. </br> Changed config names, FirstLevelExperience -> LevelExperience, "FirstLevelExperience used on each level" -> "Add LevelExperience on each level", Sorry. </br> ServerSync and Itemmanger update </br> Updated Excel sheet
| 1.8.2  | - Wacky learned Vulkan
| 1.8.1  | - Fix bug for orb XP
| 1.8.0  | - Rebalanced Specializing a little bit. </br> Update for 217.24
| 1.7.9  | - Add French </br> Updated Piecemanager </br> Added ??? level to mobs for mobs that should not be limited or known by their level. No level penalties will be applied. Use level 0 in json - kind a like a blacklist. </br> Orbs will always have a chance to drop no matter the level. </br> Updated JewelCrafting Worldbosses to be ??? - They are still very hard though </br> Updated Outsiders json
| 1.7.8  | - Add Swedish </br> Refined point calc in reset </br> Updated MonsterLaz and Default json for Hildr
| 1.7.7  | - Added RtDMonsters </br> Adjusted Orb XP amounts Greatly! </br> Added Orb Boss Max amount for orb chances </br> Bug fix </br> Update for Hildir's Update </br> @margmas helped me fix a IL error, thank you!
| 1.7.6  | - Added JewelCrafting Bosses. </br> Rounding XP Gained </br> Small bug fix for modded mobs without tamed status
| 1.7.5  | - Updated Monstrum. </br> Added Max Cap for each Attribute </br> Update Chinese </br> Disabled normal damage notification for critical attacks, so just critical attack and crit sound remain
| 1.7.4  | - Updated Monstrum and Monsterlabz, add localization for tooltip, potion and orb power. <br/> Added Speed Manager by Blaxx for compability. <br/> Added Portuguese
| 1.7.3  | - API update, Strength = 0, Agility = 1, Intellect = 2, Body = 3, Vigour = 4, Special = 5 <br/> Spanish Update <br/> Orbs no longer get extra XP multiplier </br> Mob update for Monsterlabz and Monstrum
| 1.7.2  | - Fixed Attack Speed for language heathens that use "," instead of "." for decimals, Updated GroupAPI
| 1.7.1  | - Fixed Orb XP spam,
| 1.7.0  | - BIG WACKY UPDATE! - Delete CONFIGS! <br/> Added Category Vitality and Specializing, while rearranging the categories. <br/>Added hover stats, so you can see what exactly adding points to a category does.  Red being most powerful <br/> Added 6 magic orbs that grant various levels of XP on consumption, they have 1 % chance to drop from all mobs. Guaranteed from Bosses. </br> Added Magic Potions and Magical Fermentor, so you can utilize trophies to make the mead and use the potion to get more XP! 3 Potion levels and various ways to craft mead. <br/> added attackSpeed, MiningDmg, TreeCuttingDmg and Critical attack chance and Critical Damage. 
| 1.6.7  | - Updated KG Marketplace API <br/> Updated DoororDieMob and LandAnimals jsons thanks to DeeJay <br/> Fixed a bug with FloatText XP not being correct
| 1.6.6  | - More UI adjustments, - Scaling/Position on NavBar and PointHud,
| 1.6.5  | - TeddyBears json added <br/> removeAllDropsFromNonPlayerKills is true by default (If a Tree kills a mob, you won't get drop) <br /> Added 'U Jerk, NoExpOn Red/Blue' cfg option, for unfun times/admins. 
| 1.6.4  | - Fixed 'RemoveAllDrops From NonPlayer Kills' for some configs
| 1.6.3  | - Fixed OldExp Bar <br /> close or apply, now closes window. <br /> Update Outsider json
| 1.6.2  | - Update Chinese <br/> Fixed UI so it can disappear with Ctrl-F3 <br/> Group EXP Range (GROUP MOD ONLY) - If killer didn't get xp, group won't get xp <br/> Added Player EXP Range, how far the player who killed a mob gets XP. <br/> Added the ability for tames to give their master EXP on kills <br/> Added Json for Krumpac mobs <br/> RemoveAllDrops From NonPlayer Kills config. - No more random creature drops - Not enabled by default </br> Added Config to remove alert on Left side for XP on mob death </br> Config for popup XP string
| 1.6.1  | - eXP level fix<br/> HP/stamina, XP/eitr bar elements can be changed to "none" and update/disappear realtime<br/> Update Chinese <br/> Included color exp on mob death.
| 1.6.0  | - Fixed ResetTrophy not being consumed on use.<br/> Added Chinese Translation <br/> Fixed some red errors with CLLC <br/> Included an excel file for comparison. 
| 1.5.9  | - Update for 214.2
| 1.5.8  | - Update for single char word languages - horizonal overflow <br/> MonsterDB update and added mob levels for Monstrum(beta)
| 1.5.7  | - Fix for stamina consumption for running and jumping. Was increasing instead of decreasing, credits for discovery.  Moissonneur and Kevin
| 1.5.6  | - Added German Localization <br /> Changed defaults to not take any drops away, no matter the level. - Blax complained enough - <br /> Stamina Regen is a percentage <br /> Added MOB UI string for wacky fun, aka single char word languages <br /> Update ItemManager <br /> Added ability to set "none" on color for all hud elements to make them go away, should live update. Might have relaunch to get them to come back
| 1.5.5  | - Update readme to talk about "FirstLevelExperience used on each level" - Added an excel image for XP comparisons between modes - Most people should NOT disable this, makes balancing completely different.
| 1.5.4: | - Updated to allow level and reset commands for Spaced Names. <br/>Updated Jsons, Added extra text file for people who don't read readme or version changes... you know who you are.... <br/> Added abilty for EpicMMO to recalculate maxlvls on serversync updates. I still don't recommend live updating with this mod, but less bugs now. <br/> Serious discussion: It appears if you ever changed expierence values(rateExp,expForLvlMonster, etc) after players started playing, things could get wonky unless you reset them(even after game restarts). I added a TotalExp tracker, but it won't be useful unless you restart all your players back to 0. I have added another command to Terminal recalc, but it will reset players levels to 0 if not a new charc on this update.  <br/> Added MobLevelPosition and BossLevelPosition for server admins to config mob bar placement. </br> Fixed lowDamageExtraConfig, small oversight <br/> Added ResetTrophy item for people to add to droplists <br/> Added EitrIncrease to Intellegence - More OP 
| 1.5.3: | - Fixed bug in Groups exp sharing. <br/> Added MajesticChickens json
| 1.5.2: | - Added Colors and Scale to Individual UI elements.<br/> Fixed EpicLoot drop bug, made Nav Panel moveable, Eitr UI adjustments<br/> Low_damage_config for extra configurability on low damage mode
| 1.5.1: | - Added Stamina regeneration<br/>
| 1.5.0: | - Changed Config to WackyMole.EpicMMOSystem.cfg<br/> - Made all the UI elements dragable<br/> - Realtime setting of (x,y) position in config, type "none" in BackgroundColor to remove brown bar.<br/> - Added Filewatcher to Jsons<br/> - dedicated Server only<br/> - Added filewatcher to configs, Updated Group logic<br/> - Revamped Mentor mode.<br/>
| 1.4.1: | - Fix Version Check and Multiplayer Sync, moved Monster Bar again.<br/>
| 1.4.0: | - Fix for inventory to bag JC (hopefully)<br/> - Changed Configs,PLEASE DELETE OLD CONFIGS!<br/> - added removeDropMax, removeDropMax,removeBossDropMax, removeBossDropMix, curveExp, curveBossExp.<br/> - Allow for multiple Jsons to be searched<br/> - Added admin rights to singleplayer hosting<br/> - Boss drop is determined by mob.faction(), curveBossExp Exp is just the 6 main bosses. <br/> - Updated Monster.json moved to configs instead of plugin.<br/> - Added ExtraDebugmode for future issues.<br/> - Updated MonserDB_Default for mistlands,LandAnimals mod, MonsterLabZ, Outsiders, SeaAnimals, Fantasy Creatures, Air Animals, and Outsiders.<br/> - Json file in MMO folder is searched.<br/> - Added Version text to easily update in future.<br/> - Write "NO" in Ver.txt to skip future updates. Moved Monster lvl bar [] for boss and non boss<br/>
| 1.3.1: | - Dual wield and EpicMMO Thanks to KG, sponsored by Aldhari/Skaldhari<br/>
| 1.3.0: | - WackyEpicMMOSystem release, until author comes back. Code from Azumatt - Updated Chat, Group and ServerSync<br/>
| 1.2.8: | - Added a limiter for the maximum attribute value.<br/>- New view health and stamina bar (in the configuration you can return the old display where only the experience is displayed).<br/>
| 1.2.7: | - Fix version check<br/>
| 1.2.6: | - Fixed bug of different amount of experience. Added ability to add your own items or currency to reset<br/> attributes.
| 1.2.5: | - Fix damage monsters and fix error for friends list<br/>
| 1.2.4: | - Fix version check<br/>
| 1.2.3: | - Add console command and xp loss on death<br/>
| 1.2.2: | - Add button to open the quest journal (Marketplace) and profession window<br/>
| 1.2.1: | - Fix errors with EAQS<br/>
| 1.2.0: | - Add friends list feature<br/>
| 1.1.0: | - Add creature level control<br/>
| 1.0.1: | - Fix localization and append english text for config comments.<br/>
| 1.0.0: | - Release<br/>
</details> 
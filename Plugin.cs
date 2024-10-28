using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using ServerSync;
using UnityEngine;
using UnityEngine.Rendering;
using LocalizationManager;
using System.Collections.Generic;
using fastJSON;
using Groups;
using UnityEngine.UI;
using ItemManager;
using System.Xml;
using EpicMMOSystem.MonoScripts;
using EpicMMOSystem.OtherApi;
using PieceManager;
using System.Runtime.Remoting.Messaging;
using StatusEffectManager;
using System.Collections;
using HarmonyLib.Tools;
using UnityEngine.UIElements;
using System.Reflection.Emit;


namespace EpicMMOSystem;

[BepInPlugin(ModGUID, ModName, VERSION)]
[BepInDependency("org.bepinex.plugins.groups", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("org.bepinex.plugins.creaturelevelcontrol", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("WackyMole.WackysDatabase", BepInDependency.DependencyFlags.SoftDependency)]

public partial class EpicMMOSystem : BaseUnityPlugin
{
    internal const string ModName = "EpicMMOSystem";
    internal const string VERSION = "1.9.23";
    internal const string Author = "WackyMole";
   // internal const string configV = "_1_7";
    private const string ModGUID = Author + "." + ModName; //+ configV; changes GUID
    private static string ConfigFileName = ModGUID + ".cfg";
    private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
    public static bool _isServer => SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
    internal static string ConnectionError = "";
    internal static bool NetisActive = false;

    private readonly Harmony _harmony = new(ModGUID);

    public static readonly ManualLogSource MLLogger =
        BepInEx.Logging.Logger.CreateLogSource(ModName);

    public static readonly ConfigSync ConfigSync = new(ModGUID)
    { DisplayName = ModName, CurrentVersion = VERSION, MinimumRequiredVersion = VERSION };

    public static AssetBundle _asset;
    public static string folderpath = Path.Combine(Paths.ConfigPath, EpicMMOSystem.ModName);


    public static string playerjson = "Players.json";
    public static string playerspath = Path.Combine(Paths.ConfigPath, EpicMMOSystem.ModName, playerjson);

    internal static EpicMMOSystem Instance;
    public static bool CLLCLoaded = false;
    public static CustomSE MMOXP; // must assign in awake

    internal static Recipe Mead1Alt;
    internal static Recipe Mead2Alt;
    internal static Recipe Mead3Alt;

    internal static ItemDrop Mead1D = null;
    internal static ItemDrop Mead2D = null;
    internal static ItemDrop Mead3D = null;
    internal static ItemDrop Drink1 = null;
    internal static ItemDrop Drink2 = null;
    internal static ItemDrop Drink3 = null;

    internal static int numofCats = 6;
    internal static GameObject fx_seeker_crit = null;
    public static string  PlayerAliveString =  "_playeralivedays";


    public static Localizationold localizationold;
    //public static ConfigEntry<string> language;
    public static ConfigEntry<bool> extraDebug;
    //LevelSystem
    public static ConfigEntry<int> maxLevel;
    public static ConfigEntry<int> priceResetPoints;
    public static ConfigEntry<int> freePointForLevel;
    public static ConfigEntry<int> startFreePoint;
    public static ConfigEntry<bool> levelexpforeachlevel;
    public static ConfigEntry<int> levelExp;
    public static ConfigEntry<float> multiNextLevel;
    public static ConfigEntry<float> expForLvlMonster;
    public static ConfigEntry<float> rateExp;
    public static ConfigEntry<float> groupExp;
    public static ConfigEntry<float> groupRange;
    public static ConfigEntry<float> playerRange;
    public static ConfigEntry<bool> lossExp;
    public static ConfigEntry<float> minLossExp;
    public static ConfigEntry<float> maxLossExp;
    //public static ConfigEntry<int> maxValueAttribute;
    public static ConfigEntry<string> levelsForBinusFreePoint;
    public static ConfigEntry<bool> tamesGiveXP;
    public static ConfigEntry<bool> leftMessageXP;
    public static ConfigEntry<string> XPstring;
    public static ConfigEntry<bool> UseCustomXPTable;
    public static ConfigEntry<int> maxValueStrength; 
    public static ConfigEntry<int> maxValueDexterity; 
    public static ConfigEntry<int> maxValueIntelligence; 
    public static ConfigEntry<int> maxValueEndurance; 
    public static ConfigEntry<int> maxValueVigour; 
    public static ConfigEntry<int> maxValueSpecializing; 
    public static ConfigEntry<bool> altLevelUpSound; 
    public static ConfigEntry<bool> debugNonCombatObjects; 
 



    #region Parameters
    //LevelSystem arg property <Strength>
    public static ConfigEntry<float> physicDamage;
    public static ConfigEntry<float> addWeight;
    public static ConfigEntry<float> staminaBlock;
    public static ConfigEntry<float> critDmg;

    //LevelSystem arg property <Dexterity>
    public static ConfigEntry<float> attackSpeed;
    public static ConfigEntry<float> attackStamina;
    public static ConfigEntry<float> staminaReduction;

    //LevelSystem arg property <Intellect>
    public static ConfigEntry<float> addEitr;
    public static ConfigEntry<float> magicDamage;
    public static ConfigEntry<float> magicEitrRegen;

    //LevelSystem arg property <Endurance>
    public static ConfigEntry<float> addStamina;
    public static ConfigEntry<float> staminaRegen;
    public static ConfigEntry<float> physicArmor;

    //LevelSystem arg property <Vigour>
    public static ConfigEntry<float> addHp;
    public static ConfigEntry<float> regenHp;
    public static ConfigEntry<float> magicArmor;

    //LevelSystem arg property <Specializing>
    public static ConfigEntry<float> critChance;
    public static ConfigEntry<float> startCritChance;
    public static ConfigEntry<float> miningSpeed;
    public static ConfigEntry<float> constructionPieceHealth;
    public static ConfigEntry<float> treeCuttingSpeed;

    #endregion


    //Creature level control 2
    public static ConfigEntry<bool> enabledLevelControl;
    public static ConfigEntry<bool> removeDropMax;
    public static ConfigEntry<bool> removeDropMin;
    public static ConfigEntry<bool> removeBossDropMax;
    public static ConfigEntry<bool> removeBossDropMin;
    public static ConfigEntry<bool> mentor;
    public static ConfigEntry<bool> curveExp;
    public static ConfigEntry<bool> curveBossExp;
    public static ConfigEntry<bool> lowDamageLevel;
    public static ConfigEntry<int> lowDamageExtraConfig;
    public static ConfigEntry<int> maxLevelExp;
    public static ConfigEntry<int> minLevelExp;
    public static ConfigEntry<string> MobLVLChars;
    public static ConfigEntry<string> XPColor;
    public static ConfigEntry<bool> removeAllDropsFromNonPlayerKills;
    public static ConfigEntry<bool> noExpPastLVL;
    public static ConfigEntry<string> noExpPastLVLPopup;

    //Reset attributes items
    public static ConfigEntry<String> prefabNameCoins;
    public static ConfigEntry<String> viewTextCoins;

    //Hud
    public static ConfigEntry<bool> oldExpBar;
    public static ConfigEntry<bool> showMaxHp;
    public static ConfigEntry<bool> forceMagicBar;
    public static ConfigEntry<float> HudBarScale;
    public static ConfigEntry<float> LevelHudGroupScale;
    public static ConfigEntry<string> HudExpBackgroundCol;
    public static ConfigEntry<string> HudPostionCords;
    public static ConfigEntry<bool> HealthIcons;
    

    // Position Saves
    internal static ConfigEntry<Vector2> HudPanelPosition = null!;
    internal static ConfigEntry<Vector2> ExpPanelPosition = null!;
    internal static ConfigEntry<Vector2> HpPanelPosition = null!;
    internal static ConfigEntry<Vector2> StaminaPanelPosition = null!;
    internal static ConfigEntry<Vector2> EitrPanelPosition = null!;
    internal static ConfigEntry<Vector2> MobLevelPosition = null!;
    internal static ConfigEntry<Vector2> BossLevelPosition = null!;
    internal static ConfigEntry<Vector2> LevelPointPosition = null!;
    internal static ConfigEntry<Vector2> LevelNavPosition = null!;

    // HUD Colors
    public static ConfigEntry<string> HpColor;
    public static ConfigEntry<string> StaminaColor;
    public static ConfigEntry<string> EitrColor;
    public static ConfigEntry<string> ExpColor;

    //HUD Scales
    internal static ConfigEntry<Vector3> ExpScale = null!;
    internal static ConfigEntry<Vector3> HPScale = null!;
    internal static ConfigEntry<Vector3> EitrScale = null!;
    internal static ConfigEntry<Vector3> StaminaScale = null!;

    //Optional
    public static ConfigEntry<float> addDefaultHealth;
    public static ConfigEntry<float> addDefaultWeight;
    public static ConfigEntry<float> CriticalStartChance;
    public static ConfigEntry <float> CriticalDefaultDamage;
    public static ConfigEntry<float> addDefaultEitr;

    // Orb and Potions
    public static ConfigEntry<int> XPforOrb1;
    public static ConfigEntry<int> XPforOrb2;
    public static ConfigEntry<int> XPforOrb3;
    public static ConfigEntry<int> XPforOrb4;
    public static ConfigEntry<int> XPforOrb5;
    public static ConfigEntry<int> XPforOrb6;

    public static ConfigEntry<float> XPforMinorPotion;
    public static ConfigEntry<float> XPforMediumPotion;
    public static ConfigEntry<float> XPforGreaterPotion;
    public static ConfigEntry<int> OrbSEtime;
    public static ConfigEntry<int> PotionSEtime;
    public static ConfigEntry<float> OrbDropChance;
    public static ConfigEntry<float> OrbDropChancefromBoss;
    public static ConfigEntry<int> OrdDropMaxAmountFromBoss;
    public static ConfigEntry<bool> UseRegFerm;

    // Non Combat
    public static ConfigEntry<bool> disableNonCombatObjects;
    public static ConfigEntry<int> MultiplierForXPTaming;
    public static ConfigEntry<bool> disablePieceXP;
    public static ConfigEntry<bool> disableTameXP;
    public static ConfigEntry<bool> disableFishPickupXP;
    public static ConfigEntry<bool> disableMiningXP;
    public static ConfigEntry<bool> disablePickableXP;
    public static ConfigEntry<bool> disableTreeXP;
    public static ConfigEntry<bool> disableDestructablesXP;

    // PVP 
    public static ConfigEntry<string> displayPlayerLevel;
    public static ConfigEntry<string> displayPlayerXP;
    public static ConfigEntry<string> displayDaysAlive;
    public static ConfigEntry<bool> enablePVPXP;
    public static ConfigEntry<int> xpPerLevelPVP;
    public static ConfigEntry<int> xpPerDayNotDead;





    public void Awake()
    {
        Localizer.Load();
       
        Instance = this;
        MMOXP = new CustomSE("MMO_XP");

        string general = "0.General---------------";
        _serverConfigLocked = config(general, "Force Server Config", true, "Force Server Config");
       // language = config(general, "Language", "eng", "Language prefix", false);
        extraDebug = config(general, "EnableExtraDebug", false, "Enable Extra Debug mode for Debugging", false);
        string levelSystem = "1.LevelSystem-----------";
        maxLevel = config(levelSystem, "MaxLevel", 100, "Maximum level. Максимальный уровень");
        priceResetPoints = config(levelSystem, "PriceResetPoints", 3, "Reset price per point. Цена сброса за один поинт");
        freePointForLevel = config(levelSystem, "FreePointForLevel", 5, "Free points per level. Свободных поинтов за один уровень");
        startFreePoint = config(levelSystem, "StartFreePoint", 5, "Additional free points start. Дополнительных свободных поинтов");
        levelExp = config(levelSystem, "LevelExperience", 300, "Amount of experience needed per level. Количество опыта необходимого на 1 уровень");
        levelexpforeachlevel = config(levelSystem, "Add LevelExperience on each level", true, "By default the calculations per level are (previous_amount * 1.05 + 300) disabled it will be (previous_amount * 1.05) per level ");
        multiNextLevel = config(levelSystem, "MultiplyNextLevelExperience", 1.05f, "Experience multiplier for the next level - Should never go below 1.00. Умножитель опыта для следующего уровня");
        expForLvlMonster = config(levelSystem, "ExpForLvlMonster", 0.25f, "Extra experience (from the sum of the basic experience) for the level of the monster. Доп опыт (из суммы основного опыта) за уровень монстра");
        rateExp = config(levelSystem, "RateExp", 1f, "Experience multiplier. Множитель опыта");
        groupExp = config(levelSystem, "GroupExp", 0.70f, "Experience multiplier that the other players in the group get. Множитель опыта который получают остальные игроки в группе");
        minLossExp = config(levelSystem, "MinLossExp", 0.05f, "Minimum Loss Exp if player death, default 5% loss");
        maxLossExp = config(levelSystem, "MaxLossExp", 0.25f, "Maximum Loss Exp if player death, default 25% loss");
        lossExp = config(levelSystem, "LossExp", true, "Enabled exp loss");
        //maxValueAttribute = config(levelSystem, "MaxValueAttribute", 200, "Maximum number of points you can put into one attribute");
        levelsForBinusFreePoint = config(levelSystem, "BonusLevelPoints", "5:5,10:5", "Added bonus point for level. Example(level:points): 5:10,15:20 add all 30 points ");
        groupRange = config(levelSystem, "Group EXP Range", 70f, "The range at which people in a group (Group MOD ONLY) get XP, relative to player who killed mob - only works if the killer gets xp. - Default 70f, a large number like 999999999999f, will probably cover map");
        playerRange = config(levelSystem, "Player EXP Range", 70f, "The range at which a player gets XP");
        tamesGiveXP = config(levelSystem, "Tames give XP on Mob kill", true, "Your tames give players in range XP");
        leftMessageXP = config(levelSystem, "Display XP Received on Left", true, "Display XP Amount on Left Message when mob is killed");
        XPColor = config(levelSystem, "XP death Color", "#fff708", "The Color of XP popup market on a mob death");
        XPstring = config(levelSystem, "XP String", "+@ XP", "@ for XP Received, must include '@'");
        //UseCustomXPTable = config(levelSystem, "Use Custom XP Table", false, "Use the CustomXPTable.yml file for levels instead of maxLevel, levelExp, levelexpforeach and multiNext Level");
        maxValueStrength = config(levelSystem, "maxValueStrength", 200, "Maximum number of points you can put into Strength");
        maxValueDexterity = config(levelSystem, "maxValueDexterity", 200, "Maximum number of points you can put into Dexterity");
        maxValueIntelligence = config(levelSystem, "maxValueIntelligence", 200, "Maximum number of points you can put into Intelleigence");
        maxValueEndurance = config(levelSystem, "maxValueEndurance", 200, "Maximum number of points you can put into Endurance");
        maxValueVigour = config(levelSystem, "maxValueVigour", 200, "Maximum number of points you can put into Vigour");
        maxValueSpecializing = config(levelSystem, "maxValueSpecializing", 200, "Maximum number of points you can put into Specializing");
        MultiplierForXPTaming = config(levelSystem, "Taming XP Multiplier", 5, "You get normal xp amount, times this number for taming a creature");
        altLevelUpSound = config(levelSystem, "altLevelUpSound", false, "It's Loud and Heart attack inducing");




        #region ParameterCofig
        string levelSystemStrngth = "1.LevelSystem Strength-----";
        physicDamage = config(levelSystemStrngth, "PhysicDamage", 0.20f, "Damage multiplier per point.");
        addWeight = config(levelSystemStrngth, "AddWeight", 2f, "Adds carry weight per point. ");
        staminaBlock = config(levelSystemStrngth, "StaminaBlock", 0.2f, "Decrease stamina consumption per unit per point.");
        critDmg = config(levelSystemStrngth, "CriticalDmg", 1f, "Amount of Critical Dmg done per point"); // check

        string levelSystemAgility = "1.LevelSystem Dexterity---";
        attackSpeed = config(levelSystemAgility, "AttackSpeed", 0.2f, "Swing your blade faster with every point - OP warning-");  // skip bows
        attackStamina = config(levelSystemAgility, "StaminaAttack", 0.1f, "Reduces attack stamina consumption. ");
        staminaReduction = config(levelSystemAgility, "StaminaReduction", 0.15f, "Decrease stamina consumption for running, jumping for one point.");
        

        string levelSystemIntellect = "1.LevelSystem Intellect-----";
        magicDamage = config(levelSystemIntellect, "MagicAttack", 0.20f, "Increase magic attack per point.");      
        magicEitrRegen = config(levelSystemIntellect, "MagicEitrReg", 0.3f, "Increase magical Eitr Regeneration per point.");
        addEitr = config(levelSystemIntellect, "AddEitr", 0.3f, "Eitr Increase per point ONLY when player has above 1 base Eitr");

        string levelSystemBody = "1.LevelSystem Endurance------";
        addStamina = config(levelSystemBody, "AddStamina", 1f, "One Point Stamina Increase.");      
        staminaRegen = config(levelSystemBody, "StaminaReg", 0.4f, "Increase stamina regeneration per point.");
        physicArmor = config(levelSystemBody, "PhysicArmor", 0.15f, "Increase in physical protection per point.");

        string levelSystemVigour = "1.LevelSystem Vigour------";
        addHp = config(levelSystemVigour, "AddHp", 1f, "One Point Health Increase. ");
        regenHp = config(levelSystemVigour, "RegenHp", 0.1f, "Increase health regeneration per point.");
        magicArmor = config(levelSystemVigour, "MagicArmor", 0.1f, "Increase magical protection per point. ");

        string levelSystemSpecializing = "1.LevelSystem Specializing------";
        startCritChance = config(levelSystemSpecializing, "Critical Start Chance", 1f, "Critical Chance Starts at (1%) after 1 point assigned ");
        critChance = config(levelSystemSpecializing, "Critical Chance", 0.1f, "Critical Chance, Starts with Critical Start Chance, increment per point - default 0.1% ");
        miningSpeed = config(levelSystemSpecializing, "MiningSpeed", 0.4f, "Mining Dmg Multiplier per point"); //check
        constructionPieceHealth = config(levelSystemSpecializing, "PieceHealth", 2f, "Increase max health of new pieces built per point"); // check
        treeCuttingSpeed = config(levelSystemSpecializing, "TreeCuttingSpeed", 0.4f, "Increase tree cutting speed per point.");

        #endregion

        string creatureLevelControl = "2.Creature level control";
        mentor = config(creatureLevelControl, "Mentor", true, "Give full eXP for low level members in Groups");
        enabledLevelControl = config(creatureLevelControl, "Enabled_creature_level", true, "Enable creature Level control - Disable this to remove levels, but still get eXP gain");
        removeDropMax = config(creatureLevelControl, "Remove_creature_drop_max", false, "Monsters after death do not give items if their level is higher than player level + MaxLevel");
        removeDropMin = config(creatureLevelControl, "Remove_creature_drop_min", false, "Monsters after death do not give items if their level is lower than player level - MinLevel");
        removeBossDropMax = config(creatureLevelControl, "Remove_boss_drop_max", false, "Bosses after death do not give items if their level is higher than player level + MaxLevel");
        removeBossDropMin = config(creatureLevelControl, "Remove_boss_drop_min", false, "Bosses after death do not give items if their level is lower than player level - Minlevel");
        curveExp = config(creatureLevelControl, "Curve_creature_exp", true, "Monsters after death will give less exp if player is outside Max or Min Level Range");
        curveBossExp = config(creatureLevelControl, "Curve_Boss_exp", true, "Bosses after death will give less exp if player is outside Max or Min Level Range");
        lowDamageLevel = config(creatureLevelControl, "Low_damage_level", false, "Decreased damage to the monster if the level is insufficient");
        lowDamageExtraConfig = config(creatureLevelControl, "Low_damage_config", 0, "Extra paramater to low damage config - for reference(float)(playerLevel + lowDamageConfig) / monsterLevel; when player is below lvl");
        minLevelExp = config(creatureLevelControl, "MinLevelRange", 10, "Character level - MinLevelRange is less than the level of the monster, then you will receive reduced experience. Уровень персонажа - MinLevelRange меньше уровня монстра, то вы будете получать урезанный опыт");
        maxLevelExp = config(creatureLevelControl, "MaxLevelRange", 10, "Character level + MaxLevelRange is less than the level of the monster, then you will not receive experience. Уровень персонажа + MaxLevelRange меньше уровня монстра, то вы не будете получать опыт");
        MobLevelPosition = config(creatureLevelControl, "LevelBar Position", new Vector2(40, -30), "LevelBar Position for regular mobs - synced");
        BossLevelPosition = config(creatureLevelControl, "Boss LevelBar Position", new Vector2(0, 30), "LevelBar Position for Boss Bars - synced");
        MobLVLChars = config(creatureLevelControl, "Mob Level UI String", "[@]", "[@] uses @ for moblevel, must include '@', but you coudl do 'Level @' or something similar");
        removeAllDropsFromNonPlayerKills = config(creatureLevelControl, "RemoveAllDrops From NonPlayer Kills", true, "Remove all drops from mobs that were not killed by a player or tame -ie no drops from mobs attacking each other");
        noExpPastLVL = config(creatureLevelControl, "U Jerk, NoExpOn Red/Blue", false, "You are a jerk admin if you enable this, this removes all exp on creatures that are above level (RED) or Below (Blue)");
        noExpPastLVLPopup = config(creatureLevelControl, "Popup Message For noExpPastLVL", "No XP for red/blue creatures :(", "This message displays when you enable U Jerk, NoExpOn Red/Blue and Kill a Red or blue mob");

        string resetAttributesItems = "3.Reset attributes items";
        prefabNameCoins = config(resetAttributesItems, "prefabName", "Coins", "Name prefab item");
        viewTextCoins = config(resetAttributesItems, "viewText", "coins or 1 Reset Trophy", "Name item");

        // HealthIcons = config(hud, "DisabledDefaultHealth", true, "Default is true, not synced", false);


        string optionalEffect = "5.Optional perk---------";
        addDefaultHealth = config(optionalEffect, "AddDefaultHealth", 0f, "Add health by default");
        addDefaultWeight = config(optionalEffect, "AddDefaultWeight", 0f, "Add weight by default");
        CriticalStartChance = config(optionalEffect, "CriticalStartChance", 0f, "Add Critical Chance by default");
        CriticalDefaultDamage = config(optionalEffect, "DefaultCriticalDamage", 30f, "Add Critical Damage by default");
        addDefaultEitr =config(optionalEffect, "addDefaultEitr", 0f, "Add Eitr by default");

        string OrbandPotion = "6.Orbs and Potions-------";
        XPforOrb1 = config(OrbandPotion, "XP for Orb 1", 300, "Consuming Orb grants how much exp?");
        XPforOrb2 = config(OrbandPotion, "XP for Orb 2", 600, "Consuming Orb grants how much exp?");
        XPforOrb3 = config(OrbandPotion, "XP for Orb 3", 1000, "Consuming Orb grants how much exp?");
        XPforOrb4 = config(OrbandPotion, "XP for Orb 4", 2000, "Consuming Orb grants how much exp?");
        XPforOrb5 = config(OrbandPotion, "XP for Orb 5", 4000, "Consuming Orb grants how much exp?");
        XPforOrb6 = config(OrbandPotion, "XP for Orb 6", 8000, "Consuming Orb grants how much exp?");
        XPforMinorPotion = config(OrbandPotion, "Minor Potion", 1.3f, "XP Multiplier for XP Potion Minor");
        XPforMediumPotion = config(OrbandPotion, "Medium Potion", 1.6f, "XP Multiplier for XP Potion Medium");
        XPforGreaterPotion = config(OrbandPotion, "Greater Potion", 2.0f, "XP Multiplier for XP Potion Greater");
        OrbSEtime = config(OrbandPotion, "Orb SE time", 10, "Orb Consumption SE Icon lasts for default 10 seconds - dont set to 0");
        PotionSEtime = config(OrbandPotion, "Potion SE time", 600, "Potion SE timer last for default of 10 minutes");
        OrbDropChance = config(OrbandPotion, "Orb Drop Chance", 1f, "Chance for Magic Orb to drop from a monster - default 1%");
        OrbDropChancefromBoss = config(OrbandPotion, "Ord Drop Boss", 100f, "Drop Chance for Orbs to drop from a boss - default 100%");
        OrdDropMaxAmountFromBoss = config(OrbandPotion, "Orb Boss Max Amount", 3, "Max Amount of Orbs to drop from Boss if any orbs drop. So there is a chance 1-3 will drop on default");
        UseRegFerm = config(OrbandPotion, "Use Regular Fermentor", false, "For the people that can't handle a little fireworks in their bases, you can use the regular fermentor instead.");

        string NonCombat = "7.Non Combat XP------";

        disableNonCombatObjects = config(NonCombat, "1.Disable All NonCombat XP", false, "Disables all non combat xp.");
        disablePieceXP = config(NonCombat, "Disable Piece XP", false, "Disables Piece XP. You only get xp for once per building, then you have to change to another piece. It still could be abused.");
        disableTameXP = config(NonCombat, "Disable Tame XP", false, "Disables XP for tames.");
        disableFishPickupXP = config(NonCombat, "Disable Fish Pickup XP", false, "Disable Fish pickup XP, you still get xp for catching fish.");
        disableMiningXP = config(NonCombat, "Disable Mining XP", false, "Disable XP for mining.");
        disablePickableXP = config(NonCombat, "Disable Pickup XP", false, "Disable XP for pickables.");
        disableTreeXP = config(NonCombat, "Disable Tree XP", false, "Disable XP for Chopping Trees.");
        disableDestructablesXP = config(NonCombat, "Disable Destructables XP", false, "Disable XP Destructables.");
        debugNonCombatObjects = config(NonCombat, "2.Debug NonCombat Objects", false, "Gives a Warning log for various objects names. Don't forgot that (Clone) is added to everything in the jsons.", false);

        string PVPCombat = "8.PVP Combat XP------";
        displayPlayerLevel = config(PVPCombat, "Display Player Level", "(Lvl<color=blue> @ </color>) ", "Display player Level Next to name.");
        displayPlayerXP = config(PVPCombat, "Display Players XP Worth", " [@ XP]", "Display the current XP a player is worth next to name."  );
        displayDaysAlive = config(PVPCombat, "Display Days Alive", " <color=red>(@ Days Alive)</color>", "Display the how long the player has been alive in Days.");
        enablePVPXP = config(PVPCombat, "Enable PVP XP", true, "Enable PVP XP, victor gets XP of fallen player.");
        xpPerLevelPVP = config(PVPCombat, "XP Player baseline Per Level", 50, "How Much XP is a player worth on defeat by another player per level.");
        xpPerDayNotDead = config(PVPCombat, "XP Player Gains Daily for no Deaths", 10, "How extra a player is worth if they haven't died, per day.");



        Localizer.AddPlaceholder("mmoxpdrink1_description", "power1", XPforMinorPotion, power1 => ((power1 -1)*100).ToString());
        Localizer.AddPlaceholder("mmoxpdrink2_description", "power2", XPforMediumPotion, power2 => ((power2 - 1) * 100).ToString());
        Localizer.AddPlaceholder("mmoxpdrink3_description", "power3", XPforGreaterPotion, power3 => ((power3 - 1) * 100).ToString());


        AnimationSpeedManager.Add((character, speed) => { // animation controller because reasons and stuff
            if (character is not Player player || !player.InAttack() || player.m_currentAttack is null || (LevelSystem.Instance.getParameter(Parameter.Agility) == 0) || player.GetCurrentWeapon().m_shared.m_skillType == Skills.SkillType.Bows)
            {
                return speed;
            }
            return speed * (1+LevelSystem.Instance.getAddAttackSpeed()/100);
        });

        //ConfigSync.value

        _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);

        Assembly assembly = Assembly.GetExecutingAssembly();
        _harmony.PatchAll(assembly);
        
        _asset = GetAssetBundle("epicasset");
        itemassets();
        localizationold = new Localizationold();

        if (Chainloader.PluginInfos.ContainsKey("org.bepinex.plugins.creaturelevelcontrol")){
            CLLCLoaded = true;
        }
    }

    private static void itemassets()
    {
        Item DrinkMinor = new("mmo_xp", "mmo_xp_drink1", "asset");
        DrinkMinor.ToggleConfigurationVisibility(Configurability.Drop);
        Item DrinkMed = new("mmo_xp", "mmo_xp_drink2", "asset");
        DrinkMed.ToggleConfigurationVisibility(Configurability.Drop);
        Item DrinkGreater = new("mmo_xp", "mmo_xp_drink3", "asset");
        DrinkGreater.ToggleConfigurationVisibility(Configurability.Drop);
        Drink1 = DrinkMinor.Prefab.GetComponent<ItemDrop>();
        Drink2 = DrinkMed.Prefab.GetComponent<ItemDrop>();
        Drink3 = DrinkGreater.Prefab.GetComponent<ItemDrop>();



        DataMonsters.InitItems(); // call for obs and add configs

        Item Mead1 = new("mmo_xp", "mmo_mead_minor", "asset");
        Mead1.ToggleConfigurationVisibility(Configurability.Disabled);
        Item Mead2 = new("mmo_xp", "mmo_mead_med", "asset");
        Mead2.ToggleConfigurationVisibility(Configurability.Disabled);
        Item Mead3 = new("mmo_xp", "mmo_mead_greater", "asset");
        Mead3.ToggleConfigurationVisibility (Configurability.Disabled);

        Item Chunks = new("mmo_xp", "Mob_chunks", "asset");

       // Chunks.Snapshot();
        Chunks.RequireOnlyOneIngredient = true;
        Chunks.Crafting.Add(ItemManager.CraftingTable.Cauldron,1);
        Chunks.RequiredItems.Add("TrophyBoar", 4);
        Chunks.RequiredItems.Add("TrophyNeck", 4);
        Chunks.RequiredItems.Add("TrophyDeer", 4);
        Chunks.RequiredItems.Add("TrophyFrostTroll", 2);
        Chunks.RequiredItems.Add("TrophyEikthyr", 2);
        Chunks.RequiredItems.Add("TrophyGreydwarfBrute", 2);
        Chunks.RequiredItems.Add("TrophyGreydwarfShaman", 3);
        Chunks.RequiredItems.Add("TrophyBlob", 3);
        Chunks.RequiredItems.Add("TrophyGreydwarf", 4);
        Chunks.RequiredItems.Add("TrophyDraugr", 3);
        Chunks.RequiredItems.Add("TrophySkeleton", 4);
        Chunks.RequiredItems.Add("TrophySerpent", 2);
        Chunks.RequiredItems.Add("TrophyWolf", 4);


        Mead1.Crafting.Add(ItemManager.CraftingTable.Cauldron, 1);
        Mead1.RequiredItems.Add("Mob_chunks", 1);
        Mead1.RequiredItems.Add("mmo_orb1", 2);
        Mead2.Crafting.Add(ItemManager.CraftingTable.Cauldron, 1);
        Mead2.RequiredItems.Add("Mob_chunks", 1);
        Mead2.RequiredItems.Add("mmo_orb3", 2);
        Mead3.Crafting.Add(ItemManager.CraftingTable.Cauldron, 1);
        Mead3.RequiredItems.Add("Mob_chunks", 1);
        Mead3.RequiredItems.Add("mmo_orb5", 2);

        Mead1D = Mead1.Prefab.GetComponent<ItemDrop>();
        Mead2D = Mead2.Prefab.GetComponent<ItemDrop>();
        Mead3D = Mead3.Prefab.GetComponent<ItemDrop>();


        BuildPiece Ferm = new("mmo_xp", "mmo_fermenter", "asset");
        Ferm.Category.Set(BuildPieceCategory.Crafting);
        Ferm.Crafting.Set(PieceManager.CraftingTable.Forge);
        //Ferm.Snapshot();
        Ferm.RequiredItems.Add("FineWood", 30, true);
        Ferm.RequiredItems.Add("Bronze", 5, true);
        Ferm.RequiredItems.Add("Coins",200,true);

        
        Item ResetTrophy = new("epicmmoitems", "ResetTrophy", "asset");
        ResetTrophy.ToggleConfigurationVisibility(Configurability.Drop);
        //ResetTrophy.Snapshot();

        
    }
    
    internal static void runSSvalues()
    {
        if (EpicMMOSystem.UseRegFerm.Value)
        {
            Fermenter cald = null;
            var list2 = Resources.FindObjectsOfTypeAll<Fermenter>();
            foreach (var item in list2)
            {
                
                if (item.name == "fermenter")
                    cald = item;
            }
            if (cald != null)
            {
                bool contains = false;
                foreach (var conv in cald.m_conversion)
                {
                    if (conv.m_from == Mead1D)
                        contains = true;
                }
                if (!contains)
                {
                    Fermenter.ItemConversion one = new();
                    Fermenter.ItemConversion two = new();
                    Fermenter.ItemConversion three = new();
                    one.m_from = Mead1D;
                    one.m_to = Drink1;
                    one.m_producedItems = 3;

                    two.m_from = Mead2D;
                    two.m_to = Drink2;
                    two.m_producedItems = 3;

                    three.m_from = Mead3D;
                    three.m_to = Drink3;
                    three.m_producedItems = 3;

                    cald.m_conversion.Add(one);
                    cald.m_conversion.Add(two);
                    cald.m_conversion.Add(three);
                }
            }

        }
    }


    [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
    internal static class DBPatMMO {
        internal static void Postfix()
        {
            ObjectDB __instance = ObjectDB.m_instance;


            if (__instance.GetItemPrefab("Wood") == null)
            {
                return;
            }

            Mead1Alt = ScriptableObject.CreateInstance<Recipe>();
            Mead2Alt = ScriptableObject.CreateInstance<Recipe>();
            Mead3Alt = ScriptableObject.CreateInstance<Recipe>();
            Mead1Alt.name = "MMO_Recipe_Mead1Alt";
            Mead2Alt.name = "MMO_Recipe_Mead2Alt";
            Mead3Alt.name = "MMO_Recipe_Mead3Alt";

            var list = Resources.FindObjectsOfTypeAll<CraftingStation>();

            CraftingStation cald = null;
            foreach (var item in list)
            {

                if (item.name == "piece_cauldron")
                    cald = item;
            }
            Mead1Alt.m_craftingStation = cald;
            Mead2Alt.m_craftingStation = cald;
            Mead3Alt.m_craftingStation = cald;

            Mead1Alt.m_item = Mead1D;
            List<Piece.Requirement> reqs1 = new List<Piece.Requirement>();
            Piece.Requirement item1 = new Piece.Requirement
            {
                m_amount = 1,
                m_recover = true,
                m_resItem = __instance.GetItemPrefab("Mob_chunks").GetComponent<ItemDrop>(),
            };
            
            Piece.Requirement item2 = new Piece.Requirement
            {
                m_amount = 1,
                m_recover = true,
                m_resItem = __instance.GetItemPrefab("mmo_orb2").GetComponent<ItemDrop>(),
            };
            reqs1.Add(item1);
            reqs1.Add(item2);

            Mead1Alt.m_resources = reqs1.ToArray();
            __instance.m_recipes.Add(Mead1Alt);


            Mead2Alt.m_item = Mead2D;
            List<Piece.Requirement> reqs2 = new List<Piece.Requirement>();
            Piece.Requirement item3 = new Piece.Requirement
            {
                m_amount = 1,
                m_recover = true,
                m_resItem = __instance.GetItemPrefab("mmo_orb4").GetComponent<ItemDrop>(),
            };
            reqs2.Add(item1);
            reqs2.Add(item3);

            Mead2Alt.m_resources = reqs2.ToArray();
            __instance.m_recipes.Add(Mead2Alt);

            Mead3Alt.m_item = Mead3D;
            List<Piece.Requirement> reqs3 = new List<Piece.Requirement>();
            
            Piece.Requirement item4 = new Piece.Requirement
            {
                m_amount = 1,
                m_recover = true,
                m_resItem = __instance.GetItemPrefab("mmo_orb6").GetComponent<ItemDrop>(),
            };
            reqs3.Add(item1);
            reqs3.Add(item4);

            Mead3Alt.m_resources = reqs3.ToArray();
            __instance.m_recipes.Add(Mead3Alt);
           
        }
    }
    
   
    private void Start()
    {
        MyUI.Init();
        DataMonsters.Init();
        FriendsSystem.Init();
        SetupWatcher();
        
    }

    private void OnDestroy()
    {
        Config.Save();
    }

    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    private static class ZRouteAwake
    {
        private static void Postfix()
        {
            NetisActive = true;
            GameObject[] all = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject val2 in all)
            {
                if (val2.name.Contains("fx_seeker_hurt"))
                {
                    fx_seeker_crit = val2;
                }
            }
        }
    } 


    [HarmonyPatch(typeof(EnvMan), nameof(EnvMan.OnMorning))]
    private static class MorningAwakeMMO
    {
        private static void Postfix()
        {
            int.TryParse(Player.m_localPlayer.m_customData[PlayerAliveString], out int days);
            days = days + 1;
            string daystring = days.ToString();
            Player.m_localPlayer.m_customData[PlayerAliveString] = daystring;

            var zdo = Player.m_localPlayer.m_nview.GetZDO();
            zdo.Set($"{EpicMMOSystem.ModName + EpicMMOSystem.PlayerAliveString}", days);
            ZDOMan.instance.ForceSendZDO(zdo.m_uid);

        }
    }

    private void SetupWatcher()
    { 
            if (!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }

            FileSystemWatcher watcher = new(folderpath); // jsons in config
            watcher.Changed += ReadJsonValues;
            watcher.Created += ReadJsonValues;
            watcher.Renamed += ReadJsonValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;

            FileSystemWatcher watcher2 = new(Paths.ConfigPath, ConfigFileName);
            watcher2.Changed += ReadConfigValues;
            watcher2.IncludeSubdirectories = false;
            watcher2.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher2.EnableRaisingEvents = true;

    }

    private void ReadJsonValues(object sender, FileSystemEventArgs e)
    {
        if (NetisActive)
        {
            if (ZNet.instance.IsServer() ) // only server
            {
                List<string> list = new List<string>();
                foreach (string file in Directory.GetFiles(folderpath, "*.json", SearchOption.AllDirectories))
                {
                    var nam = Path.GetFileName(file);
                    if (EpicMMOSystem.extraDebug.Value)
                        EpicMMOSystem.MLLogger.LogInfo(nam + " read");

                    var temp = File.ReadAllText(file);
                    list.Add(temp);

                }
                DataMonsters.MonsterDBL = list;
                DataMonsters.createNewDataMonsters(list); // for coop and singleplayer

                if (EpicMMOSystem.extraDebug.Value)
                    EpicMMOSystem.MLLogger.LogInfo($"Mobs Updated on Server");

                List<ZNetPeer> peers = ZNet.instance.GetPeers();
                foreach (var peer in peers)
                {
                    if (peer == null) return;
                    ZRoutedRpc.instance.InvokeRoutedRPC(peer.m_uid, $"{EpicMMOSystem.ModName} SetMonsterDB", list); //sync list
                }
            }
        }
    }




        internal void ReadConfigValues(object sender = null, FileSystemEventArgs e = null)
    {
        if (!File.Exists(ConfigFileFullPath)) return;

        var updatelvl = LevelSystem.Instance;
        updatelvl.FillLevelsExp(); // updates when people change levelExp,multiNextLeve, maxlevel

        if (ObjectDB.m_instance != null)
        {
            var orb = ObjectDB.m_instance.GetStatusEffect("MMO_XP".GetStableHashCode());
            orb.m_ttl = OrbSEtime.Value;
            var xpP1 = ObjectDB.m_instance.GetStatusEffect("Potion_MMO_Greater".GetStableHashCode());
            xpP1.m_ttl = PotionSEtime.Value;
            var xpP2 = ObjectDB.m_instance.GetStatusEffect("Potion_MMO_Medium".GetStableHashCode());
            xpP2.m_ttl = PotionSEtime.Value;
            var xpP3 = ObjectDB.m_instance.GetStatusEffect("Potion_MMO_Minor".GetStableHashCode());
            xpP3.m_ttl = PotionSEtime.Value;
        }

    }



    #region ConfigOptions

    private static ConfigEntry<bool>? _serverConfigLocked;

    private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
        bool synchronizedSetting = true)
    {
        ConfigDescription extendedDescription =
            new(
                description.Description +
                (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                description.AcceptableValues, description.Tags);
        ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);
        //var configEntry = Config.Bind(group, name, value, description);

        SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
        syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

        return configEntry;
    }

    private ConfigEntry<T> config<T>(string group, string name, T value, string description,
        bool synchronizedSetting = true)
    {
        return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
    }

    private class ConfigurationManagerAttributes
    {
        public bool? Browsable = false;
    }

    #endregion
        
    public static AssetBundle GetAssetBundle(string filename)
    {
        var execAssembly = Assembly.GetExecutingAssembly();

        string resourceName = execAssembly.GetManifestResourceNames()
            .Single(str => str.EndsWith(filename));

        using (var stream = execAssembly.GetManifestResourceStream(resourceName))
        {
            return AssetBundle.LoadFromStream(stream);
        }
    }
}
    
[BepInPlugin(ModGUID, ModName, VERSION)]
public partial class EpicMMOSystemUI : BaseUnityPlugin
{
    internal const string ModName = "EpicMMOSystemUI";
    internal const string VERSION = EpicMMOSystem.VERSION;
    internal const string Author = EpicMMOSystem.Author;
    private const string ModGUID = Author + "." + ModName; //+ configV; changes GUID
    private static string ConfigFileName = ModGUID + ".cfg";

    internal static EpicMMOSystemUI Instance;
    public static Coroutine coroutine = null!;
    private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
        bool synchronizedSetting = false)
    {
        ConfigDescription extendedDescription =
            new(
                description.Description +
                (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                description.AcceptableValues, description.Tags);
        ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);
        //var configEntry = Config.Bind(group, name, value, description);
        
        return configEntry;
    }

    private ConfigEntry<T> config<T>(string group, string name, T value, string description,
        bool synchronizedSetting = false)
    {
        return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
    }
    

    private void Awake()
    {
        Instance = this;
         string hud = "4.Hud--------------------";
        EpicMMOSystem.oldExpBar = config(hud, "OldXPBar", false, "Use the old eXP Bar only (need restart, not server sync) Does not move or scale");
        EpicMMOSystem.showMaxHp = config(hud, "ShowMaxHp", true, "Show max hp (100 / 100)");
        EpicMMOSystem.forceMagicBar = config(hud, "Force EitrBar", false, "Force the Eitr bar to always show up even when empty");

        EpicMMOSystem.HudPanelPosition = config(hud, "1.0HudGrouplPosition", new Vector2(0, 0), "Position of the Main EpicHudBarBackground in (x,y) - You can drag individual components below");
        EpicMMOSystem.HudExpBackgroundCol = config(hud, "1.1BackgroundCol", "#2F1600", "Background color in Hex, set to 'none' to make transparent");
        EpicMMOSystem.HudBarScale = config(hud, "1.2HudGroupScale", .90f, "Scale for HudGroup - exp, background, hp, stamina, eitr - You can do individual below");
        EpicMMOSystem.ExpPanelPosition = config(hud, "2.0ExpPanelPosition", new Vector2(0, 0), "Position of the Exp panel (x,y)");
        EpicMMOSystem.ExpColor = config(hud, "2.1ExpColor", "#FFFFFF", "Exp fill color in Hex - White bleeds through with purple, set to 'none' to have no xp bar");
        EpicMMOSystem.ExpScale = config(hud, "2.2ExpScale", new Vector3(1, 1, 1), "Exp Bar Scale factor");
        EpicMMOSystem.StaminaPanelPosition = config(hud, "3.0StaminaPanelPosition", new Vector2(0, 0), "Position of the Stamina panel (x,y)");
        EpicMMOSystem.StaminaColor = config(hud, "3.1StaminaColor", "#986100", "Stamina color in Hex, set to 'none' to make vanilla");
        EpicMMOSystem.StaminaScale = config(hud, "3.2StaminaScale", new Vector3(1, 1, 1), "Stamina Bar Scale factor");
        EpicMMOSystem.HpPanelPosition = config(hud, "4.0HpPanelPosition", new Vector2(0, 0), "Position of the Hp panel (x,y)");
        EpicMMOSystem.HpColor = config(hud, "4.1HPColor", "#870000", "HP color in Hex, set to 'none' to make vanilla");
        EpicMMOSystem.HPScale = config(hud, "4.2HPScale", new Vector3(1, 1, 1), "HP Bar Scale factor");
        EpicMMOSystem.EitrPanelPosition = config(hud, "5.0EitrPanelPosition", new Vector2(0, 0), "Position of the Eitr panel (x,y)");
        EpicMMOSystem.EitrColor = config(hud, "5.1EitrColor", "#84257C", "Eitr color in Hex, set to 'none' to make vanilla");
        EpicMMOSystem.EitrScale = config(hud, "5.2EitrScale", new Vector3(1, 1, 1), "Eitr Bar Scale factor");
        EpicMMOSystem.LevelHudGroupScale = config(hud, "6.2LevelHudGroupScale", 1.0f, "LevelHud Group of Objects Scale factor (Nav Bar, Point Hub)");
        EpicMMOSystem.LevelPointPosition = config(hud, "7.0PointHudPosition", new Vector2(0, 0), "Position of the Point panel (x,y)");
        EpicMMOSystem.LevelNavPosition = config(hud, "8.0NavBarPosition", new Vector2(0, 100), "Position of the NavBar (x,y)");

        SetupWatcher();
    }

    private void SetupWatcher()
    {


        FileSystemWatcher watcher2 = new(Paths.ConfigPath, ConfigFileName);
        watcher2.Changed += ReadConfigValuesUI;
        watcher2.IncludeSubdirectories = false;
        watcher2.SynchronizingObject = ThreadingHelper.SynchronizingObject;
        watcher2.EnableRaisingEvents = true;

    }


    IEnumerator Positionpanels()
    {
        yield return new WaitForSeconds(0.5f); // For some reason it doesn't work when directly called in Start(), have to add a small delay
        EpicMMOSystem.MLLogger.LogInfo("Restoring UI Positions now");
        Instance.ReadConfigValuesUI();
        StopCoroutine(coroutine);
    }


    public static void UIReload()
    {

        Color tempC;
        MyUI.expPanelRoot.GetComponent<CanvasScaler>().scaleFactor = EpicMMOSystem.HudBarScale.Value;
        MyUI.levelSystemPanelRoot.GetComponent<CanvasScaler>().scaleFactor = EpicMMOSystem.LevelHudGroupScale.Value;

        if (EpicMMOSystem.HudExpBackgroundCol.Value == "none")
            MyUI.expPanelBackground.SetActive(false);
        else
        {
            MyUI.expPanelBackground.SetActive(true);
            if (ColorUtility.TryParseHtmlString(EpicMMOSystem.HudExpBackgroundCol.Value, out tempC))
                MyUI.expPanelBackground.GetComponent<UnityEngine.UI.Image>().color = tempC;
        }
        MyUI.DisableHPBar = (EpicMMOSystem.HpColor.Value == "none");
        MyUI.DisableStaminaBar = (EpicMMOSystem.StaminaColor.Value == "none");
        MyUI.DisableEitrBar = (EpicMMOSystem.EitrColor.Value == "none");
        MyUI.DisableExpBar = (EpicMMOSystem.ExpColor.Value == "none");


        MyUI.hp.gameObject.SetActive(!MyUI.DisableHPBar);
        MyUI.DHpBar.SetActive(MyUI.DisableHPBar);
        MyUI.IconHpBar.SetActive(MyUI.DisableHPBar);
        MyUI.Exp.gameObject.SetActive(!MyUI.DisableExpBar);
        MyUI.stamina.gameObject.SetActive(!MyUI.DisableStaminaBar);


        if (ColorUtility.TryParseHtmlString(EpicMMOSystem.HpColor.Value, out tempC))
            MyUI.hpImage.color = tempC;
        if (ColorUtility.TryParseHtmlString(EpicMMOSystem.StaminaColor.Value, out tempC))
            MyUI.staminaImage.color = tempC;
        if (ColorUtility.TryParseHtmlString(EpicMMOSystem.EitrColor.Value, out tempC))
            MyUI.EitrImage.color = tempC;
        if (ColorUtility.TryParseHtmlString(EpicMMOSystem.ExpColor.Value, out tempC))
        {
            if (EpicMMOSystem.ExpColor.Value == "#FFFFFF")
                MyUI.eBarImage.color = tempC;
            else
                MyUI.eBarImage.color = tempC * 2;
        }

        MyUI.Exp.GetComponent<RectTransform>().localScale = EpicMMOSystem.ExpScale.Value;
        MyUI.hp.GetComponent<RectTransform>().localScale = EpicMMOSystem.HPScale.Value;
        MyUI.stamina.GetComponent<RectTransform>().localScale = EpicMMOSystem.StaminaScale.Value;
        MyUI.EitrTran.GetComponent<RectTransform>().localScale = EpicMMOSystem.EitrScale.Value;

        try
        {
            if (EpicMMOSystem.extraDebug.Value)
                EpicMMOSystem.MLLogger.LogInfo("MMO Manual ReadConfigValues called");
            
            DragControl.RestoreWindow(MyUI.expPanel.gameObject);
            DragControl.RestoreWindow(MyUI.hp.gameObject);
            DragControl.RestoreWindow(MyUI.Exp.gameObject);
            DragControl.RestoreWindow(MyUI.stamina.gameObject);
            DragControl.RestoreWindow(MyUI.EitrGameObj);
            DragWindowCntrl.RestoreWindow(MyUI.navigationPanel, false);
            DragWindowCntrl.RestoreWindow(MyUI.levelSystemPanel, false);
        }
        catch
        {
            EpicMMOSystem.MLLogger.LogError($"There was an issue loading your {ConfigFileName}");
            EpicMMOSystem.MLLogger.LogError("Please check your config entries for spelling and format!");
        }
    }

    internal void ReadConfigValuesUI(object sender = null, FileSystemEventArgs e = null)
    {
        Config.Reload();
        UIReload();
    }
}
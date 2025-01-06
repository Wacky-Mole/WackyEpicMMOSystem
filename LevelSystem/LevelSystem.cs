using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using BepInEx;
using EpicMMOSystem.Gui;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;
using YamlDotNet.Core.Tokens;
using Random = UnityEngine.Random;

namespace EpicMMOSystem;

/*
 *  Strength:   
    • Player Phys Dmg%
    • Flat Carry Weight
    • Decreased Block Stamina Consumption Rate%
    • Critical Damage
Dexterity:
    • Player Attack/Usage Speed%
    • Decreased Attack Stamina Consumption Rate%
    • Decreased Running/Jumping Stamina Consumption Rate%
Intelligence:
    • Player Ele Dmg%
    • Flat Eitr
    • Eitr Regen Multi%
Endurance:
    • Flat Stamina
    • Stamina Regen Multi% //or// Health Regen Multi%
    • Phys Dmg Reduction%
Vigour: 
    • Flat Health
    • Health Regen
    • Ele Dmg Reduction% 
Specializing
    • Mining Speed
    • Construction piece health?
    • Tree cutting  
*/

[HarmonyPatch(typeof(Game), nameof(Game.SpawnPlayer))]
public static class MainReloadStartSoLoad
{
    static void Postfix(Game __instance)
    {
        EpicMMOSystem.MLLogger.LogInfo("ReLoading exp chart");
        LevelSystem.Instance.FillLevelsExp();
        EpicMMOSystem.runSSvalues();
    }

}
public enum Parameter
{
    Strength = 0, Agility = 1, Intellect = 2, Body = 3, Vigour = 4 , Special = 5, // Strength, Dexterity, Intelligence, Endurance, Vigour, Specializing
}
public partial class LevelSystem
{

    CultureInfo invC = CultureInfo.InvariantCulture;
    #region Singlton
    private static LevelSystem instance;
    public static LevelSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LevelSystem();
                return instance;
            }
            return instance;
        }
    }
    #endregion

    private static Dictionary<int, long> levelsExp;
    private string pluginKey = EpicMMOSystem.ModName;
    private const string midleKey = "LevelSystem";
    private int[] depositPoint = { 0, 0, 0, 0 ,0 , 0}; //6
    private float singleRate = 0;

    public LevelSystem()
    {
       // FillLevelsExp();
    }
    
    public int getLevel()
    {
        if (!Player.m_localPlayer) return 1;
        if (!Player.m_localPlayer.m_knownTexts.ContainsKey($"{pluginKey}_{midleKey}_Level"))
        {
            return 1;
        }
        return int.Parse(Player.m_localPlayer.m_knownTexts[$"{pluginKey}_{midleKey}_Level"]);
    }
    
    private void setLevel(int value)
    {
        if (!Player.m_localPlayer) return;
        Player.m_localPlayer.m_knownTexts[$"{pluginKey}_{midleKey}_Level"]= value.ToString();
    }

    public void recalcLevel()
    {
        var currentexp = getTotalExp();
        setLevel(1);
        FillLevelsExp();

        var need = getNeedExp();
        int addLvl = 0;
        while (currentexp > need)
        {
            currentexp -= need;
            addLvl++;
            need = getNeedExp(addLvl);
        }
        setCurrentExp(currentexp);
        setLevel(addLvl+1);
        MyUI.updateExpBar();

    }
    
    public long getCurrentExp()
    {
        if (!Player.m_localPlayer) return 0;
        if (!Player.m_localPlayer.m_knownTexts.ContainsKey($"{pluginKey}_{midleKey}_CurrentExp"))
        {
            return 0;
        }
        long hold = 0;
        try
        {
             hold = int.Parse(Player.m_localPlayer.m_knownTexts[$"{pluginKey}_{midleKey}_CurrentExp"]);
        }
        catch { Player.m_localPlayer.m_knownTexts[$"{pluginKey}_{midleKey}_CurrentExp"] = "1";
            hold = 1;
            EpicMMOSystem.MLLogger.LogWarning($"Error in getting current exp, setting exp to 1");
        }
        if (hold == 1) // try to restore
        {
            try
            {
                var total = getTotalExp();
                hold = total; // try
                Player.m_localPlayer.m_knownTexts[$"{pluginKey}_{midleKey}_CurrentExp"] = hold.ToString(invC);
            }
            catch
            { }
        }

        return hold;
    }
    
    private void setCurrentExp(long value)
    {
        if (!Player.m_localPlayer) return;
        Player.m_localPlayer.m_knownTexts[$"{pluginKey}_{midleKey}_CurrentExp"]= value.ToString(invC);
    }

    public long getTotalExp()
    {
        if (!Player.m_localPlayer) return 0;
        if (!Player.m_localPlayer.m_knownTexts.ContainsKey($"{pluginKey}_{midleKey}_TotalExp"))
        {
            return 0;
        }

        if (long.TryParse(Player.m_localPlayer.m_knownTexts[$"{pluginKey}_{midleKey}_TotalExp"], out long result))
        {
            return result;
        }
        else
        {
            EpicMMOSystem.MLLogger.LogWarning("error on TotalExp, returned 0");
            return 0;
        }
       
    }

    private void setTotalExp(long value)
    {
        if (!Player.m_localPlayer) return;
        Player.m_localPlayer.m_knownTexts[$"{pluginKey}_{midleKey}_TotalExp"] = value.ToString(invC);
    }
    public void addTotalExp(long value)
    {
        if (!Player.m_localPlayer) return;
        if (!Player.m_localPlayer.m_knownTexts.ContainsKey($"{pluginKey}_{midleKey}_TotalExp"))
        {
            Player.m_localPlayer.m_knownTexts[$"{pluginKey}_{midleKey}_TotalExp"] = "1";
        }
        long total = 1;
        if (long.TryParse((Player.m_localPlayer.m_knownTexts[$"{pluginKey}_{midleKey}_TotalExp"]), out var totalexp)){
            total = totalexp + value;
        }else
        {
            total = value;
        }
        //long total = int.Parse(Player.m_localPlayer.m_knownTexts[$"{pluginKey}_{midleKey}_TotalExp"]) + value;
        Player.m_localPlayer.m_knownTexts[$"{pluginKey}_{midleKey}_TotalExp"] = total.ToString(invC);
    }

    private void setParameter(Parameter parameter, int value)
    {
        if (!Player.m_localPlayer) return;
       // int max = EpicMMOSystem.maxValueAttribute.Value;

        int max = parameter.ToString() switch
        {
            "Strength" => EpicMMOSystem.maxValueStrength.Value,
            "Agility" => EpicMMOSystem.maxValueDexterity.Value,
            "Dexterity" => EpicMMOSystem.maxValueDexterity.Value,
            "Intellect" => EpicMMOSystem.maxValueIntelligence.Value,
            "Body" => EpicMMOSystem.maxValueEndurance.Value,
            "Endurance" => EpicMMOSystem.maxValueEndurance.Value,
            "Vigour" => EpicMMOSystem.maxValueVigour.Value,
            "Special" => EpicMMOSystem.maxValueSpecializing.Value,
            "Specializing" => EpicMMOSystem.maxValueSpecializing.Value,
            _ => max = 205,
        };
        int setValue = Mathf.Clamp(value, 0, max);
        Player.m_localPlayer.m_knownTexts[$"{pluginKey}_{midleKey}_{parameter.ToString()}"] = setValue.ToString();
       // Player.m_localPlayer.m_customData. maybe later
    }
    
    public int getParameter(Parameter parameter)
    {
        
        if (!Player.m_localPlayer) return 0;
        if (!Player.m_localPlayer.m_knownTexts.ContainsKey($"{pluginKey}_{midleKey}_{parameter.ToString()}"))
        {
            return 0;
        }
        int value = int.Parse(Player.m_localPlayer.m_knownTexts[$"{pluginKey}_{midleKey}_{parameter.ToString()}"]);
        //int max = EpicMMOSystem.maxValueAttribute.Value;
        //EpicMMOSystem.MLLogger.LogWarning(parameter.ToString() + " level " + value); // strength is going off like 8 times every second

        int max = parameter.ToString() switch 
        {
            "Strength" => EpicMMOSystem.maxValueStrength.Value,
            "Agility" => EpicMMOSystem.maxValueDexterity.Value,
            "Dexterity" => EpicMMOSystem.maxValueDexterity.Value,
            "Intellect" => EpicMMOSystem.maxValueIntelligence.Value,
            "Body" => EpicMMOSystem.maxValueEndurance.Value,
            "Endurance" => EpicMMOSystem.maxValueEndurance.Value,
             "Vigour" => EpicMMOSystem.maxValueVigour.Value,
            "Special" => EpicMMOSystem.maxValueSpecializing.Value,
            "Specializing" => EpicMMOSystem.maxValueSpecializing.Value,
            _ => max = 205,
        };
        return Mathf.Clamp(value, 0, max);
    }

    public int getFreePoints()
    {
        var levelPoint = EpicMMOSystem.freePointForLevel.Value;
        var freePoint = EpicMMOSystem.startFreePoint.Value;
        var level = getLevel();
        int addPoints = 0;
        try
        {           
            string str = EpicMMOSystem.levelsForBinusFreePoint.Value;
            if (str.IsNullOrWhiteSpace()) { }
            else
            {
                var map = str.Split(',');
                foreach (var value in map)
                {
                    var keyValue = value.Split(':');
                    if (Int32.Parse(keyValue[0]) <= level)
                    {
                        addPoints += Int32.Parse(keyValue[1]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        catch (Exception e)
        {
            EpicMMOSystem.print($"Free point, bonus error: {e.Message}");
        }


        var total = level * levelPoint + freePoint + addPoints;
        int usedUp = 0;
        for (int i = 0; i < EpicMMOSystem.numofCats; i++)
        {
            usedUp += getParameter((Parameter)i);
        }
        int totalpending = getTotalDepositPoints();
        //EpicMMOSystem.MLLogger.LogWarning("totalpending " + totalpending);

        return total - usedUp -totalpending;
    }

    public void addPointsParametr(Parameter parameter, int addPoint)
    {
        var freePoint = getFreePoints();
        if (!(freePoint > 0)) return;
        var applyPoint = Mathf.Clamp(addPoint, 1, freePoint);
        depositPoint[(int)parameter] += applyPoint;
       // var currentPoint = getParameter(parameter); set after apply instead for other mods
       // setParameter(parameter, currentPoint + applyPoint);
    }
    
    public long getNeedExp(int addLvl = 0)
    {
        var lvl = Mathf.Clamp(getLevel() + 1 + addLvl, 1, EpicMMOSystem.maxLevel.Value);
        return levelsExp[lvl];
    }
    
    public void ResetAllParameter()
    {
        for (int i = 0; i < EpicMMOSystem.numofCats; i++)
        {
            setParameter((Parameter)i, 0);
        }
        MyUI.UpdateParameterPanel();
    }

    public void ResetTotalPoints()
    {
        if (!Player.m_localPlayer) return;
        Player.m_localPlayer.m_knownTexts[$"{pluginKey}_{midleKey}_CurrentExp"] = "1";
        Player.m_localPlayer.m_knownTexts[$"{pluginKey}_{midleKey}_TotalExp"] = "1";
        Player.m_localPlayer.m_knownTexts[$"{pluginKey}_{midleKey}_Level"] = "1";
        MyUI.UpdateParameterPanel();
        MyUI.updateExpBar();
    }

    public void ResetAllParameterPayment()
    {

        var text = EpicMMOSystem.prefabNameCoins.Value;
        var pref = ZNetScene.instance.GetPrefab(text);
        var name = pref.GetComponent<ItemDrop>().m_itemData.m_shared.m_name;
        var price = getPriceResetPoints();
        var currentCoins = Player.m_localPlayer.m_inventory.CountItems(name);
        if (currentCoins < price)
        {
            var prefTrophy = ZNetScene.instance.GetPrefab("ResetTrophy");
            var TrophyName = prefTrophy.GetComponent<ItemDrop>().m_itemData;
            var TrophySharedname = TrophyName.m_shared.m_name;
            var currentTrophy = Player.m_localPlayer.m_inventory.CountItems(TrophySharedname);
            if (currentTrophy > 0)
            {
                Player.m_localPlayer.m_inventory.RemoveItem(TrophySharedname, 1);
                ResetAllParameter();
                return;
            }
            return;// return if no coins and no ResetTrophy
        }
        Player.m_localPlayer.m_inventory.RemoveItem(name, price);
        ResetAllParameter();
    }

    public void SetSingleRate(float rate)
    {
        singleRate = rate;
    }

    public void AddExp(int exp, bool noxpMulti=false)
    {
        if(exp < 1)
        {
            if (exp == -2)
            {
                Util.FloatingText(EpicMMOSystem.noExpPastLVLPopup.Value);
            }
            return;
        }
       
        float rate = EpicMMOSystem.rateExp.Value;
        var giveExp = exp * (rate + singleRate);
        if (noxpMulti)
            giveExp = exp;

        if (Player.m_localPlayer.m_seman.HaveStatusEffect("Potion_MMO_Greater".GetStableHashCode()) )
        {
            giveExp = EpicMMOSystem.XPforGreaterPotion.Value * giveExp;
        }
        else if (Player.m_localPlayer.m_seman.HaveStatusEffect("Potion_MMO_Medium".GetStableHashCode()))
        {
            giveExp = EpicMMOSystem.XPforMediumPotion.Value * giveExp;
        }
        else if (Player.m_localPlayer.m_seman.HaveStatusEffect("Potion_MMO_Minor".GetStableHashCode()))
        {
            giveExp = EpicMMOSystem.XPforMinorPotion.Value * giveExp;
        }
        var current = getCurrentExp();
        var need = getNeedExp();
        current += (int)giveExp;
        int addLvl = 0;
        var currentcopy = current;
        while (current > need)
        {
            current -= need;
            addLvl++;
            need = getNeedExp(addLvl);
        }
        if (addLvl > 0)
        {
            AddLevel(addLvl); 
        }
        addTotalExp((int)giveExp);// add to total the exp used to go up levels, this will take a while for people to see benefit as before exp was lost and no way to recalc levels. 
        setCurrentExp(current);
        MyUI.updateExpBar();
        if (EpicMMOSystem.leftMessageXP.Value)
        {
            Player.m_localPlayer.Message(
                MessageHud.MessageType.TopLeft,
                $"{(EpicMMOSystem.localizationold["$get_exp"])}: {(int)giveExp}"
            );
        }
        giveExp = (float)Math.Round(giveExp);
        string stringtolvl = EpicMMOSystem.XPstring.Value;
        stringtolvl = stringtolvl.Replace("@", giveExp.ToString());
        //Util.FloatingText($"+{exp} XP");
        Util.FloatingText($"{stringtolvl}");
    }

    public void AddLevel(int count)
    {
        if (count <= 0) return;
        var current = getLevel();
        current += count;
        setLevel(Mathf.Clamp(current,1, EpicMMOSystem.maxLevel.Value));
        PlayerFVX.levelUp();
        var zdo = Player.m_localPlayer.m_nview.GetZDO();
        zdo.Set($"{pluginKey}_level", current);
        ZDOMan.instance.ForceSendZDO(zdo.m_uid);
    }

    public bool hasDepositPoints()
    {
        bool result = false;
        foreach (var deposit in depositPoint)
        {
            if (deposit > 0)
            {
                result = true;
                break;
            }
        }
        return result;
    }

    public void applyDepositPoints()
    {
        for (int i = 0; i < depositPoint.Length; i++)
        {        
            //var freePoint = getFreePoints();
            //EpicMMOSystem.MLLogger.LogWarning("About to add points , free left" + freePoint);
             //if (!(freePoint > 0)) return; free points have already been alloc at this point from deposit
            if (depositPoint[i] == 0) continue;   
            var parameter = (Parameter)i;
            var deposit = depositPoint[i];
            var applyPoint = deposit;// Mathf.Clamp(deposit, 1, freePoint);
            var currentPoint = getParameter(parameter);
            setParameter(parameter, currentPoint + applyPoint);
            depositPoint[i] = 0;
        }
        MyUI.UpdateParameterPanel();
    }

    public int getDepositPoint(Parameter parameter )
    {
        return depositPoint[(int)parameter];
    }
    public int getTotalDepositPoints()
    {
        var count = 0;
        for (int i = 0; i < depositPoint.Length; i++)
        {
            if (depositPoint[i] == 0) continue;
            count= count+ depositPoint[i];

        }
        return count;
    }
    
    public void cancelDepositPoints()
    {
        if (!Player.m_localPlayer) return;
        for (int i = 0; i < depositPoint.Length; i++)
        {
            if (depositPoint[i] == 0) continue;
            //var parameter = (Parameter)i;
            //var deposit = depositPoint[i];
            //var point = getParameter(parameter);
            //setParameter(parameter, point - deposit);
            depositPoint[i] = 0;
        }
        MyUI.UpdateParameterPanel();
    }

    public int getPriceResetPoints()
    {
        var count = 0;
        for (int i = 0; i < EpicMMOSystem.numofCats; i++)
        {
            count += getParameter((Parameter)i);
        }
        count *= EpicMMOSystem.priceResetPoints.Value;
        return count;
    }

    public void DeathPlayer()
    {
        Player.m_localPlayer.m_customData[EpicMMOSystem.PlayerAliveString] = "0";
        var zdo = Player.m_localPlayer.m_nview.GetZDO();
        zdo.Set($"{EpicMMOSystem.ModName + EpicMMOSystem.PlayerAliveString}", 0);


        if (!EpicMMOSystem.lossExp.Value) return;
        if (!Player.m_localPlayer.HardDeath()) return;
        var minExp = EpicMMOSystem.minLossExp.Value;
        var maxExp = EpicMMOSystem.maxLossExp.Value;
        var lossExp = 1f - Random.Range(minExp, maxExp);
        var TotalExp = getTotalExp();
        
        var currentExp = getCurrentExp();
        long newExp = (long)(currentExp * lossExp);
        setCurrentExp(newExp);
        setTotalExp(TotalExp - (long)(currentExp * lossExp));// remove some totalexp as well
        MyUI.updateExpBar();
        ZDOMan.instance.ForceSendZDO(zdo.m_uid);

    }
    
    //FillLevelExp
    public void FillLevelsExp()
    {


        var levelExp = EpicMMOSystem.levelExp.Value;
        var multiply = EpicMMOSystem.multiNextLevel.Value;
        var maxLevel = EpicMMOSystem.maxLevel.Value;
        levelsExp = new ();
        if (EpicMMOSystem.levelexpforeachlevel.Value)
        {
            long current = 0;
            for (int i = 1; i <= maxLevel; i++)
            {
                current = (long)Math.Round(current * multiply + levelExp);
                levelsExp[i + 1] = current;
            }
        }else
        {
            long current = levelExp;
            for (int i = 1; i <= maxLevel; i++)
            {
                current = (long)Math.Round(current * multiply);
                levelsExp[i + 1] = current;
            }

        }
    }
    
    //Terminal
    public void terminalSetLevel(int value)
    {
        var level = Mathf.Clamp(value, 1, EpicMMOSystem.maxLevel.Value);
        setLevel(level);
        setCurrentExp(0);
        ResetAllParameter();
        PlayerFVX.levelUp();
        MyUI.updateExpBar();
        Player.m_localPlayer.m_customData["epicmmolevel"] = level.ToString();
        var zdo = Player.m_localPlayer.m_nview.GetZDO();
        zdo.Set($"{pluginKey}_level", level);
        ZDOMan.instance.ForceSendZDO(zdo.m_uid);
    }
}

[HarmonyPatch(typeof(Game), nameof(Game.SpawnPlayer))]
[HarmonyPriority(998)]
public static class SetZDOLevel
{
    public static void Postfix()
    { 
        var level = LevelSystem.Instance.getLevel();
        var stringlevel = level.ToString();
        if (Player.m_localPlayer.m_customData.TryGetValue("epicmmolevel", out var playerlevel) == false)       
            Player.m_localPlayer.m_customData.Add("epicmmolevel", stringlevel);

        if (Player.m_localPlayer.m_customData.TryGetValue(EpicMMOSystem.PlayerAliveString, out var val))     
            int.TryParse(val, out int days);     
        else     
            Player.m_localPlayer.m_customData.Add(EpicMMOSystem.PlayerAliveString, "0");
        

        var zdo = Player.m_localPlayer.m_nview.GetZDO();
        zdo.Set($"{EpicMMOSystem.ModName}_level", level);
        zdo.Set($"{EpicMMOSystem.ModName + EpicMMOSystem.PlayerAliveString}", int.Parse(Player.m_localPlayer.m_customData[EpicMMOSystem.PlayerAliveString]));
        ZDOMan.instance.ForceSendZDO(zdo.m_uid);
    }
}

// [HarmonyPatch(typeof(ZNet), nameof(ZNet.RPC_CharacterID))]
// public static class SetZDOPeer
// {
//     public static void Postfix()
//     {
//         foreach (var peer in ZNet.instance.m_peers)
//         {
//             ZDOMan.instance.ForceSendZDO(peer.m_characterID);
//         }
//     }
// }

[HarmonyPatch(typeof(Player), nameof(Player.OnDeath))]
public static class Death
{
    public static void Prefix(Player __instance)
    {    
        if (__instance.m_nview.IsOwner())
        {
            Debug.Log("OnDeath call but not the owner");
            //return;
        }
        LevelSystem.Instance.DeathPlayer();
    }
}

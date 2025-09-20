using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace EpicMMOSystem;

public partial class LevelSystem
{
    public float getAddPhysicDamage(int pointpending = 0)
    {
        var parameter = getParameter(Parameter.Strength) + pointpending;
        var multiplayer = EpicMMOSystem.physicDamage.Value;
        return parameter * multiplayer;
    }
  
   private static int HoldCarryWeightTimes = 0; // this gets called too much for my liking
   private static float HoldCarryWeight = 0;  
    public float getAddWeight(int pointpending = 0)
    {
        if (!Player.m_localPlayer) return 0;

        float HOLD = 0;
        if (HoldCarryWeightTimes == 0)
        {         
            var parameter = getParameter(Parameter.Strength) + pointpending;
            var multiplayer = EpicMMOSystem.addWeight.Value;
            //EpicMMOSystem.MLLogger.LogWarning("Call Strenth 2");Performance enhancement
            HOLD = parameter * multiplayer;
            HoldCarryWeight = HOLD;
        }
        else
            HOLD = HoldCarryWeight;
      
        HoldCarryWeightTimes++;
        if (HoldCarryWeightTimes == 50) // calls like 50 times a second sometimes
            HoldCarryWeightTimes = 0;

        return HOLD;
    }

    public float getReducedStaminaBlock(int pointpending = 0)
    {
        var parameter = getParameter(Parameter.Strength) + pointpending;
        var multiplayer = EpicMMOSystem.staminaBlock.Value;
        return parameter * multiplayer;
    }

    public float getAddCriticalDmg(int pointpending = 0)
    {
        var parameter = getParameter(Parameter.Strength) + pointpending;
        var multiplayer = EpicMMOSystem.critDmg.Value;
        return (float)parameter * multiplayer + EpicMMOSystem.CriticalDefaultDamage.Value;

    }

    public float getAddCriticalChance(int pointpending = 0) // this in in special field, but its nice to see. 
    {
        var parameter = getParameter(Parameter.Special) + pointpending;
        var multiplayer = EpicMMOSystem.critChance.Value ;
        var hello = parameter * multiplayer;
        hello = hello + EpicMMOSystem.startCritChance.Value;
        return hello;
    }


    [HarmonyPatch(typeof(ItemDrop.ItemData), nameof(ItemDrop.ItemData.GetDamage), new[] { typeof(int), typeof(float) })]
    public class AddDamageStrength_Path
    {
        public static void Postfix(ref ItemDrop.ItemData __instance, ref HitData.DamageTypes __result)
        {
            if (Player.m_localPlayer == null) return;
            if (!Player.m_localPlayer.m_inventory.ContainsItem(__instance)) return;
            float add = Instance.getAddPhysicDamage();
            var value = add / 100 + 1;

            __result.m_blunt *= value;
            __result.m_slash *= value;
            __result.m_pierce *= value;
            __result.m_chop *= value;
            __result.m_pickaxe *= value;
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.GetMaxCarryWeight))]
    public class AddWeight_Path
    {
        static void Postfix(ref float __result)
        {         
            var addWeight = Instance.getAddWeight() + EpicMMOSystem.addDefaultWeight.Value;
            float hold = (float)Math.Round(addWeight);
            __result += hold;
        }
    }




    //SEMan.ModifyBlockStaminaUsage

    [HarmonyPatch(typeof(SEMan), nameof(SEMan.ModifyBlockStaminaUsage))]
    static class ModifyBlockStaminaUsage_BlockAttack_Patch
    {
        static void Postfix(float baseStaminaUse, ref float staminaUse)
        {
            staminaUse = staminaUse - ((Instance.getReducedStaminaBlock() / 100)* staminaUse);
        }

    }





        [HarmonyPatch(typeof(Character), nameof(Character.ApplyDamage))] // Crit Dmg
        public class AddCritDmg
        {
            static void Prefix(Character __instance, ref bool showDamageText, ref HitData hit)
            {
            if (__instance != null && hit.HaveAttacker() && __instance.m_faction != 0 && hit.GetAttacker().m_faction == Character.Faction.Players && Player.m_localPlayer == hit.GetAttacker())
            {
                float num = UnityEngine.Random.Range(0f, 100f);
                //EpicMMOSystem.MLLogger.LogInfo("RandChance Crit " +num + " needed "+ Instance.getAddCriticalChance());
                if (num < Instance.getAddCriticalChance())
                {
                    float num2 = 1f + (Instance.getAddCriticalDmg() / 100f);
                    hit.m_damage.m_blunt *= num2;
                    hit.m_damage.m_slash *= num2;
                    hit.m_damage.m_pierce *= num2;
                    hit.m_damage.m_chop *= num2;
                    hit.m_damage.m_pickaxe *= num2;
                    hit.m_damage.m_fire *= num2;
                    hit.m_damage.m_frost *= num2;
                    hit.m_damage.m_lightning *= num2;
                    hit.m_damage.m_poison *= num2;
                    hit.m_damage.m_spirit *= num2;
                    CritDmgVFX vfx = new CritDmgVFX();
                    vfx.CriticalVFX(hit.m_point, hit.GetTotalDamage());
                    showDamageText = false;
                    EpicMMOSystem.MLLogger.LogInfo("You got a Critical Hit with damage of " + hit.GetTotalDamage());
                }
            }
        }
    }





}
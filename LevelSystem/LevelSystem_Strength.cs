using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace EpicMMOSystem;

public partial class LevelSystem
{
    public float getAddPhysicDamage()
    {
        var parameter = getParameter(Parameter.Strength);
        var multiplayer = EpicMMOSystem.physicDamage.Value;
        return parameter * multiplayer;
    }
    
    public float getAddWeight()
    {
        if (!Player.m_localPlayer) return 0;
        var parameter = getParameter(Parameter.Strength);
        var multiplayer = EpicMMOSystem.addWeight.Value;
        return parameter * multiplayer;
    }

    public float getReducedStaminaBlock()
    {
        var parameter = getParameter(Parameter.Strength);
        var multiplayer = EpicMMOSystem.staminaBlock.Value;
        return parameter * multiplayer;
    }

    public float getAddCriticalDmg()
    {
        var parameter = getParameter(Parameter.Strength);
        var multiplayer = EpicMMOSystem.critDmg.Value;
        return (float)parameter * multiplayer + EpicMMOSystem.CriticalDefaultDamage.Value;

    }

    public float getAddCriticalChance() // this in in special field, but its nice to see. 
    {
        var parameter = getParameter(Parameter.Special);
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
            __result += (float)Math.Round(addWeight);
        }
    }


    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.BlockAttack))]
    static class Humanoid_BlockAttack_Patch
    {
        private static float ReturnMyValue()
        {
            return 1 - Instance.getReducedStaminaBlock() / 100;
        }

        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> StaminaBlock(IEnumerable<CodeInstruction> code)
        {
            var method = AccessTools.DeclaredMethod(typeof(Character), nameof(Character.UseStamina));
            var MyMethod = AccessTools.DeclaredMethod(typeof(Humanoid_BlockAttack_Patch), nameof(ReturnMyValue));
            var instructions = new List<CodeInstruction>(code);

            for (int i = 0; i < instructions.Count; i++)
            {
                if (i < instructions.Count - 1)
                {
                    CodeInstruction nextInstruction = instructions[i + 1];

                    if (nextInstruction.opcode == OpCodes.Callvirt && nextInstruction.operand == method)
                    {
                        yield return new CodeInstruction(OpCodes.Call, MyMethod);
                        yield return new CodeInstruction(OpCodes.Mul);
                    }
                }
                yield return instructions[i];
            }
        }
    }





        [HarmonyPatch(typeof(Character), nameof(Character.ApplyDamage))] // Crit Dmg
        public class AddCritDmg
        {
            static void Prefix(Character __instance, ref bool showDamageText, ref HitData hit)
            {
            if (__instance != null && hit.HaveAttacker() && __instance.m_faction != 0 && hit.GetAttacker().m_faction == Character.Faction.Players)
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
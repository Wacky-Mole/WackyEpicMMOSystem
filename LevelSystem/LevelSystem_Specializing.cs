using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace EpicMMOSystem;

public partial class LevelSystem
{

    public float getaddMiningDmg(int pointpending = 0)
    {
        var parameter = getParameter(Parameter.Special) + pointpending;
        var multiplayer = EpicMMOSystem.miningSpeed.Value;
        return parameter * multiplayer;
    }

    public float getAddPieceHealth(int pointpending = 0)
    {
        var parameter = getParameter(Parameter.Special) + pointpending;
        var multiplayer = EpicMMOSystem.constructionPieceHealth.Value;
        return parameter * multiplayer;
    }

    public float getAddTreeCuttingDmg(int pointpending = 0) 
    {
        var parameter = getParameter(Parameter.Special) + pointpending;
        var multiplayer = EpicMMOSystem.treeCuttingSpeed.Value;
        return parameter * multiplayer;
    }





    [HarmonyPatch(typeof(ItemDrop.ItemData), nameof(ItemDrop.ItemData.GetDamage), new[] { typeof(int), typeof(float) }) ]
    private static class MiningPostfix
    {
        private static void Postfix(ItemDrop.ItemData __instance, ref HitData.DamageTypes __result)
        {
            if (__instance != null && __instance.m_shared.m_skillType == Skills.SkillType.Pickaxes) // don't really care about mobs weak to pickaxe, StoneGolems
            {
                float num = 1f + (Instance.getaddMiningDmg() / 100f);
                __result.m_pickaxe *= num;
            }
        }
    }


    [HarmonyPatch(typeof(WearNTear), "OnPlaced")]
    internal static class Player_HealthChange
    {
        internal static void Prefix(ref WearNTear __instance)
        {
            //piece.gameObject.GetComponent<WearNTear>().m_health
            __instance.m_health = Instance.getAddPieceHealth() + __instance.m_health;
            //EpicMMOSystem.MLLogger.LogWarning("health "+ __instance.m_health);

        }
    }



    [HarmonyPatch(typeof(ItemDrop.ItemData), nameof(ItemDrop.ItemData.GetDamage), new[] { typeof(int), typeof(float) })]
    private static class TreeCuttingPostfix
    {
        private static void Postfix(ItemDrop.ItemData __instance, ref HitData.DamageTypes __result)
        {
            // Early exit 1: Null check
            if (__instance == null) return;

            // Early exit 2: Skill check (fastest enums first)
            var skill = __instance.m_shared.m_skillType;
            if (skill != Skills.SkillType.Axes && skill != Skills.SkillType.WoodCutting) return;

            // Early exit 3: Check if we actually have a bonus to apply before doing math/allocations
            // If the Instance isn't ready or GetParameter would be 0, we can skip.
            // Assuming getParameter(Parameter.Special) corresponds to the 'Special' attribute.
            var bonus = Instance.getAddTreeCuttingDmg();
            if (bonus <= 0f) return;

            float num = 1f + (bonus / 100f);
            __result.m_chop *= num;
        }
    }

}
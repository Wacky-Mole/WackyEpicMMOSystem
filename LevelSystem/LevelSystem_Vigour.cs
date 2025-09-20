using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace EpicMMOSystem;

public partial class LevelSystem
{
    public float getAddHp(int pointpending = 0)
    {
        var parameter = getParameter(Parameter.Vigour) + pointpending;
        var multiplayer = EpicMMOSystem.addHp.Value;
        return parameter * multiplayer;
    }

    public float getAddRegenHp(int pointpending = 0)
    {
        var parameter = getParameter(Parameter.Vigour) + pointpending;
        var multiplayer = EpicMMOSystem.regenHp.Value;
        return parameter * multiplayer;
    }

    public float getAddMagicArmor(int pointpending = 0)
    {
        var parameter = getParameter(Parameter.Vigour) + pointpending;
        var multiplayer = EpicMMOSystem.magicArmor.Value;
        return parameter * multiplayer;
    }


    [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
    public static class AddHp_Path
    {
        public static void Postfix(ref float hp)
        {
            var addHp = Instance.getAddHp() + EpicMMOSystem.addDefaultHealth.Value;
            hp += addHp;
        }
    }

    [HarmonyPatch(typeof(SEMan), nameof(SEMan.ModifyHealthRegen))]
    public static class RegenHp_Patch
    {
        public static void Postfix(SEMan __instance, ref float regenMultiplier)
        {
            if (__instance.m_character.IsPlayer())
            {
                float add = Instance.getAddRegenHp();
                regenMultiplier += add / 100;
            }
        }
    }

    [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
    public static class RPC_Damage
    {
        public static void Prefix(Character __instance, HitData hit)
        {
            if (!__instance.IsPlayer()) return;
            if (hit.GetAttacker() == __instance) return;

            float add = Instance.getAddMagicArmor();
            var value = 1 - add / 100;

            hit.m_damage.m_fire *= value;
            hit.m_damage.m_frost *= value;
            hit.m_damage.m_lightning *= value;
            hit.m_damage.m_poison *= value;
            hit.m_damage.m_spirit *= value;
            //hit.m_damage.m_elemntal?
        }
    }


}
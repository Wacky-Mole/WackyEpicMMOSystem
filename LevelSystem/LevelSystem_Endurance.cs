using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace EpicMMOSystem;

public partial class LevelSystem // Body or Endurance
{

    public float getStaminaRegen(int pointpending = 0)
    {
        var parameter = getParameter(Parameter.Body) + pointpending;
        var multiplayer = EpicMMOSystem.staminaRegen.Value;
        return parameter * multiplayer;
    }

    public float getAddStamina(int pointpending = 0)
    {
        var parameter = getParameter(Parameter.Body) + pointpending;
        var multiplayer = EpicMMOSystem.addStamina.Value;
        return parameter * multiplayer;
    }

    public float getAddPhysicArmor(int pointpending = 0)
    {
        var parameter = getParameter(Parameter.Body) + pointpending;
        var multiplayer = EpicMMOSystem.physicArmor.Value;
        return parameter * multiplayer;
    }



    [HarmonyPatch(typeof(SEMan), nameof(SEMan.ModifyStaminaRegen))]
    public static class RegenStamina_Patch
    {
        public static void Postfix(SEMan __instance, ref float staminaMultiplier)
        {
            if (__instance.m_character.IsPlayer())
            {
                float add = Instance.getStaminaRegen();
                staminaMultiplier += add / 100;
            }
        }
    }


    [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
    public static class AddStamina_Path
    {
        public static void Postfix(ref float stamina)
        {
            stamina += Instance.getAddStamina();
        }
    }


    [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
    public static class PhysicArmor_Path
    {
        public static void Prefix(Character __instance, HitData hit)
        {
            if (!__instance.IsPlayer()) return;
            if (hit.GetAttacker() == __instance) return;

            float add = Instance.getAddPhysicArmor();
            var value = 1 - add / 100;

            hit.m_damage.m_blunt *= value;
            hit.m_damage.m_slash *= value;
            hit.m_damage.m_pierce *= value;
            hit.m_damage.m_chop *= value;
            hit.m_damage.m_pickaxe *= value;
        }
    }

}
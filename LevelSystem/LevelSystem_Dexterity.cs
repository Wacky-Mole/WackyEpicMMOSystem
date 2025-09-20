using HarmonyLib;
using System;
using System.Globalization;
using UnityEngine;
namespace EpicMMOSystem;

public partial class LevelSystem
{

    public float getAddAttackSpeed(int pointpending = 0)
    {
        var parameter = getParameter(Parameter.Agility) + pointpending;
        var multiplayer = EpicMMOSystem.attackSpeed.Value;
        return parameter * multiplayer;
    }
    public float getAttackStamina(int pointpending = 0)
    {
        var parameter = getParameter(Parameter.Agility) + pointpending;
        var multiplayer = EpicMMOSystem.attackStamina.Value;
        return parameter * multiplayer;
    }
    
    public float getStaminaReduction(int pointpending = 0)
    {
        var parameter = getParameter(Parameter.Agility) + pointpending;
        var multiplayer = EpicMMOSystem.staminaReduction.Value;
        return parameter * multiplayer;
    }


    /* replaced by Speed Manager by blaxx

     [HarmonyPatch(typeof(CharacterAnimEvent), "CustomFixedUpdate")]
     private static class CharacterAnimEvent_Awake_Patch
     {
         private static void Prefix(CharacterAnimEvent __instance)
         {
             //Bows warning, can be OP easy regardless
            if (Player.m_localPlayer != __instance.m_character) return;
            if (!__instance.m_character.InAttack()) return;
            if (Instance.getParameter(Parameter.Agility) == 0) return; // nothing there don't do

            Player localPlayer = Player.m_localPlayer;
            GameObject val = localPlayer.GetCurrentWeapon()?.m_dropPrefab;
            var skilltype = localPlayer.GetCurrentWeapon().m_shared.m_skillType;
            //EpicMMOSystem.MLLogger.LogWarning(" normal speed " + __instance.m_animator.speed + " for " + skilltype);
           if (skilltype == Skills.SkillType.Bows) return; // no bows

            float animatorSpeed = __instance.m_animator.speed;

            CultureInfo myCIintl = new CultureInfo("en-US", false);
            string number = __instance.m_animator.speed.ToString(myCIintl);
            
            if (number.IndexOf(".") != -1 && number.Length - number.IndexOf(".") > 2)
            {
                // if has 2 decimal places
            }else 
            { 
                // Every anaimatinon speed is different but none that I saw go past the first decimal so 1.5 or 0.2 is valid, need to increase relative too their speed
               // EpicMMOSystem.MLLogger.LogWarning(" adjusting speed ");

                var speed3 = 1.0f;
                speed3 = (Instance.getAddAttackSpeed() * __instance.m_animator.speed )/ 100 + __instance.m_animator.speed + .000001f; // a lets me know that I already modified the number and not go keep grow it. 
                __instance.m_animator.speed = speed3;
                return;

            }
         }
    }
    */


    [HarmonyPatch(typeof(Attack), nameof(Attack.GetAttackStamina))]
    public static class StaminaAttack_Patch
    {
        public static void Postfix(ref float __result)
        {
            var multi = 1 - Instance.getAttackStamina() / 100;
            __result *= multi;
        }
    }


    [HarmonyPatch(typeof(SEMan), nameof(SEMan.ModifyRunStaminaDrain))]
    public static class ModifyRun_Patch
    {
        public static void Postfix(ref float drain)
        {
            var multi = 1 - Instance.getStaminaReduction() / 100;
            drain *= multi;
        }
    }
    
    [HarmonyPatch(typeof(SEMan), nameof(SEMan.ModifyJumpStaminaUsage))]
    public static class ModifyJump_Patch
    {
        public static void Postfix( ref float staminaUse)
        {
            var multi = 1 - Instance.getStaminaReduction() / 100 ;
            staminaUse *= multi;
        }
    } 
   
}
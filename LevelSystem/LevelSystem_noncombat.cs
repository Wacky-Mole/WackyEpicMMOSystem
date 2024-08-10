using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicMMOSystem
{
    internal static class LevelSystem_noncombat
    {
        
        [HarmonyPatch(typeof(Destructible), nameof(Destructible.RPC_Damage))]
        private static class Destructible_dmg_patch
        {
            private static void Postfix(Destructible __instance, HitData hit)
            {
                if (!__instance.m_nview.IsOwner()) return;
                if (__instance.m_health <= 0) return;
                if (!DataMonsters.contains(__instance.gameObject.name)) return;
                Character attacker = hit.GetAttacker();
                if (attacker == null) return;
                if (attacker is not Player player) return;
                if (!hit.CheckToolTier(__instance.m_minToolTier)) return;
                if (hit.GetTotalDamage() < 1) return;
                int expMonster = DataMonsters.getExp(__instance.gameObject.name);
                LevelSystem.Instance.AddExp(expMonster);

            }
        }

        [HarmonyPatch(typeof(MineRock), nameof(MineRock.RPC_Hit))]
        private static class MineRock_dmg_patch
        {
            private static void Postfix(MineRock __instance, HitData hit)
            {
                if (__instance.m_health <= 0) return;
                if (!DataMonsters.contains(__instance.gameObject.name)) return;
                Character attacker = hit.GetAttacker();
                if (attacker == null) return;
                if (attacker is not Player player) return;
                if (!hit.CheckToolTier(__instance.m_minToolTier)) return;
                if (hit.GetTotalDamage() < 1) return;
                int expMonster = DataMonsters.getExp(__instance.gameObject.name);
                LevelSystem.Instance.AddExp(expMonster);
            }
        }

        [HarmonyPatch(typeof(MineRock5), nameof(MineRock5.RPC_Damage))]
        private static class MineRock5_dmg_patch
        {
            private static void Postfix(MineRock5 __instance, HitData hit)
            {
                if (__instance.m_health > 0) return;
                if (!DataMonsters.contains(__instance.gameObject.name)) return;
                Character attacker = hit.GetAttacker();
                if (attacker == null) return;
                if (attacker is not Player player) return;
                if (!hit.CheckToolTier(__instance.m_minToolTier)) return;
                if (hit.GetTotalDamage() < 1) return;
                int expMonster = DataMonsters.getExp(__instance.gameObject.name);
                LevelSystem.Instance.AddExp(expMonster);
            }
        }

        [HarmonyPatch(typeof(TreeBase), nameof(TreeBase.RPC_Damage))]
        private static class TreeBase_dmg_patch
        {
            private static void Postfix(TreeBase __instance, HitData hit)
            {
                if (__instance.m_health <= 0) return;
                if (!DataMonsters.contains(__instance.gameObject.name)) return;
                Character attacker = hit.GetAttacker();
                if (attacker == null) return;
                if (attacker is not Player player) return;
                if (!hit.CheckToolTier(__instance.m_minToolTier)) return;
                if (hit.GetTotalDamage() < 1) return;
                int expMonster = DataMonsters.getExp(__instance.gameObject.name);
                LevelSystem.Instance.AddExp(expMonster);
            }
        }

        [HarmonyPatch(typeof(TreeLog), nameof(TreeLog.RPC_Damage))]
        private static class TreeLog_dmg_patch
        {
            private static void Postfix(TreeLog __instance, HitData hit)
            {
                if (__instance.m_health <= 0) return;
                if (!DataMonsters.contains(__instance.gameObject.name)) return;
                var attacker = hit.GetAttacker();
                if (attacker == null) return;
                if (attacker is not Player player) return;
                if (!hit.CheckToolTier(__instance.m_minToolTier)) return;
                if (hit.GetTotalDamage() < 1) return;
                int expMonster = DataMonsters.getExp(__instance.gameObject.name);
                LevelSystem.Instance.AddExp(expMonster);
            }
        }

        [HarmonyPatch(typeof(Tameable), nameof(Tameable.Tame))]
        private static class Tameable_give_xp_patch
        {
            private static void Postfix(Tameable __instance)
            {
                if (!__instance.m_nview.IsOwner()) return;
                if (!DataMonsters.contains(__instance.gameObject.name)) return;
                Player closestPlayer = Player.GetClosestPlayer(__instance.transform.position, 30f);
                if (!closestPlayer) return;
                int expMonster = DataMonsters.getExp(__instance.gameObject.name);
                LevelSystem.Instance.AddExp(expMonster* EpicMMOSystem.MultiplierForXPTaming.Value);
            }
        }
        [HarmonyPatch(typeof(Player), nameof(Player.PlacePiece))]
        private static class Player_placepiece_patch_epicmmo
        {
            private static void Postfix(Player __instance, Piece piece, ref bool __result)
            {
                if (!piece.m_cultivatedGroundOnly || !__result) return;
                if (!DataMonsters.contains(__instance.gameObject.name)) return;
                int expMonster = DataMonsters.getExp(__instance.gameObject.name);
                LevelSystem.Instance.AddExp(expMonster);
            }
        }

        [HarmonyPatch(typeof(Fish), nameof(Fish.OnHooked))]
        private static class Fish_Caught_patch
        {
            private static void Postfix(Fish __instance)
            {
                if (!__instance) return;
                if (__instance.m_fishingFloat == null) return;
                if (!DataMonsters.contains(__instance.gameObject.name)) return;
                Character owner = __instance.m_fishingFloat.GetOwner();
                if (owner == null) return;
                if (owner is not Player player) return;
                int expMonster = DataMonsters.getExp(__instance.gameObject.name);
                int maxExp = DataMonsters.getMaxExp(__instance.gameObject.name);
                float lvlExp = EpicMMOSystem.expForLvlMonster.Value;
                int monsterLevel = DataMonsters.getLevel(__instance.gameObject.name);
                var resultExp = expMonster + (maxExp * lvlExp * (monsterLevel - 1));
                var exp = Convert.ToInt32(resultExp);
                var playerExp = exp;
                LevelSystem.Instance.AddExp(playerExp);
            }
        }
        
    }
}

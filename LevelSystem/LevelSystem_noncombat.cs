﻿using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace EpicMMOSystem
{

    internal class LevelSystem_noncombat
    {


        [HarmonyPatch(typeof(Destructible), nameof(Destructible.RPC_Damage))]
        private static class Destructible_dmg_patch
        {
            private static void Postfix(Destructible __instance, HitData hit)
            {
                if (EpicMMOSystem.disableNonCombatObjects.Value || EpicMMOSystem.disableDestructablesXP.Value) return;
                if (!__instance.m_nview.IsOwner()) return;
                if (EpicMMOSystem.debugNonCombatObjects.Value)
                    EpicMMOSystem.MLLogger.LogWarning("Destructible name" + __instance.gameObject.name);
                if (!__instance.m_destroyed) return;

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
        internal static bool readytopick = true;
        public static IEnumerator PickWaitme()
        {
            yield return new WaitForSeconds(3f); // 3 second delay to pickables
            readytopick = true;
        }



        [HarmonyPatch(typeof(Pickable), nameof(Pickable.RPC_Pick))]
        private static class PickablePickMMOWacky
        {
            private static void Postfix(Pickable __instance)
            {

                if (EpicMMOSystem.disableNonCombatObjects.Value || EpicMMOSystem.disableMiningXP.Value) return;

                if (!readytopick)
                {
                    return;
                }
                if (EpicMMOSystem.debugNonCombatObjects.Value)
                    EpicMMOSystem.MLLogger.LogWarning("pickable name" + __instance.name);

                if (!DataMonsters.contains(__instance.name)) return;
                int expMonster = DataMonsters.getExp(__instance.name);
                LevelSystem.Instance.AddExp(expMonster);
                readytopick = false;
                EpicMMOSystem.Instance.StartCoroutine(PickWaitme());
            }
        }



        [HarmonyPatch(typeof(MineRock), nameof(MineRock.RPC_Hit))]
        private static class MineRock_dmg_patch
        {
            private static void Postfix(MineRock __instance, HitData hit)
            {
                if (EpicMMOSystem.disableNonCombatObjects.Value || EpicMMOSystem.disableMiningXP.Value) return;
                if (EpicMMOSystem.debugNonCombatObjects.Value)
                    EpicMMOSystem.MLLogger.LogWarning("MineRock name" + __instance.gameObject.name);
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
                if (EpicMMOSystem.disableNonCombatObjects.Value || EpicMMOSystem.disableMiningXP.Value) return;
                if (EpicMMOSystem.debugNonCombatObjects.Value)
                    EpicMMOSystem.MLLogger.LogWarning("MineRock5 name" + __instance.gameObject.name);
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
        internal static Dictionary<PlayerStatType, int> SeeifDied = new() { }; // 0 is default // 1 is active checking // 2 is playerstat was set
        [HarmonyPatch(typeof(Game), nameof(Game.IncrementPlayerStat))]
        private static class CheckPlayerStats
        {
            private static void Postfix(PlayerStatType stat)
            {
                if (SeeifDied.ContainsKey(stat))
                {
                    switch (stat)
                    {
                        case PlayerStatType.Logs:
                            if (SeeifDied[PlayerStatType.Logs] == 1) SeeifDied[PlayerStatType.Logs] = 2;
                            break;
                        case PlayerStatType.Tree:
                            if (SeeifDied[PlayerStatType.Tree] == 1) SeeifDied[PlayerStatType.Tree] = 2;
                            break;

                        case PlayerStatType.LogChops:
                            if (SeeifDied[PlayerStatType.LogChops] == 1) SeeifDied[PlayerStatType.LogChops] = 2;
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(TreeBase), nameof(TreeBase.RPC_Damage))]
        private static class TreeBase_dmg_patch
        {
            private static void Prefix()
            {
                SeeifDied[PlayerStatType.Tree] = 1;
            }
            private static void Postfix(TreeBase __instance, HitData hit)
            {
                if (EpicMMOSystem.disableNonCombatObjects.Value || EpicMMOSystem.disableTreeXP.Value) return;
                if (EpicMMOSystem.debugNonCombatObjects.Value)
                    EpicMMOSystem.MLLogger.LogWarning("Treebase name" + __instance.gameObject.name);
                if (SeeifDied[PlayerStatType.Tree] == 1)
                {
                    SeeifDied[PlayerStatType.Tree] = 0;
                    return;
                }
                SeeifDied[PlayerStatType.Tree] = 0;

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

        [HarmonyPatch(typeof(TreeLog), nameof(TreeLog.RPC_Damage))] // TreeLog has a destroy
        private static class TreeLog_dmg_patch
        {
            private static void Prefix()
            {
                SeeifDied[PlayerStatType.Logs] = 1;
                SeeifDied[PlayerStatType.LogChops] = 1;
            }
            private static void Postfix(TreeLog __instance, HitData hit)
            {
                if (EpicMMOSystem.disableNonCombatObjects.Value || EpicMMOSystem.disableTreeXP.Value) return;
                if (!__instance.m_nview.IsOwner()) return;
                if (EpicMMOSystem.debugNonCombatObjects.Value)
                    EpicMMOSystem.MLLogger.LogWarning("TreeLog name" + __instance.gameObject.name);
                /*if (SeeifDied[PlayerStatType.Logs] == 1 || SeeifDied[PlayerStatType.LogChops] == 1)
                {
                    SeeifDied[PlayerStatType.LogChops] = 0;
                    return;
                }
                SeeifDied[PlayerStatType.LogChops] = 0;
                SeeifDied[PlayerStatType.Logs] = 0; */
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
                if (!__instance.m_nview.IsOwner() || EpicMMOSystem.disableTameXP.Value) return;
                
                if (!DataMonsters.contains(__instance.gameObject.name)) return;
                Player closestPlayer = Player.GetClosestPlayer(__instance.transform.position, 30f);
                if (!closestPlayer) return;
                int expMonster = DataMonsters.getExp(__instance.gameObject.name);
                LevelSystem.Instance.AddExp(expMonster * EpicMMOSystem.MultiplierForXPTaming.Value);
            }
        }

        private static string lastpiece = "";
        [HarmonyPatch(typeof(Player), nameof(Player.PlacePiece))]
        private static class Player_placepiece_patch_epicmmoA
        {
            private static void Postfix(Player __instance, Piece piece)
            {
                if (EpicMMOSystem.disableNonCombatObjects.Value || EpicMMOSystem.disablePieceXP.Value) return;
                if (piece.m_cultivatedGroundOnly)
                {
                    if (EpicMMOSystem.debugNonCombatObjects.Value)
                        EpicMMOSystem.MLLogger.LogWarning("plant name" + piece.name);

                    if (!DataMonsters.contains(piece.name + "(Clone)")) return;
                    int expMonster = DataMonsters.getExp(piece.name + "(Clone)");
                    LevelSystem.Instance.AddExp(expMonster);
                }
                else
                {
                    if (lastpiece == piece.name) return;
                    lastpiece = piece.name;
                    if (EpicMMOSystem.debugNonCombatObjects.Value)
                        EpicMMOSystem.MLLogger.LogWarning("piece name" + piece.name);
                    //if (!__result) return;
                    if (!DataMonsters.contains(piece.name + "(Clone)")) return;
                    int expMonster = DataMonsters.getExp(piece.name + "(Clone)");
                    LevelSystem.Instance.AddExp(expMonster);
                }
            }
        }

        [HarmonyPatch(typeof(Fish), nameof(Fish.OnHooked))]
        private static class Fish_Hook_patch
        {
            private static void Postfix(Fish __instance)
            {
                if (EpicMMOSystem.disableNonCombatObjects.Value) return;
                // if (!__instance) return;
                if (__instance.m_fishingFloat == null) return;
                if (EpicMMOSystem.debugNonCombatObjects.Value)
                    EpicMMOSystem.MLLogger.LogWarning("fish name" + __instance.gameObject.name);

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
        private static string lastfishname = "";
        [HarmonyPatch(typeof(Fish), nameof(Fish.Interact))]
        private static class Fish_Caught_patch_interact
        {
            private static void Postfix(Fish __instance, Humanoid character, bool repeat, bool alt)
            {
                if (EpicMMOSystem.disableNonCombatObjects.Value || EpicMMOSystem.disableFishPickupXP.Value) return;
                if (!__instance) return;

                if (repeat)
                {
                    return ;
                }

                if (character == null) return;
                if (character is not Player player) return;


                if (EpicMMOSystem.debugNonCombatObjects.Value)
                    EpicMMOSystem.MLLogger.LogWarning("fish Interact name" + __instance.name);
                if (!DataMonsters.contains(__instance.name)) return;

                if (__instance.gameObject.name == lastfishname) return; // cant pickup fish over and over
                lastfishname = __instance.gameObject.name;

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

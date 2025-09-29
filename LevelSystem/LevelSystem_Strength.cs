using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace EpicMMOSystem;

public partial class LevelSystem
{

    [HarmonyPatch(typeof(Game), nameof(Game.Start))]
    public static class RegisterRpcStrength
    {
        public static void Postfix()
        {
            ZRoutedRpc.instance.Register($"{EpicMMOSystem.ModName} GiveCritToPlayer", new Action<long, ZPackage>(RPC_GiveCritToPlayer));
        }
    }

    private static void RPC_GiveCritToPlayer(long sender, ZPackage pkg)
    {
        try
        {
            if (!Player.m_localPlayer) return;

            Vector3 hitPoint = pkg.ReadVector3();
            float totalDamage = pkg.ReadSingle();

            // Play the same crit VFX and suppress default damage text here on the attacker’s client
            CritDmgVFX vfx = new CritDmgVFX();
            vfx.CriticalVFX(hitPoint, totalDamage);

            // Optional: lightweight message for feedback (comment out if too spammy)
            // Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, $"Critical! ({Mathf.RoundToInt(totalDamage)})");

            EpicMMOSystem.MLLogger.LogInfo($"You recieved a Critical Hit from the combat owner: ");
        }
        catch (Exception e)
        {
            EpicMMOSystem.MLLogger.LogWarning($"Bug catch RPC_GiveCritToPlayer: {e}");
        }
    }

    // ---------- Small helper to get a player's peerId for routed RPCs ----------
    private static bool TryGetPeerId(Player p, out long peerId)
    {
        peerId = 0;
        if (!p) return false;
        var zdo = p.m_nview ? p.m_nview.GetZDO() : null;
        if (zdo == null) return false;
        peerId = zdo.m_uid.UserID; // owner peer id
        return peerId != 0;
    }





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
            // Basic guards
            if (__instance == null || !hit.HaveAttacker()) return;

            // Only care when attacker is players and target is not player-faction 0 (i.e., not "None")
            var attacker = hit.GetAttacker();
            if (attacker == null) return; // attacker may despawn or be null
            if (__instance.m_faction == Character.Faction.Players) return; // don't crit allies; adjust if you want PvP crits
            if (attacker.m_faction != Character.Faction.Players) return;

            // Calculate crit roll using the *attacker's* stats (local or remote)
            float roll = UnityEngine.Random.Range(0f, 100f);
            if (roll >= Instance.getAddCriticalChance()) return;

            // Apply crit multiplier to all channels
            float mult = 1f + (Instance.getAddCriticalDmg() / 100f);
            hit.m_damage.m_blunt *= mult;
            hit.m_damage.m_slash *= mult;
            hit.m_damage.m_pierce *= mult;
            hit.m_damage.m_chop *= mult;
            hit.m_damage.m_pickaxe *= mult;
            hit.m_damage.m_fire *= mult;
            hit.m_damage.m_frost *= mult;
            hit.m_damage.m_lightning *= mult;
            hit.m_damage.m_poison *= mult;
            hit.m_damage.m_spirit *= mult;

            // Local feedback if the local player is the attacker
            if (Player.m_localPlayer && attacker == Player.m_localPlayer)
            {
                CritDmgVFX vfx = new CritDmgVFX();
                vfx.CriticalVFX(hit.m_point, hit.GetTotalDamage());
                showDamageText = false; // we show our own crit text via VFX
                EpicMMOSystem.MLLogger.LogInfo($"You got a Critical Hit: {hit.GetTotalDamage():0}");
                return;
            }

            // If attacker is another player, send them the crit VFX via routed RPC
            if (attacker is Player p1 && TryGetPeerId(p1, out long peerId))
            {
                var pkg = new ZPackage();
                pkg.Write(hit.m_point);
                pkg.Write(hit.GetTotalDamage());

                // Fire-and-forget to the attacker’s client
                ZRoutedRpc.instance.InvokeRoutedRPC(
                    peerId,
                    $"{EpicMMOSystem.ModName} GiveCritToPlayer",
                    new object[] { pkg }
                );

                // Keep local damage text minimal; VFX will appear on the attacker’s side
                EpicMMOSystem.MLLogger.LogInfo($"Sent crit VFX to {p1.GetHoverName()} for {hit.GetTotalDamage():0}");
            }
        }
    }



}
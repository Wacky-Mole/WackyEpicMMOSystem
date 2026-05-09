using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using Groups;
using HarmonyLib;
using ItemManager;
using UnityEngine;
//using UnityEngine.UIElements;



namespace EpicMMOSystem;

public static class MonsterDeath_Path
{
    private static readonly Dictionary<Character, long> CharacterLastDamageList = new();
    private static readonly Dictionary<Character, Dictionary<long, float>> CharacterDamageBySender = new();

    private static void TrackCharacterDamage(Character character, long sender, float damage)
    {
        if (character == null || sender <= 0 || damage <= 0f) return;

        if (!CharacterDamageBySender.TryGetValue(character, out var damageBySender))
        {
            damageBySender = new Dictionary<long, float>();
            CharacterDamageBySender[character] = damageBySender;
        }

        if (damageBySender.TryGetValue(sender, out float currentDamage))
        {
            damageBySender[sender] = currentDamage + damage;
        }
        else
        {
            damageBySender[sender] = damage;
        }
    }

    private static bool TryGetBestDamageSender(Character character, out long sender)
    {
        sender = 0L;

        if (!CharacterDamageBySender.TryGetValue(character, out var damageBySender) || damageBySender.Count == 0)
        {
            return false;
        }

        float bestDamage = float.MinValue;
        foreach (var entry in damageBySender)
        {
            if (entry.Value > bestDamage)
            {
                bestDamage = entry.Value;
                sender = entry.Key;
            }
        }

        return sender > 0L;
    }

    private static bool TryGetKillCreditSender(Character character, out long sender)
    {
        if (TryGetBestDamageSender(character, out sender))
        {
            return true;
        }

        return CharacterLastDamageList.TryGetValue(character, out sender) && sender > 0L;
    }

    private static void ClearCharacterDamageTracking(Character character)
    {
        CharacterLastDamageList.Remove(character);
        CharacterDamageBySender.Remove(character);
    }



    [HarmonyPatch(typeof(Game), nameof(Game.Start))]
    public static class RegisterRpc
    {
        
        public static void Postfix()
        {
            ZRoutedRpc.instance.Register($"{EpicMMOSystem.ModName} DeadMonsters", new Action<long, ZPackage>(RPC_DeadMonster));
            ZRoutedRpc.instance.Register($"{EpicMMOSystem.ModName} AddGroupExp", new Action<long, int, Vector3, int>(RPC_AddGroupExp));
        }
    }


    public static void RPC_AddGroupExp(long sender, int exp, Vector3 position, int monsterLevel)
    {
        try
        {
            if (EpicMMOSystem.extraDebug.Value)
                EpicMMOSystem.MLLogger.LogInfo("Player was in group so applying exp from group kill");

            float groupRange = EpicMMOSystem.groupRange.Value;
            if ((position - Player.m_localPlayer.transform.position).sqrMagnitude >= groupRange * groupRange) return;

            var playerExp = exp;
            var MobisBoss = false;
            if (monsterLevel < 0)
            {
                MobisBoss = true;
                monsterLevel = -1 * monsterLevel; // or -monsterLevel
            }

            if (EpicMMOSystem.enabledLevelControl.Value && (EpicMMOSystem.curveExp.Value || MobisBoss && EpicMMOSystem.curveBossExp.Value || EpicMMOSystem.noExpPastLVL.Value) && monsterLevel != 0)
            {
                if (EpicMMOSystem.extraDebug.Value)
                    EpicMMOSystem.MLLogger.LogInfo("Checking player lvl for group exp");

                int maxRangeLevel = LevelSystem.Instance.getLevel() + EpicMMOSystem.maxLevelExp.Value;
                if (monsterLevel > maxRangeLevel)
                {
                    if (EpicMMOSystem.noExpPastLVL.Value)
                        playerExp = 0; // no exp
                    else if (EpicMMOSystem.curveExp.Value)
                        playerExp = Convert.ToInt32(exp / (monsterLevel - maxRangeLevel));
                    else if (MobisBoss && EpicMMOSystem.curveBossExp.Value)
                        playerExp = Convert.ToInt32(exp / (monsterLevel - maxRangeLevel));
                }
                int minRangeLevel = LevelSystem.Instance.getLevel() - EpicMMOSystem.minLevelExp.Value;
                if (monsterLevel < minRangeLevel)
                {
                    if (EpicMMOSystem.noExpPastLVL.Value)
                        playerExp = 0; // no exp
                    else if (EpicMMOSystem.curveExp.Value)
                        playerExp = Convert.ToInt32(exp / (minRangeLevel - monsterLevel));
                    else if (MobisBoss && EpicMMOSystem.curveBossExp.Value)
                        playerExp = Convert.ToInt32(exp / (minRangeLevel - monsterLevel));
                }

                if (monsterLevel > maxRangeLevel && EpicMMOSystem.mentor.Value)
                    playerExp = exp; // give full *group exp with mentor mode
            }

            if (playerExp > 0)
                LevelSystem.Instance.AddExp(playerExp);
        }
        catch { EpicMMOSystem.MLLogger.LogWarning("Bug catch RPC_AddGroupExp"); }
    }
    

    public static void RPC_DeadMonster(long sender, ZPackage pkg)
    {

        if (!Player.m_localPlayer) return;
        if(Player.m_localPlayer.IsDead()) return;
        string monsterName = pkg.ReadString();
        int level = pkg.ReadInt();
        bool isBoss = pkg.ReadBool();
        Vector3 position = pkg.ReadVector3();
        bool playerdead  = false;
        var MobisBoss = isBoss;
        int monsterLevel = 1;
        int playerExp = 0;
        int exp = 0;

        if (monsterName == "Player(Clone)")
        {
            EpicMMOSystem.MLLogger.LogInfo("You Killed Player - PVP");
            playerdead = true;
            // monsterLevel = level;
            playerExp = level;
            LevelSystem.Instance.AddExp(playerExp, true);
        }
        else
        {
            if (!DataMonsters.contains(monsterName))
            {
                EpicMMOSystem.print($"{EpicMMOSystem.ModName}: Can't find monster {monsterName}");
                return;
            }

            monsterLevel = DataMonsters.getLevel(monsterName);

            if (EpicMMOSystem.mobLvlPerStar.Value)
            {
                monsterLevel = monsterLevel + level - 1;
            }

            if (DataMonsters.getLevel(monsterName) == 0)
                monsterLevel = 0;


            float playerRange = EpicMMOSystem.playerRange.Value;
            if ((position - Player.m_localPlayer.transform.position).sqrMagnitude >= playerRange * playerRange) return;

            int expMonster = DataMonsters.getExp(monsterName);
            int maxExp = DataMonsters.getMaxExp(monsterName);
            float lvlExp = EpicMMOSystem.expForLvlMonster.Value;
            var resultExp = expMonster + (maxExp * lvlExp * (level - 1));
            exp = Convert.ToInt32(resultExp);
            playerExp = exp;
       

            if (EpicMMOSystem.enabledLevelControl.Value && (EpicMMOSystem.curveExp.Value || MobisBoss && EpicMMOSystem.curveBossExp.Value || EpicMMOSystem.noExpPastLVL.Value) && monsterLevel != 0)
            {
                if (EpicMMOSystem.extraDebug.Value) 
                    EpicMMOSystem.MLLogger.LogInfo("Checking player lvl");

                int maxRangeLevel = LevelSystem.Instance.getLevel() + EpicMMOSystem.maxLevelExp.Value;
                if (monsterLevel > maxRangeLevel)
                {
                    if (EpicMMOSystem.noExpPastLVL.Value)
                        playerExp = -2;// no exp
                    else if (EpicMMOSystem.curveExp.Value)
                        playerExp = Convert.ToInt32(exp / (monsterLevel - maxRangeLevel));
                    else if (MobisBoss && EpicMMOSystem.curveBossExp.Value)
                        playerExp = Convert.ToInt32(exp / (monsterLevel - maxRangeLevel));
                }
                int minRangeLevel = LevelSystem.Instance.getLevel() - EpicMMOSystem.minLevelExp.Value;
                if (monsterLevel < minRangeLevel)
                {
                    if (EpicMMOSystem.noExpPastLVL.Value)
                        playerExp = -2; // no exp
                    else if (EpicMMOSystem.curveExp.Value)
                        playerExp = Convert.ToInt32( exp / (minRangeLevel - monsterLevel));
                    else if (MobisBoss && EpicMMOSystem.curveBossExp.Value)
                        playerExp = Convert.ToInt32(exp / (minRangeLevel - monsterLevel));
                }
            }      
            LevelSystem.Instance.AddExp(playerExp);   
        }
        if (!Groups.API.IsLoaded()) return;

        if (EpicMMOSystem.extraDebug.Value)
            EpicMMOSystem.MLLogger.LogInfo("Player in Group");

        //Convert Monsterlvl to negative if boss because max send amount is 3 para
        if (MobisBoss && monsterLevel != 0)
            monsterLevel = -1 * monsterLevel;

        var groupFactor = EpicMMOSystem.groupExp.Value;
        string localPlayerName = Player.m_localPlayer.GetPlayerName();
        foreach (var playerReference in Groups.API.GroupPlayers())
        {
            if (playerReference.name != localPlayerName && exp > 0)
            {
                var sendExp = exp * groupFactor;
                ZRoutedRpc.instance.InvokeRoutedRPC(
                    playerReference.peerId, 
                    $"{EpicMMOSystem.ModName} AddGroupExp", 
                    new object[] { (int)sendExp, position, monsterLevel }
                    );
            }
        }
    }

    [HarmonyPatch(typeof(Character), nameof(Character.Damage))]
    public static class ModifierDamage
    {
        public static void Prefix(Character __instance, HitData hit) // maybe check for tames as well to prevent them from hurting high lvl
        {
            if (__instance.IsPlayer() && hit.GetAttacker() is Player attackPlayer)
            {
                Player defendPlayer = __instance as Player;
                if (defendPlayer == null) return;

                var defenderZdo = defendPlayer.m_nview?.GetZDO();
                var attackerZdo = attackPlayer.m_nview?.GetZDO();
                if (defenderZdo == null || attackerZdo == null) return;

                string playerDefendingName = defendPlayer.GetPlayerName();
                int defenderLevel = defenderZdo.GetInt($"{EpicMMOSystem.ModName}_level", 1);
                int attackerLevel = attackerZdo.GetInt($"{EpicMMOSystem.ModName}_level", 1);

                if (defenderLevel > 0 && attackerLevel > 0)
                {
                    int allowedRange = EpicMMOSystem.pvpPlayerRange.Value;
                    int levelDifference = Math.Abs(attackerLevel - defenderLevel);

                    if (levelDifference > allowedRange)
                    {
                        defendPlayer.Message(MessageHud.MessageType.TopLeft,
                            $"PvP blocked: level difference too high (Range: {allowedRange})");

                        EpicMMOSystem.MLLogger.LogInfo( $"PvP blocked: level difference too high (Range: {allowedRange})");

                        attackPlayer.Message(MessageHud.MessageType.TopLeft,
                            $"Your level difference with {playerDefendingName} is too high for PvP (Range: {allowedRange})");
                        hit.ApplyModifier(0);
                    }

                }
            }
                

            if (!EpicMMOSystem.enabledLevelControl.Value) return;
            //if (EpicMMOSystem.removeDrop.Value) hit.m_toolTier = LevelSystem.Instance.getLevel(); // using toolTier to pass the Lvl of player         
            hit.m_toolTier = (short)LevelSystem.Instance.getLevel();
            if (EpicMMOSystem.lowDamageLevel.Value)
            {
                if (__instance.IsPlayer()) return;
                if (__instance.IsTamed()) return;

                if (!DataMonsters.contains(__instance.gameObject.name)) return;
                int playerLevel = LevelSystem.Instance.getLevel();
                int maxLevelExp = playerLevel + EpicMMOSystem.maxLevelExp.Value +EpicMMOSystem.lowDamageExtraConfig.Value;
                int monsterLevel = DataMonsters.getLevel(__instance.gameObject.name); 
                if (EpicMMOSystem.mobLvlPerStar.Value)
                {
                    monsterLevel = monsterLevel + __instance.m_level - 1;
                }

                if (DataMonsters.getLevel(__instance.gameObject.name) == 0)
                    return;
                if (monsterLevel > maxLevelExp)
                {
                    int i = Mathf.Clamp(4, 1, 3);
                    var damageFactor = Mathf.Clamp( (float)((playerLevel + EpicMMOSystem.lowDamageExtraConfig.Value) / monsterLevel),0.1f, 1.0f );
                    hit.ApplyModifier(damageFactor);
                }
            }
        }
    }

    private static bool lasthitplayer = false;

    private static void SendKillCredit(Character character, long attacker)
    {
        var pkg = new ZPackage();
        pkg.Write(character.gameObject.name);

        if (character.gameObject.name == "Player(Clone)" && lasthitplayer)
        {
            if (!EpicMMOSystem.enablePVPXP.Value) return;
            Player player = character as Player;
            if (player != null)
            {
                string playerName = player.GetPlayerName();
                EpicMMOSystem.MLLogger.LogWarning(playerName + " Player was killed pvp ");
                var zdopla = player.m_nview.GetZDO();
                int daysalive = zdopla.GetInt(EpicMMOSystem.ModName + EpicMMOSystem.PlayerAliveString, -1);
                if (daysalive == -1)
                {
                    EpicMMOSystem.MLLogger.LogWarning("Days alive not found" + daysalive);
                    daysalive = 0;
                }
                int level = zdopla.GetInt($"{EpicMMOSystem.ModName}_level", 1);
                int xpworth = (level * EpicMMOSystem.xpPerLevelPVP.Value) + (daysalive * EpicMMOSystem.xpPerDayNotDead.Value);
                pkg.Write(xpworth);
                pkg.Write(false);
            }
            else
            {
                EpicMMOSystem.MLLogger.LogWarning("Didnt find player");
                return;
            }
        }
        else
        {
            pkg.Write(character.GetLevel());
            pkg.Write(character.GetFaction() == Character.Faction.Boss);
        }

        pkg.Write(character.transform.position);
        ZRoutedRpc.instance.InvokeRoutedRPC(attacker, $"{EpicMMOSystem.ModName} DeadMonsters", new object[] { pkg });
        ClearCharacterDamageTracking(character);
    }

    [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
    static class QuestEnemyKill
    {
        static void Prefix(Character __instance, long sender, HitData hit)
        {
            if (__instance == null || hit == null || hit.m_damage == null) return;

            if (__instance.GetHealth() <= 0) return;

            var nview = __instance.m_nview;
            bool nviewValid = nview != null && nview.IsValid();
            var zdo = nviewValid ? nview.GetZDO() : null;
            bool hasSEMan = __instance.m_seman != null;

            if (!hasSEMan || zdo == null)
            {
                var dmg = hit.m_damage;
                if (dmg.m_frost > 0f) dmg.m_frost = 0f;
                if (dmg.m_spirit > 0f) dmg.m_spirit = 0f;
                if (dmg.m_fire > 0f) dmg.m_fire = 0f;
                if (dmg.m_poison > 0f) dmg.m_poison = 0f;
                if (dmg.m_lightning > 0f) dmg.m_lightning = 0f;
            }

            bool bossDropFlag = __instance.GetFaction() == Character.Faction.Boss;
            var attacker = hit.GetAttacker();
            if (!attacker) return;

            lasthitplayer = attacker.IsPlayer();
            if (attacker.IsPlayer() || ((attacker.IsTamed() || attacker.name == "staff_greenroots_tentaroot(Clone)" || attacker.name == "Staff_root_TW(Clone)") && EpicMMOSystem.tamesGiveXP.Value))
            {
                TrackCharacterDamage(__instance, sender, hit.GetTotalDamage());
                CharacterLastDamageList[__instance] = sender;

                if (EpicMMOSystem.enabledLevelControl.Value && (EpicMMOSystem.removeBossDropMax.Value || EpicMMOSystem.removeBossDropMin.Value) && bossDropFlag)
                {
                    __instance.m_nview.GetZDO().Set("epic playerLevel", hit.m_toolTier);
                }
                else if (EpicMMOSystem.enabledLevelControl.Value && (EpicMMOSystem.removeDropMax.Value || EpicMMOSystem.removeDropMin.Value) && !bossDropFlag)
                {
                    __instance.m_nview.GetZDO().Set("epic playerLevel", -hit.m_toolTier);
                }
                else if (EpicMMOSystem.enabledLevelControl.Value && EpicMMOSystem.removeAllDropsFromNonPlayerKills.Value && attacker.IsTamed())
                {
                    __instance.m_nview.GetZDO().Set("epic playerLevel", -hit.m_toolTier);
                }
                else
                {
                    if (EpicMMOSystem.extraDebug.Value)
                        EpicMMOSystem.MLLogger.LogInfo("else ZDO epic playerLevel to 0");

                    if (0 != __instance.m_nview.GetZDO().GetInt("epic playerLevel"))
                    {
                        __instance.m_nview.GetZDO().Set("epic playerLevel", 0);
                        if (EpicMMOSystem.extraDebug.Value)
                            EpicMMOSystem.MLLogger.LogInfo("Set ZDO epic playerLevel to 0");
                    }
                }
            }
            else if (!attacker.IsTamed())
            {
                if (EpicMMOSystem.enabledLevelControl.Value && (EpicMMOSystem.removeBossDropMax.Value || EpicMMOSystem.removeBossDropMin.Value || EpicMMOSystem.removeDropMax.Value || EpicMMOSystem.removeDropMin.Value || EpicMMOSystem.removeAllDropsFromNonPlayerKills.Value))
                {
                    __instance.m_nview.GetZDO().Set("epic playerLevel", 1000512);
                }
            }
        }

        static void Postfix(Character __instance, long sender, HitData hit)
        {
            if (__instance.IsTamed()) return;
            if (__instance.GetHealth() <= 0f)
            {
                if (!TryGetKillCreditSender(__instance, out long attacker))
                {
                    ClearCharacterDamageTracking(__instance);
                    return;
                }

                SendKillCredit(__instance, attacker);
            }
        }
    }

    [HarmonyPatch(typeof(Character), nameof(Character.ApplyDamage))]
    public static class ApplyDamage
    {
        public static void Postfix(Character __instance, HitData hit)
        {
            if (__instance.IsTamed()) return;
            if (__instance.GetHealth() <= 0f && TryGetKillCreditSender(__instance, out long attacker))
            {
                SendKillCredit(__instance, attacker);
            }
        }
    }

    [HarmonyPatch(typeof(Character), nameof(Character.OnDestroy))]
    static class Character_OnDestroy_Patch
    {
        static void Postfix(Character __instance)
        {
            ClearCharacterDamageTracking(__instance);
        }
    }
}

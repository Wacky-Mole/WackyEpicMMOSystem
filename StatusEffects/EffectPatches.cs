using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using HarmonyLib;
using UnityEngine;

namespace EpicMMOSystem.StatusEffects;

public static class EffectPatches
{
    private static void AddMyStatusEffect(ObjectDB odb)
    {
        if (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0 ||
            ObjectDB.instance.GetItemPrefab("Amber") == null) return;

        // if (!odb.m_StatusEffects.Find(se => se.name == SE_FallenGod.id))
        // {
        //     odb.m_StatusEffects.Add(ScriptableObject.CreateInstance<SE_FallenGod>());
        // }
        // if (ObjectDB.instance.GetItemPrefab(EpicMmoSystemPlugin.raidoPrefab.name.GetStableHashCode()) == null)
        // {
        //     ObjectDB.instance.m_items.Add(EpicMmoSystemPlugin.raidoPrefab);
        //     ObjectDB.instance.m_itemByHash[EpicMmoSystemPlugin.raidoPrefab.name.GetStableHashCode()] = EpicMmoSystemPlugin.raidoPrefab;
        // }
    }

    [HarmonyPatch(typeof(ObjectDB), "Awake")]
    public static class ObjectDBAwake
    {
        public static void Postfix(ObjectDB __instance)
        {
            AddMyStatusEffect(__instance);
        }
    }
    
    [HarmonyPatch(typeof(ObjectDB), "CopyOtherDB")]
    public static class ObjectDBCopyOtherDB
    {
        public static void Postfix(ObjectDB __instance)
        {
            AddMyStatusEffect(__instance);
        }
    }

    [HarmonyPatch(typeof(Player), "ConsumeItem")]
    public static class ConsumeMMOXP
    {
        public static void Prefix(ItemDrop.ItemData item)
        {
            //EpicMMOSystem.MLLogger.LogInfo("Player Consume "  );
            if (!Player.m_localPlayer.m_seman.HaveStatusEffect("MMO_XP".GetStableHashCode()))
            {

                GameObject found = null;
                foreach (var GameItem in ObjectDB.instance.m_items) // much bad
                {
                    if (GameItem.GetComponent<ItemDrop>()?.m_itemData.m_shared.m_name == item.m_shared.m_name)
                        found = GameItem;
                }

                switch (found.name)
                {
                    case "mmo_orb1":
                        LevelSystem.Instance.AddExp(EpicMMOSystem.XPforOrb1.Value, true);  break;
                    case "mmo_orb2":
                        LevelSystem.Instance.AddExp(EpicMMOSystem.XPforOrb2.Value, true);  break;
                    case "mmo_orb3":
                        LevelSystem.Instance.AddExp(EpicMMOSystem.XPforOrb3.Value, true);  break;
                    case "mmo_orb4":
                        LevelSystem.Instance.AddExp(EpicMMOSystem.XPforOrb4.Value, true);  break;
                    case "mmo_orb5":
                        LevelSystem.Instance.AddExp(EpicMMOSystem.XPforOrb5.Value, true);  break;
                    case "mmo_orb6":
                        LevelSystem.Instance.AddExp(EpicMMOSystem.XPforOrb6.Value, true);  break;

                    default: break;
                }
            }
        }
    }




}
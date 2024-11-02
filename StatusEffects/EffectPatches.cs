using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using HarmonyLib;
using UnityEngine;

namespace EpicMMOSystem.StatusEffects;

public static class EffectPatches
{
    [HarmonyPriority(Priority.Low)]
    [HarmonyPatch(typeof(Player), "ConsumeItem")]
    public static class ConsumeMMOXP
    {
        private static ItemDrop.ItemData tempItem;

        public static void Prefix(Inventory inventory, ItemDrop.ItemData item, bool checkWorldLevel = false)
        {
            if (Player.m_localPlayer.m_seman.HaveStatusEffect("MMO_XP".GetStableHashCode()))
                return;

            tempItem = item;
        }

        public static void Postfix(ref bool __result)
        {
            // Only proceed if the item was successfully consumed (__result is true)
            if (!__result || tempItem == null)
                return;

            // badish
            GameObject found = null;
            foreach (var GameItem in ObjectDB.instance.m_items)
            {
                if (GameItem.GetComponent<ItemDrop>()?.m_itemData.m_shared.m_name == tempItem.m_shared.m_name)
                {
                    found = GameItem;
                    break;
                }
            }

            switch (found?.name)
            {
                case "mmo_orb1":
                    LevelSystem.Instance.AddExp(EpicMMOSystem.XPforOrb1.Value, true);
                    break;
                case "mmo_orb2":
                    LevelSystem.Instance.AddExp(EpicMMOSystem.XPforOrb2.Value, true);
                    break;
                case "mmo_orb3":
                    LevelSystem.Instance.AddExp(EpicMMOSystem.XPforOrb3.Value, true);
                    break;
                case "mmo_orb4":
                    LevelSystem.Instance.AddExp(EpicMMOSystem.XPforOrb4.Value, true);
                    break;
                case "mmo_orb5":
                    LevelSystem.Instance.AddExp(EpicMMOSystem.XPforOrb5.Value, true);
                    break;
                case "mmo_orb6":
                    LevelSystem.Instance.AddExp(EpicMMOSystem.XPforOrb6.Value, true);
                    break;
                default:
                    break;
            }

            // Clear tempItem to avoid data leakage between calls
            tempItem = null;
        }
    }





}
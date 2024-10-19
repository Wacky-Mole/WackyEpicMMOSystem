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
        public static void Postfix(ItemDrop.ItemData item, ref bool __result)
        {
            //EpicMMOSystem.MLLogger.LogInfo("Player Consume "  );
            if (!__result)
                return;
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
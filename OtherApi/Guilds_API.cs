using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace EpicMMOSystem.OtherApi
{
    public static class Guilds_API
    {
        private static API_State state = API_State.NotReady;
        private static GameObject mGuildsPanel;
        private static MethodInfo mOpenPanel;
        private static bool firstOpen = true;

        private enum API_State
        {
            NotReady,
            NotInstalled,
            Ready,
        }

        public static bool IsInstalled()
        {
            Init();
            return state is API_State.Ready;
        }

        public static void ShowGuilds()
        {
            if (Player.m_localPlayer == null)
            {
                Debug.LogWarning("[EpicMMO] Guilds UI: player not ready (no local player).");
                return;
            }

            // The UI lives in Guilds.Interface
            var ifaceType = Type.GetType("Guilds.Interface, Guilds");
            if (ifaceType == null)
            {
                Debug.LogWarning("[EpicMMO] Guilds UI: Interface type not found.");
                return;
            }

            // Static UI fields
            var fNoGuild = ifaceType.GetField("NoGuildUI", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var fMgmtGuild = ifaceType.GetField("GuildManagementUI", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            var noGuildGO = fNoGuild?.GetValue(null) as GameObject;
            var mgmtGuildGO = fMgmtGuild?.GetValue(null) as GameObject;

            if (noGuildGO == null && mgmtGuildGO == null)
            {
                Debug.LogWarning("[EpicMMO] Guilds UI: UI objects not found.");
                return;
            }

            // Check if the player is in a guild (use API if available)
            bool hasGuild = true; // default to management UI
            var apiType = Type.GetType("Guilds.API.GuildsAPI, GuildsAPI");
            if (apiType != null)
            {
                try
                {
                    var getOwnGuild = apiType.GetMethod("GetOwnGuild", BindingFlags.Public | BindingFlags.Static);
                    hasGuild = getOwnGuild?.Invoke(null, null) != null;
                }
                catch { /* ignore and keep default */ }
            }

            // Mirror the hotkey behaviour
            if (noGuildGO) noGuildGO.SetActive(!hasGuild);
            if (mgmtGuildGO) mgmtGuildGO.SetActive(hasGuild);

            Debug.Log("[EpicMMO] Guilds UI opened.");
        }



        private static void Init()
        {
            if (!firstOpen)
            {
                if (state is API_State.Ready or API_State.NotInstalled) return;
            }

            if (Type.GetType("Guilds.Guilds, Guilds") == null)
            {
                state = API_State.NotInstalled;
                return;
            }

            state = API_State.Ready;

            Type guildsType = Type.GetType("Guilds.Guilds, Guilds");

            // field or property name might differ – adjust once you inspect GuildsAPI.dll
            mGuildsPanel = AccessTools.Field(guildsType, "guildPanelInstance")?.GetValue(null) as GameObject;

            // same here, check exported method names in GuildsAPI.dll
            mOpenPanel = guildsType.GetMethod("OpenUI", BindingFlags.Public | BindingFlags.Static)
                        ?? guildsType.GetMethod("OpenGuildsUI", BindingFlags.Public | BindingFlags.Static);
        }

        private static void ForceInit()
        {
            if (Type.GetType("Guilds.Guilds, Guilds") == null)
            {
                state = API_State.NotInstalled;
                return;
            }

            state = API_State.Ready;

            Type guildsType = Type.GetType("Guilds.Guilds, Guilds");
            mGuildsPanel = AccessTools.Field(guildsType, "guildPanelInstance")?.GetValue(null) as GameObject;
            mOpenPanel = guildsType.GetMethod("OpenUI", BindingFlags.Public | BindingFlags.Static)
                        ?? guildsType.GetMethod("OpenGuildsUI", BindingFlags.Public | BindingFlags.Static);
        }
    }
}

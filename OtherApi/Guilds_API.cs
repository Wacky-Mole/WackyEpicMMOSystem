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
            if (Player.m_localPlayer == null )
            {
                Debug.LogWarning("[EpicMMO] Guilds UI: player not ready (no local player / no input).");
                return;
            }

            var ifaceType = Type.GetType("Guilds.Interface, Guilds");
            if (ifaceType == null)
            {
                Debug.LogWarning("[EpicMMO] Guilds UI: Interface type not found.");
                return;
            }

            // 1) Fetch canvases
            var fNoGuild = ifaceType.GetField("NoGuildUI", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var fMgmtGuild = ifaceType.GetField("GuildManagementUI", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var noGuildGO = fNoGuild?.GetValue(null) as GameObject;
            var mgmtGuildGO = fMgmtGuild?.GetValue(null) as GameObject;

            if (noGuildGO == null && mgmtGuildGO == null)
            {
                Debug.LogWarning("[EpicMMO] Guilds UI: UI GameObjects not found (NoGuildUI/GuildManagementUI).");
                return;
            }

            // 2) Determine if player has a guild (soft: use API if present)
            bool hasGuild = true; // default to management UI if API not found
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

            // 3) Mirror hotkey: show exactly one canvas
            if (noGuildGO != null) noGuildGO.SetActive(!hasGuild);
            if (mgmtGuildGO != null) mgmtGuildGO.SetActive(hasGuild);

            // 4) Flip internal "open" style flags so buttons are live
            //    (the mod may guard handlers on these)
            foreach (var bf in ifaceType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                var n = bf.Name.ToLowerInvariant();
                if (bf.FieldType == typeof(bool) && (n.Contains("open") || n.Contains("visible") || n.Contains("active")))
                {
                    try { bf.SetValue(null, true); } catch { /* ignore */ }
                }
            }

            // 5) Call any likely open/refresh hooks if they exist (no-arg)
            string[] openNames = { "OnOpen", "Open", "OpenUI", "Refresh", "RefreshUI", "UpdateUI" };
            foreach (var mn in openNames)
            {
                var m = ifaceType.GetMethod(mn, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (m != null && m.GetParameters().Length == 0)
                {
                    try { m.Invoke(null, null); } catch { /* ignore */ }
                }
            }

            // Optional: make sure the canvas group is interactable if they use one
            void EnsureInteractable(GameObject go)
            {
                if (!go) return;
                var cg = go.GetComponentInChildren<CanvasGroup>(true);
                if (cg != null) { cg.interactable = true; cg.blocksRaycasts = true; cg.alpha = 1f; }
            }
            EnsureInteractable(hasGuild ? mgmtGuildGO : noGuildGO);

            Debug.Log("[EpicMMO] Guilds UI opened via reflection with state flags set.");
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

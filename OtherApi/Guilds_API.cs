using System;
using System.Collections;
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

        private const int MaxTries = 4;
        private static int _tries;

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
            if (!Player.m_localPlayer) return;
            _tries = 0;
            TryOpenGuilds();
        }

        private static void TryOpenGuilds()
        {
            try
            {
                // Guilds.Interface contains static refs to the UI roots
                var ifaceType = Type.GetType("Guilds.Interface, Guilds");
                if (ifaceType == null) { Debug.LogWarning("[EpicMMO] Guilds UI: Interface not found."); return; }

                var fNoGuild = ifaceType.GetField("NoGuildUI", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                var fMgmtGuild = ifaceType.GetField("GuildManagementUI", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                var noGuildGO = fNoGuild?.GetValue(null) as GameObject;
                var mgmtGuildGO = fMgmtGuild?.GetValue(null) as GameObject;

                // If neither is available yet, delay and retry
                if (!noGuildGO && !mgmtGuildGO)
                {
                    RetryLater("[EpicMMO] Guilds UI: objects not ready yet.");
                    return;
                }

                // Determine if player has a guild (best-effort; default true)
                bool hasGuild = true;
                var apiType = Type.GetType("Guilds.API.GuildsAPI, GuildsAPI");
                if (apiType != null)
                {
                    try
                    {
                        var getOwnGuild = apiType.GetMethod("GetOwnGuild", BindingFlags.Public | BindingFlags.Static);
                        hasGuild = getOwnGuild?.Invoke(null, null) != null;
                    }
                    catch { /* keep default */ }
                }

                // Deactivate both first, then activate the one we want
                if (noGuildGO && noGuildGO.activeSelf) noGuildGO.SetActive(false);
                if (mgmtGuildGO && mgmtGuildGO.activeSelf) mgmtGuildGO.SetActive(false);

                if (hasGuild)
                {
                    if (!mgmtGuildGO)
                    {
                        RetryLater("[EpicMMO] Guilds UI: mgmt panel not ready yet.");
                        return;
                    }
                    mgmtGuildGO.SetActive(true);   // may invoke Guilds.GuildManagementUI.OnEnable()
                }
                else
                {
                    if (!noGuildGO)
                    {
                        RetryLater("[EpicMMO] Guilds UI: no-guild panel not ready yet.");
                        return;
                    }
                    noGuildGO.SetActive(true);
                }

                Debug.Log("[EpicMMO] Guilds UI opened.");
                _tries = 0; // success
            }
            catch (NullReferenceException)
            {
                RetryLater("[EpicMMO] Guilds UI: NRE during open (Guilds not fully initialized).");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[EpicMMO] Guilds UI: failed to open ({e.GetType().Name}).");
            }
        }

        private static void RetryLater(string reason)
        {
            if (_tries++ >= MaxTries) { Debug.LogWarning($"{reason} Gave up after {_tries} tries."); return; }
            if (!Player.m_localPlayer) return; // no runner available
            Player.m_localPlayer.StartCoroutine(DelayedRetry());
        }

        private static IEnumerator DelayedRetry()
        {
            // wait a couple frames to let Guilds assign its static fields
            yield return null;
            yield return new WaitForSeconds(0.25f);
            TryOpenGuilds();
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

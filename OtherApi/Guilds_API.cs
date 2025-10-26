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
                // --- grab Guilds.Interface static fields ---
                var ifaceType = Type.GetType("Guilds.Interface, Guilds");
                if (ifaceType == null)
                {
                    Debug.LogWarning("[EpicMMO] Guilds UI: Interface type not found.");
                    return;
                }

                var fNoGuild = ifaceType.GetField("NoGuildUI",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                var fMgmtGuild = ifaceType.GetField("GuildManagementUI",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                var noGuildGO = fNoGuild?.GetValue(null) as GameObject;
                var mgmtGuildGO = fMgmtGuild?.GetValue(null) as GameObject;

                // If neither panel exists yet, Guilds hasn't spawned UI prefabs
                if (!noGuildGO && !mgmtGuildGO)
                {
                    RetryLater("[EpicMMO] Guilds UI: objects not ready yet.");
                    return;
                }

                // --- ask Guilds.API if we're in a guild ---
                // this is basically what Guilds.Interface.Update() does:
                // if (API.GetOwnGuild() is null) { NoGuildUI.SetActive(true); }
                // else { GuildManagementUI.SetActive(true); }

                var apiType = Type.GetType("Guilds.API, Guilds"); // NOTE: namespace is Guilds; class is API
                bool inStableGuild = false;

                if (apiType != null)
                {
                    var getOwnGuild = apiType.GetMethod("GetOwnGuild",
                        BindingFlags.Public | BindingFlags.Static);

                    if (getOwnGuild != null)
                    {
                        // guildObj is whatever Guilds.API thinks is "your guild"
                        var guildObj = getOwnGuild.Invoke(null, null);

                        if (guildObj != null)
                        {
                            // extra safety: make sure the guild is actually initialized
                            // We'll reflect a "General" field/property and require it's non-null.
                            var guildType = guildObj.GetType();

                            // Try field first
                            var generalField = guildType.GetField("General",
                                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                            object generalVal = generalField?.GetValue(guildObj);

                            if (generalVal == null)
                            {
                                // Sometimes mods make it a property instead of a field.
                                var generalProp = guildType.GetProperty("General",
                                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                                generalVal = generalProp?.GetValue(guildObj, null);
                            }

                            // If General is not null, we consider this a "ready" guild.
                            // If it's null, it's that half-synced stub that explodes GuildManagementUI.OnEnable().
                            inStableGuild = generalVal != null;
                        }
                    }
                }

                // Before showing anything, hide both (if they exist & are active)
                if (noGuildGO && noGuildGO.activeSelf)
                    noGuildGO.SetActive(false);
                if (mgmtGuildGO && mgmtGuildGO.activeSelf)
                    mgmtGuildGO.SetActive(false);

                if (inStableGuild)
                {
                    // Player is actually in a fully initialized guild
                    if (!mgmtGuildGO)
                    {
                        RetryLater("[EpicMMO] Guilds UI: mgmt panel not ready yet.");
                        return;
                    }

                    // Enabling this normally triggers GuildManagementUI.OnEnable().
                    // Now it's safe because we verified guild.General != null.
                    mgmtGuildGO.SetActive(true);
                }
                else
                {
                    // Player is not in a real / synced guild yet.
                    // Show the create/search popup instead of crashing mgmt UI.
                    if (!noGuildGO)
                    {
                        RetryLater("[EpicMMO] Guilds UI: no-guild panel not ready yet.");
                        return;
                    }

                    noGuildGO.SetActive(true);
                }

                Debug.Log("[EpicMMO] Guilds UI opened safely.");
                _tries = 0; // success, stop retry loop
            }
            catch (NullReferenceException)
            {
                // Guilds is still wiring itself, bail and retry.
                RetryLater("[EpicMMO] Guilds UI: NRE during open (Guilds not fully initialized).");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[EpicMMO] Guilds UI: failed to open ({e.GetType().Name}): {e}");
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

using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace EpicMMOSystem.OtherApi
{
    public static class Guilds_API
    {
        private static API_State state = API_State.NotReady;

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
                var ifaceType = Type.GetType("Guilds.Interface, Guilds");
                if (ifaceType == null)
                {
                    Debug.LogWarning("[EpicMMO] Guilds UI: Interface type not found.");
                    return;
                }

                var fNoGuild = ifaceType.GetField("NoGuildUI", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                var fMgmtGuild = ifaceType.GetField("GuildManagementUI", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                var noGuildGO = fNoGuild?.GetValue(null) as GameObject;
                var mgmtGuildGO = fMgmtGuild?.GetValue(null) as GameObject;

                if (!noGuildGO && !mgmtGuildGO)
                {
                    RetryLater("[EpicMMO] Guilds UI: objects not ready yet.");
                    return;
                }

                var apiType = Type.GetType("Guilds.API, Guilds");
                bool inStableGuild = false;

                if (apiType != null)
                {
                    var getOwnGuild = apiType.GetMethod("GetOwnGuild", BindingFlags.Public | BindingFlags.Static);
                    if (getOwnGuild != null)
                    {
                        var guildObj = getOwnGuild.Invoke(null, null);
                        if (guildObj != null)
                        {
                            var guildType = guildObj.GetType();
                            var generalField = guildType.GetField("General", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                            object generalVal = generalField?.GetValue(guildObj);

                            if (generalVal == null)
                            {
                                var generalProp = guildType.GetProperty("General", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                                generalVal = generalProp?.GetValue(guildObj, null);
                            }

                            inStableGuild = generalVal != null;
                        }
                    }
                }

                if (noGuildGO && noGuildGO.activeSelf)
                    noGuildGO.SetActive(false);
                if (mgmtGuildGO && mgmtGuildGO.activeSelf)
                    mgmtGuildGO.SetActive(false);

                if (inStableGuild)
                {
                    if (!mgmtGuildGO)
                    {
                        RetryLater("[EpicMMO] Guilds UI: mgmt panel not ready yet.");
                        return;
                    }

                    mgmtGuildGO.SetActive(true);
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

                Debug.Log("[EpicMMO] Guilds UI opened safely.");
                _tries = 0;
            }
            catch (NullReferenceException)
            {
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
            if (!Player.m_localPlayer) return;
            Player.m_localPlayer.StartCoroutine(DelayedRetry());
        }

        private static IEnumerator DelayedRetry()
        {
            yield return null;
            yield return new WaitForSeconds(0.25f);
            TryOpenGuilds();
        }

        private static void Init()
        {
            if (state is API_State.Ready or API_State.NotInstalled) return;

            if (Type.GetType("Guilds.Guilds, Guilds") == null &&
                Type.GetType("Guilds.Interface, Guilds") == null &&
                Type.GetType("Guilds.API, Guilds") == null)
            {
                state = API_State.NotInstalled;
                return;
            }

            state = API_State.Ready;
        }

        private static void ForceInit()
        {
            state = API_State.NotReady;
            Init();
        }
    }
}

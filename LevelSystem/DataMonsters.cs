using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using BepInEx;
using fastJSON;
using HarmonyLib;

using UnityEngine;
//using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
//using Text = UnityEngine.UI.Text;
using TMPro;
using UnityEngine.UI;
using ItemManager;
using UnityEngine.XR;

namespace EpicMMOSystem;


public static class DataMonsters
{
    private static Dictionary<string, Monster> dictionary = new();
    private static string MonsterDB = "";
    public static List<string> MonsterDBL;
    public static string PlayerDBL;
    private static readonly Dictionary<Heightmap.Biome, GameObject> OrbsByBiomes = new(10);
    public static readonly Dictionary<GameObject, int> MagicOrbDictionary = new Dictionary<GameObject, int>(10);// thx KG


    public static void InitItems()
    {// Visit  https://github.com/Wacky-Mole/MagicHeim/blob/master/MagicTomes.cs for more details

        Item Orb1 = new("mmo_xp", "mmo_orb1", "asset");
        Orb1.ToggleConfigurationVisibility(Configurability.Disabled);
        Item Orb2 = new("mmo_xp", "mmo_orb2", "asset");
        Orb2.ToggleConfigurationVisibility(Configurability.Disabled);
        Item Orb3 = new("mmo_xp", "mmo_orb3", "asset");
        Orb3.ToggleConfigurationVisibility(Configurability.Disabled);
        Item Orb4 = new("mmo_xp", "mmo_orb4", "asset");
        Orb4.ToggleConfigurationVisibility(Configurability.Disabled);
        Item Orb5 = new("mmo_xp", "mmo_orb5", "asset");
        Orb5.ToggleConfigurationVisibility(Configurability.Disabled);
        Item Orb6 = new("mmo_xp", "mmo_orb6", "asset");
        Orb6.ToggleConfigurationVisibility(Configurability.Disabled);       
        Item Orb7 = new("mmo_xp", "mmo_orb7", "asset");
        Orb7.ToggleConfigurationVisibility(Configurability.Disabled);        
        Item Orb8 = new("mmo_xp", "mmo_orb8", "asset");
        Orb8.ToggleConfigurationVisibility(Configurability.Disabled);

        MagicOrbDictionary.Add(Orb1.Prefab, EpicMMOSystem.XPforOrb1.Value);
        MagicOrbDictionary.Add(Orb2.Prefab, EpicMMOSystem.XPforOrb2.Value);
        MagicOrbDictionary.Add(Orb3.Prefab, EpicMMOSystem.XPforOrb3.Value);
        MagicOrbDictionary.Add(Orb4.Prefab, EpicMMOSystem.XPforOrb4.Value);
        MagicOrbDictionary.Add(Orb5.Prefab, EpicMMOSystem.XPforOrb5.Value);
        MagicOrbDictionary.Add(Orb6.Prefab, EpicMMOSystem.XPforOrb6.Value);
        MagicOrbDictionary.Add(Orb7.Prefab, EpicMMOSystem.XPforOrb7.Value);
        MagicOrbDictionary.Add(Orb8.Prefab, EpicMMOSystem.XPforOrb8.Value);
        


        OrbsByBiomes.Add(Heightmap.Biome.Meadows, Orb1.Prefab);
        OrbsByBiomes.Add(Heightmap.Biome.BlackForest, Orb2.Prefab);
        OrbsByBiomes.Add(Heightmap.Biome.Swamp, Orb3.Prefab);
        OrbsByBiomes.Add(Heightmap.Biome.Mountain, Orb4.Prefab);
        OrbsByBiomes.Add(Heightmap.Biome.Plains, Orb5.Prefab);
        OrbsByBiomes.Add(Heightmap.Biome.DeepNorth, Orb8.Prefab);
        OrbsByBiomes.Add(Heightmap.Biome.AshLands, Orb7.Prefab);
        OrbsByBiomes.Add(Heightmap.Biome.Ocean, Orb3.Prefab);
        OrbsByBiomes.Add(Heightmap.Biome.None, Orb2.Prefab);
        OrbsByBiomes.Add(Heightmap.Biome.Mistlands, Orb6.Prefab);
    }

    /*

    [HarmonyPatch(typeof(ItemDrop.ItemData), nameof(ItemDrop.ItemData.GetTooltip), typeof(ItemDrop.ItemData),typeof(int),typeof(bool), typeof(float) )]
    public class GetTooltipPatch
    {
        public static void Postfix(ItemDrop.ItemData item, bool crafting, ref string __result)
        {
            if (crafting || item == null || !item.m_dropPrefab) return;
            if (MagicOrbDictionary.TryGetValue(item.m_dropPrefab, out var expGain))
            {
                __result = __result + $"Right Mouse Button click to get <color=yellow>{expGain}</color> EXP";
            }
        }
    }
    */
    [HarmonyPatch(typeof(ItemDrop.ItemData), nameof(ItemDrop.ItemData.GetTooltip), new Type[] { typeof(int) })]
    public class GetTooltipPatch
    {
        public static void Postfix(ItemDrop.ItemData __instance, int stackOverride, ref string __result)
        {
            // Ensure that `crafting` is checked and `item` validity is confirmed as in the original patch logic.
            if (__instance == null || !__instance.m_dropPrefab) return;

            if (MagicOrbDictionary.TryGetValue(__instance.m_dropPrefab, out var expGain))
            {
                __result += $" Right Mouse Button click to get <color=yellow>{expGain}</color> EXP";
            }
        }
    }


    public static bool contains(string name)
    {
        return  dictionary.ContainsKey(name);
    }

    public static int getExp(string name)
    { 
        var monster = dictionary[name];
        int exp = Random.Range(monster.minExp, monster.maxExp);
        return exp;
    }

    public static int getMaxExp(string name)
    {
      return dictionary[name].maxExp;
    }

    public static int getLevel(string name)
    {
        return dictionary[name].level;     
    }


    public static void createNewDataMonsters(List<string> json)
    {
        dictionary.Clear();

        foreach (var monster2 in json)
        {
        if (EpicMMOSystem.extraDebug.Value)
            EpicMMOSystem.MLLogger.LogInfo($"/n Json loading /n");

            //var temp = JsonUtility.FromJson<Monster[]>(monster2);
            var temp = (fastJSON.JSON.ToObject<Monster[]>(monster2));
            foreach (var monster in temp)
            {
                if (EpicMMOSystem.extraDebug.Value)
                    EpicMMOSystem.MLLogger.LogInfo($"{monster.name}");

                if (dictionary.ContainsKey(monster.name + "(Clone)"))
                {
                    EpicMMOSystem.MLLogger.LogWarning($"{monster.name} is already entered");
                }
                else
                    dictionary.Add($"{monster.name}(Clone)", monster);
            }            
        }   
     
    }

    public static void setplayeralivetoZero(string playerset)
    {
        //dictionaryPlayer[playerset].daysAlive = 0;
    }

    public static void createUpdateDataPlayer(List<string> json)
    {
        /*
        if (EpicMMOSystem.extraDebug.Value)
            EpicMMOSystem.MLLogger.LogInfo($"/n Player json Updating /n");
        var json2 = json[0];
        //var temp = JsonUtility.FromJson<Monster[]>(monster2);
        var temp = (fastJSON.JSON.ToObject<PlayerXP[]>(json2));
        foreach (var monster in temp)
        {
            string name = monster.name;//.ToUpper(); // players name always uppper
            if (EpicMMOSystem.extraDebug.Value)
                EpicMMOSystem.MLLogger.LogInfo($"{name}");


            if (dictionaryPlayer.ContainsKey(name))
            {
                dictionaryPlayer[name] = monster;
            }
            else
                dictionaryPlayer.Add($"{name}", monster);

        }       
        */
    }

    public static void Init()
    {
        var versionpath = Path.Combine(Paths.ConfigPath, EpicMMOSystem.ModName, $"Version.txt");
        var folderpath = Path.Combine(Paths.ConfigPath, EpicMMOSystem.ModName);
        //var folderpathbackup = Path.Combine(Paths.ConfigPath, EpicMMOSystem.ModName +"_backup");
        var warningtext = Path.Combine(Paths.ConfigPath, EpicMMOSystem.ModName, $"If you want to stop from updating.txt");
        var json = "Default.json";
        var json1 = "AirAnimals.json";
        var json2 = "LandAnimals.json";
        var json3 = "Fantasy_Creatures.json";
        var json4 = "SeaAnimals.json";
        var json5 = "MMO_MonsterLabZ.json";
        var json6 = "Outsiders.json";
        var json7 = "DoorDieMonsters.json";
        var json8 = "MajesticChickens.json";
        var json9 = "Therzie_Monstrum.json";
        var json10 = "Reforge_Krumpac.json";
        var json11 = "TeddyBears.json";
        var json12 = "PungusSouls.json";
        var json13 = "Jewelcrafting.json";
        var json14 = "RtDMonsters.json";
        var json15 = "Therzie_Wizardry.json";
        var json16 = "NonCombat.json";
        var json17 = "Monstrum_DeepNorth.json";
        var json18 = "RtDHorrors.json";
        var json19 = "RtDMonstrum.json";
        var json20 = "RtDSea.json";
        var json21 = "RtDFairyTale.json";
        var json22 = "RtDAdditions.json";
        var json23 = "RtDSouls.json";


        var badfile = "MonsterLabZ.json";
        var badfilepath = Path.Combine(folderpath, badfile);



        if (!Directory.Exists(folderpath)){
            Directory.CreateDirectory(folderpath);
        }
        var cleartowrite = true;
        if (File.Exists(versionpath))
        {
            //MonsterDB = File.ReadAllText(path);
            var filev = File.ReadAllText(versionpath);
            cleartowrite = false; // default is false because it exists in the first place

            if (filev == "1.4.0")
                cleartowrite = true;
            if (filev == "1.4.1")
                cleartowrite = true;
            if (filev == "1.5.0")
                cleartowrite = true;
            if (filev == "1.5.3")
                cleartowrite = true;
            if (filev == "1.5.4")
                cleartowrite = true;
            if (filev == "1.5.8")
                cleartowrite = true;
            if (filev == "1.6.2")
                cleartowrite = true;
            if (filev == "1.6.3")
                cleartowrite = true;            
            if (filev == "1.6.5")
                cleartowrite = true;
            if (filev == "1.6.7")
                cleartowrite = true;
            if (filev == "1.7.0")
                cleartowrite = true;            
            if (filev == "1.7.3")
                cleartowrite = true;            
            if (filev == "1.7.4")
                cleartowrite = true;            
            if (filev == "1.7.5")
                cleartowrite = true;            
            if (filev == "1.7.6")
                cleartowrite = true;            
            if (filev == "1.7.7")
                cleartowrite = true;
            if (filev == "1.7.8")
                cleartowrite = true;            
            if (filev == "1.7.9")
                cleartowrite = true;
            if (filev == "1.8.7")
                cleartowrite = true;
            if (filev == "1.8.8")
                cleartowrite = true;
            if (filev == "1.8.97")
                cleartowrite = true;            
            if (filev == "1.9.02")
                cleartowrite = true;            
            if (filev == "1.9.12")
                cleartowrite = true;        
            if (filev == "1.9.20")
                cleartowrite = true;            
            if (filev == "1.9.21")
                cleartowrite = true;            
            if (filev == "1.9.23")
                cleartowrite = true;            
            if (filev == "1.9.30")
                cleartowrite = true;
            if (filev == "1.9.32")           
                cleartowrite = true;           
            if (filev == "1.9.35")           
                cleartowrite = true;


            if (File.Exists(badfilepath))
            {
                File.Delete(badfilepath);
            }

                        

            if (filev == "1.9.38") // last version to get a DB update
                cleartowrite = false;

            if (filev == "NO" || filev == "no" || filev == "No" || filev == "STOP" || filev == "stop" || filev == "Stop")
            {// don't update
                cleartowrite = false;
            }

        }
        if (cleartowrite)
        {
            //list.Clear();
            File.WriteAllText(versionpath, "1.9.38"); // Write Version file, don't auto update

            File.WriteAllText(warningtext, "Erase numbers in Version.txt and write NO or stop in file. This should stop DB json files from updating on an update. If you make your own custom json file, then that one should never be updated.");

            File.WriteAllText(Path.Combine(folderpath, json), getDefaultJsonMonster(json));

            File.WriteAllText(Path.Combine(folderpath, json1), getDefaultJsonMonster(json1));

            File.WriteAllText(Path.Combine(folderpath, json2), getDefaultJsonMonster(json2));

            File.WriteAllText(Path.Combine(folderpath, json3), getDefaultJsonMonster(json3));

            File.WriteAllText(Path.Combine(folderpath, json4), getDefaultJsonMonster(json4));

            File.WriteAllText(Path.Combine(folderpath, json5), getDefaultJsonMonster(json5));

            File.WriteAllText(Path.Combine(folderpath, json6), getDefaultJsonMonster(json6));

            File.WriteAllText(Path.Combine(folderpath, json7), getDefaultJsonMonster(json7));

            File.WriteAllText(Path.Combine(folderpath, json8), getDefaultJsonMonster(json8));

            File.WriteAllText(Path.Combine(folderpath, json9), getDefaultJsonMonster(json9));

            File.WriteAllText(Path.Combine(folderpath, json10), getDefaultJsonMonster(json10));
            
            File.WriteAllText(Path.Combine(folderpath, json11), getDefaultJsonMonster(json11));

            File.WriteAllText(Path.Combine(folderpath, json12), getDefaultJsonMonster(json12));

            File.WriteAllText(Path.Combine(folderpath, json13), getDefaultJsonMonster(json13));

            File.WriteAllText(Path.Combine(folderpath, json14), getDefaultJsonMonster(json14));

            File.WriteAllText(Path.Combine(folderpath, json15), getDefaultJsonMonster(json15));

            File.WriteAllText(Path.Combine(folderpath, json16), getDefaultJsonMonster(json16));

            File.WriteAllText(Path.Combine(folderpath, json17), getDefaultJsonMonster(json17));

            File.WriteAllText(Path.Combine(folderpath, json18), getDefaultJsonMonster(json18));

            File.WriteAllText(Path.Combine(folderpath, json19), getDefaultJsonMonster(json19));

            File.WriteAllText(Path.Combine(folderpath, json20), getDefaultJsonMonster(json20));

            File.WriteAllText(Path.Combine(folderpath, json21), getDefaultJsonMonster(json21));

            File.WriteAllText(Path.Combine(folderpath, json22), getDefaultJsonMonster(json22));

            File.WriteAllText(Path.Combine(folderpath, json23), getDefaultJsonMonster(json23));



            if (EpicMMOSystem.extraDebug.Value)
                EpicMMOSystem.MLLogger.LogInfo($"Mobs Jsons Written");
        }
        /*
        if (!File.Exists(EpicMMOSystem.playerspath))
        {
            File.WriteAllText(EpicMMOSystem.playerspath, getDefaultJsonMonster(EpicMMOSystem.playerjson));
        } */ 
        List<string> list = new List<string>();
        foreach (string file in Directory.GetFiles(folderpath, "*.json", SearchOption.AllDirectories))
        { 
            var nam = Path.GetFileName(file);
            if (nam == EpicMMOSystem.playerjson)
                continue;

            if (EpicMMOSystem.extraDebug.Value)
                EpicMMOSystem.MLLogger.LogInfo(nam + " read");

            var temp = File.ReadAllText(file);
            list.Add(temp);
            MonsterDB += temp;
           
        }
        if (EpicMMOSystem.extraDebug.Value)
            EpicMMOSystem.MLLogger.LogInfo($"Mobs Read");

        /*
        var players = File.ReadAllText(EpicMMOSystem.playerspath);
        PlayerDBL = players;
        List<string> playerslist = new List<string>();
        playerslist.Add(players);
        createUpdateDataPlayer(playerslist);        
        if (EpicMMOSystem.extraDebug.Value)
            EpicMMOSystem.MLLogger.LogInfo($"Player Read"); */


        MonsterDBL = list;
        createNewDataMonsters(list);
        
    }

    private static string getDefaultJsonMonster(string jsonname)
    {
        var assembly = Assembly.GetExecutingAssembly();
        string resourceName = assembly.GetManifestResourceNames()
            .Single(str => str.EndsWith(jsonname));

        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }
    
    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))] // clients
    private static class ZrouteMethodsServerFeedback
    {
        private static void Postfix()
        {
            if (EpicMMOSystem._isServer)
            {
                ZRoutedRpc.instance.Register($"{EpicMMOSystem.ModName} ReloadJsons",
                 new Action<long, bool>(ReloadJsons));

            }
            else
            {
                ZRoutedRpc.instance.Register($"{EpicMMOSystem.ModName} SetMonsterDB",
                    new Action<long, List<string>>(SetMonsterDB));                
                
            }
        }
    }
    public static void ReloadJsons(long peer, bool reload)
    {
        List<string> list = new List<string>();
        foreach (string file in Directory.GetFiles(EpicMMOSystem.folderpath, "*.json", SearchOption.AllDirectories))
        {
            var nam = Path.GetFileName(file);
            if (EpicMMOSystem.extraDebug.Value)
                EpicMMOSystem.MLLogger.LogInfo(nam + " read");

            var temp = File.ReadAllText(file);
            list.Add(temp);

        }
        if (EpicMMOSystem.extraDebug.Value)
            EpicMMOSystem.MLLogger.LogInfo($"Mobs Updated on Server");

        DataMonsters.MonsterDBL = list;

        createNewDataMonsters(list); // could update coop

        List<ZNetPeer> peers2 = ZNet.instance.GetPeers();
        foreach (var peer1 in peers2)
        {
            if (peer1 == null) return;
            ZRoutedRpc.instance.InvokeRoutedRPC(peer1.m_uid, $"{EpicMMOSystem.ModName} SetMonsterDB", MonsterDBL); //sync list
        }
    }


    public static void SetMonsterDB(long peer, List<string> json)
    {
        createNewDataMonsters(json);
    }


    [HarmonyPatch(typeof(ZNet), "RPC_PeerInfo")]
    private static class ZnetSyncServerInfo
    {
        private static void Postfix(ZRpc rpc) // for server
        {
            if (!EpicMMOSystem._isServer) return; // doesn't work on Coop
            //if (!(ZNet.instance.IsServer() && ZNet.instance.IsDedicated())) return;
            ZNetPeer peer = ZNet.instance.GetPeer(rpc);
            if(peer == null) return;
            ZRoutedRpc.instance.InvokeRoutedRPC(peer.m_uid, $"{EpicMMOSystem.ModName} SetMonsterDB", MonsterDBL); //sync list
        }
    }

    // [HarmonyPatch(typeof(Character), nameof(Character.GetHoverName))]
    // [HarmonyPriority(Priority.First)]
    // public static class MonsterColorText
    // {
    //     public static void Postfix(Character __instance, ref string __result)
    //     {
    //         if (!contains(__instance.gameObject.name)) return;
    //         int maxLevelExp = LevelSystem.Instance.getLevel() + EpicMMOSystem.maxLevelExp.Value;
    //         int minLevelExp = LevelSystem.Instance.getLevel() + EpicMMOSystem.minLevelExp.Value;
    //         int monsterLevel = getLevel(__instance.gameObject.name);
    //         if (monsterLevel > maxLevelExp)
    //         {
    //             __result = $"<color=red>{__result} [{monsterLevel}]</color>";
    //         } else if (monsterLevel < minLevelExp)
    //         {
    //             __result = $"<color=#2FFFDC>{__result} [{monsterLevel}]</color>";
    //         }
    //         else
    //         {
    //             __result = $"{__result} [{monsterLevel}]";
    //         }
    //     }
    // }




    [HarmonyPatch(typeof(EnemyHud), nameof(EnemyHud.ShowHud))]
    [HarmonyPriority(1)] //almost last
    public static class MonsterColorTexts
    {
        public static void Postfix(EnemyHud __instance, Character c, Dictionary<Character, EnemyHud.HudData> ___m_huds, bool __state)
        {
            try { if (c.m_tamed) return; } catch { } // might remove this in future so tames can give xp ect

            //if (___m_huds)

            if (c.IsPlayer() && ___m_huds[c].m_gui != null) // player pvp
            {
                  Transform go2 = ___m_huds[c].m_gui.transform.Find("Name/Name(Clone)");
                 if (go2) return;

                int level = 1;
                int daysalive = 0;
                int maxLevelExpplayer = LevelSystem.Instance.getLevel() + EpicMMOSystem.maxLevelExp.Value;
                int minLevelExpplayer = LevelSystem.Instance.getLevel() - EpicMMOSystem.minLevelExp.Value;


                GameObject component2 = ___m_huds[c].m_gui.transform.Find("Name").gameObject;
                GameObject compCheck = Object.Instantiate(component2, component2.transform);
                compCheck.GetComponent<TextMeshProUGUI>().text = "";
               string namesearch = c.GetHoverName();

                var playerlist2 = Player.GetAllPlayers();
                foreach (var pla in playerlist2)
                {
                    if (pla.GetPlayerName() == namesearch)
                    {
                        var zdopla = pla.m_nview.GetZDO();
                        daysalive = zdopla.GetInt(EpicMMOSystem.ModName + EpicMMOSystem.PlayerAliveString, -1);
                        if (daysalive == -1)
                        {
                            EpicMMOSystem.MLLogger.LogWarning("Days alive not found" + daysalive + " for player " + namesearch);
                            daysalive = 0;
                        }
                        level = zdopla.GetInt($"{EpicMMOSystem.ModName}_level", 1);
                        break;
                    }
                }

                int monsterLevelplayer = level;
                Color color2 = monsterLevelplayer > maxLevelExpplayer ? Color.red : Color.white;
                if (monsterLevelplayer < minLevelExpplayer) color2 = Color.cyan;

                string levelstring = "";
                string xpstring = "";
                string daysstring = "";
                int xpworth = (level * EpicMMOSystem.xpPerLevelPVP.Value) + (daysalive * EpicMMOSystem.xpPerDayNotDead.Value);

                levelstring = EpicMMOSystem.displayPlayerLevel.Value;
                levelstring = levelstring.Replace("@", level.ToString());

                xpstring = EpicMMOSystem.displayPlayerXP.Value;
                xpstring = xpstring.Replace("@", xpworth.ToString());

                daysstring = EpicMMOSystem.displayDaysAlive.Value;
                daysstring = daysstring.Replace("@", daysalive.ToString());

                component2.GetComponent<TextMeshProUGUI>().text = levelstring + c.GetHoverName() + xpstring + daysstring;

            } // end player search



            if (!EpicMMOSystem.enabledLevelControl.Value) return;
            if (!contains(c.gameObject.name)) return;
            Transform go = ___m_huds[c].m_gui.transform.Find("Name/Name(Clone)");
            if (go) return;
            int maxLevelExp = LevelSystem.Instance.getLevel() + EpicMMOSystem.maxLevelExp.Value;
            int minLevelExp = LevelSystem.Instance.getLevel() - EpicMMOSystem.minLevelExp.Value;
            int monsterLevel = getLevel(c.gameObject.name);
            if (EpicMMOSystem.mobLvlPerStar.Value)
                monsterLevel = monsterLevel + c.m_level - 1;

            GameObject component = ___m_huds[c].m_gui.transform.Find("Name").gameObject;
            //var textspace = component.GetComponent<Text>().text;
            //component.GetComponent<Text>().text = " "+ textspace + " "; // add some spacing for single letter names
            GameObject levelName = Object.Instantiate(component, component.transform);
            levelName.GetComponent<RectTransform>().anchoredPosition = EpicMMOSystem.MobLevelPosition.Value;
            if (c.m_boss)
            {
                levelName.GetComponent<RectTransform>().anchoredPosition = EpicMMOSystem.BossLevelPosition.Value;
            }
            string stringtolvl = EpicMMOSystem.MobLVLChars.Value;
            string moblvlstring = monsterLevel.ToString();
            Color color = monsterLevel > maxLevelExp ? Color.red : Color.white;
            if (monsterLevel < minLevelExp) color = Color.cyan;
            if (getLevel(c.gameObject.name) == 0)
            {
                moblvlstring = "???";
                color = Color.yellow;
            }
            stringtolvl = stringtolvl.Replace("@", moblvlstring); // not sure how fast this is
                                                                 // levelName.GetComponent<TextMeshProUGUI>().horizontalOverflow = UnityEngine.HorizontalWrapMode.Overflow; 
            levelName.GetComponent<TextMeshProUGUI>().overflowMode = TextOverflowModes.Overflow;
            levelName.AddComponent<ContentSizeFitter>().SetLayoutHorizontal();
            levelName.GetComponent<TextMeshProUGUI>().text = stringtolvl;
            component.GetComponent<TextMeshProUGUI>().color = color;
            levelName.GetComponent<TextMeshProUGUI>().color = color;
            if (___m_huds[c].m_gui.transform.Find("extraeffecttext"))
            {
                ___m_huds[c].m_gui.transform.Find("extraeffecttext").TryGetComponent<TextMeshProUGUI>(out var hi);
                if (hi != null)
                {
                    hi.color = color;
                }
            }
        }
        
        [HarmonyPatch(typeof(EnemyHud), nameof(EnemyHud.UpdateHuds))]
        public static class StarVisibilityMMO
        {
            private static void Postfix(Dictionary<Character, EnemyHud.HudData> ___m_huds)
            {
                if (___m_huds == null) return;
                //if (EpicMMOSystem.CLLCLoaded) return;
       
                foreach (KeyValuePair<Character, EnemyHud.HudData> keyValuePair in ___m_huds)
                {
                    if (keyValuePair.Key.IsPlayer() && keyValuePair.Value.m_gui != null) // player pvp
                    {
                        int level = 1;
                        int daysalive = 0;
                        int maxLevelExp = LevelSystem.Instance.getLevel() + EpicMMOSystem.maxLevelExp.Value;
                        int minLevelExp = LevelSystem.Instance.getLevel() - EpicMMOSystem.minLevelExp.Value;
                        GameObject component = keyValuePair.Value.m_gui.transform.Find("Name").gameObject;
                        string namesearch = keyValuePair.Key.GetHoverName();

                        var playerlist2 = Player.GetAllPlayers();
                        foreach (var pla in playerlist2)
                        {
                            if (pla.GetPlayerName() == namesearch)
                            {                             
                                var zdopla = pla.m_nview.GetZDO();
                                daysalive = zdopla.GetInt(EpicMMOSystem.ModName + EpicMMOSystem.PlayerAliveString, -1);
                                if (daysalive == -1)
                                {
                                    EpicMMOSystem.MLLogger.LogWarning("Days alive not found" + daysalive + " for player "+ namesearch);
                                    daysalive = 0;
                                }
                                level = zdopla.GetInt($"{EpicMMOSystem.ModName}_level", 1);
                                break;
                            }
                        }

                        int monsterLevel = level;
                        Color color = monsterLevel > maxLevelExp ? Color.red : Color.white;
                        if (monsterLevel < minLevelExp) color = Color.cyan;

                        string levelstring = "";
                        string xpstring = "";
                        string daysstring = "";
                        int xpworth = (level * EpicMMOSystem.xpPerLevelPVP.Value) + (daysalive * EpicMMOSystem.xpPerDayNotDead.Value);

                        levelstring = EpicMMOSystem.displayPlayerLevel.Value;
                        levelstring = levelstring.Replace("@", level.ToString());

                        xpstring = EpicMMOSystem.displayPlayerXP.Value;
                        xpstring = xpstring.Replace("@", xpworth.ToString());

                        daysstring = EpicMMOSystem.displayDaysAlive.Value;
                        daysstring = daysstring.Replace("@", daysalive.ToString());

                        component.GetComponent<TextMeshProUGUI>().text = levelstring + keyValuePair.Key.GetHoverName() + xpstring + daysstring;
                                             
                    } // end player search

                    if (!EpicMMOSystem.enabledLevelControl.Value) continue;

                    Character key = keyValuePair.Key;
                    if (key.IsTamed()) return;
                    if (key != null && keyValuePair.Value.m_gui)
                    {
                        if (!contains(key.gameObject.name)) return;

                        //key.IsPlayer();
                        int maxLevelExp = LevelSystem.Instance.getLevel() + EpicMMOSystem.maxLevelExp.Value;
                        int minLevelExp = LevelSystem.Instance.getLevel() - EpicMMOSystem.minLevelExp.Value;
                        int monsterLevel = getLevel(key.gameObject.name);
                        if (EpicMMOSystem.mobLvlPerStar.Value)
                            monsterLevel = monsterLevel + key.m_level - 1;

                        string mobLevelString = monsterLevel.ToString();
                        Color color = monsterLevel > maxLevelExp ? Color.red : Color.white;
                        if (monsterLevel < minLevelExp) color = Color.cyan;
                        if (getLevel(key.gameObject.name) == 0)
                        {
                            mobLevelString = "???";
                            color = Color.yellow;
                        }
                        Transform transform = keyValuePair.Value.m_gui.transform.Find("Name/Name(Clone)");
                        if (transform != null)
                        {
                            transform.gameObject.SetActive(true);
                        }
                        else
                        {
                            GameObject component = keyValuePair.Value.m_gui.transform.Find("Name").gameObject;
                            transform = Object.Instantiate(component, component.transform).transform;
                            transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(37, -30);
                            transform.GetComponent<TextMeshProUGUI>().fontSize = 13;
                            transform.GetComponent<TextMeshProUGUI>().text = $"[{mobLevelString}]";
                        }
                        transform.GetComponent<TextMeshProUGUI>().color = color;
                        keyValuePair.Value.m_gui.transform.Find("Name").GetComponent<TextMeshProUGUI>().color = color;
                       if (keyValuePair.Value.m_gui.transform.Find("extraeffecttext")) // for cllc extra components
                        {
                            keyValuePair.Value.m_gui.transform.Find("extraeffecttext").TryGetComponent<TextMeshProUGUI>(out var hi);
                            if (hi != null) // null check if not set
                            {
                                hi.color = color;
                            }
                        }
                    }
                }
            }
        }
    }



    [HarmonyPatch(typeof(CharacterDrop), nameof(CharacterDrop.GenerateDropList))]
    public static class MonsterDropGenerate
    {
        [HarmonyPriority(1)] // maybe stop epic loot? Last is 0, so 1 will be almost last for any other mod

        static void DropItem(GameObject prefab, Vector3 centerPos, float dropArea)  // Thx KG
        {
            Quaternion rotation = Quaternion.Euler(0f, (float)UnityEngine.Random.Range(0, 360), 0f);
            Vector3 b = UnityEngine.Random.insideUnitSphere * dropArea;
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, centerPos + b, rotation);
            Rigidbody component = gameObject.GetComponent<Rigidbody>();
            if (component)
            {
                Vector3 insideUnitSphere = UnityEngine.Random.insideUnitSphere;
                if (insideUnitSphere.y < 0f)
                {
                    insideUnitSphere.y = -insideUnitSphere.y;
                }

                component.AddForce(insideUnitSphere * 5f, ForceMode.VelocityChange);
            }
        }

        public static void Postfix(CharacterDrop __instance, ref List<KeyValuePair<GameObject, int>> __result)
        {
            if (__instance.m_character.IsTamed() ) return;
            Heightmap.Biome biome = EnvMan.instance.m_currentBiome;

            float rand = Random.value;
            var dropChance = __instance.m_character.IsBoss() ? EpicMMOSystem.OrbDropChancefromBoss.Value : EpicMMOSystem.OrbDropChance.Value;
            var isBoss = __instance.m_character.IsBoss(); // could remove extra code


            // clear to add Magic orbs now // always orb chance
            if (OrbsByBiomes.TryGetValue(biome, out var orb) && rand <= dropChance / 100f)
            {
                if (isBoss)
                {
                    for (int i = 0; i < Random.Range(1, EpicMMOSystem.OrdDropMaxAmountFromBoss.Value); i++) // random amount 1-4
                    {
                        DropItem(orb, __instance.transform.position + Vector3.up * 0.75f, 0.5f);
                    }
                }
                else
                {
                    DropItem(orb, __instance.transform.position + Vector3.up * 0.75f, 0.5f);
                }
            }

            if (EpicMMOSystem.enabledLevelControl.Value && (EpicMMOSystem.removeDropMax.Value || EpicMMOSystem.removeDropMin.Value || EpicMMOSystem.removeBossDropMax.Value || EpicMMOSystem.removeBossDropMin.Value || EpicMMOSystem.removeAllDropsFromNonPlayerKills.Value))
            {
                var playerLevel = __instance.m_character.m_nview.GetZDO().GetInt("epic playerLevel");

                if (playerLevel == 1000512)
                {
                    if (EpicMMOSystem.removeAllDropsFromNonPlayerKills.Value)
                    {
                        if (contains(__instance.m_character.gameObject.name))
                        {
                            __result = new(); // no drops from charcter related objects
                        }
                    }
                    playerLevel = 0;
                }

                if (!contains(__instance.m_character.gameObject.name)) return;
                if (playerLevel != 0)
                {                
                    // could just use isBoss above
                    var Regmob = true;
                    if (EpicMMOSystem.extraDebug.Value)
                        EpicMMOSystem.MLLogger.LogInfo("Player level " + playerLevel);
                    if (playerLevel > 0) // postive so boss
                    {
                        Regmob = false;
                    }
                    else // reg mobs
                    {
                        Regmob = true;
                        playerLevel = -playerLevel;
                    }

                    int maxLevelExp = playerLevel + EpicMMOSystem.maxLevelExp.Value;
                    int minLevelExp = playerLevel - EpicMMOSystem.minLevelExp.Value;
                    int monsterLevel = getLevel(__instance.m_character.gameObject.name) + __instance.m_character.m_level - 1; // interesting that it's using m_char as well
                    if (getLevel(__instance.m_character.gameObject.name) == 0)
                        return;

                    if ((monsterLevel > maxLevelExp) && (EpicMMOSystem.removeBossDropMax.Value && !Regmob || EpicMMOSystem.removeDropMax.Value && Regmob))
                    {
                        __result = new();
                        return;
                    }
                    if ((monsterLevel < minLevelExp) && (EpicMMOSystem.removeBossDropMin.Value && !Regmob || EpicMMOSystem.removeDropMin.Value && Regmob))
                    {
                        __result = new();
                        return;
                    }
                }
            }

        }
    }
}
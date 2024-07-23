using BepInEx;
using Microsoft.SqlServer.Server;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicMMOSystem
{

    [Serializable]
    public class CustomLevel
    {
        Dictionary <int, int> level; 
    }
    public class CustomLevels
    {

        //1: 500
        //2: 1000

        private void generateXPCustomFile()
        {
            Dictionary<int, int> CustomXPTable = new Dictionary<int, int>() {

                { 1, 500}, { 2, 1020}, { 3, 1561}, { 4, 2123}, { 5, 2708},
                { 6, 3316}, { 7, 3949}, { 8, 4607}, { 9, 5291}, { 10, 6003},
                { 11, 6743}, { 12, 7513}, { 13, 8313}, { 14, 9146}, { 15, 10012},
                { 16, 10912}, { 17, 11849}, { 18, 12823}, { 19, 13836}, { 20, 14889},

                { 21, 15985}, { 22, 17124}, { 23, 18309}, { 24, 19541}, { 25, 20823},
                { 26, 22156}, { 27, 23542}, { 28, 24984}, { 29, 26483}, { 30, 28042},
                { 31, 29664}, { 32, 31351}, { 33, 33105}, { 34, 34929}, { 35, 36826},
                { 36, 38799}, { 37, 40851}, { 38, 42985}, { 39, 45205}, { 40, 47513},

                { 41, 49913}, { 42, 52410}, { 43, 55006}, { 44, 57706}, { 45, 60515},
                { 46, 63435}, { 47, 66473}, { 48, 69632}, { 49, 72917}, { 50, 76334},
                { 51, 79887}, { 52, 83582}, { 53, 87426}, { 54, 91423}, { 55, 95580},
                { 56, 99903}, { 57, 104399}, { 58, 109075}, { 59, 113938}, { 60, 118995},

                { 61, 124255}, { 62, 129725}, { 63, 135414}, { 64, 141331}, { 65, 147484},
                { 66, 153884}, { 67, 160539}, { 68, 167460}, { 69, 174659}, { 70, 182145},
                { 71, 189931}, { 72, 198028}, { 73, 206449}, { 74, 215207}, { 75, 224316},
                { 76, 233788}, { 77, 243640}, { 78, 253885}, { 79, 264541}, { 80, 275622},

                { 81, 287147}, { 82, 299133}, { 83, 311599}, { 84, 324563}, { 85, 338045},
                { 86, 352067}, { 87, 366650}, { 88, 381816}, { 89, 397588}, { 90, 413992},
                { 91, 431051}, { 92, 448793}, { 93, 467245}, { 94, 486435}, { 95, 506392},
                { 96, 527148}, { 97, 548734}, { 98, 571183}, { 99, 594531}, { 100, 618812},


            };

            // private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + "CustomLevel.yml";
            var warningtext = Path.Combine(Paths.ConfigPath, EpicMMOSystem.ModName, $"If you want to stop from updating.txt");
           // var temp = fastJSON.JSON.ToJSON<CustomLevel>(CustomXPTable);
        }

           



        private string GenerateXPTableString(Dictionary<string, int> xpTable)
        {
            int num = 0;
            string text = "";
            foreach (KeyValuePair<string, int> item in xpTable)
            {
                text += ((num != 0) ? ", " : "");
                text = text + item.Key + ":" + item.Value;
                num++;
            }
            return text;
        }

        private void readXPCustomFile()
        {

        }

    }
}

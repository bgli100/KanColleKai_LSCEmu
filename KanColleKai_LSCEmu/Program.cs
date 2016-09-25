using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace KanColleKai_LSCEmu {

    public enum enumMaterialCategory {
        Fuel = 1,
        Bull,
        Steel,
        Bauxite,
        Build_Kit,
        Repair_Kit,
        Dev_Kit,
        Revamp_Kit
    }

    class Program {

        static Random random;
        static IEnumerable<XElement> createLargeTable;
        static IEnumerable<XElement> createLargeTable_change;

        public static int getShipIdLarge(Dictionary<enumMaterialCategory, int> material, int flag_shipid, int emptyOrCompleteDeckCount) {
            enumMaterialCategory steel = enumMaterialCategory.Steel;
            enumMaterialCategory fuel = enumMaterialCategory.Fuel;
            enumMaterialCategory bull = enumMaterialCategory.Bull;
            enumMaterialCategory bauxite = enumMaterialCategory.Bauxite;
            enumMaterialCategory devkit = enumMaterialCategory.Dev_Kit;
            int group_id = 0;
            if (material[steel] >= 2800 + random.Next(1400) && 
                material[fuel] >= 2400 + random.Next(1200) && 
                material[bull] >= 1050 + random.Next(900) && 
                material[bauxite] >= 2800 + random.Next(2400) && 
                material[devkit] >= 1 + random.Next(0)) {
                group_id = 1;
            } else if (material[steel] >= 4400 + random.Next(2200) && 
                material[fuel] >= 2240 + random.Next(1120) && 
                material[bull] >= 2940 + random.Next(2520) && 
                material[bauxite] >= 1050 + random.Next(900) && 
                material[devkit] >= 20 + random.Next(0)) {
                group_id = 2;
            } else if (material[steel] >= 3040 + random.Next(1520) && 
                material[fuel] >= 1920 + random.Next(960) && 
                material[bull] >= 2240 + random.Next(1920) && 
                material[bauxite] >= 910 + random.Next(780) && 
                material[devkit] >= 1 + random.Next(0)) {
                group_id = 3;
            } else {
                group_id = 4;
            }

            int key6 = emptyOrCompleteDeckCount - 1;
            Dictionary<int, int[]> dictionary = new Dictionary<int, int[]> {
                {0, new int[] {3, 100}},
                {1, new int[] {1, 100}},
                {2, new int[] {1, 96}},
                {3, new int[] {1, 92}}
            };
            int num2 = random.Next(dictionary[key6][0], dictionary[key6][1]);
            Dictionary<enumMaterialCategory, double[]> resourceFactors = new Dictionary<enumMaterialCategory, double[]>();
            if (group_id == 1) {
                resourceFactors = new Dictionary<enumMaterialCategory, double[]> {
                    {steel, new double[] {4000, 0.004}},
                    {fuel, new double[] {3000, 0.003}},
                    {bull, new double[] {2000, 0.003}},
                    {bauxite, new double[] {5000, 0.005}},
                    {devkit, new double[] {50, 0.1}}
                };
            } else if (group_id == 2) {
                resourceFactors = new Dictionary<enumMaterialCategory, double[]> {
                    {steel, new double[] {5500, 0.004}},
                    {fuel, new double[] {3500, 0.003}},
                    {bull, new double[] {4500, 0.005}},
                    {bauxite, new double[] {2200, 0.002}},
                    {devkit, new double[] {60, 0.2}}
                };
            } else if (group_id == 3) {
                resourceFactors = new Dictionary<enumMaterialCategory, double[]> {
                    {steel, new double[] {4000, 0.003}},
                    {fuel, new double[] {2500, 0.002}},
                    {bull, new double[] {3000, 0.003}},
                    {bauxite, new double[] {1800, 0.002}},
                    {devkit, new double[] {40, 0.2}}
                };
            } else if (group_id == 4) {
                resourceFactors = new Dictionary<enumMaterialCategory, double[]> {
                    {steel, new double[] {3000, 0.002}},
                    {fuel, new double[] {2000, 0.002}},
                    {bull, new double[] {2500, 0.003}},
                    {bauxite, new double[] {1500, 0.002}},
                    {devkit, new double[] {40, 0.2}}
                };
            }
            int num3 = (int)(((double)material[steel] - resourceFactors[steel][0]) * resourceFactors[steel][1]);
            int num4 = (int)(((double)material[fuel] - resourceFactors[fuel][0]) * resourceFactors[fuel][1]);
            int num5 = (int)(((double)material[bull] - resourceFactors[bull][0]) * resourceFactors[bull][1]);
            int num6 = (int)(((double)material[bauxite] - resourceFactors[bauxite][0]) * resourceFactors[bauxite][1]);
            int num7 = (int)(((double)material[devkit] - resourceFactors[devkit][0]) * resourceFactors[devkit][1]);
            int num8 = num3 + num4 + num5 + num6 + num7;
            int num9 = (num8 >= 0) ? random.Next(num8) : 0;
            if (num9 > 50) {
                num9 = 50;
            }
            int num10 = num2 - num9;
            if (num10 < 1) {
                num10 = 2 - num10;
            }
            if (createLargeTable == null) {
                createLargeTable = Xml_Result("mst_createship_large", "mst_createship_large", "Id");
            }
            var tmp = from data in createLargeTable
                      where data.Element("Group_id").Value == group_id.ToString()
                      select new {
                          id = int.Parse(data.Element("Id").Value),
                          ship_id = int.Parse(data.Element("Ship_id").Value),
                          change_flag = int.Parse(data.Element("Change_flag").Value)
                      };
            var __AnonType = tmp.Skip(num10).First();
            int result = __AnonType.ship_id;
            if (__AnonType.change_flag == 1) {
                int changeCreateShipId = getChangeCreateShipId(flag_shipid, __AnonType.id);
                if (changeCreateShipId != -1) {
                    result = changeCreateShipId;
                }
            }
            return result;
        }


        public static IEnumerable<XElement> Xml_Result(string tableName, string recordName, string sortName) {
            string text = tableName + ".xml";
            if (!File.Exists(text)) {
                return null;
            }
            IEnumerable<XElement> result;
            try {
                if (string.IsNullOrEmpty(sortName)) {
                    result = from datas in XElement.Load(text).Elements(recordName)
                             select datas;
                } else {
                    result = from datas in XElement.Load(text).Elements(recordName)
                             orderby int.Parse(datas.Element(sortName).Value)
                             select datas;
                }
            } catch {
                return null;
            }
            return result;
        }

        private static int getChangeCreateShipId(int flag_shipid, int create_id) {

            IEnumerable<XElement> source;

            if (createLargeTable_change == null) {
                createLargeTable_change = Xml_Result("mst_createship_large_change", "mst_createship_large_change", "Id");
            }
            source = createLargeTable_change;

            var source2 = from data in source
                          where data.Element("Flag_ship_id").Value == flag_shipid.ToString()
                          where data.Element("Ship_create_id").Value == create_id.ToString()
                          select new {
                              changed_ship_id = int.Parse(data.Element("Changed_ship_id").Value)
                          };
            if (!source2.Any()) {
                return -1;
            }
            return source2.First().changed_ship_id;
        }
        static void Main(string[] args) {
            int fuel = 0, ammo = 0, steel = 0, bauxite = 0, devkit = 0, flagship = 0, emptyslot = 0, loopcount = 0;
            foreach(string arg in args) {
                string[] arr = arg.Trim().Split('=');
                switch (arr[0]) {
                    case "--fuel":
                        fuel = int.Parse(arr[1]);
                        break;
                    case "--ammo":
                        ammo = int.Parse(arr[1]);
                        break;
                    case "--steel":
                        steel = int.Parse(arr[1]);
                        break;
                    case "--bauxite":
                        bauxite = int.Parse(arr[1]);
                        break;
                    case "--devkit":
                        devkit = int.Parse(arr[1]);
                        break;
                    case "--flagship":
                        flagship = int.Parse(arr[1]);
                        break;
                    case "--emptyslot":
                        emptyslot = int.Parse(arr[1]);
                        break;
                    case "--loopcount":
                        loopcount = int.Parse(arr[1]);
                        break;
                }
            }

            if(fuel == 0 || ammo == 0 || steel == 0 || bauxite == 0 || devkit == 0 || flagship == 0 || emptyslot == 0 || loopcount == 0) {
                Console.WriteLine("Wrong Input.");
                return;
            }
            Dictionary<enumMaterialCategory, int> material = new Dictionary<enumMaterialCategory, int> {
                [enumMaterialCategory.Fuel] = fuel,
                [enumMaterialCategory.Bull] = ammo,
                [enumMaterialCategory.Steel] = steel,
                [enumMaterialCategory.Bauxite] = bauxite,
                [enumMaterialCategory.Dev_Kit] = devkit
            };

            IEnumerable<XElement> shiplist = Xml_Result("mst_ship", "mst_ship", String.Empty);


            Dictionary<int, int> results = new Dictionary<int, int>();
            for(int i = loopcount; i --> 0;) {
                random = new Random((int)DateTime.Now.Ticks);
                
                int tmp = getShipIdLarge(material, flagship, emptyslot);
                if (results.ContainsKey(tmp))
                    results[tmp]++;
                else
                    results.Add(tmp, 1);
            }
            foreach(KeyValuePair<int, int> x in results) {
                var ship = from data in shiplist
                           where data.Element("Id").Value == x.Key.ToString()
                           select new {
                               name = data.Element("Name").Value
                           };
                Console.WriteLine(string.Format("{0}: {1} 次, {2}%", ship.First().name, x.Value, (double)x.Value / loopcount * 100));
            }
            //
            Console.ReadKey();
        }

    }
}

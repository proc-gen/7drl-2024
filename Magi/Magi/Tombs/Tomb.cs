using Magi.Constants;
using Magi.Containers.DatasetContainers;
using Magi.Maps;
using Magi.Maps.Generators;
using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Tombs
{
    public class Tomb
    {
        static Dictionary<string, MageNameContainer> MageNameContainers;

        static Tomb()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Content", "Datasets", "mage-names.json");
            MageNameContainers = JsonFileManager.LoadDataset<MageNameContainer>(path);
        }

        public Dictionary<int, string> Levels { get; set; }
        public int CurrentLevel { get; protected set; }
        public Elements Element { get; set; }
        public string Mage { get; set; }

        Random Random;
        RandomTable<string> GeneratorTable;
        public Tomb(int playerLevel, Elements element)
        {
            Element = element;
            Random = new Random();
            GeneratorTable = new RandomTable<string>()
                .Add("Random", 1)
                .Add("RoomsAndCorridors", 1)
                .Add("BspRoom", 1)
                .Add("BspInterior", 1)
                .Add("CellularAutomata", 1)
                .Add("DrunkardWalk", 1);

            int numLevels = 2 + playerLevel / 8;
            CurrentLevel = -1;
            Mage = MageNameContainers.ToList()[Random.Next(MageNameContainers.Count)].Value.Name;
            Levels = new Dictionary<int, string>();
            for(int i = 0; i < numLevels; i++)
            {
                Levels.Add(i, GeneratorTable.Roll(Random));
            }
        }

        public Generator IncrementLevel(int playerLevel)
        {
            CurrentLevel++;
            return GenerateLevel(playerLevel);
        }

        private Generator GenerateLevel(int playerLevel)
        {
            var generator = GetGenerator(Levels[CurrentLevel], playerLevel);
            generator.Generate();
            return generator;
        }

        private Generator GetGenerator(string key, int playerLevel)
        {
            int width = 50 + playerLevel * 2;

            switch (key)
            {
                case "RoomsAndCorridors":
                    return new RoomsAndCorridorsGenerator(width, width);
                case "BspRoom":
                    return new BspRoomGenerator(width, width);
                case "BspInterior":
                    return new BspInteriorGenerator(width, width);
                case "CellularAutomata":
                    return new CellularAutomataGenerator(width, width);
                case "DrunkardWalk":
                    return new DrunkardWalkGenerator(width, width);
                case "Random":
                default:
                    return new RandomGenerator(width, width);
            }
        }
    }
}

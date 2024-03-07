using Magi.Constants;
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
        public Dictionary<int, Generator> Levels { get; set; }
        public int CurrentLevel { get; set; }
        public Elements Element { get; set; }

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
            CurrentLevel = 0;

            Levels = new Dictionary<int, Generator>();
            for(int i = 0; i < numLevels; i++)
            {
                Levels.Add(i, GenerateLevel());
            }
        }

        private Generator GenerateLevel()
        {
            var generator = GetRandomGenerator();
            generator.Generate();
            return generator;
        }

        private Generator GetRandomGenerator()
        {
            var key = GeneratorTable.Roll(Random);
            switch (key)
            {
                case "RoomsAndCorridors":
                    return new RoomsAndCorridorsGenerator(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
                case "BspRoom":
                    return new BspRoomGenerator(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
                case "BspInterior":
                    return new BspInteriorGenerator(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
                case "CellularAutomata":
                    return new CellularAutomataGenerator(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
                case "DrunkardWalk":
                    return new DrunkardWalkGenerator(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
                case "Random":
                default:
                    return new RandomGenerator(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
            }
        }
    }
}

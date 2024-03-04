using Arch.Core;
using Arch.Core.Extensions;
using Magi.ECS.Components;
using Magi.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Utils
{
    public static class SaveGameManager
    {
        public static GameWorld NewGame()
        {
            DeleteSaveData();
            return new GameWorld();
        }

        public static GameWorld LoadGame()
        {
            string data = string.Empty;
            using (StreamReader file = new StreamReader("savegame.json"))
            {
                data = file.ReadToEnd();
            }

            GameWorld world = null;

            using (var sr = new StringReader(data))
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    world = serializer.Deserialize<GameWorld>(reader);
                }
            }

            world = PostLoadProcessing(world);
            return world;
        }

        private static GameWorld PostLoadProcessing(GameWorld world)
        {
            world.World = SerializableWorld.CreateWorldFromSerializableWorld(world.SerializableWorld);

            QueryDescription query = new QueryDescription().WithAll<Position>();
            world.World.Query(in query, (Entity entity, ref Position pos) =>
            {
                var reference = entity.Reference();
                world.PhysicsWorld.AddEntity(reference, pos.Point);
                if (entity.Has<Player>())
                {
                    world.PlayerReference = reference;
                }
            });

            return world;
        }

        public static void SaveGame(GameWorld world)
        {
            world.SerializableWorld = SerializableWorld.CreateSerializableWorld(world.World);

            string jsonData;
            using (var sw = new StringWriter())
            {
                if (world != null)
                {
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(writer, world);
                    }
                }
                jsonData = sw.ToString();
            }

            DeleteSaveData();

            using (StreamWriter file = new StreamWriter("savegame.json"))
            {
                file.Write(jsonData);
            }
        }

        public static void DeleteSaveData()
        {
            if (File.Exists("savegame.json"))
            {
                File.Delete("savegame.json");
            }
        }
    }
}

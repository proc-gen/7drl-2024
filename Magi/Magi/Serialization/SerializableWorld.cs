using Arch.Core.Extensions;
using Arch.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Magi.ECS.Helpers;

namespace Magi.Serialization
{
    public class SerializableWorld
    {
        public List<SerializableEntity> Entities { get; set; }

        public static SerializableWorld CreateSerializableWorld(World world)
        {
            SerializableWorld serializableWorld = new SerializableWorld();

            QueryDescription query = new QueryDescription();

            serializableWorld.Entities = new List<SerializableEntity>(world.CountEntities(in query));

            world.Query(in query, (Entity entity) =>
            {
                var components = entity.GetAllComponents();
                serializableWorld.Entities.Add(SerializableEntity.SerializeEntity(entity, components));
            });

            return serializableWorld;
        }

        public static World CreateWorldFromSerializableWorld(SerializableWorld serializableWorld)
        {
            var world = World.Create();

            foreach (var serializableEntity in serializableWorld.Entities)
            {
                serializableEntity.EntityReference = world.CreateFromArray(serializableEntity.GetDeserializedComponents()).Reference();
            }

            foreach (var entity in serializableWorld.Entities)
            {
                SetEntityReferencesForComponents(serializableWorld, entity);
            }

            return world;
        }

        private static void SetEntityReferencesForComponents(SerializableWorld serializableWorld, SerializableEntity entity)
        {
            var entityComponents = entity.EntityReference.Entity.GetAllComponents();
            foreach (var component in entityComponents)
            {
                var ct = component.GetType();
                PropertyInfo[] properties = ct.GetProperties();

                if (properties.Where(a => a.PropertyType.Name == "EntityReference").Any())
                {
                    var jObject = (JObject)entity.Components[ct];

                    foreach (var pi in properties.Where(a => a.PropertyType.Name == "EntityReference"))
                    {
                        pi.SetValue(component, FindNewReference(serializableWorld, (int)jObject[pi.Name]["Entity"]["Id"], (int)jObject[pi.Name]["Version"]));
                    }

                    entity.EntityReference.Entity.Set(component);
                }
            }
        }

        private static EntityReference FindNewReference(SerializableWorld serializableWorld, int id, int version)
        {
            if (id == -1 && version == -1)
            {
                return EntityReference.Null;
            }

            return serializableWorld.Entities.Where(a => a.SourceId == id && a.SourceVersionId == version).First().EntityReference;
        }
    }
}

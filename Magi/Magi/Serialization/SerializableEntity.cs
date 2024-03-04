using Arch.Core;
using Arch.Core.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Serialization
{
    public class SerializableEntity
    {
        public int SourceId { get; set; }
        public int SourceVersionId { get; set; }
        [JsonIgnore]
        public EntityReference EntityReference { get; set; }
        public Dictionary<Type, object> Components { get; set; }

        public SerializableEntity(int id, int versionId)
        {
            SourceId = id;
            SourceVersionId = versionId;
            Components = new Dictionary<Type, object>();
            EntityReference = EntityReference.Null;
        }

        public static SerializableEntity SerializeEntity(Entity entity, object[] components)
        {
            var ec = new SerializableEntity(entity.Id, entity.Version());

            foreach (var component in components)
            {
                ec.Components.Add(component.GetType(), component);
            }

            return ec;
        }

        public object[] GetDeserializedComponents()
        {
            List<object> components = new List<object>();
            JObjectConverter jObjectConverter = new JObjectConverter();
            foreach (var component in Components)
            {
                components.Add(jObjectConverter.Convert(component.Key, (JObject)component.Value));
            }

            return components.ToArray();
        }
    }
}

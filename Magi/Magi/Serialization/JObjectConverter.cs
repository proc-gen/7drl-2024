using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Serialization
{
    public class JObjectConverter
    {
        public object Convert(Type t, JObject data)
        {
            MethodInfo method = GetType().GetMethod("ConvertToTyped")
                             .MakeGenericMethod([t]);
            return method.Invoke(this, [data]);
        }

        public T ConvertToTyped<T>(JObject data)
            where T : new()
        {
            T retVal = Activator.CreateInstance<T>();

            var DeserializedType = typeof(T);

            PropertyInfo[] properties = DeserializedType.GetProperties();
            FieldInfo[] fields = DeserializedType.GetFields();

            TypedReference tRef = __makeref(retVal);

            foreach (var pi in properties)
            {
                if (data[pi.Name] != null)
                {
                    switch (pi.PropertyType.Name)
                    {
                        case "Color":
                            pi.SetValue(retVal, new Color((uint)data[pi.Name]["_packedValue"]));
                            break;
                        case "Point":
                            pi.SetValue(retVal, new Point((int)data[pi.Name]["X"], (int)data[pi.Name]["Y"]));
                            break;
                        case "EntityReference":
                            //Handled in second pass after creating entities and their components
                            break;
                        default:
                            pi.SetValue(retVal, System.Convert.ChangeType(data[pi.Name], pi.PropertyType));
                            break;
                    }
                }
            }

            foreach (var fi in fields)
            {
                if (data[fi.Name] != null)
                {
                    fi.SetValueDirect(tRef, System.Convert.ChangeType(data[fi.Name], fi.FieldType));
                }
            }

            return (T)retVal;
        }
    }
}

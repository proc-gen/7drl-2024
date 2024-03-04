using Arch.Core.Utils;
using Arch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.Core.Extensions;

namespace Magi.ECS.Helpers
{
    public static class ArchExtensions
    {
        private static ComponentType[] GetComponentTypesForArchetype(object[] components)
        {
            ComponentType[] types = new ComponentType[components.Length];
            for (int i = 0; i < components.Length; i++)
            {
                ComponentType type;
                if (!ComponentRegistry.TryGet(components[i].GetType(), out type))
                {
                    type = ComponentRegistry.Add(components[i].GetType());
                }
                types[i] = type;
            }
            return types;
        }

        public static Entity CreateFromArray(this World world, object[] components)
        {
            ComponentType[] types = GetComponentTypesForArchetype(components);
            Entity entity = world.Create(types);
            entity.SetFromArray(components);
            return entity;
        }

        public static void AddFromArray(this Entity entity, object[] components)
        {
            switch (components.Length)
            {
                case 1:
                    entity.AddRange(components[0]);
                    break;
                case 2:
                    entity.AddRange(components[0], components[1]);
                    break;
                case 3:
                    entity.AddRange(components[0], components[1], components[2]);
                    break;
                case 4:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3]);
                    break;
                case 5:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3], components[4]);
                    break;
                case 6:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3], components[4], components[5]);
                    break;
                case 7:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6]);
                    break;
                case 8:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7]);
                    break;
                case 9:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8]);
                    break;
                case 10:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9]);
                    break;
                case 11:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10]);
                    break;
                case 12:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11]);
                    break;
                case 13:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12]);
                    break;
                case 14:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13]);
                    break;
                case 15:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14]);
                    break;
                case 16:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14],
                                components[15]);
                    break;
                case 17:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14],
                                components[15], components[16]);
                    break;
                case 18:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14],
                                components[15], components[16], components[17]);
                    break;
                case 19:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14],
                                components[15], components[16], components[17],
                                components[18]);
                    break;
                case 20:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14],
                                components[15], components[16], components[17],
                                components[18], components[19]);
                    break;
                case 21:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14],
                                components[15], components[16], components[17],
                                components[18], components[19], components[20]);
                    break;
                case 22:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14],
                                components[15], components[16], components[17],
                                components[18], components[19], components[20],
                                components[21]);
                    break;
                case 23:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14],
                                components[15], components[16], components[17],
                                components[18], components[19], components[20],
                                components[21], components[22]);
                    break;
                case 24:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14],
                                components[15], components[16], components[17],
                                components[18], components[19], components[20],
                                components[21], components[22], components[23]);
                    break;
                case 25:
                    entity.AddRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14],
                                components[15], components[16], components[17],
                                components[18], components[19], components[20],
                                components[21], components[22], components[23],
                                components[24]);
                    break;
            }
        }

        public static void SetFromArray(this Entity entity, object[] components)
        {
            switch (components.Length)
            {
                case 1:
                    entity.SetRange(components[0]);
                    break;
                case 2:
                    entity.SetRange(components[0], components[1]);
                    break;
                case 3:
                    entity.SetRange(components[0], components[1], components[2]);
                    break;
                case 4:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3]);
                    break;
                case 5:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3], components[4]);
                    break;
                case 6:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3], components[4], components[5]);
                    break;
                case 7:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6]);
                    break;
                case 8:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7]);
                    break;
                case 9:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8]);
                    break;
                case 10:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9]);
                    break;
                case 11:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10]);
                    break;
                case 12:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11]);
                    break;
                case 13:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12]);
                    break;
                case 14:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13]);
                    break;
                case 15:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14]);
                    break;
                case 16:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14],
                                components[15]);
                    break;
                case 17:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14],
                                components[15], components[16]);
                    break;
                case 18:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14],
                                components[15], components[16], components[17]);
                    break;
                case 19:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14],
                                components[15], components[16], components[17],
                                components[18]);
                    break;
                case 20:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14],
                                components[15], components[16], components[17],
                                components[18], components[19]);
                    break;
                case 21:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14],
                                components[15], components[16], components[17],
                                components[18], components[19], components[20]);
                    break;
                case 22:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14],
                                components[15], components[16], components[17],
                                components[18], components[19], components[20],
                                components[21]);
                    break;
                case 23:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14],
                                components[15], components[16], components[17],
                                components[18], components[19], components[20],
                                components[21], components[22]);
                    break;
                case 24:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14],
                                components[15], components[16], components[17],
                                components[18], components[19], components[20],
                                components[21], components[22], components[23]);
                    break;
                case 25:
                    entity.SetRange(components[0], components[1], components[2],
                                components[3], components[4], components[5],
                                components[6], components[7], components[8],
                                components[9], components[10], components[11],
                                components[12], components[13], components[14],
                                components[15], components[16], components[17],
                                components[18], components[19], components[20],
                                components[21], components[22], components[23],
                                components[24]);
                    break;
            }
        }

        public static void RemoveFromArray(this World world, Entity entity, object[] components)
        {
            ComponentType[] componentsToRemove = new ComponentType[components.Length];

            for (int i = 0; i < components.Length; i++)
            {
                ComponentType type;
                if (!ComponentRegistry.TryGet(components[i].GetType(), out type))
                {
                    type = ComponentRegistry.Add(components[i].GetType());
                }
                componentsToRemove[i] = type;
            }

            world.RemoveRange(entity, componentsToRemove);
        }
    }
}

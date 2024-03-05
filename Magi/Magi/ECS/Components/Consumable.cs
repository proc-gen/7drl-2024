using Magi.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Components
{
    public struct Consumable
    {
        public ConsumableTypes ConsumableType { get; set; }
        public int ConsumableAmount { get; set; }
    }
}

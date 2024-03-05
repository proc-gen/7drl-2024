using Magi.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Containers
{
    public struct ConsumableContainer : IContainer
    {
        public string Name { get; set; }
        public ConsumableTypes ConsumableType { get; set; }
        public int ConsumableAmount { get; set; }
    }
}

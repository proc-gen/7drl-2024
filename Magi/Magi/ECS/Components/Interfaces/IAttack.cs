using Arch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Components.Interfaces
{
    public interface IAttack
    {
        EntityReference Source { get; set; }
        EntityReference Target { get; set; }
    }
}

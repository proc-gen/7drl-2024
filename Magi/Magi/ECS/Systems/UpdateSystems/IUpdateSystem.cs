using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Systems.UpdateSystems
{
    public interface IUpdateSystem
    {
        void Update(TimeSpan delta);
    }
}

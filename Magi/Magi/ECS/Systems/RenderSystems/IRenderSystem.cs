using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Systems.RenderSystems
{
    public interface IRenderSystem
    {
        void Render(ScreenSurface screen);
    }
}

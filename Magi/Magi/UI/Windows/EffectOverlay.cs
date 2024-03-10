using Magi.ECS.Systems.RealTimeRenderSystems;
using Magi.ECS.Systems.RealTimeUpdateSystems;
using Magi.ECS.Systems.RenderSystems;
using Magi.ECS.Systems.UpdateSystems;
using Magi.Utils;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.UI.Windows
{
    public class EffectOverlay : Overlay
    {
        GameWorld World;
        List<IUpdateSystem> realTimeUpdateSystems;
        List<IRenderSystem> realTimeRenderSystems;

        public EffectOverlay(GameWorld world)
            : base()
        {
            World = world;
            Visible = true;
            Console.Surface.DefaultBackground = new Color(0, 0, 0, 0);

            realTimeUpdateSystems = new List<IUpdateSystem>()
            {
                new TimedLifeSystem(world),
            };

            realTimeRenderSystems = new List<IRenderSystem>()
            {
                new RenderRealtimeRenderablesSystem(world),
            };
        }

        public override bool HandleKeyboard(Keyboard keyboard)
        {
            return false;
        }

        public override void Update(TimeSpan delta)
        {
            foreach (var system in realTimeUpdateSystems)
            {
                system.Update(delta);
            }
        }

        public override void Render(TimeSpan delta)
        {
            Console.Clear();
            foreach(var system in realTimeRenderSystems)
            {
                system.Render(Console);
            }
            Console.Render(delta);
        }
    }
}

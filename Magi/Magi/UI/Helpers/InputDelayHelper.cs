using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.UI.Helpers
{
    public class InputDelayHelper
    {
        float ResponseTime = .15f;
        public bool ReadyForInput { get; private set; }
        float counter = 0f;

        public InputDelayHelper(float delay = .15f)
        {
            ReadyForInput = false;
            ResponseTime = delay;
        }

        public void Update(TimeSpan delta)
        {
            if (!ReadyForInput)
            {
                counter += (float)delta.TotalSeconds;
                if (counter > ResponseTime)
                {
                    counter = 0f;
                    ReadyForInput = true;
                }
            }
        }

        public void Reset()
        {
            ReadyForInput = false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.UI.Windows
{
    public abstract class Overlay : Window
    {
        public Overlay()
            : base()
        {
            Console.Surface.DefaultBackground = new Color(0, 0, 0, 0);
        }
    }
}

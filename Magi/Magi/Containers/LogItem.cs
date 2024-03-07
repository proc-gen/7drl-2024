using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Containers
{
    public class LogItem
    {
        public DateTime LogTime { get; set; }
        public string LogText { get; set; }

        public LogItem(string logText)
        {
            LogTime = DateTime.Now;
            LogText = logText;
        }

        public override string ToString()
        {
            // return string.Concat("[", LogTime.ToString("MM/dd/yy H:mm:ss"), "] ", LogText);
            return LogText;
        }
    }
}

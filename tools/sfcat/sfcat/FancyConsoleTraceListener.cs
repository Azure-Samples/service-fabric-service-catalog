using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat
{
    public class FancyConsoleTraceListener: ConsoleTraceListener
    {
        
        public override void WriteLine(string message)
        {
            FancyConsole.WriteColorCodedLine("#r" + message);
        }
    }
}

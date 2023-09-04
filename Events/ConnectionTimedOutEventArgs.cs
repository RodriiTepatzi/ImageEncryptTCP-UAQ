using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEncryptTCP.Events
{
    public class ConnectionTimedOutEventArgs
    {
        public bool IsTimedOut { get; }

        public ConnectionTimedOutEventArgs(bool isTimedOut)
        {
            IsTimedOut = isTimedOut;
        }
    }
}

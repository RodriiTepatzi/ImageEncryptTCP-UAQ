using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEncryptTCP.Events
{
    public class ConnectionChangedEventArgs : EventArgs
    {
        public bool IsConnected { get; }

        public ConnectionChangedEventArgs(bool isConnected)
        {
            IsConnected = isConnected;
        }
    }
}

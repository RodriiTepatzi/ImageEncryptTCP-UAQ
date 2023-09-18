using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEncryptTCP.Events
{
	public class MessageChangedEventArgs : EventArgs
	{

		public string Message { get; }
        public MessageChangedEventArgs(string message)
        {
                this.Message = message;
        }
    }
}

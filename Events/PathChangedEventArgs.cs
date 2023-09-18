using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEncryptTCP.Events
{
	public class PathChangedEventArgs : EventArgs
	{
		public string Path;
		public PathChangedEventArgs(string path) 
		{ 
			this.Path = path;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ImageEncryptTCP.Events;

namespace ImageEncryptTCP.Manager
{
    public class ConnectionManager
	{
		private static ConnectionManager _instance = new ConnectionManager();
		public static ConnectionManager Instance { get { return _instance; } }
		public string? ToIPAddress { get; set; }
		public string? Port { get; set; }
		public string? ImagePath { get; set; }
		public string? EncryptKey { get; set; }
		public bool IsActive { get; set; }

		public event EventHandler<ConnectionChangedEventArgs>? ConnectionChanged;
		public event EventHandler<ConnectionTimedOutEventArgs>? ConnectionTimedOut;

		private ConnectionManager() 
		{
			IsActive = true;
		}

		public async void StartClient()
		{
			try
			{
				if(Instance.ToIPAddress == null)
				{
					throw new Exception("An IP Address is needed.");
				}

                if(Instance.Port == null)
                {
					throw new Exception("A port is needed.");
                }

				Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(Instance.ToIPAddress), int.Parse(Instance.Port));

				Console.WriteLine("Conectando al servidor...");
				await client.ConnectAsync(remoteEP);
				Console.WriteLine("Conectado al servidor {0}", remoteEP);

				while (Instance.IsActive)
				{
					Instance.SendData(client);
				}

				client.Shutdown(SocketShutdown.Both);
				client.Close();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		private void SendData(Socket client)
		{
			if (!string.IsNullOrEmpty(Instance.ImagePath))
			{
				byte[] img = File.ReadAllBytes(Instance.ImagePath);
				Console.WriteLine("Ingresa la llave: ");
				String key = Console.ReadLine();
				byte[] keyb = Encoding.ASCII.GetBytes(key!);

				var encryptedImg = EncryptionManager.EncryptImage(Instance.ImagePath);
				List<char> imgb = encryptedImg.Select(i => (char)i).ToList();
				byte[] imgEb = Encoding.ASCII.GetBytes(imgb.ToArray());

				byte[] combinedData = new byte[sizeof(int) + imgEb.Length + keyb.Length];
				BitConverter.GetBytes(imgEb.Length).CopyTo(combinedData, 0);
				imgEb.CopyTo(combinedData, sizeof(int));
				keyb.CopyTo(combinedData, sizeof(int) + imgEb.Length);
				client.Send(combinedData);

				Console.WriteLine("Datos enviados");
				IsActive = false;
			}
		}

		private void OnConnectionChanged(ConnectionChangedEventArgs e)
		{
			ConnectionChanged?.Invoke(this, e);
		}

		private void OnTimedOutChanged(ConnectionTimedOutEventArgs e)
		{
			ConnectionTimedOut?.Invoke(this, e);
		}

		private void HandleConnectionStatusChange(bool isConnected) => OnConnectionChanged(new ConnectionChangedEventArgs(isConnected));
		private void HandleTimedOutStatusChange(bool isTimedOut) => OnTimedOutChanged(new ConnectionTimedOutEventArgs(isTimedOut));
	}
}

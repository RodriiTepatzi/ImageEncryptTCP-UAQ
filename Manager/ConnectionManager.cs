using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ImageEncryptTCP.Manager
{
	public class ConnectionManager
	{
		private static ConnectionManager _instance = new ConnectionManager();
		public static ConnectionManager Instance { get { return _instance; } }
		public string? ToIPAddress { get; set; }
		public string? Port { get; set; }
		public string? ImagePath { get; set; }

		private ConnectionManager() 
		{

		}


		static async void StartClient()
		{
			try
			{
				//Creacion del socket
				if(Instance.ToIPAddress == null)
				{
					throw new Exception();
				}

                if(Instance.Port == null)
                {
					throw new Exception();
                }

				if (Instance.ImagePath == null)
				{
					throw new Exception();
				}

				Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(Instance.ToIPAddress), int.Parse(Instance.Port));

				//Se conecta el cliente al socket
				Console.WriteLine("Conectando al servidor...");
				await client.ConnectAsync(remoteEP);
				Console.WriteLine("Conectado al servidor {0}", remoteEP);

				while (true)
				{
					//Se convierte la imagen en bytes
					byte[] img = File.ReadAllBytes(Instance.ImagePath);
					//Ingresa la llave
					Console.WriteLine("Ingresa la llave: ");
					String key = Console.ReadLine();
					//Se convierte la llave en bytes
					byte[] keyb = Encoding.ASCII.GetBytes(key);

					var encryptedImg = EncryptionManager.EncryptImage(Instance.ImagePath);
					List<char> imgb = encryptedImg.Select(i => (char)i).ToList();
					byte[] imgEb = Encoding.ASCII.GetBytes(imgb.ToArray());

					//Se convinan los bytes para poder mandarlos como uno solo
					byte[] combinedData = new byte[sizeof(int) + imgEb.Length + keyb.Length];
					BitConverter.GetBytes(imgEb.Length).CopyTo(combinedData, 0);
					imgEb.CopyTo(combinedData, sizeof(int));
					keyb.CopyTo(combinedData, sizeof(int) + imgEb.Length);

					//Se envian los bytes combinados
					client.Send(combinedData);

					Console.WriteLine("Datos enviados");
					break;
				}

				client.Shutdown(SocketShutdown.Both);
				client.Close();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}



	}
}

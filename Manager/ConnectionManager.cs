using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ImageEncryptTCP.Events;
using System.Threading;
using System.Security.Cryptography.Xml;

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
        public bool DataReceived { get; set; }

        public event EventHandler<ConnectionChangedEventArgs>? ConnectionChanged;
        public event EventHandler<ConnectionTimedOutEventArgs>? ConnectionTimedOut;
        public event EventHandler<MessageChangedEventArgs>? MessageChanged;
        public event EventHandler<PathChangedEventArgs>? PathChanged;

	    private CancellationTokenSource cancellationTokenSource;

        private ConnectionManager()
        {
            IsActive = true;
            DataReceived = false;
		}

        public void StartClient()
        {
            if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
            {
                Console.WriteLine("Client is already running.");
                HandleConnectionStatusChange(true);
                HandleTimedOutStatusChange(true);

                return;
            }

            cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => StartClientAsync(cancellationTokenSource.Token));
            HandleConnectionStatusChange(true);
            HandleTimedOutStatusChange(true);
        }

        public void StopClient()
        {
            cancellationTokenSource?.Cancel();
        }

        private async Task StartClientAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (Instance.ToIPAddress == null)
                {
                    throw new Exception("An IP Address is needed.");
                }

                if (Instance.Port == null)
                {
                    throw new Exception("A port is needed.");
                }

                using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(Instance.ToIPAddress), int.Parse(Instance.Port));

                    Console.WriteLine("Conectando al servidor...");
                    await client.ConnectAsync(remoteEP);
                    Console.WriteLine("Conectado al servidor {0}", remoteEP);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        Instance.SendData(client);
                        Instance.ReceiveAndProcessData(client);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void SendData(Socket client)
        {
            while (IsActive)
            {
                if (!string.IsNullOrEmpty(Instance.ImagePath))
                {
                    byte[] img = File.ReadAllBytes(Instance.ImagePath);
                    byte[] keyb = Encoding.ASCII.GetBytes(Instance.EncryptKey!);

                    var encryptedImg = EncryptionManager.EncryptImage(Instance.ImagePath, Instance.EncryptKey!);

                    Console.WriteLine("Datos enviados");

                    string encryptionKey = Instance.EncryptKey!;
                    byte[] keyBytes = Encoding.ASCII.GetBytes(encryptionKey);

                    RC4 rc4 = new RC4(keyBytes);

                    byte[] dataToEncrypt = File.ReadAllBytes(Instance.ImagePath);

                    byte[] encryptedData = rc4.Encrypt(dataToEncrypt);

                    byte[] combinedData = new byte[sizeof(int) + encryptedData.Length + keyBytes.Length];
                    BitConverter.GetBytes(encryptedData.Length).CopyTo(combinedData, 0);
                    encryptedData.CopyTo(combinedData, sizeof(int));
                    keyBytes.CopyTo(combinedData, sizeof(int) + encryptedData.Length);


                    client.Send(combinedData);

                    IsActive = false;
                }
            }
        }


        private void ReceiveAndProcessData(Socket client)
        {
            if (!DataReceived)
            {
                int bufferSize = 8192;
                byte[] buffer = new byte[bufferSize];
                int bytesRead = client.Receive(buffer);

                if (bytesRead == 0)
                {
                    HandleConnectionStatusChange(false);
                    return;
                }

				byte[] img = File.ReadAllBytes(Instance.ImagePath!);
				byte[] bufferFit = new byte[bytesRead];

                for(int i = 0; i < bytesRead; i++)
                    bufferFit[i] = buffer[i];

				if (AreArraysEqual(img, bufferFit))
				{
					File.WriteAllBytes("desencriptada_cliente.jpg", buffer);
                    HandlePathChanged($"{ Directory.GetCurrentDirectory()}/desencriptada_cliente.jpg");
                    HandleMessageChanged("Las imagenes son iguales");
				}
                else
                {
					HandleMessageChanged("Las imagenes no son iguales");
				}

				DataReceived = true;
			}
		}


		bool AreArraysEqual(byte[] array1, byte[] array2)
		{
			if (ReferenceEquals(array1, array2))
				return true;

			if (array1 == null || array2 == null)
				return false;
            
			if (array1.Length != array2.Length)
				return false;

			for (int i = 0; i < array1.Length; i++)
			    if (array1[i] != array2[i])
					return false;
			

			return true;
		}

		private void OnConnectionChanged(ConnectionChangedEventArgs e) => ConnectionChanged?.Invoke(this, e);
		private void OnTimedOutChanged(ConnectionTimedOutEventArgs e) => ConnectionTimedOut?.Invoke(this, e);
        private void OnMessageChanged(MessageChangedEventArgs e) => MessageChanged?.Invoke(this, e);
        private void OnPathChanged(PathChangedEventArgs e) => PathChanged?.Invoke(this, e);
		private void HandleConnectionStatusChange(bool isConnected) => OnConnectionChanged(new ConnectionChangedEventArgs(isConnected));
		private void HandleTimedOutStatusChange(bool isTimedOut) => OnTimedOutChanged(new ConnectionTimedOutEventArgs(isTimedOut));
        private void HandleMessageChanged(string message) => OnMessageChanged(new MessageChangedEventArgs(message));
        private void HandlePathChanged(string path) => OnPathChanged(new PathChangedEventArgs(path));

	}
}
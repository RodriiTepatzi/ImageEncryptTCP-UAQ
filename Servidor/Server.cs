using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Servidor
{
    class Program
    {
        static void StartServer()
        {
            try
            {
                // Se crea el servidor
                Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint connect = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6400);

                server.Bind(connect);
                server.Listen(1);

                Console.WriteLine("Esperando conexión...");
                using (Socket client = server.Accept())
                {
                    Console.WriteLine("Conexión establecida...");

                    // Aquí se coloca la lógica para recibir los datos encriptados del cliente

                    // Crear un buffer para recibir datos
                    byte[] buffer = new byte[8192];
                    int bytesRead = client.Receive(buffer);

                    // Obtener el tamaño de la clave
                    int keySize = BitConverter.ToInt32(buffer, 0);
                    int encryptedImageSize = BitConverter.ToInt32(buffer, sizeof(int));

                    // Leer la clave
                    byte[] key = new byte[keySize];
                    byte[] encryptedImage = new byte[encryptedImageSize];

                    Array.Copy(buffer, 2 * sizeof(int), key, 0, keySize);
                    Array.Copy(buffer, 2 * sizeof(int) + keySize, encryptedImage, 0, encryptedImageSize);

                    // Desencriptar la imagen utilizando RC4
                    using (RC4 rc4Alg = new RC4(key))
                    {
                        byte[] decryptedImage = rc4Alg.Decrypt(encryptedImage);

                        // Enviar el tamaño de la imagen desencriptada al cliente
                        byte[] decryptedImageSizeBytes = BitConverter.GetBytes(decryptedImage.Length);
                        client.Send(decryptedImageSizeBytes);

                        // Enviar la imagen desencriptada al cliente
                        client.Send(decryptedImage);

                        Console.WriteLine("Imagen desencriptada enviada al cliente.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando el servidor");
            StartServer();
        }
    }

    class RC4
    {
        private byte[] s;
        private byte[] key;
        private int i;
        private int j;

        public RC4(byte[] key)
        {
            this.key = key;
            s = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                s[i] = (byte)i;
            }

            j = 0;
            for (int i = 0; i < 256; i++)
            {
                j = (j + s[i] + key[i % key.Length]) % 256;
                byte temp = s[i];
                s[i] = s[j];
                s[j] = temp;
            }

            i = j = 0;
        }

        public byte[] Decrypt(byte[] data)
        {
            byte[] decryptedData = new byte[data.Length];
            for (int x = 0; x < data.Length; x++)
            {
                i = (i + 1) % 256;
                j = (j + s[i]) % 256;
                byte temp = s[i];
                s[i] = s[j];
                s[j] = temp;
                int t = (s[i] + s[j]) % 256;
                decryptedData[x] = (byte)(data[x] ^ s[t]);
            }
            return decryptedData;
        }
    }
}

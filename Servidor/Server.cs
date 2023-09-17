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
                    int ivSize = BitConverter.ToInt32(buffer, sizeof(int));
                    int encryptedImageSize = BitConverter.ToInt32(buffer, 2 * sizeof(int));

                    // Leer la clave y el IV
                    byte[] key = new byte[keySize];
                    byte[] iv = new byte[ivSize];
                    byte[] encryptedImage = new byte[encryptedImageSize];

                    Array.Copy(buffer, 3 * sizeof(int), key, 0, keySize);
                    Array.Copy(buffer, 3 * sizeof(int) + keySize, iv, 0, ivSize);
                    Array.Copy(buffer, 3 * sizeof(int) + keySize + ivSize, encryptedImage, 0, encryptedImageSize);

                    // Desencriptar la imagen utilizando AES
                    using (Aes aesAlg = Aes.Create())
                    {
                        aesAlg.Key = key;
                        aesAlg.IV = iv;

                        using (MemoryStream msDecryptedImage = new MemoryStream())
                        {
                            using (CryptoStream csDecrypt = new CryptoStream(msDecryptedImage, aesAlg.CreateDecryptor(), CryptoStreamMode.Write))
                            {
                                csDecrypt.Write(encryptedImage, 0, encryptedImage.Length);
                            }

                            // Obtener la imagen desencriptada en forma de bytes
                            byte[] decryptedImageBytes = msDecryptedImage.ToArray();

                            // Enviar el tamaño de la imagen desencriptada al cliente
                            byte[] decryptedImageSizeBytes = BitConverter.GetBytes(decryptedImageBytes.Length);
                            client.Send(decryptedImageSizeBytes);

                            // Enviar la imagen desencriptada al cliente
                            client.Send(decryptedImageBytes);

                            Console.WriteLine("Imagen desencriptada enviada al cliente.");
                        }
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
            //JASIEL PUTO
        }
    }
}

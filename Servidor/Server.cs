using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Servidor
{
    class Program
    {
        static void Main(string[] args)
        {
            StartServer();

        }

        static void StartServer()
        {
            try
            {
                //Se crea el servidor
                Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint connect = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6400);

                server.Bind(connect);
                server.Listen(1);

                Console.WriteLine("Esperando conexion...");
                Socket client = server.Accept();
                Console.WriteLine("Esperando conexion...");

                //Aqui se pone el path por si se quiere guardar la imagen encriptada
                //string folderPath = "";

                while (true)
                {
                    //Se pone un buffer de mayor tamaño para que los bytes de la imagen puedan ser recibidos
                    byte[] buffer = new byte[8192];
                    //Se reciben los datos combinados
                    int byteRec = client.Receive(buffer);
                    //Se obtiene el tamaño de la imagen para poder separar los bytes
                    int imgsize = BitConverter.ToInt32(buffer, 0);

                    //Se obtienen los bytes de la imagen
                    byte[] img = new byte[imgsize];
                    Array.Copy(buffer, sizeof(int), img, 0, imgsize);
                    //Se obtiene la llave en string
                    string key = Encoding.ASCII.GetString(buffer, sizeof(int) + imgsize, byteRec - sizeof(int) - imgsize);
                    //Console.WriteLine("La llave es: {0}", key);
                    
                    //Nombre del archivo si se quiere guardar en el servidor y opciones si se quiere guardar la imagen
                    //string imageFileName = "";
                    //string imagePath = Path.Combine(folderPath, imageFileName);
                    //File.WriteAllBytes(imagePath, img);

                    //Console.WriteLine("Imagen guardada");
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

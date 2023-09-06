using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Cliente
{
    class Program
    {
        static void Main(string[] args)
        {
            StartClient();
        }

        static void StartClient()
        {
            try
            {
                //Creacion del socket
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6400);

                //Se conecta el cliente al socket
                Console.WriteLine("Conectando al servidor...");
                client.Connect(remoteEP);
                Console.WriteLine("Conectado al servidor {0}", remoteEP);

                while (true)
                {
                    //Aqui se pone la direccion de la imagen
                    String path = "";
                    //Se convierte la imagen en bytes
                    byte[] img = File.ReadAllBytes(path);
                    //Ingresa la llave
                    Console.WriteLine("Ingresa la llave: ");
                    String key = Console.ReadLine();
                    //Se convierte la llave en bytes
                    byte[] keyb = Encoding.ASCII.GetBytes(key);

                    /*
                    Aqui se supone que se manda llamar a la encriptacion
                    List <int> imgE = new List <int>(Encript(img, keyb));
                    byte[] imgEb = Encoding.ASCII.GetBytes(imgE);
                    */

                    //Se convinan los bytes para poder mandarlos como uno solo
                    byte[] combinedData = new byte[sizeof(int) + imgb.Length + keyb.Length];
                    BitConverter.GetBytes(imgb.Length).CopyTo(combinedData, 0);
                    imgb.CopyTo(combinedData, sizeof(int));
                    keyb.CopyTo(combinedData, sizeof(int) + imgb.Length);
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

        static void CombineData()
        {
            try
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
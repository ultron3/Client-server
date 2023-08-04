using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace NewClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string serverIP = "192.168.3.232";
            int serverPort = 5555;

            TcpClient client = new TcpClient();
            client.Connect(serverIP, serverPort);

            Console.WriteLine("Connected to the server.");

            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);
     
            
            string welcomeMessage = reader.ReadLine();
            Console.WriteLine(welcomeMessage);

            string message;
            while (true)
            {
                message = Console.ReadLine();

                if (message.ToLower() == "exit")
                {
                    break;
                }

                writer.WriteLine(message);
                writer.Flush();
                Console.WriteLine("Message sent to the server");

                try
                {
                    string serverResponse = reader.ReadLine();
                    Console.WriteLine("Response from the server: " + serverResponse);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            client.Close();

            Console.WriteLine("Connection closed.");

        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace Projetc_contact_server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Server server = new Server("192.168.3.232", 5555);
            Console.WriteLine("Server started. Waiting for incoming connections...");
        }
    }

    public class Server
    {
        // ... (properties and constructor remain the same)

        public void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();

            try
            {
                using (StreamReader reader = new StreamReader(stream))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    bool isRunning = true;
                    while (isRunning)
                    {
                        string dataReceived = reader.ReadLine();

                        if (dataReceived == null)
                        {
                            isRunning = false;
                            break;
                        }

                        // Handle different operations based on received data
                        if (dataReceived.StartsWith("delete "))
                        {
                            string jsonToDelete = dataReceived.Substring(7);
                            // Handle deletion logic here...
                            // ...
                            string response = "Contact deleted successfully.";
                            writer.WriteLine(response);
                        }
                        else if (dataReceived == "exit")
                        {
                            isRunning = false;
                        }
                        else
                        {
                            // Handle contact insertion logic here...
                            // ...
                            string response = "Data received successfully.";
                            writer.WriteLine(response);
                        }

                        writer.Flush();
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                ConnectedClients.Remove(client);
                client.Close();
                Console.WriteLine("Client disconnected.");
            }
        }

        // ... (other methods remain the same)
    }
}

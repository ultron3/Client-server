using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Server
{
    private TcpListener listener;
    private List<NetworkStream> clientStreams;

    public Server(int port)
    {
        listener = new TcpListener(IPAddress.Any, port);
        clientStreams = new List<NetworkStream>();
    }

    public void Start()
    {
        listener.Start();
        Console.WriteLine("Server started. Waiting for clients...");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected.");

            NetworkStream clientStream = client.GetStream();
            clientStreams.Add(clientStream);

            Thread clientThread = new Thread(() => HandleClient(clientStream));
            clientThread.Start();
        }
    }

    private void HandleClient(NetworkStream clientStream)
    {
        try
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = clientStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received message: " + message);

                // Inoltra il messaggio a tutti gli altri client
                foreach (var stream in clientStreams)
                {
                    if (stream != clientStream)
                    {
                        byte[] sendData = Encoding.ASCII.GetBytes(message);
                        stream.Write(sendData, 0, sendData.Length);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
        finally
        {
            clientStreams.Remove(clientStream);
            clientStream.Close();
        }
    }

    public static void Main(string[] args)
    {
        Server server = new Server(5555);
        server.Start();
    }
}

/* il server continuerà ad accettare nuove connessioni client all'interno del primo ciclo while,
 mentre all'interno del secondo ciclo while verranno gestiti i messaggi ricevuti dai client.
 Quando un client invia il messaggio "exit", 
 la connessione con quel client verrà chiusa e il server tornerà al primo ciclo while per accettare nuove connessioni.*/
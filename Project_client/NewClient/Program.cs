using System;
using System.IO;
using System.Net.Sockets;

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Client
{
    private static TcpClient client;
    private static NetworkStream stream;

    public static void Main(string[] args)
    {
        string serverIP = "192.168.3.232";
        int serverPort = 5555;

        client = new TcpClient();
        client.Connect(serverIP, serverPort);

        Console.WriteLine("Connected to the server.");

        stream = client.GetStream();

        Thread receiveThread = new Thread(ReceiveMessages);
        receiveThread.Start();

        string message;
        while (true)
        {
            message = Console.ReadLine();

            if (message.ToLower() == "exit")
            {
                break;
            }

            byte[] sendData = Encoding.ASCII.GetBytes(message);
            stream.Write(sendData, 0, sendData.Length);
        }

        stream.Close();
        client.Close();

        Console.WriteLine("Connection closed.");
    }

    private static void ReceiveMessages()
    {
        try
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received: " + message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }
}

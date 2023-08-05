using System;
using System.IO;
using System.Net.Sockets;

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

            // Leggi il messaggio di benvenuto dal server
            string welcomeMessage = reader.ReadLine();
            Console.WriteLine(welcomeMessage);

            // Inizializza una nuova istanza del gestore dei messaggi in uscita
            MessageSender messageSender = new MessageSender(writer);

            // Avvia un thread separato per gestire la lettura dei messaggi in arrivo dal server
            MessageReceiver messageReceiver = new MessageReceiver(reader);
            messageReceiver.MessageReceived += (sender, receivedMessage) =>
            {
                Console.WriteLine("Message from the server: " + receivedMessage);
            };
            messageReceiver.Start();

            string message;
            while (true)
            {
                message = Console.ReadLine();

                if (message.ToLower() == "exit")
                {
                    break;
                }

                // Invia il messaggio al server tramite il gestore dei messaggi in uscita
                messageSender.SendMessage(message);
            }

            // Chiudi la connessione con il server
            client.Close();

            Console.WriteLine("Connection closed.");
        }
    }

    // Classe per la gestione dell'invio dei messaggi al server
    public class MessageSender
    {
        private StreamWriter writer;

        public MessageSender(StreamWriter writer)
        {
            this.writer = writer;
        }

        public void SendMessage(string message)
        {
            // Invia il messaggio al server e lo flusha per assicurarsi che venga inviato subito
            writer.WriteLine(message);
            writer.Flush();
            Console.WriteLine("Message sent to the server");
        }
    }

    // Classe per la gestione della ricezione dei messaggi dal server
    public class MessageReceiver
    {
        private StreamReader reader{get;set;}
        private bool receiving{get;set;}
        public event EventHandler<string> MessageReceived{get;set;}

        public MessageReceiver(StreamReader reader)
        {
            this.reader = reader;
            this.receiving = false;
        }

        public void Start()
        {
            receiving = true;
            while (receiving)
            {
                try
                {
                    // Leggi il messaggio dal server
                    string serverResponse = reader.ReadLine();
                    if (serverResponse == null)
                    {
                        receiving = false; // Esci dal ciclo se non ci sono più messaggi
                        break;
                    }

                    // Scatena l'evento per notificare il messaggio ricevuto
                    OnMessageReceived(serverResponse);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                    receiving = false;
                }
            }
        }

        protected virtual void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(this, message);
        }
    }
}

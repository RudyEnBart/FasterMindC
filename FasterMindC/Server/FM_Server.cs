using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    class FM_Server
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            new FM_Server();
        }

        public FM_Server()
        {
            TcpListener server = new TcpListener(_localIP, 11000);
            server.Start();

            TcpClient incomingClient;
            Console.WriteLine("Waiting for connection...");
            while (true)
            {
                incomingClient = server.AcceptTcpClient();

                new Thread(() =>
                {
                    Console.WriteLine("Connection found!");
                    BinaryFormatter formatter = new BinaryFormatter();
                    TcpClient sender = incomingClient;
                    SslStream sslStream = new SslStream(incomingClient.GetStream());
                    sslStream.AuthenticateAsServer(certificate);

                    while (true)
                    {

                        String dataString = "";
                        FM_Packet packet = null;
                        if (sender.Connected)
                        {
                            dataString = (String)formatter.Deserialize(sslStream);
                            //dataString = (String)formatter.Deserialize(sender.GetStream());
                            packet = Utils.GetPacket(dataString);

                            //Console.WriteLine(dataString);

                            //Console.WriteLine("Incoming action" + packet._type);
                            switch (packet._type)
                            {
                                //sender = incoming client
                                //packet = data van de client
                                case "Chat":
                                    HandleChatPacket(packet);
                                    break;
                                default: //nothing
                                    break;
                            }
                        }
                    } // end While
                }).Start();
            }

        }
    }
}

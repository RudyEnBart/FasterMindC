using FMNetworkLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using System.Web.Script.Serialization;

namespace Server
{
    class FM_Server
    {
        private IPAddress _localIP = IPAddress.Parse("127.0.0.1");

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
                            packet = new JavaScriptSerializer().Deserialize<FM_Packet>(dataString);

                            //Console.WriteLine(dataString);

                            //Console.WriteLine("Incoming action" + packet._type);
                            switch (packet._type)
                            {
                                //sender = incoming client
                                //packet = data van de client
                                case "Chat":
                                    HandleConnectPacket(packet);
                                    break;
                                default: //nothing
                                    break;
                            }
                        }
                    } // end While
                }).Start();
            }

        }

        private void HandleConnectPacket(FM_Packet p)
        {

        }
    }
}

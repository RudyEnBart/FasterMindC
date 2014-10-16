using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
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
        private byte _codePlayer1 = (byte)ENUMS.color.BLUE;
        private byte _codePlayer2;
        static void Main()
        {
            new FM_Server();
        }

        public FM_Server()
        {
            TcpListener _server = null;
            try
            {
                X509Certificate2 _certificate = new X509Certificate2("C:\\certs\\testpfx.pfx", "FIETSA3");
                IPAddress _localIP = IPAddress.Parse("127.0.0.1");
                byte _port = 42;
                _server = new TcpListener(_localIP, _port);
                _server.Start();
                Console.WriteLine("Waiting for connection...");

                while (true)
                {

                    TcpClient _clientConnection = _server.AcceptTcpClient();

                    new Thread(() =>
                    {
                        Console.WriteLine("Connection found!");
                        BinaryFormatter formatter = new BinaryFormatter();
                        SslStream sslStream = new SslStream(_clientConnection.GetStream(), false);
                        try
                        {
                            sslStream.AuthenticateAsServer(_certificate, false, SslProtocols.Tls, false);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception: {0}", e.Message);
                            if (e.InnerException != null)
                            {
                                Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                            }
                            Console.WriteLine("Authentication failed - closing the connection.");
                            sslStream.Close();
                            _clientConnection.Close();
                            return;
                        }
                        while (true)
                        {
                            String dataString = "";
                            FM_Packet packet = null;
                            if (_clientConnection.Connected)
                            {
                                dataString = (String) formatter.Deserialize(sslStream);
                                packet = new JavaScriptSerializer().Deserialize<FM_Packet>(dataString);
                                switch (packet._type)
                                {
                                        //sender = incoming client
                                        //packet = data van de client
                                    case "Connect":
                                        HandleConnectPacket(packet);
                                        break;
                                    case "InitialCode":
                                        HandleInitialCodePacket(packet);
                                        break;
                                    case "CodeSubmit":
                                        HandleCodeSubmitPacket(packet);
                                        break;
                                    case "NameChange":
                                        HandleNameChangePacket(packet);
                                        break;
                                    case "Disconnect":
                                        HandleDisconnectPacket(packet);
                                        break;
                                    default: //nothing
                                        break;
                                }
                            }
                        } // end While
                    }).Start();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: " + e);
            }
            finally
            {
                _server.Stop();
            }

        }

        private void HandleConnectPacket(FM_Packet p)
        {
            throw new NotImplementedException();
        }

        private void HandleDisconnectPacket(FM_Packet packet)
        {
            throw new NotImplementedException();
        }

        private void HandleNameChangePacket(FM_Packet packet)
        {
            throw new NotImplementedException();
        }

        private void HandleCodeSubmitPacket(FM_Packet packet)
        {
            throw new NotImplementedException();
        }

        private void HandleInitialCodePacket(FM_Packet packet)
        {
            throw new NotImplementedException();
        }
    }
}

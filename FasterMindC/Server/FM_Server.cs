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
        private const byte MAX_PLAYERS = 2;

        private string[] _playerCodes = new string[MAX_PLAYERS];
        private SslStream[] streamArray = new SslStream[MAX_PLAYERS];
        private short _playerCount = 0;
        private BinaryFormatter formatter = new BinaryFormatter();
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
                        if (_playerCount < MAX_PLAYERS)
                        {
                            Console.WriteLine("Connection found!");
                            BinaryFormatter formatter = new BinaryFormatter();
                            SslStream sslStream = new SslStream(_clientConnection.GetStream(), false);
                            try
                            {
                                Console.WriteLine("Authenticating connection...");
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
                            streamArray[_playerCount] = sslStream;
                            _playerCount++;
                            SendPacket(new FM_Packet("ID", "" + _playerCount), sslStream);
                            Console.WriteLine("Authentication succesfull.");
                            while (true)
                            {
                                String dataString = "";
                                FM_Packet packet = null;
                                if (_clientConnection.Connected)
                                {
                                    dataString = (String)formatter.Deserialize(sslStream);
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
                        }
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
            int OpponentID = CheckID(packet);
            SendPacket(packet, streamArray[OpponentID]);
        }

        private void HandleCodeSubmitPacket(FM_Packet packet)
        {
            int result = 0;
            int ComparePacket = CheckID(packet);
            int i = 0;
            while (i < 4)
            {
                i++;
                if (_playerCodes[ComparePacket].Substring(i, 1).Equals(packet._message.Substring(i, 1)))
                {
                    result += 10;
                    break;
                }
                if (_playerCodes[ComparePacket].Contains(packet._message.Substring(i, 1)))
                {
                    result += 1;
                }
            }
            if (result == 40)
            {
                SendPacket(new FM_Packet(ComparePacket + "", "GameLost", "You are a n00b and lost the game!"));
            }
            SendPacket(new FM_Packet(packet._id, "CodeResult", result + ""));
        }

        private void HandleInitialCodePacket(FM_Packet packet)
        {
            Console.WriteLine("Received code: " + packet._message + " from player " + packet._id);
            _playerCodes[int.Parse(packet._id)-1] = packet._message;
            if (_playerCodes.Count(s => s != null) == MAX_PLAYERS)
            {
                foreach (SslStream s in streamArray)
                {
                    SendPacket(new FM_Packet("Ready", "GET READY"), s);
                }
            }
        }

        public void SendPacket(FM_Packet packet)
        {
            formatter.Serialize(streamArray[int.Parse(packet._id)], new JavaScriptSerializer().Serialize(packet));
        }

        public void SendPacket(FM_Packet packet, SslStream stream)
        {
            formatter.Serialize(stream, new JavaScriptSerializer().Serialize(packet));
        }

        public int CheckID(FM_Packet packet)
        {
            int OpponentID;
            if (packet._id == "1")
            {
                OpponentID = 1;
            }
            else
            {
                OpponentID = 0;
            }
            return OpponentID;
        }
    }
}

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
using System.Text;

namespace Server
{
    class FM_Server
    {
        private const byte MAX_PLAYERS = 2;

        private string[] _playerCodes = new string[MAX_PLAYERS];
        private SslStream[] streamArray = new SslStream[MAX_PLAYERS];
        private short _playerCount = 0;
        private BinaryFormatter formatter = new BinaryFormatter();
        private byte _connectPackages = 0;
        private DateTime _timeStart;
        private DateTime _timeEnd;
        private string _highscoreTimeName = "No one";
        private long _highscoreTime = 10*60*1000;
        private string _mostWinsName = "No one";
        private int _mostWins = 0;
        private string[] _names = {"Anonymous", "Anonymous"};
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
                byte port = FM_Settings.SERVERPORT;
                _server = new TcpListener(/*_localIP,*/ port);
                _server.Start();
                Console.WriteLine("Current fastest time is: " + FormatTime(_highscoreTime) + " by '" + _highscoreTimeName + "'");
                Console.WriteLine("Current recordholder for most wins is: '" + _mostWinsName + "' with " + _mostWins + " wins");
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
                            SendPacket(new FM_Packet("ID", "" + _playerCount), sslStream);
                            _playerCount++;
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
                                        case "Highscores":
                                            HandleHighscoresPacket(packet);
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

        private void HandleHighscoresPacket(FM_Packet packet)
        {
            Console.WriteLine("Testing sending highscore packet");
            string highscores = "Current fastest time is: " + FormatTime(_highscoreTime) + " by '" + _highscoreTimeName + "'" +
                "\nCurrent most wins recordholder is: '" + _mostWinsName + "' with " + _mostWins + " wins";
            Console.WriteLine(highscores);
            SendPacket(new FM_Packet(packet._id, "Highscores", highscores));
            Console.WriteLine("Testing sending highscore packet succesful");
        }

        private void HandleConnectPacket(FM_Packet packet)
        {
            _connectPackages++;
            Console.WriteLine("amount of connectpackages: " + _connectPackages);
            if (_connectPackages == MAX_PLAYERS)
            {
                FM_Packet p = new FM_Packet("Connect", "All players have connected");
                foreach (SslStream s in streamArray)
                {
                    SendPacket(p, s);
                } 
                _connectPackages = 0;
            }
        }

        private void HandleDisconnectPacket(FM_Packet packet)
        {
            throw new NotImplementedException();
        }

        private void HandleNameChangePacket(FM_Packet packet)
        {
            int OpponentID = CheckID(packet);
            _names[Byte.Parse(packet._id)] = packet._message;
            SendPacket(packet, streamArray[OpponentID]);
        }

        private void HandleCodeSubmitPacket(FM_Packet packet)
        {
            int result = 0;
            int OpponentID = CheckID(packet);
            //Console.WriteLine("code to check: " + packet._message + " - code to check against: " + _playerCodes[OpponentID]);
            //int timesWhileloopfinished = 0;
            string tempOpponentCode = _playerCodes[OpponentID];
            //Console.WriteLine("tempcode is: " + tempOpponentCode + "and playercode is: " + _playerCodes[OpponentID]);
            string checkCode = packet._message;
            StringBuilder sbOppCode = new StringBuilder(tempOpponentCode);
            StringBuilder sbCheckCode = new StringBuilder(checkCode);

            for (int i = 0; i < tempOpponentCode.Length; i++ )
            {
                if (tempOpponentCode.Substring(i, 1).Equals(checkCode.Substring(i, 1)))
                {
                    Console.WriteLine("+10!");
                    sbOppCode[i] = 'x';
                    tempOpponentCode = sbOppCode.ToString();
                    sbCheckCode[i] = 'y';
                    checkCode = sbCheckCode.ToString();
                    result += 10;
                }
                //Console.WriteLine("Result has changed to: " + result);
                //Console.WriteLine("tempcodes are: " + tempOpponentCode + " and " + checkCode);
            }
            for (int i = 0; i < tempOpponentCode.Length; i++)
            {
                if (tempOpponentCode.Contains(checkCode.Substring(i, 1)))
                {
                    Console.WriteLine("+1!");
                    int index = tempOpponentCode.IndexOf(checkCode.Substring(i, 1));
                    sbOppCode[index] = 'x';
                    tempOpponentCode = sbOppCode.ToString();
                    sbCheckCode[i] = 'y';
                    checkCode = sbCheckCode.ToString();
                    result += 1;
                }
                //Console.WriteLine("Result has changed to: " + result);
                //Console.WriteLine("tempcodes are: " + tempOpponentCode + " and " + checkCode);
            }

            if (result == 40)
            {
                _timeEnd = DateTime.Now;
                Console.WriteLine("OMG Player " + packet._id + " HAS WON THE GAME!");
                SendPacket(new FM_Packet(packet._id, "CodeResult", result + ""));
                SendPacket(new FM_Packet(OpponentID + "", "GameLost", "Your opponent guessed your code!\nSadly, you have lost."));
                _playerCodes = new string[MAX_PLAYERS];
                long totalTicksGameTime = (_timeEnd.Ticks - _timeStart.Ticks) / TimeSpan.TicksPerMillisecond;
                if(totalTicksGameTime < _highscoreTime)
                {
                    Console.WriteLine("New fastest time! Old fastest time was: " + FormatTime(_highscoreTime) + " - New fastest time is: " + FormatTime(totalTicksGameTime));
                    _highscoreTime = totalTicksGameTime;
                    _highscoreTimeName = _names[Byte.Parse(packet._id)];
                }
            }
            else
            {
                if (result < 10)
                {
                    SendPacket(new FM_Packet(packet._id, "CodeResult", "0" + result));
                    SendPacket(new FM_Packet(OpponentID + "", "OpponentSubmit", packet._message)); 
                }
                else
                {
                    SendPacket(new FM_Packet(packet._id, "CodeResult", result + ""));
                    SendPacket(new FM_Packet(OpponentID + "", "OpponentSubmit", packet._message)); 
                }
            }          
        }

        private string FormatTime(long milliseconds)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(milliseconds);
            string answer = string.Format("{1:D2}m:{2:D2}s:{3:D3}ms",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds,
                                    t.Milliseconds);
            return answer;
        }

        private void HandleInitialCodePacket(FM_Packet packet)
        {
            Console.WriteLine("Received code: " + packet._message + " from player " + packet._id);
            _playerCodes[int.Parse(packet._id)] = packet._message;
            if (_playerCodes.Count(s => s != null) == MAX_PLAYERS)
            {
                foreach (SslStream s in streamArray)
                {
                    SendPacket(new FM_Packet("Ready", "GET READY"), s);
                }
                _timeStart = DateTime.Now;
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
                OpponentID = 0;
            }
            else
            {
                OpponentID = 1;
            }
            return OpponentID;
        }
    }
}

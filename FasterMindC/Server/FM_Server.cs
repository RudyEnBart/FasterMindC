﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using FMNetworkLibrary;

namespace Server
{
    internal class FM_Server
    {
        private const byte MAX_PLAYERS = 2;

        private byte _connectPackages;
        private long[] _fastestTimes = new long[10];
        private string[] _fastestTimesNames = new string[10];
        private long _highscoreTime = 10*60*1000;
        private string _highscoreTimeName = "No one";
        private short _mostWins;
        private string _mostWinsName = "Anonymous";
        private string[] _names = {"Anonymous", "Anonymous"};
        private string[] _playerCodes = new string[MAX_PLAYERS];
        private short _playerCount;
        private String[] _playerNames = new String[MAX_PLAYERS];
        private Stopwatch _stopWatch = new Stopwatch();
        private DateTime _timeEnd;
        private DateTime _timeStart;
        private short[] _wins = {0, 0};
        private BinaryFormatter formatter = new BinaryFormatter();
        private SslStream[] streamArray = new SslStream[MAX_PLAYERS];
        private byte _ties = 0;

        public FM_Server()
        {
            GetHighscores();
            TcpListener _server = null;
            try
            {
                X509Certificate2 _certificate = new X509Certificate2("C:\\certs\\testpfx.pfx", "FIETSA3");
                IPAddress _localIP = IPAddress.Parse("127.0.0.1");
                byte port = FM_Settings.SERVERPORT;
                _server = new TcpListener( /*_localIP,*/ port);
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
                            SendPacket(new FM_Packet("ID", "" + _playerCount), sslStream);
                            _playerCount++;
                            Console.WriteLine("Authentication succesfull.");
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
                                        case "Highscores":
                                            HandleHighscoresPacket(packet);
                                            break;
                                        case "GameTie":
                                            HandleGameTiePacket(packet);
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

        private void HandleGameTiePacket(FM_Packet packet)
        {
            _ties++;
            if (_ties == 2)
            {
                FM_Packet gameTie = new FM_Packet("GameTie", "Reset game.");
                foreach (SslStream s in streamArray)
                {
                    SendPacket(gameTie,s);
                }
                _ties = 0;
            }
        }

        private static void Main()
        {
            new FM_Server();
        }

        private void GetHighscores()
        {
            _mostWins = (short) DataHandling.ReadData(DataHandling.MOSTWINS);
            _mostWinsName = (string) DataHandling.ReadData(DataHandling.MOSTWINSNAME);
            //Console.WriteLine("MOST WINS: " + _mostWins + " by " + _mostWinsName);
            _fastestTimes = (long[]) DataHandling.ReadData(DataHandling.BESTTIMES);
            _fastestTimesNames = (string[]) DataHandling.ReadData(DataHandling.BESTTIMESNAMES);
        }

        private void SetHighscores()
        {
            DataHandling.SaveData(_mostWins, _mostWinsName);
            DataHandling.SaveData(_fastestTimes, _fastestTimesNames);
        }

        private void HandleHighscoresPacket(FM_Packet packet)
        {
            //Console.WriteLine("fastest times are: " + _fastestTimes.ToString());
            string highscores = "Current fastest times are: \n";
            for (int i = 0; i < _fastestTimes.Length; i++)
            {
                if (i == 9)
                {
                    highscores += (i + 1) + ".  '" + _fastestTimesNames[i] + "'    with    " +
                                  FormatTime(_fastestTimes[i]) + "\n";
                }
                else
                {
                    highscores += (i + 1) + ".    '" + _fastestTimesNames[i] + "'    with    " +
                                  FormatTime(_fastestTimes[i]) + "\n";
                }
            }
            highscores += "\nCurrent most wins recordholder is:    '" + _mostWinsName + "'    with    " + _mostWins +
                          " wins";

            //Console.WriteLine(highscores);
            SendPacket(new FM_Packet(packet._id, "Highscores", highscores));
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
            //Console.WriteLine("Player with ID " + packet._id + " has disconnected");
            _playerCount--;
            _names[Int16.Parse(packet._id)] = "Anonymous";
            _wins[Int16.Parse(packet._id)] = 0;
            if (_connectPackages > _playerCount)
            {
                _connectPackages--;
            }
        }

        private void HandleNameChangePacket(FM_Packet packet)
        {
            int OpponentID = CheckID(packet);
            _names[Byte.Parse(packet._id)] = packet._message;
            if (packet._message == _mostWinsName)
            {
                _wins[Byte.Parse(packet._id)] = _mostWins;
            }
            SendPacket(packet, streamArray[OpponentID]);
            if (packet._id == "0")
            {
                _playerNames[0] = packet._message;
            }
            else
            {
                _playerNames[1] = packet._message;
            }
        }

        private void HandleCodeSubmitPacket(FM_Packet packet)
        {
            int result = 0;
            int OpponentID = CheckID(packet);
            //Console.WriteLine("code to check: " + packet._message + " - code to check against: " + _playerCodes[OpponentID]);
            string tempOpponentCode = _playerCodes[OpponentID];
            //Console.WriteLine("tempcode is: " + tempOpponentCode + "and playercode is: " + _playerCodes[OpponentID]);
            string checkCode = packet._message;
            StringBuilder sbOppCode = new StringBuilder(tempOpponentCode);
            StringBuilder sbCheckCode = new StringBuilder(checkCode);

            for (int i = 0; i < tempOpponentCode.Length; i++)
            {
                if (tempOpponentCode.Substring(i, 1).Equals(checkCode.Substring(i, 1)))
                {
                    //Console.WriteLine("+10!");
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
                    //Console.WriteLine("+1!");
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

            if (result == 40)
            {
                _timeEnd = DateTime.Now;
                bool highscoresChanged = false;
                //Console.WriteLine("OMG Player " + packet._id + " HAS WON THE GAME!");
                SendPacket(new FM_Packet(OpponentID + "", "GameLost", "Your opponent guessed your code!\nSadly, you have lost."));
                _playerCodes = new string[MAX_PLAYERS];

                long totalTicksGameTime = ((_timeEnd.Ticks - _timeStart.Ticks)/TimeSpan.TicksPerMillisecond) - 3000;
                byte winID = Byte.Parse(packet._id);
                _wins[Byte.Parse(packet._id)]++;
                if (_wins[winID] > _mostWins)
                {
                    //Console.WriteLine("New record wins!");
                    _mostWins = _wins[winID];
                    _mostWinsName = _names[winID];
                    highscoresChanged = true;
                }
                for (int i = 0; i < _fastestTimes.Length; i++)
                {
                    //Console.WriteLine("i: " + i);
                    //Console.WriteLine("Comparing: " + totalTicksGameTime + " with " + _fastestTimes[i]);
                    if (totalTicksGameTime < _fastestTimes[i])
                    {
                        highscoresChanged = true;
                        //Console.WriteLine("New record time!");
                        for (int j = 9; j > i; j--)
                        {
                            //Console.WriteLine("replacing " + _fastestTimes[j] + " with " + _fastestTimes[j - 1]);
                            _fastestTimes[j] = _fastestTimes[j - 1];
                            //Console.WriteLine("replacing " + _fastestTimesNames[j] + " with " + _fastestTimesNames[j - 1]);
                            _fastestTimesNames[j] = _fastestTimesNames[j - 1];
                        }
                        //Console.WriteLine("replacing " + _fastestTimes[i] + " with " + totalTicksGameTime);
                        _fastestTimes[i] = totalTicksGameTime;
                        _fastestTimesNames[i] = _names[Byte.Parse(packet._id)];
                        SaveTimes(_fastestTimes, _fastestTimesNames);
                        break;
                    }
                }
                if (highscoresChanged)
                {
                    SetHighscores();
                }
            }
        }

        private void SaveTimes(long[] fastestArrayList, string[] fastestStringArrayList)
        {
            //TODO save times and names
        }

        private string FormatTime(long milliseconds)
        {
            //Console.WriteLine("transforming " + milliseconds);
            TimeSpan t = TimeSpan.FromMilliseconds(milliseconds);
            string answer = string.Format("{0:D2}:{1:D2}:{2:D3}",
                t.Minutes,
                t.Seconds,
                t.Milliseconds);
            return answer;
        }

        private void HandleInitialCodePacket(FM_Packet packet)
        {
            //Console.WriteLine("Received code: " + packet._message + " from player " + packet._id);
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
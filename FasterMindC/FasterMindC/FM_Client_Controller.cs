﻿using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using FMNetworkLibrary;

namespace FasterMindC
{
    public class FM_Client_Controller
    {
        private string _ID;

        private Connection_Form _conForm;
        private FM_Client_GUI _gui;
        private int _opTry;
        private string _opponentCode;
        private string _opponentName;
        private StreamReader _reader;
        private GetReady_Form _readyForm;
        private TcpClient _serverConnection;

        private string _serverIP = FM_Settings.SERVERIP;
        private byte _serverPort = FM_Settings.SERVERPORT;
        private SslStream _sslServerConnection;
        private Waiting_Form _waitingForm;
        private BinaryFormatter formatter = new BinaryFormatter();

        public FM_Client_Controller()
        {
            Application.ApplicationExit += OnApplicationExit;
            try
            {
                _serverConnection = new TcpClient();
                _serverConnection.Connect(_serverIP, _serverPort);
                _sslServerConnection = new SslStream(_serverConnection.GetStream(), false,
                    ValidateServerCertificate, null);
                try
                {
                    _sslServerConnection.AuthenticateAsClient(_serverIP, null, SslProtocols.Tls, false);
                    _reader = new StreamReader(_sslServerConnection, Encoding.ASCII);
                }
                catch (Exception e1)
                {
                    Console.WriteLine("Exception: {0}", e1.Message);
                    if (e1.InnerException != null)
                    {
                        Console.WriteLine("Inner exception: {0}", e1.InnerException.Message);
                    }
                    Console.WriteLine("Authentication failed - closing the connection.");
                    _sslServerConnection.Close();
                    _serverConnection.Close();

                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Couldnt connect to server: " + e);
            }
            new Thread(() =>
            {
                while (true)
                {
                    String dataString = "";
                    FM_Packet packet = null;

                    if (_serverConnection.Connected)
                    {
                        dataString = (String) formatter.Deserialize(_sslServerConnection);
                        packet = new JavaScriptSerializer().Deserialize<FM_Packet>(dataString);
                        switch (packet._type)
                        {
                                //sender = incoming client
                                //packet = data van de client
                            case "ID":
                                HandleIDPacket(packet);
                                break;
                            case "OpponentSubmit":
                                HandleOpponentSubmitPacket(packet);
                                break;
                            case "CodeResult":
                                HandleCodeResultPacket(packet);
                                break;
                            case "NameChange":
                                HandleNameChangePacket(packet);
                                break;
                            case "Connect":
                                HandleConnectPacket();
                                break;
                            case "Disconnect":
                                HandleDisconnectPacket(packet);
                                break;
                            case "Ready":
                                HandleReadyPacket();
                                break;
                            case "GameLost":
                                HandleGameLostPacket(packet);
                                break;
                            case "GameTie":
                                HandleGameTiePacket(packet);
                                break;
                            case "Highscores":
                                HandleHighscoresPacket(packet);
                                break;
                            default: //nothing
                                break;
                        }
                    }
                }
            }).Start();
        }

        private string _name { get; set; }
        private int _ownCode { get; set; }
        private int _submitCode { get; set; }
        private bool _firstSubmit { get; set; }
        private byte _attempt { get; set; }

        private static void Main()
        {
            FM_Client_Controller control = new FM_Client_Controller();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            control.init();
            Application.Run(control._gui);
        }

        private void HandleHighscoresPacket(FM_Packet packet)
        {
            MessageBox.Show(packet._message);
        }

        private void HandleGameTiePacket(FM_Packet packet)
        {
            _gui.Enabled = false;
            DialogResult result = MessageBox.Show("The game has ended in a tie... \nDo you wish to play again?", "Tie",
                MessageBoxButtons.YesNo);
            switch (result)
            {
                case DialogResult.Yes:
                    init();
                    break;
                case DialogResult.No:
                    Application.Exit();
                    break;
            }
        }

        public void init()
        {
            _attempt = 0;
            _firstSubmit = true;
            _ownCode = 0;
            _submitCode = 0;
            _opTry = 0;
            InitializeGUI();
            SendConnectPacket();
        }

        private void InitializeGUI()
        {
            if (_gui == null)
            {
                _gui = new FM_Client_GUI(this);
            }
            if (_gui.InvokeRequired)
            {
                InitGUIDel d = InitializeGUI;
                _gui.Invoke(d);
            }
            else
            {
                _conForm = new Connection_Form();
                _conForm.Show(_gui);
                _conForm.TopMost = true;
                _gui.Enabled = false;
                _gui.init();
            }
        }

        private void HandleConnectPacket()
        {
            if (_conForm.InvokeRequired)
            {
                HandleConnectPacketDel d = HandleConnectPacket;
                _conForm.Invoke(d);
            }
            else
            {
                _conForm.Close();
                _gui.Enabled = true;
                _gui.Focus();
                MessageBox.Show("The goal of the game is to guess the code of your opponent.\n" +
                                "By clicking the squares at the bottom, they will change colors to signify a code.\n" +
                                "The first code you submit will be the code your opponent needs to guess.\n" +
                                "After that, every code you submit will be a guess towards your opponent's code.\n" +
                                "Your guesses will be on the left side and your opponent's guesses on the right.\n" +
                                "Next to your guesses you will also see how many of your colors were:\n" +
                                "- The right color and in the right spot (RED).\n" +
                                "- The right color but in the wrong spot (WHITE).\n\n" +
                                "The colors that can make up a code are: Red, Blue, Green, Yellow, Pink and Cyan.\n" +
                                "White counts as an empty space and is not an allowed color within the codes.\n" +
                                "The first player to guess the opponent's code within the 9 guesses, WINS!\n" +
                                "Be the fastest and smartest. Prove that you have the... FASTERMIND!",
                                "Welcome to... FASTERMIND!");
                _gui.Focus();
            }
        }

        private void HandleCodeResultPacket(FM_Packet packet)
        {
            if (_gui.InvokeRequired)
            {
                HandleCodeResultPacketDel d = HandleCodeResultPacket;
                _gui.Invoke(d, new object[] {packet});
            }
            else
            {
                _gui.SetResultColor(_attempt, packet._message);
                // all 10s are red all 1s are white
                if (packet._message == "40")
                {
                    _gui.Enabled = false;
                    DialogResult result = MessageBox.Show("YOU WIN! \nDo you wish to play again?", "You are the winner!", MessageBoxButtons.YesNo);
                    switch (result)
                    {
                        case DialogResult.Yes:
                            init();
                            break;
                        case DialogResult.No:
                            Application.Exit();
                            break;
                    }
                }
                else if (_attempt == 9)
                {
                    _gui.Enabled = false;
                    MessageBox.Show("You failed to guess your opponents code :( \nWaiting for opponent to finish", "Waiting for opponent");
                    SendPacket(new FM_Packet(_ID, "GameTie","Failed to guess within 9 attempts."));
                }
            }
        }

        private void HandleGameLostPacket(FM_Packet packet)
        {
            if (_gui.InvokeRequired)
            {
                HandleGameLostPacketDel d = HandleGameLostPacket;
                _gui.Invoke(d, new object[] {packet});
            }
            else
            {
                _gui.Enabled = false;
                DialogResult result = MessageBox.Show(packet._message + "\nDo you wish to play again?", "You lose!", MessageBoxButtons.YesNo);
                switch (result)
                {
                    case DialogResult.Yes:
                        init();
                        break;
                    case DialogResult.No:
                        Application.Exit();
                        break;
                }
            }
        }

        private void HandleReadyPacket()
        {
            if (_gui.InvokeRequired)
            {
                HandleReadyPacketDel d = HandleReadyPacket;
                _gui.Invoke(d);
            }
            else
            {
                _waitingForm.Close();
                _gui.Enabled = true;
                _readyForm = new GetReady_Form();
                _readyForm.ShowDialog(_gui);
            }
        }

        private void HandleDisconnectPacket(FM_Packet packet)
        {
            throw new NotImplementedException();
        }

        private void HandleNameChangePacket(FM_Packet packet)
        {
            _opponentName = packet._message;
            _gui.ChangeOpponentName(packet._message);
            //Set Label opponent gui.
        }

        private void HandleOpponentSubmitPacket(FM_Packet packet)
        {
            _opponentCode = packet._message;
            _gui.SetOpponentTry(_opTry, packet._message);
            _opTry++;
        }

        private void HandleIDPacket(FM_Packet packet)
        {
            _ID = packet._message;
            Console.WriteLine("ID REGISTERED: " + _ID);
        }

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            var serverCertificate = new X509Certificate2("C:\\certs\\testpfx.pfx", "FIETSA3");
            return serverCertificate.Equals(certificate);
        }

        public void SendPacket(FM_Packet packet)
        {
            formatter.Serialize(_sslServerConnection, new JavaScriptSerializer().Serialize(packet));
        }

        private void SendConnectPacket()
        {
            SendPacket(new FM_Packet("Connect", "Connection established!"));
        }

        public void NameButtonClick(object sender, EventArgs e, string name)
        {
            _name = name;
            SendPacket(new FM_Packet(_ID, "NameChange", name));
        }

        internal void InputCodeClicked(object sender, EventArgs e, int p)
        {
            if (_firstSubmit)
            {
                if ((int) (_ownCode/Math.Pow(10, p - 1))%10 == 6)
                {
                    _ownCode -= (short) (5*Math.Pow(10, p - 1));
                }
                else
                {
                    _ownCode += (short) (Math.Pow(10, p - 1));
                }
            }
            else
            {
                if ((int) (_submitCode/Math.Pow(10, p - 1))%10 == 6)
                {
                    _submitCode -= (short) (5*Math.Pow(10, p - 1));
                }
                else
                {
                    _submitCode += (short) (Math.Pow(10, p - 1));
                }
            }
        }

        internal void SubmitButtonClicked(object sender, EventArgs e)
        {
            if (_serverConnection.Connected)
            {
                if (_firstSubmit)
                {
                    if (_name == null)
                    {
                        MessageBox.Show("Please fill in a name first");
                    }
                    else
                    {
                        if (!("" + _ownCode).Contains("0"))
                        {
                            //Console.WriteLine("Initial code is: " + _ownCode);
                            SendPacket(new FM_Packet(_ID, "InitialCode", "" + _ownCode));
                            _firstSubmit = false;
                            _gui.MoveCode(true, _attempt);
                            _gui.Enabled = false;
                            _waitingForm = new Waiting_Form();
                            _waitingForm.Show();
                            _waitingForm.TopMost = true;
                        }
                        else
                        {
                            MessageBox.Show(
                                "Please fill in all the colors before submitting (white is not counted as a color)");
                        }
                    }
                }
                else
                {
                    if (!("" + _submitCode).Contains("0"))
                    {
                        Console.WriteLine("Submit code is: " + _submitCode);
                        SendPacket(new FM_Packet(_ID, "CodeSubmit", "" + _submitCode));
                        _gui.MoveCode(false, _attempt);
                        _attempt++;
                        _submitCode = 0;
                    }
                    else
                    {
                        MessageBox.Show(
                            "Please fill in all the colors before submitting (white is not counted as a color)");
                    }
                }
            }
        }

        internal void GetHighscores()
        {
            SendPacket(new FM_Packet(_ID, "Highscores", "Request for highscores"));
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            SendPacket(new FM_Packet(_ID, "Disconnect", "Disconnecting"));
        }

        private delegate void HandleCodeResultPacketDel(FM_Packet packet);

        private delegate void HandleConnectPacketDel();

        private delegate void HandleGameLostPacketDel(FM_Packet packet);

        private delegate void HandleReadyPacketDel();

        private delegate void InitGUIDel();
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using FMNetworkLibrary;
using System.Diagnostics;

namespace FasterMindC
{
    public class FM_Client_Controller
    {

        private string _name { get; set; }
        private short _ownCode { get; set; }
        private short _submitCode { get; set; }
        private bool _firstSubmit { get; set; }
        private byte _attempt { get; set; }
        private string _ID;

        private FM_Client_GUI _gui;

        private TcpClient _serverConnection;
        private SslStream _sslServerConnection;
        private StreamReader _reader;
        private BinaryFormatter formatter = new BinaryFormatter();
        private string _intialCodeFromOpponent;
        private string _opponentCode;
        private string _opponentName;

        private string _serverIP = "127.0.0.1";
        private byte _serverPort = 42;
        static void Main()
        {
            FM_Client_Controller control = new FM_Client_Controller();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            control._gui = new FM_Client_GUI(control);
            Application.Run(control._gui);
        }

        public FM_Client_Controller()
        {
            this._ownCode = 0;
            _firstSubmit = true;
            try
            {
                _serverConnection = new TcpClient();
                _serverConnection.Connect(_serverIP, _serverPort);
                _sslServerConnection = new SslStream(_serverConnection.GetStream(), false,
                    new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                try
                {
                    _sslServerConnection.AuthenticateAsClient("127.0.0.1", null, SslProtocols.Tls, false);
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
                        dataString = (String)formatter.Deserialize(_sslServerConnection);
                        packet = new JavaScriptSerializer().Deserialize<FM_Packet>(dataString);
                        switch (packet._type)
                        {
                            //sender = incoming client
                            //packet = data van de client
                            case "ID":
                                HandleIDPacket(packet);
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
                            case "Ready":
                                HandleReadyPacket(packet);
                                break;
                            default: //nothing
                                break;
                        }
                    }
                }
            }).Start();
        }

        private void HandleReadyPacket(FM_Packet packet)
        {
            //TODO make both clients start at the same time
            Debug.WriteLine("START!");
        }

        private void HandleDisconnectPacket(FM_Packet packet)
        {
            throw new NotImplementedException();
        }

        private void HandleNameChangePacket(FM_Packet packet)
        {
            _opponentName = packet._message;
            //Set Label opponent gui.
        }

        private void HandleCodeSubmitPacket(FM_Packet packet)
        {
            _opponentCode = packet._message;
            //Set Color doorsturen naar gui.
        }

        private void HandleIDPacket(FM_Packet packet)
        {
            this._ID = packet._message;
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

        public void NameButtonClick(object sender, EventArgs e, string name)
        {
            this._name = name;
        }

        internal void InputCodeClicked(object sender, EventArgs e, int p)
        {
            if ((short)(_ownCode / Math.Pow(10, p - 1)) % 10 == 6)
            {
                Debug.WriteLine("The number to edit is too big, resetting");
                _ownCode -= (short)(5 * Math.Pow(10, p - 1));
            }
            else
            {
                _ownCode += (short)(Math.Pow(10, p - 1));
                Debug.WriteLine("Changing code to: " + _ownCode);
            }
        }

        internal void SubmitButtonClicked(object sender, EventArgs e)
        {
            if (_serverConnection.Connected)
            {
                if (_firstSubmit)
                {
                    SendPacket(new FM_Packet(_ID, "InitialCode", "" + _ownCode));
                    _firstSubmit = false;
                    _gui.MoveCode(true, _attempt);
                }
                else
                {
                    SendPacket(new FM_Packet(_ID, "CodeSubmit", "" + _submitCode));
                    _gui.MoveCode(false, _attempt);
                    _attempt++;
                }
            }
        }


    }
}

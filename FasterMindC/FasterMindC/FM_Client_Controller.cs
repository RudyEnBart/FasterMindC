using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace FasterMindC
{
    class FM_Client_Controller
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private TcpClient _serverConnection;
        private SslStream _sslServerConnection;
        private StreamReader _reader;
        private BinaryFormatter formatter = new BinaryFormatter();

        private string _serverIP = "127.0.0.1";
        private byte _serverPort = 42;
        static void Main()
        {
            new FM_Client_Controller();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FM_Client_GUI());
            
        }

        public FM_Client_Controller()
        {
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
            }
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

        public void buttonpressed()
        {
            FM_Packet packet = new FM_Packet("CodeSubmit", "1234");
            SendPacket(packet);
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
    }
}

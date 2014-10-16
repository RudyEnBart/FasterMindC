﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FasterMindC
{
    public class FM_Client_Controller
    {

        private string _name { get; set; }
        private short _code { get; set; }

        private TcpClient _serverConnection;
        private SslStream _sslServerConnection;
        private StreamReader _reader;
        static void Main()
        {
            FM_Client_Controller control = new FM_Client_Controller();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FM_Client_GUI(control));
        }

        public FM_Client_Controller()
        {
            this._code = 0;
            try
            {
                _serverConnection = new TcpClient();
                _serverConnection.Connect("127.0.0.1", 42);
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
        }

        public void NameButtonClick(object sender, EventArgs e, string name)
        {
            this._name = name;
        }

        internal void InputCodeClicked(object sender, EventArgs e, int p)
        {
            if ((_code / Math.Pow(1, p - 1)) == 6)
            {

            }
            else
            {
                _code += (short)(Math.Pow(1, p - 1));
            }
        }

        internal void SubmitButtonClicked(object sender, EventArgs e)
        {
            
        }

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            var serverCertificate = new X509Certificate2("C:\\certs\\testpfx.pfx", "FIETSA3");
            return serverCertificate.Equals(certificate);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace FasterMindC
{
    public partial class Connection_Form : Form
    {
        int timesElapsed = 0;
        System.Timers.Timer t = new System.Timers.Timer(1000);
       
        public Connection_Form()
        {
            InitializeComponent();
            t.SynchronizingObject = this;
            t.AutoReset = true;
            t.Elapsed += new ElapsedEventHandler(ChangeText);
            t.Enabled = true;
        }

        private void Connection_Form_Load(object sender, EventArgs e)
        {

        }

        private void ChangeText(object sender, ElapsedEventArgs e)
        {
            if (timesElapsed == 0)
            {
                CON_label.Text = "Waiting for connection";
                timesElapsed++;
            }
            else if (timesElapsed == 1)
            {
                CON_label.Text = "Waiting for connection.";
                timesElapsed++;
            }
            else if (timesElapsed == 2)
            {
                CON_label.Text = "Waiting for connection..";
                timesElapsed++;
            }
            else if (timesElapsed == 3)
            {
                CON_label.Text = "Waiting for connection...";
                timesElapsed = 0;
            }
        }
    }
}

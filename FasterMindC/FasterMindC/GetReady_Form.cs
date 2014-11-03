using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace FasterMindC
{
    public partial class GetReady_Form : Form
    {
        private int timesElapsed = 0;
        private System.Timers.Timer t = new System.Timers.Timer(1000);

        public GetReady_Form()
        {
            InitializeComponent();
            t.SynchronizingObject = this;
            t.AutoReset = true;
            t.Elapsed += new ElapsedEventHandler(ChangeText);
            t.Enabled = true;
        }

        private void ChangeText(object sender, ElapsedEventArgs e)
        {
            if (timesElapsed == 0)
            {
                ReadyLabel.Text = "Starting in 3..";
                timesElapsed++;
            }
            else if (timesElapsed == 1)
            {
                ReadyLabel.Text = "Starting in 3..2..";
                timesElapsed++;
            }
            else if (timesElapsed == 2)
            {
                ReadyLabel.Text = "Starting in 3..2..1..";
                timesElapsed++;
            }
            else if (timesElapsed == 3)
            {
                ReadyLabel.Text = "Starting in 3..2..1..Go!";
                this.Close();
            }
        }
    }
}

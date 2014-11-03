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
    public partial class Waiting_Form : Form
    {
        int timesElapsed = 0;
        System.Timers.Timer t = new System.Timers.Timer(1000);
        public Waiting_Form()
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
                Waiting_Label.Text = "Waiting";
                timesElapsed++;
            }
            else if (timesElapsed == 1)
            {
                Waiting_Label.Text = "Waiting.";
                timesElapsed++;
            }
            else if (timesElapsed == 2)
            {
                Waiting_Label.Text = "Waiting..";
                timesElapsed++;
            }
            else if (timesElapsed == 3)
            {
                Waiting_Label.Text = "Waiting...";
                timesElapsed = 0;
            }
        }
    }
}

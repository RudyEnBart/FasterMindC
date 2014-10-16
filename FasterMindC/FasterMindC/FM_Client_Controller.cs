using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FasterMindC
{
    public class FM_Client_Controller
    {

        private string _name { get; set; }
        private short _code { get; set; }

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
            throw new NotImplementedException();
        }
    }
}

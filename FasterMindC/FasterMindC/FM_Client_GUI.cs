using FMNetworkLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FasterMindC
{
    public partial class FM_Client_GUI : Form
    {
        private Dictionary<ENUMS.color, Color> _colorDict;
        private FM_Client_Controller _controller;
        public FM_Client_GUI(FM_Client_Controller controller)
        {
            this._controller = controller;
            InitializeComponent();
        }

        private void NameButtonClicked(object sender, EventArgs e)
        {
            PlayerName.Text = nameBox.Text;
            _controller.NameButtonClick(sender, e, PlayerName.Text);
        }

        private void SubmitButtonClicked(object sender, EventArgs e)
        {
            _controller.SubmitButtonClicked(sender, e);
        }

        private void InputCode1Clicked(object sender, EventArgs e)
        {
            _controller.InputCodeClicked(sender, e, 1);
            _inputCode1.BackColor = ENUMS.color.BLUE;
        }

        private void InputCode2Clicked(object sender, EventArgs e)
        {
            _controller.InputCodeClicked(sender, e, 2);
        }

        private void InputCode3Clicked(object sender, EventArgs e)
        {
            _controller.InputCodeClicked(sender, e, 3);
        }

        private void InputCode4Clicked(object sender, EventArgs e)
        {
            _controller.InputCodeClicked(sender, e, 4);
        }

        private void _code1_4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

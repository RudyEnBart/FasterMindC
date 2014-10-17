using FMNetworkLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        private byte code1 = 6;
        private byte code2 = 6;
        private byte code3 = 6;
        private byte code4 = 6;
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
            code1++;
            if (code1 > 5)
            {
                code1 = 0;
            }
            this._input1.BackColor = ENUMS.GetColor((ENUMS.color)code1);
            _controller.InputCodeClicked(sender, e, 1);
        }

        private void InputCode2Clicked(object sender, EventArgs e)
        {
            code2++;
            if (code2 > 5)
            {
                code2 = 0;
            }
            this._input2.BackColor = ENUMS.GetColor((ENUMS.color)code2);
            _controller.InputCodeClicked(sender, e, 2);
        }

        private void InputCode3Clicked(object sender, EventArgs e)
        {
            code3++;
            if (code3 > 5)
            {
                code3 = 0;
            }
            this._input3.BackColor = ENUMS.GetColor((ENUMS.color)code3);
            _controller.InputCodeClicked(sender, e, 3);
        }

        private void InputCode4Clicked(object sender, EventArgs e)
        {
            code4++;
            if (code4 > 5)
            {
                code4 = 0;
            }
            this._input4.BackColor = ENUMS.GetColor((ENUMS.color)code4);
            _controller.InputCodeClicked(sender, e, 4);
        }

        public void MoveCodeRight()
        {
            Debug.WriteLine("white: " + (byte)ENUMS.color.WHITE);
            this._inputCode1.BackColor = this._input1.BackColor;
            this._inputCode2.BackColor = this._input2.BackColor;
            this._inputCode3.BackColor = this._input3.BackColor;
            this._inputCode4.BackColor = this._input4.BackColor;
            this._input1.BackColor = ENUMS.GetColor(ENUMS.color.WHITE);
            this._input2.BackColor = ENUMS.GetColor(ENUMS.color.WHITE);
            this._input3.BackColor = ENUMS.GetColor(ENUMS.color.WHITE);
            this._input4.BackColor = ENUMS.GetColor(ENUMS.color.WHITE);
        }

        private void _code1_4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

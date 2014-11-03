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
        private FM_Client_Controller _controller;
        private byte code1;
        private byte code2;
        private byte code3;
        private byte code4;
        private List<Panel> _myPanels = new List<Panel>();
        private List<Panel> _myResultPanels = new List<Panel>();
        public FM_Client_GUI(FM_Client_Controller controller)
        {
            this._controller = controller;
            InitializeComponent();
            foreach(Panel p in this.Controls.OfType<Panel>())
            {
                if (p.Name.Contains("_"))
                {
                    if (p.Name.Contains("result"))
                    {
                        _myResultPanels.Add(p);
                    }
                    else
                    {
                        _myPanels.Add(p);
                    }
                }
            }
            init();
        }

        public void init()
        {
            foreach(Panel p in _myPanels)
            {
                p.BackColor = Color.White;
            }
            foreach(Panel p in _myResultPanels)
            {
                p.BackColor = Color.Black;
            }
            ResetCodes();
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

        private void InputCode1Clicked(object sender, MouseEventArgs e)
        {
            code1++;
            if (code1 > 5)
            {
                code1 = 0;
            }
            this._input1.BackColor = ENUMS.GetColor((ENUMS.color)code1);
            _controller.InputCodeClicked(sender, e, 1);
        }

        private void InputCode2Clicked(object sender, MouseEventArgs e)
        {
            code2++;
            if (code2 > 5)
            {
                code2 = 0;
            }
            this._input2.BackColor = ENUMS.GetColor((ENUMS.color)code2);
            _controller.InputCodeClicked(sender, e, 2);
        }

        private void InputCode3Clicked(object sender, MouseEventArgs e)
        {
            code3++;
            if (code3 > 5)
            {
                code3 = 0;
            }
            this._input3.BackColor = ENUMS.GetColor((ENUMS.color)code3);
            _controller.InputCodeClicked(sender, e, 3);
        }

        private void InputCode4Clicked(object sender, MouseEventArgs e)
        {
            code4++;
            if (code4 > 5)
            {
                code4 = 0;
            }
            this._input4.BackColor = ENUMS.GetColor((ENUMS.color)code4);
            _controller.InputCodeClicked(sender, e, 4);
        }

        public void MoveCode(bool first, byte attempt)
        {
            if (first)
            {
                this._inputCode1.BackColor = this._input1.BackColor;
                this._inputCode2.BackColor = this._input2.BackColor;
                this._inputCode3.BackColor = this._input3.BackColor;
                this._inputCode4.BackColor = this._input4.BackColor;

                ResetCodes();
            }
            else 
            {
                this._myPanels[8 + (attempt * 4) + 0].BackColor = this._input1.BackColor;
                this._myPanels[8 + (attempt * 4) + 1].BackColor = this._input2.BackColor;
                this._myPanels[8 + (attempt * 4) + 2].BackColor = this._input3.BackColor;
                this._myPanels[8 + (attempt * 4) + 3].BackColor = this._input4.BackColor;

                ResetCodes();

                /*switch(attempt)
                {
                    case 1:
                        this._code1_1.BackColor = this._input1.BackColor;
                        this._code1_2.BackColor = this._input2.BackColor;
                        this._code1_3.BackColor = this._input3.BackColor;
                        this._code1_4.BackColor = this._input4.BackColor;
                        break;
                    default:
                        break;

                }*/
            }
            this._input1.BackColor = ENUMS.GetColor(ENUMS.color.WHITE);
            this._input2.BackColor = ENUMS.GetColor(ENUMS.color.WHITE);
            this._input3.BackColor = ENUMS.GetColor(ENUMS.color.WHITE);
            this._input4.BackColor = ENUMS.GetColor(ENUMS.color.WHITE);
        }

        private void ResetCodes()
        {
            Console.WriteLine("Codes reset!");
            code1 = 6;
            code2 = 6;
            code3 = 6;
            code4 = 6;
        }

        private void NameEnterPressed(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                PlayerName.Text = nameBox.Text;
                _controller.NameButtonClick(sender, e, PlayerName.Text);
            }
        }

        internal void SetResultColor(byte _attempt, string result)
        {
            int correct = Int32.Parse(result.Substring(0, 1));
            int halfcorrect = Int32.Parse(result.Substring(1, 1));
            int index = 0;
            for (int i = 0; i < correct; i++) 
            {
                this._myResultPanels[((_attempt - 1) * 4) + index].BackColor = Color.Red;
                index++;
            }
            for (int i = 0; i < halfcorrect; i++)
            {
                this._myResultPanels[((_attempt - 1) * 4) + index].BackColor = Color.White;
                index++;
            }
        }

        delegate void ChangeOpponentNameDel(string name);

        public void ChangeOpponentName(string name)
        {
            if(this.InvokeRequired)
            {
                ChangeOpponentNameDel d = new ChangeOpponentNameDel(ChangeOpponentName);
                this.Invoke(d, new object[] { name });
            }
            else
            {
                OpponentName.Text = name;
            }
        }

        public void SetOpponentTry(int attempt, string code)
        {
            this._myPanels[8 + 36 + (attempt * 4) + 3].BackColor = ENUMS.GetColor((ENUMS.color)Int32.Parse(code.Substring(0, 1)) - 1);
            this._myPanels[8 + 36 + (attempt * 4) + 2].BackColor = ENUMS.GetColor((ENUMS.color)Int32.Parse(code.Substring(1, 1)) - 1);
            this._myPanels[8 + 36 + (attempt * 4) + 1].BackColor = ENUMS.GetColor((ENUMS.color)Int32.Parse(code.Substring(2, 1)) - 1);
            this._myPanels[8 + 36 + (attempt * 4) + 0].BackColor = ENUMS.GetColor((ENUMS.color)Int32.Parse(code.Substring(3, 1)) - 1);
        }

        private void GUI_Closed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void highscoreButton_MouseClick(object sender, MouseEventArgs e)
        {
            _controller.GetHighscores();
        }
    }
}

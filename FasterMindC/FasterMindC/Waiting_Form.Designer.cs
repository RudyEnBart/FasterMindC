namespace FasterMindC
{
    partial class Waiting_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Waiting_Label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Waiting_Label
            // 
            this.Waiting_Label.AutoSize = true;
            this.Waiting_Label.Location = new System.Drawing.Point(12, 9);
            this.Waiting_Label.Name = "Waiting_Label";
            this.Waiting_Label.Size = new System.Drawing.Size(43, 13);
            this.Waiting_Label.TabIndex = 0;
            this.Waiting_Label.Text = "Waiting";
            // 
            // Waiting_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(156, 31);
            this.ControlBox = false;
            this.Controls.Add(this.Waiting_Label);
            this.Name = "Waiting_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Waiting for opponent.";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Waiting_Label;
    }
}
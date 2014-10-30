namespace FasterMindC
{
    partial class Connection_Form
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
            this.CON_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // CON_label
            // 
            this.CON_label.AutoSize = true;
            this.CON_label.Location = new System.Drawing.Point(12, 9);
            this.CON_label.Name = "CON_label";
            this.CON_label.Size = new System.Drawing.Size(114, 13);
            this.CON_label.TabIndex = 0;
            this.CON_label.Text = "Waiting for connection";
            // 
            // Connection_Form
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(156, 31);
            this.ControlBox = false;
            this.Controls.Add(this.CON_label);
            this.Name = "Connection_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Connecting...";
            this.Load += new System.EventHandler(this.Connection_Form_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label CON_label;
    }
}
namespace TheTechIdea.Beep.Winform.Controls.Template
{
    partial class BeepDialog
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
            BottomPanel = new Panel();
            Okbutton = new Button();
            Cancelbutton = new Button();
            Toppanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)IconpictureBox).BeginInit();
            BottomPanel.SuspendLayout();
            SuspendLayout();
            // 
            // BottomLine
            // 
            BottomLine.Location = new Point(0, 308);
            BottomLine.Size = new Size(458, 5);
            // 
            // RightLine
            // 
            RightLine.Location = new Point(453, 0);
            RightLine.Size = new Size(5, 308);
            // 
            // LeftLine
            // 
            LeftLine.Size = new Size(5, 308);
            // 
            // TopLine
            // 
            TopLine.Size = new Size(448, 5);
            // 
            // Toppanel
            // 
            Toppanel.BackColor = Color.White;
            Toppanel.BorderStyle = BorderStyle.FixedSingle;
            Toppanel.Size = new Size(448, 34);
            // 
            // Titlelabel
            // 
            Titlelabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Titlelabel.ForeColor = Color.Black;
            Titlelabel.Location = new Point(34, 6);
            Titlelabel.Size = new Size(87, 21);
            // 
            // Maxbutton
            // 
            Maxbutton.FlatAppearance.BorderSize = 0;
            Maxbutton.Location = new Point(-343, 6);
            // 
            // IconpictureBox
            // 
            IconpictureBox.BackColor = Color.Transparent;
            // 
            // Closebutton
            // 
            Closebutton.FlatAppearance.BorderSize = 0;
            Closebutton.Location = new Point(-317, 6);
            // 
            // Minbutton
            // 
            Minbutton.FlatAppearance.BorderSize = 0;
            Minbutton.Location = new Point(-369, 6);
            // 
            // Insidepanel
            // 
            Insidepanel.Size = new Size(452, 217);
            // 
            // BottomPanel
            // 
            BottomPanel.BackColor = Color.WhiteSmoke;
            BottomPanel.BorderStyle = BorderStyle.FixedSingle;
            BottomPanel.Controls.Add(Okbutton);
            BottomPanel.Controls.Add(Cancelbutton);
            BottomPanel.Dock = DockStyle.Bottom;
            BottomPanel.Location = new Point(5, 250);
            BottomPanel.Name = "BottomPanel";
            BottomPanel.Size = new Size(448, 58);
            BottomPanel.TabIndex = 27;
            // 
            // Okbutton
            // 
            Okbutton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Okbutton.BackColor = Color.Black;
            Okbutton.BackgroundImageLayout = ImageLayout.Zoom;
            Okbutton.DialogResult = DialogResult.OK;
            Okbutton.FlatAppearance.BorderSize = 0;
            Okbutton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 192);
            Okbutton.FlatAppearance.MouseOverBackColor = Color.Silver;
            Okbutton.FlatStyle = FlatStyle.Flat;
            Okbutton.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold);
            Okbutton.ForeColor = Color.White;
            Okbutton.Location = new Point(360, 8);
            Okbutton.Name = "Okbutton";
            Okbutton.Size = new Size(75, 40);
            Okbutton.TabIndex = 1;
            Okbutton.Text = "OK";
            Okbutton.UseVisualStyleBackColor = false;
            // 
            // Cancelbutton
            // 
            Cancelbutton.BackColor = Color.Black;
            Cancelbutton.BackgroundImageLayout = ImageLayout.Zoom;
            Cancelbutton.DialogResult = DialogResult.Cancel;
            Cancelbutton.FlatAppearance.BorderSize = 0;
            Cancelbutton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 192);
            Cancelbutton.FlatAppearance.MouseOverBackColor = Color.Silver;
            Cancelbutton.FlatStyle = FlatStyle.Flat;
            Cancelbutton.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold);
            Cancelbutton.ForeColor = Color.White;
            Cancelbutton.Location = new Point(5, 8);
            Cancelbutton.Name = "Cancelbutton";
            Cancelbutton.Size = new Size(75, 40);
            Cancelbutton.TabIndex = 0;
            Cancelbutton.Text = "Cancel";
            Cancelbutton.UseVisualStyleBackColor = false;
            // 
            // BeepDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = Cancelbutton;
            ClientSize = new Size(458, 313);
            Controls.Add(BottomPanel);
            Name = "BeepDialog";
            Text = "BeepDialog";
            Title = "BeepDialog";
            Controls.SetChildIndex(BottomLine, 0);
            Controls.SetChildIndex(RightLine, 0);
            Controls.SetChildIndex(LeftLine, 0);
            Controls.SetChildIndex(TopLine, 0);
            Controls.SetChildIndex(Toppanel, 0);
            Controls.SetChildIndex(BottomPanel, 0);
            Controls.SetChildIndex(Insidepanel, 0);
            Toppanel.ResumeLayout(false);
            Toppanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)IconpictureBox).EndInit();
            BottomPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel BottomPanel;
        private Button Okbutton;
        private Button Cancelbutton;
    }
}
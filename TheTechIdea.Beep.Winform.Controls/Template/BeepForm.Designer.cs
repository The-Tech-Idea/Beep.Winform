namespace TheTechIdea.Beep.Winform.Controls.Template
{
    partial class BeepForm
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
            BottomLine = new Panel();
            RightLine = new Panel();
            LeftLine = new Panel();
            TopLine = new Panel();
            Toppanel = new Panel();
            Titlelabel = new Label();
            Maxbutton = new Button();
            IconpictureBox = new PictureBox();
            Closebutton = new Button();
            Minbutton = new Button();
            Insidepanel = new Panel();
            Toppanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)IconpictureBox).BeginInit();
            SuspendLayout();
            // 
            // BottomLine
            // 
            BottomLine.BackColor = Color.Black;
            BottomLine.Dock = DockStyle.Bottom;
            BottomLine.Location = new Point(0, 700);
            BottomLine.Name = "BottomLine";
            BottomLine.Size = new Size(1017, 2);
            BottomLine.TabIndex = 21;
            // 
            // RightLine
            // 
            RightLine.BackColor = Color.Black;
            RightLine.Dock = DockStyle.Right;
            RightLine.Location = new Point(1015, 0);
            RightLine.Name = "RightLine";
            RightLine.Size = new Size(2, 700);
            RightLine.TabIndex = 22;
            // 
            // LeftLine
            // 
            LeftLine.BackColor = Color.Black;
            LeftLine.Dock = DockStyle.Left;
            LeftLine.Location = new Point(0, 0);
            LeftLine.Name = "LeftLine";
            LeftLine.Size = new Size(2, 700);
            LeftLine.TabIndex = 23;
            // 
            // TopLine
            // 
            TopLine.BackColor = Color.Black;
            TopLine.Dock = DockStyle.Top;
            TopLine.Location = new Point(2, 0);
            TopLine.Name = "TopLine";
            TopLine.Size = new Size(1013, 2);
            TopLine.TabIndex = 24;
            // 
            // Toppanel
            // 
            Toppanel.Controls.Add(Titlelabel);
            Toppanel.Controls.Add(Maxbutton);
            Toppanel.Controls.Add(IconpictureBox);
            Toppanel.Controls.Add(Closebutton);
            Toppanel.Controls.Add(Minbutton);
            Toppanel.Dock = DockStyle.Top;
            Toppanel.Location = new Point(2, 2);
            Toppanel.Name = "Toppanel";
            Toppanel.Size = new Size(1013, 34);
            Toppanel.TabIndex = 25;
            // 
            // Titlelabel
            // 
            Titlelabel.AutoSize = true;
            Titlelabel.FlatStyle = FlatStyle.Flat;
            Titlelabel.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Titlelabel.Location = new Point(45, 7);
            Titlelabel.Name = "Titlelabel";
            Titlelabel.Size = new Size(68, 17);
            Titlelabel.TabIndex = 18;
            Titlelabel.Text = "Form Title";
            Titlelabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Maxbutton
            // 
            Maxbutton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Maxbutton.BackgroundImage = Properties.Resources.maximize;
            Maxbutton.BackgroundImageLayout = ImageLayout.Zoom;
            Maxbutton.FlatAppearance.BorderSize = 0;
            Maxbutton.FlatStyle = FlatStyle.Flat;
            Maxbutton.Location = new Point(957, 7);
            Maxbutton.Name = "Maxbutton";
            Maxbutton.Size = new Size(20, 20);
            Maxbutton.TabIndex = 16;
            Maxbutton.UseVisualStyleBackColor = true;
            // 
            // IconpictureBox
            // 
            IconpictureBox.BackColor = Color.White;
            IconpictureBox.Location = new Point(8, 7);
            IconpictureBox.Name = "IconpictureBox";
            IconpictureBox.Padding = new Padding(3);
            IconpictureBox.Size = new Size(20, 20);
            IconpictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            IconpictureBox.TabIndex = 19;
            IconpictureBox.TabStop = false;
            // 
            // Closebutton
            // 
            Closebutton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Closebutton.BackgroundImage = Properties.Resources.close1;
            Closebutton.BackgroundImageLayout = ImageLayout.Zoom;
            Closebutton.FlatAppearance.BorderSize = 0;
            Closebutton.FlatStyle = FlatStyle.Flat;
            Closebutton.Location = new Point(983, 7);
            Closebutton.Name = "Closebutton";
            Closebutton.Size = new Size(20, 20);
            Closebutton.TabIndex = 13;
            Closebutton.UseVisualStyleBackColor = true;
            // 
            // Minbutton
            // 
            Minbutton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Minbutton.BackgroundImage = Properties.Resources.minimize;
            Minbutton.BackgroundImageLayout = ImageLayout.Zoom;
            Minbutton.FlatAppearance.BorderSize = 0;
            Minbutton.FlatStyle = FlatStyle.Flat;
            Minbutton.Location = new Point(931, 7);
            Minbutton.Name = "Minbutton";
            Minbutton.Size = new Size(20, 20);
            Minbutton.TabIndex = 17;
            Minbutton.UseVisualStyleBackColor = true;
            // 
            // Insidepanel
            // 
            Insidepanel.AllowDrop = true;
            Insidepanel.Dock = DockStyle.Fill;
            Insidepanel.Location = new Point(2, 36);
            Insidepanel.Margin = new Padding(4, 3, 4, 3);
            Insidepanel.Name = "Insidepanel";
            Insidepanel.Padding = new Padding(2);
            Insidepanel.Size = new Size(1013, 664);
            Insidepanel.TabIndex = 26;
            // 
            // BeepForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1017, 702);
            Controls.Add(Insidepanel);
            Controls.Add(Toppanel);
            Controls.Add(TopLine);
            Controls.Add(LeftLine);
            Controls.Add(RightLine);
            Controls.Add(BottomLine);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 3, 4, 3);
            MaximumSize = new Size(4011, 1600);
            MinimumSize = new Size(219, 40);
            Name = "BeepForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "";
            TransparencyKey = Color.Fuchsia;
            Toppanel.ResumeLayout(false);
            Toppanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)IconpictureBox).EndInit();
            ResumeLayout(false);
        }

        #endregion
        public Panel BottomLine;
        public Panel RightLine;
        public Panel LeftLine;
        public Panel TopLine;
        public Panel Toppanel;
        public Label Titlelabel;
        public Button Maxbutton;
        public PictureBox IconpictureBox;
        public Button Closebutton;
        public Button Minbutton;
        public Panel Insidepanel;
    }
}
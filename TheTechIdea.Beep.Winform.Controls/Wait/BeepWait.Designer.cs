namespace TheTechIdea.Beep.Winform.Controls.Wait
{
    partial class BeepWait
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
            pictureBox1 = new PictureBox();
            messege = new TextBox();
            Title = new Label();
            panel2 = new Panel();
            LogopictureBox = new PictureBox();
            label2 = new Label();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)LogopictureBox).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.Image = Properties.Resources.please_wait;
            pictureBox1.Location = new Point(-1, 67);
            pictureBox1.Margin = new Padding(4, 3, 4, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(468, 111);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.UseWaitCursor = true;
            // 
            // messege
            // 
            messege.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            messege.BackColor = Color.White;
            messege.BorderStyle = BorderStyle.None;
            messege.CausesValidation = false;
            messege.Enabled = false;
            messege.Location = new Point(0, 184);
            messege.Margin = new Padding(4, 3, 4, 3);
            messege.Multiline = true;
            messege.Name = "messege";
            messege.ReadOnly = true;
            messege.Size = new Size(466, 135);
            messege.TabIndex = 1;
            messege.TextAlign = HorizontalAlignment.Center;
            messege.UseWaitCursor = true;
            // 
            // Title
            // 
            Title.BackColor = Color.Transparent;
            Title.Dock = DockStyle.Top;
            Title.Font = new Font("Rockwell", 18F, FontStyle.Bold, GraphicsUnit.Point);
            Title.ForeColor = Color.Black;
            Title.Location = new Point(0, 0);
            Title.Margin = new Padding(4, 0, 4, 0);
            Title.Name = "Title";
            Title.Size = new Size(467, 64);
            Title.TabIndex = 1;
            Title.Text = "Beep Platform";
            Title.TextAlign = ContentAlignment.MiddleCenter;
            Title.UseWaitCursor = true;
            // 
            // panel2
            // 
            panel2.BackColor = Color.White;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(LogopictureBox);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(Title);
            panel2.Controls.Add(pictureBox1);
            panel2.Controls.Add(messege);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 0);
            panel2.Margin = new Padding(4, 3, 4, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(469, 354);
            panel2.TabIndex = 3;
            panel2.UseWaitCursor = true;
            // 
            // LogopictureBox
            // 
            LogopictureBox.Anchor = AnchorStyles.Bottom;
            LogopictureBox.Image = Properties.Resources.SimpleInfoApps1;
            LogopictureBox.Location = new Point(4, 325);
            LogopictureBox.Margin = new Padding(4, 3, 4, 3);
            LogopictureBox.Name = "LogopictureBox";
            LogopictureBox.Size = new Size(27, 24);
            LogopictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            LogopictureBox.TabIndex = 2;
            LogopictureBox.TabStop = false;
            LogopictureBox.UseWaitCursor = true;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom;
            label2.AutoSize = true;
            label2.Location = new Point(29, 330);
            label2.Name = "label2";
            label2.Size = new Size(78, 15);
            label2.TabIndex = 4;
            label2.Text = "The Tech Idea";
            label2.UseWaitCursor = true;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom;
            label1.AutoSize = true;
            label1.Location = new Point(366, 329);
            label1.Name = "label1";
            label1.Size = new Size(98, 15);
            label1.TabIndex = 3;
            label1.Text = "Powered by Beep";
            label1.UseWaitCursor = true;
            // 
            // BeepWait
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(469, 354);
            Controls.Add(panel2);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 3, 4, 3);
            Name = "BeepWait";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "BeepWait";
            TopMost = true;
            UseWaitCursor = true;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)LogopictureBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        public PictureBox pictureBox1;
        public TextBox messege;
        public Label Title;
        public Panel panel2;
        public PictureBox LogopictureBox;
        public Label label1;
        public Label label2;
    }
}
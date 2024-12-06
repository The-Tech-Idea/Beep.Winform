namespace TheTechIdea.Beep.Winform.Controls.Design.UIEditor
{
    partial class BeepImageSelectorDialog
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
            panel1 = new Panel();
            Viewbutton = new Button();
            label2 = new Label();
            ImagelistBox = new ListBox();
            panel2 = new Panel();
            label1 = new Label();
            ImportLocalResourcesbutton = new Button();
            Findbutton = new Button();
            panel3 = new Panel();
            textBox1 = new TextBox();
            PreviewpictureBox = new PictureBox();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PreviewpictureBox).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(Viewbutton);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(ImagelistBox);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(200, 450);
            panel1.TabIndex = 0;
            // 
            // Viewbutton
            // 
            Viewbutton.Location = new Point(62, 415);
            Viewbutton.Name = "Viewbutton";
            Viewbutton.Size = new Size(73, 23);
            Viewbutton.TabIndex = 12;
            Viewbutton.Text = "View";
            Viewbutton.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(47, 17);
            label2.Name = "label2";
            label2.Size = new Size(105, 15);
            label2.TabIndex = 12;
            label2.Text = "Embedded Images";
            // 
            // ImagelistBox
            // 
            ImagelistBox.FormattingEnabled = true;
            ImagelistBox.ItemHeight = 15;
            ImagelistBox.Location = new Point(6, 44);
            ImagelistBox.Name = "ImagelistBox";
            ImagelistBox.Size = new Size(191, 364);
            ImagelistBox.TabIndex = 6;
            // 
            // panel2
            // 
            panel2.Controls.Add(label1);
            panel2.Controls.Add(ImportLocalResourcesbutton);
            panel2.Controls.Add(Findbutton);
            panel2.Controls.Add(panel3);
            panel2.Controls.Add(textBox1);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(200, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(600, 450);
            panel2.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 13);
            label1.Name = "label1";
            label1.Size = new Size(67, 15);
            label1.TabIndex = 11;
            label1.Text = "Image Path";
            // 
            // ImportLocalResourcesbutton
            // 
            ImportLocalResourcesbutton.Location = new Point(17, 412);
            ImportLocalResourcesbutton.Name = "ImportLocalResourcesbutton";
            ImportLocalResourcesbutton.Size = new Size(73, 23);
            ImportLocalResourcesbutton.TabIndex = 8;
            ImportLocalResourcesbutton.Text = "Import";
            ImportLocalResourcesbutton.UseVisualStyleBackColor = true;
            // 
            // Findbutton
            // 
            Findbutton.Location = new Point(516, 9);
            Findbutton.Name = "Findbutton";
            Findbutton.Size = new Size(72, 23);
            Findbutton.TabIndex = 10;
            Findbutton.Text = "Find";
            Findbutton.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            panel3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel3.Controls.Add(PreviewpictureBox);
            panel3.Location = new Point(17, 38);
            panel3.Name = "panel3";
            panel3.Size = new Size(571, 368);
            panel3.TabIndex = 1;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(79, 9);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(431, 23);
            textBox1.TabIndex = 0;
            // 
            // PreviewpictureBox
            // 
            PreviewpictureBox.Dock = DockStyle.Fill;
            PreviewpictureBox.Location = new Point(0, 0);
            PreviewpictureBox.Name = "PreviewpictureBox";
            PreviewpictureBox.Size = new Size(571, 368);
            PreviewpictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            PreviewpictureBox.TabIndex = 0;
            PreviewpictureBox.TabStop = false;
            // 
            // BeepImageSelectorDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panel2);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "BeepImageSelectorDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Beep Image Selector Dialog";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PreviewpictureBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private ListBox ImagelistBox;
        private Button ImportLocalResourcesbutton;
        private Label label1;
        private Button Findbutton;
        private Panel panel3;
        private TextBox textBox1;
        private Button Viewbutton;
        private Label label2;
        private PictureBox PreviewpictureBox;
    }
}
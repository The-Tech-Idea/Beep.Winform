using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;

namespace TheTechIdea.Beep.Winform.Default.Views
{
    partial class Form1
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
            SuspendLayout();
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 255, 255);
            ClientSize = new Size(1817, 1770);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Location = new Point(0, 0);
            Margin = new Padding(11, 13, 11, 13);
            Name = "Form1";
            Padding = new Padding(19, 21, 19, 21);
            PaintOverContentArea = true;
            ShowModernRenderingInDesignMode = true;
            ShowProfileButton = true;
            ShowSearchBox = true;
            ShowStyleButton = true;
            ShowThemeButton = true;
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion
    }
}
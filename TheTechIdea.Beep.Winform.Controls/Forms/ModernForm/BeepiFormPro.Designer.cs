namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    partial class BeepiFormPro
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
            _bgBrush?.Dispose(); // Clean up cached brush
            _designModeInvalidateTimer?.Dispose(); // Clean up design-mode timer
         
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
            // BeepiFormPro
            // 
            AutoValidate = AutoValidate.Disable;
            ClientSize = new Size(800, 450);
            Name = "BeepiFormPro";
            Text = "BeepiFormPro";
            ResumeLayout(false);
        }

        #endregion
    }
}
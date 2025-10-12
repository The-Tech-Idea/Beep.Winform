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
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            // CRITICAL FIX: Don't set AutoScaleMode here!
            // Let child forms (like BeepWait) set their own AutoScaleMode.Font
            // Base form should not interfere with child form scaling.
            // Removing AutoScaleMode setting allows proper Font-based scaling in derived forms.
            
            // DO NOT SET: this.AutoScaleMode = ...
            // DO NOT SET: this.AutoScaleDimensions = ...
            
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "BeepiFormPro";
        }

        #endregion
    }
}
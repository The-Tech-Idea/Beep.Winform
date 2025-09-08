namespace TheTechIdea.Beep.Winform.Default.Views
{
    partial class Form2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            SuspendLayout();
            // 
            // beepuiManager1
            // 
            beepuiManager1.BeepiForm = this;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(200, 200, 200);
            BorderRadius = 8;
            ClientSize = new Size(836, 656);
            Name = "Form2";
            StylePresets.Presets = (Dictionary<string, Controls.BeepFormStyleMetrics>)resources.GetObject("Form2.StylePresets.Presets");
            Text = "Form2";
            Theme = "DefaultTheme";
            ResumeLayout(false);
        }

        #endregion
    }
}
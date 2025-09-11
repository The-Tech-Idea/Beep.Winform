namespace TheTechIdea.Beep.Winform.Default.Views.Template
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            SuspendLayout();
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(200, 200, 200);
            BorderRadius = 4;
            ClientSize = new Size(859, 794);
            EnableGlow = false;
            FormStyle = Winform.Controls.BeepFormStyle.Office;
            GlowColor = Color.FromArgb(90, 50, 100, 200);
            GlowSpread = 0F;
            Name = "Form1";
            ShadowDepth = 4;
            StylePresets.Presets = (Dictionary<string, Controls.BeepFormStyleMetrics>)resources.GetObject("Form1.StylePresets.Presets");
            Text = "Form1";
            Theme = "DefaultTheme";
            ResumeLayout(false);
        }

        #endregion

        private Controls.MDI.BeepMdiManager beepMdiManager1;
    }
}
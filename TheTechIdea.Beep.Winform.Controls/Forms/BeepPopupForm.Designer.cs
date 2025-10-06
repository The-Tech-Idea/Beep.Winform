namespace TheTechIdea.Beep.Winform.Controls
{
    partial class BeepPopupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BeepPopupForm));
            SuspendLayout();
            // 
            // BeepPopupForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(200, 200, 200);
            BorderRadius = 20;
            BorderThickness = 3;
            ClientSize = new Size(800, 450);
            Name = "BeepPopupForm";
            ShowCaptionBar = false;
            ShowIcon = false;
            ShowInTaskbar = false;
          
            ShowSnapHints = false;
            ShowSystemButtons = false;
            StylePresets.Presets = (Dictionary<string, BeepFormStyleMetrics>)resources.GetObject("BeepPopupForm.StylePresets.Presets");
            Text = "BeepPopupForm";
            Theme = "DefaultType";
            ResumeLayout(false);
        }

        #endregion
    }
}
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
            // beepuiManager1
            // 
            beepuiManager1.BeepiForm = this;
            // 
            // BeepPopupForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(200, 200, 200);
            ClientSize = new Size(800, 450);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Name = "BeepPopupForm";
            ShowCaptionBar = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            ShowQuickAccess = false;
            ShowSnapHints = false;
            ShowSystemButtons = false;
            StylePresets.Presets = (Dictionary<string, BeepFormStyleMetrics>)resources.GetObject("BeepPopupForm.StylePresets.Presets");
            Text = "BeepPopupForm";
            Theme = "DefaultTheme";
            ResumeLayout(false);
        }

        #endregion
    }
}
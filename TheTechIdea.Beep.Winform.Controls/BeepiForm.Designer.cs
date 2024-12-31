using TheTechIdea.Beep.Winform.Controls.Managers;

namespace TheTechIdea.Beep.Winform.Controls
{
    partial class BeepiForm
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
            components = new System.ComponentModel.Container();
            beepuiManager1 = new BeepUIManager(components);
            SuspendLayout();
            // 
            // beepuiManager1
            // 
            beepuiManager1.ApplyThemeOnImage = false;
            beepuiManager1.BeepFunctionsPanel = null;
            beepuiManager1.BeepiForm = null;
            beepuiManager1.IsRounded = true;
            beepuiManager1.LogoImage = "";
            beepuiManager1.ShowBorder = true;
            beepuiManager1.ShowShadow = false;
            beepuiManager1.Theme = Vis.Modules.EnumBeepThemes.FlatDesignTheme;
            beepuiManager1.Title = "Beep Form";
            beepuiManager1.ViewRouter = null;
            // 
            // BeepiForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(774, 644);
            FormBorderStyle = FormBorderStyle.None;
            Name = "BeepiForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Beep i Form";
            ResumeLayout(false);
        }

        #endregion
        public BeepUIManager beepuiManager1;
    }
}
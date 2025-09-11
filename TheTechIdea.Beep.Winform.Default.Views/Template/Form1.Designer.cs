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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            beepMdiManager1 = new TheTechIdea.Beep.Winform.Controls.MDI.BeepMdiManager(components);
            SuspendLayout();
            // 
            // beepMdiManager1
            // 
            beepMdiManager1.AllowTabReorder = true;
            beepMdiManager1.EnableMenuMerge = false;
            beepMdiManager1.EnableTabbedMdi = true;
            beepMdiManager1.HideChildCaptions = true;
            beepMdiManager1.HostForm = null;
            beepMdiManager1.MergeTargetMenuStrip = null;
            beepMdiManager1.TabHeight = 30;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Name = "Form1";
            StylePresets.Presets = (Dictionary<string, Controls.BeepFormStyleMetrics>)resources.GetObject("Form1.StylePresets.Presets");
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Controls.MDI.BeepMdiManager beepMdiManager1;
    }
}
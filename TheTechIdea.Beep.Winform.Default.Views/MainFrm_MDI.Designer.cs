namespace TheTechIdea.Beep.Winform.Default.Views
{
    partial class MainFrm_MDI
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
            beepDocumentManager1 = new TheTechIdea.Beep.Winform.Controls.DocumentHost.BeepDocumentManager(components);
            (beepDocumentManager1).BeginInit();
            SuspendLayout();
            // 
            // beepFormuiManager1
            // 
            beepFormuiManager1.ApplyBeepFormStyle = true;
            beepFormuiManager1.ShowCaptionBar = false;
            beepFormuiManager1.UseImmersiveDarkMode = true;
            // 
            // beepDocumentManager1
            // 
            beepDocumentManager1.ContainerType = ContainerTypeEnum.TabbedPanel;
            // 
            // MainFrm_MDI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1279, 537);
            IsMdiContainer = true;
            KeyPreview = true;
            Name = "MainFrm_MDI";
            Text = "MainFrm_MDI";
            (beepDocumentManager1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Controls.DocumentHost.BeepDocumentManager beepDocumentManager1;
       
    }
}
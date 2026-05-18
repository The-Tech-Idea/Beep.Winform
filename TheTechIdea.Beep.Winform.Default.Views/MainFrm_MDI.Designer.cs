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
            beepTabbedView1 = new Controls.DocumentHost.BeepTabbedView(components);
            beepDocumentHost1 = new Controls.DocumentHost.BeepDocumentHost();
            documentPanel1 = new Controls.DocumentHost.BeepDocumentPanel();
            documentPanel2 = new Controls.DocumentHost.BeepDocumentPanel();
            documentPanel3 = new Controls.DocumentHost.BeepDocumentPanel();
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
            beepDocumentManager1.View = beepTabbedView1;
            //
            // beepTabbedView1
            //
            beepTabbedView1.Host = beepDocumentHost1;
            //
            // beepDocumentHost1
            //
            beepDocumentHost1.Dock = DockStyle.Fill;
            beepDocumentHost1.Name = "beepDocumentHost1";
            beepDocumentHost1.TabIndex = 0;
            //
            // documentPanel1
            //
            documentPanel1.DocumentId = "875973f366694501bf75453d2c3f9e73";
            documentPanel1.DocumentTitle = "Document 1";
            documentPanel1.Name = "documentPanel1";
            //
            // documentPanel2
            //
            documentPanel2.DocumentId = "62e27fe712af475ba3da450c281570f7";
            documentPanel2.DocumentTitle = "Document 2";
            documentPanel2.Name = "documentPanel2";
            //
            // documentPanel3
            //
            documentPanel3.DocumentId = "b0ff3b5b52fa45158c3a65c5f07fb2ba";
            documentPanel3.DocumentTitle = "Document 3";
            documentPanel3.Name = "documentPanel3";
            beepDocumentHost1.DocumentPanels.Add(documentPanel1);
            beepDocumentHost1.DocumentPanels.Add(documentPanel2);
            beepDocumentHost1.DocumentPanels.Add(documentPanel3);
            // 
            // MainFrm_MDI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1279, 537);
            Controls.Add(beepDocumentHost1);
            IsMdiContainer = true;
            KeyPreview = true;
            Name = "MainFrm_MDI";
            Text = "MainFrm_MDI";
            (beepDocumentManager1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Controls.DocumentHost.BeepDocumentManager beepDocumentManager1;
        private Controls.DocumentHost.BeepTabbedView beepTabbedView1;
        private Controls.DocumentHost.BeepDocumentHost beepDocumentHost1;
        private Controls.DocumentHost.BeepDocumentPanel documentPanel1;
        private Controls.DocumentHost.BeepDocumentPanel documentPanel2;
        private Controls.DocumentHost.BeepDocumentPanel documentPanel3;
    }
}

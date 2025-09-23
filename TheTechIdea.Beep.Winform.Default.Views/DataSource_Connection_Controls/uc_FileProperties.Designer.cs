namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    partial class uc_FileProperties
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.File_propertiesPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            this.File_DelimiterbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.File_ExtbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.File_FileNamebeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.File_FilePathbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.ConnectionPropertytabPage.SuspendLayout();
            this.File_propertiesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConnectionPropertytabPage
            // 
            this.ConnectionPropertytabPage.Controls.Add(this.File_propertiesPanel);
            // 
            // File_propertiesPanel
            // 
            this.File_propertiesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.File_propertiesPanel.Name = "File_propertiesPanel";
            this.File_propertiesPanel.TabIndex = 0;
            this.File_propertiesPanel.Controls.Add(this.File_DelimiterbeepTextBox);
            this.File_propertiesPanel.Controls.Add(this.File_ExtbeepTextBox);
            this.File_propertiesPanel.Controls.Add(this.File_FileNamebeepTextBox);
            this.File_propertiesPanel.Controls.Add(this.File_FilePathbeepTextBox);
            // 
            // File_FilePathbeepTextBox
            // 
            this.File_FilePathbeepTextBox.Name = "File_FilePathbeepTextBox";
            this.File_FilePathbeepTextBox.PlaceholderText = "File Path";
            this.File_FilePathbeepTextBox.Location = new System.Drawing.Point(24, 24);
            this.File_FilePathbeepTextBox.Size = new System.Drawing.Size(496, 40);
            // 
            // File_FileNamebeepTextBox
            // 
            this.File_FileNamebeepTextBox.Name = "File_FileNamebeepTextBox";
            this.File_FileNamebeepTextBox.PlaceholderText = "File Name";
            this.File_FileNamebeepTextBox.Location = new System.Drawing.Point(24, 80);
            this.File_FileNamebeepTextBox.Size = new System.Drawing.Size(320, 40);
            // 
            // File_ExtbeepTextBox
            // 
            this.File_ExtbeepTextBox.Name = "File_ExtbeepTextBox";
            this.File_ExtbeepTextBox.PlaceholderText = "Extension";
            this.File_ExtbeepTextBox.Location = new System.Drawing.Point(360, 80);
            this.File_ExtbeepTextBox.Size = new System.Drawing.Size(160, 40);
            // 
            // File_DelimiterbeepTextBox
            // 
            this.File_DelimiterbeepTextBox.Name = "File_DelimiterbeepTextBox";
            this.File_DelimiterbeepTextBox.PlaceholderText = "Delimiter";
            this.File_DelimiterbeepTextBox.Location = new System.Drawing.Point(24, 136);
            this.File_DelimiterbeepTextBox.Size = new System.Drawing.Size(80, 40);
            // 
            // uc_FileProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "uc_FileProperties";
            this.Size = new System.Drawing.Size(547, 669);
            this.ConnectionPropertytabPage.ResumeLayout(false);
            this.File_propertiesPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private TheTechIdea.Beep.Winform.Controls.BeepPanel File_propertiesPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox File_FilePathbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox File_FileNamebeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox File_ExtbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox File_DelimiterbeepTextBox;
    }
}

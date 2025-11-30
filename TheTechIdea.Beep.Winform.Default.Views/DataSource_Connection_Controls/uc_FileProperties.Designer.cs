namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    partial class uc_FileProperties
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            base.InitializeComponent();
            this.File_propertiesPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            this.File_FilePathbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.File_FileNamebeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.File_ExtbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.File_DelimiterbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.File_BrowsebeepButton = new TheTechIdea.Beep.Winform.Controls.BeepButton();
            this.File_propertiesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // File_propertiesPanel
            // 
            this.File_propertiesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.File_propertiesPanel.Name = "File_propertiesPanel";
            this.File_propertiesPanel.TabIndex = 0;
            this.File_propertiesPanel.IsChild = true;
            this.File_propertiesPanel.Controls.Add(this.File_FilePathbeepTextBox);
            this.File_propertiesPanel.Controls.Add(this.File_BrowsebeepButton);
            this.File_propertiesPanel.Controls.Add(this.File_FileNamebeepTextBox);
            this.File_propertiesPanel.Controls.Add(this.File_ExtbeepTextBox);
            this.File_propertiesPanel.Controls.Add(this.File_DelimiterbeepTextBox);
            // 
            // File_FilePathbeepTextBox
            // 
            this.File_FilePathbeepTextBox.Name = "File_FilePathbeepTextBox";
            this.File_FilePathbeepTextBox.LabelText = "File Path";
            this.File_FilePathbeepTextBox.LabelTextOn = true;
            this.File_FilePathbeepTextBox.Location = new System.Drawing.Point(20, 20);
            this.File_FilePathbeepTextBox.Size = new System.Drawing.Size(400, 50);
            this.File_FilePathbeepTextBox.IsChild = true;
            // 
            // File_BrowsebeepButton
            // 
            this.File_BrowsebeepButton.Name = "File_BrowsebeepButton";
            this.File_BrowsebeepButton.Text = "...";
            this.File_BrowsebeepButton.Location = new System.Drawing.Point(430, 20);
            this.File_BrowsebeepButton.Size = new System.Drawing.Size(60, 50);
            this.File_BrowsebeepButton.IsChild = true;
            // 
            // File_FileNamebeepTextBox
            // 
            this.File_FileNamebeepTextBox.Name = "File_FileNamebeepTextBox";
            this.File_FileNamebeepTextBox.LabelText = "File Name";
            this.File_FileNamebeepTextBox.LabelTextOn = true;
            this.File_FileNamebeepTextBox.Location = new System.Drawing.Point(20, 90);
            this.File_FileNamebeepTextBox.Size = new System.Drawing.Size(350, 50);
            this.File_FileNamebeepTextBox.IsChild = true;
            // 
            // File_ExtbeepTextBox
            // 
            this.File_ExtbeepTextBox.Name = "File_ExtbeepTextBox";
            this.File_ExtbeepTextBox.LabelText = "Extension";
            this.File_ExtbeepTextBox.LabelTextOn = true;
            this.File_ExtbeepTextBox.Location = new System.Drawing.Point(390, 90);
            this.File_ExtbeepTextBox.Size = new System.Drawing.Size(100, 50);
            this.File_ExtbeepTextBox.IsChild = true;
            // 
            // File_DelimiterbeepTextBox
            // 
            this.File_DelimiterbeepTextBox.Name = "File_DelimiterbeepTextBox";
            this.File_DelimiterbeepTextBox.LabelText = "Delimiter";
            this.File_DelimiterbeepTextBox.LabelTextOn = true;
            this.File_DelimiterbeepTextBox.Location = new System.Drawing.Point(20, 160);
            this.File_DelimiterbeepTextBox.Size = new System.Drawing.Size(100, 50);
            this.File_DelimiterbeepTextBox.IsChild = true;
            // 
            // uc_FileProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "uc_FileProperties";
            this.Size = new System.Drawing.Size(550, 280);
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            // Add the panel to this UserControl
            this.Controls.Add(this.File_propertiesPanel);
            this.File_propertiesPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private TheTechIdea.Beep.Winform.Controls.BeepPanel File_propertiesPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox File_FilePathbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox File_FileNamebeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox File_ExtbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox File_DelimiterbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepButton File_BrowsebeepButton;
    }
}

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    partial class uc_DatabaseProperties
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
            this.Database_propertiesPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            this.Database_DatabaseTypebeepComboBox = new TheTechIdea.Beep.Winform.Controls.BeepComboBox();
            this.Database_DatabasebeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Database_SchemaNamebeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Database_OracleSIDorServicebeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Database_propertiesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // Database_propertiesPanel
            // 
            this.Database_propertiesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Database_propertiesPanel.Name = "Database_propertiesPanel";
            this.Database_propertiesPanel.TabIndex = 0;
            this.Database_propertiesPanel.IsChild = true;
            this.Database_propertiesPanel.Controls.Add(this.Database_DatabaseTypebeepComboBox);
            this.Database_propertiesPanel.Controls.Add(this.Database_DatabasebeepTextBox);
            this.Database_propertiesPanel.Controls.Add(this.Database_SchemaNamebeepTextBox);
            this.Database_propertiesPanel.Controls.Add(this.Database_OracleSIDorServicebeepTextBox);
            // 
            // Database_DatabaseTypebeepComboBox
            // 
            this.Database_DatabaseTypebeepComboBox.Name = "Database_DatabaseTypebeepComboBox";
            this.Database_DatabaseTypebeepComboBox.LabelText = "Database Type";
            this.Database_DatabaseTypebeepComboBox.LabelTextOn = true;
            this.Database_DatabaseTypebeepComboBox.Location = new System.Drawing.Point(20, 20);
            this.Database_DatabaseTypebeepComboBox.Size = new System.Drawing.Size(300, 50);
            this.Database_DatabaseTypebeepComboBox.IsChild = true;
            // 
            // Database_DatabasebeepTextBox
            // 
            this.Database_DatabasebeepTextBox.Name = "Database_DatabasebeepTextBox";
            this.Database_DatabasebeepTextBox.LabelText = "Database Name";
            this.Database_DatabasebeepTextBox.LabelTextOn = true;
            this.Database_DatabasebeepTextBox.Location = new System.Drawing.Point(20, 90);
            this.Database_DatabasebeepTextBox.Size = new System.Drawing.Size(490, 50);
            this.Database_DatabasebeepTextBox.IsChild = true;
            // 
            // Database_SchemaNamebeepTextBox
            // 
            this.Database_SchemaNamebeepTextBox.Name = "Database_SchemaNamebeepTextBox";
            this.Database_SchemaNamebeepTextBox.LabelText = "Schema Name";
            this.Database_SchemaNamebeepTextBox.LabelTextOn = true;
            this.Database_SchemaNamebeepTextBox.Location = new System.Drawing.Point(20, 160);
            this.Database_SchemaNamebeepTextBox.Size = new System.Drawing.Size(490, 50);
            this.Database_SchemaNamebeepTextBox.IsChild = true;
            // 
            // Database_OracleSIDorServicebeepTextBox
            // 
            this.Database_OracleSIDorServicebeepTextBox.Name = "Database_OracleSIDorServicebeepTextBox";
            this.Database_OracleSIDorServicebeepTextBox.LabelText = "Oracle SID or Service";
            this.Database_OracleSIDorServicebeepTextBox.LabelTextOn = true;
            this.Database_OracleSIDorServicebeepTextBox.Location = new System.Drawing.Point(20, 230);
            this.Database_OracleSIDorServicebeepTextBox.Size = new System.Drawing.Size(490, 50);
            this.Database_OracleSIDorServicebeepTextBox.IsChild = true;
            // 
            // uc_DatabaseProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "uc_DatabaseProperties";
            this.Size = new System.Drawing.Size(550, 350);
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            // Add the panel to this UserControl
            this.Controls.Add(this.Database_propertiesPanel);
            this.Database_propertiesPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private TheTechIdea.Beep.Winform.Controls.BeepPanel Database_propertiesPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepComboBox Database_DatabaseTypebeepComboBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Database_DatabasebeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Database_SchemaNamebeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Database_OracleSIDorServicebeepTextBox;
    }
}

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    partial class uc_DatabaseProperties
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
            this.Database_propertiesPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            this.Database_OracleSIDorServicebeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Database_SchemaNamebeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Database_DatabasebeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Database_PortbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Database_HostbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.ConnectionPropertytabPage.SuspendLayout();
            this.Database_propertiesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConnectionPropertytabPage
            // 
            this.ConnectionPropertytabPage.Controls.Add(this.Database_propertiesPanel);
            // 
            // Database_propertiesPanel
            // 
            this.Database_propertiesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Database_propertiesPanel.Name = "Database_propertiesPanel";
            this.Database_propertiesPanel.TabIndex = 0;
            this.Database_propertiesPanel.Controls.Add(this.Database_OracleSIDorServicebeepTextBox);
            this.Database_propertiesPanel.Controls.Add(this.Database_SchemaNamebeepTextBox);
            this.Database_propertiesPanel.Controls.Add(this.Database_DatabasebeepTextBox);
            this.Database_propertiesPanel.Controls.Add(this.Database_PortbeepTextBox);
            this.Database_propertiesPanel.Controls.Add(this.Database_HostbeepTextBox);
            // 
            // Database_HostbeepTextBox
            // 
            this.Database_HostbeepTextBox.Name = "Database_HostbeepTextBox";
            this.Database_HostbeepTextBox.PlaceholderText = "Host";
            this.Database_HostbeepTextBox.Location = new System.Drawing.Point(24, 24);
            this.Database_HostbeepTextBox.Size = new System.Drawing.Size(260, 40);
            // 
            // Database_PortbeepTextBox
            // 
            this.Database_PortbeepTextBox.Name = "Database_PortbeepTextBox";
            this.Database_PortbeepTextBox.PlaceholderText = "Port";
            this.Database_PortbeepTextBox.Location = new System.Drawing.Point(300, 24);
            this.Database_PortbeepTextBox.Size = new System.Drawing.Size(100, 40);
            // 
            // Database_DatabasebeepTextBox
            // 
            this.Database_DatabasebeepTextBox.Name = "Database_DatabasebeepTextBox";
            this.Database_DatabasebeepTextBox.PlaceholderText = "Database";
            this.Database_DatabasebeepTextBox.Location = new System.Drawing.Point(24, 80);
            this.Database_DatabasebeepTextBox.Size = new System.Drawing.Size(376, 40);
            // 
            // Database_SchemaNamebeepTextBox
            // 
            this.Database_SchemaNamebeepTextBox.Name = "Database_SchemaNamebeepTextBox";
            this.Database_SchemaNamebeepTextBox.PlaceholderText = "Schema Name";
            this.Database_SchemaNamebeepTextBox.Location = new System.Drawing.Point(24, 136);
            this.Database_SchemaNamebeepTextBox.Size = new System.Drawing.Size(240, 40);
            // 
            // Database_OracleSIDorServicebeepTextBox
            // 
            this.Database_OracleSIDorServicebeepTextBox.Name = "Database_OracleSIDorServicebeepTextBox";
            this.Database_OracleSIDorServicebeepTextBox.PlaceholderText = "Oracle SID/Service";
            this.Database_OracleSIDorServicebeepTextBox.Location = new System.Drawing.Point(280, 136);
            this.Database_OracleSIDorServicebeepTextBox.Size = new System.Drawing.Size(220, 40);
            // 
            // uc_DatabaseProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "uc_DatabaseProperties";
            this.Size = new System.Drawing.Size(547, 669);
            this.ConnectionPropertytabPage.ResumeLayout(false);
            this.Database_propertiesPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private TheTechIdea.Beep.Winform.Controls.BeepPanel Database_propertiesPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Database_HostbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Database_PortbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Database_DatabasebeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Database_SchemaNamebeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Database_OracleSIDorServicebeepTextBox;
    }
}

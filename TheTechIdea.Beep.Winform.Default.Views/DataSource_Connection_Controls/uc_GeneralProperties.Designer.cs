namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    partial class uc_GeneralProperties
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
            this.General_propertiesPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            this.General_CompositeLayerNamebeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.General_ConnectionNamebeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.General_GuidIDbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.General_IDbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.ConnectionPropertytabPage.SuspendLayout();
            this.General_propertiesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConnectionPropertytabPage
            // 
            this.ConnectionPropertytabPage.Controls.Add(this.General_propertiesPanel);
            // 
            // General_propertiesPanel
            // 
            this.General_propertiesPanel.Dock = DockStyle.Fill;
            this.General_propertiesPanel.Name = "General_propertiesPanel";
            this.General_propertiesPanel.TabIndex = 0;
            this.General_propertiesPanel.Controls.Add(this.General_CompositeLayerNamebeepTextBox);
            this.General_propertiesPanel.Controls.Add(this.General_ConnectionNamebeepTextBox);
            this.General_propertiesPanel.Controls.Add(this.General_GuidIDbeepTextBox);
            this.General_propertiesPanel.Controls.Add(this.General_IDbeepTextBox);
            // 
            // General_IDbeepTextBox
            // 
            this.General_IDbeepTextBox.Name = "General_IDbeepTextBox";
            this.General_IDbeepTextBox.PlaceholderText = "ID";
            this.General_IDbeepTextBox.Location = new System.Drawing.Point(24, 24);
            this.General_IDbeepTextBox.Size = new System.Drawing.Size(160, 40);
            this.General_IDbeepTextBox.ReadOnly = true;
            // 
            // General_GuidIDbeepTextBox
            // 
            this.General_GuidIDbeepTextBox.Name = "General_GuidIDbeepTextBox";
            this.General_GuidIDbeepTextBox.PlaceholderText = "Guid ID";
            this.General_GuidIDbeepTextBox.Location = new System.Drawing.Point(200, 24);
            this.General_GuidIDbeepTextBox.Size = new System.Drawing.Size(300, 40);
            this.General_GuidIDbeepTextBox.ReadOnly = true;
            // 
            // General_ConnectionNamebeepTextBox
            // 
            this.General_ConnectionNamebeepTextBox.Name = "General_ConnectionNamebeepTextBox";
            this.General_ConnectionNamebeepTextBox.PlaceholderText = "Connection Name";
            this.General_ConnectionNamebeepTextBox.Location = new System.Drawing.Point(24, 80);
            this.General_ConnectionNamebeepTextBox.Size = new System.Drawing.Size(476, 40);
            // 
            // General_CompositeLayerNamebeepTextBox
            // 
            this.General_CompositeLayerNamebeepTextBox.Name = "General_CompositeLayerNamebeepTextBox";
            this.General_CompositeLayerNamebeepTextBox.PlaceholderText = "Composite Layer Name";
            this.General_CompositeLayerNamebeepTextBox.Location = new System.Drawing.Point(24, 136);
            this.General_CompositeLayerNamebeepTextBox.Size = new System.Drawing.Size(476, 40);
            // 
            // uc_GeneralProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "uc_GeneralProperties";
            this.Size = new System.Drawing.Size(547, 669);
            this.ConnectionPropertytabPage.ResumeLayout(false);
            this.General_propertiesPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private TheTechIdea.Beep.Winform.Controls.BeepPanel General_propertiesPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox General_IDbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox General_GuidIDbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox General_ConnectionNamebeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox General_CompositeLayerNamebeepTextBox;
    }
}

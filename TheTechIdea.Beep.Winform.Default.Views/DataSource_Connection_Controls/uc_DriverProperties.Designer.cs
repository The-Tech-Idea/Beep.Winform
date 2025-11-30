namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    partial class uc_DriverProperties
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
            this.Driver_propertiesPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            this.Driver_DriverNamebeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Driver_DriverVersionbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Driver_ParametersbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Driver_propertiesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // Driver_propertiesPanel
            // 
            this.Driver_propertiesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Driver_propertiesPanel.Name = "Driver_propertiesPanel";
            this.Driver_propertiesPanel.TabIndex = 0;
            this.Driver_propertiesPanel.IsChild = true;
            this.Driver_propertiesPanel.Controls.Add(this.Driver_DriverNamebeepTextBox);
            this.Driver_propertiesPanel.Controls.Add(this.Driver_DriverVersionbeepTextBox);
            this.Driver_propertiesPanel.Controls.Add(this.Driver_ParametersbeepTextBox);
            // 
            // Driver_DriverNamebeepTextBox
            // 
            this.Driver_DriverNamebeepTextBox.Name = "Driver_DriverNamebeepTextBox";
            this.Driver_DriverNamebeepTextBox.LabelText = "Driver Name";
            this.Driver_DriverNamebeepTextBox.LabelTextOn = true;
            this.Driver_DriverNamebeepTextBox.Location = new System.Drawing.Point(20, 20);
            this.Driver_DriverNamebeepTextBox.Size = new System.Drawing.Size(350, 50);
            this.Driver_DriverNamebeepTextBox.IsChild = true;
            // 
            // Driver_DriverVersionbeepTextBox
            // 
            this.Driver_DriverVersionbeepTextBox.Name = "Driver_DriverVersionbeepTextBox";
            this.Driver_DriverVersionbeepTextBox.LabelText = "Driver Version";
            this.Driver_DriverVersionbeepTextBox.LabelTextOn = true;
            this.Driver_DriverVersionbeepTextBox.Location = new System.Drawing.Point(390, 20);
            this.Driver_DriverVersionbeepTextBox.Size = new System.Drawing.Size(120, 50);
            this.Driver_DriverVersionbeepTextBox.IsChild = true;
            // 
            // Driver_ParametersbeepTextBox
            // 
            this.Driver_ParametersbeepTextBox.Name = "Driver_ParametersbeepTextBox";
            this.Driver_ParametersbeepTextBox.LabelText = "Additional Parameters";
            this.Driver_ParametersbeepTextBox.LabelTextOn = true;
            this.Driver_ParametersbeepTextBox.Location = new System.Drawing.Point(20, 90);
            this.Driver_ParametersbeepTextBox.Size = new System.Drawing.Size(490, 80);
            this.Driver_ParametersbeepTextBox.Multiline = true;
            this.Driver_ParametersbeepTextBox.IsChild = true;
            // 
            // uc_DriverProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "uc_DriverProperties";
            this.Size = new System.Drawing.Size(550, 250);
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            // Add the panel to this UserControl
            this.Controls.Add(this.Driver_propertiesPanel);
            this.Driver_propertiesPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private TheTechIdea.Beep.Winform.Controls.BeepPanel Driver_propertiesPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Driver_DriverNamebeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Driver_DriverVersionbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Driver_ParametersbeepTextBox;
    }
}

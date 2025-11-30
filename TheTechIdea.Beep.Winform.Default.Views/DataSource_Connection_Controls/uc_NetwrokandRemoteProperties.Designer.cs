namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    partial class uc_NetwrokandRemoteProperties
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
            this.Network_propertiesPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            this.Network_HostbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Network_PortbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Network_UrlbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Network_propertiesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // Network_propertiesPanel
            // 
            this.Network_propertiesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Network_propertiesPanel.Name = "Network_propertiesPanel";
            this.Network_propertiesPanel.TabIndex = 0;
            this.Network_propertiesPanel.IsChild = true;
            this.Network_propertiesPanel.Controls.Add(this.Network_HostbeepTextBox);
            this.Network_propertiesPanel.Controls.Add(this.Network_PortbeepTextBox);
            this.Network_propertiesPanel.Controls.Add(this.Network_UrlbeepTextBox);
            // 
            // Network_HostbeepTextBox
            // 
            this.Network_HostbeepTextBox.Name = "Network_HostbeepTextBox";
            this.Network_HostbeepTextBox.LabelText = "Host / Server";
            this.Network_HostbeepTextBox.LabelTextOn = true;
            this.Network_HostbeepTextBox.Location = new System.Drawing.Point(20, 20);
            this.Network_HostbeepTextBox.Size = new System.Drawing.Size(350, 50);
            this.Network_HostbeepTextBox.IsChild = true;
            // 
            // Network_PortbeepTextBox
            // 
            this.Network_PortbeepTextBox.Name = "Network_PortbeepTextBox";
            this.Network_PortbeepTextBox.LabelText = "Port";
            this.Network_PortbeepTextBox.LabelTextOn = true;
            this.Network_PortbeepTextBox.Location = new System.Drawing.Point(390, 20);
            this.Network_PortbeepTextBox.Size = new System.Drawing.Size(120, 50);
            this.Network_PortbeepTextBox.IsChild = true;
            // 
            // Network_UrlbeepTextBox
            // 
            this.Network_UrlbeepTextBox.Name = "Network_UrlbeepTextBox";
            this.Network_UrlbeepTextBox.LabelText = "URL";
            this.Network_UrlbeepTextBox.LabelTextOn = true;
            this.Network_UrlbeepTextBox.Location = new System.Drawing.Point(20, 90);
            this.Network_UrlbeepTextBox.Size = new System.Drawing.Size(490, 50);
            this.Network_UrlbeepTextBox.IsChild = true;
            // 
            // uc_NetwrokandRemoteProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "uc_NetwrokandRemoteProperties";
            this.Size = new System.Drawing.Size(550, 200);
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            // Add the panel to this UserControl
            this.Controls.Add(this.Network_propertiesPanel);
            this.Network_propertiesPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private TheTechIdea.Beep.Winform.Controls.BeepPanel Network_propertiesPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Network_HostbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Network_PortbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Network_UrlbeepTextBox;
    }
}

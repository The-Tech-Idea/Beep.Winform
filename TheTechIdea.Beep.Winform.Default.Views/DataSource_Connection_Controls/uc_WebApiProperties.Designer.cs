namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    partial class uc_WebApiProperties
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
            this.WebApi_propertiesPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            this.WebApi_RequiresTokenRefreshbeepCheckBox = new TheTechIdea.Beep.Winform.Controls.BeepCheckBoxBool();
            this.WebApi_RequiresAuthenticationbeepCheckBox = new TheTechIdea.Beep.Winform.Controls.BeepCheckBoxBool();
            this.WebApi_ValidateServerCertificatebeepCheckBox = new TheTechIdea.Beep.Winform.Controls.BeepCheckBoxBool();
            this.WebApi_IgnoreSSLErrorsbeepCheckBox = new TheTechIdea.Beep.Winform.Controls.BeepCheckBoxBool();
            this.WebApi_RetryIntervalMsbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.WebApi_MaxRetriesbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.WebApi_TimeoutMsbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.WebApi_HttpMethodbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.ConnectionPropertytabPage.SuspendLayout();
            this.WebApi_propertiesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConnectionPropertytabPage
            // 
            this.ConnectionPropertytabPage.Controls.Add(this.WebApi_propertiesPanel);
            // 
            // WebApi_propertiesPanel
            // 
            this.WebApi_propertiesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WebApi_propertiesPanel.Name = "WebApi_propertiesPanel";
            this.WebApi_propertiesPanel.TabIndex = 0;
            this.WebApi_propertiesPanel.Controls.Add(this.WebApi_RequiresTokenRefreshbeepCheckBox);
            this.WebApi_propertiesPanel.Controls.Add(this.WebApi_RequiresAuthenticationbeepCheckBox);
            this.WebApi_propertiesPanel.Controls.Add(this.WebApi_ValidateServerCertificatebeepCheckBox);
            this.WebApi_propertiesPanel.Controls.Add(this.WebApi_IgnoreSSLErrorsbeepCheckBox);
            this.WebApi_propertiesPanel.Controls.Add(this.WebApi_RetryIntervalMsbeepTextBox);
            this.WebApi_propertiesPanel.Controls.Add(this.WebApi_MaxRetriesbeepTextBox);
            this.WebApi_propertiesPanel.Controls.Add(this.WebApi_TimeoutMsbeepTextBox);
            this.WebApi_propertiesPanel.Controls.Add(this.WebApi_HttpMethodbeepTextBox);
            // 
            // WebApi_HttpMethodbeepTextBox
            // 
            this.WebApi_HttpMethodbeepTextBox.Name = "WebApi_HttpMethodbeepTextBox";
            this.WebApi_HttpMethodbeepTextBox.PlaceholderText = "HTTP Method";
            this.WebApi_HttpMethodbeepTextBox.Location = new System.Drawing.Point(24, 24);
            this.WebApi_HttpMethodbeepTextBox.Size = new System.Drawing.Size(200, 40);
            // 
            // WebApi_TimeoutMsbeepTextBox
            // 
            this.WebApi_TimeoutMsbeepTextBox.Name = "WebApi_TimeoutMsbeepTextBox";
            this.WebApi_TimeoutMsbeepTextBox.PlaceholderText = "Timeout (ms)";
            this.WebApi_TimeoutMsbeepTextBox.Location = new System.Drawing.Point(240, 24);
            this.WebApi_TimeoutMsbeepTextBox.Size = new System.Drawing.Size(140, 40);
            // 
            // WebApi_MaxRetriesbeepTextBox
            // 
            this.WebApi_MaxRetriesbeepTextBox.Name = "WebApi_MaxRetriesbeepTextBox";
            this.WebApi_MaxRetriesbeepTextBox.PlaceholderText = "Max Retries";
            this.WebApi_MaxRetriesbeepTextBox.Location = new System.Drawing.Point(396, 24);
            this.WebApi_MaxRetriesbeepTextBox.Size = new System.Drawing.Size(124, 40);
            // 
            // WebApi_RetryIntervalMsbeepTextBox
            // 
            this.WebApi_RetryIntervalMsbeepTextBox.Name = "WebApi_RetryIntervalMsbeepTextBox";
            this.WebApi_RetryIntervalMsbeepTextBox.PlaceholderText = "Retry Interval (ms)";
            this.WebApi_RetryIntervalMsbeepTextBox.Location = new System.Drawing.Point(24, 80);
            this.WebApi_RetryIntervalMsbeepTextBox.Size = new System.Drawing.Size(200, 40);
            // 
            // WebApi_IgnoreSSLErrorsbeepCheckBox
            // 
            this.WebApi_IgnoreSSLErrorsbeepCheckBox.Name = "WebApi_IgnoreSSLErrorsbeepCheckBox";
            this.WebApi_IgnoreSSLErrorsbeepCheckBox.Text = "Ignore SSL Errors";
            this.WebApi_IgnoreSSLErrorsbeepCheckBox.Location = new System.Drawing.Point(24, 136);
            this.WebApi_IgnoreSSLErrorsbeepCheckBox.Size = new System.Drawing.Size(160, 32);
            // 
            // WebApi_ValidateServerCertificatebeepCheckBox
            // 
            this.WebApi_ValidateServerCertificatebeepCheckBox.Name = "WebApi_ValidateServerCertificatebeepCheckBox";
            this.WebApi_ValidateServerCertificatebeepCheckBox.Text = "Validate Server Certificate";
            this.WebApi_ValidateServerCertificatebeepCheckBox.Location = new System.Drawing.Point(200, 136);
            this.WebApi_ValidateServerCertificatebeepCheckBox.Size = new System.Drawing.Size(220, 32);
            // 
            // WebApi_RequiresAuthenticationbeepCheckBox
            // 
            this.WebApi_RequiresAuthenticationbeepCheckBox.Name = "WebApi_RequiresAuthenticationbeepCheckBox";
            this.WebApi_RequiresAuthenticationbeepCheckBox.Text = "Requires Authentication";
            this.WebApi_RequiresAuthenticationbeepCheckBox.Location = new System.Drawing.Point(24, 184);
            this.WebApi_RequiresAuthenticationbeepCheckBox.Size = new System.Drawing.Size(200, 32);
            // 
            // WebApi_RequiresTokenRefreshbeepCheckBox
            // 
            this.WebApi_RequiresTokenRefreshbeepCheckBox.Name = "WebApi_RequiresTokenRefreshbeepCheckBox";
            this.WebApi_RequiresTokenRefreshbeepCheckBox.Text = "Requires Token Refresh";
            this.WebApi_RequiresTokenRefreshbeepCheckBox.Location = new System.Drawing.Point(240, 184);
            this.WebApi_RequiresTokenRefreshbeepCheckBox.Size = new System.Drawing.Size(200, 32);
            // 
            // uc_WebApiProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "uc_WebApiProperties";
            this.Size = new System.Drawing.Size(547, 669);
            this.ConnectionPropertytabPage.ResumeLayout(false);
            this.WebApi_propertiesPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private TheTechIdea.Beep.Winform.Controls.BeepPanel WebApi_propertiesPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox WebApi_HttpMethodbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox WebApi_TimeoutMsbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox WebApi_MaxRetriesbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox WebApi_RetryIntervalMsbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepCheckBoxBool WebApi_IgnoreSSLErrorsbeepCheckBox;
        private TheTechIdea.Beep.Winform.Controls.BeepCheckBoxBool WebApi_ValidateServerCertificatebeepCheckBox;
        private TheTechIdea.Beep.Winform.Controls.BeepCheckBoxBool WebApi_RequiresAuthenticationbeepCheckBox;
        private TheTechIdea.Beep.Winform.Controls.BeepCheckBoxBool WebApi_RequiresTokenRefreshbeepCheckBox;
    }
}

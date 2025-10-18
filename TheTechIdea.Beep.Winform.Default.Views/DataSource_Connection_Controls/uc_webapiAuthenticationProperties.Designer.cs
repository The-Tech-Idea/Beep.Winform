using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    partial class uc_webapiAuthenticationProperties
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
            this.WebApiAuth_propertiesPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            this.WebApiAuth_AuthTypebeepComboBox = new TheTechIdea.Beep.Winform.Controls.BeepComboBox();
            this.WebApiAuth_ApiKeybeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.WebApiAuth_ApiKeyHeaderbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.WebApiAuth_ClientIdbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.WebApiAuth_ClientSecretbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.WebApiAuth_AuthUrlbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.WebApiAuth_TokenUrlbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.WebApiAuth_ScopebeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.WebApiAuth_GrantTypebeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.WebApiAuth_RedirectUribeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.WebApiAuth_AuthCodebeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.ConnectionPropertytabPage.SuspendLayout();
            this.WebApiAuth_propertiesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConnectionPropertytabPage
            // 
            this.ConnectionPropertytabPage.Controls.Add(this.WebApiAuth_propertiesPanel);
            // 
            // WebApiAuth_propertiesPanel
            // 
            this.WebApiAuth_propertiesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WebApiAuth_propertiesPanel.Name = "WebApiAuth_propertiesPanel";
            this.WebApiAuth_propertiesPanel.TabIndex = 0;
            this.WebApiAuth_propertiesPanel.Controls.Add(this.WebApiAuth_AuthTypebeepComboBox);
            this.WebApiAuth_propertiesPanel.Controls.Add(this.WebApiAuth_ApiKeybeepTextBox);
            this.WebApiAuth_propertiesPanel.Controls.Add(this.WebApiAuth_ApiKeyHeaderbeepTextBox);
            this.WebApiAuth_propertiesPanel.Controls.Add(this.WebApiAuth_ClientIdbeepTextBox);
            this.WebApiAuth_propertiesPanel.Controls.Add(this.WebApiAuth_ClientSecretbeepTextBox);
            this.WebApiAuth_propertiesPanel.Controls.Add(this.WebApiAuth_AuthUrlbeepTextBox);
            this.WebApiAuth_propertiesPanel.Controls.Add(this.WebApiAuth_TokenUrlbeepTextBox);
            this.WebApiAuth_propertiesPanel.Controls.Add(this.WebApiAuth_ScopebeepTextBox);
            this.WebApiAuth_propertiesPanel.Controls.Add(this.WebApiAuth_GrantTypebeepTextBox);
            this.WebApiAuth_propertiesPanel.Controls.Add(this.WebApiAuth_RedirectUribeepTextBox);
            this.WebApiAuth_propertiesPanel.Controls.Add(this.WebApiAuth_AuthCodebeepTextBox);
            // 
            // WebApiAuth_AuthTypebeepComboBox
            // 
            this.WebApiAuth_AuthTypebeepComboBox.Name = "WebApiAuth_AuthTypebeepComboBox";
            this.WebApiAuth_AuthTypebeepComboBox.PlaceholderText = "Auth Type";
            this.WebApiAuth_AuthTypebeepComboBox.Location = new System.Drawing.Point(24, 24);
            this.WebApiAuth_AuthTypebeepComboBox.Size = new System.Drawing.Size(220, 48);
            // 
            // WebApiAuth_ApiKeybeepTextBox
            // 
            this.WebApiAuth_ApiKeybeepTextBox.Name = "WebApiAuth_ApiKeybeepTextBox";
            this.WebApiAuth_ApiKeybeepTextBox.PlaceholderText = "API Key";
            this.WebApiAuth_ApiKeybeepTextBox.Location = new System.Drawing.Point(260, 24);
            this.WebApiAuth_ApiKeybeepTextBox.Size = new System.Drawing.Size(260, 40);
            // 
            // WebApiAuth_ApiKeyHeaderbeepTextBox
            // 
            this.WebApiAuth_ApiKeyHeaderbeepTextBox.Name = "WebApiAuth_ApiKeyHeaderbeepTextBox";
            this.WebApiAuth_ApiKeyHeaderbeepTextBox.PlaceholderText = "API Key Header";
            this.WebApiAuth_ApiKeyHeaderbeepTextBox.Location = new System.Drawing.Point(24, 80);
            this.WebApiAuth_ApiKeyHeaderbeepTextBox.Size = new System.Drawing.Size(220, 40);
            // 
            // WebApiAuth_ClientIdbeepTextBox
            // 
            this.WebApiAuth_ClientIdbeepTextBox.Name = "WebApiAuth_ClientIdbeepTextBox";
            this.WebApiAuth_ClientIdbeepTextBox.PlaceholderText = "Client ID";
            this.WebApiAuth_ClientIdbeepTextBox.Location = new System.Drawing.Point(260, 80);
            this.WebApiAuth_ClientIdbeepTextBox.Size = new System.Drawing.Size(260, 40);
            // 
            // WebApiAuth_ClientSecretbeepTextBox
            // 
            this.WebApiAuth_ClientSecretbeepTextBox.Name = "WebApiAuth_ClientSecretbeepTextBox";
            this.WebApiAuth_ClientSecretbeepTextBox.PlaceholderText = "Client Secret";
            this.WebApiAuth_ClientSecretbeepTextBox.UseSystemPasswordChar = true;
            this.WebApiAuth_ClientSecretbeepTextBox.Location = new System.Drawing.Point(24, 136);
            this.WebApiAuth_ClientSecretbeepTextBox.Size = new System.Drawing.Size(220, 40);
            // 
            // WebApiAuth_AuthUrlbeepTextBox
            // 
            this.WebApiAuth_AuthUrlbeepTextBox.Name = "WebApiAuth_AuthUrlbeepTextBox";
            this.WebApiAuth_AuthUrlbeepTextBox.PlaceholderText = "Auth URL";
            this.WebApiAuth_AuthUrlbeepTextBox.Location = new System.Drawing.Point(260, 136);
            this.WebApiAuth_AuthUrlbeepTextBox.Size = new System.Drawing.Size(260, 40);
            // 
            // WebApiAuth_TokenUrlbeepTextBox
            // 
            this.WebApiAuth_TokenUrlbeepTextBox.Name = "WebApiAuth_TokenUrlbeepTextBox";
            this.WebApiAuth_TokenUrlbeepTextBox.PlaceholderText = "Token URL";
            this.WebApiAuth_TokenUrlbeepTextBox.Location = new System.Drawing.Point(24, 192);
            this.WebApiAuth_TokenUrlbeepTextBox.Size = new System.Drawing.Size(220, 40);
            // 
            // WebApiAuth_ScopebeepTextBox
            // 
            this.WebApiAuth_ScopebeepTextBox.Name = "WebApiAuth_ScopebeepTextBox";
            this.WebApiAuth_ScopebeepTextBox.PlaceholderText = "Scope";
            this.WebApiAuth_ScopebeepTextBox.Location = new System.Drawing.Point(260, 192);
            this.WebApiAuth_ScopebeepTextBox.Size = new System.Drawing.Size(260, 40);
            // 
            // WebApiAuth_GrantTypebeepTextBox
            // 
            this.WebApiAuth_GrantTypebeepTextBox.Name = "WebApiAuth_GrantTypebeepTextBox";
            this.WebApiAuth_GrantTypebeepTextBox.PlaceholderText = "Grant Type";
            this.WebApiAuth_GrantTypebeepTextBox.Location = new System.Drawing.Point(24, 248);
            this.WebApiAuth_GrantTypebeepTextBox.Size = new System.Drawing.Size(220, 40);
            // 
            // WebApiAuth_RedirectUribeepTextBox
            // 
            this.WebApiAuth_RedirectUribeepTextBox.Name = "WebApiAuth_RedirectUribeepTextBox";
            this.WebApiAuth_RedirectUribeepTextBox.PlaceholderText = "Redirect URI";
            this.WebApiAuth_RedirectUribeepTextBox.Location = new System.Drawing.Point(260, 248);
            this.WebApiAuth_RedirectUribeepTextBox.Size = new System.Drawing.Size(260, 40);
            // 
            // WebApiAuth_AuthCodebeepTextBox
            // 
            this.WebApiAuth_AuthCodebeepTextBox.Name = "WebApiAuth_AuthCodebeepTextBox";
            this.WebApiAuth_AuthCodebeepTextBox.PlaceholderText = "Auth Code";
            this.WebApiAuth_AuthCodebeepTextBox.Location = new System.Drawing.Point(24, 304);
            this.WebApiAuth_AuthCodebeepTextBox.Size = new System.Drawing.Size(220, 40);
            // 
            // uc_webapiAuthenticationProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "uc_webapiAuthenticationProperties";
            this.Size = new System.Drawing.Size(547, 669);
            this.ConnectionPropertytabPage.ResumeLayout(false);
            this.WebApiAuth_propertiesPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private TheTechIdea.Beep.Winform.Controls.BeepPanel WebApiAuth_propertiesPanel;
        private BeepComboBox WebApiAuth_AuthTypebeepComboBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox WebApiAuth_ApiKeybeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox WebApiAuth_ApiKeyHeaderbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox WebApiAuth_ClientIdbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox WebApiAuth_ClientSecretbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox WebApiAuth_AuthUrlbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox WebApiAuth_TokenUrlbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox WebApiAuth_ScopebeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox WebApiAuth_GrantTypebeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox WebApiAuth_RedirectUribeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox WebApiAuth_AuthCodebeepTextBox;
    }
}

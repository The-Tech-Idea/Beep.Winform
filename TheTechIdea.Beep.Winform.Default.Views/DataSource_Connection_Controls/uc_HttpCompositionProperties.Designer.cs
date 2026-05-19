namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    partial class uc_HttpCompositionProperties
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
            this.Http_propertiesPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            this.Http_BasePathbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Http_AcceptHeaderbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Http_ContentTypebeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Http_DefaultHeadersbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Http_DefaultQueryParamsbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Http_UserAgentbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Http_UseCompressionbeepCheckBox = new TheTechIdea.Beep.Winform.Controls.CheckBoxes.BeepCheckBoxBool();
            this.Http_FollowRedirectsbeepCheckBox = new TheTechIdea.Beep.Winform.Controls.CheckBoxes.BeepCheckBoxBool();
            this.Http_propertiesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // Http_propertiesPanel
            // 
            this.Http_propertiesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Http_propertiesPanel.Name = "Http_propertiesPanel";
            this.Http_propertiesPanel.TabIndex = 0;
            this.Http_propertiesPanel.IsChild = true;
            this.Http_propertiesPanel.AutoScroll = true;
            this.Http_propertiesPanel.Controls.Add(this.Http_BasePathbeepTextBox);
            this.Http_propertiesPanel.Controls.Add(this.Http_AcceptHeaderbeepTextBox);
            this.Http_propertiesPanel.Controls.Add(this.Http_ContentTypebeepTextBox);
            this.Http_propertiesPanel.Controls.Add(this.Http_DefaultHeadersbeepTextBox);
            this.Http_propertiesPanel.Controls.Add(this.Http_DefaultQueryParamsbeepTextBox);
            this.Http_propertiesPanel.Controls.Add(this.Http_UserAgentbeepTextBox);
            this.Http_propertiesPanel.Controls.Add(this.Http_UseCompressionbeepCheckBox);
            this.Http_propertiesPanel.Controls.Add(this.Http_FollowRedirectsbeepCheckBox);
            // 
            // Http_BasePathbeepTextBox
            // 
            this.Http_BasePathbeepTextBox.Name = "Http_BasePathbeepTextBox";
            this.Http_BasePathbeepTextBox.LabelText = "Base Path";
            this.Http_BasePathbeepTextBox.LabelTextOn = true;
            this.Http_BasePathbeepTextBox.Location = new System.Drawing.Point(20, 20);
            this.Http_BasePathbeepTextBox.Size = new System.Drawing.Size(480, 50);
            this.Http_BasePathbeepTextBox.IsChild = true;
            // 
            // Http_AcceptHeaderbeepTextBox
            // 
            this.Http_AcceptHeaderbeepTextBox.Name = "Http_AcceptHeaderbeepTextBox";
            this.Http_AcceptHeaderbeepTextBox.LabelText = "Accept";
            this.Http_AcceptHeaderbeepTextBox.LabelTextOn = true;
            this.Http_AcceptHeaderbeepTextBox.Location = new System.Drawing.Point(20, 80);
            this.Http_AcceptHeaderbeepTextBox.Size = new System.Drawing.Size(480, 50);
            this.Http_AcceptHeaderbeepTextBox.IsChild = true;
            // 
            // Http_ContentTypebeepTextBox
            // 
            this.Http_ContentTypebeepTextBox.Name = "Http_ContentTypebeepTextBox";
            this.Http_ContentTypebeepTextBox.LabelText = "Content Type";
            this.Http_ContentTypebeepTextBox.LabelTextOn = true;
            this.Http_ContentTypebeepTextBox.Location = new System.Drawing.Point(20, 140);
            this.Http_ContentTypebeepTextBox.Size = new System.Drawing.Size(480, 50);
            this.Http_ContentTypebeepTextBox.IsChild = true;
            // 
            // Http_DefaultHeadersbeepTextBox
            // 
            this.Http_DefaultHeadersbeepTextBox.Name = "Http_DefaultHeadersbeepTextBox";
            this.Http_DefaultHeadersbeepTextBox.LabelText = "Default Headers";
            this.Http_DefaultHeadersbeepTextBox.LabelTextOn = true;
            this.Http_DefaultHeadersbeepTextBox.Location = new System.Drawing.Point(20, 200);
            this.Http_DefaultHeadersbeepTextBox.Size = new System.Drawing.Size(480, 80);
            this.Http_DefaultHeadersbeepTextBox.IsChild = true;
            this.Http_DefaultHeadersbeepTextBox.Multiline = true;
            // 
            // Http_DefaultQueryParamsbeepTextBox
            // 
            this.Http_DefaultQueryParamsbeepTextBox.Name = "Http_DefaultQueryParamsbeepTextBox";
            this.Http_DefaultQueryParamsbeepTextBox.LabelText = "Default Query Params";
            this.Http_DefaultQueryParamsbeepTextBox.LabelTextOn = true;
            this.Http_DefaultQueryParamsbeepTextBox.Location = new System.Drawing.Point(20, 290);
            this.Http_DefaultQueryParamsbeepTextBox.Size = new System.Drawing.Size(480, 80);
            this.Http_DefaultQueryParamsbeepTextBox.IsChild = true;
            this.Http_DefaultQueryParamsbeepTextBox.Multiline = true;
            // 
            // Http_UserAgentbeepTextBox
            // 
            this.Http_UserAgentbeepTextBox.Name = "Http_UserAgentbeepTextBox";
            this.Http_UserAgentbeepTextBox.LabelText = "User Agent";
            this.Http_UserAgentbeepTextBox.LabelTextOn = true;
            this.Http_UserAgentbeepTextBox.Location = new System.Drawing.Point(20, 380);
            this.Http_UserAgentbeepTextBox.Size = new System.Drawing.Size(480, 50);
            this.Http_UserAgentbeepTextBox.IsChild = true;
            // 
            // Http_UseCompressionbeepCheckBox
            // 
            this.Http_UseCompressionbeepCheckBox.Name = "Http_UseCompressionbeepCheckBox";
            this.Http_UseCompressionbeepCheckBox.Text = "Use Compression";
            this.Http_UseCompressionbeepCheckBox.LabelText = "Use Compression";
            this.Http_UseCompressionbeepCheckBox.LabelTextOn = true;
            this.Http_UseCompressionbeepCheckBox.Location = new System.Drawing.Point(20, 440);
            this.Http_UseCompressionbeepCheckBox.Size = new System.Drawing.Size(200, 40);
            this.Http_UseCompressionbeepCheckBox.IsChild = true;
            // 
            // Http_FollowRedirectsbeepCheckBox
            // 
            this.Http_FollowRedirectsbeepCheckBox.Name = "Http_FollowRedirectsbeepCheckBox";
            this.Http_FollowRedirectsbeepCheckBox.Text = "Follow Redirects";
            this.Http_FollowRedirectsbeepCheckBox.LabelText = "Follow Redirects";
            this.Http_FollowRedirectsbeepCheckBox.LabelTextOn = true;
            this.Http_FollowRedirectsbeepCheckBox.Location = new System.Drawing.Point(20, 490);
            this.Http_FollowRedirectsbeepCheckBox.Size = new System.Drawing.Size(200, 40);
            this.Http_FollowRedirectsbeepCheckBox.IsChild = true;
            // 
            // uc_HttpCompositionProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "uc_HttpCompositionProperties";
            this.Size = new System.Drawing.Size(550, 600);
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Controls.Add(this.Http_propertiesPanel);
            this.Text = "Http Composition";
            this.Http_propertiesPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private TheTechIdea.Beep.Winform.Controls.BeepPanel Http_propertiesPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Http_BasePathbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Http_AcceptHeaderbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Http_ContentTypebeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Http_DefaultHeadersbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Http_DefaultQueryParamsbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Http_UserAgentbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.CheckBoxes.BeepCheckBoxBool Http_UseCompressionbeepCheckBox;
        private TheTechIdea.Beep.Winform.Controls.CheckBoxes.BeepCheckBoxBool Http_FollowRedirectsbeepCheckBox;
    }
}

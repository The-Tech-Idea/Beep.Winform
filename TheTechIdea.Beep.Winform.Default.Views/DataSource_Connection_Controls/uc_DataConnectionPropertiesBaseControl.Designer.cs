namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    partial class uc_DataConnectionPropertiesBaseControl
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
            beepTabs1 = new TheTechIdea.Beep.Winform.Controls.BeepTabs();
            ConnectionPropertytabPage = new TabPage();
            MainTemplatePanel.SuspendLayout();
            beepTabs1.SuspendLayout();
            SuspendLayout();
            // 
            // MainTemplatePanel
            // 
            MainTemplatePanel.Controls.Add(beepTabs1);
            MainTemplatePanel.DrawingRect = new Rectangle(0, 0, 547, 669);
            MainTemplatePanel.Size = new Size(547, 669);
            // 
            // beepTabs1
            // 
            beepTabs1.AccessibleName = "Beep Tabs";
            beepTabs1.AccessibleRole = AccessibleRole.PageTabList;
            beepTabs1.AllowDrop = true;
            beepTabs1.Appearance = TabAppearance.FlatButtons;
            beepTabs1.Controls.Add(ConnectionPropertytabPage);
            beepTabs1.Dock = DockStyle.Fill;
            beepTabs1.DrawMode = TabDrawMode.OwnerDrawFixed;
            beepTabs1.ForeColor = Color.DimGray;
            beepTabs1.HeaderHeight = 30;
            beepTabs1.HeaderPosition = Winform.Controls.TabHeaderPosition.Top;
            beepTabs1.ItemSize = new Size(0, 1);
            beepTabs1.Location = new Point(0, 0);
            beepTabs1.Name = "beepTabs1";
            beepTabs1.Padding = new Point(5, 5);
            beepTabs1.SelectedIndex = 0;
            beepTabs1.SelectTab = ConnectionPropertytabPage;
            beepTabs1.Size = new Size(547, 669);
            beepTabs1.SizeMode = TabSizeMode.Fixed;
            beepTabs1.TabIndex = 0;
            beepTabs1.Theme = "DefaultTheme";
            // 
            // ConnectionPropertytabPage
            // 
            ConnectionPropertytabPage.BackColor = Color.White;
            ConnectionPropertytabPage.ForeColor = Color.DimGray;
            ConnectionPropertytabPage.Location = new Point(0, 30);
            ConnectionPropertytabPage.Name = "ConnectionPropertytabPage";
            ConnectionPropertytabPage.Padding = new Padding(3);
            ConnectionPropertytabPage.Size = new Size(547, 639);
            ConnectionPropertytabPage.TabIndex = 0;
            ConnectionPropertytabPage.Text = "Connection Property";
            // 
            // uc_DataConnectionPropertiesBaseControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Name = "uc_DataConnectionPropertiesBaseControl";
            Size = new Size(547, 669);
            MainTemplatePanel.ResumeLayout(false);
            beepTabs1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        public Controls.BeepTabs beepTabs1;
        public TabPage ConnectionPropertytabPage;
    }
}

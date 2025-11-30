namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    partial class uc_GeneralProperties
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
            this.General_propertiesPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            this.General_IDbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.General_GuidIDbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.General_ConnectionNamebeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.General_ConnectionStringbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.General_CategorybeepComboBox = new TheTechIdea.Beep.Winform.Controls.BeepComboBox();
            this.General_FavouritebeepCheckBox = new TheTechIdea.Beep.Winform.Controls.BeepCheckBoxBool();
            this.General_IsDefaultbeepCheckBox = new TheTechIdea.Beep.Winform.Controls.BeepCheckBoxBool();
            this.General_DrawnbeepCheckBox = new TheTechIdea.Beep.Winform.Controls.BeepCheckBoxBool();
            this.General_propertiesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // General_propertiesPanel
            // 
            this.General_propertiesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.General_propertiesPanel.Name = "General_propertiesPanel";
            this.General_propertiesPanel.TabIndex = 0;
            this.General_propertiesPanel.IsChild = true;
            this.General_propertiesPanel.Controls.Add(this.General_IDbeepTextBox);
            this.General_propertiesPanel.Controls.Add(this.General_GuidIDbeepTextBox);
            this.General_propertiesPanel.Controls.Add(this.General_ConnectionNamebeepTextBox);
            this.General_propertiesPanel.Controls.Add(this.General_ConnectionStringbeepTextBox);
            this.General_propertiesPanel.Controls.Add(this.General_CategorybeepComboBox);
            this.General_propertiesPanel.Controls.Add(this.General_FavouritebeepCheckBox);
            this.General_propertiesPanel.Controls.Add(this.General_IsDefaultbeepCheckBox);
            this.General_propertiesPanel.Controls.Add(this.General_DrawnbeepCheckBox);
            // 
            // General_IDbeepTextBox
            // 
            this.General_IDbeepTextBox.Name = "General_IDbeepTextBox";
            this.General_IDbeepTextBox.LabelText = "ID";
            this.General_IDbeepTextBox.LabelTextOn = true;
            this.General_IDbeepTextBox.Location = new System.Drawing.Point(20, 20);
            this.General_IDbeepTextBox.Size = new System.Drawing.Size(120, 50);
            this.General_IDbeepTextBox.ReadOnly = true;
            this.General_IDbeepTextBox.IsChild = true;
            // 
            // General_GuidIDbeepTextBox
            // 
            this.General_GuidIDbeepTextBox.Name = "General_GuidIDbeepTextBox";
            this.General_GuidIDbeepTextBox.LabelText = "Guid ID";
            this.General_GuidIDbeepTextBox.LabelTextOn = true;
            this.General_GuidIDbeepTextBox.Location = new System.Drawing.Point(160, 20);
            this.General_GuidIDbeepTextBox.Size = new System.Drawing.Size(350, 50);
            this.General_GuidIDbeepTextBox.ReadOnly = true;
            this.General_GuidIDbeepTextBox.IsChild = true;
            // 
            // General_ConnectionNamebeepTextBox
            // 
            this.General_ConnectionNamebeepTextBox.Name = "General_ConnectionNamebeepTextBox";
            this.General_ConnectionNamebeepTextBox.LabelText = "Connection Name";
            this.General_ConnectionNamebeepTextBox.LabelTextOn = true;
            this.General_ConnectionNamebeepTextBox.Location = new System.Drawing.Point(20, 90);
            this.General_ConnectionNamebeepTextBox.Size = new System.Drawing.Size(490, 50);
            this.General_ConnectionNamebeepTextBox.IsChild = true;
            // 
            // General_ConnectionStringbeepTextBox
            // 
            this.General_ConnectionStringbeepTextBox.Name = "General_ConnectionStringbeepTextBox";
            this.General_ConnectionStringbeepTextBox.LabelText = "Connection String";
            this.General_ConnectionStringbeepTextBox.LabelTextOn = true;
            this.General_ConnectionStringbeepTextBox.Location = new System.Drawing.Point(20, 160);
            this.General_ConnectionStringbeepTextBox.Size = new System.Drawing.Size(490, 80);
            this.General_ConnectionStringbeepTextBox.Multiline = true;
            this.General_ConnectionStringbeepTextBox.IsChild = true;
            // 
            // General_CategorybeepComboBox
            // 
            this.General_CategorybeepComboBox.Name = "General_CategorybeepComboBox";
            this.General_CategorybeepComboBox.LabelText = "Category";
            this.General_CategorybeepComboBox.LabelTextOn = true;
            this.General_CategorybeepComboBox.Location = new System.Drawing.Point(20, 260);
            this.General_CategorybeepComboBox.Size = new System.Drawing.Size(240, 50);
            this.General_CategorybeepComboBox.IsChild = true;
            // 
            // General_FavouritebeepCheckBox
            // 
            this.General_FavouritebeepCheckBox.Name = "General_FavouritebeepCheckBox";
            this.General_FavouritebeepCheckBox.Text = "Favourite";
            this.General_FavouritebeepCheckBox.Location = new System.Drawing.Point(20, 330);
            this.General_FavouritebeepCheckBox.Size = new System.Drawing.Size(150, 30);
            this.General_FavouritebeepCheckBox.IsChild = true;
            // 
            // General_IsDefaultbeepCheckBox
            // 
            this.General_IsDefaultbeepCheckBox.Name = "General_IsDefaultbeepCheckBox";
            this.General_IsDefaultbeepCheckBox.Text = "Is Default";
            this.General_IsDefaultbeepCheckBox.Location = new System.Drawing.Point(180, 330);
            this.General_IsDefaultbeepCheckBox.Size = new System.Drawing.Size(150, 30);
            this.General_IsDefaultbeepCheckBox.IsChild = true;
            // 
            // General_DrawnbeepCheckBox
            // 
            this.General_DrawnbeepCheckBox.Name = "General_DrawnbeepCheckBox";
            this.General_DrawnbeepCheckBox.Text = "Drawn";
            this.General_DrawnbeepCheckBox.Location = new System.Drawing.Point(340, 330);
            this.General_DrawnbeepCheckBox.Size = new System.Drawing.Size(150, 30);
            this.General_DrawnbeepCheckBox.IsChild = true;
            // 
            // uc_GeneralProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "uc_GeneralProperties";
            this.Size = new System.Drawing.Size(550, 450);
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            // Add the panel to this UserControl
            this.Controls.Add(this.General_propertiesPanel);
            this.General_propertiesPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private TheTechIdea.Beep.Winform.Controls.BeepPanel General_propertiesPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox General_IDbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox General_GuidIDbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox General_ConnectionNamebeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox General_ConnectionStringbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepComboBox General_CategorybeepComboBox;
        private TheTechIdea.Beep.Winform.Controls.BeepCheckBoxBool General_FavouritebeepCheckBox;
        private TheTechIdea.Beep.Winform.Controls.BeepCheckBoxBool General_IsDefaultbeepCheckBox;
        private TheTechIdea.Beep.Winform.Controls.BeepCheckBoxBool General_DrawnbeepCheckBox;
    }
}

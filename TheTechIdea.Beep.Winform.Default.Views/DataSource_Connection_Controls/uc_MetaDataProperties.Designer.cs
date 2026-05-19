namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    partial class uc_MetaDataProperties
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
            this.Meta_propertiesPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            this.Meta_SchemaNamebeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Meta_MetadataCatalogbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Meta_EntityFilterbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Meta_RefreshSecondsbeepTextBox = new TheTechIdea.Beep.Winform.Controls.BeepTextBox();
            this.Meta_IncludeViewsbeepCheckBox = new TheTechIdea.Beep.Winform.Controls.CheckBoxes.BeepCheckBoxBool();
            this.Meta_IncludeSystemObjectsbeepCheckBox = new TheTechIdea.Beep.Winform.Controls.CheckBoxes.BeepCheckBoxBool();
            this.Meta_propertiesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // Meta_propertiesPanel
            // 
            this.Meta_propertiesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Meta_propertiesPanel.Name = "Meta_propertiesPanel";
            this.Meta_propertiesPanel.TabIndex = 0;
            this.Meta_propertiesPanel.IsChild = true;
            this.Meta_propertiesPanel.AutoScroll = true;
            this.Meta_propertiesPanel.Controls.Add(this.Meta_SchemaNamebeepTextBox);
            this.Meta_propertiesPanel.Controls.Add(this.Meta_MetadataCatalogbeepTextBox);
            this.Meta_propertiesPanel.Controls.Add(this.Meta_EntityFilterbeepTextBox);
            this.Meta_propertiesPanel.Controls.Add(this.Meta_RefreshSecondsbeepTextBox);
            this.Meta_propertiesPanel.Controls.Add(this.Meta_IncludeViewsbeepCheckBox);
            this.Meta_propertiesPanel.Controls.Add(this.Meta_IncludeSystemObjectsbeepCheckBox);
            // 
            // Meta_SchemaNamebeepTextBox
            // 
            this.Meta_SchemaNamebeepTextBox.Name = "Meta_SchemaNamebeepTextBox";
            this.Meta_SchemaNamebeepTextBox.LabelText = "Schema Name";
            this.Meta_SchemaNamebeepTextBox.LabelTextOn = true;
            this.Meta_SchemaNamebeepTextBox.Location = new System.Drawing.Point(20, 20);
            this.Meta_SchemaNamebeepTextBox.Size = new System.Drawing.Size(480, 50);
            this.Meta_SchemaNamebeepTextBox.IsChild = true;
            // 
            // Meta_MetadataCatalogbeepTextBox
            // 
            this.Meta_MetadataCatalogbeepTextBox.Name = "Meta_MetadataCatalogbeepTextBox";
            this.Meta_MetadataCatalogbeepTextBox.LabelText = "Metadata Catalog";
            this.Meta_MetadataCatalogbeepTextBox.LabelTextOn = true;
            this.Meta_MetadataCatalogbeepTextBox.Location = new System.Drawing.Point(20, 80);
            this.Meta_MetadataCatalogbeepTextBox.Size = new System.Drawing.Size(480, 50);
            this.Meta_MetadataCatalogbeepTextBox.IsChild = true;
            // 
            // Meta_EntityFilterbeepTextBox
            // 
            this.Meta_EntityFilterbeepTextBox.Name = "Meta_EntityFilterbeepTextBox";
            this.Meta_EntityFilterbeepTextBox.LabelText = "Entity Filter";
            this.Meta_EntityFilterbeepTextBox.LabelTextOn = true;
            this.Meta_EntityFilterbeepTextBox.Location = new System.Drawing.Point(20, 140);
            this.Meta_EntityFilterbeepTextBox.Size = new System.Drawing.Size(480, 50);
            this.Meta_EntityFilterbeepTextBox.IsChild = true;
            // 
            // Meta_RefreshSecondsbeepTextBox
            // 
            this.Meta_RefreshSecondsbeepTextBox.Name = "Meta_RefreshSecondsbeepTextBox";
            this.Meta_RefreshSecondsbeepTextBox.LabelText = "Refresh Seconds";
            this.Meta_RefreshSecondsbeepTextBox.LabelTextOn = true;
            this.Meta_RefreshSecondsbeepTextBox.Location = new System.Drawing.Point(20, 200);
            this.Meta_RefreshSecondsbeepTextBox.Size = new System.Drawing.Size(480, 50);
            this.Meta_RefreshSecondsbeepTextBox.IsChild = true;
            // 
            // Meta_IncludeViewsbeepCheckBox
            // 
            this.Meta_IncludeViewsbeepCheckBox.Name = "Meta_IncludeViewsbeepCheckBox";
            this.Meta_IncludeViewsbeepCheckBox.Text = "Include Views";
            this.Meta_IncludeViewsbeepCheckBox.LabelText = "Include Views";
            this.Meta_IncludeViewsbeepCheckBox.LabelTextOn = true;
            this.Meta_IncludeViewsbeepCheckBox.Location = new System.Drawing.Point(20, 260);
            this.Meta_IncludeViewsbeepCheckBox.Size = new System.Drawing.Size(200, 40);
            this.Meta_IncludeViewsbeepCheckBox.IsChild = true;
            // 
            // Meta_IncludeSystemObjectsbeepCheckBox
            // 
            this.Meta_IncludeSystemObjectsbeepCheckBox.Name = "Meta_IncludeSystemObjectsbeepCheckBox";
            this.Meta_IncludeSystemObjectsbeepCheckBox.Text = "Include System Objects";
            this.Meta_IncludeSystemObjectsbeepCheckBox.LabelText = "Include System Objects";
            this.Meta_IncludeSystemObjectsbeepCheckBox.LabelTextOn = true;
            this.Meta_IncludeSystemObjectsbeepCheckBox.Location = new System.Drawing.Point(20, 310);
            this.Meta_IncludeSystemObjectsbeepCheckBox.Size = new System.Drawing.Size(200, 40);
            this.Meta_IncludeSystemObjectsbeepCheckBox.IsChild = true;
            // 
            // uc_MetaDataProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "uc_MetaDataProperties";
            this.Size = new System.Drawing.Size(550, 400);
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Controls.Add(this.Meta_propertiesPanel);
            this.Text = "Metadata";
            this.Meta_propertiesPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private TheTechIdea.Beep.Winform.Controls.BeepPanel Meta_propertiesPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Meta_SchemaNamebeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Meta_MetadataCatalogbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Meta_EntityFilterbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.BeepTextBox Meta_RefreshSecondsbeepTextBox;
        private TheTechIdea.Beep.Winform.Controls.CheckBoxes.BeepCheckBoxBool Meta_IncludeViewsbeepCheckBox;
        private TheTechIdea.Beep.Winform.Controls.CheckBoxes.BeepCheckBoxBool Meta_IncludeSystemObjectsbeepCheckBox;
    }
}

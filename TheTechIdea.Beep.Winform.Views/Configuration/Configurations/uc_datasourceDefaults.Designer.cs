
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Winform.Controls.BindingNavigator;

namespace Beep.Config.Winform.Configurations
{
    partial class uc_datasourceDefaults
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
            this.components = new System.ComponentModel.Container();
            this.datasourceDefaults1DataGridView = new ReaLTaiizor.Controls.PoisonDataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RuleComboBox = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ValueTextBox = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TypedataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.datasourceDefaultsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.BeepbindingNavigator1 = new BeepbindingNavigator();
            this.poisonPanel1 = new ReaLTaiizor.Controls.PoisonPanel();
            this.poisonLabel1 = new ReaLTaiizor.Controls.PoisonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.datasourceDefaults1DataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.datasourceDefaultsBindingSource)).BeginInit();
            this.poisonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // datasourceDefaults1DataGridView
            // 
            this.datasourceDefaults1DataGridView.AutoGenerateColumns = false;
            this.datasourceDefaults1DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datasourceDefaults1DataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.RuleComboBox,
            this.ValueTextBox,
            this.TypedataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
            this.datasourceDefaults1DataGridView.DataSource = this.datasourceDefaultsBindingSource;
            this.datasourceDefaults1DataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.datasourceDefaults1DataGridView.Location = new System.Drawing.Point(0, 58);
            this.datasourceDefaults1DataGridView.Name = "datasourceDefaults1DataGridView";
            this.datasourceDefaults1DataGridView.Size = new System.Drawing.Size(703, 638);
            this.datasourceDefaults1DataGridView.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.DataPropertyName = "propertyName";
            this.dataGridViewTextBoxColumn1.HeaderText = "Property Name";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // RuleComboBox
            // 
            this.RuleComboBox.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.RuleComboBox.DataPropertyName = "Rule";
            this.RuleComboBox.HeaderText = "Rule";
            this.RuleComboBox.Name = "RuleComboBox";
            this.RuleComboBox.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.RuleComboBox.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.RuleComboBox.Width = 54;
            // 
            // ValueTextBox
            // 
            this.ValueTextBox.DataPropertyName = "propoertValue";
            this.ValueTextBox.HeaderText = "Static Default Value";
            this.ValueTextBox.Name = "ValueTextBox";
            this.ValueTextBox.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // TypedataGridViewTextBoxColumn3
            // 
            this.TypedataGridViewTextBoxColumn3.DataPropertyName = "propertyType";
            this.TypedataGridViewTextBoxColumn3.HeaderText = "Type";
            this.TypedataGridViewTextBoxColumn3.Name = "TypedataGridViewTextBoxColumn3";
            this.TypedataGridViewTextBoxColumn3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.TypedataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "propertyCategory";
            this.dataGridViewTextBoxColumn4.HeaderText = "Category";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            // 
            // datasourceDefaultsBindingSource
            // 
            this.datasourceDefaultsBindingSource.DataSource = typeof(DefaultValue);
            // 
            // BeepbindingNavigator1
            // 
            this.BeepbindingNavigator1.AddinName = null;
            this.BeepbindingNavigator1.BackColor = System.Drawing.Color.White;
            this.BeepbindingNavigator1.bindingSource = null;
            this.BeepbindingNavigator1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BeepbindingNavigator1.ButtonBorderSize = 0;
            this.BeepbindingNavigator1.CausesValidation = false;
            this.BeepbindingNavigator1.ControlHeight = 30;
            this.BeepbindingNavigator1.DefaultCreate = false;
            this.BeepbindingNavigator1.Description = null;
            this.BeepbindingNavigator1.DllName = null;
            this.BeepbindingNavigator1.DllPath = null;
            this.BeepbindingNavigator1.DMEEditor = null;
            this.BeepbindingNavigator1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BeepbindingNavigator1.EntityName = null;
            this.BeepbindingNavigator1.EntityStructure = null;
            this.BeepbindingNavigator1.ErrorObject = null;
            this.BeepbindingNavigator1.HightlightColor = System.Drawing.Color.Red;
            this.BeepbindingNavigator1.Location = new System.Drawing.Point(0, 696);
            this.BeepbindingNavigator1.Logger = null;
            this.BeepbindingNavigator1.Name = "BeepbindingNavigator1";
            this.BeepbindingNavigator1.NameSpace = null;
            this.BeepbindingNavigator1.ObjectName = null;
            this.BeepbindingNavigator1.ObjectType = null;
            this.BeepbindingNavigator1.ParentName = null;
            this.BeepbindingNavigator1.Passedarg = null;
            this.BeepbindingNavigator1.SelectedColor = System.Drawing.Color.Green;
            this.BeepbindingNavigator1.Size = new System.Drawing.Size(703, 31);
            this.BeepbindingNavigator1.TabIndex = 2;
            this.BeepbindingNavigator1.VerifyDelete = true;
            // 
            // poisonPanel1
            // 
            this.poisonPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.poisonPanel1.Controls.Add(this.poisonLabel1);
            this.poisonPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.poisonPanel1.HorizontalScrollbarBarColor = true;
            this.poisonPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.poisonPanel1.HorizontalScrollbarSize = 10;
            this.poisonPanel1.Location = new System.Drawing.Point(0, 0);
            this.poisonPanel1.Name = "poisonPanel1";
            this.poisonPanel1.Size = new System.Drawing.Size(703, 58);
            this.poisonPanel1.TabIndex = 6;
            this.poisonPanel1.UseStyleColors = true;
            this.poisonPanel1.VerticalScrollbarBarColor = true;
            this.poisonPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.poisonPanel1.VerticalScrollbarSize = 10;
            // 
            // poisonLabel1
            // 
            this.poisonLabel1.AutoSize = true;
            this.poisonLabel1.FontSize = ReaLTaiizor.Extension.Poison.PoisonLabelSize.Tall;
            this.poisonLabel1.FontWeight = ReaLTaiizor.Extension.Poison.PoisonLabelWeight.Bold;
            this.poisonLabel1.Location = new System.Drawing.Point(26, 13);
            this.poisonLabel1.Name = "poisonLabel1";
            this.poisonLabel1.Size = new System.Drawing.Size(267, 25);
            this.poisonLabel1.TabIndex = 3;
            this.poisonLabel1.Text = "DataSource Defaults Manager";
            this.poisonLabel1.UseStyleColors = true;
            // 
            // uc_datasourceDefaults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.datasourceDefaults1DataGridView);
            this.Controls.Add(this.poisonPanel1);
            this.Controls.Add(this.BeepbindingNavigator1);
            this.Name = "uc_datasourceDefaults";
            this.Size = new System.Drawing.Size(703, 727);
            ((System.ComponentModel.ISupportInitialize)(this.datasourceDefaults1DataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.datasourceDefaultsBindingSource)).EndInit();
            this.poisonPanel1.ResumeLayout(false);
            this.poisonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource datasourceDefaultsBindingSource;
        private ReaLTaiizor.Controls.PoisonDataGridView datasourceDefaults1DataGridView;
        private BeepbindingNavigator BeepbindingNavigator1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewComboBoxColumn RuleComboBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn ValueTextBox;
        private System.Windows.Forms.DataGridViewComboBoxColumn TypedataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private ReaLTaiizor.Controls.PoisonPanel poisonPanel1;
        private ReaLTaiizor.Controls.PoisonLabel poisonLabel1;
    }
}

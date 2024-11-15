
 
using TheTechIdea.Beep.Winform.Controls.BindingNavigator;

namespace TheTechIdea.Beep.Winform.Views
{
    partial class uc_objectTypes
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
            this.objectTypesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.objectTypesDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ObjectTypeinGrid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BeepbindingNavigator1 = new BeepbindingNavigator();
            ((System.ComponentModel.ISupportInitialize)(this.objectTypesBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectTypesDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // objectTypesBindingSource
            // 
            this.objectTypesBindingSource.DataSource = typeof(TheTechIdea.Beep.Workflow.ObjectTypes);
            // 
            // objectTypesDataGridView
            // 
            this.objectTypesDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.objectTypesDataGridView.AutoGenerateColumns = false;
            this.objectTypesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.objectTypesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.ObjectTypeinGrid});
            this.objectTypesDataGridView.DataSource = this.objectTypesBindingSource;
            this.objectTypesDataGridView.Location = new System.Drawing.Point(32, 25);
            this.objectTypesDataGridView.Name = "objectTypesDataGridView";
            this.objectTypesDataGridView.Size = new System.Drawing.Size(566, 611);
            this.objectTypesDataGridView.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.DataPropertyName = "ObjectName";
            this.dataGridViewTextBoxColumn1.HeaderText = "Object Name";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // ObjectTypeinGrid
            // 
            this.ObjectTypeinGrid.DataPropertyName = "ObjectType";
            this.ObjectTypeinGrid.HeaderText = "Object Type";
            this.ObjectTypeinGrid.Name = "ObjectTypeinGrid";
            // 
            // BeepbindingNavigator1
            // 
            this.BeepbindingNavigator1.AddinName = null;
            this.BeepbindingNavigator1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.BeepbindingNavigator1.EntityName = null;
            this.BeepbindingNavigator1.EntityStructure = null;
            this.BeepbindingNavigator1.ErrorObject = null;
            this.BeepbindingNavigator1.HightlightColor = System.Drawing.Color.Red;
            this.BeepbindingNavigator1.Location = new System.Drawing.Point(32, 636);
            this.BeepbindingNavigator1.Logger = null;
            this.BeepbindingNavigator1.Name = "BeepbindingNavigator1";
            this.BeepbindingNavigator1.NameSpace = null;
            this.BeepbindingNavigator1.ObjectName = null;
            this.BeepbindingNavigator1.ObjectType = null;
            this.BeepbindingNavigator1.ParentName = null;
            this.BeepbindingNavigator1.Passedarg = null;
            this.BeepbindingNavigator1.SelectedColor = System.Drawing.Color.Green;
            this.BeepbindingNavigator1.Size = new System.Drawing.Size(566, 31);
            this.BeepbindingNavigator1.TabIndex = 2;
            this.BeepbindingNavigator1.VerifyDelete = true;
            // 
            // uc_objectTypes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.BeepbindingNavigator1);
            this.Controls.Add(this.objectTypesDataGridView);
            this.Name = "uc_objectTypes";
            this.Size = new System.Drawing.Size(621, 675);
            ((System.ComponentModel.ISupportInitialize)(this.objectTypesBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectTypesDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource objectTypesBindingSource;
        private System.Windows.Forms.DataGridView objectTypesDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ObjectTypeinGrid;
        private BeepbindingNavigator BeepbindingNavigator1;
    }
}

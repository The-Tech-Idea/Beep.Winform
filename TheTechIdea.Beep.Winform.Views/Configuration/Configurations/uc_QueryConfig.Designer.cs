

using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Winform.Controls.BindingNavigator;

namespace Beep.Config.Winform.Configurations
{
    partial class uc_QueryConfig
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.queryListBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.DatabaseTypebindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.queryListDataGridView = new ReaLTaiizor.Controls.PoisonDataGridView();
            this.DatabasetypeComboBox = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.SQLTypeComboBox = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BeepbindingNavigator1 = new BeepbindingNavigator();
            this.poisonPanel1 = new ReaLTaiizor.Controls.PoisonPanel();
            this.poisonLabel1 = new ReaLTaiizor.Controls.PoisonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.queryListBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DatabaseTypebindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.queryListDataGridView)).BeginInit();
            this.poisonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // queryListBindingSource
            // 
            this.queryListBindingSource.DataSource = typeof(QuerySqlRepo);
            // 
            // DatabaseTypebindingSource
            // 
            this.DatabaseTypebindingSource.DataSource = typeof(QuerySqlRepo);
            // 
            // queryListDataGridView
            // 
            this.queryListDataGridView.AllowUserToResizeRows = false;
            this.queryListDataGridView.AutoGenerateColumns = false;
            this.queryListDataGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.queryListDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.queryListDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.queryListDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.queryListDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.queryListDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.queryListDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DatabasetypeComboBox,
            this.SQLTypeComboBox,
            this.dataGridViewTextBoxColumn3});
            this.queryListDataGridView.DataSource = this.queryListBindingSource;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.queryListDataGridView.DefaultCellStyle = dataGridViewCellStyle3;
            this.queryListDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queryListDataGridView.EnableHeadersVisualStyles = false;
            this.queryListDataGridView.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.queryListDataGridView.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.queryListDataGridView.Location = new System.Drawing.Point(0, 58);
            this.queryListDataGridView.Name = "queryListDataGridView";
            this.queryListDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.queryListDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.queryListDataGridView.RowHeadersWidth = 62;
            this.queryListDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.queryListDataGridView.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.queryListDataGridView.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.queryListDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.queryListDataGridView.Size = new System.Drawing.Size(1025, 545);
            this.queryListDataGridView.TabIndex = 2;
            // 
            // DatabasetypeComboBox
            // 
            this.DatabasetypeComboBox.DataPropertyName = "DatabaseType";
            this.DatabasetypeComboBox.HeaderText = "DatabaseType";
            this.DatabasetypeComboBox.MinimumWidth = 8;
            this.DatabasetypeComboBox.Name = "DatabasetypeComboBox";
            this.DatabasetypeComboBox.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.DatabasetypeComboBox.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.DatabasetypeComboBox.Width = 150;
            // 
            // SQLTypeComboBox
            // 
            this.SQLTypeComboBox.DataPropertyName = "Sqltype";
            this.SQLTypeComboBox.HeaderText = "Sqltype";
            this.SQLTypeComboBox.MinimumWidth = 8;
            this.SQLTypeComboBox.Name = "SQLTypeComboBox";
            this.SQLTypeComboBox.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SQLTypeComboBox.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.SQLTypeComboBox.Width = 150;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn3.DataPropertyName = "Sql";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewTextBoxColumn3.HeaderText = "Sql";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
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
            this.BeepbindingNavigator1.Location = new System.Drawing.Point(0, 603);
            this.BeepbindingNavigator1.Logger = null;
            this.BeepbindingNavigator1.Name = "BeepbindingNavigator1";
            this.BeepbindingNavigator1.NameSpace = null;
            this.BeepbindingNavigator1.ObjectName = null;
            this.BeepbindingNavigator1.ObjectType = null;
            this.BeepbindingNavigator1.ParentName = null;
            this.BeepbindingNavigator1.Passedarg = null;
            this.BeepbindingNavigator1.SelectedColor = System.Drawing.Color.Green;
            this.BeepbindingNavigator1.Size = new System.Drawing.Size(1025, 31);
            this.BeepbindingNavigator1.TabIndex = 3;
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
            this.poisonPanel1.Size = new System.Drawing.Size(1025, 58);
            this.poisonPanel1.TabIndex = 8;
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
            this.poisonLabel1.Size = new System.Drawing.Size(269, 25);
            this.poisonLabel1.TabIndex = 3;
            this.poisonLabel1.Text = "Query Configuration Manager";
            this.poisonLabel1.UseStyleColors = true;
            // 
            // uc_QueryConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.queryListDataGridView);
            this.Controls.Add(this.poisonPanel1);
            this.Controls.Add(this.BeepbindingNavigator1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "uc_QueryConfig";
            this.Size = new System.Drawing.Size(1025, 634);
            ((System.ComponentModel.ISupportInitialize)(this.queryListBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DatabaseTypebindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.queryListDataGridView)).EndInit();
            this.poisonPanel1.ResumeLayout(false);
            this.poisonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource queryListBindingSource;
        private System.Windows.Forms.BindingSource DatabaseTypebindingSource;
        private ReaLTaiizor.Controls.PoisonDataGridView queryListDataGridView;
        private System.Windows.Forms.DataGridViewComboBoxColumn DatabasetypeComboBox;
        private System.Windows.Forms.DataGridViewComboBoxColumn SQLTypeComboBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private BeepbindingNavigator BeepbindingNavigator1;
        private ReaLTaiizor.Controls.PoisonPanel poisonPanel1;
        private ReaLTaiizor.Controls.PoisonLabel poisonLabel1;
    }
}

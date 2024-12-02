
using TheTechIdea.Beep.DriversConfigurations;
using TheTechIdea.Beep.Winform.Controls.BindingNavigator;

namespace Beep.Config.Winform.Configurations
{
    partial class uc_ConnectionDrivers
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.connectiondriversConfigDataGridView = new ReaLTaiizor.Controls.PoisonDataGridView();
            this.iconname = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DatasourceCategoryComboBox = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.DatasourceTypeComboBox = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.classHandlerComboBox = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.InMemory = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.CreateLocal = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ADOType = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Favourite = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.extensionstoHandle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ConnectionString = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.connectiondriversConfigBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BeepbindingNavigator1 = new BeepbindingNavigator();
            this.poisonStyleManager1 = new ReaLTaiizor.Manager.PoisonStyleManager(this.components);
            this.poisonPanel1 = new ReaLTaiizor.Controls.PoisonPanel();
            this.poisonLabel1 = new ReaLTaiizor.Controls.PoisonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.connectiondriversConfigDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.connectiondriversConfigBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.poisonStyleManager1)).BeginInit();
            this.poisonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // connectiondriversConfigDataGridView
            // 
            this.connectiondriversConfigDataGridView.AllowUserToResizeRows = false;
            this.connectiondriversConfigDataGridView.AutoGenerateColumns = false;
            this.connectiondriversConfigDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.connectiondriversConfigDataGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.connectiondriversConfigDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.connectiondriversConfigDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.connectiondriversConfigDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.connectiondriversConfigDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.connectiondriversConfigDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.connectiondriversConfigDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iconname,
            this.dataGridViewTextBoxColumn1,
            this.DatasourceCategoryComboBox,
            this.DatasourceTypeComboBox,
            this.classHandlerComboBox,
            this.InMemory,
            this.CreateLocal,
            this.ADOType,
            this.Favourite,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.extensionstoHandle,
            this.ConnectionString});
            this.connectiondriversConfigDataGridView.DataSource = this.connectiondriversConfigBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.connectiondriversConfigDataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.connectiondriversConfigDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connectiondriversConfigDataGridView.EnableHeadersVisualStyles = false;
            this.connectiondriversConfigDataGridView.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.connectiondriversConfigDataGridView.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.connectiondriversConfigDataGridView.Location = new System.Drawing.Point(0, 58);
            this.connectiondriversConfigDataGridView.Margin = new System.Windows.Forms.Padding(2);
            this.connectiondriversConfigDataGridView.Name = "connectiondriversConfigDataGridView";
            this.connectiondriversConfigDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.connectiondriversConfigDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.connectiondriversConfigDataGridView.RowHeadersWidth = 62;
            this.connectiondriversConfigDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.connectiondriversConfigDataGridView.RowTemplate.Height = 28;
            this.connectiondriversConfigDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.connectiondriversConfigDataGridView.Size = new System.Drawing.Size(1335, 566);
            this.connectiondriversConfigDataGridView.TabIndex = 1;
            // 
            // iconname
            // 
            this.iconname.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.iconname.DataPropertyName = "iconname";
            this.iconname.HeaderText = "Icon";
            this.iconname.MinimumWidth = 8;
            this.iconname.Name = "iconname";
            this.iconname.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.iconname.Width = 42;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dataGridViewTextBoxColumn1.DataPropertyName = "PackageName";
            this.dataGridViewTextBoxColumn1.HeaderText = "Package Name";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 127;
            // 
            // DatasourceCategoryComboBox
            // 
            this.DatasourceCategoryComboBox.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.DatasourceCategoryComboBox.DataPropertyName = "DatasourceCategory";
            this.DatasourceCategoryComboBox.HeaderText = "Category";
            this.DatasourceCategoryComboBox.Name = "DatasourceCategoryComboBox";
            this.DatasourceCategoryComboBox.Width = 77;
            // 
            // DatasourceTypeComboBox
            // 
            this.DatasourceTypeComboBox.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.DatasourceTypeComboBox.DataPropertyName = "DatasourceType";
            this.DatasourceTypeComboBox.HeaderText = "Type";
            this.DatasourceTypeComboBox.Name = "DatasourceTypeComboBox";
            this.DatasourceTypeComboBox.Width = 48;
            // 
            // classHandlerComboBox
            // 
            this.classHandlerComboBox.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.classHandlerComboBox.DataPropertyName = "classHandler";
            this.classHandlerComboBox.HeaderText = "Class Handler";
            this.classHandlerComboBox.MinimumWidth = 8;
            this.classHandlerComboBox.Name = "classHandlerComboBox";
            this.classHandlerComboBox.Width = 102;
            // 
            // InMemory
            // 
            this.InMemory.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.InMemory.DataPropertyName = "InMemory";
            this.InMemory.HeaderText = "InMemory";
            this.InMemory.Name = "InMemory";
            this.InMemory.Width = 81;
            // 
            // CreateLocal
            // 
            this.CreateLocal.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.CreateLocal.DataPropertyName = "CreateLocal";
            this.CreateLocal.HeaderText = "Local";
            this.CreateLocal.MinimumWidth = 8;
            this.CreateLocal.Name = "CreateLocal";
            this.CreateLocal.Width = 51;
            // 
            // ADOType
            // 
            this.ADOType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ADOType.DataPropertyName = "ADOType";
            this.ADOType.HeaderText = "ADO Type";
            this.ADOType.MinimumWidth = 8;
            this.ADOType.Name = "ADOType";
            this.ADOType.Width = 77;
            // 
            // Favourite
            // 
            this.Favourite.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Favourite.DataPropertyName = "Favourite";
            this.Favourite.HeaderText = "Fav";
            this.Favourite.Name = "Favourite";
            this.Favourite.Width = 38;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dataGridViewTextBoxColumn2.DataPropertyName = "DriverClass";
            this.dataGridViewTextBoxColumn2.HeaderText = "Driver Class";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 109;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dataGridViewTextBoxColumn3.DataPropertyName = "version";
            this.dataGridViewTextBoxColumn3.HeaderText = "Version";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Width = 86;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dataGridViewTextBoxColumn4.DataPropertyName = "dllname";
            this.dataGridViewTextBoxColumn4.HeaderText = "DLL";
            this.dataGridViewTextBoxColumn4.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Width = 60;
            // 
            // extensionstoHandle
            // 
            this.extensionstoHandle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.extensionstoHandle.DataPropertyName = "extensionstoHandle";
            this.extensionstoHandle.HeaderText = "Extensions to Handle";
            this.extensionstoHandle.Name = "extensionstoHandle";
            this.extensionstoHandle.Width = 122;
            // 
            // ConnectionString
            // 
            this.ConnectionString.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ConnectionString.DataPropertyName = "ConnectionString";
            this.ConnectionString.HeaderText = "Connection String";
            this.ConnectionString.MinimumWidth = 8;
            this.ConnectionString.Name = "ConnectionString";
            // 
            // connectiondriversConfigBindingSource
            // 
            this.connectiondriversConfigBindingSource.DataSource = typeof(ConnectionDriversConfig);
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn5.DataPropertyName = "ConnectionString";
            this.dataGridViewTextBoxColumn5.HeaderText = "Connection String";
            this.dataGridViewTextBoxColumn5.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn6.DataPropertyName = "ConnectionString";
            this.dataGridViewTextBoxColumn6.HeaderText = "Connection String";
            this.dataGridViewTextBoxColumn6.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
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
            this.BeepbindingNavigator1.Location = new System.Drawing.Point(0, 624);
            this.BeepbindingNavigator1.Logger = null;
            this.BeepbindingNavigator1.Name = "BeepbindingNavigator1";
            this.BeepbindingNavigator1.NameSpace = null;
            this.BeepbindingNavigator1.ObjectName = null;
            this.BeepbindingNavigator1.ObjectType = null;
            this.BeepbindingNavigator1.ParentName = null;
            this.BeepbindingNavigator1.Passedarg = null;
            this.BeepbindingNavigator1.SelectedColor = System.Drawing.Color.Green;
            this.BeepbindingNavigator1.Size = new System.Drawing.Size(1335, 31);
            this.BeepbindingNavigator1.TabIndex = 2;
            this.BeepbindingNavigator1.VerifyDelete = true;
            // 
            // poisonStyleManager1
            // 
            this.poisonStyleManager1.Owner = null;
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
            this.poisonPanel1.Size = new System.Drawing.Size(1335, 58);
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
            this.poisonLabel1.Location = new System.Drawing.Point(30, 13);
            this.poisonLabel1.Name = "poisonLabel1";
            this.poisonLabel1.Size = new System.Drawing.Size(377, 25);
            this.poisonLabel1.TabIndex = 3;
            this.poisonLabel1.Text = "Datasource Drivers Configuration Manager";
            this.poisonLabel1.UseStyleColors = true;
            // 
            // uc_ConnectionDrivers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.connectiondriversConfigDataGridView);
            this.Controls.Add(this.poisonPanel1);
            this.Controls.Add(this.BeepbindingNavigator1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "uc_ConnectionDrivers";
            this.Size = new System.Drawing.Size(1335, 655);
            ((System.ComponentModel.ISupportInitialize)(this.connectiondriversConfigDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.connectiondriversConfigBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.poisonStyleManager1)).EndInit();
            this.poisonPanel1.ResumeLayout(false);
            this.poisonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource connectiondriversConfigBindingSource;
        private ReaLTaiizor.Controls.PoisonDataGridView connectiondriversConfigDataGridView;
        private BeepbindingNavigator BeepbindingNavigator1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewComboBoxColumn iconname;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewComboBoxColumn DatasourceCategoryComboBox;
        private System.Windows.Forms.DataGridViewComboBoxColumn DatasourceTypeComboBox;
        private System.Windows.Forms.DataGridViewComboBoxColumn classHandlerComboBox;
        private System.Windows.Forms.DataGridViewCheckBoxColumn InMemory;
        private System.Windows.Forms.DataGridViewCheckBoxColumn CreateLocal;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ADOType;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Favourite;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn extensionstoHandle;
        private System.Windows.Forms.DataGridViewTextBoxColumn ConnectionString;
        private ReaLTaiizor.Manager.PoisonStyleManager poisonStyleManager1;
        private ReaLTaiizor.Controls.PoisonPanel poisonPanel1;
        private ReaLTaiizor.Controls.PoisonLabel poisonLabel1;
    }
}

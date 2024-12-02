
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Winform.Controls.BindingNavigator;

namespace Beep.Config.Winform.Configurations
{
    partial class uc_FolderConfigurationControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.addinFoldersDataGridView = new ReaLTaiizor.Controls.PoisonDataGridView();
            this.folderPathDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.folderFilesTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.foldersBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.BeepbindingNavigator1 = new BeepbindingNavigator();
            this.poisonPanel1 = new ReaLTaiizor.Controls.PoisonPanel();
            this.poisonLabel1 = new ReaLTaiizor.Controls.PoisonLabel();
            this.poisonStyleManager1 = new ReaLTaiizor.Manager.PoisonStyleManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.addinFoldersDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.foldersBindingSource)).BeginInit();
            this.poisonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.poisonStyleManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // addinFoldersDataGridView
            // 
            this.addinFoldersDataGridView.AllowUserToResizeRows = false;
            this.addinFoldersDataGridView.AutoGenerateColumns = false;
            this.addinFoldersDataGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.addinFoldersDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.addinFoldersDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.addinFoldersDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.addinFoldersDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.addinFoldersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.addinFoldersDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.folderPathDataGridViewTextBoxColumn,
            this.folderFilesTypeDataGridViewTextBoxColumn});
            this.addinFoldersDataGridView.DataSource = this.foldersBindingSource;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.addinFoldersDataGridView.DefaultCellStyle = dataGridViewCellStyle3;
            this.addinFoldersDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addinFoldersDataGridView.EnableHeadersVisualStyles = false;
            this.addinFoldersDataGridView.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.addinFoldersDataGridView.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.addinFoldersDataGridView.Location = new System.Drawing.Point(0, 58);
            this.addinFoldersDataGridView.Margin = new System.Windows.Forms.Padding(2);
            this.addinFoldersDataGridView.Name = "addinFoldersDataGridView";
            this.addinFoldersDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.addinFoldersDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.addinFoldersDataGridView.RowHeadersVisible = false;
            this.addinFoldersDataGridView.RowHeadersWidth = 62;
            this.addinFoldersDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.addinFoldersDataGridView.RowTemplate.Height = 28;
            this.addinFoldersDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.addinFoldersDataGridView.ShowCellErrors = false;
            this.addinFoldersDataGridView.ShowRowErrors = false;
            this.addinFoldersDataGridView.Size = new System.Drawing.Size(999, 567);
            this.addinFoldersDataGridView.TabIndex = 0;
            this.addinFoldersDataGridView.UseStyleColors = true;
            // 
            // folderPathDataGridViewTextBoxColumn
            // 
            this.folderPathDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.folderPathDataGridViewTextBoxColumn.DataPropertyName = "FolderPath";
            this.folderPathDataGridViewTextBoxColumn.HeaderText = "Folder Path";
            this.folderPathDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.folderPathDataGridViewTextBoxColumn.Name = "folderPathDataGridViewTextBoxColumn";
            // 
            // folderFilesTypeDataGridViewTextBoxColumn
            // 
            this.folderFilesTypeDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.folderFilesTypeDataGridViewTextBoxColumn.DataPropertyName = "FolderFilesType";
            this.folderFilesTypeDataGridViewTextBoxColumn.HeaderText = "Folder Type";
            this.folderFilesTypeDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.folderFilesTypeDataGridViewTextBoxColumn.Name = "folderFilesTypeDataGridViewTextBoxColumn";
            this.folderFilesTypeDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.folderFilesTypeDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.folderFilesTypeDataGridViewTextBoxColumn.Width = 89;
            // 
            // foldersBindingSource
            // 
            this.foldersBindingSource.DataSource = typeof(StorageFolders);
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
            this.BeepbindingNavigator1.Location = new System.Drawing.Point(0, 625);
            this.BeepbindingNavigator1.Logger = null;
            this.BeepbindingNavigator1.Name = "BeepbindingNavigator1";
            this.BeepbindingNavigator1.NameSpace = null;
            this.BeepbindingNavigator1.ObjectName = null;
            this.BeepbindingNavigator1.ObjectType = null;
            this.BeepbindingNavigator1.ParentName = null;
            this.BeepbindingNavigator1.Passedarg = null;
            this.BeepbindingNavigator1.SelectedColor = System.Drawing.Color.Green;
            this.BeepbindingNavigator1.Size = new System.Drawing.Size(999, 31);
            this.BeepbindingNavigator1.TabIndex = 1;
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
            this.poisonPanel1.Size = new System.Drawing.Size(999, 58);
            this.poisonPanel1.TabIndex = 7;
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
            this.poisonLabel1.Size = new System.Drawing.Size(154, 25);
            this.poisonLabel1.TabIndex = 3;
            this.poisonLabel1.Text = "Folders Manager";
            this.poisonLabel1.UseStyleColors = true;
            // 
            // poisonStyleManager1
            // 
            this.poisonStyleManager1.Owner = this;
            // 
            // uc_FolderConfigurationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.addinFoldersDataGridView);
            this.Controls.Add(this.poisonPanel1);
            this.Controls.Add(this.BeepbindingNavigator1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "uc_FolderConfigurationControl";
            this.Size = new System.Drawing.Size(999, 656);
            ((System.ComponentModel.ISupportInitialize)(this.addinFoldersDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.foldersBindingSource)).EndInit();
            this.poisonPanel1.ResumeLayout(false);
            this.poisonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.poisonStyleManager1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private ReaLTaiizor.Controls.PoisonDataGridView addinFoldersDataGridView;
        private System.Windows.Forms.BindingSource foldersBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn folderPathDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn folderFilesTypeDataGridViewTextBoxColumn;
        private BeepbindingNavigator BeepbindingNavigator1;
        private ReaLTaiizor.Controls.PoisonPanel poisonPanel1;
        private ReaLTaiizor.Controls.PoisonLabel poisonLabel1;
        private ReaLTaiizor.Manager.PoisonStyleManager poisonStyleManager1;
    }
}

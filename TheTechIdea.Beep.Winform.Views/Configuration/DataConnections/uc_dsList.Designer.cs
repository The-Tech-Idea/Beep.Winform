using TheTechIdea.Beep.ConfigUtil;

namespace Beep.Config.Winform.DataConnections
{
    partial class uc_dsList
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
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            poisonDataGridView1 = new ReaLTaiizor.Controls.PoisonDataGridView();
            connectionNameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            dataConnectionsBindingSource = new BindingSource(components);
            poisonPanel1 = new ReaLTaiizor.Controls.PoisonPanel();
            poisonLabel1 = new ReaLTaiizor.Controls.PoisonLabel();
            DatasourceCategorycomboBox = new ReaLTaiizor.Controls.PoisonComboBox();
            CreatepoisonButton = new ReaLTaiizor.Controls.PoisonButton();
            poisonPanel2 = new ReaLTaiizor.Controls.PoisonPanel();
            poisonPanel3 = new ReaLTaiizor.Controls.PoisonPanel();
            SaveChangespoisonButton = new ReaLTaiizor.Controls.PoisonButton();
            ExitCancelpoisonButton = new ReaLTaiizor.Controls.PoisonButton();
            poisonStyleManager1 = new ReaLTaiizor.Manager.PoisonStyleManager(components);
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)poisonDataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataConnectionsBindingSource).BeginInit();
            poisonPanel1.SuspendLayout();
            poisonPanel2.SuspendLayout();
            poisonPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)poisonStyleManager1).BeginInit();
            SuspendLayout();
            // 
            // poisonDataGridView1
            // 
            poisonDataGridView1.AllowUserToAddRows = false;
            poisonDataGridView1.AllowUserToDeleteRows = false;
            poisonDataGridView1.AllowUserToResizeRows = false;
            poisonDataGridView1.AutoGenerateColumns = false;
            poisonDataGridView1.BackgroundColor = Color.FromArgb(255, 255, 255);
            poisonDataGridView1.BorderStyle = BorderStyle.None;
            poisonDataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.None;
            poisonDataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(0, 0, 0);
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle1.ForeColor = Color.FromArgb(255, 255, 255);
            dataGridViewCellStyle1.SelectionBackColor = Color.FromArgb(25, 25, 25);
            dataGridViewCellStyle1.SelectionForeColor = Color.FromArgb(17, 17, 17);
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            poisonDataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            poisonDataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            poisonDataGridView1.Columns.AddRange(new DataGridViewColumn[] { connectionNameDataGridViewTextBoxColumn });
            poisonDataGridView1.DataSource = dataConnectionsBindingSource;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(255, 255, 255);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Pixel);
            dataGridViewCellStyle2.ForeColor = Color.FromArgb(136, 136, 136);
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(25, 25, 25);
            dataGridViewCellStyle2.SelectionForeColor = Color.FromArgb(17, 17, 17);
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            poisonDataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            poisonDataGridView1.Dock = DockStyle.Fill;
            poisonDataGridView1.EnableHeadersVisualStyles = false;
            poisonDataGridView1.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Pixel);
            poisonDataGridView1.GridColor = Color.FromArgb(255, 255, 255);
            poisonDataGridView1.Location = new Point(0, 0);
            poisonDataGridView1.Margin = new Padding(4, 3, 4, 3);
            poisonDataGridView1.Name = "poisonDataGridView1";
            poisonDataGridView1.ReadOnly = true;
            poisonDataGridView1.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = Color.FromArgb(0, 0, 0);
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(255, 255, 255);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(25, 25, 25);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(17, 17, 17);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            poisonDataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            poisonDataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            poisonDataGridView1.RowTemplate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            poisonDataGridView1.RowTemplate.DefaultCellStyle.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            poisonDataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            poisonDataGridView1.Size = new Size(838, 661);
            poisonDataGridView1.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            poisonDataGridView1.TabIndex = 2;
            poisonDataGridView1.UseStyleColors = true;
            // 
            // connectionNameDataGridViewTextBoxColumn
            // 
            connectionNameDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            connectionNameDataGridViewTextBoxColumn.DataPropertyName = "ConnectionName";
            connectionNameDataGridViewTextBoxColumn.HeaderText = "Connection";
            connectionNameDataGridViewTextBoxColumn.Name = "connectionNameDataGridViewTextBoxColumn";
            connectionNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // dataConnectionsBindingSource
            // 
            dataConnectionsBindingSource.DataSource = typeof(ConnectionProperties);
            // 
            // poisonPanel1
            // 
            poisonPanel1.BorderStyle = BorderStyle.FixedSingle;
            poisonPanel1.Controls.Add(poisonLabel1);
            poisonPanel1.Controls.Add(DatasourceCategorycomboBox);
            poisonPanel1.Controls.Add(CreatepoisonButton);
            poisonPanel1.Dock = DockStyle.Top;
            poisonPanel1.HorizontalScrollbarBarColor = true;
            poisonPanel1.HorizontalScrollbarHighlightOnWheel = false;
            poisonPanel1.HorizontalScrollbarSize = 12;
            poisonPanel1.Location = new Point(0, 0);
            poisonPanel1.Margin = new Padding(4, 3, 4, 3);
            poisonPanel1.Name = "poisonPanel1";
            poisonPanel1.Size = new Size(838, 67);
            poisonPanel1.TabIndex = 3;
            poisonPanel1.UseStyleColors = true;
            poisonPanel1.VerticalScrollbarBarColor = true;
            poisonPanel1.VerticalScrollbarHighlightOnWheel = false;
            poisonPanel1.VerticalScrollbarSize = 12;
            // 
            // poisonLabel1
            // 
            poisonLabel1.AutoSize = true;
            poisonLabel1.FontSize = ReaLTaiizor.Extension.Poison.PoisonLabelSize.Tall;
            poisonLabel1.FontWeight = ReaLTaiizor.Extension.Poison.PoisonLabelWeight.Bold;
            poisonLabel1.Location = new Point(24, 15);
            poisonLabel1.Margin = new Padding(4, 0, 4, 0);
            poisonLabel1.Name = "poisonLabel1";
            poisonLabel1.Size = new Size(243, 25);
            poisonLabel1.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            poisonLabel1.TabIndex = 3;
            poisonLabel1.Text = "Data Connections Manager";
            poisonLabel1.UseStyleColors = true;
            // 
            // DatasourceCategorycomboBox
            // 
            DatasourceCategorycomboBox.FormattingEnabled = true;
            DatasourceCategorycomboBox.ItemHeight = 23;
            DatasourceCategorycomboBox.Location = new Point(443, 15);
            DatasourceCategorycomboBox.Margin = new Padding(4, 3, 4, 3);
            DatasourceCategorycomboBox.Name = "DatasourceCategorycomboBox";
            DatasourceCategorycomboBox.Size = new Size(243, 29);
            DatasourceCategorycomboBox.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            DatasourceCategorycomboBox.TabIndex = 2;
            DatasourceCategorycomboBox.UseSelectable = true;
            DatasourceCategorycomboBox.UseStyleColors = true;
            // 
            // CreatepoisonButton
            // 
            CreatepoisonButton.Location = new Point(694, 17);
            CreatepoisonButton.Margin = new Padding(4, 3, 4, 3);
            CreatepoisonButton.Name = "CreatepoisonButton";
            CreatepoisonButton.Size = new Size(88, 27);
            CreatepoisonButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            CreatepoisonButton.TabIndex = 2;
            CreatepoisonButton.Text = "New";
            CreatepoisonButton.UseSelectable = true;
            CreatepoisonButton.UseStyleColors = true;
            // 
            // poisonPanel2
            // 
            poisonPanel2.Controls.Add(poisonDataGridView1);
            poisonPanel2.Controls.Add(poisonPanel3);
            poisonPanel2.Dock = DockStyle.Fill;
            poisonPanel2.HorizontalScrollbarBarColor = true;
            poisonPanel2.HorizontalScrollbarHighlightOnWheel = false;
            poisonPanel2.HorizontalScrollbarSize = 12;
            poisonPanel2.Location = new Point(0, 67);
            poisonPanel2.Margin = new Padding(4, 3, 4, 3);
            poisonPanel2.Name = "poisonPanel2";
            poisonPanel2.Size = new Size(838, 735);
            poisonPanel2.TabIndex = 4;
            poisonPanel2.VerticalScrollbarBarColor = true;
            poisonPanel2.VerticalScrollbarHighlightOnWheel = false;
            poisonPanel2.VerticalScrollbarSize = 12;
            // 
            // poisonPanel3
            // 
            poisonPanel3.BorderStyle = BorderStyle.FixedSingle;
            poisonPanel3.Controls.Add(SaveChangespoisonButton);
            poisonPanel3.Controls.Add(ExitCancelpoisonButton);
            poisonPanel3.Dock = DockStyle.Bottom;
            poisonPanel3.HorizontalScrollbarBarColor = true;
            poisonPanel3.HorizontalScrollbarHighlightOnWheel = false;
            poisonPanel3.HorizontalScrollbarSize = 12;
            poisonPanel3.Location = new Point(0, 661);
            poisonPanel3.Margin = new Padding(4, 3, 4, 3);
            poisonPanel3.Name = "poisonPanel3";
            poisonPanel3.Size = new Size(838, 74);
            poisonPanel3.TabIndex = 3;
            poisonPanel3.UseStyleColors = true;
            poisonPanel3.VerticalScrollbarBarColor = true;
            poisonPanel3.VerticalScrollbarHighlightOnWheel = false;
            poisonPanel3.VerticalScrollbarSize = 12;
            // 
            // SaveChangespoisonButton
            // 
            SaveChangespoisonButton.Location = new Point(694, 23);
            SaveChangespoisonButton.Margin = new Padding(4, 3, 4, 3);
            SaveChangespoisonButton.Name = "SaveChangespoisonButton";
            SaveChangespoisonButton.Size = new Size(88, 27);
            SaveChangespoisonButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            SaveChangespoisonButton.TabIndex = 4;
            SaveChangespoisonButton.Text = "Save";
            SaveChangespoisonButton.UseSelectable = true;
            SaveChangespoisonButton.UseStyleColors = true;
            // 
            // ExitCancelpoisonButton
            // 
            ExitCancelpoisonButton.Location = new Point(24, 23);
            ExitCancelpoisonButton.Margin = new Padding(4, 3, 4, 3);
            ExitCancelpoisonButton.Name = "ExitCancelpoisonButton";
            ExitCancelpoisonButton.Size = new Size(88, 27);
            ExitCancelpoisonButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Black;
            ExitCancelpoisonButton.TabIndex = 3;
            ExitCancelpoisonButton.Text = "Exit/Cancel";
            ExitCancelpoisonButton.UseSelectable = true;
            ExitCancelpoisonButton.UseStyleColors = true;
            // 
            // poisonStyleManager1
            // 
            poisonStyleManager1.Owner = this;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewTextBoxColumn1.DataPropertyName = "ConnectionName";
            dataGridViewTextBoxColumn1.HeaderText = "Connection";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // uc_dsList
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(poisonPanel2);
            Controls.Add(poisonPanel1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "uc_dsList";
            Size = new Size(838, 802);
            ((System.ComponentModel.ISupportInitialize)poisonDataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataConnectionsBindingSource).EndInit();
            poisonPanel1.ResumeLayout(false);
            poisonPanel1.PerformLayout();
            poisonPanel2.ResumeLayout(false);
            poisonPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)poisonStyleManager1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private BindingSource dataConnectionsBindingSource;
        private ReaLTaiizor.Controls.PoisonDataGridView poisonDataGridView1;
        private ReaLTaiizor.Controls.PoisonPanel poisonPanel1;
        private ReaLTaiizor.Controls.PoisonPanel poisonPanel2;
        private ReaLTaiizor.Controls.PoisonComboBox DatasourceCategorycomboBox;
        private DataGridViewTextBoxColumn connectionNameDataGridViewTextBoxColumn;
        private ReaLTaiizor.Controls.PoisonPanel poisonPanel3;
        private ReaLTaiizor.Manager.PoisonStyleManager poisonStyleManager1;
        private ReaLTaiizor.Controls.PoisonButton ExitCancelpoisonButton;
        private ReaLTaiizor.Controls.PoisonButton CreatepoisonButton;
        private ReaLTaiizor.Controls.PoisonLabel poisonLabel1;
        private ReaLTaiizor.Controls.PoisonButton SaveChangespoisonButton;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    }
}

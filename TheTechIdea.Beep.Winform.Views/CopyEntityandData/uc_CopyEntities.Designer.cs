
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Winform.Controls;

namespace Beep.Winform.Vis.ETL.CopyEntityandData
{
    partial class uc_CopyEntities
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
            panel1 = new Panel();
            scriptDataGridView = new DataGridView();
            scriptTypeComboBox = new DataGridViewTextBoxColumn();
            destinationentityname = new DataGridViewTextBoxColumn();
            Failed = new DataGridViewCheckBoxColumn();
            IsCreated = new DataGridViewCheckBoxColumn();
            IsModified = new DataGridViewCheckBoxColumn();
            sourcedatasourcename = new DataGridViewComboBoxColumn();
            dataConnectionsBindingSource = new BindingSource(components);
            destinationdatasourcename = new DataGridViewComboBoxColumn();
            ddlDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            scriptBindingSource = new BindingSource(components);
            LogtextBox = new TextBox();
            label2 = new Label();
            EntitiesnumericUpDown = new NumericUpDown();
            label5 = new Label();
            ErrorsAllowdnumericUpDown = new NumericUpDown();
            RunMainScripButton = new Button();
            label1 = new Label();
            panel2 = new Panel();
            panel3 = new Panel();
            progressBar1 = new BeepProgressBar();
            StopButton = new Button();
            loadDataLogsBindingSource = new BindingSource(components);
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)scriptDataGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataConnectionsBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)scriptBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)EntitiesnumericUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ErrorsAllowdnumericUpDown).BeginInit();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)loadDataLogsBindingSource).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(scriptDataGridView);
            panel1.Controls.Add(LogtextBox);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(EntitiesnumericUpDown);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(ErrorsAllowdnumericUpDown);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 83);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(1075, 719);
            panel1.TabIndex = 22;
            // 
            // scriptDataGridView
            // 
            scriptDataGridView.AllowUserToDeleteRows = false;
            scriptDataGridView.AllowUserToOrderColumns = true;
            scriptDataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            scriptDataGridView.AutoGenerateColumns = false;
            scriptDataGridView.BackgroundColor = Color.WhiteSmoke;
            scriptDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            scriptDataGridView.Columns.AddRange(new DataGridViewColumn[] { scriptTypeComboBox, destinationentityname, Failed, IsCreated, IsModified, sourcedatasourcename, destinationdatasourcename, ddlDataGridViewTextBoxColumn });
            scriptDataGridView.DataSource = scriptBindingSource;
            scriptDataGridView.Enabled = false;
            scriptDataGridView.Location = new Point(4, 60);
            scriptDataGridView.Margin = new Padding(4, 3, 4, 3);
            scriptDataGridView.MultiSelect = false;
            scriptDataGridView.Name = "scriptDataGridView";
            scriptDataGridView.ShowCellErrors = false;
            scriptDataGridView.Size = new Size(1065, 379);
            scriptDataGridView.TabIndex = 33;
            // 
            // scriptTypeComboBox
            // 
            scriptTypeComboBox.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            scriptTypeComboBox.DataPropertyName = "scriptType";
            scriptTypeComboBox.HeaderText = "Type";
            scriptTypeComboBox.Name = "scriptTypeComboBox";
            scriptTypeComboBox.Resizable = DataGridViewTriState.True;
            scriptTypeComboBox.SortMode = DataGridViewColumnSortMode.NotSortable;
            scriptTypeComboBox.Width = 37;
            // 
            // destinationentityname
            // 
            destinationentityname.DataPropertyName = "destinationentityname";
            destinationentityname.HeaderText = "Entity";
            destinationentityname.Name = "destinationentityname";
            // 
            // Failed
            // 
            Failed.DataPropertyName = "Failed";
            Failed.HeaderText = "Failed";
            Failed.Name = "Failed";
            // 
            // IsCreated
            // 
            IsCreated.DataPropertyName = "IsCreated";
            IsCreated.HeaderText = "Created";
            IsCreated.Name = "IsCreated";
            // 
            // IsModified
            // 
            IsModified.DataPropertyName = "IsModified";
            IsModified.HeaderText = "Modified";
            IsModified.Name = "IsModified";
            // 
            // sourcedatasourcename
            // 
            sourcedatasourcename.DataPropertyName = "sourcedatasourcename";
            sourcedatasourcename.DataSource = dataConnectionsBindingSource;
            sourcedatasourcename.DisplayMember = "ConnectionName";
            sourcedatasourcename.HeaderText = "Source";
            sourcedatasourcename.Name = "sourcedatasourcename";
            sourcedatasourcename.Resizable = DataGridViewTriState.True;
            sourcedatasourcename.SortMode = DataGridViewColumnSortMode.Automatic;
            sourcedatasourcename.ValueMember = "ConnectionName";
            // 
            // dataConnectionsBindingSource
            // 
            dataConnectionsBindingSource.DataSource = typeof(ConnectionProperties);
            // 
            // destinationdatasourcename
            // 
            destinationdatasourcename.DataPropertyName = "destinationdatasourcename";
            destinationdatasourcename.DataSource = dataConnectionsBindingSource;
            destinationdatasourcename.DisplayMember = "ConnectionName";
            destinationdatasourcename.HeaderText = "Destination";
            destinationdatasourcename.Name = "destinationdatasourcename";
            destinationdatasourcename.ValueMember = "ConnectionName";
            // 
            // ddlDataGridViewTextBoxColumn
            // 
            ddlDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ddlDataGridViewTextBoxColumn.DataPropertyName = "ddl";
            ddlDataGridViewTextBoxColumn.HeaderText = "Script";
            ddlDataGridViewTextBoxColumn.Name = "ddlDataGridViewTextBoxColumn";
            // 
            // scriptBindingSource
            // 
            scriptBindingSource.DataSource = typeof(TheTechIdea.Beep.Editor.ETLScriptDet);
            // 
            // LogtextBox
            // 
            LogtextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LogtextBox.BorderStyle = BorderStyle.FixedSingle;
            LogtextBox.Location = new Point(4, 445);
            LogtextBox.Margin = new Padding(4, 3, 4, 3);
            LogtextBox.Multiline = true;
            LogtextBox.Name = "LogtextBox";
            LogtextBox.ScrollBars = ScrollBars.Vertical;
            LogtextBox.Size = new Size(1065, 217);
            LogtextBox.TabIndex = 31;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Myanmar Text", 10F, FontStyle.Bold);
            label2.Location = new Point(202, 20);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(103, 25);
            label2.TabIndex = 31;
            label2.Text = "No.of Scripts:";
            // 
            // EntitiesnumericUpDown
            // 
            EntitiesnumericUpDown.Location = new Point(343, 22);
            EntitiesnumericUpDown.Margin = new Padding(4, 3, 4, 3);
            EntitiesnumericUpDown.Name = "EntitiesnumericUpDown";
            EntitiesnumericUpDown.ReadOnly = true;
            EntitiesnumericUpDown.Size = new Size(140, 23);
            EntitiesnumericUpDown.TabIndex = 32;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Myanmar Text", 10F, FontStyle.Bold);
            label5.Location = new Point(495, 20);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(115, 25);
            label5.TabIndex = 29;
            label5.Text = "Errors Allowed:";
            // 
            // ErrorsAllowdnumericUpDown
            // 
            ErrorsAllowdnumericUpDown.Location = new Point(636, 22);
            ErrorsAllowdnumericUpDown.Margin = new Padding(4, 3, 4, 3);
            ErrorsAllowdnumericUpDown.Name = "ErrorsAllowdnumericUpDown";
            ErrorsAllowdnumericUpDown.Size = new Size(140, 23);
            ErrorsAllowdnumericUpDown.TabIndex = 30;
            ErrorsAllowdnumericUpDown.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // RunMainScripButton
            // 
            RunMainScripButton.Anchor = AnchorStyles.None;
            RunMainScripButton.ForeColor = Color.Black;
            RunMainScripButton.Location = new Point(213, 2);
            RunMainScripButton.Margin = new Padding(4, 3, 4, 3);
            RunMainScripButton.Name = "RunMainScripButton";
            RunMainScripButton.Size = new Size(105, 29);
            RunMainScripButton.TabIndex = 24;
            RunMainScripButton.Text = "Run";
            RunMainScripButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            label1.ForeColor = Color.Gold;
            label1.Location = new Point(0, 0);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(1073, 81);
            label1.TabIndex = 0;
            label1.Text = "Script Runner ";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            panel2.BackColor = Color.SteelBlue;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(label1);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 0);
            panel2.Margin = new Padding(4, 3, 4, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(1075, 83);
            panel2.TabIndex = 26;
            // 
            // panel3
            // 
            panel3.BackColor = Color.SteelBlue;
            panel3.BorderStyle = BorderStyle.FixedSingle;
            panel3.Controls.Add(progressBar1);
            panel3.Controls.Add(RunMainScripButton);
            panel3.Controls.Add(StopButton);
            panel3.Dock = DockStyle.Bottom;
            panel3.Location = new Point(0, 757);
            panel3.Margin = new Padding(4, 3, 4, 3);
            panel3.Name = "panel3";
            panel3.Size = new Size(1075, 45);
            panel3.TabIndex = 27;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = AnchorStyles.None;
            progressBar1.CustomText = "";
            progressBar1.Location = new Point(325, 3);
            progressBar1.Margin = new Padding(4, 3, 4, 3);
            progressBar1.Name = "progressBar1";
            progressBar1.ProgressColor = Color.LightGreen;
            progressBar1.Size = new Size(396, 27);
            progressBar1.TabIndex = 25;
            progressBar1.TextColor = Color.Black;
            progressBar1.TextFont = new Font("Times New Roman", 11F, FontStyle.Bold | FontStyle.Italic);
            progressBar1.VisualMode = ProgressBarDisplayMode.CurrProgress;
            // 
            // StopButton
            // 
            StopButton.Anchor = AnchorStyles.None;
            StopButton.Location = new Point(729, 3);
            StopButton.Margin = new Padding(4, 3, 4, 3);
            StopButton.Name = "StopButton";
            StopButton.Size = new Size(82, 27);
            StopButton.TabIndex = 28;
            StopButton.Text = "Stop";
            StopButton.UseVisualStyleBackColor = true;
            // 
            // loadDataLogsBindingSource
            // 
            loadDataLogsBindingSource.DataSource = typeof(TheTechIdea.Beep.Workflow.LoadDataLogResult);
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewTextBoxColumn1.DataPropertyName = "scriptType";
            dataGridViewTextBoxColumn1.HeaderText = "scriptType";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            dataGridViewTextBoxColumn1.Resizable = DataGridViewTriState.True;
            dataGridViewTextBoxColumn1.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewTextBoxColumn2.DataPropertyName = "scriptType";
            dataGridViewTextBoxColumn2.HeaderText = "scriptType";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.ReadOnly = true;
            dataGridViewTextBoxColumn2.Resizable = DataGridViewTriState.True;
            dataGridViewTextBoxColumn2.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewTextBoxColumn3.DataPropertyName = "errormessage";
            dataGridViewTextBoxColumn3.HeaderText = "Messege";
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // uc_CopyEntities
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel3);
            Controls.Add(panel1);
            Controls.Add(panel2);
            Name = "uc_CopyEntities";
            Size = new Size(1075, 802);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)scriptDataGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataConnectionsBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)scriptBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)EntitiesnumericUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)ErrorsAllowdnumericUpDown).EndInit();
            panel2.ResumeLayout(false);
            panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)loadDataLogsBindingSource).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.BindingSource scriptBindingSource;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button RunMainScripButton;
        private BeepProgressBar progressBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.BindingSource loadDataLogsBindingSource;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown ErrorsAllowdnumericUpDown;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.TextBox LogtextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown EntitiesnumericUpDown;
        private System.Windows.Forms.BindingSource dataConnectionsBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridView scriptDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn scriptTypeComboBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn destinationentityname;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Failed;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsCreated;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsModified;
        private System.Windows.Forms.DataGridViewComboBoxColumn sourcedatasourcename;
        private System.Windows.Forms.DataGridViewComboBoxColumn destinationdatasourcename;
        private System.Windows.Forms.DataGridViewTextBoxColumn ddlDataGridViewTextBoxColumn;
    }
}

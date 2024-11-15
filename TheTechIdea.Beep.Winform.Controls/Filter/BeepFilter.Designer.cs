namespace TheTechIdea.Beep.Winform.Controls.Filter
{
    partial class BeepFilter
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            dataGridView1 = new DataGridView();
            iDDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            guidIDDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            fieldNameDataGridViewTextBoxColumn = new DataGridViewComboBoxColumn();
            operatorDataGridViewTextBoxColumn = new DataGridViewComboBoxColumn();
            filterValueDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            valueTypeDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            filterValue1DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            filtersBindingSource = new BindingSource(components);
            panel1 = new Panel();
            Cancelbutton = new Button();
            Savebutton = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)filtersBindingSource).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { iDDataGridViewTextBoxColumn, guidIDDataGridViewTextBoxColumn, fieldNameDataGridViewTextBoxColumn, operatorDataGridViewTextBoxColumn, filterValueDataGridViewTextBoxColumn, valueTypeDataGridViewTextBoxColumn, filterValue1DataGridViewTextBoxColumn });
            dataGridView1.DataSource = filtersBindingSource;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.GridColor = Color.White;
            dataGridView1.Location = new Point(0, 0);
            dataGridView1.Margin = new Padding(4, 3, 4, 3);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.Size = new Size(550, 494);
            dataGridView1.TabIndex = 2;
            // 
            // iDDataGridViewTextBoxColumn
            // 
            iDDataGridViewTextBoxColumn.DataPropertyName = "ID";
            iDDataGridViewTextBoxColumn.HeaderText = "ID";
            iDDataGridViewTextBoxColumn.Name = "iDDataGridViewTextBoxColumn";
            iDDataGridViewTextBoxColumn.Visible = false;
            // 
            // guidIDDataGridViewTextBoxColumn
            // 
            guidIDDataGridViewTextBoxColumn.DataPropertyName = "GuidID";
            guidIDDataGridViewTextBoxColumn.HeaderText = "GuidID";
            guidIDDataGridViewTextBoxColumn.Name = "guidIDDataGridViewTextBoxColumn";
            guidIDDataGridViewTextBoxColumn.Visible = false;
            // 
            // fieldNameDataGridViewTextBoxColumn
            // 
            fieldNameDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            fieldNameDataGridViewTextBoxColumn.DataPropertyName = "FieldName";
            fieldNameDataGridViewTextBoxColumn.HeaderText = "Field";
            fieldNameDataGridViewTextBoxColumn.Name = "fieldNameDataGridViewTextBoxColumn";
            fieldNameDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            fieldNameDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // operatorDataGridViewTextBoxColumn
            // 
            operatorDataGridViewTextBoxColumn.DataPropertyName = "Operator";
            operatorDataGridViewTextBoxColumn.HeaderText = "Operator";
            operatorDataGridViewTextBoxColumn.Items.AddRange(new object[] { "=", "<>", ">", "<", ">=", "<=" });
            operatorDataGridViewTextBoxColumn.Name = "operatorDataGridViewTextBoxColumn";
            operatorDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            operatorDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // filterValueDataGridViewTextBoxColumn
            // 
            filterValueDataGridViewTextBoxColumn.DataPropertyName = "FilterValue";
            filterValueDataGridViewTextBoxColumn.HeaderText = "Value1";
            filterValueDataGridViewTextBoxColumn.Name = "filterValueDataGridViewTextBoxColumn";
            // 
            // valueTypeDataGridViewTextBoxColumn
            // 
            valueTypeDataGridViewTextBoxColumn.DataPropertyName = "valueType";
            valueTypeDataGridViewTextBoxColumn.HeaderText = "valueType";
            valueTypeDataGridViewTextBoxColumn.Name = "valueTypeDataGridViewTextBoxColumn";
            valueTypeDataGridViewTextBoxColumn.Visible = false;
            // 
            // filterValue1DataGridViewTextBoxColumn
            // 
            filterValue1DataGridViewTextBoxColumn.DataPropertyName = "FilterValue1";
            filterValue1DataGridViewTextBoxColumn.HeaderText = "Value2";
            filterValue1DataGridViewTextBoxColumn.Name = "filterValue1DataGridViewTextBoxColumn";
            // 
            // filtersBindingSource
            // 
            filtersBindingSource.DataSource = typeof(Report.AppFilter);
            // 
            // panel1
            // 
            panel1.BackColor = Color.WhiteSmoke;
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(Cancelbutton);
            panel1.Controls.Add(Savebutton);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 494);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(550, 33);
            panel1.TabIndex = 3;
            // 
            // Cancelbutton
            // 
            Cancelbutton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Cancelbutton.BackgroundImage = Properties.Resources._053_trash;
            Cancelbutton.BackgroundImageLayout = ImageLayout.Zoom;
            Cancelbutton.DialogResult = DialogResult.Cancel;
            Cancelbutton.ImageAlign = ContentAlignment.MiddleLeft;
            Cancelbutton.Location = new Point(508, 2);
            Cancelbutton.Margin = new Padding(4, 3, 4, 3);
            Cancelbutton.Name = "Cancelbutton";
            Cancelbutton.Size = new Size(36, 27);
            Cancelbutton.TabIndex = 1;
            Cancelbutton.UseVisualStyleBackColor = true;
            // 
            // Savebutton
            // 
            Savebutton.BackgroundImage = Properties.Resources._054_upload;
            Savebutton.BackgroundImageLayout = ImageLayout.Zoom;
            Savebutton.DialogResult = DialogResult.OK;
            Savebutton.Location = new Point(4, 2);
            Savebutton.Margin = new Padding(4, 3, 4, 3);
            Savebutton.Name = "Savebutton";
            Savebutton.Size = new Size(34, 27);
            Savebutton.TabIndex = 0;
            Savebutton.UseCompatibleTextRendering = true;
            Savebutton.UseVisualStyleBackColor = true;
            // 
            // BeepFilter
            // 
            AcceptButton = Savebutton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = Cancelbutton;
            ClientSize = new Size(550, 527);
            ControlBox = false;
            Controls.Add(dataGridView1);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4, 3, 4, 3);
            Name = "BeepFilter";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Filter";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)filtersBindingSource).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private BindingSource filtersBindingSource;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn guidIDDataGridViewTextBoxColumn;
        private DataGridViewComboBoxColumn fieldNameDataGridViewTextBoxColumn;
        private DataGridViewComboBoxColumn operatorDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn filterValueDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn valueTypeDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn filterValue1DataGridViewTextBoxColumn;
        private Panel panel1;
        private Button Cancelbutton;
        private Button Savebutton;
    }
}
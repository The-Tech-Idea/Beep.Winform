
 
using TheTechIdea.Beep.Winform.Controls.BindingNavigator;

namespace Beep.Config.Winform.DataConnections
{
    partial class uc_webapiQueryParameters
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.entitiesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataGridView1 = new ReaLTaiizor.Controls.PoisonDataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParentId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.KeyToken = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fieldsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.paramentersBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataGridView2 = new ReaLTaiizor.Controls.PoisonDataGridView();
            this.parameterNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parameterIndexDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BeepbindingNavigator1 = new BeepBindingNavigator();
            this.BeepbindingNavigator2 = new BeepBindingNavigator();
            this.poisonPanel2 = new ReaLTaiizor.Controls.PoisonPanel();
            this.poisonLabel1 = new ReaLTaiizor.Controls.PoisonLabel();
            this.poisonPanel1 = new ReaLTaiizor.Controls.PoisonPanel();
            this.poisonPanel3 = new ReaLTaiizor.Controls.PoisonPanel();
            this.poisonPanel4 = new ReaLTaiizor.Controls.PoisonPanel();
            ((System.ComponentModel.ISupportInitialize)(this.entitiesBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fieldsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.paramentersBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.poisonPanel2.SuspendLayout();
            this.poisonPanel1.SuspendLayout();
            this.poisonPanel3.SuspendLayout();
            this.poisonPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // entitiesBindingSource
            // 
            this.entitiesBindingSource.DataSource = typeof(TheTechIdea.Beep.DataBase.EntityStructure);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.ParentId,
            this.dataGridViewTextBoxColumn11,
            this.dataGridViewTextBoxColumn12,
            this.dataGridViewTextBoxColumn13,
            this.KeyToken});
            this.dataGridView1.DataSource = this.entitiesBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.EnableHeadersVisualStyles = false;
            this.dataGridView1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.dataGridView1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(809, 261);
            this.dataGridView1.TabIndex = 46;
            // 
            // GuidId
            // 
            this.Id.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Id.DataPropertyName = "GuidId";
            this.Id.HeaderText = "GuidId";
            this.Id.Name = "GuidId";
            this.Id.Width = 40;
            // 
            // ParentId
            // 
            this.ParentId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.ParentId.DataPropertyName = "ParentId";
            this.ParentId.HeaderText = "ParentId";
            this.ParentId.Name = "ParentId";
            this.ParentId.Width = 73;
            // 
            // dataGridViewTextBoxColumn11
            // 
            this.dataGridViewTextBoxColumn11.DataPropertyName = "EntityName";
            this.dataGridViewTextBoxColumn11.HeaderText = "EntityName";
            this.dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
            // 
            // dataGridViewTextBoxColumn12
            // 
            this.dataGridViewTextBoxColumn12.DataPropertyName = "StatusDescription";
            this.dataGridViewTextBoxColumn12.HeaderText = "Description";
            this.dataGridViewTextBoxColumn12.Name = "dataGridViewTextBoxColumn12";
            // 
            // dataGridViewTextBoxColumn13
            // 
            this.dataGridViewTextBoxColumn13.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn13.DataPropertyName = "CustomBuildQuery";
            this.dataGridViewTextBoxColumn13.HeaderText = "Query";
            this.dataGridViewTextBoxColumn13.Name = "dataGridViewTextBoxColumn13";
            // 
            // KeyToken
            // 
            this.KeyToken.DataPropertyName = "KeyToken";
            this.KeyToken.HeaderText = "Key Token";
            this.KeyToken.Name = "KeyToken";
            // 
            // fieldsBindingSource
            // 
            this.fieldsBindingSource.DataMember = "Fields";
            this.fieldsBindingSource.DataSource = this.entitiesBindingSource;
            // 
            // paramentersBindingSource
            // 
            this.paramentersBindingSource.DataMember = "Paramenters";
            this.paramentersBindingSource.DataSource = this.entitiesBindingSource;
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToResizeRows = false;
            this.dataGridView2.AutoGenerateColumns = false;
            this.dataGridView2.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.dataGridView2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView2.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView2.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView2.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.parameterNameDataGridViewTextBoxColumn,
            this.parameterIndexDataGridViewTextBoxColumn});
            this.dataGridView2.DataSource = this.paramentersBindingSource;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView2.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView2.EnableHeadersVisualStyles = false;
            this.dataGridView2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.dataGridView2.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.dataGridView2.Location = new System.Drawing.Point(0, 0);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView2.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView2.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView2.Size = new System.Drawing.Size(806, 345);
            this.dataGridView2.TabIndex = 49;
            // 
            // parameterNameDataGridViewTextBoxColumn
            // 
            this.parameterNameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.parameterNameDataGridViewTextBoxColumn.DataPropertyName = "parameterName";
            this.parameterNameDataGridViewTextBoxColumn.HeaderText = "Parameter Name";
            this.parameterNameDataGridViewTextBoxColumn.Name = "parameterNameDataGridViewTextBoxColumn";
            // 
            // parameterIndexDataGridViewTextBoxColumn
            // 
            this.parameterIndexDataGridViewTextBoxColumn.DataPropertyName = "parameterIndex";
            this.parameterIndexDataGridViewTextBoxColumn.HeaderText = "Index";
            this.parameterIndexDataGridViewTextBoxColumn.Name = "parameterIndexDataGridViewTextBoxColumn";
            // 
            // BeepbindingNavigator1
            // 
            this.BeepbindingNavigator1.AddinName = null;
            this.BeepbindingNavigator1.BackColor = System.Drawing.Color.White;
            this.BeepbindingNavigator1.BindingSource = null;
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
            this.BeepbindingNavigator1.Location = new System.Drawing.Point(0, 261);
            this.BeepbindingNavigator1.Logger = null;
            this.BeepbindingNavigator1.Name = "BeepbindingNavigator1";
            this.BeepbindingNavigator1.NameSpace = null;
            this.BeepbindingNavigator1.ObjectName = null;
            this.BeepbindingNavigator1.ObjectType = null;
            this.BeepbindingNavigator1.ParentName = null;
            this.BeepbindingNavigator1.Passedarg = null;
            this.BeepbindingNavigator1.SelectedColor = System.Drawing.Color.Green;
            this.BeepbindingNavigator1.Size = new System.Drawing.Size(809, 31);
            this.BeepbindingNavigator1.TabIndex = 50;
            this.BeepbindingNavigator1.VerifyDelete = true;
            // 
            // BeepbindingNavigator2
            // 
            this.BeepbindingNavigator2.AddinName = null;
            this.BeepbindingNavigator2.BackColor = System.Drawing.Color.White;
            this.BeepbindingNavigator2.BindingSource = null;
            this.BeepbindingNavigator2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BeepbindingNavigator2.ButtonBorderSize = 0;
            this.BeepbindingNavigator2.CausesValidation = false;
            this.BeepbindingNavigator2.ControlHeight = 30;
            this.BeepbindingNavigator2.DefaultCreate = false;
            this.BeepbindingNavigator2.Description = null;
            this.BeepbindingNavigator2.DllName = null;
            this.BeepbindingNavigator2.DllPath = null;
            this.BeepbindingNavigator2.DMEEditor = null;
            this.BeepbindingNavigator2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BeepbindingNavigator2.EntityName = null;
            this.BeepbindingNavigator2.EntityStructure = null;
            this.BeepbindingNavigator2.ErrorObject = null;
            this.BeepbindingNavigator2.HightlightColor = System.Drawing.Color.Red;
            this.BeepbindingNavigator2.Location = new System.Drawing.Point(0, 345);
            this.BeepbindingNavigator2.Logger = null;
            this.BeepbindingNavigator2.Name = "BeepbindingNavigator2";
            this.BeepbindingNavigator2.NameSpace = null;
            this.BeepbindingNavigator2.ObjectName = null;
            this.BeepbindingNavigator2.ObjectType = null;
            this.BeepbindingNavigator2.ParentName = null;
            this.BeepbindingNavigator2.Passedarg = null;
            this.BeepbindingNavigator2.SelectedColor = System.Drawing.Color.Green;
            this.BeepbindingNavigator2.Size = new System.Drawing.Size(806, 31);
            this.BeepbindingNavigator2.TabIndex = 51;
            this.BeepbindingNavigator2.VerifyDelete = true;
            // 
            // poisonPanel2
            // 
            this.poisonPanel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.poisonPanel2.Controls.Add(this.poisonLabel1);
            this.poisonPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.poisonPanel2.HorizontalScrollbarBarColor = true;
            this.poisonPanel2.HorizontalScrollbarHighlightOnWheel = false;
            this.poisonPanel2.HorizontalScrollbarSize = 10;
            this.poisonPanel2.Location = new System.Drawing.Point(0, 0);
            this.poisonPanel2.Name = "poisonPanel2";
            this.poisonPanel2.Size = new System.Drawing.Size(815, 58);
            this.poisonPanel2.TabIndex = 52;
            this.poisonPanel2.UseStyleColors = true;
            this.poisonPanel2.VerticalScrollbarBarColor = true;
            this.poisonPanel2.VerticalScrollbarHighlightOnWheel = false;
            this.poisonPanel2.VerticalScrollbarSize = 10;
            // 
            // poisonLabel1
            // 
            this.poisonLabel1.AutoSize = true;
            this.poisonLabel1.FontSize = ReaLTaiizor.Extension.Poison.PoisonLabelSize.Tall;
            this.poisonLabel1.FontWeight = ReaLTaiizor.Extension.Poison.PoisonLabelWeight.Bold;
            this.poisonLabel1.Location = new System.Drawing.Point(205, 14);
            this.poisonLabel1.Name = "poisonLabel1";
            this.poisonLabel1.Size = new System.Drawing.Size(318, 25);
            this.poisonLabel1.TabIndex = 3;
            this.poisonLabel1.Text = "Web API Parameters Configurations";
            this.poisonLabel1.UseStyleColors = true;
            // 
            // poisonPanel1
            // 
            this.poisonPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.poisonPanel1.Controls.Add(this.dataGridView1);
            this.poisonPanel1.Controls.Add(this.BeepbindingNavigator1);
            this.poisonPanel1.HorizontalScrollbarBarColor = true;
            this.poisonPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.poisonPanel1.HorizontalScrollbarSize = 10;
            this.poisonPanel1.Location = new System.Drawing.Point(3, 3);
            this.poisonPanel1.Name = "poisonPanel1";
            this.poisonPanel1.Size = new System.Drawing.Size(809, 292);
            this.poisonPanel1.TabIndex = 53;
            this.poisonPanel1.VerticalScrollbarBarColor = true;
            this.poisonPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.poisonPanel1.VerticalScrollbarSize = 10;
            // 
            // poisonPanel3
            // 
            this.poisonPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.poisonPanel3.Controls.Add(this.dataGridView2);
            this.poisonPanel3.Controls.Add(this.BeepbindingNavigator2);
            this.poisonPanel3.HorizontalScrollbarBarColor = true;
            this.poisonPanel3.HorizontalScrollbarHighlightOnWheel = false;
            this.poisonPanel3.HorizontalScrollbarSize = 10;
            this.poisonPanel3.Location = new System.Drawing.Point(6, 296);
            this.poisonPanel3.Name = "poisonPanel3";
            this.poisonPanel3.Size = new System.Drawing.Size(806, 376);
            this.poisonPanel3.TabIndex = 54;
            this.poisonPanel3.VerticalScrollbarBarColor = true;
            this.poisonPanel3.VerticalScrollbarHighlightOnWheel = false;
            this.poisonPanel3.VerticalScrollbarSize = 10;
            // 
            // poisonPanel4
            // 
            this.poisonPanel4.Controls.Add(this.poisonPanel1);
            this.poisonPanel4.Controls.Add(this.poisonPanel3);
            this.poisonPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.poisonPanel4.HorizontalScrollbarBarColor = true;
            this.poisonPanel4.HorizontalScrollbarHighlightOnWheel = false;
            this.poisonPanel4.HorizontalScrollbarSize = 10;
            this.poisonPanel4.Location = new System.Drawing.Point(0, 58);
            this.poisonPanel4.Name = "poisonPanel4";
            this.poisonPanel4.Size = new System.Drawing.Size(815, 675);
            this.poisonPanel4.TabIndex = 55;
            this.poisonPanel4.VerticalScrollbarBarColor = true;
            this.poisonPanel4.VerticalScrollbarHighlightOnWheel = false;
            this.poisonPanel4.VerticalScrollbarSize = 10;
            // 
            // uc_webapiQueryParameters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.poisonPanel4);
            this.Controls.Add(this.poisonPanel2);
            this.Name = "uc_webapiQueryParameters";
            this.Size = new System.Drawing.Size(815, 733);
            ((System.ComponentModel.ISupportInitialize)(this.entitiesBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fieldsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.paramentersBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.poisonPanel2.ResumeLayout(false);
            this.poisonPanel2.PerformLayout();
            this.poisonPanel1.ResumeLayout(false);
            this.poisonPanel3.ResumeLayout(false);
            this.poisonPanel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource entitiesBindingSource;
        private ReaLTaiizor.Controls.PoisonDataGridView dataGridView1;
        private System.Windows.Forms.BindingSource fieldsBindingSource;
        private ReaLTaiizor.Controls.PoisonDataGridView dataGridView2;
        private System.Windows.Forms.BindingSource paramentersBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn parameterNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn parameterIndexDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParentId;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn12;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn13;
        private System.Windows.Forms.DataGridViewTextBoxColumn KeyToken;
        private BeepBindingNavigator BeepbindingNavigator1;
        private BeepBindingNavigator BeepbindingNavigator2;
        private ReaLTaiizor.Controls.PoisonPanel poisonPanel2;
        private ReaLTaiizor.Controls.PoisonLabel poisonLabel1;
        private ReaLTaiizor.Controls.PoisonPanel poisonPanel1;
        private ReaLTaiizor.Controls.PoisonPanel poisonPanel3;
        private ReaLTaiizor.Controls.PoisonPanel poisonPanel4;
    }
}

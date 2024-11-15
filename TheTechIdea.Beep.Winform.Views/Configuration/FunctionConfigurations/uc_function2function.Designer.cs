
 
using DataManagementModels.ConfigUtil;
using TheTechIdea.Beep.Winform.Controls.BindingNavigator;

namespace TheTechIdea.Beep.Winform.Views
{
    partial class uc_function2function
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
            ReaLTaiizor.Controls.PoisonLabel fromClassLabel;
            ReaLTaiizor.Controls.PoisonLabel fromMethodLabel;
            ReaLTaiizor.Controls.PoisonLabel toClassLabel;
            ReaLTaiizor.Controls.PoisonLabel toMethodLabel;
            ReaLTaiizor.Controls.PoisonLabel eventLabel;
            ReaLTaiizor.Controls.PoisonLabel actionTypeLabel;
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.function2FunctionsDataGridView = new ReaLTaiizor.Controls.PoisonDataGridView();
            this.FromClassdataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActionType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Event = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FromMethoddataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ToClassdataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ToMethoddataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.function2FunctionsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.fromClassComboBox = new ReaLTaiizor.Controls.PoisonComboBox();
            this.fromMethodComboBox = new ReaLTaiizor.Controls.PoisonComboBox();
            this.toClassComboBox = new ReaLTaiizor.Controls.PoisonComboBox();
            this.toMethodComboBox = new ReaLTaiizor.Controls.PoisonComboBox();
            this.eventComboBox = new ReaLTaiizor.Controls.PoisonComboBox();
            this.panel1 = new ReaLTaiizor.Controls.PoisonPanel();
            this.actionTypeComboBox = new ReaLTaiizor.Controls.PoisonComboBox();
            this.panel2 = new ReaLTaiizor.Controls.PoisonPanel();
            this.BeepbindingNavigator1 = new BeepbindingNavigator();
            this.poisonStyleManager1 = new ReaLTaiizor.Manager.PoisonStyleManager(this.components);
            this.poisonPanel1 = new ReaLTaiizor.Controls.PoisonPanel();
            this.poisonPanel2 = new ReaLTaiizor.Controls.PoisonPanel();
            this.poisonLabel1 = new ReaLTaiizor.Controls.PoisonLabel();
            fromClassLabel = new ReaLTaiizor.Controls.PoisonLabel();
            fromMethodLabel = new ReaLTaiizor.Controls.PoisonLabel();
            toClassLabel = new ReaLTaiizor.Controls.PoisonLabel();
            toMethodLabel = new ReaLTaiizor.Controls.PoisonLabel();
            eventLabel = new ReaLTaiizor.Controls.PoisonLabel();
            actionTypeLabel = new ReaLTaiizor.Controls.PoisonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.function2FunctionsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.function2FunctionsBindingSource)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.poisonStyleManager1)).BeginInit();
            this.poisonPanel1.SuspendLayout();
            this.poisonPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // fromClassLabel
            // 
            fromClassLabel.AutoSize = true;
            fromClassLabel.Location = new System.Drawing.Point(14, 7);
            fromClassLabel.Name = "fromClassLabel";
            fromClassLabel.Size = new System.Drawing.Size(77, 19);
            fromClassLabel.TabIndex = 2;
            fromClassLabel.Text = "From Class:";
            // 
            // fromMethodLabel
            // 
            fromMethodLabel.AutoSize = true;
            fromMethodLabel.Location = new System.Drawing.Point(437, 7);
            fromMethodLabel.Name = "fromMethodLabel";
            fromMethodLabel.Size = new System.Drawing.Size(94, 19);
            fromMethodLabel.TabIndex = 4;
            fromMethodLabel.Text = "From Method:";
            // 
            // toClassLabel
            // 
            toClassLabel.AutoSize = true;
            toClassLabel.Location = new System.Drawing.Point(33, 45);
            toClassLabel.Name = "toClassLabel";
            toClassLabel.Size = new System.Drawing.Size(58, 19);
            toClassLabel.TabIndex = 6;
            toClassLabel.Text = "To Class:";
            // 
            // toMethodLabel
            // 
            toMethodLabel.AutoSize = true;
            toMethodLabel.Location = new System.Drawing.Point(469, 45);
            toMethodLabel.Name = "toMethodLabel";
            toMethodLabel.Size = new System.Drawing.Size(75, 19);
            toMethodLabel.TabIndex = 8;
            toMethodLabel.Text = "To Method:";
            // 
            // eventLabel
            // 
            eventLabel.AutoSize = true;
            eventLabel.Location = new System.Drawing.Point(664, 7);
            eventLabel.Name = "eventLabel";
            eventLabel.Size = new System.Drawing.Size(79, 19);
            eventLabel.TabIndex = 10;
            eventLabel.Text = "From Event:";
            // 
            // actionTypeLabel
            // 
            actionTypeLabel.AutoSize = true;
            actionTypeLabel.Location = new System.Drawing.Point(224, 7);
            actionTypeLabel.Name = "actionTypeLabel";
            actionTypeLabel.Size = new System.Drawing.Size(80, 19);
            actionTypeLabel.TabIndex = 11;
            actionTypeLabel.Text = "Action Type:";
            // 
            // function2FunctionsDataGridView
            // 
            this.function2FunctionsDataGridView.AllowUserToResizeRows = false;
            this.function2FunctionsDataGridView.AutoGenerateColumns = false;
            this.function2FunctionsDataGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.function2FunctionsDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.function2FunctionsDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.function2FunctionsDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.function2FunctionsDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.function2FunctionsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.function2FunctionsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FromClassdataGridViewTextBoxColumn1,
            this.ActionType,
            this.Event,
            this.FromMethoddataGridViewTextBoxColumn2,
            this.ToClassdataGridViewTextBoxColumn3,
            this.ToMethoddataGridViewTextBoxColumn4});
            this.function2FunctionsDataGridView.DataSource = this.function2FunctionsBindingSource;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.function2FunctionsDataGridView.DefaultCellStyle = dataGridViewCellStyle5;
            this.function2FunctionsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.function2FunctionsDataGridView.EnableHeadersVisualStyles = false;
            this.function2FunctionsDataGridView.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.function2FunctionsDataGridView.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.function2FunctionsDataGridView.Location = new System.Drawing.Point(0, 0);
            this.function2FunctionsDataGridView.Name = "function2FunctionsDataGridView";
            this.function2FunctionsDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.function2FunctionsDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.function2FunctionsDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.function2FunctionsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.function2FunctionsDataGridView.Size = new System.Drawing.Size(880, 459);
            this.function2FunctionsDataGridView.TabIndex = 0;
            // 
            // FromClassdataGridViewTextBoxColumn1
            // 
            this.FromClassdataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.FromClassdataGridViewTextBoxColumn1.DataPropertyName = "FromClass";
            this.FromClassdataGridViewTextBoxColumn1.HeaderText = "From Class";
            this.FromClassdataGridViewTextBoxColumn1.Name = "FromClassdataGridViewTextBoxColumn1";
            this.FromClassdataGridViewTextBoxColumn1.ReadOnly = true;
            this.FromClassdataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ActionType
            // 
            this.ActionType.DataPropertyName = "ActionType";
            this.ActionType.HeaderText = "Action Type";
            this.ActionType.Name = "ActionType";
            this.ActionType.ReadOnly = true;
            // 
            // Event
            // 
            this.Event.DataPropertyName = "Event";
            this.Event.HeaderText = "From Event";
            this.Event.Name = "Event";
            this.Event.ReadOnly = true;
            // 
            // FromMethoddataGridViewTextBoxColumn2
            // 
            this.FromMethoddataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.FromMethoddataGridViewTextBoxColumn2.DataPropertyName = "FromMethod";
            this.FromMethoddataGridViewTextBoxColumn2.HeaderText = "From Method";
            this.FromMethoddataGridViewTextBoxColumn2.Name = "FromMethoddataGridViewTextBoxColumn2";
            this.FromMethoddataGridViewTextBoxColumn2.ReadOnly = true;
            this.FromMethoddataGridViewTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ToClassdataGridViewTextBoxColumn3
            // 
            this.ToClassdataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ToClassdataGridViewTextBoxColumn3.DataPropertyName = "ToClass";
            this.ToClassdataGridViewTextBoxColumn3.HeaderText = "To Class";
            this.ToClassdataGridViewTextBoxColumn3.Name = "ToClassdataGridViewTextBoxColumn3";
            this.ToClassdataGridViewTextBoxColumn3.ReadOnly = true;
            this.ToClassdataGridViewTextBoxColumn3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ToMethoddataGridViewTextBoxColumn4
            // 
            this.ToMethoddataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ToMethoddataGridViewTextBoxColumn4.DataPropertyName = "ToMethod";
            this.ToMethoddataGridViewTextBoxColumn4.HeaderText = "To Method";
            this.ToMethoddataGridViewTextBoxColumn4.Name = "ToMethoddataGridViewTextBoxColumn4";
            this.ToMethoddataGridViewTextBoxColumn4.ReadOnly = true;
            this.ToMethoddataGridViewTextBoxColumn4.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // function2FunctionsBindingSource
            // 
            this.function2FunctionsBindingSource.DataSource = typeof(DataManagementModels.ConfigUtil.Function2FunctionAction);
            // 
            // fromClassComboBox
            // 
            this.fromClassComboBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.function2FunctionsBindingSource, "FromClass", true));
            this.fromClassComboBox.FormattingEnabled = true;
            this.fromClassComboBox.ItemHeight = 23;
            this.fromClassComboBox.Location = new System.Drawing.Point(97, 7);
            this.fromClassComboBox.Name = "fromClassComboBox";
            this.fromClassComboBox.Size = new System.Drawing.Size(121, 29);
            this.fromClassComboBox.TabIndex = 0;
            this.fromClassComboBox.UseSelectable = true;
            // 
            // fromMethodComboBox
            // 
            this.fromMethodComboBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.function2FunctionsBindingSource, "FromMethod", true));
            this.fromMethodComboBox.FormattingEnabled = true;
            this.fromMethodComboBox.ItemHeight = 23;
            this.fromMethodComboBox.Location = new System.Drawing.Point(537, 7);
            this.fromMethodComboBox.Name = "fromMethodComboBox";
            this.fromMethodComboBox.Size = new System.Drawing.Size(121, 29);
            this.fromMethodComboBox.TabIndex = 2;
            this.fromMethodComboBox.UseSelectable = true;
            // 
            // toClassComboBox
            // 
            this.toClassComboBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.function2FunctionsBindingSource, "ToClass", true));
            this.toClassComboBox.FormattingEnabled = true;
            this.toClassComboBox.ItemHeight = 23;
            this.toClassComboBox.Location = new System.Drawing.Point(97, 42);
            this.toClassComboBox.Name = "toClassComboBox";
            this.toClassComboBox.Size = new System.Drawing.Size(121, 29);
            this.toClassComboBox.TabIndex = 0;
            this.toClassComboBox.UseSelectable = true;
            // 
            // toMethodComboBox
            // 
            this.toMethodComboBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.function2FunctionsBindingSource, "ToMethod", true));
            this.toMethodComboBox.FormattingEnabled = true;
            this.toMethodComboBox.ItemHeight = 23;
            this.toMethodComboBox.Location = new System.Drawing.Point(537, 42);
            this.toMethodComboBox.Name = "toMethodComboBox";
            this.toMethodComboBox.Size = new System.Drawing.Size(121, 29);
            this.toMethodComboBox.TabIndex = 1;
            this.toMethodComboBox.UseSelectable = true;
            // 
            // eventComboBox
            // 
            this.eventComboBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.function2FunctionsBindingSource, "Event", true));
            this.eventComboBox.FormattingEnabled = true;
            this.eventComboBox.ItemHeight = 23;
            this.eventComboBox.Location = new System.Drawing.Point(740, 7);
            this.eventComboBox.Name = "eventComboBox";
            this.eventComboBox.Size = new System.Drawing.Size(121, 29);
            this.eventComboBox.TabIndex = 3;
            this.eventComboBox.UseSelectable = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.poisonPanel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.HorizontalScrollbarBarColor = true;
            this.panel1.HorizontalScrollbarHighlightOnWheel = false;
            this.panel1.HorizontalScrollbarSize = 10;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(919, 641);
            this.panel1.TabIndex = 12;
            this.panel1.VerticalScrollbarBarColor = true;
            this.panel1.VerticalScrollbarHighlightOnWheel = false;
            this.panel1.VerticalScrollbarSize = 10;
            // 
            // actionTypeComboBox
            // 
            this.actionTypeComboBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.function2FunctionsBindingSource, "ActionType", true));
            this.actionTypeComboBox.FormattingEnabled = true;
            this.actionTypeComboBox.ItemHeight = 23;
            this.actionTypeComboBox.Location = new System.Drawing.Point(310, 7);
            this.actionTypeComboBox.Name = "actionTypeComboBox";
            this.actionTypeComboBox.Size = new System.Drawing.Size(121, 29);
            this.actionTypeComboBox.TabIndex = 1;
            this.actionTypeComboBox.UseSelectable = true;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.poisonLabel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.HorizontalScrollbarBarColor = true;
            this.panel2.HorizontalScrollbarHighlightOnWheel = false;
            this.panel2.HorizontalScrollbarSize = 10;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(919, 44);
            this.panel2.TabIndex = 13;
            this.panel2.VerticalScrollbarBarColor = true;
            this.panel2.VerticalScrollbarHighlightOnWheel = false;
            this.panel2.VerticalScrollbarSize = 10;
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
            this.BeepbindingNavigator1.Location = new System.Drawing.Point(0, 459);
            this.BeepbindingNavigator1.Logger = null;
            this.BeepbindingNavigator1.Name = "BeepbindingNavigator1";
            this.BeepbindingNavigator1.NameSpace = null;
            this.BeepbindingNavigator1.ObjectName = null;
            this.BeepbindingNavigator1.ObjectType = null;
            this.BeepbindingNavigator1.ParentName = null;
            this.BeepbindingNavigator1.Passedarg = null;
            this.BeepbindingNavigator1.SelectedColor = System.Drawing.Color.Green;
            this.BeepbindingNavigator1.Size = new System.Drawing.Size(880, 31);
            this.BeepbindingNavigator1.TabIndex = 14;
            this.BeepbindingNavigator1.VerifyDelete = true;
            // 
            // poisonStyleManager1
            // 
            this.poisonStyleManager1.Owner = this;
            // 
            // poisonPanel1
            // 
            this.poisonPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.poisonPanel1.Controls.Add(this.function2FunctionsDataGridView);
            this.poisonPanel1.Controls.Add(this.BeepbindingNavigator1);
            this.poisonPanel1.HorizontalScrollbarBarColor = true;
            this.poisonPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.poisonPanel1.HorizontalScrollbarSize = 10;
            this.poisonPanel1.Location = new System.Drawing.Point(20, 148);
            this.poisonPanel1.Name = "poisonPanel1";
            this.poisonPanel1.Size = new System.Drawing.Size(880, 490);
            this.poisonPanel1.TabIndex = 16;
            this.poisonPanel1.VerticalScrollbarBarColor = true;
            this.poisonPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.poisonPanel1.VerticalScrollbarSize = 10;
            // 
            // poisonPanel2
            // 
            this.poisonPanel2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.poisonPanel2.Controls.Add(actionTypeLabel);
            this.poisonPanel2.Controls.Add(this.fromMethodComboBox);
            this.poisonPanel2.Controls.Add(this.toMethodComboBox);
            this.poisonPanel2.Controls.Add(this.toClassComboBox);
            this.poisonPanel2.Controls.Add(this.eventComboBox);
            this.poisonPanel2.Controls.Add(toClassLabel);
            this.poisonPanel2.Controls.Add(eventLabel);
            this.poisonPanel2.Controls.Add(toMethodLabel);
            this.poisonPanel2.Controls.Add(fromClassLabel);
            this.poisonPanel2.Controls.Add(this.actionTypeComboBox);
            this.poisonPanel2.Controls.Add(fromMethodLabel);
            this.poisonPanel2.Controls.Add(this.fromClassComboBox);
            this.poisonPanel2.HorizontalScrollbarBarColor = true;
            this.poisonPanel2.HorizontalScrollbarHighlightOnWheel = false;
            this.poisonPanel2.HorizontalScrollbarSize = 10;
            this.poisonPanel2.Location = new System.Drawing.Point(20, 59);
            this.poisonPanel2.Name = "poisonPanel2";
            this.poisonPanel2.Size = new System.Drawing.Size(880, 83);
            this.poisonPanel2.TabIndex = 17;
            this.poisonPanel2.VerticalScrollbarBarColor = true;
            this.poisonPanel2.VerticalScrollbarHighlightOnWheel = false;
            this.poisonPanel2.VerticalScrollbarSize = 10;
            // 
            // poisonLabel1
            // 
            this.poisonLabel1.AutoSize = true;
            this.poisonLabel1.FontSize = ReaLTaiizor.Extension.Poison.PoisonLabelSize.Tall;
            this.poisonLabel1.FontWeight = ReaLTaiizor.Extension.Poison.PoisonLabelWeight.Bold;
            this.poisonLabel1.Location = new System.Drawing.Point(326, 6);
            this.poisonLabel1.Name = "poisonLabel1";
            this.poisonLabel1.Size = new System.Drawing.Size(296, 25);
            this.poisonLabel1.TabIndex = 4;
            this.poisonLabel1.Text = "System Functions  Configurations";
            this.poisonLabel1.UseStyleColors = true;
            // 
            // uc_function2function
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.poisonPanel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "uc_function2function";
            this.Size = new System.Drawing.Size(919, 641);
            ((System.ComponentModel.ISupportInitialize)(this.function2FunctionsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.function2FunctionsBindingSource)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.poisonStyleManager1)).EndInit();
            this.poisonPanel1.ResumeLayout(false);
            this.poisonPanel2.ResumeLayout(false);
            this.poisonPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource function2FunctionsBindingSource;
        private ReaLTaiizor.Controls.PoisonDataGridView function2FunctionsDataGridView;
        private  ReaLTaiizor.Controls.PoisonComboBox fromClassComboBox;
        private  ReaLTaiizor.Controls.PoisonComboBox fromMethodComboBox;
        private  ReaLTaiizor.Controls.PoisonComboBox toClassComboBox;
        private  ReaLTaiizor.Controls.PoisonComboBox toMethodComboBox;
        private  ReaLTaiizor.Controls.PoisonComboBox eventComboBox;
        private ReaLTaiizor.Controls.PoisonPanel panel1;
        private ReaLTaiizor.Controls.PoisonPanel panel2;
        private  ReaLTaiizor.Controls.PoisonComboBox actionTypeComboBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn FromClassdataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ActionType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Event;
        private System.Windows.Forms.DataGridViewTextBoxColumn FromMethoddataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn ToClassdataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn ToMethoddataGridViewTextBoxColumn4;
        private BeepbindingNavigator BeepbindingNavigator1;
        private ReaLTaiizor.Manager.PoisonStyleManager poisonStyleManager1;
        private ReaLTaiizor.Controls.PoisonPanel poisonPanel1;
        private ReaLTaiizor.Controls.PoisonPanel poisonPanel2;
        private ReaLTaiizor.Controls.PoisonLabel poisonLabel1;
    }
}

using TheTechIdea.Beep.Winform.Controls.BindingNavigator;

namespace TheTechIdea.Beep.Winform.Controls.Grid
{
    partial class BeepGrid
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            Toppanel = new Panel();
            CSVExportbutton = new BeepButton();
            TotalShowbutton = new BeepButton();
            Sharebutton = new BeepButton();
            Printbutton = new BeepButton();
            FilterShowbutton = new BeepButton();
            Titlelabel = new BeepLabel();
            Bottompanel = new Panel();
            BindingNavigator = new BeepbindingNavigator();
            dataGridView1 = new DataGridView();
            filterPanel = new Panel();
            FilterMessagepanel = new Panel();
            Totalspanel = new Panel();
            customHeaderPanel = new Panel();
            Toppanel.SuspendLayout();
            Bottompanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // Toppanel
            // 
            Toppanel.BackColor = Color.White;
            Toppanel.BorderStyle = BorderStyle.FixedSingle;
            Toppanel.Controls.Add(CSVExportbutton);
            Toppanel.Controls.Add(TotalShowbutton);
            Toppanel.Controls.Add(Sharebutton);
            Toppanel.Controls.Add(Printbutton);
            Toppanel.Controls.Add(FilterShowbutton);
            Toppanel.Controls.Add(Titlelabel);
            Toppanel.Dock = DockStyle.Top;
            Toppanel.Location = new Point(0, 0);
            Toppanel.Margin = new Padding(4, 3, 4, 3);
            Toppanel.Name = "Toppanel";
            Toppanel.Size = new Size(858, 30);
            Toppanel.TabIndex = 0;
            // 
            // CSVExportbutton
            // 
            CSVExportbutton.BackgroundImage = Properties.Resources._16x16;
            CSVExportbutton.BackgroundImageLayout = ImageLayout.Zoom;
            CSVExportbutton.Location = new Point(24, 3);
            CSVExportbutton.Margin = new Padding(4, 3, 4, 3);
            CSVExportbutton.Name = "CSVExportbutton";
            CSVExportbutton.Size = new Size(23, 23);
            CSVExportbutton.TabIndex = 11;
     
            // 
            // TotalShowbutton
            // 
            TotalShowbutton.BackgroundImage = Properties.Resources.plus;
            TotalShowbutton.BackgroundImageLayout = ImageLayout.Zoom;
            TotalShowbutton.Location = new Point(2, 3);
            TotalShowbutton.Margin = new Padding(4, 3, 4, 3);
            TotalShowbutton.Name = "TotalShowbutton";
            TotalShowbutton.Size = new Size(23, 23);
            TotalShowbutton.TabIndex = 10;
          
            // 
            // Sharebutton
            // 
            Sharebutton.BackgroundImage = Properties.Resources.messages;
            Sharebutton.BackgroundImageLayout = ImageLayout.Zoom;
            Sharebutton.Location = new Point(47, 3);
            Sharebutton.Margin = new Padding(4, 3, 4, 3);
            Sharebutton.Name = "Sharebutton";
            Sharebutton.Size = new Size(23, 23);
            Sharebutton.TabIndex = 2;
      
            // 
            // Printbutton
            // 
            Printbutton.BackgroundImage = Properties.Resources._015_printer;
            Printbutton.BackgroundImageLayout = ImageLayout.Zoom;
            Printbutton.Location = new Point(69, 3);
            Printbutton.Margin = new Padding(4, 3, 4, 3);
            Printbutton.Name = "Printbutton";
            Printbutton.Size = new Size(23, 23);
            Printbutton.TabIndex = 1;
     
            // 
            // FilterShowbutton
            // 
            FilterShowbutton.BackgroundImage = Properties.Resources._052_filter;
            FilterShowbutton.BackgroundImageLayout = ImageLayout.Zoom;
            FilterShowbutton.Location = new Point(90, 3);
            FilterShowbutton.Margin = new Padding(4, 3, 4, 3);
            FilterShowbutton.Name = "FilterShowbutton";
            FilterShowbutton.Size = new Size(23, 23);
            FilterShowbutton.TabIndex = 0;

            // 
            // Titlelabel
            // 
            Titlelabel.Anchor = AnchorStyles.None;
            Titlelabel.Location = new Point(275, 3);
            Titlelabel.Margin = new Padding(4, 0, 4, 0);
            Titlelabel.Name = "Titlelabel";
            Titlelabel.Size = new Size(317, 23);
            Titlelabel.TabIndex = 9;
            Titlelabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Bottompanel
            // 
            Bottompanel.BackColor = Color.White;
            Bottompanel.Controls.Add(BindingNavigator);
            Bottompanel.Dock = DockStyle.Bottom;
            Bottompanel.Location = new Point(0, 374);
            Bottompanel.Margin = new Padding(4, 3, 4, 3);
            Bottompanel.Name = "Bottompanel";
            Bottompanel.Size = new Size(858, 28);
            Bottompanel.TabIndex = 1;
            // 
            // BindingNavigator
            // 
            BindingNavigator.AddinName = null;
            BindingNavigator.BackColor = Color.White;
            BindingNavigator.bindingSource = null;
            BindingNavigator.ButtonBorderSize = 0;
            BindingNavigator.CausesValidation = false;
            BindingNavigator.ControlHeight = 30;
            BindingNavigator.DefaultCreate = false;
            BindingNavigator.Description = null;
            BindingNavigator.DestConnection = null;
            BindingNavigator.DllName = null;
            BindingNavigator.DllPath = null;
            BindingNavigator.DMEEditor = null;
            BindingNavigator.Dock = DockStyle.Bottom;
            BindingNavigator.Dset = null;
            BindingNavigator.EntityName = null;
            BindingNavigator.EntityStructure = null;
            BindingNavigator.ErrorObject = null;
            BindingNavigator.ExtensionsHelpers = null;
            BindingNavigator.HightlightColor = Color.Empty;
            BindingNavigator.Location = new Point(0, 0);
            BindingNavigator.Logger = null;
            BindingNavigator.Margin = new Padding(5, 3, 5, 3);
            BindingNavigator.Name = "BindingNavigator";
            BindingNavigator.NameSpace = null;
            BindingNavigator.ObjectName = null;
            BindingNavigator.ObjectType = null;
            BindingNavigator.ParentBranch = null;
            BindingNavigator.ParentName = null;
            BindingNavigator.Passedarg = null;
            BindingNavigator.pbr = null;
            BindingNavigator.Progress = null;
            BindingNavigator.RootBranch = null;
            BindingNavigator.SelectedColor = Color.Empty;
            BindingNavigator.Size = new Size(858, 28);
            BindingNavigator.SourceConnection = null;
            BindingNavigator.TabIndex = 9;
            BindingNavigator.Tree = null;
            BindingNavigator.UnitofWork = null;
            BindingNavigator.util = null;
            BindingNavigator.VerifyDelete = true;
            BindingNavigator.ViewRootBranch = null;
            BindingNavigator.Visutil = null;
            // 
            // dataGridView1
            // 
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle1.BackColor = Color.White;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle1.ForeColor = Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = Color.Gold;
            dataGridViewCellStyle1.SelectionForeColor = Color.FromArgb(0, 0, 192);
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = Color.White;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dataGridView1.ColumnHeadersHeight = 35;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = SystemColors.Window;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dataGridView1.DefaultCellStyle = dataGridViewCellStyle3;
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.GridColor = Color.White;
            dataGridView1.Location = new Point(0, 0);
            dataGridView1.Margin = new Padding(4, 3, 4, 3);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = Color.White;
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 8.25F);
            dataGridViewCellStyle4.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            dataGridView1.RowHeadersVisible = false;
            dataGridViewCellStyle5.BackColor = Color.White;
            dataGridViewCellStyle5.ForeColor = Color.Black;
            dataGridViewCellStyle5.SelectionBackColor = Color.AliceBlue;
            dataGridViewCellStyle5.SelectionForeColor = Color.Navy;
            dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle5;
            dataGridView1.RowTemplate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ShowCellErrors = false;
            dataGridView1.ShowRowErrors = false;
            dataGridView1.Size = new Size(858, 224);
            dataGridView1.TabIndex = 2;
            dataGridView1.VirtualMode = true;
            // 
            // filterPanel
            // 
            filterPanel.BackColor = Color.White;
            filterPanel.BorderStyle = BorderStyle.FixedSingle;
            filterPanel.Dock = DockStyle.Top;
            filterPanel.Location = new Point(0, 61);
            filterPanel.Margin = new Padding(4, 3, 4, 3);
            filterPanel.Name = "filterPanel";
            filterPanel.Size = new Size(858, 31);
            filterPanel.TabIndex = 3;
            filterPanel.Visible = false;
            // 
            // FilterMessagepanel
            // 
            FilterMessagepanel.BackColor = Color.White;
            FilterMessagepanel.BorderStyle = BorderStyle.FixedSingle;
            FilterMessagepanel.Dock = DockStyle.Bottom;
            FilterMessagepanel.Location = new Point(0, 347);
            FilterMessagepanel.Margin = new Padding(4, 3, 4, 3);
            FilterMessagepanel.Name = "FilterMessagepanel";
            FilterMessagepanel.Size = new Size(858, 27);
            FilterMessagepanel.TabIndex = 4;
            FilterMessagepanel.Visible = false;
            // 
            // Totalspanel
            // 
            Totalspanel.BackColor = Color.White;
            Totalspanel.BorderStyle = BorderStyle.FixedSingle;
            Totalspanel.Dock = DockStyle.Bottom;
            Totalspanel.Location = new Point(0, 316);
            Totalspanel.Margin = new Padding(4, 3, 4, 3);
            Totalspanel.Name = "Totalspanel";
            Totalspanel.Size = new Size(858, 31);
            Totalspanel.TabIndex = 5;
            Totalspanel.Visible = false;
            // 
            // customHeaderPanel
            // 
            customHeaderPanel.BackColor = Color.White;
            customHeaderPanel.BorderStyle = BorderStyle.FixedSingle;
            customHeaderPanel.Dock = DockStyle.Top;
            customHeaderPanel.Location = new Point(0, 30);
            customHeaderPanel.Margin = new Padding(4, 3, 4, 3);
            customHeaderPanel.Name = "customHeaderPanel";
            customHeaderPanel.Size = new Size(858, 31);
            customHeaderPanel.TabIndex = 6;
            // 
            // BeepGrid
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(dataGridView1);
            Controls.Add(filterPanel);
            Controls.Add(customHeaderPanel);
            Controls.Add(Totalspanel);
            Controls.Add(FilterMessagepanel);
            Controls.Add(Bottompanel);
            Controls.Add(Toppanel);
            Margin = new Padding(5, 3, 5, 3);
            Name = "BeepGrid";
            Size = new Size(858, 402);
            Toppanel.ResumeLayout(false);
            Bottompanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        public Panel Toppanel;
        public Panel Bottompanel;

        public BeepButton FilterShowbutton;
        public BeepButton Printbutton;
        public BeepButton Sharebutton;
        public BeepLabel Titlelabel;
        private DataGridView dataGridView1;
        private Panel filterPanel;
        public BeepbindingNavigator BindingNavigator;
        private Panel FilterMessagepanel;
        private Panel Totalspanel;
        public BeepButton TotalShowbutton;
        private BeepButton CSVExportbutton;
        private Panel customHeaderPanel;
    }
}

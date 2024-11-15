namespace TheTechIdea.Beep.Winform.Views.Crud
{
    partial class uc_crudView
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
            PrimaryKeycomboBox = new ComboBox();
            label2 = new Label();
            label1 = new Label();
            beepGrid1 = new Controls.Grid.BeepGrid();
            panel1 = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // PrimaryKeycomboBox
            // 
            PrimaryKeycomboBox.Anchor = AnchorStyles.Top;
            PrimaryKeycomboBox.FormattingEnabled = true;
            PrimaryKeycomboBox.Location = new Point(1016, 23);
            PrimaryKeycomboBox.Name = "PrimaryKeycomboBox";
            PrimaryKeycomboBox.Size = new Size(157, 23);
            PrimaryKeycomboBox.TabIndex = 2;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top;
            label2.BackColor = Color.White;
            label2.FlatStyle = FlatStyle.Flat;
            label2.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold);
            label2.Location = new Point(881, 18);
            label2.Name = "label2";
            label2.Size = new Size(129, 33);
            label2.TabIndex = 1;
            label2.Text = "Primary Key";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            label1.BackColor = Color.White;
            label1.FlatStyle = FlatStyle.Flat;
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            label1.Location = new Point(3, 10);
            label1.Name = "label1";
            label1.Size = new Size(456, 37);
            label1.TabIndex = 0;
            label1.Text = "CRUD";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // beepGrid1
            // 
            beepGrid1.AddinName = null;
            beepGrid1.AllowDrop = true;
            beepGrid1.AllowUserToAddRows = true;
            beepGrid1.AllowUserToDeleteRows = true;
            beepGrid1.BorderStyle = BorderStyle.FixedSingle;
            beepGrid1.DefaultCreate = true;
            beepGrid1.Description = null;
            beepGrid1.DestConnection = null;
            beepGrid1.DllName = null;
            beepGrid1.DllPath = null;
            beepGrid1.DMEEditor = null;
            beepGrid1.Dock = DockStyle.Fill;
            beepGrid1.Dset = null;
            beepGrid1.EntityName = null;
            beepGrid1.EntityStructure = null;
            beepGrid1.ErrorObject = null;
            beepGrid1.ExtensionsHelpers = null;
            beepGrid1.Location = new Point(0, 63);
            beepGrid1.Logger = null;
            beepGrid1.Margin = new Padding(5, 3, 5, 3);
            beepGrid1.Name = "beepGrid1";
            beepGrid1.NameSpace = null;
            beepGrid1.ObjectName = null;
            beepGrid1.ObjectType = "UserControl";
            beepGrid1.ParentBranch = null;
            beepGrid1.ParentName = null;
            beepGrid1.Passedarg = null;
            beepGrid1.pbr = null;
            beepGrid1.Progress = null;
            beepGrid1.ReadOnly = false;
            beepGrid1.RootBranch = null;
            beepGrid1.ShowFilterPanel = true;
            beepGrid1.ShowTotalsPanel = true;
            beepGrid1.Size = new Size(1206, 619);
            beepGrid1.SourceConnection = null;
            beepGrid1.TabIndex = 1;
            beepGrid1.Tree = null;
            beepGrid1.util = null;
            beepGrid1.VerifyDelete = true;
            beepGrid1.ViewRootBranch = null;
            beepGrid1.Visutil = null;
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(PrimaryKeycomboBox);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(label2);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1206, 63);
            panel1.TabIndex = 2;
            // 
            // uc_crudView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            AutoSize = true;
            Controls.Add(beepGrid1);
            Controls.Add(panel1);
            DoubleBuffered = true;
            Name = "uc_crudView";
            Size = new Size(1206, 682);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Label label1;
        private Panel Containerpanel;
        private Label label2;
        private ComboBox PrimaryKeycomboBox;
        private Controls.Grid.BeepGrid beepGrid1;
        private Panel panel1;
    }
}

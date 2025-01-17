//namespace TheTechIdea.Beep.Winform.Controls.MainForm
//{
//    partial class Frm_Main
//    {
//        /// <summary>
//        /// Required designer variable.
//        /// </summary>
//        private System.ComponentModel.IContainer components = null;

//        /// <summary>
//        /// Clean up any resources being used.
//        /// </summary>
//        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing && (components != null))
//            {
//                components.Dispose();
//            }
//            base.Dispose(disposing);
//        }

//        #region Windows Form Designer generated code

//        /// <summary>
//        /// Required method for Designer support - do not modify
//        /// the contents of this method with the code editor.
//        /// </summary>
//        private void InitializeComponent()
//        {
//            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Main));
//            MenuPanel1 = new Panel();
//            toolStripHoriz = new ToolStrip();
//            SearchtoolStripButton = new ToolStripButton();
//            SearchtoolStripTextBox = new ToolStripTextBox();
//            HometoolStripButton1 = new ToolStripButton();
//            menuControl1 = new MenuStrip();
//            uc_MainSplitPanel1 = new uc_MainSplitPanel();
//            toolStripVertical = new ToolStrip();
//            CollapseTreetoolStripButton = new ToolStripButton();
//            LogWindowstoolStripButton = new ToolStripButton();
//            MenuPanel1.SuspendLayout();
//            toolStripHoriz.SuspendLayout();
//            toolStripVertical.SuspendLayout();
//            SuspendLayout();
//            // 
//            // MenuPanel1
//            // 
//            MenuPanel1.Controls.Add(toolStripHoriz);
//            MenuPanel1.Controls.Add(menuControl1);
//            MenuPanel1.Dock = DockStyle.Top;
//            MenuPanel1.Location = new Point(0, 0);
//            MenuPanel1.Margin = new Padding(4, 3, 4, 3);
//            MenuPanel1.Name = "MenuPanel1";
//            MenuPanel1.Size = new Size(1400, 61);
//            MenuPanel1.TabIndex = 1;
//            // 
//            // toolStripHoriz
//            // 
//            toolStripHoriz.BackColor = Color.White;
//            toolStripHoriz.Dock = DockStyle.Fill;
//            toolStripHoriz.Items.AddRange(new ToolStripItem[] { SearchtoolStripButton, SearchtoolStripTextBox, HometoolStripButton1 });
//            toolStripHoriz.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
//            toolStripHoriz.Location = new Point(0, 24);
//            toolStripHoriz.Name = "toolStripHoriz";
//            toolStripHoriz.Size = new Size(1400, 37);
//            toolStripHoriz.TabIndex = 5;
//            toolStripHoriz.Text = "toolStrip2";
//            // 
//            // SearchtoolStripButton
//            // 
//            SearchtoolStripButton.Alignment = ToolStripItemAlignment.Right;
//            SearchtoolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
//            SearchtoolStripButton.Image = Properties.Resources.Search;
//            SearchtoolStripButton.ImageScaling = ToolStripItemImageScaling.None;
//            SearchtoolStripButton.ImageTransparentColor = Color.Magenta;
//            SearchtoolStripButton.Name = "SearchtoolStripButton";
//            SearchtoolStripButton.Size = new Size(23, 34);
//            SearchtoolStripButton.ToolTipText = "Search";
//            // 
//            // SearchtoolStripTextBox
//            // 
//            SearchtoolStripTextBox.AcceptsReturn = true;
//            SearchtoolStripTextBox.AcceptsTab = true;
//            SearchtoolStripTextBox.Alignment = ToolStripItemAlignment.Right;
//            SearchtoolStripTextBox.AutoToolTip = true;
//            SearchtoolStripTextBox.BorderStyle = BorderStyle.FixedSingle;
//            SearchtoolStripTextBox.Name = "SearchtoolStripTextBox";
//            SearchtoolStripTextBox.Padding = new Padding(5);
//            SearchtoolStripTextBox.Size = new Size(110, 37);
//            SearchtoolStripTextBox.ToolTipText = "Enter Text To Search for";
//            // 
//            // HometoolStripButton1
//            // 
//            HometoolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
//            HometoolStripButton1.Image = Properties.Resources._128_home_button;
//            HometoolStripButton1.ImageTransparentColor = Color.Magenta;
//            HometoolStripButton1.Name = "HometoolStripButton1";
//            HometoolStripButton1.Size = new Size(23, 34);
//            HometoolStripButton1.Text = "Home";
//            // 
//            // menuControl1
//            // 
//            menuControl1.BackColor = Color.White;
//            menuControl1.TextFont = new TextFont("Segoe UI", 9F);
//            menuControl1.Location = new Point(0, 0);
//            menuControl1.Name = "menuControl1";
//            menuControl1.Padding = new Padding(7, 2, 0, 2);
//            menuControl1.RenderMode = ToolStripRenderMode.Professional;
//            menuControl1.Size = new Size(1400, 24);
//            menuControl1.TabIndex = 0;
//            menuControl1.Text = "menuStrip1";
//            // 
//            // uc_MainSplitPanel1
//            // 
//            uc_MainSplitPanel1.AddinName = null;
//            uc_MainSplitPanel1.AllowDrop = true;
//            uc_MainSplitPanel1.DefaultCreate = true;
//            uc_MainSplitPanel1.Description = null;
//            uc_MainSplitPanel1.DestConnection = null;
//            uc_MainSplitPanel1.DllName = null;
//            uc_MainSplitPanel1.DllPath = null;
//            uc_MainSplitPanel1.DMEEditor = null;
//            uc_MainSplitPanel1.Dock = DockStyle.Fill;
//            uc_MainSplitPanel1.Dset = null;
//            uc_MainSplitPanel1.EntityName = null;
//            uc_MainSplitPanel1.EntityStructure = null;
//            uc_MainSplitPanel1.ErrorObject = null;
//            uc_MainSplitPanel1.IsLogPanelOpen = false;
//            uc_MainSplitPanel1.IsTreeSideOpen = true;
//            uc_MainSplitPanel1.Location = new Point(37, 61);
//            uc_MainSplitPanel1.Logger = null;
//            uc_MainSplitPanel1.Margin = new Padding(6, 3, 6, 3);
//            uc_MainSplitPanel1.Name = "uc_MainSplitPanel1";
//            uc_MainSplitPanel1.NameSpace = null;
//            uc_MainSplitPanel1.ObjectName = null;
//            uc_MainSplitPanel1.ObjectType = "UserControl";
//            uc_MainSplitPanel1.ParentBranch = null;
//            uc_MainSplitPanel1.ParentName = null;
//            uc_MainSplitPanel1.Passedarg = null;
//            uc_MainSplitPanel1.pbr = null;
//            uc_MainSplitPanel1.Progress = null;
//            uc_MainSplitPanel1.RootBranch = null;
//            uc_MainSplitPanel1.Size = new Size(1363, 664);
//            uc_MainSplitPanel1.SourceConnection = null;
//            uc_MainSplitPanel1.StartLoggin = false;
//            uc_MainSplitPanel1.TabIndex = 2;
//            uc_MainSplitPanel1.util = null;
//            uc_MainSplitPanel1.ViewRootBranch = null;
//            uc_MainSplitPanel1.Visutil = null;
//            // 
//            // toolStripVertical
//            // 
//            toolStripVertical.BackColor = Color.White;
//            toolStripVertical.Dock = DockStyle.Left;
//            toolStripVertical.ImageScalingSize = new Size(32, 32);
//            toolStripVertical.Items.AddRange(new ToolStripItem[] { CollapseTreetoolStripButton, LogWindowstoolStripButton });
//            toolStripVertical.LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
//            toolStripVertical.Location = new Point(0, 61);
//            toolStripVertical.Name = "toolStripVertical";
//            toolStripVertical.RenderMode = ToolStripRenderMode.Professional;
//            toolStripVertical.Size = new Size(37, 664);
//            toolStripVertical.TabIndex = 3;
//            // 
//            // CollapseTreetoolStripButton
//            // 
//            CollapseTreetoolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
//            CollapseTreetoolStripButton.Image = Properties.Resources.hamburgerblue;
//            CollapseTreetoolStripButton.ImageTransparentColor = Color.Magenta;
//            CollapseTreetoolStripButton.Name = "CollapseTreetoolStripButton";
//            CollapseTreetoolStripButton.Size = new Size(34, 36);
//            CollapseTreetoolStripButton.Text = "StandardTree Show/Hide";
//            // 
//            // LogWindowstoolStripButton
//            // 
//            LogWindowstoolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
//            LogWindowstoolStripButton.Image = Properties.Resources.log;
//            LogWindowstoolStripButton.ImageTransparentColor = Color.Magenta;
//            LogWindowstoolStripButton.Name = "LogWindowstoolStripButton";
//            LogWindowstoolStripButton.Size = new Size(34, 36);
//            LogWindowstoolStripButton.Text = "Log Window Show/Hide";
//            // 
//            // Frm_Main
//            // 
//            AutoScaleDimensions = new SizeF(7F, 15F);
//            AutoScaleMode = AutoScaleMode.TextFont;
//            ClientSize = new Size(1400, 725);
//            Controls.Add(uc_MainSplitPanel1);
//            Controls.Add(toolStripVertical);
//            Controls.Add(MenuPanel1);
//            HelpButton = true;
//            Icon = (Icon)resources.GetObject("$this.Icon");
//            Margin = new Padding(4, 3, 4, 3);
//            MaximumSize = new Size(4011, 1600);
//            MinimumSize = new Size(219, 40);
//            Name = "Frm_Main";
//            StartPosition = FormStartPosition.CenterScreen;
//            Text = "Beep - The Data  Platform";
//            TopMost = true;
//            MenuPanel1.ResumeLayout(false);
//            MenuPanel1.PerformLayout();
//            toolStripHoriz.ResumeLayout(false);
//            toolStripHoriz.PerformLayout();
//            toolStripVertical.ResumeLayout(false);
//            toolStripVertical.PerformLayout();
//            ResumeLayout(false);
//            PerformLayout();
//        }

//        #endregion
//        private Panel MenuPanel1;
//        private uc_MainSplitPanel uc_MainSplitPanel1;
//        private ToolStrip toolStripVertical;
//        private MenuStrip menuControl1;
//        private ToolStrip toolStripHoriz;
//        private ToolStripButton SearchtoolStripButton;
//        public ToolStripTextBox SearchtoolStripTextBox;
//        private ToolStripButton HometoolStripButton1;
//        private ToolStripButton CollapseTreetoolStripButton;
//        private ToolStripButton LogWindowstoolStripButton;
//    }
//}
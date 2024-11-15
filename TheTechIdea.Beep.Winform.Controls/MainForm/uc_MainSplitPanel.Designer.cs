namespace TheTechIdea.Beep.Winform.Controls.MainForm
{
    partial class uc_MainSplitPanel
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
            MainsplitContainer = new SplitContainer();
            MinimizeLeftPanelbutton = new Button();
            treeView1 = new TreeView();
            LogsplitContainer = new SplitContainer();
            MinimizeLogPanelButton = new Button();
            MaximizeLogPanelbutton = new Button();
            MaximizeLeftPanelbutton = new Button();
            uc_Container1 = new Containers.uc_Container();
            uc_logPanel1 = new LogPanel.uc_logPanel();
            ((System.ComponentModel.ISupportInitialize)MainsplitContainer).BeginInit();
            MainsplitContainer.Panel1.SuspendLayout();
            MainsplitContainer.Panel2.SuspendLayout();
            MainsplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)LogsplitContainer).BeginInit();
            LogsplitContainer.Panel1.SuspendLayout();
            LogsplitContainer.Panel2.SuspendLayout();
            LogsplitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // MainsplitContainer
            // 
            MainsplitContainer.BorderStyle = BorderStyle.FixedSingle;
            MainsplitContainer.Dock = DockStyle.Fill;
            MainsplitContainer.Location = new Point(0, 0);
            MainsplitContainer.Margin = new Padding(10);
            MainsplitContainer.Name = "MainsplitContainer";
            // 
            // MainsplitContainer.Panel1
            // 
            MainsplitContainer.Panel1.BackColor = Color.White;
            MainsplitContainer.Panel1.Controls.Add(MinimizeLeftPanelbutton);
            MainsplitContainer.Panel1.Controls.Add(treeView1);
            // 
            // MainsplitContainer.Panel2
            // 
            MainsplitContainer.Panel2.Controls.Add(LogsplitContainer);
            MainsplitContainer.Size = new Size(1023, 786);
            MainsplitContainer.SplitterDistance = 263;
            MainsplitContainer.SplitterWidth = 5;
            MainsplitContainer.TabIndex = 0;
            MainsplitContainer.SplitterMoved += MainsplitContainer_SplitterMoved;
            // 
            // MinimizeLeftPanelbutton
            // 
            MinimizeLeftPanelbutton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            MinimizeLeftPanelbutton.BackColor = Color.Transparent;
            MinimizeLeftPanelbutton.BackgroundImage = Properties.Resources.CollapseLeft;
            MinimizeLeftPanelbutton.BackgroundImageLayout = ImageLayout.Zoom;
            MinimizeLeftPanelbutton.Location = new Point(243, 0);
            MinimizeLeftPanelbutton.Margin = new Padding(4, 3, 4, 3);
            MinimizeLeftPanelbutton.Name = "MinimizeLeftPanelbutton";
            MinimizeLeftPanelbutton.Size = new Size(18, 17);
            MinimizeLeftPanelbutton.TabIndex = 0;
            MinimizeLeftPanelbutton.UseVisualStyleBackColor = false;
            MinimizeLeftPanelbutton.Visible = false;
            // 
            // treeView1
            // 
            treeView1.BorderStyle = BorderStyle.None;
            treeView1.CheckBoxes = true;
            treeView1.Dock = DockStyle.Fill;
            treeView1.FullRowSelect = true;
            treeView1.HideSelection = false;
            treeView1.HotTracking = true;
            treeView1.ItemHeight = 32;
            treeView1.Location = new Point(0, 0);
            treeView1.Margin = new Padding(4, 3, 4, 3);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(261, 784);
            treeView1.TabIndex = 1;
            // 
            // LogsplitContainer
            // 
            LogsplitContainer.BorderStyle = BorderStyle.FixedSingle;
            LogsplitContainer.Dock = DockStyle.Fill;
            LogsplitContainer.Location = new Point(0, 0);
            LogsplitContainer.Margin = new Padding(4, 3, 4, 3);
            LogsplitContainer.Name = "LogsplitContainer";
            LogsplitContainer.Orientation = Orientation.Horizontal;
            // 
            // LogsplitContainer.Panel1
            // 
            LogsplitContainer.Panel1.BackColor = Color.White;
            LogsplitContainer.Panel1.Controls.Add(MinimizeLogPanelButton);
            LogsplitContainer.Panel1.Controls.Add(MaximizeLogPanelbutton);
            LogsplitContainer.Panel1.Controls.Add(MaximizeLeftPanelbutton);
            LogsplitContainer.Panel1.Controls.Add(uc_Container1);
            // 
            // LogsplitContainer.Panel2
            // 
            LogsplitContainer.Panel2.Controls.Add(uc_logPanel1);
            LogsplitContainer.Panel2Collapsed = true;
            LogsplitContainer.Size = new Size(755, 786);
            LogsplitContainer.SplitterDistance = 680;
            LogsplitContainer.SplitterWidth = 5;
            LogsplitContainer.TabIndex = 0;
            // 
            // MinimizeLogPanelButton
            // 
            MinimizeLogPanelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            MinimizeLogPanelButton.BackColor = Color.Transparent;
            MinimizeLogPanelButton.BackgroundImage = Properties.Resources.CollapseDown;
            MinimizeLogPanelButton.BackgroundImageLayout = ImageLayout.Zoom;
            MinimizeLogPanelButton.Location = new Point(734, 764);
            MinimizeLogPanelButton.Margin = new Padding(4, 3, 4, 3);
            MinimizeLogPanelButton.Name = "MinimizeLogPanelButton";
            MinimizeLogPanelButton.Size = new Size(19, 18);
            MinimizeLogPanelButton.TabIndex = 2;
            MinimizeLogPanelButton.UseVisualStyleBackColor = false;
            MinimizeLogPanelButton.Visible = false;
            // 
            // MaximizeLogPanelbutton
            // 
            MaximizeLogPanelbutton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            MaximizeLogPanelbutton.BackColor = Color.Transparent;
            MaximizeLogPanelbutton.BackgroundImage = Properties.Resources.CollapseUp;
            MaximizeLogPanelbutton.BackgroundImageLayout = ImageLayout.Zoom;
            MaximizeLogPanelbutton.Location = new Point(734, 764);
            MaximizeLogPanelbutton.Margin = new Padding(4, 3, 4, 3);
            MaximizeLogPanelbutton.Name = "MaximizeLogPanelbutton";
            MaximizeLogPanelbutton.Size = new Size(19, 18);
            MaximizeLogPanelbutton.TabIndex = 3;
            MaximizeLogPanelbutton.UseVisualStyleBackColor = false;
            MaximizeLogPanelbutton.Visible = false;
            // 
            // MaximizeLeftPanelbutton
            // 
            MaximizeLeftPanelbutton.BackColor = Color.Transparent;
            MaximizeLeftPanelbutton.BackgroundImage = Properties.Resources.Collapseright;
            MaximizeLeftPanelbutton.BackgroundImageLayout = ImageLayout.Zoom;
            MaximizeLeftPanelbutton.Location = new Point(-1, 0);
            MaximizeLeftPanelbutton.Margin = new Padding(4, 3, 4, 3);
            MaximizeLeftPanelbutton.Name = "MaximizeLeftPanelbutton";
            MaximizeLeftPanelbutton.Size = new Size(18, 17);
            MaximizeLeftPanelbutton.TabIndex = 1;
            MaximizeLeftPanelbutton.UseVisualStyleBackColor = false;
            MaximizeLeftPanelbutton.Visible = false;
            // 
            // uc_Container1
            // 
            uc_Container1.AutoScroll = true;
            uc_Container1.BackColor = Color.White;
          
            uc_Container1.Dock = DockStyle.Fill;
            uc_Container1.Editor = null;
            uc_Container1.Location = new Point(0, 0);
            uc_Container1.Margin = new Padding(5, 3, 5, 3);
            uc_Container1.Name = "uc_Container1";
            uc_Container1.Size = new Size(753, 784);
            uc_Container1.TabIndex = 3;
            uc_Container1.VisManager = null;
            // 
            // uc_logPanel1
            // 
            uc_logPanel1.AddinName = null;
            uc_logPanel1.BackColor = Color.White;
            uc_logPanel1.DefaultCreate = true;
            uc_logPanel1.Description = null;
            uc_logPanel1.DestConnection = null;
            uc_logPanel1.DllName = null;
            uc_logPanel1.DllPath = null;
            uc_logPanel1.DMEEditor = null;
            uc_logPanel1.Dock = DockStyle.Fill;
            uc_logPanel1.Dset = null;
            uc_logPanel1.EntityName = null;
            uc_logPanel1.EntityStructure = null;
            uc_logPanel1.ErrorObject = null;
            uc_logPanel1.ForeColor = Color.Black;
            uc_logPanel1.Location = new Point(0, 0);
            uc_logPanel1.Logger = null;
            uc_logPanel1.Margin = new Padding(6, 3, 6, 3);
            uc_logPanel1.Name = "uc_logPanel1";
            uc_logPanel1.NameSpace = null;
            uc_logPanel1.ObjectName = null;
            uc_logPanel1.ObjectType = "UserControl";
            uc_logPanel1.ParentBranch = null;
            uc_logPanel1.ParentName = null;
            uc_logPanel1.Passedarg = null;
            uc_logPanel1.pbr = null;
            uc_logPanel1.Progress = null;
            uc_logPanel1.RootBranch = null;
            uc_logPanel1.Size = new Size(753, 99);
            uc_logPanel1.SourceConnection = null;
            uc_logPanel1.startLoggin = false;
            uc_logPanel1.TabIndex = 3;
            uc_logPanel1.Tree = null;
            uc_logPanel1.util = null;
            uc_logPanel1.ViewRootBranch = null;
            uc_logPanel1.Visutil = null;
            // 
            // uc_MainSplitPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(MainsplitContainer);
            Margin = new Padding(5, 3, 5, 3);
            Name = "uc_MainSplitPanel";
            Size = new Size(1023, 786);
            MainsplitContainer.Panel1.ResumeLayout(false);
            MainsplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)MainsplitContainer).EndInit();
            MainsplitContainer.ResumeLayout(false);
            LogsplitContainer.Panel1.ResumeLayout(false);
            LogsplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)LogsplitContainer).EndInit();
            LogsplitContainer.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer MainsplitContainer;
        private SplitContainer LogsplitContainer;
        private Button MinimizeLeftPanelbutton;
        private Button MinimizeLogPanelButton;
        private Button MaximizeLeftPanelbutton;
        private Button MaximizeLogPanelbutton;
        private Containers.uc_Container uc_Container1;
        private LogPanel.uc_logPanel uc_logPanel1;
        private TreeView treeView1;
    }
}

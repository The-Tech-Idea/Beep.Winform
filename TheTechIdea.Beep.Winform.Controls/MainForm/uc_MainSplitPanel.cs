
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Basic;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Winform.Controls.MainForm
{
    public partial class uc_MainSplitPanel : uc_Addin
    {
        public override void SetConfig(IDMEEditor pDMEEditor, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            base.SetConfig(pDMEEditor, plogger, putil, args, e, per);
            uc_logPanel1.SetConfig(pDMEEditor, plogger, putil, args, e, per);
            SetLogger(Logger);
           
          
        }
        public uc_MainSplitPanel()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.MainsplitContainer.AllowDrop = true;
            this.LogsplitContainer.AllowDrop = true;
            uc_Container1.ContainerType = ContainerTypeEnum.SinglePanel;
            this.MainsplitContainer.Panel1.AllowDrop = true;
            this.MainsplitContainer.Panel2.AllowDrop = true;
            this.LogsplitContainer.Panel1.AllowDrop = true;
            this.LogsplitContainer.Panel2.AllowDrop = true;
            this.LogsplitContainer.Panel2.Resize += Panel2_Resize;
            this.uc_Container1.AllowDrop = true;
            this.treeView1.ImageList = new ImageList();

            MinimizeLogPanelButton.Top = LogsplitContainer.Panel1.Height - MaximizeLogPanelbutton.Height - 2;
            MinimizeLogPanelButton.Left = LogsplitContainer.Panel1.Width - MaximizeLogPanelbutton.Width - 2;
            this.MaximizeLeftPanelbutton.Click += MaximizeLeftPanelbutton_Click;
            this.MaximizeLogPanelbutton.Click += MaximizeLogPanelbutton_Click;
            this.MinimizeLeftPanelbutton.Click += MinimizeLeftPanelbutton_Click;
            this.MinimizeLogPanelButton.Click += MinimizeLogPanelButton_Click;
            this.MaximizeLogPanelbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));

            //SetLogger(Logger);
        }
        public void MinMaxLog()
        {
            if (_isLogPanelOpen)
            {
                LogsplitContainer.Panel2Collapsed = false;
               // MaximizeLogPanelbutton.Visible = false;
               // MinimizeLogPanelButton.Visible = true;
                //   IsLogPanelOpen = !IsLogPanelOpen;

            }
            else
            {
               // MaximizeLogPanelbutton.Visible = true;
               // MinimizeLogPanelButton.Visible = false;
                LogsplitContainer.Panel2Collapsed = true;
                //  IsLogPanelOpen = !IsLogPanelOpen;
            }
        }
        public void MinMaxTree()
        {
            if (_isTreeSideOpen)
            {
               // MaximizeLeftPanelbutton.Visible = false;
              //  MinimizeLeftPanelbutton.Visible = true;
                MainsplitContainer.Panel1Collapsed = false;
                //  IsTreeSideOpen = !IsTreeSideOpen;
            }
            else
            {
               // MaximizeLeftPanelbutton.Visible = true;
              //  MinimizeLeftPanelbutton.Visible = false;
                MainsplitContainer.Panel1Collapsed = true;
                // IsTreeSideOpen = !IsTreeSideOpen;
            }

        }
        private void Panel2_Resize(object sender, EventArgs e)
        {
            MaximizeLogPanelbutton.Top = LogsplitContainer.Panel1.Height - MaximizeLogPanelbutton.Height - 2;
            MaximizeLogPanelbutton.Left = LogsplitContainer.Panel1.Width - MaximizeLogPanelbutton.Width - 2;

            MinimizeLogPanelButton.Top = LogsplitContainer.Panel1.Height - MaximizeLogPanelbutton.Height - 2;
            MinimizeLogPanelButton.Left = LogsplitContainer.Panel1.Width - MaximizeLogPanelbutton.Width - 2;
        }
        private void MinimizeLogPanelButton_Click(object? sender, EventArgs e)
        {
            IsLogPanelOpen = false;
            // IsLogPanelOpen = !IsLogPanelOpen;
        }

        private void MinimizeLeftPanelbutton_Click(object? sender, EventArgs e)
        {

            IsTreeSideOpen = false;
        }

        private void MaximizeLogPanelbutton_Click(object? sender, EventArgs e)
        {
            IsLogPanelOpen = true;
            // IsLogPanelOpen = !IsLogPanelOpen;
        }

        private void MaximizeLeftPanelbutton_Click(object? sender, EventArgs e)
        {

            IsTreeSideOpen = true;
        }
        bool _isTreeSideOpen = true;
        // bool _isSidePanelsOpen = true;
        bool _isLogPanelOpen = true;
        public bool IsTreeSideOpen
        {
            get { return _isTreeSideOpen; }
            set { _isTreeSideOpen = value; MinMaxTree(); }
        }
        //public bool IsSidePanelsOpen {
        //    get { return _isTreeSideOpen; }
        //    set { _isTreeSideOpen = value; MinMaxTree(); }
        //}
        public bool IsLogPanelOpen
        {
            get { return _isLogPanelOpen; }
            set { _isLogPanelOpen = value; MinMaxLog(); }
        }
        //bool _isHideLogPanel;
        //public bool HideLogPanel
        //{
        //    get { return _isHideLogPanel; }
        //    set
        //    {
        //        _isHideLogPanel = value;
        //        IsLogPanelOpen = !value;
        //        //        MaximizeLogPanelbutton.Visible = !value;

        //    }
        //}
        public IDisplayContainer Container => uc_Container1;
        public TreeView Tree => treeView1;
        public void SetLogger(IDMLogger logAndError)
        {
            uc_logPanel1.SetLogger(logAndError);
        }

        private void MainsplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        public bool StartLoggin { get { return uc_logPanel1.StartLogging; } set { uc_logPanel1.StartLogging = value; } }

    }
}

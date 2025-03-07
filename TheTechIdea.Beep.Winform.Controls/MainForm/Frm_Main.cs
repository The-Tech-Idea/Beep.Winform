//using TheTechIdea.Beep.Utilities;
//using TheTechIdea.Beep.Logger;
//using TheTechIdea.Beep.Vis;
//using TheTechIdea.Beep.Winform.Controls.Template;
//using TheTechIdea.Beep.Winform.Controls.ITrees.FormsTreeView;
//using TheTechIdea.Beep.Winform.Controls.MenuBar;
//using TheTechIdea.Beep.Winform.Controls.ToolBar;
//using TheTechIdea.Beep.Vis.Modules;
//using TheTechIdea.Beep.DataBase;
//using TheTechIdea.Beep.Editor;
//using TheTechIdea.Beep.Addin;
//using TheTechIdea.Beep.ConfigUtil;


//namespace TheTechIdea.Beep.Winform.Controls.MainForm
//{
//    [AddinAttribute(Caption = "Main Form", Name = "Frm_Main", misc = "Config", menu = "Configuration", addinType = AddinType.Form, displayType = DisplayType.Popup)]
//    public partial class Frm_Main : frm_Addin, IMainForm
//    {
//        TreeViewControl ApptreeControl;
//        MenuControl AppmenuControl;
//        ToolbarControl ApptoolbarControl;
//        ToolbarControl BeepVerticaltoolbarControl;
//        ToolbarControl BeepHorizantaltoolbarControl;
//        MenuControl BeepmenuControl;
//        TreeViewControl BeepAppTree;
//        PassedArgs args = new PassedArgs();
//        Progress<PassedArgs> progress;
//        bool _hideLog = true;
//        bool _hideSide = true;

//        public event EventHandler<KeyCombination> KeyPressed;

//        public bool IsTreeShown
//        {
//            get { return _hideSide; }
//            set
//            {
//                _hideSide = value;
//                uc_MainSplitPanel1.IsTreeSideOpen = value;
//            }
//        }
//        public bool IsLogPanelShown
//        {
//            get { return _hideLog; }
//            set { _hideLog = value; 
//                uc_MainSplitPanel1.IsLogPanelOpen = value; 
//              }
//        }
//        public bool IsLogStarted
//        {
//            get { return uc_MainSplitPanel1.StartLoggin; }
//            set
//            {
                
//                    uc_MainSplitPanel1.StartLoggin = value;
               
//            }
//        }
//        public IDM_Addin HorizantalToolBar { get; set; }
//        public IDM_Addin VerticalToolBar { get; set; }
//        public IDM_Addin MenuBar { get; set; }
//        public IDM_Addin DisplayContainer { get; set; }
//        public IDM_Addin EntityListContainer { get; set; }
//        public object CurrentObjectEntity { get; set; }
//        public EntityStructure CurrentEntityStructure { get; set; }
//        public IAppManager VisManager { get; set; }
//        public string SearchBoxText { get  ; set  ; }
//        public string SearchBoxAutoCompleteData { get  ; set  ; }
//        public bool IsSearchBoxAutoComplete { get  ; set  ; }
//        public string SearchDataSource { get  ; set  ; }

//        public Frm_Main()
//        {
//            InitializeComponent();
//            this.SearchtoolStripButton.Click += SearchtoolStripButton_Click;
//            this.SearchtoolStripTextBox.AcceptsReturn = true;
//            this.SearchtoolStripTextBox.TextChanged += SearchtoolStripTextBox_TextChanged;
//            this.HometoolStripButton1.Click += HometoolStripButton1_Click;
//            this.LogWindowstoolStripButton.Click += LogWindowstoolStripButton_Click;
//            this.CollapseTreetoolStripButton.Click += CollapseTreetoolStripButton_Click;

//        }

//        private void CollapseTreetoolStripButton_Click(object? sender, EventArgs e)
//        {
//            IsTreeShown=!IsTreeShown;
//            //VisManager.ShowTreeWindow = !VisManager.ShowTreeWindow;
//        }

//        private void LogWindowstoolStripButton_Click(object? sender, EventArgs e)
//        {
//            //if(VisManager.ShowLogWindow!=IsLogPanelShown)
//            //{
//            IsLogPanelShown = !IsLogPanelShown;
//            //}else
//            //    VisManager.ShowLogWindow = !VisManager.ShowLogWindow;
//        }

//        private void HometoolStripButton1_Click(object? sender, EventArgs e)
//        {
//            if (string.IsNullOrEmpty(VisManager.HomePageName))
//            {
//                VisManager.ShowHome();
//            }
//        }
//        private void SearchtoolStripTextBox_TextChanged(object? sender, EventArgs e)
//        {

//        }
//        private void SearchtoolStripButton_Click(object? sender, EventArgs e)
//        {

//        }
//        public override void SetConfig(IDMEEditor pbl, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
//        {
//            base.SetConfig(pbl, plogger, putil, args, e, per);
//            uc_MainSplitPanel1.SetConfig(pbl, plogger, putil, args, e, per);
//            VisManager = Visutil;
//    //        Visutil.BeepObjectsName = "Beep";
//            SetUpUI();
            
//            if (DMEEditor.ErrorObject.Flag == Errors.Ok)
//            {

//            }
//        }
//        private void SetBeepUI(string BeepUIID = "beep")
//        {
//            if (Visutil.IsBeepDataOn)
//            {
                
                
               
//            }
//            SetUpTree();
//            SetUpMenu();
//            SetUpVerticalBar();
//            SetUpHorizentalBar();
//        }
//        private void SetAppUI(string AppUIID = "dhub")
//        {
//            if (Visutil.IsAppOn)
//            {
//                ///------------ Setup App  
//                ApptreeControl.TreeType = Visutil.AppObjectsName;
//                ApptreeControl.ObjectType = Visutil.AppObjectsName;  //"dhub");
//                ApptoolbarControl.ObjectType = Visutil.AppObjectsName;
//                AppmenuControl.ObjectType = Visutil.AppObjectsName; ;



//                ApptreeControl.TreeV = uc_MainSplitPanel1.StandardTree;
//                ApptoolbarControl.TreeV = uc_MainSplitPanel1.StandardTree;
//                AppmenuControl.TreeV = uc_MainSplitPanel1.StandardTree;

//                AppmenuControl.vismanager = Visutil;
//                ApptoolbarControl.vismanager = Visutil;

//                SendMessege(progress, "Loading DHUB Functions and StandardTree");

//                ApptreeControl.CreateRootTree();

//                SendMessege(progress, "Loading App Toobar Functions ");
//                Visutil.PasstoWaitForm((PassedArgs)Passedarg);

//                SendMessege(progress, "Loading Function Extensions ToolBar for App ");

//                ApptoolbarControl.ToolStrip = toolStripVertical;

//                ApptoolbarControl.CreateToolbar();

//                SendMessege(progress, "Loading Function Extensions Menu for App ");

//                AppmenuControl.MenuStrip = menuControl1;
//                AppmenuControl.CreateGlobalMenu();
//                if (menuControl1.Items.Count == 0)
//                {
//                    menuControl1.Visible = false;
//                }
//            }
//        }
//        private void SendMessege(IProgress<PassedArgs> progress, string messege)
//        {
//            if (progress != null)
//            {
//                args.Messege = messege;
//                progress.Report(args);
//            }
//        }
//        public void StartStopLog(bool val)
//        {
//            IsLogStarted = val;
//            IsLogPanelShown = !val;

//        }
//        public void SetSearchBoxAutoCompleteData(ISearchDataBoxSettings settings)
//        {
//            DMEEditor.ErrorObject.Flag = Errors.Ok;
//            DMEEditor.ErrorObject.Message = string.Empty;
//            try
//            {

//            }
//            catch (Exception ex)
//            {
//                DMEEditor.AddLogMessage("Beep", $"{ex.Message}", DateTime.Now, -1, null, Errors.Failed);
//            }

//        }
//        public IErrorsInfo SetUpMenu()
//        {
//            DMEEditor.ErrorObject.Flag = Errors.Ok;
//            DMEEditor.ErrorObject.Message = string.Empty;
//            try
//            {
//                BeepmenuControl.ObjectType = Visutil.BeepObjectsName;
//                BeepmenuControl.TreeV = uc_MainSplitPanel1.StandardTree;
//                BeepmenuControl.vismanager = Visutil;
//                SendMessege(progress, "Loading Function Extensions Menu for Beep  Data Management");


//                BeepmenuControl.MenuStrip = menuControl1;
//                MenuBar = (IDM_Addin)BeepmenuControl;
//                BeepmenuControl.CreateGlobalMenu();
//                if (menuControl1.Items.Count == 0)
//                {
//                    menuControl1.Visible = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                DMEEditor.AddLogMessage("Beep", $"{ex.Message}", DateTime.Now, -1, null, Errors.Failed);
//            }
//            return DMEEditor.ErrorObject;
//        }
//        public IErrorsInfo SetUpTree()
//        {
//            DMEEditor.ErrorObject.Flag = Errors.Ok;
//            DMEEditor.ErrorObject.Message = string.Empty;
//            try
//            {
//                BeepAppTree.TreeType = Visutil.BeepObjectsName;
//                BeepAppTree.ObjectType = Visutil.BeepObjectsName;
//                BeepAppTree.TreeV = uc_MainSplitPanel1.StandardTree;

//                //  you can change icon size in StandardTree controls  ex. Apptree.IconsSize = new Value(24, 24);

//                SendMessege(progress, "Loading Beep Data Management Functions and StandardTree");

//                BeepAppTree.CreateRootTree();
//            }
//            catch (Exception ex)
//            {
//                DMEEditor.AddLogMessage("Beep", $"{ex.Message}", DateTime.Now, -1, null, Errors.Failed);
//            }
//            return DMEEditor.ErrorObject;
//        }
//        public IErrorsInfo SetUpHorizentalBar()
//        {
//            DMEEditor.ErrorObject.Flag = Errors.Ok;
//            DMEEditor.ErrorObject.Message = string.Empty;
//            try
//            {
//                BeepHorizantaltoolbarControl.ObjectType = Visutil.BeepObjectsName;
//                BeepHorizantaltoolbarControl.IsHorizentalBar = true;
//                BeepHorizantaltoolbarControl.TreeV = uc_MainSplitPanel1.StandardTree;
//                BeepHorizantaltoolbarControl.vismanager = Visutil;



//                SendMessege(progress, "Loading Function Extensions ToolBar for Beep  Data Management");


//                BeepHorizantaltoolbarControl.ToolStrip = toolStripHoriz;
//                HorizantalToolBar = (IDM_Addin)BeepHorizantaltoolbarControl;
//                BeepHorizantaltoolbarControl.CreateToolbar();

//            }
//            catch (Exception ex)
//            {
//                DMEEditor.AddLogMessage("Beep", $"{ex.Message}", DateTime.Now, -1, null, Errors.Failed);
//            }
//            return DMEEditor.ErrorObject;
//        }
//        public IErrorsInfo SetUpVerticalBar()
//        {
//            DMEEditor.ErrorObject.Flag = Errors.Ok;
//            DMEEditor.ErrorObject.Message = string.Empty;
//            try
//            {
//                BeepVerticaltoolbarControl.ObjectType = Visutil.BeepObjectsName;
//                BeepVerticaltoolbarControl.TreeV = uc_MainSplitPanel1.StandardTree;

//                BeepVerticaltoolbarControl.vismanager = Visutil;
//                VerticalToolBar = (IDM_Addin)BeepVerticaltoolbarControl;


//                SendMessege(progress, "Loading Function Extensions ToolBar for Beep  Data Management");


//                BeepVerticaltoolbarControl.ToolStrip = toolStripVertical;
//                BeepVerticaltoolbarControl.CreateToolbar();


//            }
//            catch (Exception ex)
//            {
//                DMEEditor.AddLogMessage("Beep", $"{ex.Message}", DateTime.Now, -1, null, Errors.Failed);
//            }
//            return DMEEditor.ErrorObject;
//        }
//        public IErrorsInfo SetUpUI()
//        {
//            DMEEditor.ErrorObject.Flag = Errors.Ok;
//            DMEEditor.ErrorObject.Message = string.Empty;
//            try
//            {
//                //StartStopLog(false);
//                Visutil.Container = uc_MainSplitPanel1.Container;
//                BeepAppTree = (TreeViewControl)Visutil.StandardTree;
//                BeepVerticaltoolbarControl = (ToolbarControl)Visutil.ToolStrip;
//                BeepHorizantaltoolbarControl = (ToolbarControl)Visutil.SecondaryToolStrip;
//                BeepmenuControl = (MenuControl)Visutil.MenuStrip;

//                ApptreeControl = (TreeViewControl)Visutil.SecondaryTree;
//                AppmenuControl = (MenuControl)Visutil.SecondaryMenuStrip;
//                ApptoolbarControl = (ToolbarControl)Visutil.SecondaryToolStrip;

//                //  this.Text = Visutil.Title;
//                //this.Icon = (Icon)Visutil.visHelper.(Visutil.IconUrl);
//                PassedArgs p = new PassedArgs();
//                p.ParameterString1 = "Loading DLL's";
//                // Config Wait Form
//                Visutil.CloseWaitForm();
//                Visutil.ShowWaitForm(p);
//                // Passing Message to WaitForm
//                Visutil.PasstoWaitForm(p);

//                // Prepare Async Data Notification from Assembly loader to WaitForm

//                progress = new Progress<PassedArgs>(percent =>
//                {

//                    p.Messege = percent.Messege;
//                    Visutil.PasstoWaitForm(p);
//                });
//                SetBeepUI();
//                Visutil.CloseWaitForm();
//               // StartStopLog(false);
//            }
//            catch (Exception ex)
//            {
//                DMEEditor.AddLogMessage("Beep", $"{ex.Message}", DateTime.Now, -1, null, Errors.Failed);
//            }
//            return DMEEditor.ErrorObject;

//        }

//        public IErrorsInfo PressKey(KeyCombination keyCombination)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}

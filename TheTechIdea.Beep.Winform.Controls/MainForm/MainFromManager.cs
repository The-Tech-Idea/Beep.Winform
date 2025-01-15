//using TheTechIdea.Beep.Vis.Modules;
//using TheTechIdea.Beep.DataBase;
//using TheTechIdea.Beep.Vis;
//using TheTechIdea.Beep.Winform.Controls.MenuBar;
//using TheTechIdea.Beep.Winform.Controls.ToolBar;
//using TheTechIdea.Beep.Winform.Controls.ITrees.FormsTreeView;
//using TheTechIdea.Beep.Utilities;
//using TheTechIdea.Beep.Editor;
//using TheTechIdea.Beep.Addin;
//using TheTechIdea.Beep.ConfigUtil;

//namespace TheTechIdea.Beep.Winform.Controls.MainForm
//{
//    public class MainFromManager : IMainForm
//    {
//        public MainFromManager(IDMEEditor dMEEditor,IAppManager vis)
//        {
//            DMEEditor = dMEEditor;
//            Visutil = vis;
//            _hideLog = false;
//            _hideSide = true;
//        }
//        public MenuStrip WinformMenu { get;  set; }
//        public ToolStrip WinformToolStripHoriz { get;  set; }
//        public ToolStrip WinformToolStripVertical { get;  set; }

//        ToolbarControl BeepVerticaltoolbarControl;
//        ToolbarControl BeepHorizantaltoolbarControl;
//        MenuControl BeepmenuControl;
//        TreeViewControl BeepTreeControl;


//        PassedArgs args = new PassedArgs();
//        Progress<PassedArgs> progress;
//        bool _hideLog = true;
//        bool _hideSide = false;

//        public event EventHandler<KeyCombination> KeyPressed;

//        uc_MainSplitPanel MainSplitPanel { get; set; }
//        public bool IsTreeShown
//        {
//            get { return _hideSide; }
//            set
//            {
//                _hideSide = value;
//                MainSplitPanel.IsTreeSideOpen = value;
//            }
//        }
//        public bool IsLogPanelShown
//        {
//            get { return _hideLog; }
//            set { _hideLog = value; MainSplitPanel.IsLogPanelOpen = value; IsLogStarted = value; }
//        }
//        public bool IsLogStarted
//        {
//            get { return MainSplitPanel.StartLoggin; }
//            set
//            {
//                if (!_hideLog)
//                {
//                    MainSplitPanel.StartLoggin = value;
//                }
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
//        public IDMEEditor DMEEditor { get; }
//        public IAppManager Visutil { get; }
//        public string SearchBoxText { get  ; set  ; }
//        public string SearchBoxAutoCompleteData { get  ; set  ; }
//        public bool IsSearchBoxAutoComplete { get  ; set  ; }
//        public string SearchDataSource { get  ; set  ; }

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
//                BeepmenuControl.TreeV = MainSplitPanel.Tree;
//                BeepmenuControl.vismanager = Visutil;
//                SendMessege(progress, "Loading Function Extensions Menu for Beep  Data Management");


//                BeepmenuControl.MenuStrip = WinformMenu;
//                MenuBar = (IDM_Addin)BeepmenuControl;
//                BeepmenuControl.CreateGlobalMenu();
//                if (WinformMenu.Items.Count == 0)
//                {
//                    WinformMenu.Visible = false;
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
//                BeepTreeControl.TreeType = Visutil.BeepObjectsName;
//                BeepTreeControl.ObjectType = Visutil.BeepObjectsName;
//                BeepTreeControl.TreeV = MainSplitPanel.Tree;

//                //  you can change icon size in Tree controls  ex. Apptree.IconsSize = new Size(24, 24);

//                SendMessege(progress, "Loading Beep Data Management Functions and Tree");

//                BeepTreeControl.CreateRootTree();
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
//                BeepHorizantaltoolbarControl.TreeV = MainSplitPanel.Tree;
//                BeepHorizantaltoolbarControl.vismanager = Visutil;



//                SendMessege(progress, "Loading Function Extensions ToolBar for Beep  Data Management");


//                BeepHorizantaltoolbarControl.ToolStrip = WinformToolStripHoriz;
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
//                BeepVerticaltoolbarControl.TreeV = MainSplitPanel.Tree;

//                BeepVerticaltoolbarControl.vismanager = Visutil;
//                VerticalToolBar = (IDM_Addin)BeepVerticaltoolbarControl;


//                SendMessege(progress, "Loading Function Extensions ToolBar for Beep  Data Management");


//                BeepVerticaltoolbarControl.ToolStrip = WinformToolStripVertical;
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
//                Visutil.Container = MainSplitPanel.Container;
//                BeepTreeControl = (TreeViewControl)Visutil.Tree;
//                BeepVerticaltoolbarControl = (ToolbarControl)Visutil.ToolStrip;
//                BeepHorizantaltoolbarControl = (ToolbarControl)Visutil.SecondaryToolStrip;
//                BeepmenuControl = (MenuControl)Visutil.MenuStrip;

//                //ApptreeControl = (TreeControl)Visutil.SecondaryTree;
//                //AppmenuControl = (MenuControl)Visutil.SecondaryMenuStrip;
//                //ApptoolbarControl = (ToolbarControl)Visutil.SecondaryToolStrip;

//                //  this.Text = Visutil.Title;
//                //this.Icon = (Icon)Visutil.visHelper.(Visutil.IconUrl);
//                PassedArgs p = new PassedArgs();
//                p.ParameterString1 = "Loading DLL's";
//                // Show Wait Form
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
//                StartStopLog(false);
//            }
//            catch (Exception ex)
//            {
//                DMEEditor.AddLogMessage("Beep", $"{ex.Message}", DateTime.Now, -1, null, Errors.Failed);
//            }
//            return DMEEditor.ErrorObject;

//        }
//        private void SetBeepUI(string BeepUIID = "beep")
//        {
//            if (Visutil.IsBeepDataOn)
//            {
//                Visutil.BeepObjectsName = BeepUIID;
//                SetUpTree();
//                SetUpMenu();
//                SetUpVerticalBar();
//                SetUpHorizentalBar();
//            }
//        }
//        public void StartStopLog(bool val)
//        {
//            IsLogStarted = val;
//            IsLogPanelShown = !val;

//        }
//        private void SendMessege(IProgress<PassedArgs> progress, string messege)
//        {
//            if (progress != null)
//            {
//                args.Messege = messege;
//                progress.Report(args);
//            }
//        }

//        public IErrorsInfo PressKey(KeyCombination keyCombination)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}

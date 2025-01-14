using System.Reflection;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Utilities;

using TheTechIdea.Beep.Winform.Controls.Managers.Wizards;
using TheTechIdea.Beep.Winform.Controls.Template;
using TheTechIdea.Beep.Winform.Controls.Wait;
using TheTechIdea.Beep.Vis.Logic;
using System.Data;
using System.Runtime.InteropServices;
using DialogResult = TheTechIdea.Beep.Vis.Modules.DialogResult;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Managers
{
    public partial class VisManager : IVisManager
    {
        //public VisManager(IDMEEditor editor)
        //{
        //    DMEEditor = editor;
        //}
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        public bool IsinCaptureMenuMode { get; set; }
        public event EventHandler<IPassedArgs> PreClose;
        public event EventHandler<IPassedArgs> PreCallModule;
        public event EventHandler<IPassedArgs> PreLogin;
        public event EventHandler<IPassedArgs> PostLogin;
        public event EventHandler<IPassedArgs> PreShowItem;
        public event EventHandler<IPassedArgs> PostShowItem;
        public event EventHandler<IPassedArgs> PostCallModule;
        public event EventHandler<KeyCombination> KeyPressed;

        public EnumBeepThemes Theme { get; set; }
        public IBeepUser User { get; set; }
        public string LogoUrl { get; set; }
        string tt;
        public string Title
        {
            get { return tt; }
            set { tt = value; }
        }
        public string IconUrl { get; set; }
        public IDMEEditor DMEEditor { get; set; }
        public IBeepUIComponent MenuStrip { get; set; }
        public IBeepUIComponent ToolStrip { get; set; }
        public IBeepUIComponent Tree { get; set; }
        public ITree MyTree { get; set; }
        public bool WaitFormShown { get; set; } = false;
        public IBeepUIComponent SecondaryTree { get; set; }
        public IBeepUIComponent SecondaryToolStrip { get; set; }
        public IBeepUIComponent SecondaryMenuStrip { get; set; }
        public List<ObjectItem> objects { get; set; } = new List<ObjectItem>();
        public bool IsBeepDataOn { get; set; } = true;
        public bool IsAppOn { get; set; } = true;
        public int TreeIconSize { get; set; } = 32;
        public bool TreeExpand { get; set; } = false;
        public int SecondaryTreeIconSize { get; set; } = 32;
        public bool SecondaryTreeExpand { get; set; } = false;
        public bool IsDevModeOn { get; set; } = false;
        public bool ShowSideBarWindow { get; set; }
        public string AppObjectsName { get; set; }
        public string BeepObjectsName { get; set; } = "Beep";
        public IVisHelper visHelper { get; set; }
        public List<AddinsShownData> addinsShowns { get; set; } = new List<AddinsShownData>();
        public List<IDM_Addin> Addins { get; set; } = new List<IDM_Addin>();
        public IControlManager Controlmanager { get; set; }
        public ControlManager _controlManager { get { return (ControlManager)Controlmanager; } }
        public ErrorsInfo ErrorsandMesseges { get; set; }
        public IDM_Addin CurrentDisplayedAddin { get; set; }
        public IBeepUIComponent MainDisplay { get; set; }
        public IFunctionandExtensionsHelpers Helpers { get; set; }
        bool _isLogOn = false;
        public bool IsLogOn
        {
            get { return _isLogOn; }
            set
            {
                _isLogOn = value;
                LogOnOffWindows(value);
            }
        }
        public bool IsDataModified { get; set; }
        bool _showLogWindow = false;
        bool _showTreeWindow = true;
        public bool IsShowingWaitForm { get; set; }
        public bool ShowLogWindow
        {
            get => _showLogWindow; set
            {
                SetLogWindows(value);
            }
        }
        IMainForm MainDisplayForm;
        private void SetLogWindows(bool val)
        {
            MainDisplayForm = (IMainForm)MainForm;
            _showLogWindow = val;
            MainDisplayForm.IsLogPanelShown = val;
        }
        private void LogOnOffWindows(bool val)
        {
            MainDisplayForm = (IMainForm)MainForm;
            MainDisplayForm.IsLogStarted = val;

        }
        public bool ShowTreeWindow { get => _showTreeWindow; set { SetTreeWindow(value); } }
        private void SetTreeWindow(bool val)
        {
            MainDisplayForm = (IMainForm)MainForm;
            _showTreeWindow = val;
            MainDisplayForm.IsTreeShown = val;
        }
        public int Width { get; set; } = 1200;
        public int Height { get; set; } = 800;

        public VisManager(IDMEEditor pdmeeditor)
        {
            PassedArgs a = new PassedArgs();
            PreLogin?.Invoke(this, a);
            IsDataModified = false;
            CurrentDisplayedAddin = null;
            DMEEditor = pdmeeditor;


            //Tree = new TreeViewControl(DMEEditor, this);
            //ToolStrip = new ToolbarControl(DMEEditor, (TreeViewControl)Tree);
            //MenuStrip = new MenuControl(DMEEditor, (TreeViewControl)Tree);

            //SecondaryTree = new TreeViewControl(DMEEditor, this);
            //SecondaryToolStrip = new ToolbarControl(DMEEditor, (TreeViewControl)Tree);
            //SecondaryMenuStrip = new MenuControl(DMEEditor, (TreeViewControl)Tree);

            Controlmanager = new ControlManager(DMEEditor, this);
            wizardManager = new WizardManager(DMEEditor, this);
            if (DMEEditor.Passedarguments == null)
            {

                DMEEditor.Passedarguments = new PassedArgs();
                DMEEditor.Passedarguments.Objects = new List<ObjectItem>();

            }
            Helpers = new FunctionandExtensionsHelpers(DMEEditor, this, (ITree)Tree);
            DMEEditor.Passedarguments.Objects = CreateArgsParameterForVisUtil(DMEEditor.Passedarguments.Objects);
            visHelper = new VisHelper(DMEEditor, this);
            a = new PassedArgs();
      //      MyTree = (ITree)Tree;
      //      MyTree.PreShowItem += MyTree_PreShowItem;
            PostLogin?.Invoke(this, a);

        }

        private void MyTree_PreShowItem(object? sender, IPassedArgs e)
        {

            e.Cancel = false;
            PreShowItem?.Invoke(this, e);


        }

        #region "Winform Implemetation Properties"
        public ImageList Images { get; set; } = new ImageList();

        //    public List<IFileStorage> ImagesUrls { get; set; } = new List<IFileStorage>();
        BeepWait BeepWaitForm { get; set; }
        public IWaitForm WaitForm { get; set; }

        public Form MainForm { get; set; }
        IDisplayContainer container;
        // private Control _container;
        public IDisplayContainer Container { get { return container; } set { container = value; } } // container.AddinRemoved += Container_AddinRemoved; container.VisManager = this;container.Editor = DMEEditor;
        //container.AddinChanged += Container_AddinChanged; _controlManager.DisplayPanel = (Control) value;
        #endregion
        public IWizardManager wizardManager { get; set; }
        public bool IsShowingMainForm { get; set; } = false;
        public bool TurnonOffCheckBox { get; set; }

        IDM_Addin MainFormView;
        private bool disposedValue;

        private bool CurrentSingltonChecked;

        public IErrorsInfo PressKey(KeyCombination keyCombination)
        {
            try
            {

                if (Tree != null)
                {
                    ITree r = (ITree)Tree;
                    AssemblyClassDefinition s = DMEEditor.ConfigEditor.AppComponents.FirstOrDefault(p => p.GuidID == keyCombination.AssemblyGuid);
                    MethodsClass methodsClass = null;
                    if (s != null)
                    {
                        methodsClass = s.Methods.FirstOrDefault(p => p.GuidID == keyCombination.MappedFunction.GuidID);
                    }
                    if (methodsClass != null)
                    {
                        r.CreateFunctionExtensions(methodsClass);
                    }
                }


            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Beep", $"Error in Pressing Key {ex.Message}", DateTime.Now, 0, "", Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }
        public virtual IErrorsInfo LoadSetting()
        {
            try
            {
                ErrorsandMesseges.Flag = Errors.Ok;
                ErrorsandMesseges.Message = $"Function Executed";
            }
            catch (Exception ex)
            {
                ErrorsandMesseges.Flag = Errors.Failed;
                ErrorsandMesseges.Message = ex.Message;
                ErrorsandMesseges.Ex = ex;

            }
            return ErrorsandMesseges;
        }
        public virtual IErrorsInfo SaveSetting()
        {
            try
            {
                ErrorsandMesseges.Flag = Errors.Ok;
                ErrorsandMesseges.Message = $"Function Executed";
            }
            catch (Exception ex)
            {
                ErrorsandMesseges.Flag = Errors.Failed;
                ErrorsandMesseges.Message = ex.Message;
                ErrorsandMesseges.Ex = ex;

            }
            return ErrorsandMesseges;
        }
        public IErrorsInfo CheckSystemEntryDataisSet()
        {
            ErrorsandMesseges.Flag = Errors.Ok;

            try
            {
                if (string.IsNullOrEmpty(DMEEditor.ConfigEditor.Config.SystemEntryFormName))
                {
                    ErrorsandMesseges.Flag = Errors.Failed;
                }
            }
            catch (System.Exception ex)
            {

                DMEEditor.AddLogMessage("Fail", $"Error check main System entry variables ({ex.Message})", DateTime.Now, 0, "", Errors.Failed);

            }
            return ErrorsandMesseges;
        }
        public IErrorsInfo ShowMainPage()
        {
            try
            {
                PassedArgs E = null;
                ErrorsandMesseges = new ErrorsInfo();
                ErrorsandMesseges = (ErrorsInfo)CheckSystemEntryDataisSet();
                IsShowingMainForm = true;
                if (ErrorsandMesseges.Flag == Errors.Ok)
                {
                    string[] args = { null, null, null };
                    if (DMEEditor.Passedarguments == null)
                    {
                        E = CreateDefaultArgsForVisUtil();
                    }
                    else
                    {
                        DMEEditor.Passedarguments.Objects = CreateArgsParameterForVisUtil(DMEEditor.Passedarguments.Objects);
                        E = (PassedArgs)DMEEditor.Passedarguments;
                    }
                    IsShowingMainForm = true;
                    MainFormView = ShowForm(DMEEditor.ConfigEditor.Config.SystemEntryFormName, DMEEditor, args, E);

                }
                IsShowingMainForm = false;
                ErrorsandMesseges.Flag = Errors.Ok;
                ErrorsandMesseges.Message = $"Function Executed";
            }
            catch (Exception ex)
            {
                ErrorsandMesseges.Flag = Errors.Failed;
                ErrorsandMesseges.Message = ex.Message;
                ErrorsandMesseges.Ex = ex;
            }
            return ErrorsandMesseges;
        }
        public IErrorsInfo ShowPage(string pagename, PassedArgs Passedarguments, DisplayType displayType = DisplayType.InControl, bool Singleton = false)
        {
            try
            {
                CurrentSingltonChecked = Singleton;
                ErrorsandMesseges = new ErrorsInfo();
                AddinAttribute attrib = new AddinAttribute();

                if (IsDataModified)
                {
                    if (Controlmanager.InputBoxYesNo("Beep", "Module/Data not Saved, Do you want to continue?") == DialogResult.No)
                    {
                        return ErrorsandMesseges;
                    }

                }
                CurrentDisplayedAddin = null;
                IsDataModified = false;
                if (DMEEditor.ConfigEditor.Addins.Where(c => c.className.Equals(pagename, StringComparison.OrdinalIgnoreCase)).Any())
                {
                    Type type = DMEEditor.ConfigEditor.Addins.Where(c => c.className.Equals(pagename, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().type;
                    attrib = (AddinAttribute)type.GetCustomAttribute(typeof(AddinAttribute), false);

                    if (attrib != null)
                    {
                        PassedArgs args = new PassedArgs();
                        args.Cancel = false;
                        args.ObjectName = attrib.Name;
                        args.ObjectType = attrib.ObjectType;
                        args.AddinName = attrib.Name;
                        PreCallModule?.Invoke(this, args);
                        if (args.Cancel)
                        {
                            DMEEditor.AddLogMessage("Beep Vis", $"You dont have Access Privilige on {pagename}", DateTime.Now, 0, pagename, Errors.Failed);
                            ErrorsandMesseges.Flag = Errors.Failed;
                            ErrorsandMesseges.Message = $"Function Access Denied";
                            return ErrorsandMesseges;
                        }
                        if (attrib.displayType == DisplayType.Popup)
                        {
                            displayType = DisplayType.Popup;
                        }


                        switch (attrib.addinType)
                        {
                            case AddinType.Form:
                                ShowForm(pagename, DMEEditor, new string[] { }, Passedarguments);
                                break;
                            case AddinType.Control:
                                if (displayType == DisplayType.InControl)
                                {
                                    ShowUserControlInContainer(pagename, DMEEditor, new string[] { }, Passedarguments);
                                }
                                else
                                {
                                    ShowUserControlPopUp(pagename, DMEEditor, new string[] { }, Passedarguments);
                                }
                                break;
                            case AddinType.Class:
                                RunAddinClass(pagename, DMEEditor, new string[] { }, Passedarguments);
                                break;
                            case AddinType.Page:
                                break;
                            case AddinType.Link:
                                break;
                            default:
                                break;
                        }
                    }
                    else
                        DMEEditor.AddLogMessage("Beep Vis", $"Could Find Attrib for Addin {pagename}", DateTime.Now, 0, pagename, Errors.Failed);
                }
                else
                    DMEEditor.AddLogMessage("Beep Vis", $"Could Find  Addin {pagename}", DateTime.Now, 0, pagename, Errors.Failed);
                ErrorsandMesseges.Flag = Errors.Ok;
                ErrorsandMesseges.Message = $"Function Executed";
            }
            catch (Exception ex)
            {
                ErrorsandMesseges.Flag = Errors.Failed;
                ErrorsandMesseges.Message = ex.Message;
                ErrorsandMesseges.Ex = ex;
                DMEEditor.AddLogMessage("Beep Vis", $"Error in Getting Addin {pagename}", DateTime.Now, 0, pagename, Errors.Failed);

            }
            return ErrorsandMesseges;
        }
        #region "Misc"
        private void Container_AddinChanged(object sender, ContainerEvents e)
        {

        }
        private void Container_AddinRemoved(object sender, ContainerEvents e)
        {
            if (addinsShowns.Any(o => o.Name == e.TitleText))
            {
                AddinsShownData t = addinsShowns.FirstOrDefault(o => o.Name == e.TitleText);
                addinsShowns.Remove(t);
            }
        }
        private List<ObjectItem> CreateArgsParameterForVisUtil(List<ObjectItem> e)
        {

            if (!e.Where(c => c.Name.Equals("VISUTIL", StringComparison.InvariantCultureIgnoreCase)).Any())
            {
                ObjectItem v = new ObjectItem { Name = "VISUTIL", obj = this };
                e.Add(v);
            }

            if (objects.Count > 0)
            {
                foreach (ObjectItem o in objects)
                {
                    if (!e.Where(c => c.Name.Equals(o.Name, StringComparison.InvariantCultureIgnoreCase)).Any())
                    {
                        ObjectItem v = new ObjectItem { Name = o.Name, obj = o.obj };
                        e.Add(v);
                    }
                }
                //IEnumerable<ObjectItem> diff = objects.Except<ObjectItem>(e);
                //if (diff.Any())
                //{
                //    foreach (ObjectItem item in diff)
                //    {
                //        e.Add(item);
                //    }

                //}
            }

            return e;
        }
        private PassedArgs CreateDefaultArgsForVisUtil()
        {
            List<ObjectItem> items = new List<ObjectItem>(); ;
            items = CreateArgsParameterForVisUtil(items);

            PassedArgs E = new PassedArgs { Objects = items };
            return E;
        }

        private Tuple<AddinsShownData, bool> GetShownData(string formname, Type type)
        {
            UserControl uc = new UserControl();
            bool IsJustAdded = false;
            bool AddinExist = false;
            int idx;
            AddinsShownData shownData = null;
            if (addinsShowns.Count == 0)
            {
                addinsShowns.Add(new AddinsShownData() { IsSingleton = CurrentSingltonChecked, IsAdded = true, Name = formname, Type = type.FullName });
                IsJustAdded = true;
            }
            if (!IsJustAdded)
            {
                idx = addinsShowns.FindIndex(p => p.Name.Equals(formname, StringComparison.InvariantCultureIgnoreCase));
                if (idx == -1)
                {
                    addinsShowns.Add(new AddinsShownData() { IsSingleton = CurrentSingltonChecked, IsAdded = true, Name = formname, Type = type.FullName });
                    IsJustAdded = true;
                    AddinExist = false;
                }
                else
                {
                    AddinExist = true;
                    IsJustAdded = false;
                }
            }
            idx = addinsShowns.FindIndex(p => p.Name.Equals(formname, StringComparison.InvariantCultureIgnoreCase));
            shownData = addinsShowns[idx];
            if (!IsJustAdded)
            {
                if (shownData != null && shownData.IsShown && !Container.IsControlExit(shownData.Addin))
                {
                    shownData.IsRemoved = true;
                }
                if (shownData.IsRemoved)
                {
                    if (idx != -1)
                    {
                        addinsShowns.Remove(addinsShowns[idx]);
                        shownData = null;
                        AddinExist = false;
                    }

                }
            }

            if (AddinExist && shownData.IsShown)
            {
                if (shownData.IsSingleton && shownData.IsShown)
                {
                    uc = (UserControl)shownData.Addin;
                    try
                    {
                        shownData.Addin.Run(DMEEditor.Passedarguments);
                    }
                    catch (Exception ex)
                    {


                    }
                    Container.ShowControl(shownData.Name, shownData.Addin);

                }

            }
            else
            {
                if (idx > -1)
                {
                    addinsShowns.Remove(addinsShowns[idx]);
                    AddinExist = false;
                }

                shownData = new AddinsShownData();
            }
            return new(shownData, AddinExist);
        }
        #endregion
        #region "Show Control/Form in Winforms Method"
        //--------------- User Control Show methods
        public IDM_Addin ShowUserControlInContainer(string usercontrolname, IDMEEditor pDMEEditor, string[] args, IPassedArgs e)
        {
            // string path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + @"\Addin\";
            if (DMEEditor.ConfigEditor.Addins.Where(c => c.className.Equals(usercontrolname, StringComparison.OrdinalIgnoreCase)).Any())
            {
                return ShowUserControlDialogOnControl(usercontrolname, (Control)Container, pDMEEditor, args, e);
            }
            else
            {
                return null;
            }

        }
        public IDM_Addin ShowUserControlPopUp(string usercontrolname, IDMEEditor pDMEEditor, string[] args, IPassedArgs e)
        {
            if (DMEEditor.ConfigEditor.Addins.Where(c => c.className.Equals(usercontrolname, StringComparison.OrdinalIgnoreCase)).Any())
            {
                // string path = DMEEditor.ConfigEditor.Addins.Where(c => c.PackageName.Equals(usercontrolname, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().;

                return ShowUserControlDialog(usercontrolname, pDMEEditor, args, e);
            }
            else
            {
                return null;
            }

        }
        private IDM_Addin ShowUserControlDialog(string formname, IDMEEditor pDMEEditor, string[] args, IPassedArgs e)
        {
            ErrorsandMesseges.Flag = Errors.Ok;
            BeepForm form = new BeepForm();
            // var path = Path.Combine(dllpath, dllname);
            IDM_Addin addin = null;
            if (e == null)
            {
                e = new PassedArgs();
            }
            try
            {
                // Assembly assembly = Assembly.LoadFile(path);
                //Type type = assembly.GetType(dllname + ".UserControls." + formname);
                Type type = DMEEditor.ConfigEditor.Addins.Where(c => c.className.Equals(formname, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().type;
                UserControl uc = (UserControl)Activator.CreateInstance(type);
                if (uc != null)
                {
                    addin = (IDM_Addin)uc;
                    if (e.Objects == null)
                    {
                        e.Objects = new List<ObjectItem>();
                    }
                    e.Objects.AddRange(CreateArgsParameterForVisUtil(DMEEditor.Passedarguments.Objects));
                    form.Text = addin.Details.AddinName;
                    // addin.SetConfig(pDMEEditor, DMEEditor.Logger, DMEEditor.Utilfunction, args, e, ErrorsandMesseges);
                    form.AddControl(uc, addin.Details.AddinName);
                    form.Width = uc.Width + 40;
                    form.Height = uc.Height + 40;
                    uc.Dock = DockStyle.Fill;
                    //     CurrentDisplayedAddin = addin;
                    IsDataModified = false;
                    form.Title = addin.Details.AddinName;
                    form.PreClose -= Form_PreClose;
                    form.PreClose += Form_PreClose;
                    form.AutoSize = true;
                    form.FormBorderStyle = FormBorderStyle.None;
                    form.WindowState = FormWindowState.Normal;
                    //   form.StartPosition = FormStartPosition.CenterParent;
                    form.Show(MainForm);
                }
                else
                {
                    DMEEditor.AddLogMessage("Fail", $"Error Could not Show UserControl {uc.Name}", DateTime.Now, 0, "", Errors.Failed);
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Fail", $"Error While Loading Assembly ({ex.Message})", DateTime.Now, 0, "", Errors.Failed);


            }
            return addin;
            //BeepWaitForm.GetType().GetField("")
        }
        private void Form_PreClose(object sender, FormClosingEventArgs e)
        {
            PassedArgs a = new PassedArgs();
            a.IsError = e.Cancel;
            PreClose?.Invoke(this, a);
        }

        private IDM_Addin ShowUserControlDialogOnControl(string formname, Control control, IDMEEditor pDMEEditor, string[] args, IPassedArgs e)
        {
            UserControl uc = new UserControl();
            ErrorsandMesseges.Flag = Errors.Ok;
            //Form BeepWaitForm = new Form();
            // var path = Path.Combine(dllpath, dllname);

            IDM_Addin addin = null;
            if (e == null)
            {
                e = new PassedArgs();
            }
            try
            {
                Type type = DMEEditor.ConfigEditor.Addins.Where(c => c.className.Equals(formname, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().type; //dllname.Remove(dllname.IndexOf(".")) + ".Forms." + formname
                AddinAttribute attrib = (AddinAttribute)type.GetCustomAttribute(typeof(AddinAttribute), false);
                Tuple<AddinsShownData, bool> Tdata = GetShownData(formname, type);
                AddinsShownData shownData = Tdata.Item1;
                bool AddinExist = Tdata.Item2;
                if (AddinExist && CurrentSingltonChecked)
                {
                    return shownData.Addin;
                }
                if (attrib != null)
                {
                    if (attrib.addinType != AddinType.Class)
                    {
                        uc = (UserControl)Activator.CreateInstance(type);
                        if (uc != null)
                        {
                            addin = (IDM_Addin)uc;
                            if (e.Objects == null)
                            {
                                e.Objects = new List<ObjectItem>();
                                e.Objects = CreateArgsParameterForVisUtil(DMEEditor.Passedarguments.Objects);
                            }
                            else
                            {
                                e.Objects = CreateArgsParameterForVisUtil(e.Objects);
                            }

                            // addin.SetConfig(pDMEEditor, DMEEditor.Logger, DMEEditor.Utilfunction, args, e, ErrorsandMesseges);
                            CurrentDisplayedAddin = addin;
                            IsDataModified = false;
                            container = (IDisplayContainer)control;
                            string title = null;
                            if (e.Objects.Any(o => o.Name.Equals("TitleText", StringComparison.CurrentCultureIgnoreCase)))
                            {
                                ObjectItem x = e.Objects.First(o => o.Name.Equals("TitleText", StringComparison.CurrentCultureIgnoreCase));
                                title = (string)x.obj;
                                e.Objects.Remove(x);
                            }
                            else
                                title = addin.Details.AddinName;
                            container.AddControl(title, addin, ContainerTypeEnum.TabbedPanel);
                            if (addinsShowns.Count > 0 && CurrentSingltonChecked)
                            {
                                int idx = addinsShowns.FindIndex(p => p.Name.Equals(formname, StringComparison.InvariantCultureIgnoreCase));
                                if (idx > -1)
                                {
                                    shownData = addinsShowns[idx];
                                }
                            }
                            shownData.Addin = addin;
                            shownData.IsShown = true;
                            //  uc.Dock = DockStyle.Fill;
                            try
                            {
                                addin.Run(e);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        else
                        {
                            DMEEditor.AddLogMessage("Fail", $"Error Could not Show UserControl {uc.Name}", DateTime.Now, 0, "", Errors.Failed);
                        }
                    }
                    else
                    {
                        addin = (IDM_Addin)Activator.CreateInstance(type);
                        if (e.Objects == null)
                        {
                            e.Objects = new List<ObjectItem>();
                            e.Objects = CreateArgsParameterForVisUtil(DMEEditor.Passedarguments.Objects);
                        }
                        else
                        {
                            e.Objects = CreateArgsParameterForVisUtil(e.Objects);
                        }

                        //   addin.Details.SetConfig(pDMEEditor, DMEEditor.Logger, DMEEditor.Utilfunction, args, e, ErrorsandMesseges);
                        CurrentDisplayedAddin = addin;
                        IsDataModified = false;
                        if (addinsShowns.Count > 0 && CurrentSingltonChecked)
                        {
                            int idx = addinsShowns.FindIndex(p => p.Name.Equals(formname, StringComparison.InvariantCultureIgnoreCase));
                            if (idx > -1)
                            {
                                shownData = addinsShowns[idx];
                            }
                        }
                        shownData.Addin = addin;
                        shownData.IsShown = true;
                        addin.Run(e);
                    }
                }

            }
            catch (Exception ex)
            {

                DMEEditor.AddLogMessage("Fail", $"Error While Loading Assembly ({ex.Message})", DateTime.Now, 0, "", Errors.Failed);
            }

            return addin;
        }
        //-----------------------------------------
        private IDM_Addin ShowForm(string formname, IDMEEditor pDMEEditor, string[] args, IPassedArgs e)
        {
            if (DMEEditor.ConfigEditor.Addins.Where(c => c.className.Equals(formname, StringComparison.OrdinalIgnoreCase)).Any())
            {
                return ShowFormDialog(formname, pDMEEditor, args, e);
            }
            else
            {
                return null;
            }
        }
        private IDM_Addin ShowFormDialog(string formname, IDMEEditor pDMEEditor, string[] args, IPassedArgs e)
        {
            Form form = null;
            IDM_Addin addin = null;


            ErrorsandMesseges.Flag = Errors.Ok;
            if (e == null)
            {
                e = new PassedArgs();
            }
            try
            {
                Type type = DMEEditor.ConfigEditor.Addins.Where(c => c.className.Equals(formname, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().type; //dllname.Remove(dllname.IndexOf(".")) + ".Forms." + formname
                AddinAttribute attrib = (AddinAttribute)type.GetCustomAttribute(typeof(AddinAttribute), false);
                Tuple<AddinsShownData, bool> Tdata = GetShownData(formname, type);
                AddinsShownData shownData = Tdata.Item1;
                bool AddinExist = Tdata.Item2;
                if (AddinExist && !IsShowingMainForm)
                {
                    return shownData.Addin;
                }
                form = (Form)Activator.CreateInstance(type);
                if (form != null)
                {
                    addin = (IDM_Addin)form;
                    if (e.Objects == null)
                    {
                        e.Objects = new List<ObjectItem>();
                        e.Objects = CreateArgsParameterForVisUtil(DMEEditor.Passedarguments.Objects);
                    }
                    else
                    {
                        e.Objects = CreateArgsParameterForVisUtil(e.Objects);
                    }
                    //    addin.SetConfig(pDMEEditor, DMEEditor.Logger, DMEEditor.Utilfunction, args, e, ErrorsandMesseges);
                    form.Text = addin.Details.AddinName;
                    IsDataModified = false;
                    CurrentDisplayedAddin = addin;
                    form.ShowInTaskbar = true;
                    if (IsShowingMainForm)
                    {
                        MainForm = form;
                        form.Shown += Form_Shown;
                        form.FormClosing += Form_FormClosing;
                        MainForm.StartPosition = FormStartPosition.CenterScreen;
                        //  MainForm.TopMost = true;
                        MainForm.WindowState = FormWindowState.Normal;
                        MainForm.TopMost = true;
                        form.Width = Width;
                        form.Height = Height;
                        MainDisplay = (IBeepUIComponent)addin;
                        if (!string.IsNullOrEmpty(Title))
                        {
                            form.Text = Title;
                        }
                        if (!string.IsNullOrEmpty(IconUrl) && visHelper.LogoSmallImage != null)
                        {
                            form.Icon = (Icon?)visHelper.LogoSmallImage;

                        }
                        if (!string.IsNullOrEmpty(HomePageName))
                        {
                            ShowPage(HomePageName, (PassedArgs)e, DisplayType.InControl, true);
                        }
                        IsShowingMainForm = false;

                    }
                    else
                    {
                        if (addinsShowns.Count > 0 && CurrentSingltonChecked)
                        {
                            int idx = addinsShowns.FindIndex(p => p.Name.Equals(formname, StringComparison.InvariantCultureIgnoreCase));
                            if (idx > -1)
                            {
                                shownData = addinsShowns[idx];
                            }
                        }
                        shownData.Addin = addin;
                        shownData.IsShown = true;
                        form.StartPosition = FormStartPosition.CenterParent;
                    }

                    try
                    {
                        form.Activate();
                        form.Focus();
                        form.ShowDialog();


                        //   SetForegroundWindow(form.Handle);

                    }
                    catch (Exception ex)
                    {

                        ErrorsandMesseges.Flag = Errors.Failed;
                        ErrorsandMesseges.Message = $"Error Could not Show Form {form.Name}";
                        DMEEditor.AddLogMessage("Fail", $"Error Could not Show Form {form.Name}", DateTime.Now, 0, "", Errors.Failed);
                    }


                }
                else
                {
                    ErrorsandMesseges.Flag = Errors.Failed;
                    ErrorsandMesseges.Message = $"Error Could not Show Form {form.Name}";
                    DMEEditor.AddLogMessage("Fail", $"Error Could not Show Form {form.Name}", DateTime.Now, 0, "", Errors.Failed);
                };
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Fail", $"Error While Loading Assembly ({ex.Message})", DateTime.Now, 0, "", Errors.Failed);
            }

            return addin;
            //BeepWaitForm.GetType().GetField("")
        }

        private void Form_FormClosing(object? sender, FormClosingEventArgs e)
        {
            IsLogOn = false;
        }

        private void Form_Shown(object? sender, EventArgs e)
        {
            IsLogOn = true;
            MainDisplayForm.IsLogPanelShown = false;

        }

        //---------------- Run Class Addin -----------------
        private IDM_Addin RunAddinClass(string pclassname, IDMEEditor pDMEEditor, string[] args, IPassedArgs e)
        {
            IDM_Addin addin = null;

            ErrorsandMesseges.Flag = Errors.Ok;

            if (e == null)
            {
                e = new PassedArgs();
            }
            try
            {
                Type type = DMEEditor.ConfigEditor.Addins.Where(c => c.className.Equals(pclassname, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().type; //dllname.Remove(dllname.IndexOf(".")) + ".Forms." + formname
                AddinAttribute attrib = (AddinAttribute)type.GetCustomAttribute(typeof(AddinAttribute), false);
                if (attrib != null)
                {
                    if (attrib.addinType == AddinType.Class)
                    {
                        addin = (IDM_Addin)Activator.CreateInstance(type);
                        if (e.Objects == null)
                        {
                            e.Objects = new List<ObjectItem>();
                            e.Objects = CreateArgsParameterForVisUtil(DMEEditor.Passedarguments.Objects);
                        }
                        else

                        {
                            e.Objects = CreateArgsParameterForVisUtil(e.Objects);
                        }
                        //   addin.SetConfig(pDMEEditor, DMEEditor.Logger, DMEEditor.Utilfunction, args, e, ErrorsandMesseges);
                        CurrentDisplayedAddin = addin;
                        IsDataModified = false;
                        addin.Run(e);

                    }
                }

            }
            catch (Exception ex)
            {
                CurrentDisplayedAddin = null;
                IsDataModified = false;
                DMEEditor.AddLogMessage("Fail", $"Error While Loading Assembly ({ex.Message})", DateTime.Now, 0, "", Errors.Failed);
            }

            return addin;
            //BeepWaitForm.GetType().GetField("")
        }
        //--------------------------------------------------

        #endregion
        #region "Wait Forms"
        delegate void SetTextCallback(Form f, TextBox ctrl, string text);
        /// <summary>
        /// Set text property of various controls
        /// </summary>
        /// <param name="form">The calling BeepWaitForm</param>
        /// <param name="ctrl"></param>
        /// <param name="text"></param>
        public static void SetText(Form form, BeepTextBox ctrl, string text)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 

            //if (ctrl.InvokeRequired)
            //{
            //    SetTextCallback d = new SetTextCallback(SetText);
            //    BeepWaitForm.Invoke(d, new object[] { BeepWaitForm, ctrl, text });
            //}
            //else
            //{
            //  ctrl.Text = text;
            ctrl.BeginInvoke(new Action(() =>
            {
                ctrl.AppendText(text + Environment.NewLine);
                ctrl.SelectionStart = ctrl.Text.Length;
                ctrl.ScrollToCaret();
            }));
            //}
        }
        private async void startwait(PassedArgs Passedarguments)
        {
            string[] args = null;
            BeepWaitForm = (BeepWait)Application.OpenForms["BeepWait"];
            if (BeepWaitForm != null)
            {
                CloseWaitForm();
            }
            await Task.Run(() =>
            {
                BeepWaitForm = new BeepWait();
                if (!string.IsNullOrEmpty(Title))
                {
                    BeepWaitForm.Title.Text = Title;
                }
                //Debug.WriteLine($"Getting Logourl {LogoUrl}");
                //if (!string.IsNullOrEmpty(LogoUrl))
                //{
                //    ImagePath logurl = (ImagePath)visHelper.LogoBigImage;
                //    Debug.WriteLine($"found or not = {logurl}");

                //    BeepWaitForm.SetImage(logurl);
                //}
                //Debug.WriteLine($"not found logurl");
                BeepWaitForm.TopMost = true;
                // Form frm = (Form)MainFormView;
                BeepWaitForm.StartPosition = FormStartPosition.CenterScreen;
                // BeepWaitForm.ParentNode = frm;
                BeepWaitForm.ShowDialog();

            });
        }
        public virtual IErrorsInfo ShowWaitForm(PassedArgs Passedarguments)
        {
            try
            {
                BeepWaitForm = (BeepWait)Application.OpenForms["BeepWait"];
                if (BeepWaitForm != null)
                {
                    IsShowingWaitForm = false;
                    CloseWaitForm();
                }


                ErrorsandMesseges = new ErrorsInfo();

                startwait(Passedarguments);
                IsShowingWaitForm = true;
                WaitFormShown = true;
                while ((BeepWait)Application.OpenForms["BeepWait"] == null) Application.DoEvents();
                BeepWaitForm = (BeepWait)Application.OpenForms["BeepWait"];
                // BeepWaitForm.SetTitle(Title);

            }
            catch (Exception ex)
            {
                ErrorsandMesseges.Flag = Errors.Failed;
                ErrorsandMesseges.Message = ex.Message;
                ErrorsandMesseges.Ex = ex;

            }
            return ErrorsandMesseges;
        }
        public virtual IErrorsInfo PasstoWaitForm(PassedArgs Passedarguments)
        {
            try
            {

                ErrorsandMesseges = new ErrorsInfo();
                BeepWaitForm = (BeepWait)Application.OpenForms["BeepWait"];
                if (BeepWaitForm != null)
                {
                    SetText(BeepWaitForm, BeepWaitForm.messege, Passedarguments.Messege);
                    WaitFormShown = true;
                    IsShowingWaitForm = true;
                }

            }
            catch (Exception ex)
            {
                ErrorsandMesseges.Flag = Errors.Failed;
                ErrorsandMesseges.Message = ex.Message;
                ErrorsandMesseges.Ex = ex;
            }
            return ErrorsandMesseges;
        }
        public virtual IErrorsInfo CloseWaitForm()
        {
            try
            {
                BeepWaitForm = (BeepWait)Application.OpenForms["BeepWait"];

                if (BeepWaitForm != null)
                {
                    System.Windows.Forms.MethodInvoker action = delegate ()
                     {
                         BeepWaitForm.CloseForm();
                         WaitFormShown = false;
                         IsShowingWaitForm = false;
                     };

                    if (BeepWaitForm.InvokeRequired)
                    {
                        BeepWaitForm.Invoke(action);
                    }
                    else
                    {
                        action();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorsandMesseges.Flag = Errors.Failed;
                ErrorsandMesseges.Message = ex.Message;
                ErrorsandMesseges.Ex = ex;
            }
            return ErrorsandMesseges;
        }
        public IErrorsInfo ShowLogin()
        {
            try
            {
                // use the view router to show the home page

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }
        public IErrorsInfo ShowProfile()
        {
            try
            {
                // use the view router to show the home page

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }
        public IErrorsInfo ShowAdmin()
        {
            try
            {
                // use the view router to show the home page

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }

        #endregion
        #region "Resource Loaders"

        #endregion
        public IErrorsInfo PrintGrid(IPassedArgs e)
        {
            try
            {
                ErrorsandMesseges = new ErrorsInfo();
                // List<ReportColumn> columns = new List<ReportColumn>();
                object obj = null;
                object obj2 = null;
                DataGridView dv = null;
                DataTable dt = new DataTable();
                //if (e.Objects.Where(c => c.Name == "ReportColumn").Any())
                //{
                //    columns = (List<ReportColumn>)e.Objects.FirstOrDefault(c => c.Name == "ReportColumn").obj;

                //}
                if (e.Objects.Where(c => c.Name == "DATA").Any())
                {
                    obj = (object)e.Objects.FirstOrDefault(c => c.Name == "DATA").obj;

                }
                if (e.Objects.Where(c => c.Name == "DataGridView").Any())
                {
                    obj2 = (object)e.Objects.FirstOrDefault(c => c.Name == "DataGridView").obj;

                }
                //var f = new ReportForm();
                //f.ReportColumns = columns;
                //f.ReportData = obj;
                //f.ShowDialog();
                //DataGrid grid = new DataGrid();
                dv = (DataGridView)obj2;
                dt = (DataTable)obj;
                // DataGridPrinting pr = new DataGridPrinting(e.CurrentEntity, dv, dt);
                //pr.Print();

                //PrintingSystem printingSystem = new PrintingSystem();
                //DataGridLink dgLink = new DataGridLink();
                //dgLink.DataGrid = grid;

                //printingSystem.Links.Add(dgLink);
                //dgLink.ShowPreviewDialog();

                ErrorsandMesseges.Flag = Errors.Ok;
                ErrorsandMesseges.Message = $"Function Executed";
            }
            catch (Exception ex)
            {
                ErrorsandMesseges.Flag = Errors.Failed;
                ErrorsandMesseges.Message = ex.Message;
                ErrorsandMesseges.Ex = ex;

            }
            return ErrorsandMesseges;

        }
        public IErrorsInfo CallAddinRun()
        {
            try
            {
                if (CurrentDisplayedAddin != null)
                {
                    CurrentDisplayedAddin.Run(DMEEditor.Passedarguments);
                }
            }
            catch (Exception ex)
            {
                DMEEditor.ErrorObject.Ex = ex;
                DMEEditor.ErrorObject.Message = ex.Message;
                DMEEditor.ErrorObject.Flag = Errors.Failed;
            }

            return DMEEditor.ErrorObject;
        }
        public IErrorsInfo CloseAddin()
        {
            try
            {
                if (CurrentDisplayedAddin != null)
                {
                    Form frm = (Form)CurrentDisplayedAddin;
                    frm.Close();
                }
            }
            catch (Exception ex)
            {
                DMEEditor.ErrorObject.Ex = ex;
                DMEEditor.ErrorObject.Message = ex.Message;
                DMEEditor.ErrorObject.Flag = Errors.Failed;
            }

            return DMEEditor.ErrorObject;
        }
        public IErrorsInfo ShowHome()
        {
            try
            {

                if (!string.IsNullOrEmpty(HomePageName))
                {
                    ShowPage(HomePageName, (PassedArgs)DMEEditor.Passedarguments, DisplayType.InControl, true);
                }


            }
            catch (Exception ex)
            {
                DMEEditor.ErrorObject.Ex = ex;
                DMEEditor.ErrorObject.Message = ex.Message;
                DMEEditor.ErrorObject.Flag = Errors.Failed;
            }

            return DMEEditor.ErrorObject;
        }
        public string HomePageName { get; set; }
        public string HomePageTitle { get; set; }
        public string HomePageDescription { get; set; }
        public IProfile DefaultProfile { get; set; }
        public List<IBeepPrivilege> Privileges { get; set; } = new List<IBeepPrivilege>();
        public List<IBeepUser> Users { get; set; } = new List<IBeepUser>();

        public string BreadCrumb { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Tree = null;
                    SecondaryTree = null;
                    ToolStrip = null;
                    MenuStrip = null;
                    SecondaryMenuStrip = null;
                    SecondaryToolStrip = null;
                    foreach (var item in addinsShowns)
                    {
                        item.Addin = null;
                    }
                    if (container != null)
                    {
                        container.Clear();
                    }

                    MainFormView = null;

                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~VisManager()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public virtual void PrintData(object data)
        {

        }

        public virtual void Notify(object data)
        {

        }

        public virtual void Email(object data)
        {

        }

        public virtual void Ticket(object data)
        {

        }

        public void NavigateBack()
        {
            throw new NotImplementedException();
        }

        public void NavigateForward()
        {
            throw new NotImplementedException();
        }

        public void NavigateTo(string routeName, Dictionary<string, object> parameters = null)
        {
            throw new NotImplementedException();
        }
    }
}

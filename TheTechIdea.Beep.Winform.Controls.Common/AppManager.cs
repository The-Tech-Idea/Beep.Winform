using System.ComponentModel;
using System.Reflection;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;
using DialogResult = TheTechIdea.Beep.Vis.Modules.DialogResult;

namespace TheTechIdea.Beep.Desktop.Common
{
    public class AppManager : IAppManager
    {
        #region "Variables"
        private readonly IBeepService beepservices;
        private readonly IRoutingManager viewrouter;
        private RoutingManager ViewRouter;
        #endregion "Variables"
        #region "Constructors and Init"
        public AppManager(IBeepService service, IRoutingManager viewRouter)
        {
            beepservices = service;
            viewrouter = viewRouter;
           
        }
        public void init()
        {
            ErrorsandMesseges = new ErrorsInfo();
            Addins = new List<IDM_Addin>();
            IsDataModified = false;
            IsShowingMainForm = false;
            IsShowingWaitForm = false;
            IsBeepDataOn = false;
            IsAppOn = false;
            IsDevModeOn = false;
            IsinCaptureMenuMode = false;
            AppObjectsName = "Beep";
            SecondaryTreeIconSize = 16;
            SecondaryTreeExpand = false;
            TreeIconSize = 16;
            TreeExpand = false;
            LogoUrl = "";
            Title = "Beep";
            IconUrl = "";
            ShowLogWindow = false;
            ShowTreeWindow = false;
            Width = 800;
            Height = 600;
            Theme = EnumBeepThemes.DefaultTheme;
            IsLogOn = false;
            HomePageTitle = "Home";
            HomePageName = "Home";
            HomePageDescription = "Home Page";
            Privileges = new List<IBeepPrivilege>();
            Users = new List<IBeepUser>();
            User = new BeepUser();
        }
        #endregion "Constructors and Init"
        #region "Properties"
        public IDMEEditor DMEEditor { get; set; }
        public ErrorsInfo ErrorsandMesseges { get; set; }
        #region "Main Controls"
        public List<IDM_Addin> Addins { get; set; }
        public IBeepUIComponent ToolStrip { get; set; }
        public IBeepUIComponent SecondaryToolStrip { get; set; }
        public IBeepUIComponent Tree { get; set; }
        public IBeepUIComponent SecondaryTree { get; set; }
        public IBeepUIComponent MenuStrip { get; set; }
        public IBeepUIComponent SecondaryMenuStrip { get; set; }
        public IDM_Addin CurrentDisplayedAddin { get; set; }
        public IBeepUIComponent MainDisplay { get; set; }
        public IWaitForm WaitForm { get; set; }
        #endregion "Main Controls"
        #region "Helpers"
        public IDisplayContainer Container { get; set; }
        public IControlManager Controlmanager { get; set; }
        #endregion "Helpers"
        #region "UI State"
        public bool IsDataModified { get; set; }
        public bool IsShowingMainForm { get; set; }
        public bool IsShowingWaitForm { get; set; }
        public bool ShowSideBarWindow { get; set; }
        public bool IsBeepDataOn { get; set; }
        public bool IsAppOn { get; set; }
        public bool IsDevModeOn { get; set; }
        public bool IsinCaptureMenuMode { get; set; }
        public string AppObjectsName { get; set; }
        public string BeepObjectsName { get; set; } = "Beep";
        public int SecondaryTreeIconSize { get; set; }
        public bool SecondaryTreeExpand { get; set; }
        public int TreeIconSize { get; set; }
        public bool TreeExpand { get; set; }
        public string LogoUrl { get; set; }
        public string Title { get; set; }
        public string IconUrl { get; set; }
        public bool ShowLogWindow { get; set; }
        public bool ShowTreeWindow { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string BreadCrumb { get { return viewrouter != null ? viewrouter?.BreadCrumb : string.Empty; } }
        protected EnumBeepThemes _themeEnum = EnumBeepThemes.DefaultTheme;
        protected BeepTheme _currentTheme = BeepThemesManager.DefaultTheme;

        public EnumBeepThemes Theme
        {
            get => _themeEnum;
            set
            {
                _themeEnum = value;
                _currentTheme = BeepThemesManager.GetTheme(value);
                //      this.ApplyTheme();
                ApplyTheme();
            }
        }
        #endregion "UI State"
        #region "User and Profile"

        public bool IsLogOn { get; set; }
        public string HomePageTitle { get; set; }
        public string HomePageName { get; set; }
        public string HomePageDescription { get; set; }
        public IProfile DefaultProfile { get; set; }
        public List<IBeepPrivilege> Privileges { get; set; }
        public List<IBeepUser> Users { get; set; }
        public IBeepUser User { get; set; }
        #endregion "User and Profile"
        #endregion "Properties"
        #region "Events"
        public event EventHandler<KeyCombination> KeyPressed;
        public event EventHandler<IPassedArgs> PreLogin;
        public event EventHandler<IPassedArgs> PostLogin;
        public event EventHandler<IPassedArgs> PreClose;
        public event EventHandler<IPassedArgs> PreCallModule;
        public event EventHandler<IPassedArgs> PreShowItem;
        public event EventHandler<IPassedArgs> PostShowItem;
        public event EventHandler<IPassedArgs> PostCallModule;
        #endregion "Events"
        #region "Methods"
        #region "Loading and Initialization"
        public async Task<IErrorsInfo> LoadGraphics(string[] namespacestoinclude)
        {
            try
            {
                if (namespacestoinclude == null)
                {
                    namespacestoinclude = new string[3] { "BeepEnterprize", "TheTechIdea", "Beep" };
                }
                // Load Graphics from Embedded Resources
                ImageListHelper.GetGraphicFilesLocationsFromEmbedded(namespacestoinclude);
                // Load Graphics from Folders
                ImageListHelper.GetGraphicFilesLocations(DMEEditor.ConfigEditor.Config.Folders.Where(x => x.FolderFilesType == FolderFileTypes.GFX).FirstOrDefault().FolderPath);
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }
        public async Task<IErrorsInfo> LoadAssemblies(string[] namespacestoinclude)
        {
            try
            { //    visManager.SetMainDisplay("Form1", "Beep - The Data Plaform", "SimpleODM.ico", "", "", "");
                PassedArgs p = new PassedArgs();
                p.Messege = "Loading DLL's";
                // Show Wait Form
                ShowWaitForm(p);
                // Passing Message to WaitForm
                PasstoWaitForm(p);
                // Prepare Async Data Notification from Assembly loader to WaitForm
                var progress = new Progress<PassedArgs>(percent =>
                {
                    p.Messege = percent.Messege;
                    PasstoWaitForm(p);
                });
                // Load Assemblies from folders (DataSources,Drivers, Extensions,...)
                beepservices.LoadAssemblies(progress);
                // Load Assemblies from folders (DataSources,Drivers, Extensions,...)
               
                beepservices.Config_editor.LoadedAssemblies = beepservices.LLoader.Assemblies.Select(c => c.DllLib).ToList();
                p.Messege = "Loading DLL's Completed";
                PasstoWaitForm(p);
                // Load Graphics
                p.Messege = "Loading Graphics";
                PasstoWaitForm(p);
                LoadGraphics(namespacestoinclude);
                p.Messege = "Loading Graphics Completed";
                PasstoWaitForm(p);
                CloseWaitForm();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
                CloseWaitForm();
            }
            return DMEEditor.ErrorObject;
        }
      
        #endregion "Loading and Initialization"
        #region "Addin Management"
      
        #endregion "Addin Management"
        #region "WaitForm"

        /// <summary>
        /// Displays the wait form with the specified arguments.
        /// </summary>
        /// <param name="Passedarguments">Arguments to configure the wait form.</param>
        /// <returns>Operation result with status.</returns>
        public IErrorsInfo ShowWaitForm(PassedArgs Passedarguments)
        {
            var result = new ErrorsInfo();
            try
            {
                if (WaitForm == null)
                {
                    throw new InvalidOperationException("WaitForm is not initialized.");
                }

                if (!string.IsNullOrEmpty(Passedarguments?.Messege))
                {
                    WaitForm.SetText(Passedarguments.Messege);
                }

                if (!string.IsNullOrEmpty(Passedarguments?.Title))
                {
                    WaitForm.SetTitle(Passedarguments.Title);
                }

                if (!string.IsNullOrEmpty(Passedarguments?.ImagePath))
                {
                    WaitForm.SetImage(Passedarguments.ImagePath);
                }

                WaitForm.Show(Passedarguments);
                IsShowingWaitForm = true;

                result.Flag = Errors.Ok;
                result.Message = "Wait form displayed successfully.";
            }
            catch (Exception ex)
            {
                string methodName = nameof(ShowWaitForm);
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error: {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
                result.Flag = Errors.Failed;
                result.Message = ex.Message;
                result.Ex = ex;
            }
            return result;
        }

        /// <summary>
        /// Updates the wait form with progress and message.
        /// </summary>
        /// <param name="Passedarguments">Arguments containing progress and message.</param>
        /// <returns>Operation result with status.</returns>
        public IErrorsInfo PasstoWaitForm(PassedArgs Passedarguments)
        {
            var result = new ErrorsInfo();
            try
            {
                if (WaitForm == null)
                {
                    throw new InvalidOperationException("WaitForm is not initialized.");
                }

                if (!string.IsNullOrEmpty(Passedarguments?.Messege))
                {
                    WaitForm.UpdateProgress(Passedarguments.Progress , Passedarguments.Messege);
                }

                result.Flag = Errors.Ok;
                result.Message = "Progress passed to wait form successfully.";
            }
            catch (Exception ex)
            {
                string methodName = nameof(PasstoWaitForm);
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error: {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
                result.Flag = Errors.Failed;
                result.Message = ex.Message;
                result.Ex = ex;
            }
            return result;
        }

        /// <summary>
        /// Closes the wait form.
        /// </summary>
        /// <returns>Operation result with status.</returns>
        public IErrorsInfo CloseWaitForm()
        {
            var result = new ErrorsInfo();
            try
            {
                if (WaitForm == null)
                {
                    throw new InvalidOperationException("WaitForm is not initialized.");
                }

                WaitForm.Close();
                IsShowingWaitForm = false;

                result.Flag = Errors.Ok;
                result.Message = "Wait form closed successfully.";
            }
            catch (Exception ex)
            {
                string methodName = nameof(CloseWaitForm);
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error: {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
                result.Flag = Errors.Failed;
                result.Message = ex.Message;
                result.Ex = ex;
            }
            return result;
        }

        #endregion "WaitForm"
        #region "Settings Management"
        public IErrorsInfo SaveSetting()
        {
            try
            {
                // Example control GUIDs and property names
                string controlGuid = "AppManager"; // You can define a unique identifier as needed

                // Save settings
                StoreSettings.Set(controlGuid, "Title", Title);
                StoreSettings.Set(controlGuid, "Width", Width);
                StoreSettings.Set(controlGuid, "Height", Height);
                StoreSettings.Set(controlGuid, "Theme", Theme);
                StoreSettings.Set(controlGuid, "HomePageTitle", HomePageTitle);
                StoreSettings.Set(controlGuid, "HomePageName", HomePageName);
                StoreSettings.Set(controlGuid, "HomePageDescription", HomePageDescription);
                StoreSettings.Set(controlGuid, "IsLogOn", IsLogOn);
                StoreSettings.Set(controlGuid, "ShowLogWindow", ShowLogWindow);
                StoreSettings.Set(controlGuid, "ShowTreeWindow", ShowTreeWindow);
                // Add more properties as needed

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }
        public IErrorsInfo LoadSetting()
        {
            try
            {
                // Example control GUIDs and property names
                string controlGuid = "AppManager"; // You can define a unique identifier as needed

                // Load settings
                Title = StoreSettings.Get<string>(controlGuid, "Title", Title);
                Width = StoreSettings.Get<int>(controlGuid, "Width", Width);
                Height = StoreSettings.Get<int>(controlGuid, "Height", Height);
                Theme = StoreSettings.Get<EnumBeepThemes>(controlGuid, "Theme", Theme);
                HomePageTitle = StoreSettings.Get<string>(controlGuid, "HomePageTitle", HomePageTitle);
                HomePageName = StoreSettings.Get<string>(controlGuid, "HomePageName", HomePageName);
                HomePageDescription = StoreSettings.Get<string>(controlGuid, "HomePageDescription", HomePageDescription);
                IsLogOn = StoreSettings.Get<bool>(controlGuid, "IsLogOn", IsLogOn);
                ShowLogWindow = StoreSettings.Get<bool>(controlGuid, "ShowLogWindow", ShowLogWindow);
                ShowTreeWindow = StoreSettings.Get<bool>(controlGuid, "ShowTreeWindow", ShowTreeWindow);
                // Add more properties as needed
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }
        #endregion "Settings Management"
        #region "Notification"
        public void Email(object data)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            
        }
        public void Ticket(object data)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
           
        }
        public IErrorsInfo PressKey(KeyCombination keyCombination)
        {
            try
            {
                if (Tree != null)
                {
                    ITree r = (ITree)Tree;
                    AssemblyClassDefinition s = DMEEditor.ConfigEditor.AppComponents
                        .FirstOrDefault(p => p.GuidID == keyCombination.AssemblyGuid);
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
        public void Notify(object data)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
          
        }
        #endregion "Notification"
        #region "Navigation"
        public void NavigateBack()
        {
            try
            {
                // use the view router to navigate back

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
          
        }
        public void NavigateForward()
        {
            try
            {
                // use the view router to navigate forward


            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
           
        }
        public void NavigateTo(string routeName, Dictionary<string, object> parameters = null)
        {
            try
            {
                // use the view router to navigate to a specific route from List<IDM_Addin> s


            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            
        }
        public IErrorsInfo ShowHome()
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
     
        public IErrorsInfo ShowPage(string pagename, PassedArgs Passedarguments, DisplayType displayType = DisplayType.InControl, bool Singleton = false)
        {
            try
            {
               
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
                                GetAddinClass(pagename, DMEEditor, new string[] { }, Passedarguments);
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
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }

        private IDM_Addin ShowForm(string pagename, IDMEEditor dMEEditor, string[] strings, PassedArgs passedarguments)
        {
            IDM_Addin addin = null;
            try
            {

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            return addin;
        }

        private IDM_Addin ShowUserControlInContainer(string pagename, IDMEEditor dMEEditor, string[] strings, PassedArgs passedarguments)
        {
            IDM_Addin addin = null;
            try
            {

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            return addin;
        }

        public IDM_Addin ShowUserControlPopUp(string usercontrolname, IDMEEditor pDMEEditor, string[] args, IPassedArgs e)
        {
            IDM_Addin addin = null;
            try
            {

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            return addin;
        }
        //---------------- Run Class Addin -----------------
        private IDM_Addin GetAddinClass(string pclassname, IDMEEditor pDMEEditor, string[] args, IPassedArgs e)
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
        #endregion "Navigation"
        #region "Printing and Exporting"
        public void PrintData(object data)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            
        }

        public IErrorsInfo PrintGrid(IPassedArgs passedArgs)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                DMEEditor.AddLogMessage("Beep",$"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;

        }
        #endregion "Printing and Exporting"
        #endregion "Methods"
        #region "Theme Management"
        private bool HasThemeProperty(Control control)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["Theme"];
            if (themeProperty != null && themeProperty.PropertyType == typeof(EnumBeepThemes))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void ApplyTheme()
        {
            try
            {
                foreach (var item in Addins)
                {
                    // check if item has property Theme and apply the theme
                    Control control = (Control)item;
                    if (HasThemeProperty(control))
                    {
                        var themeProperty = TypeDescriptor.GetProperties(control)["Theme"];
                        if (themeProperty != null && themeProperty.PropertyType == typeof(EnumBeepThemes))
                        {
                            themeProperty.SetValue(control, _currentTheme);
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            
        }
        #endregion "Theme Management"
        #region "Events Handling"
        /// <summary>
        /// Handles the form closing event.
        /// </summary>
        private void Form_PreClose(object sender, FormClosingEventArgs e)
        {
            PassedArgs a = new PassedArgs
            {
                IsError = e.Cancel
            };
            PreClose?.Invoke(this, a);
        }
        /// <summary>
        /// Handles the form closing event.
        /// </summary>
        private void Form_FormClosing(object? sender, FormClosingEventArgs e)
        {
            IsLogOn = false;
        }

        /// <summary>
        /// Handles the form shown event.
        /// </summary>
        private void Form_Shown(object? sender, EventArgs e)
        {
            IsLogOn = true;
            
        }
        #endregion "Events Handling"

        #region "Dispose"
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~AppManager()
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
        #endregion "Dispose"

    }
}

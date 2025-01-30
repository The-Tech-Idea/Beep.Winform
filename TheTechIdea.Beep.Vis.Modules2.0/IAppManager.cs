
using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IAppManager : IDisposable
    {
        event Action<EnumBeepThemes> OnThemeChanged;
        EnumBeepThemes Theme { get; set; }
        bool IsLogOn { get; set; }
        IDMEEditor DMEEditor { get; set; }
        ErrorsInfo ErrorsandMesseges { get; set; }
        IControlManager Controlmanager { get; set; }
        IBeepUIComponent ToolStrip { get; set; }
        IBeepUIComponent SecondaryToolStrip { get; set; }
        IBeepUIComponent Tree { get; set; }
        IBeepUIComponent SecondaryTree { get; set; }
        IBeepUIComponent MenuStrip { get; set; }
        IBeepUIComponent SecondaryMenuStrip { get; set; }
        IDM_Addin CurrentDisplayedAddin { get; set; }
        IDM_Addin MainDisplay { get; set; }
        IDM_Addin SplashScreen { get; set; }
        IWaitForm WaitForm { get; set; }
        Type WaitFormType { get; set; }
        IDisplayContainer Container { get; set; }
        IRoutingManager RoutingManager { get; set; }
        IPopupDisplayContainer PopupDisplay { get; set; }
        bool IsDataModified { get; set; }
        bool IsShowingMainForm { get; set; }
        bool IsShowingWaitForm { get; set; }
        bool IsBeepDataOn { get; set; }
        bool IsAppOn { get; set; }
        bool IsDevModeOn { get; set; }
        bool IsinCaptureMenuMode { get; set; }
        int TreeIconSize { get; set; }
        bool TreeExpand { get; set; }

        int SecondaryTreeIconSize { get; set; }
        bool SecondaryTreeExpand { get; set; }
        string AppObjectsName { get; set; }
        string BeepObjectsName { get; set; }
        string LogoUrl { get; set; }
        string Title { get; set; }
        string IconUrl { get; set; }
        bool ShowLogWindow { get; set; }
        bool ShowTreeWindow { get; set; }
        bool ShowSideBarWindow { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        Task<IErrorsInfo> LoadGraphics(string[] namespacestoinclude);
        Task<IErrorsInfo> LoadAssemblies(string[] namespacestoinclude);
        List<IDM_Addin> Addins { get; set; }
     
        IErrorsInfo LoadSetting();
        IErrorsInfo SaveSetting();
        IErrorsInfo PrintGrid(IPassedArgs passedArgs);
        IDM_Addin ShowUserControlPopUp(string usercontrolname, IDMEEditor pDMEEditor, string[] args, IPassedArgs e);
        IErrorsInfo ShowPage(string pagename, PassedArgs Passedarguments, DisplayType displayType = DisplayType.InControl, bool Singleton = false);
        Task<IErrorsInfo> ShowPageAsync(string pagename, PassedArgs Passedarguments, DisplayType displayType = DisplayType.InControl, bool Singleton = false);
        IErrorsInfo ShowWaitForm(PassedArgs Passedarguments);
        IErrorsInfo PasstoWaitForm(PassedArgs Passedarguments);
        Task<IErrorsInfo> CloseWaitFormAsync();
        IErrorsInfo CloseWaitForm();
        Task<IErrorsInfo> ShowHomeAsync();
        Task<IErrorsInfo> ShowAdminAsync();
        Task<IErrorsInfo> ShowProfileAsync();
        Task<IErrorsInfo> ShowLoginAsync();
        IErrorsInfo ShowHome();
        IErrorsInfo ShowAdmin();
        IErrorsInfo ShowProfile();
        IErrorsInfo ShowLogin();

        event EventHandler<KeyCombination> KeyPressed;
        IErrorsInfo PressKey(KeyCombination keyCombination);
        string HomePageTitle { get; set; }
        string HomePageName { get; set; }
        string HomePageDescription { get; set; }
        IErrorsInfo NavigateBack();
        IErrorsInfo NavigateForward();
        IErrorsInfo NavigateTo(string routeName, Dictionary<string, object> parameters = null);

        Task<IErrorsInfo> NavigateBackAsync();
        Task<IErrorsInfo> NavigateForwardAsync();
        Task<IErrorsInfo> NavigateToAsync(string routeName, Dictionary<string, object> parameters = null);
        string BreadCrumb { get; }
        IProfile DefaultProfile { get; set; }
        List<IBeepPrivilege> Privileges { get; set; }
        List<IBeepUser> Users { get; set; }
        IBeepUser User { get; set; }

        event EventHandler<IPassedArgs> PreLogin;
        event EventHandler<IPassedArgs> PostLogin;

        event EventHandler<IPassedArgs> PreClose;
        event EventHandler<IPassedArgs> PreCallModule;

        event EventHandler<IPassedArgs> PreShowItem;
        event EventHandler<IPassedArgs> PostShowItem;
        event EventHandler<IPassedArgs> PostCallModule;
        void PrintData(object data);
        void Notify(object data);
        void Email(object data);
        void Ticket(object data);


    }
}

using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IVisManager: IDisposable
    {
        EnumBeepThemes Theme { get; set; }
        bool IsLogOn { get; set; }
        IDMEEditor DMEEditor { get; set; }
        ErrorsInfo ErrorsandMesseges { get; set; }
        IControlManager Controlmanager { get; set; }
        IDM_Addin ToolStrip { get; set; }
        IDM_Addin SecondaryToolStrip { get; set; }
        IDM_Addin Tree { get; set; }
        IDM_Addin SecondaryTree { get; set; }
        IDM_Addin MenuStrip { get; set; }
        IDM_Addin SecondaryMenuStrip { get; set; }
        IDM_Addin CurrentDisplayedAddin { get; set; }
        IDM_Addin MainDisplay { get; set; }
        bool IsDataModified { get; set; }
        bool IsShowingMainForm { get; set; }
        bool IsShowingWaitForm { get; set; }
        bool IsBeepDataOn { get; set; }
        bool IsAppOn { get; set; }
        bool IsDevModeOn { get; set; }
        bool IsinCaptureMenuMode { get; set; }
        int TreeIconSize { get; set; }
        bool TreeExpand { get; set; }
        IFunctionandExtensionsHelpers Helpers { get; set; }
        IDisplayContainer Container { get; set; }
        int SecondaryTreeIconSize { get; set; }
        bool SecondaryTreeExpand { get; set; }
        string AppObjectsName { get; set; }
        string BeepObjectsName { get; set; }
        string LogoUrl { get; set; }
        string Title { get; set; }
        string IconUrl { get;set; }
         bool ShowLogWindow { get ; set ; }
         bool ShowTreeWindow { get ; set; }
        int Width { get; set; }
        int Height { get; set; }
        List<IDM_Addin> Addins { get; set; }
        IWaitForm WaitForm { get; set; }
        IErrorsInfo LoadSetting();
        IErrorsInfo SaveSetting();
        IVisHelper visHelper { get; set; }
        IErrorsInfo ShowMainPage();
        IErrorsInfo CallAddinRun();
        IErrorsInfo CloseAddin();
        IErrorsInfo PrintGrid(IPassedArgs passedArgs);
        IDM_Addin ShowUserControlPopUp(string usercontrolname, IDMEEditor pDMEEditor, string[] args, IPassedArgs e);
        IErrorsInfo ShowPage(string pagename,  PassedArgs Passedarguments,  DisplayType displayType = DisplayType.InControl,bool Singleton=false);
        IErrorsInfo ShowWaitForm(PassedArgs Passedarguments);
        IErrorsInfo PasstoWaitForm(PassedArgs Passedarguments);
        IErrorsInfo CloseWaitForm();
        IErrorsInfo ShowHome();

        event EventHandler<KeyCombination> KeyPressed;
        IErrorsInfo PressKey(KeyCombination keyCombination);
        string HomePageTitle { get; set; }
        string HomePageName { get; set; }
        string HomePageDescription { get; set; }
        
        IProfile DefaultProfile { get; set; }
        List<IBeepPrivilege> Privileges { get; set; }
        List<IBeepUser>    Users { get; set; }
        IBeepUser User { get; set; }
     
        event EventHandler<IPassedArgs> PreLogin;
        event EventHandler<IPassedArgs> PostLogin;
       
        event EventHandler<IPassedArgs> PreClose;
        event EventHandler<IPassedArgs> PreCallModule;
       
        event EventHandler<IPassedArgs> PreShowItem;
        event EventHandler<IPassedArgs> PostShowItem;
        event EventHandler<IPassedArgs> PostCallModule;
        void PrintData( object data);
        void Notify(object data);
        void Email(object data);
        void Ticket(object data);

    }
}
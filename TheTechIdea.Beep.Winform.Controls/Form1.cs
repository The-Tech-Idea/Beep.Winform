using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Logic;
using TheTechIdea.Beep.Vis.Modules;
using static TheTechIdea.Beep.Winform.Controls.Native.WinApi;


namespace TheTechIdea.Beep.Winform.Controls
{
    [AddinAttribute(Caption = "Home", Name = "Form1", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.Popup, ObjectType = "Beep")]
    public partial class Form1 : BeepiForm,IDM_Addin
    {
        private readonly IBeepService? beepService;

        public IDMEEditor Editor { get; }
        public AddinDetails Details { get; set; } = new AddinDetails();
        public Dependencies Dependencies { get; set; } = new Dependencies();
        string IDM_Addin.GuidID { get  ; set  ; }

        public Form1(IServiceProvider serviceProvider) : base()
        {
            InitializeComponent();
            beepService = serviceProvider.GetService<IBeepService>();
            Dependencies.DMEEditor= beepService.DMEEditor;
            MethodHandler.DMEEditor = beepService.DMEEditor;
            beepTreeControl1.init(beepService);
            beepButton1.Click += BeepButton1_Click;
            beepButton2.PopupMode = true;
        }
      

        private void BeepButton1_Click(object? sender, EventArgs e)
        {

            beepTreeControl1.CreateRootTree();
            beepButton2.ListItems = beepTreeControl1.Nodes;
            //BeepPopupListForm beepFileDialog = new BeepPopupListForm(beepTreeControl1.Nodes.ToList());
            //// Get the screen position of the control's top-left corner
            ////Point screenPoint = beepButton1.PointToScreen(Point.Empty);
            ////Point point = new Point(screenPoint.X, screenPoint.Y + beepButton1.Height);
            //SimpleItem x=beepFileDialog.ShowPopup("Tree",beepButton1, BeepPopupFormPosition.Top);
            beepTreeControl1.ShowCheckBox = true;
            BeepThemesManager.CurrentTheme = EnumBeepThemes.WinterTheme;
            //    beepListBox1.ListItems = beepTreeControl1.Nodes;
            //    beepListBox1.InitializeMenu();
            //beepTreeControl1.AllowMultiSelect = false;
        }
      
        void IDM_Addin.Initialize()
        {
            try
            {

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                beepService.DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
        }

        void IDM_Addin.Dispose()
        {
            try
            {

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                beepService.DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
        }

        void IDM_Addin.Suspend()
        {
            try
            {

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                beepService.DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            
        }

        void IDM_Addin.Resume()
        {
            try
            {

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                beepService.DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            
        }

        string IDM_Addin.GetErrorDetails()
        {
            try
            {

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                beepService.DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
           return beepService.DMEEditor.ErrorObject.Message;
        }

        void IDM_Addin.Run(IPassedArgs pPassedarg)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                beepService.DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            
        }

        void IDM_Addin.Run(params object[] args)
        {
           
        }

        Task<IErrorsInfo> IDM_Addin.RunAsync(IPassedArgs pPassedarg)
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                beepService.DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            return  Task.FromResult<IErrorsInfo>(beepService.DMEEditor.ErrorObject);
        }

        Task<IErrorsInfo> IDM_Addin.RunAsync(params object[] args)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                beepService.DMEEditor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            return Task.FromResult<IErrorsInfo>(beepService.DMEEditor.ErrorObject);
        }

        void IDM_Addin.Configure(Dictionary<string, object> settings)
        {
            
        }

        void INavigable.OnNavigatedTo(Dictionary<string, object> parameters)
        {
            
        }

        void IErrorView.SetError(string message)
        {
            
        }

      

        event EventHandler IDM_Addin.OnStart
        {
            add
            {
               
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler IDM_Addin.OnStop
        {
            add
            {
                
            }

            remove
            {
                
            }
        }

        event EventHandler<ErrorEventArgs> IDM_Addin.OnError
        {
            add
            {
               
            }

            remove
            {
               
            }
        }
       
    }
}

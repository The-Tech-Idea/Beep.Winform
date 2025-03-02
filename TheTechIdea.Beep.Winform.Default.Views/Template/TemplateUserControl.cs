
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using System.Reflection;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Container.Services;

namespace TheTechIdea.Beep.Winform.Default.Views.Template
{
    public partial class TemplateUserControl: UserControl,IDM_Addin
    {
        private readonly IBeepService? beepService;

        private IDMEEditor Editor { get; }
        public TemplateUserControl()
        {
            InitializeComponent();
            Details = new AddinDetails();
            Dependencies = new Dependencies();
            Details.ObjectType = "UserControl";
        }
        public TemplateUserControl(IBeepService service) : base()
        {
            InitializeComponent();
            Details = new AddinDetails();
            Dependencies = new Dependencies();
            Details.ObjectType = "UserControl";
            Dependencies.DMEEditor = service.DMEEditor;
            Dependencies.Logger = service.lg;
           
            beepService = service;
            Dependencies.DMEEditor = beepService.DMEEditor;

        }

        public AddinDetails Details { get  ; set  ; }
        public Dependencies Dependencies { get  ; set  ; }
        public string GuidID { get  ; set  ; }

        public event EventHandler OnStart;
        public event EventHandler OnStop;
        public event EventHandler<ErrorEventArgs> OnError;

        public virtual void ApplyTheme()
        {
           
        }

        public virtual void Configure(Dictionary<string, object> settings)
        {
            
        }

        public virtual void Dispose()
        {
           
        }

        public virtual string GetErrorDetails()
        {
            // if error occured return the error details
            // create error messege sring 
            string errormessage = "";
            if (Editor.ErrorObject != null)
            {
                if (Editor.ErrorObject.Errors.Count > 0)
                {
                    foreach (var item in Editor.ErrorObject.Errors)
                    {
                        errormessage += item.Message + "\n";
                    }
                }
            }
           
            return errormessage;
        }

        public virtual void Initialize()
        {
           
        }

        public virtual void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            
        }

        public virtual void Resume()
        {
            
        }

        public virtual void Run(IPassedArgs pPassedarg)
        {
           
        }

        public virtual void Run(params object[] args)
        {
           
        }

        public virtual Task<IErrorsInfo> RunAsync(IPassedArgs pPassedarg)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                Editor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            return Task.FromResult(Editor.ErrorObject);
        }

        public virtual Task<IErrorsInfo> RunAsync(params object[] args)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                Editor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            return Task.FromResult(Editor.ErrorObject);
        }

        public virtual void SetError(string message)
        {
            throw new NotImplementedException();
        }

        public virtual void Suspend()
        {
            throw new NotImplementedException();
        }
    }
}

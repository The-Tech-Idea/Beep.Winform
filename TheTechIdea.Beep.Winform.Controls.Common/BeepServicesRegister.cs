using TheTechIdea.Beep.Vis.Modules;
using Microsoft.Extensions.DependencyInjection;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Addin;
using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;
using System.Reflection;
using TheTechIdea.Beep.Desktop.Common.KeyManagement;
using TheTechIdea.Beep.ConfigUtil;



namespace TheTechIdea.Beep.Desktop.Common
{
    public static class BeepServicesRegister
    {
        private static IServiceCollection Services;
  
        public static IServiceCollection RegisterRouter(this IServiceCollection services)
        {
            Services = services;
            Services.AddSingleton<IRoutingManager, RoutingManager>();
            return Services;
        }
        public static IServiceCollection RegisterAppManager(this IServiceCollection services)
        {
            Services = services;
            Services.AddSingleton<IAppManager,AppManager>();
            return Services;
        }
        public static IServiceProvider ConfigureAppManager(this IServiceProvider serviceProvider, Action<AppManager> configure)
        {
            var appManager = serviceProvider.GetRequiredService<IAppManager>() as AppManager;
            if (appManager != null)
            {
                configure(appManager); // Configure the AppManager instance
            }

            return serviceProvider;
        }
        public static IServiceProvider ShowHome(this IServiceProvider serviceProvider)
        {
            var appManager = serviceProvider.GetRequiredService<IAppManager>() as AppManager;
           appManager.ShowHome();

            return serviceProvider;
        }
        public static IServiceCollection RegisterKeyHandler(this IServiceCollection services)
        {
            Services = services;
            Services.AddSingleton<IKeyHandlingManager, KeyHandlingManager>();
            return Services;
        }
        public static IAppManager SetBeepReference(this IAppManager ViewManager, IBeepService beepService)
        {
            beepService.vis = ViewManager;
            ViewManager.DMEEditor = beepService.DMEEditor;
            return ViewManager;
        }
        public static IAppManager SetMainDisplay(this IAppManager ViewManager, string mainform, string title, string iconname, string homePage = null, string homePageDescription = null, string logourl = null)
        {
            ViewManager.DMEEditor.ConfigEditor.Config.SystemEntryFormName = mainform;
            ViewManager.Title = title;
            ViewManager.IconUrl = iconname;
            ViewManager.LogoUrl = logourl;
            ViewManager.HomePageName = mainform;
            ViewManager.HomePageDescription = homePageDescription;
            return ViewManager;
        }
        public static IAppManager LoadAssemblies(this IAppManager ViewManager, IBeepService beepService, Progress<PassedArgs> progress)
        {
            // Create A parameter object for Wait Form

            // Load Assemblies
            beepService.LoadAssemblies(progress);
            beepService.Config_editor.LoadedAssemblies = beepService.LLoader.Assemblies.Select(c => c.DllLib).ToList();

            return ViewManager;
        }
        public static async Task<IErrorsInfo> LoadAssembliesAsync(this IAppManager viewManager, IBeepService beepService, IProgress<PassedArgs> progress)
        {
            var result = new ErrorsInfo();
            try
            {
                // Create a parameter object for the Wait Form
                PassedArgs args = new PassedArgs { Messege = "Loading assemblies..." };
                progress?.Report(args);

                // Asynchronous loading of assemblies
                await Task.Run(() =>
                {
                    beepService.LoadAssemblies((Progress<PassedArgs>)progress);
                    beepService.Config_editor.LoadedAssemblies = beepService.LLoader.Assemblies.Select(c => c.DllLib).ToList();
                }).ConfigureAwait(false); ;

                // Update progress
                args.Messege = "Assemblies loaded successfully.";
                progress?.Report(args);

                result.Flag = Errors.Ok;
                result.Message = "Assemblies loaded successfully.";
            }
            catch (Exception ex)
            {
                string methodName = nameof(LoadAssembliesAsync);
                viewManager.DMEEditor.AddLogMessage("Beep", $"in {methodName} Error: {ex.Message}", DateTime.Now, -1, null, Errors.Failed);

                result.Flag = Errors.Failed;
                result.Message = ex.Message;
                result.Ex = ex;

                // Update progress with error
                PassedArgs errorArgs = new PassedArgs { Messege = $"Error: {ex.Message}" };
                progress?.Report(errorArgs);
            }

            return result;
        }

        public static IServiceCollection RegisterViewModels(this IServiceCollection services)
        {
            Services = services;

            // Collect assemblies
            var assemblies = new List<Assembly>
    {
        Assembly.GetExecutingAssembly(),
        Assembly.GetCallingAssembly(),
        Assembly.GetEntryAssembly()!
    };

            var loadedFromContext = DependencyContext.Default.RuntimeLibraries
                .SelectMany(lib => lib.GetDefaultAssemblyNames(DependencyContext.Default))
                .Select(Assembly.Load)
                .ToList();
            assemblies.AddRange(loadedFromContext);

            assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.FullName.StartsWith("System", StringComparison.OrdinalIgnoreCase)
                         && !a.FullName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase)));

            // Register types implementing IBeepViewModels
            var viewModelTypes = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => typeof(IBeepViewModel).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var viewModelType in viewModelTypes)
            {
                Services.AddTransient(typeof(IBeepViewModel), viewModelType);
            }

            return Services;
        }

        public static IServiceCollection RegisterViews(this IServiceCollection services)
        {
            Services = services;

            // Collect assemblies (reuse same logic as above)
            var assemblies = new List<Assembly>
    {
        Assembly.GetExecutingAssembly(),
        Assembly.GetCallingAssembly(),
        Assembly.GetEntryAssembly()!
    };

            var loadedFromContext = DependencyContext.Default.RuntimeLibraries
                .SelectMany(lib => lib.GetDefaultAssemblyNames(DependencyContext.Default))
                .Select(Assembly.Load)
                .ToList();
            assemblies.AddRange(loadedFromContext);

            assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.FullName.StartsWith("System", StringComparison.OrdinalIgnoreCase)
                         && !a.FullName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase)));

            // Register types implementing IDM_Addin
            var viewTypes = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => typeof(IDM_Addin).IsAssignableFrom(t) && !t.IsInterface); //&& !t.IsInterface && !t.IsAbstract

            foreach (var viewType in viewTypes)
            {
                Services.AddTransient(typeof(IDM_Addin), viewType);
            }

            return Services;
        }

    }
}

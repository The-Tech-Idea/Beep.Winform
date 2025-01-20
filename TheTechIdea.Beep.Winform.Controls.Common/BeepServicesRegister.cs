using TheTechIdea.Beep.Vis.Modules;
using Microsoft.Extensions.DependencyInjection;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Addin;
using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;
using System.Reflection;



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
            ViewManager.HomePageName = homePage;
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
                .Where(t => typeof(IDM_Addin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var viewType in viewTypes)
            {
                Services.AddTransient(typeof(IDM_Addin), viewType);
            }

            return Services;
        }

    }
}

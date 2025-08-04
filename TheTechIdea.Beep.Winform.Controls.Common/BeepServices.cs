using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Desktop.Common.KeyManagement;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;



namespace TheTechIdea.Beep.Desktop.Common
{
    public static class BeepServices
    {
        public static IServiceProvider Provider;
        private static IServiceCollection Services;
        public static IContainer AutoFacContainer { get; private set; }
        private static ContainerBuilder Builder;
      
        public static IBeepService beepService { get; private set; }
        public static IAppManager AppManager { get; private set; }
        public static IKeyHandlingManager keyhandler { get; private set; }
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
            Provider= serviceProvider;
            return serviceProvider;
        }

        private static bool _mappingCreated;
        private static string _beepDataPath;



        #region "AutoFac"

        public static ContainerBuilder RegisterRouter(this ContainerBuilder builder)
        {
            Builder = builder;
            Builder.RegisterType<RoutingManager>().As<IRoutingManager>().SingleInstance();
            return Builder;
        }
        public static ContainerBuilder RegisterKeyHandler(this ContainerBuilder builder)
        {
            Builder = builder;
            Builder.RegisterType<KeyHandlingManager>().As<IKeyHandlingManager>().SingleInstance();

            return Builder;
        }
        public static ContainerBuilder RegisterAppManager(this ContainerBuilder builder)
        {
            Builder = builder;
            Builder.RegisterType<AppManager>().As<IAppManager>().SingleInstance();
            return Builder;
        }
        public static IContainer ConfigureAppManager(this IContainer container, Action<AppManager> configure)
        {

            var appManager = container.Resolve<IAppManager>() as AppManager;
            AppManager = appManager;
            if (AppManager != null)
            {
                configure(appManager); // Configure the AppManager instance
            }

            return container;
        }

        public static IContainer ShowHome(this IContainer container)
        {
            var appManager = container.Resolve<IAppManager>() as AppManager;
            appManager.ShowHome();
            return container;
        }
        public static ContainerBuilder RegisterViewModels(this ContainerBuilder builder)
        {
            Builder = builder;

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

            // Register types implementing IBeepViewModel
            var viewModelTypes = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => typeof(IBeepViewModel).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var viewModelType in viewModelTypes)
            {
                // Register with the type name as the key
                Builder.RegisterType(viewModelType)
                       .Keyed<IBeepViewModel>(viewModelType.Name) // Use the type name as the key
                       .InstancePerDependency();
            }

            return Builder;
        }

        public static ContainerBuilder RegisterViews(this ContainerBuilder builder)
        {
            Builder = builder;

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
                .Where(t => typeof(IDM_Addin).IsAssignableFrom(t) && !t.IsInterface);

            foreach (var viewType in viewTypes)
            {
                // Use the type name as the key
                var addinName = viewType.Name;

                // Register with the type name as the key
                Builder.RegisterType(viewType)
                       .Keyed<IDM_Addin>(addinName) // Use the type name as the key
                       .InstancePerDependency();
            }

            return Builder;
        }

        public static void RegisterServices(this ContainerBuilder builder)
        {
            // Register services

            builder.Register(AppContext.BaseDirectory, null, BeepConfigType.Application, true)
                   .RegisterRouter()
                   .RegisterAppManager()
                   .RegisterKeyHandler()
                   .RegisterViewModels() // Register view models with type names as keys
                   .RegisterViews();     // Register views with type names as keys


            // Add additional service registrations here
        }
        public static void LinkStaticServices()
        {
            DynamicFunctionCallingManager.DMEEditor = beepService.DMEEditor;
            DynamicFunctionCallingManager.Vismanager = beepService.vis;

        }
        public static void ConfigureServices(IContainer autofacContainer)
        {
            if (autofacContainer == null)
            {
                return;
            }
            AutoFacContainer = autofacContainer;
            // Extracted service retrieval and initial configuration into a separate method
            beepService = AutoFacContainer.Resolve<IBeepService>()!;
            AppManager = AutoFacContainer.Resolve<IAppManager>()!;
            beepService.vis = AppManager;
            keyhandler = AutoFacContainer.Resolve<IKeyHandlingManager>()!;
            DynamicFunctionCallingManager.DMEEditor = beepService.DMEEditor;
            DynamicFunctionCallingManager.Vismanager = beepService.vis;

            // Resolve a specific view model by key (type name)

            // Assuming these method calls setup and configure the services as necessary
            //Connect Winform Visula Manager to My Beep Service
            //if Web or other UI use the appropriate VisManager

            // SetupVisManager();
        }
        #endregion


        public static void ShowHome()
        {

            AppManager.ShowHome();

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
                // get Addinattribute ScopeCreateType to decide how to add the IDM_Addin 
                var addinAttribute = viewType.GetCustomAttribute<AddinAttribute>();
                if (addinAttribute != null)
                {
                    if (addinAttribute.ScopeCreateType == AddinScopeCreateType.Single)
                    {
                        Services.AddSingleton(typeof(IDM_Addin), viewType);
                    }
                    else
                    {
                        Services.AddTransient(typeof(IDM_Addin), viewType);
                    }
                }
              
            }

            return Services;
        }
        /// <summary>
        ///  Register Services
        /// </summary>
        /// <param name="builder"></param>
        public static void RegisterServices(HostApplicationBuilder builder)
        {
            // Register beep services
            builder.Services.Register(AppContext.BaseDirectory, null, BeepConfigType.Application, true);
            builder.Services.RegisterRouter();
            builder.Services.RegisterKeyHandler();
            builder.Services.RegisterViewModels();
            builder.Services.RegisterViews();
            builder.Services.RegisterAppManager();
            // Add additional service registrations here
        }
        /// <summary>
       
        /// <summary>
        /// Initialize and Configure Services
        /// </summary>
        /// <param name="host"></param>
        public static void ConfigureServices(IHost host)
        {
            Provider = host.Services;
           
            // Extracted service retrieval and initial configuration into a separate method
            beepService = host.Services.GetService<IBeepService>()!;
            AppManager = host.Services.GetService<IAppManager>()!;
            keyhandler = host.Services.GetService<IKeyHandlingManager>()!;
            // Assuming these method calls setup and configure the services as necessary
            //Connect Winform Visula Manager to My Beep Service
            //if Web or other UI use the appropriate VisManager

            // SetupVisManager();
        }
        /// <summary>
        /// Dispose Services
        /// </summary>
        /// <param name="services"></param>
        public static void DisposeServices()
        {
            keyhandler.UnregisterGlobalKeyHandler();
            AppManager.Dispose();
         //   beepService.DMEEditor.Dispose();
            beepService?.Dispose();
            // Add additional dispose logic as necessary
        }
    }
}

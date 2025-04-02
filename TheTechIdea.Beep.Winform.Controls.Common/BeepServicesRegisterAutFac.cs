using Autofac;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Addin;
using Microsoft.Extensions.DependencyModel;
using System.Reflection;
using TheTechIdea.Beep.Desktop.Common.KeyManagement;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Shared;
using TheTechIdea.Beep.Container;
using TheTechIdea.Beep.Winform.Controls.Helpers;




namespace TheTechIdea.Beep.Desktop.Common
{
    public  static partial class BeepServicesRegisterAutFac
    {
        private static ContainerBuilder Builder;
        private static bool _mappingCreated;
        private static string _beepDataPath;

        public static IContainer AutoFacContainer { get; private set; }
        public static IBeepService beepService { get; private set; }
        public static IAppManager AppManager { get; private set; }
        public static IKeyHandlingManager keyhandler { get; private set; }

      
        public static ContainerBuilder RegisterBeep(this ContainerBuilder builder, string directorypath, string containername, BeepConfigType configType, bool addAsSingleton = true)
        {
            Builder = builder;

            // Create and configure BeepService
            beepService = new BeepServiceAutoFac(builder);
            beepService.Configure(directorypath, containername, configType, addAsSingleton);

            // Register BeepService with Autofac
            if (addAsSingleton)
            {
                builder.RegisterInstance(beepService).As<IBeepService>().SingleInstance();
            }
            else
            {
                builder.RegisterInstance(beepService).As<IBeepService>().InstancePerLifetimeScope();
            }

            // Create the main folder and mappings
            _beepDataPath = ContainerMisc.CreateMainFolder();
     //       CreateBeepMapping(builder);

            return builder;
        }
        public static ContainerBuilder CreateBeepMapping(this ContainerBuilder builder)
        {
            if (beepService != null && !_mappingCreated)
            {
                _mappingCreated = true;
                ContainerMisc.AddAllConnectionConfigurations(beepService);
                ContainerMisc.AddAllDataSourceMappings(beepService);
                ContainerMisc.AddAllDataSourceQueryConfigurations(beepService);
            }

            return builder;
        }

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
       
        public static void ShowHome()
        {
            AppManager.CloseWaitForm();
            AppManager.ShowHome();

        }
        public static IContainer ConfigureAppManager(this IContainer container, Action<AppManager> configure)
        {
          
           var  appManager = container.Resolve<IAppManager>() as AppManager;
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
           
            builder.RegisterBeep(AppContext.BaseDirectory, null, BeepConfigType.Application, true)
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
            AssemblyClassDefinitionManager.Editor = beepService.DMEEditor;
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
            AssemblyClassDefinitionManager.Editor = beepService.DMEEditor;
            // Resolve a specific view model by key (type name)

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
            beepService.DMEEditor.Dispose();
            beepService?.Dispose();
            // Add additional dispose logic as necessary
        }

    }
}
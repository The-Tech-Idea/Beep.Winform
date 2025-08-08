using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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
    /// <summary>
    /// Modern desktop services registration and management for Beep framework.
    /// Provides comprehensive service registration, configuration, and lifecycle management
    /// for Windows Forms applications using .NET 8+ dependency injection patterns.
    /// </summary>
    public  static class BeepDesktopServices
    {
        #region Private Fields
        private static readonly object _lock = new object();
        private static volatile bool _isInitialized = false;
        private static IServiceProvider _provider;
        private static IServiceCollection _services;
        
        // Core service instances
        private static IBeepService _beepService;
        private static IAppManager _appManager;
        private static IKeyHandlingManager _keyHandler;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the configured service provider.
        /// </summary>
        public static IServiceProvider Provider 
        { 
            get => _provider;
            private set => _provider = value;
        }

        /// <summary>
        /// Gets the current Beep service instance.
        /// </summary>
        public static IBeepService BeepService 
        { 
            get => _beepService ?? throw new InvalidOperationException("BeepService not initialized. Call RegisterServices first.");
            private set => _beepService = value;
        }

        /// <summary>
        /// Gets the current application manager instance.
        /// </summary>
        public static IAppManager AppManager 
        { 
            get => _appManager ?? throw new InvalidOperationException("AppManager not initialized. Call ConfigureServices first.");
            private set => _appManager = value;
        }

        /// <summary>
        /// Gets the key handling manager instance.
        /// </summary>
        public static IKeyHandlingManager KeyHandler 
        { 
            get => _keyHandler ?? throw new InvalidOperationException("KeyHandler not initialized. Call ConfigureServices first.");
            private set => _keyHandler = value;
        }
        #endregion

        #region Events and Delegates
        /// <summary>
        /// Event handler for registering additional routes.
        /// </summary>
        /// <param name="routingManager">The routing manager to register routes with.</param>
        public delegate void RegisterAddinEventHandler(IServiceCollection services);

        /// <summary>
        /// Event handler for registering additional routes.
        /// </summary>
        /// <param name="routingManager">The routing manager to register routes with.</param>
        public delegate void RegisterBeepViewModelEventHandler(IServiceCollection services);


        /// <summary>
        /// Event handler for registering additional routes.
        /// </summary>
        /// <param name="routingManager">The routing manager to register routes with.</param>
        public delegate void RegisterRoutesEventHandler(IRoutingManager routingManager);

        /// <summary>
        /// Event handler for loading additional graphics locations.
        /// </summary>
        /// <param name="graphicsLocations">List of graphics file locations to add to.</param>
        public delegate void LoadGraphicsEventHandler(List<string> graphicsLocations);

        /// <summary>
        /// Event handler for loading additional font locations.
        /// </summary>
        /// <param name="fontLocations">List of font file locations to add to.</param>
        public delegate void LoadFontsEventHandler(List<string> fontLocations);

        /// <summary>
        /// Event fired when additional routes need to be registered.
        /// Subscribe to this event to register custom routes.
        /// </summary>
        public static event RegisterRoutesEventHandler OnRegisterRoutes;

         /// <summary>
        /// Event fired when additional routes need to be registered.
        /// Subscribe to this event to register custom routes.
        /// </summary>
        public static event RegisterAddinEventHandler OnRegisterAddins;
        /// <summary>
        /// Event fired when additional routes need to be registered.
        /// Subscribe to this event to register custom routes.
        /// </summary>
        public static event RegisterBeepViewModelEventHandler OnRegisterViewModels;
        /// <summary>
        /// Event fired when additional graphics locations need to be loaded.
        /// Subscribe to this event to add custom graphics paths.
        /// </summary>
        public static event LoadGraphicsEventHandler OnLoadGraphics;

        /// <summary>
        /// Event fired when additional font locations need to be loaded.
        /// Subscribe to this event to add custom font paths.
        /// </summary>
        public static event LoadFontsEventHandler OnLoadFonts;

        #endregion

        #region Service Registration Methods
        /// <summary>
        /// Registers all desktop services with modern configuration options.
        /// This is the primary registration method supporting fluent configuration.
        /// </summary>
        /// <param name="builder">The host application builder.</param>
        /// <param name="configure">Configuration action for desktop services.</param>
        public static void RegisterDesktopServices(this IServiceCollection services,
            Action<DesktopServiceOptions> configure = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var options = new DesktopServiceOptions();
            configure?.Invoke(options);
            options.Validate();

            RegisterServicesInternal(services, options);
        }
        /// <summary>
        /// Registers all desktop services with modern configuration options.
        /// This is the primary registration method supporting fluent configuration.
        /// </summary>
        /// <param name="builder">The host application builder.</param>
        /// <param name="configure">Configuration action for desktop services.</param>
        public static void RegisterDesktopServices(this HostApplicationBuilder builder, 
            Action<DesktopServiceOptions> configure = null)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var options = new DesktopServiceOptions();
            configure?.Invoke(options);
            options.Validate();

            RegisterServicesInternal(builder.Services, options);
        }

        /// <summary>
        /// Legacy registration method for backward compatibility.
        /// </summary>
        /// <param name="builder">The host application builder.</param>
        public static void RegisterServices(HostApplicationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            RegisterDesktopServices(builder, options =>
            {
                options.BeepDirectory = AppContext.BaseDirectory;
                options.ContainerName = "WinFormApp";
                options.ConfigType = BeepConfigType.Application;
                options.EnableAutoMapping = true;
                options.EnableAssemblyLoading = true;
            });
        }

        /// <summary>
        /// Registers control-specific services for Windows Forms integration.
        /// </summary>
        /// <param name="services">The service collection to extend.</param>
        /// <returns>The service collection for method chaining.</returns>
        public static IServiceCollection AddControlServices(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IUserControlService, UserControlService>();
            return services;
        }

        /// <summary>
        /// Registers routing services for navigation management.
        /// </summary>
        /// <param name="services">The service collection to extend.</param>
        /// <returns>The service collection for method chaining.</returns>
        public static IServiceCollection AddRoutingServices(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IRoutingManager, RoutingManager>();
            return services;
        }

        /// <summary>
        /// Registers application manager services.
        /// </summary>
        /// <param name="services">The service collection to extend.</param>
        /// <returns>The service collection for method chaining.</returns>
        public static IServiceCollection AddAppManager(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IAppManager, AppManager>();
            return services;
        }

        /// <summary>
        /// Registers key handling services for global keyboard shortcuts.
        /// </summary>
        /// <param name="services">The service collection to extend.</param>
        /// <returns>The service collection for method chaining.</returns>
        public static IServiceCollection AddKeyHandling(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IKeyHandlingManager, KeyHandlingManager>();
            return services;
        }

        /// <summary>
        /// Registers view models with automatic discovery and keyed services.
        /// </summary>
        /// <param name="services">The service collection to extend.</param>
        /// <param name="assemblies">Optional assemblies to scan. If null, scans all loaded assemblies.</param>
        /// <returns>The service collection for method chaining.</returns>
        public static IServiceCollection AddViewModels(this IServiceCollection services, 
            IEnumerable<Assembly> assemblies = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var assembliesToScan = assemblies?.ToList() ?? GetRelevantAssemblies();

            var viewModelTypes = assembliesToScan
                .SelectMany(a => GetTypesFromAssembly(a))
                .Where(t => typeof(IBeepViewModel).IsAssignableFrom(t) && 
                           !t.IsInterface && 
                           !t.IsAbstract)
                .ToList();

            foreach (var viewModelType in viewModelTypes)
            {
                services.AddKeyedTransient<IBeepViewModel>(viewModelType.Name, (serviceProvider, key) =>
                    (IBeepViewModel)ActivatorUtilities.CreateInstance(serviceProvider, viewModelType));
            }
           
            return services;
        }
        /// <summary>
        /// Registers additional Views by firing the OnRegisterView event.
        /// </summary>
        public static void AddView(this IServiceCollection services, IDM_Addin addin)
        {
            try
            {
                Type viewType= addin.GetType();
                var addinName = viewType.Name;
                var addinAttribute = viewType.GetCustomAttribute<AddinAttribute>();
                if (addinAttribute == null) return;
                if (addinAttribute.ScopeCreateType == AddinScopeCreateType.Single)
                {
                    services.AddSingleton(viewType);
                    services.AddKeyedSingleton<IDM_Addin>(addinName, (serviceProvider, key) =>
                        (IDM_Addin)serviceProvider.GetRequiredService(viewType));
                }
                else
                {
                    services.AddTransient(viewType);
                    services.AddKeyedTransient<IDM_Addin>(addinName, (serviceProvider, key) =>
                        (IDM_Addin)serviceProvider.GetRequiredService(viewType));
                }
            }
            catch (Exception ex)
            {
                BeepService?.DMEEditor?.AddLogMessage("BeepDesktopServices",
                    $"Error registering additional views: {ex.Message}",
                    DateTime.Now, -1, null, Errors.Failed);
            }
        }
      
        public static void AddViewModel(this IServiceCollection services,IBeepViewModel viewModel)
        {
            if (viewModel == null) return;
            try
            {
                    Type viewModelType = viewModel.GetType();
                    services.AddKeyedTransient<IBeepViewModel>(viewModelType.Name, (serviceProvider, key) =>
                   (IBeepViewModel)ActivatorUtilities.CreateInstance(serviceProvider, viewModelType));

            }
            catch (Exception ex)
            {
                BeepService?.DMEEditor?.AddLogMessage("BeepDesktopServices",
                    $"Error adding view model: {ex.Message}",
                    DateTime.Now, -1, null, Errors.Failed);
            }
        }
        /// <summary>
        /// Registers views (add-ins) with automatic discovery and proper lifecycle management.
        /// </summary>
        /// <param name="services">The service collection to extend.</param>
        /// <param name="assemblies">Optional assemblies to scan. If null, scans all loaded assemblies.</param>
        /// <returns>The service collection for method chaining.</returns>
        public static IServiceCollection AddViews(this IServiceCollection services, 
            IEnumerable<Assembly> assemblies = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var assembliesToScan = assemblies?.ToList() ?? GetRelevantAssemblies();

            var viewTypes = assembliesToScan
                .SelectMany(a => GetTypesFromAssembly(a))
                .Where(t => typeof(IDM_Addin).IsAssignableFrom(t) && !t.IsInterface)
                .ToList();

            foreach (var viewType in viewTypes)
            {
                var addinAttribute = viewType.GetCustomAttribute<AddinAttribute>();
                if (addinAttribute == null) continue;

                var addinName = viewType.Name;

                if (addinAttribute.ScopeCreateType == AddinScopeCreateType.Single)
                {
                    services.AddSingleton(viewType);
                    services.AddKeyedSingleton<IDM_Addin>(addinName, (serviceProvider, key) =>
                        (IDM_Addin)serviceProvider.GetRequiredService(viewType));
                }
                else
                {
                    services.AddTransient(viewType);
                    services.AddKeyedTransient<IDM_Addin>(addinName, (serviceProvider, key) =>
                        (IDM_Addin)serviceProvider.GetRequiredService(viewType));
                }
            }
         
            return services;
        }

        #endregion

        #region Configuration and Initialization Methods

        /// <summary>
        /// Configures and initializes all desktop services after registration.
        /// </summary>
        /// <param name="host">The configured host instance.</param>
        /// <returns>Task representing the async initialization.</returns>
        public static async Task ConfigureServicesAsync(IHost host)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            Provider = host.Services;

            try
            {
                // Retrieve core services
                BeepService = host.Services.GetRequiredService<IBeepService>();
                AppManager = host.Services.GetRequiredService<IAppManager>();
                KeyHandler = host.Services.GetRequiredService<IKeyHandlingManager>();

                // Cross-wire services
                BeepService.vis = AppManager;
                
                // Use our extension method instead of conflicting ones
                BeepDesktopServices.SetBeepReference(AppManager, BeepService);

                // Configure dynamic function calling
                ConfigureDynamicFunctionCalling();
                BeepThemesManager.InitializeThemes();
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to configure desktop services.", ex);
            }
        }

        /// <summary>
        /// Legacy synchronous configuration method for backward compatibility.
        /// </summary>
        /// <param name="host">The configured host instance.</param>
        public static void ConfigureServices(IHost host)
        {
            ConfigureServicesAsync(host).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Configures the application manager with fluent API.
        /// </summary>
        /// <param name="serviceProvider">Service provider containing registered services.</param>
        /// <param name="configure">Configuration action for the app manager.</param>
        /// <returns>The configured service provider for method chaining.</returns>
        public static IServiceProvider ConfigureAppManager(this IServiceProvider serviceProvider, 
            Action<AppManager> configure)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var appManager = serviceProvider.GetRequiredService<IAppManager>() as AppManager;
            if (appManager != null)
            {
                configure(appManager);
            }

            Provider = serviceProvider;
            return serviceProvider;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Shows the application home page.
        /// </summary>
        public static void ShowHome()
        {
            AppManager?.ShowHome();
        }

        /// <summary>
        /// Safely disposes all desktop services and resources.
        /// </summary>
        public static void DisposeServices()
        {
            try
            {
                _keyHandler?.UnregisterGlobalKeyHandler();
                _appManager?.Dispose();
                _beepService?.Dispose();

                // Reset static references
                _keyHandler = null;
                _appManager = null;
                _beepService = null;
                _provider = null;
                _isInitialized = false;
            }
            catch (Exception ex)
            {
                // Log disposal errors but don't throw
                System.Diagnostics.Debug.WriteLine($"Error during service disposal: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the configuration summary for all registered services.
        /// </summary>
        /// <returns>Configuration summary string.</returns>
        public static string GetConfigurationSummary()
        {
            if (!_isInitialized)
                return "Desktop services not initialized.";

            return $"Initialized: {_isInitialized}, " +
                   $"BeepService: {(_beepService != null ? "Ready" : "Not Ready")}, " +
                   $"AppManager: {(_appManager != null ? "Ready" : "Not Ready")}, " +
                   $"KeyHandler: {(_keyHandler != null ? "Ready" : "Not Ready")}";
        }

        #endregion

        #region AppManager Extension Methods

        /// <summary>
        /// Sets the Beep service reference for the AppManager.
        /// </summary>
        /// <param name="appManager">The app manager instance.</param>
        /// <param name="beepService">The Beep service instance.</param>
        /// <returns>The app manager for method chaining.</returns>
        public static IAppManager SetBeepReference(this IAppManager appManager, IBeepService beepService)
        {
            if (appManager == null)
                throw new ArgumentNullException(nameof(appManager));
            if (beepService == null)
                throw new ArgumentNullException(nameof(beepService));

            beepService.vis = appManager;
            appManager.DMEEditor = beepService.DMEEditor;
            return appManager;
        }

        /// <summary>
        /// Configures the main display settings for the application.
        /// </summary>
        /// <param name="appManager">The app manager instance.</param>
        /// <param name="mainForm">Main form name.</param>
        /// <param name="title">Application title.</param>
        /// <param name="iconName">Icon name.</param>
        /// <param name="homePage">Home page name.</param>
        /// <param name="homePageDescription">Home page description.</param>
        /// <param name="logoUrl">Logo URL.</param>
        /// <returns>The app manager for method chaining.</returns>
        public static IAppManager SetMainDisplay(this IAppManager appManager, 
            string mainForm, 
            string title, 
            string iconName, 
            string homePage = null, 
            string homePageDescription = null, 
            string logoUrl = null)
        {
            if (appManager == null)
                throw new ArgumentNullException(nameof(appManager));

            if (appManager.DMEEditor?.ConfigEditor?.Config != null)
            {
                appManager.DMEEditor.ConfigEditor.Config.SystemEntryFormName = mainForm;
            }

            appManager.Title = title;
            appManager.IconUrl = iconName;
            appManager.LogoUrl = logoUrl;
            appManager.HomePageName = homePage ?? mainForm;
            appManager.HomePageDescription = homePageDescription;

            return appManager;
        }

        /// <summary>
        /// Loads assemblies with progress reporting (legacy synchronous version).
        /// </summary>
        /// <param name="appManager">The app manager instance.</param>
        /// <param name="beepService">The Beep service instance.</param>
        /// <param name="progress">Progress reporter.</param>
        /// <returns>The app manager for method chaining.</returns>
        public static IAppManager LoadAssemblies(this IAppManager appManager, 
            IBeepService beepService, 
            Progress<PassedArgs> progress)
        {
            if (appManager == null)
                throw new ArgumentNullException(nameof(appManager));
            if (beepService == null)
                throw new ArgumentNullException(nameof(beepService));

            beepService.LoadAssemblies(progress);
            
            if (beepService.Config_editor != null && beepService.LLoader?.Assemblies != null)
            {
                beepService.Config_editor.LoadedAssemblies = beepService.LLoader.Assemblies
                    .Select(c => c.DllLib)
                    .ToList();
            }

            return appManager;
        }

        /// <summary>
        /// Loads assemblies asynchronously with comprehensive error handling and progress reporting.
        /// </summary>
        /// <param name="appManager">The app manager instance.</param>
        /// <param name="beepService">The Beep service instance.</param>
        /// <param name="progress">Progress reporter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task containing the operation result.</returns>
        public static async Task<IErrorsInfo> LoadAssembliesAsync(this IAppManager appManager, 
            IBeepService beepService, 
            IProgress<PassedArgs> progress = null,
            CancellationToken cancellationToken = default)
        {
            if (appManager == null)
                throw new ArgumentNullException(nameof(appManager));
            if (beepService == null)
                throw new ArgumentNullException(nameof(beepService));

            var result = new ErrorsInfo();

            try
            {
                progress?.Report(new PassedArgs { Messege = "Starting assembly loading..." });

                await Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    if (beepService.LoadAssembliesAsync != null)
                    {
                        await beepService.LoadAssembliesAsync(progress as Progress<PassedArgs>);
                    }
                    else
                    {
                        // Fallback to synchronous loading
                        beepService.LoadAssemblies(progress as Progress<PassedArgs>);
                    }

                    if (beepService.Config_editor != null && beepService.LLoader?.Assemblies != null)
                    {
                        beepService.Config_editor.LoadedAssemblies = beepService.LLoader.Assemblies
                            .Select(c => c.DllLib)
                            .ToList();
                    }
                }, cancellationToken);

                progress?.Report(new PassedArgs { Messege = "Assemblies loaded successfully." });
                result.Flag = Errors.Ok;
                result.Message = "Assemblies loaded successfully.";
            }
            catch (OperationCanceledException)
            {
                result.Flag = Errors.Failed;
                result.Message = "Assembly loading was cancelled.";
                progress?.Report(new PassedArgs { Messege = "Assembly loading cancelled." });
            }
            catch (Exception ex)
            {
                var methodName = nameof(LoadAssembliesAsync);
                appManager.DMEEditor?.AddLogMessage("Beep", 
                    $"in {methodName} Error: {ex.Message}", 
                    DateTime.Now, -1, null, Errors.Failed);

                result.Flag = Errors.Failed;
                result.Message = ex.Message;
                result.Ex = ex;

                progress?.Report(new PassedArgs { Messege = $"Error: {ex.Message}" });
            }

            return result;
        }

        #endregion

        #region BeepAppServices Integration

        /// <summary>
        /// Integrates BeepAppServices functionality into BeepDesktopServices.
        /// Loads graphics, fonts, and registers routes with optional wait form progress.
        /// </summary>
        /// <param name="namespacesToInclude">Namespaces to include for embedded resources.</param>
        /// <param name="showWaitForm">Whether to show wait form during loading.</param>
        /// <returns>Error information about the operation.</returns>
        public static IErrorsInfo StartLoading(string[] namespacesToInclude = null, bool showWaitForm = true)
        {
            var result = new ErrorsInfo();
            
            try
            {
                if (namespacesToInclude == null)
                {
                    namespacesToInclude = new string[] { "BeepEnterprize", "TheTechIdea", "Beep" };
                }

                PassedArgs waitArgs = null;
                Debug.WriteLine("Starting application initialization...");
                if (showWaitForm)
                {
                    Debug.WriteLine("Showing wait form...");
                    waitArgs = new PassedArgs { Messege = "Starting application initialization..." };
                    AppManager.ShowWaitForm(waitArgs);
                }
                Debug.WriteLine("Loading resources...");
                // Load graphics and fonts
                LoadResources(namespacesToInclude, waitArgs);
                
                // Load assemblies with progress
                LoadAssembliesWithProgress(waitArgs);
                
                // Register routes
                //RegisterDefaultRoutes();
                RegisterAdditionalRoutes();

                if (showWaitForm)
                {
                    waitArgs.Messege = "Initialization completed successfully";
                    AppManager.PasstoWaitForm(waitArgs);
                    Thread.Sleep(1000); // Replace Task.Delay with Thread.Sleep
                    AppManager.CloseWaitForm();
                }

                result.Flag = Errors.Ok;
                result.Message = "Loading completed successfully";
            }
            catch (Exception ex)
            {
                if (showWaitForm)
                {
                    AppManager.CloseWaitForm();
                }
                
                result.Flag = Errors.Failed;
                result.Message = ex.Message;
                result.Ex = ex;
            }

            return result;
        }

        /// <summary>
        /// Loads graphics and font resources with progress reporting.
        /// </summary>
        /// <param name="namespacesToInclude">Namespaces to include for embedded resources.</param>
        /// <param name="waitArgs">Wait form arguments for progress reporting.</param>
        private static void LoadResources(string[] namespacesToInclude, PassedArgs waitArgs)
        {
            // Update progress
            if (waitArgs != null)
            {
                waitArgs.Messege = "Loading graphics from embedded resources...";
                AppManager.PasstoWaitForm(waitArgs);
            }

            // Load graphics from embedded resources
            LoadGraphicsFromEmbedded(namespacesToInclude);

            // Update progress
            if (waitArgs != null)
            {
                waitArgs.Messege = "Loading graphics from folders...";
                AppManager.PasstoWaitForm(waitArgs);
            }

            // Load graphics from folders
            LoadGraphicsFromFolders();

            // Update progress
            if (waitArgs != null)
            {
                waitArgs.Messege = "Loading fonts...";
                AppManager.PasstoWaitForm(waitArgs);
            }

            // Load fonts
            LoadFontsFromResources(namespacesToInclude);
        }

        /// <summary>
        /// Loads graphics from embedded resources and fires event for additional locations.
        /// </summary>
        /// <param name="namespacesToInclude">Namespaces to scan for embedded graphics.</param>
        public static void LoadGraphicsFromEmbedded(string[] namespacesToInclude)
        {
            try
            {
                // Load from embedded resources
                ImageListHelper.GetGraphicFilesLocationsFromEmbedded(namespacesToInclude);

                // Fire event for additional graphics locations
                var additionalLocations = new List<string>();
                OnLoadGraphics?.Invoke(additionalLocations);

                // Load additional graphics if any were provided
                foreach (var location in additionalLocations)
                {
                    if (Directory.Exists(location))
                    {
                        ImageListHelper.GetGraphicFilesLocations(location);
                    }
                }
            }
            catch (Exception ex)
            {
                BeepService?.DMEEditor?.AddLogMessage("BeepDesktopServices", 
                    $"Error loading graphics from embedded resources: {ex.Message}", 
                    DateTime.Now, -1, null, Errors.Failed);
            }
        }

        /// <summary>
        /// Loads graphics from configured folders.
        /// </summary>
        public static void LoadGraphicsFromFolders()
        {
            try
            {
                var gfxFolder = BeepService?.DMEEditor?.ConfigEditor?.Config?.Folders?
                    .FirstOrDefault(x => x.FolderFilesType == FolderFileTypes.GFX);

                if (gfxFolder != null && Directory.Exists(gfxFolder.FolderPath))
                {
                    ImageListHelper.GetGraphicFilesLocations(gfxFolder.FolderPath);
                }
            }
            catch (Exception ex)
            {
                BeepService?.DMEEditor?.AddLogMessage("BeepDesktopServices", 
                    $"Error loading graphics from folders: {ex.Message}", 
                    DateTime.Now, -1, null, Errors.Failed);
            }
        }

        /// <summary>
        /// Loads fonts from resources and fires event for additional locations.
        /// </summary>
        /// <param name="namespacesToInclude">Namespaces to scan for embedded fonts.</param>
        public static void LoadFontsFromResources(string[] namespacesToInclude)
        {
            try
            {
                var gfxFolder = BeepService?.DMEEditor?.ConfigEditor?.Config?.Folders?
                    .FirstOrDefault(x => x.FolderFilesType == FolderFileTypes.GFX);

                if (gfxFolder != null && Directory.Exists(gfxFolder.FolderPath))
                {
                    // Load fonts from folders
                    FontListHelper.GetFontFilesLocations(gfxFolder.FolderPath);

                    // Load specific font types
                    var fontExtensions = new[] { "*.ttf", "*.otf", "*.woff", "*.woff2" };
                    var fontFiles = fontExtensions
                        .SelectMany(ext => Directory.GetFiles(gfxFolder.FolderPath, ext))
                        .ToArray();
                }

                // Load fonts from embedded resources
                FontListHelper.GetFontResourcesFromEmbedded(namespacesToInclude);

                // Fire event for additional font locations
                var additionalFontLocations = new List<string>();
                OnLoadFonts?.Invoke(additionalFontLocations);

                // Load additional fonts if any were provided
                foreach (var location in additionalFontLocations)
                {
                    if (Directory.Exists(location))
                    {
                        FontListHelper.GetFontFilesLocations(location);
                    }
                }
            }
            catch (Exception ex)
            {
                BeepService?.DMEEditor?.AddLogMessage("BeepDesktopServices", 
                    $"Error loading fonts: {ex.Message}", 
                    DateTime.Now, -1, null, Errors.Failed);
            }
        }

        /// <summary>
        /// Loads assemblies with progress reporting.
        /// </summary>
        /// <param name="waitArgs">Wait form arguments for progress reporting.</param>
        private static void LoadAssembliesWithProgress(PassedArgs waitArgs)
        {
            if (waitArgs != null)
            {
                waitArgs.Messege = "Loading assemblies...";
                AppManager.PasstoWaitForm(waitArgs);
            }

            var progress = new Progress<PassedArgs>(args =>
            {
                if (waitArgs != null)
                {
                    waitArgs.Messege = args.Messege ?? "Loading assemblies...";
                    AppManager.PasstoWaitForm(waitArgs);
                }
            });

            BeepService.LoadAssemblies(progress);
            BeepService.Config_editor.LoadedAssemblies = BeepService.LLoader.Assemblies
                .Select(c => c.DllLib).ToList();
        }
        
        /// <summary>
        /// Registers additional routes by firing the OnRegisterRoutes event.
        /// </summary>
        public static void RegisterAdditionalRoutes()
        {
            try
            {
                var routingManager = AppManager.RoutingManager;
                if (routingManager != null)
                {
                    OnRegisterRoutes?.Invoke(routingManager);
                }
            }
            catch (Exception ex)
            {
                BeepService?.DMEEditor?.AddLogMessage("BeepDesktopServices", 
                    $"Error registering additional routes: {ex.Message}", 
                    DateTime.Now, -1, null, Errors.Failed);
            }
        }

        /// <summary>
        /// Adds a graphics location to be loaded.
        /// </summary>
        /// <param name="path">Path to graphics folder.</param>
        public static void AddGraphicsLocation(string path)
        {
            if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
            {
                ImageListHelper.GetGraphicFilesLocations(path);
            }
        }

        /// <summary>
        /// Adds a font location to be loaded.
        /// </summary>
        /// <param name="path">Path to fonts folder.</param>
        public static void AddFontLocation(string path)
        {
            if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
            {
                FontListHelper.GetFontFilesLocations(path);
            }
        }

        /// <summary>
        /// Registers a single route.
        /// </summary>
        /// <param name="routeName">Name of the route.</param>
        /// <param name="addinName">Name of the addin/module.</param>
        public static void RegisterRoute(string routeName, string addinName)
        {
            try
            {
                AppManager?.RoutingManager?.RegisterRouteByName(routeName, addinName);
            }
            catch (Exception ex)
            {
                BeepService?.DMEEditor?.AddLogMessage("BeepDesktopServices", 
                    $"Error registering route '{routeName}': {ex.Message}", 
                    DateTime.Now, -1, null, Errors.Failed);
            }
        }

        /// <summary>
        /// Registers multiple routes from a dictionary.
        /// </summary>
        /// <param name="routes">Dictionary of route name to addin name mappings.</param>
        public static void RegisterRoutes(Dictionary<string, string> routes)
        {
            if (routes == null) return;

            foreach (var route in routes)
            {
                RegisterRoute(route.Key, route.Value);
            }
        }

        #endregion

        #region Private Implementation Methods

        /// <summary>
        /// Internal service registration logic.
        /// </summary>
        private static void RegisterServicesInternal(IServiceCollection services, DesktopServiceOptions options)
        {
            lock (_lock)
            {
                if (_isInitialized)
                {
                    throw new InvalidOperationException("Desktop services have already been registered.");
                }

                _services = services;

                // Register Beep services using the modernized registration
                services.AddBeepServices(beepOptions =>
                {
                    beepOptions.DirectoryPath = options.BeepDirectory;
                    beepOptions.ContainerName = options.ContainerName;
                    beepOptions.ConfigType = options.ConfigType;
                    beepOptions.ServiceLifetime = ServiceLifetime.Singleton;
                    beepOptions.EnableAutoMapping = options.EnableAutoMapping;
                    beepOptions.EnableAssemblyLoading = options.EnableAssemblyLoading;
                    beepOptions.InitializationTimeout = options.InitializationTimeout;
                });

                // Register desktop-specific services
                services.AddRoutingServices()
                       .AddKeyHandling()
                       .AddAppManager()
                       .AddControlServices();

                // Register views and view models if enabled
                if (options.EnableViewDiscovery)
                {
                    services.AddViewModels(options.AssembliesToScan)
                           .AddViews(options.AssembliesToScan);
                }
            }
        }

        /// <summary>
        /// Configures dynamic function calling manager.
        /// </summary>
        private static void ConfigureDynamicFunctionCalling()
        {
            try
            {
                if (BeepService?.DMEEditor != null)
                {
                    DynamicFunctionCallingManager.DMEEditor = BeepService.DMEEditor;
                }

                if (BeepService?.vis != null)
                {
                    DynamicFunctionCallingManager.Vismanager = BeepService.vis;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error configuring dynamic function calling: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets relevant assemblies for scanning, excluding system assemblies.
        /// </summary>
        private static List<Assembly> GetRelevantAssemblies()
        {
            var assemblies = new List<Assembly>();

            // Add core assemblies
            var coreAssemblies = new[]
            {
                Assembly.GetExecutingAssembly(),
                Assembly.GetCallingAssembly(),
                Assembly.GetEntryAssembly()
            }.Where(a => a != null);

            assemblies.AddRange(coreAssemblies);

            try
            {
                // Add assemblies from dependency context
                var dependencyAssemblies = DependencyContext.Default?.RuntimeLibraries
                    .SelectMany(lib => lib.GetDefaultAssemblyNames(DependencyContext.Default))
                    .Where(name => !IsSystemAssembly(name.Name))
                    .Select(name => 
                    {
                        try { return Assembly.Load(name); }
                        catch { return null; }
                    })
                    .Where(a => a != null);

                if (dependencyAssemblies != null)
                {
                    assemblies.AddRange(dependencyAssemblies);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dependency assemblies: {ex.Message}");
            }

            // Add currently loaded assemblies
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !IsSystemAssembly(a.FullName));

            assemblies.AddRange(loadedAssemblies);

            return assemblies.Distinct().ToList();
        }

        /// <summary>
        /// Safely gets types from an assembly.
        /// </summary>
        private static IEnumerable<Type> GetTypesFromAssembly(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t != null);
            }
            catch (Exception)
            {
                return Enumerable.Empty<Type>();
            }
        }

        /// <summary>
        /// Determines if an assembly name represents a system assembly.
        /// </summary>
        private static bool IsSystemAssembly(string assemblyName)
        {
            if (string.IsNullOrEmpty(assemblyName))
                return true;

            var systemPrefixes = new[]
            {
                "System",
                "Microsoft",
                "mscorlib",
                "netstandard",
                "WindowsBase",
                "PresentationCore",
                "PresentationFramework"
            };

            return systemPrefixes.Any(prefix => 
                assemblyName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }

        #endregion
        public static void ConfigureControlsandMenus()
        {
            // For example:
            AssemblyClassDefinitionManager.TreeStructures = BeepService.Config_editor.AddinTreeStructure;
            AssemblyClassDefinitionManager.BranchesClasses = BeepService.Config_editor.BranchesClasses;
            AssemblyClassDefinitionManager.GlobalFunctions = BeepService.Config_editor.GlobalFunctions;

            // Inject shared references
            DynamicFunctionCallingManager.DMEEditor = BeepService.DMEEditor;
            DynamicFunctionCallingManager.Vismanager = BeepService.vis;
            DynamicFunctionCallingManager.TreeEditor = (Vis.Modules.ITree)BeepService.vis.Tree;


            // Assign delegates
            HandlersFactory.GlobalMenuItemsProvider = DynamicMenuManager.GetMenuItemsList; // Set this in the main form if needed

            HandlersFactory.RunFunctionHandler = DynamicFunctionCallingManager.RunFunctionFromExtensions;

            HandlersFactory.RunFunctionWithTreeHandler = (item, method) =>
                DynamicFunctionCallingManager.RunFunctionFromExtensions(item, method);

            HandlersFactory.RunMethodFromObjectHandler = (branch, method) =>
                DynamicFunctionCallingManager.RunMethodFromObject(branch, method);

            HandlersFactory.RunMethodFromExtensionHandler = (branch, def, method) =>
                DynamicFunctionCallingManager.RunMethodFromExtension(branch, def, method);

            HandlersFactory.RunMethodFromExtensionWithTreeHandler = (branch, method) =>
                DynamicFunctionCallingManager.RunMethodFromExtension(branch, method);

        }
    }
    

    /// <summary>
    /// Configuration options for desktop services.
    /// </summary>
    public class DesktopServiceOptions
    {
        /// <summary>
        /// Gets or sets the Beep data directory path.
        /// </summary>
        public string BeepDirectory { get; set; } = AppContext.BaseDirectory;

        /// <summary>
        /// Gets or sets the container name.
        /// </summary>
        public string ContainerName { get; set; } = "DesktopApp";

        /// <summary>
        /// Gets or sets the configuration type.
        /// </summary>
        public BeepConfigType ConfigType { get; set; } = BeepConfigType.Application;

        /// <summary>
        /// Gets or sets whether to enable automatic mapping creation.
        /// </summary>
        public bool EnableAutoMapping { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to enable automatic assembly loading.
        /// </summary>
        public bool EnableAssemblyLoading { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to enable automatic view and view model discovery.
        /// </summary>
        public bool EnableViewDiscovery { get; set; } = true;

        /// <summary>
        /// Gets or sets the initialization timeout.
        /// </summary>
        public TimeSpan InitializationTimeout { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Gets or sets specific assemblies to scan for views and view models.
        /// If null, all relevant assemblies will be scanned.
        /// </summary>
        public IEnumerable<Assembly> AssembliesToScan { get; set; }

        /// <summary>
        /// Validates the configuration options.
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(BeepDirectory))
                throw new ArgumentException("BeepDirectory cannot be null or empty.", nameof(BeepDirectory));

            if (string.IsNullOrWhiteSpace(ContainerName))
                throw new ArgumentException("ContainerName cannot be null or empty.", nameof(ContainerName));

            if (InitializationTimeout <= TimeSpan.Zero)
                throw new ArgumentException("InitializationTimeout must be positive.", nameof(InitializationTimeout));
        }
    }
}

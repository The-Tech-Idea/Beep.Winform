using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System.Web;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Desktop.Common
{
    public partial class RoutingManager : IRoutingManager
    {
        private readonly IServiceProvider _serviceProvider; // Microsoft DI
        private readonly IComponentContext _autofacContext; // Autofac container
        private readonly SemaphoreSlim _navigationLock = new SemaphoreSlim(1, 1);

        public EnumBeepThemes Theme { get; set; }
        public IBeepService Beepservices { get; }
        public IDMEEditor DMEEditor { get; }

        private readonly IDisplayContainer _displayContainer;
        private readonly ContainerTypeEnum _containerType;

        private readonly Dictionary<string, Lazy<IDM_Addin>> _viewCache = new Dictionary<string, Lazy<IDM_Addin>>();
        private readonly Dictionary<string, Type> _routes = new Dictionary<string, Type>();
        private readonly Dictionary<string, RouteGuard> _routeGuards = new Dictionary<string, RouteGuard>();
        private readonly Dictionary<string, string> _aliases = new Dictionary<string, string>();
        private readonly Stack<string> _navigationHistory = new Stack<string>();
        private readonly Stack<string> _forwardHistory = new Stack<string>();
        private readonly object _historyLock = new object();

        private string _defaultRoute;
        private IDM_Addin _currentView;
        private Type _errorViewType;

        public event EventHandler<string> Navigating;
        public event EventHandler<string> Navigated;
        public event EventHandler<IRouteArgs> PreShowItem;
        public event EventHandler<IRouteArgs> PostShowItem;

        private bool _isCustomCreatorSet = false;
        private bool _useCustomCreator = false;

        public IDM_Addin CurrentControl => _currentView;

        public bool UseCustomCreator
        {
            get => _useCustomCreator;
            set => _useCustomCreator = value;
        }

        private Func<Type, IDM_Addin>? _customControlCreator;
        public RoutingManager(IServiceProvider service)
        {
            _serviceProvider = service;
            
            Beepservices = (IBeepService)service.GetService(typeof(IBeepService));
            DMEEditor = Beepservices.DMEEditor;
        }
        public RoutingManager(IComponentContext autofacContext)
        {
            _autofacContext = autofacContext; // Autofac container

            Beepservices = _autofacContext.Resolve<IBeepService>();
            DMEEditor = Beepservices.DMEEditor;
        }
        // Constructor


        #region Navigation

        public async Task<IErrorsInfo> NavigateToAsync(string routeName, Dictionary<string, object> parameters = null, bool popup = false)
        {
            var result = new ErrorsInfo();
            try
            {
                Navigating?.Invoke(this, routeName);

                if (!_routes.ContainsKey(routeName))
                {
                    await NavigateToErrorAsync($"No view registered for route: {routeName}");
                    result.Flag = Errors.Failed;
                    result.Message = $"No view registered for route: {routeName}";
                    return result;
                }

                if (_routeGuards.TryGetValue(routeName, out var guard) && guard != null)
                {
                    if (!guard(parameters ?? new Dictionary<string, object>()))
                    {
                        await NavigateToErrorAsync($"Access denied for route: {routeName}");
                        result.Flag = Errors.Failed;
                        result.Message = $"Access denied for route: {routeName}";
                        return result;
                    }
                }

                lock (_historyLock)
                {
                    _navigationHistory.Push(routeName);
                    _forwardHistory.Clear();
                }

                // Trigger PreShowItem event
                var preShowArgs = new RouteArgs(routeName, parameters);
                PreShowItem?.Invoke(this, preShowArgs);
                if (preShowArgs.Cancel)
                {
                    result.Flag = Errors.Failed;
                    result.Message = "Navigation canceled by PreShowItem.";
                    return result;
                }

                // Remove the current view in SinglePanel mode
                if (_containerType == ContainerTypeEnum.SinglePanel && _currentView != null)
                {
                    _displayContainer.RemoveControl(_currentView.Details.AddinName, _currentView);
                }

                // Create or retrieve the new view
                IDM_Addin view = GetAddin(routeName);

                if (view == null)
                {
                    view = CreateUsingActivator(_routes[routeName]);
                    if (view != null)
                    {
                        view.Dependencies = new Dependencies
                        {
                            DMEEditor = Beepservices.DMEEditor,
                            ErrorObject = Beepservices.DMEEditor.ErrorObject,
                            Logger = Beepservices.lg
                        };
                    }
                }

                if (view == null)
                {
                    await NavigateToErrorAsync($"Failed to create view for route: {routeName}");
                    result.Flag = Errors.Failed;
                    result.Message = $"Failed to create view for route: {routeName}";
                    return result;
                }

                if (view is INavigable navigableView)
                {
                    navigableView.OnNavigatedTo(parameters ?? new Dictionary<string, object>());
                }

                if (!popup)
                {
                    _displayContainer?.AddControl(routeName, view, _containerType);
                }
                else
                {
                    // Config as popup
                    await ShowPopupAsync(view);
                }

                _currentView = view;

                // Trigger PostShowItem event
                PostShowItem?.Invoke(this, new RouteArgs(routeName, parameters));
                Navigated?.Invoke(this, routeName);

                result.Flag = Errors.Ok;
                result.Message = "Navigation successful.";
                return result;
            }
            catch (Exception ex)
            {
                await NavigateToErrorAsync($"An error occurred during navigation: {ex.Message}");
                result.Flag = Errors.Failed;
                result.Message = ex.Message;
                result.Ex = ex;
                return result;
            }
        }
        public async Task<IErrorsInfo> NavigateBackAsync()
        {
            var result = new ErrorsInfo();
            try
            {
                await _navigationLock.WaitAsync();
                if (_navigationHistory.Count <= 1)
                {
                    result.Flag = Errors.Failed;
                    result.Message = "No previous route to navigate back to.";
                    return result;
                }

                _forwardHistory.Push(_navigationHistory.Pop());
                var previousRoute = _navigationHistory.Peek();
                return await NavigateToAsync(previousRoute);
            }
            catch (Exception ex)
            {
                result.Flag = Errors.Failed;
                result.Message = ex.Message;
                result.Ex = ex;
                return result;
            }
            finally
            {
                _navigationLock.Release();
            }
        }
        public async Task<IErrorsInfo> NavigateForwardAsync()
        {
            var result = new ErrorsInfo();
            try
            {
                await _navigationLock.WaitAsync();
                if (_forwardHistory.Count == 0)
                {
                    result.Flag = Errors.Failed;
                    result.Message = "No forward route to navigate to.";
                    return result;
                }

                var nextRoute = _forwardHistory.Pop();
                return await NavigateToAsync(nextRoute);
            }
            catch (Exception ex)
            {
                result.Flag = Errors.Failed;
                result.Message = ex.Message;
                result.Ex = ex;
                return result;
            }
            finally
            {
                _navigationLock.Release();
            }
        }
        private async Task<IErrorsInfo> NavigateToErrorAsync(string errorMessage)
        {
            var result = new ErrorsInfo();
            try
            {
                if (_errorViewType == null)
                {
                    throw new InvalidOperationException("Error view is not set.");
                }

                var errorView = (IDM_Addin)Activator.CreateInstance(_errorViewType);

                if (errorView is IErrorView error)
                {
                    error.SetError(errorMessage);
                }

                if (_containerType == ContainerTypeEnum.SinglePanel && _currentView != null)
                {
                    _displayContainer.RemoveControl(_currentView.Details.AddinName, _currentView);
                }

                _displayContainer.AddControl("Error", errorView, _containerType);
                _currentView = errorView;

                result.Flag = Errors.Ok;
                result.Message = "Error view displayed.";
                return result;
            }
            catch (Exception ex)
            {
                result.Flag = Errors.Failed;
                result.Message = ex.Message;
                result.Ex = ex;
                return result;
            }
        }
        private async Task ShowPopupAsync(IDM_Addin view)
        {
            if (view is Form form)
            {
                if (!form.IsDisposed)
                {
                    // Ensure the dialog is shown on the UI thread
                    await Task.Yield(); // Ensures asynchronous context is maintained
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.ShowDialog();
                }
                else
                {
                    throw new InvalidOperationException("The form is disposed and cannot be shown.");
                }
            }
            else if (view is Control control)
            {
                var popupForm = new Form
                {
                    StartPosition = FormStartPosition.CenterParent,
                    AutoSize = true
                };

                popupForm.Controls.Add(control);
                control.Dock = DockStyle.Fill;

                // Ensure the dialog is shown on the UI thread
                await Task.Yield(); // Ensures asynchronous context is maintained
                popupForm.ShowDialog();
            }
            else
            {
                throw new InvalidOperationException("The provided view is not a valid Form or Control.");
            }
        }

        public IErrorsInfo NavigateTo(string routeName, Dictionary<string, object> parameters = null, bool popup = false)
        {
            var result = new ErrorsInfo();
            try
            {
                Navigating?.Invoke(this, routeName);

                if (!_routes.ContainsKey(routeName))
                {
                     NavigateToError($"No view registered for route: {routeName}");
                    result.Flag = Errors.Failed;
                    result.Message = $"No view registered for route: {routeName}";
                    return result;
                }

                if (_routeGuards.TryGetValue(routeName, out var guard) && guard != null)
                {
                    if (!guard(parameters ?? new Dictionary<string, object>()))
                    {
                         NavigateToError($"Access denied for route: {routeName}");
                        result.Flag = Errors.Failed;
                        result.Message = $"Access denied for route: {routeName}";
                        return result;
                    }
                }

                lock (_historyLock)
                {
                    _navigationHistory.Push(routeName);
                    _forwardHistory.Clear();
                }

                // Trigger PreShowItem event
                var preShowArgs = new RouteArgs(routeName, parameters);
                PreShowItem?.Invoke(this, preShowArgs);
                if (preShowArgs.Cancel)
                {
                    result.Flag = Errors.Failed;
                    result.Message = "Navigation canceled by PreShowItem.";
                    return result;
                }

                // Remove the current view in SinglePanel mode
                if (_containerType == ContainerTypeEnum.SinglePanel && _currentView != null)
                {
                    _displayContainer.RemoveControl(_currentView.Details.AddinName, _currentView);
                }

                // Create or retrieve the new view
                IDM_Addin view = GetAddin(routeName);

                if (view == null)
                {
                    view = CreateUsingActivator(_routes[routeName]);
                    if (view != null)
                    {
                        view.Dependencies = new Dependencies
                        {
                            DMEEditor = Beepservices.DMEEditor,
                            ErrorObject = Beepservices.DMEEditor.ErrorObject,
                            Logger = Beepservices.lg
                        };
                    }
                }

                if (view == null)
                {
                     NavigateToError($"Failed to create view for route: {routeName}");
                    result.Flag = Errors.Failed;
                    result.Message = $"Failed to create view for route: {routeName}";
                    return result;
                }

                if (view is INavigable navigableView)
                {
                    navigableView.OnNavigatedTo(parameters ?? new Dictionary<string, object>());
                }

                if (!popup)
                {
                    _displayContainer?.AddControl(routeName, view, _containerType);
                }
                else
                {
                    // Config as popup
                     ShowPopup(view);
                }

                _currentView = view;

                // Trigger PostShowItem event
                PostShowItem?.Invoke(this, new RouteArgs(routeName, parameters));
                Navigated?.Invoke(this, routeName);

                result.Flag = Errors.Ok;
                result.Message = "Navigation successful.";
                return result;
            }
            catch (Exception ex)
            {
                NavigateToError($"An error occurred during navigation: {ex.Message}");
                result.Flag = Errors.Failed;
                result.Message = ex.Message;
                result.Ex = ex;
                return result;
            }
        }
        public IErrorsInfo NavigateBack()
        {
            var result = new ErrorsInfo();
            try
            {
                _navigationLock.Wait();
                if (_navigationHistory.Count <= 1)
                {
                    result.Flag = Errors.Failed;
                    result.Message = "No previous route to navigate back to.";
                    return result;
                }

                _forwardHistory.Push(_navigationHistory.Pop());
                var previousRoute = _navigationHistory.Peek();
                return NavigateTo(previousRoute);
            }
            catch (Exception ex)
            {
                result.Flag = Errors.Failed;
                result.Message = ex.Message;
                result.Ex = ex;
                return result;
            }
            finally
            {
                _navigationLock.Release();
            }
        }
        public  IErrorsInfo NavigateForward()
        {
            var result = new ErrorsInfo();
            try
            {
                 _navigationLock.WaitAsync();
                if (_forwardHistory.Count == 0)
                {
                    result.Flag = Errors.Failed;
                    result.Message = "No forward route to navigate to.";
                    return result;
                }

                var nextRoute = _forwardHistory.Pop();
                return  NavigateTo(nextRoute);
            }
            catch (Exception ex)
            {
                result.Flag = Errors.Failed;
                result.Message = ex.Message;
                result.Ex = ex;
                return result;
            }
            finally
            {
                _navigationLock.Release();
            }
        }
        private IErrorsInfo NavigateToError(string errorMessage)
        {
            var result = new ErrorsInfo();
            try
            {
                if (_errorViewType == null)
                {
                    throw new InvalidOperationException("Error view is not set.");
                }

                var errorView = (IDM_Addin)Activator.CreateInstance(_errorViewType);

                if (errorView is IErrorView error)
                {
                    error.SetError(errorMessage);
                }

                if (_containerType == ContainerTypeEnum.SinglePanel && _currentView != null)
                {
                    _displayContainer.RemoveControl(_currentView.Details.AddinName, _currentView);
                }

                _displayContainer.AddControl("Error", errorView, _containerType);
                _currentView = errorView;

                result.Flag = Errors.Ok;
                result.Message = "Error view displayed.";
                return result;
            }
            catch (Exception ex)
            {
                result.Flag = Errors.Failed;
                result.Message = ex.Message;
                result.Ex = ex;
                return result;
            }
        }
        private async void ShowPopup(IDM_Addin view)
        {
            try
            {
               

                if (view is Form form)
                {
          
                    if (!form.IsDisposed)
                    {
                        IBeepUIComponent beepUIComponent = (IBeepUIComponent)view;
                        form.Load += (s, e) =>
                        {
                            MiscFunctions.SetThemePropertyinControlifexist( form, Theme);
                        };
                        // Ensure this runs on the UI thread
                        if (form.InvokeRequired)
                        {
                            form.Invoke(new Action(() =>
                            {
                                form.StartPosition = FormStartPosition.CenterParent;
                               
                                form.Show();
                            }));
                        }
                        else
                        {
                            form.StartPosition = FormStartPosition.CenterParent;
                            beepUIComponent.Theme = Theme;
                            form.Show();
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("The form is disposed and cannot be shown.");
                    }
                }
                else if (view is Control control)
                {
                    var popupForm = new Form
                    {
                        StartPosition = FormStartPosition.CenterParent,
                        AutoSize = true
                    };

                    popupForm.Controls.Add(control);
                    control.Dock = DockStyle.Fill;

                    // Ensure the dialog is shown on the UI thread
                    popupForm.Show();
                }
                else
                {
                    throw new InvalidOperationException("The provided view is not a valid Form or Control.");
                }
            }
            catch (Exception ex)
            {
                // Log the error
                DMEEditor.AddLogMessage("Beep", $"Error in ShowPopup: {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
        }
        #endregion


        #region Breadcrumb and History
        public string BreadCrumb
        {
            get
            {
                return string.Join(" > ", _navigationHistory.Reverse().Select(route =>
                {
                    var (name, parameters) = ParseRoute(route);
                    var paramString = string.Join(", ", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));
                    return string.IsNullOrEmpty(paramString) ? name : $"{name} ({paramString})";
                }));
            }
        }
        #endregion

        #region Routing
     
        public void RegisterRouteByName(string routeName, string moduleOrAddinName, RouteGuard guard = null)
        {
            if (string.IsNullOrWhiteSpace(routeName))
                throw new ArgumentException("Route name cannot be null or empty.", nameof(routeName));

            if (string.IsNullOrWhiteSpace(moduleOrAddinName))
                throw new ArgumentException("Module or Add-in name cannot be null or empty.", nameof(moduleOrAddinName));

            // Find the type of the module or add-in from services
            var type = FindAddinTypeFromServices(moduleOrAddinName);
            if (type == null)
                throw new ArgumentException($"No add-in or module found with name: {moduleOrAddinName}");

            // Register the route using the resolved type
            RegisterRoute(routeName, type, guard);
        }
        public void RegisterRoute(string routeName, Type viewType, RouteGuard guard = null)
        {
            if (string.IsNullOrWhiteSpace(routeName))
                throw new ArgumentException("Route name cannot be null or empty.", nameof(routeName));

            if (viewType == null || !typeof(IDM_Addin).IsAssignableFrom(viewType))
                throw new ArgumentException("View type must implement IDM_Addin.", nameof(viewType));

            if (!_routes.ContainsKey(routeName))
            {
                _routes[routeName] = viewType;
                _viewCache[routeName] = new Lazy<IDM_Addin>(() => CreateUsingActivator(viewType));
            }

            if (guard != null)
            {
                _routeGuards[routeName] = guard;
            }
        }

        public void RegisterAlias(string alias, string routeName)
        {
            if (!_routes.ContainsKey(routeName))
                throw new ArgumentException($"No view registered for route: {routeName}");

            _aliases[alias] = routeName;
        }

        public void SetDefaultRoute(string routeName)
        {
            if (!_routes.ContainsKey(routeName))
                throw new ArgumentException($"No view registered for route: {routeName}");

            _defaultRoute = routeName;
        }

        public void SetErrorView(Type errorViewType)
        {
            if (errorViewType == null || !typeof(IDM_Addin).IsAssignableFrom(errorViewType))
                throw new ArgumentException("Error view type must implement IDM_Addin.", nameof(errorViewType));

            _errorViewType = errorViewType;
        }

        public (string RouteName, Dictionary<string, object> Parameters) ParseRoute(string routeWithParams)
        {
            var uri = new Uri($"http://dummy{routeWithParams}");
            var routeName = uri.AbsolutePath.Trim('/');
            var parameters = HttpUtility.ParseQueryString(uri.Query)
                .AllKeys
                .ToDictionary(key => key, key => (object)HttpUtility.UrlDecode(HttpUtility.ParseQueryString(uri.Query)[key]));
            return (routeName, parameters);
        }

        #endregion

        #region View Creation
        public IDM_Addin GetAddin(string routeName)
        {
            // Create or retrieve the new view
            IDM_Addin view = _useCustomCreator && _isCustomCreatorSet
                ? CreateControlUsingCustomCreator(_routes[routeName])
                : ResolveAddin(_routes[routeName]);
            return view;
        }
        private IDM_Addin ResolveAddin(Type viewType)
        {
            if (_autofacContext != null)
            {
                // Use Autofac to resolve the view
                return _autofacContext.ResolveKeyed<IDM_Addin>(viewType.Name); // Resolve by type name
            }
            else if (_serviceProvider != null)
            {
                // Use Microsoft DI to resolve the view
                return CreateUsingServiceLocator(viewType);
            }
            throw new InvalidOperationException("No dependency injection container is available.");
        }
        public Type FindAddinTypeFromServices(string moduleOrAddinName)
        {
            if (_autofacContext != null)
            {
                try
                {
                    // Resolve the IDM_Addin by key (type name)
                    var addin = _autofacContext.ResolveKeyed<IDM_Addin>(moduleOrAddinName);

                    if (addin != null)
                    {
                        return addin.GetType();
                    }
                }
                catch (Autofac.Core.Registration.ComponentNotRegisteredException)
                {
                    // Handle the case where the key is not registered
                    Console.WriteLine($"Addin with key '{moduleOrAddinName}' is not registered.");
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    Console.WriteLine($"Error resolving addin: {ex.Message}");
                }
            }
            else if (_serviceProvider != null)
            {
                // Retrieve all registered services
                foreach (var service in _serviceProvider.GetServices(typeof(IDM_Addin)))
                {
                    Control addin = (Control)service;
                    if (addin.Name.Contains(moduleOrAddinName, StringComparison.OrdinalIgnoreCase))
                    {
                        return service.GetType();
                    }
                }
            }
           

            return null;
        }
        private IDM_Addin CreateUsingServiceLocator(Type viewType)
        {
            // Retrieve all registered services of type IDM_Addin
            var services = _serviceProvider.GetServices(typeof(IDM_Addin));
            if (services == null)
                throw new InvalidOperationException("No add-ins are registered in the service locator.");

            // Find the matching add-in instance by type
            foreach (var service in services)
            {
                if (service.GetType() == viewType && service is IDM_Addin addin)
                {
                    return addin;
                }
            }

            // If no matching instance is found, return null or throw an exception
            throw new InvalidOperationException($"No add-in found for type: {viewType.FullName}");
        }

        private IDM_Addin CreateUsingActivator(Type viewType)
        {
            return (IDM_Addin)Activator.CreateInstance(viewType);
        }

        private IDM_Addin CreateControlUsingCustomCreator(Type viewType)
        {
            if (_isCustomCreatorSet && _customControlCreator != null)
            {
                return _customControlCreator(viewType);
            }
            throw new InvalidOperationException("Custom control creator is not set.");
        }

        public bool SetControlCreator(Func<Type, IDM_Addin> customCreator)
        {
            _customControlCreator = customCreator;
            _isCustomCreatorSet = true;
            return _isCustomCreatorSet;
        }

        #endregion
      
    }


}



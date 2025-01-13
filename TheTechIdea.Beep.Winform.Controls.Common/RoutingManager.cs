using System.Web;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Container.Services;

using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Desktop.Common
{
    public partial class RoutingManager : IRoutingManager
    {
        public IBeepService Beepservices { get; }
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

        // Constructor
        public RoutingManager(IDisplayContainer displayContainer, IBeepService beepservices = null, ContainerTypeEnum containerType = ContainerTypeEnum.SinglePanel)
        {
            _displayContainer = displayContainer ?? throw new ArgumentNullException(nameof(displayContainer));
            Beepservices = beepservices;
            _containerType = containerType;
        }

        #region Navigation
        public void NavigateTo(string routeName, Dictionary<string, object> parameters = null)
        {
            try
            {
                Navigating?.Invoke(this, routeName);

                if (!_routes.ContainsKey(routeName))
                {
                    NavigateToError($"No view registered for route: {routeName}");
                    return;
                }

                if (_routeGuards.TryGetValue(routeName, out var guard) && guard != null)
                {
                    if (!guard(parameters ?? new Dictionary<string, object>()))
                    {
                        NavigateToError($"Access denied for route: {routeName}");
                        return;
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
                if (preShowArgs.Cancel) return;

                // Remove the current view in SinglePanel mode
                if (_containerType == ContainerTypeEnum.SinglePanel && _currentView != null)
                {
                    _displayContainer.RemoveControl(_currentView.Details.AddinName, _currentView);
                }

                // Create or retrieve the new view
                IDM_Addin view = _useCustomCreator && _isCustomCreatorSet
                    ? CreateControlUsingCustomCreator(_routes[routeName])
                    : CreateUsingActivator(_routes[routeName]);

                if (view is INavigable navigableView)
                {
                    navigableView.OnNavigatedTo(parameters ?? new Dictionary<string, object>());
                }

                // Add the new view to the display container
                _displayContainer.AddControl(routeName, view, _containerType);
                _currentView = view;

                // Trigger PostShowItem event
                PostShowItem?.Invoke(this, new RouteArgs(routeName, parameters));
                Navigated?.Invoke(this, routeName);
            }
            catch (Exception ex)
            {
                NavigateToError($"An error occurred during navigation: {ex.Message}");
            }
        }

        public void NavigateBack()
        {
            lock (_historyLock)
            {
                if (_navigationHistory.Count <= 1) return;

                _forwardHistory.Push(_navigationHistory.Pop());
                var previousRoute = _navigationHistory.Peek();
                NavigateTo(previousRoute);
            }
        }

        public void NavigateForward()
        {
            lock (_historyLock)
            {
                if (_forwardHistory.Count == 0) return;

                var nextRoute = _forwardHistory.Pop();
                NavigateTo(nextRoute);
            }
        }

        private void NavigateToError(string errorMessage)
        {
            if (_errorViewType == null)
                throw new InvalidOperationException("Error view is not set.");

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

    public delegate bool RouteGuard(Dictionary<string, object> parameters);
}

public interface IRouteArgs
{
    string RouteName { get; }
    Dictionary<string, object> Parameters { get; }
    bool Cancel { get; set; }
}

public class RouteArgs : IRouteArgs
{
    public string RouteName { get; }
    public Dictionary<string, object> Parameters { get; }
    public bool Cancel { get; set; }

    public RouteArgs(string routeName, Dictionary<string, object> parameters)
    {
        RouteName = routeName;
        Parameters = parameters;
    }
}

using System.Web;
namespace TheTechIdea.Beep.Desktop.Common;
public partial class ViewRouter
{

    private readonly object _historyLock = new object();
    private readonly Panel _hostPanel;
    private readonly Dictionary<string, Lazy<Control>> _viewCache = new Dictionary<string, Lazy<Control>>();
    private readonly Dictionary<string, Type> _routes = new Dictionary<string, Type>();
    private readonly Dictionary<string, RouteGuard> _routeGuards = new Dictionary<string, RouteGuard>();
    private readonly Dictionary<string, string> _aliases = new Dictionary<string, string>();
    private readonly Stack<string> _navigationHistory = new Stack<string>();
    private readonly Stack<string> _forwardHistory = new Stack<string>();
    private string _defaultRoute;
    private Control _currentView;
    private Type _errorViewType;

    public event EventHandler<string> Navigating;
    public event EventHandler<string> Navigated;

    // New Events
    public event EventHandler<IRouteArgs> PreClose;
    public event EventHandler<IRouteArgs> PreLogin;
    public event EventHandler<IRouteArgs> PostLogin;
    public event EventHandler<IRouteArgs> PreShowItem;
    public event EventHandler<IRouteArgs> PostShowItem;
    private bool _isCustomeCreatorSet = false;
    private bool _useCustomeCreator=false;
    
    public Control CurrentControl => _currentView;

    public bool UseCustomeCreator
    {
        get => _useCustomeCreator; set => _useCustomeCreator = value;
    }

    private Func<Type, Control>? _customControlCreator;
    public bool SetControlCreator(Func<Type, Control> customCreator)
    {
        _customControlCreator = customCreator;
        _isCustomeCreatorSet = true; // Indicate that a custom creator is set
        return _isCustomeCreatorSet;
    }
    public delegate bool RouteGuard(Dictionary<string, object> parameters);
    public ViewRouter(Panel hostPanel)
    {
        _hostPanel = hostPanel ?? throw new ArgumentNullException(nameof(hostPanel));
    }
    public void RegisterRoute(string routeName, Type viewType, RouteGuard guard = null)
    {
        if (string.IsNullOrWhiteSpace(routeName))
            throw new ArgumentException("Route name cannot be null or empty.", nameof(routeName));

        if (viewType == null || !typeof(Control).IsAssignableFrom(viewType))
            throw new ArgumentException("View type must be a subclass of Control.", nameof(viewType));

        if (!_routes.ContainsKey(routeName))
        {
            _routes[routeName] = viewType;
            _viewCache[routeName] = new Lazy<Control>(() => CreateUsingActivator(viewType));
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
        if (errorViewType == null || !typeof(Control).IsAssignableFrom(errorViewType))
            throw new ArgumentException("Error view type must be a subclass of Control.", nameof(errorViewType));

        _errorViewType = errorViewType;
    }

    public void NavigateToDefault()
    {
        if (string.IsNullOrEmpty(_defaultRoute))
            throw new InvalidOperationException("Default route is not set.");

        NavigateTo(_defaultRoute);
    }

    public void NavigateTo(string routeWithParams)
    {
        if (string.IsNullOrWhiteSpace(routeWithParams))
            throw new ArgumentException("Route cannot be null or empty.", nameof(routeWithParams));

        var (routeName, parameters) = ParseRoute(routeWithParams);

        if (_aliases.TryGetValue(routeName, out var actualRoute))
        {
            routeName = actualRoute;
        }

        NavigateTo(routeName, parameters);
    }

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

            _navigationHistory.Push(routeName);
            _forwardHistory.Clear();

            // Trigger PreShowItem event
            var preShowArgs = new RouteArgs(routeName, parameters);
            PreShowItem?.Invoke(this, preShowArgs);
            if (preShowArgs.Cancel) return;

            // Dispose the current view if it exists
            _currentView?.Dispose();

            // Create a new instance of the view
            Control view = _useCustomeCreator && _isCustomeCreatorSet
                ? CreateControlUsingCustomeCreator(_routes[routeName])
                : CreateUsingActivator(_routes[routeName]);

            if (view is INavigable navigableView)
            {
                navigableView.OnNavigatedTo(parameters ?? new Dictionary<string, object>());
            }

            _hostPanel.Controls.Clear();
            view.Dock = DockStyle.Fill;
            _hostPanel.Controls.Add(view);

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

    private Control CreateUsingActivator(Type viewType)
    {

        return (Control)Activator.CreateInstance(viewType);
    }
    public Control? CreateControlUsingCustomeCreator(Type viewType)
    {
        if (_isCustomeCreatorSet && _customControlCreator != null)
        {
            // Call the custom creator to create the control
            return _customControlCreator(viewType);
        }
        else
        {
            throw new InvalidOperationException("Custom control creator is not set.");
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

        var errorView = (Control)Activator.CreateInstance(_errorViewType);

        if (errorView is IErrorView error)
        {
            error.SetError(errorMessage);
        }

        _hostPanel.Controls.Clear();
        errorView.Dock = DockStyle.Fill;
        _hostPanel.Controls.Add(errorView);

        _currentView = errorView;
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

    // Breadcrumb Property
    public string Breadcrumb
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

}

public interface INavigable
{
    void OnNavigatedTo(Dictionary<string, object> parameters);
}

public interface IErrorView
{
    void SetError(string message);
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

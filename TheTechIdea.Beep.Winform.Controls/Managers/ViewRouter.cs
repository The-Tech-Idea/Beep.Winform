using System.Web;

public class ViewRouter
{
    private readonly Panel _hostPanel;
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

        _routes[routeName] = viewType;
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
        var viewType = _routes[routeName];
        var view = (Control)Activator.CreateInstance(viewType);

        // If the view implements INavigable, pass parameters
        if (view is INavigable navigableView)
        {
            navigableView.OnNavigatedTo(parameters ?? new Dictionary<string, object>());
        }

        // Add the new view to the host panel
        _hostPanel.Controls.Clear();
        view.Dock = DockStyle.Fill;
        _hostPanel.Controls.Add(view);

        _currentView = view;

        // Trigger PostShowItem event
        PostShowItem?.Invoke(this, new RouteArgs(routeName, parameters));

        Navigated?.Invoke(this, routeName);
    }

    public void NavigateBack()
    {
        if (_navigationHistory.Count <= 1) return;

        _forwardHistory.Push(_navigationHistory.Pop());
        var previousRoute = _navigationHistory.Peek();
        NavigateTo(previousRoute);
    }

    public void NavigateForward()
    {
        if (_forwardHistory.Count == 0) return;

        var nextRoute = _forwardHistory.Pop();
        NavigateTo(nextRoute);
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
            return string.Join(" > ", _navigationHistory.Reverse());
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

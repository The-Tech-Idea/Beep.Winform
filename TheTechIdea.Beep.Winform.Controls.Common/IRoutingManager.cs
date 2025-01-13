using TheTechIdea.Beep.Addin;

namespace TheTechIdea.Beep.Desktop.Common
{
    public interface IRoutingManager
    {
        string BreadCrumb { get; }
        IDM_Addin CurrentControl { get; }
        bool UseCustomCreator { get; set; }

        event EventHandler<string> Navigated;
        event EventHandler<string> Navigating;
        event EventHandler<IRouteArgs> PostShowItem;
        event EventHandler<IRouteArgs> PreShowItem;

        void NavigateBack();
        void NavigateForward();
        void NavigateTo(string routeName, Dictionary<string, object> parameters = null);
        (string RouteName, Dictionary<string, object> Parameters) ParseRoute(string routeWithParams);
        void RegisterAlias(string alias, string routeName);
        void RegisterRoute(string routeName, Type viewType, RouteGuard guard = null);
        bool SetControlCreator(Func<Type, IDM_Addin> customCreator);
        void SetDefaultRoute(string routeName);
        void SetErrorView(Type errorViewType);
    }
}
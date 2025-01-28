using System.Collections.Generic;
using System;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Vis.Modules
{
    public delegate bool RouteGuard(Dictionary<string, object> parameters);
    public interface IRoutingManager
    {
        EnumBeepThemes Theme { get; set; }
        string BreadCrumb { get; }
        IDM_Addin CurrentControl { get; }
        bool UseCustomCreator { get; set; }

        event EventHandler<string> Navigated;
        event EventHandler<string> Navigating;
        event EventHandler<IRouteArgs> PostShowItem;
        event EventHandler<IRouteArgs> PreShowItem;

        IErrorsInfo NavigateBack();
        IErrorsInfo NavigateForward();
        IErrorsInfo NavigateTo(string routeName, Dictionary<string, object> parameters = null, bool popup = false);
        Task<IErrorsInfo> NavigateUriAsync(string uri, bool popup = false);
        Task<IErrorsInfo> NavigateBackAsync();
        Task<IErrorsInfo> NavigateForwardAsync();
        Task<IErrorsInfo> NavigateToAsync(string routeName, Dictionary<string, object> parameters = null,bool popup = false);
        (string RouteName, Dictionary<string, object> Parameters) ParseRoute(string routeWithParams);
        void RegisterAlias(string alias, string routeName);
        void RegisterRoute(string routeName, Type viewType, RouteGuard guard = null);
        void RegisterRouteByName(string routeName, string moduleOrAddinName, RouteGuard guard = null);
        bool SetControlCreator(Func<Type, IDM_Addin> customCreator);
        void SetDefaultRoute(string routeName);
        void SetErrorView(Type errorViewType);
        Type FindAddinTypeFromServices(string moduleOrAddinName);
        IDM_Addin GetAddin(string moduleOrAddinName);
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
}
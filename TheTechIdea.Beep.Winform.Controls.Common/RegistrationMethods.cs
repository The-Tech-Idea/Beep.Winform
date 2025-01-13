using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Desktop.Common
{
    public static class RegistrationMethods
    {
        public static IServiceCollection RegisterVisualManager(this IServiceCollection services)
        {
           
            services.AddSingleton<IVisManager, VisualManager>();
            return services;
        }
        public static IServiceCollection RegisterRoutingManager(this IServiceCollection services)
        {

            services.AddSingleton<IRoutingManager, RoutingManager>();
            return services;
        }
        public static IServiceCollection RegisterBeepViewModels(this IServiceCollection services)
        {
            // i want to register all viewmodels here , these class's implement IBeepViewModel
            // Load assemblies from DependencyContext
            var loadedAssemblies = DependencyContext.Default.RuntimeLibraries
                .SelectMany(lib => lib.GetDefaultAssemblyNames(DependencyContext.Default))
                .Select(Assembly.Load)
                .ToList();

            // Find all types implementing IBeepViewModel
            var viewModelTypes = loadedAssemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => typeof(IBeepViewModel).IsAssignableFrom(t) && !t.IsAbstract);

            // Register each ViewModel type
            foreach (var viewModelType in viewModelTypes)
            {
                services.AddScoped(viewModelType);
            }
            return services;
        }
        public static void RegisterAddinsAsRoutes(this IRoutingManager routingManager)
        {
            if (routingManager == null) throw new ArgumentNullException(nameof(routingManager));

            // Load all assemblies in the current AppDomain
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Find all types that implement IDM_Addin and are decorated with AddinAttribute
            var addinTypes = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IDM_Addin).IsAssignableFrom(type) && !type.IsAbstract)
                .Where(type => type.GetCustomAttributes(typeof(AddinAttribute), false).Any());

            foreach (var addinType in addinTypes)
            {
                // Get the AddinAttribute applied to the type
                var attribute = (AddinAttribute)addinType.GetCustomAttributes(typeof(AddinAttribute), false).FirstOrDefault();
                if (attribute == null)
                {
                    Console.WriteLine($"Warning: {addinType.Name} does not have a valid AddinAttribute. Skipping.");
                    continue;
                }

                var addinName = attribute.Name;
                if (string.IsNullOrWhiteSpace(addinName))
                {
                    Console.WriteLine($"Warning: Add-in {addinType.Name} has an invalid or missing Name in AddinAttribute. Skipping.");
                    continue;
                }

                // Register the route using the attribute's Name
                routingManager.RegisterRoute(addinName, addinType);
            }
        }

        
   
}
}

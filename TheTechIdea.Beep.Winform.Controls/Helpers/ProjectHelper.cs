using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    public static class ProjectHelper
    {
        public static List<Type> GetProjectTypes(IServiceProvider serviceProvider)
        {
            var typeDiscoverySvc = (ITypeDiscoveryService)serviceProvider
                .GetService(typeof(ITypeDiscoveryService));
            var types = typeDiscoverySvc.GetTypes(typeof(object), true)
                .Cast<Type>()
                .Where(item =>
                    item.IsPublic &&
                    typeof(Form).IsAssignableFrom(item) &&
                    !item.FullName.StartsWith("System")
                ).ToList();
            return types;
        }
        public static Type GetTypeFromName(IServiceProvider serviceProvider, string typeName)
        {
            ITypeResolutionService typeResolutionSvc = (ITypeResolutionService)serviceProvider
                .GetService(typeof(ITypeResolutionService));
            return typeResolutionSvc.GetType(typeName);
        }
        public static List<IComponent> GetSelectableComponents(IDesignerHost host)
        {
            var components = host.Container.Components;
            var list = new List<IComponent>();
            foreach (IComponent c in components)
                list.Add(c);
            for (var i = 0; i < list.Count; ++i)
            {
                var component1 = list[i];
                if (component1.Site != null)
                {
                    var service = (INestedContainer)component1.Site.GetService(typeof(INestedContainer));
                    if (service != null && service.Components.Count > 0)
                    {
                        foreach (IComponent component2 in service.Components)
                        {
                            if (!list.Contains(component2))
                                list.Add(component2);
                        }
                    }
                }
            }
            return list;
        }
        public static IEnumerable<Type> DiscoverUnitOfWorkTypes(ITypeDiscoveryService typeDiscoveryService)
        {
            if (typeDiscoveryService == null) return Enumerable.Empty<Type>();

            // Discover all types in the current project and references
            var allTypes = typeDiscoveryService.GetTypes(typeof(object), true)
                                               .Cast<Type>();

            // Filter types that implement IUnitofWork
            return allTypes.Where(type => IsUnitOfWorkType(type));
        }
        public static bool IsUnitOfWorkType(Type type)
        {
            return type == typeof(IUnitofWork) ||
                   type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IUnitofWork<>)) ||
                   (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(UnitofWork<>));
        }
        public static List<IComponent> GetUnitOfWorkComponents(IDesignerHost host)
        {
            if (host == null || host.Container == null)
                return new List<IComponent>();

            // Retrieve all components in the designer host
            var components = GetSelectableComponents(host);

            // Filter components implementing IUnitofWork
            return components.Where(c => IsUnitOfWorkType(c.GetType())).ToList();
        }
        //public static List<IComponent> GetUnitOfWorkEncapsulatingComponents(IDesignerHost host)
        //{
        //    if (host == null || host.Container == null)
        //        return new List<IComponent>();

        //    var components = GetSelectableComponents(host);

        //    // Filter components that encapsulate IUnitofWork in their properties or fields
        //    return components.Where(c => HasUnitOfWorkPropertyOrField(c)).ToList();
        //}
        public static List<Type> GetUnitOfWorkEncapsulatingTypesDesignTime(IServiceProvider serviceProvider)
        {
            // Retrieve the type discovery service from the design-time context
            var typeDiscoveryService = (ITypeDiscoveryService)serviceProvider.GetService(typeof(ITypeDiscoveryService));

            if (typeDiscoveryService == null)
                return new List<Type>();

            // Use the type discovery service to get all relevant types
            var allTypes = typeDiscoveryService.GetTypes(typeof(object), true).Cast<Type>();

            // Filter types that contain properties of type IUnitOfWork
            var unitOfWorkTypes = GetProjectTypes(serviceProvider)
                .Where(type => HasUnitOfWorkPropertyOrField(type))
                .Where(type => type.IsPublic && !type.FullName.StartsWith("System"))
                .ToList();

            return unitOfWorkTypes;
        }
        private static bool HasUnitOfWorkPropertyOrField(object obj)
        {
            if (obj == null) return false;

            var type = obj.GetType();

            // Check for IUnitofWork properties
            var hasUnitOfWorkProperty = type.GetProperties()
                                            .Any(p => IsUnitOfWorkType(p.PropertyType));

            // Check for IUnitofWork fields
            var hasUnitOfWorkField = type.GetFields()
                                         .Any(f => IsUnitOfWorkType(f.FieldType));

            return hasUnitOfWorkProperty || hasUnitOfWorkField;
        }
       

    }
}

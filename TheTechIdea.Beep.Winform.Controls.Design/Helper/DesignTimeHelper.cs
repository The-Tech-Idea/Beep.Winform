using System;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TheTechIdea.Beep.Editor;
using VSLangProj;

namespace TheTechIdea.Beep.Winform.Controls.Design.Helper
{
    public static class DesignTimeHelper
    { /// <summary>
      /// Finds all implementations of IUnitofWork in the current project and its referenced assemblies during design time.
      /// </summary>
      /// <summary>
      /// Finds all implementations of IUnitofWork in the active project and its referenced assemblies.
      /// </summary>
        public static List<Type> GetUnitOfWorkImplementations(IServiceProvider serviceProvider)
        {
            var unitOfWorkTypes = new List<Type>();

            // Get the active project via EnvDTE
            var dte = serviceProvider.GetService(typeof(EnvDTE.DTE)) as DTE;
            if (dte == null)
            {
                throw new InvalidOperationException("Unable to access the Visual Studio DTE.");
            }

            var activeProject = ProjectPathHelper.GetActiveProject(dte);
            if (activeProject == null)
            {
                throw new InvalidOperationException("No active project found.");
            }

            // Analyze the project itself for IUnitofWork implementations
            var projectTypes = GetTypesFromProject(activeProject);
            unitOfWorkTypes.AddRange(projectTypes.Where(IsUnitOfWorkType));

            // Analyze referenced assemblies for IUnitofWork implementations
            var referencedAssemblies = GetReferencedAssemblies(activeProject);
            foreach (var assembly in referencedAssemblies)
            {
                try
                {
                    var types = assembly.GetTypes().Where(IsUnitOfWorkType);
                    unitOfWorkTypes.AddRange(types);
                }
                catch
                {
                    // Ignore assemblies that fail to load
                }
            }

            return unitOfWorkTypes;
        }
        /// <summary>
        /// Determines whether a type is an implementation of IUnitofWork.
        /// </summary>
        private static bool IsUnitOfWorkType(Type type)
        {
            return typeof(IUnitofWork).IsAssignableFrom(type) ||
                   type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IUnitofWork<>)) ||
                   (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(UnitofWork<>));
        }
        /// <summary>
        /// Extracts all types defined in the project's source files.
        /// </summary>
        private static IEnumerable<Type> GetTypesFromProject(EnvDTE.Project project)
        {
            var sourceFilePaths = GetSourceFilesFromProject(project);
            var compiledAssembly = CompileSourceFilesToMemory(sourceFilePaths);
            return compiledAssembly?.GetTypes() ?? Enumerable.Empty<Type>();
        }

        /// <summary>
        /// Gets all source files from the project.
        /// </summary>
        private static List<string> GetSourceFilesFromProject(EnvDTE.Project project)
        {
            var sourceFiles = new List<string>();
            foreach (ProjectItem item in project.ProjectItems)
            {
                CollectSourceFiles(item, sourceFiles);
            }
            return sourceFiles;
        }
        private static void CollectSourceFiles(ProjectItem item, List<string> sourceFiles)
        {
            if (item.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile &&
                item.Name.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            {
                sourceFiles.Add(item.FileNames[1]);
            }

            foreach (ProjectItem subItem in item.ProjectItems)
            {
                CollectSourceFiles(subItem, sourceFiles);
            }
        }
        /// <summary>
        /// Compiles a list of source files into an in-memory assembly.
        /// </summary>
        private static Assembly CompileSourceFilesToMemory(List<string> sourceFiles)
        {
            // Use Roslyn or CodeDOM to compile the source files into an in-memory assembly
            // (This implementation is left as an exercise)
           return RoslynCompiler.CompileSourceFilesToMemory(sourceFiles);
        }
        /// <summary>
        /// Gets all referenced assemblies for the project.
        /// </summary>
        private static List<Assembly> GetReferencedAssemblies(EnvDTE.Project project)
        {
            var referencedAssemblies = new List<Assembly>();
            foreach (Reference reference in GetProjectReferences(project))
            {
                try
                {
                    if (!string.IsNullOrEmpty(reference.Path))
                    {
                        var assembly = Assembly.LoadFrom(reference.Path);
                        referencedAssemblies.Add(assembly);
                    }
                }
                catch
                {
                    // Ignore references that cannot be loaded
                }
            }

            return referencedAssemblies;
        }
        private static IEnumerable<Reference> GetProjectReferences(EnvDTE.Project project)
        {
            if (project.Object is VSProject vsProject)
            {
                return vsProject.References.Cast<Reference>();
            }
            return Enumerable.Empty<Reference>();
        }
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

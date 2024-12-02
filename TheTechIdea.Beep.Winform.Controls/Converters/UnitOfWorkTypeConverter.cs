using TheTechIdea.Beep.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;

namespace TheTechIdea.Beep.Winform.Controls.Converters;
public class UnitOfWorkConverter : TypeConverter
{
    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

    public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
        if (context?.Container == null) return new StandardValuesCollection(Array.Empty<object>());

        // Use ITypeDiscoveryService to get types
       
        var typeDiscoveryService = (ITypeDiscoveryService)context.GetService(typeof(ITypeDiscoveryService));
        if (typeDiscoveryService == null) return new StandardValuesCollection(Array.Empty<object>());

        // Retrieve all public, non-system types that implement IUnitOfWork or UnitOfWork<T>
        var unitOfWorkTypes = typeDiscoveryService.GetTypes(typeof(object), true)
            .Cast<Type>()
            .Where(t => t.IsPublic && !t.FullName.StartsWith("System") && IsUnitOfWorkType(t))
            .ToList();

        // Filter instances of these types within the current context container
        var unitOfWorkInstances = new List<object>();
        foreach (var type in unitOfWorkTypes)
        {
            // Find properties and fields of UnitOfWork type within the form
            TypeInfo typeInfo = type.GetTypeInfo();
            var properties = typeInfo.GetProperties().Where(p => IsUnitOfWorkType(p.PropertyType));
            var fields = type.GetFields().Where(f => IsUnitOfWorkType(f.FieldType));

            Console.WriteLine($"Detected {properties.Count()} properties instances.");

            // Collect instances from each form
            foreach (var property in properties)
            {
                var instance = property.GetValue(context.Instance);
                if (instance != null) unitOfWorkInstances.Add(instance);
            }
            foreach (var field in fields)
            {
                var instance = field.GetValue(context.Instance);
                if (instance != null) unitOfWorkInstances.Add(instance);
            }
        }

        Console.WriteLine($"Detected {unitOfWorkInstances.Count} UnitOfWork instances.");
        return new StandardValuesCollection(unitOfWorkInstances);
    }

    private bool IsUnitOfWorkType(Type type)
    {
        return type == typeof(IUnitofWork) ||
               type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IUnitofWork<>)) ||
               (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(UnitofWork<>));
    }
}

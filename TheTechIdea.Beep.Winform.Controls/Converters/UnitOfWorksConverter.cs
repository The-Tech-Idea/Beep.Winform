using static System.ComponentModel.TypeConverter;
using System.ComponentModel;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Editor.UOW;
using System.Reflection;

public class UnitOfWorksConverter : TypeConverter
{
    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

    public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
     //  // Console.WriteLine("UnitOfWorksConverter: GetStandardValues called");

        if (context?.Instance is not Component component)
        {
          // // Console.WriteLine("Context instance is not a valid component");
            return new StandardValuesCollection(Array.Empty<object>());
        }

        var parentForm = FindParentForm(component);
        if (parentForm == null)
        {
           // Console.WriteLine("No parent form found");
            return new StandardValuesCollection(Array.Empty<object>());
        }

        // Log all properties in the form
       // Console.WriteLine("Inspecting properties on parent form...");
       // foreach (var prop in parentForm.GetType().GetProperties())
        //{
        //   // Console.WriteLine($"Property: {prop.Name}, Type: {prop.PropertyType}");
        //}

        // Find UnitOfWork properties
        var unitOfWorkInstances = parentForm.GetType()
            .GetProperties()
            .Where(p => IsUnitOfWorkType(p.PropertyType))
            .Select(p => p.GetValue(parentForm))
            .Where(instance => instance != null)
            .ToList();

       // Console.WriteLine($"Detected UnitOfWork instances: {unitOfWorkInstances.Count}");
        return new StandardValuesCollection(unitOfWorkInstances);
    }

    private Form FindParentForm(Component component)
    {
        // Use reflection to find properties in both Form1 and Form1.Designer.cs
        var parentForm = component.Site?.Container?.Components.OfType<Form>().FirstOrDefault();
        if (parentForm == null)
            return null;

        //Console.WriteLine($"Inspecting properties on parent form ({parentForm.Name})...");
        //foreach (var prop in parentForm.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
        //{
        //   // Console.WriteLine($"Property: {prop.Name}, Type: {prop.PropertyType}, Value: {prop.GetValue(parentForm)}");
        //}

        return parentForm;
    }


    private bool IsUnitOfWorkType(Type type)
    {
       // Console.WriteLine($"Checking type: {type?.Name}");
        return type != null &&
               (type == typeof(IUnitofWork) ||
                type == typeof(IUnitOfWorkWrapper) ||
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IUnitofWork<>)) ||
                type.GetInterfaces().Any(i => i == typeof(IUnitofWork) ||
                                              i == typeof(IUnitOfWorkWrapper) ||
                                              (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IUnitofWork<>))));
    }
}

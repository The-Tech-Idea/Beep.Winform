using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Design.Helper;


namespace TheTechIdea.Beep.Winform.Controls.Design.Converters;
public class UnitOfWorkClassesConverter : TypeConverter
{
    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;
    public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
        if (context?.Instance == null || context.GetService(typeof(IServiceProvider)) is not IServiceProvider serviceProvider)
            return new StandardValuesCollection(Array.Empty<Type>());

        // Get UnitOfWork types using the helper method
        var unitOfWorkTypes = DesignTimeHelper.GetUnitOfWorkEncapsulatingTypesDesignTime(serviceProvider);
        // Debugging: Check detected components
        foreach (var container in unitOfWorkTypes)
        {
            Console.WriteLine($"Detected Container: {container.GetType().FullName}");
        }
        return new StandardValuesCollection(unitOfWorkTypes);
    }
}

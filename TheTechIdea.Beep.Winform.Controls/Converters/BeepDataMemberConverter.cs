using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using TheTechIdea.Beep.Vis.Modules;
namespace TheTechIdea.Beep.Winform.Controls.Converters;
public class DataMember
{
    public string ClassName { get; set; }
    public string TypeLongName { get; set; }
    public string Type { get; set; }
}
public class BeepDataMemberConverter : TypeConverter
{
  
    private List<DataMember> dataMembers = new List<DataMember>();
    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    {
        // Indicates that we provide a list of standard values (a dropdown in the property grid).
        return true;
    }

    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
    {
        // If true, the user can ONLY select from the dropdown.
        // If false, the user can also type any string they want.
        return false;
    }

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
        // If there's no control or instance, return an empty list.
        if (context?.Instance == null)
            return new StandardValuesCollection(new List<string>());

        // Cast to our interface to retrieve the "BeepViewModelClass" property.
        if (context.Instance is IHasBeepViewModelClass hasVmClass)
        {
            // The user previously selected a Type name in their control (e.g., "MyNamespace.SomeViewModel").
            string typeName = hasVmClass.BeepViewModelClass;
            if (!string.IsNullOrEmpty(typeName))
            {
                // Attempt to load the actual Type from the string name.
                Type vmType = Type.GetType(typeName);
                if (vmType != null)
                {
                    // Gather whatever "members" you want as valid "DataMember" names.
                    // For example, public instance property names:
                    var propertyNames = vmType
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Select(p => p.Name)
                        .OrderBy(n => n)
                        .ToList();

                    return new StandardValuesCollection(propertyNames);
                }
            }
        }

        // Default case: return empty
        return new StandardValuesCollection(new List<string>());
    }
}

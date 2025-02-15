using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using TheTechIdea.Beep.Addin;

namespace  TheTechIdea.Beep.Winform.Controls.Converters;

    public class BeepViewModelClassNameConverter : TypeConverter
{
    // Cache so we don't rebuild the dictionary every time
    // Key   = short name
    // Value = fully qualified name
    private static Dictionary<string, string> _shortToFull;

    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        => true;

    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        => false;
    // If true, user can ONLY pick from the drop-down.
    // If false, user may also type a custom string.

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
        EnsureCache();
        // The drop-down list will display the dictionary's *keys* (short names).
        var shortNames = _shortToFull.Keys.OrderBy(k => k).ToList();
        return new StandardValuesCollection(shortNames);
    }

    public override object ConvertTo(ITypeDescriptorContext context,
                                     CultureInfo culture,
                                     object value,
                                     Type destinationType)
    {
        // The property grid calls this to display the current property value.
        // 'value' is the *stored* string (the *full* name).
        if (destinationType == typeof(string) && value is string fullName)
        {
            EnsureCache();
            // If we have the full name in our dictionary,
            // return the corresponding short name. Otherwise, return as-is.
            var pair = _shortToFull.FirstOrDefault(kv => kv.Value == fullName);
            if (!string.IsNullOrEmpty(pair.Key))
            {
                return pair.Key; // short name
            }
            else
            {
                // Could happen if user typed a custom string not in the dictionary
                // or if the type was removed from the AppDomain.
                // We'll just display the raw stored string.
                return fullName;
            }
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context,
                                       CultureInfo culture,
                                       object value)
    {
        // The user picks a short name from the drop-down (or types something).
        // We store the FULL name in the property.
        if (value is string shortName)
        {
            EnsureCache();
            if (_shortToFull.TryGetValue(shortName, out string fullName))
            {
                return fullName;
            }
            else
            {
                // If it's not in our dictionary, user typed a custom string.
                // We can store that raw string or treat it as a full name. 
                // For example, you might do:
                return shortName;
            }
        }
        return base.ConvertFrom(context, culture, value);
    }

    private static void EnsureCache()
    {
        if (_shortToFull != null)
            return;

        _shortToFull = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // 1) Gather all classes implementing IBeepViewModel
        var beepVmTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(SafeGetTypes)
            .Where(t => typeof(IBeepViewModel).IsAssignableFrom(t)
                        && !t.IsAbstract
                        && !t.IsInterface)
            .ToList();

        // 2) Populate the dictionary:
        // Key = short name (t.Name)
        // Value = fully qualified name (t.FullName or AssemblyQualifiedName)
        // 
        // NB: If multiple classes share the same short name, the "last one wins."
        // This is a limitation of using short names; collisions can occur.
        // If that's a concern, consider storing: shortName + " (" + t.Namespace + ")"
        // or using AssemblyQualifiedName as the dictionary Value to avoid collisions.
        foreach (var t in beepVmTypes)
        {
            string shortName = t.Name;
            string fullName = t.FullName; // or t.AssemblyQualifiedName if you prefer

            _shortToFull[shortName] = fullName;
        }
    }

    private static Type[] SafeGetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch
        {
            // Some assemblies can fail to load certain types
            return Array.Empty<Type>();
        }
    }
}

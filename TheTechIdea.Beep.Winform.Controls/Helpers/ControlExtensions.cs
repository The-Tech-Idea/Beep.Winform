using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    public static class ControlExtensions
    {
        public static void DoubleBuffered(this Control control, bool enable)
        {
            var doubleBufferProperty = control.GetType().GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (doubleBufferProperty != null)
            {
                doubleBufferProperty.SetValue(control, enable, null);
            }
        }
        public static string GetFormattedText(string text, string maskFormat)
        {
            if (string.IsNullOrEmpty(maskFormat))
                return text;

            // Number formatting
            if (decimal.TryParse(text, out var number))
            {
                switch (maskFormat.ToLower())
                {
                    case "currency":
                        return number.ToString("C", CultureInfo.CurrentCulture); // Currency format
                    case "percentage":
                        return number.ToString("P", CultureInfo.CurrentCulture); // Percentage format
                    case "fixedpoint":
                        return number.ToString("F2", CultureInfo.CurrentCulture); // Fixed-point format with 2 decimals
                    case "scientific":
                        return number.ToString("E", CultureInfo.CurrentCulture); // Scientific notation
                    case "number":
                        return number.ToString("N", CultureInfo.CurrentCulture); // Number with thousand separators
                    case "hexadecimal":
                        return ((int)number).ToString("X"); // Hexadecimal format (integers only)
                }
            }

            // Date formatting
            if (DateTime.TryParse(text, out var date))
            {
                switch (maskFormat.ToLower())
                {
                    case "shortdate":
                        return date.ToString("d", CultureInfo.CurrentCulture); // Short date
                    case "longdate":
                        return date.ToString("D", CultureInfo.CurrentCulture); // Long date
                    case "shorttime":
                        return date.ToString("t", CultureInfo.CurrentCulture); // Short time
                    case "longtime":
                        return date.ToString("T", CultureInfo.CurrentCulture); // Long time
                    case "monthday":
                        return date.ToString("M", CultureInfo.CurrentCulture); // Month day format
                    case "yearmonth":
                        return date.ToString("Y", CultureInfo.CurrentCulture); // Year month format
                    case "rfc1123":
                        return date.ToString("R", CultureInfo.InvariantCulture); // RFC 1123 format
                    case "sortable":
                        return date.ToString("s", CultureInfo.InvariantCulture); // Sortable date/time format
                    case "iso":
                        return date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture); // ISO 8601 format
                }
            }

            // Custom formatting (if users want to specify their own format pattern)
            try
            {
                // Assume the user-provided maskFormat is a valid .NET format string
                return string.Format(CultureInfo.CurrentCulture, "{0:" + maskFormat + "}", text);
            }
            catch
            {
                // Return unformatted text if the format string is invalid
                return text;
            }
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
                    var service = (INestedContainer)component1.Site.GetService(
                        typeof(INestedContainer));
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
    }
}

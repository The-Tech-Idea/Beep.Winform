using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.ComponentModel.TypeConverter;

namespace TheTechIdea.Beep.Winform.Controls.Converters
{
    // Made public so it can be resolved cross-assembly by the designer
    public class TypeSelectorConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => false;
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            try
            {
                var formTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic)
                    .SelectMany(a =>
                    {
                        try { return a.GetExportedTypes(); } catch { return Array.Empty<Type>(); }
                    })
                    .Where(t => typeof(Form).IsAssignableFrom(t) && !t.IsAbstract)
                    .OrderBy(t => t.FullName)
                    .ToArray();

                return new StandardValuesCollection(formTypes);
            }
            catch { return new StandardValuesCollection(Array.Empty<Type>()); }
        }
    }

}

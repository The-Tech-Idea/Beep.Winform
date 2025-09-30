using System;
using System.ComponentModel;
using System.Globalization;
using System.Collections;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Caption.Design
{
    public class CaptionRendererKindConverter : EnumConverter
    {
        private static readonly (CaptionRendererKind Kind, string Display)[] _map = new[]
        {
            (CaptionRendererKind.Windows, "Windows (Default)"),
            (CaptionRendererKind.MacLike, "macOS-like"),
            (CaptionRendererKind.Gnome,   "GNOME / Adwaita"),
            (CaptionRendererKind.Kde,     "KDE / Breeze"),
            (CaptionRendererKind.Cinnamon,"Cinnamon"),
            (CaptionRendererKind.Elementary,"Elementary")
        };

        public CaptionRendererKindConverter() : base(typeof(CaptionRendererKind)) { }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Array.ConvertAll(_map, m => m.Kind));
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is CaptionRendererKind kind)
            {
                foreach (var (k, d) in _map) if (k.Equals(kind)) return d;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                foreach (var (k, d) in _map)
                {
                    if (string.Equals(d, s, StringComparison.CurrentCultureIgnoreCase))
                        return k;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}

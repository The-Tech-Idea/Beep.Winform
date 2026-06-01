using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace TheTechIdea.Beep.Vis.Modules
{
    /// <summary>
    /// Portable color representation — a platform-agnostic replacement for System.Drawing.Color.
    /// Provides implicit conversion to/from System.Drawing.Color for WinForms backward compatibility.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(BeepColorConverter))]
    public struct BeepColor : IEquatable<BeepColor>
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }

        public BeepColor(byte r, byte g, byte b, byte a = 255)
        {
            R = r; G = g; B = b; A = a;
        }

        // ── Named colors ────────────────────────────────────────────────
        public static readonly BeepColor Empty     = new BeepColor(0, 0, 0, 0);
        public static readonly BeepColor Red       = new BeepColor(255, 0, 0);
        public static readonly BeepColor White     = new BeepColor(255, 255, 255);
        public static readonly BeepColor Black     = new BeepColor(0, 0, 0);
        public static readonly BeepColor Transparent = new BeepColor(0, 0, 0, 0);

        // ── Parsing ─────────────────────────────────────────────────────
        public static BeepColor Parse(string hex)
        {
            if (string.IsNullOrEmpty(hex)) return Empty;
            hex = hex.TrimStart('#');
            if (hex.Length == 6)
                return new BeepColor(
                    byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber),
                    byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
                    byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber));
            if (hex.Length == 8)
                return new BeepColor(
                    byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
                    byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber),
                    byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber),
                    byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber));
            return Empty;
        }

        public string ToHex() => A == 255
            ? $"#{R:X2}{G:X2}{B:X2}"
            : $"#{A:X2}{R:X2}{G:X2}{B:X2}";

        // ── WinForms bridge — implicit conversion ───────────────────────
        public static implicit operator Color(BeepColor c)
            => Color.FromArgb(c.A, c.R, c.G, c.B);

        public static implicit operator BeepColor(Color c)
            => new BeepColor(c.R, c.G, c.B, c.A);

        // ── Equality ────────────────────────────────────────────────────
        public bool Equals(BeepColor other) => R == other.R && G == other.G && B == other.B && A == other.A;

        public override bool Equals(object obj) => obj is BeepColor other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(R, G, B, A);

        public override string ToString() => ToHex();
    }

    /// <summary>
    /// TypeConverter that allows BeepColor to be serialized/deserialized by the WinForms designer
    /// and converted from string hex values.
    /// </summary>
    public class BeepColorConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
                return BeepColor.Parse(s);
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is BeepColor c)
                return c.ToHex();
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

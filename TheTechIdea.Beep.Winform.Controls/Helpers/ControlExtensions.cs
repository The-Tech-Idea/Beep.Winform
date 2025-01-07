using System.Drawing.Drawing2D;
using System.Globalization;
using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.Winform.Controls.Helpers;
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

        // Special thanks to ReaLTaiizor for this code snippet
        public static void DrawRoundedRectangle(Graphics G, Pen P, float X, float Y, float W, float H, float Rad)
        {
            using GraphicsPath GP = new();
            GP.AddLine(X + Rad, Y, X + W - (Rad * 2), Y);
            GP.AddArc(X + W - (Rad * 2), Y, Rad * 2, Rad * 2, 270, 90);
            GP.AddLine(X + W, Y + Rad, X + W, Y + H - (Rad * 2));
            GP.AddArc(X + W - (Rad * 2), Y + H - (Rad * 2), Rad * 2, Rad * 2, 0, 90);
            GP.AddLine(X + W - (Rad * 2), Y + H, X + Rad, Y + H);
            GP.AddArc(X, Y + H - (Rad * 2), Rad * 2, Rad * 2, 90, 90);
            GP.AddLine(X, Y + H - (Rad * 2), X, Y + Rad);
            GP.AddArc(X, Y, Rad * 2, Rad * 2, 180, 90);
            GP.CloseFigure();

            G.SmoothingMode = SmoothingMode.AntiAlias;
            G.DrawPath(P, GP);
            G.SmoothingMode = SmoothingMode.Default;

        }
        public static GraphicsPath RoundRect(Rectangle Rectangle, int Curve)
        {
            GraphicsPath GP = new();

            int ArcRectangleWidth = Curve * 2;

            GP.AddArc(new(Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -180, 90);
            GP.AddArc(new(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -90, 90);
            GP.AddArc(new(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), 0, 90);
            GP.AddArc(new(Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), 90, 90);
            GP.AddLine(new Point(Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y), new Point(Rectangle.X, Curve + Rectangle.Y));

            return GP;
        }

        public static GraphicsPath RoundRect(int X, int Y, int Width, int Height, int Curve)
        {
            Rectangle Rectangle = new(X, Y, Width, Height);

            GraphicsPath GP = new();

            int EndArcWidth = Curve * 2;

            GP.AddArc(new(Rectangle.X, Rectangle.Y, EndArcWidth, EndArcWidth), -180, 90);
            GP.AddArc(new(Rectangle.Width - EndArcWidth + Rectangle.X, Rectangle.Y, EndArcWidth, EndArcWidth), -90, 90);
            GP.AddArc(new(Rectangle.Width - EndArcWidth + Rectangle.X, Rectangle.Height - EndArcWidth + Rectangle.Y, EndArcWidth, EndArcWidth), 0, 90);
            GP.AddArc(new(Rectangle.X, Rectangle.Height - EndArcWidth + Rectangle.Y, EndArcWidth, EndArcWidth), 90, 90);
            GP.AddLine(new Point(Rectangle.X, Rectangle.Height - EndArcWidth + Rectangle.Y), new Point(Rectangle.X, Curve + Rectangle.Y));

            return GP;
        }

        public static GraphicsPath RoundedTopRect(Rectangle Rectangle, int Curve)
        {
            GraphicsPath GP = new();

            int ArcRectangleWidth = Curve * 2;

            GP.AddArc(new(Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -180, 90);
            GP.AddArc(new(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -90, 90);
            GP.AddLine(new Point(Rectangle.X + Rectangle.Width, Rectangle.Y + ArcRectangleWidth), new Point(Rectangle.X + Rectangle.Width, Rectangle.Y + Rectangle.Height - 1));
            GP.AddLine(new Point(Rectangle.X, Rectangle.Height - 1 + Rectangle.Y), new Point(Rectangle.X, Rectangle.Y + Curve));

            return GP;
        }

        public static GraphicsPath CreateRoundRect(float X, float Y, float Width, float Height, float Radius)
        {
            GraphicsPath GP = new();
            GP.AddLine(X + Radius, Y, X + Width - (Radius * 2), Y);
            GP.AddArc(X + Width - (Radius * 2), Y, Radius * 2, Radius * 2, 270, 90);

            GP.AddLine(X + Width, Y + Radius, X + Width, Y + Height - (Radius * 2));
            GP.AddArc(X + Width - (Radius * 2), Y + Height - (Radius * 2), Radius * 2, Radius * 2, 0, 90);

            GP.AddLine(X + Width - (Radius * 2), Y + Height, X + Radius, Y + Height);
            GP.AddArc(X, Y + Height - (Radius * 2), Radius * 2, Radius * 2, 90, 90);

            GP.AddLine(X, Y + Height - (Radius * 2), X, Y + Radius);
            GP.AddArc(X, Y, Radius * 2, Radius * 2, 180, 90);

            GP.CloseFigure();

            return GP;
        }

        public static GraphicsPath CreateUpRoundRect(float X, float Y, float Width, float Height, float Radius)
        {
            GraphicsPath GP = new();

            GP.AddLine(X + Radius, Y, X + Width - (Radius * 2), Y);
            GP.AddArc(X + Width - (Radius * 2), Y, Radius * 2, Radius * 2, 270, 90);

            GP.AddLine(X + Width, Y + Radius, X + Width, Y + Height - (Radius * 2) + 1);
            GP.AddArc(X + Width - (Radius * 2), Y + Height - (Radius * 2), Radius * 2, 2, 0, 90);

            GP.AddLine(X + Width, Y + Height, X + Radius, Y + Height);
            GP.AddArc(X, Y + Height - (Radius * 2) + 1, Radius * 2, 1, 90, 90);

            GP.AddLine(X, Y + Height, X, Y + Radius);
            GP.AddArc(X, Y, Radius * 2, Radius * 2, 180, 90);

            GP.CloseFigure();

            return GP;
        }

        public static GraphicsPath CreateLeftRoundRect(float X, float Y, float Width, float Height, float Radius)
        {
            GraphicsPath GP = new();
            GP.AddLine(X + Radius, Y, X + Width - (Radius * 2), Y);
            GP.AddArc(X + Width - (Radius * 2), Y, Radius * 2, Radius * 2, 270, 90);

            GP.AddLine(X + Width, Y + 0, X + Width, Y + Height);
            GP.AddArc(X + Width - (Radius * 2), Y + Height - 1, Radius * 2, 1, 0, 90);

            GP.AddLine(X + Width - (Radius * 2), Y + Height, X + Radius, Y + Height);
            GP.AddArc(X, Y + Height - (Radius * 2), Radius * 2, Radius * 2, 90, 90);

            GP.AddLine(X, Y + Height - (Radius * 2), X, Y + Radius);
            GP.AddArc(X, Y, Radius * 2, Radius * 2, 180, 90);

            GP.CloseFigure();

            return GP;
        }
        // based EntityHelper.GetAllAvailableUIComponents function , create a function the select the default BeepControl based on DBFieldCategory and return Control Type
        public static Type GetDefaultControlType(DbFieldCategory category)
        {
            switch (category)
            {
                case DbFieldCategory.String:
                    return typeof(BeepTextBox);
                case DbFieldCategory.Numeric:
                    return typeof(BeepTextBox);
                case DbFieldCategory.Date:
                    return typeof(BeepDatePicker);
                case DbFieldCategory.Boolean:
                    return typeof(BeepCheckBox);
                case DbFieldCategory.Binary:
                    return typeof(BeepImage);
                case DbFieldCategory.Guid:
                    return typeof(BeepTextBox);
                case DbFieldCategory.Json:
                    return typeof(BeepTextBox);
                case DbFieldCategory.Xml:
                    return typeof(BeepTextBox);
                case DbFieldCategory.Geography:
                    return typeof(BeepTextBox);
                case DbFieldCategory.Currency:
                    return typeof(BeepTextBox);
                case DbFieldCategory.Enum:
                    return typeof(BeepComboBox);
                case DbFieldCategory.Timestamp:
                    return typeof(BeepTextBox);
                case DbFieldCategory.Complex:
                    return typeof(BeepTextBox);
                default:
                    return typeof(BeepTextBox);
            }
        }

    }


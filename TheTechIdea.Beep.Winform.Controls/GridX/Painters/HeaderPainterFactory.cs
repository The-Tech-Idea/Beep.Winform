using System;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Factory for creating header painters based on navigationStyle
    /// Ensures consistent matching between navigation and header styles
    /// </summary>
    public static class HeaderPainterFactory
    {
        /// <summary>
        /// Creates a header painter matching the specified navigation style
        /// </summary>
        /// <param name="style">The navigationStyle to match</param>
        /// <returns>IPaintGridHeader implementation for the style</returns>
        public static IPaintGridHeader CreateHeaderPainter(navigationStyle style)
        {
            switch (style)
            {
                case navigationStyle.Standard:
                    return new StandardHeaderPainter();

                case navigationStyle.Material:
                    return new MaterialHeaderPainter();

                case navigationStyle.Bootstrap:
                    return new BootstrapHeaderPainter();

                case navigationStyle.Tailwind:
                    return new TailwindHeaderPainter();

                case navigationStyle.Fluent:
                    return new FluentHeaderPainter();

                case navigationStyle.AGGrid:
                    return new AGGridHeaderPainter();

                case navigationStyle.DataTables:
                    return new DataTablesHeaderPainter();

                case navigationStyle.AntDesign:
                    return new AntDesignHeaderPainter();

                case navigationStyle.Telerik:
                    return new TelerikHeaderPainter();

                case navigationStyle.Compact:
                    return new CompactHeaderPainter();

                case navigationStyle.Minimal:
                    return new MinimalHeaderPainter();

                case navigationStyle.Card:
                    return new CardHeaderPainter();

                default:
                    return new StandardHeaderPainter();
            }
        }

        /// <summary>
        /// Gets the style name for display purposes
        /// </summary>
        public static string GetStyleName(navigationStyle style)
        {
            var painter = CreateHeaderPainter(style);
            return painter?.StyleName ?? style.ToString();
        }

        /// <summary>
        /// Checks if a header painter implementation exists for the style
        /// </summary>
        public static bool IsStyleImplemented(navigationStyle style)
        {
            switch (style)
            {
                case navigationStyle.Standard:
                case navigationStyle.Material:
                case navigationStyle.Bootstrap:
                case navigationStyle.Tailwind:
                case navigationStyle.Fluent:
                case navigationStyle.AGGrid:
                case navigationStyle.DataTables:
                case navigationStyle.AntDesign:
                case navigationStyle.Telerik:
                case navigationStyle.Compact:
                case navigationStyle.Minimal:
                case navigationStyle.Card:
                    return true;

                default:
                    return false;
            }
        }
    }
}

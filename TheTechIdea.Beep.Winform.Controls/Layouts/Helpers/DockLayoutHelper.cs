using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    /// <summary>
    /// Helper for creating dock layouts with panels docked to Top, Bottom, Left, Right, and Fill.
    /// Creates a layout with five regions using theme-aware styling.
    /// </summary>
    public static class DockLayoutHelper
    {
        /// <summary>
        /// Builds a dock layout with default options.
        /// </summary>
        /// <param name="parent">The parent control container.</param>
        /// <returns>The main Panel containing all docked panels.</returns>
        public static Control Build(Control parent)
        {
            return Build(parent, LayoutOptions.Default);
        }

        /// <summary>
        /// Builds a dock layout with theme-aware styling.
        /// </summary>
        /// <param name="parent">The parent control container.</param>
        /// <param name="options">Layout configuration options for theming and styling.</param>
        /// <returns>The main Panel containing all docked panels.</returns>
        public static Control Build(Control parent, LayoutOptions options)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            options = options ?? LayoutOptions.Default;

            var main = BaseLayoutHelper.CreateStyledPanel(options);
            main.Dock = DockStyle.Fill;
            parent.Controls.Add(main);

            var bgColor = BaseLayoutHelper.GetBackgroundColor(options);
            var borderColor = BaseLayoutHelper.GetBorderColor(options);

            var top = BaseLayoutHelper.CreateStyledPanel(options);
            top.Dock = DockStyle.Top;
            top.Height = 50;
            top.BackColor = bgColor;

            var bottom = BaseLayoutHelper.CreateStyledPanel(options);
            bottom.Dock = DockStyle.Bottom;
            bottom.Height = 50;
            bottom.BackColor = bgColor;

            var left = BaseLayoutHelper.CreateStyledPanel(options);
            left.Dock = DockStyle.Left;
            left.Width = 100;
            left.BackColor = bgColor;

            var right = BaseLayoutHelper.CreateStyledPanel(options);
            right.Dock = DockStyle.Right;
            right.Width = 100;
            right.BackColor = bgColor;

            var fill = BaseLayoutHelper.CreateStyledPanel(options);
            fill.Dock = DockStyle.Fill;
            fill.BackColor = bgColor;

            // Add in reverse order to ensure proper docking
            main.Controls.Add(fill);
            main.Controls.Add(right);
            main.Controls.Add(left);
            main.Controls.Add(bottom);
            main.Controls.Add(top);

            return main;
        }
    }
}

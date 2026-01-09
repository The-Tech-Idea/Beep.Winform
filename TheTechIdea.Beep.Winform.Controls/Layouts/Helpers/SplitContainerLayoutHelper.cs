using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    /// <summary>
    /// Helper for creating split container layouts.
    /// Creates a SplitContainer with two panels using theme-aware styling.
    /// </summary>
    public static class SplitContainerLayoutHelper
    {
        /// <summary>
        /// Builds a split container layout with default options.
        /// </summary>
        /// <param name="parent">The parent control container.</param>
        /// <param name="orientation">The orientation of the split (Vertical or Horizontal).</param>
        /// <returns>The SplitContainer with two styled panels.</returns>
        public static Control Build(Control parent, Orientation orientation)
        {
            return Build(parent, orientation, LayoutOptions.Default);
        }

        /// <summary>
        /// Builds a split container layout with theme-aware styling.
        /// </summary>
        /// <param name="parent">The parent control container.</param>
        /// <param name="orientation">The orientation of the split (Vertical or Horizontal).</param>
        /// <param name="options">Layout configuration options for theming and styling.</param>
        /// <returns>The SplitContainer with two styled panels.</returns>
        public static Control Build(Control parent, Orientation orientation, LayoutOptions options)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            options = options ?? LayoutOptions.Default;

            var split = new SplitContainer 
            { 
                Orientation = orientation, 
                Dock = DockStyle.Fill,
                BackColor = BaseLayoutHelper.GetBackgroundColor(options)
            };

            var p1 = BaseLayoutHelper.CreateStyledPanel(options);
            p1.Dock = DockStyle.Fill;

            var p2 = BaseLayoutHelper.CreateStyledPanel(options);
            p2.Dock = DockStyle.Fill;

            split.Panel1.Controls.Add(p1);
            split.Panel2.Controls.Add(p2);
            parent.Controls.Add(split);

            return split;
        }
    }
}

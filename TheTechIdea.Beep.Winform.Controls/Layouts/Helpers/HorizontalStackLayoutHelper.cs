using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    /// <summary>
    /// Helper for creating horizontal stack layouts using FlowLayoutPanel.
    /// Creates a left-to-right flow layout with Beep-styled buttons.
    /// </summary>
    public static class HorizontalStackLayoutHelper
    {
        /// <summary>
        /// Builds a horizontal stack layout with default options.
        /// </summary>
        /// <param name="parent">The parent control container.</param>
        /// <returns>The FlowLayoutPanel containing the horizontal stack.</returns>
        public static Control Build(Control parent)
        {
            return Build(parent, LayoutOptions.Default);
        }

        /// <summary>
        /// Builds a horizontal stack layout with theme-aware styling.
        /// </summary>
        /// <param name="parent">The parent control container.</param>
        /// <param name="options">Layout configuration options for theming and styling.</param>
        /// <returns>The FlowLayoutPanel containing the horizontal stack.</returns>
        public static Control Build(Control parent, LayoutOptions options)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            options = options ?? LayoutOptions.Default;

            var flow = new FlowLayoutPanel 
            { 
                FlowDirection = FlowDirection.LeftToRight, 
                WrapContents = true, 
                Dock = DockStyle.Fill, 
                AutoScroll = true,
                BackColor = BaseLayoutHelper.GetBackgroundColor(options)
            };

            for (int i = 1; i <= 3; i++)
            {
                var btn = BaseLayoutHelper.CreateBeepButton($"Button {i}", options);
                btn.AutoSize = true;
                BaseLayoutHelper.ApplySpacing(btn, options);
                flow.Controls.Add(btn);
            }

            parent.Controls.Add(flow);
            return flow;
        }
    }
}

using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Painters
{
    /// <summary>
    /// Text only: no fill, no border, no rule, no accent. Selection is carried entirely by weight
    /// and contrast — the selected label is full-strength while unselected labels are dimmed.
    /// </summary>
    /// <remarks>
    /// Typography is the only signal this style has, so it has to do real work. Previously this
    /// painter drew nothing at all and relied on the control's accent bar, which
    /// <see cref="UnderlineTabPainter"/> also drew — making the two styles pixel-identical. With the
    /// accent removed it became "Underline minus one bar" rather than a style in its own right.
    /// </remarks>
    public class MinimalTabPainter : BaseTabPainter
    {
        /// <summary>How much of full strength an unselected label is drawn at.</summary>
        private const float UnselectedTextStrength = 0.45f;

        public MinimalTabPainter(BeepTabs tabControl) : base(tabControl) { }

        protected override void DrawTabItemContent(Graphics g, BeepTabHeaderItemLayout itemLayout,
                                                   float alpha, Color? overrideTextColor = null)
        {
            if (overrideTextColor.HasValue || itemLayout.Item.IsSelected || itemLayout.Item.IsHovered)
            {
                base.DrawTabItemContent(g, itemLayout, alpha, overrideTextColor);
                return;
            }

            // Dim unselected labels toward the strip so the selected one reads as selected without
            // any chrome. Blended rather than alpha-faded so the result stays a solid, legible
            // colour instead of a washed-out one.
            Color text = TabThemeHelpers.GetTabTextColor(Theme, false, false);
            Color strip = TabThemeHelpers.GetHeaderBackgroundColor(Theme);
            Color dimmed = Blend(text, strip, UnselectedTextStrength);

            base.DrawTabItemContent(g, itemLayout, alpha, ColorUtils.EnsureReadable(dimmed, strip));
        }

        private static Color Blend(Color fore, Color back, float foreWeight)
        {
            float w = Math.Clamp(foreWeight, 0f, 1f);
            return Color.FromArgb(
                fore.A,
                (int)(fore.R * w + back.R * (1 - w)),
                (int)(fore.G * w + back.G * (1 - w)),
                (int)(fore.B * w + back.B * (1 - w)));
        }

        /// <summary>This painter draws no tab fill, so the text sits on the header background.</summary>
        protected override Color GetTabSurfaceColor(BeepTabItem item)
            => TabThemeHelpers.GetHeaderBackgroundColor(Theme);
    }
}

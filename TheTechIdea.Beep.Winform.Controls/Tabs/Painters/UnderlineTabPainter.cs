using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Painters
{
    /// <summary>
    /// The Material tab bar: a rule runs the full width of the strip, the selected tab carries a
    /// thick accent bar sitting on that rule, and the selected label takes the accent colour.
    /// </summary>
    /// <remarks>
    /// The full-width rule is what separates this from <see cref="MinimalTabPainter"/> across the
    /// whole strip rather than only under one tab. With the accent bar alone the two styles differed
    /// by 0.7% of pixels — visible if you looked for it, but not a different style at a glance.
    /// </remarks>
    public class UnderlineTabPainter : BaseTabPainter
    {
        // Design-time pixels; scaled per display via BaseTabPainter.Scale.
        private const int RuleThickness = 2;

        public UnderlineTabPainter(BeepTabs tabControl) : base(tabControl) { }

        public override void PaintHeaderBackground(Graphics g, Rectangle headerBounds)
        {
            base.PaintHeaderBackground(g, headerBounds);
            if (headerBounds.Width <= 0 || headerBounds.Height <= 0) return;

            // The rule the accent bar rides on, across the entire strip.
            Color ruleColor = TabThemeHelpers.GetTabBorderColor(Theme, false, false);
            var brush = PaintersFactory.GetSolidBrush(ruleColor);
            int ruleHeight = Scale(RuleThickness);
            g.FillRectangle(brush,
                headerBounds.Left, headerBounds.Bottom - ruleHeight,
                headerBounds.Width, ruleHeight);
        }

        /// <summary>The accent bar beneath the selected tab — thicker than the rule it sits on.</summary>
        public override void PaintSelectionAccent(Graphics g, RectangleF accentBounds, float alpha = 1.0f)
        {
            if (accentBounds == RectangleF.Empty || alpha <= 0f) return;

            Color accent = TabThemeHelpers.GetTabIndicatorColor(Theme);
            Color faded = Color.FromArgb((int)(Math.Clamp(alpha, 0f, 1f) * 255f), accent);

            // Full tab width and heavier than the underlying rule, so the selection reads instantly.
            float pad = Scale(6);
            var bar = new RectangleF(
                accentBounds.X - pad,
                accentBounds.Y - Scale(1),
                accentBounds.Width + pad * 2f,
                Math.Max(accentBounds.Height, Scale(4)));

            var brush = PaintersFactory.GetSolidBrush(faded);
            g.FillRectangle(brush, bar);
        }

        /// <summary>
        /// The selected label takes the accent colour rather than the on-fill colour: there is no
        /// fill behind it, so it sits on the header background.
        /// </summary>
        protected override void DrawTabItemContent(Graphics g, BeepTabHeaderItemLayout itemLayout,
                                                   float alpha, Color? overrideTextColor = null)
        {
            Color? accent = itemLayout.Item.IsSelected
                ? TabThemeHelpers.GetTabIndicatorColor(Theme)
                : (Color?)null;

            base.DrawTabItemContent(g, itemLayout, alpha, overrideTextColor ?? accent);
        }

        /// <summary>This painter draws no tab fill, so the text sits on the header background.</summary>
        protected override Color GetTabSurfaceColor(BeepTabItem item)
            => TabThemeHelpers.GetHeaderBackgroundColor(Theme);
    }
}

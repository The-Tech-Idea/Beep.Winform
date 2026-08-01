using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Painters
{
    public interface ITabPainter
    {
        BeepTabs TabControl { get; set; }
        IBeepTheme Theme { get; set; }

        void PaintHeaderBackground(Graphics g, Rectangle headerBounds);

        /// <summary>
        /// Measures a tab.
        /// <para>
        /// Whatever font and metrics this reports must be the ones <see cref="PaintTabItem"/> draws with. Measuring with one font and drawing with another is
        /// the defect that clipped every label in BeepTree's painters and twice in the tooltip
        /// painters; resolve fonts through <c>TabFontHelpers</c> on both sides.
        /// </para>
        /// </summary>
        SizeF MeasureTab(Graphics g, int index, Font font);

        /// <summary>
        /// Draws the style's selection accent — the sliding bar an underline-style tab strip shows
        /// beneath the selected tab. Called once per paint, after every tab, and **outside** the
        /// per-tab clip, because the accent animates between tabs and would otherwise be clipped
        /// away mid-slide.
        /// </summary>
        /// <param name="accentBounds">
        /// The animated accent rectangle, already interpolated toward the selected tab.
        /// <see cref="RectangleF.Empty"/> until the first selection change.
        /// </param>
        /// <remarks>
        /// This exists so a style's accent belongs to that style's painter. It used to be a
        /// <c>_tabStyle == TabStyle.Underline || _tabStyle == TabStyle.Minimal</c> branch inside
        /// <c>BeepTabs.Animation</c> — a switch on style sitting outside the painters, which is
        /// precisely what painters exist to avoid. It also meant Minimal drew Underline's accent,
        /// making the two styles pixel-identical.
        /// </remarks>
        void PaintSelectionAccent(Graphics g, RectangleF accentBounds, float alpha = 1.0f);

        RectangleF GetCloseButtonRect(RectangleF tabRect, bool vertical);

        /// <summary>
        /// Paints one tab from its fully-resolved layout. **This is the entry point the header host
        /// calls**; it receives the adornment bounds (icon, subtext, badge, dirty marker, busy
        /// indicator) already calculated.
        /// <para>
        /// The default implementation in <see cref="BaseTabPainter"/> draws the shared content
        /// pass and no chrome. Override it to render a style's own shape, then call through to that
        /// shared pass so icon, title, subtext, badge and close button stay consistent.
        /// </para>
        /// </summary>
        void PaintTabItem(Graphics g, BeepTabHeaderItemLayout itemLayout, float alpha = 1.0f);
    }
}

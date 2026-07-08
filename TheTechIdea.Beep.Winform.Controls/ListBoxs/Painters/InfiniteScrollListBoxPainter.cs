using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Infinite-scroll list painter.
    /// Renders items exactly like StandardListBoxPainter, but appends a
    /// "Load more…" sentinel row at the bottom of the content.
    /// Clicking the sentinel raises BeepListBox.LoadMoreRequested.
    /// </summary>
    internal class InfiniteScrollListBoxPainter : BaseListBoxPainter
    {
        private const string SentinelText = "Load more…";
        private Rectangle _sentinelRect;

        public Rectangle SentinelRect => _sentinelRect;

        public override int GetPreferredItemHeight()
            => DpiScalingHelper.ScaleValue(ListBoxTokens.ItemHeightComfortable, _owner ?? new Control());

        protected override void DrawItemBackgroundEx(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            // Keep base row-state visuals while this painter only customizes the sentinel row.
            base.DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);
        }

        public override void Paint(Graphics g, BeepListBox owner, Rectangle drawingRect)
        {
            base.Paint(g, owner, drawingRect);

            if (drawingRect.Width <= 0 || drawingRect.Height <= 0) return;

            int rowH = DpiScalingHelper.ScaleValue(ListBoxTokens.ItemHeightCompact, owner);
            _sentinelRect = new Rectangle(drawingRect.Left, drawingRect.Bottom - rowH, drawingRect.Width, rowH);

            g.DrawLine(GetPen(Color.FromArgb(40, _theme?.PrimaryColor ?? Color.Gray), 1f), _sentinelRect.Left, _sentinelRect.Top, _sentinelRect.Right, _sentinelRect.Top);

            var mp = owner.PointToClient(System.Windows.Forms.Control.MousePosition);
            bool hovered = _sentinelRect.Contains(mp);
            if (hovered)
            {
                g.FillRectangle(GetBrush(Color.FromArgb(ListBoxTokens.HoverOverlayAlpha,
                    _theme?.PrimaryColor ?? Color.DodgerBlue)), _sentinelRect);
            }

            string text = SentinelText;
            var font = GetCachedFont(owner.Font.Size, FontStyle.Regular);
            Color textColor = hovered
                ? (_theme?.PrimaryColor ?? Color.DodgerBlue)
                : Color.FromArgb(ListBoxTokens.SubTextAlpha, _theme?.ListForeColor ?? Color.Gray);
            System.Windows.Forms.TextRenderer.DrawText(g, text, font, _sentinelRect, textColor,
                System.Windows.Forms.TextFormatFlags.HorizontalCenter | System.Windows.Forms.TextFormatFlags.VerticalCenter | System.Windows.Forms.TextFormatFlags.NoPrefix);
        }

        public override bool SupportsSearch() => false;
    }
}

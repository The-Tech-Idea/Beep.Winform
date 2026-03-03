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

        public override int GetPreferredItemHeight()
            => DpiScalingHelper.ScaleValue(ListBoxTokens.ItemHeightComfortable, _owner ?? new Control());

        public override void Paint(Graphics g, BeepListBox owner, Rectangle drawingRect)
        {
            base.Paint(g, owner, drawingRect);

            // Draw sentinel row at bottom of visible content
            if (drawingRect.Width <= 0 || drawingRect.Height <= 0) return;

            int rowH = DpiScalingHelper.ScaleValue(ListBoxTokens.ItemHeightCompact, owner);
            var sentinelRect = new Rectangle(drawingRect.Left, drawingRect.Bottom - rowH, drawingRect.Width, rowH);

            // Separator line above sentinel
            using var pen = new Pen(Color.FromArgb(40, _theme?.PrimaryColor ?? Color.Gray), 1f);
            g.DrawLine(pen, sentinelRect.Left, sentinelRect.Top, sentinelRect.Right, sentinelRect.Top);

            // Sentinel background on hover
            var mp = owner.PointToClient(System.Windows.Forms.Control.MousePosition);
            bool hovered = sentinelRect.Contains(mp);
            if (hovered)
            {
                using var hb = new SolidBrush(Color.FromArgb(ListBoxTokens.HoverOverlayAlpha,
                    _theme?.PrimaryColor ?? Color.DodgerBlue));
                g.FillRectangle(hb, sentinelRect);
            }

            // Text
            string text = SentinelText;
            using var font  = new Font(owner.Font.FontFamily, owner.Font.Size, FontStyle.Regular);
            using var brush = new SolidBrush(hovered
                ? (_theme?.PrimaryColor ?? Color.DodgerBlue)
                : Color.FromArgb(ListBoxTokens.SubTextAlpha, _theme?.ListForeColor ?? Color.Gray));
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(text, font, brush, sentinelRect, sf);

            // Register sentinel hit-area via tag (checked in click handler)
            owner.Tag = sentinelRect;   // simple coupling — checked in OnMouseClick
        }

        public override bool SupportsSearch() => false;
    }
}

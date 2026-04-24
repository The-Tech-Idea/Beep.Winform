using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Multi-select chip field — synced with <see cref="Popup.ChipHeaderPopupContent"/>.
    /// Popup shows: selected-chips area at top, dashed separator, checkbox list below.
    /// Field echoes: inline chips via ChipPainter, dashed separator line below chips,
    /// count badge matching popup chip accent, focus ring using FocusBorderColor.
    /// </summary>
    internal class MultiChipCompactComboBoxPainter : DesignSystemComboBoxFieldPainterBase
    {
        protected override int CornerRadius => 8;
        protected override bool ShowButtonSeparator => false;

        public override int GetPreferredButtonWidth() => 36;
        public override Padding GetPreferredPadding() => new Padding(10, 6, 8, 6);

        protected override void DrawTextArea(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;
            var istate = GetInteractionState();

            // Light tinted background for chip container area
            Color tint = Color.Empty;
            if (istate == ComboBoxInteractionState.Focused || istate == ComboBoxInteractionState.Open)
                tint = Color.FromArgb(10, _theme?.PrimaryColor ?? Color.FromArgb(25, 118, 210));
            else if (istate == ComboBoxInteractionState.Hover)
                tint = Color.FromArgb(6, _theme?.ForeColor ?? Color.Black);

            if (tint != Color.Empty)
            {
                using var brush = new SolidBrush(tint);
                g.FillRectangle(brush, textAreaRect);
            }
        }

        protected override void DrawDecorations(Graphics g, Rectangle drawingRect)
        {
            if (drawingRect.IsEmpty) return;
            var istate = GetInteractionState();

            // Focus ring — FocusBorderColor matching popup chip selected border
            if (istate == ComboBoxInteractionState.Focused || istate == ComboBoxInteractionState.Open)
            {
                Color focusColor = _theme?.PrimaryColor ?? Color.FromArgb(25, 118, 210);
                int radius = ScaleX(CornerRadius);
                using var path = GetRoundedRectPath(Rectangle.Inflate(drawingRect, -1, -1), radius);
                using var pen = new Pen(Color.FromArgb(90, focusColor), ScaleX(2));
                g.DrawPath(pen, path);
            }

            // Dashed separator below chips — mirrors popup DashedSeparatorPanel
            bool hasChips = _owner.AllowMultipleSelection && _owner.SelectedItems != null && _owner.SelectedItems.Count > 0;
            if (hasChips)
            {
                Color sepColor = Color.FromArgb(80, _theme?.BorderColor ?? Color.Gray);
                int actualBtnW = _owner.GetDropdownButtonRect().Width;
                int sepY = drawingRect.Bottom - ScaleY(4);
                using var dashPen = new Pen(sepColor, 1f) { DashStyle = DashStyle.Dash, DashPattern = new[] { 4f, 3f } };
                g.DrawLine(dashPen, drawingRect.Left + ScaleX(6), sepY, drawingRect.Right - actualBtnW - ScaleX(4), sepY);
            }

            // Selected-count badge — accent pill matching popup PopupChip accent colors
            if (hasChips)
            {
                int count = _owner.SelectedItems.Count;
                string badge = count.ToString();
                Font badgeFont = new Font(SystemFonts.DefaultFont.FontFamily, ScaleX(8), FontStyle.Bold);
                var badgeSize = TextRenderer.MeasureText(badge, badgeFont);
                int badgeW = Math.Max(badgeSize.Width + ScaleX(6), ScaleX(18));
                int badgeH = ScaleY(16);
                int actualBtnW = _owner.GetDropdownButtonRect().Width;
                int badgeX = drawingRect.Right - actualBtnW - badgeW - ScaleX(4);
                int badgeY = drawingRect.Top + (drawingRect.Height - badgeH) / 2;

                var badgeRect = new Rectangle(badgeX, badgeY, badgeW, badgeH);
                Color accentColor = _theme?.PrimaryColor ?? Color.FromArgb(25, 118, 210);

                using var badgePath = GetRoundedRectPath(badgeRect, badgeH / 2);
                using var badgeBrush = new SolidBrush(Color.FromArgb(200, accentColor));
                g.FillPath(badgeBrush, badgePath);

                Color badgeFg = _theme?.OnPrimaryColor ?? Color.White;
                TextRenderer.DrawText(g, badge, badgeFont, badgeRect, badgeFg,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                badgeFont.Dispose();
            }
        }
    }
}

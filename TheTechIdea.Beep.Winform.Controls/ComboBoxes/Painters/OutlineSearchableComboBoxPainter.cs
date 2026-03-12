using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Outlined dropdown with search-first popup — synced with <see cref="Popup.ComboBoxPopupContent"/>.
    /// Popup uses PopupRowHoverColor+HoverBorderColor, search box accent in PrimaryColor.
    /// Field echoes: search icon in PrimaryColor, FocusBorderColor focus ring, hover border accent.
    /// </summary>
    internal sealed class OutlineSearchableComboBoxPainter : DesignSystemComboBoxFieldPainterBase
    {
        protected override int CornerRadius => 6;
        protected override bool IsSearchIconButton => true;
        protected override bool ShowButtonSeparator => true;

        public override int GetPreferredButtonWidth() => 36;
        public override Padding GetPreferredPadding() => new Padding(12, 6, 8, 6);

        protected override void DrawTextArea(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;
            var istate = GetInteractionState();

            if (istate == ComboBoxInteractionState.Focused || istate == ComboBoxInteractionState.Open)
            {
                Color primary = _theme?.ButtonOutlineBorderColor ?? _theme?.PrimaryColor ?? Color.FromArgb(25, 118, 210);
                using var brush = new SolidBrush(Color.FromArgb(10, primary));
                g.FillRectangle(brush, textAreaRect);
            }
            else if (istate == ComboBoxInteractionState.Hover)
            {
                using var brush = new SolidBrush(Color.FromArgb(8, _theme?.ForeColor ?? Color.Black));
                g.FillRectangle(brush, textAreaRect);
            }
        }

        protected override void DrawText(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;
            var istate = GetInteractionState();

            // Draw a small leading search icon — uses PrimaryColor matching popup search box accent
            int iconInset = 0;
            if (istate == ComboBoxInteractionState.Focused || istate == ComboBoxInteractionState.Open)
            {
                int iconSize = ScaleX(14);
                int iconX = textAreaRect.X + ScaleX(4);
                int iconY = textAreaRect.Y + (textAreaRect.Height - iconSize) / 2;
                var iconRect = new Rectangle(iconX, iconY, iconSize, iconSize);
                Color iconColor = Color.FromArgb(110, _theme?.PrimaryColor ?? Color.FromArgb(25, 118, 210));
                DrawSvgIcon(g, iconRect, SvgsUI.Search, iconColor, 0.6f);
                iconInset = iconSize + ScaleX(6);
            }

            // Adjust text area for the leading search icon
            var adjustedRect = new Rectangle(
                textAreaRect.X + iconInset,
                textAreaRect.Y,
                Math.Max(1, textAreaRect.Width - iconInset),
                textAreaRect.Height);

            base.DrawText(g, adjustedRect);
        }

        protected override void DrawDecorations(Graphics g, Rectangle drawingRect)
        {
            if (drawingRect.IsEmpty) return;
            var istate = GetInteractionState();

            if (istate == ComboBoxInteractionState.Focused || istate == ComboBoxInteractionState.Open)
            {
                // Focus ring using FocusBorderColor — matches popup selected row border
                Color focusColor = _theme?.ButtonOutlineBorderColor ?? _theme?.PrimaryColor ?? Color.FromArgb(25, 118, 210);
                int radius = ScaleX(CornerRadius);
                using var path = GetRoundedRectPath(Rectangle.Inflate(drawingRect, -1, -1), radius);
                using var pen = new Pen(Color.FromArgb(90, focusColor), ScaleX(2));
                g.DrawPath(pen, path);
            }
            else if (istate == ComboBoxInteractionState.Hover)
            {
                // Hover: subtle border accent — matches popup HoverBorderColor on hover rows
                Color hoverBorder = _theme?.ButtonHoverBorderColor ?? Color.FromArgb(180, 180, 195);
                int radius = ScaleX(CornerRadius);
                using var path = GetRoundedRectPath(Rectangle.Inflate(drawingRect, -1, -1), radius);
                using var pen = new Pen(Color.FromArgb(50, hoverBorder), ScaleX(1));
                g.DrawPath(pen, path);
            }
        }
    }
}

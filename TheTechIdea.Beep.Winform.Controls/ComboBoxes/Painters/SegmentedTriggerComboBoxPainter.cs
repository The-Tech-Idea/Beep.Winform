using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Split-button with accent trigger — synced with <see cref="Popup.GroupedSectionsPopupContent"/>.
    /// Popup section headers use accent bar (3px, FocusBorderColor) + tinted bg (alpha 30).
    /// Popup section pills use radius min(h/2,6) with accent fill for selected items.
    /// Field echoes: right accent-filled trigger segment, accent tint on body focus
    /// (matching section header tinted bg), focus ring using same accent color.
    /// </summary>
    internal sealed class SegmentedTriggerComboBoxPainter : DesignSystemComboBoxFieldPainterBase
    {
        protected override bool IsSegmented => true;
        protected override int CornerRadius => 8;

        public override int GetPreferredButtonWidth() => 40;
        public override Padding GetPreferredPadding() => new Padding(12, 6, 8, 6);

        protected override void DrawTextArea(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;
            var istate = GetInteractionState();

            // Body hover: subtle text-area tint (NOT the trigger segment)
            if (istate == ComboBoxInteractionState.Hover && !_owner.IsButtonHovered)
            {
                using var brush = new SolidBrush(Color.FromArgb(10, _theme?.ForeColor ?? Color.Black));
                g.FillRectangle(brush, textAreaRect);
            }
            else if (istate == ComboBoxInteractionState.Focused || istate == ComboBoxInteractionState.Open)
            {
                // Accent tint matching popup SectionHeader tinted bg (alpha 30 of FocusBorderColor)
                Color accent = _theme?.PrimaryColor ?? Color.FromArgb(25, 118, 210);
                using var brush = new SolidBrush(Color.FromArgb(14, accent));
                g.FillRectangle(brush, textAreaRect);
            }
        }

        protected override void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;

            // Accent-filled trigger zone — uses FocusBorderColor matching popup section accent bars
            Color accent = _theme?.PrimaryColor ?? _theme?.ComboBoxHoverBorderColor ?? Color.FromArgb(0, 120, 215);
            Color fill;
            if (_owner.IsDropdownOpen)
                fill = PathPainterHelpers.WithAlphaIfNotEmpty(accent, 240);
            else if (_owner.IsButtonHovered)
                fill = PathPainterHelpers.WithAlphaIfNotEmpty(accent, 210);
            else
                fill = PathPainterHelpers.WithAlphaIfNotEmpty(accent, 180);

            int r = ScaleX(CornerRadius);
            // Right-only rounded rectangle for the trigger zone
            using var segPath = new GraphicsPath();
            segPath.AddLine(buttonRect.Left, buttonRect.Top, buttonRect.Right - r, buttonRect.Top);
            segPath.AddArc(buttonRect.Right - r * 2, buttonRect.Top, r * 2, r * 2, 270, 90);
            segPath.AddArc(buttonRect.Right - r * 2, buttonRect.Bottom - r * 2, r * 2, r * 2, 0, 90);
            segPath.AddLine(buttonRect.Right - r, buttonRect.Bottom, buttonRect.Left, buttonRect.Bottom);
            segPath.CloseFigure();

            g.FillPath(PaintersFactory.GetSolidBrush(fill), segPath);

            // Vertical accent separator — echoes popup SectionSeparator line treatment
            Color separatorColor = Color.FromArgb(60, Color.White);
            g.DrawLine(PaintersFactory.GetPen(separatorColor, 1f),
                buttonRect.Left, buttonRect.Top + ScaleY(4),
                buttonRect.Left, buttonRect.Bottom - ScaleY(4));

            // White chevron icon
            Color iconColor = _theme?.OnPrimaryColor ?? Color.White;
            int iconSize = Math.Min(ScaleX(15), Math.Min(buttonRect.Width, buttonRect.Height) - ScaleX(8));
            if (iconSize > 4)
            {
                var iconRect = new Rectangle(
                    buttonRect.X + (buttonRect.Width - iconSize) / 2,
                    buttonRect.Y + (buttonRect.Height - iconSize) / 2,
                    iconSize, iconSize);
                DrawSvgIcon(g, iconRect, SvgsUI.ChevronDown, iconColor, 1f, _owner?.ChevronAngle ?? 0f);
            }
        }

        protected override void DrawDecorations(Graphics g, Rectangle drawingRect)
        {
            if (drawingRect.IsEmpty) return;
            var istate = GetInteractionState();

            // Focus: accent-tinted ring matching popup section header accent treatment
            if (istate == ComboBoxInteractionState.Focused || istate == ComboBoxInteractionState.Open)
            {
                Color focusColor = _theme?.PrimaryColor ?? Color.FromArgb(25, 118, 210);
                int radius = ScaleX(CornerRadius);
                using var path = GetRoundedRectPath(Rectangle.Inflate(drawingRect, -1, -1), radius);
                using var pen = new Pen(Color.FromArgb(80, focusColor), ScaleX(2));
                g.DrawPath(pen, path);
            }

            // Left accent bar on focus — mirrors popup SectionHeader accent bar (3px wide)
            if (istate == ComboBoxInteractionState.Focused || istate == ComboBoxInteractionState.Open)
            {
                Color accent = _theme?.PrimaryColor ?? Color.FromArgb(25, 118, 210);
                int barWidth = ScaleX(3);
                int barTop = drawingRect.Top + ScaleY(6);
                int barBot = drawingRect.Bottom - ScaleY(6);
                if (barBot > barTop)
                {
                    using var barBrush = new SolidBrush(Color.FromArgb(160, accent));
                    g.FillRectangle(barBrush, drawingRect.Left + 1, barTop, barWidth, barBot - barTop);
                }
            }
        }
    }
}

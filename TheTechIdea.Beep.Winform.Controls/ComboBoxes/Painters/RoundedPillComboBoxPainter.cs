using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// High-radius pill shell — synced with <see cref="Popup.PillGridPopupContent"/>.
    /// Popup renders items as pill buttons (radius height/2) in a wrap-flow grid with
    /// neutral bg (245,245,248), border (220,220,228), and selected=FocusBorderColor.
    /// Field echoes: pill-shaped container, matching neutral tint, shadow lift on hover,
    /// outer glow on focus using same FocusBorderColor.
    /// </summary>
    internal sealed class RoundedPillComboBoxPainter : DesignSystemComboBoxFieldPainterBase
    {
        // Dynamic pill radius: height/2 for true pill shape — matches popup pill radius
        protected override int CornerRadius => Math.Max(18, (_owner?.Height ?? 36) / 2);
        protected override bool ShowButtonSeparator => false;

        public override int GetPreferredButtonWidth() => 38;
        public override Padding GetPreferredPadding() => new Padding(16, 8, 12, 8);

        protected override void DrawTextArea(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;
            var istate = GetInteractionState();

            if (istate == ComboBoxInteractionState.Hover)
            {
                // Matches popup PillButton hover: PopupRowHoverColor ≈ faint primary tint
                Color hoverFill = Color.FromArgb(14, _theme?.PrimaryColor ?? Color.FromArgb(25, 118, 210));
                using var brush = new SolidBrush(hoverFill);
                g.FillRectangle(brush, textAreaRect);
            }
            else if (istate == ComboBoxInteractionState.Focused || istate == ComboBoxInteractionState.Open)
            {
                // Matches popup PillButton focus: PopupRowFocusColor ≈ soft primary tint
                Color focusFill = Color.FromArgb(10, _theme?.PrimaryColor ?? Color.FromArgb(25, 118, 210));
                using var brush = new SolidBrush(focusFill);
                g.FillRectangle(brush, textAreaRect);
            }
            else
            {
                // Neutral state: faint pill fill matching popup pill neutral bg (245,245,248)
                using var brush = new SolidBrush(Color.FromArgb(6, 120, 120, 140));
                g.FillRectangle(brush, textAreaRect);
            }
        }

        protected override void DrawDecorations(Graphics g, Rectangle drawingRect)
        {
            if (drawingRect.IsEmpty) return;
            var istate = GetInteractionState();

            // Hover: card-shadow lift matching popup pill hover elevation
            if (istate == ComboBoxInteractionState.Hover)
            {
                int pillRadius = ScaleX(CornerRadius);
                var shadowRect = new Rectangle(drawingRect.X + 2, drawingRect.Y + 2, drawingRect.Width, drawingRect.Height);
                using var shadowPath = GetRoundedRectPath(shadowRect, pillRadius);
                using var shadowBrush = new SolidBrush(Color.FromArgb(18, Color.Black));
                g.FillPath(shadowBrush, shadowPath);
            }

            // Focus/Open: outer glow ring using FocusBorderColor — matches popup selected pill border
            if (istate == ComboBoxInteractionState.Focused || istate == ComboBoxInteractionState.Open)
            {
                Color primaryGlow = _theme?.PrimaryColor ?? Color.FromArgb(25, 118, 210);
                int pillRadius = ScaleX(CornerRadius);
                var outerRect = Rectangle.Inflate(drawingRect, ScaleX(2), ScaleY(2));
                using var outerPath = GetRoundedRectPath(outerRect, pillRadius + ScaleX(2));
                using var glowPen = new Pen(Color.FromArgb(50, primaryGlow), ScaleX(2));
                g.DrawPath(glowPen, outerPath);
            }
        }

        protected override void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;

            // Pill-style: small chevron inside the rounded end, no separator
            Color arrowColor = GetArrowColor();
            int iconSize = Math.Min(ScaleX(14), Math.Min(buttonRect.Width, buttonRect.Height) - ScaleX(8));
            if (iconSize > 4)
            {
                var iconRect = new Rectangle(
                    buttonRect.X + (buttonRect.Width - iconSize) / 2,
                    buttonRect.Y + (buttonRect.Height - iconSize) / 2,
                    iconSize, iconSize);
                DrawSvgIcon(g, iconRect, SvgsUI.ChevronDown, arrowColor, 0.85f, _owner?.ChevronAngle ?? 0f);
            }
        }
    }
}

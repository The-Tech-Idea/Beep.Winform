using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Material-style filled field — synced with <see cref="Popup.CardRowPopupContent"/>.
    /// Card-row popup uses rounded cards (radius 8) with shadow on hover and subtle borders.
    /// Field echoes: filled tint, 8px radius top corners, bottom underline accent,
    /// card-like shadow lift on hover to match popup card shadow.
    /// </summary>
    internal sealed class FilledSoftComboBoxPainter : DesignSystemComboBoxFieldPainterBase
    {
        protected override bool IsFilled => true;
        protected override int CornerRadius => 8; // matches CardRow card radius
        protected override bool ShowButtonSeparator => false;

        public override int GetPreferredButtonWidth() => 36;
        public override Padding GetPreferredPadding() => new Padding(12, 8, 8, 8);

        protected override void DrawTextArea(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;
            var istate = GetInteractionState();

            // Filled tinted background — matches CardRow card default bg (252,253,255)
            Color primary = _theme?.PrimaryColor ?? Color.FromArgb(25, 118, 210);
            int alpha = istate == ComboBoxInteractionState.Hover ? 28 : 18;
            if (istate == ComboBoxInteractionState.Focused || istate == ComboBoxInteractionState.Open)
                alpha = 35;

            Color fillColor = Color.FromArgb(alpha, primary);

            // Top corners rounded, bottom square for filled material style (radius 8 = popup card radius)
            int r = ScaleX(CornerRadius);
            using var path = new GraphicsPath();
            path.AddArc(textAreaRect.X, textAreaRect.Y, r * 2, r * 2, 180, 90);
            path.AddArc(textAreaRect.Right - r * 2, textAreaRect.Y, r * 2, r * 2, 270, 90);
            path.AddLine(textAreaRect.Right, textAreaRect.Bottom, textAreaRect.X, textAreaRect.Bottom);
            path.CloseFigure();

            using var brush = new SolidBrush(fillColor);
            g.FillPath(brush, path);
        }

        protected override void DrawDecorations(Graphics g, Rectangle drawingRect)
        {
            if (drawingRect.IsEmpty) return;
            var istate = GetInteractionState();

            // Card-like shadow lift on hover — matches CardRow hover shadow (2px offset, alpha 20)
            if (istate == ComboBoxInteractionState.Hover)
            {
                int r = ScaleX(CornerRadius);
                var shadowRect = new Rectangle(drawingRect.X + 1, drawingRect.Y + ScaleY(2),
                    drawingRect.Width - 2, drawingRect.Height);
                using var shadowPath = GetRoundedRectPath(shadowRect, r);
                using var shadowBrush = new SolidBrush(Color.FromArgb(16, 0, 0, 0));
                g.FillPath(shadowBrush, shadowPath);
            }

            // Bottom underline — material accent matches CardRow selected-card border (FocusBorderColor)
            Color underlineColor;
            float thickness;
            if (istate == ComboBoxInteractionState.Focused || istate == ComboBoxInteractionState.Open)
            {
                underlineColor = _theme?.PrimaryColor ?? Color.FromArgb(25, 118, 210);
                thickness = ScaleY(3);
            }
            else if (istate == ComboBoxInteractionState.Error)
            {
                underlineColor = _theme?.ErrorColor ?? Color.FromArgb(183, 28, 28);
                thickness = ScaleY(2);
            }
            else
            {
                // Matches CardRow normal card border alpha (80 of PopupSeparatorColor ≈ muted gray)
                underlineColor = Color.FromArgb(100, _theme?.BorderColor ?? Color.Gray);
                thickness = ScaleY(1);
            }

            int y = drawingRect.Bottom - (int)Math.Ceiling(thickness);
            using var pen = new Pen(underlineColor, thickness);
            g.DrawLine(pen, drawingRect.Left + ScaleX(2), y, drawingRect.Right - ScaleX(2), y);
        }

        protected override void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;

            // Subtle hover highlight on button area
            if (_owner.IsButtonHovered && _owner.Enabled)
            {
                using var hoverBrush = new SolidBrush(Color.FromArgb(20, _theme?.ForeColor ?? Color.Black));
                g.FillRectangle(hoverBrush, buttonRect);
            }

            // No separator for filled style — just the chevron icon
            Color arrowColor = GetArrowColor();
            int iconSize = Math.Min(ScaleX(15), Math.Min(buttonRect.Width, buttonRect.Height) - ScaleX(8));
            if (iconSize > 4)
            {
                var iconRect = new Rectangle(
                    buttonRect.X + (buttonRect.Width - iconSize) / 2,
                    buttonRect.Y + (buttonRect.Height - iconSize) / 2,
                    iconSize, iconSize);
                DrawSvgIcon(g, iconRect, SvgsUI.ChevronDown, arrowColor, 0.9f, _owner?.ChevronAngle ?? 0f);
            }
        }
    }
}

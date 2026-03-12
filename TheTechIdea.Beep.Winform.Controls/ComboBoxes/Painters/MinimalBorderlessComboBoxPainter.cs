using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Low-chrome borderless shell — synced with <see cref="Popup.MinimalCleanPopupContent"/>.
    /// Popup is ultra-clean: text-only rows, no icons/checkmarks, rounded hover rect
    /// (radius 6, bg #F5F7FA), selected = bold + FocusBorderColor, group headers = small-caps muted.
    /// Field echoes: no border, rounded hover background matching popup hover rect,
    /// bottom underline on focus using FocusBorderColor, bold display for selected text,
    /// small muted chevron.
    /// </summary>
    internal sealed class MinimalBorderlessComboBoxPainter : DesignSystemComboBoxFieldPainterBase
    {
        protected override bool IsBorderless => true;
        protected override int CornerRadius => 6; // matches popup MinimalRow hover rect radius
        protected override bool ShowButtonSeparator => false;

        public override int GetPreferredButtonWidth() => 22;
        public override Padding GetPreferredPadding() => new Padding(4, 6, 4, 6);

        protected override void DrawTextArea(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;
            var istate = GetInteractionState();

            // Hover: rounded subtle backdrop — matches popup MinimalRow hover rect (#F5F7FA)
            if (istate == ComboBoxInteractionState.Hover)
            {
                int radius = ScaleX(CornerRadius);
                var hoverRect = Rectangle.Inflate(textAreaRect, 0, -1);
                using var hoverPath = GetRoundedRectPath(hoverRect, radius);
                using var brush = new SolidBrush(Color.FromArgb(245, 247, 250)); // #F5F7FA
                g.FillPath(brush, hoverPath);
            }
            else if (istate == ComboBoxInteractionState.Focused || istate == ComboBoxInteractionState.Open)
            {
                // Focus: even subtler tint matching popup selected row bg (240,245,252)
                int radius = ScaleX(CornerRadius);
                var focusRect = Rectangle.Inflate(textAreaRect, 0, -1);
                using var focusPath = GetRoundedRectPath(focusRect, radius);
                using var brush = new SolidBrush(Color.FromArgb(242, 246, 252));
                g.FillPath(brush, focusPath);
            }
        }

        protected override void DrawText(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;

            // If showing a selected value (not placeholder), render bold
            // matching popup MinimalRow selected = bold + FocusBorderColor
            string displayText = _helper?.GetDisplayText();
            if (string.IsNullOrEmpty(displayText))
            {
                base.DrawText(g, textAreaRect);
                return;
            }

            bool isPlaceholder = _helper.IsShowingPlaceholder();
            Color textColor;
            FontStyle fontStyle;

            if (isPlaceholder)
            {
                textColor = Color.FromArgb(140, 140, 155); // matches popup group header muted color
                fontStyle = FontStyle.Regular;
            }
            else
            {
                // Selected value: use FocusBorderColor + bold, matching popup selected row style
                textColor = _helper.GetTextColor();
                fontStyle = FontStyle.Regular; // keep regular in field for density; popup uses bold only on selected rows
            }

            Font baseFont = _owner.TextFont ?? BeepThemesManager.ToFont(_theme?.LabelFont) ?? SystemFonts.DefaultFont;
            using var textFont = new Font(baseFont.FontFamily, baseFont.Size, fontStyle);
            int horizontalInset = ScaleX(_tokens?.TextInset ?? 4);
            var textBounds = new Rectangle(
                textAreaRect.X + horizontalInset,
                textAreaRect.Y,
                Math.Max(1, textAreaRect.Width - horizontalInset * 2),
                textAreaRect.Height);

            TextRenderer.DrawText(g, displayText, textFont, textBounds, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }

        protected override void DrawDecorations(Graphics g, Rectangle drawingRect)
        {
            if (drawingRect.IsEmpty) return;
            var istate = GetInteractionState();

            // Bottom underline: hover=muted, focus=FocusBorderColor (matches popup selected text color)
            Color underlineColor;
            float thickness;
            bool drawLine = false;

            if (istate == ComboBoxInteractionState.Focused || istate == ComboBoxInteractionState.Open)
            {
                underlineColor = _theme?.PrimaryColor ?? Color.FromArgb(25, 118, 210);
                thickness = ScaleY(2);
                drawLine = true;
            }
            else if (istate == ComboBoxInteractionState.Hover)
            {
                // Muted underline matches popup separator treatment (alpha 40 of 180,180,195)
                underlineColor = Color.FromArgb(100, 180, 180, 195);
                thickness = ScaleY(1);
                drawLine = true;
            }
            else if (istate == ComboBoxInteractionState.Error)
            {
                underlineColor = _theme?.ErrorColor ?? Color.FromArgb(183, 28, 28);
                thickness = ScaleY(2);
                drawLine = true;
            }
            else
            {
                underlineColor = Color.Transparent;
                thickness = 0;
            }

            if (drawLine)
            {
                int y = drawingRect.Bottom - (int)Math.Ceiling(thickness);
                using var pen = new Pen(underlineColor, thickness);
                g.DrawLine(pen, drawingRect.Left, y, drawingRect.Right, y);
            }
        }

        protected override void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;

            // Minimal: small muted chevron, no separator, no hover highlight
            Color arrowColor = Color.FromArgb(120, _theme?.SecondaryColor ?? Color.Gray);
            if (_owner.IsButtonHovered)
                arrowColor = Color.FromArgb(180, _theme?.SecondaryColor ?? Color.Gray);

            int iconSize = Math.Min(ScaleX(11), Math.Min(buttonRect.Width, buttonRect.Height) - ScaleX(6));
            if (iconSize > 3)
            {
                var iconRect = new Rectangle(
                    buttonRect.X + (buttonRect.Width - iconSize) / 2,
                    buttonRect.Y + (buttonRect.Height - iconSize) / 2,
                    iconSize, iconSize);
                DrawSvgIcon(g, iconRect, SvgsUI.ChevronDown, arrowColor, 0.7f, _owner?.ChevronAngle ?? 0f);
            }
        }
    }
}

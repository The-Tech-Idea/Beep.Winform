using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Single choice radio-Style list (from image 5 - Tickets Setup)
    /// Radio buttons on right, one item has colored background for selection
    /// </summary>
    internal class RadioSelectionPainter : BaseListBoxPainter
    {
        public override bool SupportsCheckboxes() => false; // Uses radio buttons instead
        
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            // Slight inset for this Style
            var rect = itemRect;
            rect.Inflate(-Scale(6), -Scale(3));

                DrawItemBackgroundEx(g, rect, item, isHovered, isSelected);

            // Use layout rects as base, then reserve area for radio on the right
            var info = _layout.GetCachedLayout().FirstOrDefault(i => i.Item == item);
            var textBase = info?.TextRect ?? rect;
            var iconRect = info?.IconRect ?? Rectangle.Empty;

            // Size radio relative to row height to behave well on DPI/varied heights
            int radioSize = Math.Min(Scale(20), Math.Max(Scale(14), rect.Height - Scale(12)));
            var radioRect = new Rectangle(rect.Right - radioSize - Scale(16), rect.Y + (rect.Height - radioSize) / 2, radioSize, radioSize);

            // Shrink text area to avoid radio overlap
            var textAvail = new Rectangle(textBase.Left, textBase.Top, Math.Max(0, radioRect.Left - Scale(12) - textBase.Left), textBase.Height);

            // Optional icon (left) if provided
            if (_owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath) && !iconRect.IsEmpty)
            {
                DrawItemImage(g, iconRect, item.ImagePath);
            }

            // One title/subtitle layout, shared with every other two-line style. The split-in-
            // half arithmetic this replaces gave the subtitle less room than its own font needed,
            // so it was cut through horizontally.
            bool hasDesc = !string.IsNullOrEmpty(item.Description);
            bool disabled = item?.IsEnabled == false;

            Color mainColor = isSelected
                ? (Theme.OnPrimaryColor)
                : (disabled ? Theme.DisabledForeColor : _helper.GetTextColor());

            // Bold only when selected; never clone the owner's font.
            Font mainFont = isSelected ? GetCachedFont(_owner.TextFont.Size, FontStyle.Bold) : _owner.TextFont;

            Color onPrimary = Theme.OnPrimaryColor;
            Color secondary = Theme.SecondaryTextColor;
            Color descColor = isSelected
                ? Color.FromArgb(220, onPrimary)
                : (disabled ? Theme.DisabledForeColor : secondary);

            DrawTitleAndSubtitle(g, textAvail, item.Text, hasDesc ? item.Description : null,
                mainColor, descColor, mainFont,
                hasDesc ? GetCachedFont(Math.Max(Scale(6), _owner.TextFont.Size - 1)) : null);

            // Right-aligned radio control
            DrawRadioButton(g, radioRect, isSelected, isHovered, item);
        }
        
        // Enhanced hover effects and selection indicators
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Use BeepStyling for RadioSelection background, border, and shadow
            using (var path = Beep.Winform.Controls.Styling.BeepStyling.CreateControlStylePath(itemRect, Style))
            {
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBackground(g, path, Style);
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, false, Style);

                // Add hover effect with subtle shadow
                if (isHovered && !isSelected)
                {
                    g.FillPath(GetBrush(PathPainterHelpers.WithAlphaIfNotEmpty(_theme?.PrimaryColor ?? Color.Empty, 30)), path);
                }
            }
        }
        
        private void DrawRadioButton(Graphics g, Rectangle radioRect, bool isSelected, bool isHovered, SimpleItem item)
        {
            bool isDisabled = item?.IsEnabled == false;

            // Outer circle fill and border
            Color outerFill = Theme.ListBackColor;
            Color borderColor = isDisabled
                ? Theme.BorderColor
                : (isSelected || isHovered) ? (_theme?.PrimaryColor ?? _theme?.AccentColor ?? Color.Empty)
                                             : (Theme.BorderColor);

            g.FillEllipse(GetBrush(outerFill), radioRect);
            g.DrawEllipse(GetPen(borderColor, Scale(2)), radioRect.X + Scale(1), radioRect.Y + Scale(1), radioRect.Width - Scale(3), radioRect.Height - Scale(3));

            // Inner dot when selected
            if (isSelected)
            {
                var innerRect = radioRect;
                innerRect.Inflate(-(Math.Max(Scale(4), radioRect.Width / 4)), -(Math.Max(Scale(4), radioRect.Height / 4)));

                Color dotColor = isDisabled
                    ? Theme.DisabledForeColor
                    : (_theme?.PrimaryColor ?? _theme?.AccentColor ?? Color.Empty);
                g.FillEllipse(GetBrush(dotColor), innerRect);
            }
        }
        
        public override int GetPreferredItemHeight()
        {
            int fontH = _owner?.TextFont?.Height ?? Scale(16);
            int descH = Math.Max(Scale(10), fontH - Scale(2));
            int contentTwoLine = fontH + descH + Scale(12); // paddings
            int radioTarget = Math.Max(Scale(14), Math.Min(Scale(20), contentTwoLine - Scale(12)));
            int height = Math.Max(contentTwoLine, radioTarget + Scale(12));
            return Math.Max(Scale(48), height);
        }
    }
}

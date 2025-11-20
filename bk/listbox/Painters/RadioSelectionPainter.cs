using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;

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
            rect.Inflate(-6, -3);

            DrawItemBackground(g, rect, isHovered, isSelected);

            // Use layout rects as base, then reserve area for radio on the right
            var info = _layout.GetCachedLayout().FirstOrDefault(i => i.Item == item);
            var textBase = info?.TextRect ?? rect;
            var iconRect = info?.IconRect ?? Rectangle.Empty;

            // Size radio relative to row height to behave well on DPI/varied heights
            int radioSize = Math.Min(20, Math.Max(14, rect.Height - 12));
            var radioRect = new Rectangle(rect.Right - radioSize - 16, rect.Y + (rect.Height - radioSize) / 2, radioSize, radioSize);

            // Shrink text area to avoid radio overlap
            var textAvail = new Rectangle(textBase.Left, textBase.Top, Math.Max(0, radioRect.Left - 12 - textBase.Left), textBase.Height);

            // Optional icon (left) if provided
            if (_owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath) && !iconRect.IsEmpty)
            {
                DrawItemImage(g, iconRect, item.ImagePath);
            }

            // Layout: if description is present, use two lines; otherwise center the main text
            bool hasDesc = !string.IsNullOrEmpty(item.Description);
            int topPadding = Math.Max(4, rect.Height / 8);
            int split = textAvail.Height / 2;
            var mainRect = hasDesc
                ? new Rectangle(textAvail.Left, textAvail.Top + topPadding, textAvail.Width, Math.Max(0, split - topPadding))
                : textAvail;
            var descRect = hasDesc
                ? new Rectangle(textAvail.Left, textAvail.Top + split, textAvail.Width, Math.Max(0, textAvail.Height - split - 6))
                : Rectangle.Empty;

            bool disabled = item?.IsEnabled == false;
            Color mainColor = isSelected
                ? (_theme?.OnPrimaryColor ?? Color.White)
                : (disabled ? Color.FromArgb(160, 160, 160) : _helper.GetTextColor());

            // Use bold font only when selected; avoid cloning owner's font
            Font mainFont = _owner.TextFont;
            bool disposeMain = false;
            if (isSelected)
            {
                mainFont = new Font(_owner.TextFont, FontStyle.Bold);
                disposeMain = true;
            }
            try
            {
                if (mainRect.Height > 0)
                    DrawItemText(g, mainRect, item.Text, mainColor, mainFont);
            }
            finally
            {
                if (disposeMain) mainFont.Dispose();
            }

            if (hasDesc && descRect.Height > 0)
            {
                using (var smallFont = new Font(_owner.TextFont.FontFamily, Math.Max(6, _owner.TextFont.Size - 1)))
                {
                    Color onPrimary = _theme?.OnPrimaryColor ?? Color.White;
                    Color secondary = _theme?.SecondaryTextColor ?? Color.FromArgb(120, 120, 120);
                    Color disabledColor = Color.FromArgb(180, 180, 180);
                    Color descColor = isSelected ? Color.FromArgb(220, onPrimary) : (disabled ? disabledColor : secondary);
                    System.Windows.Forms.TextRenderer.DrawText(g, item.Description, smallFont, descRect, descColor,
                        System.Windows.Forms.TextFormatFlags.Left | System.Windows.Forms.TextFormatFlags.Top | System.Windows.Forms.TextFormatFlags.EndEllipsis);
                }
            }

            // Right-aligned radio control
            DrawRadioButton(g, radioRect, isSelected, isHovered, item);
        }
        
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Use BeepStyling for RadioSelection background, border, and shadow
           
            using (var path = Beep.Winform.Controls.Styling.BeepStyling.CreateControlStylePath(itemRect, Style))
            {
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBackground(g, path, Style);
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, false, Style);
            }
        }
        
        private void DrawRadioButton(Graphics g, Rectangle radioRect, bool isSelected, bool isHovered, SimpleItem item)
        {
            bool isDisabled = item?.IsEnabled == false;

            // Outer circle fill and border
            Color outerFill = Color.White;
            Color borderColor = isDisabled
                ? Color.FromArgb(200, 200, 200)
                : (isSelected || isHovered) ? (_theme?.PrimaryColor ?? _theme?.AccentColor ?? Color.SteelBlue)
                                             : (_theme?.BorderColor ?? Color.FromArgb(180, 180, 180));

            using (var brush = new SolidBrush(outerFill))
            {
                g.FillEllipse(brush, radioRect);
            }
            using (var pen = new Pen(borderColor, 2f))
            {
                g.DrawEllipse(pen, radioRect.X + 1, radioRect.Y + 1, radioRect.Width - 3, radioRect.Height - 3);
            }

            // Inner dot when selected
            if (isSelected)
            {
                var innerRect = radioRect;
                innerRect.Inflate(-(Math.Max(4, radioRect.Width / 4)), -(Math.Max(4, radioRect.Height / 4)));

                Color dotColor = isDisabled
                    ? Color.FromArgb(180, 180, 180)
                    : (_theme?.PrimaryColor ?? _theme?.AccentColor ?? Color.SteelBlue);
                using (var brush = new SolidBrush(dotColor))
                {
                    g.FillEllipse(brush, innerRect);
                }
            }
        }
        
        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter - 1, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter - 1, rect.Bottom - diameter - 1, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter - 1, diameter, diameter, 90, 90);
            path.CloseFigure();
            
            return path;
        }
        
        public override int GetPreferredItemHeight()
        {
            int fontH = _owner?.TextFont?.Height ?? 16;
            int descH = Math.Max(10, fontH - 2);
            int contentTwoLine = fontH + descH + 12; // paddings
            int radioTarget = Math.Max(14, Math.Min(20, contentTwoLine - 12));
            int height = Math.Max(contentTwoLine, radioTarget + 12);
            return Math.Max(48, height);
        }
    }
}

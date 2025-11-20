using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Base implementation for list box painters
    /// Provides common functionality for all variants
    /// </summary>
    internal abstract class BaseListBoxPainter : IListBoxPainter
    {
    protected BeepListBox _owner;
    protected IBeepTheme _theme;
    protected BeepListBoxHelper _helper;
    protected BeepListBoxLayoutHelper _layout;
    public BeepControlStyle Style { get; set; } = BeepControlStyle.Minimal;

    public virtual void Initialize(BeepListBox owner, IBeepTheme theme)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        _theme = theme;
        _helper = new BeepListBoxHelper(owner);
            _layout = owner.LayoutHelper;
        }
        
        public virtual void Paint(Graphics g, BeepListBox owner, Rectangle drawingRect)
        {
            if (g == null || owner == null || drawingRect.Width <= 0 || drawingRect.Height <= 0)
                return;
            
            _owner = owner;
            // Use the CurrentTheme property instead of MenuStyle string property
            _theme = owner._currentTheme;
            
            // Set high-quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            
            // Get layout and items
            var items = _helper.GetVisibleItems();
            _layout.CalculateLayout();
            var cache = _layout.GetCachedLayout();

            // Optionally draw search at top (kept simple)
            int yOffset = drawingRect.Y;
            if (_owner.ShowSearch && SupportsSearch())
            {
                yOffset = DrawSearchArea(g, drawingRect, yOffset);
            }

            // If no items, draw empty state if enabled
            if ((items == null || items.Count == 0) && _owner.ShowEmptyState)
            {
                DrawEmptyState(g, drawingRect, yOffset);
                return;
            }

            DrawItems(g, drawingRect, items, yOffset);
        }

        protected virtual void DrawEmptyState(Graphics g, Rectangle drawingRect, int yOffset)
        {
            var rect = new Rectangle(drawingRect.Left, yOffset + 12, drawingRect.Width, drawingRect.Height - (yOffset - drawingRect.Top) - 12);
            string text = !string.IsNullOrEmpty(_owner.EmptyStateText) ? _owner.EmptyStateText : "No items";

            // Small icon or circle
            int iconSize = 36;
            Rectangle iconRect = new Rectangle(rect.Left + (rect.Width - iconSize) / 2, rect.Top + 8, iconSize, iconSize);
            using (var brush = new SolidBrush(Color.FromArgb(40, _theme?.PrimaryColor ?? Color.LightBlue)))
            {
                g.FillEllipse(brush, iconRect);
            }

            // Draw text
            var textRect = new Rectangle(rect.Left + 8, iconRect.Bottom + 8, rect.Width - 16, 36);
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near })
            using (var font = new Font(_owner.Font.FontFamily, Math.Max(10, _owner.Font.Size - 1f), FontStyle.Regular))
            using (var brush = new SolidBrush(_theme?.TextColor ?? Color.Gray))
            {
                g.DrawString(text, font, brush, textRect, sf);
            }
        }
        
        public virtual int GetPreferredItemHeight()
        {
            return Math.Max(_owner.Font.Height + 12, 36); // Minimum height of 36px
        }
        
        public virtual Padding GetPreferredPadding()
        {
            return new Padding(8, 4, 8, 4);
        }
        
        public virtual bool SupportsSearch()
        {
            return true;
        }
        
        public virtual bool SupportsCheckboxes()
        {
            return true;
        }
        
        #region Abstract Methods
        
        /// <summary>
      
        #endregion
        
        #region Common Drawing Methods
        
        protected virtual int DrawSearchArea(Graphics g, Rectangle drawingRect, int yOffset)
        {
            int searchHeight = 32;
            Rectangle searchRect = new Rectangle(drawingRect.X, yOffset, drawingRect.Width, searchHeight);
            
            // Draw search background
            using (var brush = new SolidBrush(Color.FromArgb(250, 250, 250)))
            {
                g.FillRectangle(brush, searchRect);
            }
            
            // Draw search border
            using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1f))
            {
                g.DrawLine(pen, searchRect.Left, searchRect.Bottom, searchRect.Right, searchRect.Bottom);
            }
            
            return yOffset + searchHeight + 4;
        }
        
        protected virtual void DrawItems(Graphics g, Rectangle drawingRect, System.Collections.Generic.List<SimpleItem> items, int yOffset)
        {
            if (items == null || items.Count == 0)
                return;

            var layout = _layout.GetCachedLayout();
            if (layout == null || layout.Count == 0)
                return;

            Point mousePoint = _owner.PointToClient(Control.MousePosition);

            foreach (var info in layout)
            {
                var item = info.Item;
                var rowRect = info.RowRect;
                if (rowRect.IsEmpty) continue;

                bool isHovered = rowRect.Contains(mousePoint);
                bool isSelected = _owner.IsItemSelected(item);

                // Let concrete painter draw using the row rect
                DrawItem(g, rowRect, item, isHovered, isSelected);
            }

            // Optionally draw 'Page Up / Page Down' hints if the content is taller than the viewport
            try
            {
                var clientArea = _owner.GetClientArea();
                var ownerVirt = new Size();
                ownerVirt = new Size(_owner.Width, _owner.PreferredItemHeight * items.Count);
                if (clientArea.Height > 0 && ownerVirt.Height > clientArea.Height)
                {
                    using (var sf = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Far })
                    using (var font = new Font(_owner.Font.FontFamily, Math.Max(8, _owner.Font.Size - 2), FontStyle.Regular))
                    using (var brush = new SolidBrush(Color.FromArgb(140, _theme?.TextColor ?? Color.Gray)))
                    {
                        var hint = "PgUp / PgDn";
                        var hintRect = new Rectangle(drawingRect.Right - 120, drawingRect.Bottom - 26, 110, 20);
                        g.DrawString(hint, font, brush, hintRect, sf);
                    }
                }
            }
            catch { }
        }
        
        protected virtual void DrawItemText(Graphics g, Rectangle textRect, string text, Color textColor, Font font)
        {
            if (string.IsNullOrEmpty(text))
                return;
            
            TextFormatFlags flags = TextFormatFlags.Left |
                                   TextFormatFlags.VerticalCenter |
                                   TextFormatFlags.EndEllipsis |
                                   TextFormatFlags.NoPrefix;
            
            TextRenderer.DrawText(g, text, font, textRect, textColor, flags);
        }
        
        protected virtual void DrawItemImage(Graphics g, Rectangle imageRect, string imagePath)
        {
            if (imageRect.IsEmpty || string.IsNullOrEmpty(imagePath))
                return;
            
            try
            {
                
                StyledImagePainter.Paint(g, imageRect, imagePath, Style);
            }
            catch
            {
                // Fallback: draw a placeholder
                using (var brush = new SolidBrush(Color.FromArgb(150, Color.Gray)))
                {
                    var smallRect = imageRect;
                    smallRect.Inflate(-4, -4);
                    g.FillEllipse(brush, smallRect);
                }
            }
        }
        
        protected virtual void DrawCheckbox(Graphics g, Rectangle checkboxRect, bool isChecked, bool isHovered)
        {
            // Draw checkbox background
            Color bgColor = isHovered ? Color.FromArgb(240, 240, 240) : Color.White;
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, checkboxRect);
            }
            
            // Draw checkbox border
            Color borderColor = isChecked ? (_theme?.PrimaryColor ?? Color.Blue) : Color.Gray;
            using (var pen = new Pen(borderColor, 1.5f))
            {
                g.DrawRectangle(pen, checkboxRect.X, checkboxRect.Y, checkboxRect.Width - 1, checkboxRect.Height - 1);
            }
            
            // Draw checkmark if checked
            if (isChecked)
            {
                using (var pen = new Pen(_theme?.PrimaryColor ?? Color.Blue, 2f))
                {
                    // Draw checkmark
                    Point[] checkPoints = new Point[]
                    {
                        new Point(checkboxRect.Left + 3, checkboxRect.Top + checkboxRect.Height / 2),
                        new Point(checkboxRect.Left + checkboxRect.Width / 2 - 1, checkboxRect.Bottom - 4),
                        new Point(checkboxRect.Right - 3, checkboxRect.Top + 3)
                    };
                    g.DrawLines(pen, checkPoints);
                }
            }
        }
        
        protected virtual void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty) return;
            // Compute hover progress-based color blending for smooth transitions
            float hoverProgress = 0f;
            try
            {
                hoverProgress = _owner.GetHoverProgress(_owner.SelectedItem == null ? null : _owner.SelectedItem); // default 0
            }
            catch
            {
                hoverProgress = 0f;
            }

            // If owner can provide hover progress for this item directly, use it
            try
            {
                hoverProgress = Math.Max(hoverProgress, _owner.GetHoverProgress(_owner.SelectedItem == null ? null : _owner.SelectedItem));
            }
            catch { }

            // If the painter supports direct detection using IsHovered flag, we still use it as fallback
            if (isHovered)
            {
                hoverProgress = Math.Max(hoverProgress, 1f);
            }

            // Base background
            Color backgroundColor = _theme?.BackgroundColor ?? Color.White;

            // Selection override
            if (isSelected)
            {
                // Use primary color but with subtle alpha blend based on hover progress for pleasing effect
                Color primary = _theme?.PrimaryColor ?? Color.LightBlue;
                backgroundColor = Color.FromArgb(20, primary.R, primary.G, primary.B);
                using (var pen = new Pen(Color.FromArgb(200, primary), 1.5f))
                {
                    g.DrawRectangle(pen, itemRect.X + 1, itemRect.Y + 1, itemRect.Width - 2, itemRect.Height - 2);
                }
            }
            else if (hoverProgress > 0f)
            {
                // Blend between base background and hover color based on progress
                Color hoverColor = _theme?.HoverBackColor ?? Color.FromArgb(230, 230, 230);
                backgroundColor = BlendColors(backgroundColor, hoverColor, hoverProgress);
            }

            using (var brush = new SolidBrush(backgroundColor))
            {
                g.FillRectangle(brush, itemRect);
            }
        }

        /// <summary>
        /// Extended DrawItemBackground with access to item context. Backwards-compatible: default calls DrawItemBackground.
        /// Painters can override DrawItemBackground or override this Ex method for better control.
        /// </summary>
        protected virtual void DrawItemBackgroundEx(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            // Respect existing painter override for background first
            try
            {
                DrawItemBackground(g, itemRect, isHovered, isSelected);
            }
            catch { /* ignore errors in painter override */ }
            // Compute hover progress using owner helper if available
            float hoverProgress = 0f;
            try
            {
                hoverProgress = (_owner != null && item != null) ? _owner.GetHoverProgress(item) : 0f;
            }
            catch { hoverProgress = 0f; }

            // Base background color for overlay calculations
            var backgroundColor = _theme?.BackgroundColor ?? Color.White;

            // If selected, ensure a selection border/overlay is present
            if (isSelected)
            {
                // Use owner-defined selection color or fallback to theme
                var selColor = (_owner.SelectionBackColor != Color.Empty) ? _owner.SelectionBackColor : (_theme?.PrimaryColor ?? Color.LightBlue);
                int alpha = Math.Max(0, Math.Min(255, _owner.SelectionOverlayAlpha > 0 ? _owner.SelectionOverlayAlpha : 90));
                using (var fillBrush = new SolidBrush(Color.FromArgb(alpha, selColor.R, selColor.G, selColor.B)))
                {
                    g.FillRectangle(fillBrush, itemRect);
                }

                // Draw selection border
                var borderColor = (_owner.SelectionBorderColor != Color.Empty) ? _owner.SelectionBorderColor : (_theme?.AccentColor ?? Color.FromArgb(40, 40, 40));
                int borderThickness = Math.Max(1, _owner.SelectionBorderThickness);
                using (var pen = new Pen(borderColor, borderThickness))
                {
                    g.DrawRectangle(pen, itemRect.X + 1, itemRect.Y + 1, itemRect.Width - 2, itemRect.Height - 2);
                }
                // Focus outline for focused item
                if (_owner.Focused && _owner.SelectedItem == item)
                {
                    var focusColor = (_owner.FocusOutlineColor != Color.Empty) ? _owner.FocusOutlineColor : (_theme?.PrimaryColor ?? Color.LightBlue);
                    int focusThickness = Math.Max(1, _owner.FocusOutlineThickness);
                    using (var penFocus = new Pen(focusColor, focusThickness))
                    {
                        // draw a rounded or simple rectangle as focus outline
                        g.DrawRectangle(penFocus, itemRect.X + 2, itemRect.Y + 2, itemRect.Width - 4, itemRect.Height - 4);
                    }
                }
            }
            else if (hoverProgress > 0f)
            {
                var hoverColor = _theme?.HoverBackColor ?? Color.FromArgb(230, 230, 230);
                // Instead of replacing painter background, paint a subtle overlay on top to preserve painter's custom drawing
                var overlayColor = BlendColors(Color.FromArgb(0, 0, 0, 0), hoverColor, hoverProgress);
                using (var brush = new SolidBrush(Color.FromArgb((int)(hoverProgress * 60), overlayColor.R, overlayColor.G, overlayColor.B)))
                {
                    g.FillRectangle(brush, itemRect);
                }
                // Return after overlay; we avoid drawing default rectangle to prevent overriding painter-specific backgrounds
                return;
            }
            
            // When no special overlay was drawn (selected handled earlier), still allow overriding painters to have drawn backgrounds
            // and we avoid re-drawing default rectangle to preserve those customizations.
        }

        /// <summary>
        /// Blends two colors by amount t (0..1)
        /// </summary>
        protected Color BlendColors(Color a, Color b, float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            int r = (int)(a.R + (b.R - a.R) * t);
            int g = (int)(a.G + (b.G - a.G) * t);
            int bl = (int)(a.B + (b.B - a.B) * t);
            int alpha = (int)(a.A + (b.A - a.A) * t);
            return Color.FromArgb(alpha, r, g, bl);
        }

        protected virtual void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty || item == null) return;

            // Draw background - use extended method by default
            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            // Calculate layout
            var padding = GetPreferredPadding();
            var contentRect = Rectangle.Inflate(itemRect, -padding.Left, -padding.Top);

            // Draw image if available
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                var imageRect = new Rectangle(contentRect.X, contentRect.Y, 32, 32);
                DrawItemImage(g, imageRect, item.ImagePath);
                contentRect.X += 36; // Adjust content rect after image
                contentRect.Width -= 36;
            }

            // Draw text
            Color textColor = isSelected ? Color.White : (_theme?.TextColor ?? Color.Black);
            DrawItemText(g, contentRect, item.Text, textColor, _owner.Font);
        }
        
        #endregion
    }
}

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

            DrawItems(g, drawingRect, items, yOffset);
        }
        
        public virtual int GetPreferredItemHeight()
        {
            return 32; // Default item height
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
        /// Draw a single list item
        /// </summary>
        protected abstract void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected);
        
        /// <summary>
        /// Draw the item background
        /// </summary>
        protected abstract void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected);
        
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
                bool isSelected = item == _owner.SelectedItem;

                // Let concrete painter draw using the row rect
                DrawItem(g, rowRect, item, isHovered, isSelected);
            }
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
        
        #endregion
    }
}

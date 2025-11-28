using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Base implementation for combo box painters
    /// Provides common functionality for all variants
    /// </summary>
    internal abstract class BaseComboBoxPainter : IComboBoxPainter
    {
        protected BeepComboBox _owner;
        protected IBeepTheme _theme;
        protected BeepComboBoxHelper _helper;
        
        public virtual void Initialize(BeepComboBox owner, IBeepTheme theme)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _theme = theme;
            _helper = owner.Helper;
        }
        
        public virtual void Paint(Graphics g, BeepComboBox owner, Rectangle drawingRect)
        {
            if (g == null || owner == null || drawingRect.Width <= 0 || drawingRect.Height <= 0)
                return;
            
            _owner = owner;
            _theme = owner._currentTheme;
            _helper = owner.Helper;
            
            // Set high-quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            
            // Get layout rectangles
            var textAreaRect = owner.GetTextAreaRect();
            var buttonRect = owner.GetDropdownButtonRect();
            var imageRect = owner.GetImageRect();
            
            // Draw in order
            DrawBackground(g, drawingRect);
         //   DrawBorder(g, drawingRect);
            DrawTextArea(g, textAreaRect);
            
            if (!imageRect.IsEmpty)
            {
                DrawLeadingImage(g, imageRect);
            }
            
            DrawText(g, textAreaRect);
            DrawDropdownButton(g, buttonRect);
        }
        
        public virtual int GetPreferredButtonWidth()
        {
            return 24; // Default button width
        }
        
        public virtual Padding GetPreferredPadding()
        {
            return new Padding(4); // Default padding
        }
        
        #region Abstract/Virtual Methods - Override in derived classes
        
        /// <summary>
        /// Draw the background of the combo box
        /// </summary>
        protected  void DrawBackground(Graphics g, Rectangle rect)
        {

        }
        
        /// <summary>
        /// Draw the border of the combo box
        /// </summary>
        protected abstract void DrawBorder(Graphics g, Rectangle rect);
        
        /// <summary>
        /// Draw the dropdown button
        /// </summary>
        protected abstract void DrawDropdownButton(Graphics g, Rectangle buttonRect);
        
        #endregion
        
        #region Common Drawing Methods
        
        protected virtual void DrawTextArea(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;
            
            // For editable mode, we might want to show a different background
            if (_owner.IsEditable && _owner.Focused)
            {
                var brush = PaintersFactory.GetSolidBrush(PathPainterHelpers.WithAlphaIfNotEmpty(_theme?.PrimaryColor ?? Color.Empty, 10));
                g.FillRectangle(brush, textAreaRect);
            }
        }
        
        protected virtual void DrawText(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;
            
            string displayText = _helper.GetDisplayText();
            if (string.IsNullOrEmpty(displayText)) return;
            
            Color textColor = _helper.GetTextColor();
            Font textFont = _owner.TextFont ?? BeepThemesManager.ToFont(_theme?.LabelFont) ?? PaintersFactory.GetFont("Segoe UI", 9f, FontStyle.Regular);
            
            // Calculate text bounds with padding
            var textBounds = textAreaRect;
            textBounds.Inflate(-4, 0);
            
            // Draw text
            TextFormatFlags flags = TextFormatFlags.Left | 
                                   TextFormatFlags.VerticalCenter | 
                                   TextFormatFlags.EndEllipsis |
                                   TextFormatFlags.NoPrefix;
            
            TextRenderer.DrawText(g, displayText, textFont, textBounds, textColor, flags);
        }
        
        protected virtual void DrawLeadingImage(Graphics g, Rectangle imageRect)
        {
            if (imageRect.IsEmpty) return;
            
            string imagePath = null;
            
            // Prioritize LeadingImagePath over LeadingIconPath
            if (!string.IsNullOrEmpty(_owner.LeadingImagePath))
            {
                imagePath = _owner.LeadingImagePath;
            }
            else if (!string.IsNullOrEmpty(_owner.LeadingIconPath))
            {
                imagePath = _owner.LeadingIconPath;
            }
            
            if (string.IsNullOrEmpty(imagePath)) return;
            
            try
            {
                // Use StyledImagePainter for consistent image rendering
                var style = BeepStyling.CurrentControlStyle;
                StyledImagePainter.Paint(g, imageRect, imagePath, style);
            }
            catch
            {
                // Fallback: draw a placeholder icon
                DrawPlaceholderIcon(g, imageRect);
            }
        }
        
        protected void DrawPlaceholderIcon(Graphics g, Rectangle iconRect)
        {
            var brush = PaintersFactory.GetSolidBrush(PathPainterHelpers.WithAlphaIfNotEmpty(_theme?.SecondaryColor ?? Color.Empty, 150));
            var smallRect = iconRect;
            smallRect.Inflate(-4, -4);
            g.FillEllipse(brush, smallRect);
        }
        
        protected void DrawDropdownArrow(Graphics g, Rectangle buttonRect, Color arrowColor)
        {
            // Draw a simple down arrow
            var arrowSize = Math.Min(buttonRect.Width, buttonRect.Height) / 3;
            var centerX = buttonRect.Left + buttonRect.Width / 2;
            var centerY = buttonRect.Top + buttonRect.Height / 2;
            
            Point[] arrowPoints = new Point[]
            {
                new Point(centerX - arrowSize, centerY - arrowSize / 2),
                new Point(centerX + arrowSize, centerY - arrowSize / 2),
                new Point(centerX, centerY + arrowSize / 2)
            };
            
            var brush = PaintersFactory.GetSolidBrush(arrowColor);
            g.FillPolygon(brush, arrowPoints);
        }
        
        #endregion
    }
}

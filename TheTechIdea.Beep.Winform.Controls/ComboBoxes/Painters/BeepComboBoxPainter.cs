using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Painter for BeepComboBox following the modern painter methodology
    /// Uses StyledImagePainter for images and integrates with BaseControl's drawing system
    /// </summary>
    internal class BeepComboBoxPainter
    {
        private BeepComboBox _owner;
        private IBeepTheme _theme;
        private BeepComboBoxHelper _helper;
        
        public void Initialize(BeepComboBox owner, IBeepTheme theme)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _theme = theme;
            _helper = owner.Helper;
        }
        
        /// <summary>
        /// Main paint method - draws the combo box content
        /// </summary>
        public void Paint(Graphics g, BeepComboBox owner, Rectangle drawingRect)
        {
            if (g == null || owner == null || drawingRect.Width <=0 || drawingRect.Height <=0)
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
            
            // Draw the main combo box content area
            DrawTextArea(g, textAreaRect);
            
            // Draw leading image if exists
            if (!imageRect.IsEmpty)
            {
                DrawLeadingImage(g, imageRect);
            }
            
            // Draw text or placeholder
            DrawText(g, textAreaRect);
            
            // Draw dropdown button
            DrawDropdownButton(g, buttonRect);
        }
        
        #region Text Area Drawing
        
        private void DrawTextArea(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;
            
            // The background is already drawn by BaseControl's painter
            // We just need to draw any text-area-specific decorations here if needed
            
            // For editable mode, we might want to show a different background
            if (_owner.IsEditable && _owner.Focused)
            {
                var brush = PaintersFactory.GetSolidBrush(PathPainterHelpers.WithAlphaIfNotEmpty(_theme?.PrimaryColor ?? Color.Empty, 10));
                g.FillRectangle(brush, textAreaRect);
            }
        }
        
        #endregion
        
        #region Text Drawing
        
        private void DrawText(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;
            
            string displayText = _helper.GetDisplayText();
            if (string.IsNullOrEmpty(displayText)) return;
            
            Color textColor = _helper.GetTextColor();
            Font textFont = _owner.TextFont;
            
            // Calculate text bounds with padding
            var textBounds = textAreaRect;
            textBounds.Inflate(-4,0);
            
            // Draw text
            TextFormatFlags flags = TextFormatFlags.Left |
                                   TextFormatFlags.VerticalCenter |
                                   TextFormatFlags.EndEllipsis |
                                   TextFormatFlags.NoPrefix;
            
            TextRenderer.DrawText(g, displayText, textFont, textBounds, textColor, flags);
        }
        
        #endregion
        
        #region Image Drawing
        
        private void DrawLeadingImage(Graphics g, Rectangle imageRect)
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
        
        private void DrawPlaceholderIcon(Graphics g, Rectangle iconRect)
        {
            var pen = PaintersFactory.GetPen(_theme?.BorderColor ?? Color.Gray,1.5f);
            g.DrawRectangle(pen, Rectangle.Inflate(iconRect, -3, -3));
        }
        
        #endregion
        
        #region Dropdown Button Drawing
        
        private void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;
            
            // Check if button is hovered (from owner's state)
            bool isHovered = _owner.HitTestControl != null &&
                           _owner.HitTestControl.Name == "DropdownButton" &&
                           _owner.HitTestControl.IsHovered;
            
            // Draw button background on hover
            if (isHovered)
            {
                Color hoverColor = _helper.GetButtonHoverColor();
                var brush = PaintersFactory.GetSolidBrush(hoverColor);
                using (var path = CreateRoundedPath(buttonRect, _owner.BorderRadius /2))
                {
                    g.FillPath(brush, path);
                }
            }
            
            // Draw dropdown arrow icon
            DrawDropdownArrow(g, buttonRect);
        }
        
        private void DrawDropdownArrow(Graphics g, Rectangle buttonRect)
        {
            // Try to use dropdown icon path first
            if (!string.IsNullOrEmpty(_owner.DropdownIconPath))
            {
                try
                {
                    // Use StyledImagePainter for dropdown icon
                    var style = BeepStyling.CurrentControlStyle;
                    var iconRect = GetCenteredIconRect(buttonRect,12,12);
                    StyledImagePainter.Paint(g, iconRect, _owner.DropdownIconPath, style);
                    return;
                }
                catch { }
            }
            
            // Fallback: draw a simple arrow
            DrawSimpleArrow(g, buttonRect, false);
        }
        
        #endregion
        
        #region Helper Methods
        
        private GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            
            if (radius <=0)
            {
                path.AddRectangle(rect);
                return path;
            }
            
            int diameter = radius *2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(rect.Location, size);
            
            // Top left
            path.AddArc(arc,180,90);
            
            // Top right
            arc.X = rect.Right - diameter;
            path.AddArc(arc,270,90);
            
            // Bottom right
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc,0,90);
            
            // Bottom left
            arc.X = rect.Left;
            path.AddArc(arc,90,90);
            
            path.CloseFigure();
            return path;
        }
        
        private Rectangle GetCenteredIconRect(Rectangle bounds, int iconWidth, int iconHeight)
        {
            int x = bounds.X + (bounds.Width - iconWidth) /2;
            int y = bounds.Y + (bounds.Height - iconHeight) /2;
            
            return new Rectangle(x, y, iconWidth, iconHeight);
        }
        
        private void DrawSimpleArrow(Graphics g, Rectangle buttonRect, bool isOpen)
        {
            Color arrowColor = _owner.Enabled ?
                              (_theme?.ForeColor ?? Color.Black) :
                              Color.Gray;
            
            var pen = PaintersFactory.GetPen(arrowColor,2f);
            // Pen caps set on clone if necessary; small stroke without modification is fine
            
            // Calculate arrow points
            int centerX = buttonRect.X + buttonRect.Width /2;
            int centerY = buttonRect.Y + buttonRect.Height /2;
            int arrowSize =4;
            
            Point[] arrowPoints;
            
            if (isOpen)
            {
                // Up arrow
                arrowPoints = new Point[]
                {
                    new Point(centerX - arrowSize, centerY + arrowSize /2),
                    new Point(centerX, centerY - arrowSize /2),
                    new Point(centerX + arrowSize, centerY + arrowSize /2)
                };
            }
            else
            {
                // Down arrow
                arrowPoints = new Point[]
                {
                    new Point(centerX - arrowSize, centerY - arrowSize /2),
                    new Point(centerX, centerY + arrowSize /2),
                    new Point(centerX + arrowSize, centerY - arrowSize /2)
                };
            }
            
            g.DrawLines(pen, arrowPoints);
        }
        
        #endregion
    }
}

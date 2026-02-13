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

        protected int ScaleX(int logicalPixels) => _owner?.ScaleLogicalX(logicalPixels) ?? logicalPixels;
        protected int ScaleY(int logicalPixels) => _owner?.ScaleLogicalY(logicalPixels) ?? logicalPixels;
        
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
        /// Draw the dropdown button with separator line and state-aware arrow.
        /// Override in derived classes to customize (e.g., skip separator).
        /// </summary>
        protected virtual void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;

            // Draw subtle separator line between text area and button
            if (ShowButtonSeparator)
            {
                int separatorMargin = ScaleY(6);
                Color separatorColor = Color.FromArgb(100, _theme?.BorderColor ?? Color.Gray);
                var sepPen = PaintersFactory.GetPen(separatorColor, 1f);
                g.DrawLine(sepPen, buttonRect.Left, buttonRect.Top + separatorMargin,
                           buttonRect.Left, buttonRect.Bottom - separatorMargin);
            }

            // Draw arrow with state-aware coloring
            Color arrowColor = GetArrowColor();
            DrawDropdownArrow(g, buttonRect, arrowColor);
        }

        /// <summary>
        /// Whether to draw a vertical separator line between text and dropdown button.
        /// Override to return false in painters that don't want a separator (Rounded, Filled, Borderless, etc.).
        /// </summary>
        protected virtual bool ShowButtonSeparator => true;
        
        #endregion
        
        #region Common Drawing Methods
        
        /// <summary>
        /// Returns the correct arrow color based on control state (disabled, focused, hovered, normal).
        /// </summary>
        protected virtual Color GetArrowColor()
        {
            if (!_owner.Enabled)
            {
                Color disabledBase = _theme?.ForeColor ?? Color.Gray;
                return Color.FromArgb(115, disabledBase.R, disabledBase.G, disabledBase.B);
            }
            if (_owner.Focused)
            {
                return _theme?.ComboBoxHoverBorderColor != Color.Empty
                    ? _theme.ComboBoxHoverBorderColor
                    : (_theme?.PrimaryColor ?? Color.Black);
            }
            if (_owner.IsButtonHovered)
            {
                return _theme?.SecondaryColor ?? Color.Black;
            }
            // Normal state: slightly muted secondary color
            Color normalBase = _theme?.SecondaryColor ?? Color.Gray;
            return Color.FromArgb(180, normalBase.R, normalBase.G, normalBase.B);
        }
        
        protected virtual void DrawTextArea(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;
            
            // Use subtle overlays for hover/focus so control feels interactive without heavy repainting.
            if (_owner.IsEditable && _owner.Focused)
            {
                var brush = PaintersFactory.GetSolidBrush(PathPainterHelpers.WithAlphaIfNotEmpty(_theme?.PrimaryColor ?? Color.Empty, 10));
                g.FillRectangle(brush, textAreaRect);
            }
            else if (_owner.IsControlHovered && _owner.Enabled)
            {
                // Subtle hover overlay on the entire text area for interactive feedback
                var hoverColor = _theme?.ComboBoxHoverBackColor ?? Color.Empty;
                Color fillColor;
                if (hoverColor != Color.Empty)
                {
                    fillColor = PathPainterHelpers.WithAlphaIfNotEmpty(hoverColor, 40);
                }
                else
                {
                    // Fallback: use a faint version of the foreground color
                    fillColor = Color.FromArgb(18, _theme?.ForeColor ?? Color.Black);
                }
                var brush = PaintersFactory.GetSolidBrush(fillColor);
                g.FillRectangle(brush, textAreaRect);
            }
        }
        
        protected virtual void DrawText(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;
            
            string displayText = _helper.GetDisplayText();
            if (string.IsNullOrEmpty(displayText)) return;
            
            // Use muted color for placeholder text, normal color for selected item text
            Color textColor;
            if (_helper.IsShowingPlaceholder())
            {
                Color placeholderColor = _theme?.TextBoxPlaceholderColor ?? Color.Empty;
                if (placeholderColor != Color.Empty)
                {
                    textColor = placeholderColor;
                }
                else
                {
                    Color baseColor = _theme?.SecondaryColor ?? _theme?.ForeColor ?? Color.Gray;
                    textColor = Color.FromArgb(128, baseColor.R, baseColor.G, baseColor.B);
                }
            }
            else
            {
                textColor = _helper.GetTextColor();
            }
            
            Font textFont = _owner.TextFont ?? BeepThemesManager.ToFont(_theme?.LabelFont) ?? PaintersFactory.GetFont("Segoe UI", 9f, FontStyle.Regular);
            
            // Calculate text bounds with padding
            var textBounds = textAreaRect;
            int horizontalInset = ScaleX(6);
            textBounds = new Rectangle(
                textBounds.X + horizontalInset,
                textBounds.Y,
                Math.Max(1, textBounds.Width - (horizontalInset * 2)),
                textBounds.Height);
            
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
            smallRect.Inflate(-ScaleX(4), -ScaleY(4));
            g.FillEllipse(brush, smallRect);
        }
        
        protected void DrawDropdownArrow(Graphics g, Rectangle buttonRect, Color arrowColor)
        {
            // Draw a modern chevron with rounded caps and DPI-aware geometry.
            var arrowSize = Math.Max(ScaleX(3), Math.Min(buttonRect.Width, buttonRect.Height) / 5);
            var arrowHalfHeight = Math.Max(ScaleY(2), arrowSize / 2);
            var centerX = buttonRect.Left + buttonRect.Width / 2;
            var centerY = buttonRect.Top + buttonRect.Height / 2;

            float stroke = Math.Max(1f, _owner?.ScaleLogicalX(2) ?? 2);
            var pen = (System.Drawing.Pen)PaintersFactory.GetPen(arrowColor, stroke).Clone();
            try
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;

                g.DrawLines(
                    pen,
                    new[]
                    {
                        new Point(centerX - arrowSize, centerY - arrowHalfHeight),
                        new Point(centerX, centerY + arrowHalfHeight),
                        new Point(centerX + arrowSize, centerY - arrowHalfHeight)
                    });
            }
            finally
            {
                pen.Dispose();
            }
        }
        
        #endregion
    }
}

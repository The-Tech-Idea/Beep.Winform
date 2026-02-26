using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;
 
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters;
using TheTechIdea.Beep.Winform.Controls.Models;

using TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;



namespace TheTechIdea.Beep.Winform.Controls.Base
{
    public partial class BaseControl
    {
        // Ensure a painter instance exists and matches PainterKind
        private void EnsurePainter()
        {
            try
            {
                var desired = PainterKind;
                
                // None means NO painter - return early
                if (desired == BaseControlPainterKind.None)
                {
                    _painter = null;
                    return;
                }
                
                bool needsNew = _painter == null;
                if (!needsNew)
                {
                    // Check type mismatch
                    needsNew = desired switch
                    {
                        BaseControlPainterKind.Classic => _painter is not ClassicBaseControlPainter,
                        _ => _painter is null
                    };
                }

                if (needsNew)
                {
                    _painter = desired switch
                    {
                        BaseControlPainterKind.Classic => new ClassicBaseControlPainter(),
                        _ => new ClassicBaseControlPainter()
                    };
                }
            }
            catch { /* keep previous painter if any */ }
        }
        private void UpdatePainterFromKind()
        {
            switch (_painterKind)
            {
                case BaseControlPainterKind.None:
                    _painter = null; // No painter - control handles its own rendering
                    break;
                case BaseControlPainterKind.Classic:
                    _painter = new ClassicBaseControlPainter();
                    break;
                default:
                    // Auto defaults to Classic painter
                    _painter = new ClassicBaseControlPainter();
                    break;
            }
        }
        private void UpdateBorderPainter()
        {
            // Use BorderPainterFactory to create the painter for ANY style
            // The factory handles all styles including Terminal, Metro, Gnome, etc.
            if (_borderPainterStyle != BeepControlStyle.None)
            {
                // Factory returns null for None style, otherwise returns appropriate painter
                _currentBorderPainter = BorderPainterFactory.CreatePainter(_borderPainterStyle);
                
                // If factory returned null (unknown style), fallback to Minimal
                if (_currentBorderPainter == null)
                {
                    _currentBorderPainter = BorderPainterFactory.CreatePainter(BeepControlStyle.Minimal);
                }
                
                BorderRadius = BeepStyling.GetRadius(_borderPainterStyle);
                BorderThickness = (int)BeepStyling.GetBorderThickness(_borderPainterStyle);
                BorderColor = BeepStyling.GetBorderColor(_borderPainterStyle);
            }

            // Trigger a repaint if the border style changed
            Invalidate();
        }

        /// <summary>
        /// Invokes the appropriate static border painter based on current BorderPainterStyle
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="path">Border path to paint</param>
        /// <param name="isFocused">Whether control is focused</param>
        /// <param name="state">Current control state</param>
        protected void InvokeBorderPainter(Graphics g, GraphicsPath path, bool isFocused, ControlState state = ControlState.Normal)
        {
            if (_borderPainterStyle == BeepControlStyle.None || _currentTheme == null)
                return;

            // If the configured border width is zero for the style, do not draw border.
            float bw = TheTechIdea.Beep.Winform.Controls.Styling.Borders.StyleBorders.GetBorderWidth(_borderPainterStyle);
            if (bw <= 0f) return;

            bool useTheme = true; // Use theme colors by default

            // Use the cached painter from the factory (set in InitBorderPainter)
            // This handles ALL styles including Terminal, Metro, Gnome, etc.
            if (_currentBorderPainter != null)
            {
                _currentBorderPainter.Paint(g, path, isFocused, _borderPainterStyle, _currentTheme, useTheme, state);
            }
            else
            {
                // Fallback: get painter from factory if not cached
                var painter = BorderPainterFactory.CreatePainter(_borderPainterStyle);
                painter?.Paint(g, path, isFocused, _borderPainterStyle, _currentTheme, useTheme, state);
            }
        }

        #region Theme Methods
        public virtual void ApplyTheme()
        {
            try
            {
                if (_currentTheme == null) return;

                ForeColor = _currentTheme.ForeColor;
                BorderColor = _currentTheme.BorderColor;
                
                ShadowColor = _currentTheme.ShadowColor;
                GradientStartColor = _currentTheme.GradientStartColor;
                GradientEndColor = _currentTheme.GradientEndColor;
                BadgeForeColor= _currentTheme.BadgeForeColor;
                BadgeBackColor= _currentTheme.BadgeBackColor;
                DisabledForeColor= _currentTheme.DisabledForeColor;
                DisabledBackColor= _currentTheme.DisabledBackColor;
                DisabledBorderColor= _currentTheme.DisabledBorderColor;
     
                // Respect IsChild same as base BeepControl
                if (IsChild)
                {
                    if(Parent==null)
                    {
                        ParentBackColor = SystemColors.Control;
                    }
                    else
                        ParentBackColor = Parent.BackColor;
                    BackColor = ParentBackColor;
                }
                else
                {
                    BackColor = _currentTheme.BackColor;
                }

                // Update tooltip with new theme colors
                UpdateTooltipTheme();

                //if (ApplyThemeToChilds)
                //{
                //    foreach (Control c in Controls)
                //    {
                //        var themeProp = TypeDescriptor.GetProperties(c)["Theme"];
                //        themeProp?.SetValue(c, Theme);
                //    }
                //}

               // Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public virtual void ApplyTheme(string theme) => Theme = theme;
        public virtual void ApplyTheme(IBeepTheme theme) 
        { 
            _currentTheme = theme; 
            ApplyTheme(); 
        }

        public virtual bool SetFont()
        {
            if (_currentTheme == null) return false;

            bool retval = true;
            switch (OverrideFontSize)
            {
                case TypeStyleFontSize.None:
                    retval = false;
                    break;
                case TypeStyleFontSize.Small:
                    Font = BeepThemesManager.ToFont(_currentTheme.FontFamily, 8, FontWeight.Normal, FontStyle.Regular);
                    break;
                case TypeStyleFontSize.Medium:
                    Font = BeepThemesManager.ToFont(_currentTheme.FontFamily, 10, FontWeight.Normal, FontStyle.Regular);
                    break;
                case TypeStyleFontSize.Large:
                    Font = BeepThemesManager.ToFont(_currentTheme.FontFamily, 12, FontWeight.Normal, FontStyle.Regular);
                    break;
                case TypeStyleFontSize.ExtraLarge:
                    Font = BeepThemesManager.ToFont(_currentTheme.FontFamily, 14, FontWeight.Normal, FontStyle.Regular);
                    break;
                case TypeStyleFontSize.ExtraExtraLarge:
                    Font = BeepThemesManager.ToFont(_currentTheme.FontFamily, 16, FontWeight.Normal, FontStyle.Regular);
                    break;
                case TypeStyleFontSize.ExtraExtraExtraLarge:
                    Font = BeepThemesManager.ToFont(_currentTheme.FontFamily, 18, FontWeight.Normal, FontStyle.Regular);
                    break;
            }
            return retval;
        }

        public virtual void SafeApplyFont(Font newFont, bool preserveLocation = true)
        {
            Font = newFont;
            if (!preserveLocation)
            {
                // Allow control to reposition if needed
                PerformLayout();
            }
        }
        #endregion

        #region IBeepUIComponent Methods
        public Size GetSize() => new Size(Width, Height);

        public virtual void ShowToolTip(string title,string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            // Get the top-level form for this control
            var form = FindForm();
            if (form == null) return;
            
            _toolTip?.Show(text, this, PointToClient(MousePosition), 3000);
        }

        public virtual void HideToolTip()
        {
            _toolTip?.Hide(this);
        }

        public virtual bool ValidateData(out string message)
        {
            return _dataBinding.ValidateData(out message);
        }

        public virtual void SetValue(object value)
        {
            _oldValue = GetValue();
            _dataBinding.SetValue(value);
            IsDirty = true;
            _dataBinding.RaiseValueChanged(value);
        }

        public virtual object GetValue() => _dataBinding.GetValue();
        public virtual void ClearValue() => _dataBinding.ClearValue();
        public virtual bool HasFilterValue() => _dataBinding.HasFilterValue();
        public virtual AppFilter ToFilter() => _dataBinding.ToFilter();
     
        public virtual void Draw(Graphics graphics, Rectangle rectangle) { /* extension point */ }
        public void SuspendFormLayout() => _dataBinding.SuspendFormLayout();
        public void ResumeFormLayout() => _dataBinding.ResumeFormLayout();

        public virtual void ReceiveMouseEvent(HitTestEventArgs eventArgs)
        {
            _input.ReceiveMouseEvent(eventArgs);
        }
        public virtual void SendMouseEvent(IBeepUIComponent targetControl, MouseEventType eventType, Point screenLocation) => 
            _hitTest.SendMouseEvent(targetControl, eventType, screenLocation);

        public void SetBinding(string controlProperty, string dataSourceProperty) => 
            _dataBinding.SetBinding(controlProperty, dataSourceProperty);

        public virtual void RaiseSubmitChanges() => _dataBinding.RaiseSubmitChanges();
        #endregion

        #region Hit Testing Methods
        public void AddHitTest(ControlHitTest hitTest) => _hitTest.AddHitTest(hitTest);
        public void AddHitArea(string name, IBeepUIComponent component = null, Action hitAction = null) => 
            _hitTest.AddHitArea(name, component, hitAction);
        public void AddHitArea(string name, Rectangle rect, IBeepUIComponent component = null, Action hitAction = null) => 
            _hitTest.AddHitArea(name, rect, component, hitAction);
        public void AddHitTest(Control childControl) => _hitTest?.AddHitTest(childControl);
        public void RemoveHitTest(ControlHitTest hitTest) => _hitTest?.RemoveHitTest(hitTest);
        public void ClearHitList() => _hitTest?.ClearHitList();
        public void UpdateHitTest(ControlHitTest hitTest) => _hitTest?.UpdateHitTest(hitTest);
        public bool HitTest(Point location) => _hitTest.HitTest(location);
        public bool HitTest(Point location, out ControlHitTest hitTest) => _hitTest.HitTest(location, out hitTest);
        public bool HitTest(Rectangle rectangle, out ControlHitTest hitTest) => _hitTest.HitTest(rectangle, out hitTest);
        public bool HitTestWithMouse() => _hitTest.HitTestWithMouse();
        #endregion

        #region External Drawing Methods
        /// <summary>
        /// Adds an external drawing handler for a child control.
        /// The handler will be called by the parent when painting, allowing the child to draw on the parent's surface.
        /// </summary>
        /// <param name="child">The child control that wants to draw on the parent</param>
        /// <param name="handler">The drawing handler that matches DrawExternalHandler signature: void Handler(Graphics parentGraphics, Rectangle childBounds)</param>
        /// <param name="layer">The drawing layer (BeforeContent or AfterAll)</param>
        public void AddChildExternalDrawing(Control child, DrawExternalHandler handler, 
            DrawingLayer layer = DrawingLayer.AfterAll) =>
            _externalDrawing.AddChildExternalDrawing(child, handler, layer);

        /// <summary>
        /// Sets whether a child's external drawing should be redrawn.
        /// </summary>
        public void SetChildExternalDrawingRedraw(Control child, bool redraw) =>
            _externalDrawing.SetChildExternalDrawingRedraw(child, redraw);

        /// <summary>
        /// Clears all external drawing handlers for a specific child control.
        /// </summary>
        public void ClearChildExternalDrawing(Control child) =>
            _externalDrawing.ClearChildExternalDrawing(child);

        /// <summary>
        /// Clears all external drawing handlers for all child controls.
        /// </summary>
        public void ClearAllChildExternalDrawing() =>
            _externalDrawing.ClearAllChildExternalDrawing();

        /// <summary>
        /// Creates a badge drawing handler that can be registered with AddChildExternalDrawing.
        /// This is a helper method that creates a DrawExternalHandler for drawing badges.
        /// </summary>
        /// <param name="badgeText">The badge text to display</param>
        /// <param name="badgeBackColor">The badge background color</param>
        /// <param name="badgeForeColor">The badge foreground/text color</param>
        /// <param name="badgeFont">The badge font</param>
        /// <param name="badgeShape">The badge shape</param>
        /// <returns>A DrawExternalHandler that draws the badge</returns>
        public static DrawExternalHandler CreateBadgeDrawingHandler(string badgeText, Color badgeBackColor, Color badgeForeColor, Font badgeFont, BadgeShape badgeShape = BadgeShape.Circle)
        {
            return (Graphics parentGraphics, Rectangle childBounds) =>
            {
                if (parentGraphics == null || string.IsNullOrEmpty(badgeText)) return;
                // Use the helper's utility method to draw the badge
                // Note: We need to access the helper, but this is a static method
                // So we'll implement the drawing directly here
                const int badgeSize = 22;
                int x = childBounds.Right - badgeSize / 2;
                int y = childBounds.Top - badgeSize / 2;
                var badgeRect = new Rectangle(x, y, badgeSize, badgeSize);

                parentGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Badge background
                using (var brush = new SolidBrush(badgeBackColor))
                {
                    switch (badgeShape)
                    {
                        case BadgeShape.Circle:
                            parentGraphics.FillEllipse(brush, badgeRect);
                            break;
                        case BadgeShape.RoundedRectangle:
                            using (var path = Helpers.ControlPaintHelper.GetRoundedRectPath(badgeRect, badgeRect.Height / 4))
                                parentGraphics.FillPath(brush, path);
                            break;
                        case BadgeShape.Rectangle:
                            parentGraphics.FillRectangle(brush, badgeRect);
                            break;
                    }
                }

                // Badge text
                if (!string.IsNullOrEmpty(badgeText))
                {
                    using (var textBrush = new SolidBrush(badgeForeColor))
                    {
                        // Simple font scaling
                        Font scaledFont = badgeFont;
                        if (badgeText.Length > 2)
                        {
                            float fontSize = Math.Max(6, Math.Min(badgeRect.Height * 0.5f, badgeFont.Size));
                            scaledFont = new Font(badgeFont.FontFamily, fontSize, FontStyle.Bold);
                        }
                        using (scaledFont)
                        {
                            var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                            parentGraphics.DrawString(badgeText, scaledFont, textBrush, badgeRect, fmt);
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Creates a label and helper/error drawing handler that can be registered with AddChildExternalDrawing.
        /// This is a helper method that creates a DrawExternalHandler for drawing labels and error/helper text.
        /// </summary>
        /// <param name="labelText">The label text to display above the control</param>
        /// <param name="helperText">The helper text to display below the control</param>
        /// <param name="errorText">The error text to display below the control (takes precedence over helperText)</param>
        /// <param name="labelLocation">The horizontal location of the label</param>
        /// <param name="imageLocation">The location of the message image</param>
        /// <param name="messageImagePath">Path to the message image (optional)</param>
        /// <param name="showImage">Whether to show the image</param>
        /// <param name="showHelperText">Whether to show helper text</param>
        /// <param name="showErrorText">Whether to show error text</param>
        /// <param name="labelColor">The label text color</param>
        /// <param name="errorColor">The error text color</param>
        /// <param name="helperColor">The helper text color</param>
        /// <param name="font">The font to use for text</param>
        /// <returns>A DrawExternalHandler that draws the label and helper/error text</returns>
        public static DrawExternalHandler CreateLabelAndHelperDrawingHandler(
            string labelText, string helperText, string errorText,
            LabelLocation labelLocation = LabelLocation.Left,
            ImageLocation imageLocation = ImageLocation.Top,
            string messageImagePath = "",
            bool showImage = false,
            bool showHelperText = true,
            bool showErrorText = true,
            Color? labelColor = null,
            Color? errorColor = null,
            Color? helperColor = null,
            Font font = null)
        {
            return (Graphics parentGraphics, Rectangle childBounds) =>
            {
                if (parentGraphics == null) return;

                const int labelSpacing = 4;
                const int errorSpacing = 4;
                const int imageSize = 16;
                const int imageSpacing = 4;

                // Use provided font or default
                Font labelFont = font ?? new Font("Arial", 8, FontStyle.Regular);
                Font helperFont = font ?? new Font("Arial", 8, FontStyle.Regular);

                // Draw label above the control
                if (!string.IsNullOrEmpty(labelText))
                {
                    float labelSize = Math.Max(7f, labelFont.Size - 2f);
                    using var labelFontScaled = new Font(labelFont.FontFamily, labelSize, FontStyle.Regular);
                    var labelHeight = TextRenderer.MeasureText(parentGraphics, "Ag", labelFontScaled, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    
                    int labelY = childBounds.Top - labelHeight - labelSpacing;
                    TextFormatFlags labelFlags = TextFormatFlags.Left | TextFormatFlags.EndEllipsis;
                    
                    switch (labelLocation)
                    {
                        case LabelLocation.Center:
                            labelFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis;
                            break;
                        case LabelLocation.Right:
                            labelFlags = TextFormatFlags.Right | TextFormatFlags.EndEllipsis;
                            break;
                    }

                    int imageOffset = 0;
                    if (showImage && !string.IsNullOrEmpty(messageImagePath) && imageLocation == ImageLocation.Left)
                    {
                        imageOffset = imageSize + imageSpacing;
                    }

                    var labelRect = new Rectangle(
                        childBounds.Left + imageOffset,
                        labelY,
                        Math.Max(10, childBounds.Width - imageOffset),
                        labelHeight
                    );

                    Color lblColor = labelColor ?? Color.Black;
                    TextRenderer.DrawText(parentGraphics, labelText, labelFontScaled, labelRect, lblColor, labelFlags);

                    // Draw message image if provided
                    if (showImage && !string.IsNullOrEmpty(messageImagePath))
                    {
                        Rectangle imageRect = Rectangle.Empty;
                        switch (imageLocation)
                        {
                            case ImageLocation.Top:
                                imageRect = new Rectangle(childBounds.Left, labelY - imageSize / 2, imageSize, imageSize);
                                break;
                            case ImageLocation.Left:
                                imageRect = new Rectangle(childBounds.Left, labelY + (labelHeight - imageSize) / 2, imageSize, imageSize);
                                break;
                            case ImageLocation.Right:
                                imageRect = new Rectangle(childBounds.Right - imageSize, labelY + (labelHeight - imageSize) / 2, imageSize, imageSize);
                                break;
                        }

                        if (!imageRect.IsEmpty)
                        {
                            try
                            {
                                Styling.ImagePainters.StyledImagePainter.Paint(parentGraphics, imageRect, messageImagePath);
                            }
                            catch { }
                        }
                    }
                }

                // Draw helper or error text below the control
                if (showHelperText || showErrorText)
                {
                    string supporting = !string.IsNullOrEmpty(errorText) ? errorText : helperText;
                    if (!string.IsNullOrEmpty(supporting))
                    {
                        float supSize = Math.Max(7f, helperFont.Size - 2f);
                        using var supportFont = new Font(helperFont.FontFamily, supSize, FontStyle.Regular);
                        var supportHeight = TextRenderer.MeasureText(parentGraphics, "Ag", supportFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                        
                        int supportY = childBounds.Bottom + errorSpacing;
                        var supportRect = new Rectangle(
                            childBounds.Left + 6,
                            supportY,
                            Math.Max(10, childBounds.Width - 12),
                            supportHeight
                        );
                        
                        Color supportColor = !string.IsNullOrEmpty(errorText) 
                            ? (errorColor ?? Color.Red)
                            : (helperColor ?? Color.Gray);
                        TextRenderer.DrawText(parentGraphics, supporting, supportFont, supportRect, supportColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

                        // Draw message image below if imageLocation is Bottom
                        if (showImage && !string.IsNullOrEmpty(messageImagePath) && imageLocation == ImageLocation.Bottom)
                        {
                            Rectangle imageRect = new Rectangle(
                                childBounds.Left,
                                supportY + (supportHeight - imageSize) / 2,
                                imageSize,
                                imageSize
                            );
                            try
                            {
                                Styling.ImagePainters.StyledImagePainter.Paint(parentGraphics, imageRect, messageImagePath);
                            }
                            catch { }
                        }
                    }
                }
            };
        }
        #endregion

        #region Animation Methods
        public void ShowWithAnimation(DisplayAnimationType animationType, Control parentControl = null) => 
            _input.ShowWithAnimation(animationType, parentControl);
        public void ShowWithDropdownAnimation(Control parentControl = null) => 
            _input.ShowWithDropdownAnimation(parentControl);
        public void StopAnimation() => _input.StopAnimation();
        public void StartRippleEffect(Point center) => _input.StartRippleEffect(center);
        #endregion
        #region Control State Management
        protected virtual void ShowToolTipIfExists()
        {
            if (!string.IsNullOrEmpty(ToolTipText))
            {
                _toolTip?.Show(ToolTipText, this, PointToClient(MousePosition), 3000);
            }
        }
       

        #region Image and Size Utility Methods - .NET 8/9+ Simplified
        // REMOVED: Manual scaling methods that used ControlDpiHelper
        // The framework now handles DPI scaling automatically.
        // If you need image scaling, use GetScaledImageSize() from BaseControl.Properties.cs
        // which uses the framework's DeviceDpi property.
        #endregion

        public override Size GetPreferredSize(Size proposedSize)
        {
            EnsurePainter();

            // Let the active painter decide first (Classic only)
            try
            {
                Size? painterSize = _painter?.GetPreferredSize(this, proposedSize);
                if (painterSize.HasValue && !painterSize.Value.IsEmpty)
                    return painterSize.Value;
            }
            catch { /* ignore painter failures and fall through */ }

            // Measure main content using the current text and font
            Size textSize;
            var textToMeasure = string.IsNullOrEmpty(Text) ? "Ag" : Text;
            try
            {
                textSize = TextRenderer.MeasureText(textToMeasure, TextFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
            }
            catch
            {
                textSize = new Size(Math.Max(60, TextFont.Height * 2), TextFont.Height);
            }

            // Measure label
            int labelWidth = 0, labelHeight = 0;
            if (LabelTextOn && !string.IsNullOrEmpty(LabelText))
            {
                float labelSize = Math.Max(8f, Font.Size - 1f);
                try
                {
                    using var lf = new Font(Font.FontFamily, labelSize, FontStyle.Regular);
                    var lbl = TextRenderer.MeasureText(LabelText, lf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                    labelWidth = lbl.Width;
                    labelHeight = lbl.Height;
                }
                catch
                {
                    labelHeight = (int)Math.Ceiling(labelSize);
                }
            }

            // Measure helper/error text (prioritize error)
            int supportWidth = 0, supportHeight = 0;
            string supporting = HasError && !string.IsNullOrEmpty(ErrorText)
                ? ErrorText
                : (HelperTextOn ? HelperText : string.Empty);
            if (!string.IsNullOrEmpty(supporting))
            {
                float supSize = Math.Max(8f, Font.Size - 1f);
                try
                {
                    using var sf = new Font(Font.FontFamily, supSize, FontStyle.Regular);
                    var sup = TextRenderer.MeasureText(supporting, sf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                    supportWidth = sup.Width;
                    supportHeight = sup.Height;
                }
                catch
                {
                    supportHeight = (int)Math.Ceiling(supSize);
                }
            }

            // Icon contribution
            int iconWidth = 0, iconHeight = 0;
            bool hasLeading = !string.IsNullOrEmpty(LeadingIconPath) || !string.IsNullOrEmpty(LeadingImagePath);
            bool hasTrailing = !string.IsNullOrEmpty(TrailingIconPath) || !string.IsNullOrEmpty(TrailingImagePath) || ShowClearButton;
            if (hasLeading)
            {
                iconWidth += IconSize + (IconPadding * 2) + 8;
                iconHeight = Math.Max(iconHeight, IconSize);
            }
            if (hasTrailing)
            {
                iconWidth += IconSize + (IconPadding * 2) + 8;
                iconHeight = Math.Max(iconHeight, IconSize);
            }

            int border = 0;
            if (ShowAllBorders || (BorderThickness > 0 && (ShowTopBorder || ShowBottomBorder || ShowLeftBorder || ShowRightBorder)))
                border = BorderThickness * 2;
            int shadow = ShowShadow ? ShadowOffset * 2 : 0;
            var pad = Padding;

            int contentWidth = textSize.Width + iconWidth;
            int contentHeight = Math.Max(textSize.Height, iconHeight);

            int width = contentWidth + pad.Horizontal + border + shadow;
            width = Math.Max(width, labelWidth + pad.Horizontal + border + shadow);
            width = Math.Max(width, supportWidth + pad.Horizontal + border + shadow);

            int height = contentHeight + pad.Vertical + border + shadow;
            if (labelHeight > 0) height += labelHeight + 2;
            if (supportHeight > 0) height += supportHeight + 2;

            // Respect proposed size when provided; otherwise don't shrink current size
            width = proposedSize.Width > 0 ? Math.Max(width, proposedSize.Width) : Math.Max(width, Width);
            height = proposedSize.Height > 0 ? Math.Max(height, proposedSize.Height) : Math.Max(height, Height);

            width = Math.Max(60, width);
            height = Math.Max(TextFont.Height + pad.Vertical + border, height);

            return new Size(width, height);
        }

        /// <summary>
        /// Paints the inner shape of the control (content area excluding borders)
        /// </summary>
        /// <param name="g">Graphics context to draw on</param>
        /// <param name="fillColor">Color to fill the inner area with</param>
        protected virtual void PaintInnerShape(Graphics g, Color fillColor)
        {
            // Painters own layout; do not rely on ControlPaintHelper rectangles here

            // Use the InnerShape if available, otherwise fallback to InnerArea rectangle
            if (InnerShape != null && InnerShape.PointCount > 0)
            {
                // Paint using the shape path (handles complex borders like Material Design)
                PaintInnerShapeUsingPath(g, InnerShape, fillColor);
            }
            else
            {
                // Fallback to rectangle-based painting using DrawingRect
                Rectangle innerArea = this.DrawingRect;
                if (innerArea.Width > 0 && innerArea.Height > 0)
                {
                    using (Brush backBrush = new SolidBrush(fillColor))
                    {
                        if (IsRounded && BorderRadius > 0)
                        {
                            // For rounded controls, clip the fill to the rounded inner area
                            int innerRadius = Math.Max(0, BorderRadius - BorderThickness);
                            using (var innerPath = ControlPaintHelper.GetRoundedRectPath(innerArea, innerRadius))
                            {
                                g.FillPath(backBrush, innerPath);
                            }
                        }
                        else
                        {
                            g.FillRectangle(backBrush, innerArea);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Paints the inner area using a GraphicsPath shape
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="innerShape">The shape path to fill</param>
        /// <param name="fillColor">Color to fill with</param>
        protected virtual void PaintInnerShapeUsingPath(Graphics g, GraphicsPath innerShape, Color fillColor)
        {
            if (innerShape == null || innerShape.PointCount == 0)
                return;

            // Standard fill for classic rendering
            if (fillColor.A == 0)
                return;

            using (Brush backBrush = new SolidBrush(fillColor))
            {
                g.FillPath(backBrush, innerShape);
            }
        }

        protected virtual void DrawContent(Graphics g)
        {
         /*    if (EnableHighQualityRendering)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            }
            else
            {
                g.SmoothingMode = SmoothingMode.None;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Default;
                g.TextRenderingHint = TextRenderingHint.SystemDefault;
            } */
            UpdateDrawingRect();
                

                try
                {
                    if (IsChild)
                    {
                        ParentBackColor = Parent?.BackColor ?? SystemColors.Control;
                        BackColor = ParentBackColor;
                    }
                    else
                    {
                        if (_currentTheme != null)
                            BackColor = _currentTheme?.BackColor ?? SystemColors.Control;
                        else
                            BackColor = SystemColors.Control;
                    }
                    
                    // Background is now painted in OnPaintBackground, not here
                    // This prevents double-painting which caused darkening on hover
                }
                catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
                {
                    // Silently fail on color operations
                    System.Diagnostics.Debug.WriteLine($"BaseControl.OnPaint color error: {ex.Message}");
                }
            

           

            // Always rely on painter if available
            EnsurePainter();
            
            // When PainterKind is None, painter is null - derived controls handle their own drawing
            if (_painter != null)
            {
                _painter.UpdateLayout(this);

                // Expose painter inner rect to derived controls via BaseControl.DrawingRect
                try { this.DrawingRect = _painter.DrawingRect; } catch { }

                _painter.Paint(g, this);

                // Let painter register hit areas; wire actions when available
                _painter.UpdateHitAreas(this, (name, rect, action) => _hitTest?.AddHitArea(name, rect, null, action));
            }

           
        }

        /// <summary>
        /// Triggers Material Design size compensation when properties change
        /// </summary>
        protected virtual void OnMaterialPropertyChanged()
        {
            // Material painter removed; keep painter consistent with classic path.
            if (_painter == null && PainterKind != BaseControlPainterKind.None)
                _painter = new ClassicBaseControlPainter();
        }

        /// <summary>
        /// Updates the drawing rectangle based on current painter layout
        /// This is a critical method used by many derived controls
        /// </summary>
        public virtual void UpdateDrawingRect()
        {
            EnsurePainter();
            
            // When PainterKind is None, use the full ClientRectangle
            if (_painter == null)
            {
                _drawingRect = ClientRectangle;
                return;
            }

            // Compute layout once and assign rect without forcing redraw
            _painter.UpdateLayout(this);
            var newRect = _painter.DrawingRect;
            if (newRect != _drawingRect)
            {
                _drawingRect = newRect;
            }
        }
        #endregion

        #region React UI Helper Methods (from BeepControl)
      

     
        #endregion

        #region Material Ripple Effects (from BeepControl)
        public void StartMaterialRipple(Point clickPosition)
        {
            _effects.StartMaterialRipple(clickPosition);
        }
        #endregion

        #region Badge Methods (from BeepControl)
        public void UpdateRegionForBadge()
        {
           
            if (Parent is IExternalDrawingProvider externalDrawingProvider && Parent is Control parentControl)
            {
                const int badgeSize = 22;
                int badgeX = Bounds.Right - badgeSize / 2;
                int badgeY = Bounds.Top - badgeSize / 2;
                Rectangle badgeAreaOnParent = new Rectangle(badgeX, badgeY, badgeSize, badgeSize);
                parentControl.Invalidate(badgeAreaOnParent);
            }
            UpdateControlRegion();
        }
        #endregion

        #region Validation and Data Handling
        public virtual bool HasValidationErrors()
        {
            return !ValidateData(out _);
        }

        public virtual void ResetValidation()
        {
            // Reset any validation state
            IsValid = true;
            Invalidate();
        }

        public virtual void SetValidationError(string message)
        {
            IsValid = false;
            HelperText = message;
            Invalidate();
        }
        #endregion

        #region Child Control Management (from BeepControl)
        protected virtual void ApplyThemeToControl(Control control)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["Theme"];
            themeProperty?.SetValue(control, Theme);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (ApplyThemeToChilds && !string.IsNullOrEmpty(Theme))
            {
                ApplyThemeToControl(e.Control);
            }
        }
        #endregion

        #region Material compatibility helpers
        /// <summary>
        /// Returns padding to use for material-like sizing. With material painter removed, this defers to current Padding.
        /// </summary>
        public virtual Padding GetMaterialStylePadding() => Padding;

        /// <summary>
        /// Placeholder for material effects spacing (focus/elevation). Classic mode returns empty.
        /// </summary>
        public virtual Size GetMaterialEffectsSpace() => Size.Empty;

        /// <summary>
        /// Computes the space taken by leading/trailing icons.
        /// </summary>
        public virtual Size GetMaterialIconSpace()
        {
            int totalIconWidth = 0;
            int iconHeight = 0;

            if (!string.IsNullOrEmpty(LeadingIconPath) || !string.IsNullOrEmpty(LeadingImagePath))
            {
                totalIconWidth += IconSize + (IconPadding * 2) + 8;
                iconHeight = Math.Max(iconHeight, IconSize);
            }

            if (!string.IsNullOrEmpty(TrailingIconPath) || !string.IsNullOrEmpty(TrailingImagePath) || ShowClearButton)
            {
                totalIconWidth += IconSize + (IconPadding * 2) + 8;
                iconHeight = Math.Max(iconHeight, IconSize);
            }

            return new Size(totalIconWidth, iconHeight);
        }

        /// <summary>
        /// Returns a conservative minimum size using classic padding/borders but keeping icon space.
        /// </summary>
        public virtual Size CalculateMinimumSizeForMaterial(Size baseContentSize)
        {
            var icons = GetMaterialIconSpace();
            int border = (ShowAllBorders || (BorderThickness > 0 && (ShowTopBorder || ShowBottomBorder || ShowLeftBorder || ShowRightBorder)))
                ? BorderThickness * 2
                : 0;
            var pad = Padding;

            int width = baseContentSize.Width + icons.Width + pad.Horizontal + border;
            int height = Math.Max(baseContentSize.Height, icons.Height) + pad.Vertical + border;

            width = Math.Max(width, GetMaterialMinimumWidth());
            height = Math.Max(height, GetMaterialMinimumHeight());

            return new Size(width, height);
        }

        /// <summary>
        /// Returns the best-effort content rectangle. Falls back to the current drawing rect when painter is classic.
        /// </summary>
        public virtual Rectangle GetMaterialContentRectangle()
        {
            EnsurePainter();
            _painter?.UpdateLayout(this);

            if (_painter != null && !_painter.ContentRect.IsEmpty)
                return _painter.ContentRect;

            if (_painter != null && !_painter.DrawingRect.IsEmpty)
                return _painter.DrawingRect;

            return DrawingRect;
        }

        /// <summary>
        /// Adjusts control size using classic measurements; kept for compatibility with derived controls.
        /// </summary>
        public virtual void AdjustSizeForMaterial(Size baseContentSize, bool respectMaximumSize = true)
        {
            var requiredSize = CalculateMinimumSizeForMaterial(baseContentSize);

            if (respectMaximumSize && MaximumSize != Size.Empty)
            {
                requiredSize.Width = Math.Min(requiredSize.Width, MaximumSize.Width);
                requiredSize.Height = Math.Min(requiredSize.Height, MaximumSize.Height);
            }

            if (MinimumSize == Size.Empty)
            {
                MinimumSize = requiredSize;
            }
            else
            {
                MinimumSize = new Size(
                    Math.Max(MinimumSize.Width, requiredSize.Width),
                    Math.Max(MinimumSize.Height, requiredSize.Height));
            }

            if (Width < requiredSize.Width || Height < requiredSize.Height)
            {
                Size = new Size(
                    Math.Max(Width, requiredSize.Width),
                    Math.Max(Height, requiredSize.Height));
            }
        }

        /// <summary>
        /// Base material-like minimum width.
        /// </summary>
        protected virtual int GetMaterialMinimumWidth() => 120;

        /// <summary>
        /// Base material-like minimum height.
        /// </summary>
        protected virtual int GetMaterialMinimumHeight() => 48;

        /// <summary>
        /// Utility for derived controls: returns a minimum size including icon/padding allowances.
        /// </summary>
        protected Size GetEffectiveMaterialMinimum(Size baseContentMinimum)
        {
            return CalculateMinimumSizeForMaterial(baseContentMinimum);
        }
        #endregion

    }
}

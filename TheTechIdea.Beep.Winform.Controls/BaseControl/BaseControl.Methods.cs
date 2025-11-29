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
        public void AddChildExternalDrawing(Control child, DrawExternalHandler handler, 
            DrawingLayer layer = DrawingLayer.AfterAll) =>
            _externalDrawing.AddChildExternalDrawing(child, handler, layer);

        public void SetChildExternalDrawingRedraw(Control child, bool redraw) =>
            _externalDrawing.SetChildExternalDrawingRedraw(child, redraw);

        public void ClearChildExternalDrawing(Control child) =>
            _externalDrawing.ClearChildExternalDrawing(child);

        public void ClearAllChildExternalDrawing() =>
            _externalDrawing.ClearAllChildExternalDrawing();

        public void DrawBadgeExternally(Graphics g, Rectangle childBounds) =>
            _externalDrawing.DrawBadgeExternally(g, childBounds, BadgeText, BadgeBackColor, BadgeForeColor, BadgeFont, BadgeShape);
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
            if (EnableHighQualityRendering)
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
            }
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
           
            if (Parent is BaseControl parentBeepControl)
            {
                const int badgeSize = 22;
                int badgeX = Bounds.Right - badgeSize / 2;
                int badgeY = Bounds.Top - badgeSize / 2;
                Rectangle badgeAreaOnParent = new Rectangle(badgeX, badgeY, badgeSize, badgeSize);
                parentBeepControl.Invalidate(badgeAreaOnParent);
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

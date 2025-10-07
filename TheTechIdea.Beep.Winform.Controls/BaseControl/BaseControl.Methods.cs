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
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters;


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
                if (desired == BaseControlPainterKind.Auto)
                {
                    desired =  BaseControlPainterKind.Classic;
                }

                bool needsNew = _painter == null;
                if (!needsNew)
                {
                    // Check type mismatch
                    needsNew = desired switch
                    {
                        BaseControlPainterKind.Classic => _painter is not ClassicBaseControlPainter,
                        BaseControlPainterKind.Material => _painter is not MaterialBaseControlPainter,
                        BaseControlPainterKind.Card => _painter is not CardBaseControlPainter,
                        BaseControlPainterKind.NeoBrutalist => _painter is not NeoBrutalistBaseControlPainter,
                        BaseControlPainterKind.ReadingCard => _painter is not ReadingCardBaseControlPainter,
                        _ => _painter is null
                    };
                }

                if (needsNew)
                {
                    _painter = desired switch
                    {
                        BaseControlPainterKind.Classic => new ClassicBaseControlPainter(),
                        BaseControlPainterKind.Material => new MaterialBaseControlPainter(),
                        BaseControlPainterKind.Card => new CardBaseControlPainter(),
                        BaseControlPainterKind.NeoBrutalist => new NeoBrutalistBaseControlPainter(),
                        BaseControlPainterKind.ReadingCard => new ReadingCardBaseControlPainter(),
                        _ => new ClassicBaseControlPainter()
                    };
                }
            }
            catch { /* keep previous painter if any */ }
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

                if (ApplyThemeToChilds)
                {
                    foreach (Control c in Controls)
                    {
                        var themeProp = TypeDescriptor.GetProperties(c)["Theme"];
                        themeProp?.SetValue(c, Theme);
                    }
                }

                Invalidate();
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
            if (_dpi == null || DisableDpiAndScaling)
            {
                Font = newFont;
                return;
            }
            _dpi.SafeApplyFont(newFont, preserveLocation);
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
        public void AddHitTest(Control childControl) => _hitTest.AddHitTest(childControl);
        public void RemoveHitTest(ControlHitTest hitTest) => _hitTest.RemoveHitTest(hitTest);
        public void ClearHitList() => _hitTest.ClearHitList();
        public void UpdateHitTest(ControlHitTest hitTest) => _hitTest.UpdateHitTest(hitTest);
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
       

        #region Image and Size Utility Methods (from BeepControl)
        public float GetScaleFactor(SizeF imageSize, Size targetSize)
        {
            if (_dpi == null || DisableDpiAndScaling) return 1.0f;
            return _dpi.GetScaleFactor(imageSize, targetSize, ScaleMode);
        }

        public RectangleF GetScaledBounds(SizeF imageSize, Rectangle targetRect)
        {
            if (_dpi == null || DisableDpiAndScaling) return new RectangleF(targetRect.Location, new SizeF(imageSize.Width, imageSize.Height));
            return _dpi.GetScaledBounds(imageSize, targetRect, ScaleMode);
        }

        public RectangleF GetScaledBounds(SizeF imageSize)
        {
            if (_dpi == null || DisableDpiAndScaling) return new RectangleF(Point.Empty, new SizeF(imageSize.Width, imageSize.Height));
            return _dpi.GetScaledBounds(imageSize, ScaleMode);
        }

        public Size GetSuitableSizeForTextAndImage(Size imageSize, Size maxImageSize, TextImageRelation textImageRelation)
        {
            if (_dpi == null || DisableDpiAndScaling) return maxImageSize;
            return _dpi.GetSuitableSizeForTextAndImage(imageSize, maxImageSize, textImageRelation);
        }

        // Parity alias for exact name as in base
        public Size GetSuitableSizeForTextandImage(Size imageSize, Size MaxImageSize, TextImageRelation TextImageRelation)
        {
            return GetSuitableSizeForTextAndImage(imageSize, MaxImageSize, TextImageRelation);
        }

        public Font GetScaledFont(Graphics g, string text, Size maxSize, Font originalFont)
        {
            if (_dpi == null || DisableDpiAndScaling) return originalFont;
            return _dpi.GetScaledFont(g, text, maxSize, originalFont);
        }
        #endregion

        public override Size GetPreferredSize(Size proposedSize)
        {
            EnsurePainter();

            // Let the active painter decide first
            if (_painter != null)
            {
                try
                {
                    var painterSize = _painter.GetPreferredSize(this, proposedSize);
                    if (!painterSize.IsEmpty)
                        return painterSize;
                }
                catch { /* fallback to existing logic */ }
            }

            if (PainterKind== BaseControlPainterKind.Material)
            {
                int width = Math.Max(1, proposedSize.Width <= 0 ? Width : proposedSize.Width);
                int height = Math.Max(1, proposedSize.Height <= 0 ? Height : proposedSize.Height);

                int baseMin = GetMaterialMinimumHeight();

                int extraTop = 0;
                int extraBottom = 0;
                try
                {
                    using var g = CreateGraphics();
                    if (!string.IsNullOrEmpty(LabelText))
                    {
                        int lblH = TextRenderer.MeasureText(g, "Ag", Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                        extraTop = Math.Max(10, (int)Math.Ceiling(lblH * 0.75)) + 2;
                    }
                    string supporting = !string.IsNullOrEmpty(ErrorText) ? ErrorText : HelperText;
                    if (!string.IsNullOrEmpty(supporting))
                    {
                        using var supportFont = new Font(Font.FontFamily, Math.Max(8f, Font.Size - 1f));
                        int supH = TextRenderer.MeasureText(g, "Ag", supportFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                        extraBottom = supH + 4;
                    }
                }
                catch { }

                int requiredH = baseMin + extraTop + extraBottom;
                var effects = GetMaterialEffectsSpace();
                requiredH += effects.Height;
                requiredH = Math.Max(requiredH, Height);

                return new Size(Math.Max(Width, width), requiredH);
            }

            // Classic fallback: make a conservative estimate using padding and borders
            int border = ShowAllBorders ? BorderThickness * 2 : 0;
            var pad = Padding;
            int minW = Math.Max(Width, pad.Horizontal + border + Math.Max(60, Font.Height * 3));
            int minH = Math.Max(Height, pad.Vertical + border + Math.Max(24, Font.Height + 8));
            return new Size(minW, minH);
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
            // For Material Design, respect variant-specific fill rules
            if (PainterKind== BaseControlPainterKind.Material)
            {
                bool shouldShowFill = MaterialVariant == MaterialTextFieldVariant.Filled || MaterialShowFill;
                
                if (shouldShowFill)
                {
                    // Use Material fill color if specified, otherwise use the provided fillColor
                    Color materialFillColor = MaterialShowFill ? MaterialFillColor : fillColor;
                    using (Brush backBrush = new SolidBrush(materialFillColor))
                    {
                        g.FillPath(backBrush, innerShape);
                    }
                }
                else
                {
                    // For Outlined and Standard variants, only fill if color is different from parent
                    if (fillColor != Color.Transparent && fillColor != Parent?.BackColor)
                    {
                        using (Brush backBrush = new SolidBrush(fillColor))
                        {
                            g.FillPath(backBrush, innerShape);
                        }
                    }
                }
            }
            else
            {
                // Standard controls - always fill the shape
                using (Brush backBrush = new SolidBrush(fillColor))
                {
                    g.FillPath(backBrush, innerShape);
                }
            }
        }

        protected virtual void DrawContent(Graphics g)
        {
            // Ensure DPI info stays updated when drawing on new monitor contexts
            UpdateDpiScaling(g);

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

            // Always rely on painter
            EnsurePainter();
            _painter.UpdateLayout(this);

            // Expose painter inner rect to derived controls via BaseControl.DrawingRect
            try { this.DrawingRect = _painter.DrawingRect; } catch { }

            _painter.Paint(g, this);

            // Let painter register hit areas; wire actions when available
            _painter.UpdateHitAreas(this, (name, rect, action) => _hitTest?.AddHitArea(name, rect, null, action));

           
        }

        /// <summary>
        /// Triggers Material Design size compensation when properties change
        /// </summary>
        protected virtual void OnMaterialPropertyChanged()
        {
            if (PainterKind== BaseControlPainterKind.Material && MaterialAutoSizeCompensation && !_isInitializing)
            {
                ApplyMaterialSizeCompensation();
            }

            // Keep painter strategy in sync if painter auto-select is used
            if (_painter != null)
            {
                // Auto-switch painter when Material toggle changes
                _painter = PainterKind == BaseControlPainterKind.Material ? new MaterialBaseControlPainter() : new ClassicBaseControlPainter();
            }
        }

        /// <summary>
        /// Updates the drawing rectangle based on current painter layout
        /// This is a critical method used by many derived controls
        /// </summary>
        public virtual void UpdateDrawingRect()
        {
            EnsurePainter();
            if (_painter == null) return;

            // Compute layout once and assign rect without forcing redraw
            _painter.UpdateLayout(this);
            var newRect = _painter.DrawingRect;
            if (newRect != _drawingRect)
            {
                _drawingRect = newRect;
            }
        }

        /// <summary>
        /// Applies Material Design size compensation - can be overridden by derived controls
        /// </summary>
        public virtual void ApplyMaterialSizeCompensation()
        {
            if (PainterKind != BaseControlPainterKind.Material || !MaterialAutoSizeCompensation)
                return;

            // Calculate current text size if we have content
            Size textSize = Size.Empty;
            if (!string.IsNullOrEmpty(Text))
            {
                // Use TextRenderer to measure without creating a Graphics
                var measuredSize = System.Windows.Forms.TextRenderer.MeasureText(Text, Font);
                textSize = measuredSize;
            }
            
            // Use a reasonable default content size if no text
            if (textSize.IsEmpty)
            {
                textSize = new Size(100, 20);
            }
            
            // Apply Material size compensation
            AdjustSizeForMaterial(textSize, true);
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

        #region Material Design Size Calculation Methods
        
        /// <summary>
        /// Gets the padding requirements for the current Material Design variant
        /// </summary>
        /// <returns>Padding structure with Material Design spacing requirements</returns>
        public virtual Padding GetMaterialStylePadding()
        {
            if (PainterKind != BaseControlPainterKind.Material)
                return Padding.Empty;

            // Material Design padding based on variant
            switch (MaterialVariant)
            {
                case MaterialTextFieldVariant.Outlined:
                    return new Padding(16, 8, 16, 8);
                case MaterialTextFieldVariant.Filled:
                    return new Padding(16, 12, 16, 12);
                case MaterialTextFieldVariant.Standard:
                    return new Padding(0, 8, 0, 8);
                default:
                    return new Padding(16, 8, 16, 8);
            }
        }

        /// <summary>
        /// Gets the additional space required for Material Design focus indicators and elevation
        /// </summary>
        /// <returns>Size of additional space needed for focus and elevation effects</returns>
        public virtual Size GetMaterialEffectsSpace()
        {
            if (PainterKind != BaseControlPainterKind.Material)
                return Size.Empty;
            int focusSpace = 4; // 2px on each side for focus ring
            // Elevation applies on both sides; use 2x to reflect left+right / top+bottom
            int elevationSpace = MaterialUseElevation ? Math.Min(MaterialElevationLevel, 5) * 2 : 0;

            return new Size(focusSpace + elevationSpace, focusSpace + elevationSpace);
        }

        /// <summary>
        /// Gets the space required for Material Design icons
        /// </summary>
        /// <returns>Size of space needed for leading and trailing icons</returns>
        public virtual Size GetMaterialIconSpace()
        {
            if (PainterKind !=  BaseControlPainterKind.Material)
                return Size.Empty;

            int totalIconWidth = 0;
            int iconHeight = 0;

            // Calculate leading icon space
            if (!string.IsNullOrEmpty(LeadingIconPath) || !string.IsNullOrEmpty(LeadingImagePath))
            {
                totalIconWidth += IconSize + (IconPadding * 2) + 8; // icon + padding + margin
                iconHeight = Math.Max(iconHeight, IconSize);
            }

            // Calculate trailing icon space
            if (!string.IsNullOrEmpty(TrailingIconPath) || !string.IsNullOrEmpty(TrailingImagePath) || ShowClearButton)
            {
                totalIconWidth += IconSize + (IconPadding * 2) + 8; // icon + padding + margin
                iconHeight = Math.Max(iconHeight, IconSize);
            }

            return new Size(totalIconWidth, iconHeight);
        }

        /// <summary>
        /// Calculates the minimum size required for Material Design styling
        /// </summary>
        /// <param name="baseContentSize">The base content size (e.g., text size)</param>
        /// <returns>Minimum size that accommodates Material Design requirements</returns>
        public virtual Size CalculateMinimumSizeForMaterial(Size baseContentSize)
        {
            if (PainterKind != BaseControlPainterKind.Material)
                
            return baseContentSize;

            if (MaterialPreserveContentArea)
            {
                // Alternative approach: Keep content area the same, adjust only internal layout
                return CalculateSizePreservingContentArea(baseContentSize);
            }
            else
            {
                // Standard Material Design approach: Follow Material Design size specifications
                return CalculateStandardMaterialSize(baseContentSize);
            }
        }

        /// <summary>
        /// Calculates size while preserving the original content area dimensions
        /// </summary>
        protected virtual Size CalculateSizePreservingContentArea(Size baseContentSize)
        {
            // In this mode, we keep the overall control size the same as non-Material
            // but internally adjust how we draw the content within the available space
            
            // Add minimal adjustments for essential Material elements only
            var iconSpace = GetMaterialIconSpace();
            var minEffects = new Size(4, 4); // Minimal space for focus indicators
            
            return new Size(
                Math.Max(baseContentSize.Width + iconSpace.Width + minEffects.Width, 120),
                Math.Max(baseContentSize.Height + minEffects.Height, 24)
            );
        }

        /// <summary>
        /// Calculates size following standard Material Design specifications
        /// </summary>
        protected virtual Size CalculateStandardMaterialSize(Size baseContentSize)
        {
            // Original implementation - follows Material Design specs
            var materialPadding = GetMaterialStylePadding();
            var effectsSpace = GetMaterialEffectsSpace();
            var iconSpace = GetMaterialIconSpace();

            int requiredWidth = baseContentSize.Width + 
                              materialPadding.Horizontal + 
                              effectsSpace.Width + 
                              iconSpace.Width;

            int requiredHeight = Math.Max(
                baseContentSize.Height + materialPadding.Vertical + effectsSpace.Height,
                iconSpace.Height + materialPadding.Vertical + effectsSpace.Height
            );

            // Apply minimum Material Design dimensions
            requiredWidth = Math.Max(requiredWidth, GetMaterialMinimumWidth());
            requiredHeight = Math.Max(requiredHeight, GetMaterialMinimumHeight());

            return new Size(requiredWidth, requiredHeight);
        }

        /// <summary>
        /// Gets the minimum width for Material Design controls
        /// </summary>
        /// <returns>Minimum width in pixels</returns>
        protected virtual int GetMaterialMinimumWidth()
        {
            // Material Design minimum widths based on component type
            return 120; // Base minimum, can be overridden by derived controls
        }

        /// <summary>
        /// Gets the minimum height for Material Design controls
        /// </summary>
        /// <returns>Minimum height in pixels</returns>
        protected virtual int GetMaterialMinimumHeight()
        {
            // Material Design minimum heights based on variant
            switch (MaterialVariant)
            {
                case MaterialTextFieldVariant.Outlined:
                    return 56; // Standard Material outlined field height
                case MaterialTextFieldVariant.Filled:
                    return 56; // Standard Material filled field height
                case MaterialTextFieldVariant.Standard:
                    return 48; // Standard Material standard field height
                default:
                    return 56;
            }
        }

        /// <summary>
        /// Gets the effective content rectangle accounting for Material Design spacing
        /// </summary>
        /// <returns>Rectangle available for content after Material Design spacing is applied</returns>
        public virtual Rectangle GetMaterialContentRectangle()
        {
            if (PainterKind != BaseControlPainterKind.Material)
               
            return ClientRectangle;

            // Use painter-provided rects when available
            if (_painter != null)
            {
                var rect = _painter.ContentRect;
                if (!rect.IsEmpty)
                    return rect;
                rect = _painter.DrawingRect;
                if (!rect.IsEmpty)
                    return rect;
            }

            // Fallback calculation if painter is not set yet
            var padding = GetMaterialStylePadding();
            var effects = GetMaterialEffectsSpace();
            var icons = GetMaterialIconSpace();

            return new Rectangle(
                padding.Left + effects.Width / 2 + (icons.Width > 0 ? IconSize + IconPadding : 0),
                padding.Top + effects.Height / 2,
                Math.Max(0, Width - padding.Horizontal - effects.Width - icons.Width),
                Math.Max(0, Height - padding.Vertical - effects.Height)
            );
        }

        /// <summary>
        /// Adjusts control size to accommodate Material Design requirements
        /// </summary>
        /// <param name="baseContentSize">The base content size that needs to be accommodated</param>
        /// <param name="respectMaximumSize">Whether to respect any maximum size constraints</param>
        public virtual void AdjustSizeForMaterial(Size baseContentSize, bool respectMaximumSize = true)
        {
            if (PainterKind != BaseControlPainterKind.Material)
               
            return;

            var requiredSize = CalculateMinimumSizeForMaterial(baseContentSize);
            
            // Apply DPI scaling
            requiredSize = ScaleSize(requiredSize);

            // Respect maximum size constraints if specified
            if (respectMaximumSize && MaximumSize != Size.Empty)
            {
                requiredSize.Width = Math.Min(requiredSize.Width, MaximumSize.Width);
                requiredSize.Height = Math.Min(requiredSize.Height, MaximumSize.Height);
            }

            // Always raise MinimumSize to the required Material size (never shrink an explicit larger MinimumSize)
            if (MinimumSize != Size.Empty)
            {
                MinimumSize = new Size(
                    Math.Max(MinimumSize.Width, requiredSize.Width),
                    Math.Max(MinimumSize.Height, requiredSize.Height)
                );
            }
            else
            {
                MinimumSize = requiredSize;
            }

            // Update control size if it's smaller than required
            if (Width < requiredSize.Width || Height < requiredSize.Height)
            {
                Size = new Size(
                    Math.Max(Width, requiredSize.Width),
                    Math.Max(Height, requiredSize.Height)
                );
            }
        }

        /// <summary>
        /// Utility for derived controls: given your base content minimum (e.g., 300x30),
        /// returns the effective minimum including Material padding and effects.
        /// </summary>
        /// <param name="baseContentMinimum">The intrinsic minimum of the control's content.</param>
        /// <returns>Minimum size including Material padding, effects and icons, DPI-scaled.</returns>
        protected Size GetEffectiveMaterialMinimum(Size baseContentMinimum)
        {
            if (PainterKind != BaseControlPainterKind.Material)
              
            return baseContentMinimum;

            var min = CalculateMinimumSizeForMaterial(baseContentMinimum);
            return ScaleSize(min);
        }


        #endregion
        protected GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            return ControlPaintHelper.GetRoundedRectPath(rect, radius);
        }
        //// Handle runtime per-monitor DPI changes
        //private const int WM_DPICHANGED = 0x02E0;
        //protected override void WndProc(ref Message m)
        //{
        //    if (m.Msg == WM_DPICHANGED)
        //    {
        //        try
        //        {
        //            if (!DisableDpiAndScaling && _dpi != null)
        //            {
        //                _dpi.UpdateDpiFromControl();
        //                // Refresh layout dependent on DPI
        //                //  ;
        //                // UpdateMaterialLayout();
        //                Invalidate();
        //            }
        //        }
        //        catch { /* best-effort */ }
        //    }
        //    base.WndProc(ref m);
        //}
    }
}
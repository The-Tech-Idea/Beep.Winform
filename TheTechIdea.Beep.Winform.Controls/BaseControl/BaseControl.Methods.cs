using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;


namespace TheTechIdea.Beep.Winform.Controls.Base
{
    public partial class BaseControl
    {
        #region Theme Methods
        public virtual void ApplyTheme()
        {
            try
            {
                if (_currentTheme == null) return;

                ForeColor = _currentTheme.ForeColor;
                BorderColor = _currentTheme.BorderColor;
                
                _paint.ShadowColor = _currentTheme.ShadowColor;
                _paint.GradientStartColor = _currentTheme.GradientStartColor;
                _paint.GradientEndColor = _currentTheme.GradientEndColor;
                _paint.BadgeForeColor= _currentTheme.BadgeForeColor;
                _paint.BadgeBackColor= _currentTheme.BadgeBackColor;
                _paint.DisabledForeColor= _currentTheme.DisabledForeColor;
                _paint.DisabledBackColor= _currentTheme.DisabledBackColor;
                _paint.DisabledBorderColor= _currentTheme.DisabledBorderColor;
     
                // Respect IsChild same as base BeepControl
                if (IsChild)
                {
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
           
        }

        public virtual void HideToolTip()
        {
         
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

        #region Gradient Helper Methods
        public void AddGradientStop(float position, Color color) => _paint.AddGradientStop(position, color);
        public void ClearGradientStops() => _paint.ClearGradientStops();
        #endregion

        #region Image and Size Utility Methods (from BeepControl)
        public float GetScaleFactor(SizeF imageSize, Size targetSize)
        {
            return _dpi.GetScaleFactor(imageSize, targetSize, ScaleMode);
        }

        public RectangleF GetScaledBounds(SizeF imageSize, Rectangle targetRect)
        {
            return _dpi.GetScaledBounds(imageSize, targetRect, ScaleMode);
        }

        public RectangleF GetScaledBounds(SizeF imageSize)
        {
            return _dpi.GetScaledBounds(imageSize, ScaleMode);
        }

        public Size GetSuitableSizeForTextAndImage(Size imageSize, Size maxImageSize, TextImageRelation textImageRelation)
        {
            return _dpi.GetSuitableSizeForTextAndImage(imageSize, maxImageSize, textImageRelation);
        }

        // Parity alias for exact name as in base
        public Size GetSuitableSizeForTextandImage(Size imageSize, Size MaxImageSize, TextImageRelation TextImageRelation)
        {
            return GetSuitableSizeForTextAndImage(imageSize, MaxImageSize, TextImageRelation);
        }

        public Font GetScaledFont(Graphics g, string text, Size maxSize, Font originalFont)
        {
            return _dpi.GetScaledFont(g, text, maxSize, originalFont);
        }
        #endregion

        #region Control State Management
        protected virtual void ShowToolTipIfExists()
        {
            
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            _paint.UpdateRects();
            var drawingRect = _paint.DrawingRect;
            int adjustedWidth = drawingRect.Width;
            int adjustedHeight = drawingRect.Height;
            return new Size(adjustedWidth, adjustedHeight);
        }

        // Parity wrappers
        public virtual void UpdateDrawingRect() => _paint.UpdateRects();

        protected virtual void DrawContent(Graphics g)
        {
            // Set high quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            if (EnableMaterialStyle)
            {
                // Use material design drawing
                DrawMaterialContent(g);
            }
            else
            {
                // Use regular ControlPaintHelper drawing
                _paint.Draw(g);
            }
        }

        /// <summary>
        /// Draws material design content using the enhanced material helper
        /// </summary>
        private void DrawMaterialContent(Graphics g)
        {
            _materialHelper ??= new BaseControlMaterialHelper(this);
            _materialHelper.UpdateLayout();

            // Apply elevation settings to the material helper
            _materialHelper.SetElevation(_bcElevationLevel);
            _materialHelper.SetElevationEnabled(_bcUseElevation);

            _materialHelper.DrawAll(g);
        }
    
        /// <summary>
        /// Updates the material helper layout. Called by derived classes when layout changes.
        /// </summary>
        protected void UpdateMaterialLayout()
        {
            if (_materialHelper != null)
            {
                _materialHelper.UpdateLayout();
            }
        }

        protected  GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            return ControlPaintHelper.GetRoundedRectPath(rect, radius);
        }
        protected virtual void DrawFocusIndicator(Graphics g)
        {
            // Focus indicator handled by effects helper; override in derived if needed
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
            _paint.IsValid = true;
            Invalidate();
        }

        public virtual void SetValidationError(string message)
        {
            _paint.IsValid = false;
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
    }
}
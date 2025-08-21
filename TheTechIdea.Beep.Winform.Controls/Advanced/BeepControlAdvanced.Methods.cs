using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Advanced.Helpers;
using TheTechIdea.Beep.Desktop.Common.Util;

namespace TheTechIdea.Beep.Winform.Controls.Advanced
{
    public partial class BeepControlAdvanced
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

        public void ShowToolTip(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            _toolTip.Show(text, this, PointToClient(MousePosition), 3000);
        }

        public void HideToolTip() => _toolTip.Hide(this);

        public bool ValidateData(out string message)
        {
            return _dataBinding.ValidateData(out message);
        }

        public void SetValue(object value)
        {
            _oldValue = GetValue();
            _dataBinding.SetValue(value);
            IsDirty = true;
            _dataBinding.RaiseValueChanged(value);
        }

        public object GetValue() => _dataBinding.GetValue();
        public new void ClearValue() => _dataBinding.ClearValue();
        public new bool HasFilterValue() => _dataBinding.HasFilterValue();
        public AppFilter ToFilter() => _dataBinding.ToFilter();

        public void Draw(Graphics graphics, Rectangle rectangle) { /* extension point */ }
        public void SuspendFormLayout() => _dataBinding.SuspendFormLayout();
        public void ResumeFormLayout() => _dataBinding.ResumeFormLayout();

        public void ReceiveMouseEvent(HitTestEventArgs eventArgs)
        {
            _input.ReceiveMouseEvent(eventArgs);
        }
        public virtual void SendMouseEvent(IBeepUIComponent targetControl, MouseEventType eventType, Point screenLocation) => 
            _hitTest.SendMouseEvent(targetControl, eventType, screenLocation);

        public void SetBinding(string controlProperty, string dataSourceProperty) => 
            _dataBinding.SetBinding(controlProperty, dataSourceProperty);

        public void RaiseSubmitChanges() => _dataBinding.RaiseSubmitChanges();
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
            if (!string.IsNullOrEmpty(ToolTipText))
            {
                ShowToolTip(ToolTipText);
            }
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
        private void UpdateBorderRectangle() => _paint.UpdateRects();

        protected virtual void DrawBackground(Graphics g)
        {
            // Background handled by helper in this implementation
        }

        protected virtual void DrawContent(Graphics g)
        {
            // For derived controls to draw content
        }

        protected virtual void DrawBorders(Graphics g, Color effectiveBorderColor)
        {
            // Borders handled by helper; override in derived if needed
        }

        protected virtual void DrawFocusIndicator(Graphics g)
        {
            // Focus indicator handled by effects helper; override in derived if needed
        }
        #endregion

        #region React UI Helper Methods (from BeepControl)
        public virtual void ApplyReactUIStyles(Graphics g)
        {
            // This is handled by the paint helper now, but keeping interface for compatibility
            if (UIVariant != ReactUIVariant.Default)
            {
                Invalidate(); // Trigger repaint which will apply styles via paint helper
            }
        }

        private int GetSizeBasedValue(int xs, int sm, int md, int lg, int xl)
        {
            return UISize switch
            {
                ReactUISize.ExtraSmall => xs,
                ReactUISize.Small => sm,
                ReactUISize.Medium => md,
                ReactUISize.Large => lg,
                ReactUISize.ExtraLarge => xl,
                _ => md
            };
        }

        private void UpdateSizeBasedDimensions()
        {
            Padding newPadding = UISize switch
            {
                ReactUISize.ExtraSmall => new Padding(4),
                ReactUISize.Small => new Padding(6),
                ReactUISize.Medium => new Padding(8),
                ReactUISize.Large => new Padding(12),
                ReactUISize.ExtraLarge => new Padding(16),
                _ => new Padding(8)
            };

            if (Padding == new Padding(0))
            {
                Padding = newPadding;
            }

            if (UseThemeFont)
            {
                OverrideFontSize = UISize switch
                {
                    ReactUISize.ExtraSmall => TypeStyleFontSize.Small,
                    ReactUISize.Small => TypeStyleFontSize.Medium,
                    ReactUISize.Medium => TypeStyleFontSize.Large,
                    ReactUISize.Large => TypeStyleFontSize.ExtraLarge,
                    ReactUISize.ExtraLarge => TypeStyleFontSize.ExtraExtraLarge,
                    _ => TypeStyleFontSize.Medium
                };
            }

            if (UIDensity == ReactUIDensity.Compact)
            {
                Padding = new Padding(Padding.Left / 2, Padding.Top / 2, Padding.Right / 2, Padding.Bottom / 2);
            }
            else if (UIDensity == ReactUIDensity.Comfortable)
            {
                Padding = new Padding(Padding.Left * 2, Padding.Top * 2, Padding.Right * 2, Padding.Bottom * 2);
            }
        }
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
            if (Parent is BeepControlAdvanced parentBeepControl)
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
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel; // added for Designer attribute
using System.Windows.Forms.Design; // added for design-time support
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Designers;
using TheTechIdea.Beep.Winform.Controls.Base;
using System.Diagnostics;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    [Designer(typeof(BeepiFormProDesigner))]
    public partial class BeepiFormPro : Form, IFormStyle
    {

        private FormPainterMetrics _formpaintermaterics;
        private float _dpiScaleX = 1f;
        private float _dpiScaleY = 1f;
        // Metrics used for layout and painting; can be set externally or lazy-loaded
        /// <summary>
        /// i dont want to be serialized and persisted with the form
        /// </summary>
        /// 
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public FormPainterMetrics FormPainterMetrics
        {
            get
            {
                if (_formpaintermaterics == null)
                {
                    // Lazy load metrics based on current Style and theme
                    _formpaintermaterics = FormPainterMetrics.DefaultForCached(FormStyle, UseThemeColors ? CurrentTheme : null);
                }
                return _formpaintermaterics;
            }
            set
            {
                _formpaintermaterics = value;
                Invalidate(); // Redraw with new metrics
            }
        }
        private readonly SolidBrush _bgBrush; // Cache brush
        private DateTime _lastInvalidate = DateTime.MinValue;

        // Cached BorderShape path - recreated only when size or style changes
        private GraphicsPath _cachedBorderShape;
        private Size       _cachedBorderShapeSize;
        private FormStyle  _cachedBorderShapeStyle;
        private void DebouncedInvalidate(Rectangle? rect = null, bool invalidateChildren = false)
        {
            if ((DateTime.Now - _lastInvalidate).TotalMilliseconds < 16) //60 FPS max
                return;
            _lastInvalidate = DateTime.Now;
            if (rect.HasValue)
                Invalidate(rect.Value, invalidateChildren);
            else
                Invalidate(invalidateChildren);
        }
        private bool _isForcedClose = false;
        public BeepiFormPro()
        {

            StartPosition = FormStartPosition.CenterScreen;
            AutoSize = false;
            this.AutoScaleDimensions = new SizeF(96f, 96f); //
            AutoScaleMode = AutoScaleMode.Dpi;  // ✅ Better for DPI scaling
            // REMOVE AutoScaleDimensions - let WinForms set it automatically
            //AutoScaleDimensions will be set to (96F, 96F) at design time

            // CRITICAL: In design mode, use simpler painting to ensure designer can see changes
            // In runtime, use optimized double buffering for smooth rendering
            if (InDesignModeSafe)
            {
                // Design mode: simpler painting, no double buffering issues
                this.DoubleBuffered = false; // Let designer handle buffering
                SetStyle(
                    ControlStyles.ResizeRedraw |
                    ControlStyles.SupportsTransparentBackColor, true);
            }
            else
            {
                // Runtime: optimized double buffering for smooth rendering
                this.DoubleBuffered = true;
                SetStyle(
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.SupportsTransparentBackColor, true);
            }
            UpdateStyles();

            _hits = new BeepiFormProHitAreaManager(this);
            _interact = new BeepiFormProInteractionManager(this, _hits);

            // Don't hardcode painter - ApplyFormStyle() will set the correct one based on FormStyle property
            //BackColor = Color.White; // we paint everything
            InitializeBuiltInRegions();
            InitializeComponent();

            // Update window region when handle is created
            // NOTE: Heavy operations (layout calc, hit areas) are deferred to OnResizeEnd for better performance
            this.Resize += (s, e) => { 
                // Only update window region during resize for visual feedback
                // Layout recalculation happens in OnResizeEnd
                UpdateWindowRegion(); 
            };
            this.Scroll += (s, e) => { UpdateWindowRegion(); DebouncedInvalidate(); };
            this.HandleCreated += (s, e) => { UpdateWindowRegion(); DebouncedInvalidate(); };

            ApplyFormStyle(); // This sets ActivePainter based on FormStyle (which can be set at design time)
            BackColor = FormPainterMetrics.DefaultForCached(FormStyle, UseThemeColors ? CurrentTheme : null).BackgroundColor; _bgBrush = new SolidBrush(BackColor); // Initialize in constructor
            FormBorderStyle = FormBorderStyle.None;
            
            // CRITICAL: Design-time support - hook child control events for auto-refresh
            // This ensures the form repaints when controls are selected/moved in the designer
            this.ControlAdded += OnControlAddedDesignTime;
            this.ControlRemoved += OnControlRemovedDesignTime;
          
            // Hook existing controls
            foreach (Control c in Controls)
            {
                HookChildEvents(c);
            }

            InitializeGlobalThemeSynchronization();
        }

        private void OnControlAddedDesignTime(object sender, ControlEventArgs e)
        {
            if (e?.Control == null)
                return;

            if (e.Control is BaseControl themedCtrl)
            {
                try
                {
                    // Use ApplyControlStyle with deferred layout to avoid mid-batch repaints
                    themedCtrl.ApplyControlStyle(BeepStyling.GetControlStyle(FormStyle), preserveSize: true);
                }
                catch
                {
                    // Design-time safe: avoid blocking control add transactions.
                }
            }

            // Also apply style recursively if the added control is a container (e.g., UserControl)
            if (e.Control.HasChildren)
            {
                try
                {
                    var mappedStyle = BeepStyling.GetControlStyle(FormStyle);
                    ApplyStyleRecursive(e.Control.Controls, mappedStyle);
                }
                catch { }
            }

            // Hook events for the newly added control
            try { HookChildEvents(e.Control); } catch { }

            // Repaint after designer transaction completes
            SafeInvalidateDesignSurface(includeChildren: true, immediate: false);
        }

        private void OnControlRemovedDesignTime(object sender, ControlEventArgs e)
        {
            if (e?.Control == null)
                return;

            // Unhook events from the removed control
            try { UnhookChildEvents(e.Control); } catch { }
            
            // Remove from tracking dictionary
            _controlLastBounds.Remove(e.Control);
            
            // Repaint after designer transaction completes
            SafeInvalidateDesignSurface(includeChildren: true, immediate: false);
        }

        private System.Threading.Timer _designModeInvalidateTimer;
        private bool _designModeInvalidatePending = false;
        private readonly Dictionary<Control, Rectangle> _controlLastBounds = new Dictionary<Control, Rectangle>();
        
        private void HookChildEvents(Control ctrl)
        {
            if (ctrl == null) return;
            
            // Hook events that should trigger form repaint in design mode
            ctrl.Move += OnChildControlChanged;
            ctrl.Resize += OnChildControlChanged;
            ctrl.VisibleChanged += OnChildControlChanged;
            ctrl.EnabledChanged += OnChildControlChanged;
            ctrl.BackColorChanged += OnChildControlChanged;
            ctrl.ForeColorChanged += OnChildControlChanged;
            
            // CRITICAL: Hook GotFocus and LostFocus to repaint when designer selects controls
            ctrl.GotFocus += OnChildControlFocusChanged;
            ctrl.LostFocus += OnChildControlFocusChanged;
            
            // Hook Paint only in design mode - at runtime this causes unnecessary form repaints
            // every time any child control paints (which can be very frequent for animated controls)
            if (InDesignModeSafe)
            {
                ctrl.Paint += OnChildControlPaint;
            }
        }

        private void UnhookChildEvents(Control ctrl)
        {
            if (ctrl == null) return;
            
            ctrl.Move -= OnChildControlChanged;
            ctrl.Resize -= OnChildControlChanged;
            ctrl.VisibleChanged -= OnChildControlChanged;
            ctrl.EnabledChanged -= OnChildControlChanged;
            ctrl.BackColorChanged -= OnChildControlChanged;
            ctrl.ForeColorChanged -= OnChildControlChanged;
            ctrl.GotFocus -= OnChildControlFocusChanged;
            ctrl.LostFocus -= OnChildControlFocusChanged;
            ctrl.Paint -= OnChildControlPaint; // Safe to call even if not hooked
        }

        private void OnChildControlChanged(object sender, EventArgs e)
        {
            // CRITICAL: When controls are moved/resized in design mode, repaint immediately
            // This ensures the form background is visible under the control as it moves
            if (!InDesignModeSafe || IsDisposed || !IsHandleCreated || sender is not Control ctrl)
                return;

            if (ctrl.IsDisposed)
            {
                _controlLastBounds.Remove(ctrl);
                return;
            }

            try
            {
                _controlLastBounds[ctrl] = ctrl.Bounds;
            }
            catch
            {
                _controlLastBounds.Remove(ctrl);
                return;
            }

            // CRITICAL: In design mode, force a FULL form refresh
            // Partial invalidation doesn't work reliably in the designer
            SafeInvalidateDesignSurface(includeChildren: true, immediate: true);
        }

        private void OnChildControlFocusChanged(object sender, EventArgs e)
        {
            // CRITICAL: When a control gets/loses focus in the designer (selection change),
            // we need to repaint, but defer it to batch multiple focus events together
            // This prevents blank screen during control-to-control transitions
            if (InDesignModeSafe && !IsDisposed && IsHandleCreated)
            {
                // Cancel any pending invalidation timer
                _designModeInvalidateTimer?.Dispose();
                
                // Set up a new timer to invalidate after a short delay (50ms)
                // This batches multiple focus events (LostFocus + GotFocus) into a single repaint
                _designModeInvalidatePending = true;
                _designModeInvalidateTimer = new System.Threading.Timer(_ =>
                {
                    if (!IsDisposed && IsHandleCreated && _designModeInvalidatePending)
                    {
                        try
                        {
                            BeginInvoke(new Action(() =>
                            {
                                try
                                {
                                    if (!IsDisposed && IsHandleCreated)
                                    {
                                        this.Invalidate();
                                        this.Update();
                                    }
                                }
                                catch { }
                                finally
                                {
                                    _designModeInvalidatePending = false;
                                }
                            }));
                        }
                        catch { _designModeInvalidatePending = false; /* Form may be disposing */ }
                    }
                }, null, 50, System.Threading.Timeout.Infinite);
            }
        }

        private void SafeInvalidateDesignSurface(bool includeChildren, bool immediate)
        {
            if (!InDesignModeSafe || IsDisposed || !IsHandleCreated)
                return;

            try
            {
                BeginInvoke(new Action(() =>
                {
                    if (IsDisposed || !IsHandleCreated)
                        return;

                    try
                    {
                        if (includeChildren)
                            Invalidate(true);
                        else
                            Invalidate();

                        if (immediate)
                            Update();
                    }
                    catch { }
                }));
            }
            catch { }
        }

        private void OnChildControlPaint(object sender, PaintEventArgs e)
        {
            // When a child control paints in design mode, ensure form background is visible
            if (InDesignModeSafe)
            {
                // Don't invalidate here - it would cause infinite loop
                // Just ensure the form's background is properly painted
            }
        }

        //protected override void OnDpiChanged(DpiChangedEventArgs e)
        //{
        //    DpiScalingHelper.GetScalesFromDpiChangedEvent(
        //        e,
        //        _dpiScaleX,
        //        _dpiScaleY,
        //        out var oldScaleX,
        //        out var oldScaleY,
        //        out var newScaleX,
        //        out var newScaleY);

        //    base.OnDpiChanged(e);

        //    _dpiScaleX = newScaleX;
        //    _dpiScaleY = newScaleY;

        //    if (!DpiScalingHelper.AreScaleFactorsEqual(oldScaleX, newScaleX) ||
        //        !DpiScalingHelper.AreScaleFactorsEqual(oldScaleY, newScaleY))
        //    {
        //        // Preserve Dock/Anchor behavior: only non-auto controls are manually scaled.
        //        DpiScalingHelper.ScaleControlTreeForDpiChange(
        //            this,
        //            oldScaleX,
        //            oldScaleY,
        //            newScaleX,
        //            newScaleY,
        //            scaleFont: false);
        //        PropagateDpiChangeToChildren();
        //    }

        //    // Clear painter caches that may depend on DPI (brushes, pens, rasters, paths)
        //    try { PaintersFactory.ClearCache(); } catch { }

        //    // Mark layout dirty - will be recalculated on next paint
        //    InvalidateLayout();

        //    // Update window region for new DPI
        //    UpdateWindowRegion();

        //    PerformLayout();
        //    Invalidate(true);
        //}

        private void PropagateDpiChangeToChildren()
        {
            foreach (Control child in Controls)
            {
                PropagateDpiChangeRecursive(child);
            }
        }

        private static void PropagateDpiChangeRecursive(Control control)
        {
            if (control == null || control.IsDisposed)
                return;

            control.PerformLayout();
            control.Invalidate();

            foreach (Control child in control.Controls)
            {
                PropagateDpiChangeRecursive(child);
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            ApplyStyletoChildControls();

            UpdateWindowRegion();
            
            InvalidateLayout();
        }

        /// <summary>
        /// Called when resize operation completes. All heavy operations are deferred here
        /// to avoid performance issues during resize dragging.
        /// </summary>
        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            
            // CRITICAL: All heavy operations happen here, not during Resize event
            // This dramatically improves resize performance and reduces flickering
            
            // Mark layout as dirty and force recalculation
            InvalidateLayout();
            
            // Update window region for new size
            UpdateWindowRegion();
            
            // Force full repaint
            Invalidate(true);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_isForcedClose)
            {
                e.Cancel = false;
            }

            PreClose?.Invoke(this, e);

            base.OnFormClosing(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            UnregisterGlobalThemeEvents();

            // Raise OnFormClose event after form has closed
            OnFormClose?.Invoke(this, EventArgs.Empty);
             
            base.OnFormClosed(e);
        }

        private void ApplyFormStyle()
        {
            // Use the PaintersFactory to get cached painter instances
            // This avoids creating new painter objects on every style change
            FormBorderStyle = FormBorderStyle.None;
            ActivePainter = PaintersFactory.GetPainter(FormStyle);

            // Force layout recalculation to reposition child controls based on new DisplayRectangle
            if (!DesignMode)
            {
                PerformLayout();
            }
            BackColor = FormPainterMetrics.DefaultForCached(FormStyle, UseThemeColors ? CurrentTheme : null).BackgroundColor;
            ApplyStyletoChildControls();
            
            // CRITICAL: Update window region to match new Style's corner radius
            if (IsHandleCreated)
            {
                UpdateWindowRegion();
                UpdateFormRegion();
            }

            // Note: We no longer clear the cache here - painters are reused across style changes
            // Only clear cache when theme changes (handled in Theme setter)

            Invalidate(true);
            if (InDesignModeSafe)
            {
                SafeInvalidateDesignSurface(includeChildren: true, immediate: true);
            }
        }
        private void ApplyStyletoChildControls()
        {
            var mappedStyle = BeepStyling.GetControlStyle(FormStyle);
            ApplyStyleRecursive(Controls, mappedStyle);
        }

        private void ApplyStyleRecursive(Control.ControlCollection controls, BeepControlStyle mappedStyle)
        {
            foreach (Control ctrl in controls)
            {
                if (ctrl is BaseControl themedCtrl)
                {
                    // ApplyControlStyle with preserveSize:true defers layout/invalidation
                    // during setter, then does one synchronized refresh at the end
                    themedCtrl.ApplyControlStyle(mappedStyle, preserveSize: true);
                }
                // Recurse into containers (UserControls, Panels, etc.) to style nested controls
                if (ctrl.HasChildren)
                {
                    ApplyStyleRecursive(ctrl.Controls, mappedStyle);
                }
            }
        }
        /// <summary>
        /// Override DisplayRectangle to exclude the caption bar area and borders.
        /// This ensures controls added to the form don't overlap the caption or borders.
        /// </summary>
        public override Rectangle DisplayRectangle
        {
            get
            {
                var rect = base.DisplayRectangle;

                int borderWidth = 0;
                if (ActivePainter != null)
                {
                    var metrics = FormPainterMetrics.DefaultForCached(FormStyle, UseThemeColors ? CurrentTheme : null);
                    borderWidth = metrics?.BorderWidth ?? 1;
                }

                rect.X += borderWidth;
                rect.Y += borderWidth;
                rect.Width = Math.Max(0, rect.Width - borderWidth * 2);
                rect.Height = Math.Max(0, rect.Height - borderWidth * 2);

                if (ShowCaptionBar)
                {
                    int captionHeight = Math.Max(CaptionHeight, (int)(Font.Height * 2.5f));
                    rect.Y += captionHeight;
                    rect.Height = Math.Max(0, rect.Height - captionHeight);
                }

                return rect;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath for the form's border shape.
        /// The path is inset by half the border width so the pen draws entirely inside the client area.
        /// The result is cached and only recreated when the form size or style changes.
        /// IMPORTANT: Callers must NOT dispose this path — it is owned and managed by BeepiFormPro.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public GraphicsPath BorderShape
        {
            get
            {
                // Return cached path when nothing relevant has changed
                if (_cachedBorderShape != null
                    && _cachedBorderShapeSize  == ClientSize
                    && _cachedBorderShapeStyle == FormStyle)
                {
                    return _cachedBorderShape;
                }

                // Dispose old cached path before creating a new one
                _cachedBorderShape?.Dispose();
                _cachedBorderShape = null;

                if (ActivePainter != null)
                {
                    var radius = ActivePainter.GetCornerRadius(this);
                    var metrics = FormPainterMetrics.DefaultForCached(FormStyle, UseThemeColors ? CurrentTheme : null);
                    int borderWidth = metrics?.BorderWidth ?? 1;

                    GraphicsPath outerPath;
                    if (radius.TopLeft == radius.TopRight &&
                        radius.TopLeft == radius.BottomLeft &&
                        radius.TopLeft == radius.BottomRight)
                    {
                        outerPath = GraphicsExtensions.CreateRoundedRectanglePath(ClientRectangle, radius.TopLeft);
                    }
                    else
                    {
                        outerPath = GraphicsExtensions.CreateRoundedRectanglePath(ClientRectangle,
                            radius.TopLeft, radius.TopRight, radius.BottomLeft, radius.BottomRight);
                    }

                    float inset = borderWidth / 2f;
                    _cachedBorderShape = outerPath.CreateInsetPath(inset, radius.TopLeft);
                    outerPath.Dispose();
                }
                else
                {
                    var outerPath = GraphicsExtensions.CreateRoundedRectanglePath(ClientRectangle, 0);
                    _cachedBorderShape = outerPath.CreateInsetPath(0.5f, 0);
                    outerPath.Dispose();
                }

                _cachedBorderShapeSize  = ClientSize;
                _cachedBorderShapeStyle = FormStyle;
                return _cachedBorderShape;
            }
        }
    }
}


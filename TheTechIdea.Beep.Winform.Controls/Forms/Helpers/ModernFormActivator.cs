using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Enables modern Beep UI features on any Windows Form through helper composition.
    /// Provides a static method to retrofit existing forms with modern styling capabilities.
    /// </summary>
    public static class ModernFormActivator
    {
        /// <summary>
        /// Enables modern Beep UI features on any Form by attaching helper infrastructure.
        /// </summary>
        /// <param name="form">The form to enhance with modern features</param>
        /// <param name="theme">Optional theme to apply (uses default if null)</param>
        /// <returns>A controller object to manage the modern features</returns>
        public static IModernFormController Enable(Form form, IBeepTheme theme = null)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            
            // Don't double-enable
            if (form.Tag is IModernFormController existing)
                return existing;

            var controller = new ModernFormController(form, theme);
            form.Tag = controller; // Store reference for future calls
            
            return controller;
        }

        /// <summary>
        /// Disables modern features on a form that was previously enabled.
        /// </summary>
        /// <param name="form">The form to disable modern features on</param>
        public static void Disable(Form form)
        {
            if (form?.Tag is IModernFormController controller)
            {
                controller.Dispose();
                form.Tag = null;
            }
        }

        /// <summary>
        /// Gets the modern form controller for a form if it has been enabled.
        /// </summary>
        /// <param name="form">The form to check</param>
        /// <returns>The controller if found, null otherwise</returns>
        public static IModernFormController GetController(Form form)
        {
            return form?.Tag as IModernFormController;
        }
    }

    /// <summary>
    /// Interface for controlling modern features on a retrofitted form.
    /// </summary>
    public interface IModernFormController : IDisposable
    {
        /// <summary>Gets or sets the border radius</summary>
        int BorderRadius { get; set; }
        
        /// <summary>Gets or sets the border thickness</summary>
        int BorderThickness { get; set; }
        
        /// <summary>Gets or sets whether to show the custom caption bar</summary>
        bool ShowCaptionBar { get; set; }
        
        /// <summary>Gets or sets the caption bar height</summary>
        int CaptionHeight { get; set; }
        
        /// <summary>Gets or sets whether to show system buttons</summary>
        bool ShowSystemButtons { get; set; }
        
        /// <summary>Gets or sets whether to enable caption gradient</summary>
        bool EnableCaptionGradient { get; set; }
        
        /// <summary>Gets or sets whether to enable glow effects</summary>
        bool EnableGlow { get; set; }
        
        /// <summary>Gets or sets the glow color</summary>
        Color GlowColor { get; set; }
        
        /// <summary>Gets or sets the shadow depth</summary>
        int ShadowDepth { get; set; }
        
        /// <summary>Applies a theme to the form</summary>
        void ApplyTheme(IBeepTheme theme);
        
        /// <summary>Applies a predefined style</summary>
        void ApplyStyle(BeepFormStyle style);
        
        /// <summary>Registers an overlay painter for custom rendering</summary>
        void RegisterOverlayPainter(Action<Graphics> painter);
    }

    /// <summary>
    /// Internal implementation of the modern form controller.
    /// Creates an adapter that implements IBeepModernFormHost for any Form.
    /// </summary>
    internal class ModernFormController : IModernFormController
    {
        private readonly Form _form;
        private readonly FormAdapter _adapter;
        private readonly FormStateStore _state;
        private readonly FormRegionHelper _regionHelper;
        private readonly FormLayoutHelper _layoutHelper;
        private readonly FormShadowGlowPainter _shadowGlow;
        private readonly FormOverlayPainterRegistry _overlayRegistry;
        private readonly FormThemeHelper _themeHelper;
        private readonly FormStyleHelper _styleHelper;
        private readonly FormCaptionBarHelper _captionHelper;
        private readonly FormHitTestHelper _hitTestHelper;

        private bool _disposed;

        public ModernFormController(Form form, IBeepTheme theme = null)
        {
            _form = form ?? throw new ArgumentNullException(nameof(form));
            _adapter = new FormAdapter(form, theme ?? BeepThemesManager.GetDefaultTheme());

            // Initialize helpers
            _state = new FormStateStore();
            _shadowGlow = new FormShadowGlowPainter();
            _overlayRegistry = new FormOverlayPainterRegistry();
            _regionHelper = new FormRegionHelper(_adapter);
            _layoutHelper = new FormLayoutHelper(_adapter);
            _themeHelper = new FormThemeHelper(_adapter);
            _styleHelper = new FormStyleHelper(_adapter, _shadowGlow);
            _captionHelper = new FormCaptionBarHelper(_adapter, _overlayRegistry, RegisterPaddingProvider);
            _hitTestHelper = new FormHitTestHelper(_adapter,
                captionEnabled: () => _captionHelper?.ShowCaptionBar == true,
                captionHeight: () => _captionHelper?.CaptionHeight ?? 0,
                isOverSystemButton: () => _captionHelper?.IsCursorOverSystemButton == true,
                resizeMargin: 8);

            // Hook into form events
            AttachEventHandlers();

            // Set form border style
            if (_form.FormBorderStyle != FormBorderStyle.None)
                _form.FormBorderStyle = FormBorderStyle.None;

            // Apply initial theme
            if (theme != null || _adapter.CurrentTheme != null)
                ApplyTheme(_adapter.CurrentTheme);
        }

        #region IModernFormController Implementation
        
        public int BorderRadius
        {
            get => _adapter.BorderRadius;
            set => _adapter.BorderRadius = value;
        }

        public int BorderThickness
        {
            get => _adapter.BorderThickness;
            set => _adapter.BorderThickness = value;
        }

        public bool ShowCaptionBar
        {
            get => _captionHelper?.ShowCaptionBar ?? true;
            set { if (_captionHelper != null) _captionHelper.ShowCaptionBar = value; }
        }

        public int CaptionHeight
        {
            get => _captionHelper?.CaptionHeight ?? 36;
            set { if (_captionHelper != null) _captionHelper.CaptionHeight = value; }
        }

        public bool ShowSystemButtons
        {
            get => _captionHelper?.ShowSystemButtons ?? true;
            set { if (_captionHelper != null) _captionHelper.ShowSystemButtons = value; }
        }

        public bool EnableCaptionGradient
        {
            get => _captionHelper?.EnableCaptionGradient ?? true;
            set { if (_captionHelper != null) _captionHelper.EnableCaptionGradient = value; }
        }

        public bool EnableGlow
        {
            get => _shadowGlow?.EnableGlow ?? false;
            set { if (_shadowGlow != null) _shadowGlow.EnableGlow = value; }
        }

        public Color GlowColor
        {
            get => _shadowGlow?.GlowColor ?? Color.Blue;
            set { if (_shadowGlow != null) _shadowGlow.GlowColor = value; }
        }

        public int ShadowDepth
        {
            get => _shadowGlow?.ShadowDepth ?? 0;
            set { if (_shadowGlow != null) _shadowGlow.ShadowDepth = value; }
        }

        public void ApplyTheme(IBeepTheme theme)
        {
            _adapter.SetTheme(theme);
            _themeHelper?.ApplyTheme();
        }

        public void ApplyStyle(BeepFormStyle style)
        {
         
            
            _styleHelper?.ApplyFormStyle(style);
        }

        public void RegisterOverlayPainter(Action<Graphics> painter)
        {
            _overlayRegistry?.Add(painter);
        }

        #endregion

        #region Event Handling

        private void AttachEventHandlers()
        {
            _form.Paint += OnFormPaint;
            _form.MouseDown += OnFormMouseDown;
            _form.MouseMove += OnFormMouseMove;
            _form.MouseLeave += OnFormMouseLeave;
            _form.Resize += OnFormResize;
        }

        private void DetachEventHandlers()
        {
            if (_form != null && !_form.IsDisposed)
            {
                _form.Paint -= OnFormPaint;
                _form.MouseDown -= OnFormMouseDown;
                _form.MouseMove -= OnFormMouseMove;
                _form.MouseLeave -= OnFormMouseLeave;
                _form.Resize -= OnFormResize;
            }
        }

        private void OnFormPaint(object sender, PaintEventArgs e)
        {
            var bounds = _form.ClientRectangle;
            
            // Paint in order: shadow/glow -> background -> caption -> border -> overlays
            // Create a rounded path for shadow/glow
            using var path = CreateRoundedPath(bounds, _adapter.BorderRadius);
            _shadowGlow?.PaintShadow(e.Graphics, path);
            _shadowGlow?.PaintGlow(e.Graphics, path);
            
            // Background painting would go here
            // Caption bar painting is handled by overlay registry
            // Border painting would go here
            _overlayRegistry?.PaintOverlays(e.Graphics);
        }

        private System.Drawing.Drawing2D.GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = Math.Min(Math.Min(bounds.Width, bounds.Height), radius * 2);
            var arcRect = new Rectangle(bounds.X, bounds.Y, diameter, diameter);
            path.AddArc(arcRect, 180, 90);
            arcRect.X = bounds.Right - diameter;
            path.AddArc(arcRect, 270, 90);
            arcRect.Y = bounds.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);
            arcRect.X = bounds.Left;
            path.AddArc(arcRect, 90, 90);
            path.CloseFigure();

            return path;
        }

        private void OnFormMouseDown(object sender, MouseEventArgs e)
        {
            _captionHelper?.OnMouseDown(e);
        }

        private void OnFormMouseMove(object sender, MouseEventArgs e)
        {
            _captionHelper?.OnMouseMove(e);
        }

        private void OnFormMouseLeave(object sender, EventArgs e)
        {
            _captionHelper?.OnMouseLeave();
        }

        private void OnFormResize(object sender, EventArgs e)
        {
            _regionHelper?.EnsureRegion(true);
            _form.Invalidate();
        }

        private readonly List<FormCaptionBarHelper.PaddingAdjuster> _paddingProviders = new();

        private void RegisterPaddingProvider(FormCaptionBarHelper.PaddingAdjuster provider)
        {
            if (provider != null)
                _paddingProviders.Add(provider);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (!_disposed)
            {
                DetachEventHandlers();
                _regionHelper?.Dispose();
                _disposed = true;
            }
        }

        #endregion
    }

    /// <summary>
    /// Adapter that makes any Form compatible with IBeepModernFormHost.
    /// </summary>
    internal class FormAdapter : IBeepModernFormHost
    {
        private readonly Form _form;
        private IBeepTheme _currentTheme;
        private int _borderRadius;
        private int _borderThickness = 1;

        public FormAdapter(Form form, IBeepTheme theme = null)
        {
            _form = form ?? throw new ArgumentNullException(nameof(form));
            _currentTheme = theme ?? BeepThemesManager.GetDefaultTheme();
        }

        public Form AsForm => _form;
        public IBeepTheme CurrentTheme => _currentTheme;
        public bool IsInDesignMode => LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        public Padding Padding
        {
            get => _form.Padding;
            set => _form.Padding = value;
        }

        public void Invalidate() => _form.Invalidate();

        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                if (_borderRadius != value)
                {
                    _borderRadius = Math.Max(0, value);
                    UpdateRegion();
                    _form.Invalidate();
                }
            }
        }

        public int BorderThickness
        {
            get => _borderThickness;
            set
            {
                if (_borderThickness != value)
                {
                    _borderThickness = Math.Max(0, value);
                    _form.Invalidate();
                }
            }
        }

        public void SetTheme(IBeepTheme theme)
        {
            _currentTheme = theme ?? BeepThemesManager.GetDefaultTheme();
            if (_currentTheme.BackColor != Color.Empty)
                _form.BackColor = _currentTheme.BackColor;
        }

        public void UpdateRegion()
        {
            if (!_form.IsHandleCreated || _borderRadius <= 0 || _form.WindowState == FormWindowState.Maximized)
            {
                _form.Region?.Dispose();
                _form.Region = null;
                return;
            }

            var bounds = new Rectangle(0, 0, _form.ClientSize.Width, _form.ClientSize.Height);
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;

            using var path = CreateRoundedRectanglePath(bounds, _borderRadius);
            _form.Region?.Dispose();
            _form.Region = new Region(path);
        }

        private System.Drawing.Drawing2D.GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            {
                if (rect.Width > 0 && rect.Height > 0)
                    path.AddRectangle(rect);
                return path;
            }

            int diameter = Math.Min(Math.Min(rect.Width, rect.Height), radius * 2);
            if (diameter <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            var arcRect = new Rectangle(rect.X, rect.Y, diameter, diameter);
            path.AddArc(arcRect, 180, 90);
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
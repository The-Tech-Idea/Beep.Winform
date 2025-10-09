using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Forms.Caption;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Provides unified layout rectangles so painting and child layout stay consistent.
    /// Now consults CaptionRendererPreferences to ensure geometry aligns with painter metrics.
    /// </summary>
    internal sealed class FormLayoutHelper
    {
        private readonly IBeepModernFormHost _host;
        // Optional providers to override default geometry with custom shapes (e.g., tabbed forms)
        private Func<GraphicsPath> _customWindowPathProvider;
        private Func<GraphicsPath> _customClientPathProvider;
        
        // Providers for style-driven metrics
        private Func<BeepFormStyle> _styleProvider;
        private Func<bool> _showCaptionBarProvider;

        public FormLayoutHelper(IBeepModernFormHost host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
        }
        
        /// <summary>
        /// Set providers so layout can consult current style and caption visibility.
        /// </summary>
        public void SetStyleProviders(Func<BeepFormStyle> styleProvider, Func<bool> showCaptionBarProvider)
        {
            _styleProvider = styleProvider;
            _showCaptionBarProvider = showCaptionBarProvider;
        }

        /// <summary>
        /// Allows consumers to override the default window outline path with a custom provider.
        /// The function should return a new GraphicsPath each call; caller is responsible to dispose usage.
        /// </summary>
        public void SetCustomWindowPathProvider(Func<GraphicsPath> provider) => _customWindowPathProvider = provider;

        /// <summary>
        /// Allows consumers to override the default client/content path with a custom provider.
        /// The function should return a new GraphicsPath each call; caller is responsible to dispose usage.
        /// </summary>
        public void SetCustomClientPathProvider(Func<GraphicsPath> provider) => _customClientPathProvider = provider;

        // Rectangle helpers retained for layout math only. Prefer the path APIs below for painting and regioning.
        private Rectangle GetPaintBounds()
        {
            var form = _host.AsForm;
            if (form.ClientSize.Width <= 0 || form.ClientSize.Height <= 0) return Rectangle.Empty;
            var pad = _host.Padding;
            return new Rectangle(pad.Left, pad.Top,
                Math.Max(0, form.ClientSize.Width - pad.Horizontal),
                Math.Max(0, form.ClientSize.Height - pad.Vertical));
        }
        
        /// <summary>
        /// Get the effective caption height from centralized preferences based on current style.
        /// </summary>
        private int GetEffectiveCaptionHeight()
        {
            if (_styleProvider == null || _showCaptionBarProvider == null || !_showCaptionBarProvider())
                return 0;
            
            var style = _styleProvider();
            var pref = CaptionRendererPreferences.GetPreference(style);
            return pref.CaptionHeight;
        }
        
        /// <summary>
        /// Get the effective border radius from centralized preferences based on current style.
        /// </summary>
        private int GetEffectiveBorderRadius()
        {
            if (_styleProvider == null)
                return _host.BorderRadius; // fallback to host
            
            var style = _styleProvider();
            var pref = CaptionRendererPreferences.GetPreference(style);
            return pref.BorderRadius;
        }
        
        /// <summary>
        /// Get the effective border thickness from centralized preferences based on current style.
        /// </summary>
        private int GetEffectiveBorderThickness()
        {
            if (_styleProvider == null)
                return _host.BorderThickness; // fallback to host
            
            var style = _styleProvider();
            var pref = CaptionRendererPreferences.GetPreference(style);
            return pref.BorderThickness;
        }

    // Rectangle helper retained for internal calculations
    private Rectangle GetContentBounds() => GetPaintBounds();

        /// <summary>
        /// Full window bounds in window coordinates (0,0,Width,Height).
        /// Useful for non-client painting and region creation.
        /// </summary>
        private Rectangle GetWindowBounds()
        {
            var form = _host.AsForm;
            return new Rectangle(0, 0, Math.Max(0, form.Width), Math.Max(0, form.Height));
        }

        // Unified geometry providers (paths only)

        /// <summary>
        /// Returns the full form outline path in window coordinates, applying BorderRadius from preferences.
        /// </summary>
        public GraphicsPath GetWindowPath()
        {
            if (_customWindowPathProvider != null)
            {
                var custom = _customWindowPathProvider();
                if (custom != null) return custom;
            }
            var bounds = GetWindowBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return new GraphicsPath();
            
            int radius = GetEffectiveBorderRadius();
            if (radius <= 0)
                return bounds.ToGraphicsPath();
            return BuildRoundedRectPath(bounds, radius);
        }

        /// <summary>
        /// Returns the client content path inside Padding, applying BorderRadius and accounting for caption height.
        /// This is the area where child controls should be placed.
        /// </summary>
        public GraphicsPath GetClientPath()
        {
            if (_customClientPathProvider != null)
            {
                var custom = _customClientPathProvider();
                if (custom != null) return custom;
            }
            
            var rect = GetPaintBounds();
            if (rect.Width <= 0 || rect.Height <= 0) return new GraphicsPath();
            
            // Subtract caption height from top if caption is visible
            int captionHeight = GetEffectiveCaptionHeight();
            if (captionHeight > 0)
            {
                rect = new Rectangle(
                    rect.X, 
                    rect.Y + captionHeight, 
                    rect.Width, 
                    Math.Max(0, rect.Height - captionHeight));
            }
            
            if (rect.Width <= 0 || rect.Height <= 0) return new GraphicsPath();
            
            int radius = GetEffectiveBorderRadius();
            if (radius <= 0)
                return rect.ToGraphicsPath();
            
            int thickness = GetEffectiveBorderThickness();
            int innerRadius = Math.Max(0, radius - Math.Max(1, thickness));
            return BuildRoundedRectPath(rect, innerRadius);
        }

        /// <summary>
        /// Path for the paintable area (inside padding). Alias of GetClientPath for clarity.
        /// </summary>
        public GraphicsPath GetPaintPath() => GetClientPath();

        /// <summary>
        /// Path for the content area. Alias of GetClientPath for now; can diverge if ribbon/caption offsets are added.
        /// </summary>
        public GraphicsPath GetContentPath() => GetClientPath();

        /// <summary>
        /// Returns the caption band path across the top, using captionHeight from preferences and BorderRadius.
        /// This is the area where caption renderer should paint.
        /// </summary>
        public GraphicsPath GetCaptionPath()
        {
            var paint = GetPaintBounds();
            if (paint.Width <= 0 || paint.Height <= 0) return new GraphicsPath();
            
            int captionHeight = GetEffectiveCaptionHeight();
            if (captionHeight <= 0) return new GraphicsPath();
            
            var cap = new Rectangle(paint.X, paint.Y, paint.Width, Math.Min(paint.Height, captionHeight));
            
            int radius = GetEffectiveBorderRadius();
            if (radius <= 0)
                return cap.ToGraphicsPath();
            
            return BuildTopRoundedRectPath(cap, radius);
        }
        
        /// <summary>
        /// Legacy overload for backward compatibility - prefer parameterless GetCaptionPath().
        /// </summary>
        public GraphicsPath GetCaptionPath(int captionHeight)
        {
            // If explicit height provided, use it; otherwise delegate to preference-based method
            if (captionHeight > 0)
            {
                var paint = GetPaintBounds();
                if (paint.Width <= 0 || paint.Height <= 0) return new GraphicsPath();
                var cap = new Rectangle(paint.X, paint.Y, paint.Width, Math.Min(paint.Height, captionHeight));
                
                int radius = GetEffectiveBorderRadius();
                if (radius <= 0)
                    return cap.ToGraphicsPath();
                return BuildTopRoundedRectPath(cap, radius);
            }
            return GetCaptionPath();
        }

        /// <summary>
        /// Builds a border ring path by subtracting an inner path from an outer path.
        /// Caller is responsible to dispose the returned path.
        /// </summary>
        public GraphicsPath GetBorderRingPath(int thickness)
        {
            // For path-only drawing with inset pen, the outer path is sufficient
            return GetWindowPath();
        }

        // Internal path builders
        private static GraphicsPath BuildRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            { if (rect.Width > 0 && rect.Height > 0) path.AddRectangle(rect); return path; }
            int d = Math.Min(Math.Min(rect.Width, rect.Height), radius * 2);
            var arc = new Rectangle(rect.X, rect.Y, d, d);
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - d; path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - d; path.AddArc(arc, 0, 90);
            arc.X = rect.Left; path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static GraphicsPath BuildTopRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            { if (rect.Width > 0 && rect.Height > 0) path.AddRectangle(rect); return path; }
            int d = Math.Min(Math.Min(rect.Width, rect.Height), radius * 2);
            var arc = new Rectangle(rect.X, rect.Y, d, d);
            // Top-left arc
            path.AddArc(arc, 180, 90);
            // Top-right arc
            arc.X = rect.Right - d; path.AddArc(arc, 270, 90);
            // Right edge down
            path.AddLine(rect.Right, rect.Y + d, rect.Right, rect.Bottom);
            // Bottom edge
            path.AddLine(rect.Right, rect.Bottom, rect.Left, rect.Bottom);
            // Left edge up
            path.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Y + d);
            path.CloseFigure();
            return path;
        }
    }
}

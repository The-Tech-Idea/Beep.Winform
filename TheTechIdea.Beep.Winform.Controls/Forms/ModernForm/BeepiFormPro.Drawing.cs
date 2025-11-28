using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public partial class BeepiFormPro
    {
        // Central design-mode detection (more reliable than DesignMode alone)
        private static bool IsDesignProcess()
        {
            try
            {
                string proc = Process.GetCurrentProcess().ProcessName;
                return proc.Equals("devenv", StringComparison.OrdinalIgnoreCase)
                       || proc.Equals("Blend", StringComparison.OrdinalIgnoreCase)
                       || proc.Equals("XDesProc", StringComparison.OrdinalIgnoreCase);
            }
            catch { return false; }
        }

        // Design mode check
        private bool InDesignMode => DesignMode || IsDesignProcess() || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        // Track if design mode layout has been calculated
        private bool _designModeLayoutCalculated = false;

      

        private void PaintRegions(Graphics g)
        {
            // Draw custom regions
            foreach (var region in _regions)
            {
                var rect = ResolveRegionRect(region);
                region.OnPaint?.Invoke(g, rect);
            }
        }

        // Exposed so painters can explicitly render caption elements inside their PaintCaption,
        // ensuring the painter owns the full caption look while reusing central drawing logic.
        internal void PaintBuiltInCaptionElements(Graphics g)
        {
           

            _iconRegion?.OnPaint?.Invoke(g, CurrentLayout.IconRect);
            // Draw theme and Style buttons if visible
            if (ShowThemeButton)
                _themeButton?.OnPaint?.Invoke(g, CurrentLayout.ThemeButtonRect);

            if (ShowStyleButton)
                _styleButton?.OnPaint?.Invoke(g, CurrentLayout.StyleButtonRect);

            // Draw custom action button if theme/Style not shown
            if (!ShowThemeButton && !ShowStyleButton)
                _customActionButton?.OnPaint?.Invoke(g, CurrentLayout.CustomActionButtonRect);

            // System buttons (skip default buttons for macOS which draws traffic lights)
            if (FormStyle != FormStyle.MacOS)
            {
                _minimizeButton?.OnPaint?.Invoke(g, CurrentLayout.MinimizeButtonRect);
                _maximizeButton?.OnPaint?.Invoke(g, CurrentLayout.MaximizeButtonRect);
                _closeButton?.OnPaint?.Invoke(g, CurrentLayout.CloseButtonRect);
            }
        }

        private Rectangle ResolveRegionRect(FormRegion region)
        {
            return region.Dock switch
            {
                RegionDock.Caption => new Rectangle(CurrentLayout.CaptionRect.Left, CurrentLayout.CaptionRect.Top, CurrentLayout.CaptionRect.Width, CurrentLayout.CaptionRect.Height),
                RegionDock.Bottom => new Rectangle(ClientRectangle.Left, ClientRectangle.Bottom - 24, ClientRectangle.Width, 24),
                RegionDock.Left => new Rectangle(ClientRectangle.Left, CurrentLayout.CaptionRect.Bottom, 24, ClientRectangle.Height - CurrentLayout.CaptionRect.Height),
                RegionDock.Right => new Rectangle(ClientRectangle.Right - 24, CurrentLayout.CaptionRect.Bottom, 24, ClientRectangle.Height - CurrentLayout.CaptionRect.Height),
                RegionDock.ContentOverlay => CurrentLayout.ContentRect,
                _ => region.Bounds
            };
        }        /// <summary>
        /// Sets up graphics quality settings for modern rendering with advanced features.
        /// CRITICAL: This method configures the Graphics context for form chrome painting.
        /// It should NOT affect child control rendering (especially text).
        /// </summary>
        private void SetupGraphicsQuality(Graphics g)
        {
            if (g == null) return;

            // Determine effective anti-aliasing mode based on rendering quality
            var effectiveAntiAliasMode = AntiAliasMode;
            if (RenderingQuality != RenderingQuality.Auto)
            {
                effectiveAntiAliasMode = RenderingQuality switch
                {
                    RenderingQuality.Performance => AntiAliasMode.None,
                    RenderingQuality.Balanced => AntiAliasMode.Low,
                    RenderingQuality.Quality => AntiAliasMode.High,
                    RenderingQuality.Ultra => AntiAliasMode.Ultra,
                    _ => AntiAliasMode
                };
            }

            // CRITICAL: Set CompositingMode to SourceOver for proper alpha blending
            // This ensures semi-transparent overlays blend correctly without accumulation
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

            // Apply anti-aliasing settings based on effective mode
            // NOTE: DO NOT set TextRenderingHint here! It affects child controls' text rendering.
            // The shared Graphics object in OnPaintBackground is used by all child controls.
            // Setting TextRenderingHint globally causes blurry text in labels, textboxes, etc.
            switch (effectiveAntiAliasMode)
            {
                case AntiAliasMode.None:
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Default;
                    break;

                case AntiAliasMode.Low:
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Default;
                    break;

                case AntiAliasMode.High:
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    break;

                case AntiAliasMode.Ultra:
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    break;
            }

            // Apply high contrast mode adjustments for accessibility
            if (HighContrastMode)
            {
                g.TextContrast = 12; // Maximum contrast (0-12 scale)
                // Force high-quality rendering for better visibility
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            }

            // Apply backdrop effect optimizations
            // These override base quality settings when specific effects are enabled
            switch (BackdropEffect)
            {
                case BackdropEffect.Mica:
                case BackdropEffect.MicaAlt:
                    // Mica effects benefit from high-quality compositing for subtle layering
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                    break;

                case BackdropEffect.Acrylic:
                    // Acrylic effects need high-quality blending for transparency
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    break;

                case BackdropEffect.Blur:
                    // Blur effects require high interpolation quality for smooth blending
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    break;
            }
        }

        /// <summary>
        /// Applies modern backdrop effects like Mica, Acrylic, and Blur
        /// </summary>
        private void ApplyBackdropEffect(Graphics g)
        {
            // Note: Full backdrop effects require Windows API calls and are simplified here
            // In a production implementation, this would use DWM API for Mica/Acrylic effects

            switch (BackdropEffect)
            {
                case BackdropEffect.Mica:
                    // Mica effect: Subtle background blur with slight tint
                    using (var brush = new SolidBrush(Color.FromArgb(20, 255, 255, 255)))
                    {
                        g.FillRectangle(brush, ClientRectangle);
                    }
                    break;

                case BackdropEffect.MicaAlt:
                    // Mica Alt: Alternative mica with different tint
                    using (var brush = new SolidBrush(Color.FromArgb(15, 0, 0, 0)))
                    {
                        g.FillRectangle(brush, ClientRectangle);
                    }
                    break;

                case BackdropEffect.Acrylic:
                    // Acrylic effect: More pronounced blur with stronger tint
                    using (var brush = new SolidBrush(Color.FromArgb(40, 255, 255, 255)))
                    {
                        g.FillRectangle(brush, ClientRectangle);
                    }
                    break;

                case BackdropEffect.Blur:
                    // Simple blur effect using semi-transparent overlay
                    using (var brush = new SolidBrush(Color.FromArgb(10, 255, 255, 255)))
                    {
                        g.FillRectangle(brush, ClientRectangle);
                    }
                    break;
            }
        }

        /// <summary>
        /// Handles micro-interactions for modern UI feel
        /// </summary>
        private void HandleMicroInteractions(string regionName, bool isHover)
        {
            if (!EnableMicroInteractions) return;

            // Trigger subtle animations based on interaction
            switch (regionName)
            {
                case "region:system:minimize":
                case "region:system:maximize":
                case "region:system:close":
                    // Button hover effects could trigger here
                    if (isHover && EnableAnimations)
                    {
                        // In a full implementation, this would start smooth color transitions
                        Invalidate();
                    }
                    break;

                case "region:system:caption":
                    // Caption hover for drag operations
                    break;
            }
        }

        /// <summary>
        /// Applies focus indicators for accessibility
        /// </summary>
        private void DrawFocusIndicator(Graphics g, Rectangle bounds)
        {
            if (FocusIndicatorStyle == FocusIndicatorStyle.None) return;

            using var focusPen = FocusIndicatorStyle switch
            {
                FocusIndicatorStyle.Subtle => new Pen(Color.FromArgb(100, SystemColors.Highlight), 1),
                FocusIndicatorStyle.Prominent => new Pen(SystemColors.Highlight, 2),
                FocusIndicatorStyle.HighContrast => new Pen(SystemColors.WindowText, 2),
                _ => new Pen(SystemColors.Highlight, 1)
            };

            // Draw focus rectangle with rounded corners for modern look
            var focusRect = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2);
            using var path = CreateRoundedRectanglePath(focusRect, new CornerRadius(2));
            g.DrawPath(focusPen, path);
        }

        /// <summary>
        /// Creates a rounded rectangle graphics path
        /// </summary>
        private System.Drawing.Drawing2D.GraphicsPath CreateRoundedRectanglePath(Rectangle rect, CornerRadius radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();

            // Validate rectangle dimensions
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(new Rectangle(rect.X, rect.Y, Math.Max(1, rect.Width), Math.Max(1, rect.Height)));
                return path;
            }

            // Clamp radius values to prevent oversized corners
            int maxRadius = Math.Min(rect.Width, rect.Height) / 2;
            int topLeft = Math.Max(0, Math.Min(radius.TopLeft, maxRadius));
            int topRight = Math.Max(0, Math.Min(radius.TopRight, maxRadius));
            int bottomRight = Math.Max(0, Math.Min(radius.BottomRight, maxRadius));
            int bottomLeft = Math.Max(0, Math.Min(radius.BottomLeft, maxRadius));

            // If all radii are zero, just add a rectangle
            if (topLeft == 0 && topRight == 0 && bottomRight == 0 && bottomLeft == 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            // Top-left corner
            if (topLeft > 0)
                path.AddArc(rect.X, rect.Y, topLeft * 2, topLeft * 2, 180, 90);
            else
                path.AddLine(rect.X, rect.Y, rect.X, rect.Y);

            // Top-right corner
            if (topRight > 0)
                path.AddArc(rect.Right - topRight * 2, rect.Y, topRight * 2, topRight * 2, 270, 90);
            else
                path.AddLine(rect.Right, rect.Y, rect.Right, rect.Y);

            // Bottom-right corner
            if (bottomRight > 0)
                path.AddArc(rect.Right - bottomRight * 2, rect.Bottom - bottomRight * 2, bottomRight * 2, bottomRight * 2, 0, 90);
            else
                path.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Bottom);

            // Bottom-left corner
            if (bottomLeft > 0)
                path.AddArc(rect.X, rect.Bottom - bottomLeft * 2, bottomLeft * 2, bottomLeft * 2, 90, 90);
            else
                path.AddLine(rect.X, rect.Bottom, rect.X, rect.Bottom);

            path.CloseFigure();
            return path;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            // Skip interaction in design mode or if control is disposed
            if (InDesignMode || IsDisposed || !IsHandleCreated) return;
            
            // Only handle mouse events outside the content rect
            var pos = PointToClient(Cursor.Position);
            if (!_layout.ContentRect.Contains(pos))
            {
                _interact.OnMouseMove(pos);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            // Skip interaction in design mode or if control is disposed
            if (InDesignMode || IsDisposed || !IsHandleCreated) return;
            
            // Only handle mouse events outside the content rect
            var pos = PointToClient(Cursor.Position);
            if (!_layout.ContentRect.Contains(pos))
            {
                _interact.OnMouseDown(pos);
            }
        }
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            
            // Skip interaction in design mode or if control is disposed
            if (InDesignMode || IsDisposed || !IsHandleCreated) return;
            
            // Only handle mouse events outside the content rect
            var pos = PointToClient(Cursor.Position);
            if (!_layout.ContentRect.Contains(pos))
            {
                _interact.OnMouseHover(pos);
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            
            // Skip interaction in design mode or if control is disposed
            if (InDesignMode || IsDisposed || !IsHandleCreated) return;
            
            // Only handle mouse events outside the content rect
            var pos = PointToClient(Cursor.Position);
            if (!_layout.ContentRect.Contains(pos))
            {
                _interact.OnMouseUp(pos);
            }
        }
    }
}
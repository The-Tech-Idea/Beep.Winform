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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            // Skip all custom painting in design mode
            if (InDesignMode)
            {
                // Simple design-time rendering
                e.Graphics.Clear(BackColor);
                using var pen = new Pen(Color.Gray, 1);
                e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, Width - 1, Height - 1));
                TextRenderer.DrawText(e.Graphics, Text ?? "BeepiFormPro", Font, new Point(8, 8), ForeColor);
                return;
            }

            // Sync global style used by BeepStyling
            BeepStyling.SetControlStyle(ControlStyle);

            // Layout
            _layout.Calculate();

            // Register hit areas
            _hits.Clear();
            _hits.Register("caption", _layout.CaptionRect);
            
            // Register built-in system button hit areas
            if (FormStyle == FormStyle.Modern || FormStyle == FormStyle.Minimal || FormStyle == FormStyle.Material)
            {
                _hits.Register("region:system:icon", _layout.IconRect, _iconRegion);
                _hits.Register("region:system:title", _layout.TitleRect, _titleRegion);
                
                // Register theme and style buttons if visible
                if (ShowThemeButton && _layout.ThemeButtonRect.Width > 0)
                    _hits.Register("region:system:theme", _layout.ThemeButtonRect, _themeButton);
                    
                if (ShowStyleButton && _layout.StyleButtonRect.Width > 0)
                    _hits.Register("region:system:style", _layout.StyleButtonRect, _styleButton);
                
                // Register custom action button if theme/style not shown
                if (!ShowThemeButton && !ShowStyleButton && _layout.CustomActionButtonRect.Width > 0)
                    _hits.Register("region:custom:action", _layout.CustomActionButtonRect, _customActionButton);
                
                _hits.Register("region:system:minimize", _layout.MinimizeButtonRect, _minimizeButton);
                _hits.Register("region:system:maximize", _layout.MaximizeButtonRect, _maximizeButton);
                _hits.Register("region:system:close", _layout.CloseButtonRect, _closeButton);
            }

            // Register custom regions
            foreach (var region in _regions)
            {
                var rect = ResolveRegionRect(region);
                _hits.Register($"region:{region.Id}", rect, region);
            }

            // Paint
            if (ActivePainter != null)
            {
                // Paint the background excluding ContentRect
                var backgroundRegion = new Region(ClientRectangle);
                backgroundRegion.Exclude(_layout.ContentRect);
                e.Graphics.Clip = backgroundRegion;
                ActivePainter.PaintBackground(e.Graphics, this);
                e.Graphics.ResetClip();

                // Paint the caption bar
                if (ShowCaptionBar)
                {
                    var captionRect = _layout.CaptionRect;
                    ActivePainter.PaintCaption(e.Graphics, this, captionRect);
                }

                // Paint the borders
                ActivePainter.PaintBorders(e.Graphics, this);
            }

            // Draw built-in regions
            if (FormStyle == FormStyle.Modern || FormStyle == FormStyle.Minimal || FormStyle == FormStyle.Material)
            {
                _iconRegion?.OnPaint?.Invoke(e.Graphics, _layout.IconRect);
                _titleRegion?.OnPaint?.Invoke(e.Graphics, _layout.TitleRect);
                
                // Draw theme and style buttons if visible
                if (ShowThemeButton)
                    _themeButton?.OnPaint?.Invoke(e.Graphics, _layout.ThemeButtonRect);
                    
                if (ShowStyleButton)
                    _styleButton?.OnPaint?.Invoke(e.Graphics, _layout.StyleButtonRect);
                
                // Draw custom action button if theme/style not shown
                if (!ShowThemeButton && !ShowStyleButton)
                    _customActionButton?.OnPaint?.Invoke(e.Graphics, _layout.CustomActionButtonRect);
                
                _minimizeButton?.OnPaint?.Invoke(e.Graphics, _layout.MinimizeButtonRect);
                _maximizeButton?.OnPaint?.Invoke(e.Graphics, _layout.MaximizeButtonRect);
                _closeButton?.OnPaint?.Invoke(e.Graphics, _layout.CloseButtonRect);
            }

            // Draw custom regions
            foreach (var region in _regions)
            {
                var rect = ResolveRegionRect(region);
                region.OnPaint?.Invoke(e.Graphics, rect);
            }
        }

        private Rectangle ResolveRegionRect(FormRegion region)
        {
            return region.Dock switch
            {
                RegionDock.Caption => new Rectangle(_layout.CaptionRect.Left, _layout.CaptionRect.Top, _layout.CaptionRect.Width, _layout.CaptionRect.Height),
                RegionDock.Bottom => new Rectangle(ClientRectangle.Left, ClientRectangle.Bottom - 24, ClientRectangle.Width, 24),
                RegionDock.Left => new Rectangle(ClientRectangle.Left, _layout.CaptionRect.Bottom, 24, ClientRectangle.Height - _layout.CaptionRect.Height),
                RegionDock.Right => new Rectangle(ClientRectangle.Right - 24, _layout.CaptionRect.Bottom, 24, ClientRectangle.Height - _layout.CaptionRect.Height),
                RegionDock.ContentOverlay => _layout.ContentRect,
                _ => region.Bounds
            };
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            // Skip interaction in design mode
            if (InDesignMode) return;
            
            // Only handle mouse events outside the content rect
            if (!_layout.ContentRect.Contains(e.Location))
            {
                _interact.OnMouseMove(e.Location);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            // Skip interaction in design mode
            if (InDesignMode) return;
            
            // Only handle mouse events outside the content rect
            if (!_layout.ContentRect.Contains(e.Location))
            {
                _interact.OnMouseDown(e.Location);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            
            // Skip interaction in design mode
            if (InDesignMode) return;
            
            // Only handle mouse events outside the content rect
            if (!_layout.ContentRect.Contains(e.Location))
            {
                _interact.OnMouseUp(e.Location);
            }
        }
    }
}
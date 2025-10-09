using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public partial class BeepiFormPro
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

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
            ActivePainter?.PaintBackground(e.Graphics, this);
            ActivePainter?.PaintCaption(e.Graphics, this, _layout.CaptionRect);
            ActivePainter?.PaintBorders(e.Graphics, this);

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
            _interact.OnMouseMove(e.Location);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _interact.OnMouseDown(e.Location);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _interact.OnMouseUp(e.Location);
        }
    }
}
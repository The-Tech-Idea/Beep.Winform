using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public partial class BeepiFormPro
    {
        // Style properties
        public FormStyle FormStyle { get; set; } = FormStyle.Modern;
        
        // Painters
        public IFormPainter ActivePainter { get; set; }
        public List<IFormPainter> Painters { get; } = new();

        // Regions API
        private readonly List<FormRegion> _regions = new();

        // Managers
        internal BeepiFormProLayoutManager _layout;
        internal BeepiFormProHitAreaManager _hits;
        internal BeepiFormProInteractionManager _interact;

        // Built-in caption elements
        private FormRegion _iconRegion;
        private FormRegion _titleRegion;
        private FormRegion _minimizeButton;
        private FormRegion _maximizeButton;
        private FormRegion _closeButton;
        private FormRegion _customActionButton; // New: custom clickable region
        private FormRegion _themeButton;
        private FormRegion _styleButton;

        // Button visibility flags
        private bool _showThemeButton = false;
        private bool _showStyleButton = false;

        // Events for region interaction
        public event EventHandler<RegionEventArgs> RegionHover;
        public event EventHandler<RegionEventArgs> RegionClick;
        
        // Events for button actions
        public event EventHandler ThemeButtonClicked;
        public event EventHandler StyleButtonClicked;

        // DPI scaling factor
        private float _dpiScale = 1.0f;

        // Caption bar properties
        private bool _showCaptionBar = true;
        private int _captionHeight = 32;

        /// <summary>
        /// Gets or sets whether to show the caption bar
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue(true)]
        public bool ShowCaptionBar
        {
            get => _showCaptionBar;
            set
            {
                if (_showCaptionBar != value)
                {
                    _showCaptionBar = value;
                    Invalidate();
                    PerformLayout();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the caption bar
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue(32)]
        public int CaptionHeight
        {
            get => _captionHeight;
            set
            {
                if (_captionHeight != value && value >= 24)
                {
                    _captionHeight = value;
                    Invalidate();
                    PerformLayout();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show the theme button in the caption bar
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue(false)]
        public bool ShowThemeButton
        {
            get => _showThemeButton;
            set
            {
                if (_showThemeButton != value)
                {
                    _showThemeButton = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show the style button in the caption bar
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue(false)]
        public bool ShowStyleButton
        {
            get => _showStyleButton;
            set
            {
                if (_showStyleButton != value)
                {
                    _showStyleButton = value;
                    Invalidate();
                }
            }
        }

        // Public API to register regions
        public void AddRegion(FormRegion region)
        {
            if (region == null) return;
            _regions.Add(region);
            Invalidate();
        }

        public void ClearRegions()
        {
            _regions.Clear();
            Invalidate();
        }

        private void UpdateDpiScale()
        {
            using (var g = CreateGraphics())
            {
                _dpiScale = g.DpiX / 96f; // 96 DPI is the baseline (100% scaling)
            }
        }

        public int ScaleDpi(int value) => (int)(value * _dpiScale);

        private void InitializeBuiltInRegions()
        {
            // Icon region (left side of caption)
            _iconRegion = new FormRegion
            {
                Id = "system:icon",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (Icon != null && r.Width > 0 && r.Height > 0)
                    {
                        int size = Math.Min(r.Width, r.Height) - 4;
                        var iconRect = new Rectangle(r.Left + 2, r.Top + (r.Height - size) / 2, size, size);
                        g.DrawIcon(Icon, iconRect);
                    }
                }
            };

            // Title region (center of caption)
            _titleRegion = new FormRegion
            {
                Id = "system:title",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (string.IsNullOrEmpty(Text) || r.Width <= 0 || r.Height <= 0) return;
                    var style = BeepStyling.GetControlStyle();
                    var fg = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetForeground(style);
                    TextRenderer.DrawText(g, Text, Font, r, fg,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                }
            };

            // System buttons for Modern/Minimal form styles
            int btnSize = 32;
            _minimizeButton = new FormRegion
            {
                Id = "system:minimize",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) => DrawSystemButton(g, r, "−", _interact.IsHovered(_hits.Areas.FirstOrDefault(a => a.Name == "region:system:minimize")))
            };

            _maximizeButton = new FormRegion
            {
                Id = "system:maximize",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) => DrawSystemButton(g, r, WindowState == FormWindowState.Maximized ? "❐" : "□", _interact.IsHovered(_hits.Areas.FirstOrDefault(a => a.Name == "region:system:maximize")))
            };

            _closeButton = new FormRegion
            {
                Id = "system:close",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) => DrawSystemButton(g, r, "✕", _interact.IsHovered(_hits.Areas.FirstOrDefault(a => a.Name == "region:system:close")), true)
            };

            // Custom action button (between title and system buttons)
            _customActionButton = new FormRegion
            {
                Id = "custom:action",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (r.Width <= 0 || r.Height <= 0) return;
                    var style = BeepStyling.GetControlStyle();
                    var isHovered = _interact.IsHovered(_hits.Areas.FirstOrDefault(a => a.Name == "region:custom:action"));
                    var isPressed = _interact.IsPressed(_hits.Areas.FirstOrDefault(a => a.Name == "region:custom:action"));
                    
                    // Background on hover/press
                    if (isPressed)
                    {
                        var pressed = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetPressed(style);
                        using var brush = new SolidBrush(pressed);
                        g.FillRectangle(brush, r);
                    }
                    else if (isHovered)
                    {
                        var hover = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetHover(style);
                        using var brush = new SolidBrush(hover);
                        g.FillRectangle(brush, r);
                    }

                    // Draw icon (⚙ gear/settings icon)
                    var fg = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetForeground(style);
                    using var font = new Font("Segoe UI Symbol", Font.Size + 2, FontStyle.Regular);
                    TextRenderer.DrawText(g, "⚙", font, r, fg,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            };

            // Theme button (palette icon)
            _themeButton = new FormRegion
            {
                Id = "system:theme",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (r.Width <= 0 || r.Height <= 0 || !_showThemeButton) return;
                    var style = BeepStyling.GetControlStyle();
                    var isHovered = _interact.IsHovered(_hits.Areas.FirstOrDefault(a => a.Name == "region:system:theme"));
                    var isPressed = _interact.IsPressed(_hits.Areas.FirstOrDefault(a => a.Name == "region:system:theme"));
                    
                    // Background on hover/press
                    if (isPressed)
                    {
                        var pressed = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetPressed(style);
                        using var brush = new SolidBrush(pressed);
                        g.FillRectangle(brush, r);
                    }
                    else if (isHovered)
                    {
                        var hover = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetHover(style);
                        using var brush = new SolidBrush(hover);
                        g.FillRectangle(brush, r);
                    }

                    // Draw icon (🎨 palette icon)
                    var fg = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetForeground(style);
                    using var font = new Font("Segoe UI Emoji", Font.Size, FontStyle.Regular);
                    TextRenderer.DrawText(g, "🎨", font, r, fg,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            };

            // Style button (layout icon)
            _styleButton = new FormRegion
            {
                Id = "system:style",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (r.Width <= 0 || r.Height <= 0 || !_showStyleButton) return;
                    var style = BeepStyling.GetControlStyle();
                    var isHovered = _interact.IsHovered(_hits.Areas.FirstOrDefault(a => a.Name == "region:system:style"));
                    var isPressed = _interact.IsPressed(_hits.Areas.FirstOrDefault(a => a.Name == "region:system:style"));
                    
                    // Background on hover/press
                    if (isPressed)
                    {
                        var pressed = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetPressed(style);
                        using var brush = new SolidBrush(pressed);
                        g.FillRectangle(brush, r);
                    }
                    else if (isHovered)
                    {
                        var hover = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetHover(style);
                        using var brush = new SolidBrush(hover);
                        g.FillRectangle(brush, r);
                    }

                    // Draw icon (◧ layout/style icon)
                    var fg = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetForeground(style);
                    using var font = new Font("Segoe UI Symbol", Font.Size + 2, FontStyle.Regular);
                    TextRenderer.DrawText(g, "◧", font, r, fg,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            };
        }

        private void DrawSystemButton(Graphics g, Rectangle r, string symbol, bool isHover, bool isClose = false)
        {
            var style = BeepStyling.GetControlStyle();
            var fg = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetForeground(style);
            var hover = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetHover(style);
            var closeColor = Color.FromArgb(232, 17, 35); // Windows red

            if (isHover)
            {
                using var brush = new SolidBrush(isClose ? closeColor : hover);
                g.FillRectangle(brush, r);
                fg = Color.White;
            }

            using var font = new Font(Font.FontFamily, Font.Size + 2, FontStyle.Regular);
            TextRenderer.DrawText(g, symbol, font, r, fg,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        protected void OnRegionClicked(HitArea area)
        {
            if (area?.Name == null) return;

            // Raise event for extensibility
            var regionData = area.Data as FormRegion;
            RegionClick?.Invoke(this, new RegionEventArgs(area.Name, regionData, area.Bounds));

            switch (area.Name)
            {
                case "region:system:minimize":
                    WindowState = FormWindowState.Minimized;
                    break;

                case "region:system:maximize":
                    WindowState = WindowState == FormWindowState.Maximized 
                        ? FormWindowState.Normal 
                        : FormWindowState.Maximized;
                    break;

                case "region:system:close":
                    Close();
                    break;

                case "region:custom:action":
                    // Custom action button clicked - override or subscribe to event
                    OnCustomActionClicked();
                    break;

                case "region:system:theme":
                    // Theme button clicked
                    ThemeButtonClicked?.Invoke(this, EventArgs.Empty);
                    break;

                case "region:system:style":
                    // Style button clicked
                    StyleButtonClicked?.Invoke(this, EventArgs.Empty);
                    break;

                case "caption":
                    // Allow window dragging
                    if (WindowState == FormWindowState.Normal)
                    {
                        ReleaseCapture();
                        SendMessage(Handle, 0xA1, 0x2, 0);
                    }
                    break;
            }
        }

        protected virtual void OnCustomActionClicked()
        {
            // Override in derived class or subscribe to RegionClick event
            MessageBox.Show("Custom action button clicked! Override OnCustomActionClicked or subscribe to RegionClick event.", 
                "BeepiFormPro", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // P/Invoke for window dragging
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    }
}
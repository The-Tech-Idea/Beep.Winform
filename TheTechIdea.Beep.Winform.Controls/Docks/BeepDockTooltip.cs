using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Docks.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Docks
{
    /// <summary>
    /// Modern tooltip for dock items with smooth animations and rich content
    /// Inspired by modern web frameworks and macOS tooltips
    /// </summary>
    public class BeepDockTooltip : Form
    {
        private SimpleItem _item;
        private IBeepTheme _theme;
        private Timer _fadeTimer;
        private float _opacity = 0f;
        private bool _fadingIn = true;
        private const int FadeSteps = 10;
        private const int FadeInterval = 20;
        private const int TooltipPadding = 12;
        private const int MaxWidth = 300;
        private const int CornerRadius = 8;

        // Content
        private string _title;
        private string _description;
        private string _shortcut;
        private bool _showPreview;

        public BeepDockTooltip(SimpleItem item, IBeepTheme theme)
        {
            _item = item;
            _theme = theme;
            _title = item?.Text ?? "";
            _description = item?.Description ?? "";
            _shortcut = item?.SubText ?? ""; // Use SubText for shortcut

            InitializeTooltip();
        }

        private void InitializeTooltip()
        {
            // Window setup
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            Opacity = 0;

            // Make it a tool window
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            DoubleBuffered = true;

            // Calculate size based on content
            CalculateSize();

            // Fade animation
            _fadeTimer = new Timer { Interval = FadeInterval };
            _fadeTimer.Tick += FadeTimer_Tick;
        }

        private void CalculateSize()
        {
            using (var g = CreateGraphics())
            {
                var titleFont = new Font("Segoe UI Semibold", 10f);
                var bodyFont = new Font("Segoe UI", 9f);
                var shortcutFont = new Font("Segoe UI", 8.5f);

                // Measure text
                var titleSize = g.MeasureString(_title, titleFont, MaxWidth - TooltipPadding * 2);
                var descSize = string.IsNullOrEmpty(_description) 
                    ? SizeF.Empty 
                    : g.MeasureString(_description, bodyFont, MaxWidth - TooltipPadding * 2);
                var shortcutSize = string.IsNullOrEmpty(_shortcut)
                    ? SizeF.Empty
                    : g.MeasureString(_shortcut, shortcutFont);

                // Calculate dimensions
                int width = Math.Max(
                    (int)titleSize.Width,
                    Math.Max((int)descSize.Width, (int)shortcutSize.Width)
                ) + TooltipPadding * 2;

                width = Math.Min(width, MaxWidth);

                int height = TooltipPadding;
                height += (int)titleSize.Height;
                
                if (!string.IsNullOrEmpty(_description))
                    height += 4 + (int)descSize.Height;
                
                if (!string.IsNullOrEmpty(_shortcut))
                    height += 8 + (int)shortcutSize.Height + 4;
                
                height += TooltipPadding;

                Size = new Size(width, height);
            }
        }

        public void ShowTooltip(Point screenLocation, int delay = 500)
        {
            Location = CalculatePosition(screenLocation);
            
            // Delay before showing
            if (delay > 0)
            {
                var delayTimer = new Timer { Interval = delay };
                delayTimer.Tick += (s, e) =>
                {
                    delayTimer.Stop();
                    delayTimer.Dispose();
                    Show();
                    _fadeTimer.Start();
                };
                delayTimer.Start();
            }
            else
            {
                Show();
                _fadeTimer.Start();
            }
        }

        private Point CalculatePosition(Point itemLocation)
        {
            // Position tooltip above the item with some spacing
            int x = itemLocation.X - Width / 2;
            int y = itemLocation.Y - Height - 12;

            // Ensure tooltip stays on screen
            var screen = Screen.FromPoint(itemLocation);
            
            if (x < screen.WorkingArea.Left)
                x = screen.WorkingArea.Left + 8;
            else if (x + Width > screen.WorkingArea.Right)
                x = screen.WorkingArea.Right - Width - 8;

            if (y < screen.WorkingArea.Top)
                y = itemLocation.Y + 48; // Show below if no room above

            return new Point(x, y);
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            if (_fadingIn)
            {
                _opacity += 1f / FadeSteps;
                if (_opacity >= 1f)
                {
                    _opacity = 1f;
                    _fadeTimer.Stop();
                }
            }
            else
            {
                _opacity -= 1f / FadeSteps;
                if (_opacity <= 0f)
                {
                    _opacity = 0f;
                    _fadeTimer.Stop();
                    Close();
                    return;
                }
            }

            Opacity = _opacity;
        }

        public void HideTooltip()
        {
            _fadingIn = false;
            _fadeTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var bounds = new Rectangle(0, 0, Width, Height);

            // Modern tooltip style with subtle gradient
            using (var path = CreateRoundedPath(bounds, CornerRadius))
            {
                // Background
                var bgColor = _theme?.BackColor ?? Color.FromArgb(40, 40, 45);
                var bgColor1 = Color.FromArgb(250, bgColor.R, bgColor.G, bgColor.B);
                var bgColor2 = Color.FromArgb(250, 
                    Math.Max(0, bgColor.R - 8), 
                    Math.Max(0, bgColor.G - 8), 
                    Math.Max(0, bgColor.B - 8));

                using (var bgBrush = new LinearGradientBrush(bounds, bgColor1, bgColor2, 90f))
                {
                    g.FillPath(bgBrush, path);
                }

                // Border
                var borderColor = _theme?.BorderColor ?? Color.FromArgb(100, 255, 255, 255);
                using (var pen = new Pen(Color.FromArgb(150, borderColor), 1f))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Shadow
            PaintShadow(g, bounds);

            // Content
            PaintContent(g, bounds);
        }

        private void PaintShadow(Graphics g, Rectangle bounds)
        {
            // Outer glow/shadow
            for (int i = 3; i > 0; i--)
            {
                var shadowBounds = bounds;
                shadowBounds.Inflate(i, i);
                
                using (var path = CreateRoundedPath(shadowBounds, CornerRadius + i))
                {
                    int alpha = 20 - (i * 5);
                    using (var pen = new Pen(Color.FromArgb(alpha, 0, 0, 0), 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        private void PaintContent(Graphics g, Rectangle bounds)
        {
            int y = TooltipPadding;
            int x = TooltipPadding;
            int contentWidth = bounds.Width - TooltipPadding * 2;

            var textColor = _theme?.ForeColor ?? Color.FromArgb(240, 240, 240);

            // Title
            using (var titleFont = new Font("Segoe UI Semibold", 10f))
            using (var titleBrush = new SolidBrush(textColor))
            {
                var titleRect = new RectangleF(x, y, contentWidth, bounds.Height);
                g.DrawString(_title, titleFont, titleBrush, titleRect);
                
                var titleSize = g.MeasureString(_title, titleFont, contentWidth);
                y += (int)titleSize.Height;
            }

            // Description
            if (!string.IsNullOrEmpty(_description))
            {
                y += 4;
                using (var bodyFont = new Font("Segoe UI", 9f))
                using (var bodyBrush = new SolidBrush(Color.FromArgb(200, textColor)))
                {
                    var descRect = new RectangleF(x, y, contentWidth, bounds.Height - y);
                    g.DrawString(_description, bodyFont, bodyBrush, descRect);
                    
                    var descSize = g.MeasureString(_description, bodyFont, contentWidth);
                    y += (int)descSize.Height;
                }
            }

            // Keyboard shortcut
            if (!string.IsNullOrEmpty(_shortcut))
            {
                y += 8;
                using (var shortcutFont = new Font("Segoe UI", 8.5f))
                {
                    var shortcutSize = g.MeasureString(_shortcut, shortcutFont);
                    var shortcutRect = new RectangleF(
                        bounds.Width - TooltipPadding - shortcutSize.Width,
                        y,
                        shortcutSize.Width + 8,
                        shortcutSize.Height + 4
                    );

                    // Shortcut badge background
                    using (var badgePath = CreateRoundedPath(
                        Rectangle.Round(shortcutRect), 4))
                    {
                        var badgeColor = _theme?.ButtonBackColor ?? Color.FromArgb(60, 60, 65);
                        using (var badgeBrush = new SolidBrush(Color.FromArgb(100, badgeColor)))
                        {
                            g.FillPath(badgeBrush, badgePath);
                        }
                    }

                    // Shortcut text
                    using (var shortcutBrush = new SolidBrush(Color.FromArgb(180, textColor)))
                    {
                        g.DrawString(_shortcut, shortcutFont, shortcutBrush,
                            shortcutRect.X + 4, shortcutRect.Y + 2);
                    }
                }
            }
        }

        private GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));
            var arc = new Rectangle(bounds.X, bounds.Y, diameter, diameter);

            // Top-left
            path.AddArc(arc, 180, 90);

            // Top-right
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom-right
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom-left
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x80;  // WS_EX_TOOLWINDOW
                cp.ExStyle |= 0x20;  // WS_EX_TRANSPARENT (mouse pass-through)
                return cp;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _fadeTimer?.Stop();
                _fadeTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}


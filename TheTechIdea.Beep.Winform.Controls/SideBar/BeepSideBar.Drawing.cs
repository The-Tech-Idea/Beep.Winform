using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.SideBar
{
    /// <summary>
    /// Partial class for BeepSideBar drawing logic
    /// </summary>
    public partial class BeepSideBar
    {
        #region Drawing Fields
        private Graphics _currentGraphics;
        #endregion

        #region Drawing Methods
        /// <summary>
        /// Override DrawContent to delegate to the current painter
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            // Ensure painter initialized
            if (_currentPainter == null)
            {
                InitializePainter();
            }

            // When using theme, keep background in sync
            if (UseThemeColors && _currentTheme != null)
            {
                BackColor = _currentTheme.SideMenuBackColor;
            }

            // Always ensure background is painted once per pass
            BeepStyling.PaintStyleBackground(g, DrawingRect, Style);

            _currentGraphics = g;

            // Enable high-quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Design-time: avoid calling full painters which may read files or do heavy work.
            if (DesignMode)
            {
                // Fast placeholder rendering for Visual Studio Designer to prevent IDE freeze
                PaintBackgroundPerStyle(g);
                // Optionally draw a lightweight mock list to help the designer visualize the control
                var font = BeepFontManager.GetCachedFont("Segoe UI", 9f);
                using (var pen = new Pen(Color.FromArgb(80, Color.Gray), 1f))
                using (var brush = new SolidBrush(Color.FromArgb(100, Color.Gray)))
                {
                    int y = DrawingRect.Top + 12;
                    for (int i = 0; i < Math.Min(4, Math.Max(1, Items?.Count ?? 0)); i++)
                    {
                        g.DrawRectangle(pen, DrawingRect.Left + 12, y, DrawingRect.Width - 24, Math.Max(22, ItemHeight - 12));
                        g.DrawString("Item", font, brush, DrawingRect.Left + 20, y + 2);
                        y += ItemHeight + 4;
                    }
                }
                return; // Skip full painter work in designer
            }

            // Runtime: measured painter call with safety guard
            try
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                _currentPainter?.Paint(_painterContext);
                sw.Stop();
                // If a painter takes too long, log a warning - helps find bad painters
                const int warningMs = 300; // arbitrary threshold for noticing slow paints
                if (sw.ElapsedMilliseconds > warningMs)
                {
                    System.Diagnostics.Debug.WriteLine($"Warning: SideBar painter {(_currentPainter?.Name ?? "unknown")} took {sw.ElapsedMilliseconds}ms to paint.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BeepSideBar Paint Error: {ex.Message}");
                // Fallback to basic rendering in case painters fail
                using (var brush = new SolidBrush(BackColor))
                {
                    g.FillRectangle(brush, DrawingRect);
                }
            }
        }
        /// <summary>
        /// Paint background color based on the selected Style
        /// Each Style has a distinct, recognizable background color
        /// </summary>
        protected void PaintBackgroundPerStyle(Graphics g)
        {
            Color backColor = Color.FromArgb(255, 251, 254); // Default fallback
            bool useGradient = false;
            Color gradientStart = Color.Empty;
            Color gradientEnd = Color.Empty;

            switch (Style)
            {
                case BeepControlStyle.Material3:
                    // Material 3 - Soft lavender tonal surface
                    backColor = Color.FromArgb(255, 251, 254);
                    break;

                case BeepControlStyle.iOS15:
                    // iOS 15 - Light gray with slight blue tint
                    backColor = Color.FromArgb(242, 242, 247);
                    break;

                case BeepControlStyle.Fluent2:
                    // Fluent 2 - Warm neutral gray (Microsoft Style)
                    backColor = Color.FromArgb(243, 242, 241);
                    break;

                case BeepControlStyle.Minimal:
                    // Minimal - Pure light gray
                    backColor = Color.FromArgb(250, 250, 250);
                    break;

                case BeepControlStyle.AntDesign:
                    // Ant Design - Slightly warm white
                    backColor = Color.FromArgb(250, 250, 250);
                    break;

                case BeepControlStyle.MaterialYou:
                    // Material You - Pink-tinted surface
                    backColor = Color.FromArgb(255, 248, 250);
                    break;

                case BeepControlStyle.Windows11Mica:
                    // Windows 11 Mica - Cool gray with transparency feel
                    backColor = Color.FromArgb(248, 248, 248);
                    break;

                case BeepControlStyle.MacOSBigSur:
                    // macOS Big Sur - Clean white with subtle warmth
                    backColor = Color.FromArgb(252, 252, 252);
                    break;

                case BeepControlStyle.ChakraUI:
                    // Chakra UI - Soft blue-gray
                    backColor = Color.FromArgb(247, 250, 252);
                    break;

                case BeepControlStyle.TailwindCard:
                    // Tailwind - Pure white
                    backColor = Color.FromArgb(255, 255, 255);
                    break;

                case BeepControlStyle.NotionMinimal:
                    // Notion - Off-white with warmth
                    backColor = Color.FromArgb(251, 251, 250);
                    break;

                case BeepControlStyle.VercelClean:
                    // Vercel - Pure white (clean minimalism)
                    backColor = Color.FromArgb(255, 255, 255);
                    break;

                case BeepControlStyle.StripeDashboard:
                    // Stripe - Light blue-gray
                    backColor = Color.FromArgb(248, 250, 252);
                    break;

                case BeepControlStyle.DarkGlow:
                    // Dark Glow - Deep charcoal
                    backColor = Color.FromArgb(24, 24, 27);
                    break;

                case BeepControlStyle.DiscordStyle:
                    // Discord - Dark slate gray
                    backColor = Color.FromArgb(47, 49, 54);
                    break;

                case BeepControlStyle.GradientModern:
                    // Gradient Modern - Blue to cyan gradient
                    useGradient = true;
                    gradientStart = Color.FromArgb(58, 123, 213);
                    gradientEnd = Color.FromArgb(0, 210, 255);
                    break;

                default:
                    backColor = Color.FromArgb(250, 250, 250);
                    break;
            }

            // Apply the background
            if (useGradient)
            {
                using (var brush = new LinearGradientBrush(
                    DrawingRect, 
                    gradientStart, 
                    gradientEnd, 
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, DrawingRect);
                }
            }
            else
            {
                using (var brush = new SolidBrush(backColor))
                {
                    g.FillRectangle(brush, DrawingRect);
                }
            }
        }
        #endregion
    }
}


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

            if (_currentPainter == null)
            {
                InitializePainter();
            }
            if (UseThemeColors && _currentTheme != null)
            {
                BackColor = _currentTheme.SideMenuBackColor;
                g.Clear(BackColor);
            }
            else
            {
                // Paint background based on selected Style
                BeepStyling.PaintStyleBackground(g, DrawingRect, Style);
            }

            _currentGraphics = g;

            // Enable high-quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            try
            {
                // Let the painter handle all rendering
                _currentPainter?.Paint(_painterContext);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BeepSideBar Paint Error: {ex.Message}");
                // Fallback to basic rendering
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


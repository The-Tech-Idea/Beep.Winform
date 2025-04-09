using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Dashboard Metric Tile")]
    [Description("A dashboard tile showing a title, icon, large metric, delta text, and a central silhouette.")]
    public class BeepMetricTile : BeepControl
    {
        private string _titleText = "Views";
        private string _metricValue = "31";
        private string _deltaValue = "+3 last day";
        private Image _iconImage;              // For the top-right icon
        private Image _backgroundSilhouette;   // For the semi-transparent center image

        private string _iconImagepath;              // For the top-right icon
        private string _backgroundSilhouettepath;   // For the semi-transparent center image
        [Category("Appearance")]
        [Description("Title displayed in the top-left corner (e.g., 'Views').")]
        public string TitleText
        {
            get => _titleText;
            set { _titleText = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Main numeric or text metric (e.g., '31').")]
        public string MetricValue
        {
            get => _metricValue;
            set { _metricValue = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Delta or additional info (e.g., '+3 last day').")]
        public string DeltaValue
        {
            get => _deltaValue;
            set { _deltaValue = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Icon shown in the top-right corner (e.g., a home or stats icon).")]
        public string IconImage
        {
            get => _iconImagepath;
            set { _iconImagepath = value; _iconImage = (Image)ImageListHelper.GetImageFromName(_iconImagepath); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("A silhouette or watermark image placed in the center with partial transparency.")]
        public string BackgroundSilhouette
        {
            get => _backgroundSilhouettepath;
            set { _backgroundSilhouettepath = value; _backgroundSilhouette = (Image)ImageListHelper.GetImageFromName(_backgroundSilhouettepath); Invalidate(); }
        }

        public BeepMetricTile()
        {
            // Set up a size that closely matches the reference
            this.Size = new Size(150, 150);

            // Large corner radius to match the roundness in the image
            this.BorderRadius = 8;

            // Enable the built-in shadow and gradient background
            this.ShowShadow = true;
            this.UseGradientBackground = true;
            this.GradientDirection = LinearGradientMode.Vertical;

            // Example pastel gradient; tweak as desired
            this.GradientStartColor = Color.FromArgb(255, 235, 228, 255); // Light pink-lavender
            this.GradientEndColor = Color.FromArgb(255, 215, 233, 255); // Light lavender

            // Default foreground color for text
            this.ForeColor = Color.Black;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Base handles gradient, border, shadow
            base.OnPaint(e);

            var g = e.Graphics;
            UpdateDrawingRect();
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var clientRect = this.ClientRectangle;

            // 1) Draw a semi‐transparent silhouette in the center (optional)
            if (_backgroundSilhouette != null)
            {
                // We'll apply ~20% opacity via a color matrix
                using var ia = new ImageAttributes();
                var cm = new ColorMatrix();
                cm.Matrix33 = 0.20f;  // 0.20 = 20% opacity
                ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                // Scale the silhouette to about 60% of the control size
                int silhouetteWidth = (int)(DrawingRect.Width * 0.60);
                int silhouetteHeight = (int)(DrawingRect.Height * 0.60);
                int x = (DrawingRect.Width - silhouetteWidth) / 2;
                int y = (DrawingRect.Height - silhouetteHeight) / 2;

                var destRect = new Rectangle(x, y, silhouetteWidth, silhouetteHeight);

                g.DrawImage(
                    _backgroundSilhouette,
                    destRect,
                    0, 0,
                    _backgroundSilhouette.Width,
                    _backgroundSilhouette.Height,
                    GraphicsUnit.Pixel,
                    ia
                );
            }

            // 2) Draw the title in the top-left corner
            using (var titleFont = new Font(Font.FontFamily, 10, FontStyle.Regular))
            {
                TextRenderer.DrawText(
                    g,
                    _titleText,
                    titleFont,
                    new Point(10, 10),
                    _currentTheme.CardTextForeColor);
            }

            // 3) Draw the optional icon in the top-right corner
            if (_iconImage != null)
            {
                int iconSize = 24;
                int iconX = DrawingRect.Width - iconSize - 10;
                int iconY = 10; 
                g.DrawImage(_iconImage, new Rectangle(iconX, iconY, iconSize, iconSize));
            }

            // 4) Draw the large metric near the bottom-left
            using (var metricFont = new Font(Font.FontFamily, 28, FontStyle.Bold))
            {
                Size metricSize = TextRenderer.MeasureText(_metricValue, metricFont);

                // Place it about 10px from the left, 10px from the bottom
                int metricX = 10;
                int metricY = clientRect.Height - metricSize.Height - 10;

                TextRenderer.DrawText(
                    g,
                    _metricValue,
                    metricFont,
                    new Point(metricX, metricY),
                    _currentTheme.CardTitleForeColor);

                // 5) Draw the delta text to the right of the metric, baseline aligned
                using (var deltaFont = new Font(Font.FontFamily, 10, FontStyle.Regular))
                {
                    Size deltaSize = TextRenderer.MeasureText(_deltaValue, deltaFont);
                    int deltaX = metricX + metricSize.Width + 5;
                    int deltaY = metricY + (metricSize.Height - deltaSize.Height);
                    TextRenderer.DrawText(
                        g,
                        _deltaValue,
                        deltaFont,
                        new Point(deltaX, deltaY),
                        ForeColor);
                }
            }
        }
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            // Apply theme colors if needed
            // For example, you might want to set the background color based on the theme
            this.BackColor = _currentTheme.DashboardBackColor;
            
            this.GradientStartColor = _currentTheme.GradientStartColor;
            this.GradientEndColor = _currentTheme.GradientEndColor;
           
        }
    }
}

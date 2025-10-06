using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
 

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Dashboard Metric Tile")]
    [Description("A dashboard tile showing a title, icon, large metric, delta text, and a central silhouette.")]
    public class BeepMetricTile : BeepControl
    {

        // Controls Used for Drawing and theming
        private BeepButton button;
        private BeepLabel label;
        private BeepImage image;

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

          
        }
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            UpdateDrawingRect();
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var DrawingRect = this.DrawingRect;
            var clientRect = this.ClientRectangle;
            ClearHitList();
            // Get card theme (assume _currentTheme is CandyTheme or compatible)
            var cardTheme = _currentTheme;

            // 1. Draw the background silhouette (center, semi-transparent)
            if (_backgroundSilhouette != null)
            {
                using var ia = new ImageAttributes();
                var cm = new ColorMatrix();
                cm.Matrix33 = 0.20f;
                ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                int silhouetteWidth = (int)(DrawingRect.Width * 0.60);
                int silhouetteHeight = (int)(DrawingRect.Height * 0.60);
                int x = DrawingRect.Left + (DrawingRect.Width - silhouetteWidth) / 2;
                int y = DrawingRect.Top + (DrawingRect.Height - silhouetteHeight) / 2;
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

            // 2. Draw the title (top-left) using BeepLabel
            if (label == null)
                label = new BeepLabel();
            label.Text = _titleText;
            label.ForeColor = cardTheme?.CardTitleForeColor ?? Color.Black;
            label.Font = cardTheme != null
                ? new Font(cardTheme.CardTitleFont.FontFamily, cardTheme.CardTitleFont.FontSize, cardTheme.CardTitleFont.FontStyle)
                : new Font(Font.FontFamily, 10, FontStyle.Bold);
            label.BackColor = Color.Transparent;
            label.IsChild = true;
            var titleRect = new Rectangle(DrawingRect.Left + 10, DrawingRect.Top + 10, DrawingRect.Width - 40, 24);
            label.Draw(g, titleRect);

            // 3. Draw the icon (top-right) using BeepImage
            if (image == null)
                image = new BeepImage();
            if (_iconImage != null)
                image.Image = _iconImage;
            else if (!string.IsNullOrEmpty(_iconImagepath))
                image.ImagePath = _iconImagepath;
            image.IsChild = true;
            int iconSize = 24;
            var iconRect = new Rectangle(DrawingRect.Right - iconSize - 10, DrawingRect.Top + 10, iconSize, iconSize);
            image.Draw(g, iconRect);

            // Optionally, add a hit area for the icon
            AddHitArea("Icon", iconRect);

            // 4. Draw the metric value (bottom-left) using BeepLabel
            if (button == null)
                button = new BeepButton();
            button.Text = _metricValue;
            button.ForeColor = cardTheme?.CardTitleForeColor ?? Color.Black;
            button.Font = cardTheme != null
                ? new Font(cardTheme.CardTitleFont.FontFamily, 28, FontStyle.Bold)
                : new Font(Font.FontFamily, 28, FontStyle.Bold);
            button.BackColor = Color.Transparent;
            button.IsChild = true;
            var metricRect = new Rectangle(DrawingRect.Left + 10, DrawingRect.Bottom - 50, DrawingRect.Width / 2, 40);
            button.Draw(g, metricRect);

            // 5. Draw the delta value (right of metric) using BeepLabel
            var deltaLabel = new BeepLabel
            {
                Text = _deltaValue,
                ForeColor = cardTheme?.CardSubTitleForeColor ?? Color.Gray,
                Font = cardTheme != null
                    ? new Font(cardTheme.CardSubTitleFont.FontFamily, 10, cardTheme.CardSubTitleFont.FontStyle)
                    : new Font(Font.FontFamily, 10, FontStyle.Regular),
                BackColor = Color.Transparent,
                IsChild = true
            };
            var deltaRect = new Rectangle(metricRect.Right + 5, metricRect.Top + 10, DrawingRect.Width - metricRect.Right - 15, 24);
            deltaLabel.Draw(g, deltaRect);

            // Optionally, add hit areas for metric and delta
            AddHitArea("Metric", metricRect);
            AddHitArea("Delta", deltaRect);
        }
        public override void ApplyTheme()
        {
        //    base.ApplyTheme();
            // Apply theme colors if needed
            // For example, you might want to set the background color based on the theme
            this.BackColor = _currentTheme.DashboardBackColor;
            
            this.GradientStartColor = _currentTheme.GradientStartColor;
            this.GradientEndColor = _currentTheme.GradientEndColor;
           
        }
    }
}

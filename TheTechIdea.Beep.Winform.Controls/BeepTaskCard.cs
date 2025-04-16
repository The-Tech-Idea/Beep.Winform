using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Task Card")]
    [Description("A task or project card displaying avatars, title, subtitle, metric, and progress.")]
    public class BeepTaskCard : BeepControl
    {
        private List<string> _avatarImagePaths = new List<string>();
        private List<Image> _avatarImages = new List<Image>();
        private string _titleText = "Coin calc";
        private string _subtitleText = "Cryptocurrency";
        private string _metricText = "110 hours / 45%";
        private float _progressValue = 45f; // Progress percentage (0-100)
        private string _moreIconPath;
        private Image _moreIcon;

        [Category("Appearance")]
        [Description("List of avatar image names or paths displayed at the top-left.")]
        public List<string> AvatarImagePaths
        {
            get => _avatarImagePaths;
            set
            {
                _avatarImagePaths = value;
                // Convert each path to an Image using the helper.
                _avatarImages = _avatarImagePaths.Select(path => ImageListHelper.GetImageFromName(path) as Image).ToList();
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Main title text (e.g., 'Coin calc').")]
        public string TitleText
        {
            get => _titleText;
            set { _titleText = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Subtitle text (e.g., 'Cryptocurrency').")]
        public string SubtitleText
        {
            get => _subtitleText;
            set { _subtitleText = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Bottom metric text (e.g., '110 hours / 45%').")]
        public string MetricText
        {
            get => _metricText;
            set { _metricText = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Progress value in percentage (0-100).")]
        public float ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = Math.Max(0f, Math.Min(100f, value));
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Icon image name or path for the 'more' icon shown at the top-right.")]
        public string MoreIcon
        {
            get => _moreIconPath;
            set
            {
                _moreIconPath = value;
                _moreIcon = ImageListHelper.GetImageFromName(_moreIconPath) as Image;
                Invalidate();
            }
        }

        public BeepTaskCard()
        {
            // Set default size and style
            this.Size = new Size(180, 240);
            this.BorderRadius = 15;
            this.ShowShadow = true;
            this.UseGradientBackground = true;
            this.GradientDirection = LinearGradientMode.Vertical;
            // Example pink gradient; adjust as needed.
            this.GradientStartColor = Color.FromArgb(255, 255, 182, 193);
            this.GradientEndColor = Color.FromArgb(255, 255, 153, 187);
            this.ForeColor = Color.White;
        }
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            
            UpdateDrawingRect();
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var clientRect = this.DrawingRect;

            // 1) Draw overlapping avatars at the top-left.
            int avatarX = 10;
            int avatarY = 10;
            int avatarSize = 32;
            int overlap = 10;
            int maxVisibleAvatars = 3;
            int displayedCount = Math.Min(_avatarImages.Count, maxVisibleAvatars);

            for (int i = 0; i < displayedCount; i++)
            {
                int offsetX = avatarX + i * (avatarSize - overlap);
                var avatarRect = new Rectangle(offsetX, avatarY, avatarSize, avatarSize);
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(avatarRect);
                    g.SetClip(path);
                    if (_avatarImages[i] != null)
                        g.DrawImage(_avatarImages[i], avatarRect);
                    g.ResetClip();
                }
                using (Pen borderPen = new Pen(Color.White, 2f))
                {
                    g.DrawEllipse(borderPen, avatarRect);
                }
            }

            // If there are more avatars than visible, draw a +X circle.
            if (_avatarImages.Count > maxVisibleAvatars)
            {
                int offsetX = avatarX + displayedCount * (avatarSize - overlap);
                var plusRect = new Rectangle(offsetX, avatarY, avatarSize, avatarSize);
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(plusRect);
                    g.SetClip(path);
                    using (SolidBrush plusBrush = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
                    {
                        g.FillEllipse(plusBrush, plusRect);
                    }
                    g.ResetClip();
                }
                using (Pen borderPen = new Pen(Color.White, 2f))
                {
                    g.DrawEllipse(borderPen, plusRect);
                }
                int extraCount = _avatarImages.Count - maxVisibleAvatars;
                string plusText = $"+{extraCount}";
                using (Font plusFont = new Font(Font.FontFamily, 10, FontStyle.Bold))
                {
                    Size plusTextSize = TextRenderer.MeasureText(plusText, plusFont);
                    int plusTextX = plusRect.X + (plusRect.Width - plusTextSize.Width) / 2;
                    int plusTextY = plusRect.Y + (plusRect.Height - plusTextSize.Height) / 2;
                    TextRenderer.DrawText(g, plusText, plusFont, new Point(plusTextX, plusTextY), Color.White);
                }
            }

            // 2) Draw the 'more' icon at the top-right.
            if (_moreIcon != null)
            {
                int iconSize = 24;
                int iconX = clientRect.Width - iconSize - 10;
                int iconY = 10;
                g.DrawImage(_moreIcon, new Rectangle(iconX, iconY, iconSize, iconSize));
            }
            else
            {
                // Fallback: draw a simple vertical ellipsis.
                int dotsX = clientRect.Width - 20;
                int dotsY = 15;
                using (SolidBrush dotBrush = new SolidBrush(this.ForeColor))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        g.FillEllipse(dotBrush, dotsX, dotsY + i * 8, 4, 4);
                    }
                }
            }

            // 3) Draw the title and subtitle.
            using (Font titleFont = new Font(Font.FontFamily, 14, FontStyle.Bold))
            {
                int titleX = 10;
                int titleY = avatarY + avatarSize + 10;
                TextRenderer.DrawText(g, _titleText, titleFont, new Point(titleX, titleY), _currentTheme.CardTextForeColor);
            }
            using (Font subtitleFont = new Font(Font.FontFamily, 10, FontStyle.Regular))
            {
                int subX = 10;
                int subY = avatarY + avatarSize + 30;
                TextRenderer.DrawText(g, _subtitleText, subtitleFont, new Point(subX, subY), _currentTheme.CardTextForeColor);
            }

            // 4) Draw the metric text near the bottom and the progress bar.
            using (Font metricFont = new Font(Font.FontFamily, 10, FontStyle.Regular))
            {
                Size metricSize = TextRenderer.MeasureText(_metricText, metricFont);
                int metricX = 10;
                int metricY = clientRect.Height - 40 - metricSize.Height;
                TextRenderer.DrawText(g, _metricText, metricFont, new Point(metricX, metricY), _currentTheme.CardTitleForeColor);

                // 5) Draw the progress bar below the metric text.
                int barHeight = 6;
                int barX = metricX;
                int barY = clientRect.Height - 20;
                int barWidth = clientRect.Width - barX - 10;
                using (SolidBrush backBrush = new SolidBrush(Color.FromArgb(80, _currentTheme.CardTitleForeColor)))
                {
                    g.FillRectangle(backBrush, barX, barY, barWidth, barHeight);
                }
                float progressWidth = (ProgressValue / 100f) * barWidth;
                using (SolidBrush progressBrush = new SolidBrush(_currentTheme.CardTitleForeColor))
                {
                    g.FillRectangle(progressBrush, barX, barY, (int)progressWidth, barHeight);
                }
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            // Let the base handle the gradient, borders, and shadow.
            base.OnPaint(e);
       
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            // Apply theme colors to the control.
            this.BackColor = _currentTheme.DashboardBackColor;
            this.GradientStartColor = _currentTheme.GradientStartColor;
            this.GradientEndColor = _currentTheme.GradientEndColor;
            this.BorderColor = _currentTheme.TaskCardBackColor;
            this.ForeColor = _currentTheme.CardTextForeColor;
        }
    }
}

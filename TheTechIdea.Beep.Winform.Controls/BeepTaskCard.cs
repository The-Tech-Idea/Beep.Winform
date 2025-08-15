using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Desktop.Common.Util;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Task Card")]
    [Description("A task or project card displaying avatars, title, subtitle, metric, and progress.")]
    public class BeepTaskCard : BeepControl
    {// Controls Used for Drawing and theming
        private BeepButton button;
        private BeepLabel label;
        private BeepImage image;
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

            // 1) Draw overlapping avatars at the top-left using BeepImage
            int avatarX = clientRect.Left + 10;
            int avatarY = clientRect.Top + 10;
            int avatarSize = 32;
            int overlap = 10;
            int maxVisibleAvatars = 3;
            int displayedCount = Math.Min(_avatarImages.Count, maxVisibleAvatars);

            for (int i = 0; i < displayedCount; i++)
            {
                int offsetX = avatarX + i * (avatarSize - overlap);
                var avatarRect = new Rectangle(offsetX, avatarY, avatarSize, avatarSize);

                if (image == null) image = new BeepImage();
                image.Image = _avatarImages[i];
                image.IsChild = true;
                image.BorderColor = Color.White;
                image.BorderThickness = 2;
                image.IsRounded = true;
                image.Draw(g, avatarRect);
            }

            // If there are more avatars than visible, draw a +X circle using BeepLabel
            if (_avatarImages.Count > maxVisibleAvatars)
            {
                int offsetX = avatarX + displayedCount * (avatarSize - overlap);
                var plusRect = new Rectangle(offsetX, avatarY, avatarSize, avatarSize);

                var plusLabel = new BeepLabel
                {
                    Text = $"+{_avatarImages.Count - maxVisibleAvatars}",
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(150, 0, 0, 0),
                    Font = FontListHelper.CreateFontFromTypography(_currentTheme.TaskCardSubStyleStyle),
                    IsChild = true,
                    IsRounded = true,
                    BorderColor = Color.White,
                    BorderThickness = 2
                };
                plusLabel.Draw(g, plusRect);
            }

            // 2) Draw the 'more' icon at the top-right using BeepImage
            int iconSize = 24;
            int iconX = clientRect.Right - iconSize - 10;
            int iconY = clientRect.Top + 10;
            if (image == null) image = new BeepImage();
            if (_moreIcon != null)
                image.Image = _moreIcon;
            else if (!string.IsNullOrEmpty(_moreIconPath))
                image.ImagePath = _moreIconPath;
            image.IsChild = true;
            image.Draw(g, new Rectangle(iconX, iconY, iconSize, iconSize));

            // 3) Draw the title and subtitle using BeepLabel
            if (label == null) label = new BeepLabel();
            // Title
            label.Text = _titleText;
            label.ForeColor = _currentTheme.TaskCardTitleForeColor;
            label.Font = FontListHelper.CreateFontFromTypography(_currentTheme.TaskCardTitleFont);
            label.BackColor = Color.Transparent;
            label.IsChild = true;
            var titleRect = new Rectangle(clientRect.Left + 10, avatarY + avatarSize + 10, clientRect.Width - 20, 24);
            label.Draw(g, titleRect);

            // Subtitle
            var subtitleLabel = new BeepLabel
            {
                Text = _subtitleText,
                ForeColor = _currentTheme.TaskCardSubTitleForeColor,
                Font = FontListHelper.CreateFontFromTypography(_currentTheme.TaskCardSubStyleStyle),
                BackColor = Color.Transparent,
                IsChild = true
            };
            var subRect = new Rectangle(clientRect.Left + 10, avatarY + avatarSize + 34, clientRect.Width - 20, 20);
            subtitleLabel.Draw(g, subRect);

            // 4) Draw the metric text near the bottom using BeepLabel
            var metricLabel = new BeepLabel
            {
                Text = _metricText,
                ForeColor = _currentTheme.TaskCardMetricTextForeColor,
                Font = FontListHelper.CreateFontFromTypography(_currentTheme.TaskCardMetricTextStyle),
                BackColor = Color.Transparent,
                IsChild = true
            };
            Size metricSize = TextRenderer.MeasureText(_metricText, metricLabel.Font);
            int metricX = clientRect.Left + 10;
            int metricY = clientRect.Bottom - 40 - metricSize.Height;
            var metricRect = new Rectangle(metricX, metricY, clientRect.Width - 20, metricSize.Height);
            metricLabel.Draw(g, metricRect);

            // 5) Draw the progress bar below the metric text
            int barHeight = 6;
            int barX = metricX;
            int barY = clientRect.Bottom - 20;
            int barWidth = clientRect.Width - barX - 10;
            using (SolidBrush backBrush = new SolidBrush(Color.FromArgb(80, _currentTheme.TaskCardMetricTextForeColor)))
            {
                g.FillRectangle(backBrush, barX, barY, barWidth, barHeight);
            }
            float progressWidth = (ProgressValue / 100f) * barWidth;
            using (SolidBrush progressBrush = new SolidBrush(_currentTheme.TaskCardMetricTextForeColor))
            {
                g.FillRectangle(progressBrush, barX, barY, (int)progressWidth, barHeight);
            }
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Tasks.Helpers;
using TheTechIdea.Beep.Winform.Controls.ToolTips;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Task Card")]
    [Description("A task or project card displaying avatars, title, subtitle, metric, and progress.")]
    public class BeepTaskCard : BaseControl
    {
        // Layout rectangles for hit testing
        private Rectangle[] avatarRects;
        private Rectangle moreIconRect;
        private Rectangle titleRect;
        private Rectangle subtitleRect;
        private Rectangle metricRect;
        private Rectangle progressBarRect;
        private Rectangle plusLabelRect;
        
        // Hover states
        private string hoveredArea = null;
        private int hoveredAvatarIndex = -1;
        
        private List<string> _avatarImagePaths = new List<string>();
        private List<Image> _avatarImages = new List<Image>();
        private string _titleText = "Coin calc";
        private string _subtitleText = "Cryptocurrency";
        private string _metricText = "110 hours / 45%";
        private float _progressValue = 45f;
        private string _moreIconPath = Svgs.Cat;
        private Image _moreIcon = null;
        private bool _isApplyingTheme = false;
        private bool _autoGenerateTooltip = true;

        [Category("Appearance")]
        [Description("List of avatar image names or paths displayed at the top-left.")]
        public List<string> AvatarImagePaths
        {
            get => _avatarImagePaths;
            set
            {
                _avatarImagePaths = value;
                _avatarImages = _avatarImagePaths.Select(path => ImageListHelper.GetImageFromName(path) as Image).ToList();
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Main title text (e.g., 'Coin calc').")]
        public string TitleText
        {
            get => _titleText;
            set 
            { 
                _titleText = value;
                TaskCardAccessibilityHelpers.ApplyAccessibilitySettings(this);
                if (_autoGenerateTooltip)
                    UpdateTaskCardTooltip();
                Invalidate(); 
            }
        }

        [Category("Appearance")]
        [Description("Subtitle text (e.g., 'Cryptocurrency').")]
        public string SubtitleText
        {
            get => _subtitleText;
            set 
            { 
                _subtitleText = value;
                TaskCardAccessibilityHelpers.ApplyAccessibilitySettings(this);
                if (_autoGenerateTooltip)
                    UpdateTaskCardTooltip();
                Invalidate(); 
            }
        }

        [Category("Appearance")]
        [Description("Bottom metric text (e.g., '110 hours / 45%').")]
        public string MetricText
        {
            get => _metricText;
            set 
            { 
                _metricText = value;
                TaskCardAccessibilityHelpers.ApplyAccessibilitySettings(this);
                if (_autoGenerateTooltip)
                    UpdateTaskCardTooltip();
                Invalidate(); 
            }
        }

        [Category("Appearance")]
        [Description("Progress value in percentage (0-100).")]
        public float ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = Math.Max(0f, Math.Min(100f, value));
                TaskCardAccessibilityHelpers.ApplyAccessibilitySettings(this);
                if (_autoGenerateTooltip)
                    UpdateTaskCardTooltip();
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
                _moreIconPath = TaskCardIconHelpers.ResolveIconPath(value, TaskCardIconHelpers.GetRecommendedMoreIcon());
                _moreIcon = ImageListHelper.GetImageFromName(_moreIconPath) as Image;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Automatically generate tooltip text based on current card state.")]
        [DefaultValue(true)]
        public bool AutoGenerateTooltip
        {
            get => _autoGenerateTooltip;
            set
            {
                if (_autoGenerateTooltip != value)
                {
                    _autoGenerateTooltip = value;
                    if (_autoGenerateTooltip)
                    {
                        UpdateTaskCardTooltip();
                    }
                }
            }
        }

        public BeepTaskCard()
        {
            Size = new Size(180, 240);
            BorderRadius = 15;
            ShowShadow = true;
            UseGradientBackground = true;
            GradientDirection = LinearGradientMode.Vertical;
            GradientStartColor = Color.FromArgb(255, 255, 182, 193);
            GradientEndColor = Color.FromArgb(255, 255, 153, 187);
            ForeColor = Color.White;

            TaskCardAccessibilityHelpers.ApplyAccessibilitySettings(this);
            ApplyTheme();

            if (_autoGenerateTooltip)
            {
                UpdateTaskCardTooltip();
            }
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            UpdateDrawingRect();
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var clientRect = DrawingRect;

            ClearHitList();

            // 1) Draw overlapping avatars at the top-left
            int avatarX = clientRect.Left + Padding.Left;
            int avatarY = clientRect.Top + Padding.Top;
            int avatarSize = 32;
            int overlap = 10;
            int maxVisibleAvatars = 3;
            int displayedCount = Math.Min(_avatarImages.Count, maxVisibleAvatars);
            avatarRects = new Rectangle[displayedCount];

            for (int i = 0; i < displayedCount; i++)
            {
                int offsetX = avatarX + i * (avatarSize - overlap);
                avatarRects[i] = new Rectangle(offsetX, avatarY, avatarSize, avatarSize);
                
                if (i < _avatarImages.Count && _avatarImages[i] != null)
                {
                    DrawCircularAvatar(g, avatarRects[i], _avatarImages[i], Color.White, 2);
                    AddHitArea($"Avatar_{i}", avatarRects[i], null, () => OnAvatarClick(i));
                }
            }

            // If there are more avatars than visible, draw a +X circle
            if (_avatarImages.Count > maxVisibleAvatars)
            {
                int offsetX = avatarX + displayedCount * (avatarSize - overlap);
                plusLabelRect = new Rectangle(offsetX, avatarY, avatarSize, avatarSize);
                
                using (var path = new GraphicsPath())
                {
                    path.AddEllipse(plusLabelRect);
                    using (var brush = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
                    {
                        g.FillPath(brush, path);
                    }
                    using (var pen = new Pen(Color.White, 2))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                
                using (var font = TaskCardFontHelpers.GetAvatarLabelFont(this, ControlStyle))
                using (var brush = new SolidBrush(Color.White))
                {
                    TextRenderer.DrawText(g, $"+{_avatarImages.Count - maxVisibleAvatars}", font, plusLabelRect, brush.Color, 
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
                }
            }

            // 2) Draw the 'more' icon at the top-right
            int iconSize = 24;
            moreIconRect = TaskCardLayoutHelpers.CalculateMoreIconBounds(clientRect, new Size(iconSize, iconSize), Padding);
            
            if (!string.IsNullOrEmpty(_moreIconPath))
            {
                var iconColor = TaskCardThemeHelpers.GetIconColor(_currentTheme, UseThemeColors, null);
                if (hoveredArea == "MoreIcon")
                {
                    iconColor = Color.FromArgb(200, iconColor);
                }
                TaskCardIconHelpers.PaintIcon(g, moreIconRect, _moreIconPath, _currentTheme, UseThemeColors, iconColor);
            }
            else if (_moreIcon != null)
            {
                g.DrawImage(_moreIcon, moreIconRect);
            }
            
            AddHitArea("MoreIcon", moreIconRect, null, () => OnMoreIconClick());

            // 3) Draw the title and subtitle - measure text to prevent clipping
            Size titleSize;
            using (var titleFont = TaskCardFontHelpers.GetTitleFont(this, ControlStyle))
            {
                SizeF titleSizeF = TextUtils.MeasureText(_titleText ?? "", titleFont, int.MaxValue);
                titleSize = new Size((int)titleSizeF.Width, (int)titleSizeF.Height);
            }
            titleRect = TaskCardLayoutHelpers.CalculateTitleBounds(clientRect, new Size(avatarSize * 3, avatarSize), Padding);
            titleRect.Width = Math.Min(titleSize.Width, clientRect.Width - titleRect.Left - Padding.Right - iconSize - 10);
            
            if (!string.IsNullOrEmpty(_titleText))
            {
                using (var titleFont = TaskCardFontHelpers.GetTitleFont(this, ControlStyle))
                using (var titleBrush = new SolidBrush(TaskCardThemeHelpers.GetTitleColor(_currentTheme, UseThemeColors, null)))
                {
                    TextRenderer.DrawText(g, _titleText, titleFont, titleRect, titleBrush.Color, 
                        TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                }
            }

            Size subtitleSize;
            using (var subtitleFont = TaskCardFontHelpers.GetSubtitleFont(this, ControlStyle))
            {
                SizeF subtitleSizeF = TextUtils.MeasureText(_subtitleText ?? "", subtitleFont, int.MaxValue);
                subtitleSize = new Size((int)subtitleSizeF.Width, (int)subtitleSizeF.Height);
            }
            subtitleRect = TaskCardLayoutHelpers.CalculateSubtitleBounds(clientRect, titleRect.Size, Padding);
            subtitleRect.Width = Math.Min(subtitleSize.Width, clientRect.Width - subtitleRect.Left - Padding.Right);
            
            if (!string.IsNullOrEmpty(_subtitleText))
            {
                using (var subtitleFont = TaskCardFontHelpers.GetSubtitleFont(this, ControlStyle))
                using (var subtitleBrush = new SolidBrush(TaskCardThemeHelpers.GetSubtitleColor(_currentTheme, UseThemeColors, null)))
                {
                    TextRenderer.DrawText(g, _subtitleText, subtitleFont, subtitleRect, subtitleBrush.Color, 
                        TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                }
            }

            // 4) Draw the metric text near the bottom - measure to prevent clipping (using cached TextUtils)
            Size metricSize;
            using (var metricFont = TaskCardFontHelpers.GetMetricFont(this, ControlStyle))
            {
                SizeF metricSizeF = TextUtils.MeasureText(_metricText ?? "", metricFont, int.MaxValue);
                metricSize = new Size((int)metricSizeF.Width, (int)metricSizeF.Height);
            }
            metricRect = TaskCardLayoutHelpers.CalculateMetricBounds(clientRect, Padding);
            metricRect.Width = Math.Min(metricSize.Width, clientRect.Width - metricRect.Left - Padding.Right);
            
            if (!string.IsNullOrEmpty(_metricText))
            {
                using (var metricFont = TaskCardFontHelpers.GetMetricFont(this, ControlStyle))
                using (var metricBrush = new SolidBrush(TaskCardThemeHelpers.GetMetricTextColor(_currentTheme, UseThemeColors, null)))
                {
                    TextRenderer.DrawText(g, _metricText, metricFont, metricRect, metricBrush.Color, 
                        TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                }
            }

            // 5) Draw the progress bar below the metric text
            progressBarRect = TaskCardLayoutHelpers.CalculateProgressBarBounds(clientRect, metricRect.Size, Padding);
            Color progressBackColor = TaskCardThemeHelpers.GetProgressBarBackColor(_currentTheme, UseThemeColors, null);
            Color progressColor = TaskCardThemeHelpers.GetProgressBarColor(_currentTheme, UseThemeColors, null);
            
            using (SolidBrush backBrush = new SolidBrush(progressBackColor))
            {
                g.FillRectangle(backBrush, progressBarRect);
            }
            float progressWidth = (ProgressValue / 100f) * progressBarRect.Width;
            using (SolidBrush progressBrush = new SolidBrush(progressColor))
            {
                g.FillRectangle(progressBrush, progressBarRect.X, progressBarRect.Y, (int)progressWidth, progressBarRect.Height);
            }
        }

        private void DrawCircularAvatar(Graphics g, Rectangle bounds, Image avatar, Color borderColor, int borderThickness)
        {
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(bounds);
                
                // Draw border
                using (var pen = new Pen(borderColor, borderThickness))
                {
                    g.DrawPath(pen, path);
                }
                
                // Draw avatar clipped to circle
                g.SetClip(path);
                g.DrawImage(avatar, bounds);
                g.ResetClip();
            }
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_isApplyingTheme) return;

            _isApplyingTheme = true;
            try
            {
                if (_currentTheme == null) return;

                if (UseThemeColors)
                {
                    TaskCardThemeHelpers.ApplyThemeColors(this, _currentTheme, UseThemeColors);
                }
                else
                {
                    BackColor = TaskCardThemeHelpers.GetTaskCardBackColor(_currentTheme, UseThemeColors, null);
                    var (startColor, endColor) = TaskCardThemeHelpers.GetGradientColors(_currentTheme, UseThemeColors, null, null);
                    GradientStartColor = startColor;
                    GradientEndColor = endColor;
                    BorderColor = TaskCardThemeHelpers.GetTaskCardBackColor(_currentTheme, UseThemeColors, null);
                    ForeColor = TaskCardThemeHelpers.GetMetricTextColor(_currentTheme, UseThemeColors, null);
                }

                TaskCardAccessibilityHelpers.ApplyHighContrastAdjustments(this, _currentTheme, UseThemeColors);
            }
            finally
            {
                _isApplyingTheme = false;
            }

            Invalidate();
        }

        #region Tooltips
        private void UpdateTaskCardTooltip()
        {
            if (!EnableTooltip || !_autoGenerateTooltip) return;
            GenerateTaskCardTooltip();
        }

        private void GenerateTaskCardTooltip()
        {
            if (!EnableTooltip) return;

            string tooltipText = "";
            string tooltipTitle = !string.IsNullOrEmpty(_titleText) ? _titleText : "Task Card";
            ToolTipType tooltipType = ToolTipType.Info;

            if (!string.IsNullOrEmpty(_titleText))
                tooltipText = _titleText;
            if (!string.IsNullOrEmpty(_subtitleText))
                tooltipText += (string.IsNullOrEmpty(tooltipText) ? "" : "\n") + _subtitleText;
            if (!string.IsNullOrEmpty(_metricText))
                tooltipText += (string.IsNullOrEmpty(tooltipText) ? "" : "\n") + _metricText;
            tooltipText += (string.IsNullOrEmpty(tooltipText) ? "" : "\n") + $"Progress: {_progressValue:F0}%";

            if (_progressValue >= 80)
                tooltipType = ToolTipType.Success;
            else if (_progressValue <= 20)
                tooltipType = ToolTipType.Warning;

            if (_avatarImagePaths != null && _avatarImagePaths.Count > 0)
                tooltipText += (string.IsNullOrEmpty(tooltipText) ? "" : "\n") + $"{_avatarImagePaths.Count} team member{(_avatarImagePaths.Count == 1 ? "" : "s")}";

            TooltipText = tooltipText;
            TooltipTitle = tooltipTitle;
            TooltipType = tooltipType;
            UpdateTooltip();
        }

        public void SetTaskCardTooltip(string text, string title = null, ToolTipType type = ToolTipType.Info)
        {
            TooltipText = text;
            if (!string.IsNullOrEmpty(title))
                TooltipTitle = title;
            TooltipType = type;
            UpdateTooltip();
        }

        public void ShowTaskCardNotification(string message, ToolTipType type = ToolTipType.Info)
        {
            ShowInfo(message, 2000);
        }
        #endregion

        #region Events
        public event EventHandler CardClick;
        public event EventHandler<AvatarClickEventArgs> AvatarClick;
        public event EventHandler MoreIconClick;

        protected virtual void OnCardClick()
        {
            CardClick?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnAvatarClick(int avatarIndex)
        {
            AvatarClick?.Invoke(this, new AvatarClickEventArgs(avatarIndex));
        }

        protected virtual void OnMoreIconClick()
        {
            MoreIconClick?.Invoke(this, EventArgs.Empty);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (HitTest(e.Location, out var hitTest))
            {
                if (hitTest.Name.StartsWith("Avatar_"))
                {
                    if (int.TryParse(hitTest.Name.Substring(7), out int index))
                    {
                        OnAvatarClick(index);
                        return;
                    }
                }
                else if (hitTest.Name == "MoreIcon")
                {
                    OnMoreIconClick();
                    return;
                }
            }

            OnCardClick();
        }
      

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            string newHoveredArea = null;
            int newHoveredAvatarIndex = -1;
            
            if (HitTest(e.Location, out var hitTest))
            {
                newHoveredArea = hitTest.Name;
                if (hitTest.Name.StartsWith("Avatar_"))
                {
                    if (int.TryParse(hitTest.Name.Substring(7), out int index))
                    {
                        newHoveredAvatarIndex = index;
                    }
                }
            }
            
            if (newHoveredArea != hoveredArea || newHoveredAvatarIndex != hoveredAvatarIndex)
            {
                hoveredArea = newHoveredArea;
                hoveredAvatarIndex = newHoveredAvatarIndex;
                Cursor = (hoveredArea == "MoreIcon" || hoveredArea?.StartsWith("Avatar_") == true) 
                    ? Cursors.Hand 
                    : Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (hoveredArea != null)
            {
                hoveredArea = null;
                hoveredAvatarIndex = -1;
                Cursor = Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.Enter:
                case Keys.Space:
                    OnCardClick();
                    e.Handled = true;
                    break;
            }
            if (e.Handled)
            {
                Invalidate();
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }
        #endregion
    }

    public class AvatarClickEventArgs : EventArgs
    {
        public int AvatarIndex { get; }

        public AvatarClickEventArgs(int avatarIndex)
        {
            AvatarIndex = avatarIndex;
        }
    }
}

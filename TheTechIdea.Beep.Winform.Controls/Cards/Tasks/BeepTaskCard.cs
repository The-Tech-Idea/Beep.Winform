using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;
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
    public partial class BeepTaskCard : BaseControl
    {
        protected override Size DefaultSize => BeepLayoutMetrics.CardTask;
        
        
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
                Recompose();
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
                Recompose(); 
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
                Recompose(); 
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
                Recompose(); 
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
                Recompose();
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
                Recompose();
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

            // The literal pink gradient and white ForeColor that were set here are gone. They are
            // style rather than meaning, and a card that hard-codes its own surface cannot follow a
            // theme - which is the category of defect this refactor removes by not writing the code.

            TaskCardAccessibilityHelpers.ApplyAccessibilitySettings(this);
            Compose();

            if (_autoGenerateTooltip)
            {
                UpdateTaskCardTooltip();
            }
        }

        // DrawContent removed: the card is composed from controls in BeepTaskCard.Composition.cs.
        // The progress bar it drew by hand is a BeepProgressBar, and the avatars and overflow icon are
        // controls that can be clicked, focused and reached from the keyboard - which painted ones
        // could not be, whether or not they rendered.

        // DrawCircularAvatar removed with the paint pass that called it. The avatars are BeepImages.

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
            OnCardClick();
        }

        // The hover tracking that lived here compared the mouse against painted rectangles to decide
        // which avatar - or the overflow icon - was under it. Both are controls now, and each tracks
        // its own hover, cursor and focus and raises its own Click.

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

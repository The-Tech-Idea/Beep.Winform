using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Extension methods to easily add modern tooltips to any WinForms control
    /// </summary>
    public static class ToolTipExtensions
    {
        /// <summary>
        /// Add a simple text tooltip to any control
        /// </summary>
        public static void SetTooltip(this Control control, string text)
        {
            ToolTipManager.SetTooltip(control, text);
        }

        /// <summary>
        /// Add a rich tooltip with title and custom styling
        /// </summary>
        public static void SetRichTooltip(this Control control, string text, string title = null, 
            ToolTipTheme theme = ToolTipTheme.Auto)
        {
            var config = new ToolTipConfig
            {
                Text = text,
                Title = title,
                Theme = theme,
                ShowArrow = true,
                EnableShadow = true
            };
            ToolTipManager.SetTooltip(control, text, config);
        }

        /// <summary>
        /// Add a tooltip with custom configuration
        /// </summary>
        public static void SetTooltip(this Control control, ToolTipConfig config)
        {
            ToolTipManager.SetTooltip(control, config.Text, config);
        }

        /// <summary>
        /// Remove tooltip from control
        /// </summary>
        public static void RemoveTooltip(this Control control)
        {
            ToolTipManager.RemoveTooltip(control);
        }

        /// <summary>
        /// Show a temporary notification-style tooltip
        /// </summary>
        public static async Task<string> ShowNotificationAsync(this Control control, string text, 
            ToolTipTheme theme = ToolTipTheme.Info, int duration = 3000)
        {
            var location = control.PointToScreen(new Point(control.Width / 2, 0));
            var config = new ToolTipConfig
            {
                Text = text,
                Theme = theme,
                Duration = duration,
                Position = location,
                Placement = ToolTipPlacement.Top,
                Animation = ToolTipAnimation.Slide,
                ShowArrow = true,
                EnableShadow = true
            };
            return await ToolTipManager.ShowTooltipAsync(config);
        }

        /// <summary>
        /// Show a success message tooltip
        /// </summary>
        public static async Task<string> ShowSuccessAsync(this Control control, string text, int duration = 2000)
        {
            return await control.ShowNotificationAsync(text, ToolTipTheme.Success, duration);
        }

        /// <summary>
        /// Show an error message tooltip
        /// </summary>
        public static async Task<string> ShowErrorAsync(this Control control, string text, int duration = 4000)
        {
            return await control.ShowNotificationAsync(text, ToolTipTheme.Error, duration);
        }

        /// <summary>
        /// Show a warning message tooltip
        /// </summary>
        public static async Task<string> ShowWarningAsync(this Control control, string text, int duration = 3000)
        {
            return await control.ShowNotificationAsync(text, ToolTipTheme.Warning, duration);
        }

        /// <summary>
        /// Show an info message tooltip
        /// </summary>
        public static async Task<string> ShowInfoAsync(this Control control, string text, int duration = 3000)
        {
            return await control.ShowNotificationAsync(text, ToolTipTheme.Info, duration);
        }
    }

    /// <summary>
    /// Fluent builder for creating tooltip configurations
    /// </summary>
    public class ToolTipBuilder
    {
        private readonly ToolTipConfig _config = new();

        public static ToolTipBuilder Create() => new();

        public ToolTipBuilder WithText(string text)
        {
            _config.Text = text;
            return this;
        }

        public ToolTipBuilder WithTitle(string title)
        {
            _config.Title = title;
            return this;
        }

        public ToolTipBuilder WithTheme(ToolTipTheme theme)
        {
            _config.Theme = theme;
            return this;
        }

        public ToolTipBuilder WithCustomColors(Color backColor, Color foreColor, Color? borderColor = null)
        {
            _config.Theme = ToolTipTheme.Custom;
            _config.BackColor = backColor;
            _config.ForeColor = foreColor;
            _config.BorderColor = borderColor;
            return this;
        }

        public ToolTipBuilder WithPlacement(ToolTipPlacement placement)
        {
            _config.Placement = placement;
            return this;
        }

        public ToolTipBuilder WithAnimation(ToolTipAnimation animation)
        {
            _config.Animation = animation;
            return this;
        }

        public ToolTipBuilder WithDuration(int durationMs)
        {
            _config.Duration = durationMs;
            return this;
        }

        public ToolTipBuilder WithDelay(int showDelayMs, int? hideDelayMs = null)
        {
            _config.ShowDelay = showDelayMs;
            _config.HideDelay = hideDelayMs;
            return this;
        }

        public ToolTipBuilder WithMaxSize(Size maxSize)
        {
            _config.MaxSize = maxSize;
            return this;
        }

        public ToolTipBuilder WithFont(Font font)
        {
            _config.Font = font;
            return this;
        }

        public ToolTipBuilder WithIcon(Image icon)
        {
            _config.Icon = icon;
            return this;
        }

        public ToolTipBuilder WithIconPath(string iconPath)
        {
            _config.IconPath = iconPath;
            return this;
        }

        public ToolTipBuilder FollowCursor(bool follow = true)
        {
            _config.FollowCursor = follow;
            return this;
        }

        public ToolTipBuilder WithArrow(bool showArrow = true)
        {
            _config.ShowArrow = showArrow;
            return this;
        }

        public ToolTipBuilder WithShadow(bool enableShadow = true)
        {
            _config.EnableShadow = enableShadow;
            return this;
        }

        public ToolTipBuilder Closable(bool closable = true)
        {
            _config.Closable = closable;
            return this;
        }

        public ToolTipBuilder OnShow(Action<string> onShow)
        {
            _config.OnShow = onShow;
            return this;
        }

        public ToolTipBuilder OnClose(Action<string> onClose)
        {
            _config.OnClose = onClose;
            return this;
        }

        public ToolTipConfig Build() => _config;

        public async Task<string> ShowAsync(Point position)
        {
            _config.Position = position;
            return await ToolTipManager.ShowTooltipAsync(_config);
        }

        public void AttachTo(Control control)
        {
            ToolTipManager.SetTooltip(control, _config.Text, _config);
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Modern ToolTip Manager inspired by DevExpress, Material-UI, and Ant Design
    /// Features: Rich content, animations, positioning, theming, accessibility
    /// </summary>
    public static class ToolTipManager
    {
        private static readonly ConcurrentDictionary<string, ToolTipInstance> _activeTooltips = new();
        private static readonly ConcurrentDictionary<Control, string> _controlTooltips = new();
        private static readonly System.Threading.Timer _cleanupTimer;
        
        static ToolTipManager()
        {
            // Initialize cleanup timer
            _cleanupTimer = new System.Threading.Timer(OnCleanupTimer, null, 5000, 5000);
        }
        
        // Global settings
        public static ToolTipTheme DefaultTheme { get; set; } = ToolTipTheme.Auto;
        public static int DefaultShowDelay { get; set; } = 500;
        public static int DefaultHideDelay { get; set; } = 3000;
        public static int DefaultFadeInDuration { get; set; } = 150;
        public static int DefaultFadeOutDuration { get; set; } = 100;
        public static ToolTipPlacement DefaultPlacement { get; set; } = ToolTipPlacement.Auto;
        public static bool EnableAnimations { get; set; } = true;
        public static bool EnableAccessibility { get; set; } = true;

        /// <summary>
        /// Show a rich tooltip with modern styling and animations
        /// </summary>
        public static async Task<string> ShowTooltipAsync(ToolTipConfig config)
        {
            if (string.IsNullOrEmpty(config.Key))
                config.Key = Guid.NewGuid().ToString();

            // Cancel existing tooltip with same key
            if (_activeTooltips.TryGetValue(config.Key, out var existing))
            {
                await existing.HideAsync();
            }

            var instance = new ToolTipInstance(config);
            _activeTooltips[config.Key] = instance;

            await instance.ShowAsync();
            return config.Key;
        }

        /// <summary>
        /// Quick tooltip with just text and position
        /// </summary>
        public static Task<string> ShowTooltipAsync(string text, Point location, int duration = 0)
        {
            var config = new ToolTipConfig
            {
                Text = text,
                Position = location,
                Duration = duration > 0 ? duration : DefaultHideDelay,
                Theme = DefaultTheme,
                Placement = DefaultPlacement
            };
            return ShowTooltipAsync(config);
        }

        /// <summary>
        /// Attach tooltip to a control with hover behavior
        /// </summary>
        public static void SetTooltip(Control control, string text, ToolTipConfig config = null)
        {
            if (control == null) return;

            // Remove existing tooltip
            RemoveTooltip(control);

            config ??= new ToolTipConfig();
            config.Text = text;
            config.Key = $"control_{control.GetHashCode()}_{DateTime.Now.Ticks}";

            _controlTooltips[control] = config.Key;

            // Attach event handlers
            control.MouseEnter += async (s, e) => await OnControlMouseEnter(control, config);
            control.MouseLeave += async (s, e) => await OnControlMouseLeave(control, config);
            control.MouseMove += (s, e) => OnControlMouseMove(control, config, e);
            
            // Accessibility
            if (EnableAccessibility)
            {
                control.AccessibleDescription = text;
            }
        }

        /// <summary>
        /// Remove tooltip from control
        /// </summary>
        public static void RemoveTooltip(Control control)
        {
            if (control == null) return;

            if (_controlTooltips.TryRemove(control, out var key))
            {
                _ = HideTooltipAsync(key);
            }
        }

        /// <summary>
        /// Hide specific tooltip by key
        /// </summary>
        public static async Task HideTooltipAsync(string key)
        {
            if (string.IsNullOrEmpty(key)) return;

            if (_activeTooltips.TryRemove(key, out var instance))
            {
                await instance.HideAsync();
            }
        }

        /// <summary>
        /// Hide all active tooltips
        /// </summary>
        public static async Task HideAllTooltipsAsync()
        {
            var tasks = new List<Task>();
            foreach (var kvp in _activeTooltips)
            {
                tasks.Add(kvp.Value.HideAsync());
            }
            _activeTooltips.Clear();
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Update tooltip content dynamically
        /// </summary>
        public static void UpdateTooltip(string key, string newText, string newTitle = null)
        {
            if (_activeTooltips.TryGetValue(key, out var instance))
            {
                instance.UpdateContent(newText, newTitle);
            }
        }

        // Event handlers for control tooltips
        private static async Task OnControlMouseEnter(Control control, ToolTipConfig config)
        {
            await Task.Delay(config.ShowDelay ?? DefaultShowDelay);
            
            if (control.ClientRectangle.Contains(control.PointToClient(Cursor.Position)))
            {
                var location = control.PointToScreen(new Point(control.Width / 2, control.Height));
                config.Position = location;
                await ShowTooltipAsync(config);
            }
        }

        private static async Task OnControlMouseLeave(Control control, ToolTipConfig config)
        {
            if (_controlTooltips.TryGetValue(control, out var key))
            {
                await Task.Delay(200); // Small delay to prevent flicker
                await HideTooltipAsync(key);
            }
        }

        private static void OnControlMouseMove(Control control, ToolTipConfig config, MouseEventArgs e)
        {
            if (_controlTooltips.TryGetValue(control, out var key) && 
                _activeTooltips.TryGetValue(key, out var instance))
            {
                if (config.FollowCursor)
                {
                    var newPos = control.PointToScreen(e.Location);
                    instance.UpdatePosition(newPos);
                }
            }
        }

        private static void OnCleanupTimer(object state)
        {
            var expiredKeys = new List<string>();
            var now = DateTime.UtcNow;

            foreach (var kvp in _activeTooltips)
            {
                if (kvp.Value.IsExpired(now))
                {
                    expiredKeys.Add(kvp.Key);
                }
            }

            foreach (var key in expiredKeys)
            {
                _ = Task.Run(() => HideTooltipAsync(key));
            }
        }
    }

    /// <summary>
    /// Configuration for tooltip appearance and behavior
    /// </summary>
    public class ToolTipConfig
    {
        public string Key { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
        public string Html { get; set; } // Rich HTML content
        public Point Position { get; set; }
        public int Duration { get; set; } = 3000;
        public int? ShowDelay { get; set; }
        public int? HideDelay { get; set; }
        public ToolTipTheme Theme { get; set; } = ToolTipTheme.Auto;
        public ToolTipStyle Style { get; set; } = ToolTipStyle.Standard;
        public ToolTipPlacement Placement { get; set; } = ToolTipPlacement.Auto;
        public Size? MaxSize { get; set; }
        public Color? BackColor { get; set; }
        public Color? ForeColor { get; set; }
        public Color? BorderColor { get; set; }
        public Font Font { get; set; }
        public bool FollowCursor { get; set; } = false;
        public bool ShowArrow { get; set; } = true;
        public bool ShowShadow { get; set; } = true;
        public bool Closable { get; set; } = false;
        public Image Icon { get; set; }
        public string IconPath { get; set; }
        public string ImagePath { get; set; } // SVG/Image path like BeepButton
        public Size MaxImageSize { get; set; } = new Size(24, 24); // Max image size
        public bool ApplyThemeOnImage { get; set; } = true; // Apply theme colors to SVG
        public ToolTipAnimation Animation { get; set; } = ToolTipAnimation.Fade;
        public int AnimationDuration { get; set; } = 200;
        public int Offset { get; set; } = 8;
        public object Tag { get; set; }
        public Action<string> OnClose { get; set; }
        public Action<string> OnShow { get; set; }
        
        // Step-specific properties (for ToolTipStyle.Step)
        public int CurrentStep { get; set; } = 1;
        public int TotalSteps { get; set; } = 1;
        public string StepTitle { get; set; }
        public bool ShowNavigationButtons { get; set; } = true;
        public bool EnableShadow { get; internal set; }
    }

    public enum ToolTipTheme
    {
        Auto,
        Light,
        Dark,
        Primary,
        Success,
        Warning,
        Error,
        Info,
        Custom
    }

    public enum ToolTipPlacement
    {
        Auto,
        Top,
        TopStart,
        TopEnd,
        Bottom,
        BottomStart,
        BottomEnd,
        Left,
        LeftStart,
        LeftEnd,
        Right,
        RightStart,
        RightEnd
    }

    public enum ToolTipAnimation
    {
        None,
        Fade,
        Scale,
        Slide,
        Bounce
    }

    public enum ToolTipStyle
    {
        Standard,    // Clean, modern look
        Premium,     // Gradient with badge
        Alert,       // Left accent bar with status icons
        Notification,// Pop-up notification style
        Step         // Multi-step tutorial tooltip
    }

    /// <summary>
    /// Internal tooltip instance management
    /// </summary>
    internal class ToolTipInstance
    {
        private readonly ToolTipConfig _config;
        private CustomToolTip _tooltip;
        private readonly DateTime _createdAt;
        private CancellationTokenSource _cancellationTokenSource;

        public ToolTipInstance(ToolTipConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _createdAt = DateTime.UtcNow;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task ShowAsync()
        {
            try
            {
                _tooltip = new CustomToolTip();
                _tooltip.ApplyConfig(_config);
                
                _config.OnShow?.Invoke(_config.Key);
                
                await _tooltip.ShowAsync(_config.Position, _cancellationTokenSource.Token);

                // Auto-hide after duration
                if (_config.Duration > 0)
                {
                    _ = Task.Delay(_config.Duration, _cancellationTokenSource.Token)
                        .ContinueWith(async t =>
                        {
                            if (!t.IsCanceled)
                                await HideAsync();
                        }, TaskContinuationOptions.OnlyOnRanToCompletion);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancelled
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing tooltip: {ex.Message}");
            }
        }

        public async Task HideAsync()
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                
                if (_tooltip != null)
                {
                    await _tooltip.HideAsync();
                    _tooltip.Dispose();
                    _tooltip = null;
                }

                _config.OnClose?.Invoke(_config.Key);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error hiding tooltip: {ex.Message}");
            }
        }

        public void UpdateContent(string text, string title = null)
        {
            if (_tooltip != null)
            {
                _config.Text = text;
                if (title != null) _config.Title = title;
                _tooltip.ApplyConfig(_config);
            }
        }

        public void UpdatePosition(Point position)
        {
            if (_tooltip != null)
            {
                _config.Position = position;
                _tooltip.UpdatePosition(position);
            }
        }

        public bool IsExpired(DateTime now)
        {
            if (_config.Duration <= 0) return false;
            return (now - _createdAt).TotalMilliseconds > _config.Duration * 2; // Extra buffer
        }
    }
}
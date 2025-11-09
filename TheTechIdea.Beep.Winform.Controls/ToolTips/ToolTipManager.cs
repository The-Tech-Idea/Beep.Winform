using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Singleton tooltip manager following BeepNotificationManager architecture pattern
    /// Consolidated from partial classes into single coherent implementation
    /// Manages tooltip lifecycle, positioning, and automatic cleanup
    /// </summary>
    public sealed class ToolTipManager : IDisposable
    {
        #region Singleton Pattern

        private static readonly Lazy<ToolTipManager> _instance = 
            new Lazy<ToolTipManager>(() => new ToolTipManager());

        /// <summary>
        /// Get the singleton instance of ToolTipManager
        /// </summary>
        public static ToolTipManager Instance => _instance.Value;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private ToolTipManager()
        {
            // Initialize collections
            _activeTooltips = new ConcurrentDictionary<string, ToolTipInstance>();
            _controlTooltips = new ConcurrentDictionary<Control, string>();

            // Start cleanup timer (runs every 5 seconds)
            _cleanupTimer = new System.Threading.Timer(OnCleanupTimer, null, 5000, 5000);
        }

        #endregion

        #region Fields

        private readonly ConcurrentDictionary<string, ToolTipInstance> _activeTooltips;
        private readonly ConcurrentDictionary<Control, string> _controlTooltips;
        private readonly System.Threading.Timer _cleanupTimer;
        private bool _disposed;

        #endregion

        #region Properties - Default Settings

        /// <summary>
        /// Default tooltip type for new tooltips
        /// </summary>
        public ToolTipType DefaultType { get; set; } = ToolTipType.Default;

        /// <summary>
        /// Default control style for tooltips
        /// </summary>
        public BeepControlStyle DefaultStyle { get; set; } = BeepControlStyle.Material3;

        /// <summary>
        /// Default control style for control-attached tooltips
        /// </summary>
        public BeepControlStyle DefaultControlStyle { get; set; } = BeepControlStyle.Material3;

        /// <summary>
        /// Use theme colors by default
        /// </summary>
        public bool DefaultUseThemeColors { get; set; } = true;

        /// <summary>
        /// Default delay before showing tooltip (milliseconds)
        /// </summary>
        public int DefaultShowDelay { get; set; } = 500;

        /// <summary>
        /// Default duration to display tooltip (milliseconds)
        /// </summary>
        public int DefaultHideDelay { get; set; } = 3000;

        /// <summary>
        /// Default fade-in animation duration (milliseconds)
        /// </summary>
        public int DefaultFadeInDuration { get; set; } = 150;

        /// <summary>
        /// Default fade-out animation duration (milliseconds)
        /// </summary>
        public int DefaultFadeOutDuration { get; set; } = 100;

        /// <summary>
        /// Default tooltip placement
        /// </summary>
        public ToolTipPlacement DefaultPlacement { get; set; } = ToolTipPlacement.Auto;

        /// <summary>
        /// Enable animations globally
        /// </summary>
        public bool EnableAnimations { get; set; } = true;

        /// <summary>
        /// Enable accessibility features
        /// </summary>
        public bool EnableAccessibility { get; set; } = true;

        #endregion

        #region Show Methods

        /// <summary>
        /// Show a rich tooltip with full configuration
        /// Returns a unique key that can be used to update or hide the tooltip
        /// </summary>
        /// <param name="config">Complete tooltip configuration</param>
        /// <returns>Unique key for this tooltip instance</returns>
        public async Task<string> ShowTooltipAsync(ToolTipConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            // Generate unique key if not provided
            if (string.IsNullOrEmpty(config.Key))
            {
                config.Key = Guid.NewGuid().ToString();
            }

            // Cancel existing tooltip with same key
            if (_activeTooltips.TryGetValue(config.Key, out var existing))
            {
                await existing.HideAsync();
                _activeTooltips.TryRemove(config.Key, out _);
            }

            // Apply global defaults if not specified
            config.ShowDelay ??= DefaultShowDelay;
            config.Duration = config.Duration > 0 ? config.Duration : DefaultHideDelay;

            if (config.Type == ToolTipType.Default)
            {
                config.Type = DefaultType;
            }
            
            if (config.Placement == ToolTipPlacement.Auto)
            {
                config.Placement = DefaultPlacement;
            }

            if (!EnableAnimations)
            {
                config.Animation = ToolTipAnimation.None;
            }

            // Create and show new instance
            var instance = new ToolTipInstance(config);
            _activeTooltips[config.Key] = instance;

            try
            {
                await instance.ShowAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ToolTipManager] Error showing tooltip: {ex.Message}");
                _activeTooltips.TryRemove(config.Key, out _);
                throw;
            }

            return config.Key;
        }

        /// <summary>
        /// Show a simple text tooltip at a specific location
        /// Convenience method for quick tooltips without full configuration
        /// </summary>
        /// <param name="text">Tooltip text content</param>
        /// <param name="location">Screen location to display tooltip</param>
        /// <param name="duration">Display duration in milliseconds (0 for default)</param>
        /// <returns>Unique key for this tooltip instance</returns>
        public Task<string> ShowTooltipAsync(string text, Point location, int duration = 0)
        {
            var config = new ToolTipConfig
            {
                Text = text,
                Position = location,
                Duration = duration > 0 ? duration : DefaultHideDelay,
                Type = DefaultType,
                Placement = DefaultPlacement,
                Style = DefaultStyle
            };

            return ShowTooltipAsync(config);
        }

        /// <summary>
        /// Show a tooltip with title and text at a specific location
        /// </summary>
        /// <param name="title">Tooltip title/header</param>
        /// <param name="text">Tooltip body text</param>
        /// <param name="location">Screen location to display tooltip</param>
        /// <param name="type">Color theme (optional)</param>
        /// <returns>Unique key for this tooltip instance</returns>
        public Task<string> ShowTooltipAsync(string title, string text, Point location, ToolTipType? type = null)
        {
            var config = new ToolTipConfig
            {
                Title = title,
                Text = text,
                Position = location,
                Duration = DefaultHideDelay,
                Type = type ?? ToolTipType.Default,
                Placement = DefaultPlacement,
                Style = DefaultStyle
            };

            return ShowTooltipAsync(config);
        }

        #endregion

        #region Hide Methods

        /// <summary>
        /// Hide a specific tooltip by its key
        /// </summary>
        /// <param name="key">Tooltip key returned from ShowTooltipAsync</param>
        public async Task HideTooltipAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (_activeTooltips.TryRemove(key, out var instance))
            {
                try
                {
                    await instance.HideAsync();
                    instance.Dispose();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ToolTipManager] Error hiding tooltip: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Hide all currently active tooltips
        /// Useful for cleanup or modal operations
        /// </summary>
        public async Task HideAllTooltipsAsync()
        {
            var tasks = new List<Task>();
            var keys = new List<string>();

            // Collect all instances
            foreach (var kvp in _activeTooltips)
            {
                tasks.Add(kvp.Value.HideAsync());
                keys.Add(kvp.Key);
            }

            // Clear dictionary
            foreach (var key in keys)
            {
                if (_activeTooltips.TryRemove(key, out var instance))
                {
                    instance.Dispose();
                }
            }

            // Wait for all hide operations to complete
            if (tasks.Count > 0)
            {
                try
                {
                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ToolTipManager] Error hiding all tooltips: {ex.Message}");
                }
            }
        }

        #endregion

        #region Update Methods

        /// <summary>
        /// Update the content of an active tooltip
        /// </summary>
        /// <param name="key">Tooltip key</param>
        /// <param name="newText">New text content</param>
        /// <param name="newTitle">New title (optional)</param>
        public void UpdateTooltip(string key, string newText, string newTitle = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (_activeTooltips.TryGetValue(key, out var instance))
            {
                try
                {
                    instance.UpdateContent(newText, newTitle);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ToolTipManager] Error updating tooltip: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Update the position of an active tooltip (for follow-cursor scenarios)
        /// </summary>
        /// <param name="key">Tooltip key</param>
        /// <param name="newPosition">New screen position</param>
        public void UpdateTooltipPosition(string key, Point newPosition)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (_activeTooltips.TryGetValue(key, out var instance))
            {
                try
                {
                    instance.UpdatePosition(newPosition);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ToolTipManager] Error updating tooltip position: {ex.Message}");
                }
            }
        }

        #endregion

        #region Query Methods

        /// <summary>
        /// Check if a tooltip with the specified key is currently active
        /// </summary>
        public bool IsTooltipActive(string key)
        {
            return !string.IsNullOrEmpty(key) && _activeTooltips.ContainsKey(key);
        }

        /// <summary>
        /// Get all active tooltip keys
        /// </summary>
        public IEnumerable<string> GetActiveTooltipKeys()
        {
            return _activeTooltips.Keys;
        }

        /// <summary>
        /// Get count of active tooltips
        /// </summary>
        public int ActiveTooltipCount => _activeTooltips.Count;

        #endregion

        #region Control Integration

        /// <summary>
        /// Attach a tooltip to a control with hover behavior
        /// Automatically handles mouse enter/leave events
        /// </summary>
        /// <param name="control">Target control</param>
        /// <param name="text">Tooltip text</param>
        /// <param name="config">Optional configuration (uses defaults if null)</param>
        public void SetTooltip(Control control, string text, ToolTipConfig config = null)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            if (string.IsNullOrEmpty(text))
            {
                RemoveTooltip(control);
                return;
            }

            // Remove existing tooltip first
            RemoveTooltip(control);

            // Create or update configuration
            config ??= new ToolTipConfig();
            config.Text = text;
            config.Key = $"control_{control.GetHashCode()}_{DateTime.Now.Ticks}";
            config.Style = DefaultControlStyle;

            // Store control-tooltip mapping
            _controlTooltips[control] = config.Key;

            // Attach event handlers
            control.MouseEnter += async (s, e) => await OnControlMouseEnter(control, config);
            control.MouseLeave += async (s, e) => await OnControlMouseLeave(control, config);
            
            if (config.FollowCursor)
            {
                control.MouseMove += (s, e) => OnControlMouseMove(control, config, e);
            }

            // Set accessibility properties
            if (EnableAccessibility)
            {
                control.AccessibleDescription = text;
                
                if (!string.IsNullOrEmpty(config.Title))
                {
                    control.AccessibleName = config.Title;
                }
            }
        }

        /// <summary>
        /// Attach a tooltip with title to a control
        /// </summary>
        public void SetTooltip(Control control, string title, string text, ToolTipType type = ToolTipType.Default, BeepControlStyle style = BeepControlStyle.Material3)
        {
            var config = new ToolTipConfig
            {
                Title = title,
                Text = text,
                Style = style,
                Type = type
            };

            SetTooltip(control, text, config);
        }

        /// <summary>
        /// Attach a styled tooltip to a control
        /// </summary>
        public void SetTooltip(Control control, string text, BeepControlStyle style, ToolTipType type = ToolTipType.Default)
        {
            var config = new ToolTipConfig
            {
                Text = text,
                Style = style,
                Type = type
            };

            SetTooltip(control, text, config);
        }

        /// <summary>
        /// Remove tooltip from a control and clean up event handlers
        /// </summary>
        /// <param name="control">Control to remove tooltip from</param>
        public void RemoveTooltip(Control control)
        {
            if (control == null)
            {
                return;
            }

            if (_controlTooltips.TryRemove(control, out var key))
            {
                // Hide the tooltip if it's currently showing
                _ = HideTooltipAsync(key);

                // Note: Event handlers will be garbage collected with the control
            }

            // Clear accessibility properties
            if (EnableAccessibility)
            {
                control.AccessibleDescription = string.Empty;
            }
        }

        /// <summary>
        /// Update the text of a tooltip attached to a control
        /// </summary>
        public void UpdateControlTooltip(Control control, string newText)
        {
            if (control == null || string.IsNullOrEmpty(newText))
            {
                return;
            }

            if (_controlTooltips.TryGetValue(control, out var key))
            {
                UpdateTooltip(key, newText);
                
                if (EnableAccessibility)
                {
                    control.AccessibleDescription = newText;
                }
            }
        }

        /// <summary>
        /// Check if a control has a tooltip attached
        /// </summary>
        public bool HasTooltip(Control control)
        {
            return control != null && _controlTooltips.ContainsKey(control);
        }

        /// <summary>
        /// Get the tooltip key for a control (if one is attached)
        /// </summary>
        public string GetControlTooltipKey(Control control)
        {
            if (control == null)
            {
                return null;
            }

            return _controlTooltips.TryGetValue(control, out var key) ? key : null;
        }

        /// <summary>
        /// Remove tooltips from all controls
        /// </summary>
        public void RemoveAllControlTooltips()
        {
            var controls = new List<Control>(_controlTooltips.Keys);
            
            foreach (var control in controls)
            {
                RemoveTooltip(control);
            }
        }

        /// <summary>
        /// Set the same tooltip text on multiple controls
        /// </summary>
        public void SetTooltipForControls(string text, params Control[] controls)
        {
            if (controls == null || controls.Length == 0)
            {
                return;
            }

            foreach (var control in controls)
            {
                if (control != null)
                {
                    SetTooltip(control, text);
                }
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handle mouse enter event for control-attached tooltips
        /// Shows tooltip after configured delay
        /// </summary>
        private async Task OnControlMouseEnter(Control control, ToolTipConfig config)
        {
            if (control == null || config == null)
            {
                return;
            }

            try
            {
                // Wait for show delay
                var delay = config.ShowDelay ?? DefaultShowDelay;
                if (delay > 0)
                {
                    await Task.Delay(delay);
                }

                // Check if cursor is still over the control
                if (!control.IsDisposed && control.ClientRectangle.Contains(control.PointToClient(Cursor.Position)))
                {
                    // Calculate position relative to control
                    var location = CalculateTooltipPosition(control, config.Placement);
                    config.Position = location;

                    // Show tooltip
                    await ShowTooltipAsync(config);
                }
            }
            catch (ObjectDisposedException)
            {
                // Control was disposed during delay - ignore
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ToolTipManager] Error in OnControlMouseEnter: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle mouse leave event for control-attached tooltips
        /// Hides tooltip after small delay to prevent flicker
        /// </summary>
        private async Task OnControlMouseLeave(Control control, ToolTipConfig config)
        {
            if (control == null)
            {
                return;
            }

            try
            {
                if (_controlTooltips.TryGetValue(control, out var key))
                {
                    // Small delay to prevent flicker when moving between controls
                    await Task.Delay(200);

                    // Hide the tooltip
                    await HideTooltipAsync(key);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ToolTipManager] Error in OnControlMouseLeave: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle mouse move event for follow-cursor tooltips
        /// Updates tooltip position to follow the mouse
        /// </summary>
        private void OnControlMouseMove(Control control, ToolTipConfig config, MouseEventArgs e)
        {
            if (control == null || config == null || !config.FollowCursor)
            {
                return;
            }

            try
            {
                if (_controlTooltips.TryGetValue(control, out var key) &&
                    _activeTooltips.TryGetValue(key, out var instance))
                {
                    // Update tooltip position to follow cursor
                    var newPos = control.PointToScreen(e.Location);
                    newPos.Offset(config.Offset, config.Offset);
                    
                    instance.UpdatePosition(newPos);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ToolTipManager] Error in OnControlMouseMove: {ex.Message}");
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Calculate optimal tooltip position relative to control
        /// </summary>
        private Point CalculateTooltipPosition(Control control, ToolTipPlacement placement)
        {
            if (control == null)
            {
                return Cursor.Position;
            }

            try
            {
                // Get control bounds in screen coordinates
                var screenBounds = control.RectangleToScreen(control.ClientRectangle);

                // Calculate position based on placement
                return placement switch
                {
                    ToolTipPlacement.Top => new Point(
                        screenBounds.Left + screenBounds.Width / 2,
                        screenBounds.Top
                    ),
                    ToolTipPlacement.Bottom => new Point(
                        screenBounds.Left + screenBounds.Width / 2,
                        screenBounds.Bottom
                    ),
                    ToolTipPlacement.Left => new Point(
                        screenBounds.Left,
                        screenBounds.Top + screenBounds.Height / 2
                    ),
                    ToolTipPlacement.Right => new Point(
                        screenBounds.Right,
                        screenBounds.Top + screenBounds.Height / 2
                    ),
                    ToolTipPlacement.TopStart => new Point(
                        screenBounds.Left,
                        screenBounds.Top
                    ),
                    ToolTipPlacement.TopEnd => new Point(
                        screenBounds.Right,
                        screenBounds.Top
                    ),
                    ToolTipPlacement.BottomStart => new Point(
                        screenBounds.Left,
                        screenBounds.Bottom
                    ),
                    ToolTipPlacement.BottomEnd => new Point(
                        screenBounds.Right,
                        screenBounds.Bottom
                    ),
                    // Auto or default: show below control center
                    _ => new Point(
                        screenBounds.Left + screenBounds.Width / 2,
                        screenBounds.Bottom
                    )
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ToolTipManager] Error calculating tooltip position: {ex.Message}");
                return Cursor.Position;
            }
        }

        #endregion

        #region Cleanup Timer

        /// <summary>
        /// Periodic cleanup timer callback
        /// Removes expired tooltip instances to prevent memory leaks
        /// </summary>
        private void OnCleanupTimer(object? state)
        {
            try
            {
                var expiredKeys = new List<string>();
                var now = DateTime.UtcNow;

                // Find expired tooltips
                foreach (var kvp in _activeTooltips)
                {
                    if (kvp.Value.IsExpired(now))
                    {
                        expiredKeys.Add(kvp.Key);
                    }
                }

                // Hide and remove expired tooltips
                foreach (var key in expiredKeys)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await HideTooltipAsync(key);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[ToolTipManager] Error during cleanup: {ex.Message}");
                        }
                    });
                }

                // Log cleanup activity if any tooltips were removed
                if (expiredKeys.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[ToolTipManager] Cleaned up {expiredKeys.Count} expired tooltips");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ToolTipManager] Error in cleanup timer: {ex.Message}");
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            // Stop cleanup timer
            _cleanupTimer?.Dispose();

            // Hide all tooltips
            var hideTask = HideAllTooltipsAsync();
            hideTask.Wait(TimeSpan.FromSeconds(2)); // Wait max 2 seconds

            // Clear collections
            _activeTooltips.Clear();
            _controlTooltips.Clear();
        }

        #endregion
    }
}

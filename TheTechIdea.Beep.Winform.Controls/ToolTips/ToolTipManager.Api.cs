using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// ToolTipManager - Core API Methods
    /// Handles showing, hiding, and updating tooltips
    /// </summary>
    public static partial class ToolTipManager
    {
        #region Show Methods

        /// <summary>
        /// Show a rich tooltip with full configuration
        /// Returns a unique key that can be used to update or hide the tooltip
        /// </summary>
        /// <param name="config">Complete tooltip configuration</param>
        /// <returns>Unique key for this tooltip instance</returns>
        public static async Task<string> ShowTooltipAsync(ToolTipConfig config)
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
        public static Task<string> ShowTooltipAsync(string text, Point location, int duration = 0)
        {
            var config = new ToolTipConfig
            {
                Text = text,
                Position = location,
                Duration = duration > 0 ? duration : DefaultHideDelay,
                Type = DefaultType,
                Placement = DefaultPlacement,
                Style = BeepControlStyle.Material3
            };

            return ShowTooltipAsync(config);
        }

        /// <summary>
        /// Show a tooltip with title and text at a specific location
        /// </summary>
        /// <param name="title">Tooltip title/header</param>
        /// <param name="text">Tooltip body text</param>
        /// <param name="location">Screen location to display tooltip</param>
        /// <param name="theme">Color theme (optional)</param>
        /// <returns>Unique key for this tooltip instance</returns>
        public static Task<string> ShowTooltipAsync(string title, string text, Point location, ToolTipType? type = null)
        {
            var config = new ToolTipConfig
            {
                Title = title,
                Text = text,
                Position = location,
                Duration = DefaultHideDelay,
                Type = type ?? ToolTipType.Default,
                Placement = DefaultPlacement,
                Style =  BeepControlStyle.Material3
            };

            return ShowTooltipAsync(config);
        }

        #endregion

        #region Hide Methods

        /// <summary>
        /// Hide a specific tooltip by its key
        /// </summary>
        /// <param name="key">Tooltip key returned from ShowTooltipAsync</param>
        public static async Task HideTooltipAsync(string key)
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
        public static async Task HideAllTooltipsAsync()
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
                _activeTooltips.TryRemove(key, out _);
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
        public static void UpdateTooltip(string key, string newText, string newTitle = null)
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
        public static void UpdateTooltipPosition(string key, Point newPosition)
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
        public static bool IsTooltipActive(string key)
        {
            return !string.IsNullOrEmpty(key) && _activeTooltips.ContainsKey(key);
        }

        /// <summary>
        /// Get all active tooltip keys
        /// </summary>
        public static IEnumerable<string> GetActiveTooltipKeys()
        {
            return _activeTooltips.Keys;
        }

        #endregion
    }
}

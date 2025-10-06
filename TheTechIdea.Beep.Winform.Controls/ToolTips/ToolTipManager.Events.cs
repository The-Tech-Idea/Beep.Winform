using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// ToolTipManager - Event Handlers
    /// Handles mouse events and user interactions for control-attached tooltips
    /// </summary>
    public static partial class ToolTipManager
    {
        #region Control Event Handlers

        /// <summary>
        /// Handle mouse enter event for control-attached tooltips
        /// Shows tooltip after configured delay
        /// </summary>
        private static async Task OnControlMouseEnter(Control control, ToolTipConfig config)
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
        private static async Task OnControlMouseLeave(Control control, ToolTipConfig config)
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
        private static void OnControlMouseMove(Control control, ToolTipConfig config, MouseEventArgs e)
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

        #region Cleanup Timer

        /// <summary>
        /// Periodic cleanup timer callback
        /// Removes expired tooltip instances to prevent memory leaks
        /// </summary>
        private static void OnCleanupTimer(object state)
        {
            try
            {
                var expiredKeys = new System.Collections.Generic.List<string>();
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

        #region Helper Methods

        /// <summary>
        /// Calculate optimal tooltip position relative to control
        /// </summary>
        private static Point CalculateTooltipPosition(Control control, ToolTipPlacement placement)
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
    }
}

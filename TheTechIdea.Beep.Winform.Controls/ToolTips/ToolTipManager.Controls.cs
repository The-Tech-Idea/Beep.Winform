using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// ToolTipManager - Control Integration
    /// Handles attaching tooltips to WinForms controls with automatic hover behavior
    /// </summary>
    public static partial class ToolTipManager
    {
        #region Control Registration

        /// <summary>
        /// Attach a tooltip to a control with hover behavior
        /// Automatically handles mouse enter/leave events
        /// </summary>
        /// <param name="control">Target control</param>
        /// <param name="text">Tooltip text</param>
        /// <param name="config">Optional configuration (uses defaults if null)</param>
        public static void SetTooltip(Control control, string text, ToolTipConfig config = null)
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
        public static void SetTooltip(Control control, string title, string text, ToolTipType type = ToolTipType.Default,BeepControlStyle style= BeepControlStyle.Material3)
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
        public static void SetTooltip(Control control, string text, BeepControlStyle style, ToolTipType type = ToolTipType.Default)
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
        public static void RemoveTooltip(Control control)
        {
            if (control == null)
            {
                return;
            }

            if (_controlTooltips.TryRemove(control, out var key))
            {
                // Hide the tooltip if it's currently showing
                _ = HideTooltipAsync(key);

                // Note: We don't explicitly remove event handlers here
                // They will be garbage collected with the control
                // If needed, we could maintain handler references for explicit removal
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
        public static void UpdateControlTooltip(Control control, string newText)
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
        public static bool HasTooltip(Control control)
        {
            return control != null && _controlTooltips.ContainsKey(control);
        }

        /// <summary>
        /// Get the tooltip key for a control (if one is attached)
        /// </summary>
        public static string GetControlTooltipKey(Control control)
        {
            if (control == null)
            {
                return null;
            }

            return _controlTooltips.TryGetValue(control, out var key) ? key : null;
        }

        #endregion

        #region Batch Operations

        /// <summary>
        /// Remove tooltips from all controls
        /// Useful for cleanup or form disposal
        /// </summary>
        public static void RemoveAllControlTooltips()
        {
            var controls = new System.Collections.Generic.List<Control>(_controlTooltips.Keys);
            
            foreach (var control in controls)
            {
                RemoveTooltip(control);
            }
        }

        /// <summary>
        /// Set the same tooltip text on multiple controls
        /// </summary>
        public static void SetTooltipForControls(string text, params Control[] controls)
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

        /// <summary>
        /// Set the same themed tooltip on multiple controls
        /// </summary>
        public static void SetTooltipForControls(string text, ToolTipType type = ToolTipType.Default, params Control[] controls)
        {
            if (controls == null || controls.Length == 0)
            {
                return;
            }

            var config = new ToolTipConfig
            {
                Text = text,
                Type = type,
            };

            foreach (var control in controls)
            {
                if (control != null)
                {
                    SetTooltip(control, text, config);
                }
            }
        }

        #endregion
    }
}

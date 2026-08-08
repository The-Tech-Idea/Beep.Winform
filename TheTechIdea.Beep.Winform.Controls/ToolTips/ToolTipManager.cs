using TheTechIdea.Beep.Winform.Controls.Diagnostics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;

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

            // C8: Subscribe to theme changes. Tooltips already on-screen must
            // re-paint to match the new palette instead of waiting for the
            // next show. The previous placeholder try/catch silently swallowed
            // this — we now actually wire the handler.
            BeepThemesManager.ThemeChanged += OnThemeChanged;
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

        /// <summary>
        /// How long after a group's last tooltip closes that the next one in the same group skips
        /// its show delay.
        /// <para>
        /// Without this, sweeping a ten-button toolbar costs ten × <see cref="DefaultShowDelay"/> —
        /// five seconds of waiting to read ten labels. Every mature system special-cases the
        /// "user is already reading tooltips" state: Radix calls it <c>skipDelayDuration</c>,
        /// WinForms' own ToolTip calls it <c>ReshowDelay</c>.
        /// </para>
        /// </summary>
        public int SkipDelayWindow { get; set; } = 300;

        #endregion

        #region Delay groups

        // Last time a tooltip in each group was hidden. Only updated for tooltips that actually
        // became visible, so flicking the pointer across a toolbar without ever showing one does
        // not arm the skip window.
        private readonly ConcurrentDictionary<string, DateTime> _groupLastHidden = new();

        /// <summary>
        /// The delay group for a config: the explicit name, otherwise one derived from the anchor's
        /// parent so sibling controls share a group automatically.
        /// </summary>
        private static string ResolveDelayGroup(Control control, ToolTipConfig config)
        {
            if (!string.IsNullOrEmpty(config?.DelayGroup)) return config.DelayGroup;

            var parent = control?.Parent;
            if (parent == null) return "default";

            // RuntimeHelpers.GetHashCode is the *identity* hash: it ignores any GetHashCode
            // override the container might have. A control that overrode GetHashCode by value
            // could otherwise make two unrelated containers resolve to the same delay group, so
            // hovering in one would suppress the show delay in the other.
            int identity = System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(parent);
            return $"parent:{parent.GetType().Name}:{identity}";
        }

        /// <summary>
        /// The show delay to use, honouring the group's skip window.
        /// </summary>
        private int ResolveShowDelay(Control control, ToolTipConfig config)
        {
            int configured = config?.ShowDelay ?? DefaultShowDelay;
            if (configured <= 0) return 0;

            var group = ResolveDelayGroup(control, config);
            if (_groupLastHidden.TryGetValue(group, out var lastHidden)
                && (DateTime.UtcNow - lastHidden).TotalMilliseconds <= SkipDelayWindow)
            {
                return 0;   // still in the skip window — show immediately
            }

            return configured;
        }

        /// <summary>Records that a group just closed a tooltip, arming its skip window.</summary>
        private void MarkGroupHidden(Control control, ToolTipConfig config)
        {
            if (control == null) return;
            _groupLastHidden[ResolveDelayGroup(control, config)] = DateTime.UtcNow;
        }

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
                BeepLog.Failure(this, "Error showing tooltip", ex);
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
                    BeepLog.Failure(this, "Error hiding tooltip", ex);
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
                    BeepLog.Failure(this, "Error hiding all tooltips", ex);
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
                    BeepLog.Failure(this, "Error updating tooltip", ex);
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
                    BeepLog.Failure(this, "Error updating tooltip position", ex);
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

            // Remove existing tooltip first (also detaches any prior event handlers)
            RemoveTooltip(control);

            // Create or update configuration
            config ??= new ToolTipConfig();
            config.Text = text;
            // A GUID, not a hash code. Control.GetHashCode() is not an identity — two live controls
            // can share one, and it is not stable across a control's lifetime. The old key combined
            // it with DateTime.Now.Ticks to paper over collisions, which made the key neither
            // unique-by-construction nor stable for the same control. Lookups by control go through
            // _controlTooltips anyway, so the key only has to be unique.
            config.Key = $"control_{Guid.NewGuid():N}";
            config.Style = DefaultControlStyle;

            // Store control-tooltip mapping
            _controlTooltips[control] = config.Key;

            // Attach named event handlers so they can be cleanly unsubscribed in
            // RemoveTooltip. The previous anonymous-lambda approach leaked handlers
            // every time a tooltip was reassigned.
            AttachControlHandlers(control, config);

            // Expose the tooltip to assistive technology as the control's DESCRIPTION.
            //
            // This used to also write config.Title into control.AccessibleName, which *replaces*
            // the control's own name: a button labelled "Save" with a tooltip titled
            // "Save document" started announcing as "Save document". The tooltip describes the
            // control, it does not rename it — the same distinction as aria-describedby versus
            // aria-label. Nothing restored the previous values either, so removing a tooltip left
            // the host's own accessibility text overwritten.
            if (EnableAccessibility && _attachedHandlers.TryGetValue(control, out var attached))
            {
                attached.PriorAccessibleName = control.AccessibleName;
                attached.PriorAccessibleDescription = control.AccessibleDescription;
                attached.CapturedAccessibility = true;

                control.AccessibleDescription = string.IsNullOrEmpty(config.Title)
                    ? text
                    : $"{config.Title}. {text}";
            }
        }

        // Per-control named handler registry so RemoveTooltip can detach cleanly.
        // Stores the *actual* EventHandler/MouseEventHandler instances that were
        // subscribed to the control's events so they can be removed later. The
        // async work happens inside the handler — the handler itself is a normal
        // void-returning delegate (the returned Task is fire-and-forget).
        private sealed class TooltipHandlers
        {
            public EventHandler? EnterHandler;
            public EventHandler? LeaveHandler;
            public MouseEventHandler? MoveHandler;
            public EventHandler? GotFocusHandler;
            public EventHandler? LostFocusHandler;
            public EventHandler? ClickHandler;
            public EventHandler? DisposedHandler;

            // What the host had set before we attached, so RemoveTooltip can put it back.
            public string? PriorAccessibleName;
            public string? PriorAccessibleDescription;
            public bool CapturedAccessibility;
        }
        private readonly ConcurrentDictionary<Control, TooltipHandlers> _attachedHandlers = new();

        /// <summary>
        /// Subscribes the events that match the configured <see cref="ToolTipTriggerMode"/>.
        /// <para>
        /// <c>TriggerMode</c> was declared with four values and read by nothing, so every tooltip
        /// was hover-only no matter what the caller asked for — Focus, Click and Manual all behaved
        /// as Hover. Keyboard users in particular never saw a tooltip at all.
        /// </para>
        /// </summary>
        private void AttachControlHandlers(Control control, ToolTipConfig config)
        {
            var handlers = new TooltipHandlers();
            var mode = config.TriggerMode;

            if (mode == ToolTipTriggerMode.Hover)
            {
                handlers.EnterHandler = (s, e) => _ = OnControlMouseEnter(control, config);
                handlers.LeaveHandler = (s, e) => _ = OnControlMouseLeave(control, config);
                control.MouseEnter += handlers.EnterHandler;
                control.MouseLeave += handlers.LeaveHandler;
            }
            else if (mode == ToolTipTriggerMode.Click)
            {
                handlers.ClickHandler = (s, e) => _ = ToggleTooltipFor(control, config);
                control.Click += handlers.ClickHandler;
            }
            // Manual: nothing is subscribed; the host drives Show/Hide itself.

            // Focus triggers apply for the Focus mode, and are ALSO added on top of Hover when
            // KeyboardTriggerable is set: a hover-only tooltip is unreachable from the keyboard,
            // which is the accessibility half of WCAG 1.4.13.
            if (mode == ToolTipTriggerMode.Focus || (mode == ToolTipTriggerMode.Hover && config.KeyboardTriggerable))
            {
                handlers.GotFocusHandler = (s, e) => _ = OnControlMouseEnter(control, config);
                handlers.LostFocusHandler = (s, e) => _ = OnControlMouseLeave(control, config);
                control.GotFocus += handlers.GotFocusHandler;
                control.LostFocus += handlers.LostFocusHandler;
            }

            if (config.FollowCursor)
            {
                handlers.MoveHandler = (s, e) => OnControlMouseMove(control, config, e);
                control.MouseMove += handlers.MoveHandler;
            }

            // Release ourselves when the anchor dies.
            //
            // Both _controlTooltips and _attachedHandlers are keyed by Control, so a host that
            // disposes a control without calling RemoveTooltip — the normal case when a form
            // closes — left the manager holding the control forever. Measured: 20 disposed
            // anchors retained after 20 create/dispose cycles.
            handlers.DisposedHandler = (s, e) => RemoveTooltip(control);
            control.Disposed += handlers.DisposedHandler;

            _attachedHandlers[control] = handlers;
        }

        /// <summary>Click-trigger toggle: show if hidden, hide if already shown.</summary>
        private async Task ToggleTooltipFor(Control control, ToolTipConfig config)
        {
            if (_controlTooltips.TryGetValue(control, out var key) && _activeTooltips.ContainsKey(key))
            {
                await HideTooltipAsync(key);
                return;
            }
            await OnControlMouseEnter(control, config);
        }

        private void DetachControlHandlers(Control control)
        {
            if (_attachedHandlers.TryRemove(control, out var handlers))
            {
                if (handlers.EnterHandler != null)
                    control.MouseEnter -= handlers.EnterHandler;
                if (handlers.LeaveHandler != null)
                    control.MouseLeave -= handlers.LeaveHandler;
                if (handlers.MoveHandler != null)
                    control.MouseMove -= handlers.MoveHandler;
                if (handlers.GotFocusHandler != null)
                    control.GotFocus -= handlers.GotFocusHandler;
                if (handlers.LostFocusHandler != null)
                    control.LostFocus -= handlers.LostFocusHandler;
                if (handlers.ClickHandler != null)
                    control.Click -= handlers.ClickHandler;
                if (handlers.DisposedHandler != null)
                    control.Disposed -= handlers.DisposedHandler;

                // Put the host's own accessibility text back. Leaving ours behind would make a
                // removed tooltip permanently alter how the control announces itself.
                if (handlers.CapturedAccessibility && !control.IsDisposed)
                {
                    control.AccessibleName = handlers.PriorAccessibleName;
                    control.AccessibleDescription = handlers.PriorAccessibleDescription;
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
            }

            // Detach the handlers we registered. This also restores whatever accessibility text the
            // host had before the tooltip was attached.
            //
            // There used to be a `control.AccessibleDescription = string.Empty` here, which ran
            // *after* the restore and blanked it again — so removing a tooltip destroyed the host's
            // own description rather than returning it.
            DetachControlHandlers(control);
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
                // Wait for show delay — zero if this control's delay group is still inside its
                // skip window, so moving along a toolbar does not re-pay the delay per button.
                var delay = ResolveShowDelay(control, config);
                if (delay > 0)
                {
                    await Task.Delay(delay);
                }

                // Is showing still justified after the delay?
                //
                // This used to require the pointer to be over the control, full stop. That is right
                // for a hover trigger and wrong for every other one: a focus-triggered tooltip is
                // shown precisely when the pointer is elsewhere, so requiring it would have made
                // keyboard-triggered tooltips impossible even once TriggerMode was honoured.
                if (!control.IsDisposed && TriggerStillValid(control, config))
                {
                    // Anchor to the control's screen RECTANGLE. Positioning previously received
                    // only a point, so it could not align *Start / *End to the control's edges and
                    // placed the tooltip over the control it describes.
                    config.AnchorRect = control.RectangleToScreen(control.ClientRectangle);
                    config.AnchorControl = control;

                    // Kept for follow-cursor and for any caller still reading Position.
                    config.Position = CalculateTooltipPosition(control, config.Placement);

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
                BeepLog.FailureOnce("ToolTip.mouseEnter", this, "Error in OnControlMouseEnter", ex);
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
                if (!_controlTooltips.TryGetValue(control, out var key)) return;

                // A pinned tooltip stays until it is explicitly dismissed — leaving the anchor,
                // the close delay and the delay group are all irrelevant to it.
                if (config?.IsPinned == true) return;

                // The close delay does double duty: it stops flicker when moving between adjacent
                // controls, and it is the grace period during which the pointer can travel from the
                // anchor onto the tooltip. config.HideDelay was declared and never read — the delay
                // was a hard-coded 200ms.
                int closeDelay = config?.HideDelay ?? 200;
                await Task.Delay(closeDelay);

                // WCAG 1.4.13 "hoverable": additional content shown on hover must remain visible
                // while the pointer is over it. PersistOnHover was declared, documented as
                // implementing exactly this, and read by nothing.
                if (config?.PersistOnHover == true)
                {
                    while (IsPointerOverTooltip(key) || IsPointerOver(control))
                    {
                        await Task.Delay(100);
                    }

                    // Left the tooltip: give the same grace period again in case the pointer is
                    // travelling back to the anchor.
                    await Task.Delay(closeDelay);
                    if (IsPointerOverTooltip(key) || IsPointerOver(control)) return;
                }

                // Arm the group's skip window only if a tooltip was genuinely on screen. Flicking
                // the pointer across a toolbar never shows one, so it must not make the next hover
                // instant.
                bool wasVisible = _activeTooltips.ContainsKey(key);

                await HideTooltipAsync(key);

                if (wasVisible) MarkGroupHidden(control, config);
            }
            catch (Exception ex)
            {
                BeepLog.FailureOnce("ToolTip.mouseLeave", this, "Error in OnControlMouseLeave", ex);
            }
        }

        /// <summary>
        /// After the show delay elapses, is there still a reason to show this tooltip?
        /// Hover requires the pointer; Focus requires focus; Click and Manual are already an
        /// explicit user action so they need no further confirmation.
        /// </summary>
        private static bool TriggerStillValid(Control control, ToolTipConfig config)
        {
            try
            {
                return config.TriggerMode switch
                {
                    ToolTipTriggerMode.Hover =>
                        IsPointerOver(control) || (config.KeyboardTriggerable && control.Focused),
                    ToolTipTriggerMode.Focus => control.Focused,
                    _ => true
                };
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }

        /// <summary>True when the pointer is over the live tooltip window for <paramref name="key"/>.</summary>
        private bool IsPointerOverTooltip(string key)
            => !string.IsNullOrEmpty(key)
               && _activeTooltips.TryGetValue(key, out var instance)
               && instance.IsPointerOver();

        /// <summary>True when the pointer is over the anchor control.</summary>
        private static bool IsPointerOver(Control control)
        {
            try
            {
                return control != null && !control.IsDisposed && control.Visible
                       && control.RectangleToScreen(control.ClientRectangle).Contains(Cursor.Position);
            }
            catch (ObjectDisposedException)
            {
                return false;
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
                BeepLog.FailureOnce("ToolTip.mouseMove", this, "Error in OnControlMouseMove", ex);
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
                BeepLog.Failure(this, "Error calculating tooltip position", ex);
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
                            BeepLog.FailureOnce("ToolTip.cleanup", this, "Error during cleanup", ex);
                        }
                    });
                }

                // Log cleanup activity if any tooltips were removed
                if (expiredKeys.Count > 0)
                {
                    BeepLog.Info(this, "cleanup", $"cleaned up {expiredKeys.Count} expired tooltips");
                }
            }
            catch (Exception ex)
            {
                BeepLog.FailureOnce("ToolTip.cleanupTimer", this, "Error in cleanup timer", ex);
            }
        }

        #endregion

        #region C8: Theme propagation

        /// <summary>
        /// C8: When the app theme changes, push the new theme into every
        /// active tooltip instance so the on-screen tooltips re-paint with
        /// the new palette. Falls back to the manager default if the event
        /// arg doesn't carry an IBeepTheme.
        /// </summary>
        private void OnThemeChanged(object sender, ThemeChangeEventArgs e)
        {
            IBeepTheme theme = e?.NewTheme ?? BeepThemesManager.CurrentTheme;
            if (theme == null) return;

            foreach (var kvp in _activeTooltips)
            {
                var instance = kvp.Value;
                if (instance == null) continue;
                var tip = instance.ToolTip;
                if (tip == null || tip.IsDisposed) continue;

                try
                {
                    tip.ApplyTheme(theme);
                }
                catch
                {
                    // One bad tooltip should not block the rest.
                }
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

            // C8: unsubscribe from theme changes so the static event
            // doesn't keep a reference to this disposed singleton.
            try
            {
                BeepThemesManager.ThemeChanged -= OnThemeChanged;
            }
            catch
            {
                // Same fallback as the subscribe site.
            }

            // Hide all tooltips
            var hideTask = HideAllTooltipsAsync();
            hideTask.Wait(TimeSpan.FromSeconds(2)); // Wait max 2 seconds

            // Clear collections
            _activeTooltips.Clear();
            _controlTooltips.Clear();
        }

        #endregion

        #region Extended API — Popover, Preview, Tour

        // ── Popovers ──────────────────────────────────────────────────────────

        // Tracks popovers by owning control so they can be dismissed individually.
        private readonly System.Collections.Concurrent.ConcurrentDictionary<Control, string>
            _popoverKeys = new System.Collections.Concurrent.ConcurrentDictionary<Control, string>();

        /// <summary>
        /// Show a persistent <see cref="BeepPopover"/> anchored to <paramref name="target"/>.
        /// Any previously shown popover for the same target is dismissed first.
        /// </summary>
        /// <param name="target">The control the popover points at.</param>
        /// <param name="config">Popover-specific configuration.</param>
        /// <returns>The unique key for this popover instance.</returns>
        public async Task<string> ShowPopoverAsync(Control target, PopoverConfig config)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (config  == null) throw new ArgumentNullException(nameof(config));

            // Dismiss existing popover for this target
            await DismissPopoverAsync(target);

            // Resolve screen position
            if (config.Position == Point.Empty)
            {
                var scrBounds = target.RectangleToScreen(target.ClientRectangle);
                var resolved  = ToolTips.Helpers.ToolTipPositionResolver.Resolve(
                    scrBounds, new Size(config.MaxPopoverWidth, 200), config.Placement, config.Offset);
                config.Position  = resolved.Location;
                config.Placement = resolved.ActualPlacement;
            }

            // Popovers never auto-hide
            config.Duration = 0;

            string key = await ShowTooltipAsync(config);
            _popoverKeys[target] = key;
            return key;
        }

        /// <summary>Dismiss the popover currently shown for <paramref name="target"/>.</summary>
        public async Task DismissPopoverAsync(Control target)
        {
            if (target == null) return;
            if (_popoverKeys.TryRemove(target, out string key))
                await HideTooltipAsync(key);
        }

        /// <summary>Synchronous fire-and-forget dismiss.</summary>
        public void DismissPopover(Control target) => _ = DismissPopoverAsync(target);

        // ── Preview tooltips ──────────────────────────────────────────────────

        /// <summary>
        /// Show a hover-card / preview tooltip for <paramref name="target"/>.
        /// Sets <see cref="ToolTipLayoutVariant.Preview"/> and resolves position automatically.
        /// </summary>
        public async Task<string> ShowPreviewAsync(Control target, ToolTipConfig config)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (config  == null) throw new ArgumentNullException(nameof(config));

            config.LayoutVariant = ToolTipLayoutVariant.Preview;

            if (config.Position == Point.Empty)
            {
                var scrBounds = target.RectangleToScreen(target.ClientRectangle);
                var sz        = config.PreviewImageSize.Width > 0
                                ? new Size(config.PreviewImageSize.Width, config.PreviewImageSize.Height + 80)
                                : new Size(300, 260);
                var resolved  = ToolTips.Helpers.ToolTipPositionResolver.Resolve(
                    scrBounds, sz, config.Placement, config.Offset);
                config.Position  = resolved.Location;
                config.Placement = resolved.ActualPlacement;
            }

            return await ShowTooltipAsync(config);
        }

        // ── Guided tour ───────────────────────────────────────────────────────

        /// <summary>
        /// Get a fluent <see cref="BeepTourBuilder"/> to construct and start a guided tour.
        /// </summary>
        public BeepTourBuilder CreateTour() => BeepTourManager.Instance.CreateTour();

        #endregion

        #region Theme Management

        /// <summary>
        /// Apply theme to all active tooltips
        /// Called when theme changes to update existing tooltips
        /// </summary>
        public void ApplyThemeToAll(IBeepTheme theme)
        {
            if (theme == null) return;

            foreach (var kvp in _activeTooltips)
            {
                var tip = kvp.Value?.ToolTip;
                if (tip == null || tip.IsDisposed) continue;

                try
                {
                    tip.ApplyTheme(theme);
                }
                catch (Exception ex)
                {
                    Diagnostics.BeepLog.FailureOnce($"ToolTip.retheme:{kvp.Key}", this,
                        $"apply theme to tooltip '{kvp.Key}'", ex);
                }
            }
        }

        #endregion
    }
}

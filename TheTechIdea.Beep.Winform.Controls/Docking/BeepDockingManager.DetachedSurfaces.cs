using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Runtime;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// Lifetime of the surfaces that host a panel when it is <b>not</b> in the docked layout: float
    /// windows and auto-hide strips.
    /// </summary>
    /// <remarks>
    /// Creating and tearing these down is where a panel most easily ends up in a state nothing
    /// backs — the manager still believing a panel floats after its window has been destroyed is
    /// what made a float saved on a since-removed display unrecoverable, because three separate
    /// decisions all read that stale state and skipped the panel.
    /// <para>
    /// So the rule this file exists to keep is: <b>when the surface goes, the panel's state goes
    /// with it.</b> A change to how a float or strip is created or destroyed belongs here; a change
    /// to what floating or auto-hiding <i>means</i> to the layout does not.
    /// </para>
    /// </remarks>
    public partial class BeepDockingManager
    {
        private void CloseFloatWindowFor(DockPanel panel)
        {
            if (panel == null || string.IsNullOrWhiteSpace(panel.Key))
                return;

            if (_floatWindowsByKey.TryGetValue(panel.Key, out var tracked) && tracked != null && !tracked.IsDisposed)
            {
                tracked.PanelRedocked -= OnFloatWindowRedocked;
                tracked.ExtractHostedPanel();
                OnFloatingWindowRemoved(new FloatingWindowEventArgs(tracked, panel));
                tracked.Close();
                _floatWindowsByKey.Remove(panel.Key);
                return;
            }

            if (_hostForm == null)
                return;

            foreach (Form owned in _hostForm.OwnedForms)
            {
                if (owned is FloatWindow fw && ReferenceEquals(fw.Panel, panel))
                {
                    fw.PanelRedocked -= OnFloatWindowRedocked;
                    fw.ExtractHostedPanel();
                    OnFloatingWindowRemoved(new FloatingWindowEventArgs(fw, panel));
                    fw.Close();
                    _floatWindowsByKey.Remove(panel.Key);
                    break;
                }
            }
        }

        /// <summary>
        /// Closes every float window and returns its panel to the docked pool.
        /// </summary>
        /// <remarks>
        /// The state reset is the point. Leaving a panel at
        /// <see cref="DockPanelState.Floating"/> after its window has been destroyed leaves a state
        /// nothing backs, and every subsequent decision reads it: <see cref="FloatPanel"/> returns
        /// early because "it is already floating", and restore skips it for the same reason - so a
        /// float saved on a display that no longer exists is never re-placed and stays unreachable.
        /// The panel becomes <see cref="DockPanelState.Docked"/>; whatever is applied next decides
        /// where it actually goes, including floating it again.
        /// </remarks>
        private void CloseAllFloatWindows()
        {
            foreach (var fw in _floatWindowsByKey.Values.ToList())
            {
                if (fw == null || fw.IsDisposed)
                    continue;

                // Captured before extraction, which detaches the panel from the window.
                var panel = fw.Panel;

                fw.PanelRedocked -= OnFloatWindowRedocked;
                fw.ExtractHostedPanel();
                fw.Close();

                if (panel != null && panel.State == DockPanelState.Floating)
                    panel.State = DockPanelState.Docked;
            }

            _floatWindowsByKey.Clear();
        }

        private void ClearAllAutoHidePanels()
        {
            foreach (var strip in _autoHideStrips.Values)
            {
                if (strip == null)
                    continue;

                foreach (var panel in strip.Panels.ToList())
                    strip.RemovePanel(panel);

                strip.Visible = false;
            }
        }

        private void DetachFromAutoHideStrip(DockPanel panel)
        {
            if (panel == null)
                return;

            foreach (var kv in _autoHideStrips)
            {
                var strip = kv.Value;
                if (strip == null || !strip.Panels.Contains(panel))
                    continue;

                strip.RemovePanel(panel);
                if (strip.Panels.Count == 0)
                    strip.Visible = false;

                OnAutoHiddenGroupRemoved(new AutoHiddenGroupEventArgs(panel, kv.Key));
                break;
            }

            if (panel.Parent != null && panel.Parent != _hostForm)
                panel.Parent.Controls.Remove(panel);
        }

        /// <summary>
        /// Creates one <see cref="AutoHideStrip"/> per edge and adds them to the host form.
        /// Called once from <see cref="ManageControl"/> after the host form is known.
        /// Mirrors DockPanelSuite's per-edge auto-hide strip construction.
        /// </summary>
        private void CreateAutoHideStrips()
        {
            if (_hostForm == null || IsDesignHosted) return;

            var edges = new[] { DockPosition.Left, DockPosition.Right, DockPosition.Top, DockPosition.Bottom };
            foreach (var edge in edges)
            {
                if (_autoHideStrips.ContainsKey(edge))
                    continue;

                var strip = new AutoHideStrip(edge, _hostForm);
                strip.ControlStyle = _style;
                strip.ApplyDockingTheme(_themeColors);
                strip.Visible = false;   // only show when a panel is auto-hidden on this edge
                strip.PanelRestoreRequested += OnStripRestoreRequested;
                strip.PanelCloseRequested += OnStripCloseRequested;
                strip.PanelFloatRequested += OnStripFloatRequested;
                strip.SlideShown += OnStripSlideShown;
                strip.SlideLayoutChanged += (_, __) => RecalculateLayout();
                strip.SlideSeparatorResized += (_, e) =>
                {
                    OnAutoHiddenSeparatorResize(e);
                    RecalculateLayout();
                };
                _autoHideStrips[edge] = strip;
            }
        }

        /// <summary>
        /// Handles a FloatWindow requesting to re-dock its panel.
        /// </summary>
        private void OnFloatWindowRedocked(object sender, DockPanel panel)
        {
            if (panel == null) return;
            DockFloatingPanel(panel.Key, panel.DockPosition);
        }
    }
}

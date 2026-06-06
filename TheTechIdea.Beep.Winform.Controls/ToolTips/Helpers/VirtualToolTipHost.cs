using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Sprint 12 — Default Win32/WinForms implementation of <see cref="IToolTipHost"/>.
    /// 
    /// Wraps a <see cref="CustomToolTip"/> form and exposes it through the host abstraction,
    /// allowing callers to work against the interface without depending on the concrete form.
    /// 
    /// Usage:
    /// <code>
    /// IToolTipHost host = new VirtualToolTipHost();
    /// await host.ShowAsync(config, Cursor.Position);
    /// </code>
    /// </summary>
    public sealed class VirtualToolTipHost : IToolTipHost, IDisposable
    {
        // ──────────────────────────────────────────────────────────────
        // Fields
        // ──────────────────────────────────────────────────────────────

        private CustomToolTip _tip;
        private bool          _disposed;

        // ──────────────────────────────────────────────────────────────
        // Events
        // ──────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public event EventHandler Shown;

        /// <inheritdoc/>
        public event EventHandler Hidden;

        // ──────────────────────────────────────────────────────────────
        // IToolTipHost — properties
        // ──────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public bool IsVisible => _tip?.Visible ?? false;

        // ──────────────────────────────────────────────────────────────
        // IToolTipHost — show / hide
        // ──────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public async Task ShowAsync(ToolTipConfig config, Point screenLocation)
        {
            EnsureTip();
            _tip.ApplyConfig(config);

            // B2: Use the same positioning pipeline as CustomToolTip.ShowAsync
            // so callers that pass an anchor point get the full collision
            // detection / screen clamp behavior — not a raw Location set that
            // can clip the tooltip off the right or bottom edge of a monitor.
            var anchor = new Rectangle(screenLocation, new Size(1, 1));
            var (placement, finalPosition) = ToolTipPositioningHelpers.FindBestPlacement(
                anchor, _tip.Size, config.Placement, config.Offset);
            _tip.Location = finalPosition;
            _tip.Show();
            Shown?.Invoke(this, EventArgs.Empty);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task HideAsync()
        {
            if (_tip?.Visible == true)
            {
                _tip.Hide();
                Hidden?.Invoke(this, EventArgs.Empty);
            }
            await Task.CompletedTask;
        }

        // ──────────────────────────────────────────────────────────────
        // IToolTipHost — geometry
        // ──────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public Size MeasureSize(ToolTipConfig config)
        {
            EnsureTip();
            // Use the layout helper for a graphics-accurate measurement
            using var g = _tip.CreateGraphics();
            return ToolTipLayoutHelpers.CalculateOptimalSize(g, config,
                padding: 8, spacing: 4, minWidth: 120, maxWidth: 400);
        }

        /// <inheritdoc/>
        public Rectangle GetScreenBounds()
        {
            // Return the work area of the primary screen
            return Screen.PrimaryScreen.WorkingArea;
        }

        // ──────────────────────────────────────────────────────────────
        // Helpers
        // ──────────────────────────────────────────────────────────────

        private void EnsureTip()
        {
            if (_tip == null || _tip.IsDisposed)
            {
                _tip = new CustomToolTip();
                _tip.VisibleChanged += (s, e) =>
                {
                    if (!_tip.Visible)
                        Hidden?.Invoke(this, EventArgs.Empty);
                };
            }
        }

        // ──────────────────────────────────────────────────────────────
        // IDisposable
        // ──────────────────────────────────────────────────────────────

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _tip?.Dispose();
            _tip = null;
        }
    }
}

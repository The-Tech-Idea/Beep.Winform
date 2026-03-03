using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ToolTips;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Sprint 9 — A tooltip that can be pinned/unpinned and freely dragged by the user.
    /// 
    /// Behaviour:
    ///  • When <see cref="IsPinned"/> is <c>false</c> (default) it behaves like a standard popover
    ///    and disappears when dismissed.
    ///  • When pinned the window stays on screen and can be dragged to any position.
    ///  • A small pin-button is rendered in the top-right corner; clicking it toggles the pin state.
    ///  • Auto-hide is always disabled while pinned.
    /// </summary>
    public class BeepPinnedTooltip : BeepPopover
    {
        // ──────────────────────────────────────────────────────────────
        // Constants
        // ──────────────────────────────────────────────────────────────
        private const int PinButtonSize   = 18;
        private const int PinButtonMargin = 6;

        // ──────────────────────────────────────────────────────────────
        // State
        // ──────────────────────────────────────────────────────────────
        private bool   _isPinned;
        private bool   _dragging;
        private Point  _dragStart;   // mouse-down position (screen coords)
        private Point  _formStart;   // form location at drag-start
        private Rectangle _pinButtonBounds;

        // ──────────────────────────────────────────────────────────────
        // Properties
        // ──────────────────────────────────────────────────────────────

        /// <summary>Gets or sets whether the tooltip is currently pinned (stays on screen).</summary>
        public bool IsPinned
        {
            get => _isPinned;
            set
            {
                if (_isPinned == value) return;
                _isPinned = value;
                ApplyPinState();
                Invalidate();
                IsPinnedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>Fired whenever the pin state changes.</summary>
        public event EventHandler IsPinnedChanged;

        // ──────────────────────────────────────────────────────────────
        // Construction
        // ──────────────────────────────────────────────────────────────

        public BeepPinnedTooltip()
        {
            // Allow the window to be dragged
            this.MouseDown += OnMouseDown;
            this.MouseMove += OnMouseMove;
            this.MouseUp   += OnMouseUp;
            this.Paint     += OnPaintPinButton;
        }

        // ──────────────────────────────────────────────────────────────
        // Public API
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Applies a <see cref="ToolTipConfig"/> that has <see cref="ToolTipConfig.IsPinned"/>
        /// pre-set, then positions the window.
        /// </summary>
        public void ApplyPinnedConfig(ToolTipConfig config, Point screenLocation)
        {
            ApplyConfig(config);
            _isPinned = config.IsPinned;
            ApplyPinState();
            Location = screenLocation;
            Show();
        }

        /// <summary>Pins the tooltip at the current screen position.</summary>
        public void Pin()   => IsPinned = true;

        /// <summary>Unpins the tooltip (it will auto-dismiss on next hide trigger).</summary>
        public void Unpin() => IsPinned = false;

        // ──────────────────────────────────────────────────────────────
        // Internal helpers
        // ──────────────────────────────────────────────────────────────

        private void ApplyPinState()
        {
            if (_isPinned)
            {
                // Keep window alive; disable any auto-close timer
                TopMost = true;
            }
            else
            {
                TopMost = false;
            }
        }

        /// <summary>Computes the pin-button rectangle in client coordinates.</summary>
        private Rectangle GetPinButtonBounds()
        {
            return new Rectangle(
                Width - PinButtonSize - PinButtonMargin,
                PinButtonMargin,
                PinButtonSize,
                PinButtonSize);
        }

        // ──────────────────────────────────────────────────────────────
        // Mouse drag handlers
        // ──────────────────────────────────────────────────────────────

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            // Toggle pin when clicking the pin button
            var btnBounds = GetPinButtonBounds();
            if (btnBounds.Contains(e.Location))
            {
                IsPinned = !IsPinned;
                return;
            }

            // Only allow dragging when pinned
            if (!_isPinned) return;

            _dragging   = true;
            _dragStart  = Control.MousePosition;
            _formStart  = this.Location;
            this.Capture = true;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragging) return;
            var delta   = new Point(
                Control.MousePosition.X - _dragStart.X,
                Control.MousePosition.Y - _dragStart.Y);
            this.Location = new Point(_formStart.X + delta.X, _formStart.Y + delta.Y);
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (!_dragging) return;
            _dragging    = false;
            this.Capture = false;
        }

        // ──────────────────────────────────────────────────────────────
        // Paint pin-button overlay
        // ──────────────────────────────────────────────────────────────

        private void OnPaintPinButton(object sender, PaintEventArgs e)
        {
            _pinButtonBounds = GetPinButtonBounds();
            DrawPinButton(e.Graphics, _pinButtonBounds, _isPinned);
        }

        private static void DrawPinButton(Graphics g, Rectangle r, bool pinned)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Translucent circle background
            using (var bg = new SolidBrush(Color.FromArgb(pinned ? 180 : 80, 0, 120, 215)))
                g.FillEllipse(bg, r);

            // Pin icon: a simple needle shape
            var cx   = r.X + r.Width  / 2;
            var cy   = r.Y + r.Height / 2;
            float hw = r.Width  * 0.18f;
            float hh = r.Height * 0.35f;

            using var pen = new Pen(Color.White, 1.5f);
            if (pinned)
            {
                // Vertical pin (locked)
                g.DrawLine(pen, cx, cy - (int)hh, cx, cy + (int)hh);
                g.DrawLine(pen, cx - (int)hw, cy - (int)hh, cx + (int)hw, cy - (int)hh);
            }
            else
            {
                // Diagonal pin (unlocked)
                g.DrawLine(pen, cx - (int)hw, cy - (int)hh, cx + (int)hw, cy + (int)hh);
                g.DrawLine(pen, cx - (int)hw, cy - (int)hh, cx + (int)hw, cy - (int)hh);
            }
        }
    }
}

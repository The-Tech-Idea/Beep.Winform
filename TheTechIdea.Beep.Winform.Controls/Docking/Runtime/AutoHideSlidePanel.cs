using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime
{
    /// <summary>
    /// An animated slide panel that reveals an auto-hidden <see cref="DockPanel"/> content area.
    /// One instance is shared per <see cref="AutoHideStrip"/> (one per edge).
    ///
    /// Animation model (follows DockPanelSuite AutoHideWindowControl.AnimateWindow):
    /// - Total animation time: ~100 ms (ANIMATE_TIME), 10 ms tick → ~10 steps.
    /// - Slide direction is determined by <see cref="Edge"/>:
    ///     Left  → grow right  (X fixed, Width grows)
    ///     Right → grow left   (X shrinks, Width grows)
    ///     Top   → grow down   (Y fixed, Height grows)
    ///     Bottom→ grow up     (Y shrinks, Height grows)
    /// - No persistence — purely visual; actual panel state managed by BeepDockingManager.
    ///
    /// Reference files:
    ///   dockpanelsuite-master\WinFormsUI\Docking\DockPanel.AutoHideWindow.cs  (AnimateWindow)
    ///   Krypton.Docking\Control Docking\KryptonAutoHiddenSlidePanel.cs (concept)
    /// </summary>
    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [DesignTimeVisible(false)]
    public class AutoHideSlidePanel : Panel
    {
        // ── Constants (mirrors DockPanelSuite ANIMATE_TIME) ────────────────
        private const int AnimateTime    = 100;   // total ms
        private const int AnimateTickMs  = 10;    // timer interval ms
        private const int AnimateSteps   = AnimateTime / AnimateTickMs;  // = 10

        // ── Fields ─────────────────────────────────────────────────────────
        private readonly DockPosition _edge;
        private DockPanel _hostedPanel;
        private Timer _animTimer;
        private bool _slidingIn;     // true = growing; false = shrinking
        private int  _step;          // current animation step
        private int  _targetSize;    // full preferred size (Width or Height)
        private DockingThemeColors _themeColors = DockingThemeColors.Default;

        // ── Constructor ─────────────────────────────────────────────────────

        public AutoHideSlidePanel(DockPosition edge)
        {
            _edge = edge;

            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint  |
                     ControlStyles.ResizeRedraw, true);

            BorderStyle = BorderStyle.FixedSingle;
            Visible     = false;
            BackColor   = _themeColors.SlidePanelBackColor;

            _animTimer = new Timer { Interval = AnimateTickMs };
            _animTimer.Tick += OnAnimTick;

            ApplyEdgeDock();
        }

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>The edge this slide panel is anchored to.</summary>
        public DockPosition Edge => _edge;

        /// <summary>The DockPanel currently hosted in this slide panel (null when hidden).</summary>
        public DockPanel HostedPanel => _hostedPanel;

        internal void ApplyDockingTheme(DockingThemeColors colors)
        {
            _themeColors = colors ?? DockingThemeColors.Default;
            BackColor = _themeColors.SlidePanelBackColor;
            ForeColor = _themeColors.PanelForeColor;
            _hostedPanel?.ApplyDockingTheme(_themeColors);
            Invalidate();
        }

        /// <summary>
        /// Begins the slide-in animation and shows the given panel.
        /// If another panel is already hosted, it is removed first.
        /// </summary>
        public void Show(DockPanel panel)
        {
            if (panel == null) return;

            // Remove previous hosted panel if different
            if (_hostedPanel != null && _hostedPanel != panel)
            {
                Controls.Remove(_hostedPanel);
                _hostedPanel.Visible = false;
            }

            _hostedPanel  = panel;
            _hostedPanel.ApplyDockingTheme(_themeColors);
            _targetSize   = IsVertical ? panel.PreferredWidth : panel.PreferredHeight;
            if (_targetSize <= 0) _targetSize = 200;

            // Place panel inside slide control
            if (!Controls.Contains(panel))
            {
                panel.Dock    = DockStyle.Fill;
                panel.Visible = true;
                Controls.Add(panel);
            }

            SetSizeToZero();
            Visible    = true;
            BringToFront();

            _slidingIn = true;
            _step      = 0;
            _animTimer.Start();
        }

        /// <summary>
        /// Begins the slide-out (collapse) animation.
        /// </summary>
        public new void Hide()
        {
            if (!Visible) return;
            _slidingIn = false;
            _step      = 0;
            _animTimer.Start();
        }

        // ── Layout helpers ───────────────────────────────────────────────────

        private bool IsVertical => (_edge == DockPosition.Left || _edge == DockPosition.Right);

        private void ApplyEdgeDock()
        {
            switch (_edge)
            {
                case DockPosition.Left:
                    Dock  = DockStyle.Left;
                    Width = 0;
                    break;
                case DockPosition.Right:
                    Dock  = DockStyle.Right;
                    Width = 0;
                    break;
                case DockPosition.Top:
                    Dock   = DockStyle.Top;
                    Height = 0;
                    break;
                case DockPosition.Bottom:
                    Dock   = DockStyle.Bottom;
                    Height = 0;
                    break;
            }
        }

        private void SetSizeToZero()
        {
            if (IsVertical) Width  = 0;
            else            Height = 0;
        }

        private int CurrentSize
        {
            get => IsVertical ? Width  : Height;
            set
            {
                if (IsVertical) Width  = value;
                else            Height = value;
            }
        }

        // ── Animation tick — mirrors DockPanelSuite AnimateWindow loop ────────

        private void OnAnimTick(object sender, EventArgs e)
        {
            _step++;
            int newSize;

            if (_slidingIn)
            {
                // Ease in: size = targetSize * (step / totalSteps)
                newSize = (_targetSize * _step) / AnimateSteps;
                if (_step >= AnimateSteps || newSize >= _targetSize)
                {
                    newSize = _targetSize;
                    _animTimer.Stop();
                }
            }
            else
            {
                // Ease out: shrink from _targetSize → 0
                newSize = _targetSize - (_targetSize * _step) / AnimateSteps;
                if (_step >= AnimateSteps || newSize <= 0)
                {
                    newSize = 0;
                    _animTimer.Stop();
                    Visible      = false;
                    _hostedPanel = null;
                }
            }

            CurrentSize = newSize;
            Parent?.PerformLayout();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animTimer.Stop();
                _animTimer.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

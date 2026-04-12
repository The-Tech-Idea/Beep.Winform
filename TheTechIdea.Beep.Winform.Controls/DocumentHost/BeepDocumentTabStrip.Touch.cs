// BeepDocumentTabStrip.Touch.cs
// Sprint 18.1 — Native WM_TOUCH gesture support.
//
// Gestures implemented:
//   • Swipe left / right    → scroll the tab strip
//   • Long-press on tab     → open the context menu for that tab
//   • Pinch on tab strip    → cycle TabSizeMode (Equal → Compact → Fixed)
//   • Swipe down + hold     → raise TabFloatRequested for the touched tab
//
// WM_TOUCH is registered via CreateParams / OnHandleCreated using the
// Win32 RegisterTouchWindow API so that WinForms forwards touch packets
// to the window's message queue rather than converting them to mouse events.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentTabStrip
    {
        // ─────────────────────────────────────────────────────────────────────
        // Win32 P/Invoke — touch input
        // ─────────────────────────────────────────────────────────────────────

        private const int WM_TOUCH              = 0x0240;
        private const uint TWF_WANTPALM         = 0x00000002;

        private const uint TOUCHEVENTF_MOVE     = 0x0001;
        private const uint TOUCHEVENTF_DOWN     = 0x0002;
        private const uint TOUCHEVENTF_UP       = 0x0004;
        private const uint TOUCHEVENTF_PRIMARY  = 0x0010;

        [StructLayout(LayoutKind.Sequential)]
        private struct TOUCHINPUT
        {
            public int   x;          // in units of 1/100 of a pixel (HIMETRIC)
            public int   y;
            public IntPtr hSource;
            public int   dwID;
            public uint  dwFlags;
            public uint  dwMask;
            public uint  dwTime;
            public IntPtr dwExtraInfo;
            public uint  cxContact;
            public uint  cyContact;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterTouchWindow(IntPtr hwnd, uint ulFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetTouchInputInfo(IntPtr hTouchInput, uint cInputs,
            [MarshalAs(UnmanagedType.LPArray), In, Out] TOUCHINPUT[] pInputs,
            int cbSize);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool CloseTouchInputHandle(IntPtr lParam);

        // ─────────────────────────────────────────────────────────────────────
        // Touch state
        // ─────────────────────────────────────────────────────────────────────

        private readonly Dictionary<int, PointF> _touchStart  = new Dictionary<int, PointF>();
        private readonly Dictionary<int, PointF> _touchCurrent= new Dictionary<int, PointF>();

        // Long-press timer
        private readonly Timer _longPressTimer = new Timer { Interval = 600 };
        private int   _longPressTabIndex = -1;
        private Point _longPressPt;

        // Pinch tracking
        private float _pinchStartDist;
        private bool  _pinchActive;

        // Swipe-down float state
        private bool  _floatGestureArmed;
        private int   _floatTabIndex = -1;
        private const float SwipeDownThreshold = 30f;  // logical px
        private const float SwipeUpThreshold   = -30f; // logical px (negative = upward)

        // Double-tap detection
        private int   _lastTapIndex = -1;
        private long  _lastTapTick  = 0;
        private const int DoubleTapMs = 400;

        // Three-finger swipe
        private float _threeFingerStartX;
        private bool  _threeFingerArmed;
        private const float ThreeFingerSwipeThreshold = 60f;

        // Two-finger vertical scroll
        private float _twoFingerStartY;
        private bool  _twoFingerScrollArmed;

        // Event raised when a double-tap requests split maximise/restore
        public event EventHandler<TabEventArgs>? TouchSplitMaximizeRequested;

        // Event raised when a three-finger swipe requests workspace switch
        // +1 = forward, -1 = backward
        public event EventHandler<int>?          TouchWorkspaceSwitchRequested;
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Enables native WM_TOUCH messages on this window after the underlying
        /// Win32 HWND has been created.  Called automatically by <see cref="OnHandleCreated"/>.
        /// </summary>
        private void RegisterTouchInput()
        {
            try
            {
                if (IsHandleCreated)
                    RegisterTouchWindow(Handle, TWF_WANTPALM);
            }
            catch { /* Not available on older OS — silently skip */ }

            _longPressTimer.Tick += OnLongPressTimerTick;
        }

        // Hook into the existing OnHandleCreated (partial method — called from BeepDocumentTabStrip.cs)
        // We append the call to RegisterTouchInput at the bottom of OnHandleCreated.
        // Because OnHandleCreated is not a partial method, we override WndProc and
        // call RegisterTouchInput from OnHandleCreated via a small hook helper.

        // ─────────────────────────────────────────────────────────────────────
        // WndProc — intercept WM_TOUCH
        // ─────────────────────────────────────────────────────────────────────

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_TOUCH)
            {
                ProcessTouchMessage(m.WParam, m.LParam);
                m.Result = IntPtr.Zero;
                return;   // do NOT call base — prevents double-processing
            }
            base.WndProc(ref m);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Touch packet processing
        // ─────────────────────────────────────────────────────────────────────

        private void ProcessTouchMessage(IntPtr wParam, IntPtr lParam)
        {
            uint count = (uint)(wParam.ToInt32() & 0xFFFF);
            if (count == 0) return;

            var inputs = new TOUCHINPUT[count];
            if (!GetTouchInputInfo(lParam, count, inputs, Marshal.SizeOf<TOUCHINPUT>()))
                return;

            try
            {
                foreach (var ti in inputs)
                {
                    // Convert HIMETRIC (1/100 px) to screen pixels then to client coordinates
                    Point screenPt = new Point(ti.x / 100, ti.y / 100);
                    Point clientPt = PointToClient(screenPt);
                    var   pt       = new PointF(clientPt.X, clientPt.Y);

                    if ((ti.dwFlags & TOUCHEVENTF_DOWN) != 0)
                        OnTouchDown(ti.dwID, pt);
                    else if ((ti.dwFlags & TOUCHEVENTF_MOVE) != 0)
                        OnTouchMove(ti.dwID, pt);
                    else if ((ti.dwFlags & TOUCHEVENTF_UP) != 0)
                        OnTouchUp(ti.dwID, pt);
                }
            }
            finally
            {
                CloseTouchInputHandle(lParam);
            }
        }

        private void OnTouchDown(int id, PointF pt)
        {
            _touchStart[id]   = pt;
            _touchCurrent[id] = pt;

            if (_touchStart.Count == 1)
            {
                // Arm long-press for this touch point
                _longPressPt      = Point.Round(pt);
                _longPressTabIndex = HitTestTabIndex(_longPressPt);
                _longPressTimer.Start();

                // Arm float gesture
                _floatTabIndex    = _longPressTabIndex;
                _floatGestureArmed = _floatTabIndex >= 0;
            }
            else if (_touchStart.Count == 2)
            {
                // Two fingers down — start pinch tracking and 2-finger scroll
                _longPressTimer.Stop();
                _floatGestureArmed    = false;
                _pinchActive          = true;
                _pinchStartDist       = TouchDistance();
                _twoFingerScrollArmed = true;
                _twoFingerStartY      = AverageTouchY();
            }
            else if (_touchStart.Count == 3)
            {
                // Three fingers down — arm workspace swipe
                _longPressTimer.Stop();
                _floatGestureArmed    = false;
                _twoFingerScrollArmed = false;
                _threeFingerArmed     = true;
                _threeFingerStartX    = AverageTouchX();
            }
        }

        private void OnTouchMove(int id, PointF pt)
        {
            if (!_touchCurrent.ContainsKey(id)) return;

            PointF prev = _touchCurrent[id];
            _touchCurrent[id] = pt;

            if (_touchStart.Count == 1)
            {
                float dx = pt.X - prev.X;
                float dy = pt.Y - prev.Y;

                // Horizontal swipe → scroll
                if (Math.Abs(dx) > Math.Abs(dy))
                {
                    _longPressTimer.Stop();
                    _floatGestureArmed = false;

                    int newOffset = _scrollOffset - (int)dx;
                    ScrollOffset = Math.Max(0, newOffset);
                }
                else if (dy > SwipeDownThreshold && _floatGestureArmed && _floatTabIndex >= 0)
                {
                    // Swipe-down gesture → float
                    _floatGestureArmed = false;
                    _longPressTimer.Stop();
                    TriggerFloatGesture(_floatTabIndex);
                }
                else if (dy < SwipeUpThreshold && _floatGestureArmed && _floatTabIndex >= 0)
                {
                    // Swipe-up gesture → also float (tear out upward)
                    _floatGestureArmed = false;
                    _longPressTimer.Stop();
                    TriggerFloatGesture(_floatTabIndex);
                }
                else if (Math.Abs(dx) > 6 || Math.Abs(dy) > 6)
                {
                    // Any movement beyond tap threshold cancels long-press
                    _longPressTimer.Stop();
                }
            }
            else if (_touchStart.Count == 2)
            {
                if (_pinchActive)
                {
                    float newDist = TouchDistance();
                    float ratio   = newDist / Math.Max(1f, _pinchStartDist);

                    if (ratio < 0.75f)
                    {
                        // Pinch in → next (denser) size mode
                        _pinchStartDist = newDist;
                        CycleSizeMode(forward: true);
                    }
                    else if (ratio > 1.35f)
                    {
                        // Expand → less dense
                        _pinchStartDist = newDist;
                        CycleSizeMode(forward: false);
                    }
                }

                if (_twoFingerScrollArmed)
                {
                    // Two-finger vertical scroll — notify host via event
                    float currentAvgY = AverageTouchY();
                    float deltaY = currentAvgY - _twoFingerStartY;
                    if (Math.Abs(deltaY) > 5)
                    {
                        _twoFingerStartY = currentAvgY;
                        // Scroll the tab strip itself (horizontal scroll of tab row)
                        ScrollOffset = Math.Max(0, _scrollOffset - (int)(deltaY * 0.5f));
                    }
                }
            }
            else if (_touchStart.Count >= 3 && _threeFingerArmed)
            {
                float currentAvgX = AverageTouchX();
                float delta = currentAvgX - _threeFingerStartX;

                if (delta > ThreeFingerSwipeThreshold)
                {
                    _threeFingerArmed = false;
                    TouchWorkspaceSwitchRequested?.Invoke(this, +1);
                }
                else if (delta < -ThreeFingerSwipeThreshold)
                {
                    _threeFingerArmed = false;
                    TouchWorkspaceSwitchRequested?.Invoke(this, -1);
                }
            }
        }

        private void OnTouchUp(int id, PointF pt)
        {
            // Single-finger tap → check for double-tap
            if (_touchStart.Count == 1 && _touchStart.ContainsKey(id))
            {
                int hitIdx = HitTestTabIndex(Point.Round(pt));
                long now   = Environment.TickCount64;

                if (hitIdx >= 0 && hitIdx == _lastTapIndex &&
                    (now - _lastTapTick) <= DoubleTapMs)
                {
                    // Double-tap: request split maximize/restore
                    _lastTapIndex = -1;
                    _lastTapTick  = 0;
                    if (hitIdx < _tabs.Count)
                        TouchSplitMaximizeRequested?.Invoke(this,
                            new TabEventArgs(hitIdx, _tabs[hitIdx]));
                }
                else
                {
                    _lastTapIndex = hitIdx;
                    _lastTapTick  = now;
                }
            }

            _touchStart.Remove(id);
            _touchCurrent.Remove(id);

            if (_touchStart.Count == 0)
            {
                _longPressTimer.Stop();
                _pinchActive          = false;
                _floatGestureArmed    = false;
                _floatTabIndex        = -1;
                _twoFingerScrollArmed = false;
                _threeFingerArmed     = false;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Long-press → context menu
        // ─────────────────────────────────────────────────────────────────────

        private void OnLongPressTimerTick(object? sender, EventArgs e)
        {
            _longPressTimer.Stop();
            _floatGestureArmed = false;

            if (_longPressTabIndex >= 0 && _longPressTabIndex < _tabs.Count)
            {
                // Simulate right-click at the touch position to open the tab context menu
                var args = new MouseEventArgs(MouseButtons.Right, 1,
                    _longPressPt.X, _longPressPt.Y, 0);
                OnMouseDown(args);
                OnMouseUp(args);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Pinch → cycle TabSizeMode
        // ─────────────────────────────────────────────────────────────────────

        private void CycleSizeMode(bool forward)
        {
            // Cycle: Equal → Compact → Fixed → Equal  (or reverse)
            TabSizeMode[] modes = { TabSizeMode.Equal, TabSizeMode.Compact, TabSizeMode.Fixed };
            int idx = Array.IndexOf(modes, _tabSizeMode);
            if (idx < 0) idx = 0;

            idx = forward
                ? (idx + 1) % modes.Length
                : (idx - 1 + modes.Length) % modes.Length;

            TabSizeMode = modes[idx];
        }

        // ─────────────────────────────────────────────────────────────────────
        // Swipe-down → float gesture
        // ─────────────────────────────────────────────────────────────────────

        private void TriggerFloatGesture(int tabIndex)
        {
            if (tabIndex < 0 || tabIndex >= _tabs.Count) return;
            var tab = _tabs[tabIndex];
            TabFloatRequested?.Invoke(this, new TabEventArgs(tabIndex, tab));
        }

        // ─────────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Returns the Euclidean distance between the two active touch points.</summary>
        private float TouchDistance()
        {
            if (_touchCurrent.Count < 2) return 0f;
            var pts = new List<PointF>(_touchCurrent.Values);
            float dx = pts[0].X - pts[1].X;
            float dy = pts[0].Y - pts[1].Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>Average X of all current touch points (used for 3-finger swipe).</summary>
        private float AverageTouchX()
        {
            if (_touchCurrent.Count == 0) return 0f;
            float sum = 0f;
            foreach (var v in _touchCurrent.Values) sum += v.X;
            return sum / _touchCurrent.Count;
        }

        /// <summary>Average Y of all current touch points (used for 2-finger scroll).</summary>
        private float AverageTouchY()
        {
            if (_touchCurrent.Count == 0) return 0f;
            float sum = 0f;
            foreach (var v in _touchCurrent.Values) sum += v.Y;
            return sum / _touchCurrent.Count;
        }

        /// <summary>
        /// Returns the zero-based index of the tab hit by the given client-coordinate point, or -1.
        /// Uses a minimum 44×44 logical-pixel touch target per WCAG 2.5.5 / iOS/Android HIG.
        /// </summary>
        private int HitTestTabIndex(Point pt)
        {
            const int MinHit = 44;
            for (int i = 0; i < _tabs.Count; i++)
            {
                var r = _tabs[i].TabRect;
                // Expand narrow/short rects to the 44px minimum touch target
                var expanded = r;
                if (expanded.Width  < MinHit) { int d = MinHit - expanded.Width;  expanded.Inflate(d / 2, 0); }
                if (expanded.Height < MinHit) { int d = MinHit - expanded.Height; expanded.Inflate(0, d / 2); }
                if (expanded.Contains(pt)) return i;
            }
            return -1;
        }
    }
}

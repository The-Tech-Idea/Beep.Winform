using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepiForm.WndProc.cs - Window Message Handling and P/Invoke
    /// </summary>
    public partial class BeepiForm
    {
        #region Windows Message Constants
        private const int WM_NCHITTEST = 0x84;
        private const int WM_NCCALCSIZE = 0x83;
        private const int WM_NCPAINT = 0x85;
        private const int WM_NCACTIVATE = 0x86;
        private const int WM_ENTERSIZEMOVE = 0x0231;
        private const int WM_EXITSIZEMOVE = 0x0232;
        private const int WM_GETMINMAXINFO = 0x0024;
        private const int WM_DPICHANGED = 0x02E0;
        #endregion

        #region Hit Test Constants
        private const int HTCLIENT = 1;
        private const int HTCAPTION = 2;
        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;
        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;
        #endregion

        #region Window Style Constants
        private const int WS_SIZEBOX = 0x00040000;
        private const int MONITOR_DEFAULTTONEAREST = 2;
        #endregion

        #region User32 Constants
        private const uint RDW_FRAME = 0x0400;
        private const uint RDW_INVALIDATE = 0x0001;
        private const uint RDW_UPDATENOW = 0x0100;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_FRAMECHANGED = 0x0020;
        #endregion

        #region Native Structures
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public int dwFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NCCALCSIZE_PARAMS
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public RECT[] rgrc;
            public IntPtr lppos;
        }
        #endregion

        #region P/Invoke Declarations
        [DllImport("user32.dll")]
        private static extern uint GetDpiForWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        private static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        #endregion

        #region CreateParams Override
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (!InDesignHost)
                {
                    // Always add WS_SIZEBOX so Windows accepts HT* sizing codes on borderless forms
                    cp.Style |= WS_SIZEBOX;
                }
                return cp;
            }
        }
        #endregion

        #region WndProc Override
        protected override void WndProc(ref Message m)
        {
            if (InDesignHost)
            {
                base.WndProc(ref m);
                return;
            }

            switch (m.Msg)
            {
                case WM_NCCALCSIZE:
                    HandleNcCalcSize(ref m);
                    break;

                case WM_NCACTIVATE:
                    HandleNcActivate(ref m);
                    break;

                case WM_NCPAINT:
                    HandleNcPaint(ref m);
                    break;

                case WM_DPICHANGED:
                    HandleDpiChanged(ref m);
                    break;

                case WM_ENTERSIZEMOVE:
                    _inMoveOrResize = true;
                    break;

                case WM_EXITSIZEMOVE:
                    _inMoveOrResize = false;
                    if (UseHelperInfrastructure && _regionHelper != null)
                        _regionHelper.EnsureRegion();
                    Invalidate();
                    break;

                case WM_GETMINMAXINFO:
                    AdjustMaximizedBounds(m.LParam);
                    break;

                case WM_NCHITTEST when !_inpopupmode:
                    HandleNcHitTest(ref m);
                    return;
            }

            base.WndProc(ref m);
        }
        #endregion

        #region WndProc Message Handlers
        private void HandleNcCalcSize(ref Message m)
        {
            // Reserve space for custom border in non-client area (caption is in client area)
            if (_drawCustomWindowBorder && m.WParam != IntPtr.Zero && WindowState != FormWindowState.Maximized)
            {
                NCCALCSIZE_PARAMS nccsp = (NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(NCCALCSIZE_PARAMS));

                int borderThickness = _borderThickness;

                // Shrink client rect to reserve space for border on all sides
                // This ensures a proper NC top band for the border painter and avoids gaps
                nccsp.rgrc[0].top += borderThickness;
                nccsp.rgrc[0].left += borderThickness;
                nccsp.rgrc[0].right -= borderThickness;
                nccsp.rgrc[0].bottom -= borderThickness;

                Marshal.StructureToPtr(nccsp, m.LParam, false);
                m.Result = IntPtr.Zero;
            }
        }

        private void HandleNcActivate(ref Message m)
        {
            // Prevent default title bar repaint by setting lParam to -1
            if (_drawCustomWindowBorder && WindowState != FormWindowState.Maximized)
            {
                m.LParam = new IntPtr(-1);
            }
        }

        private void HandleNcPaint(ref Message m)
        {
            // Paint custom border in non-client area (caption bar painted in client area)
            if (_drawCustomWindowBorder && WindowState != FormWindowState.Maximized)
            {
                PaintNonClientBorder();
                m.Result = IntPtr.Zero;
            }
        }

        private void HandleDpiChanged(ref Message m)
        {
            if (DpiMode == DpiHandlingMode.Manual)
            {
                var suggested = Marshal.PtrToStructure<RECT>(m.LParam);
                var suggestedBounds = Rectangle.FromLTRB(suggested.left, suggested.top, suggested.right, suggested.bottom);
                this.Bounds = suggestedBounds;
                uint dpi = GetDpiForWindow(this.Handle);
            }
        }

        private void HandleNcHitTest(ref Message m)
        {
            if (UseHelperInfrastructure && _hitTestHelper != null)
            {
                if (_hitTestHelper.HandleNcHitTest(ref m))
                    return;
            }
            else
            {
                // Legacy hit test logic
                Point pos = PointToClient(new Point(m.LParam.ToInt32()));
                int margin = _resizeMargin;

                // Corner hit tests
                if (pos.X <= margin && pos.Y <= margin)
                {
                    m.Result = (IntPtr)HTTOPLEFT;
                    return;
                }
                if (pos.X >= ClientSize.Width - margin && pos.Y <= margin)
                {
                    m.Result = (IntPtr)HTTOPRIGHT;
                    return;
                }
                if (pos.X <= margin && pos.Y >= ClientSize.Height - margin)
                {
                    m.Result = (IntPtr)HTBOTTOMLEFT;
                    return;
                }
                if (pos.X >= ClientSize.Width - margin && pos.Y >= ClientSize.Height - margin)
                {
                    m.Result = (IntPtr)HTBOTTOMRIGHT;
                    return;
                }

                // Edge hit tests
                if (pos.X <= margin)
                {
                    m.Result = (IntPtr)HTLEFT;
                    return;
                }
                if (pos.X >= ClientSize.Width - margin)
                {
                    m.Result = (IntPtr)HTRIGHT;
                    return;
                }
                if (pos.Y <= margin)
                {
                    m.Result = (IntPtr)HTTOP;
                    return;
                }
                if (pos.Y >= ClientSize.Height - margin)
                {
                    m.Result = (IntPtr)HTBOTTOM;
                    return;
                }

                // Client area
                if (IsOverChildControl(pos))
                {
                    m.Result = (IntPtr)HTCLIENT;
                    return;
                }

                m.Result = IsInDraggableArea(pos) ? (IntPtr)HTCAPTION : (IntPtr)HTCLIENT;
            }
        }
        #endregion

        #region Hit Test Helper Methods
        private bool IsOverChildControl(Point clientPos)
        {
            var child = GetChildAtPoint(clientPos, GetChildAtPointSkip.Invisible | GetChildAtPointSkip.Transparent);
            return child != null;
        }

        private bool IsInDraggableArea(Point clientPos)
        {
            if (UseHelperInfrastructure && _captionHelper != null && _captionHelper.ShowCaptionBar)
            {
                if (clientPos.Y <= _captionHelper.CaptionHeight &&
                    !_captionHelper.IsPointInSystemButtons(clientPos) &&
                    !IsOverChildControl(clientPos))
                    return true;
                return false;
            }

            return clientPos.Y <= 36 && !IsOverChildControl(clientPos);
        }
        #endregion

        #region Maximize Bounds Adjustment
        private void AdjustMaximizedBounds(IntPtr lParam)
        {
            MINMAXINFO mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);
            IntPtr monitor = MonitorFromWindow(this.Handle, MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {
                MONITORINFO monitorInfo = new MONITORINFO();
                monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
                GetMonitorInfo(monitor, ref monitorInfo);

                Rectangle rcWorkArea = Rectangle.FromLTRB(
                    monitorInfo.rcWork.left,
                    monitorInfo.rcWork.top,
                    monitorInfo.rcWork.right,
                    monitorInfo.rcWork.bottom);

                Rectangle rcMonitorArea = Rectangle.FromLTRB(
                    monitorInfo.rcMonitor.left,
                    monitorInfo.rcMonitor.top,
                    monitorInfo.rcMonitor.right,
                    monitorInfo.rcMonitor.bottom);

                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
                mmi.ptMaxSize.x = rcWorkArea.Width;
                mmi.ptMaxSize.y = rcWorkArea.Height;

                Marshal.StructureToPtr(mmi, lParam, true);
            }
        }
        #endregion

        #region Mouse Event Handlers
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_inpopupmode || InDesignHost) return;
            base.OnMouseMove(e);
            foreach (var h in _mouseMoveHandlers) h(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (InDesignHost) return;
            Cursor = Cursors.Default;
            foreach (var h in _mouseLeaveHandlers) h();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (InDesignHost) return;
            foreach (var h in _mouseDownHandlers) h(e);
        }
        #endregion
    }
}

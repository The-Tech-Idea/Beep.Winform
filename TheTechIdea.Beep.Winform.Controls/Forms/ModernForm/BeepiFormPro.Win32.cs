using System;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public partial class BeepiFormPro
    {
      
        private bool _drawCustomWindowBorder = true;
        [Category("Beep Window")]
        [DefaultValue(true)]
        [Description("Enables custom skinned form borders and title bar (DevExpress style). Uses proper AutoScale-compatible implementation.")]
        public bool DrawCustomWindowBorder
        {
            get => _drawCustomWindowBorder;
            set
            {
                if (_drawCustomWindowBorder == value) return;
                _drawCustomWindowBorder = value;
                if (IsHandleCreated)
                {
                    // Force non-client recalc and repaint
                    SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, 0, 0,
                        SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
                    RedrawWindow(this.Handle, IntPtr.Zero, IntPtr.Zero, RDW_FRAME | RDW_INVALIDATE | RDW_UPDATENOW);
                }
            }
        }

        // Thickness of the custom border in device-independent pixels
        private int _customBorderThickness = 1;
        [Category("Beep Window")]
        [DefaultValue(1)]
        [Description("Thickness (in DIP) of the custom non-client border when DrawCustomWindowBorder is true.")]
        public int CustomBorderThickness
        {
            get => _customBorderThickness;
            set { _customBorderThickness = Math.Max(0, value); if (IsHandleCreated && _drawCustomWindowBorder) Invalidate(); }
        }

        // Hit Test Constants
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

        // Window message constants
    private const int WM_NCHITTEST = 0x84;
    private const int WM_NCLBUTTONDBLCLK = 0xA3;
        private const int WM_NCCALCSIZE = 0x83;
        private const int WM_NCPAINT = 0x85;
        private const int WM_NCACTIVATE = 0x86;
    private const int WM_ERASEBKGND = 0x0014;
        private const int WM_PRINTCLIENT = 0x0318;
    private const int WM_CTLCOLORDLG = 0x0136;
    private const int WM_CTLCOLORSTATIC = 0x0138;

    // Window style constants
    private const int WS_SIZEBOX = 0x00040000;
    private const int WS_CLIPCHILDREN = 0x02000000;
    private const int WS_CLIPSIBLINGS = 0x04000000;
    private const int WS_EX_COMPOSITED = 0x02000000; // Extended style: reduces flicker/overdraw

    // Legacy default; actual margin computed per-hit from FormPainterMetrics and DPI
    private int _resizeMarginWin32 = 8;

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                // Ensure borderless form can still be resized via HT* codes
                // and do not redraw over child controls (prevents black boxes/flicker)
                cp.Style |= WS_SIZEBOX | WS_CLIPCHILDREN | WS_CLIPSIBLINGS;
                // Use composited painting for child controls to avoid tearing/black regions at runtime
                cp.ExStyle |= WS_EX_COMPOSITED;
                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (_drawCustomWindowBorder)
            {
                switch (m.Msg)
                {
                    case WM_CTLCOLORDLG:
                    case WM_CTLCOLORSTATIC:
                        // Provide transparent background only for controls that actually request it
                        // (e.g., Label with BackColor = Transparent). Otherwise, let default processing
                        // return a proper brush for opaque backgrounds.
                        try
                        {
                            var childHwnd = m.LParam;
                            if (childHwnd != IntPtr.Zero)
                            {
                                var child = Control.FromHandle(childHwnd);
                                if (child != null)
                                {
                                    var back = child.BackColor;
                                    bool wantsTransparent = back == Color.Transparent || back.A < 255;
                                    if (wantsTransparent)
                                    {
                                        if (m.WParam != IntPtr.Zero)
                                            SetBkMode(m.WParam, 1 /* TRANSPARENT */);
                                        m.Result = GetStockObject(5 /* NULL_BRUSH */);
                                        return;
                                    }
                                }
                            }
                        }
                        catch { /* ignore and fall through to default */ }
                        break;
                    case WM_ERASEBKGND:
                        // CRITICAL: When we handle all painting in OnPaintBackground, 
                        // we must tell Windows NOT to erase the background.
                        // Return 1 to indicate we handled the erase.
                        // This prevents black boxes on child controls (Labels, etc.)
                        m.Result = (IntPtr)1;
                        return;
                    case WM_PRINTCLIENT:
                        // Provide a simple solid background to the requester. Avoid painting full chrome
                        // into the child DC, which can cause artifacts. This is sufficient for transparent
                        // labels and similar controls.
                        if (m.WParam != IntPtr.Zero)
                        {
                            using var g = Graphics.FromHdc(m.WParam);
                            using var bg = new SolidBrush(this.BackColor);
                            g.FillRectangle(bg, this.ClientRectangle);
                            m.Result = IntPtr.Zero;
                            return;
                        }
                        break;

                    case WM_NCCALCSIZE:
                        // CRITICAL FOR SKINNING: Reserve space for custom chrome without breaking AutoScale
                        // DevExpress approach: Extend client area to cover entire window
                        if (m.WParam != IntPtr.Zero)
                        {
                            var nccsp = Marshal.PtrToStructure<NCCALCSIZE_PARAMS>(m.LParam);

                            // In maximized state, adjust for screen edges to prevent overlap
                            if (WindowState == FormWindowState.Maximized)
                            {
                                // Small DPI-aware adjustment to prevent drawing over screen edges
                                // DevExpress keeps this minimal (1-2 px) to avoid visual gaps
                                int edgeMargin = Math.Max(1, GetEffectiveBorderThicknessDpi());
                                nccsp.rgrc0.top += edgeMargin;
                                nccsp.rgrc0.left += edgeMargin;
                                nccsp.rgrc0.right -= edgeMargin;
                                nccsp.rgrc0.bottom -= edgeMargin;
                            }
                            else
                            {
                                // Normal state: Extend client area to entire window (0 border reservation)
                                // This is KEY for skinning - we paint custom chrome IN the client area
                                // DisplayRectangle override will adjust control layout to account for caption
                                // This preserves AutoScale integrity because client rect stays consistent
                            }

                            Marshal.StructureToPtr(nccsp, m.LParam, false);
                            m.Result = IntPtr.Zero;
                            return;
                        }
                        break;
                    case WM_NCACTIVATE:
                        // Prevent default title bar repaint to avoid flicker.
                        // DevExpress pattern: set lParam to -1 and return 1 to indicate handled.
                        m.LParam = new IntPtr(-1);
                        m.Result = (IntPtr)1;
                        return;
                    case WM_NCPAINT:
                        // CRITICAL: Always paint non-client border to cover rectangular corners
                        // that Windows draws by default. Without this, rounded forms show
                        // rectangular corners behind the rounded shape.
                        PaintNonClientBorder();
                        m.Result = IntPtr.Zero;
                        return;
                }
            }

            if (m.Msg == WM_NCHITTEST)
            {
                // Convert screen point to client coordinates
                Point pos = PointToClient(new Point(m.LParam.ToInt32()));

                // 1) Edge/corner resize zones take precedence
                int margin = GetEffectiveResizeMarginDpi();
                if (pos.X <= margin && pos.Y <= margin) { m.Result = (IntPtr)HTTOPLEFT; return; }
                if (pos.X >= ClientSize.Width - margin && pos.Y <= margin) { m.Result = (IntPtr)HTTOPRIGHT; return; }
                if (pos.X <= margin && pos.Y >= ClientSize.Height - margin) { m.Result = (IntPtr)HTBOTTOMLEFT; return; }
                if (pos.X >= ClientSize.Width - margin && pos.Y >= ClientSize.Height - margin) { m.Result = (IntPtr)HTBOTTOMRIGHT; return; }
                if (pos.X <= margin) { m.Result = (IntPtr)HTLEFT; return; }
                if (pos.X >= ClientSize.Width - margin) { m.Result = (IntPtr)HTRIGHT; return; }
                if (pos.Y <= margin) { m.Result = (IntPtr)HTTOP; return; }
                if (pos.Y >= ClientSize.Height - margin) { m.Result = (IntPtr)HTBOTTOM; return; }

                // 2) If over a child control, don't treat as caption/content drag
                if (IsOverChildControl(pos)) { m.Result = (IntPtr)HTCLIENT; return; }

                // 3) Treat caption rect as native caption for drag if shown
                if (ShowCaptionBar && CurrentLayout.CaptionRect.Contains(pos))
                {
                    // Exclude clicks over system/style/theme/custom buttons
                    if (!IsPointInCaptionButtons(pos)) { m.Result = (IntPtr)HTCAPTION; return; }
                }

                // 4) Default client area
                m.Result = (IntPtr)HTCLIENT; return;
            }

            // Double-click on caption toggles maximize/restore
            if (m.Msg == WM_NCLBUTTONDBLCLK)
            {
                // Only toggle when we actually double-clicked the caption region (per our HTCAPTION returns)
                if (WindowState == FormWindowState.Maximized)
                    WindowState = FormWindowState.Normal;
                else if (WindowState == FormWindowState.Normal)
                    WindowState = FormWindowState.Maximized;
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }

        private void PaintNonClientBorder()
        {
            if (!IsHandleCreated || !_drawCustomWindowBorder) return;
            int bt = GetEffectiveBorderThicknessDpi();
            if (bt == 0) return;

            IntPtr hdc = GetWindowDC(this.Handle);
            if (hdc == IntPtr.Zero) return;
            try
            {
                using var g = Graphics.FromHdc(hdc);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                
                // CRITICAL: Paint the entire form background with the correct shape
                // to cover Windows' rectangular default NC area that shows behind rounded corners
                var painter = ActivePainter;
                if (painter != null)
                {
                    // Get the form's actual shape (rounded corners, etc.)
                    var radius = painter.GetCornerRadius(this);
                    var formRect = new Rectangle(0, 0, Width, Height);
                    
                    // Fill the entire form with the background color using the correct shape
                    using (var bgBrush = new SolidBrush(BackColor))
                    {
                        using (var shapePath = CreateRoundedRectanglePath(formRect, radius))
                        {
                            g.FillPath(bgBrush, shapePath);
                        }
                    }
                    
                    // Now draw the border using painter's method (if available)
                    if (painter is TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters.IFormNonClientPainter ncPainter)
                    {
                        ncPainter.PaintNonClientBorder(g, this, bt);
                    }
                    else
                    {
                        // Fallback: draw border using the form's shape
                        var metrics = FormPainterMetrics.DefaultFor(FormStyle, UseThemeColors ? _currentTheme : null);
                        using (var borderPen = new Pen(metrics.BorderColor, bt))
                        {
                            using (var shapePath = CreateRoundedRectanglePath(formRect, radius))
                            {
                                g.DrawPath(borderPen, shapePath);
                            }
                        }
                    }
                }
                else
                {
                    // Fallback: simple rectangle border (no painter available)
                    using var br = new SolidBrush(BorderColor);
                    g.FillRectangle(br, new Rectangle(0, 0, Width, bt));
                    g.FillRectangle(br, new Rectangle(0, Height - bt, Width, bt));
                    g.FillRectangle(br, new Rectangle(0, 0, bt, Height));
                    g.FillRectangle(br, new Rectangle(Width - bt, 0, bt, Height));
                }
            }
            finally
            {
                ReleaseDC(this.Handle, hdc);
            }
        }

        /// <summary>
        /// Updates the window region to match the form's shape (rounded corners, etc.)
        /// This clips the window so rectangular corners don't show through
        /// CRITICAL: This must be called whenever the form size changes or style changes
        /// </summary>
        private void UpdateWindowRegion()
        {
            if (!IsHandleCreated || !_drawCustomWindowBorder)
            {
                // Clear region if custom drawing is disabled
                if (IsHandleCreated)
                {
                    SetWindowRgn(this.Handle, IntPtr.Zero, true);
                }
                return;
            }
            
            // When maximized, clear the region to avoid edge clipping issues
            if (WindowState == FormWindowState.Maximized)
            {
                SetWindowRgn(this.Handle, IntPtr.Zero, true);
                return;
            }
            
            var painter = ActivePainter;
            if (painter == null)
            {
                // No painter - use rectangular region
                IntPtr hRgn = CreateRectRgn(0, 0, Width, Height);
                if (hRgn != IntPtr.Zero)
                {
                    SetWindowRgn(this.Handle, hRgn, true);
                }
                return;
            }
            
            var radius = painter.GetCornerRadius(this);
            
            // Check if all corners have the same radius (simple rounded rectangle)
            if (radius.TopLeft == radius.TopRight && 
                radius.TopLeft == radius.BottomLeft && 
                radius.TopLeft == radius.BottomRight &&
                radius.TopLeft > 0)
            {
                // Use fast CreateRoundRectRgn for uniform corners
                // IMPORTANT: CreateRoundRectRgn uses diameter, not radius
                // IMPORTANT: Add 1 to width/height to ensure full coverage
                int cornerDiameter = radius.TopLeft * 2;
                IntPtr hRgn = CreateRoundRectRgn(0, 0, Width + 1, Height + 1, cornerDiameter, cornerDiameter);
                
                if (hRgn != IntPtr.Zero)
                {
                    SetWindowRgn(this.Handle, hRgn, true);
                    // Note: SetWindowRgn takes ownership of the region, no need to DeleteObject
                }
            }
            else if (radius.TopLeft == 0 && radius.TopRight == 0 && 
                     radius.BottomLeft == 0 && radius.BottomRight == 0)
            {
                // No rounded corners - use rectangular region
                IntPtr hRgn = CreateRectRgn(0, 0, Width, Height);
                if (hRgn != IntPtr.Zero)
                {
                    SetWindowRgn(this.Handle, hRgn, true);
                }
            }
            else
            {
                // Complex corners with different radii - use GDI+ region from GraphicsPath
                using (var path = CreateRoundedRectanglePath(new Rectangle(0, 0, Width, Height), radius))
                {
                    using (var region = new System.Drawing.Region(path))
                    {
                        using (var g = Graphics.FromHwnd(this.Handle))
                        {
                            // Get the HRGN from the region
                            IntPtr hRgn = region.GetHrgn(g);
                            if (hRgn != IntPtr.Zero)
                            {
                                SetWindowRgn(this.Handle, hRgn, true);
                                // SetWindowRgn takes ownership, but we got it from GetHrgn, so we should delete it
                                DeleteObject(hRgn);
                            }
                        }
                    }
                }
            }
        }

        // P/Invoke and structs for non-client calculations
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int left; public int top; public int right; public int bottom; }

        [StructLayout(LayoutKind.Sequential)]
        private struct NCCALCSIZE_PARAMS
        {
            public RECT rgrc0;
            public RECT rgrc1;
            public RECT rgrc2;
            public IntPtr lppos;
        }

        [DllImport("user32.dll")] private static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("user32.dll")] private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32.dll")] private static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);
        [DllImport("user32.dll")] private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")] private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);
        [DllImport("gdi32.dll")] private static extern int SetBkMode(IntPtr hdc, int mode);
        [DllImport("gdi32.dll")] private static extern IntPtr GetStockObject(int fnObject);
        [DllImport("gdi32.dll")] private static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int cx, int cy);
        [DllImport("gdi32.dll")] private static extern IntPtr CreateRectRgn(int x1, int y1, int x2, int y2);
        [DllImport("gdi32.dll")] private static extern bool DeleteObject(IntPtr hObject);

        private const uint RDW_FRAME = 0x0400;
        private const uint RDW_INVALIDATE = 0x0001;
        private const uint RDW_UPDATENOW = 0x0100;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_FRAMECHANGED = 0x0020;

        private bool IsOverChildControl(Point clientPos)
        {
            var child = GetChildAtPoint(clientPos, GetChildAtPointSkip.Invisible | GetChildAtPointSkip.Transparent);
            return child != null;
        }

        private bool IsPointInCaptionButtons(Point p)
        {
            // Check known caption button rects from current layout
            Rectangle[] rects = new Rectangle[]
            {
                CurrentLayout.CloseButtonRect,
                CurrentLayout.MaximizeButtonRect,
                CurrentLayout.MinimizeButtonRect,
                CurrentLayout.ThemeButtonRect,
                CurrentLayout.StyleButtonRect,
                CurrentLayout.CustomActionButtonRect
            };
            foreach (var r in rects)
            {
                if (r.Width > 0 && r.Height > 0 && r.Contains(p)) return true;
            }
            return false;
        }

        // Helpers: effective metrics-driven values with safe fallbacks
        private int GetEffectiveResizeMarginDpi()
        {
            var m = FormPainterMetrics.DefaultFor(FormStyle, UseThemeColors ? _currentTheme : null);
            int raw = m?.ResizeBorderWidth ?? 0;
            if (raw <= 0) raw = m?.BorderWidth ?? 0; // fall back to border width
            if (raw <= 0) raw = _resizeMarginWin32;   // legacy default
            raw = Math.Max(2, raw);                  // enforce minimum sensible size
            return raw;
        }

        private int GetEffectiveBorderThicknessDpi()
        {
            var m = FormPainterMetrics.DefaultFor(FormStyle, UseThemeColors ? _currentTheme : null);
            int raw = m?.BorderWidth ?? 0;
            if (raw <= 0) raw = _customBorderThickness; // fallback to property if set
            if (raw <= 0) raw = 1;                      // final fallback
            raw = Math.Max(1, raw);
            return raw;
        }
    }
}

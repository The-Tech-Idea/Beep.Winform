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
        [Description("Enables custom skinned form borders and title bar (DevExpress Style). Uses proper AutoScale-compatible implementation.")]
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
        private const int WM_MOVE = 0x0003;
        private const int WM_NCMOVE = 0x00A0;
        private const int WM_EXITSIZEMOVE = 0x0232;
        private const int WM_SETFOCUS = 0x0007;
        private const int WM_KILLFOCUS = 0x0008;
        private const int WM_CHILDACTIVATE = 0x0022;

    // Window Style constants
    private const int WS_SIZEBOX = 0x00040000;
    private const int WS_CLIPCHILDREN = 0x02000000;
    private const int WS_CLIPSIBLINGS = 0x04000000;
    private const int WS_EX_COMPOSITED = 0x02000000; // Extended Style: reduces flicker/overdraw

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
             //   cp.ExStyle |= WS_EX_COMPOSITED;
                return cp;
            }
        }

        // WM_SHOWWINDOW constant
        private const int WM_SHOWWINDOW = 0x0018;
        
        protected override void WndProc(ref Message m)
        {
            // CRITICAL: In design mode, behave EXACTLY like a normal WinForm
            // Let Windows handle ALL messages normally for maximum designer compatibility
            if (InDesignModeSafe)
            {
                base.WndProc(ref m);
                return;
            }

            // CRITICAL: Protect against messages arriving when form is disposed or not ready
            if (IsDisposed || !IsHandleCreated)
            {
                try { base.WndProc(ref m); } catch { }
                return;
            }

            // CRITICAL: For WM_SHOWWINDOW, ensure form has valid dimensions before processing
            // This prevents ArgumentException when showing context menus with zero size
            if (m.Msg == WM_SHOWWINDOW)
            {
                try
                {
                    // Ensure form has valid size before showing
                    if (Width <= 0 || Height <= 0)
                    {
                        // Set minimum size to prevent graphics errors
                        if (Width <= 0) Width = 1;
                        if (Height <= 0) Height = 1;
                    }
                    base.WndProc(ref m);
                }
                catch (ArgumentException)
                {
                    // Silently ignore - form may not be ready yet
                }
                catch (ObjectDisposedException)
                {
                    // Form was disposed during show
                }
                return;
            }

            // RUNTIME: Custom window handling for skinned appearance
            if (_drawCustomWindowBorder)
            {
                switch (m.Msg)
                {
                    case WM_CTLCOLORDLG:
                    case WM_CTLCOLORSTATIC:
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
                        catch { }
                        break;
                    case WM_ERASEBKGND:
                        // In runtime, suppress erase to prevent flicker (we paint everything in OnPaintBackground)
                        m.Result = (IntPtr)1;
                        return;

                    // WM_LBUTTONDOWN and WM_LBUTTONUP removed to use standard OnMouseDown/OnMouseUp events
                    // This allows WinForms to handle capture and focus correctly


                    case WM_SETFOCUS:
                    case WM_KILLFOCUS:
                    case WM_CHILDACTIVATE:
                        // CRITICAL: In design mode, when focus changes (control selection),
                        // we need to handle this carefully to avoid blank screen during transitions
                        if (InDesignModeSafe)
                        {
                            base.WndProc(ref m);
                            // Use BeginInvoke to defer invalidation until after all focus events complete
                            // This prevents the form from going blank during control-to-control transitions
                            if (!IsDisposed && IsHandleCreated)
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    if (!IsDisposed && IsHandleCreated)
                                    {
                                        this.Invalidate();
                                        this.Update();
                                    }
                                }));
                            }
                            return;
                        }
                        break;
                    
                    case WM_PRINTCLIENT:
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
                        if (m.WParam != IntPtr.Zero)
                        {
                            var nccsp = Marshal.PtrToStructure<NCCALCSIZE_PARAMS>(m.LParam);
                            int edgeMargin = Math.Max(1, GetEffectiveBorderThicknessDpi());
                            if (WindowState == FormWindowState.Maximized)
                            {
                                // For maximized, adjust for taskbar as well
                                nccsp.rgrc0.top += edgeMargin;
                                nccsp.rgrc0.left += edgeMargin;
                                nccsp.rgrc0.right -= edgeMargin;
                                nccsp.rgrc0.bottom -= edgeMargin;
                            }
                            else
                            {
                                // For normal state, expand client area to cover standard non-client regions
                                nccsp.rgrc0.top += edgeMargin;
                                nccsp.rgrc0.left += edgeMargin;
                                nccsp.rgrc0.right -= edgeMargin;
                                nccsp.rgrc0.bottom -= edgeMargin;
                            }
                            Marshal.StructureToPtr(nccsp, m.LParam, false);
                            m.Result = IntPtr.Zero;
                            return;
                        }
                        break;
                    case WM_NCACTIVATE:
                        m.LParam = new IntPtr(-1);
                        m.Result = (IntPtr)1;
                        return;
                    case WM_NCPAINT:
                        m.Result = IntPtr.Zero;
                        PaintNonClientBorder();
                       
                        return;
                    case WM_NCHITTEST:
                        {
                            // Extract the screen point from lParam
                            long lparam = m.LParam.ToInt64();
                            int x = (short)(lparam & 0xffff);
                            int y = (short)((lparam >> 16) & 0xffff);
                            Point pos = PointToClient(new Point(x, y));

                            // CRITICAL: Ensure layout is calculated before checking button positions
                            // This ensures CurrentLayout has correct button rects even if properties changed
                            // since the last paint. Wrap in try-catch to handle early WM_NCHITTEST before form is ready.
                            try
                            {
                                EnsureLayoutCalculated();
                            }
                            catch
                            {
                                // Form not fully initialized yet, use default hit test
                                m.Result = (IntPtr)HTCLIENT;
                                return;
                            }

                            // Check if the click is within the custom caption bar
                            if (ShowCaptionBar && CurrentLayout != null && CurrentLayout.CaptionRect.Contains(pos))
                            {
                                // CRITICAL: Release mouse capture when cursor is over the caption so that
                                // the form receives WM_LBUTTONDOWN/WM_LBUTTONUP. After interacting with
                                // child controls (e.g. TextBox, Button), a child may still hold capture;
                                // without this, clicks on the close button would go to the child and the
                                // caption would appear unresponsive.
                                ReleaseCapture();

                                if (IsPointInCaptionButtons(pos))
                                {
                                    // CRITICAL: Point is on a caption button (close, minimize, maximize, etc.)
                                    // Return HTCLIENT immediately so WinForms fires OnMouseDown/OnMouseUp
                                    // Do NOT fall through to resize border checks - they would steal the
                                    // click (e.g. HTTOPRIGHT for close button in the top-right corner)
                                    m.Result = (IntPtr)HTCLIENT;
                                    return;
                                }

                                // Not on a button - treat as draggable caption
                                m.Result = (IntPtr)HTCAPTION;
                                return;
                            }

                            // Handle resize borders
                            int margin = GetEffectiveResizeMarginDpi();
                            if (pos.X <= margin && pos.Y <= margin) { m.Result = (IntPtr)HTTOPLEFT; return; }
                            if (pos.X >= ClientSize.Width - margin && pos.Y <= margin) { m.Result = (IntPtr)HTTOPRIGHT; return; }
                            if (pos.X <= margin && pos.Y >= ClientSize.Height - margin) { m.Result = (IntPtr)HTBOTTOMLEFT; return; }
                            if (pos.X >= ClientSize.Width - margin && pos.Y >= ClientSize.Height - margin) { m.Result = (IntPtr)HTBOTTOMRIGHT; return; }
                            if (pos.X <= margin) { m.Result = (IntPtr)HTLEFT; return; }
                            if (pos.X >= ClientSize.Width - margin) { m.Result = (IntPtr)HTRIGHT; return; }
                            if (pos.Y <= margin) { m.Result = (IntPtr)HTTOP; return; }
                            if (pos.Y >= ClientSize.Height - margin) { m.Result = (IntPtr)HTBOTTOM; return; }

                            // Default to client area
                            m.Result = (IntPtr)HTCLIENT;
                            return;
                        }
                    case WM_NCLBUTTONDBLCLK:
                        if (m.WParam != IntPtr.Zero && (int)m.WParam == HTCAPTION)
                        {
                            if (WindowState == FormWindowState.Maximized)
                                WindowState = FormWindowState.Normal;
                            else if (WindowState == FormWindowState.Normal)
                                WindowState = FormWindowState.Maximized;
                        }
                        m.Result = IntPtr.Zero;
                        return;
                    case WM_MOVE:
                    case WM_NCMOVE:
                        // Let base handle the move - don't repaint during dragging to avoid flicker
                        base.WndProc(ref m);
                        // Just mark that we need to repaint when movement stops
                        return;
                    case WM_EXITSIZEMOVE:
                        // Repaint only when movement/resizing is complete to avoid flicker
                        base.WndProc(ref m);
                        if (_drawCustomWindowBorder && IsHandleCreated)
                        {
                            // Mark layout as dirty - will be recalculated on next paint or hit test
                            InvalidateLayout();
                            
                            // Update window region for new size
                            UpdateWindowRegion();
                            
                            // Invalidate the entire form (client + non-client) to repaint everything
                            Invalidate(true);
                            
                            // Also repaint non-client area (title bar/borders)
                            RedrawWindow(this.Handle, IntPtr.Zero, IntPtr.Zero, RDW_FRAME | RDW_INVALIDATE | RDW_UPDATENOW);
                        }
                        return;
                        
                    default:
                        try
                        {
                            base.WndProc(ref m);
                        }
                        catch (ArgumentException)
                        {
                            // Ignore ArgumentException that can occur during form initialization
                            // or when the form has invalid dimensions
                        }
                        catch (ObjectDisposedException)
                        {
                            // Ignore if form was disposed during message processing
                        }
                        break;
                }
            }
            else
            {
                // When custom border is disabled, use default WndProc
                try
                {
                    base.WndProc(ref m);
                }
                catch (ArgumentException)
                {
                    // Ignore ArgumentException that can occur during form initialization
                }
                catch (ObjectDisposedException)
                {
                    // Ignore if form was disposed during message processing
                }
            }
        }

        private void PaintNonClientBorder()
        {
            if (!IsHandleCreated || !_drawCustomWindowBorder) return;
            if (ActivePainter == null)
            {
                ApplyFormStyle();
            }
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
                        var metrics = FormPainterMetrics.DefaultForCached(FormStyle, UseThemeColors ? _currentTheme : null);
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
        /// CRITICAL: This must be called whenever the form size changes or Style changes
        /// </summary>
        private void UpdateWindowRegion()
        {
           
            if (ActivePainter == null)
            {
                ApplyFormStyle();
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

        [DllImport("dwmapi.dll")] private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);
        [DllImport("dwmapi.dll")] private static extern int DwmSetWindowAttribute(IntPtr hwnd, uint dwAttribute, ref uint pvAttribute, uint cbAttribute);

        [StructLayout(LayoutKind.Sequential)]
        private struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cxTopHeight;
            public int cxBottomHeight;
        }

        private const uint RDW_FRAME = 0x0400;
        private const uint RDW_INVALIDATE = 0x0001;
        private const uint RDW_UPDATENOW = 0x0100;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_FRAMECHANGED = 0x0020;

        private const uint DWMWA_NCRENDERING_POLICY = 2;
        private const uint DWMNCRP_DISABLED = 1;

        // Windows 10/11 backdrop effects (Acrylic, Mica)
        [DllImport("user32.dll")] private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        [StructLayout(LayoutKind.Sequential)]
        private struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        private enum WindowCompositionAttribute
        {
            WCA_ACCENT_POLICY = 19
        }

        private struct AccentPolicy
        {
            public AccentState AccentState;
            public uint AccentFlags;
            public uint GradientColor;
            public uint AnimationId;
        }

        private enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
            ACCENT_INVALID_STATE = 5
        }

        // For Mica (Windows 11)
        private const uint DWMWA_SYSTEMBACKDROP_TYPE = 38;
        private const uint DWMSBT_NONE = 0;
        private const uint DWMSBT_MAINWINDOW = 1; // Mica
        private const uint DWMSBT_TRANSIENTWINDOW = 2; // Acrylic
        private const uint DWMSBT_TABBEDWINDOW = 3; // Tabbed

       

     

        private bool IsOverChildControl(Point clientPos)
        {
            var child = GetChildAtPoint(clientPos, GetChildAtPointSkip.Invisible | GetChildAtPointSkip.Transparent);
            return child != null;
        }

        private bool IsPointInCaptionButtons(Point p)
        {
            // Respect runtime visibility flags so hidden buttons never block caption drag.
            if (ShowCloseButton && CurrentLayout.CloseButtonRect.Width > 0 && CurrentLayout.CloseButtonRect.Height > 0 && CurrentLayout.CloseButtonRect.Contains(p))
                return true;

            if (ShowMinMaxButtons)
            {
                if (CurrentLayout.MaximizeButtonRect.Width > 0 && CurrentLayout.MaximizeButtonRect.Height > 0 && CurrentLayout.MaximizeButtonRect.Contains(p))
                    return true;
                if (CurrentLayout.MinimizeButtonRect.Width > 0 && CurrentLayout.MinimizeButtonRect.Height > 0 && CurrentLayout.MinimizeButtonRect.Contains(p))
                    return true;
            }

            if (ShowThemeButton && CurrentLayout.ThemeButtonRect.Width > 0 && CurrentLayout.ThemeButtonRect.Height > 0 && CurrentLayout.ThemeButtonRect.Contains(p))
                return true;

            if (ShowStyleButton && CurrentLayout.StyleButtonRect.Width > 0 && CurrentLayout.StyleButtonRect.Height > 0 && CurrentLayout.StyleButtonRect.Contains(p))
                return true;

            if (ShowCustomActionButton && CurrentLayout.CustomActionButtonRect.Width > 0 && CurrentLayout.CustomActionButtonRect.Height > 0 && CurrentLayout.CustomActionButtonRect.Contains(p))
                return true;

            if (ShowSearchBox && CurrentLayout.SearchBoxRect.Width > 0 && CurrentLayout.SearchBoxRect.Height > 0 && CurrentLayout.SearchBoxRect.Contains(p))
                return true;

            if (ShowProfileButton && CurrentLayout.ProfileButtonRect.Width > 0 && CurrentLayout.ProfileButtonRect.Height > 0 && CurrentLayout.ProfileButtonRect.Contains(p))
                return true;

            return false;
        }

        // Helpers: effective metrics-driven values with safe fallbacks
        private int GetEffectiveResizeMarginDpi()
        {
            var m = FormPainterMetrics.DefaultForCached(FormStyle, UseThemeColors ? _currentTheme : null);
            int raw = m?.ResizeBorderWidth ?? 0;
            if (raw <= 0) raw = m?.BorderWidth ?? 0; // fall back to border width
            if (raw <= 0) raw = _resizeMarginWin32;   // legacy default
            raw = Math.Max(2, raw);                  // enforce minimum sensible size
            return raw;
        }

        private int GetEffectiveBorderThicknessDpi()
        {
            var m = FormPainterMetrics.DefaultForCached(FormStyle, UseThemeColors ? _currentTheme : null);
            int raw = m?.BorderWidth ?? 0;
            if (raw <= 0) raw = _customBorderThickness; // fallback to property if set
            if (raw <= 0) raw = 1;                      // final fallback
            raw = Math.Max(1, raw);
            return raw;
        }

        private void DisableDwmNonClientRendering()
        {
            if (Environment.OSVersion.Version.Major >= 6) // Vista+
            {
                uint policy = DWMNCRP_DISABLED;
                DwmSetWindowAttribute(this.Handle, DWMWA_NCRENDERING_POLICY, ref policy, sizeof(uint));
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            DpiScalingHelper.RefreshScaleFactors(this, ref _dpiScaleX, ref _dpiScaleY);

            DisableDwmNonClientRendering();
            
            ApplyBackdrop();
            
            // CRITICAL: Calculate hit areas when handle is created
            // This ensures buttons are clickable from the start
            if (!DesignMode)
            {
                RecalculateLayoutAndHitAreas();
            }
        }

    

    }
}

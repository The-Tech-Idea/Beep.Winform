using System;
using System.Runtime.InteropServices;
using System.Security;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Interop
{
    /// <summary>
    /// P/Invoke wrapper for native Win32 MDI API calls.
    /// Provides access to MDI-specific window operations and structures.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static class MdiNativeApi
    {
        #region Structs

        /// <summary>
        /// Window rectangle coordinates.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            /// <summary>Gets the width of the rectangle.</summary>
            public int Width => Right - Left;

            /// <summary>Gets the height of the rectangle.</summary>
            public int Height => Bottom - Top;

            /// <summary>Converts to System.Drawing.Rectangle.</summary>
            public System.Drawing.Rectangle ToRectangle() => 
                System.Drawing.Rectangle.FromLTRB(Left, Top, Right, Bottom);

            /// <summary>Converts from System.Drawing.Rectangle.</summary>
            public static RECT FromRectangle(System.Drawing.Rectangle rect) =>
                new RECT(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        /// <summary>
        /// Window placement information for GetWindowPlacement/SetWindowPlacement.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public uint length;
            public uint flags;
            public uint showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public RECT rcNormalPosition;
        }

        /// <summary>
        /// Point structure for x,y coordinates.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public System.Drawing.Point ToPoint() => new System.Drawing.Point(x, y);

            public static POINT FromPoint(System.Drawing.Point p) => new POINT(p.X, p.Y);
        }

        /// <summary>
        /// MDICREATESTRUCT passed to MDI child creation.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MDICREATESTRUCT
        {
            public IntPtr szClass;
            public IntPtr szTitle;
            public IntPtr hOwner;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public uint style;
            public IntPtr lParam;
        }

        #endregion

        #region P/Invoke Methods

        /// <summary>
        /// Creates a window with extended style options.
        /// Used to create the MDI client area (WC_MDICLIENT class).
        /// </summary>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateWindowEx(
            uint dwExStyle,
            string lpClassName,
            string lpWindowName,
            uint dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

        /// <summary>
        /// Destroys a window and releases its resources.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyWindow(IntPtr hWnd);

        /// <summary>
        /// Sends a message to a window.
        /// Used to send MDI-specific messages (WM_MDICREATE, WM_MDIDESTROY, etc.).
        /// </summary>
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(
            IntPtr hWnd,
            uint Msg,
            IntPtr wParam,
            IntPtr lParam);

        /// <summary>
        /// Sends a message to a window, with custom struct parameter.
        /// </summary>
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(
            IntPtr hWnd,
            uint Msg,
            IntPtr wParam,
            ref MDICREATESTRUCT lParam);

        /// <summary>
        /// Gets the handle of the active MDI child window.
        /// </summary>
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(
            IntPtr hWnd,
            uint Msg,
            IntPtr wParam,
            out IntPtr lParam);

        /// <summary>
        /// Sets the window size and position.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int x,
            int y,
            int cx,
            int cy,
            uint uFlags);

        /// <summary>
        /// Gets information about a window (size, position, style, etc.).
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        /// <summary>
        /// Gets the client area coordinates of a window.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        /// <summary>
        /// Shows or hides a window.
        /// </summary>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// Gets the window style and extended style flags.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        /// <summary>
        /// Sets window style and extended style flags.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        /// <summary>
        /// Enumerates child windows of a parent window.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(
            IntPtr hWndParent,
            EnumWindowsProc lpEnumFunc,
            IntPtr lParam);

        /// <summary>
        /// Delegate for EnumChildWindows callback.
        /// </summary>
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        /// <summary>
        /// Updates a window after changes to its size, position, or display state.
        /// </summary>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UpdateWindow(IntPtr hWnd);

        /// <summary>
        /// Invalidates a window's client area, forcing a repaint.
        /// </summary>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        /// <summary>
        /// Begins a group of window position updates (for batch operations).
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr BeginDeferWindowPos(int nNumWindows);

        /// <summary>
        /// Defers a SetWindowPos operation (used with BeginDeferWindowPos).
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr DeferWindowPos(
            IntPtr hWinPosInfo,
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int x,
            int y,
            int cx,
            int cy,
            uint uFlags);

        /// <summary>
        /// Completes a deferred window position operation (used with BeginDeferWindowPos).
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EndDeferWindowPos(IntPtr hWinPosInfo);

        /// <summary>
        /// Gets the window text (title).
        /// </summary>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

        /// <summary>
        /// Sets the window text (title).
        /// </summary>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowText(IntPtr hWnd, string lpString);

        /// <summary>
        /// Gets the window placement (size, position, state).
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        /// <summary>
        /// Sets the window placement (size, position, state).
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        /// <summary>
        /// Gets the window handle at the given point.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr WindowFromPoint(POINT Point);

        /// <summary>
        /// Gets the parent window of a window.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        /// <summary>
        /// Sets the window parent (reparent).
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        /// <summary>
        /// Validates a rectangle in a window's client area.
        /// </summary>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ValidateRect(IntPtr hWnd, IntPtr lpRect);

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the error message for the last Win32 error code.
        /// </summary>
        public static string GetLastErrorMessage()
        {
            int errorCode = Marshal.GetLastWin32Error();
            if (errorCode == 0) return "No error";
            return new System.ComponentModel.Win32Exception(errorCode).Message;
        }

        #endregion
    }
}

namespace TheTechIdea.Beep.Winform.Controls.Docking.Interop
{
    /// <summary>
    /// Win32 constants used for MDI operations and window management.
    /// </summary>
    internal static class MdiConstants
    {
        #region Window Messages

        /// <summary>Create an MDI child window.</summary>
        public const uint WM_MDICREATE = 0x0220;

        /// <summary>Destroy an MDI child window.</summary>
        public const uint WM_MDIDESTROY = 0x0221;

        /// <summary>Activate an MDI child window.</summary>
        public const uint WM_MDIACTIVATE = 0x0222;

        /// <summary>Restore an MDI child window from minimized or maximized state.</summary>
        public const uint WM_MDIRESTORE = 0x0223;

        /// <summary>Get the next MDI child window.</summary>
        public const uint WM_MDINEXT = 0x0224;

        /// <summary>Maximize an MDI child window.</summary>
        public const uint WM_MDIMAXIMIZE = 0x0225;

        /// <summary>Tile MDI child windows horizontally.</summary>
        public const uint WM_MDITILE = 0x0226;

        /// <summary>Cascade MDI child windows.</summary>
        public const uint WM_MDICASCADE = 0x0227;

        /// <summary>Arrange minimized MDI child windows.</summary>
        public const uint WM_MDIICONARRANGE = 0x0228;

        /// <summary>Get the maximized MDI child window.</summary>
        public const uint WM_MDIGETACTIVE = 0x0229;

        /// <summary>Minimize an MDI child window.</summary>
        public const uint WM_MDISETMENU = 0x0230;

        #endregion

        #region Window Styles

        /// <summary>Creates a child window. A window with this style cannot have a menu bar.</summary>
        public const uint WS_CHILD = 0x40000000;

        /// <summary>Excludes the area occupied by child windows when drawing occurs within the parent window.</summary>
        public const uint WS_CLIPCHILDREN = 0x02000000;

        /// <summary>Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message, the WS_CLIPCHILDREN style excludes the updated area from the region of the child window.</summary>
        public const uint WS_CLIPSIBLINGS = 0x04000000;

        /// <summary>Specifies a control.</summary>
        public const uint WS_DISABLED = 0x08000000;

        /// <summary>Creates a window that has a thin-line border.</summary>
        public const uint WS_BORDER = 0x00800000;

        /// <summary>Creates a window with a double border; the window can, optionally, be created with a title bar by specifying the WS_CAPTION style in the dwStyle parameter.</summary>
        public const uint WS_DLGFRAME = 0x00400000;

        /// <summary>Creates a window that is initially maximized.</summary>
        public const uint WS_MAXIMIZE = 0x01000000;

        /// <summary>Creates a window that is initially minimized.</summary>
        public const uint WS_MINIMIZE = 0x20000000;

        /// <summary>Creates a window with a title bar (includes the WS_BORDER style).</summary>
        public const uint WS_CAPTION = 0x00C00000;

        /// <summary>Creates a window with a window menu on its title bar. The WS_CAPTION style must also be specified.</summary>
        public const uint WS_SYSMENU = 0x00080000;

        /// <summary>Creates a window that has a sizing border.</summary>
        public const uint WS_THICKFRAME = 0x00040000;

        /// <summary>Creates a window with a minimize button.</summary>
        public const uint WS_MINIMIZEBOX = 0x00020000;

        /// <summary>Creates a window with a maximize button.</summary>
        public const uint WS_MAXIMIZEBOX = 0x00010000;

        /// <summary>Creates an overlapped window. An overlapped window has a title bar and a border.</summary>
        public const uint WS_OVERLAPPED = 0x00000000;

        /// <summary>Specifies that a window created with this style accepts drag-drop files.</summary>
        public const uint WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);

        /// <summary>Creates a pop-up window. This style cannot be used with the WS_CHILD style.</summary>
        public const uint WS_POPUP = 0x80000000;

        /// <summary>Creates a pop-up window with WS_BORDER, WS_POPUP, and WS_SYSMENU styles. The WS_CAPTION and WS_POPUPWINDOW styles must be combined to make the window menu visible.</summary>
        public const uint WS_POPUPWINDOW = (WS_POPUP | WS_BORDER | WS_SYSMENU);

        /// <summary>Creates a window with a horizontal scroll bar.</summary>
        public const uint WS_HSCROLL = 0x00100000;

        /// <summary>Creates a window with a vertical scroll bar.</summary>
        public const uint WS_VSCROLL = 0x00200000;

        /// <summary>Window is initially visible.</summary>
        public const uint WS_VISIBLE = 0x10000000;

        #endregion

        #region Extended Window Styles

        /// <summary>Specifies that a window created with this style accepts drag-drop files.</summary>
        public const uint WS_EX_ACCEPTFILES = 0x00000010;

        /// <summary>Forces a top-level window onto the taskbar when the window is visible.</summary>
        public const uint WS_EX_APPWINDOW = 0x00040000;

        /// <summary>Specifies that a window has a border with a sunken edge.</summary>
        public const uint WS_EX_CLIENTEDGE = 0x00000200;

        /// <summary>The window itself contains child windows that should take part in dialog box navigation.</summary>
        public const uint WS_EX_CONTROLPARENT = 0x00010000;

        /// <summary>Creates a window that is intended to be used as a floating toolbar.</summary>
        public const uint WS_EX_PALETTEWINDOW = 0x00000188;

        /// <summary>Specifies that a window should be placed above all non-topmost windows and stay above them, even when the window is deactivated.</summary>
        public const uint WS_EX_TOPMOST = 0x00000008;

        /// <summary>Specifies that a window created with this style is to be transparent. That is, any windows beneath the window are not obscured by the window.</summary>
        public const uint WS_EX_TRANSPARENT = 0x00000020;

        #endregion

        #region ShowWindow Commands

        /// <summary>Hides the window and activates another window.</summary>
        public const int SW_HIDE = 0;

        /// <summary>Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position.</summary>
        public const int SW_SHOWNORMAL = 1;

        /// <summary>Activates the window and displays it as a maximized window.</summary>
        public const int SW_SHOWMAXIMIZED = 3;

        /// <summary>Displays the window as a minimized window. The active window remains active.</summary>
        public const int SW_SHOWMINIMIZED = 2;

        /// <summary>Displays the window and activates it. The window is displayed as a maximized window.</summary>
        public const int SW_SHOW = 5;

        /// <summary>Minimizes the specified window and activates the next top-level window in the Z order.</summary>
        public const int SW_MINIMIZE = 6;

        /// <summary>Displays the window as a minimized window. The active window remains active.</summary>
        public const int SW_SHOWMINNOACTIVE = 7;

        /// <summary>Displays the window in its current state. The active window remains active.</summary>
        public const int SW_SHOWNOACTIVATE = 8;

        /// <summary>Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position.</summary>
        public const int SW_RESTORE = 9;

        #endregion

        #region SetWindowPos Flags

        /// <summary>Retains the current size (ignores the cx and cy parameters).</summary>
        public const uint SWP_NOSIZE = 0x0001;

        /// <summary>Retains the current position (ignores X and Y parameters).</summary>
        public const uint SWP_NOMOVE = 0x0002;

        /// <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
        public const uint SWP_NOZORDER = 0x0004;

        /// <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved.</summary>
        public const uint SWP_NOREDRAW = 0x0008;

        /// <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).</summary>
        public const uint SWP_NOACTIVATE = 0x0010;

        /// <summary>Draws a frame (defined in the window's class description) around the window.</summary>
        public const uint SWP_DRAWFRAME = 0x0020;

        /// <summary>Applies new frame styles set by the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.</summary>
        public const uint SWP_FRAMECHANGED = 0x0020;

        /// <summary>Displays the window.</summary>
        public const uint SWP_SHOWWINDOW = 0x0040;

        /// <summary>Hides the window.</summary>
        public const uint SWP_HIDEWINDOW = 0x0080;

        /// <summary>Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.</summary>
        public const uint SWP_NOCOPYBITS = 0x0100;

        /// <summary>Does not change the owner window's position in the Z order.</summary>
        public const uint SWP_NOOWNERZORDER = 0x0200;

        /// <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
        public const uint SWP_NOSENDCHANGING = 0x0400;

        /// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
        public const uint SWP_DEFERERASE = 0x2000;

        /// <summary>If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.</summary>
        public const uint SWP_ASYNCWINDOWPOS = 0x4000;

        #endregion

        #region Window Index Constants

        /// <summary>Places the window below all non-topmost windows. The window loses its topmost status even if it is currently active.</summary>
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        /// <summary>Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a topmost window.</summary>
        public static readonly IntPtr HWND_TOP = new IntPtr(0);

        /// <summary>Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, this flag has no effect.</summary>
        public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        /// <summary>Places the window above all non-topmost windows. The topmost window loses its topmost status as soon as another window is activated.</summary>
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        #endregion

        #region GWL Constants (GetWindowLong indices)

        /// <summary>Sets a new extended window style.</summary>
        public const int GWL_EXSTYLE = -20;

        /// <summary>Sets a new window style.</summary>
        public const int GWL_STYLE = -16;

        /// <summary>Sets a new address for the window procedure.</summary>
        public const int GWL_WNDPROC = -4;

        /// <summary>Sets a new identifier of the window.</summary>
        public const int GWL_ID = -12;

        /// <summary>Sets the user data associated with the window.</summary>
        public const int GWL_USERDATA = -21;

        #endregion

        #region MDI Client Class Name

        /// <summary>The window class name for MDI client area (used with CreateWindowEx).</summary>
        public const string MDICLIENT_CLASS = "MDICLIENT";

        #endregion
    }
}

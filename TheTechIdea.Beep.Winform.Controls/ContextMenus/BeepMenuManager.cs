using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus
{
    /// <summary>
    /// Global manager for BeepContextMenu instances
    /// Implements ToolStripManager.ModalMenuFilter pattern from WinForms
    /// </summary>
    public static class BeepMenuManager
    {
        #region Fields
        
        private static ModalMenuFilter _modalMenuFilter;
        private static readonly object _lock = new object();
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets whether any menu is currently in menu mode
        /// </summary>
        public static bool IsInMenuMode
        {
            get
            {
                lock (_lock)
                {
                    return _modalMenuFilter != null && _modalMenuFilter.InMenuMode;
                }
            }
        }

        /// <summary>
        /// Gets the currently active menu
        /// </summary>
        public static BeepContextMenu? ActiveMenu
        {
            get
            {
                lock (_lock)
                {
                    return _modalMenuFilter?.GetActiveMenu();
                }
            }
        }

        /// <summary>
        /// Gets the initial/root menu (the first menu opened)
        /// </summary>
        public static BeepContextMenu? InitialMenu
        {
            get
            {
                lock (_lock)
                {
                    return _modalMenuFilter?.GetInitialMenu();
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Enters menu mode with the specified menu
        /// </summary>
        internal static void EnterMenuMode(BeepContextMenu menu)
        {
            lock (_lock)
            {
                if (_modalMenuFilter == null)
                {
                    _modalMenuFilter = new ModalMenuFilter();
                }
                _modalMenuFilter.SetActiveMenu(menu);
            }
        }

        /// <summary>
        /// Closes all active menus and opens a new menu
        /// This is used when switching from one context menu to another
        /// </summary>
        public static void SwitchToMenu(BeepContextMenu newMenu)
        {
            lock (_lock)
            {
                if (_modalMenuFilter != null)
                {
                    // Close all existing menus
                    _modalMenuFilter.CloseAllMenusPublic(BeepContextMenuCloseReason.CloseCalled);
                    _modalMenuFilter.ExitMenuMode();
                }

                // Start fresh with the new menu
                if (newMenu != null)
                {
                    EnterMenuMode(newMenu);
                }
            }
        }

        /// <summary>
        /// Exits menu mode
        /// </summary>
        internal static void ExitMenuMode()
        {
            lock (_lock)
            {
                _modalMenuFilter?.ExitMenuMode();
            }
        }

        /// <summary>
        /// Removes a specific menu from tracking
        /// </summary>
        internal static void RemoveActiveMenu(BeepContextMenu menu)
        {
            lock (_lock)
            {
                _modalMenuFilter?.RemoveMenu(menu);
            }
        }

        #endregion

        #region ModalMenuFilter (WinForms Pattern)

        /// <summary>
        /// Message filter for menu mode - implements ToolStripManager.ModalMenuFilter pattern
        /// </summary>
        private class ModalMenuFilter : IMessageFilter
        {
            private readonly List<BeepContextMenu> _activeMenus = new List<BeepContextMenu>();
            private BeepContextMenu? _initialMenu; // The root/first menu opened
            private IntPtr _lastActiveWindow = IntPtr.Zero;
            private bool _inMenuMode = false;
            private bool _processingClose = false; // Prevent re-entrancy

            #region P/Invoke

            [DllImport("user32.dll")]
            private static extern IntPtr GetActiveWindow();

            [DllImport("user32.dll")]
            private static extern bool IsChild(IntPtr hWndParent, IntPtr hWnd);

            [DllImport("user32.dll")]
            private static extern IntPtr WindowFromPoint(Point pt);

            [DllImport("user32.dll")]
            private static extern bool GetCursorPos(out Point lpPoint);

            private const int WM_LBUTTONDOWN = 0x0201;
            private const int WM_RBUTTONDOWN = 0x0204;
            private const int WM_MBUTTONDOWN = 0x0207;
            private const int WM_NCLBUTTONDOWN = 0x00A1;
            private const int WM_NCRBUTTONDOWN = 0x00A4;
            private const int WM_NCMBUTTONDOWN = 0x00A7;
            private const int WM_KEYDOWN = 0x0100;
            private const int WM_SYSKEYDOWN = 0x0104;
            private const int WM_MOUSEMOVE = 0x0200;
            private const int WM_NCMOUSEMOVE = 0x00A0;

            #endregion

            public bool InMenuMode => _inMenuMode;

            public BeepContextMenu? GetActiveMenu()
            {
                return _activeMenus.Count > 0 ? _activeMenus[_activeMenus.Count - 1] : null;
            }

            public BeepContextMenu? GetInitialMenu()
            {
                return _initialMenu;
            }

            public void SetActiveMenu(BeepContextMenu menu)
            {
                if (menu == null) return;

                // First menu becomes the initial menu
                if (_initialMenu == null)
                {
                    _initialMenu = menu;
                }

                if (!_activeMenus.Contains(menu))
                {
                    _activeMenus.Add(menu);
                }

                if (!_inMenuMode)
                {
                    _inMenuMode = true;
                    Application.AddMessageFilter(this);
                }
            }

            public void RemoveMenu(BeepContextMenu menu)
            {
                if (menu == null) return;

                _activeMenus.Remove(menu);

                // If we're removing the initial menu, clear it
                if (menu == _initialMenu)
                {
                    _initialMenu = null;
                }

                if (_activeMenus.Count == 0)
                {
                    ExitMenuMode();
                }
            }

            public void ExitMenuMode()
            {
                if (_inMenuMode)
                {
                    _inMenuMode = false;
                    Application.RemoveMessageFilter(this);
                    _activeMenus.Clear();
                    _initialMenu = null;
                    _processingClose = false;
                }
            }

            public bool PreFilterMessage(ref Message m)
            {
                if (!_inMenuMode || _activeMenus.Count == 0 || _processingClose)
                {
                    return false;
                }

                // Clean up disposed menus
                _activeMenus.RemoveAll(menu => menu == null || menu.IsDisposed);
                
                if (_activeMenus.Count == 0)
                {
                    ExitMenuMode();
                    return false;
                }

                var activeMenu = GetActiveMenu();
                if (activeMenu == null || activeMenu.IsDisposed)
                {
                    ExitMenuMode();
                    return false;
                }

                // Check for activation changes
                IntPtr activeWindow = GetActiveWindow();
                if (activeWindow != _lastActiveWindow)
                {
                    try
                    {
                        if (ProcessActivationChange(activeWindow))
                        {
                            return false; // Let it through
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        // Menu was disposed during processing, exit cleanly
                        ExitMenuMode();
                        return false;
                    }
                    _lastActiveWindow = activeWindow;
                }

                // Handle mouse messages
                if (IsMouseMessage(m.Msg))
                {
                    try
                    {
                        return ProcessMouseMessage(ref m);
                    }
                    catch (ObjectDisposedException)
                    {
                        ExitMenuMode();
                        return false;
                    }
                }

                // Handle keyboard messages
                if (IsKeyboardMessage(m.Msg))
                {
                    try
                    {
                        return ProcessKeyboardMessage(ref m);
                    }
                    catch (ObjectDisposedException)
                    {
                        ExitMenuMode();
                        return false;
                    }
                }

                return false;
            }

            private bool ProcessActivationChange(IntPtr activeWindow)
            {
                // Clean up disposed menus first
                _activeMenus.RemoveAll(menu => menu == null || menu.IsDisposed);
                
                if (_activeMenus.Count == 0)
                {
                    return true;
                }

                // If another window is being activated that's not one of our menus
                bool isMenuWindow = false;
                foreach (var menu in _activeMenus.ToList()) // ToList to avoid modification during iteration
                {
                    if (menu == null || menu.IsDisposed)
                        continue;

                    if (menu.Handle == activeWindow || IsChild(menu.Handle, activeWindow))
                    {
                        isMenuWindow = true;
                        break;
                    }
                }

                if (!isMenuWindow && activeWindow != IntPtr.Zero)
                {
                    // Another window is being activated - close all menus
                    CloseAllMenus(BeepContextMenuCloseReason.AppFocusChange);
                    return true;
                }

                return false;
            }

            private bool ProcessMouseMessage(ref Message m)
            {
                // Clean up disposed menus
                _activeMenus.RemoveAll(menu => menu == null || menu.IsDisposed);
                
                if (_activeMenus.Count == 0)
                {
                    return false;
                }

                // Check if it's a mouse button down message
                if (IsMouseButtonDown(m.Msg))
                {
                    // Get the click location
                    Point screenPos;
                    if (IsNonClientMessage(m.Msg))
                    {
                        GetCursorPos(out screenPos);
                    }
                    else
                    {
                        int x = unchecked((short)(long)m.LParam);
                        int y = unchecked((short)((long)m.LParam >> 16));
                        var control = Control.FromHandle(m.HWnd);
                        if (control != null)
                        {
                            screenPos = control.PointToScreen(new Point(x, y));
                        }
                        else
                        {
                            GetCursorPos(out screenPos);
                        }
                    }

                    // Check if click is inside any menu
                    bool clickedInsideMenu = false;
                    foreach (var menu in _activeMenus.ToList())
                    {
                        if (menu != null && !menu.IsDisposed && menu.Visible)
                        {
                            var menuBounds = menu.Bounds;
                            if (menuBounds.Contains(screenPos))
                            {
                                clickedInsideMenu = true;
                                break;
                            }
                        }
                    }

                    // If clicked outside all menus, close them
                    // WinForms behavior: Close on click outside
                    if (!clickedInsideMenu)
                    {
                        CloseAllMenus(BeepContextMenuCloseReason.AppClicked);
                        return false; // Don't eat the message
                    }
                }

                // WinForms ContextMenu behavior: 
                // - Does NOT auto-close on mouse leave
                // - Only closes on: click outside, ESC key, or focus change
                // This matches the native Windows context menu behavior

                return false;
            }

            private bool ProcessKeyboardMessage(ref Message m)
            {
                if (m.Msg == WM_KEYDOWN || m.Msg == WM_SYSKEYDOWN)
                {
                    Keys keyCode = (Keys)(int)m.WParam;

                    switch (keyCode)
                    {
                        case Keys.Escape:
                            CloseAllMenus(BeepContextMenuCloseReason.Keyboard);
                            return true; // Eat the message

                        case Keys.Menu: // Alt key
                            CloseAllMenus(BeepContextMenuCloseReason.Keyboard);
                            return false; // Let it through for other processing
                    }
                }

                return false;
            }

            private void CloseAllMenus(BeepContextMenuCloseReason reason)
            {
                if (_processingClose)
                    return; // Prevent re-entrancy

                _processingClose = true;

                try
                {
                    // Make a copy to avoid modification during iteration
                    var menusToClose = _activeMenus.ToList();

                    // Close in reverse order (child to parent)
                    for (int i = menusToClose.Count - 1; i >= 0; i--)
                    {
                        var menu = menusToClose[i];
                        if (menu != null && !menu.IsDisposed && menu.Visible)
                        {
                            try
                            {
                                menu.SetCloseReason(reason);
                                menu.Close();
                            }
                            catch (ObjectDisposedException)
                            {
                                // Menu was already disposed, skip it
                            }
                        }
                    }
                }
                finally
                {
                    _processingClose = false;
                }
            }

            // Public method for external access (used by BeepMenuManager.SwitchToMenu)
            public void CloseAllMenusPublic(BeepContextMenuCloseReason reason)
            {
                CloseAllMenus(reason);
            }

            private bool IsMouseMessage(int msg)
            {
                return msg >= 0x0200 && msg <= 0x020E || // Mouse messages
                       msg >= 0x00A0 && msg <= 0x00A9;   // Non-client mouse messages
            }

            private bool IsKeyboardMessage(int msg)
            {
                return msg >= 0x0100 && msg <= 0x0109; // Keyboard messages
            }

            private bool IsMouseButtonDown(int msg)
            {
                return msg == WM_LBUTTONDOWN || msg == WM_RBUTTONDOWN || msg == WM_MBUTTONDOWN ||
                       msg == WM_NCLBUTTONDOWN || msg == WM_NCRBUTTONDOWN || msg == WM_NCMBUTTONDOWN;
            }

            private bool IsNonClientMessage(int msg)
            {
                return msg >= 0x00A0 && msg <= 0x00A9;
            }
        }

        #endregion
    }
}

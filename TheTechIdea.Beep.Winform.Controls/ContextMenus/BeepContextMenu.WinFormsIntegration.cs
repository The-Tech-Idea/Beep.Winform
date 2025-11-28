using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus
{
    /// <summary>
    /// WinForms-specific integration for BeepContextMenu
    /// Implements patterns from ToolStripDropDown for proper behavior
    /// </summary>
    public partial class BeepContextMenu
    {
        #region WinForms P/Invoke Declarations
        
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        private const int GWL_HWNDPARENT = -8;
        private const uint RDW_FRAME = 0x0400;
        private const uint RDW_UPDATENOW = 0x0100;
        private const uint RDW_INVALIDATE = 0x0001;
        
        #endregion

        #region Owner Window Management (WinForms Pattern)

        /// <summary>
        /// Gets or creates the hidden owner window for dropdown menus
        /// This prevents the menu from appearing in the taskbar
        /// Mirrors ToolStripDropDown.DropDownOwnerWindow
        /// </summary>
        internal static NativeWindow DropDownOwnerWindow
        {
            get
            {
                if (_dropDownOwnerWindow == null)
                {
                    _dropDownOwnerWindow = new NativeWindow();
                }

                if (_dropDownOwnerWindow.Handle == IntPtr.Zero)
                {
                    CreateParams cp = new CreateParams
                    {
                        Caption = "BeepContextMenuOwner",
                        Style = unchecked((int)0x80000000), // WS_POPUP
                        ClassStyle = 0,
                        X = 0,
                        Y = 0,
                        Width = 0,
                        Height = 0,
                        Parent = IntPtr.Zero,
                        Param = null
                    };
                    _dropDownOwnerWindow.CreateHandle(cp);
                }

                return _dropDownOwnerWindow;
            }
        }

        /// <summary>
        /// Reparents this menu to the hidden owner window
        /// This prevents taskbar entry
        /// Mirrors ToolStripDropDown.ReparentToDropDownOwnerWindow
        /// </summary>
        private void ReparentToDropDownOwnerWindow()
        {
            if (IsHandleCreated && TopMost)
            {
                SetWindowLong(Handle, GWL_HWNDPARENT, DropDownOwnerWindow.Handle);
            }
        }

        #endregion

        #region CloseReason Management (WinForms Pattern)

        /// <summary>
        /// Sets the reason for closing this menu
        /// Mirrors ToolStripDropDown.SetCloseReason
        /// </summary>
        internal void SetCloseReason(BeepContextMenuCloseReason reason)
        {
            _closeReason = reason;
        }

        /// <summary>
        /// Resets the close reason to default
        /// </summary>
        private void ResetCloseReason()
        {
            _closeReason = BeepContextMenuCloseReason.AppFocusChange;
        }

        #endregion

        #region Window Message Processing (WinForms Pattern)

        /// <summary>
        /// Override WndProc to handle special messages for dropdown behavior
        /// Mirrors ToolStripDropDown.WndProc patterns
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            // Safety check: prevent processing messages if form is disposed
            if (IsDisposed || !IsHandleCreated)
            {
                base.WndProc(ref m);
                return;
            }

            switch (m.Msg)
            {
                case WM_NCACTIVATE:
                    // Keep the owner window's title bar active when we show
                    // This prevents desktop flicker
                    WmNCActivate(ref m);
                    return;

                case WM_ACTIVATE:
                    // Handle activation changes
                    // ContextMenuManager handles menu lifecycle, not BeepMenuManager
                    base.WndProc(ref m);
                    return;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <summary>
        /// Handles WM_NCACTIVATE to keep owner window title bar active
        /// Mirrors ToolStripDropDown.WmNCActivate
        /// </summary>
        private void WmNCActivate(ref Message m)
        {
            if (m.WParam == IntPtr.Zero)
            {
                // We're being deactivated
                base.WndProc(ref m);
            }
            else
            {
                // We're being activated - notify the owner to stay active
                if (!_sendingActivateMessage)
                {
                    _sendingActivateMessage = true;
                    try
                    {
                        IntPtr activeWindow = GetActiveWindow();
                        if (activeWindow != IntPtr.Zero && activeWindow != Handle)
                        {
                            // Send WM_NCACTIVATE to keep the owner's title bar active
                            SendMessage(activeWindow, WM_NCACTIVATE, new IntPtr(1), new IntPtr(-1));
                            
                            // Redraw the owner's frame
                            RedrawWindow(activeWindow, IntPtr.Zero, IntPtr.Zero, 
                                RDW_FRAME | RDW_UPDATENOW | RDW_INVALIDATE);
                        }
                    }
                    finally
                    {
                        _sendingActivateMessage = false;
                    }
                }

                base.WndProc(ref m);
            }
        }

        #endregion

        #region SetVisibleCore Override (WinForms Pattern)

        /// <summary>
        /// Core visibility change logic
        /// Mirrors ToolStripDropDown.SetVisibleCore for proper event sequencing
        /// </summary>
        protected override void SetVisibleCore(bool visible)
        {
            if (visible)
            {
                // Showing the menu
                if (!Visible)
                {
                    // Fire Opening event before showing
                    var openingArgs = new FormClosingEventArgs(CloseReason.None, false);
                    OnMenuOpening(openingArgs);

                    if (openingArgs.Cancel)
                    {
                        return; // User cancelled opening
                    }

                    // TEMPORARILY DISABLED: Reparent might be blocking mouse events
                    // TODO: Re-enable once mouse tracking is working
                    // ReparentToDropDownOwnerWindow();

                    // Actually show the window
                    base.SetVisibleCore(true);
                    
                    // CRITICAL FIX: Enable mouse tracking
                    // Without this, MouseMove events don't fire properly
                    if (IsHandleCreated)
                    {
                        // Enable mouse hover tracking for the window
                        NativeMethods.TRACKMOUSEEVENT tme = new NativeMethods.TRACKMOUSEEVENT
                        {
                            cbSize = Marshal.SizeOf(typeof(NativeMethods.TRACKMOUSEEVENT)),
                            dwFlags = NativeMethods.TME_HOVER | NativeMethods.TME_LEAVE,
                            hwndTrack = this.Handle,
                            dwHoverTime = 1 // Immediate hover detection
                        };
                        NativeMethods.TrackMouseEvent(ref tme);
                        
                        System.Diagnostics.Debug.WriteLine("[BeepContextMenu] Mouse tracking enabled");
                    }

                    // ContextMenuManager handles menu lifecycle - no BeepMenuManager needed

                    // Fire Opened event
                    OnMenuOpened(EventArgs.Empty);
                }
            }
            else
            {
                // Hiding the menu
                if (Visible)
                {
                    // Fire Closing event with close reason
                    var closingArgs = new BeepContextMenuClosingEventArgs(_closeReason, false);
                    OnMenuClosing(closingArgs);

                    if (closingArgs.Cancel)
                    {
                        return; // User cancelled closing
                    }

                    // Actually hide the window
                    base.SetVisibleCore(false);

                    // ContextMenuManager handles menu lifecycle - no BeepMenuManager needed

                    // Fire Closed event
                    var closedArgs = new BeepContextMenuClosedEventArgs(_closeReason);
                    OnMenuClosed(closedArgs);

                    // Reset close reason for next time
                    ResetCloseReason();
                }
            }
        }
        
        /// <summary>
        /// Native methods for mouse tracking
        /// </summary>
        private static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

            [StructLayout(LayoutKind.Sequential)]
            public struct TRACKMOUSEEVENT
            {
                public int cbSize;
                public uint dwFlags;
                public IntPtr hwndTrack;
                public uint dwHoverTime;
            }

            public const uint TME_HOVER = 0x00000001;
            public const uint TME_LEAVE = 0x00000002;
        }

        #endregion

        #region Event Helpers

        /// <summary>
        /// Fires when the menu is about to open
        /// </summary>
        protected virtual void OnMenuOpening(FormClosingEventArgs e)
        {
            // Can be overridden by derived classes
        }

        /// <summary>
        /// Fires when the menu has opened
        /// </summary>
        protected virtual void OnMenuOpened(EventArgs e)
        {
            // Can be overridden by derived classes
        }

        /// <summary>
        /// Fires when the menu has closed
        /// </summary>
        protected virtual void OnMenuClosed(BeepContextMenuClosedEventArgs e)
        {
            // Can be overridden by derived classes
        }

        #endregion
    }

    #region Event Args

    /// <summary>
    /// Event arguments for menu closing event
    /// Mirrors ToolStripDropDownClosingEventArgs
    /// </summary>
    public class BeepContextMenuClosingEventArgs : FormClosingEventArgs
    {
        public BeepContextMenuCloseReason MenuCloseReason { get; }

        public BeepContextMenuClosingEventArgs(BeepContextMenuCloseReason reason, bool cancel)
            : base(CloseReason.None, cancel)
        {
            MenuCloseReason = reason;
        }
    }

    /// <summary>
    /// Event arguments for menu closed event
    /// Mirrors ToolStripDropDownClosedEventArgs
    /// </summary>
    public class BeepContextMenuClosedEventArgs : EventArgs
    {
        public BeepContextMenuCloseReason MenuCloseReason { get; }

        public BeepContextMenuClosedEventArgs(BeepContextMenuCloseReason reason)
        {
            MenuCloseReason = reason;
        }
    }

    #endregion
}

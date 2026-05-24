using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Interop;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime
{
    /// <summary>
    /// Intercepts and routes window messages from MDI child windows.
    /// Captures mouse events and dispatches them to chrome handlers.
    /// </summary>
    public class EventInterceptor : IMessageFilter, IDisposable
    {
        private readonly Dictionary<IntPtr, WindowMessageHook> _windowHooks = new();
        private bool _isInstalled = false;
        private bool _disposed = false;

        /// <summary>
        /// Delegate for window message handlers.
        /// </summary>
        public delegate void WindowMessageHandler(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Tracks hooks for a specific window.
        /// </summary>
        private class WindowMessageHook
        {
            public IntPtr WindowHandle { get; set; }
            public Dictionary<uint, List<WindowMessageHandler>> MessageHandlers { get; set; } = new();
            public int MessageCount { get; set; }
        }

        /// <summary>
        /// Installs the message filter.
        /// </summary>
        public void Install()
        {
            if (_isInstalled)
                return;

            Application.AddMessageFilter(this);
            _isInstalled = true;
            Debug.WriteLine("[EventInterceptor] Message filter installed");
        }

        /// <summary>
        /// Uninstalls the message filter.
        /// </summary>
        public void Uninstall()
        {
            if (!_isInstalled)
                return;

            Application.RemoveMessageFilter(this);
            _isInstalled = false;
            Debug.WriteLine("[EventInterceptor] Message filter uninstalled");
        }

        /// <summary>
        /// Hooks a window for specific message notifications.
        /// </summary>
        public void HookWindow(IntPtr hwnd, uint message, WindowMessageHandler handler)
        {
            if (hwnd == IntPtr.Zero)
                throw new ArgumentException("Window handle cannot be zero", nameof(hwnd));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (!_windowHooks.TryGetValue(hwnd, out var hook))
            {
                hook = new WindowMessageHook { WindowHandle = hwnd };
                _windowHooks[hwnd] = hook;
            }

            if (!hook.MessageHandlers.ContainsKey(message))
            {
                hook.MessageHandlers[message] = new List<WindowMessageHandler>();
            }

            hook.MessageHandlers[message].Add(handler);
            Debug.WriteLine($"[EventInterceptor] Hooked window 0x{hwnd:X8} for message {message:X4}");
        }

        /// <summary>
        /// Unhooks a specific message handler.
        /// </summary>
        public void UnhookMessage(IntPtr hwnd, uint message, WindowMessageHandler handler)
        {
            if (_windowHooks.TryGetValue(hwnd, out var hook))
            {
                if (hook.MessageHandlers.TryGetValue(message, out var handlers))
                {
                    handlers.Remove(handler);

                    if (handlers.Count == 0)
                    {
                        hook.MessageHandlers.Remove(message);
                    }
                }

                if (hook.MessageHandlers.Count == 0)
                {
                    _windowHooks.Remove(hwnd);
                    Debug.WriteLine($"[EventInterceptor] Removed all hooks for window 0x{hwnd:X8}");
                }
            }
        }

        /// <summary>
        /// Unhooks all messages for a window.
        /// </summary>
        public void UnhookWindow(IntPtr hwnd)
        {
            if (_windowHooks.Remove(hwnd))
            {
                Debug.WriteLine($"[EventInterceptor] Unhooked all messages for window 0x{hwnd:X8}");
            }
        }

        /// <summary>
        /// Implemented IMessageFilter.PreFilterMessage.
        /// Intercepts all application messages before they are dispatched.
        /// </summary>
        public bool PreFilterMessage(ref Message m)
        {
            // Ignore if not hooked
            if (!_windowHooks.TryGetValue(m.HWnd, out var hook))
                return false;

            // Check if we have handlers for this message
            if (!hook.MessageHandlers.TryGetValue((uint)m.Msg, out var handlers))
                return false;

            // Dispatch to all handlers for this message
            foreach (var handler in handlers)
            {
                try
                {
                    handler(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[EventInterceptor] Handler exception: {ex.Message}");
                }
            }

            hook.MessageCount++;

            // Don't consume the message - let it propagate normally
            return false;
        }

        /// <summary>
        /// Gets diagnostic information.
        /// </summary>
        public EventInterceptorDiagnostics GetDiagnostics()
        {
            var windowStats = new List<WindowStat>();
            foreach (var kvp in _windowHooks)
            {
                windowStats.Add(new WindowStat
                {
                    WindowHandle = kvp.Key,
                    MessageHandlers = kvp.Value.MessageHandlers.Count,
                    MessageCount = kvp.Value.MessageCount,
                    HookedMessages = new List<uint>(kvp.Value.MessageHandlers.Keys)
                });
            }

            return new EventInterceptorDiagnostics
            {
                IsInstalled = _isInstalled,
                HookedWindows = _windowHooks.Count,
                TotalMessageHandlers = _windowHooks.Values.Sum(x => x.MessageHandlers.Count),
                WindowStats = windowStats
            };
        }

        /// <summary>
        /// Diagnostics result.
        /// </summary>
        public class EventInterceptorDiagnostics
        {
            public bool IsInstalled { get; set; }
            public int HookedWindows { get; set; }
            public int TotalMessageHandlers { get; set; }
            public List<WindowStat> WindowStats { get; set; }
        }

        /// <summary>
        /// Statistics for a hooked window.
        /// </summary>
        public class WindowStat
        {
            public IntPtr WindowHandle { get; set; }
            public int MessageHandlers { get; set; }
            public int MessageCount { get; set; }
            public List<uint> HookedMessages { get; set; }
        }

        /// <summary>
        /// Disposes resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            Uninstall();
            _windowHooks.Clear();
            _disposed = true;
            Debug.WriteLine("[EventInterceptor] Disposed");
        }
    }

    /// <summary>
    /// Handles mouse events on MDI child windows and routes to chrome handlers.
    /// </summary>
    public class DockingMouseEventHandler
    {
        private EventInterceptor _interceptor;
        private WindowChrome _chrome;
        private PanelWindowManager _panelManager;
        private SplitterDragHandler _dragHandler;
        private Dictionary<IntPtr, string> _windowToPanelMap;

        /// <summary>
        /// Initializes the mouse event handler.
        /// </summary>
        public DockingMouseEventHandler(
            EventInterceptor interceptor,
            WindowChrome chrome,
            PanelWindowManager panelManager,
            SplitterDragHandler dragHandler,
            Dictionary<IntPtr, string> windowToPanelMap)
        {
            _interceptor = interceptor ?? throw new ArgumentNullException(nameof(interceptor));
            _chrome = chrome ?? throw new ArgumentNullException(nameof(chrome));
            _panelManager = panelManager ?? throw new ArgumentNullException(nameof(panelManager));
            _dragHandler = dragHandler ?? throw new ArgumentNullException(nameof(dragHandler));
            _windowToPanelMap = windowToPanelMap ?? throw new ArgumentNullException(nameof(windowToPanelMap));
        }

        /// <summary>
        /// Registers a window for mouse event interception.
        /// </summary>
        public void RegisterWindow(IntPtr hwnd)
        {
            // Hook for mouse events
            _interceptor.HookWindow(hwnd, WindowMessages.WM_LBUTTONDOWN, HandleMouseDown);
            _interceptor.HookWindow(hwnd, WindowMessages.WM_MOUSEMOVE, HandleMouseMove);
            _interceptor.HookWindow(hwnd, WindowMessages.WM_LBUTTONUP, HandleMouseUp);
            _interceptor.HookWindow(hwnd, WindowMessages.WM_PAINT, HandlePaint);

            Debug.WriteLine($"[MouseEventHandler] Registered window 0x{hwnd:X8}");
        }

        /// <summary>
        /// Unregisters a window from mouse event interception.
        /// </summary>
        public void UnregisterWindow(IntPtr hwnd)
        {
            _interceptor.UnhookWindow(hwnd);
            Debug.WriteLine($"[MouseEventHandler] Unregistered window 0x{hwnd:X8}");
        }

        private void HandleMouseDown(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (!_windowToPanelMap.TryGetValue(hwnd, out var panelKey))
                return;

            // Extract mouse position from lParam (LOWORD = x, HIWORD = y)
            int x = (int)(lParam.ToInt64() & 0xFFFF);
            int y = (int)((lParam.ToInt64() >> 16) & 0xFFFF);
            var mousePos = new System.Drawing.Point(x, y);

            Debug.WriteLine($"[MouseEventHandler] Mouse down at {mousePos} on panel '{panelKey}'");

            // Check for tab click
            var tabKey = _chrome.HitTestTab(mousePos);
            if (!string.IsNullOrEmpty(tabKey))
            {
                _panelManager?.HandleTabClick(tabKey);
                return;
            }

            // Check for close button click
            var closeRect = GetTabCloseButtonRect(mousePos);
            var closeTabKey = _chrome.HitTestCloseButton(mousePos, closeRect);
            if (!string.IsNullOrEmpty(closeTabKey))
            {
                _panelManager?.HandleTabCloseClick(closeTabKey);
                return;
            }

            // Check for splitter drag start
            _dragHandler?.StartDrag(mousePos);
        }

        private void HandleMouseMove(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            int x = (int)(lParam.ToInt64() & 0xFFFF);
            int y = (int)((lParam.ToInt64() >> 16) & 0xFFFF);
            var mousePos = new System.Drawing.Point(x, y);

            // Update drag if in progress
            _dragHandler?.UpdateDrag(mousePos);
        }

        private void HandleMouseUp(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            int x = (int)(lParam.ToInt64() & 0xFFFF);
            int y = (int)((lParam.ToInt64() >> 16) & 0xFFFF);
            var mousePos = new System.Drawing.Point(x, y);

            // End drag if in progress
            _dragHandler?.EndDrag(mousePos);
        }

        private void HandlePaint(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            // Paint will be handled separately with painter integration
            // For now, just log that paint was requested
        }

        private System.Drawing.Rectangle GetTabCloseButtonRect(System.Drawing.Point mousePos)
        {
            // This would be calculated from the chrome rendering
            // Simplified for now
            return new System.Drawing.Rectangle(mousePos.X - 8, mousePos.Y - 8, 16, 16);
        }
    }

    /// <summary>
    /// Window message constants.
    /// </summary>
    internal static class WindowMessages
    {
        public const uint WM_LBUTTONDOWN = 0x0201;
        public const uint WM_LBUTTONUP = 0x0202;
        public const uint WM_MOUSEMOVE = 0x0200;
        public const uint WM_PAINT = 0x000F;
        public const uint WM_RBUTTONDOWN = 0x0204;
        public const uint WM_RBUTTONUP = 0x0205;
        public const uint WM_LBUTTONDBLCLK = 0x0203;
    }
}

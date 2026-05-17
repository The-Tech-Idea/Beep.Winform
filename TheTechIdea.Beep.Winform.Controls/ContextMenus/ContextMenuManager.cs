using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus
{
    /// <summary>
    /// Centralized manager for BeepContextMenu with full features (sub-menus, tracking, etc.)
    /// Revised implementation with improved stability and performance.
    /// </summary>
    public static class ContextMenuManager
    {
        #region Fields
        
        private static readonly Dictionary<string, MenuContext> _activeMenus = new Dictionary<string, MenuContext>();
        private static readonly object _lock = new object();
        private static int _menuIdCounter = 0;
        private static ClickOutsideFilter _clickOutsideFilter;
        private static bool _isClosingAll = false;
        private static Timer _autoCloseTimer;

        // ─────────────────────────────────────────────────────────────────
        // Phase 01 — Dismissal/Re-open Hot-Fix
        //
        // Raised after a context menu has actually closed. Subscribers
        // (notably BeepMenuBar) use this to suppress the immediate re-open
        // echo caused when the same WM_LBUTTONDOWN that dismissed the
        // popup is subsequently delivered to the owner's OnMouseClick.
        // See .plans/Menus-Phase-01-DismissalReopenHotFix.md.
        // ─────────────────────────────────────────────────────────────────
        public static event EventHandler<MenuDismissedEventArgs> MenuDismissed;
        private const int AUTO_CLOSE_DELAY_MS = 300;
        
        #endregion

        #region Properties
        
        public static bool EnableSubMenus { get; set; } = true;
        
        public static bool IsAnyMenuActive
        {
            get
            {
                lock (_lock)
                {
                    return _activeMenus.Count > 0;
                }
            }
        }
        
        public static int ActiveMenuCount
        {
            get
            {
                lock (_lock)
                {
                    return _activeMenus.Count;
                }
            }
        }
        
        #endregion

        #region Internal Classes
        
        private class MenuContext : IDisposable
        {
            public string Id { get; set; }
            public BeepContextMenu Menu { get; set; }
            public Control Owner { get; set; }
            public SimpleItem SelectedItem { get; set; }
            public List<SimpleItem> SelectedItems { get; set; } = new List<SimpleItem>();
            public bool IsClosed { get; set; }
            public bool IsMultiSelect { get; set; }
            public string ParentMenuId { get; set; }
            public List<string> ChildMenuIds { get; set; } = new List<string>();
            public bool IsClosing { get; set; }
            public Timer MousePoller { get; set; }
            public bool WasHovered { get; set; }
            public TaskCompletionSource<bool> CloseTcs { get; set; }

            // Phase 01: guard so MenuDismissed fires exactly once per context.
            public bool DismissedFired { get; set; }
            
            // Event handlers stored for cleanup
            public EventHandler<MenuItemEventArgs> ItemClickedHandler { get; set; }
            public EventHandler<MenuItemEventArgs> SubmenuOpeningHandler { get; set; }
            public FormClosedEventHandler FormClosedHandler { get; set; }
            
            public void Dispose()
            {
                try
                {
                    MousePoller?.Stop();
                    MousePoller?.Dispose();
                    MousePoller = null;
                }
                catch { }
                
                try
                {
                    if (Menu != null && !Menu.IsDisposed)
                    {
                        if (ItemClickedHandler != null)
                            Menu.ItemClicked -= ItemClickedHandler;
                        if (SubmenuOpeningHandler != null)
                            Menu.SubmenuOpening -= SubmenuOpeningHandler;
                        if (FormClosedHandler != null)
                            Menu.FormClosed -= FormClosedHandler;
                    }
                }
                catch { }
            }
        }
        
        private class ClickOutsideFilter : IMessageFilter
        {
            private const int WM_LBUTTONDOWN = 0x0201;
            private const int WM_RBUTTONDOWN = 0x0204;
            private const int WM_MBUTTONDOWN = 0x0207;
            private const int WM_NCLBUTTONDOWN = 0x00A1;
            private const int WM_NCRBUTTONDOWN = 0x00A4;
            
            public bool PreFilterMessage(ref Message m)
            {
                // Only handle mouse down messages
                if (m.Msg != WM_LBUTTONDOWN && m.Msg != WM_RBUTTONDOWN && 
                    m.Msg != WM_MBUTTONDOWN && m.Msg != WM_NCLBUTTONDOWN && 
                    m.Msg != WM_NCRBUTTONDOWN)
                {
                    return false;
                }
                
                try
                {
                    // Check if click is inside any menu
                    var clickedControl = Control.FromHandle(m.HWnd);

                    // If clicked on a BeepContextMenu, don't close
                    if (IsClickOnMenu(clickedControl))
                        return false;

                    // Get screen position
                    Point screenPos = GetScreenPosition(m, clickedControl);

                    // Check if click is inside any active menu bounds
                    if (IsClickInsideAnyMenu(screenPos))
                        return false;

                    // Click is outside all menus → close them and let the
                    // message continue to its original target.
                    //
                    // History note (2026-05-17): Phase 05 briefly added a
                    // "swallow the dismissing click on the owner" branch
                    // here as defence-in-depth. It was reverted because:
                    //   * For BeepMenuBar (the only consumer that ever
                    //     hit the re-open echo), Phase 01's
                    //     IsInDismissalCoolDown + same-item toggle in
                    //     BeepMenuBar.Popup.cs already prevent the echo.
                    //   * The Phase 05 swallow used the owner's WHOLE
                    //     client rectangle, which for a menubar is the
                    //     full top strip. Clicking a DIFFERENT top-level
                    //     item to cross-navigate (open Edit while File
                    //     was open) was swallowed before OnMouseClick
                    //     could route it, breaking the standard menubar
                    //     hover/click-swap UX.
                    //   * Other BaseControl popup consumers (right-click
                    //     menus, dropdown buttons) open popups via
                    //     explicit gestures; clicking the same target
                    //     again is intended toggle UX, not a bug.
                    CloseAllMenus();
                }
                catch
                {
                    // Silently handle click outside filter errors
                }

                return false; // Default: deliver to the original target.
            }
            
            private bool IsClickOnMenu(Control control)
            {
                var current = control;
                while (current != null)
                {
                    if (current is BeepContextMenu)
                        return true;
                    current = current.Parent;
                }
                return false;
            }
            
            private Point GetScreenPosition(Message m, Control clickedControl)
            {
                if (m.Msg == WM_NCLBUTTONDOWN || m.Msg == WM_NCRBUTTONDOWN)
                {
                    return Cursor.Position;
                }
                
                if (clickedControl != null)
                {
                    int x = unchecked((short)(long)m.LParam);
                    int y = unchecked((short)((long)m.LParam >> 16));
                    return clickedControl.PointToScreen(new Point(x, y));
                }
                
                return Cursor.Position;
            }
            
            private bool IsClickInsideAnyMenu(Point screenPos)
            {
                lock (_lock)
                {
                    if (_isClosingAll || _activeMenus.Count == 0)
                        return false;
                    
                    foreach (var context in _activeMenus.Values)
                    {
                        try
                        {
                            if (context.Menu != null && !context.Menu.IsDisposed && 
                                context.Menu.Visible && !context.IsClosing)
                            {
                                var menuBounds = new Rectangle(context.Menu.Location, context.Menu.Size);
                                if (menuBounds.Contains(screenPos))
                                    return true;
                            }
                        }
                        catch (ObjectDisposedException)
                        {
                            continue;
                        }
                    }
                }
                return false;
            }
        }
        
        #endregion

        #region Public Methods
        
        /// <summary>
        /// Shows a context menu at the specified screen location.
        /// This method blocks until the menu is closed and returns the selected item.
        /// </summary>
        public static SimpleItem Show(
            List<SimpleItem> items,
            Point screenLocation,
            Control owner = null,
            FormStyle style = FormStyle.Modern,
            bool multiSelect = false,
            string theme = null,
            string parentMenuId = null)
        {
            if (items == null || items.Count == 0)
                return null;
            
            lock (_lock)
            {
                if (_isClosingAll)
                    return null;
            }
            
            if (string.IsNullOrEmpty(parentMenuId))
            {
                CloseRootMenus();
            }
            
            var context = CreateMenuContext(parentMenuId, multiSelect);
            if (context == null)
                return null;

            // Phase 01: record owner so MenuDismissed can route to it.
            context.Owner = owner;

            string menuId = context.Id;
            SimpleItem selectedItem = null;
            bool isClosed = false;
            
            var menu = CreateMenu(style, multiSelect, theme);
            PopulateMenu(menu, items);
            menu.RecalculateSize();
            
            lock (_lock)
            {
                if (!_activeMenus.ContainsKey(menuId))
                {
                    menu.Dispose();
                    return null;
                }
                _activeMenus[menuId].Menu = menu;
            }
            
            EventHandler<MenuItemEventArgs> itemClickedHandler = null;
            EventHandler<MenuItemEventArgs> submenuOpeningHandler = null;
            FormClosedEventHandler formClosedHandler = null;
            
            itemClickedHandler = (s, e) =>
            {
                selectedItem = e.Item;
                context.SelectedItem = e.Item;
            };
            
            submenuOpeningHandler = (s, e) =>
            {
                if (e?.Item == null || !HasChildren(e.Item))
                    return;
                
                StopAutoCloseTimer();
                CloseChildMenus(menuId);
                
                var menuLocation = menu.PointToScreen(Point.Empty);
                var itemBounds = menu.LayoutHelper.GetItemRect(e.Item);
                var subMenuLocation = CalculateSubMenuPosition(menuLocation, itemBounds);
                
                try
                {
                    ShowChildMenu(e.Item.Children.ToList(), subMenuLocation, menu, style, theme, menuId);
                }
                catch
                {
                    // Silently handle sub-menu errors
                }
            };
            
            formClosedHandler = (s, e) =>
            {
                context.IsClosed = true;
                isClosed = true;
            };
            
            menu.ItemClicked += itemClickedHandler;
            menu.SubmenuOpening += submenuOpeningHandler;
            menu.FormClosed += formClosedHandler;
            
            context.ItemClickedHandler = itemClickedHandler;
            context.SubmenuOpeningHandler = submenuOpeningHandler;
            context.FormClosedHandler = formClosedHandler;
            
            if (!string.IsNullOrEmpty(parentMenuId))
            {
                SetupMouseTracking(context);
            }
            
            if (string.IsNullOrEmpty(parentMenuId))
            {
                InstallClickOutsideFilter();
            }
            
            try
            {
                // Phase 05 — clamp the requested location to the target
                // monitor's working area so the popup never spills off-screen.
                var shownAt = ClampToWorkingArea(menu.Size, screenLocation);
                menu.Show(shownAt, owner);
                ActivateMenu(menu);

                // Phase 05 — replaced Application.DoEvents()+Thread.Sleep(1)
                // busy-wait with a real blocking message-pump wait.
                PumpUntilClosed(menu, () => isClosed);

                return context.SelectedItem;
            }
            catch
            {
                return null;
            }
            finally
            {
                menu.ItemClicked -= itemClickedHandler;
                menu.SubmenuOpening -= submenuOpeningHandler;
                menu.FormClosed -= formClosedHandler;
                CleanupMenuContext(menuId);
            }
        }

        /// <summary>
        /// Shows a context menu at the specified screen location (async).
        /// Returns the selected item when the menu is closed.
        /// </summary>
        public static async Task<SimpleItem> ShowAsync(
            List<SimpleItem> items,
            Point screenLocation,
            Control owner = null,
            FormStyle style = FormStyle.Modern,
            bool multiSelect = false,
            string theme = null,
            string parentMenuId = null)
        {
            if (items == null || items.Count == 0)
                return null;
            
            return await Task.Run(() => 
                Show(items, screenLocation, owner, style, multiSelect, theme, parentMenuId));
        }
        
        /// <summary>
        /// Shows a multi-select context menu and returns all selected items.
        /// </summary>
        public static async Task<List<SimpleItem>> ShowMultiSelectAsync(
            List<SimpleItem> items,
            Point screenLocation,
            Control owner = null,
            FormStyle style = FormStyle.Modern,
            string theme = null)
        {
            if (items == null || items.Count == 0)
                return new List<SimpleItem>();
            
            await ShowAsync(items, screenLocation, owner, style, true, theme, null);
            
            lock (_lock)
            {
                foreach (var ctx in _activeMenus.Values.Where(c => c.IsMultiSelect && c.IsClosed))
                {
                    return ctx.SelectedItems ?? new List<SimpleItem>();
                }
            }
            
            return new List<SimpleItem>();
        }
        
        /// <summary>
        /// Shows a multi-select context menu and returns all selected items (sync).
        /// </summary>
        public static List<SimpleItem> ShowMultiSelect(
            List<SimpleItem> items,
            Point screenLocation,
            Control owner = null,
            FormStyle style = FormStyle.Modern,
            string theme = null)
        {
            if (items == null || items.Count == 0)
                return new List<SimpleItem>();
            
            lock (_lock)
            {
                if (_isClosingAll)
                    return new List<SimpleItem>();
            }
            
            CloseRootMenus();
            
            var context = CreateMenuContext(null, true);
            if (context == null)
                return new List<SimpleItem>();

            // Phase 01: record owner so MenuDismissed can route to it.
            context.Owner = owner;

            string menuId = context.Id;
            List<SimpleItem> selectedItems = null;
            bool isClosed = false;
            
            var menu = CreateMenu(style, true, theme);
            PopulateMenu(menu, items);
            menu.RecalculateSize();
            
            lock (_lock)
            {
                if (!_activeMenus.ContainsKey(menuId))
                {
                    menu.Dispose();
                    return new List<SimpleItem>();
                }
                _activeMenus[menuId].Menu = menu;
            }
            
            EventHandler<MenuItemEventArgs> itemClickedHandler = null;
            EventHandler<MenuItemEventArgs> submenuOpeningHandler = null;
            FormClosedEventHandler formClosedHandler = null;
            
            itemClickedHandler = (s, e) =>
            {
                context.SelectedItem = e.Item;
            };
            
            submenuOpeningHandler = (s, e) =>
            {
                if (e?.Item == null || !HasChildren(e.Item))
                    return;
                
                StopAutoCloseTimer();
                CloseChildMenus(menuId);
                
                var menuLocation = menu.PointToScreen(Point.Empty);
                var itemBounds = menu.LayoutHelper.GetItemRect(e.Item);
                var subMenuLocation = CalculateSubMenuPosition(menuLocation, itemBounds);
                
                try
                {
                    ShowChildMenu(e.Item.Children.ToList(), subMenuLocation, menu, style, theme, menuId);
                }
                catch
                {
                    // Silently handle sub-menu errors
                }
            };
            
            formClosedHandler = (s, e) =>
            {
                context.IsClosed = true;
                selectedItems = context.SelectedItems ?? new List<SimpleItem>();
                isClosed = true;
            };
            
            menu.ItemClicked += itemClickedHandler;
            menu.SubmenuOpening += submenuOpeningHandler;
            menu.FormClosed += formClosedHandler;
            
            context.ItemClickedHandler = itemClickedHandler;
            context.SubmenuOpeningHandler = submenuOpeningHandler;
            context.FormClosedHandler = formClosedHandler;
            
            InstallClickOutsideFilter();
            
            try
            {
                // Phase 05 — clamp + non-blocking pump (see Show() above).
                var shownAt = ClampToWorkingArea(menu.Size, screenLocation);
                menu.Show(shownAt, owner);
                ActivateMenu(menu);

                PumpUntilClosed(menu, () => isClosed);

                return selectedItems ?? new List<SimpleItem>();
            }
            catch
            {
                return new List<SimpleItem>();
            }
            finally
            {
                menu.ItemClicked -= itemClickedHandler;
                menu.SubmenuOpening -= submenuOpeningHandler;
                menu.FormClosed -= formClosedHandler;
                CleanupMenuContext(menuId);
            }
        }

        /// <summary>
        /// Phase 05 — Shows a context menu without blocking the caller and
        /// returns an <see cref="IDisposable"/> handle. Disposing the
        /// handle closes the popup if it is still open.
        ///
        /// Use this overload when the caller needs to keep processing
        /// input (e.g., the menubar hover-swap path in Phase 04). The
        /// classic blocking <see cref="Show(System.Collections.Generic.List{SimpleItem}, Point, Control, FormStyle, bool, string, string)"/>
        /// overload remains for callers that want a synchronous selected-
        /// item return value.
        /// </summary>
        /// <param name="items">The menu items to display.</param>
        /// <param name="screenLocation">Requested top-left in screen coordinates (will be clamped to the target monitor's working area).</param>
        /// <param name="owner">Owner control. Used for owner-targeted click-swallow and for routing <c>MenuDismissed</c>.</param>
        /// <param name="style">Visual style preset.</param>
        /// <param name="theme">Optional theme override.</param>
        /// <param name="onItemSelected">
        /// Optional callback fired once when the user picks an item.
        /// Invoked on the UI thread before the popup auto-closes.
        /// </param>
        /// <returns>
        /// A <see cref="ContextMenuHandle"/> that closes the popup on
        /// <see cref="IDisposable.Dispose"/>. Never <c>null</c>; returns
        /// <see cref="ContextMenuHandle.Empty"/> on failure.
        /// </returns>
        public static IDisposable ShowNonBlocking(
            List<SimpleItem> items,
            Point screenLocation,
            Control owner = null,
            FormStyle style = FormStyle.Modern,
            string theme = null,
            Action<SimpleItem> onItemSelected = null)
        {
            if (items == null || items.Count == 0)
                return ContextMenuHandle.Empty;

            lock (_lock)
            {
                if (_isClosingAll)
                    return ContextMenuHandle.Empty;
            }

            // Same root-menu policy as Show(): only one top-level popup at a time.
            CloseRootMenus();

            var context = CreateMenuContext(null, false);
            if (context == null)
                return ContextMenuHandle.Empty;

            context.Owner = owner; // For MenuDismissed routing + owner-targeted click swallow.

            string menuId = context.Id;
            var menu = CreateMenu(style, false, theme);
            PopulateMenu(menu, items);
            menu.RecalculateSize();

            lock (_lock)
            {
                if (!_activeMenus.ContainsKey(menuId))
                {
                    menu.Dispose();
                    return ContextMenuHandle.Empty;
                }
                _activeMenus[menuId].Menu = menu;
            }

            EventHandler<MenuItemEventArgs> itemClickedHandler = null;
            EventHandler<MenuItemEventArgs> submenuOpeningHandler = null;
            FormClosedEventHandler formClosedHandler = null;

            itemClickedHandler = (s, e) =>
            {
                context.SelectedItem = e.Item;
                if (onItemSelected != null && e.Item != null)
                {
                    try { onItemSelected(e.Item); }
                    catch { /* user callback — swallow to keep popup robust */ }
                }
            };

            submenuOpeningHandler = (s, e) =>
            {
                if (e?.Item == null || !HasChildren(e.Item))
                    return;

                StopAutoCloseTimer();
                CloseChildMenus(menuId);

                var menuLocation = menu.PointToScreen(Point.Empty);
                var itemBounds = menu.LayoutHelper.GetItemRect(e.Item);
                var subMenuLocation = CalculateSubMenuPosition(menuLocation, itemBounds);

                try
                {
                    ShowChildMenu(e.Item.Children.ToList(), subMenuLocation, menu, style, theme, menuId);
                }
                catch { /* silently handle sub-menu errors */ }
            };

            formClosedHandler = (s, e) =>
            {
                context.IsClosed = true;
            };

            menu.ItemClicked    += itemClickedHandler;
            menu.SubmenuOpening += submenuOpeningHandler;
            menu.FormClosed     += formClosedHandler;

            context.ItemClickedHandler     = itemClickedHandler;
            context.SubmenuOpeningHandler  = submenuOpeningHandler;
            context.FormClosedHandler      = formClosedHandler;

            InstallClickOutsideFilter();

            try
            {
                var shownAt = ClampToWorkingArea(menu.Size, screenLocation);
                menu.Show(shownAt, owner);
                ActivateMenu(menu);
            }
            catch
            {
                CleanupMenuContext(menuId);
                return ContextMenuHandle.Empty;
            }

            return new ContextMenuHandle(menuId);
        }

        /// <summary>
        /// Closes a specific menu by ID.
        /// </summary>
        public static void CloseMenu(string menuId)
        {
            if (string.IsNullOrEmpty(menuId))
                return;
            
            MenuContext context;
            lock (_lock)
            {
                if (!_activeMenus.TryGetValue(menuId, out context))
                    return;
                
                if (context.IsClosing)
                    return;
                
                context.IsClosing = true;
            }
            
            try
            {
                // Close child menus first
                var childIds = context.ChildMenuIds.ToList();
                foreach (var childId in childIds)
                {
                    CloseMenu(childId);
                }
                
                // Close this menu
                if (context.Menu != null && !context.Menu.IsDisposed)
                {
                    if (context.Menu.InvokeRequired)
                    {
                        context.Menu.BeginInvoke(new Action(() => SafeCloseMenu(context.Menu)));
                    }
                    else
                    {
                        SafeCloseMenu(context.Menu);
                    }
                }
            }
            catch
            {
                // Silently handle close menu errors
            }
        }
        
        /// <summary>
        /// Closes all active menus.
        /// </summary>
        public static void CloseAllMenus()
        {
            lock (_lock)
            {
                if (_isClosingAll || _activeMenus.Count == 0)
                    return;
                
                _isClosingAll = true;
            }
            
            try
            {
                StopAutoCloseTimer();
                
                List<string> menuIds;
                List<MenuContext> contextSnapshot;
                lock (_lock)
                {
                    menuIds = _activeMenus.Keys.ToList();
                    // Phase 01: snapshot contexts so we can fire MenuDismissed
                    // *after* CloseMenu has run (so LastCloseReason is final),
                    // but *before* the bulk Clear() wipes our reference.
                    contextSnapshot = _activeMenus.Values.ToList();
                }
                
                foreach (var id in menuIds)
                {
                    try
                    {
                        CloseMenu(id);
                    }
                    catch { }
                }

                // Phase 01: notify per dismissed root.
                foreach (var ctx in contextSnapshot)
                {
                    RaiseMenuDismissed(ctx);
                }

                RemoveClickOutsideFilter();
                
                lock (_lock)
                {
                    _activeMenus.Clear();
                }
            }
            finally
            {
                lock (_lock)
                {
                    _isClosingAll = false;
                }
            }
        }
        
        /// <summary>
        /// Checks if an item has children (for submenu support).
        /// </summary>
        public static bool HasChildren(SimpleItem item)
        {
            return item?.Children != null && item.Children.Count > 0;
        }
        
        #endregion

        #region Private Methods - Menu Creation
        
        private static MenuContext CreateMenuContext(string parentMenuId, bool multiSelect)
        {
            lock (_lock)
            {
                if (_isClosingAll)
                    return null;
                
                string menuId = $"Menu_{++_menuIdCounter}_{DateTime.Now.Ticks}";
                
                var context = new MenuContext
                {
                    Id = menuId,
                    IsMultiSelect = multiSelect,
                    ParentMenuId = parentMenuId
                };
                
                // Link to parent if this is a child menu
                if (!string.IsNullOrEmpty(parentMenuId) && _activeMenus.TryGetValue(parentMenuId, out var parent))
                {
                    // Close existing siblings
                    foreach (var siblingId in parent.ChildMenuIds.ToList())
                    {
                        CloseMenu(siblingId);
                    }
                    parent.ChildMenuIds.Add(menuId);
                }
                
                _activeMenus[menuId] = context;
                return context;
            }
        }
        
        private static BeepContextMenu CreateMenu(FormStyle style, bool multiSelect, string theme)
        {
            var menu = new BeepContextMenu
            {
                ContextMenuType = style,
                DestroyOnClose = true,
                MultiSelect = multiSelect,
                ShowCheckBox = multiSelect,
                StartPosition = FormStartPosition.Manual,
                CloseOnFocusLost = false
            };
            
            if (!string.IsNullOrEmpty(theme))
            {
                menu.Theme = theme;
            }
            
            return menu;
        }
        
        private static void PopulateMenu(BeepContextMenu menu, List<SimpleItem> items)
        {
            foreach (var item in items)
            {
                menu.AddItem(item);
            }
        }
        
        private static void ActivateMenu(BeepContextMenu menu)
        {
            try
            {
                menu.Activate();
                menu.Focus();
            }
            catch { }
        }
        
        #endregion

        #region Private Methods - Event Handlers
        
        private static void SetupMenuEventHandlers(MenuContext context, BeepContextMenu menu, FormStyle style, string theme)
        {
            string menuId = context.Id;
            
            context.ItemClickedHandler = (sender, e) =>
            {
                if (!HasChildren(e.Item))
                {
                    lock (_lock)
                    {
                        if (_activeMenus.TryGetValue(menuId, out var ctx))
                        {
                            ctx.SelectedItem = e.Item;
                            ctx.IsClosed = true;
                            CloseMenuHierarchy(menuId);
                        }
                    }
                }
            };
            
            context.SubmenuOpeningHandler = (sender, e) =>
            {
                if (e?.Item == null || !HasChildren(e.Item))
                    return;
                
                StopAutoCloseTimer();
                CloseChildMenus(menuId);
                
                var menuLocation = menu.PointToScreen(Point.Empty);
                var itemBounds = menu.LayoutHelper.GetItemRect(e.Item);
                var subMenuLocation = CalculateSubMenuPosition(menuLocation, itemBounds);
                
                try
                {
                    ShowChildMenu(e.Item.Children.ToList(), subMenuLocation, menu, style, theme, menuId);
                }
                catch
                {
                    // Silently handle sub-menu errors
                }
            };
            
            context.FormClosedHandler = (sender, e) =>
            {
                lock (_lock)
                {
                    if (_activeMenus.TryGetValue(menuId, out var ctx))
                    {
                        ctx.IsClosed = true;
                        ctx.CloseTcs?.TrySetResult(true);
                        CloseChildMenus(menuId);
                    }
                }
            };
            
            menu.ItemClicked += context.ItemClickedHandler;
            menu.SubmenuOpening += context.SubmenuOpeningHandler;
            menu.FormClosed += context.FormClosedHandler;
        }
        
        #endregion

        #region Private Methods - Mouse Tracking
        
        private static void SetupMouseTracking(MenuContext context)
        {
            if (context.Menu == null)
                return;
            
            var menu = context.Menu;
            string menuId = context.Id;
            
            context.MousePoller = new Timer { Interval = 50 };
            context.MousePoller.Tick += (s, e) =>
            {
                // Safety checks
                if (menu.IsDisposed || !menu.Visible)
                {
                    context.MousePoller?.Stop();
                    return;
                }
                
                lock (_lock)
                {
                    if (!_activeMenus.TryGetValue(menuId, out var ctx) || ctx.IsClosing)
                    {
                        context.MousePoller?.Stop();
                        return;
                    }
                }
                
                Point mousePos = Cursor.Position;
                bool isHovered = menu.Bounds.Contains(mousePos);
                
                if (context.WasHovered && !isHovered)
                {
                    // Mouse left this sub-menu
                    StartAutoCloseTimer();
                    context.WasHovered = false;
                }
                else if (!context.WasHovered && isHovered)
                {
                    // Mouse entered this sub-menu
                    StopAutoCloseTimer();
                    context.WasHovered = true;
                }
            };
            
            // Calculate initial hover state
            context.WasHovered = menu.Bounds.Contains(Cursor.Position);
            context.MousePoller.Start();
        }
        
        #endregion

        #region Private Methods - Auto Close Timer
        
        private static void StartAutoCloseTimer()
        {
            lock (_lock)
            {
                if (_isClosingAll || _autoCloseTimer != null)
                    return;
                
                _autoCloseTimer = new Timer { Interval = AUTO_CLOSE_DELAY_MS };
                _autoCloseTimer.Tick += (s, e) =>
                {
                    StopAutoCloseTimer();
                    
                    // Check if mouse is over any menu
                    lock (_lock)
                    {
                        foreach (var ctx in _activeMenus.Values)
                        {
                            if (ctx.Menu != null && !ctx.Menu.IsDisposed && ctx.Menu.Visible)
                            {
                                if (ctx.Menu.Bounds.Contains(Cursor.Position))
                                    return; // Mouse is over a menu, don't close
                            }
                        }
                    }
                    
                    // Close sub-menus only (not root)
                    CloseOrphanedSubMenus();
                };
                _autoCloseTimer.Start();
            }
        }
        
        private static void StopAutoCloseTimer()
        {
            lock (_lock)
            {
                if (_autoCloseTimer != null)
                {
                    _autoCloseTimer.Stop();
                    _autoCloseTimer.Dispose();
                    _autoCloseTimer = null;
                }
            }
        }
        
        private static void CloseOrphanedSubMenus()
        {
            lock (_lock)
            {
                var subMenus = _activeMenus.Values
                    .Where(c => !string.IsNullOrEmpty(c.ParentMenuId) && !c.IsClosing)
                    .Select(c => c.Id)
                    .ToList();
                
                foreach (var id in subMenus)
                {
                    CloseMenu(id);
                }
            }
        }
        
        #endregion

        #region Private Methods - Click Outside Filter
        
        private static void InstallClickOutsideFilter()
        {
            lock (_lock)
            {
                if (_clickOutsideFilter == null)
                {
                    _clickOutsideFilter = new ClickOutsideFilter();
                    Application.AddMessageFilter(_clickOutsideFilter);
                }
            }
        }
        
        private static void RemoveClickOutsideFilter()
        {
            lock (_lock)
            {
                if (_clickOutsideFilter != null)
                {
                    try
                    {
                        Application.RemoveMessageFilter(_clickOutsideFilter);
                    }
                    catch { }
                    _clickOutsideFilter = null;
                }
            }
        }
        
        #endregion

        #region Private Methods - Submenu Support
        
        private static string ShowChildMenu(
            List<SimpleItem> items,
            Point screenLocation,
            Control owner,
            FormStyle style,
            string theme,
            string parentMenuId)
        {
            if (items == null || items.Count == 0)
                return null;
            
            var context = CreateMenuContext(parentMenuId, false);
            if (context == null)
                return null;

            // Phase 01: child menus inherit the originating owner.
            context.Owner = owner;

            string menuId = context.Id;
            
            try
            {
                var menu = CreateMenu(style, false, theme);
                menu.DestroyOnClose = false; // Child menus are managed by parent
                
                PopulateMenu(menu, items);
                menu.RecalculateSize();
                
                lock (_lock)
                {
                    if (!_activeMenus.ContainsKey(menuId))
                        return null;
                    _activeMenus[menuId].Menu = menu;
                }
                
                SetupMenuEventHandlers(context, menu, style, theme);
                SetupMouseTracking(context);
                
                // Force handle creation before showing
                if (!menu.IsHandleCreated)
                {
                    var _ = menu.Handle;
                }

                // Phase 05 — clamp child-menu location to its monitor's
                // working area too, in case CalculateSubMenuPosition's
                // estimated size disagrees with the actual menu size.
                menu.Location = ClampToWorkingArea(menu.Size, screenLocation);
                menu.Show();
                menu.BringToFront();
                ActivateMenu(menu);

                // Phase 04B — Submenu triangle tracker integration.
                // Inform the PARENT BeepContextMenu about the child's
                // screen bounds so its UpdateHoveredItem can defer
                // dismissal while the cursor is drifting diagonally
                // toward this child. The owner of a child menu is the
                // parent BeepContextMenu form (see all four call sites).
                if (owner is BeepContextMenu parentMenu && !parentMenu.IsDisposed)
                {
                    try { parentMenu.NoteSubmenuOpened(menu.Bounds); }
                    catch { /* non-fatal */ }
                }

                return menuId;
            }
            catch
            {
                CleanupMenuContext(menuId);
                return null;
            }
        }
        
        private static Point CalculateSubMenuPosition(Point parentMenuLocation, Rectangle parentItemBounds)
        {
            int x = parentMenuLocation.X + parentItemBounds.Right - 5;
            int y = parentMenuLocation.Y + parentItemBounds.Top;
            
            var screen = Screen.FromPoint(new Point(x, y));
            var screenBounds = screen.WorkingArea;
            
            const int estimatedMenuWidth = 250;
            const int estimatedMenuHeight = 300;
            
            // Adjust if menu would go off-screen
            if (x + estimatedMenuWidth > screenBounds.Right)
            {
                x = parentMenuLocation.X - estimatedMenuWidth + 5;
                if (x < screenBounds.Left)
                    x = screenBounds.Left + 5;
            }
            
            if (y + estimatedMenuHeight > screenBounds.Bottom)
            {
                y = screenBounds.Bottom - estimatedMenuHeight - 5;
                if (y < screenBounds.Top)
                    y = screenBounds.Top + 5;
            }
            
            return new Point(x, y);
        }
        
        #endregion

        #region Private Methods - Menu Lifecycle
        
        private static void SafeCloseMenu(BeepContextMenu menu)
        {
            try
            {
                if (menu != null && !menu.IsDisposed)
                {
                    menu.Close();
                }
            }
            catch { }
        }
        
        private static void CloseRootMenus()
        {
            lock (_lock)
            {
                var rootMenuIds = _activeMenus
                    .Where(kvp => string.IsNullOrEmpty(kvp.Value.ParentMenuId))
                    .Select(kvp => kvp.Key)
                    .ToList();
                
                foreach (var id in rootMenuIds)
                {
                    CloseMenu(id);
                }
            }
            
            // Brief pause to allow close to complete
            System.Threading.Thread.Sleep(10);
        }
        
        private static void CloseChildMenus(string parentMenuId)
        {
            if (string.IsNullOrEmpty(parentMenuId))
                return;

            BeepContextMenu parentMenuForm = null;
            lock (_lock)
            {
                if (_activeMenus.TryGetValue(parentMenuId, out var parent))
                {
                    parentMenuForm = parent.Menu;
                    foreach (var childId in parent.ChildMenuIds.ToList())
                    {
                        CloseMenu(childId);
                    }
                }
            }

            // Phase 04B — Clear the parent's submenu-bounds tracker
            // outside the lock to avoid re-entry on Note* methods.
            if (parentMenuForm != null && !parentMenuForm.IsDisposed)
            {
                try { parentMenuForm.NoteSubmenuClosed(); }
                catch { /* non-fatal */ }
            }
        }
        
        private static void CloseMenuHierarchy(string menuId)
        {
            if (string.IsNullOrEmpty(menuId))
                return;
            
            // Find root menu
            string rootMenuId = menuId;
            lock (_lock)
            {
                while (!string.IsNullOrEmpty(rootMenuId) && _activeMenus.TryGetValue(rootMenuId, out var ctx))
                {
                    if (string.IsNullOrEmpty(ctx.ParentMenuId))
                        break;
                    
                    // Propagate selection to parent
                    if (_activeMenus.TryGetValue(ctx.ParentMenuId, out var parent))
                    {
                        parent.SelectedItem = ctx.SelectedItem;
                    }
                    
                    rootMenuId = ctx.ParentMenuId;
                }
            }
            
            // Close from root
            if (!string.IsNullOrEmpty(rootMenuId))
            {
                CloseMenu(rootMenuId);
            }
        }
        
        // ─────────────────────────────────────────────────────────────────
        // Phase 05 — ContextMenu Lifecycle Hardening
        //
        // Native message-pump helpers. The synchronous Show / ShowMultiSelect
        // paths used to spin with Application.DoEvents() + Thread.Sleep(1),
        // which pegged a CPU core and re-entered the UI message pump for
        // every iteration. PumpUntilClosed yields to the OS between
        // messages via MsgWaitForMultipleObjectsEx, mirroring the loop
        // Form.ShowDialog uses internally but without the modal disable.
        // ─────────────────────────────────────────────────────────────────

        private const uint QS_ALLINPUT = 0x04FF;
        private const uint MWMO_INPUTAVAILABLE = 0x0004;
        private const uint WAIT_TIMEOUT = 0x00000102;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern uint MsgWaitForMultipleObjectsEx(
            uint nCount,
            IntPtr pHandles,
            uint dwMilliseconds,
            uint dwWakeMask,
            uint dwFlags);

        /// <summary>
        /// Yields to the OS until the menu form closes. Replaces the legacy
        /// busy-wait spin (~1 ms tick) with a real blocking wait that wakes
        /// when a Windows message is posted to this thread's queue.
        ///
        /// The 50 ms safety timeout exists only to guard against missed
        /// programmatic close-completions in unusual hosts; normal
        /// dismissal generates WM_CLOSE which wakes the wait instantly.
        /// </summary>
        private static void PumpUntilClosed(BeepContextMenu menu, Func<bool> isClosed)
        {
            if (menu == null) return;

            while (true)
            {
                if (isClosed())                 break;
                if (menu.IsDisposed)            break;
                if (!menu.Visible)              break;

                Application.DoEvents();

                if (isClosed())                 break;
                if (menu.IsDisposed)            break;
                if (!menu.Visible)              break;

                MsgWaitForMultipleObjectsEx(
                    0, IntPtr.Zero,
                    50,                  // dwMilliseconds — short safety timeout
                    QS_ALLINPUT,
                    MWMO_INPUTAVAILABLE);
            }
        }

        /// <summary>
        /// Clamps <paramref name="requested"/> so a popup of
        /// <paramref name="menuSize"/> stays inside the working area of the
        /// monitor that contains the requested point. Standard fix for
        /// "popup spills off the right/bottom edge of a secondary monitor".
        /// </summary>
        private static Point ClampToWorkingArea(Size menuSize, Point requested)
        {
            try
            {
                var wa = Screen.FromPoint(requested).WorkingArea;
                int x = requested.X, y = requested.Y;

                if (menuSize.Width  > 0 && x + menuSize.Width  > wa.Right)  x = wa.Right  - menuSize.Width;
                if (menuSize.Height > 0 && y + menuSize.Height > wa.Bottom) y = wa.Bottom - menuSize.Height;

                if (x < wa.Left) x = wa.Left;
                if (y < wa.Top)  y = wa.Top;

                return new Point(x, y);
            }
            catch
            {
                return requested; // Defensive fallback — never block the show path.
            }
        }

        private static void CleanupMenuContext(string menuId)
        {
            if (string.IsNullOrEmpty(menuId))
                return;
            
            MenuContext context;
            lock (_lock)
            {
                if (!_activeMenus.TryGetValue(menuId, out context))
                    return;
                
                // Remove from parent's child list
                if (!string.IsNullOrEmpty(context.ParentMenuId) && 
                    _activeMenus.TryGetValue(context.ParentMenuId, out var parent))
                {
                    parent.ChildMenuIds.Remove(menuId);
                }
                
                _activeMenus.Remove(menuId);
                
                // Remove filter if no more menus
                if (_activeMenus.Count == 0)
                {
                    RemoveClickOutsideFilter();
                }
            }

            // Phase 01: notify subscribers BEFORE disposing so they can read
            // context.Menu.LastCloseReason while the menu instance is still alive.
            RaiseMenuDismissed(context);

            // Dispose context (cleanup timers, event handlers)
            context?.Dispose();
            
            // Dispose menu if it's a child menu
            try
            {
                if (context?.Menu != null && !context.Menu.IsDisposed && 
                    !string.IsNullOrEmpty(context.ParentMenuId))
                {
                    context.Menu.Dispose();
                }
            }
            catch { }
        }

        // ─────────────────────────────────────────────────────────────────
        // Phase 01 — Dismissal/Re-open Hot-Fix
        //
        // Centralised, single-fire raiser for the MenuDismissed event.
        // Guards against re-firing for the same context (CleanupMenuContext
        // and CloseAllMenus can both touch the same context in rapid
        // succession during a "close all" cascade).
        //
        // Wrapped in try/catch so a buggy subscriber cannot leak into the
        // cleanup path.
        // ─────────────────────────────────────────────────────────────────
        private static void RaiseMenuDismissed(MenuContext context)
        {
            if (context == null || context.DismissedFired) return;
            context.DismissedFired = true;

            var handler = MenuDismissed;
            if (handler == null) return;

            try
            {
                var reason = context.Menu != null && !context.Menu.IsDisposed
                    ? context.Menu.LastCloseReason
                    : BeepContextMenuCloseReason.CloseCalled;

                Point screenPt;
                try { screenPt = Cursor.Position; }
                catch { screenPt = Point.Empty; }

                handler(null, new MenuDismissedEventArgs(context.Owner, reason, screenPt));
            }
            catch
            {
                // Never let a subscriber exception leak into the close path.
            }
        }

        #endregion
    }
}

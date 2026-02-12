using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
                    
                    // Click is outside all menus - close them
                    CloseAllMenus();
                }
                catch
                {
                    // Silently handle click outside filter errors
                }
                
                return false; // Don't consume the message
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
            
            // Prevent showing during close operation
            lock (_lock)
            {
                if (_isClosingAll)
                    return null;
            }
            
            // Close existing root menus if this is a root menu
            if (string.IsNullOrEmpty(parentMenuId))
            {
                CloseRootMenus();
            }
            
            // Create menu context
            var context = CreateMenuContext(parentMenuId, multiSelect);
            if (context == null)
                return null;
            
            string menuId = context.Id;
            
            try
            {
                // Create and configure the menu
                var menu = CreateMenu(style, multiSelect, theme);
                PopulateMenu(menu, items);
                
                // Store menu in context
                lock (_lock)
                {
                    if (!_activeMenus.ContainsKey(menuId))
                        return null;
                    _activeMenus[menuId].Menu = menu;
                }
                
                // Setup event handlers
                SetupMenuEventHandlers(context, menu, style, theme);
                
                // Setup mouse tracking for sub-menus only
                if (!string.IsNullOrEmpty(parentMenuId))
                {
                    SetupMouseTracking(context);
                }
                
                // Install click-outside filter for root menus
                if (string.IsNullOrEmpty(parentMenuId))
                {
                    InstallClickOutsideFilter();
                }
                // Show the menu
                menu.Show(screenLocation, owner);
                ActivateMenu(menu);
                
                // Wait for menu to close using message pumping
                WaitForMenuClose(menuId);
                
                // Return selected item
                lock (_lock)
                {
                    if (_activeMenus.TryGetValue(menuId, out var ctx))
                    {
                        return ctx.SelectedItem;
                    }
                }
                return null;
            }
            finally
            {
                CleanupMenuContext(menuId);
            }
        }
        
        /// <summary>
        /// Shows a multi-select context menu and returns all selected items.
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
            
            // Use Show with multiSelect=true
            var result = Show(items, screenLocation, owner, style, true, theme, null);
            
            // For multi-select, we need to get the selected items list
            // The result will be null but selected items are tracked in context
            lock (_lock)
            {
                // Find the most recently closed multi-select menu
                foreach (var ctx in _activeMenus.Values.Where(c => c.IsMultiSelect && c.IsClosed))
                {
                    return ctx.SelectedItems ?? new List<SimpleItem>();
                }
            }
            
            return new List<SimpleItem>();
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
                lock (_lock)
                {
                    menuIds = _activeMenus.Keys.ToList();
                }
                
                foreach (var id in menuIds)
                {
                    try
                    {
                        CloseMenu(id);
                    }
                    catch { }
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
                
                menu.Location = screenLocation;
                menu.Show();
                menu.BringToFront();
                ActivateMenu(menu);
                
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
        
        private static void WaitForMenuClose(string menuId)
        {
            int iterationCount = 0;
            const int maxIterations = 60000; // ~10 minutes max wait
            
            while (iterationCount < maxIterations)
            {
                bool isClosed;
                lock (_lock)
                {
                    if (!_activeMenus.TryGetValue(menuId, out var ctx))
                    {
                        break;
                    }
                    isClosed = ctx.IsClosed || ctx.Menu == null || ctx.Menu.IsDisposed;
                }
                
                if (isClosed)
                    break;
                
                // Process messages without blocking
                Application.DoEvents();
                System.Threading.Thread.Sleep(10);
                iterationCount++;
            }
        }
        
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
            
            lock (_lock)
            {
                if (_activeMenus.TryGetValue(parentMenuId, out var parent))
                {
                    foreach (var childId in parent.ChildMenuIds.ToList())
                    {
                        CloseMenu(childId);
                    }
                }
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
        
        #endregion
    }
}

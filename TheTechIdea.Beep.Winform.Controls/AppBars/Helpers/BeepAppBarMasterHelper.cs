using System;
using System.ComponentModel;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.AppBars.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.Helpers
{
    /// <summary>
    /// Main coordinator helper that manages all BeepAppBar functionality
    /// This is the single entry point that orchestrates all other helpers
    /// </summary>
    internal class BeepAppBarMasterHelper : IDisposable
    {
        #region "Helper Instances"

        public BeepAppBarStateStore StateStore { get; private set; }
        public BeepAppBarLayoutHelper Layout { get; private set; }
        public BeepAppBarComponentFactory ComponentFactory { get; private set; }
        public BeepAppBarDrawingHelper Drawing { get; private set; }
        public BeepAppBarEventHelper Events { get; private set; }
        public BeepAppBarDragHelper Drag { get; private set; }

        #endregion

        #region "Fields"

        private readonly IBeepAppBarHost _host;
        private bool _disposed = false;

        #endregion

        #region "Constructor"

        public BeepAppBarMasterHelper(IBeepAppBarHost host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));

            InitializeHelpers();
            WireUpEvents();
        }

        #endregion

        #region "Initialization"

        private void InitializeHelpers()
        {
            // Initialize in dependency order
            StateStore = new BeepAppBarStateStore();
            Layout = new BeepAppBarLayoutHelper(_host, StateStore);
            ComponentFactory = new BeepAppBarComponentFactory(_host);
            Drawing = new BeepAppBarDrawingHelper(_host, StateStore, Layout, ComponentFactory);
            Events = new BeepAppBarEventHelper(_host, StateStore, Layout, ComponentFactory);
            Drag = new BeepAppBarDragHelper(_host, StateStore, Layout);

            // Configure drag helper with default settings
            Drag.SetDraggableAreas("Title", "Logo");
            Drag.SetFormDraggingEnabled(true);
        }

        private void WireUpEvents()
        {
            // No cross-helper communication needed at this time
            // Events are exposed directly through the Events helper
        }

        #endregion

        #region "Main Coordination Methods"

        /// <summary>
        /// Main drawing coordination method
        /// </summary>
        public void DrawAll(Graphics g)
        {
            Drawing.DrawAll(g);
        }

        /// <summary>
        /// Handles all mouse clicks
        /// </summary>
        public void HandleMouseClick(MouseEventArgs e)
        {
            Events.HandleMouseClick(e);
        }

        /// <summary>
        /// Handles mouse move events and returns appropriate cursor
        /// </summary>
        public Cursor HandleMouseMove(MouseEventArgs e)
        {
            // Check for drag cursor first
            if (Drag.IsInDraggableArea(e.Location) && !Layout.IsInteractiveArea(e.Location))
            {
                return Cursors.SizeAll;
            }

            // Let event helper handle hover states and return cursor
            return Events.HandleMouseMove(e);
        }

        /// <summary>
        /// Handles mouse down events (for dragging)
        /// </summary>
        public void HandleMouseDown(MouseEventArgs e)
        {
            // Try to start drag first
            bool dragStarted = Drag.HandleMouseDown(e);
            
            // If drag wasn't started, don't process other click logic
            if (!dragStarted)
            {
                // Mouse down on interactive elements is handled in click events
            }
        }

        /// <summary>
        /// Handles mouse leave events
        /// </summary>
        public void HandleMouseLeave()
        {
            Events.HandleMouseLeave();
        }

        /// <summary>
        /// Handles resize events
        /// </summary>
        public void HandleResize()
        {
            // Mark layout as dirty
            Layout.InvalidateLayout();
            
            // Remove search box during resize to avoid layout conflicts
            Events.RemoveSearchBoxControlSafe();
            
            // Update component sizes for DPI changes
            ComponentFactory.UpdateComponentSizes();
            
            // Force redraw
            _host.Invalidate();
        }

        /// <summary>
        /// Applies theme to all components
        /// </summary>
        public void ApplyTheme()
        {
            ComponentFactory.ApplyTheme();
            _host.Invalidate();
        }

        /// <summary>
        /// Updates component when properties change
        /// </summary>
        public void HandlePropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(IBeepAppBarHost.Title):
                    ComponentFactory.UpdateTitle(_host.Title);
                    break;
                case nameof(IBeepAppBarHost.LogoImage):
                    ComponentFactory.UpdateLogo(_host.LogoImage);
                    break;
                case nameof(IBeepAppBarHost.ShowTitle):
                case nameof(IBeepAppBarHost.ShowLogo):
                case nameof(IBeepAppBarHost.ShowSearchBox):
                case nameof(IBeepAppBarHost.ShowProfileIcon):
                case nameof(IBeepAppBarHost.ShowNotificationIcon):
                case nameof(IBeepAppBarHost.ShowThemeIcon):
                case nameof(IBeepAppBarHost.ShowCloseIcon):
                case nameof(IBeepAppBarHost.ShowMaximizeIcon):
                case nameof(IBeepAppBarHost.ShowMinimizeIcon):
                    Layout.InvalidateLayout();
                    break;
            }
            _host.Invalidate();
        }

        #endregion

        #region "Public Access Methods"

        /// <summary>
        /// Gets layout rectangles for external use
        /// </summary>
        public void GetLayoutRectangles(out Rectangle logo, out Rectangle title, out Rectangle search,
            out Rectangle notification, out Rectangle profile, out Rectangle theme,
            out Rectangle minimize, out Rectangle maximize, out Rectangle close)
        {
            Layout.EnsureLayout();
            logo = Layout.GetLogoRect();
            title = Layout.GetTitleRect();
            search = Layout.GetSearchRect();
            notification = Layout.GetNotificationRect();
            profile = Layout.GetProfileRect();
            theme = Layout.GetThemeRect();
            minimize = Layout.GetMinimizeRect();
            maximize = Layout.GetMaximizeRect();
            close = Layout.GetCloseRect();
        }

        /// <summary>
        /// Sets draggable areas
        /// </summary>
        public void SetDraggableAreas(params string[] areas)
        {
            Drag.SetDraggableAreas(areas);
        }

        /// <summary>
        /// Enables/disables form dragging
        /// </summary>
        public void SetFormDraggingEnabled(bool enabled)
        {
            Drag.SetFormDraggingEnabled(enabled);
        }

        /// <summary>
        /// Shows badge on notification icon
        /// </summary>
        public void ShowBadgeOnNotificationIcon(string badgeText)
        {
            if (ComponentFactory.NotificationButton != null)
            {
                ComponentFactory.NotificationButton.BadgeText = badgeText;
                _host.Invalidate();
            }
        }

        /// <summary>
        /// Forces layout recalculation
        /// </summary>
        public void RefreshLayout()
        {
            Layout.InvalidateLayout();
            _host.Invalidate();
        }

        #endregion

        #region "Event Exposure"

        /// <summary>
        /// Exposes the Clicked event from the Events helper
        /// </summary>
        public event EventHandler<BeepMouseEventArgs> Clicked
        {
            add => Events.Clicked += value;
            remove => Events.Clicked -= value;
        }

        /// <summary>
        /// Exposes the OnButtonClicked event from the Events helper
        /// </summary>
        public event EventHandler<Models.BeepAppBarEventsArgs> OnButtonClicked
        {
            add => Events.OnButtonClicked += value;
            remove => Events.OnButtonClicked -= value;
        }

        /// <summary>
        /// Exposes the OnSearchBoxSelected event from the Events helper
        /// </summary>
        public event EventHandler<Models.BeepAppBarEventsArgs> OnSearchBoxSelected
        {
            add => Events.OnSearchBoxSelected += value;
            remove => Events.OnSearchBoxSelected -= value;
        }

        #endregion

        #region "IDisposable Implementation"

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                // Dispose all helpers
                ComponentFactory?.Dispose();
                
                // Clear references
                StateStore = null;
                Layout = null;
                ComponentFactory = null;
                Drawing = null;
                Events = null;
                Drag = null;

                _disposed = true;
            }
        }

        #endregion
    }
}
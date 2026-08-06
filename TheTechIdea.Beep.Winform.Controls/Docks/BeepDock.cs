using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Docks;
using TheTechIdea.Beep.Winform.Controls.Docks.Painters;
using TheTechIdea.Beep.Winform.Controls.Docks.Helpers;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Modern dock control with painter-based rendering and smooth animations
    /// Main class - Fields and Constructor
    /// </summary>
    [ToolboxItem(true)]
    [DisplayName("Beep Dock")]
    [Category("Beep Controls")]
    [Description("Enhanced docking control with painter-based rendering")]
    public partial class BeepDock : BaseControl
    {
        protected override Size DefaultSize => BeepLayoutMetrics.Dock;
        #region Fields
        private readonly BindingList<SimpleItem> _items;
        private readonly List<Docks.DockItemState> _itemStates;
        private readonly DockConfig _config;
        private IDockPainter _dockPainter;
        private Timer _animationTimer;
        private readonly Timer _hoverIntentTimer;
        private BeepDockTooltip? _activeTooltip;
        
        private SimpleItem? _selectedItem;
        private int _selectedIndex = -1;
        private int _hoveredIndex = -1;
        private int _pressedIndex = -1;
        private int _overflowStartIndex = -1;
        private Rectangle _overflowBounds = Rectangle.Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of BeepDock
        /// </summary>
        public BeepDock()
        {
            // Initialize collections
            _items = new BindingList<SimpleItem>();
            _itemStates = new List<Docks.DockItemState>();
            
            // Initialize configuration. The dimensions are deliberately not set here: leaving them
            // unset is what lets them follow the style, and assigning the AppleDock numbers would
            // have marked them as user-chosen and frozen them against every later style change.
            _config = new DockConfig
            {
                Style = Docks.DockStyle.AppleDock,
                Position = Docks.DockPosition.Bottom,
                Orientation = Docks.DockOrientation.Horizontal,
                ShowBackground = true,
                AnimationSpeed = 0.2f,
                ShowBadges = true,
                ShowTooltips = true,
                ShowRunningIndicator = true,
                EnableContextMenu = true
            };

            // Projections of _config, refreshed by every path that writes it.
            SyncProfiles();

            // Initialize painter
            _dockPainter = DockPainterFactory.GetPainter(_config.Style);

            // Initialize animation timer
            _animationTimer = new Timer { Interval = 16 }; // ~60 FPS
            _animationTimer.Tick += AnimationTimer_Tick!;
            _animationTimer.Start();
            _hoverIntentTimer = new Timer { Interval = _config.HoverEnterDelay };
            _hoverIntentTimer.Tick += HoverIntentTimer_Tick;

            // Configure BaseControl properties
            DoubleBuffered = true;
            IsChild = true;
            IsFrameless = true;
            ShowAllBorders = false;
            IsBorderAffectedByTheme = false;

            // Enable keyboard navigation
            TabStop = true;
            KeyboardNavigationEnabled = true;

            // Initialize drag-drop support
            InitializeDragDrop();
            InitializeAutoHide();

            // Hook up events
            _items.ListChanged += Items_ListChanged!;

            // Initial size
            UpdateDockSize();
        }
        #endregion

        #region Cleanup
        /// <summary>
        /// Disposes the dock control
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animationTimer?.Stop();
                _animationTimer?.Dispose();
                _hoverIntentTimer?.Stop();
                _hoverIntentTimer?.Dispose();
                DisposeAutoHide();
                _activeTooltip?.Dispose();
                if (_items != null)
                    _items.ListChanged -= Items_ListChanged;
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}

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
        #region Fields
        private readonly BindingList<SimpleItem> _items;
        private readonly List<Docks.DockItemState> _itemStates;
        private readonly DockConfig _config;
        private IDockPainter _dockPainter;
        private Timer _animationTimer;
        
        private SimpleItem? _selectedItem;
        private int _selectedIndex = -1;
        private int _hoveredIndex = -1;
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
            
            // Initialize configuration
            _config = new DockConfig
            {
                Style = Docks.DockStyle.AppleDock,
                ItemSize = 56,
                DockHeight = 72,
                Spacing = 8,
                Padding = 12,
                MaxScale = 1.5f,
                Position = Docks.DockPosition.Bottom,
                Orientation = Docks.DockOrientation.Horizontal,
                ShowBackground = true,
                ShowShadow = true,
                BackgroundOpacity = 0.85f,
                AnimationSpeed = 0.2f,
                ShowBadges = true,
                ShowTooltips = true,
                ShowRunningIndicator = true,
                EnableContextMenu = true
            };

            // Initialize painter
            _dockPainter = DockPainterFactory.GetPainter(_config.Style);

            // Initialize animation timer
            _animationTimer = new Timer { Interval = 16 }; // ~60 FPS
            _animationTimer.Tick += AnimationTimer_Tick!;
            _animationTimer.Start();

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
                if (_items != null)
                    _items.ListChanged -= Items_ListChanged;
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}

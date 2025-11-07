using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.DisplayContainers.Helpers;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Styling;


namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Display Container 2")]
    [Description("A modern, self-contained display container for addins with native rendering and advanced features.")]
    public partial class BeepDisplayContainer2 : BaseControl, IDisplayContainer
    {
        #region Fields

        private readonly List<AddinTab> _tabs = new();
        private readonly Dictionary<string, IDM_Addin> _addins = new();
        private AddinTab _activeTab;
        private AddinTab _hoveredTab;
        private ContainerTypeEnum _containerType = ContainerTypeEnum.TabbedPanel;
        private ContainerDisplayMode _displayMode = ContainerDisplayMode.Tabbed;
        private TabPosition _tabPosition = TabPosition.Top;
        private bool _showCloseButtons = true;
        private bool _allowTabReordering = true;
        private bool _enableAnimations = true;
        private AnimationSpeed _animationSpeed = AnimationSpeed.Normal;
        private System.Windows.Forms.Timer _animationTimer;
        private Rectangle _tabArea;
        private Rectangle _contentArea;
        private int _tabHeight = 36;
        private int _tabMinWidth = 80;
        private int _tabMaxWidth = 200;
        private int _scrollOffset = 0;
        private bool _needsScrolling = false;
        private Rectangle _scrollLeftButton;
        private Rectangle _scrollRightButton;
        private Rectangle _newTabButton;

        // Helper classes
        private TabPaintHelper _paintHelper;
        private TabLayoutHelper _layoutHelper;
        private TabAnimationHelper _animationHelper;

        // Theme colors
        private Color _tabBackColor;
        private Color _tabForeColor;
        private Color _activeTabBackColor;
        private Color _activeTabForeColor;
        private Color _hoverTabBackColor;
        private Color _borderColor;
        private Color _contentBackColor;

        // Tab transition animation
        private AddinTab _previousTab;
        
        // Batch update mode to prevent flickering when adding multiple tabs
        private bool _batchMode = false;
        private int _batchUpdateDepth = 0;

        #endregion

        #region IDisplayContainer Events

        public event EventHandler<ContainerEvents> AddinAdded;
        public event EventHandler<ContainerEvents> AddinRemoved;
        public event EventHandler<ContainerEvents> AddinMoved;
        public event EventHandler<ContainerEvents> AddinChanging;
        public event EventHandler<ContainerEvents> AddinChanged;
        public event EventHandler<IPassedArgs> PreCallModule;
        public event EventHandler<IPassedArgs> PreShowItem;
        public event EventHandler<KeyCombination> KeyPressed;

        #endregion

        #region Constructor

        public BeepDisplayContainer2():base()
        {
            InitializeComponent();
            InitializeContainer();
        }

        private void InitializeComponent()
        {
            // Follow BaseControl patterns for painting setup
            SetStyle(ControlStyles.SupportsTransparentBackColor | 
                     ControlStyles.ResizeRedraw | 
                     ControlStyles.UserPaint | 
                     ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer, true);
            
            DoubleBuffered = true;
            UseExternalBufferedGraphics = true;
            
            // Enable high-quality rendering like BaseControl
            EnableHighQualityRendering = true;
            
            // Use form style paint for modern appearance (like BeepMenuBar)
            UseFormStylePaint = false; // Container paints its own background, tabs use BeepStyling
            
            // Set modern defaults for better appearance
            IsRounded = true;
            BorderRadius = 8;
            BorderThickness = 1;
            
            // Set default ControlStyle for tabs (can be overridden by user)
            if (ControlStyle == BeepControlStyle.None)
            {
                ControlStyle = BeepControlStyle.Modern;
            }
            
            // Transparent background support (like BeepMenuBar)
            IsTransparentBackground = false; // Default to opaque for containers
            
            // Set initial BackColor from theme
            //if (IsTransparentBackground)
            //{
            //    base.BackColor = Color.Transparent;
            //}
            //else
            //{
            //    base.BackColor = Color.White;
            //}

            // Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            // Ensure control is visible and will paint
            Visible = true;
        }

        private void InitializeContainer()
        {
            // Get control style from FormStyle for modern appearance
            var controlStyle = ControlStyle;
            
            // Initialize helpers with modern styling
            _paintHelper = new TabPaintHelper(_currentTheme, controlStyle, IsTransparentBackground);
            _layoutHelper = new TabLayoutHelper();
            _animationHelper = new TabAnimationHelper(() => Invalidate());
            
            _animationTimer = new System.Windows.Forms.Timer { Interval = 16 }; // 60 FPS
            _animationTimer.Tick += AnimationTimer_Tick;
            
            // Start animation timer if animations are enabled
            if (_enableAnimations)
            {
                _animationTimer.Start();
            }

            // Initialize theme colors (follows BaseControl pattern)
            ApplyTheme();

            // Calculate initial layout
            RecalculateLayout();
            
            // Hook into handle creation to force initial paint (like BaseControl)
            HandleCreated += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"[BeepDisplayContainer2] HandleCreated - forcing initial paint");
                Invalidate(true);
            };
            
            // Hook into visible changed to force paint when control becomes visible
            VisibleChanged += (s, e) =>
            {
                if (Visible)
                {
                    System.Diagnostics.Debug.WriteLine($"[BeepDisplayContainer2] VisibleChanged to true - forcing paint");
                    Invalidate(true);
                }
            };
            
            // Subscribe to ControlStyle changes to update tab appearance
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ControlStyle))
                {
                    UpdateTabPainterStyle();
                    Invalidate();
                }
            };
        }
        
        /// <summary>
        /// Updates the tab painter and layout when ControlStyle changes
        /// </summary>
        private void UpdateTabPainterStyle()
        {
            if (_paintHelper != null)
            {
                _paintHelper.ControlStyle = ControlStyle;
                _paintHelper.IsTransparent = IsTransparentBackground;
            }
            
            // Update layout helper with new style and font for proper tab sizing
            if (_layoutHelper != null)
            {
                _layoutHelper.UpdateStyle(ControlStyle, Font);
            }
            
            // Recalculate layout with new metrics
            RecalculateLayout();
        }

        #endregion

        #region Helper Methods

        private AddinTab GetTabAt(Point point)
        {
            return _layoutHelper?.GetTabAt(_tabs, point);
        }

        private Rectangle GetCloseButtonRect(Rectangle tabBounds)
        {
            return _layoutHelper?.GetCloseButtonRect(tabBounds) ?? Rectangle.Empty;
        }

        private void StartAnimation(AddinTab tab, float targetProgress)
        {
            if (!_enableAnimations || _animationHelper == null) return;
            _animationHelper.StartAnimation(tab, targetProgress);
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Only handle hover animations, not transitions (to prevent crashes)
            if (_animationHelper != null)
            {
                // Update hover animations only
                _animationHelper.UpdateAnimations(_tabs, _animationSpeed);
            }
        }
        
        /// <summary>
        /// Begin batch mode to add multiple tabs without flickering.
        /// Call EndUpdate() when done to trigger a single repaint.
        /// </summary>
        public void BeginUpdate()
        {
            _batchUpdateDepth++;
            if (_batchUpdateDepth == 1)
            {
                _batchMode = true;
                // Suspend layout to prevent multiple recalculations
                SuspendLayout();
            }
        }
        
        /// <summary>
        /// End batch mode and trigger a single repaint for all changes.
        /// </summary>
        public void EndUpdate()
        {
            _batchUpdateDepth = Math.Max(0, _batchUpdateDepth - 1);
            if (_batchUpdateDepth == 0)
            {
                _batchMode = false;
                // Resume layout and perform a single recalculation
                ResumeLayout(true);
                // Single invalidation for all changes
                if (IsHandleCreated)
                {
                    Invalidate();
                }
            }
        }

        #endregion
    }
}

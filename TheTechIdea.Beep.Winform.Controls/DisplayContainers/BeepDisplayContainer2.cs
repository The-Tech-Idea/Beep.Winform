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
        private AddinTab? _activeTab;
        private AddinTab? _hoveredTab;
        private ContainerTypeEnum _containerType = ContainerTypeEnum.TabbedPanel;
        private ContainerDisplayMode _displayMode = ContainerDisplayMode.Tabbed;
        private TabPosition _tabPosition = TabPosition.Top;
        private bool _showCloseButtons = true;
        private bool _allowTabReordering = true;
        private bool _enableAnimations = true;
        private AnimationSpeed _animationSpeed = AnimationSpeed.Normal;
        private System.Windows.Forms.Timer? _animationTimer;
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
        private TabPaintHelper? _paintHelper;
        private TabLayoutHelper? _layoutHelper;
        private TabAnimationHelper? _animationHelper;

        // Theme colors
        private Color _tabBackColor;
        private Color _tabForeColor;
        private Color _activeTabBackColor;
        private Color _activeTabForeColor;
        private Color _hoverTabBackColor;
        private Color _borderColor;
        private Color _contentBackColor;

        // Tab transition animation
        private AddinTab? _previousTab;
        
        // Batch update mode to prevent flickering when adding multiple tabs
        private bool _batchMode = false;
        private int _batchUpdateDepth = 0;

        private BeepControlStyle _lastControlStyle = BeepControlStyle.None;

        /// <summary>
        /// Gets or sets the visual style of the control.
        /// Shadows the base property to trigger tab style updates.
        /// </summary>
        public new BeepControlStyle ControlStyle
        {
            get => base.ControlStyle;
            set
            {
                base.ControlStyle = value;
                if (_lastControlStyle != value)
                {
                    UpdateTabStyleFromControlStyle();
                    _lastControlStyle = value;
                    ApplyThemeColorsToTabs();
                }
            }
        }

        /// <summary>
        /// Occurs when an addin is added to the container.
        /// </summary>
        public event EventHandler<ContainerEvents>? AddinAdded;
        /// <summary>
        /// Occurs when an addin is removed from the container.
        /// </summary>
        public event EventHandler<ContainerEvents>? AddinRemoved;
        /// <summary>
        /// Occurs when an addin is moved within the container.
        /// </summary>
        public event EventHandler<ContainerEvents>? AddinMoved;
        /// <summary>
        /// Occurs when the active addin is changing.
        /// </summary>
        public event EventHandler<ContainerEvents>? AddinChanging;
        /// <summary>
        /// Occurs when the active addin has changed.
        /// </summary>
        public event EventHandler<ContainerEvents>? AddinChanged;
        /// <summary>
        /// Occurs before a module is called.
        /// </summary>
        public event EventHandler<IPassedArgs>? PreCallModule;
        /// <summary>
        /// Occurs before an item is shown.
        /// </summary>
        public event EventHandler<IPassedArgs>? PreShowItem;
        /// <summary>
        /// Occurs when a key combination is pressed.
        /// </summary>
        public event EventHandler<KeyCombination>? KeyPressed;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BeepDisplayContainer2"/> class.
        /// </summary>
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
            ApplyThemeColorsToTabs();

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

        private void UpdateTabStyleFromControlStyle()
        {
            switch (ControlStyle)
            {
                case BeepControlStyle.FigmaCard:
                case BeepControlStyle.TailwindCard:
                case BeepControlStyle.DiscordStyle:
                case BeepControlStyle.StripeDashboard:
                case BeepControlStyle.NeoBrutalist:
                    TabStyle = TabStyle.Card;
                    break;
                case BeepControlStyle.Minimal:
                case BeepControlStyle.NotionMinimal:
                case BeepControlStyle.VercelClean:
                    TabStyle = TabStyle.Minimal;
                    break;
                case BeepControlStyle.GlassAcrylic:
                case BeepControlStyle.Windows11Mica:
                case BeepControlStyle.Fluent2:
                case BeepControlStyle.Material3:
                    TabStyle = TabStyle.Underline;
                    break;
                case BeepControlStyle.PillRail:
                case BeepControlStyle.ChakraUI:
                    TabStyle = TabStyle.Capsule;
                    break;
                case BeepControlStyle.iOS15:
                case BeepControlStyle.MacOSBigSur:
                    TabStyle = TabStyle.Segmented;
                    break;
                case BeepControlStyle.Bootstrap:
                case BeepControlStyle.AntDesign:
                case BeepControlStyle.Neumorphism:
                    TabStyle = TabStyle.Button;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Applies the current theme to the container and tabs.
        /// Follows BaseControl pattern for consistent theme application.
        /// </summary>
        public override void ApplyTheme()
        {
            // Call base.ApplyTheme() first for proper theme initialization (like BaseControl)
            // This sets up _currentTheme and applies basic theme properties
            base.ApplyTheme();

            // Update tab painter with current theme and style
            var controlStyle = ControlStyle;
            if (_paintHelper == null)
            {
                _paintHelper = new TabPaintHelper(_currentTheme, controlStyle, IsTransparentBackground);
            }
            else
            {
                _paintHelper.ControlStyle = controlStyle;
                _paintHelper.IsTransparent = IsTransparentBackground;
            }
            // Ensure paint helper uses the selected tab style
            _paintHelper.TabStyle = this.TabStyle;

            if (_currentTheme != null)
            {
                // Apply theme colors to tabs (follows BaseControl pattern)
                ApplyThemeColorsToTabs();

                // Apply theme font if UseThemeFont is enabled (like BaseControl)
                if (UseThemeFont && _currentTheme.LabelFont != null)
                {
                    try
                    {
                        TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.LabelFont);

                        // Update layout helper with new font for proper tab sizing
                        if (_layoutHelper != null)
                        {
                            _layoutHelper.UpdateStyle(ControlStyle, Font);
                        }

                        // Recalculate layout with new font metrics
                        RecalculateLayout();
                    }
                    catch
                    {
                        // Keep existing font on error
                    }
                }

                // Set background color based on theme and transparency setting
                if (IsTransparentBackground)
                {
                    base.BackColor = Color.Transparent;
                }
                else if (IsChild && Parent != null)
                {
                    // Follow parent background when IsChild is true (like BaseControl)
                    base.BackColor = Parent.BackColor;
                }
                else
                {
                    base.BackColor = _contentBackColor;
                }
            }
            else
            {
                // Fallback colors with modern defaults
                ApplyFallbackColors();
            }

            // Invalidate to trigger repaint with new theme
            Invalidate();
        }
        #endregion

        #region Helper Methods

        private AddinTab? GetTabAt(Point point)
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

        private void AnimationTimer_Tick(object? sender, EventArgs e)
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

        /// <summary>
        /// Apply the style preset to this container
        /// </summary>
        public void SetTabStylePreset(TheTechIdea.Beep.Winform.Controls.TabStyle style)
        {
            TheTechIdea.Beep.Winform.Controls.Styling.TabStylePresets.ApplyPreset(this, style);
        }

        #endregion
    }
}

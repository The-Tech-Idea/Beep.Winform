using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.SideBar
{
    public partial class BeepSideBar
    {
        #region Painter Fields
        private ISideBarPainter _currentPainter;
        private BeepControlStyle _style = BeepControlStyle.Material3;
        private SideBarPainterContextAdapter _painterContext;
        
        // New fields for enhanced features
        internal SimpleItem _pressedItem;
        internal float _hoverAnimationProgress = 0f;
        internal float _selectionAnimationProgress = 1f;
        internal Dictionary<SimpleItem, float> _accordionAnimationProgress = new Dictionary<SimpleItem, float>();
        internal Dictionary<SimpleItem, string> _itemBadges = new Dictionary<SimpleItem, string>();
        internal Dictionary<SimpleItem, Color> _itemBadgeColors = new Dictionary<SimpleItem, Color>();
        internal List<(int BeforeIndex, string HeaderText)> _sectionHeaders = new List<(int, string)>();
        internal List<int> _dividerPositions = new List<int>();
        #endregion

        #region Painter Properties
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual Style/painter to use for rendering the sidebar.")]
        [DefaultValue(BeepControlStyle.Material3)]
        public BeepControlStyle Style
        {
            get => _style;
            set
            {
                if (_style != value)
                {
                    // Dispose old painter if it implements IDisposable
                    try { _currentPainter?.Dispose(); } catch { }
                    
                    _style = value;
                    
                    // Only initialize painter at runtime, not design-time
                    if (!DesignMode && IsHandleCreated)
                    {
                        InitializePainter();
                    }
                    
                    Invalidate();
                }
            }
        }
        #endregion

        #region Badge and Section Methods
        /// <summary>
        /// Sets a badge on a menu item (e.g., notification count)
        /// </summary>
        public void SetItemBadge(SimpleItem item, string badgeText, Color? badgeColor = null)
        {
            if (item == null) return;
            
            if (string.IsNullOrEmpty(badgeText))
            {
                _itemBadges.Remove(item);
                _itemBadgeColors.Remove(item);
            }
            else
            {
                _itemBadges[item] = badgeText;
                _itemBadgeColors[item] = badgeColor ?? Color.Red;
            }
            Invalidate();
        }

        /// <summary>
        /// Clears all badges
        /// </summary>
        public void ClearAllBadges()
        {
            _itemBadges.Clear();
            _itemBadgeColors.Clear();
            Invalidate();
        }

        /// <summary>
        /// Adds a section header before a specific item index
        /// </summary>
        public void AddSectionHeader(int beforeIndex, string headerText)
        {
            _sectionHeaders.Add((beforeIndex, headerText));
            _sectionHeaders.Sort((a, b) => a.BeforeIndex.CompareTo(b.BeforeIndex));
            Invalidate();
        }

        /// <summary>
        /// Adds a divider after a specific item index
        /// </summary>
        public void AddDivider(int afterIndex)
        {
            if (!_dividerPositions.Contains(afterIndex))
            {
                _dividerPositions.Add(afterIndex);
                _dividerPositions.Sort();
            }
            Invalidate();
        }

        /// <summary>
        /// Clears all section headers and dividers
        /// </summary>
        public void ClearSectionsAndDividers()
        {
            _sectionHeaders.Clear();
            _dividerPositions.Clear();
            Invalidate();
        }
        #endregion

        #region Painter Initialization
        private void InitializePainter()
        {
            // Skip painter initialization in design mode to prevent flickering
            if (DesignMode)
            {
                _currentPainter = null;
                _painterContext = null;
                return;
            }
            
            // Specialized painters for styles requiring unique visual effects
            // All other styles use UniversalSideBarPainter which leverages StyleColors
            _currentPainter = _style switch
            {
                // Specialized painters with unique visual effects
                BeepControlStyle.Glassmorphism => new Painters.GlassmorphismSideBarPainter(),
                BeepControlStyle.Neumorphism => new Painters.NeumorphicSideBarPainter(),
                BeepControlStyle.Cyberpunk => new Painters.CyberpunkSideBarPainter(),

                // Universal painter handles ALL other styles using StyleColors/BeepStyling
                _ => new Painters.UniversalSideBarPainter(),
            };

            _painterContext = new SideBarPainterContextAdapter(this);
        }
        #endregion

        #region Painter Context Adapter
        /// <summary>
        /// Adapter class that exposes BeepSideBar's internal state to painters
        /// </summary>
        private class SideBarPainterContextAdapter : ISideBarPainterContext
        {
            private readonly BeepSideBar _sideBar;

            public SideBarPainterContextAdapter(BeepSideBar sideBar)
            {
                _sideBar = sideBar ?? throw new ArgumentNullException(nameof(sideBar));
            }

            // Drawing surface
            public Graphics Graphics => _sideBar._currentGraphics;
            public Rectangle Bounds => _sideBar.ClientRectangle;
            public Rectangle DrawingRect => _sideBar.DrawingRect;
            
            // Theme and colors
            public string ThemeName => _sideBar.Theme;
            public IBeepTheme Theme => _sideBar._currentTheme;
            public bool UseThemeColors => _sideBar.UseThemeColors;
            public Color AccentColor => _sideBar.AccentColor;
            public Color BackColor => _sideBar.BackColor;
            
            // Menu items
            public BindingList<SimpleItem> Items => _sideBar._items;
            public SimpleItem SelectedItem => _sideBar._selectedItem;
            public SimpleItem HoveredItem => _sideBar._hoveredItem;
            public SimpleItem PressedItem => _sideBar._pressedItem;
            public Dictionary<SimpleItem, bool> ExpandedState => _sideBar._expandedState;
            
            // State
            public bool IsCollapsed => _sideBar._isCollapsed;
            public bool IsAnimating => _sideBar._isAnimating;
            public int ItemHeight => _sideBar._itemHeight;
            public int ChildItemHeight => _sideBar._childItemHeight;
            
            // Layout
            public int ExpandedWidth => _sideBar._expandedWidth;
            public int CollapsedWidth => _sideBar._collapsedWidth;
            public int IndentationWidth => _sideBar._indentationWidth;
            public int ChromeCornerRadius => _sideBar.ChromeCornerRadius;
            public bool EnableRailShadow => _sideBar.EnableRailShadow;
            
            // Interaction
            public bool IsEnabled => _sideBar.Enabled;
            public bool ShowToggleButton => _sideBar._showToggleButton;
            public bool UseExpandCollapseIcon => _sideBar.UseExpandCollapseIcon;
            public string ExpandIconPath => _sideBar.ExpandIconPath;
            public string CollapseIconPath => _sideBar.CollapseIconPath;
            public string DefaultImagePath => _sideBar.DefaultItemImagePath;
            public TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle ControlStyle => _sideBar.Style;
            
            // Animation state (NEW)
            public float HoverAnimationProgress => _sideBar._hoverAnimationProgress;
            public float SelectionAnimationProgress => _sideBar._selectionAnimationProgress;
            public Dictionary<SimpleItem, float> AccordionAnimationProgress => _sideBar._accordionAnimationProgress;
            
            // Badges and headers (NEW)
            public Dictionary<SimpleItem, string> ItemBadges => _sideBar._itemBadges;
            public Dictionary<SimpleItem, Color> ItemBadgeColors => _sideBar._itemBadgeColors;
            public List<(int BeforeIndex, string HeaderText)> SectionHeaders => _sideBar._sectionHeaders;
            public List<int> DividerPositions => _sideBar._dividerPositions;
        }
        #endregion
    }
}

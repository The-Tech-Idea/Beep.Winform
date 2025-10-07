using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepTree - Properties partial class.
    /// Contains all public properties that expose private fields from Core.cs
    /// </summary>
    public partial class BeepTree
    {
        #region Tree Style and Visual Properties
        
        /// <summary>
        /// Gets or sets the tree style which determines which painter is used for rendering.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual style of the tree.")]
        public TreeStyle TreeStyle
        {
            get => _treeStyle;
            set
            {
                if (_treeStyle != value)
                {
                    _treeStyle = value;
                    InitializePainter();
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets whether to use scaled font based on DPI.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        public bool UseScaledFont
        {
            get => _useScaledFont;
            set
            {
                _useScaledFont = value;
                ApplyTheme();
                Invalidate();
            }
        }
        
        /// <summary>
        /// Gets or sets the font used for text rendering.
        /// </summary>
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TextFont
        {
            get => _textFont;
            set
            {
                _textFont = value;
                UseThemeFont = false;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Gets or sets whether to use the theme font.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("If true, the tree's font is always set to the theme font.")]
        public new bool UseThemeFont
        {
            get => _useThemeFont;
            set
            {
                _useThemeFont = value;
                ApplyTheme();
                Invalidate();
            }
        }
        
        /// <summary>
        /// Gets or sets the text alignment for node labels.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        public TextAlignment TextAlignment
        {
            get => _textAlignment;
            set
            {
                _textAlignment = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Gets or sets whether checkboxes are shown for each node.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        public bool ShowCheckBox
        {
            get => _showCheckBox;
            set
            {
                _showCheckBox = value;
                RecalculateLayoutCache();
                UpdateScrollBars();
                Invalidate();
            }
        }
        
        #endregion
        
        #region Tree-Specific Theme Colors
        
        /// <summary>
        /// Gets the tree background color from the current theme.
        /// </summary>
        public Color TreeBackColor => _currentTheme?.TreeBackColor ?? BackColor;
        
        /// <summary>
        /// Gets the tree foreground color from the current theme.
        /// </summary>
        public Color TreeForeColor => _currentTheme?.TreeForeColor ?? ForeColor;
        
        /// <summary>
        /// Gets the selected node background color from the current theme.
        /// </summary>
        public Color TreeNodeSelectedBackColor => _currentTheme?.TreeNodeSelectedBackColor ?? Color.Blue;
        
        /// <summary>
        /// Gets the selected node foreground color from the current theme.
        /// </summary>
        public Color TreeNodeSelectedForeColor => _currentTheme?.TreeNodeSelectedForeColor ?? Color.White;
        
        /// <summary>
        /// Gets the hovered node background color from the current theme.
        /// </summary>
        public Color TreeNodeHoverBackColor => _currentTheme?.TreeNodeHoverBackColor ?? Color.LightBlue;
        
        /// <summary>
        /// Gets the hovered node foreground color from the current theme.
        /// </summary>
        public Color TreeNodeHoverForeColor => _currentTheme?.TreeNodeHoverForeColor ?? Color.Black;
        
        #endregion
        
        #region Node Data and Selection Properties
        
        /// <summary>
        /// Gets or sets the collection of root nodes in the tree.
        /// </summary>
        [Browsable(false)]
        public IList<SimpleItem> Nodes
        {
            get => _nodes;
            set
            {
                _nodes.Clear();
                if (value != null)
                {
                    _nodes.AddRange(value);
                }
                RebuildVisible();
                Invalidate();
            }
        }
        
        /// <summary>
        /// Gets or sets the currently selected node.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleItem SelectedNode
        {
            get => _lastSelectedNode;
            set
            {
                if (_lastSelectedNode != value)
                {
                    if (_lastSelectedNode != null)
                        _lastSelectedNode.IsSelected = false;
                    
                    _lastSelectedNode = value;
                    
                    if (_lastSelectedNode != null)
                    {
                        _lastSelectedNode.IsSelected = true;
                    }
                }
                else
                {
                    _lastSelectedNode = value;
                    if (_lastSelectedNode != null)
                        _lastSelectedNode.IsSelected = true;
                }
            }
        }
        
        /// <summary>
        /// Gets the list of currently selected nodes (for multi-select).
        /// </summary>
        public List<SimpleItem> SelectedNodes
        {
            get => _selectedNodes;
            private set => _selectedNodes = value;
        }
        
        /// <summary>
        /// Gets the most recently clicked node.
        /// </summary>
        public SimpleItem ClickedNode { get; internal set; }
        
        /// <summary>
        /// Gets or sets whether multiple nodes can be selected at once.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        public bool AllowMultiSelect
        {
            get => _allowMultiSelect;
            set
            {
                _allowMultiSelect = value;
                if (!_allowMultiSelect && SelectedNodes != null && SelectedNodes.Count > 1)
                {
                    // Deselect all but the first node
                    for (int i = 1; i < SelectedNodes.Count; i++)
                    {
                        SelectedNodes[i].IsSelected = false;
                    }
                    SelectedNodes = SelectedNodes.Take(1).ToList();
                }
                Invalidate();
            }
        }
        
        #endregion
        
        #region Performance and Layout Properties
        
        /// <summary>
        /// Gets or sets whether to virtualize layout for performance.
        /// </summary>
        [Browsable(true)]
        [Category("Performance")]
        [Description("If true, only measures nodes near the viewport to reduce work on massive trees.")]
        public bool VirtualizeLayout
        {
            get => _virtualizeLayout;
            set
            {
                _virtualizeLayout = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Gets or sets the extra rows to measure when virtualizing.
        /// </summary>
        [Browsable(true)]
        [Category("Performance")]
        [Description("Extra rows to measure above/below the viewport when virtualization is enabled.")]
        public int VirtualizationBufferRows
        {
            get => _virtualizationBufferRows;
            set
            {
                _virtualizationBufferRows = Math.Max(0, value);
                Invalidate();
            }
        }
        
        #endregion
        
        #region Scrollbar Properties
        
        /// <summary>
        /// Gets or sets whether the vertical scrollbar is shown.
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowVerticalScrollBar
        {
            get => _showVerticalScrollBar;
            set
            {
                _showVerticalScrollBar = value;
                UpdateScrollBars();
                Invalidate();
            }
        }
        
        /// <summary>
        /// Gets or sets whether the horizontal scrollbar is shown.
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowHorizontalScrollBar
        {
            get => _showHorizontalScrollBar;
            set
            {
                _showHorizontalScrollBar = value;
                UpdateScrollBars();
                Invalidate();
            }
        }
        
        #endregion
        
        #region Context Menu Properties
        
        /// <summary>
        /// Gets or sets the current menu items.
        /// </summary>
        public BindingList<SimpleItem> CurrentMenutems
        {
            get => _currentMenuItems;
            set
            {
                _currentMenuItems = value;
                Invalidate();
            }
        }
        
        #endregion
    }
}

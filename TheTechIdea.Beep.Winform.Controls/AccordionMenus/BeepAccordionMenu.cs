using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Editors;
using TheTechIdea.Beep.Winform.Controls.AccordionMenus.Helpers;
using TheTechIdea.Beep.Winform.Controls.AccordionMenus.Painters;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;
using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.Winform.Controls.AccordionMenus
{
    [ToolboxItem(true)]
    [Description("A modern collapsible accordion control with expandable menu items and painter-based rendering.")]
    [DisplayName("Beep Accordion Menu")]
    public partial class BeepAccordionMenu : BaseControl
    {
        #region Private Fields
        private bool isCollapsed = false;
        private int animationStep = 20;
        private int animationDelay = 15;

        private int itemHeight = AccordionStyleHelpers.GetRecommendedItemHeight(AccordionStyle.Material3);
        private int childItemHeight = AccordionStyleHelpers.GetRecommendedChildItemHeight(AccordionStyle.Material3);
        private bool isInitialized = false;
        private Timer animationTimer;
        private bool isAnimating = false;
        private SimpleItem _selectedItem = null!;
        private SimpleItem _focusedItem = null!;
        private int headerHeight = AccordionStyleHelpers.GetRecommendedHeaderHeight(AccordionStyle.Material3);
        private int spacing = AccordionStyleHelpers.GetRecommendedSpacing(AccordionStyle.Material3);
        private Rectangle _toggleButtonRect;
        private Dictionary<SimpleItem, bool> expandedState = new Dictionary<SimpleItem, bool>();
        private int indentationWidth = AccordionStyleHelpers.GetRecommendedIndentation(AccordionStyle.Material3);

        // Painter system
        private IAccordionPainter _accordionPainter;
        private AccordionStyle _accordionStyle = AccordionStyle.Material3;
        #endregion

        #region Constructor
        public BeepAccordionMenu()
        {
            items = new BindingList<SimpleItem>();
            DoubleBuffered = true;
            TabStop = true;
            Padding = new Padding(5);
            AllowDrop = true;

            // Configure BaseControl properties
            IsChild = false;
            IsFrameless = true;
            ShowAllBorders = false;

            // Initialize painter
            _accordionPainter = AccordionPainterFactory.GetPainter(_accordionStyle);

            animationTimer = new Timer { Interval = 10 };
            animationTimer.Tick += AnimationTimer_Tick;
        }
        #endregion

        #region Properties
        [Category("Behavior")]
        public int ExpandedWidth { get; set; } = BeepLayoutMetrics.SideMenu.Width;

        [Category("Behavior")]
        public int CollapsedWidth { get; set; } = BeepLayoutMetrics.CollapsedRailWidth;

        [Category("Animation")]
        public int AnimationStep
        {
            get => animationStep;
            set => animationStep = Math.Max(1, value);
        }

        [Category("Animation")]
        public int AnimationDelay
        {
            get => animationDelay;
            set => animationDelay = Math.Max(1, value);
        }

        [Category("Appearance")]
        public int ItemHeight
        {
            get => itemHeight;
            set
            {
                itemHeight = Math.Max(20, value);
                childItemHeight = Math.Max(15, itemHeight - 10);
                if (isInitialized)
                {
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        public int ChildItemHeight
        {
            get => childItemHeight;
            set
            {
                childItemHeight = Math.Max(15, value);
                if (isInitialized)
                {
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        public int IndentationWidth
        {
            get => indentationWidth;
            set
            {
                indentationWidth = Math.Max(10, value);
                Invalidate();
            }
        }

        [Category("Behavior")]
        [Description("Auto-adjust height to fit all items")]
        public bool AutoFitHeight { get; set; } = false;

        [Category("Behavior")]
        [Description("Enable vertical scrolling when items exceed height")]
        public bool AutoScrollEnabled { get; set; } = true;

        [Category("Behavior")]
        [Description("Enable drag and drop reordering of items")]
        public bool AllowItemDragDrop { get; set; } = true;

        [Category("Behavior")]
        [Description("Enable icons for items that have ImagePath set")]
        public bool HasItemIcons { get; set; } = true;

        [Category("Appearance")]
        [Description("Accordion visual style")]
        public AccordionStyle AccordionStyle
        {
            get => _accordionStyle;
            set
            {
                if (_accordionStyle != value)
                {
                    _accordionStyle = value;
                    _accordionPainter = AccordionPainterFactory.GetPainter(_accordionStyle);
                    Invalidate();
                }
            }
        }

        private string _title = "Accordion";
        [Category("Appearance")]
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                Invalidate();
            }
        }

        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                Invalidate();
            }
        }

        private BindingList<SimpleItem> items = new BindingList<SimpleItem>();
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [Editor(typeof(MenuItemCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> ListItems
        {
            get => items;
            set
            {
                if (value != null && value != items)
                {
                    items = value;
                    if (isInitialized)
                    {
                        InitializeMenuItemState();
                        Invalidate();
                    }
                }
            }
        }

        protected override Size DefaultSize => BeepLayoutMetrics.SideMenu;
        #endregion

        #region Events
        public event EventHandler<BeepMouseEventArgs>? ItemClick;
        public event EventHandler<BeepMouseEventArgs>? ToggleClicked;
        public event EventHandler<BeepMouseEventArgs>? HeaderExpandedChanged;
        public event EventHandler<AccordionItemIconEventArgs>? ItemIconNeeded;
        #endregion

        public virtual void OnItemIconNeeded(SimpleItem item, AccordionItemIconEventArgs e)
        {
            ItemIconNeeded?.Invoke(this, e);
        }
    }
}

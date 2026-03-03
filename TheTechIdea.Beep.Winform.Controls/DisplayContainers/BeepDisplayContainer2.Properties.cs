using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    public partial class BeepDisplayContainer2
    {
        [Category("Appearance")]
        [DefaultValue(ContainerDisplayMode.Tabbed)]
        public ContainerDisplayMode DisplayMode
        {
            get => _displayMode;
            set { _displayMode = value; RecalculateLayout(); Invalidate(); }
        }

        // Use BaseControl's IsTransparentBackground property (like BeepMenuBar does)
        // No need for separate IsTransparent property - use the base one

        [Category("Appearance")]
        [DefaultValue(TabPosition.Top)]
        public TabPosition TabPosition
        {
            get => _tabPosition;
            set { _tabPosition = value; RecalculateLayout(); Invalidate(); }
        }

        [Category("Behavior")]
        [DefaultValue(true)]
        public bool ShowCloseButtons
        {
            get => _showCloseButtons;
            set { _showCloseButtons = value; Invalidate(); }
        }

        [Category("Behavior")]
        [DefaultValue(true)]
        public bool AllowTabReordering
        {
            get => _allowTabReordering;
            set => _allowTabReordering = value;
        }

        [Category("Animation")]
        [DefaultValue(true)]
        public bool EnableAnimations
        {
            get => _enableAnimations;
            set => _enableAnimations = value;
        }

        [Category("Animation")]
        [DefaultValue(AnimationSpeed.Normal)]
        public AnimationSpeed AnimationSpeed
        {
            get => _animationSpeed;
            set => _animationSpeed = value;
        }

        [Category("Appearance")]
        [DefaultValue(36)]
        public int TabHeight
        {
            get => _tabHeight;
            set { _tabHeight = Math.Max(24, value); RecalculateLayout(); Invalidate(); }
        }

        /// <summary>
        /// When <c>true</c> (default), the tab strip height is automatically calculated
        /// from the current font metrics and style padding so tab text is never clipped.
        /// Set to <c>false</c> to use the fixed <see cref="TabHeight"/> value instead.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool AutoTabHeight
        {
            get => _autoTabHeight;
            set { _autoTabHeight = value; RecalculateLayout(); Invalidate(); }
        }

        public ContainerTypeEnum ContainerType
        {
            get => _containerType;
            set => _containerType = value;
        }

        /// <summary>Message shown in the content area when there are no open tabs.</summary>
        [Category("Appearance")]
        [DefaultValue("No tabs open.\nClick + to add a new tab.")]
        public string EmptyStateText
        {
            get => _emptyStateText;
            set { _emptyStateText = value ?? string.Empty; Invalidate(); }
        }

        /// <summary>
        /// Controls whether the empty-state placeholder is shown when there are no open tabs.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ShowEmptyState
        {
            get => _showEmptyState;
            set { _showEmptyState = value; Invalidate(); }
        }

        // ---- P4 Enhancement 4: Keyboard Navigation ----
        /// <summary>
        /// When true, arrow keys, Ctrl+Tab, and Ctrl+W perform tab navigation/close
        /// while this control has keyboard focus.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool EnableKeyboardNavigation
        {
            get => _enableKeyboardNav;
            set => _enableKeyboardNav = value;
        }

        //// ---- P4 Enhancement 5: Drag-to-reorder ----
        ///// <summary>Gets or sets whether tabs can be repositioned by dragging.</summary>
        //[Category("Behavior")]
        //[DefaultValue(true)]
        //public bool AllowTabReordering
        //{
        //    get => _allowTabReordering;
        //    set => _allowTabReordering = value;
        //}

        // ---- P4 Enhancement 6: Gradient Tab Strip Background ----
        /// <summary>
        /// When true the tab strip is painted with a subtle gradient instead of a solid fill.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool UseTabStripGradient
        {
            get => _useTabStripGradient;
            set { _useTabStripGradient = value; Invalidate(); }
        }

        /// <summary>
        /// End color for the tab-strip gradient.  Leave as <see cref="Color.Empty"/> to
        /// auto-derive a slightly darker shade from the current tab-back color.
        /// </summary>
        [Category("Appearance")]
        public Color TabStripGradientEndColor
        {
            get => _tabStripGradientEndColor;
            set { _tabStripGradientEndColor = value; Invalidate(); }
        }

        private TheTechIdea.Beep.Winform.Controls.TabStyle _tabStyle = TheTechIdea.Beep.Winform.Controls.TabStyle.Capsule;

        [Category("Appearance")]
        [DefaultValue(TheTechIdea.Beep.Winform.Controls.TabStyle.Capsule)]
        public TheTechIdea.Beep.Winform.Controls.TabStyle TabStyle
        {
            get => _tabStyle;
            set { _tabStyle = value; if (_paintHelper != null) _paintHelper.TabStyle = value; RecalculateLayout(); Invalidate(); }
        }

        /// <summary>
       /// <summary>
        /// Override BackColor to sync with IsTransparentBackground property
        /// Allows setting BackColor = Color.Transparent directly
        /// Like BeepMenuBar, we preserve Transparent and prevent BaseControl from resetting it
        /// </summary>
        //public override Color BackColor
        //{
        //    get => base.BackColor;
        //    set
        //    {
        //        // Check if trying to set transparent
        //        bool shouldBeTransparent = (value == Color.Transparent || value.A == 0);
                
        //        // Always allow setting to Transparent (like BeepMenuBar does)
        //        if (shouldBeTransparent)
        //        {
        //            // Set IsTransparentBackground first, then set BackColor
        //            IsTransparentBackground = true;
        //            base.BackColor = Color.Transparent;
        //            Invalidate();
        //            return;
        //        }

        //        // For non-transparent colors, check if value actually changed
        //        if (base.BackColor == value) return;

        //        // Sync IsTransparentBackground when setting opaque color
        //        if (!shouldBeTransparent && IsTransparentBackground && value.A == 255)
        //        {
        //            IsTransparentBackground = false;
        //        }

        //        base.BackColor = value;
        //        Invalidate();
        //    }
        //}
    }
}


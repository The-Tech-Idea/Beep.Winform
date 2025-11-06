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

        public ContainerTypeEnum ContainerType
        {
            get => _containerType;
            set => _containerType = value;
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


using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Docks;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;
using DockStyle = TheTechIdea.Beep.Winform.Controls.Docks.DockStyle;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepDock control
    /// Provides smart tags for dock configuration, positioning, and styling
    /// </summary>
    public class BeepDockDesigner : BaseBeepControlDesigner
    {
        public BeepDock? Dock => Component as BeepDock;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepDockActionList(this));
            return lists;
        }
    }

    /// <summary>
    /// Action list for BeepDock smart tags
    /// Provides quick configuration presets and common property access
    /// </summary>
    public class BeepDockActionList : DesignerActionList
    {
        private readonly BeepDockDesigner _designer;

        public BeepDockActionList(BeepDockDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepDock? Dock => Component as BeepDock;

        #region Properties (for smart tags)

        [Category("Beep Dock")]
        [Description("Visual style of the dock")]
        public DockStyle DockStyleType
        {
            get => _designer.GetProperty<DockStyle>("DockStyleType");
            set => _designer.SetProperty("DockStyleType", value);
        }

        [Category("Beep Dock")]
        [Description("Size of dock items in pixels")]
        public int ItemSize
        {
            get => _designer.GetProperty<int>("ItemSize");
            set => _designer.SetProperty("ItemSize", value);
        }

        [Category("Beep Dock")]
        [Description("Height of the dock container")]
        public int DockHeight
        {
            get => _designer.GetProperty<int>("DockHeight");
            set => _designer.SetProperty("DockHeight", value);
        }

        [Category("Beep Dock")]
        [Description("Spacing between dock items")]
        public int ItemSpacing
        {
            get => _designer.GetProperty<int>("ItemSpacing");
            set => _designer.SetProperty("ItemSpacing", value);
        }

        [Category("Beep Dock")]
        [Description("Maximum scale factor for hovered items")]
        public float MaxScale
        {
            get => _designer.GetProperty<float>("MaxScale");
            set => _designer.SetProperty("MaxScale", value);
        }

        [Category("Beep Dock")]
        [Description("Position of the dock")]
        public DockPosition DockPositionType
        {
            get => _designer.GetProperty<DockPosition>("DockPositionType");
            set => _designer.SetProperty("DockPositionType", value);
        }

        [Category("Beep Dock")]
        [Description("Orientation of the dock (horizontal or vertical)")]
        public DockOrientation DockOrientationType
        {
            get => _designer.GetProperty<DockOrientation>("DockOrientationType");
            set => _designer.SetProperty("DockOrientationType", value);
        }

        #endregion

        #region Quick Configuration Actions

        /// <summary>
        /// Configure as macOS-style Apple Dock
        /// </summary>
        public void ConfigureAsAppleDock()
        {
            DockStyleType = DockStyle.AppleDock;
            DockPositionType = DockPosition.Bottom;
            DockOrientationType = DockOrientation.Horizontal;
            ItemSize = 56;
            DockHeight = 72;
            MaxScale = 1.5f;
        }

        /// <summary>
        /// Configure as Windows 11 taskbar style
        /// </summary>
        public void ConfigureAsWindows11Dock()
        {
            DockStyleType = DockStyle.Windows11Dock;
            DockPositionType = DockPosition.Bottom;
            DockOrientationType = DockOrientation.Horizontal;
            ItemSize = 48;
            DockHeight = 48;
            MaxScale = 1.3f;
        }

        /// <summary>
        /// Configure as Material Design 3 dock
        /// </summary>
        public void ConfigureAsMaterial3Dock()
        {
            DockStyleType = DockStyle.Material3Dock;
            DockPositionType = DockPosition.Bottom;
            DockOrientationType = DockOrientation.Horizontal;
            ItemSize = 56;
            DockHeight = 64;
            MaxScale = 1.4f;
        }

        /// <summary>
        /// Configure as Glassmorphism dock
        /// </summary>
        public void ConfigureAsGlassmorphismDock()
        {
            DockStyleType = DockStyle.GlassmorphismDock;
            DockPositionType = DockPosition.Bottom;
            DockOrientationType = DockOrientation.Horizontal;
            ItemSize = 56;
            DockHeight = 72;
            MaxScale = 1.5f;
        }

        /// <summary>
        /// Configure as iOS-style dock
        /// </summary>
        public void ConfigureAsiOSDock()
        {
            DockStyleType = DockStyle.iOSDock;
            DockPositionType = DockPosition.Bottom;
            DockOrientationType = DockOrientation.Horizontal;
            ItemSize = 60;
            DockHeight = 80;
            MaxScale = 1.6f;
        }

        /// <summary>
        /// Configure as Minimal dock
        /// </summary>
        public void ConfigureAsMinimalDock()
        {
            DockStyleType = DockStyle.MinimalDock;
            DockPositionType = DockPosition.Bottom;
            DockOrientationType = DockOrientation.Horizontal;
            ItemSize = 48;
            DockHeight = 56;
            MaxScale = 1.2f;
        }

        /// <summary>
        /// Position dock at bottom
        /// </summary>
        public void PositionAtBottom()
        {
            DockPositionType = DockPosition.Bottom;
            DockOrientationType = DockOrientation.Horizontal;
        }

        /// <summary>
        /// Position dock at top
        /// </summary>
        public void PositionAtTop()
        {
            DockPositionType = DockPosition.Top;
            DockOrientationType = DockOrientation.Horizontal;
        }

        /// <summary>
        /// Position dock on left side
        /// </summary>
        public void PositionOnLeft()
        {
            DockPositionType = DockPosition.Left;
            DockOrientationType = DockOrientation.Vertical;
        }

        /// <summary>
        /// Position dock on right side
        /// </summary>
        public void PositionOnRight()
        {
            DockPositionType = DockPosition.Right;
            DockOrientationType = DockOrientation.Vertical;
        }

        /// <summary>
        /// Set standard item size (56px)
        /// </summary>
        public void SetStandardItemSize()
        {
            ItemSize = 56;
        }

        /// <summary>
        /// Set compact item size (48px)
        /// </summary>
        public void SetCompactItemSize()
        {
            ItemSize = 48;
        }

        /// <summary>
        /// Set large item size (64px)
        /// </summary>
        public void SetLargeItemSize()
        {
            ItemSize = 64;
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            // Quick configuration presets
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsAppleDock", "Apple Dock (macOS)", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsWindows11Dock", "Windows 11 Taskbar", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsMaterial3Dock", "Material Design 3", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsGlassmorphismDock", "Glassmorphism", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsiOSDock", "iOS Dock", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsMinimalDock", "Minimal", "Style Presets", true));

            // Position presets
            items.Add(new DesignerActionHeaderItem("Position Presets"));
            items.Add(new DesignerActionMethodItem(this, "PositionAtBottom", "Bottom", "Position Presets", true));
            items.Add(new DesignerActionMethodItem(this, "PositionAtTop", "Top", "Position Presets", true));
            items.Add(new DesignerActionMethodItem(this, "PositionOnLeft", "Left", "Position Presets", true));
            items.Add(new DesignerActionMethodItem(this, "PositionOnRight", "Right", "Position Presets", true));

            // Size presets
            items.Add(new DesignerActionHeaderItem("Item Size Presets"));
            items.Add(new DesignerActionMethodItem(this, "SetStandardItemSize", "Standard (56px)", "Item Size Presets", true));
            items.Add(new DesignerActionMethodItem(this, "SetCompactItemSize", "Compact (48px)", "Item Size Presets", true));
            items.Add(new DesignerActionMethodItem(this, "SetLargeItemSize", "Large (64px)", "Item Size Presets", true));

            // Dock properties
            items.Add(new DesignerActionHeaderItem("Dock Properties"));
            items.Add(new DesignerActionPropertyItem("DockStyleType", "Dock Style", "Dock Properties"));
            items.Add(new DesignerActionPropertyItem("DockPositionType", "Dock Position", "Dock Properties"));
            items.Add(new DesignerActionPropertyItem("DockOrientationType", "Dock Orientation", "Dock Properties"));
            items.Add(new DesignerActionPropertyItem("ItemSize", "Item Size", "Dock Properties"));
            items.Add(new DesignerActionPropertyItem("DockHeight", "Dock Height", "Dock Properties"));
            items.Add(new DesignerActionPropertyItem("ItemSpacing", "Item Spacing", "Dock Properties"));
            items.Add(new DesignerActionPropertyItem("MaxScale", "Max Scale", "Dock Properties"));

            return items;
        }
    }
}

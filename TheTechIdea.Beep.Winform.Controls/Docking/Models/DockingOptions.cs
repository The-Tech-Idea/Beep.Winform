using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Models
{
    /// <summary>
    /// A single bag of manager-level docking options. Mirrors Krypton's <c>DockingOptions</c>
    /// group and DockPanelSuite's <c>DockPanelExt</c> options cluster: it lets a host form
    /// bind one <see cref="System.Windows.Forms.PropertyGrid"/> category to the manager's
    /// behavior knobs rather than scattered individual properties.
    ///
    /// The properties on this class are pass-through wrappers over <see cref="BeepDockingManager"/>
    /// — assigning to a property on the options bag is equivalent to assigning the same-named
    /// property on the manager. The bag itself is owned by the manager and exposed via
    /// <see cref="BeepDockingManager.Options"/>.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class DockingOptions
    {
        private readonly BeepDockingManager _owner;

        internal DockingOptions(BeepDockingManager owner)
        {
            _owner = owner ?? throw new System.ArgumentNullException(nameof(owner));
        }

        /// <summary>Whether user-initiated docking interactions are allowed. See
        /// <see cref="BeepDockingManager.AllowEndUserDocking"/>.</summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Allow user-initiated docking interactions: tab drag-to-float, splitter drag, " +
                     "tab reorder, and float-window dock-to-edge drops.")]
        public bool AllowEndUserDocking
        {
            get => _owner.AllowEndUserDocking;
            set => _owner.AllowEndUserDocking = value;
        }

        /// <summary>Whether the manager focuses an auto-hidden panel's content after its
        /// slide-in animation completes. See
        /// <see cref="BeepDockingManager.ActiveAutoHideContent"/>.</summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Focus the content of an auto-hidden panel after its slide-in animation completes.")]
        public bool ActiveAutoHideContent
        {
            get => _owner.ActiveAutoHideContent;
            set => _owner.ActiveAutoHideContent = value;
        }

        /// <summary>Whether the dock-overlay snap-line is drawn during tab-drag. See
        /// <see cref="BeepDockingManager.ShowSnapGuides"/>.</summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Draw a thin accent snap-line indicator during tab-drag for group-edge and " +
                     "center-stack drop targets.")]
        public bool ShowSnapGuides
        {
            get => _owner.ShowSnapGuides;
            set => _owner.ShowSnapGuides = value;
        }

        /// <summary>Default size for new float windows when no preferred size is available. See
        /// <see cref="BeepDockingManager.DefaultFloatWindowSize"/>.</summary>
        [Category("Layout")]
        [DefaultValue(typeof(Size), "0,0")]
        [Description("Default size for new float windows when the panel has no preferred size.")]
        public Size DefaultFloatWindowSize
        {
            get => _owner.DefaultFloatWindowSize;
            set => _owner.DefaultFloatWindowSize = value;
        }
    }
}

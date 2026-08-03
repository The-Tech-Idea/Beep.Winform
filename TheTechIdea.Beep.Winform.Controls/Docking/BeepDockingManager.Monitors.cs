using System;
using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Docking.Layout;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// Multi-monitor awareness for floating windows.
    /// </summary>
    /// <remarks>
    /// The display set is a <b>dependency</b>, not something read from the machine at the point of
    /// use. That is what makes "the second monitor has been unplugged since this layout was saved"
    /// an argument rather than a hardware configuration, and it is the only reason the three cases
    /// this feature exists for can be asserted at all.
    /// </remarks>
    public partial class BeepDockingManager
    {
        private IMonitorProvider _monitors;

        /// <summary>
        /// Displays used when saving and restoring floating windows. Defaults to the real ones;
        /// assign a different provider to resolve layouts against a supplied set.
        /// </summary>
        [Browsable(false)]
        public IMonitorProvider Monitors
        {
            get => _monitors ??= SystemMonitorProvider.Instance;
            set => _monitors = value;
        }

        /// <summary>
        /// Raised when a float could not be restored where it was saved — the display is gone, or
        /// the bounds fell outside it.
        /// </summary>
        /// <remarks>
        /// A notification rather than an error: relocating is the correct behaviour, not a failure.
        /// It is surfaced because a tool window silently appearing somewhere else is confusing, and
        /// a host may want to tell the user why.
        /// </remarks>
        public event EventHandler<FloatRelocatedEventArgs> FloatRelocated;

        private void OnFloatRelocated(DockPanel panel, FloatingPanelInfo saved,
                                      FloatBoundsResolver.Resolution placement)
            => FloatRelocated?.Invoke(this, new FloatRelocatedEventArgs(panel, saved, placement));
    }

    /// <summary>Describes a float that was restored somewhere other than where it was saved.</summary>
    public sealed class FloatRelocatedEventArgs : EventArgs
    {
        public FloatRelocatedEventArgs(DockPanel panel, FloatingPanelInfo saved,
                                       FloatBoundsResolver.Resolution placement)
        {
            Panel = panel;
            SavedBounds = saved?.Bounds ?? System.Drawing.Rectangle.Empty;
            SavedDeviceName = saved?.DeviceName ?? string.Empty;
            Placement = placement;
        }

        public DockPanel Panel { get; }

        public System.Drawing.Rectangle SavedBounds { get; }

        /// <summary>Display the layout was saved on. Empty for a layout that predates the field.</summary>
        public string SavedDeviceName { get; }

        /// <summary>Where it went, which display, and why.</summary>
        public FloatBoundsResolver.Resolution Placement { get; }

        public override string ToString()
            => $"{Panel?.Key}: {SavedBounds} on '{SavedDeviceName}' -> {Placement}";
    }
}

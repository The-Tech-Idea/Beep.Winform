using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Models
{
    /// <summary>
    /// One display, described by value.
    /// </summary>
    /// <remarks>
    /// Deliberately a plain record rather than a wrapper around <see cref="Screen"/>: the set of
    /// monitors has to be an <b>input</b> to layout restore, not an ambient fact read from the
    /// machine. Every interesting case — a saved layout whose second monitor is gone, bounds that
    /// land off every display — is otherwise untestable without physically rearranging hardware.
    /// </remarks>
    public readonly struct MonitorInfo : IEquatable<MonitorInfo>
    {
        public MonitorInfo(string deviceName, Rectangle bounds, Rectangle workingArea, bool isPrimary)
        {
            DeviceName = deviceName ?? string.Empty;
            Bounds = bounds;
            WorkingArea = workingArea;
            IsPrimary = isPrimary;
        }

        /// <summary>Stable per-display identifier, e.g. <c>\\.\DISPLAY1</c>.</summary>
        public string DeviceName { get; }

        /// <summary>Full display rectangle in virtual-screen coordinates.</summary>
        public Rectangle Bounds { get; }

        /// <summary>Display rectangle excluding taskbars and appbars.</summary>
        public Rectangle WorkingArea { get; }

        public bool IsPrimary { get; }

        public bool Equals(MonitorInfo other)
            => string.Equals(DeviceName, other.DeviceName, StringComparison.Ordinal)
               && Bounds == other.Bounds && WorkingArea == other.WorkingArea
               && IsPrimary == other.IsPrimary;

        public override bool Equals(object obj) => obj is MonitorInfo other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(DeviceName, Bounds, WorkingArea, IsPrimary);

        public override string ToString()
            => $"{DeviceName} {Bounds}{(IsPrimary ? " (primary)" : "")}";
    }

    /// <summary>Supplies the set of displays layout restore reasons about.</summary>
    public interface IMonitorProvider
    {
        /// <summary>All displays. Never empty in a working configuration.</summary>
        IReadOnlyList<MonitorInfo> GetMonitors();
    }

    /// <summary>
    /// The real displays, from <see cref="Screen.AllScreens"/>.
    /// </summary>
    public sealed class SystemMonitorProvider : IMonitorProvider
    {
        public static readonly SystemMonitorProvider Instance = new SystemMonitorProvider();

        public IReadOnlyList<MonitorInfo> GetMonitors()
            => Screen.AllScreens
                     .Select(s => new MonitorInfo(s.DeviceName, s.Bounds, s.WorkingArea, s.Primary))
                     .ToList();
    }
}

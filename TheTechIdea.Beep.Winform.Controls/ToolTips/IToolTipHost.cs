using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Sprint 12 — Abstract host contract that the tooltip system calls
    /// when it needs to show, hide, or measure a tooltip.
    /// 
    /// Implement this interface to host tooltips in non-standard environments:
    ///  • Headless unit tests (mock host, no actual window).
    ///  • WPF / Avalonia surfaces via HwndHost.
    ///  • Custom rendering canvases.
    /// </summary>
    public interface IToolTipHost
    {
        // ──────────────────────────────────────────────────────────────
        // Display lifecycle
        // ──────────────────────────────────────────────────────────────

        /// <summary>Shows a tooltip with the given config at a screen position.</summary>
        Task ShowAsync(ToolTipConfig config, Point screenLocation);

        /// <summary>Hides the currently visible tooltip.</summary>
        Task HideAsync();

        /// <summary>Returns <c>true</c> when a tooltip is currently visible.</summary>
        bool IsVisible { get; }

        // ──────────────────────────────────────────────────────────────
        // Geometry
        // ──────────────────────────────────────────────────────────────

        /// <summary>Calculates the ideal tooltip size for the given config.</summary>
        Size MeasureSize(ToolTipConfig config);

        /// <summary>
        /// Returns the screen bounds available to the host
        /// (e.g. the monitor work-area or the canvas bounds).
        /// </summary>
        Rectangle GetScreenBounds();

        // ──────────────────────────────────────────────────────────────
        // Events
        // ──────────────────────────────────────────────────────────────

        /// <summary>Raised just after the tooltip becomes visible.</summary>
        event EventHandler Shown;

        /// <summary>Raised just after the tooltip is hidden.</summary>
        event EventHandler Hidden;
    }
}

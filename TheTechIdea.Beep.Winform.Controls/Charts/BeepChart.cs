using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Charts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Charts
{
    public enum ChartSelectionMode
    {
        Single,
        Multiple
    }

    public enum ChartVisualPreset
    {
        Dashboard,      // Light colors, compact spacing, no grid
        Analytical,     // High contrast, detailed grid, emphasis on precision
        HighContrast,   // Accessibility: strong color separation, thick lines
        Print,          // Monochrome-friendly, high detail, optimized for B&W output
        Presentation    // Vibrant, larger fonts, bold colors, minimal clutter
    }

    public enum ChartSeriesFillPattern
    {
        Solid,          // Solid fill (default)
        Horizontal,     // Horizontal lines
        Vertical,       // Vertical lines
        Diagonal,       // Diagonal forward slash
        BackDiagonal,   // Diagonal backslash
        Cross,          // Crosshatch
        DiagonalCross,  // Diagonal crosshatch
        Dots            // Dotted pattern
    }

    public enum ChartSurfaceStyle
    {
        Classic,
        Card,
        Outline,
        Glass
    }

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepChart), "BeepChart.bmp")]
    [Description("A custom chart control for WinForms.")]
    [DefaultProperty("DataSeries")]
    [DisplayName("BeepChart")]
    [Category("Beep Controls")]
    public partial class BeepChart : BaseControl
    {
        public BeepChart() : base()
        {
            try
            {
                InitializeDefaultSettings();
                InitializeDesignTimeSampleData();
                InitializePainter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"BeepChart Constructor Error: {ex.Message}");
            }
        }

        protected override Size DefaultSize => new Size(400, 300);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DataPointToolTip?.Dispose();
                AnimTimer?.Dispose();
                StreamRenderTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
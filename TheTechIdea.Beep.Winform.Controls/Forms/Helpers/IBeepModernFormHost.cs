using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Minimal contract a Form (or adapter) must implement so helper components can interact
    /// without tight coupling to BeepiForm concrete implementation.
    /// </summary>
    internal interface IBeepModernFormHost
    {
        Form AsForm { get; }
        IBeepTheme CurrentTheme { get; }
        int BorderRadius { get; set; }
        int BorderThickness { get; set; }
        Padding Padding { get; set; }
        bool IsInDesignMode { get; }
        void Invalidate();
        void UpdateRegion();
    }
}

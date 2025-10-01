using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Chips.Helpers
{
    internal interface IChipGroupPainter
    {
        void Initialize(BaseControl owner, IBeepTheme theme);
        void UpdateTheme(IBeepTheme theme);

        // Measure a single chip (width/height). Height may be dictated by ChipSize.
        Size MeasureChip(SimpleItem item, Graphics g, ChipRenderOptions options);

        // Draw a single chip and optionally return a close (X) rect to register as a hit-area.
        void RenderChip(Graphics g, SimpleItem item, Rectangle bounds, ChipVisualState state, ChipRenderOptions options, out Rectangle closeRect);

        // Optional: draw title/background inside DrawingRect (outer container by BaseControl painters)
        void RenderGroupBackground(Graphics g, Rectangle drawingRect, ChipRenderOptions options);
    }

    internal struct ChipVisualState
    {
        public bool IsSelected;
        public bool IsHovered;
        public bool IsFocused;
        public ChipVariant Variant;
        public ChipSize Size;
        public ChipColor Color;
    }

    public enum SelectionMarkKind { Dot, Check }

    internal class ChipRenderOptions
    {
        public Font Font { get; set; }
        public int CornerRadius { get; set; } = 15;
        public int Gap { get; set; } = 5; // horizontal gap between chips
        public bool ShowCloseOnSelected { get; set; } = true;
        public bool ShowSelectionCheck { get; set; } = true;
        public bool ShowBorders { get; set; } = true;
        public int BorderWidth { get; set; } = 1;
        public ChipSize Size { get; set; } = ChipSize.Medium;
        public IBeepTheme Theme { get; set; }
        public Chips.ChipStyle Style { get; set; } = Chips.ChipStyle.Default;
        // Icons
        public bool ShowIcon { get; set; } = true;
        public Size IconMaxSize { get; set; } = new Size(16, 16);
        // Selection mark kind
        public SelectionMarkKind SelectionMark { get; set; } = SelectionMarkKind.Check;
        // Optional badge counter provider
        public Func<SimpleItem, int?> GetBadgeCount { get; set; }
    }
}

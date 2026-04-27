using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Models
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class ProgressPainterContext
    {
        public ProgressPainterKind PainterKind { get; set; }
        public Rectangle Bounds { get; set; }
        public IBeepTheme Theme { get; set; }
        public BeepControlStyle ControlStyle { get; set; }
        public ProgressPainterState State { get; set; } = new ProgressPainterState();
        public IReadOnlyDictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }
}

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Text.Json;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Accessibility;
using TheTechIdea.Beep.Winform.Controls.Backstage;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Customization;
using TheTechIdea.Beep.Winform.Controls.Gallery;
using TheTechIdea.Beep.Winform.Controls.Rendering;
using TheTechIdea.Beep.Winform.Controls.Search;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Tokens;
using TheTechIdea.Beep.Winform.Controls.Tooltips;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Ribbon Control")]
    [Description("A full-featured ribbon control with tabbed groups, quick access toolbar, backstage, search, and theming.")]
    [Designer("TheTechIdea.Beep.Winform.Controls.Design.Server.Designers.BeepRibbonControlDesigner, TheTechIdea.Beep.Winform.Controls.Design.Server")]
    public partial class BeepRibbonControl : BaseControl
    {
        protected override Size DefaultSize => BeepLayoutMetrics.NavBar;
    }
}

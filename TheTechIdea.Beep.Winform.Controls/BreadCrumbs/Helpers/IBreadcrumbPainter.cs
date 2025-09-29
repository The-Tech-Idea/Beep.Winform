using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
 

namespace TheTechIdea.Beep.Winform.Controls.BreadCrumbs.Helpers
{
    internal interface IBreadcrumbPainter
    {
        void Initialize(BaseControl owner, IBeepTheme theme, Font textFont, bool showIcons);
        Rectangle CalculateItemRect(Graphics g, SimpleItem item, int x, int y, int height, bool isHovered);
        void DrawItem(Graphics g, BeepButton button, SimpleItem item, Rectangle rect, bool isHovered, bool isSelected, bool isLast);
        int DrawSeparator(Graphics g, BeepLabel label, int x, int y, int height, string separatorText, Font textFont, Color separatorColor, int itemSpacing);
    }
}

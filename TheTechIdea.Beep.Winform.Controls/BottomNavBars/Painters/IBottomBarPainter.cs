using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters
{
    internal interface IBottomBarPainter
    {
        string Name { get; }
        void Paint(BottomBarPainterContext context);
        void CalculateLayout(BottomBarPainterContext context);
        void RegisterHitAreas(BottomBarPainterContext context);
        void Dispose();
    }
}

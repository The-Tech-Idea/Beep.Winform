using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Company.Models;

namespace TheTechIdea.Beep.Winform.Controls.Company.Painters
{
    /// <summary>
    /// Computes the layout and draws one company profile view variant.
    /// </summary>
    internal interface ICompanyProfilePainter
    {
        CompanyProfileViewType ViewType { get; }
        Size PreferredSize { get; }
        CompanyProfileLayout Layout(Size bounds);
        void Draw(Graphics g, CompanyProfileLayout layout);
    }
}

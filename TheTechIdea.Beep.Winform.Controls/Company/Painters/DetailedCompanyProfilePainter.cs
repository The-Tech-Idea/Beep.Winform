using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Company.Models;

namespace TheTechIdea.Beep.Winform.Controls.Company.Painters
{
    internal sealed class DetailedCompanyProfilePainter : CompanyProfilePainterBase
    {
        public DetailedCompanyProfilePainter(BeepCompanyProfile owner) : base(owner) { }

        public override CompanyProfileViewType ViewType => CompanyProfileViewType.Detailed;

        public override Size PreferredSize => new Size(500, 350);

        public override CompanyProfileLayout Layout(Size bounds)
        {
            int padding = S(15);
            int logo = S(60);
            int gap = S(10);
            int wideGap = S(20);
            int btnW = S(80);
            int btnH = S(30);
            int menu = S(30);

            var layout = new CompanyProfileLayout
            {
                LogoRect = new Rectangle(padding, padding, logo, logo)
            };
            layout.NameRect = new Rectangle(padding, layout.LogoRect.Bottom + gap, bounds.Width - 2 * padding, S(30));
            layout.TypeRect = new Rectangle(padding, layout.NameRect.Bottom + gap, bounds.Width - 2 * padding, S(20));
            layout.DescriptionRect = new Rectangle(padding, layout.TypeRect.Bottom + gap, bounds.Width - 2 * padding, S(80));
            layout.LikesRect = new Rectangle(padding, layout.DescriptionRect.Bottom + gap, bounds.Width / 4, S(20));
            layout.WebsiteRect = new Rectangle(
                layout.LikesRect.Right + wideGap,
                layout.DescriptionRect.Bottom + gap,
                bounds.Width - layout.LikesRect.Right - padding - wideGap,
                S(20));
            layout.LikeButtonRect = new Rectangle(padding, layout.WebsiteRect.Bottom + wideGap, btnW, btnH);
            layout.ProfileButtonRect = new Rectangle(layout.LikeButtonRect.Right + gap, layout.WebsiteRect.Bottom + wideGap, btnW, btnH);
            layout.DropdownRect = new Rectangle(bounds.Width - padding - menu, padding, menu, menu);
            return layout;
        }
    }
}

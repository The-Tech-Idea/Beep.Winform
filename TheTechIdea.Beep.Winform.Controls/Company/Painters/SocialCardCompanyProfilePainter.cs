using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Company.Models;

namespace TheTechIdea.Beep.Winform.Controls.Company.Painters
{
    internal sealed class SocialCardCompanyProfilePainter : CompanyProfilePainterBase
    {
        public SocialCardCompanyProfilePainter(BeepCompanyProfile owner) : base(owner) { }

        public override CompanyProfileViewType ViewType => CompanyProfileViewType.SocialCard;

        public override Size PreferredSize => new Size(300, 230);

        public override CompanyProfileLayout Layout(Size bounds)
        {
            int padding = S(15);
            int logo = S(60);
            int gap = S(10);
            int smallGap = S(5);
            int btnW = S(60);
            int btnH = S(25);
            int menu = S(30);

            var layout = new CompanyProfileLayout
            {
                LogoRect = new Rectangle(padding, padding, logo, logo)
            };
            layout.NameRect = new Rectangle(padding, layout.LogoRect.Bottom + gap, bounds.Width - 2 * padding, S(25));
            layout.TypeRect = new Rectangle(padding, layout.NameRect.Bottom + smallGap, bounds.Width - 2 * padding, S(20));
            layout.DescriptionRect = new Rectangle(padding, layout.TypeRect.Bottom + smallGap, bounds.Width - 2 * padding, S(40));
            layout.LikesRect = new Rectangle(padding, layout.DescriptionRect.Bottom + gap, bounds.Width / 3 - padding, S(20));
            layout.WebsiteRect = new Rectangle(
                layout.LikesRect.Right + gap,
                layout.DescriptionRect.Bottom + gap,
                bounds.Width - layout.LikesRect.Right - padding - gap,
                S(20));
            layout.LikeButtonRect = new Rectangle(padding, layout.WebsiteRect.Bottom + gap, btnW, btnH);
            layout.ProfileButtonRect = new Rectangle(layout.LikeButtonRect.Right + gap, layout.WebsiteRect.Bottom + gap, btnW, btnH);
            layout.DropdownRect = new Rectangle(bounds.Width - padding - menu, padding, menu, menu);
            return layout;
        }
    }
}

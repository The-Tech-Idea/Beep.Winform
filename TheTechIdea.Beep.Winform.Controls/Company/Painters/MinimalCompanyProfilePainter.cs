using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Company.Models;

namespace TheTechIdea.Beep.Winform.Controls.Company.Painters
{
    internal sealed class MinimalCompanyProfilePainter : CompanyProfilePainterBase
    {
        public MinimalCompanyProfilePainter(BeepCompanyProfile owner) : base(owner) { }

        public override CompanyProfileViewType ViewType => CompanyProfileViewType.Minimal;

        public override Size PreferredSize => new Size(300, 200);

        public override void Draw(Graphics g, CompanyProfileLayout layout)
            => DrawMinimalView(g, layout);

        public override CompanyProfileLayout Layout(Size bounds)
        {
            int padding = S(15);
            int logo = S(60);
            int btnW = S(80);
            int btnH = S(30);

            var layout = new CompanyProfileLayout
            {
                LogoRect = new Rectangle((bounds.Width - logo) / 2, padding, logo, logo)
            };
            layout.NameRect = new Rectangle(padding, layout.LogoRect.Bottom + S(10), bounds.Width - 2 * padding, S(25));
            layout.ProfileButtonRect = new Rectangle((bounds.Width - btnW) / 2, layout.NameRect.Bottom + S(20), btnW, btnH);

            // Everything else stays Rectangle.Empty in the minimal view.
            return layout;
        }
    }
}

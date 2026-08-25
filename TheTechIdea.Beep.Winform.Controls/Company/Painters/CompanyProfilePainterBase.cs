using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Company.Models;

namespace TheTechIdea.Beep.Winform.Controls.Company.Painters
{
    /// <summary>
    /// Shared drawing logic for company profile painters. Concrete painters only supply
    /// the view type, the preferred size, and the layout rectangle computation.
    /// </summary>
    internal abstract class CompanyProfilePainterBase : ICompanyProfilePainter
    {
        protected readonly BeepCompanyProfile Owner;

        protected CompanyProfilePainterBase(BeepCompanyProfile owner)
        {
            Owner = owner;
        }

        public abstract CompanyProfileViewType ViewType { get; }
        public abstract Size PreferredSize { get; }
        public abstract CompanyProfileLayout Layout(Size bounds);

        public virtual void Draw(Graphics g, CompanyProfileLayout layout)
            => DrawStandardView(g, layout);

        /// <summary>Scales a design-time pixel value for the owner's current DPI.</summary>
        protected int S(int px) => DpiScalingHelper.ScaleValue(px, Owner);

        protected void DrawStandardView(Graphics g, CompanyProfileLayout layout)
        {
            if (!layout.LogoRect.IsEmpty && Owner.LogoButton != null)
                Owner.LogoButton.Draw(g, layout.LogoRect);

            if (!layout.NameRect.IsEmpty && Owner.CompanyNameLabel != null)
                Owner.CompanyNameLabel.Draw(g, layout.NameRect);

            if (!layout.TypeRect.IsEmpty && Owner.CompanyTypeLabel != null)
                Owner.CompanyTypeLabel.Draw(g, layout.TypeRect);

            if (!layout.DescriptionRect.IsEmpty && Owner.DescriptionBox != null)
                Owner.DescriptionBox.Draw(g, layout.DescriptionRect);

            if (!layout.LikesRect.IsEmpty && Owner.LikesLabel != null)
                Owner.LikesLabel.Draw(g, layout.LikesRect);

            DrawWebsite(g, layout);

            if (!layout.LikeButtonRect.IsEmpty && Owner.LikeButton != null)
                Owner.LikeButton.Draw(g, layout.LikeButtonRect);

            if (!layout.ProfileButtonRect.IsEmpty && Owner.ProfileButton != null)
                Owner.ProfileButton.Draw(g, layout.ProfileButtonRect);

            if (!layout.DropdownRect.IsEmpty && Owner.DropdownButton != null)
                Owner.DropdownButton.Draw(g, layout.DropdownRect);
        }

        protected void DrawMinimalView(Graphics g, CompanyProfileLayout layout)
        {
            if (!layout.LogoRect.IsEmpty && Owner.LogoButton != null)
                Owner.LogoButton.Draw(g, layout.LogoRect);

            if (!layout.NameRect.IsEmpty && Owner.CompanyNameLabel != null)
            {
                Owner.CompanyNameLabel.TextAlign = ContentAlignment.MiddleCenter;
                Owner.CompanyNameLabel.Draw(g, layout.NameRect);
            }

            if (!layout.ProfileButtonRect.IsEmpty && Owner.ProfileButton != null)
                Owner.ProfileButton.Draw(g, layout.ProfileButtonRect);
        }

        private void DrawWebsite(Graphics g, CompanyProfileLayout layout)
        {
            if (layout.WebsiteRect.IsEmpty)
                return;

            IBeepTheme theme = Owner.CurrentTheme;
            Color linkColor = theme.CompanyLinkColor != Color.Empty
                ? theme.CompanyLinkColor
                : theme.LinkColor;

            using (SolidBrush brush = new SolidBrush(linkColor))
            using (Font linkFont = ResolveLinkFont())
            {
                g.DrawString(Owner.Website, linkFont, brush, layout.WebsiteRect);
            }
        }

        private Font ResolveLinkFont()
        {
            IBeepTheme theme = Owner.CurrentTheme;

            if (Owner.UseThemeFont && theme.CompanyLinkFont != null)
                return FontListHelper.CreateFontFromTypography(theme.CompanyLinkFont);

            if (Owner.UseThemeFont && theme.LinkStyle != null)
                return BeepThemesManager.ToFont(theme.LinkStyle);

            return new Font("Segoe UI", 9, FontStyle.Underline);
        }
    }
}

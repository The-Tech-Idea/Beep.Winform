using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Company.Models
{
    /// <summary>
    /// Computed layout rectangles for one company profile view. A painter produces this
    /// structure once per layout pass; the control uses it for both drawing and hit testing.
    /// </summary>
    internal sealed class CompanyProfileLayout
    {
        public Rectangle LogoRect;
        public Rectangle NameRect;
        public Rectangle TypeRect;
        public Rectangle DescriptionRect;
        public Rectangle LikesRect;
        public Rectangle WebsiteRect;
        public Rectangle LikeButtonRect;
        public Rectangle ProfileButtonRect;
        public Rectangle DropdownRect;
    }
}

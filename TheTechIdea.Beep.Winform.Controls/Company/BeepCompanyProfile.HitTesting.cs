using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Company.Models;
using TheTechIdea.Beep.Winform.Controls.Diagnostics;

namespace TheTechIdea.Beep.Winform.Controls.Company
{
    public partial class BeepCompanyProfile : BaseControl
    {
        #region Hit Testing
        private void SetupHitTesting(CompanyProfileLayout layout)
        {
            ClearHitList();

            // Logo / like / profile are component-backed hit areas: associating the
            // template control forwards hover and click mouse events to it. The website
            // and dropdown entries carry a real action because they have side effects.
            AddHitArea("CompanyLogo", layout.LogoRect, btnCompanyLogo);
            AddHitArea("LikeButton", layout.LikeButtonRect, btnLikePage);
            AddHitArea("ProfileButton", layout.ProfileButtonRect, btnCompanyProfile);

            if (!layout.WebsiteRect.IsEmpty)
                AddHitArea("WebsiteLink", layout.WebsiteRect, null!, OpenWebsite);

            if (!layout.DropdownRect.IsEmpty)
                AddHitArea("DropdownMenu", layout.DropdownRect, btnDropdown, ShowDropdownMenu);
        }

        private void OpenWebsite()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Website.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                        ? Website
                        : "https://" + Website,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                BeepLog.Failure(this, "Opening company website", ex);
            }
        }

        private void ShowDropdownMenu()
        {
            if (dropdownMenu != null)
            {
                dropdownMenu.Show(this, new Point(_layout.DropdownRect.X, _layout.DropdownRect.Bottom));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Repaint so hover state forwarded to the drawn template controls is visible.
            Invalidate();
        }
        #endregion
    }
}

using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Company
{
    public partial class BeepCompanyProfile : BaseControl
    {
        #region Theme
        public override void ApplyTheme()
        {
            if (_currentTheme == null)
                return;

            base.ApplyTheme();

            // Company name label
            if (lblCompanyName != null)
            {
                lblCompanyName.Theme = Theme;
                lblCompanyName.ForeColor = _currentTheme.CompanyTitleColor != Color.Empty
                    ? _currentTheme.CompanyTitleColor
                    : _currentTheme.TitleStyle?.TextColor ?? _currentTheme.PrimaryTextColor;

                if (UseThemeFont)
                {
                    if (_currentTheme.CompanyTitleFont != null)
                        lblCompanyName.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.CompanyTitleFont);
                    else if (_currentTheme.TitleStyle != null)
                        lblCompanyName.TextFont = BeepThemesManager.ToFont(_currentTheme.TitleStyle);
                }
            }

            // Company type label
            if (lblCompanyType != null)
            {
                lblCompanyType.Theme = Theme;
                lblCompanyType.ForeColor = _currentTheme.CompanySubtitleColor != Color.Empty
                    ? _currentTheme.CompanySubtitleColor
                    : _currentTheme.SubtitleStyle?.TextColor ?? _currentTheme.SecondaryTextColor;

                if (UseThemeFont)
                {
                    if (_currentTheme.CompanySubTitleFont != null)
                        lblCompanyType.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.CompanySubTitleFont);
                    else if (_currentTheme.SubtitleStyle != null)
                        lblCompanyType.TextFont = BeepThemesManager.ToFont(_currentTheme.SubtitleStyle);
                }
            }

            // Company description box
            if (lblCompanyDesc != null)
            {
                lblCompanyDesc.Theme = Theme;
                lblCompanyDesc.ForeColor = _currentTheme.CompanyDescriptionColor != Color.Empty
                    ? _currentTheme.CompanyDescriptionColor
                    : _currentTheme.TextBoxForeColor;
                lblCompanyDesc.BackColor = _currentTheme.CompanyPopoverBackgroundColor != Color.Empty
                    ? _currentTheme.CompanyPopoverBackgroundColor
                    : _currentTheme.CardBackColor;

                if (UseThemeFont)
                {
                    if (_currentTheme.CompanyDescriptionFont != null)
                        lblCompanyDesc.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.CompanyDescriptionFont);
                    else if (_currentTheme.BodyStyle != null)
                        lblCompanyDesc.TextFont = BeepThemesManager.ToFont(_currentTheme.BodyStyle);
                }
            }

            // Likes label
            if (lblLikes != null)
            {
                lblLikes.Theme = Theme;
                lblLikes.ForeColor = _currentTheme.CompanyDescriptionColor != Color.Empty
                    ? _currentTheme.CompanyDescriptionColor
                    : _currentTheme.CaptionStyle?.TextColor ?? _currentTheme.SecondaryTextColor;

                if (UseThemeFont)
                {
                    if (_currentTheme.CompanyDescriptionFont != null)
                        lblLikes.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.CompanyDescriptionFont);
                    else if (_currentTheme.CaptionStyle != null)
                        lblLikes.TextFont = BeepThemesManager.ToFont(_currentTheme.CaptionStyle);
                }
            }

            // Like button
            if (btnLikePage != null)
            {
                btnLikePage.Theme = Theme;
                btnLikePage.BackColor = _currentTheme.CompanyButtonBackgroundColor != Color.Empty
                    ? _currentTheme.CompanyButtonBackgroundColor
                    : _currentTheme.ButtonBackColor;
                btnLikePage.ForeColor = _currentTheme.CompanyButtonTextColor != Color.Empty
                    ? _currentTheme.CompanyButtonTextColor
                    : _currentTheme.ButtonForeColor;

                if (UseThemeFont)
                {
                    if (_currentTheme.CompanyButtonFont != null)
                        btnLikePage.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.CompanyButtonFont);
                    else if (_currentTheme.ButtonStyle != null)
                        btnLikePage.TextFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
                }
            }

            // Profile button
            if (btnCompanyProfile != null)
            {
                btnCompanyProfile.Theme = Theme;
                btnCompanyProfile.BackColor = _currentTheme.CompanyButtonBackgroundColor != Color.Empty
                    ? _currentTheme.CompanyButtonBackgroundColor
                    : _currentTheme.ButtonBackColor;
                btnCompanyProfile.ForeColor = _currentTheme.CompanyButtonTextColor != Color.Empty
                    ? _currentTheme.CompanyButtonTextColor
                    : _currentTheme.ButtonForeColor;

                if (UseThemeFont)
                {
                    if (_currentTheme.CompanyButtonFont != null)
                        btnCompanyProfile.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.CompanyButtonFont);
                    else if (_currentTheme.ButtonStyle != null)
                        btnCompanyProfile.TextFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
                }
            }

            // Dropdown menu
            if (dropdownMenu != null)
            {
                dropdownMenu.BackColor = _currentTheme.CompanyDropdownBackgroundColor != Color.Empty
                    ? _currentTheme.CompanyDropdownBackgroundColor
                    : _currentTheme.MenuBackColor;
                dropdownMenu.ForeColor = _currentTheme.CompanyDropdownTextColor != Color.Empty
                    ? _currentTheme.CompanyDropdownTextColor
                    : _currentTheme.MenuForeColor;

                if (UseThemeFont)
                {
                    Font? menuFont = null;

                    if (_currentTheme.CompanyButtonFont != null)
                        menuFont = FontListHelper.CreateFontFromTypography(_currentTheme.CompanyButtonFont);
                    else if (_currentTheme.MenuItemUnSelectedFont != null)
                        menuFont = FontListHelper.CreateFontFromTypography(_currentTheme.MenuItemUnSelectedFont);

                    if (menuFont != null)
                    {
                        foreach (ToolStripItem item in dropdownMenu.Items)
                            item.Font = menuFont;
                    }
                }
            }

            // Dropdown button
            if (btnDropdown != null)
            {
                btnDropdown.Theme = Theme;
                btnDropdown.BackColor = _currentTheme.CompanyButtonBackgroundColor != Color.Empty
                    ? _currentTheme.CompanyButtonBackgroundColor
                    : _currentTheme.ButtonBackColor;
                btnDropdown.ForeColor = _currentTheme.CompanyButtonTextColor != Color.Empty
                    ? _currentTheme.CompanyButtonTextColor
                    : _currentTheme.ButtonForeColor;

                if (UseThemeFont)
                {
                    if (_currentTheme.CompanyButtonFont != null)
                        btnDropdown.TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.CompanyButtonFont);
                    else if (_currentTheme.ButtonStyle != null)
                        btnDropdown.TextFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
                }
            }

            // Company logo button
            if (btnCompanyLogo != null)
            {
                btnCompanyLogo.Theme = Theme;
                btnCompanyLogo.BackColor = _currentTheme.CompanyLogoBackgroundColor != Color.Empty
                    ? _currentTheme.CompanyLogoBackgroundColor
                    : _currentTheme.AccentColor;

                if (!string.IsNullOrEmpty(_logoPath))
                    btnCompanyLogo.ImagePath = _logoPath;
            }

            Invalidate();
        }
        #endregion
    }
}

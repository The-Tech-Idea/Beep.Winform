using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
 
 
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.TextFields;
using static TheTechIdea.Beep.Winform.Controls.Base.BaseControl;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.Helpers
{
    /// <summary>
    /// Creates and configures all BeepAppBar child components (buttons, search box, etc.)
    /// </summary>
    internal class BeepAppBarComponentFactory : IDisposable
    {
        private readonly IBeepAppBarHost _host;
        private bool _disposed = false;

        // Component instances
        public BeepImage Logo { get; private set; }
        public BeepLabel TitleLabel { get; private set; }
        public BeepTextBox SearchBox { get; private set; }
        public BeepButton ProfileButton { get; private set; }
        public BeepButton NotificationButton { get; private set; }
        public BeepButton CloseButton { get; private set; }
        public BeepButton MaximizeButton { get; private set; }
        public BeepButton MinimizeButton { get; private set; }
        public BeepButton ThemeButton { get; private set; }

        public BeepAppBarComponentFactory(IBeepAppBarHost host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            InitializeComponents();
        }

        #region "Initialization"

        private void InitializeComponents()
        {
            CreateLogo();
            CreateTitleLabel();
            CreateSearchBox();
            CreateButtons();
            ApplyTheme();
        }

        private void CreateLogo()
        {
            Logo = new BeepImage
            {
                Size = _host.LogoSize,
                ApplyThemeOnImage = false,
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                IsChild = true
            };

            if (!string.IsNullOrEmpty(_host.LogoImage))
            {
                Logo.ImagePath = _host.LogoImage;
            }
        }

        private void CreateTitleLabel()
        {
            TitleLabel = new BeepLabel
            {
                TextAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                IsFrameless = true,
                ShowAllBorders = false,
                Text = _host.Title,
                IsChild = true,
                ApplyThemeOnImage = false,
                UseScaledFont = false,
                TextFont = _host.TitleFont
            };
        }

        private void CreateSearchBox()
        {
            SearchBox = new BeepTextBox
            {
                Width = 300,
                Height = 32,
                Margin = new Padding(0),
                Padding = new Padding(0),
                Theme = _host.Theme,
                Text = string.Empty,
                ApplyThemeOnImage = true,
                PlaceholderText = "Search...",
                Anchor = AnchorStyles.Right,
                ApplyThemeToChilds = true,
                PainterKind = BaseControlPainterKind.Classic,
                IsFrameless = false,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                TextFont = _host.TextFont,
                TextAlignment = HorizontalAlignment.Left,
                ImageAlign = ContentAlignment.MiddleRight,
                Tag = "Search",
                Visible = false
            };

            SearchBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            SearchBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            SearchBox.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.079-search.svg";
        }

        private void CreateButtons()
        {
            int scaledWindowIconsHeight = _host.ScaleValue(40);
            int imageOffset = 2;

            // Create notification button
            NotificationButton = CreateButton(
                scaledWindowIconsHeight,
                imageOffset,
                "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.093-waving.svg"
            );
            NotificationButton.BadgeText = "1";
            NotificationButton.Visible = false;

            // Create profile button
            ProfileButton = CreateButton(
                scaledWindowIconsHeight,
                imageOffset,
                "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.025-user.svg"
            );
            ProfileButton.PopupMode = true;

            // Create theme button
            ThemeButton = CreateButton(
                scaledWindowIconsHeight,
                imageOffset,
                "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.024-dashboard.svg"
            );
            ThemeButton.PopupMode = true;

            // Populate theme menu items
            if (!_host.DesignMode)
            {
                try
                {
                    foreach (string themeName in BeepThemesManager.GetThemeNames())
                    {
                        ThemeButton.ListItems.Add(new SimpleItem { Text = themeName });
                    }
                }
                catch
                {
                    // Ignore in designer
                }
            }

            // Create window control buttons
            MinimizeButton = CreateButton(
                scaledWindowIconsHeight,
                imageOffset,
                "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.055-minimize.svg"
            );

            MaximizeButton = CreateButton(
                scaledWindowIconsHeight,
                imageOffset,
                "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.054-maximize.svg"
            );

            CloseButton = CreateButton(
                scaledWindowIconsHeight,
                imageOffset,
                "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.078-remove.svg"
            );
        }

        private BeepButton CreateButton(int iconSize, int imageOffset, string imagePath)
        {
            return new BeepButton
            {
                MaxImageSize = new Size(iconSize - imageOffset, iconSize - imageOffset),
                ImagePath = imagePath,
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ShowAllBorders = false,
                ApplyThemeOnImage = true,
                ShowShadow = false,
                IsChild = true,
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true
            };
        }

        #endregion

        #region "Update Methods"

        public void UpdateTitle(string title)
        {
            if (TitleLabel != null)
                TitleLabel.Text = title;
        }

        public void UpdateLogo(string logoPath)
        {
            if (Logo != null)
                Logo.ImagePath = logoPath;
        }

        public void UpdateSearchPlaceholder(string placeholder)
        {
            if (SearchBox != null)
                SearchBox.PlaceholderText = placeholder;
        }

        public void UpdateSearchText(string text)
        {
            if (SearchBox != null)
                SearchBox.Text = text;
        }

        public void UpdateComponentSizes()
        {
            int scaledIconSize = _host.ScaleValue(40);
            Size iconSize = new Size(scaledIconSize, scaledIconSize);
            Size imageSize = new Size(scaledIconSize - 2, scaledIconSize - 2);

            // Update all button sizes
            UpdateButtonSize(NotificationButton, iconSize, imageSize);
            UpdateButtonSize(ProfileButton, iconSize, imageSize);
            UpdateButtonSize(ThemeButton, iconSize, imageSize);
            UpdateButtonSize(MinimizeButton, iconSize, imageSize);
            UpdateButtonSize(MaximizeButton, iconSize, imageSize);
            UpdateButtonSize(CloseButton, iconSize, imageSize);

            // Update logo size
            if (Logo != null)
            {
                Logo.Size = _host.ScaleSize(_host.LogoSize);
            }

            // Update search box size
            if (SearchBox != null)
            {
                SearchBox.Height = _host.ScaleValue(30);
                SearchBox.Width = _host.ScaleValue(200);
            }
        }

        private void UpdateButtonSize(BeepButton button, Size iconSize, Size imageSize)
        {
            if (button != null)
            {
                button.MaxImageSize = imageSize;
                button.Size = iconSize;
            }
        }

        #endregion

        #region "Theme Application"

        public void ApplyTheme()
        {
            var currentTheme = _host.CurrentTheme;
            if (currentTheme == null) return;

            ApplyLogoTheme(currentTheme);
            ApplyTitleTheme(currentTheme);
            ApplySearchBoxTheme(currentTheme);
            ApplyButtonsTheme(currentTheme);
        }

        private void ApplyLogoTheme(IBeepTheme theme)
        {
            if (Logo == null) return;

            Logo.Theme = _host.Theme;
            Logo.BackColor = theme.AppBarBackColor;
            Logo.ParentBackColor = theme.AppBarBackColor;
            Logo.IsChild = true;
        }

        private void ApplyTitleTheme(IBeepTheme theme)
        {
            if (TitleLabel == null) return;

            TitleLabel.Theme = _host.Theme;
            TitleLabel.ForeColor = theme.AppBarTitleForeColor != Color.Empty
                ? theme.AppBarTitleForeColor : theme.ForeColor;
            TitleLabel.BackColor = theme.AppBarTitleBackColor != Color.Empty
                ? theme.AppBarTitleBackColor : theme.AppBarBackColor;
            TitleLabel.ParentBackColor = theme.AppBarBackColor;
            TitleLabel.IsChild = true;
            TitleLabel.TextFont = _host.TitleFont;
        }

        private void ApplySearchBoxTheme(IBeepTheme theme)
        {
            if (SearchBox == null) return;

            SearchBox.Theme = _host.Theme;
            SearchBox.BackColor = theme.AppBarTextBoxBackColor != Color.Empty
                ? theme.AppBarTextBoxBackColor : theme.TextBoxBackColor;
            SearchBox.ForeColor = theme.AppBarTextBoxForeColor != Color.Empty
                ? theme.AppBarTextBoxForeColor : theme.TextBoxForeColor;
            SearchBox.BorderColor = theme.BorderColor;
            SearchBox.HoverBackColor = theme.TextBoxHoverBackColor;
            SearchBox.HoverForeColor = theme.TextBoxHoverForeColor;
            SearchBox.TextFont = _host.TextFont;
            SearchBox.ParentBackColor = SearchBox.BackColor;
            SearchBox.IsChild = true;
        }

        private void ApplyButtonsTheme(IBeepTheme theme)
        {
            ApplyButtonTheme(NotificationButton, theme);
            ApplyButtonTheme(ProfileButton, theme);
            ApplyButtonTheme(ThemeButton, theme);
            ApplyButtonTheme(MinimizeButton, theme);
            ApplyButtonTheme(MaximizeButton, theme);
            ApplyButtonTheme(CloseButton, theme);

            // Apply specific colors for window buttons
            if (MinimizeButton != null && theme.AppBarMinButtonColor != Color.Empty)
                MinimizeButton.ForeColor = theme.AppBarMinButtonColor;

            if (MaximizeButton != null && theme.AppBarMaxButtonColor != Color.Empty)
                MaximizeButton.ForeColor = theme.AppBarMaxButtonColor;

            if (CloseButton != null && theme.AppBarCloseButtonColor != Color.Empty)
                CloseButton.ForeColor = theme.AppBarCloseButtonColor;
        }

        private void ApplyButtonTheme(BeepButton button, IBeepTheme theme)
        {
            if (button == null) return;

            button.Theme = _host.Theme;
            button.ImageEmbededin = ImageEmbededin.AppBar;
            button.BackColor = theme.AppBarButtonBackColor;
            button.ForeColor = theme.AppBarButtonForeColor;
            button.ParentBackColor = theme.AppBarBackColor;
            button.HoverBackColor = theme.ButtonHoverBackColor;
            button.HoverForeColor = theme.ButtonHoverForeColor;
            button.SelectedBackColor = theme.ButtonSelectedBackColor;
            button.SelectedForeColor = theme.ButtonSelectedForeColor;
            button.IsColorFromTheme = false;
            button.IsChild = true;
            button.ApplyThemeOnImage = true;

            // Ensure the inner image gets the correct theme
            button.ApplyThemeToSvg();
        }

        #endregion

        #region "Dispose"

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                // Dispose components (they will be handled by the main control's disposal)
                // We don't dispose them here as they might be added to the control's Controls collection
                _disposed = true;
            }
        }

        #endregion
    }
}
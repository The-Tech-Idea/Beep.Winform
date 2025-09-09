using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.Helpers
{
    /// <summary>
    /// Handles drawing coordination for all BeepAppBar components
    /// </summary>
    internal class BeepAppBarDrawingHelper
    {
        private readonly IBeepAppBarHost _host;
        private readonly BeepAppBarStateStore _stateStore;
        private readonly BeepAppBarLayoutHelper _layoutHelper;
        private readonly BeepAppBarComponentFactory _componentFactory;

        public BeepAppBarDrawingHelper(
            IBeepAppBarHost host,
            BeepAppBarStateStore stateStore,
            BeepAppBarLayoutHelper layoutHelper,
            BeepAppBarComponentFactory componentFactory)
        {
            _host = host;
            _stateStore = stateStore;
            _layoutHelper = layoutHelper;
            _componentFactory = componentFactory;
        }

        #region "Main Drawing Method"

        /// <summary>
        /// Main drawing coordination method
        /// </summary>
        public void DrawAll(Graphics g)
        {
            if (_host.DesignMode) return;

            // Ensure layout is current
            _layoutHelper.EnsureLayout();

            // Draw each component with proper hover states
            DrawLogo(g);
            DrawTitle(g);
            DrawSearchBox(g);
            DrawButtons(g);
        }

        #endregion

        #region "Component Drawing Methods"

        private void DrawLogo(Graphics g)
        {
            if (!_host.ShowLogo || string.IsNullOrEmpty(_host.LogoImage) || _componentFactory.Logo == null)
                return;

            var logoRect = _layoutHelper.GetLogoRect();
            if (logoRect.IsEmpty) return;

            _componentFactory.Logo.IsHovered = _stateStore.IsComponentHovered("Logo");
            _componentFactory.Logo.Draw(g, logoRect);
        }

        private void DrawTitle(Graphics g)
        {
            if (!_host.ShowTitle || _componentFactory.TitleLabel == null)
                return;

            var titleRect = _layoutHelper.GetTitleRect();
            if (titleRect.IsEmpty) return;

            _componentFactory.TitleLabel.IsHovered = _stateStore.IsComponentHovered("Title");
            _componentFactory.TitleLabel.Text = _host.Title; // Ensure text is current
            _componentFactory.TitleLabel.Draw(g, titleRect);
        }

        private void DrawSearchBox(Graphics g)
        {
            // Only draw search box directly when visible and not already added as a control
            if (!_host.ShowSearchBox || _stateStore.SearchBoxAddedToControls || _componentFactory.SearchBox == null)
                return;

            var searchRect = _layoutHelper.GetSearchRect();
            if (searchRect.IsEmpty) return;

            var searchBox = _componentFactory.SearchBox;
            searchBox.Width = searchRect.Width;
            searchBox.Height = searchRect.Height;
            searchBox.Left = searchRect.Left;
            searchBox.Top = searchRect.Top;
            searchBox.IsHovered = _stateStore.IsComponentHovered("Search");
            searchBox.Draw(g, searchRect);
        }

        private void DrawButtons(Graphics g)
        {
            DrawNotificationButton(g);
            DrawProfileButton(g);
            DrawThemeButton(g);
            DrawMinimizeButton(g);
            DrawMaximizeButton(g);
            DrawCloseButton(g);
        }

        private void DrawNotificationButton(Graphics g)
        {
            if (!_host.ShowNotificationIcon || _componentFactory.NotificationButton == null)
                return;

            var rect = _layoutHelper.GetNotificationRect();
            if (rect.IsEmpty) return;

            _componentFactory.NotificationButton.IsHovered = _stateStore.IsComponentHovered("Notification");
            _componentFactory.NotificationButton.Draw(g, rect);
        }

        private void DrawProfileButton(Graphics g)
        {
            if (!_host.ShowProfileIcon || _componentFactory.ProfileButton == null)
                return;

            var rect = _layoutHelper.GetProfileRect();
            if (rect.IsEmpty) return;

            _componentFactory.ProfileButton.IsHovered = _stateStore.IsComponentHovered("Profile");
            _componentFactory.ProfileButton.Draw(g, rect);
        }

        private void DrawThemeButton(Graphics g)
        {
            if (!_host.ShowThemeIcon || _componentFactory.ThemeButton == null)
                return;

            var rect = _layoutHelper.GetThemeRect();
            if (rect.IsEmpty) return;

            _componentFactory.ThemeButton.IsHovered = _stateStore.IsComponentHovered("Theme");
            _componentFactory.ThemeButton.Draw(g, rect);
        }

        private void DrawMinimizeButton(Graphics g)
        {
            if (!_host.ShowMinimizeIcon || _componentFactory.MinimizeButton == null)
                return;

            var rect = _layoutHelper.GetMinimizeRect();
            if (rect.IsEmpty) return;

            _componentFactory.MinimizeButton.IsHovered = _stateStore.IsComponentHovered("Minimize");
            _componentFactory.MinimizeButton.Draw(g, rect);
        }

        private void DrawMaximizeButton(Graphics g)
        {
            if (!_host.ShowMaximizeIcon || _componentFactory.MaximizeButton == null)
                return;

            var rect = _layoutHelper.GetMaximizeRect();
            if (rect.IsEmpty) return;

            _componentFactory.MaximizeButton.IsHovered = _stateStore.IsComponentHovered("Maximize");
            _componentFactory.MaximizeButton.Draw(g, rect);
        }

        private void DrawCloseButton(Graphics g)
        {
            if (!_host.ShowCloseIcon || _componentFactory.CloseButton == null)
                return;

            var rect = _layoutHelper.GetCloseRect();
            if (rect.IsEmpty) return;

            _componentFactory.CloseButton.IsHovered = _stateStore.IsComponentHovered("Close");
            _componentFactory.CloseButton.Draw(g, rect);
        }

        #endregion
    }
}
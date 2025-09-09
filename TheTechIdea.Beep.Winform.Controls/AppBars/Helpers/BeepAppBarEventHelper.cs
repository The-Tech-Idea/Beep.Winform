using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.Helpers
{
    /// <summary>
    /// Handles all mouse and keyboard events for BeepAppBar
    /// </summary>
    internal class BeepAppBarEventHelper
    {
        private readonly IBeepAppBarHost _host;
        private readonly BeepAppBarStateStore _stateStore;
        private readonly BeepAppBarLayoutHelper _layoutHelper;
        private readonly BeepAppBarComponentFactory _componentFactory;

        // Events that the helper will raise
        public event EventHandler<BeepMouseEventArgs> Clicked;
        public event EventHandler<BeepAppBarEventsArgs> OnButtonClicked;
        public event EventHandler<BeepAppBarEventsArgs> OnSearchBoxSelected;

        public BeepAppBarEventHelper(
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

        #region "Mouse Event Handling"

        /// <summary>
        /// Handles mouse click events
        /// </summary>
        public void HandleMouseClick(MouseEventArgs e)
        {
            if (_host.DesignMode) return;

            string componentName = _layoutHelper.GetComponentAtPoint(e.Location);
            if (string.IsNullOrEmpty(componentName)) return;

            switch (componentName)
            {
                case "Logo":
                    HandleLogoClick();
                    break;
                case "Title":
                    HandleTitleClick();
                    break;
                case "Search":
                    HandleSearchClick();
                    break;
                case "Notification":
                    HandleNotificationClick();
                    break;
                case "Profile":
                    HandleProfileClick();
                    break;
                case "Theme":
                    HandleThemeClick();
                    break;
                case "Minimize":
                    HandleMinimizeClick();
                    break;
                case "Maximize":
                    HandleMaximizeClick();
                    break;
                case "Close":
                    HandleCloseClick();
                    break;
            }
        }

        /// <summary>
        /// Handles mouse move events for hover states
        /// </summary>
        public Cursor HandleMouseMove(MouseEventArgs e)
        {
            if (_host.DesignMode) return Cursors.Default;

            string previousHovered = _stateStore.HoveredComponentName;
            string componentName = _layoutHelper.GetComponentAtPoint(e.Location);

            _stateStore.SetHoveredComponent(componentName);

            // Only redraw if hover state changed
            if (previousHovered != componentName)
            {
                _host.Invalidate();
            }

            // Return appropriate cursor
            if (!string.IsNullOrEmpty(componentName))
            {
                return Cursors.Hand;
            }

            return Cursors.Default;
        }

        /// <summary>
        /// Handles mouse leave events
        /// </summary>
        public void HandleMouseLeave()
        {
            if (_stateStore.HoveredComponentName != null)
            {
                _stateStore.ClearHover();
                _host.Invalidate();
            }
        }

        #endregion

        #region "Component Click Handlers"

        private void HandleLogoClick()
        {
            var arg = new BeepAppBarEventsArgs("Logo");
            OnButtonClicked?.Invoke(this, arg);
            Clicked?.Invoke(this, new BeepMouseEventArgs("Logo", _componentFactory.Logo));
        }

        private void HandleTitleClick()
        {
            var arg = new BeepAppBarEventsArgs("Title");
            OnButtonClicked?.Invoke(this, arg);
            Clicked?.Invoke(this, new BeepMouseEventArgs("Title", _componentFactory.TitleLabel));
        }

        private void HandleSearchClick()
        {
            var searchRect = _layoutHelper.GetSearchRect();
            if (searchRect.IsEmpty) return;

            var searchBox = _componentFactory.SearchBox;
            if (!_stateStore.SearchBoxAddedToControls)
            {
                // Position and add the search box to controls
                searchBox.SetBounds(searchRect.Left, searchRect.Top, searchRect.Width, searchRect.Height);
                searchBox.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                searchBox.EnableMaterialStyle = false;
                searchBox.Visible = true;
                
                // Add to parent control
                var parentControl = _host.AsControl;
                parentControl.Controls.Add(searchBox);
                _stateStore.SearchBoxAddedToControls = true;

                // Focus the search box
                searchBox.Focus();
                searchBox.SelectAll();

                // Register for lost focus
                searchBox.LostFocus += SearchBox_LostFocus;
            }
            else
            {
                searchBox.SetBounds(searchRect.Left, searchRect.Top, searchRect.Width, searchRect.Height);
                searchBox.Focus();
                searchBox.SelectAll();
            }

            // Trigger event
            var arg = new BeepAppBarEventsArgs("Search");
            arg.Selectedstring = searchBox.Text;
            OnSearchBoxSelected?.Invoke(this, arg);
        }

        private void SearchBox_LostFocus(object sender, EventArgs e)
        {
            var searchBox = _componentFactory.SearchBox;
            if (!searchBox.Focused)
            {
                _host.AsControl.BeginInvoke(new Action(() =>
                {
                    if (!searchBox.Focused)
                    {
                        RemoveSearchBoxControl();
                    }
                }));
            }
        }

        private void RemoveSearchBoxControl()
        {
            try
            {
                var searchBox = _componentFactory.SearchBox;
                var parentControl = _host.AsControl;
                
                if (_stateStore.SearchBoxAddedToControls && parentControl.Controls.Contains(searchBox))
                {
                    searchBox.LostFocus -= SearchBox_LostFocus;
                    parentControl.Controls.Remove(searchBox);
                    _stateStore.SearchBoxAddedToControls = false;
                    searchBox.Visible = false;
                    _host.Invalidate();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RemoveSearchBoxControl error: {ex.Message}");
            }
        }

        private void HandleNotificationClick()
        {
            var arg = new BeepAppBarEventsArgs("Notifications", _componentFactory.NotificationButton);
            OnButtonClicked?.Invoke(this, arg);
            Clicked?.Invoke(this, new BeepMouseEventArgs("Notifications", _componentFactory.NotificationButton));

            MessageBox.Show("Showing notifications");
        }

        private void HandleProfileClick()
        {
            var arg = new BeepAppBarEventsArgs("Profile", _componentFactory.ProfileButton);
            OnButtonClicked?.Invoke(this, arg);
            Clicked?.Invoke(this, new BeepMouseEventArgs("Profile", _componentFactory.ProfileButton));

            // Profile menu handling would be done by BeepAppBarMenuHelper
        }

        private void HandleThemeClick()
        {
            var arg = new BeepAppBarEventsArgs("Theme", _componentFactory.ThemeButton);
            OnButtonClicked?.Invoke(this, arg);
            Clicked?.Invoke(this, new BeepMouseEventArgs("Theme", _componentFactory.ThemeButton));

            // Theme menu handling would be done by BeepAppBarMenuHelper
        }

        private void HandleMinimizeClick()
        {
            Form form = _host.AsControl.FindForm();
            if (form != null)
            {
                form.WindowState = FormWindowState.Minimized;
            }
        }

        private void HandleMaximizeClick()
        {
            Form form = _host.AsControl.FindForm();
            if (form != null)
            {
                form.WindowState = form.WindowState == FormWindowState.Maximized
                    ? FormWindowState.Normal
                    : FormWindowState.Maximized;
            }
        }

        private void HandleCloseClick()
        {
            var form = _host.AsControl.FindForm();
            if (form == null) return;

            var args = new FormClosingEventArgs(CloseReason.UserClosing, false);

            if (!args.Cancel)
            {
                form.Close();
            }
        }

        #endregion

        #region "Search Box Management"

        /// <summary>
        /// Safely removes the search box control during resize operations
        /// </summary>
        public void RemoveSearchBoxControlSafe()
        {
            if (!_stateStore.SearchBoxAddedToControls) return;

            try
            {
                var parentControl = _host.AsControl;
                if (parentControl.InvokeRequired)
                {
                    parentControl.BeginInvoke(new Action(RemoveSearchBoxControlSafe));
                    return;
                }

                parentControl.SuspendLayout();

                var searchBox = _componentFactory.SearchBox;
                if (searchBox != null && parentControl.Controls.Contains(searchBox))
                {
                    searchBox.LostFocus -= SearchBox_LostFocus;
                    parentControl.Controls.Remove(searchBox);
                }

                _stateStore.SearchBoxAddedToControls = false;
                if (searchBox != null)
                {
                    searchBox.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RemoveSearchBoxControlSafe error: {ex.Message}");
            }
            finally
            {
                _host.AsControl.ResumeLayout(false);
                _host.Invalidate();
            }
        }

        #endregion
    }
}
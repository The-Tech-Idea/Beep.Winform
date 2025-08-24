using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Forms.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Forms
{
    public partial class BeepFormAdvanced : Form
    {
        #region Private Fields
        private FormUIStyle _uiStyle = FormUIStyle.Modern;
        private int _titleBarHeight = 44;
        private bool _showTitleBar = true;
        private bool _showStatusBar = false;
        private bool _showIcon = true;
        private ContentAlignment _titleAlignment = ContentAlignment.MiddleLeft;
        private bool _enableShadow = true;
        private bool _enableGlow = true;

        private bool _showMinimizeButton = true;
        private bool _showMaximizeButton = true;
        private bool _showCloseButton = true;

        // Modern theme colors
        private Color _titleBarColor = Color.FromArgb(32, 32, 32);
        private Color _titleBarGradientStart = Color.FromArgb(48, 48, 48);
        private Color _titleBarGradientEnd = Color.FromArgb(24, 24, 24);
        private Color _titleTextColor = Color.White;
        private Color _contentBackColor = Color.FromArgb(250, 250, 250);
        private Color _statusBarColor = Color.FromArgb(240, 240, 240);
        private Color _borderColor = Color.FromArgb(64, 64, 64);

        // Helper instances
        private FormComponentsAccessor _componentsAccessor;
        private FormUIStyleHelper _styleHelper;
        private FormButtonStyleHelper _buttonHelper;
        private FormThemeHelper _themeHelper;
        private FormWindowHelper _windowHelper;

        // Theme management
        protected IBeepTheme _currentTheme = BeepThemesManager.GetDefaultTheme();
        private string _theme;
        #endregion

        #region Constructor and Initialization
        public BeepFormAdvanced()
        {
            InitializeComponent();
            InitializeHelpers();
            InitializeModernChrome();
            SetupDragDropSupport();
        }

        private void InitializeHelpers()
        {
            _componentsAccessor = new FormComponentsAccessor(this);
            _styleHelper = new FormUIStyleHelper(this, _componentsAccessor);
            _buttonHelper = new FormButtonStyleHelper(this, _componentsAccessor);
            _themeHelper = new FormThemeHelper(this, _componentsAccessor);
            _windowHelper = new FormWindowHelper(this, _componentsAccessor);
        }

        private void InitializeModernChrome()
        {
            SetupTitleBarEvents();
            SetupCaptionButtons();
            ApplyUIStyle();
            
            _windowHelper.SetupWindowBehavior();
            
            if (!DesignMode)
            {
                Load += OnFormLoad;
                Resize += OnFormResize;
                Paint += OnFormPaint;
            }

            // Keep title in sync
            TextChanged += (s, e) => { if (_titleLabel != null) _titleLabel.Text = Text; };
            
            // Update icon when available
            if (Icon != null && _appIcon != null)
            {
                try { _appIcon.Image = Icon.ToBitmap(); } catch { }
            }

            // Setup drag-drop after all components are initialized
            if (!DesignMode)
            {
                SetupDragDropSupport();
            }
        }
        #endregion

        #region Core Methods
        private void ApplyUIStyle()
        {
            _styleHelper.ApplyUIStyle(_uiStyle, ref _titleBarHeight);
            _buttonHelper.SetupButtons(_uiStyle, _titleBarHeight);
            
            // Apply theme colors after layout if theme is set
            if (_currentTheme != null && !string.IsNullOrEmpty(_theme))
            {
                ApplyThemeColorsToStyle();
            }
            
            Invalidate();
        }

        private void ApplyThemeColorsToStyle()
        {
            var colorScheme = _themeHelper.ApplyThemeToStyle(_uiStyle, _currentTheme);
            ApplyColorScheme(colorScheme);
            
            // Apply theme to controls and buttons
            _themeHelper.ApplyThemeToControls(_currentTheme, _theme);
            _themeHelper.ApplyThemeToButtons(_uiStyle, _currentTheme, _theme);
            
            // Apply theme to the overall form
            BackColor = _currentTheme.BackgroundColor;
            ForeColor = _currentTheme.PrimaryTextColor;
            BorderColor = _currentTheme.BorderColor;
            ShadowColor = _currentTheme.ShadowColor;
            GradientStartColor = _currentTheme.GradientStartColor;
            GradientEndColor = _currentTheme.GradientEndColor;
        }

        private void ApplyColorScheme(FormColorScheme colorScheme)
        {
            _titleBarColor = colorScheme.TitleBarColor;
            _titleBarGradientStart = colorScheme.TitleBarGradientStart;
            _titleBarGradientEnd = colorScheme.TitleBarGradientEnd;
            _titleTextColor = colorScheme.TitleTextColor;
            _contentBackColor = colorScheme.ContentBackColor;
            _statusBarColor = colorScheme.StatusBarColor;
            _borderColor = colorScheme.BorderColor;
            
            ApplyColors();
        }

        private void ApplyColors()
        {
            if (_titleBar != null) _titleBar.BackColor = _titleBarColor;
            if (_titleLabel != null) _titleLabel.ForeColor = _titleTextColor;
            if (_contentHost != null) _contentHost.BackColor = _contentBackColor;
            if (_statusBar != null) _statusBar.BackColor = _statusBarColor;
        }

        private void UpdateButtonsForStyle()
        {
            _buttonHelper.UpdateButtonsForStyle(_uiStyle);
            SetupCaptionButtons();
        }

        public virtual void ApplyTheme()
        {
            try
            {
                if (_currentTheme == null) return;
                
                // Apply base theme properties
                ForeColor = _currentTheme.PrimaryTextColor;
                BackColor = _currentTheme.BackgroundColor;
                BorderColor = _currentTheme.BorderColor;
                ShadowColor = _currentTheme.ShadowColor;
                GradientStartColor = _currentTheme.GradientStartColor;
                GradientEndColor = _currentTheme.GradientEndColor;
                
                // Apply theme to current UI style
                if (!string.IsNullOrEmpty(_theme))
                {
                    ApplyThemeColorsToStyle();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Event Handlers
        private void SetupTitleBarEvents()
        {
            if (_titleBar != null)
            {
                _titleBar.Paint += OnTitleBarPaint;
                _titleBar.MouseDown += OnTitleBarMouseDown;
                _titleBar.DoubleClick += OnTitleBarDoubleClick;
            }
        }

        private void SetupCaptionButtons()
        {
            if (_btnMin != null)
            {
                _btnMin.Click -= OnMinimizeClick;
                _btnMin.Click += OnMinimizeClick;
                _btnMin.Cursor = Cursors.Hand;
                _btnMin.TabStop = false;
            }

            if (_btnMax != null)
            {
                _btnMax.Click -= OnMaximizeClick;
                _btnMax.Click += OnMaximizeClick;
                _btnMax.Cursor = Cursors.Hand;
                _btnMax.TabStop = false;
            }

            if (_btnClose != null)
            {
                _btnClose.Click -= OnCloseClick;
                _btnClose.Click += OnCloseClick;
                _btnClose.Cursor = Cursors.Hand;
                _btnClose.TabStop = false;
            }
        }

        private void OnMinimizeClick(object sender, EventArgs e)
        {
            MinimizeClicked?.Invoke(this, EventArgs.Empty);
            WindowState = FormWindowState.Minimized;
        }

        private void OnMaximizeClick(object sender, EventArgs e)
        {
            MaximizeClicked?.Invoke(this, EventArgs.Empty);
            
            if (_uiStyle == FormUIStyle.Floating)
            {
                return; // Floating style doesn't maximize
            }
            
            _windowHelper.ToggleMaximizeRestore();
        }

        private void OnCloseClick(object sender, EventArgs e)
        {
            CloseClicked?.Invoke(this, EventArgs.Empty);
            Close();
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            _windowHelper.ApplyWindowEffects(_enableShadow, _enableGlow);
            _buttonHelper.UpdateMaximizeButton(_uiStyle, WindowState);
        }

        private void OnFormResize(object sender, EventArgs e)
        {
            _buttonHelper.UpdateMaximizeButton(_uiStyle, WindowState);
        }

        private void OnTitleBarMouseDown(object sender, MouseEventArgs e)
        {
            _windowHelper.HandleTitleBarMouseDown(e);
        }

        private void OnTitleBarDoubleClick(object sender, EventArgs e)
        {
            _windowHelper.HandleTitleBarDoubleClick();
        }

        private void OnTitleBarPaint(object sender, PaintEventArgs e)
        {
            _windowHelper.DrawTitleBarGradient(e, _uiStyle, _titleBarGradientStart, _titleBarGradientEnd, _borderColor);
        }

        private void OnFormPaint(object sender, PaintEventArgs e)
        {
            _windowHelper.DrawFormBorder(e, _uiStyle, _borderColor);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                return _windowHelper.ModifyCreateParams(cp);
            }
        }
        #endregion

        #region Internal Component Access for Helpers
        internal BeepPanel GetTitleBar() => _titleBar;
        internal PictureBox GetAppIcon() => _appIcon;
        internal BeepLabel GetTitleLabel() => _titleLabel;
        internal BeepPanel GetTitleRightHost() => _titleRightHost;
        internal BeepButton GetBtnMin() => _btnMin;
        internal BeepButton GetBtnMax() => _btnMax;
        internal BeepButton GetBtnClose() => _btnClose;
        internal BeepPanel GetContentHost() => _contentHost;
        internal BeepPanel GetStatusBar() => _statusBar;
        internal BeepLabel GetStatusLabel() => _statusLabel;
        internal bool GetDesignMode() => DesignMode;
        internal void SetFormStyle(ControlStyles styles, bool value) => SetStyle(styles, value);
        #endregion
    }
}

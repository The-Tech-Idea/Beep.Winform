using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Forms
{
    public partial class BeepFormAdvanced
    {
        #region Public Properties
        [Browsable(true)]
        [TypeConverter(typeof(ThemeEnumConverter))]
        public string Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                _currentTheme = BeepThemesManager.GetTheme(value);
                ApplyTheme();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(FormUIStyle.Modern)]
        [Description("Sets the visual style of the form chrome")]
        [TypeConverter(typeof(FormUIStyleConverter))]
        public FormUIStyle UIStyle
        {
            get => _uiStyle;
            set { 
                if (_uiStyle != value)
                {
                    _uiStyle = value; 
                    ApplyUIStyle();
                    UpdateButtonsForStyle();
                  //  _buttonHelper.UpdateMaximizeButton(_uiStyle, WindowState);
                    
                    if (DesignMode)
                    {
                        Invalidate();
                        Update();
                    }
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ShowTitleBar
        {
            get => _showTitleBar;
            set { _showTitleBar = value; if (_titleBar != null) _titleBar.Visible = value; }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(44)]
        public int TitleBarHeight
        {
            get => _titleBarHeight;
            set { 
                _titleBarHeight = Math.Max(28, value); 
                if (_titleBar != null) _titleBar.Height = _titleBarHeight;
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ShowIcon
        {
            get => _showIcon;
            set { _showIcon = value; if (_appIcon != null) _appIcon.Visible = value; }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(ContentAlignment.MiddleLeft)]
        public ContentAlignment TitleAlignment
        {
            get => _titleAlignment;
            set { _titleAlignment = value; if (_titleLabel != null) _titleLabel.TextAlign = value; }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool ShowStatusBar
        {
            get => _showStatusBar;
            set { _showStatusBar = value; if (_statusBar != null) _statusBar.Visible = value; }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        public string StatusText
        {
            get => _statusLabel?.Text ?? "";
            set { if (_statusLabel != null) _statusLabel.Text = value; }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool EnableShadow
        {
            get => _enableShadow;
            set { _enableShadow = value; if (!DesignMode) _windowHelper.ApplyWindowEffects(_enableShadow, _enableGlow); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool EnableGlow
        {
            get => _enableGlow;
            set { _enableGlow = value; if (!DesignMode) _windowHelper.ApplyWindowEffects(_enableShadow, _enableGlow); }
        }

        // Caption buttons visibility
        [Browsable(true)]
        [Category("Window Buttons")]
        [DefaultValue(true)]
        public bool ShowMinimizeButton
        {
            get => _showMinimizeButton;
            set { _showMinimizeButton = value; if (_btnMin != null) _btnMin.Visible = value; }
        }

        [Browsable(true)]
        [Category("Window Buttons")]
        [DefaultValue(true)]
        public bool ShowMaximizeButton
        {
            get => _showMaximizeButton;
            set { _showMaximizeButton = value; if (_btnMax != null) _btnMax.Visible = value; }
        }

        [Browsable(true)]
        [Category("Window Buttons")]
        [DefaultValue(true)]
        public bool ShowCloseButton
        {
            get => _showCloseButton;
            set { _showCloseButton = value; if (_btnClose != null) _btnClose.Visible = value; }
        }

        // Modern color properties
        [Browsable(true)]
        [Category("Modern Colors")]
        public Color TitleBarColor
        {
            get => _titleBarColor;
            set { 
                _titleBarColor = value; 
                if (_titleBar != null) _titleBar.BackColor = value;
            }
        }

        [Browsable(true)]
        [Category("Modern Colors")]
        public Color TitleTextColor
        {
            get => _titleTextColor;
            set { _titleTextColor = value; if (_titleLabel != null) _titleLabel.ForeColor = value; }
        }

        [Browsable(true)]
        [Category("Modern Colors")]
        public Color ContentBackColor
        {
            get => _contentBackColor;
            set { 
                _contentBackColor = value; 
                if (_contentHost != null) _contentHost.BackColor = value;
            }
        }

        [Browsable(true)]
        [Category("Modern Colors")]
        public Color StatusBarColor
        {
            get => _statusBarColor;
            set { 
                _statusBarColor = value; 
                if (_statusBar != null) _statusBar.BackColor = value;
            }
        }

        // Events
        public event EventHandler MinimizeClicked;
        public event EventHandler MaximizeClicked;
        public event EventHandler CloseClicked;

        [Browsable(true)]
        [Category("Layout")]
        [Description("The main content panel where you add your controls")]
        public BeepPanel ContentPanel => _contentHost;

        // Additional properties for helpers
        public Color BorderColor { get; private set; }
        public Color ShadowColor { get; private set; }
        public Color GradientStartColor { get; private set; }
        public Color GradientEndColor { get; private set; }
        #endregion
    }
}
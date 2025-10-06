using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using TheTechIdea.Beep.Vis.Modules;
 
using TheTechIdea.Beep.Winform.Controls.Base;
 
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.AppBars
{
    /// <summary>
    /// Material 3 Design compliant top app bar component
    /// </summary>
    public enum Material3AppBarVariant
    {
        /// <summary>Small top app bar - 64dp height</summary>
        Small,
        /// <summary>Center-aligned top app bar - 64dp height</summary>
        CenterAligned,
        /// <summary>Medium top app bar - 112dp height</summary>
        Medium,
        /// <summary>Large top app bar - 152dp height</summary>
        Large
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Material 3 AppBar")]
    [Category("Beep Controls")]
    [Description("Material Design 3 compliant top app bar with navigation, actions, and overflow menu")]
    public class BeepMaterial3AppBar : BaseControl
    {
        #region Fields and Constants

        // Material 3 sizing specifications (in dp)
        private const int MD3_SMALL_HEIGHT = 64;
        private const int MD3_MEDIUM_HEIGHT = 112;
        private const int MD3_LARGE_HEIGHT = 152;
        private const int MD3_ICON_SIZE = 24;
        private const int MD3_TOUCH_TARGET = 48;
        private const int MD3_START_PADDING = 16;
        private const int MD3_END_PADDING = 16;
        private const int MD3_ICON_SPACING = 16;
        private const int MD3_SEARCH_HEIGHT = 40;

        // Components
        private BeepImage _navigationIcon;
        private BeepLabel _titleLabel;
        private BeepTextBox _searchBox;
        private BeepButton _actionButton1;
        private BeepButton _actionButton2;
        private BeepButton _actionButton3;
        private BeepButton _overflowButton;

        // Layout rectangles
        private Rectangle _navigationRect;
        private Rectangle _titleRect;
        private Rectangle _searchRect;
        private Rectangle _action1Rect;
        private Rectangle _action2Rect;
        private Rectangle _action3Rect;
        private Rectangle _overflowRect;

        // State
        private Material3AppBarVariant _variant = Material3AppBarVariant.Small;
        private string _title = "App Title";
        private bool _showSearch = false;
        private bool _isSearchActive = false;
        private string _navigationIconPath = "";
        private bool _showDivider = false;

        // Dropdown menu
        private BeepPopupListForm _overflowMenu;
        private BindingList<SimpleItem> _overflowMenuItems = new BindingList<SimpleItem>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Material 3 app bar variant
        /// </summary>
        [Browsable(true)]
        [Category("Material Design")]
        [Description("The Material 3 top app bar variant")]
        [DefaultValue(Material3AppBarVariant.Small)]
        public Material3AppBarVariant Variant
        {
            get => _variant;
            set
            {
                if (_variant != value)
                {
                    _variant = value;
                    ApplyVariantSettings();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the app bar title
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The title displayed in the app bar")]
        [DefaultValue("App Title")]
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    if (_titleLabel != null)
                        _titleLabel.Text = _title;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show the search field
        /// </summary>
        [Browsable(true)]
        [Category("Features")]
        [Description("Show or hide the search field")]
        [DefaultValue(false)]
        public bool ShowSearch
        {
            get => _showSearch;
            set
            {
                if (_showSearch != value)
                {
                    _showSearch = value;
                    if (_searchBox != null)
                        _searchBox.Visible = _showSearch;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the navigation icon path
        /// </summary>
        [Browsable(true)]
        [Category("Icons")]
        [Description("SVG path for the navigation icon (typically menu or back)")]
        public string NavigationIconPath
        {
            get => _navigationIconPath;
            set
            {
                if (_navigationIconPath != value)
                {
                    _navigationIconPath = value;
                    if (_navigationIcon != null)
                        _navigationIcon.ImagePath = _navigationIconPath;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show a bottom divider
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show a subtle divider line at the bottom")]
        [DefaultValue(false)]
        public bool ShowDivider
        {
            get => _showDivider;
            set
            {
                if (_showDivider != value)
                {
                    _showDivider = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the overflow menu items
        /// </summary>
        [Browsable(false)]
        public BindingList<SimpleItem> OverflowMenuItems
        {
            get => _overflowMenuItems;
            set
            {
                _overflowMenuItems = value ?? new BindingList<SimpleItem>();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets the search box component for direct access
        /// </summary>
        [Browsable(false)]
        public BeepTextBox SearchBox => _searchBox;

        #endregion

        #region Events

        /// <summary>
        /// Fired when the navigation icon is clicked
        /// </summary>
        public event EventHandler NavigationClicked;

        /// <summary>
        /// Fired when an action button is clicked
        /// </summary>
        public event EventHandler<ActionButtonClickedEventArgs> ActionButtonClicked;

        /// <summary>
        /// Fired when search text changes
        /// </summary>
        public event EventHandler<SearchEventArgs> SearchChanged;

        /// <summary>
        /// Fired when overflow menu item is selected
        /// </summary>
        public event EventHandler<SelectedItemChangedEventArgs> OverflowItemSelected;

        #endregion

        #region Constructor

        public BeepMaterial3AppBar() : base()
        {
            // Set up base control properties
            Size = new Size(800, ScaleValue(MD3_SMALL_HEIGHT));
            
            // Initialize components
            InitializeComponents();
            
            // Set up layout
            ApplyVariantSettings();
            
            // Apply default theme
            ApplyTheme();
        }

        #endregion

        #region Component Initialization

        private void InitializeComponents()
        {
            var touchTarget = ScaleValue(MD3_TOUCH_TARGET);
            var iconSize = ScaleValue(MD3_ICON_SIZE);

            // Navigation icon
            _navigationIcon = new BeepImage
            {
                Size = new Size(iconSize, iconSize),
                IsChild = true,
                ApplyThemeOnImage = true,
                ImagePath = NavigationIconPath
            };

            // Title label
            _titleLabel = new BeepLabel
            {
                Text = _title,
                TextAlign = ContentAlignment.MiddleLeft,
                IsChild = true,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                AutoSize = false
            };

            // Search field
            _searchBox = new BeepTextBox
            {
                Height = ScaleValue(MD3_SEARCH_HEIGHT),
                PlaceholderText = "Search...",
                Visible = _showSearch,
                IsChild = true,
                //EnableMaterialStyle = true,
                MaterialAutoSizeCompensation = false,
                MaterialPreserveContentArea = true,
                MaterialUseVariantPadding = false,
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                StylePreset = MaterialTextFieldStylePreset.DenseOutlined,
                TrailingIconPath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.079-search.svg"
            };

            // Action buttons
            _actionButton1 = CreateActionButton();
            _actionButton2 = CreateActionButton();
            _actionButton3 = CreateActionButton();

            // Overflow button (three dots)
            _overflowButton = CreateActionButton();
            _overflowButton.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.016-more-vertical.svg";

            // Set up event handlers
            SetupEventHandlers();
        }

        private BeepButton CreateActionButton()
        {
            var touchTarget = ScaleValue(MD3_TOUCH_TARGET);
            var iconSize = ScaleValue(MD3_ICON_SIZE);

            return new BeepButton
            {
                Size = new Size(touchTarget, touchTarget),
                MaxImageSize = new Size(iconSize, iconSize),
                IsFrameless = true,
                ShowAllBorders = false,
                ShowShadow = false,
                IsChild = true,
                ApplyThemeOnImage = true,
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true,
                Visible = false
            };
        }

        private void SetupEventHandlers()
        {
            // Navigation icon click
            _navigationIcon.Click += (s, e) => NavigationClicked?.Invoke(this, EventArgs.Empty);

            // Search field events
            _searchBox.TextChanged += (s, e) => 
                SearchChanged?.Invoke(this, new SearchEventArgs(_searchBox.Text));

            // Action button events
            _actionButton1.Click += (s, e) => 
                ActionButtonClicked?.Invoke(this, new ActionButtonClickedEventArgs(1));
            _actionButton2.Click += (s, e) => 
                ActionButtonClicked?.Invoke(this, new ActionButtonClickedEventArgs(2));
            _actionButton3.Click += (s, e) => 
                ActionButtonClicked?.Invoke(this, new ActionButtonClickedEventArgs(3));

            // Overflow button click
            _overflowButton.Click += (s, e) => ShowOverflowMenu();
        }

        #endregion

        #region Layout and Drawing

        private void ApplyVariantSettings()
        {
            int height = _variant switch
            {
                Material3AppBarVariant.Small => ScaleValue(MD3_SMALL_HEIGHT),
                Material3AppBarVariant.CenterAligned => ScaleValue(MD3_SMALL_HEIGHT),
                Material3AppBarVariant.Medium => ScaleValue(MD3_MEDIUM_HEIGHT),
                Material3AppBarVariant.Large => ScaleValue(MD3_LARGE_HEIGHT),
                _ => ScaleValue(MD3_SMALL_HEIGHT)
            };

            Height = height;

            // Update title alignment for center-aligned variant
            if (_titleLabel != null)
            {
                _titleLabel.TextAlign = _variant == Material3AppBarVariant.CenterAligned
                    ? ContentAlignment.MiddleCenter
                    : ContentAlignment.MiddleLeft;
            }
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            // Calculate layout
            CalculateLayout();

            // Draw background (handled by base)
            
            // Draw components
            DrawComponents(g);

            // Draw divider if enabled
            if (_showDivider)
            {
                DrawDivider(g);
            }
        }

        private void CalculateLayout()
        {
            var startPadding = ScaleValue(MD3_START_PADDING);
            var endPadding = ScaleValue(MD3_END_PADDING);
            var iconSpacing = ScaleValue(MD3_ICON_SPACING);
            var touchTarget = ScaleValue(MD3_TOUCH_TARGET);
            var iconSize = ScaleValue(MD3_ICON_SIZE);

            var rect = DrawingRect;
            var centerY = rect.Y + rect.Height / 2;

            // Start from left edge
            var currentX = rect.X + startPadding;

            // Navigation icon
            if (!string.IsNullOrEmpty(_navigationIconPath))
            {
                _navigationRect = new Rectangle(
                    currentX,
                    centerY - touchTarget / 2,
                    touchTarget,
                    touchTarget
                );
                currentX = _navigationRect.Right + iconSpacing;
            }

            // Calculate right edge positions working backwards
            var rightX = rect.Right - endPadding;

            // Overflow button
            _overflowRect = new Rectangle(
                rightX - touchTarget,
                centerY - touchTarget / 2,
                touchTarget,
                touchTarget
            );
            rightX = _overflowRect.Left - iconSpacing;

            // Action buttons (work backwards)
            if (_actionButton3.Visible)
            {
                _action3Rect = new Rectangle(
                    rightX - touchTarget,
                    centerY - touchTarget / 2,
                    touchTarget,
                    touchTarget
                );
                rightX = _action3Rect.Left - iconSpacing;
            }

            if (_actionButton2.Visible)
            {
                _action2Rect = new Rectangle(
                    rightX - touchTarget,
                    centerY - touchTarget / 2,
                    touchTarget,
                    touchTarget
                );
                rightX = _action2Rect.Left - iconSpacing;
            }

            if (_actionButton1.Visible)
            {
                _action1Rect = new Rectangle(
                    rightX - touchTarget,
                    centerY - touchTarget / 2,
                    touchTarget,
                    touchTarget
                );
                rightX = _action1Rect.Left - iconSpacing;
            }

            // Search field or title
            if (_showSearch && _isSearchActive)
            {
                _searchRect = new Rectangle(
                    currentX,
                    centerY - ScaleValue(MD3_SEARCH_HEIGHT) / 2,
                    rightX - currentX - iconSpacing,
                    ScaleValue(MD3_SEARCH_HEIGHT)
                );
            }
            else
            {
                // Title area
                if (_variant == Material3AppBarVariant.CenterAligned)
                {
                    // Center the title
                    var titleWidth = Math.Min(ScaleValue(200), rightX - currentX - iconSpacing);
                    var titleX = currentX + (rightX - currentX - titleWidth) / 2;
                    _titleRect = new Rectangle(
                        titleX,
                        centerY - ScaleValue(28) / 2,
                        titleWidth,
                        ScaleValue(28)
                    );
                }
                else
                {
                    // Left-aligned title
                    _titleRect = new Rectangle(
                        currentX,
                        centerY - ScaleValue(28) / 2,
                        rightX - currentX - iconSpacing,
                        ScaleValue(28)
                    );
                }
            }
        }

        private void DrawComponents(Graphics g)
        {
            // Draw navigation icon
            if (!string.IsNullOrEmpty(_navigationIconPath) && _navigationIcon != null)
            {
                _navigationIcon.Draw(g, _navigationRect);
            }

            // Draw title or search
            if (_showSearch && _isSearchActive)
            {
                _searchBox?.Draw(g, _searchRect);
            }
            else if (_titleLabel != null)
            {
                _titleLabel.Draw(g, _titleRect);
            }

            // Draw action buttons
            if (_actionButton1.Visible)
                _actionButton1.Draw(g, _action1Rect);
            if (_actionButton2.Visible)
                _actionButton2.Draw(g, _action2Rect);
            if (_actionButton3.Visible)
                _actionButton3.Draw(g, _action3Rect);

            // Draw overflow button
            _overflowButton?.Draw(g, _overflowRect);
        }

        private void DrawDivider(Graphics g)
        {
            var dividerColor = _currentTheme?.BorderColor ?? Color.FromArgb(31, 31, 31);
            using (var pen = new Pen(dividerColor, 1))
            {
                var y = DrawingRect.Bottom - 1;
                g.DrawLine(pen, DrawingRect.Left, y, DrawingRect.Right, y);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets an action button's icon and visibility
        /// </summary>
        public void SetActionButton(int buttonIndex, string iconPath, bool visible = true)
        {
            var button = buttonIndex switch
            {
                1 => _actionButton1,
                2 => _actionButton2,
                3 => _actionButton3,
                _ => throw new ArgumentException("Button index must be 1, 2, or 3")
            };

            button.ImagePath = iconPath;
            button.Visible = visible;
            Invalidate();
        }

        /// <summary>
        /// Activates the search mode
        /// </summary>
        public void ActivateSearch()
        {
            _isSearchActive = true;
            _showSearch = true;
            if (_searchBox != null)
            {
                _searchBox.Visible = true;
                _searchBox.Focus();
            }
            Invalidate();
        }

        /// <summary>
        /// Deactivates the search mode
        /// </summary>
        public void DeactivateSearch()
        {
            _isSearchActive = false;
            if (_searchBox != null)
            {
                _searchBox.Text = "";
                _searchBox.Visible = _showSearch;
            }
            Invalidate();
        }

        /// <summary>
        /// Adds an item to the overflow menu
        /// </summary>
        public void AddOverflowMenuItem(string text, string iconPath = "")
        {
            _overflowMenuItems.Add(new SimpleItem { Text = text, ImagePath = iconPath });
        }

        /// <summary>
        /// Shows the overflow menu
        /// </summary>
        public void ShowOverflowMenu()
        {
            if (_overflowMenuItems.Count == 0) return;

            _overflowMenu?.Close();

            _overflowMenu = new BeepPopupListForm(_overflowMenuItems.ToList())
            {
                Theme = Theme,
                ShowTitle = false
            };

            _overflowMenu.SelectedItemChanged += (s, e) =>
            {
                OverflowItemSelected?.Invoke(this, e);
                _overflowMenu.Close();
            };

            var screenLocation = PointToScreen(new Point(_overflowRect.Right, _overflowRect.Bottom));
            _overflowMenu.ShowPopup(this, screenLocation);
        }

        #endregion

        #region Theme Support

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_currentTheme == null) return;

            // Apply theme to components
            ApplyThemeToComponents();
        }

        private void ApplyThemeToComponents()
        {
            var appBarBack = _currentTheme.AppBarBackColor != Color.Empty 
                ? _currentTheme.AppBarBackColor 
                : _currentTheme.BackColor;
            var appBarFore = _currentTheme.AppBarForeColor != Color.Empty 
                ? _currentTheme.AppBarForeColor 
                : _currentTheme.ForeColor;

            // Set app bar colors
            BackColor = appBarBack;
            ForeColor = appBarFore;

            // Apply to navigation icon
            if (_navigationIcon != null)
            {
                _navigationIcon.Theme = Theme;
                _navigationIcon.ParentBackColor = appBarBack;
            }

            // Apply to title
            if (_titleLabel != null)
            {
                _titleLabel.Theme = Theme;
                _titleLabel.ForeColor = _currentTheme.AppBarTitleForeColor != Color.Empty 
                    ? _currentTheme.AppBarTitleForeColor 
                    : appBarFore;
                _titleLabel.BackColor = appBarBack;
                _titleLabel.ParentBackColor = appBarBack;
            }

            // Apply to search field
            if (_searchBox != null)
            {
                _searchBox.Theme = Theme;
                _searchBox.ParentBackColor = appBarBack;
                _searchBox.BackColor = _currentTheme.AppBarTextBoxBackColor != Color.Empty
                    ? _currentTheme.AppBarTextBoxBackColor 
                    : _currentTheme.TextBoxBackColor;
                _searchBox.ForeColor = _currentTheme.AppBarTextBoxForeColor != Color.Empty
                    ? _currentTheme.AppBarTextBoxForeColor 
                    : _currentTheme.TextBoxForeColor;
            }

            // Apply to action buttons
            ApplyThemeToButton(_actionButton1);
            ApplyThemeToButton(_actionButton2);
            ApplyThemeToButton(_actionButton3);
            ApplyThemeToButton(_overflowButton);
        }

        private void ApplyThemeToButton(BeepButton button)
        {
            if (button == null) return;

            button.Theme = Theme;
            button.BackColor = _currentTheme.AppBarButtonBackColor;
            button.ForeColor = _currentTheme.AppBarButtonForeColor;
            button.ParentBackColor = _currentTheme.AppBarBackColor;
            button.HoverBackColor = _currentTheme.ButtonHoverBackColor;
            button.HoverForeColor = _currentTheme.ButtonHoverForeColor;
            button.ApplyThemeOnImage = true;
            button.ApplyThemeToSvg();
        }

        #endregion

        #region Event Args Classes

        public class ActionButtonClickedEventArgs : EventArgs
        {
            public int ButtonIndex { get; }

            public ActionButtonClickedEventArgs(int buttonIndex)
            {
                ButtonIndex = buttonIndex;
            }
        }

        public class SearchEventArgs : EventArgs
        {
            public string SearchText { get; }

            public SearchEventArgs(string searchText)
            {
                SearchText = searchText;
            }
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public partial class BeepiFormPro
    {
        private const int WM_NCLBUTTONDOWN = 0x00A1;

        #region Layout Caching
        // Layout caching to prevent recalculating on every paint
        private Size _lastLayoutSize = Size.Empty;
        private bool _layoutDirty = true;
        private FormStyle _lastLayoutStyle = FormStyle.Modern;
        private bool _lastShowCaptionBar = true;
        private bool _lastShowCloseButton = true;
        private bool _lastShowMinMaxButtons = true;
        private bool _lastShowThemeButton = false;
        private bool _lastShowStyleButton = false;
        private bool _lastShowProfileButton = false;
        private bool _lastShowSearchBox = false;
        private bool _lastShowCustomActionButton = false;

        /// <summary>
        /// Ensures layout is calculated only when necessary (size changed, style changed, etc.)
        /// This dramatically improves performance by avoiding redundant calculations on every paint.
        /// </summary>
        private void EnsureLayoutCalculated()
        {
            // CRITICAL: Skip layout calculation if form has invalid dimensions
            // This prevents ArgumentException during form initialization or when size is zero
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0)
                return;

            // Check if layout needs recalculation
            bool needsRecalc = _layoutDirty 
                || _lastLayoutSize != ClientSize 
                || _lastLayoutStyle != FormStyle
                || _lastShowCaptionBar != ShowCaptionBar
                || _lastShowCloseButton != ShowCloseButton
                || _lastShowMinMaxButtons != ShowMinMaxButtons
                || _lastShowThemeButton != ShowThemeButton
                || _lastShowStyleButton != ShowStyleButton
                || _lastShowProfileButton != ShowProfileButton
                || _lastShowSearchBox != ShowSearchBox
                || _lastShowCustomActionButton != ShowCustomActionButton;

            if (!needsRecalc)
                return;

            // Ensure managers exist
            if (_hits == null)
                _hits = new BeepiFormProHitAreaManager(this);
            if (_interact == null)
                _interact = new BeepiFormProInteractionManager(this, _hits);

            // Ensure painter exists
            if (ActivePainter == null)
            {
                try { ApplyFormStyle(); } catch { }
            }

            if (ActivePainter == null)
                return;

            // Clear hit areas - painters will register their own hit areas
            _hits.Clear();
            
            bool layoutSucceeded = false;
            try
            {
                // Let the painter calculate layout and register all hit areas
                // Painters are responsible for registering caption, buttons, etc.
                ActivePainter.CalculateLayoutAndHitAreas(this);
                layoutSucceeded = true;

                // DL-01: Set form Padding to match ContentRect so DisplayRectangle
                // returns only the usable content area. Child controls will NOT overlap
                // the caption bar or borders.
                var cr = CurrentLayout.ContentRect;

                // Inset by the form's border width so docked children stay inside
                // the visible border. Respects SafeContentInsets for rounded-corner
                // styles (iOS, macOS, Ubuntu, etc.) which need deeper insets.
                int borderW = GetCurrentMetrics().BorderWidth;
                int safeL  = Math.Max(borderW, CurrentLayout.SafeContentInsets.Left);
                int safeR  = Math.Max(borderW, CurrentLayout.SafeContentInsets.Right);
                int safeB  = Math.Max(borderW, CurrentLayout.SafeContentInsets.Bottom);
                cr = new Rectangle(
                    cr.X + safeL,
                    cr.Y,
                    Math.Max(0, cr.Width  - safeL - safeR),
                    Math.Max(0, cr.Height - safeB));

                if (!cr.IsEmpty && cr.Width > 0 && cr.Height > 0)
                {
                    Padding = new Padding(
                        cr.Left - ClientRectangle.Left,
                        cr.Top - ClientRectangle.Top,
                        ClientRectangle.Right - cr.Right,
                        ClientRectangle.Bottom - cr.Bottom);
                }
            }
            catch
            {
                // CRITICAL: If the painter throws, do NOT mark layout as clean.
                // Keep _layoutDirty so the next EnsureLayoutCalculated() call retries.
                // Otherwise hit areas remain empty and buttons (close, minimize, etc.) stop working.
            }

            // NOTE: Do NOT register caption hit area here - painters handle it themselves
            // Registering it twice causes hit test issues

            // Only update cache tracking if layout calculation succeeded
            // If it failed, leave _layoutDirty true so we retry on next call
            if (layoutSucceeded)
            {
                RegisterInteractiveRegions();
                _lastLayoutSize = ClientSize;
                _lastLayoutStyle = FormStyle;
                _lastShowCaptionBar = ShowCaptionBar;
                _lastShowCloseButton = ShowCloseButton;
                _lastShowMinMaxButtons = ShowMinMaxButtons;
                _lastShowThemeButton = ShowThemeButton;
                _lastShowStyleButton = ShowStyleButton;
                _lastShowProfileButton = ShowProfileButton;
                _lastShowSearchBox = ShowSearchBox;
                _lastShowCustomActionButton = ShowCustomActionButton;
                _layoutDirty = false;
                ClearKeyboardCaptionFocusIfTargetMissing();
            }
        }

        /// <summary>
        /// Marks the layout as dirty, forcing recalculation on next paint.
        /// Also clears the cached BorderShape path so it is rebuilt with the new size/style.
        /// Call this when properties that affect layout change.
        /// </summary>
        public void InvalidateLayout()
        {
            _layoutDirty = true;
            // Dispose the cached border path and force it to regenerate on the next access.
            // Just clearing the cache keys (size/style) without disposing the old object
            // leaks a GDI+ GraphicsPath handle every time the style or size changes.
            if (_cachedBorderShape != null)
            {
                _cachedBorderShape.Dispose();
                _cachedBorderShape = null;
            }
            _cachedBorderShapeSize  = Size.Empty;
            _cachedBorderShapeStyle = (FormStyle)(-1);
        }

        /// <summary>
        /// Recalculates caption/layout hit areas, rebuilds cached <see cref="BorderShape"/>,
        /// and reapplies Win32 + managed regions after client bounds have settled (not during live resize ticks).
        /// Border strokes remain owned by <see cref="IFormPainter.PaintBorders"/>; this only syncs geometry they draw on.
        /// </summary>
        private void RefreshChromeGeometryAfterBoundsSettled()
        {
            if (!IsHandleCreated || ClientSize.Width <= 0 || ClientSize.Height <= 0)
                return;

            InvalidateLayout();
            try { EnsureLayoutCalculated(); } catch { }

            UpdateWindowRegion();
            UpdateFormRegion();

            Invalidate(true);
        }
        #endregion

        private bool InDesignModeSafe =>
           LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
           (Site?.DesignMode ?? false);
        // Theme properties
        private string _theme = "DefaultTheme";
        [Browsable(true)]
        [TypeConverter(typeof(ThemeConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Theme
        {
            get => _theme;
            set
            {
                if (string.Equals(value, _theme, StringComparison.Ordinal) && _currentTheme != null)
                    return;

                ApplyResolvedTheme(BeepThemesManager.GetTheme(value) ?? BeepThemesManager.GetDefaultTheme());
            }
        }
        private Color _bordercolor= Color.LightGray;
        public Color BorderColor
        {
            get => _bordercolor;
            set
            {
                if (_bordercolor != value)
                {
                    _bordercolor = value;
                    Invalidate();
                }
            }
        }

        // Style properties
        private FormStyle _formstyle = FormStyle.Modern;
        /// <summary>Guard that prevents cascading/re-entrant FormStyle applications
        /// (e.g. when the global FormStyleChanged event fires while we are
        /// already inside the setter).</summary>
        private bool _isApplyingFormStyle = false;

        public FormStyle FormStyle
        {
            get => _formstyle;
            set
            {
                if (_formstyle != value && !_isApplyingFormStyle)
                {
                    _isApplyingFormStyle = true;
                    try
                    {
                        _formstyle = value;
                        // Dispose old managed region when style changes (corner radius may change)
                        if (IsHandleCreated && Region != null)
                        {
                            var oldRegion = Region;
                            Region = null;
                            oldRegion.Dispose();
                        }
                        InvalidateLayout(); // Clear cached BorderShape BEFORE recalculating regions
                        ApplyFormStyle(); // This calls UpdateWindowRegion() + UpdateFormRegion() with fresh cache
                        DebouncedInvalidate();
                    }
                    finally
                    {
                        _isApplyingFormStyle = false;
                    }
                }
            }
        }

        // UseThemeColors property - when true, metrics use theme colors; when false, use default skin colors
        private bool _useThemeColors = true;
        [System.ComponentModel.Category("Beep Theme")]
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Description("When true, uses theme colors from the current theme. When false, uses default skin-specific colors.")]
        public bool UseThemeColors
        {
            get => _useThemeColors;
            set
            {
                if (_useThemeColors != value)
                {
                    _useThemeColors = value;
                    // Reset metrics to force recalculation with new color mode
                    _formpaintermaterics = null;
                    Invalidate();
                }
            }
        }
        
        // Painters - Hidden from designer to prevent serialization
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public IFormPainter ActivePainter { get; set; }
        
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public List<IFormPainter> Painters { get; } = new();

        // Regions API
        private readonly List<FormRegion> _regions = new();

        // Managers
        internal BeepiFormProHitAreaManager _hits;
        internal BeepiFormProInteractionManager _interact;

        // Current layout info calculated by the active painter - Hidden from designer
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public PainterLayoutInfo CurrentLayout { get;  set; } = new();

        // Built-in caption elements
        public FormRegion _iconRegion;
        private FormRegion _titleRegion;
        private FormRegion _minimizeButton;
        private FormRegion _maximizeButton;
        private FormRegion _closeButton;
        public FormRegion _customActionButton; // New: custom clickable region
        public FormRegion _themeButton;
        public FormRegion _styleButton;
        private FormRegion _profileButton;
        private FormRegion _searchBox;
        
       

        // Internal properties for manager access
        internal FormRegion IconRegion => _iconRegion;
        internal FormRegion TitleRegion => _titleRegion;
        internal FormRegion MinimizeButton => _minimizeButton;
        internal FormRegion MaximizeButton => _maximizeButton;
        internal FormRegion CloseButton => _closeButton;
        internal FormRegion CustomActionButton => _customActionButton;
        internal FormRegion ThemeButton => _themeButton;
        internal FormRegion StyleButton => _styleButton;
        internal FormRegion ProfileButton => _profileButton;
        internal FormRegion SearchBox => _searchBox;
        internal List<FormRegion> Regions => _regions;

        // Button visibility flags
        private bool _showThemeButton = false;
        private bool _showStyleButton = false;
        private bool _showProfileButton = false;
        private bool _showSearchBox = false;
        private bool _showCloseButton = true;
        private bool _showMinMaxButtons = true;
        private bool _showCustomActionButton = false;
        
        // Search box state
        private string _searchText = string.Empty;
        private bool _searchBoxFocused = false;
        private string _keyboardFocusedCaptionAreaName = string.Empty;
        private string _baseAccessibleName = string.Empty;
        private string _baseAccessibleDescription = string.Empty;
        private string _baseAccessibleDefaultActionDescription = string.Empty;
        private AccessibleRole _baseAccessibleRole = AccessibleRole.Default;
        private bool _captionAccessibilitySnapshotCaptured = false;
        private bool _highContrastMode = false;
        private bool _screenReaderSupport = true;
        private FocusIndicatorStyle _focusIndicatorStyle = FocusIndicatorStyle.Subtle;

        // Events for region interaction
        public event EventHandler<RegionEventArgs> RegionHover;
        public event EventHandler<RegionEventArgs> RegionClick;
        
        // Events for button actions
        public event EventHandler ThemeButtonClicked;
        public event EventHandler StyleButtonClicked;
        public event EventHandler ProfileButtonClicked;
        public event EventHandler<string> SearchBoxTextChanged;

        private bool HasKeyboardCaptionFocus => !string.IsNullOrWhiteSpace(_keyboardFocusedCaptionAreaName);

        private IEnumerable<FormRegion> GetInteractiveRegions()
        {
            return _regions.Where(region =>
                region != null
                && region.IsInteractive
                && region.IsEnabled
                && !string.IsNullOrWhiteSpace(region.Id));
        }

        private static string GetInteractiveRegionHitAreaName(FormRegion region)
        {
            if (region == null || string.IsNullOrWhiteSpace(region.Id))
                return string.Empty;

            return $"region:user:{region.Id.Trim()}";
        }

        private void RegisterInteractiveRegions()
        {
            foreach (var region in GetInteractiveRegions())
            {
                Rectangle bounds = ResolveRegionRect(region);
                if (bounds.Width <= 0 || bounds.Height <= 0)
                    continue;

                _hits.RegisterHitArea(GetInteractiveRegionHitAreaName(region), bounds, region);
            }
        }

        private FormRegion FindCustomCaptionRegion(string areaName, out Rectangle bounds)
        {
            string key = BeepiFormProHitAreaManager.NormalizeName(areaName);
            foreach (var region in GetInteractiveRegions())
            {
                if (region.Dock != RegionDock.Caption)
                    continue;

                if (BeepiFormProHitAreaManager.NormalizeName(GetInteractiveRegionHitAreaName(region)) != key)
                    continue;

                bounds = ResolveRegionRect(region);
                if (bounds.Width <= 0 || bounds.Height <= 0)
                    continue;

                return region;
            }

            bounds = Rectangle.Empty;
            return null;
        }

        private void AddKeyboardCaptionTarget(List<(string Name, Rectangle Bounds)> targets, string areaName, bool isVisible, Rectangle bounds)
        {
            if (!isVisible || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            targets.Add((BeepiFormProHitAreaManager.NormalizeName(areaName), bounds));
        }

        private List<(string Name, Rectangle Bounds)> GetKeyboardCaptionTargets()
        {
            try { EnsureLayoutCalculated(); } catch { }

            var targets = new List<(string Name, Rectangle Bounds)>();
            AddKeyboardCaptionTarget(targets, FormHitAreaNames.Search, ShowSearchBox, CurrentLayout.SearchBoxRect);
            AddKeyboardCaptionTarget(targets, FormHitAreaNames.Profile, ShowProfileButton, CurrentLayout.ProfileButtonRect);
            AddKeyboardCaptionTarget(targets, FormHitAreaNames.CustomAction, ShowCustomActionButton, CurrentLayout.CustomActionButtonRect);
            AddKeyboardCaptionTarget(targets, FormHitAreaNames.Theme, ShowThemeButton, CurrentLayout.ThemeButtonRect);
            AddKeyboardCaptionTarget(targets, FormHitAreaNames.Style, ShowStyleButton, CurrentLayout.StyleButtonRect);
            AddKeyboardCaptionTarget(targets, FormHitAreaNames.Minimize, ShowMinMaxButtons, CurrentLayout.MinimizeButtonRect);
            AddKeyboardCaptionTarget(targets, FormHitAreaNames.Maximize, ShowMinMaxButtons, CurrentLayout.MaximizeButtonRect);
            AddKeyboardCaptionTarget(targets, FormHitAreaNames.Close, ShowCloseButton, CurrentLayout.CloseButtonRect);

            foreach (var region in GetInteractiveRegions())
            {
                if (region.Dock != RegionDock.Caption)
                    continue;

                string areaName = GetInteractiveRegionHitAreaName(region);
                string normalizedId = BeepiFormProHitAreaManager.NormalizeName(areaName);
                if (targets.Any(target => target.Name == normalizedId))
                    continue;

                AddKeyboardCaptionTarget(targets, areaName, true, ResolveRegionRect(region));
            }

            return targets
                .OrderBy(target => target.Bounds.Left)
                .ThenBy(target => target.Bounds.Top)
                .ToList();
        }

        private bool IsKeyboardCaptionTargetCurrentlyVisible(string areaName)
        {
            if (!ShowCaptionBar)
                return false;

            string key = BeepiFormProHitAreaManager.NormalizeName(areaName);
            if (FindCustomCaptionRegion(key, out var customBounds) != null)
                return customBounds.Width > 0 && customBounds.Height > 0;

            Rectangle bounds = GetCaptionAreaBounds(key);
            bool hasBounds = bounds.Width > 0 && bounds.Height > 0;

            return key switch
            {
                FormHitAreaNames.Search => ShowSearchBox && hasBounds,
                FormHitAreaNames.Profile => ShowProfileButton && hasBounds,
                FormHitAreaNames.CustomAction => ShowCustomActionButton && hasBounds,
                FormHitAreaNames.Theme => ShowThemeButton && hasBounds,
                FormHitAreaNames.Style => ShowStyleButton && hasBounds,
                FormHitAreaNames.Minimize => ShowMinMaxButtons && hasBounds,
                FormHitAreaNames.Maximize => ShowMinMaxButtons && hasBounds,
                FormHitAreaNames.Close => ShowCloseButton && hasBounds,
                _ => false
            };
        }

        private void ClearKeyboardCaptionFocusIfTargetMissing()
        {
            string current = BeepiFormProHitAreaManager.NormalizeName(_keyboardFocusedCaptionAreaName);
            if (string.IsNullOrWhiteSpace(current) && !_searchBoxFocused)
                return;

            if (!IsKeyboardCaptionTargetCurrentlyVisible(current))
                ClearKeyboardCaptionFocus();
        }

        private Rectangle GetCaptionAreaBounds(string areaName)
        {
            string key = BeepiFormProHitAreaManager.NormalizeName(areaName);
            return key switch
            {
                FormHitAreaNames.Search => CurrentLayout.SearchBoxRect,
                FormHitAreaNames.Profile => CurrentLayout.ProfileButtonRect,
                FormHitAreaNames.CustomAction => CurrentLayout.CustomActionButtonRect,
                FormHitAreaNames.Theme => CurrentLayout.ThemeButtonRect,
                FormHitAreaNames.Style => CurrentLayout.StyleButtonRect,
                FormHitAreaNames.Minimize => CurrentLayout.MinimizeButtonRect,
                FormHitAreaNames.Maximize => CurrentLayout.MaximizeButtonRect,
                FormHitAreaNames.Close => CurrentLayout.CloseButtonRect,
                _ => FindCustomCaptionRegion(key, out var bounds) != null ? bounds : Rectangle.Empty
            };
        }

        private Rectangle GetKeyboardFocusedCaptionBounds()
            => GetCaptionAreaBounds(_keyboardFocusedCaptionAreaName);

        private void InvalidateKeyboardCaptionFocus(Rectangle oldBounds, Rectangle newBounds, bool forceCaptionRefresh = false)
        {
            Rectangle invalidRect = Rectangle.Empty;

            if (!oldBounds.IsEmpty)
                invalidRect = oldBounds;

            if (!newBounds.IsEmpty)
                invalidRect = invalidRect.IsEmpty ? newBounds : Rectangle.Union(invalidRect, newBounds);

            if (!invalidRect.IsEmpty)
            {
                Invalidate(invalidRect, false);
            }
            else if (forceCaptionRefresh && !CurrentLayout.CaptionRect.IsEmpty)
            {
                Invalidate(CurrentLayout.CaptionRect, false);
            }
        }

        private bool MoveKeyboardCaptionFocus(bool forward)
        {
            var targets = GetKeyboardCaptionTargets();
            if (targets.Count == 0)
                return false;

            string current = BeepiFormProHitAreaManager.NormalizeName(_keyboardFocusedCaptionAreaName);
            int currentIndex = string.IsNullOrWhiteSpace(current)
                ? -1
                : targets.FindIndex(target => target.Name == current);

            int nextIndex;
            if (currentIndex < 0)
            {
                nextIndex = forward ? 0 : targets.Count - 1;
            }
            else
            {
                nextIndex = (currentIndex + (forward ? 1 : -1) + targets.Count) % targets.Count;
            }

            SetKeyboardFocusedCaptionArea(targets[nextIndex].Name, focusForm: true);
            return true;
        }

        private void SetKeyboardFocusedCaptionArea(string areaName, bool focusForm)
        {
            string normalized = BeepiFormProHitAreaManager.NormalizeName(areaName);
            var targets = GetKeyboardCaptionTargets();
            var target = targets.FirstOrDefault(item => item.Name == normalized);
            if (string.IsNullOrWhiteSpace(target.Name))
            {
                ClearKeyboardCaptionFocus();
                return;
            }

            Rectangle oldBounds = GetKeyboardFocusedCaptionBounds();
            bool searchFocusChanged = _searchBoxFocused != (normalized == FormHitAreaNames.Search);

            _keyboardFocusedCaptionAreaName = normalized;
            _searchBoxFocused = normalized == FormHitAreaNames.Search;

            if (focusForm && CanFocus && !Focused)
                Focus();

            InvalidateKeyboardCaptionFocus(oldBounds, target.Bounds, searchFocusChanged);
            UpdateCaptionAccessibilityFocus(normalized);
        }

        private void ClearKeyboardCaptionFocus()
        {
            Rectangle oldBounds = GetKeyboardFocusedCaptionBounds();
            bool hadSearchFocus = _searchBoxFocused;

            if (!HasKeyboardCaptionFocus && !hadSearchFocus)
                return;

            _keyboardFocusedCaptionAreaName = string.Empty;
            _searchBoxFocused = false;

            RestoreCaptionAccessibilityFocus();
            InvalidateKeyboardCaptionFocus(oldBounds, Rectangle.Empty, hadSearchFocus);
        }

        private bool ActivateKeyboardFocusedCaptionArea()
        {
            if (!HasKeyboardCaptionFocus)
                return false;

            if (_keyboardFocusedCaptionAreaName == FormHitAreaNames.Search)
            {
                SetKeyboardFocusedCaptionArea(FormHitAreaNames.Search, focusForm: true);
                return true;
            }

            var bounds = GetKeyboardFocusedCaptionBounds();
            if (bounds.IsEmpty)
                return false;

            var area = _hits?.GetHitArea(_keyboardFocusedCaptionAreaName)
                ?? new HitArea { Name = _keyboardFocusedCaptionAreaName, Bounds = bounds };

            OnRegionClicked(area);
            return true;
        }

        private void CaptureCaptionAccessibilitySnapshot()
        {
            if (_captionAccessibilitySnapshotCaptured)
                return;

            _baseAccessibleName = AccessibleName ?? string.Empty;
            _baseAccessibleDescription = AccessibleDescription ?? string.Empty;
            _baseAccessibleDefaultActionDescription = AccessibleDefaultActionDescription ?? string.Empty;
            _baseAccessibleRole = AccessibleRole;
            _captionAccessibilitySnapshotCaptured = true;
        }

        private AccessibleRole GetCaptionAccessibilityRole(string areaName)
        {
            string key = BeepiFormProHitAreaManager.NormalizeName(areaName);
            return key switch
            {
                FormHitAreaNames.Search => AccessibleRole.Text,
                FormHitAreaNames.Profile => AccessibleRole.PushButton,
                FormHitAreaNames.CustomAction => AccessibleRole.PushButton,
                FormHitAreaNames.Theme => AccessibleRole.PushButton,
                FormHitAreaNames.Style => AccessibleRole.PushButton,
                FormHitAreaNames.Minimize => AccessibleRole.PushButton,
                FormHitAreaNames.Maximize => AccessibleRole.PushButton,
                FormHitAreaNames.Close => AccessibleRole.PushButton,
                _ => FindCustomCaptionRegion(key, out _)?.AccessibleRole ?? AccessibleRole.PushButton
            };
        }

        private string GetCaptionAccessibilityName(string areaName)
        {
            string key = BeepiFormProHitAreaManager.NormalizeName(areaName);
            FormRegion customRegion = FindCustomCaptionRegion(key, out _);
            return key switch
            {
                FormHitAreaNames.Search => "Caption search box",
                FormHitAreaNames.Profile => "Caption profile button",
                FormHitAreaNames.CustomAction => "Caption custom action",
                FormHitAreaNames.Theme => "Caption theme button",
                FormHitAreaNames.Style => "Caption style button",
                FormHitAreaNames.Minimize => "Caption minimize button",
                FormHitAreaNames.Maximize => WindowState == FormWindowState.Maximized ? "Caption restore button" : "Caption maximize button",
                FormHitAreaNames.Close => "Caption close button",
                _ => !string.IsNullOrWhiteSpace(customRegion?.AccessibleName)
                    ? customRegion.AccessibleName
                    : $"Caption region {customRegion?.Id ?? key}"
            };
        }

        private string GetCaptionAccessibilityDescription(string areaName)
        {
            string key = BeepiFormProHitAreaManager.NormalizeName(areaName);
            return key switch
            {
                FormHitAreaNames.Search => "Search box in the caption bar. Type to search, and press F6 or Tab to move to another caption action.",
                FormHitAreaNames.Profile => "Profile button in the caption bar. Press Enter or Space to open the profile action.",
                FormHitAreaNames.CustomAction => "Custom action button in the caption bar. Press Enter or Space to activate it.",
                FormHitAreaNames.Theme => "Theme button in the caption bar. Press Enter or Space to open the theme menu.",
                FormHitAreaNames.Style => "Style button in the caption bar. Press Enter or Space to open the style menu.",
                FormHitAreaNames.Minimize => "Minimize button in the caption bar. Press Enter or Space to minimize the window.",
                FormHitAreaNames.Maximize => WindowState == FormWindowState.Maximized
                    ? "Restore button in the caption bar. Press Enter or Space to restore the window."
                    : "Maximize button in the caption bar. Press Enter or Space to maximize the window.",
                FormHitAreaNames.Close => "Close button in the caption bar. Press Enter or Space to close the window.",
                _ => FindCustomCaptionRegion(key, out _) is FormRegion region && !string.IsNullOrWhiteSpace(region.AccessibleDescription)
                    ? region.AccessibleDescription
                    : "Custom caption region. Press Enter or Space to activate it."
            };
        }

        private string GetCaptionAccessibilityDefaultActionDescription(string areaName)
        {
            string key = BeepiFormProHitAreaManager.NormalizeName(areaName);
            return key switch
            {
                FormHitAreaNames.Search => "Edit search text",
                FormHitAreaNames.Profile => "Open profile action",
                FormHitAreaNames.CustomAction => "Activate custom action",
                FormHitAreaNames.Theme => "Open theme menu",
                FormHitAreaNames.Style => "Open style menu",
                FormHitAreaNames.Minimize => "Minimize window",
                FormHitAreaNames.Maximize => WindowState == FormWindowState.Maximized ? "Restore window" : "Maximize window",
                FormHitAreaNames.Close => "Close window",
                _ => FindCustomCaptionRegion(key, out _) is FormRegion region && !string.IsNullOrWhiteSpace(region.AccessibleDefaultActionDescription)
                    ? region.AccessibleDefaultActionDescription
                    : "Activate custom caption region"
            };
        }

        private void InvalidateCaptionAccessibilityChrome()
        {
            if (CurrentLayout != null && !CurrentLayout.CaptionRect.IsEmpty)
            {
                Invalidate(CurrentLayout.CaptionRect);
                return;
            }

            Invalidate();
        }

        private void UpdateCaptionAccessibilityFocus(string areaName)
        {
            if (!ScreenReaderSupport)
                return;

            CaptureCaptionAccessibilitySnapshot();
            AccessibleName = GetCaptionAccessibilityName(areaName);
            AccessibleDescription = GetCaptionAccessibilityDescription(areaName);
            AccessibleDefaultActionDescription = GetCaptionAccessibilityDefaultActionDescription(areaName);
            AccessibleRole = GetCaptionAccessibilityRole(areaName);

            try
            {
                AccessibilityNotifyClients(AccessibleEvents.NameChange, 0);
                AccessibilityNotifyClients(AccessibleEvents.DescriptionChange, 0);
                AccessibilityNotifyClients(AccessibleEvents.DefaultActionChange, 0);
                AccessibilityNotifyClients(AccessibleEvents.Focus, 0);
            }
            catch
            {
                // Accessibility notifications are best-effort and should not block interaction.
            }
        }

        private void RestoreCaptionAccessibilityFocus(bool force = false)
        {
            if ((!ScreenReaderSupport && !force) || !_captionAccessibilitySnapshotCaptured)
                return;

                AccessibleName = _baseAccessibleName;
                AccessibleDescription = _baseAccessibleDescription;
                AccessibleDefaultActionDescription = _baseAccessibleDefaultActionDescription;
                AccessibleRole = _baseAccessibleRole;
                _captionAccessibilitySnapshotCaptured = false;

                try
                {
                    AccessibilityNotifyClients(AccessibleEvents.NameChange, 0);
                    AccessibilityNotifyClients(AccessibleEvents.DescriptionChange, 0);
                    AccessibilityNotifyClients(AccessibleEvents.DefaultActionChange, 0);
                }
                catch
                {
                    // Accessibility notifications are best-effort and should not block interaction.
                }
            }

            /// <summary>
            /// Returns a defensive snapshot of the currently registered hit areas.
            /// This is intended for diagnostics surfaces and should not be mutated by callers.
            /// </summary>
            public IReadOnlyList<HitArea> GetRegisteredHitAreasSnapshot()
            {
                EnsureLayoutCalculated();

                if (_hits == null || _hits.Areas.Count == 0)
                {
                    return Array.Empty<HitArea>();
                }

                return _hits.Areas
                    .Select(area => new HitArea
                    {
                        Name = area.Name,
                        Bounds = area.Bounds,
                        Data = area.Data
                    })
                    .ToList();
            }

          public event EventHandler OnFormClose;
        public event EventHandler OnFormLoad;
        public event EventHandler OnFormShown;
        public event EventHandler<FormClosingEventArgs> PreClose;

        // DPI scaling factor
        private float _dpiScale = 1.0f;

        // Caption bar properties
        private bool _showCaptionBar = true;
        private int _captionHeight = 32;

        // Modern effect properties
        private ShadowEffect _shadowEffect = new ShadowEffect();
        private CornerRadius _cornerRadius = new CornerRadius(8);
        private AntiAliasMode _antiAliasMode = AntiAliasMode.High;
        private bool _enableAnimations = true;
        private int _animationDuration = 200;

        /// <summary>
        /// Gets or sets whether to show the caption bar
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue(true)]
        public bool ShowCaptionBar
        {
            get => _showCaptionBar;
            set
            {
                if (_showCaptionBar != value)
                {
                    _showCaptionBar = value;
                    InvalidateLayout(); // Mark layout dirty
                    PerformLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the caption bar
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue(32)]
        public int CaptionHeight
        {
            get => _captionHeight;
            set
            {
                if (_captionHeight != value)
                {
                    _captionHeight = value;
                    InvalidateLayout(); // Mark layout dirty
                    PerformLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the shadow effect for the form
        /// </summary>
        [System.ComponentModel.Category("Beep Effects")]
        [System.ComponentModel.Description("Shadow effect configuration")]
        public ShadowEffect ShadowEffect
        {
            get => _shadowEffect ?? (_shadowEffect = new ShadowEffect());
            set
            {
                _shadowEffect = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the corner radius for the form
        /// </summary>
        [System.ComponentModel.Category("Beep Effects")]
        [System.ComponentModel.Description("Corner radius configuration")]
        public CornerRadius CornerRadius
        {
            get => _cornerRadius ?? (_cornerRadius = new CornerRadius(8));
            set
            {
                if (_cornerRadius != value)
                {
                    _cornerRadius = value;
                    // Invalidate region when corner radius changes (rounded corners will change)
                    if (IsHandleCreated)
                    {
                        // Dispose old managed region if it exists
                        if (Region != null)
                        {
                            var oldRegion = Region;
                            Region = null;
                            oldRegion.Dispose();
                        }
                        UpdateWindowRegion();
                        UpdateFormRegion();
                    }
                    InvalidateLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the anti-aliasing mode
        /// </summary>
        [System.ComponentModel.Category("Beep Effects")]
        [System.ComponentModel.DefaultValue(AntiAliasMode.High)]
        [System.ComponentModel.Description("Anti-aliasing quality mode")]
        public AntiAliasMode AntiAliasMode
        {
            get => _antiAliasMode;
            set
            {
                _antiAliasMode = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the rendering quality preset
        /// </summary>
        [System.ComponentModel.Category("Beep Effects")]
        [System.ComponentModel.DefaultValue(RenderingQuality.Auto)]
        [System.ComponentModel.Description("Rendering quality preset for performance optimization")]
        public RenderingQuality RenderingQuality { get; set; } = RenderingQuality.Auto;

        /// <summary>
        /// Gets or sets whether animations are enabled
        /// </summary>
        [System.ComponentModel.Category("Beep Animation")]
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Description("Enable smooth animations")]
        public bool EnableAnimations
        {
            get => _enableAnimations;
            set => _enableAnimations = value;
        }

        /// <summary>
        /// Gets or sets whether to paint decorative effects over the entire form including content area.
        /// When true, effects like blur, gradients, and overlays are painted over controls.
        /// Controls remain interactive as mouse events are not intercepted.
        /// </summary>
        [System.ComponentModel.Category("Beep Effects")]
        [System.ComponentModel.DefaultValue(false)]
        [System.ComponentModel.Description("Paint decorative effects over entire form including content area. Controls remain interactive.")]
        public bool PaintOverContentArea { get; set; } = false;

        /// <summary>
        /// Gets or sets the animation duration in milliseconds
        /// </summary>
        [System.ComponentModel.Category("Beep Animation")]
        [System.ComponentModel.DefaultValue(200)]
        [System.ComponentModel.Description("Animation duration in milliseconds")]
        public int AnimationDuration
        {
            get => _animationDuration;
            set => _animationDuration = Math.Max(0, value);
        }

        // Advanced modern properties for the best modern form experience

        /// <summary>
        /// Gets or sets the backdrop effect mode (Mica, Acrylic, etc.)
        /// </summary>
        [System.ComponentModel.Category("Beep Effects")]
        [System.ComponentModel.DefaultValue(BackdropEffect.None)]
        [System.ComponentModel.Description("Advanced backdrop effect for modern appearance")]
        public BackdropEffect BackdropEffect { get; set; } = BackdropEffect.None;

        /// <summary>
        /// Gets or sets whether high contrast mode is enabled for accessibility
        /// </summary>
        [System.ComponentModel.Category("Beep Accessibility")]
        [System.ComponentModel.DefaultValue(false)]
        [System.ComponentModel.Description("Enable high contrast mode for better accessibility")]
        public bool HighContrastMode
        {
            get => _highContrastMode;
            set
            {
                if (_highContrastMode == value)
                    return;

                _highContrastMode = value;
                Invalidate(true);
            }
        }

        /// <summary>
        /// Gets or sets whether screen reader support is enabled
        /// </summary>
        [System.ComponentModel.Category("Beep Accessibility")]
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Description("Enable screen reader support for accessibility")]
        public bool ScreenReaderSupport
        {
            get => _screenReaderSupport;
            set
            {
                if (_screenReaderSupport == value)
                    return;

                _screenReaderSupport = value;

                if (!_screenReaderSupport)
                {
                    RestoreCaptionAccessibilityFocus(force: true);
                    return;
                }

                if (HasKeyboardCaptionFocus)
                    UpdateCaptionAccessibilityFocus(_keyboardFocusedCaptionAreaName);
            }
        }

        /// <summary>
        /// Gets or sets the focus indicator Style for keyboard navigation
        /// </summary>
        [System.ComponentModel.Category("Beep Accessibility")]
        [System.ComponentModel.DefaultValue(FocusIndicatorStyle.Subtle)]
        [System.ComponentModel.Description("Style of focus indicators for keyboard navigation")]
        public FocusIndicatorStyle FocusIndicatorStyle
        {
            get => _focusIndicatorStyle;
            set
            {
                if (_focusIndicatorStyle == value)
                    return;

                _focusIndicatorStyle = value;
                InvalidateCaptionAccessibilityChrome();
            }
        }

        /// <summary>
        /// Gets or sets whether micro-interactions are enabled
        /// </summary>
        [System.ComponentModel.Category("Beep Animation")]
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Description("Enable subtle micro-interactions and hover effects")]
        public bool EnableMicroInteractions { get; set; } = true;

        /// <summary>
        /// Gets or sets the adaptive layout mode for responsive design
        /// </summary>
        [System.ComponentModel.Category("Beep Layout")]
        [System.ComponentModel.DefaultValue(AdaptiveLayoutMode.Auto)]
        [System.ComponentModel.Description("Adaptive layout behavior for different screen sizes")]
        public AdaptiveLayoutMode AdaptiveLayoutMode { get; set; } = AdaptiveLayoutMode.Auto;

        /// <summary>
        /// Gets or sets whether smart invalidation is enabled for performance
        /// </summary>
        [System.ComponentModel.Category("Beep Performance")]
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Description("Enable smart invalidation to improve rendering performance")]
        public bool EnableSmartInvalidation { get; set; } = true;

      

        /// <summary>
        /// Gets or sets the hover animation duration for micro-interactions
        /// </summary>
        [System.ComponentModel.Category("Beep Animation")]
        [System.ComponentModel.DefaultValue(150)]
        [System.ComponentModel.Description("Duration of hover animations in milliseconds")]
        public int HoverAnimationDuration { get; set; } = 150;

        /// <summary>
        /// Gets or sets the focus transition duration
        /// </summary>
        [System.ComponentModel.Category("Beep Animation")]
        [System.ComponentModel.DefaultValue(200)]
        [System.ComponentModel.Description("Duration of focus transitions in milliseconds")]
        public int FocusTransitionDuration { get; set; } = 200;

        /// <summary>
        /// Gets or sets whether smooth window resizing is enabled
        /// </summary>
        [System.ComponentModel.Category("Beep Animation")]
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Description("Enable smooth animations during window resize")]
        public bool EnableSmoothResize { get; set; } = true;

        /// <summary>
        /// Gets or sets the window state transition duration
        /// </summary>
        [System.ComponentModel.Category("Beep Animation")]
        [System.ComponentModel.DefaultValue(300)]
        [System.ComponentModel.Description("Duration of minimize/maximize/restore transitions")]
        public int WindowStateTransitionDuration { get; set; } = 300;

        /// <summary>
        /// Gets or sets whether to show the theme button in the caption bar
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue(false)]
        public bool ShowThemeButton
        {
            get => _showThemeButton;
            set
            {
                if (_showThemeButton != value)
                {
                    _showThemeButton = value;
                    InvalidateLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show the Style button in the caption bar
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue(false)]
        public bool ShowStyleButton
        {
            get => _showStyleButton;
            set
            {
                if (_showStyleButton != value)
                {
                    _showStyleButton = value;
                    InvalidateLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show the profile button in the caption bar
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue(false)]
        [System.ComponentModel.Description("Show profile/user button in caption bar")]
        public bool ShowProfileButton
        {
            get => _showProfileButton;
            set
            {
                if (_showProfileButton != value)
                {
                    _showProfileButton = value;
                    InvalidateLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show the search box in the caption bar
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue(false)]
        [System.ComponentModel.Description("Show search box in caption bar")]
        public bool ShowSearchBox
        {
            get => _showSearchBox;
            set
            {
                if (_showSearchBox != value)
                {
                    _showSearchBox = value;
                    InvalidateLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the search text in the search box
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue("")]
        [System.ComponentModel.Description("Text in the search box")]
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value ?? string.Empty;
                    if (ShowSearchBox && CurrentLayout.SearchBoxRect.Width > 0)
                    {
                        Invalidate(CurrentLayout.SearchBoxRect);
                    }
                    SearchBoxTextChanged?.Invoke(this, _searchText);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show the close button in the caption bar
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Description("Show close button in caption bar")]
        public bool ShowCloseButton
        {
            get => _showCloseButton;
            set
            {
                if (_showCloseButton != value)
                {
                    _showCloseButton = value;
                    InvalidateLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show minimize and maximize buttons in the caption bar
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Description("Show minimize and maximize buttons in caption bar")]
        public bool ShowMinMaxButtons
        {
            get => _showMinMaxButtons;
            set
            {
                if (_showMinMaxButtons != value)
                {
                    _showMinMaxButtons = value;
                    InvalidateLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show the custom action button in the caption bar
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue(false)]
        [System.ComponentModel.Description("Show custom action button in caption bar")]
        public bool ShowCustomActionButton
        {
            get => _showCustomActionButton;
            set
            {
                if (_showCustomActionButton != value)
                {
                    _showCustomActionButton = value;
                    InvalidateLayout();
                    Invalidate();
                }
            }
        }

        // Public API to register regions
        public void AddRegion(FormRegion region)
        {
            if (region == null) return;
            _regions.Add(region);
            InvalidateLayout();
            Invalidate();
        }

        public void ClearRegions()
        {
            _regions.Clear();
            InvalidateLayout();
            Invalidate();
        }

     

        /// <summary>
        /// Returns the current FormPainterMetrics for the active FormStyle and theme.
        /// Used by built-in region OnPaint delegates so they always reflect the
        /// current style instead of a stale closure captured at construction time.
        /// </summary>
        private FormPainterMetrics GetCurrentMetrics()
            => FormPainterMetrics.DefaultForCached(FormStyle, UseThemeColors ? _currentTheme : null);

        private void InitializeBuiltInRegions()
        {
            _iconRegion = new FormRegion
            {
                Id = "system:icon",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (Icon != null && r.Width > 0 && r.Height > 0)
                    {
                        int size = Math.Min(r.Width, r.Height) - 4;
                        var iconRect = new Rectangle(r.Left + 2, r.Top + (r.Height - size) / 2, size, size);
                        g.DrawIcon(Icon, iconRect);
                    }
                }
            };

            // Title region (center of caption)
            _titleRegion = new FormRegion
            {
                Id = "system:title",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (string.IsNullOrEmpty(Text) || r.Width <= 0 || r.Height <= 0) return;

                    var m = GetCurrentMetrics();
                    TextRenderer.DrawText(g, Text, Font, r, m.ForegroundColor,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                }
            };

            // System buttons for Modern/Minimal form styles
            int btnSize = 32;
            _minimizeButton = new FormRegion
            {
                Id = "system:minimize",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (!_showMinMaxButtons || r.Width <= 0 || r.Height <= 0) return;
                    var m = GetCurrentMetrics();
                    bool isHover = _interact?.IsHovered(_hits?.GetHitArea(FormHitAreaNames.Minimize)) ?? false;
                    FormPainterRenderHelper.DrawSystemButton(
                        g,
                        r,
                        "−",
                        isHover,
                        false,
                        Font,
                        m.ForegroundColor,
                        m.CaptionButtonHoverColor);
                }
            };

            _maximizeButton = new FormRegion
            {
                Id = "system:maximize",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (!_showMinMaxButtons || r.Width <= 0 || r.Height <= 0) return;
                    var m = GetCurrentMetrics();
                    bool isHover = _interact?.IsHovered(_hits?.GetHitArea(FormHitAreaNames.Maximize)) ?? false;
                    string symbol = WindowState == FormWindowState.Maximized ? "❐" : "□";
                    FormPainterRenderHelper.DrawSystemButton(
                        g,
                        r,
                        symbol,
                        isHover,
                        false,
                        Font,
                        m.ForegroundColor,
                        m.CaptionButtonHoverColor);
                }
            };

            _closeButton = new FormRegion
            {
                Id = "system:close",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (!_showCloseButton || r.Width <= 0 || r.Height <= 0) return;
                    var m = GetCurrentMetrics();
                    bool isHover = _interact?.IsHovered(_hits?.GetHitArea(FormHitAreaNames.Close)) ?? false;
                    FormPainterRenderHelper.DrawSystemButton(
                        g,
                        r,
                        "✕",
                        isHover,
                        true,
                        Font,
                        m.ForegroundColor,
                        m.CaptionButtonHoverColor,
                        Color.FromArgb(232, 17, 35));
                }
            };

            // Custom action button (between title and system buttons)
            _customActionButton = new FormRegion
            {
                Id = "custom:action",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (!_showCustomActionButton || r.Width <= 0 || r.Height <= 0) return;
                    var m = GetCurrentMetrics();

                    var isHovered = _interact?.IsHovered(_hits?.GetHitArea(FormHitAreaNames.CustomAction)) ?? false;
                    var isPressed = _interact?.IsPressed(_hits?.GetHitArea(FormHitAreaNames.CustomAction)) ?? false;
                    
                    // Hover/press indicator: circle outline around the icon
                    var hoverColor = isPressed ? m.CaptionButtonPressedColor : m.CaptionButtonHoverColor;
                    if (isHovered || isPressed)
                        FormPainterRenderHelper.DrawHoverOutlineCircle(g, r, hoverColor, isPressed ? 3 : 2, 6);

                    // Draw icon (⚙ gear/settings icon)
                    var fg = m.ForegroundColor;
                    Font font = BeepThemesManager.ToFont(BeepThemesManager.CurrentTheme?.BodyMedium) ?? SystemFonts.DefaultFont;
                    TextRenderer.DrawText(g, "⚙", font, r, fg,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            };

            // MenuStyle button (palette icon)
            _themeButton = new FormRegion
            {
                Id = "system:theme",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (r.Width <= 0 || r.Height <= 0 || !_showThemeButton) return;
                    var m = GetCurrentMetrics();

                    var isHovered = _interact?.IsHovered(_hits?.GetHitArea(FormHitAreaNames.Theme)) ?? false;
                    var isPressed = _interact?.IsPressed(_hits?.GetHitArea(FormHitAreaNames.Theme)) ?? false;
                    
                    // Hover/press indicator: circle outline
                    var hoverColor = isPressed ? m.CaptionButtonPressedColor : m.CaptionButtonHoverColor;
                    if (isHovered || isPressed)
                        FormPainterRenderHelper.DrawHoverOutlineCircle(g, r, hoverColor, isPressed ? 3 : 2, 6);

                    // Draw icon (🎨 palette icon)
                    var fg = m.ForegroundColor;
                    Font font = BeepThemesManager.ToFont(BeepThemesManager.CurrentTheme?.BodyMedium) ?? SystemFonts.DefaultFont; // was: new Font("Segoe UI Emoji", Font.Size, FontStyle.Regular);
                    TextRenderer.DrawText(g, "🎨", font, r, fg,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            };

            // Style button (layout icon)
            _styleButton = new FormRegion
            {
                Id = "system:Style",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (r.Width <= 0 || r.Height <= 0 || !_showStyleButton) return;
                    var m = GetCurrentMetrics();

                    var isHovered = _interact?.IsHovered(_hits?.GetHitArea(FormHitAreaNames.Style)) ?? false;
                    var isPressed = _interact?.IsPressed(_hits?.GetHitArea(FormHitAreaNames.Style)) ?? false;
                    
                    // Hover/press indicator: circle outline
                    var hoverColor = isPressed ? m.CaptionButtonPressedColor : m.CaptionButtonHoverColor;
                    if (isHovered || isPressed)
                        FormPainterRenderHelper.DrawHoverOutlineCircle(g, r, hoverColor, isPressed ? 3 : 2, 6);

                    // Draw icon (◧ layout/Style icon)
                    var fg = m.ForegroundColor;
                    Font font = BeepThemesManager.ToFont(BeepThemesManager.CurrentTheme?.BodyMedium) ?? SystemFonts.DefaultFont; // was: new Font("Segoe UI Symbol", Font.Size + 2, FontStyle.Regular);
                    TextRenderer.DrawText(g, "◧", font, r, fg,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            };

            _profileButton = new FormRegion
            {
                Id = "system:profile",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (!_showProfileButton || r.Width <= 0 || r.Height <= 0) return;

                    var m = GetCurrentMetrics();
                    var isHovered = _interact?.IsHovered(_hits?.GetHitArea(FormHitAreaNames.Profile)) ?? false;
                    var isPressed = _interact?.IsPressed(_hits?.GetHitArea(FormHitAreaNames.Profile)) ?? false;
                    var outlineColor = isPressed ? m.CaptionButtonPressedColor : m.CaptionButtonHoverColor;

                    if (isHovered || isPressed)
                        FormPainterRenderHelper.DrawHoverOutlineCircle(g, r, outlineColor, isPressed ? 3 : 2, 6);

                    int avatarSize = Math.Max(12, Math.Min(r.Width, r.Height) - 12);
                    var avatarRect = new Rectangle(
                        r.Left + (r.Width - avatarSize) / 2,
                        r.Top + (r.Height - avatarSize) / 2,
                        avatarSize,
                        avatarSize);

                    var fillColor = Color.FromArgb(36, m.ForegroundColor);
                    var oldSmoothing = g.SmoothingMode;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    using (var fillBrush = new SolidBrush(fillColor))
                    using (var borderPen = new Pen(m.ForegroundColor, 1.25f))
                    {
                        g.FillEllipse(fillBrush, avatarRect);
                        g.DrawEllipse(borderPen, avatarRect);
                    }

                    g.SmoothingMode = oldSmoothing;

                    Font font = BeepThemesManager.ToFont(BeepThemesManager.CurrentTheme?.BodyMedium) ?? SystemFonts.DefaultFont; // was: new Font("Segoe MDL2 Assets", Font.Size + 1, FontStyle.Regular);
                    TextRenderer.DrawText(g, "\uE77B", font, r, m.ForegroundColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            };

            // Search box region
            _searchBox = new FormRegion
            {
                Id = "system:search",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (!_showSearchBox || r.Width <= 0 || r.Height <= 0) return;
                    
                    var isHovered = _interact?.IsHovered(_hits?.GetHitArea(FormHitAreaNames.Search)) ?? false;
                    var metrics = FormPainterMetrics.DefaultForCached(FormStyle, UseThemeColors ? _currentTheme : null);
                    
                    // Background with rounded corners
                    int radius = 4;
                    using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                    {
                        path.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90);
                        path.AddArc(r.Right - radius * 2, r.Y, radius * 2, radius * 2, 270, 90);
                        path.AddArc(r.Right - radius * 2, r.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                        path.AddArc(r.X, r.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                        path.CloseFigure();
                        
                        // Background color (slightly lighter than caption)
                        var bgColor = _searchBoxFocused 
                            ? Color.FromArgb(255, 255, 255)
                            : (isHovered 
                                ? Color.FromArgb(245, 245, 245)
                                : Color.FromArgb(240, 240, 240));
                        
                        if (UseThemeColors && _currentTheme != null && _currentTheme.IsDarkTheme)
                        {
                            bgColor = _searchBoxFocused
                                ? Color.FromArgb(60, 60, 60)
                                : (isHovered
                                    ? Color.FromArgb(50, 50, 50)
                                    : Color.FromArgb(45, 45, 45));
                        }
                        
                        using (var brush = new SolidBrush(bgColor))
                        {
                            g.FillPath(brush, path);
                        }
                        
                        // Border
                        var accentColor = _currentTheme?.AccentColor ?? Color.FromArgb(99, 102, 241); // Default indigo
                        var borderColor = _searchBoxFocused
                            ? accentColor
                            : Color.FromArgb(200, 200, 200);
                        
                        if (UseThemeColors && _currentTheme != null && _currentTheme.IsDarkTheme)
                        {
                            borderColor = _searchBoxFocused
                                ? accentColor
                                : Color.FromArgb(80, 80, 80);
                        }
                        
                        using (var pen = new Pen(borderColor, _searchBoxFocused ? 2f : 1f))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                    
                    // Search icon (magnifying glass) on the left
                    int iconSize = 14;
                    int iconPadding = 8;
                    int iconX = r.X + iconPadding;
                    int iconY = r.Y + (r.Height - iconSize) / 2;
                    
                    var iconColor = UseThemeColors && _currentTheme != null && _currentTheme.IsDarkTheme
                        ? Color.FromArgb(180, 180, 180)
                        : Color.FromArgb(120, 120, 120);
                    
                    using (var iconPen = new Pen(iconColor, 1.5f))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        // Circle
                        g.DrawEllipse(iconPen, iconX, iconY, iconSize, iconSize);
                        // Handle
                        int handleX = iconX + iconSize - 2;
                        int handleY = iconY + iconSize - 2;
                        g.DrawLine(iconPen, handleX, handleY, handleX + 4, handleY + 4);
                    }
                    
                    // Text area (to the right of icon)
                    int textX = iconX + iconSize + iconPadding;
                    int textY = r.Y;
                    int textWidth = r.Width - (textX - r.X) - iconPadding;
                    var textRect = new Rectangle(textX, textY, textWidth, r.Height);
                    
                    if (string.IsNullOrEmpty(_searchText))
                    {
                        // Placeholder text
                        var placeholderColor = UseThemeColors && _currentTheme != null && _currentTheme.IsDarkTheme
                            ? Color.FromArgb(120, 120, 120)
                            : Color.FromArgb(160, 160, 160);
                        TextRenderer.DrawText(g, "Search...", Font, textRect, placeholderColor,
                            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                    }
                    else
                    {
                        // Actual search text
                        var textColor = UseThemeColors && _currentTheme != null && _currentTheme.IsDarkTheme
                            ? Color.FromArgb(220, 220, 220)
                            : Color.FromArgb(50, 50, 50);
                        TextRenderer.DrawText(g, _searchText, Font, textRect, textColor,
                            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                        
                        // Caret when focused
                        if (_searchBoxFocused)
                        {
                            var textSize = TextRenderer.MeasureText(g, _searchText, Font);
                            int caretX = textX + textSize.Width + 1;
                            int caretY = textY + 4;
                            int caretHeight = r.Height - 8;
                            using (var caretPen = new Pen(textColor, 1f))
                            {
                                g.DrawLine(caretPen, caretX, caretY, caretX, caretY + caretHeight);
                            }
                        }
                    }
                }
            };
        }

        protected void OnRegionClicked(HitArea area)
        {
            if (area?.Name == null) return;

            string key = BeepiFormProHitAreaManager.NormalizeName(area.Name);
            if (key == FormHitAreaNames.Title)
                key = FormHitAreaNames.Caption;

            var regionData = area.Data as FormRegion;
            RegionClick?.Invoke(this, new RegionEventArgs(area.Name, regionData, area.Bounds));

            switch (key)
            {
                case FormHitAreaNames.Minimize:
                    if (!ShowMinMaxButtons) return;
                    WindowState = FormWindowState.Minimized;
                    break;

                case FormHitAreaNames.Maximize:
                    if (!ShowMinMaxButtons) return;
                    WindowState = WindowState == FormWindowState.Maximized
                        ? FormWindowState.Normal
                        : FormWindowState.Maximized;
                    break;

                case FormHitAreaNames.Close:
                    if (!ShowCloseButton) return;
                    BeginInvoke(new Action(() =>
                    {
                        AutoValidate = AutoValidate.Disable;
                        _isForcedClose = true;
                        Close();
                    }));
                    break;

                case FormHitAreaNames.CustomAction:
                    if (!ShowCustomActionButton) return;
                    OnCustomActionClicked();
                    break;

                case FormHitAreaNames.Theme:
                    if (!ShowThemeButton) return;
                    ThemeButtonClicked?.Invoke(this, EventArgs.Empty);
                    if (ThemeButtonClicked == null)
                        ShowThemeMenu();
                    break;

                case FormHitAreaNames.Style:
                    if (!ShowStyleButton) return;
                    StyleButtonClicked?.Invoke(this, EventArgs.Empty);
                    if (StyleButtonClicked == null)
                        ShowFormStyleMenu();
                    break;

                case FormHitAreaNames.Profile:
                    if (!ShowProfileButton) return;
                    ProfileButtonClicked?.Invoke(this, EventArgs.Empty);
                    break;

                case FormHitAreaNames.Search:
                    if (!ShowSearchBox) return;
                    SetKeyboardFocusedCaptionArea(FormHitAreaNames.Search, focusForm: true);
                    break;

                case FormHitAreaNames.Caption:
                    if (WindowState == FormWindowState.Normal)
                    {
                        ReleaseCapture();
                        SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
                    }

                    if (_searchBoxFocused)
                    {
                        _searchBoxFocused = false;
                        Invalidate(CurrentLayout.SearchBoxRect);
                    }

                    if (HasKeyboardCaptionFocus)
                        ClearKeyboardCaptionFocus();
                    break;
            }

        }

        private void ShowFormStyleMenu()
        {
            var menu = new ContextMenuStrip();
            try
            {
                foreach (FormStyle style in Enum.GetValues(typeof(FormStyle)))
                {
                    var item = new ToolStripMenuItem(style.ToString())
                    {
                        Checked = FormStyle == style
                    };
                    item.Click += (s, e) =>
                    {
                        try
                        {
                            BeepThemesManager.SetCurrentStyle(style);
                        }
                        catch { }
                    };
                    menu.Items.Add(item);
                }
            }
            catch { }

            var styleRect = CurrentLayout.StyleButtonRect;
            Point pt;
            if (styleRect.Width > 0 && styleRect.Height > 0)
            {
                pt = PointToScreen(new Point(styleRect.Left, styleRect.Bottom));
            }
            else
            {
                pt = Cursor.Position;
            }
            menu.Show(pt);
        }

        private void ShowThemeMenu()
        {
            var menu = new ContextMenuStrip();
            try
            {
                foreach (string theme in BeepThemesManager.GetThemeNames())
                {
                    var item = new ToolStripMenuItem(theme);
                    item.Click += (s, e) =>
                    {
                         // Defer theme change to avoid repaints during menu interaction
                         BeginInvoke(new Action(() =>
                          {
                            try
                          {
                                BeepThemesManager.SetCurrentTheme(theme);
                               //  Theme = theme; // This will handle invalidation smartly
                          }
                             catch { }
                            }));
                    };
                    menu.Items.Add(item);
                }
            }
            catch { }
            // Show menu below the theme button using the current layout rectangle
            var themeRect = CurrentLayout.ThemeButtonRect;
            Point pt;
            if (themeRect.Width > 0 && themeRect.Height > 0)
            {
                pt = PointToScreen(new Point(themeRect.Left, themeRect.Bottom));
            }
            else
            {
                // Fallback: show at cursor to avoid (0,0) when rect isn't available
                pt = Cursor.Position;
            }
            menu.Show(pt);
        }

        protected virtual void OnCustomActionClicked()
        {
            // Override in derived class or subscribe to RegionClick event
            MessageBox.Show("Custom action button clicked! Override OnCustomActionClicked or subscribe to RegionClick event.", 
                "BeepiFormPro", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        


        /// <summary>
        /// Calculates layout and updates hit areas when form properties change.
        /// Now uses the layout caching system for better performance.
        /// </summary>
        private void RecalculateLayoutAndHitAreas()
        {
            // Mark layout as dirty and force immediate recalculation
            InvalidateLayout();
            
            try
            {
                EnsureLayoutCalculated();
            }
            catch
            {
                // Top-level safety: swallow any unexpected exceptions so the
                // Visual Studio designer does not throw when loading the form.
            }
        }

        // P/Invoke for window dragging
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    }
}

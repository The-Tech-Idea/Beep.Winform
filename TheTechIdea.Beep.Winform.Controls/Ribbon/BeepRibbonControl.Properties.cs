using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Accessibility;
using TheTechIdea.Beep.Winform.Controls.Customization;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Search;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Tokens;
using TheTechIdea.Beep.Winform.Controls.Tooltips;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepRibbonControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BeepRibbonTabStrip Tabs => _tabStrip;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> CommandItems => _commandItems;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Ribbon")]
        [Description("Collection of ribbon tabs, groups, and items defined as components. Added at design time via toolbox.")]
        public List<RibbonTabItem> RibbonTabs { get; } = new();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public RibbonTheme RibbonThemeProvider
        {
            get => _theme;
            set
            {
                _theme = value ?? new RibbonTheme();
                Invalidate(true);
                ApplyTheme();
            }
        }

        [DefaultValue(false)]
        public bool DarkMode
        {
            get => _darkMode;
            set
            {
                _darkMode = value;
                _resolvedStylePreset = RibbonThemeMapper.GetPreset(_ribbonFormStyle, _darkMode);
                ApplyTheme();
            }
        }

        [DefaultValue(true)]
        public bool FollowGlobalFormStyle
        {
            get => _followGlobalFormStyle;
            set
            {
                if (_followGlobalFormStyle == value) return;
                _followGlobalFormStyle = value;
                if (_followGlobalFormStyle)
                {
                    _ribbonFormStyle = BeepThemesManager.CurrentStyle;
                    ApplyThemeFromBeep(BeepThemesManager.CurrentTheme, _ribbonFormStyle);
                }
            }
        }

        [DefaultValue(FormStyle.Modern)]
        public FormStyle RibbonFormStyle
        {
            get => _ribbonFormStyle;
            set
            {
                if (_ribbonFormStyle == value) return;
                _ribbonFormStyle = value;
                if (!_followGlobalFormStyle)
                    ApplyThemeFromBeep(BeepThemesManager.CurrentTheme, _ribbonFormStyle);
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RibbonStylePreset ResolvedStylePreset => _resolvedStylePreset;

        [DefaultValue(RibbonLayoutMode.Classic)]
        public RibbonLayoutMode LayoutMode
        {
            get => _layoutMode;
            set
            {
                if (_layoutMode == value) return;
                _layoutMode = value;
                RefreshCommandView();
            }
        }

        [DefaultValue(RibbonDensity.Comfortable)]
        public RibbonDensity Density
        {
            get => _density;
            set
            {
                if (_density == value) return;
                _density = value;
                RefreshCommandView();
            }
        }

        [DefaultValue(RibbonSearchMode.Off)]
        public RibbonSearchMode SearchMode
        {
            get => _searchMode;
            set
            {
                if (_searchMode == value) return;
                _searchMode = value;
                EnsureSearchControls();
            }
        }

        [DefaultValue(false)]
        public bool SearchIncludeBackstage
        {
            get => _searchIncludeBackstage;
            set => _searchIncludeBackstage = value;
        }

        [DefaultValue(true)]
        public bool ShowSearchHistorySuggestions
        {
            get => _showSearchHistorySuggestions;
            set => _showSearchHistorySuggestions = value;
        }

        [DefaultValue(true)]
        public bool UseSuperToolTips
        {
            get => _useSuperToolTips;
            set
            {
                if (_useSuperToolTips == value) return;
                _useSuperToolTips = value;
                RefreshCommandView();
            }
        }

        [DefaultValue(9000)]
        public int SuperTooltipDurationMs
        {
            get => _superTooltipDurationMs;
            set => _superTooltipDurationMs = Math.Max(1200, value);
        }

        [DefaultValue(20)]
        public int SearchHistoryLimit
        {
            get => _searchHistoryLimit;
            set => _searchHistoryLimit = Math.Max(1, value);
        }

        [DefaultValue(12)]
        public int SearchMaxResults
        {
            get => _searchMaxResults;
            set => _searchMaxResults = Math.Clamp(value, 4, 64);
        }

        [DefaultValue(RibbonPersonalizationOptions.All)]
        public RibbonPersonalizationOptions PersonalizationOptions
        {
            get => _personalizationOptions;
            set => _personalizationOptions = value;
        }

        [DefaultValue(true)]
        public bool QuickAccessAboveRibbon
        {
            get => _quickAccessAboveRibbon;
            set
            {
                if (_quickAccessAboveRibbon == value) return;
                _quickAccessAboveRibbon = value;
                ApplyQuickAccessPlacement();
            }
        }

        [DefaultValue(true)]
        public bool EnableKeyTips
        {
            get => _enableKeyTips;
            set
            {
                if (_enableKeyTips == value) return;
                _enableKeyTips = value;
                if (!_enableKeyTips)
                {
                    HideKeyTips();
                }
                else
                {
                    RefreshKeyTips();
                }
            }
        }

        [DefaultValue(true)]
        public bool AllowMinimize
        {
            get => _allowMinimize;
            set
            {
                if (_allowMinimize == value) return;
                _allowMinimize = value;
                if (!_allowMinimize && _isMinimized)
                {
                    IsMinimized = false;
                }
            }
        }

        [DefaultValue(false)]
        public bool IsMinimized
        {
            get => _isMinimized;
            set
            {
                if (_isMinimized == value) return;
                _isMinimized = value;
                ApplyMinimizedState();
                RibbonMinimizedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [DefaultValue(true)]
        public bool ShowMinimizedPopupOnTabClick
        {
            get => _showMinimizedPopupOnTabClick;
            set => _showMinimizedPopupOnTabClick = value;
        }

        [DefaultValue(12)]
        public int BackstageRecentLimit
        {
            get => _backstageRecentLimit;
            set => _backstageRecentLimit = Math.Max(1, value);
        }

        [DefaultValue(true)]
        public bool BackstageShowTimestamps
        {
            get => _backstageShowTimestamps;
            set
            {
                if (_backstageShowTimestamps == value) return;
                _backstageShowTimestamps = value;
                if (_activeBackstageIndex >= 0)
                {
                    ShowBackstageSection(_activeBackstageIndex);
                }
            }
        }

        [DefaultValue(true)]
        public bool BackstageUseRelativeTimestamps
        {
            get => _backstageUseRelativeTimestamps;
            set
            {
                if (_backstageUseRelativeTimestamps == value) return;
                _backstageUseRelativeTimestamps = value;
                if (_activeBackstageIndex >= 0)
                {
                    ShowBackstageSection(_activeBackstageIndex);
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<SimpleItem, DateTime, string>? BackstageTimestampFormatter
        {
            get => _backstageTimestampFormatter;
            set
            {
                _backstageTimestampFormatter = value;
                if (_activeBackstageIndex >= 0)
                {
                    ShowBackstageSection(_activeBackstageIndex);
                }
            }
        }

        [DefaultValue(true)]
        public bool EnableBackstageTransitions
        {
            get => _enableBackstageTransitions;
            set => _enableBackstageTransitions = value;
        }

        [DefaultValue(180)]
        public int BackstageTransitionDurationMs
        {
            get => _backstageTransitionDurationMs;
            set => _backstageTransitionDurationMs = Math.Max(50, value);
        }

        [DefaultValue(true)]
        public bool EnableContextTransitions
        {
            get => _enableContextTransitions;
            set
            {
                if (_enableContextTransitions == value) return;
                _enableContextTransitions = value;
                if (!_enableContextTransitions)
                {
                    _contextTransitionTimer.Stop();
                    _contextTransitionProgress = 1f;
                    _contextHeader.Invalidate();
                }
            }
        }

        [DefaultValue(180)]
        public int ContextTransitionDurationMs
        {
            get => _contextTransitionDurationMs;
            set => _contextTransitionDurationMs = Math.Max(50, value);
        }

        [DefaultValue(true)]
        public bool AdaptiveTransitionTiming
        {
            get => _adaptiveTransitionTiming;
            set => _adaptiveTransitionTiming = value;
        }

        [DefaultValue(true)]
        public bool RespectSystemReducedMotion
        {
            get => _respectSystemReducedMotion;
            set => _respectSystemReducedMotion = value;
        }

        [DefaultValue(false)]
        public bool ReducedMotion
        {
            get => _reducedMotion;
            set
            {
                if (_reducedMotion == value) return;
                _reducedMotion = value;
                if (_reducedMotion)
                {
                    _contextTransitionTimer.Stop();
                    _backstageTransitionTimer.Stop();
                    _contextTransitionProgress = 1f;
                    _contextHeader.Invalidate();
                }
            }
        }

        [DefaultValue(false)]
        public bool RibbonRightToLeft
        {
            get => _ribbonRightToLeft;
            set
            {
                if (_ribbonRightToLeft == value) return;
                _ribbonRightToLeft = value;
                RightToLeft = _ribbonRightToLeft ? RightToLeft.Yes : RightToLeft.No;
                ApplyRightToLeftLayout();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IRibbonSearchProvider? SearchProvider
        {
            get => _searchProvider;
            set => _searchProvider = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IRibbonSearchTelemetry? SearchTelemetry
        {
            get => _searchTelemetry;
            set => _searchTelemetry = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<SimpleItem, int>? SearchScoreBoostProvider
        {
            get => _searchScoreBoostProvider;
            set => _searchScoreBoostProvider = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<SimpleItem, RibbonSuperTooltipModel>? SuperTooltipModelProvider
        {
            get => _superTooltipModelProvider;
            set => _superTooltipModelProvider = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IReadOnlyList<string> SearchHistory => _searchHistory;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RibbonKeyboardMap KeyboardMap => _keyboardMap;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RibbonVariantMatrix VariantMatrix
        {
            get => _variantMatrix;
            set => _variantMatrix = value ?? RibbonVariantMatrix.CreateDefault();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IReadOnlyList<string> LastTokenImportDiagnostics => _lastTokenImportDiagnostics;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IReadOnlyList<string> QuickAccessCommandKeys => _quickAccessCommandKeys;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ActiveContextKey => _activeContextKey;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMerged => _isMerged;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCustomizationDefaults => _defaultCustomizationSnapshot is { Count: > 0 };

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> BackstageItems => _backstageItems;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> BackstageRecentItems => _backstageRecentItems;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> BackstagePinnedItems => _backstagePinnedItems;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> BackstageFooterItems => _backstageFooterItems;

        // Backstage right content surface
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Panel BackstageContent => _backstageContentHost;

        // Full backstage host panel (navigation + content)
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Panel BackstageSurface => _backstagePanelContent;
    }
}

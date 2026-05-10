using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Editors;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes.Painters;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes
{
    public enum CheckBoxMouseHitMode
    {
        WholeControl,
        CheckBoxGlyph
    }

    //-------------------------------------------------
    // Non-generic wrapper for bool
    [ToolboxItem(true)]
    [DisplayName("Beep CheckBox Bool")]
    [Category("Beep Controls")]
    [Description("A checkbox control with boolean values.")]
    public class BeepCheckBoxBool : BeepCheckBox<bool>
    {
        public BeepCheckBoxBool()
        {
            CheckedValue = true;
            UncheckedValue = false;
            CurrentValue = false;
        }
    }

    // Non-generic wrapper for char
    [ToolboxItem(true)]
    [DisplayName("Beep CheckBox Char")]
    [Category("Beep Controls")]
    [Description("A checkbox control with character values.")]
    public class BeepCheckBoxChar : BeepCheckBox<char>
    {
        public BeepCheckBoxChar()
        {
            CheckedValue = 'Y';
            UncheckedValue = 'N';
            CurrentValue = 'N';
        }
    }

    // Non-generic wrapper for string
    [ToolboxItem(true)]
    [DisplayName("Beep CheckBox String")]
    [Category("Beep Controls")]
    [Description("A checkbox control with string values.")]
    public class BeepCheckBoxString : BeepCheckBox<string>
    {
        public BeepCheckBoxString()
        {
            CheckedValue = "YES";
            UncheckedValue = "NO";
            CurrentValue = "NO";
        }
    }
    //-------------------------------------------------
    // Generic class for flexibility
    public partial class BeepCheckBox<T> : BaseControl
    {
        #region Private Fields
        private Dictionary<Color, SolidBrush> _brushCache = new Dictionary<Color, SolidBrush>();
        private Dictionary<Color, Pen> _penCache = new Dictionary<Color, Pen>();
        private Dictionary<string, GraphicsPath> _pathCache = new Dictionary<string, GraphicsPath>();

        // State tracking fields
        private CheckBoxState _lastDrawnState = CheckBoxState.Unchecked;
        private string _lastDrawnText = "";
        private Rectangle _lastDrawnRect = Rectangle.Empty;
        private bool _stateChanged = true;

        private CheckBoxState _state = CheckBoxState.Unchecked;
        private T _checkedValue;
        private T _uncheckedValue;
        private T _checkedStateValue;
        private T _uncheckedStateValue;
        private T _indeterminateStateValue;
        private T _currentValue;
        private bool _hasCheckedStateValue;
        private bool _hasUncheckedStateValue;
        private bool _hasIndeterminateStateValue;
        private bool _mapStateValuesToLegacy = true;
        private bool _useUncheckedValueForIndeterminate = true;
        private BeepImage _beepImage;
        private string _checkIconPath;
        private string _indeterminateIconPath;
        private Rectangle _lastCheckBoxRect = Rectangle.Empty;
        private Rectangle _lastTextRect = Rectangle.Empty;
        private bool _keyboardFocusVisible;
        private int _minimumHitTargetSize = 32;
        private int _minimumAutoSizeWidth;
        private bool _hideText = false;
        private bool _autoCheck = true;
        private bool _threeState = true;
        private CheckBoxMouseHitMode _mouseHitMode = CheckBoxMouseHitMode.WholeControl;
        private bool _syncControlStyleWithCheckBoxStyle = true;
        private bool _syncLayoutMetricsWithStyle = true;
        private bool _suppressCurrentValueNotifications;
        private Font _textFont;
        private bool _ownsTextFont;
        private int checkboxsize = 15;
        private int _spacing = 5;
        private TextAlignment _textAlignRelativeToCheckBox = TextAlignment.Right;

        // Painter system
        private ICheckBoxPainter _painter;
        private CheckBoxStyle _checkBoxStyle = CheckBoxStyle.Material3;
        #endregion

        #region Constructors
        public BeepCheckBox() : base()
        {
            Padding = new Padding(1, 1, 1, 1);
            _beepImage = new BeepImage
            {
                Theme = Theme
            };
            BoundProperty = nameof(CurrentValue);

            TabStop = true;

            _textFont = SystemFonts.MessageBoxFont;
            _ownsTextFont = false;

            _painter = CheckBoxPainterFactory.GetPainter(_checkBoxStyle);
            ApplyCheckBoxControlStyleRecommendation();
            ApplyCheckBoxStyleRecommendations();

            ApplyTheme();
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            if (Width <= 0 || Height <= 0)
            {
                Width = 200;
                Height = 30;
            }
        }

        protected override void OnDpiChangedBeforeParent(EventArgs e)
        {
            base.OnDpiChangedBeforeParent(e);
            
            // Re-apply theme to update fonts with new DPI
            ApplyTheme();
            ClearGraphicsCaches();
            if (AutoSize)
            {
                Size = GetPreferredSize(Size.Empty);
            }
            RequestVisualRefresh(includeText: true);
        }
        #endregion

        #region Properties
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TextFont
        {
            get => _textFont;
            set
            {
                if (_ownsTextFont && _textFont != null)
                {
                    _textFont.Dispose();
                }
                _textFont = value;
                _ownsTextFont = false;
                //SafeApplyFont(_textFont);
                UseThemeFont = false;
                ClearGraphicsCaches();
                RequestVisualRefresh(includeText: true);
            }
        }

        public override string Text
        {
            get => base.Text;
            set
            {
                if (base.Text != value)
                {
                    base.Text = value;
                    _stateChanged = true;
                    RequestVisualRefresh(includeText: true);
                }
            }
        }

        [Category("Appearance")]
        [Description("Position of the text relative to the checkbox.")]
        [DefaultValue(TextAlignment.Right)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public TextAlignment TextAlignRelativeToCheckBox
        {
            get => _textAlignRelativeToCheckBox;
            set
            {
                _textAlignRelativeToCheckBox = value;
                RequestVisualRefresh(includeText: true);
            }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CheckMarkShape CheckMark { get; set; } = CheckMarkShape.Square;

        [Category("Appearance")]
        [Description("Checkbox visual style")]
        [DefaultValue(CheckBoxStyle.Material3)]
        public CheckBoxStyle CheckBoxStyle
        {
            get => _checkBoxStyle;
            set
            {
                if (_checkBoxStyle != value)
                {
                    _checkBoxStyle = value;
                    _painter = CheckBoxPainterFactory.GetPainter(_checkBoxStyle);
                    if (_syncControlStyleWithCheckBoxStyle)
                    {
                        ApplyCheckBoxControlStyleRecommendation();
                    }
                    if (_syncLayoutMetricsWithStyle)
                    {
                        ApplyCheckBoxStyleRecommendations();
                    }
                    else
                    {
                        ClearGraphicsCaches();
                        RequestVisualRefresh(includeText: true);
                    }
                }
            }
        }

        [Category("Appearance")]
        [Description("BaseControl visual style override. Setting this manually preserves the current CheckBoxStyle and disables automatic ControlStyle sync until re-enabled.")]
        public new BeepControlStyle ControlStyle
        {
            get => base.ControlStyle;
            set
            {
                if (base.ControlStyle != value)
                {
                    base.ControlStyle = value;
                    if (_syncControlStyleWithCheckBoxStyle)
                    {
                        _syncControlStyleWithCheckBoxStyle = false;
                    }
                    ClearGraphicsCaches();
                    RequestVisualRefresh(includeText: true);
                }
            }
        }

        [Category("Layout")]
        [Description("Automatically apply recommended CheckBoxSize, Spacing, MinimumHitTargetSize, and MinimumAutoSizeWidth values when CheckBoxStyle changes.")]
        [DefaultValue(true)]
        public bool SyncLayoutMetricsWithStyle
        {
            get => _syncLayoutMetricsWithStyle;
            set
            {
                if (_syncLayoutMetricsWithStyle != value)
                {
                    _syncLayoutMetricsWithStyle = value;
                    if (_syncLayoutMetricsWithStyle)
                    {
                        ApplyCheckBoxStyleRecommendations();
                    }
                }
            }
        }

        [Category("Appearance")]
        [Description("Automatically apply the mapped BaseControl ControlStyle when CheckBoxStyle changes.")]
        [DefaultValue(true)]
        public bool SyncControlStyleWithCheckBoxStyle
        {
            get => _syncControlStyleWithCheckBoxStyle;
            set
            {
                if (_syncControlStyleWithCheckBoxStyle != value)
                {
                    _syncControlStyleWithCheckBoxStyle = value;
                    if (_syncControlStyleWithCheckBoxStyle)
                    {
                        ApplyCheckBoxControlStyleRecommendation();
                    }
                }
            }
        }

        private void ApplyCheckBoxControlStyleRecommendation()
        {
            BeepControlStyle recommendedControlStyle = CheckBoxStyleHelpers.GetControlStyleForCheckBox(_checkBoxStyle);
            if (base.ControlStyle != recommendedControlStyle)
            {
                base.ControlStyle = recommendedControlStyle;
            }
        }

        private void ApplyCheckBoxStyleRecommendations()
        {
            checkboxsize = Math.Max(12, CheckBoxStyleHelpers.GetRecommendedCheckBoxSize(_checkBoxStyle));
            _spacing = Math.Max(2, CheckBoxStyleHelpers.GetRecommendedSpacing(_checkBoxStyle));
            _minimumHitTargetSize = Math.Max(24, CheckBoxStyleHelpers.GetRecommendedMinimumHitTargetSize(_checkBoxStyle));
            _minimumAutoSizeWidth = Math.Max(0, CheckBoxStyleHelpers.GetRecommendedMinimumAutoSizeWidth(_checkBoxStyle));

            ClearGraphicsCaches();

            if (AutoSize)
            {
                Size = GetPreferredSize(Size.Empty);
            }

            RequestVisualRefresh(includeText: true);
        }

        private void DisableLayoutSyncForManualOverrideIfNeeded(int normalizedValue, int recommendedValue)
        {
            if (_syncLayoutMetricsWithStyle && normalizedValue != recommendedValue)
            {
                _syncLayoutMetricsWithStyle = false;
            }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new CheckBoxState State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    SetStateCore(value, syncCurrentValue: true, raiseEvents: true, raiseSubmitChanges: true);
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Indicates whether the checkbox is checked.")]
        [DefaultValue(false)]
        public bool Checked
        {
            get => _state == CheckBoxState.Checked;
            set => State = value ? CheckBoxState.Checked : CheckBoxState.Unchecked;
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Gets or sets the current checkbox state.")]
        [DefaultValue(CheckBoxState.Unchecked)]
        public CheckBoxState CheckState
        {
            get => State;
            set => State = value;
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("When true, the control toggles through unchecked, checked, and indeterminate states.")]
        [DefaultValue(true)]
        public bool ThreeState
        {
            get => _threeState;
            set
            {
                if (_threeState == value)
                {
                    return;
                }

                _threeState = value;
                if (!_threeState && _state == CheckBoxState.Indeterminate)
                {
                    SetStateCore(CheckBoxState.Unchecked, syncCurrentValue: true, raiseEvents: true, raiseSubmitChanges: false);
                }

                RequestVisualRefresh(includeText: true);
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("When true, keyboard and mouse interaction toggle the checkbox state automatically.")]
        [DefaultValue(true)]
        public bool AutoCheck
        {
            get => _autoCheck;
            set => _autoCheck = value;
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Determines whether mouse clicks toggle from anywhere in the control or only from the painted checkbox glyph.")]
        [DefaultValue(CheckBoxMouseHitMode.WholeControl)]
        public CheckBoxMouseHitMode MouseHitMode
        {
            get => _mouseHitMode;
            set => _mouseHitMode = value;
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Prevents user interaction from changing the state while keeping the control enabled.")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get => IsReadOnly;
            set
            {
                if (IsReadOnly == value)
                {
                    return;
                }

                IsReadOnly = value;
                RequestVisualRefresh(includeText: false);
            }
        }

        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public T CheckedValue
        {
            get => _checkedValue;
            set
            {
                _checkedValue = value;
                if (!_hasCheckedStateValue)
                {
                    _checkedStateValue = value;
                }
                UpdateCurrentValue();
            }
        }

        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public T UncheckedValue
        {
            get => _uncheckedValue;
            set
            {
                _uncheckedValue = value;
                if (!_hasUncheckedStateValue)
                {
                    _uncheckedStateValue = value;
                }
                UpdateCurrentValue();
            }
        }

        [Category("Behavior")]
        [Description("Optional explicit value mapped to Checked state. When enabled, it can be synchronized to legacy CheckedValue.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public T CheckedStateValue
        {
            get => _checkedStateValue;
            set
            {
                _checkedStateValue = value;
                _hasCheckedStateValue = true;
                if (_mapStateValuesToLegacy)
                {
                    _checkedValue = value;
                }
                UpdateCurrentValue();
            }
        }

        [Category("Behavior")]
        [Description("Optional explicit value mapped to Unchecked state. When enabled, it can be synchronized to legacy UncheckedValue.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public T UncheckedStateValue
        {
            get => _uncheckedStateValue;
            set
            {
                _uncheckedStateValue = value;
                _hasUncheckedStateValue = true;
                if (_mapStateValuesToLegacy)
                {
                    _uncheckedValue = value;
                }
                UpdateCurrentValue();
            }
        }

        [Category("Behavior")]
        [Description("Optional explicit value mapped to Indeterminate state.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public T IndeterminateStateValue
        {
            get => _indeterminateStateValue;
            set
            {
                _indeterminateStateValue = value;
                _hasIndeterminateStateValue = true;
                UpdateCurrentValue();
            }
        }

        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("When true, CheckedStateValue/UncheckedStateValue are synchronized to legacy CheckedValue/UncheckedValue for backward compatibility.")]
        public bool MapStateValuesToLegacy
        {
            get => _mapStateValuesToLegacy;
            set
            {
                _mapStateValuesToLegacy = value;
                if (_mapStateValuesToLegacy)
                {
                    if (_hasCheckedStateValue)
                    {
                        _checkedValue = _checkedStateValue;
                    }
                    if (_hasUncheckedStateValue)
                    {
                        _uncheckedValue = _uncheckedStateValue;
                    }
                }
                UpdateCurrentValue();
            }
        }

        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("When IndeterminateStateValue is not set, map indeterminate state to UncheckedValue (true) or CheckedValue (false).")]
        public bool UseUncheckedValueForIndeterminate
        {
            get => _useUncheckedValueForIndeterminate;
            set
            {
                _useUncheckedValueForIndeterminate = value;
                UpdateCurrentValue();
            }
        }

        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public T CurrentValue
        {
            get => _currentValue;
            set => SetCurrentValueCore(value, raiseEvents: !_suppressCurrentValueNotifications, raiseSubmitChanges: !_suppressCurrentValueNotifications);
        }

        [Category("Appearance")]
        public bool HideText
        {
            get => _hideText;
            set
            {
                _hideText = value;
                RequestVisualRefresh(includeText: true);
            }
        }

        [Category("Appearance")]
        [Description("Select the image file (SVG, PNG, JPG, etc.) to load as the custom check mark.")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string ImagePath
        {
            get => _beepImage.ImagePath;
            set
            {
                _beepImage.ImagePath = value;
                if (_beepImage.IsSvgPath(value))
                {
                    _beepImage.ApplyThemeToSvg();
                }
                _beepImage.ApplyTheme();
                ApplyTheme();
                RequestVisualRefresh(includeText: false);
            }
        }

        [Category("Appearance")]
        [Description("Icon path used for checked glyph rendering. When empty, icon resolves from IconsManagement defaults.")]
        public string CheckIconPath
        {
            get => string.IsNullOrWhiteSpace(_checkIconPath) ? CheckBoxIconHelpers.GetCheckIconPath() : _checkIconPath;
            set
            {
                _checkIconPath = value;
                RequestVisualRefresh(includeText: false);
            }
        }

        [Category("Appearance")]
        [Description("Icon path used for indeterminate glyph rendering. When empty, icon resolves from IconsManagement defaults.")]
        public string IndeterminateIconPath
        {
            get => string.IsNullOrWhiteSpace(_indeterminateIconPath) ? CheckBoxIconHelpers.GetIndeterminateIconPath() : _indeterminateIconPath;
            set
            {
                _indeterminateIconPath = value;
                RequestVisualRefresh(includeText: false);
            }
        }

        [Category("Layout")]
        [Description("Minimum clickable target size for checkbox interactions.")]
        [DefaultValue(32)]
        public int MinimumHitTargetSize
        {
            get => _minimumHitTargetSize;
            set
            {
                int normalizedValue = Math.Max(24, value);
                DisableLayoutSyncForManualOverrideIfNeeded(normalizedValue, Math.Max(24, CheckBoxStyleHelpers.GetRecommendedMinimumHitTargetSize(_checkBoxStyle)));
                _minimumHitTargetSize = normalizedValue;
                if (AutoSize)
                {
                    Size = GetPreferredSize(Size.Empty);
                }
                RequestVisualRefresh(includeText: true);
            }
        }

        [Category("Layout")]
        [Description("Minimum width used when AutoSize computes the preferred checkbox size. 0 disables an extra width floor.")]
        [DefaultValue(0)]
        public int MinimumAutoSizeWidth
        {
            get => _minimumAutoSizeWidth;
            set
            {
                int normalizedValue = Math.Max(0, value);
                DisableLayoutSyncForManualOverrideIfNeeded(normalizedValue, Math.Max(0, CheckBoxStyleHelpers.GetRecommendedMinimumAutoSizeWidth(_checkBoxStyle)));
                _minimumAutoSizeWidth = normalizedValue;
                if (AutoSize)
                {
                    Size = GetPreferredSize(Size.Empty);
                }
                RequestVisualRefresh(includeText: true);
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Value of the checkbox.")]
        [DefaultValue(15)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int CheckBoxSize
        {
            get { return checkboxsize; }
            set
            {
                int normalizedValue = Math.Max(12, value);
                DisableLayoutSyncForManualOverrideIfNeeded(normalizedValue, Math.Max(12, CheckBoxStyleHelpers.GetRecommendedCheckBoxSize(_checkBoxStyle)));
                checkboxsize = normalizedValue;
                if (AutoSize)
                {
                    Size = GetPreferredSize(Size.Empty);
                }
                RequestVisualRefresh(includeText: true);
            }
        }

        public int Spacing
        {
            get => _spacing;
            set
            {
                int normalizedValue = Math.Max(2, value);
                DisableLayoutSyncForManualOverrideIfNeeded(normalizedValue, Math.Max(2, CheckBoxStyleHelpers.GetRecommendedSpacing(_checkBoxStyle)));
                _spacing = normalizedValue;
                if (AutoSize)
                {
                    Size = GetPreferredSize(Size.Empty);
                }
                RequestVisualRefresh(includeText: true);
            }
        }

        [Category("Behavior")]
        [Description("Indicates whether the control can be navigated to using the Tab key.")]
        [DefaultValue(true)]
        public new bool TabStop
        {
            get => base.TabStop;
            set => base.TabStop = value;
        }

        private bool _autoSize = false;
        [Browsable(true)]
        [Category("Layout")]
        [Description("Automatically resize the control based on the text and image size.")]
        public override bool AutoSize
        {
            get => _autoSize;
            set
            {
                _autoSize = value;
                if (_autoSize)
                {
                    this.Size = GetPreferredSize(Size.Empty);
                }
                RequestVisualRefresh(includeText: true);
            }
        }
        #endregion

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            // Apply theme font only while theme-font mode is enabled.
            // This preserves user-assigned TextFont values during design-time/property-grid edits.
            if (UseThemeFont || _textFont == null)
            {
                if (_ownsTextFont && _textFont != null)
                {
                    _textFont.Dispose();
                }

                // Apply font theme based on ControlStyle
                _textFont = CheckBoxFontHelpers.GetCheckBoxFont(_currentTheme, ControlStyle, this);
                _ownsTextFont = true;
            }
          //  SafeApplyFont(_textFont);
            ClearGraphicsCaches();

            if (_currentTheme != null && _beepImage != null)
            {
                // Update specific theme properties if needed
                _beepImage.Theme = Theme;
                _beepImage.ApplyTheme();
            }

            RequestVisualRefresh(includeText: true);
        }

        #region Accessibility

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new BeepCheckBoxAccessibleObject(this);
        }

        private sealed class BeepCheckBoxAccessibleObject : ControlAccessibleObject
        {
            private readonly BeepCheckBox<T> _owner;

            public BeepCheckBoxAccessibleObject(BeepCheckBox<T> owner) : base(owner)
            {
                _owner = owner;
            }

            public override string Name =>
                !string.IsNullOrEmpty(_owner.AccessibleName)
                    ? _owner.AccessibleName
                    : _owner.Text;

            public override string Description =>
                !string.IsNullOrEmpty(_owner.AccessibleDescription)
                    ? _owner.AccessibleDescription
                    : _owner._state switch
                    {
                        CheckBoxState.Checked => "Checked",
                        CheckBoxState.Indeterminate => "Indeterminate",
                        _ => "Unchecked"
                    };

            public override string DefaultAction =>
                _owner._state == CheckBoxState.Checked ? "Uncheck" : "Check";

            public override AccessibleRole Role => AccessibleRole.CheckButton;

            public override AccessibleStates State
            {
                get
                {
                    var states = AccessibleStates.Focusable;
                    if (_owner.Focused)
                        states |= AccessibleStates.Focused;
                    if (!_owner.Enabled)
                        states |= AccessibleStates.Unavailable;
                    if (_owner.IsReadOnly)
                        states |= AccessibleStates.ReadOnly;
                    if (_owner._state == CheckBoxState.Checked)
                        states |= AccessibleStates.Checked;
                    if (_owner._state == CheckBoxState.Indeterminate)
                        states |= AccessibleStates.Indeterminate;
                    return states;
                }
            }

            public override void DoDefaultAction()
            {
                if (_owner.Enabled && !_owner.IsReadOnly && _owner.AutoCheck)
                {
                    _owner.ToggleState();
                }
            }
        }

        #endregion

        #region Events
        public event EventHandler? CheckStateChanged;
        public event EventHandler? CheckedChanged;

        // Declare the StateChanged event
        public event EventHandler? StateChanged;
        #endregion
    }
}

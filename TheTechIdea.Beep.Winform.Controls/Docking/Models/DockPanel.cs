using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Models
{
    /// <summary>
    /// A visual dockable panel.
    /// Inherits Panel so it renders as a real control on the host form —
    /// following DockPanelSuite (DockPanel : Panel) and Krypton (KryptonPanel : Panel).
    /// 
    /// The BeepDockingManager creates, positions and removes instances at runtime.
    /// All docking properties serialise into .designer.cs via the WinForms designer.
    /// </summary>
    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [DesignTimeVisible(false)]
    [Category("Docking")]
    [Description("Docking panel component.")]
    [Designer("TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.Designers.DockPanelDesigner, TheTechIdea.Beep.Winform.Controls.Design.Server")]
    [DefaultEvent(nameof(Activated))]
    [DefaultProperty(nameof(Title))]
    public class DockPanel : Panel
    {
        private BeepDockingManager _manager;
        private string _key = string.Empty;
        private string _title = "Tool Window";
        private string _iconPath = string.Empty;
        private DockPanelState _state = DockPanelState.Docked;
        private DockPosition _dockPosition = DockPosition.Left;
        private int _preferredWidth = 250;
        private int _preferredHeight = 150;
        private bool _canClose = true;
        private bool _canFloat = true;
        private bool _canAutoHide = true;
        private DockAreas _allowedAreas = DockAreas.All;
        private DockingThemeColors _themeColors = DockingThemeColors.Default;

        /// <summary>
        /// Unique identifier for this panel (must be unique within the host).
        /// </summary>
        [Category("Docking")]
        [Description("Unique key for this panel within the docking host")]
        [DefaultValue("")]
        public string Key
        {
            get => _key;
            set
            {
                if (_key == value) return;
                _key = value;
                OnPropertyChanged(nameof(Key));
                TryRegisterWithManager();
            }
        }

        /// <summary>
        /// The BeepDockingManager that orchestrates this panel at runtime.
        /// </summary>
        [Category("Docking")]
        [Description("The docking manager that owns this panel")]
        [Browsable(false)]
        [TypeConverter(typeof(ReferenceConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public BeepDockingManager Manager
        {
            get => _manager;
            set
            {
                if (_manager == value) return;
                var previous = _manager;
                if (previous != null && !IsDesigning)
                    previous.UnregisterExistingPanel(this);

                _manager = value;
                _manager?.ApplyThemeToPanel(this);
                OnPropertyChanged(nameof(Manager));
                TryRegisterWithManager();
            }
        }

        /// <summary>
        /// True when hosted in the Visual Studio designer.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDesigning =>
            Site?.DesignMode == true ||
            LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            IsWinFormsDesignerProcess();

        /// <summary>
        /// Gets the rectangle available for hosted content.
        /// Header/tab chrome is owned by <see cref="BeepDockspace"/>, not DockPanel.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle ContentBounds => new Rectangle(0, 0, Width, Height);

        /// <summary>Display title shown in the caption strip.</summary>
        [Category("Docking")]
        [Description("Title displayed in the panel caption")]
        [DefaultValue("Tool Window")]
        public string Title
        {
            get => _title;
            set
            {
                if (_title == value) return;
                _title = value;
                Invalidate();
                OnPropertyChanged(nameof(Title));
                if (!IsDesigning)
                    _manager?.NotifyPanelTitleChanged(this);
            }
        }

        /// <summary>Path to icon file. Leave empty for no icon.</summary>
        [Category("Docking")]
        [Description("Optional path to icon file shown in the caption")]
        [DefaultValue("")]
        public string IconPath
        {
            get => _iconPath;
            set
            {
                if (_iconPath == value) return;
                _iconPath = value;
                Invalidate();
                OnPropertyChanged(nameof(IconPath));
            }
        }

        /// <summary>
        /// The user control / panel containing this panel's content.
        /// Set at runtime; not serialized by the designer.
        /// When set the control is added as a child and fills ContentBounds.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Control Content
        {
            get => _content;
            set
            {
                if (_content == value) return;

                if (_content != null)
                    Controls.Remove(_content);

                _content = value;

                if (_content != null)
                {
                    _content.Bounds = ContentBounds;
                    _content.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                    Controls.Add(_content);
                }
            }
        }
        private Control _content;

        /// <summary>Current display state of the panel.</summary>
        [Category("Docking")]
        [Description("Current display state of the panel")]
        [DefaultValue(DockPanelState.Docked)]
        public DockPanelState State
        {
            get => _state;
            set
            {
                if (_state == value) return;
                _state = value;
                OnPropertyChanged(nameof(State));
            }
        }

        /// <summary>The edge this panel docks to.</summary>
        [Category("Docking")]
        [Description("The edge this panel docks to")]
        [DefaultValue(DockPosition.Left)]
        public DockPosition DockPosition
        {
            get => _dockPosition;
            set
            {
                if (_dockPosition == value) return;
                var oldPosition = _dockPosition;
                _dockPosition = value;
                OnPropertyChanged(nameof(DockPosition));
                if (!IsDesigning)
                    _manager?.NotifyPanelDockPositionChanged(this, oldPosition);
            }
        }

        /// <summary>Preferred width when docked Left or Right.</summary>
        [Category("Docking")]
        [Description("Preferred width when docked Left or Right")]
        [DefaultValue(250)]
        public int PreferredWidth
        {
            get => _preferredWidth;
            set
            {
                if (_preferredWidth == value) return;
                _preferredWidth = value;
                OnPropertyChanged(nameof(PreferredWidth));
                if (!IsDesigning)
                    _manager?.NotifyPanelPreferredSizeChanged(this);
            }
        }

        /// <summary>Preferred height when docked Top or Bottom.</summary>
        [Category("Docking")]
        [Description("Preferred height when docked Top or Bottom")]
        [DefaultValue(150)]
        public int PreferredHeight
        {
            get => _preferredHeight;
            set
            {
                if (_preferredHeight == value) return;
                _preferredHeight = value;
                OnPropertyChanged(nameof(PreferredHeight));
                if (!IsDesigning)
                    _manager?.NotifyPanelPreferredSizeChanged(this);
            }
        }

        /// <summary>The group this panel is currently in (set by DockGroup).</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockGroup Group { get; internal set; }

        /// <summary>Whether the user can close this panel.</summary>
        [Category("Docking")]
        [Description("Whether the user can close this panel")]
        [DefaultValue(true)]
        public bool CanClose
        {
            get => _canClose;
            set
            {
                if (_canClose == value) return;
                _canClose = value;
                Invalidate();
                OnPropertyChanged(nameof(CanClose));
            }
        }

        /// <summary>Whether this panel can float as a separate window.</summary>
        [Category("Docking")]
        [Description("Whether this panel can float as a separate window")]
        [DefaultValue(true)]
        public bool CanFloat
        {
            get => _canFloat;
            set
            {
                if (_canFloat == value) return;
                _canFloat = value;
                Invalidate();
                OnPropertyChanged(nameof(CanFloat));
            }
        }

        /// <summary>Whether this panel can be auto-hidden to a side tab strip.</summary>
        [Category("Docking")]
        [Description("Whether this panel can be auto-hidden")]
        [DefaultValue(true)]
        public bool CanAutoHide
        {
            get => _canAutoHide;
            set
            {
                if (_canAutoHide == value) return;
                _canAutoHide = value;
                Invalidate();
                OnPropertyChanged(nameof(CanAutoHide));
            }
        }

        /// <summary>
        /// Flags controlling which dock positions and states are allowed.
        /// Mirrors DockContent.DockAreas in DockPanelSuite and per-page flags in Krypton.
        /// </summary>
        [Category("Docking")]
        [Description("Flags controlling which dock positions and states are allowed for this panel")]
        [DefaultValue(DockAreas.All)]
        public DockAreas AllowedAreas
        {
            get => _allowedAreas;
            set
            {
                if (_allowedAreas == value) return;
                _allowedAreas = value;
                _canFloat = (_allowedAreas & DockAreas.Float) != 0;
                _canAutoHide = (_allowedAreas & DockAreas.AutoHide) != 0;
                Invalidate();
                OnPropertyChanged(nameof(AllowedAreas));
            }
        }

        /// <summary>Whether this panel's content has unsaved changes.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDirty { get; set; }

        /// <summary>Cached native window handle. Set by runtime manager when using Win32 MDI path.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IntPtr NativeHandle { get; internal set; } = IntPtr.Zero;

        /// <summary>Cached layout bounds set by the layout engine.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle LayoutBounds { get; internal set; } = Rectangle.Empty;

        /// <summary>Cached tab strip bounds for this panel.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle TabBounds { get; internal set; } = Rectangle.Empty;

        /// <summary>Optional user-defined metadata tag.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new object Tag { get; set; }

        // ── events ──────────────────────────────────────────────────────────

        /// <summary>Raised when this panel is activated (becomes active in its group).</summary>
        public event EventHandler Activated;

        /// <summary>Raised when this panel is deactivated.</summary>
        public event EventHandler Deactivated;

        /// <summary>Raised when this panel is closed.</summary>
        public event EventHandler Closed;

        /// <summary>Raised when any docking property changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        // ── constructor ──────────────────────────────────────────────────────

        public DockPanel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.ResizeRedraw, true);

            BorderStyle = BorderStyle.None;
            Padding = Padding.Empty;
        }

        internal void ApplyDockingTheme(DockingThemeColors colors)
        {
            _themeColors = colors ?? DockingThemeColors.Default;
            BackColor = _themeColors.PanelBackColor;
            ForeColor = _themeColors.PanelForeColor;
            Invalidate();
        }

        // ── visual rendering ─────────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (_content != null)
                _content.Bounds = ContentBounds;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            Padding = Padding.Empty;
            TryRegisterWithManager();
        }

        private void TryRegisterWithManager()
        {
            if (_manager == null || IsDesigning || Parent == null || string.IsNullOrWhiteSpace(_key))
                return;

            _manager.RegisterExistingPanel(this);
        }

        // ── diagnostics / overrides ──────────────────────────────────────────

        public override string ToString() =>
            $"DockPanel[Key={Key}, Title={Title}, State={State}, Position={DockPosition}]";

        // ── internal event raisers ───────────────────────────────────────────

        internal void OnActivated() => Activated?.Invoke(this, EventArgs.Empty);
        internal void OnDeactivated() => Deactivated?.Invoke(this, EventArgs.Empty);
        internal void OnClosed() => Closed?.Invoke(this, EventArgs.Empty);
        internal void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private static bool IsWinFormsDesignerProcess()
        {
            try
            {
                string processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                return processName.IndexOf("DesignToolsServer", StringComparison.OrdinalIgnoreCase) >= 0 ||
                       string.Equals(processName, "devenv", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}

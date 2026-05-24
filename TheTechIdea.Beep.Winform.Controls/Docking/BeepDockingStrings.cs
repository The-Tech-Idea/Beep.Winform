using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// Storage for docking manager display strings.
    /// Replace individual properties to localise without touching runtime logic.
    /// Mirrors Krypton's <c>DockingManagerStrings</c> with backing fields,
    /// <see cref="PropertyChanged"/>, <c>Reset*</c> helpers, and <see cref="IsDefault"/>.
    /// </summary>
    public class BeepDockingStrings : INotifyPropertyChanged
    {
        // ── Default constants ─────────────────────────────────────────────────
        private const string DefaultTextAutoHide        = "Auto Hide";
        private const string DefaultTextClose           = "Close";
        private const string DefaultTextCloseAllButThis = "Close All But This";
        private const string DefaultTextDock            = "Dock";
        private const string DefaultTextFloat           = "Float";
        private const string DefaultTextHide            = "Hide";
        private const string DefaultTextTabbedDocument  = "Tabbed Document";
        private const string DefaultTextWindowLocation  = "Window Position";

        // ── Backing fields ────────────────────────────────────────────────────
        private string _textAutoHide        = DefaultTextAutoHide;
        private string _textClose           = DefaultTextClose;
        private string _textCloseAllButThis = DefaultTextCloseAllButThis;
        private string _textDock            = DefaultTextDock;
        private string _textFloat           = DefaultTextFloat;
        private string _textHide            = DefaultTextHide;
        private string _textTabbedDocument  = DefaultTextTabbedDocument;
        private string _textWindowLocation  = DefaultTextWindowLocation;

        // ── Events ────────────────────────────────────────────────────────────

        /// <summary>Occurs whenever a property value changes.</summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        // ── IsDefault ─────────────────────────────────────────────────────────

        /// <summary>
        /// Gets a value indicating whether all properties are at their default values.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDefault =>
            _textAutoHide.Equals(DefaultTextAutoHide)           &&
            _textClose.Equals(DefaultTextClose)                 &&
            _textCloseAllButThis.Equals(DefaultTextCloseAllButThis) &&
            _textDock.Equals(DefaultTextDock)                   &&
            _textFloat.Equals(DefaultTextFloat)                 &&
            _textHide.Equals(DefaultTextHide)                   &&
            _textTabbedDocument.Equals(DefaultTextTabbedDocument) &&
            _textWindowLocation.Equals(DefaultTextWindowLocation);

        // ── TextAutoHide ──────────────────────────────────────────────────────

        /// <summary>Gets or sets the text for the Auto Hide button tooltip and menu item.</summary>
        [Category("Visuals")]
        [Description("Text to use for the auto hide button tooltip and menu item.")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(DefaultTextAutoHide)]
        [Localizable(true)]
        public string TextAutoHide
        {
            get => _textAutoHide;
            set
            {
                if (_textAutoHide != value)
                {
                    _textAutoHide = value;
                    OnPropertyChanged(nameof(TextAutoHide));
                }
            }
        }

        /// <summary>Resets <see cref="TextAutoHide"/> to its default value.</summary>
        public void ResetTextAutoHide() => TextAutoHide = DefaultTextAutoHide;

        // ── TextClose ─────────────────────────────────────────────────────────

        /// <summary>Gets or sets the text for the Close button tooltip and menu item.</summary>
        [Category("Visuals")]
        [Description("Text to use for the close button tooltip and menu item.")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(DefaultTextClose)]
        [Localizable(true)]
        public string TextClose
        {
            get => _textClose;
            set
            {
                if (_textClose != value)
                {
                    _textClose = value;
                    OnPropertyChanged(nameof(TextClose));
                }
            }
        }

        /// <summary>Resets <see cref="TextClose"/> to its default value.</summary>
        public void ResetTextClose() => TextClose = DefaultTextClose;

        // ── TextCloseAllButThis ───────────────────────────────────────────────

        /// <summary>Gets or sets the text for the "Close All But This" menu item.</summary>
        [Category("Visuals")]
        [Description("Text to use for the 'Close All But This' menu item.")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(DefaultTextCloseAllButThis)]
        [Localizable(true)]
        public string TextCloseAllButThis
        {
            get => _textCloseAllButThis;
            set
            {
                if (_textCloseAllButThis != value)
                {
                    _textCloseAllButThis = value;
                    OnPropertyChanged(nameof(TextCloseAllButThis));
                }
            }
        }

        /// <summary>Resets <see cref="TextCloseAllButThis"/> to its default value.</summary>
        public void ResetTextCloseAllButThis() => TextCloseAllButThis = DefaultTextCloseAllButThis;

        // ── TextDock ──────────────────────────────────────────────────────────

        /// <summary>Gets or sets the text for the Dock menu item.</summary>
        [Category("Visuals")]
        [Description("Text to use for the dock menu item.")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(DefaultTextDock)]
        [Localizable(true)]
        public string TextDock
        {
            get => _textDock;
            set
            {
                if (_textDock != value)
                {
                    _textDock = value;
                    OnPropertyChanged(nameof(TextDock));
                }
            }
        }

        /// <summary>Resets <see cref="TextDock"/> to its default value.</summary>
        public void ResetTextDock() => TextDock = DefaultTextDock;

        // ── TextFloat ─────────────────────────────────────────────────────────

        /// <summary>Gets or sets the text for the Float menu item.</summary>
        [Category("Visuals")]
        [Description("Text to use for the float menu item.")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(DefaultTextFloat)]
        [Localizable(true)]
        public string TextFloat
        {
            get => _textFloat;
            set
            {
                if (_textFloat != value)
                {
                    _textFloat = value;
                    OnPropertyChanged(nameof(TextFloat));
                }
            }
        }

        /// <summary>Resets <see cref="TextFloat"/> to its default value.</summary>
        public void ResetTextFloat() => TextFloat = DefaultTextFloat;

        // ── TextHide ──────────────────────────────────────────────────────────

        /// <summary>Gets or sets the text for the Hide menu item.</summary>
        [Category("Visuals")]
        [Description("Text to use for the hide menu item.")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(DefaultTextHide)]
        [Localizable(true)]
        public string TextHide
        {
            get => _textHide;
            set
            {
                if (_textHide != value)
                {
                    _textHide = value;
                    OnPropertyChanged(nameof(TextHide));
                }
            }
        }

        /// <summary>Resets <see cref="TextHide"/> to its default value.</summary>
        public void ResetTextHide() => TextHide = DefaultTextHide;

        // ── TextTabbedDocument ────────────────────────────────────────────────

        /// <summary>Gets or sets the text for the Tabbed Document menu item.</summary>
        [Category("Visuals")]
        [Description("Text to use for the tabbed document menu item.")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(DefaultTextTabbedDocument)]
        [Localizable(true)]
        public string TextTabbedDocument
        {
            get => _textTabbedDocument;
            set
            {
                if (_textTabbedDocument != value)
                {
                    _textTabbedDocument = value;
                    OnPropertyChanged(nameof(TextTabbedDocument));
                }
            }
        }

        /// <summary>Resets <see cref="TextTabbedDocument"/> to its default value.</summary>
        public void ResetTextTabbedDocument() => TextTabbedDocument = DefaultTextTabbedDocument;

        // ── TextWindowLocation ────────────────────────────────────────────────

        /// <summary>Gets or sets the text for the Window Position drop-down button tooltip.</summary>
        [Category("Visuals")]
        [Description("Text to use for the window position drop-down button tooltip.")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(DefaultTextWindowLocation)]
        [Localizable(true)]
        public string TextWindowLocation
        {
            get => _textWindowLocation;
            set
            {
                if (_textWindowLocation != value)
                {
                    _textWindowLocation = value;
                    OnPropertyChanged(nameof(TextWindowLocation));
                }
            }
        }

        /// <summary>Resets <see cref="TextWindowLocation"/> to its default value.</summary>
        public void ResetTextWindowLocation() => TextWindowLocation = DefaultTextWindowLocation;

        // ── Protected ─────────────────────────────────────────────────────────

        /// <summary>Raises the <see cref="PropertyChanged"/> event.</summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

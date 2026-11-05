using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.GridX.Footer;
using TheTechIdea.Beep.Winform.Controls.GridX.Formatting;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    public partial class BeepGridPro
    {
        private GridConditionalFormatEngine? _conditionalFormatting;
        private readonly GridFooterConfig _footer = new();
        private bool _footerSubscribed;

        /// <summary>
        /// Conditional formatting rules applied during paint. Rules are
        /// evaluated once per frame and cached by row/column.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridConditionalFormatEngine ConditionalFormatting
            => _conditionalFormatting ??= new GridConditionalFormatEngine(this);

        /// <summary>
        /// Pinned aggregate footer configuration. Set
        /// <see cref="GridFooterConfig.Visible"/> and per-column aggregates.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridFooterConfig Footer
        {
            get
            {
                if (!_footerSubscribed)
                {
                    _footer.Changed += OnFooterChanged;
                    _footerSubscribed = true;
                }
                return _footer;
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Shows a pinned aggregate footer below the rows.")]
        public bool ShowFooter
        {
            get => Footer.Visible;
            set
            {
                if (Footer.Visible == value) return;
                Footer.Visible = value;
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(28)]
        [Description("Height in pixels of the pinned aggregate footer.")]
        public int FooterHeight
        {
            get => Footer.Height;
            set
            {
                if (Footer.Height == value) return;
                Footer.Height = value;
            }
        }

        private void OnFooterChanged(object? sender, System.EventArgs e)
        {
            Layout?.Recalculate();
            ScrollBars?.UpdateBars();
            SafeInvalidate();
        }

        /// <summary>The rectangle reserved for the pinned footer, in client coordinates.</summary>
        [Browsable(false)]
        public System.Drawing.Rectangle FooterRect => Layout?.FooterRect ?? System.Drawing.Rectangle.Empty;
    }
}

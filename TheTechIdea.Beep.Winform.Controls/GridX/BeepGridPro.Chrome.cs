using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.GridX.Chrome;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    public partial class BeepGridPro
    {
        private GridChromeDensity _chromeDensity = GridChromeDensity.Standard;
        private int _focusedHeaderColumnIndex = -1;

        /// <summary>
        /// Column receiving header keyboard focus, or -1 when the body owns
        /// focus. F6 toggles header focus while the header band is visible.
        /// </summary>
        internal int FocusedHeaderColumnIndex
        {
            get => _focusedHeaderColumnIndex;
            set
            {
                int next = value;
                if (next < -1 || next >= Data.Columns.Count) next = -1;
                if (_focusedHeaderColumnIndex == next) return;
                _focusedHeaderColumnIndex = next;
                if (!Layout.HeaderRect.IsEmpty)
                    SafeInvalidate(Layout.HeaderRect);
            }
        }

        /// <summary>
        /// Density used by headers, toolbar/search, chips, and the navigator.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(GridChromeDensity.Standard)]
        [Description("Controls header, toolbar, chip, and navigator density.")]
        public GridChromeDensity ChromeDensity
        {
            get => _chromeDensity;
            set
            {
                if (_chromeDensity == value) return;
                _chromeDensity = value;
                Layout?.Recalculate();
                SafeInvalidate();
            }
        }

        [Browsable(false)]
        public GridChromeMetrics ChromeMetrics => GridChromeMetrics.Resolve(this);

        [Browsable(false)]
        public GridChromeState ChromeState => GridChromeState.Resolve(this);
    }
}

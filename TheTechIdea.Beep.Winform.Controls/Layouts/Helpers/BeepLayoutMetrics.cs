using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    public static class BeepLayoutMetrics
    {
        // ── Dialog Sizes ──────────────────────────────────────────────────
        public static readonly Size DialogSmall  = new(420, 320);
        public static readonly Size DialogMedium = new(600, 460);
        public static readonly Size DialogLarge  = new(840, 560);

        // ── Padding ────────────────────────────────────────────────────────
        public static readonly Padding DialogPadding    = new(12);
        public static readonly Padding ContainerPadding = new(8);
        public static readonly Padding ButtonStripPd    = new(10);
        public static readonly Padding HeaderPadding    = new(16, 8, 16, 8);

        public const int ButtonGap  = 8;
        public const int SmallGap   = 4;

        // ── Oracle Forms visual cues ──────────────────────────────────────
        /// <summary>
        /// Background color used by a data block when it is in Enter-Query /
        /// Query mode. Mirrors the yellow tint Oracle Forms applies to
        /// signal "you are typing criteria, not data".
        /// </summary>
        public static readonly Color QueryModeBackground = Color.FromArgb(255, 250, 205);

        // ── Layout Grid ────────────────────────────────────────────────────
        public const int LabelColumnWidth = 130;
        public const int TextRowHeight    = 35;
        public const int InterRowSpacing  = 5;
        public const int CornerRadius     = 4;

        // ── Accessibility ──────────────────────────────────────────────────
        public const int MinTouchTarget   = 44;
        public const double MinContrast   = 4.5;

        // ── Button Sizing ─────────────────────────────────────────────────
        public static readonly Size ButtonStandard = new(100, 32);
        public static readonly Size ButtonSmall    = new(80, 28);
        public static readonly Size ButtonLarge    = new(130, 36);
        public static readonly Size ButtonToolbar  = new(110, 32);

        // ── Input / Field Sizes ───────────────────────────────────────────
        public static readonly Size FieldStandard    = new(200, 34);   // textbox, combobox, etc.
        public static readonly Size FieldMultiline  = new(200, 80);   // multiline textareas
        public static readonly Size FieldDate       = new(200, 36);   // date picker
        public static readonly Size FieldTime       = new(150, 36);   // time picker
        public static readonly Size FieldDateRange  = new(240, 34);   // date-range picker
        public static readonly Size FieldDateTime   = new(200, 34);   // combined datetime picker
        public static readonly Size FieldNumeric    = new(100, 34);   // numeric up/down
        public static readonly Size FieldDateView    = new(200, 200);  // calendar-style date view

        // ── Inline / Small Controls ──────────────────────────────────────
        public static readonly Size LabelStandard    = new(120, 24);
        public static readonly Size CheckboxStandard = new(140, 24);
        public static readonly Size ToggleStandard   = new(50, 26);
        public static readonly Size SwitchStandard   = new(50, 26);
        public static readonly Size RatingStandard   = new(120, 24);

        // ── Cards ─────────────────────────────────────────────────────────
        public static readonly Size Card             = new(280, 160); // general-purpose card
        public static readonly Size CardFeature      = new(320, 240); // feature card
        public static readonly Size CardStat         = new(200, 100); // stat / KPI card
        public static readonly Size CardTask         = new(300, 100); // task card row
        public static readonly Size CardTestimonial  = new(300, 200); // testimonial card
        public static readonly Size CardMetric       = new(200, 120); // metric tile
        public static readonly Size CardProject      = new(320, 200); // project card
        public static readonly Size CompanyProfile   = new(300, 150);

        // ── Containers / Panels ───────────────────────────────────────────
        public static readonly Size Panel            = new(300, 200); // generic grouping surface
        public static readonly Size MultiSplitter    = new(300, 200);
        public static readonly Size LayoutControl    = new(300, 200); // BeepLayoutControl
        public static readonly Size ScrollList       = new(280, 200);
        public static readonly Size DisplayContainer = new(400, 300);
        public static readonly Size FlyoutPanel      = new(240, 300);

        // ── Tabs ──────────────────────────────────────────────────────────
        public static readonly Size TabsStrip        = new(400, 32);  // tab header strip
        public static readonly Size TabHost          = new(400, 200); // tab host container
        public static readonly Size TabPage          = new(400, 200); // individual tab page

        // ── Lists / Trees ─────────────────────────────────────────────────
        public static readonly Size ListBox          = new(280, 200);
        public static readonly Size ListOfValues     = new(300, 240);
        public static readonly Size ChipList         = new(200, 200);
        public static readonly Size RadioList        = new(200, 200);
        public static readonly Size Tree             = new(250, 300);
        public static readonly Size HierarchicalRadio = new(200, 200);
        public static readonly Size RadioGroup       = new(200, 100);
        public static readonly Size DropDownSelect   = new(200, 34);
        public static readonly Size ComboDropdown    = new(200, 200);
        public static readonly Size MultiChipGroup   = new(300, 32);
        public static readonly Size ComboBox         = new(200, 34);

        // ── Data / Grids ─────────────────────────────────────────────────
        public static readonly Size Grid             = new(600, 300);
        public static readonly Size VerticalTable    = new(300, 200);
        public static readonly Size DataRecord       = new(400, 32);
        public static readonly Size DataNavigator    = new(400, 36);
        public static readonly Size BindingNavigator = new(400, 32);

        // ── Charts ────────────────────────────────────────────────────────
        public static readonly Size Chart            = new(400, 300);

        // ── Filters / Query ──────────────────────────────────────────────
        public static readonly Size FilterRow        = new(300, 32);
        public static readonly Size FilterBar        = new(400, 32);
        public static readonly Size QuickFilterBar   = new(400, 32);
        public static readonly Size QueryAndFilter   = new(400, 36);

        // ── Navigation ──────────────────────────────────────────────────
        public static readonly Size NavBar           = new(800, 56);
        public static readonly Size WebHeaderAppBar  = new(800, 56);
        public static readonly Size Breadcrumb       = new(300, 36);
        public static readonly Size MenuBar          = new(600, 32);
        public static readonly Size Sidebar          = new(240, 400);
        public static readonly Size SideMenu         = new(240, 400);
        public static readonly Size AccordionItem    = new(240, 36);
        public static readonly Size Stepper          = new(320, 32);
        public static readonly Size StepperBreadcrumb = new(300, 36);

        // ── Notifications / Feedback ─────────────────────────────────────
        public static readonly Size Notification     = new(320, 80);
        public static readonly Size NotificationGroup = new(320, 200);
        public static readonly Size NotificationHistory = new(400, 300);
        public static readonly Size Badge            = new(24, 24);

        // ── Date / Time ───────────────────────────────────────────────────
        public static readonly Size Calendar         = new(280, 320);

        // ── Progress / Numeric ────────────────────────────────────────────
        public static readonly Size ProgressBar      = new(200, 16);
        public static readonly Size DualPercentage   = new(200, 60);

        // ── Misc ──────────────────────────────────────────────────────────
        public static readonly Size Marquee          = new(400, 32);
        public static readonly Size Shape            = new(100, 100);
        public static readonly Size Image            = new(100, 100);
        public static readonly Size Login            = new(400, 500);
        public static readonly Size Dock             = new(200, 200);
        public static readonly Size DockSplitter     = new(8, 100);

        // ── Font Defaults ──────────────────────────────────────────────────
        public const float TitleFontSize    = 16f;
        public const float SubtitleFontSize  = 12f;
        public const float BodyFontSize      = 9f;

        // ── DPI-aware helpers ──────────────────────────────────────────────
        public static Size ScaleSize(this Size size, Control control) => DpiScalingHelper.ScaleSize(size, control);
        public static int ScaleValue(this int value, Control control) => DpiScalingHelper.ScaleValue(value, control);
        public static Padding ScalePadding(this Padding padding, Control control)
        {
            return new Padding(
                DpiScalingHelper.ScaleValue(padding.Left, control),
                DpiScalingHelper.ScaleValue(padding.Top, control),
                DpiScalingHelper.ScaleValue(padding.Right, control),
                DpiScalingHelper.ScaleValue(padding.Bottom, control));
        }
    }
}

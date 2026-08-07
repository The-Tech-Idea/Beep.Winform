using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// A single command inside a <see cref="BeepRibbonGroup"/> column grid.
    /// </summary>
    /// <remarks>
    /// A ribbon command is a <see cref="BeepButton"/> — icon via <see cref="BeepButton.ImagePath"/>,
    /// label via <see cref="Control.Text"/> — because that is the control that already renders and
    /// themes an SVG and gives the command a focus rectangle, a tab stop and an accessible role.
    /// The two things a plain <see cref="BeepButton"/> does not have are added here: the large/small
    /// geometry that separates a whole-column command from a one-row command, and the chevron that
    /// tells the user a command opens a menu.
    ///
    /// This replaces <c>ToolStripButton</c> and <c>ToolStripDropDownButton</c> in a group. Those two
    /// could only be laid out by a <c>ToolStrip</c>, which is a toolbar: one horizontal flow with an
    /// overflow chevron. Nothing about that arrangement can produce columns of three rows.
    /// </remarks>
    [ToolboxItem(false)]
    [DesignerCategory("Code")]
    public class BeepRibbonCommandButton : BeepButton
    {
        private RibbonItemSize _itemSize = RibbonItemSize.Small;
        private bool _showDropDownArrow;
        private ToolStripDropDown? _dropDownMenu;

        public BeepRibbonCommandButton()
        {
            IsChild = true;
            UseThemeFont = false;
            IsRounded = true;
            ApplyItemGeometry();
        }

        /// <summary>Whether this command owns a whole column or a single row of one.</summary>
        [Browsable(false)]
        public RibbonItemSize ItemSize
        {
            get => _itemSize;
            set
            {
                if (_itemSize == value) return;
                _itemSize = value;
                ApplyItemGeometry();
            }
        }

        /// <summary>Draws the chevron that says this command opens a menu.</summary>
        [Browsable(false)]
        public bool ShowDropDownArrow
        {
            get => _showDropDownArrow;
            set
            {
                if (_showDropDownArrow == value) return;
                _showDropDownArrow = value;
                ApplyItemGeometry();
            }
        }

        /// <summary>
        /// The menu this command opens, or null for a plain command. Owned by the button: a group
        /// rebuild disposes its items, and an orphaned <see cref="ToolStripDropDown"/> keeps its
        /// handle and its item images alive.
        /// </summary>
        [Browsable(false)]
        public ToolStripDropDown? DropDownMenu
        {
            get => _dropDownMenu;
            set => _dropDownMenu = value;
        }

        /// <summary>Colour of the drop-down chevron. Resolved from the ribbon theme by the group.</summary>
        [Browsable(false)]
        public Color ArrowColor { get; set; } = Color.Empty;

        /// <summary>True while this command's menu is on screen.</summary>
        [Browsable(false)]
        public bool IsDropDownOpen => _dropDownMenu is { Visible: true };

        public void ShowDropDown()
        {
            if (_dropDownMenu == null || _dropDownMenu.Items.Count == 0) return;
            if (_dropDownMenu.Visible)
            {
                _dropDownMenu.Close();
                return;
            }

            _dropDownMenu.Show(this, new Point(0, Height));
        }

        public void CloseDropDown()
        {
            if (_dropDownMenu is { Visible: true }) _dropDownMenu.Close();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            // A command with children opens its menu instead of invoking. The ribbon deliberately does
            // not wire an invoke handler on these, so a click has exactly one meaning.
            if (_dropDownMenu != null) ShowDropDown();
        }

        /// <summary>
        /// Geometry for the two ribbon sizes. This is what <c>TextImageRelation</c> alone used to
        /// stand in for on a ToolStripButton — where "large" meant nothing more than "label under the
        /// icon", at whatever size the toolbar happened to give the item.
        /// </summary>
        private void ApplyItemGeometry()
        {
            if (_itemSize == RibbonItemSize.Large)
            {
                TextImageRelation = TextImageRelation.ImageAboveText;
                ImageAlign = ContentAlignment.TopCenter;
                TextAlign = ContentAlignment.BottomCenter;

                // BeepButton derives its inner inset from Padding.Horizontal / Padding.Vertical, and
                // falls back to the (much larger) per-style padding when either is zero. The extra
                // vertical unit when a chevron is shown is what keeps the label off the chevron.
                Padding = _showDropDownArrow ? new Padding(3, 3, 3, 3) : new Padding(3, 2, 3, 2);
            }
            else
            {
                TextImageRelation = TextImageRelation.ImageBeforeText;
                ImageAlign = ContentAlignment.MiddleLeft;
                TextAlign = ContentAlignment.MiddleLeft;
                Padding = new Padding(3, 1, 3, 1);
            }

            Invalidate();
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            if (!_showDropDownArrow) return;

            Color color = Enabled
                ? (ArrowColor != Color.Empty ? ArrowColor : ForeColor)
                : DisabledForeColor;

            int cx;
            int cy;
            if (_itemSize == RibbonItemSize.Large)
            {
                cx = Width / 2;
                cy = Height - 6;
            }
            else
            {
                cx = Width - 8;
                cy = Height / 2;
            }

            var saved = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var pen = new Pen(color, 1.4f))
            {
                g.DrawLines(pen, new[]
                {
                    new Point(cx - 3, cy - 2),
                    new Point(cx, cy + 1),
                    new Point(cx + 3, cy - 2)
                });
            }
            g.SmoothingMode = saved;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _dropDownMenu != null)
            {
                _dropDownMenu.Dispose();
                _dropDownMenu = null;
            }

            base.Dispose(disposing);
        }
    }
}

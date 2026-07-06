// BeepMenuBar.cs
// Class header + constructor + DefaultSize only.
//
// The control was split into nine focused partials during Phase 02 of
// the Menus revise/enhance program:
//   â€¢ BeepMenuBar.cs              â† this file (shell)
//   â€¢ BeepMenuBar.Properties.cs   â† public properties + shared fields
//   â€¢ BeepMenuBar.Layout.cs       â† hit areas + rect math + GetPreferredSize
//   â€¢ BeepMenuBar.Drawing.cs      â† DrawContent + per-item paint pipeline
//   â€¢ BeepMenuBar.Input.cs        â† OnMouse* + HandleMenuItemClick
//   â€¢ BeepMenuBar.Popup.cs        â† cool-down + toggle + ShowMenuItemPopup
//                                   (also subscribes to ContextMenuManager.MenuDismissed)
//   â€¢ BeepMenuBar.Theming.cs      â† ApplyTheme
//   â€¢ BeepMenuBar.Lifecycle.cs    â† resize / parent / SafeInvoke / Dispose
//   â€¢ BeepMenuBar.Utility.cs      â† LoadSampleMenuItems + RunMethodFromGlobalFunctions
//
// See .plans/Menus-Phase-02-PartialClassSplit.md.
// â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Menu Bar")]
    [Category("Beep Controls")]
    [Description("A menu bar control that displays a list of items.")]
    public partial class BeepMenuBar : BaseControl
    {
        public BeepMenuBar() : base()
        {
            if (items == null)
            {
                items = new BindingList<Models.SimpleItem>();
            }

            if (Width <= 0 || Height <= 0)
            {
                Width = ScaleUi(200);
                int verticalBuffer = ScaleUi(12);
                base.Height = ScaledMenuItemHeight + verticalBuffer;
            }

            IsTransparentBackground = true;
            ApplyThemeToChilds      = true;
            BoundProperty           = "SelectedMenuItem";
            IsFrameless             = true;
            IsRounded               = false;
            IsChild                 = false;

            // The menubar itself never participates in style chrome.
            // Each item drives its own chrome through DrawWithBeepStyling.
            UseThemeFont               = false;
            IsRoundedAffectedByTheme   = false;
            IsBorderAffectedByTheme    = false;
            IsShadowAffectedByTheme    = false;
            EnableSplashEffect         = false;
            EnableRippleEffect         = false;
            CanBeFocused               = false;
            CanBeSelected              = false;
            CanBePressed               = false;
            CanBeHovered               = false;

            // One-time font-driven height calculation, then lock so a
            // later FormStyle/theme swap cannot resize the bar mid-flight.
            UpdateMenuItemHeightForFont();
            _menuItemHeightLocked = true;

            RefreshHitAreas();
            SetStyle(System.Windows.Forms.ControlStyles.SupportsTransparentBackColor, true);
        }

        protected override Size DefaultSize
        {
            get
            {
                int verticalBuffer = ScaleUi(12);
                return new Size(ScaleUi(200), ScaledMenuItemHeight + verticalBuffer);
            }
        }
    }
}

using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    /// <summary>
    /// Multi-select chips + checkbox list — selected items shown as removable
    /// chips at the top with a dashed separator, checkbox rows below, and
    /// Select All / Clear All footer. No search box.
    /// Reference: Category picker with chips area + checkbox list.
    /// </summary>
    internal sealed class MultiChipCompactPopupHostForm : ComboBoxPopupHostForm
    {
        protected override ComboBoxPopupHostProfile CreateProfile(System.Windows.Forms.Control owner, ComboBoxPopupModel model)
        {
            return ComboBoxPopupHostProfile.MultiChipCompact();
        }

        protected override IPopupContentPanel CreateContentPanel(ComboBoxPopupHostProfile profile, ComboBoxThemeTokens tokens)
        {
            // Chip header content: chips area + dashed separator + checkbox list + footer
            var content = new ChipHeaderPopupContent();
            content.ApplyProfile(profile);
            content.ApplyThemeTokens(tokens);
            return content;
        }

        protected override int CalculatePopupHeight(ComboBoxPopupModel model, ComboBoxPopupHostProfile profile)
        {
            int h = 0;

            // Chip area (dynamic based on selection count)
            int checkedCount = model.AllRows?.Count(r => r.IsChecked) ?? 0;
            if (checkedCount > 0)
            {
                h += Math.Min(42 + (checkedCount > 4 ? 30 : 0), profile.ChipAreaMaxHeight);
                h += 14; // dashed separator
            }

            // Row list
            h += (model.FilteredRows?.Count ?? 1) * profile.BaseRowHeight;

            // Footer
            if (model.ShowFooter || profile.ForceFooterVisible)
                h += profile.FooterHeight;

            return Math.Min(h, profile.MaxHeight);
        }

        protected override ComboBoxThemeTokens ResolveThemeTokens(System.Windows.Forms.Control owner, ComboBoxPopupHostProfile profile, ComboBoxPopupModel model)
        {
            var baseTokens = base.ResolveThemeTokens(owner, profile, model);
            var chipsAccent = Blend(baseTokens.SelectedBackColor, baseTokens.FocusBorderColor, 0.25f);
            return DeriveTokens(
                baseTokens,
                popupBack: Blend(baseTokens.PopupBackColor, Color.White, 0.04f),
                popupBorder: WithAlpha(baseTokens.PopupBorderColor, 185),
                rowHover: Blend(baseTokens.PopupRowHoverColor, chipsAccent, 0.15f),
                rowSelected: Blend(baseTokens.PopupRowSelectedColor, chipsAccent, 0.33f),
                rowFocus: Blend(baseTokens.PopupRowFocusColor, chipsAccent, 0.20f),
                focusBorder: chipsAccent);
        }
    }
}

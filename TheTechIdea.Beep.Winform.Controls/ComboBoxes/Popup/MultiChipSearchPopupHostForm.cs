using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    /// <summary>
    /// Multi-select with search + chips + checkbox list. Search box at top,
    /// selected items as chips below, dashed separator, filtered checkbox rows,
    /// Apply/Cancel footer. Reference: category search with "Pla" filtering.
    /// </summary>
    internal sealed class MultiChipSearchPopupHostForm : ComboBoxPopupHostForm
    {
        protected override ComboBoxPopupHostProfile CreateProfile(System.Windows.Forms.Control owner, ComboBoxPopupModel model)
        {
            return ComboBoxPopupHostProfile.MultiChipSearch();
        }

        protected override IPopupContentPanel CreateContentPanel(ComboBoxPopupHostProfile profile, ComboBoxThemeTokens tokens)
        {
            // Chip header content with search enabled — search + chips + separator + checkbox list
            var content = new ChipHeaderPopupContent();
            content.ApplyProfile(profile);
            content.ApplyThemeTokens(tokens);
            return content;
        }

        protected override int CalculatePopupHeight(ComboBoxPopupModel model, ComboBoxPopupHostProfile profile)
        {
            int h = profile.SearchBoxHeight; // search always visible

            // Chip area
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
            var accent = Blend(baseTokens.FocusBorderColor, Color.FromArgb(66, 133, 244), 0.30f);
            return DeriveTokens(
                baseTokens,
                popupBack: Blend(baseTokens.PopupBackColor, Color.White, 0.05f),
                popupBorder: WithAlpha(accent, 170),
                rowHover: Blend(baseTokens.PopupRowHoverColor, accent, 0.14f),
                rowSelected: Blend(baseTokens.PopupRowSelectedColor, accent, 0.25f),
                rowFocus: Blend(baseTokens.PopupRowFocusColor, accent, 0.20f),
                focusBorder: accent,
                openBorder: accent);
        }
    }
}

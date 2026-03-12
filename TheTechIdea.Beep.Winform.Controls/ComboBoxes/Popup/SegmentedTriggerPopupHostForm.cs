using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    /// <summary>
    /// Grouped/segmented popup — section headers with accent, row separators
    /// between groups, and a left-aligned footer. For split-trigger combos with
    /// categorized content. Reference: multi-section app store dropdown with
    /// "Top categories" and "Genres" sections.
    /// </summary>
    internal sealed class SegmentedTriggerPopupHostForm : ComboBoxPopupHostForm
    {
        protected override ComboBoxPopupHostProfile CreateProfile(System.Windows.Forms.Control owner, ComboBoxPopupModel model)
        {
            return ComboBoxPopupHostProfile.SegmentedTrigger();
        }

        protected override IPopupContentPanel CreateContentPanel(ComboBoxPopupHostProfile profile, ComboBoxThemeTokens tokens)
        {
            // Grouped sections: pill-grid for compact groups, list for detail groups,
            // section headers with accent bars, separator lines between groups
            var content = new GroupedSectionsPopupContent();
            content.ApplyProfile(profile);
            content.ApplyThemeTokens(tokens);
            return content;
        }

        protected override int CalculatePopupHeight(ComboBoxPopupModel model, ComboBoxPopupHostProfile profile)
        {
            int count = model.FilteredRows?.Count ?? 1;
            int h = count * profile.BaseRowHeight + 32; // extra for section headers
            if (model.ShowSearchBox || profile.ForceSearchVisible) h += profile.SearchBoxHeight;
            if (model.ShowFooter || profile.ForceFooterVisible) h += profile.FooterHeight;
            return System.Math.Min(h, profile.MaxHeight);
        }

        protected override ComboBoxThemeTokens ResolveThemeTokens(System.Windows.Forms.Control owner, ComboBoxPopupHostProfile profile, ComboBoxPopupModel model)
        {
            var baseTokens = base.ResolveThemeTokens(owner, profile, model);
            var accent = Blend(baseTokens.FocusBorderColor, baseTokens.OpenBorderColor, 0.40f);
            return DeriveTokens(
                baseTokens,
                popupBack: Blend(baseTokens.PopupBackColor, Color.White, 0.05f),
                popupBorder: WithAlpha(accent, 180),
                rowHover: Blend(baseTokens.PopupRowHoverColor, accent, 0.18f),
                rowSelected: Blend(baseTokens.PopupRowSelectedColor, accent, 0.28f),
                rowFocus: Blend(baseTokens.PopupRowFocusColor, accent, 0.22f),
                groupBack: Blend(baseTokens.PopupGroupHeaderBack, accent, 0.20f),
                groupFore: Blend(baseTokens.PopupGroupHeaderFore, baseTokens.ForeColor, 0.30f),
                separator: WithAlpha(accent, 120),
                focusBorder: accent,
                openBorder: accent);
        }
    }
}

using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    /// <summary>
    /// Soft-filled card-style dropdown — rows rendered as indented cards with
    /// subtle borders, warm background, and gentle hover feedback.
    /// Reference: filled dropdown variants, design system card rows.
    /// </summary>
    internal sealed class FilledSoftPopupHostForm : ComboBoxPopupHostForm
    {
        protected override ComboBoxPopupHostProfile CreateProfile(System.Windows.Forms.Control owner, ComboBoxPopupModel model)
        {
            return ComboBoxPopupHostProfile.FilledSoft();
        }

        protected override IPopupContentPanel CreateContentPanel(ComboBoxPopupHostProfile profile, ComboBoxThemeTokens tokens)
        {
            // Card-style rows — each item is a rounded card with border and hover shadow
            var content = new CardRowPopupContent();
            content.ApplyProfile(profile);
            content.ApplyThemeTokens(tokens);
            return content;
        }

        protected override int CalculatePopupHeight(ComboBoxPopupModel model, ComboBoxPopupHostProfile profile)
        {
            // Card rows are taller due to inset + spacing
            int count = model.FilteredRows?.Count ?? 1;
            int cardRowH = profile.BaseRowHeight + 12; // card height includes margin+inset
            int h = count * cardRowH + 16;
            if (model.ShowSearchBox || profile.ForceSearchVisible) h += profile.SearchBoxHeight;
            return System.Math.Min(h, profile.MaxHeight);
        }

        protected override ComboBoxThemeTokens ResolveThemeTokens(System.Windows.Forms.Control owner, ComboBoxPopupHostProfile profile, ComboBoxPopupModel model)
        {
            var baseTokens = base.ResolveThemeTokens(owner, profile, model);
            var warmSurface = Blend(baseTokens.PopupBackColor, Color.FromArgb(246, 248, 252), 0.30f);
            return DeriveTokens(
                baseTokens,
                popupBack: warmSurface,
                popupBorder: WithAlpha(baseTokens.PopupBorderColor, 120),
                rowHover: Blend(baseTokens.PopupRowHoverColor, Color.FromArgb(234, 240, 248), 0.45f),
                rowSelected: Blend(baseTokens.PopupRowSelectedColor, baseTokens.SelectionHighlight, 0.40f),
                rowFocus: Blend(baseTokens.PopupRowFocusColor, baseTokens.FocusBorderColor, 0.15f),
                groupBack: Blend(baseTokens.PopupGroupHeaderBack, warmSurface, 0.55f),
                separator: WithAlpha(baseTokens.PopupSeparatorColor, 160));
        }
    }
}

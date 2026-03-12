using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    /// <summary>
    /// Search-first dropdown — search box at top, filtered list below with
    /// match highlighting. Reference: Account selector with "Acco" typed.
    /// </summary>
    internal sealed class OutlineSearchablePopupHostForm : ComboBoxPopupHostForm
    {
        protected override ComboBoxPopupHostProfile CreateProfile(System.Windows.Forms.Control owner, ComboBoxPopupModel model)
        {
            return ComboBoxPopupHostProfile.OutlineSearchable();
        }

        protected override IPopupContentPanel CreateContentPanel(ComboBoxPopupHostProfile profile, ComboBoxThemeTokens tokens)
        {
            // Standard list with search forced visible — auto-focus search on open
            var content = new ComboBoxPopupContent();
            content.ApplyProfile(profile);
            content.ApplyThemeTokens(tokens);
            return content;
        }

        protected override ComboBoxThemeTokens ResolveThemeTokens(System.Windows.Forms.Control owner, ComboBoxPopupHostProfile profile, ComboBoxPopupModel model)
        {
            var baseTokens = base.ResolveThemeTokens(owner, profile, model);
            var searchBlue = Blend(baseTokens.FocusBorderColor, Color.FromArgb(66, 133, 244), 0.45f);
            return DeriveTokens(
                baseTokens,
                popupBack: Blend(baseTokens.PopupBackColor, Color.White, 0.06f),
                popupBorder: WithAlpha(searchBlue, 170),
                rowHover: Blend(baseTokens.PopupRowHoverColor, searchBlue, 0.16f),
                rowSelected: Blend(baseTokens.PopupRowSelectedColor, searchBlue, 0.22f),
                rowFocus: Blend(baseTokens.PopupRowFocusColor, searchBlue, 0.25f),
                focusBorder: searchBlue,
                openBorder: searchBlue);
        }
    }
}

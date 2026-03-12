using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    /// <summary>
    /// Simple select dropdown — clean vertical list, no search, checkmark for selected.
    /// Reference: basic "Select a country" dropdown pattern.
    /// </summary>
    internal sealed class OutlineDefaultPopupHostForm : ComboBoxPopupHostForm
    {
        protected override ComboBoxPopupHostProfile CreateProfile(System.Windows.Forms.Control owner, ComboBoxPopupModel model)
        {
            return ComboBoxPopupHostProfile.OutlineDefault();
        }

        protected override IPopupContentPanel CreateContentPanel(ComboBoxPopupHostProfile profile, ComboBoxThemeTokens tokens)
        {
            // Standard vertical list — no search, no footer, just a clean dropdown
            var content = new ComboBoxPopupContent();
            content.ApplyProfile(profile);
            content.ApplyThemeTokens(tokens);
            return content;
        }

        protected override ComboBoxThemeTokens ResolveThemeTokens(System.Windows.Forms.Control owner, ComboBoxPopupHostProfile profile, ComboBoxPopupModel model)
        {
            var baseTokens = base.ResolveThemeTokens(owner, profile, model);
            return DeriveTokens(
                baseTokens,
                popupBack: Blend(baseTokens.PopupBackColor, Color.White, 0.08f),
                popupBorder: WithAlpha(baseTokens.BorderColor, 200),
                rowHover: Blend(baseTokens.PopupRowHoverColor, baseTokens.SelectionHighlight, 0.25f),
                rowSelected: Blend(baseTokens.PopupRowSelectedColor, baseTokens.SelectionHighlight, 0.35f),
                rowFocus: Blend(baseTokens.PopupRowFocusColor, baseTokens.FocusBorderColor, 0.18f));
        }
    }
}

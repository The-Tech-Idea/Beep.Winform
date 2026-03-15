using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    /// <summary>
    /// Command-menu popup host. Uses the standard popup content panel and
    /// relies on row metadata rendering for shortcut text.
    /// </summary>
    internal sealed class CommandMenuPopupHostForm : ComboBoxPopupHostForm
    {
        protected override ComboBoxPopupHostProfile CreateProfile(System.Windows.Forms.Control owner, ComboBoxPopupModel model)
        {
            return ComboBoxPopupHostProfile.CommandMenu();
        }

        protected override IPopupContentPanel CreateContentPanel(ComboBoxPopupHostProfile profile, ComboBoxThemeTokens tokens)
        {
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
                popupBack: Blend(baseTokens.PopupBackColor, Color.White, 0.03f),
                popupBorder: WithAlpha(baseTokens.PopupBorderColor, 210),
                rowHover: Blend(baseTokens.PopupRowHoverColor, baseTokens.SelectionHighlight, 0.18f),
                rowSelected: Blend(baseTokens.PopupRowSelectedColor, baseTokens.SelectionHighlight, 0.20f),
                rowFocus: Blend(baseTokens.PopupRowFocusColor, baseTokens.FocusBorderColor, 0.15f));
        }
    }
}

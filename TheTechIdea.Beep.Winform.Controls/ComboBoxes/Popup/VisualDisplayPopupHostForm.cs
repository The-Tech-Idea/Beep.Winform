using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    /// <summary>
    /// Visual display popup host that favors avatar/icon oriented rows.
    /// </summary>
    internal sealed class VisualDisplayPopupHostForm : ComboBoxPopupHostForm
    {
        protected override ComboBoxPopupHostProfile CreateProfile(System.Windows.Forms.Control owner, ComboBoxPopupModel model)
        {
            return ComboBoxPopupHostProfile.VisualDisplay();
        }

        protected override IPopupContentPanel CreateContentPanel(ComboBoxPopupHostProfile profile, ComboBoxThemeTokens tokens)
        {
            var content = new DenseAvatarPopupContent();
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
                popupBorder: WithAlpha(baseTokens.PopupBorderColor, 180),
                rowHover: Blend(baseTokens.PopupRowHoverColor, baseTokens.SelectionHighlight, 0.20f),
                rowSelected: Blend(baseTokens.PopupRowSelectedColor, baseTokens.SelectionHighlight, 0.20f),
                rowFocus: Blend(baseTokens.PopupRowFocusColor, baseTokens.FocusBorderColor, 0.18f));
        }
    }
}

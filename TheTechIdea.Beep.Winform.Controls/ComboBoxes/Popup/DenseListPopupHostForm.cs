using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    /// <summary>
    /// Dense compact list — tight row spacing (28px), circular avatar images,
    /// search at top, minimal separators. For people pickers, contact lists.
    /// Reference: "Add guests" picker with avatar + name + status dot.
    /// </summary>
    internal sealed class DenseListPopupHostForm : ComboBoxPopupHostForm
    {
        protected override ComboBoxPopupHostProfile CreateProfile(System.Windows.Forms.Control owner, ComboBoxPopupModel model)
        {
            return ComboBoxPopupHostProfile.DenseList();
        }

        protected override IPopupContentPanel CreateContentPanel(ComboBoxPopupHostProfile profile, ComboBoxThemeTokens tokens)
        {
            // Dense avatar list — circular photo, name, colored status indicator dot
            var content = new DenseAvatarPopupContent();
            content.ApplyProfile(profile);
            content.ApplyThemeTokens(tokens);
            return content;
        }

        protected override int CalculatePopupHeight(ComboBoxPopupModel model, ComboBoxPopupHostProfile profile)
        {
            // Dense layout — use DenseRowHeight for tighter packing
            int count = model.FilteredRows?.Count ?? 1;
            int rowH = profile.DenseRowHeight > 0 ? profile.DenseRowHeight : profile.BaseRowHeight;
            int h = count * rowH;
            if (model.ShowSearchBox || profile.ForceSearchVisible)
                h += profile.SearchBoxHeight;
            return System.Math.Min(h, profile.MaxHeight);
        }

        protected override ComboBoxThemeTokens ResolveThemeTokens(System.Windows.Forms.Control owner, ComboBoxPopupHostProfile profile, ComboBoxPopupModel model)
        {
            var baseTokens = base.ResolveThemeTokens(owner, profile, model);
            return DeriveTokens(
                baseTokens,
                popupBack: Blend(baseTokens.PopupBackColor, Color.White, 0.03f),
                popupBorder: WithAlpha(baseTokens.PopupBorderColor, 160),
                rowHover: Blend(baseTokens.PopupRowHoverColor, Color.FromArgb(235, 240, 250), 0.50f),
                rowSelected: Blend(baseTokens.PopupRowSelectedColor, baseTokens.FocusBorderColor, 0.18f),
                rowFocus: Blend(baseTokens.PopupRowFocusColor, baseTokens.FocusBorderColor, 0.22f),
                separator: WithAlpha(baseTokens.PopupSeparatorColor, 100));
        }
    }
}

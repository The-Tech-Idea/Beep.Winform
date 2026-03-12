using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    /// <summary>
    /// Minimal borderless popup — clean list with no visible border, shadow only,
    /// no checkmarks, no separators. Ultra-clean aesthetic for simple menus.
    /// Reference: floating "Select your speciality" panel with shadow.
    /// </summary>
    internal sealed class MinimalBorderlessPopupHostForm : ComboBoxPopupHostForm
    {
        protected override ComboBoxPopupHostProfile CreateProfile(System.Windows.Forms.Control owner, ComboBoxPopupModel model)
        {
            return ComboBoxPopupHostProfile.MinimalBorderless();
        }

        protected override IPopupContentPanel CreateContentPanel(ComboBoxPopupHostProfile profile, ComboBoxThemeTokens tokens)
        {
            // Ultra-minimal text-only content — no icons, no checkmarks, no borders,
            // rounded hover rects, generous padding, clean sans-serif
            var content = new MinimalCleanPopupContent();
            content.ApplyProfile(profile);
            content.ApplyThemeTokens(tokens);
            return content;
        }

        protected override void ConfigurePopupForm(BeepPopupForm form, ComboBoxPopupHostProfile profile, ComboBoxThemeTokens tokens)
        {
            // Minimal borderless relies on form shadow for visual separation.
            // BeepPopupForm uses BaseControl's border — set it to None so no border paints.
          //  form.ShowBorder = false;
        }

        protected override ComboBoxThemeTokens ResolveThemeTokens(System.Windows.Forms.Control owner, ComboBoxPopupHostProfile profile, ComboBoxPopupModel model)
        {
            var baseTokens = base.ResolveThemeTokens(owner, profile, model);
            return DeriveTokens(
                baseTokens,
                popupBack: Color.White,
                popupBorder: Color.FromArgb(30, baseTokens.PopupBorderColor), // Nearly invisible
                rowHover: Color.FromArgb(245, 247, 250),
                rowSelected: Blend(Color.White, baseTokens.FocusBorderColor, 0.08f),
                rowFocus: Blend(Color.White, baseTokens.FocusBorderColor, 0.12f),
                separator: Color.Transparent);
        }
    }
}

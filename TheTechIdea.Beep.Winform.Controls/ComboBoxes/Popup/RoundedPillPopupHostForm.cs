using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    /// <summary>
    /// Pill/tag grid popup — items displayed as clickable pill buttons in a
    /// wrap-flow layout instead of a vertical list. Search at bottom.
    /// Reference: Tags picker with "Node ×  React ×  Next ×" and grid of pills below.
    /// </summary>
    internal sealed class RoundedPillPopupHostForm : ComboBoxPopupHostForm
    {
        private PillGridPopupContent _pillContent;

        protected override ComboBoxPopupHostProfile CreateProfile(System.Windows.Forms.Control owner, ComboBoxPopupModel model)
        {
            return ComboBoxPopupHostProfile.RoundedPill();
        }

        protected override IPopupContentPanel CreateContentPanel(ComboBoxPopupHostProfile profile, ComboBoxThemeTokens tokens)
        {
            // Pill grid layout — NOT a vertical list
            _pillContent = new PillGridPopupContent();
            _pillContent.ApplyProfile(profile);
            _pillContent.ApplyThemeTokens(tokens);
            return _pillContent;
        }

        protected override int CalculatePopupHeight(ComboBoxPopupModel model, ComboBoxPopupHostProfile profile)
        {
            // Pill grid needs different height calculation based on flow wrapping
            int rowCount = model.FilteredRows?.Count ?? 0;
            int pillH = profile.PillHeight > 0 ? profile.PillHeight : 36;
            int spacing = profile.PillSpacing > 0 ? profile.PillSpacing : 4;

            // Estimate: ~3 pills per row, calculate number of rows
            int pillsPerRow = Math.Max(1, (profile.MinWidth - 20) / (80 + spacing * 2));
            int gridRows = Math.Max(1, (int)Math.Ceiling((double)rowCount / pillsPerRow));
            int gridHeight = gridRows * (pillH + spacing * 2) + 20;

            int searchHeight = (model.ShowSearchBox || profile.ForceSearchVisible) ? profile.SearchBoxHeight : 0;
            return Math.Min(gridHeight + searchHeight, profile.MaxHeight);
        }

        protected override ComboBoxThemeTokens ResolveThemeTokens(System.Windows.Forms.Control owner, ComboBoxPopupHostProfile profile, ComboBoxPopupModel model)
        {
            var baseTokens = base.ResolveThemeTokens(owner, profile, model);
            return DeriveTokens(
                baseTokens,
                popupBack: Blend(baseTokens.PopupBackColor, Color.FromArgb(250, 252, 255), 0.35f),
                popupBorder: WithAlpha(baseTokens.PopupBorderColor, 140),
                rowHover: Blend(baseTokens.PopupRowHoverColor, Color.White, 0.35f),
                rowSelected: Blend(baseTokens.PopupRowSelectedColor, baseTokens.FocusBorderColor, 0.12f),
                rowFocus: Blend(baseTokens.PopupRowFocusColor, baseTokens.FocusBorderColor, 0.20f),
                separator: WithAlpha(baseTokens.PopupSeparatorColor, 150));
        }
    }
}

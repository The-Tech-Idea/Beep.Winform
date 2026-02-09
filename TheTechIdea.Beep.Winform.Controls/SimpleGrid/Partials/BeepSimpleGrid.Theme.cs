using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Partial class containing theme application for BeepSimpleGrid
    /// Handles theme application to all grid components and child controls
    /// </summary>
    public partial class BeepSimpleGrid
    {
        #region Theme Application

        /// <summary>
        /// Applies the current theme to all grid components
        /// </summary>
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            // Apply theme to grid base colors
            this.BackColor = _currentTheme.GridBackColor;
            this.ForeColor = _currentTheme.GridForeColor;
            SelectedForeColor = _currentTheme.GridForeColor;
            SelectedBackColor = _currentTheme.GridBackColor;
            HoverBackColor = _currentTheme.GridBackColor;
            HoverForeColor = _currentTheme.GridForeColor;
            FocusBackColor = _currentTheme.GridBackColor;
            FocusForeColor = _currentTheme.GridForeColor;

            Color footerback = _currentTheme.GridHeaderBackColor;

            // Apply theme to title label
            if (titleLabel != null)
            {
                titleLabel.ParentBackColor = _currentTheme.GridHeaderBackColor;
                titleLabel.BackColor = _currentTheme.GridHeaderBackColor;
                titleLabel.IsChild = true;
                titleLabel.TextFont = BeepThemesManager.ToFont(_currentTheme.CardHeaderStyle);
                titleLabel.ForeColor = _currentTheme.GridForeColor;
                titleLabel.Invalidate();
            }

            // Apply theme to record number label
            if (Recordnumberinglabel1 != null)
            {
                Recordnumberinglabel1.TextFont = BeepThemesManager.ToFont(_currentTheme.SmallText);
                Recordnumberinglabel1.ForeColor = _currentTheme.GridHeaderForeColor;
                Recordnumberinglabel1.BackColor = footerback;
                Recordnumberinglabel1.ParentBackColor = footerback;
                Recordnumberinglabel1.HoverBackColor = _currentTheme.GridHeaderHoverBackColor;
                Recordnumberinglabel1.HoverForeColor = _currentTheme.GridHeaderHoverForeColor;
                Recordnumberinglabel1.BorderColor = _currentTheme.GridForeColor;
                Recordnumberinglabel1.DisabledBackColor = _currentTheme.DisabledBackColor;
                Recordnumberinglabel1.DisabledForeColor = _currentTheme.DisabledForeColor;
                Recordnumberinglabel1.SelectedBackColor = _currentTheme.GridHeaderSelectedBackColor;
                Recordnumberinglabel1.SelectedForeColor = _currentTheme.GridHeaderSelectedForeColor;
            }

            // Apply theme to page label
            if (PageLabel != null)
            {
                PageLabel.TextFont = BeepThemesManager.ToFont(_currentTheme.SmallText);
                PageLabel.ForeColor = _currentTheme.GridHeaderForeColor;
                PageLabel.BackColor = footerback;
                PageLabel.ParentBackColor = footerback;
                PageLabel.HoverBackColor = _currentTheme.GridHeaderHoverBackColor;
                PageLabel.HoverForeColor = _currentTheme.GridHeaderHoverForeColor;
                PageLabel.BorderColor = _currentTheme.GridForeColor;
                PageLabel.DisabledBackColor = _currentTheme.DisabledBackColor;
                PageLabel.DisabledForeColor = _currentTheme.DisabledForeColor;
                PageLabel.SelectedBackColor = _currentTheme.GridHeaderSelectedBackColor;
                PageLabel.SelectedForeColor = _currentTheme.GridHeaderSelectedForeColor;

                // Apply theme to navigation buttons
                foreach (BeepButton x in buttons)
                {
                    x.Theme = Theme;
                    x.ForeColor = _currentTheme.GridHeaderForeColor;
                    x.BackColor = footerback;
                    x.ParentBackColor = footerback;
                    x.HoverBackColor = _currentTheme.GridHeaderHoverBackColor;
                    x.HoverForeColor = _currentTheme.GridHeaderHoverForeColor;
                    x.BorderColor = _currentTheme.GridForeColor;
                    x.DisabledBackColor = _currentTheme.DisabledBackColor;
                    x.DisabledForeColor = _currentTheme.DisabledForeColor;
                    x.SelectedBackColor = _currentTheme.GridHeaderSelectedBackColor;
                    x.SelectedForeColor = _currentTheme.GridHeaderSelectedForeColor;
                    x.ApplyThemeToSvg();
                }

                // Apply theme to paging buttons
                foreach (BeepButton x in pagingButtons)
                {
                    x.Theme = Theme;
                    x.ForeColor = _currentTheme.GridHeaderForeColor;
                    x.BackColor = footerback;
                    x.ParentBackColor = footerback;
                    x.HoverBackColor = _currentTheme.GridHeaderHoverBackColor;
                    x.HoverForeColor = _currentTheme.GridHeaderHoverForeColor;
                    x.BorderColor = _currentTheme.GridForeColor;
                    x.DisabledBackColor = _currentTheme.DisabledBackColor;
                    x.DisabledForeColor = _currentTheme.DisabledForeColor;
                    x.SelectedBackColor = _currentTheme.GridHeaderSelectedBackColor;
                    x.SelectedForeColor = _currentTheme.GridHeaderSelectedForeColor;
                    x.ApplyThemeToSvg();
                }

                // Apply theme to select all checkbox
                _selectAllCheckBox.Theme = Theme;
                _selectAllCheckBox.ForeColor = _currentTheme.GridHeaderForeColor;
                _selectAllCheckBox.BackColor = footerback;
                _selectAllCheckBox.ParentBackColor = footerback;
                _selectAllCheckBox.HoverBackColor = _currentTheme.GridHeaderHoverBackColor;
                _selectAllCheckBox.HoverForeColor = _currentTheme.GridHeaderHoverForeColor;
                _selectAllCheckBox.BorderColor = _currentTheme.GridForeColor;
                _selectAllCheckBox.DisabledBackColor = _currentTheme.DisabledBackColor;
                _selectAllCheckBox.DisabledForeColor = _currentTheme.DisabledForeColor;
                _selectAllCheckBox.SelectedBackColor = _currentTheme.GridHeaderSelectedBackColor;
                _selectAllCheckBox.SelectedForeColor = _currentTheme.GridHeaderSelectedForeColor;
            }

            // Apply theme to scrollbars
            if (_verticalScrollBar != null)
            {
                _verticalScrollBar.Theme = Theme;
            }
            if (_horizontalScrollBar != null)
            {
                _horizontalScrollBar.Theme = Theme;
            }

            // Apply theme to cell controls
            foreach (var row in Rows)
            {
                foreach (var cell in row.Cells)
                {
                    if (cell.UIComponent is BeepControl ctrl)
                    {
                        ctrl.Theme = Theme;
                    }
                }
            }

            // Apply theme to filter controls
            filterTextBox.TextFont = BeepThemesManager.ToFont(_currentTheme.SmallText);
            filterTextBox.ForeColor = _currentTheme.GridHeaderForeColor;
            filterTextBox.BackColor = _currentTheme.GridHeaderBackColor;
            filterTextBox.ParentBackColor = _currentTheme.GridHeaderBackColor;
            filterTextBox.HoverBackColor = _currentTheme.GridHeaderHoverBackColor;
            filterTextBox.HoverForeColor = _currentTheme.GridHeaderHoverForeColor;
            filterTextBox.BorderColor = _currentTheme.GridForeColor;
            filterTextBox.DisabledBackColor = _currentTheme.DisabledBackColor;
            filterTextBox.DisabledForeColor = _currentTheme.DisabledForeColor;
            filterTextBox.SelectedBackColor = _currentTheme.GridHeaderSelectedBackColor;
            filterTextBox.SelectedForeColor = _currentTheme.GridHeaderSelectedForeColor;

            filterColumnComboBox.TextFont = BeepThemesManager.ToFont(_currentTheme.SmallText);
            filterColumnComboBox.ForeColor = _currentTheme.GridHeaderForeColor;
            filterColumnComboBox.BackColor = _currentTheme.GridHeaderBackColor;
            filterColumnComboBox.ParentBackColor = _currentTheme.GridHeaderBackColor;
            filterColumnComboBox.HoverBackColor = _currentTheme.GridHeaderHoverBackColor;

            // Reinitialize painting resources with new theme colors
            InitializePaintingResources();
            
            Invalidate();
        }

        #endregion
    }
}

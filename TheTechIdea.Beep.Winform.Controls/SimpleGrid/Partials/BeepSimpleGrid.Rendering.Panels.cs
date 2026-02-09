using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Partial class containing panel rendering functionality for BeepSimpleGrid
    /// Handles drawing of header, footer, filter, and navigation panels
    /// </summary>
    public partial class BeepSimpleGrid
    {
        #region Header Panel Drawing

        /// <summary>
        /// Draws the header panel (title area)
        /// </summary>
        private void DrawHeaderPanel(Graphics g, Rectangle rect)
        {
            using (var borderPen = new Pen(_currentTheme.BorderColor))
            {
                if (ShowHeaderPanelBorder)
                {
                    g.DrawLine(borderPen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
                }
            }

            // Position the title label with proper padding
            int padding = 10;
            int titleX = rect.Left + padding;
            int titleY = rect.Top + (rect.Height - titleLabel.Height) / 2;
            titleLabel.Location = new Point(titleX, titleY);

            // Draw title label
            titleLabel.TextImageRelation = TextImageRelation.ImageBeforeText;

            if (Entity != null)
            {
                if (Entity?.EntityName != null)
                {
                    titleLabel.Text = Entity.EntityName;
                }
            }
            else
            {
                titleLabel.Text = TitleText;
            }

            titleLabel.Draw(g, rect);

            // Calculate right margin and control dimensions
            int rightMargin = padding;

            // Configure and position the combo box at the right side of the panel
            int comboBoxWidth = 120;
            int filterTextBoxWidth = 150;
            int controlHeight = Math.Min(24, rect.Height);

            // Position ComboBox on the right side with proper margin
            int comboBoxX = rect.Right - comboBoxWidth - rightMargin;
            int comboBoxY = rect.Top + (rect.Height - controlHeight) / 2; // Center vertically
            filterColumnComboBox.Location = new Point(comboBoxX, comboBoxY);
            filterColumnComboBox.Size = new Size(comboBoxWidth, controlHeight);

            // Position TextBox to the left of ComboBox with proper spacing
            int filterTextBoxX = comboBoxX - filterTextBoxWidth - rightMargin;
            int filterTextBoxY = comboBoxY; // Same Y as the combo box
            filterTextBox.Location = new Point(filterTextBoxX, filterTextBoxY);
            filterTextBox.Size = new Size(filterTextBoxWidth, controlHeight);

            // Position the percentage label (if needed)
            int percentageX = filterTextBoxX - percentageLabel.Width - padding;
            int percentageY = rect.Top + (rect.Height - percentageLabel.Height) / 2;
            percentageLabel.Location = new Point(percentageX, percentageY);
            percentageLabel.Visible = false;

            // Make sure title label is correctly positioned
            titleLabel.Location = new Point(titleX, titleY);
        }

        #endregion

        #region Filter Panel Drawing

        /// <summary>
        /// Draws the filter panel
        /// </summary>
        private void DrawFilterPanel(Graphics g, Rectangle filterPanelRect)
        {
            int padding = 10; // Consistent padding from the edge
            int spacing = 5;  // Spacing between controls
            int controlHeight = filterPanelRect.Height - (2 * filtercontrolsheight);

            // Position controls from the right edge of the filter panel
            if (filterColumnComboBox != null && filterTextBox != null)
            {
                // Position the ComboBox first, aligned to the far right
                int comboBoxWidth = 200;
                int comboBoxX = filterPanelRect.Right - comboBoxWidth - padding;
                int controlY = filterPanelRect.Y + filtercontrolsheight;

                filterColumnComboBox.Location = new Point(comboBoxX, controlY);
                filterColumnComboBox.Size = new Size(comboBoxWidth, controlHeight);

                // Position the TextBox to the left of the ComboBox
                int filterTextBoxWidth = 200;
                int filterTextBoxX = comboBoxX - filterTextBoxWidth - spacing;

                filterTextBox.Location = new Point(filterTextBoxX, controlY);
                filterTextBox.Size = new Size(filterTextBoxWidth, controlHeight);
            }
        }

        #endregion

        #region Footer Panel Drawing

        /// <summary>
        /// Draws the footer panel
        /// </summary>
        private void DrawFooterRow(Graphics g, Rectangle rect)
        {
            using (var brush = new SolidBrush(_currentTheme.BackColor))
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                g.FillRectangle(brush, rect);
                g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
                g.DrawString("Footer", Font, new SolidBrush(_currentTheme.PrimaryTextColor), rect,
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }

        #endregion

        #region Aggregation Panel Drawing

        /// <summary>
        /// Draws the bottom aggregation row
        /// </summary>
        private void DrawBottomAggregationRow(Graphics g, Rectangle rect)
        {
            using (var brush = new SolidBrush(_currentTheme.BackColor))
            using (var pen = new Pen(_currentTheme.GridLineColor))
            using (var thickPen = new Pen(_currentTheme.GridLineColor, 2)) // Thicker pen for emphasis
            {
                g.FillRectangle(brush, rect);

                // Draw a prominent top line
                g.DrawLine(thickPen, rect.Left, rect.Top, rect.Right, rect.Top);

                if (aggregationRow != null)
                {
                    // Draw scrolling columns, adjusted for horizontal scroll
                    var scrollingColumns = Columns.Where(c => !c.Sticked && c.Visible).ToList();
                    int scrollingXOffset = rect.Left + _stickyWidth - _xOffset; // Start after sticky columns, adjusted by scroll
                    foreach (var scrollingCol in scrollingColumns)
                    {
                        int columnIndex = Columns.IndexOf(scrollingCol);
                        if (columnIndex >= 0 && columnIndex < aggregationRow.Cells.Count)
                        {
                            var cell = aggregationRow.Cells[columnIndex];
                            var cellRect = new Rectangle(scrollingXOffset, rect.Top, scrollingCol.Width, rect.Height);
                            PaintCell(g, cell, cellRect, _currentTheme.GridBackColor);
                            scrollingXOffset += scrollingCol.Width;
                        }
                    }
                    // Draw sticky columns first
                    var stickyColumns = Columns.Where(c => c.Sticked && c.Visible).ToList();
                    int stickyXOffset = rect.Left;
                    foreach (var stickyCol in stickyColumns)
                    {
                        int columnIndex = Columns.IndexOf(stickyCol);
                        if (columnIndex >= 0 && columnIndex < aggregationRow.Cells.Count)
                        {
                            var cell = aggregationRow.Cells[columnIndex];
                            int stickyX = stickyXOffset;
                            var cellRect = new Rectangle(stickyX, rect.Top, stickyCol.Width, rect.Height);
                            PaintCell(g, cell, cellRect, _currentTheme.GridBackColor);
                            stickyXOffset += stickyCol.Width;
                        }
                    }
                }
            }
        }

        #endregion

        #region Column Headers Drawing

        /// <summary>
        /// Draws the column header row
        /// </summary>
        private void PaintColumnHeaders(Graphics g, Rectangle headerRect)
        {
            int xOffset = headerRect.Left;
            var selColumn = Columns.FirstOrDefault(c => c.IsSelectionCheckBox);
            if (_showCheckboxes)
            {
                selColumn.Visible = true;
            }
            else
                selColumn.Visible = false;
            // Ensure _stickyWidth is calculated and capped
            UpdateStickyWidth();
            int stickyWidth = _stickyWidth;
            stickyWidth = Math.Min(stickyWidth, headerRect.Width); // Prevent overflow

            // Draw Border line on Top
            using (Pen borderPen = new Pen(_currentTheme.GridLineColor))
            {
                g.DrawLine(borderPen, headerRect.Left, headerRect.Top, headerRect.Right, headerRect.Top);
            }
            StringFormat centerFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            // Define sticky and scrolling regions
            Rectangle stickyRegion = new Rectangle(headerRect.Left, headerRect.Top, stickyWidth, headerRect.Height);
            Rectangle scrollingRegion = new Rectangle(headerRect.Left + stickyWidth, headerRect.Top,
                                                     headerRect.Width - stickyWidth, headerRect.Height);

            // Draw scrolling column headers first
            using (Region clipRegion = new Region(scrollingRegion))
            {
                g.Clip = clipRegion;
                int scrollingXOffset = headerRect.Left + stickyWidth - _xOffset; // Adjusted for horizontal scroll
                foreach (var col in Columns.Where(c => !c.Sticked && c.Visible))
                {
                    var headerCellRect = new Rectangle(scrollingXOffset, headerRect.Top, col.Width, headerRect.Height);
                    PaintHeaderCell(g, col, headerCellRect, centerFormat);
                    scrollingXOffset += col.Width;
                }
            }

            // Draw sticky column headers last
            using (Region clipRegion = new Region(stickyRegion))
            {
                g.Clip = clipRegion;
                xOffset = headerRect.Left;

                foreach (var col in Columns.Where(c => c.Sticked && c.Visible))
                {
                    var headerCellRect = new Rectangle(xOffset, headerRect.Top, col.Width, headerRect.Height);
                    PaintHeaderCell(g, col, headerCellRect, centerFormat);
                    xOffset += col.Width;
                }
            }

            if (stickyWidth > 0)
            {
                g.ResetClip();
                using (Pen borderPen = new Pen(_currentTheme.GridLineColor))
                {
                    g.DrawLine(borderPen, headerRect.Left + stickyWidth, headerRect.Top, headerRect.Left + stickyWidth, headerRect.Bottom);
                }
            }
        }

        /// <summary>
        /// Paints a single header cell with icons
        /// </summary>
        private void PaintHeaderCell(Graphics g, BeepColumnConfig col, Rectangle cellRect, StringFormat format)
        {
            // Determine header colors
            Color headerBackColor = _currentTheme.GridHeaderBackColor;
            Color headerForeColor = _currentTheme.GridHeaderForeColor;

            if (col.UseCustomColors)
            {
                if (col.HasCustomHeaderBackColor)
                    headerBackColor = col.ColumnHeaderBackColor;
                if (col.HasCustomHeaderForeColor)
                    headerForeColor = col.ColumnHeaderForeColor;
            }

            using (Brush bgBrush = new SolidBrush(headerBackColor))
            using (Brush textBrush = new SolidBrush(headerForeColor))
            {
                g.FillRectangle(bgBrush, cellRect);

                if (!col.IsSelectionCheckBox)
                {
                    // Calculate text area (reserve space for icons in corners)
                    Rectangle textRect = cellRect;
                    int iconSpace = 0;
                    int iconSize = 14; // Smaller icon size
                    int iconPadding = 2;

                    if (col.ShowSortIcon) iconSpace += iconSize + iconPadding;
                    if (col.ShowFilterIcon) iconSpace += iconSize + iconPadding;

                    if (iconSpace > 0)
                    {
                        textRect.Width -= iconSpace + 4; // Reserve space for icons
                        textRect.Height -= iconSize + iconPadding; // Reserve vertical space too
                    }

                    // Draw column text (centered in the available space)
                    string caption = col.ColumnCaption ?? col.ColumnName ?? "";
                    var centeredFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(caption, _columnHeadertextFont ?? Font, textBrush, textRect, centeredFormat);

                    // Position icons in corners
                    if (col.ShowSortIcon && col.ShowFilterIcon)
                    {
                        // Sort icon in UPPER RIGHT corner
                        Rectangle sortIconRect = new Rectangle(
                            cellRect.Right - iconSize - iconPadding,
                            cellRect.Top + iconPadding,
                            iconSize,
                            iconSize
                        );
                        DrawSortIcon(g, sortIconRect, col.SortDirection, headerForeColor);
                        AddHitArea($"SortIcon_{col.Index}", sortIconRect);

                        // Filter icon in BOTTOM RIGHT corner
                        Rectangle filterIconRect = new Rectangle(
                            cellRect.Right - iconSize - iconPadding,
                            cellRect.Bottom - iconSize - iconPadding,
                            iconSize,
                            iconSize
                        );
                        DrawFilterIcon(g, filterIconRect, col.IsFiltered, headerForeColor);
                        AddHitArea($"FilterIcon_{col.Index}", filterIconRect);
                    }
                    else if (col.ShowSortIcon)
                    {
                        // Only sort icon - place in upper right
                        Rectangle sortIconRect = new Rectangle(
                            cellRect.Right - iconSize - iconPadding,
                            cellRect.Top + iconPadding,
                            iconSize,
                            iconSize
                        );
                        DrawSortIcon(g, sortIconRect, col.SortDirection, headerForeColor);
                        AddHitArea($"SortIcon_{col.Index}", sortIconRect);
                    }
                    else if (col.ShowFilterIcon)
                    {
                        // Only filter icon - place in upper right  
                        Rectangle filterIconRect = new Rectangle(
                            cellRect.Right - iconSize - iconPadding,
                            cellRect.Top + iconPadding,
                            iconSize,
                            iconSize
                        );
                        DrawFilterIcon(g, filterIconRect, col.IsFiltered, headerForeColor);
                        AddHitArea($"FilterIcon_{col.Index}", filterIconRect);
                    }
                }
            }

            // Draw border
            Color borderColor = col.HasCustomBorderColor ? col.ColumnBorderColor : _currentTheme.GridLineColor;
            using (Pen borderPen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(borderPen, cellRect);
            }
        }

        /// <summary>
        /// Draws the sort icon
        /// </summary>
        private void DrawSortIcon(Graphics g, Rectangle iconRect, SortDirection direction, Color color)
        {
            using (Pen pen = new Pen(color, 1.5f))
            using (Brush brush = new SolidBrush(color))
            {
                int centerX = iconRect.X + iconRect.Width / 2;
                int centerY = iconRect.Y + iconRect.Height / 2;
                int arrowSize = Math.Min(iconRect.Width, iconRect.Height) / 3; // Smaller arrows

                switch (direction)
                {
                    case SortDirection.Ascending:
                        // Draw up arrow (smaller and more refined)
                        Point[] upArrowAsc = {
                    new Point(centerX, iconRect.Y + 2),
                    new Point(centerX - arrowSize, centerY + 1),
                    new Point(centerX + arrowSize, centerY + 1)
                };
                        g.FillPolygon(brush, upArrowAsc);
                        break;

                    case SortDirection.Descending:
                        // Draw down arrow (smaller and more refined)
                        Point[] downArrowDesc = {
                    new Point(centerX, iconRect.Bottom - 2),
                    new Point(centerX - arrowSize, centerY - 1),
                    new Point(centerX + arrowSize, centerY - 1)
                };
                        g.FillPolygon(brush, downArrowDesc);
                        break;

                    case SortDirection.None:
                        // Draw both arrows (inactive, lighter)
                        using (Pen lightPen = new Pen(Color.FromArgb(100, color), 1))
                        {
                            Point[] upArrowNone = {
                        new Point(centerX, iconRect.Y + 2),
                        new Point(centerX - arrowSize + 1, centerY),
                        new Point(centerX + arrowSize - 1, centerY)
                    };
                            Point[] downArrowNone = {
                        new Point(centerX, iconRect.Bottom - 2),
                        new Point(centerX - arrowSize + 1, centerY),
                        new Point(centerX + arrowSize - 1, centerY)
                    };
                            g.DrawPolygon(lightPen, upArrowNone);
                            g.DrawPolygon(lightPen, downArrowNone);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Draws the filter icon
        /// </summary>
        private void DrawFilterIcon(Graphics g, Rectangle iconRect, bool isActive, Color color)
        {
            using (Pen pen = new Pen(isActive ? color : Color.FromArgb(100, color), isActive ? 1.5f : 1f))
            using (Brush brush = new SolidBrush(isActive ? color : Color.FromArgb(100, color)))
            {
                // Draw a more compact filter funnel shape
                int padding = 1;
                int funnelWidth = iconRect.Width - (padding * 2);
                int funnelHeight = iconRect.Height - (padding * 2);

                Point[] funnel = {
            new Point(iconRect.X + padding, iconRect.Y + padding),
            new Point(iconRect.Right - padding, iconRect.Y + padding),
            new Point(iconRect.X + iconRect.Width / 2 + 2, iconRect.Y + funnelHeight / 2),
            new Point(iconRect.X + iconRect.Width / 2 + 2, iconRect.Bottom - padding),
            new Point(iconRect.X + iconRect.Width / 2 - 2, iconRect.Bottom - padding),
            new Point(iconRect.X + iconRect.Width / 2 - 2, iconRect.Y + funnelHeight / 2)
        };

                if (isActive)
                    g.FillPolygon(brush, funnel);
                else
                    g.DrawPolygon(pen, funnel);

                // Add a small circle or dot to indicate active filter
                if (isActive)
                {
                    using (Brush dotBrush = new SolidBrush(Color.FromArgb(200, Color.White)))
                    {
                        int dotSize = 2;
                        g.FillEllipse(dotBrush,
                            iconRect.X + iconRect.Width / 2 - dotSize / 2,
                            iconRect.Y + iconRect.Height / 2 - dotSize / 2,
                            dotSize, dotSize);
                    }
                }
            }
        }

        #endregion

        #region Navigation Row Drawing

        /// <summary>
        /// Draws the navigation row (with buttons and record counter)
        /// </summary>
        private void DrawNavigationRow(Graphics g, Rectangle rect)
        {
            // Fill background
            using (var brush = new SolidBrush(_currentTheme.GridHeaderBackColor))
            {
                g.FillRectangle(brush, rect);
            }

            // Draw top border line
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
            }

            // Calculate layout positions
            int buttonHeight = 24;
            int buttonWidth = 24;
            int padding = 6;
            int y = rect.Top + (rect.Height - buttonHeight) / 2; // Center buttons vertically
            int x = rect.Left + padding;

            // Draw navigation buttons (left side)
            DrawNavigationButton(g, "FindButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.079-search.svg",
                new Rectangle(x, y, buttonWidth, buttonHeight));
            x += buttonWidth + padding;

            DrawNavigationButton(g, "EditButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.062-pencil.svg",
                new Rectangle(x, y, buttonWidth, buttonHeight));
            x += buttonWidth + padding;

            DrawNavigationButton(g, "PrinterButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.072-printer.svg",
                new Rectangle(x, y, buttonWidth, buttonHeight));
            x += buttonWidth + padding;

            DrawNavigationButton(g, "MessageButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.083-share.svg",
                new Rectangle(x, y, buttonWidth, buttonHeight));
            x += buttonWidth + padding;

            DrawNavigationButton(g, "SaveButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.036-floppy disk.svg",
                new Rectangle(x, y, buttonWidth, buttonHeight));
            x += buttonWidth + padding;

            DrawNavigationButton(g, "NewButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.068-plus.svg",
                new Rectangle(x, y, buttonWidth, buttonHeight));
            x += buttonWidth + padding;

            DrawNavigationButton(g, "RemoveButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.035-eraser.svg",
                new Rectangle(x, y, buttonWidth, buttonHeight));
            x += buttonWidth + padding;

            DrawNavigationButton(g, "RollbackButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.005-back arrow.svg",
                new Rectangle(x, y, buttonWidth, buttonHeight));
            x += buttonWidth + padding;

            // Draw record counter (center) with navigation buttons
            string recordCounter = (_fullData != null && _fullData.Any())
                ? $"{(_currentRowIndex + _dataOffset + 1)} - {_fullData.Count}"
                : "0 - 0";

            // Calculate center area for record counter display
            using (var font = new Font(Font.FontFamily, 9f))
            using (var brush = new SolidBrush(_currentTheme.GridHeaderForeColor))
            {
                SizeF textSize = TextUtils.MeasureText(g, recordCounter, font);

                // Center x position for record counter text
                float recordX = rect.Left + ((rect.Width - textSize.Width) / 2);

                // Add record navigation buttons around the counter
                int navButtonWidth = 20;
                int navButtonSpacing = 6;

                // First Record button - left of counter
                DrawNavigationButton(g, "FirstRecordButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-double-small-left.svg",
                    new Rectangle((int)recordX - navButtonWidth * 2 - navButtonSpacing * 2, y, navButtonWidth, buttonHeight));

                // Previous Record button - left of counter
                DrawNavigationButton(g, "PreviousRecordButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-left.svg",
                    new Rectangle((int)recordX - navButtonWidth - navButtonSpacing, y, navButtonWidth, buttonHeight));

                // Draw the record counter text
                g.DrawString(recordCounter, font, brush, recordX, y + (buttonHeight - textSize.Height) / 2);

                // Next Record button - right of counter
                DrawNavigationButton(g, "NextRecordButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-right.svg",
                    new Rectangle((int)(recordX + textSize.Width + navButtonSpacing), y, navButtonWidth, buttonHeight));

                // Last Record button - right of counter
                DrawNavigationButton(g, "LastRecordButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-double-small-right.svg",
                    new Rectangle((int)(recordX + textSize.Width + navButtonWidth + navButtonSpacing * 2), y, navButtonWidth, buttonHeight));
            }


            // Draw pagination controls (right side)
            int pageButtonX = rect.Right - padding - buttonWidth;

            DrawNavigationButton(g, "LastPageButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-double-small-right.svg",
                new Rectangle(pageButtonX, y, buttonWidth, buttonHeight));
            pageButtonX -= buttonWidth + padding;

            DrawNavigationButton(g, "NextPageButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-right.svg",
                new Rectangle(pageButtonX, y, buttonWidth, buttonHeight));
            pageButtonX -= buttonWidth + padding;

            // Draw page counter
            int visrowcount = GetVisibleRowCount();
            _totalPages = _fullData != null ?
                (int)Math.Ceiling((double)_fullData.Count / (visrowcount == 1 ? _fullData.Count : visrowcount)) : 1;
            _currentPage = Math.Max(1, Math.Min(_currentPage, _totalPages));
            string pageCounter = $"{_currentPage} of {_totalPages}";

            using (var font = new Font(Font.FontFamily, 9f))
            using (var brush = new SolidBrush(_currentTheme.GridHeaderForeColor))
            {
                SizeF textSize = TextUtils.MeasureText(g, pageCounter, font);
                pageButtonX -= (int)textSize.Width + padding;
                g.DrawString(pageCounter, font, brush, pageButtonX, y + (buttonHeight - textSize.Height) / 2);
            }

            pageButtonX -= buttonWidth + padding;
            DrawNavigationButton(g, "PrevPageButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-left.svg",
                new Rectangle(pageButtonX, y, buttonWidth, buttonHeight));
            pageButtonX -= buttonWidth + padding;

            DrawNavigationButton(g, "FirstPageButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-double-small-left.svg",
                new Rectangle(pageButtonX, y, buttonWidth, buttonHeight));

            _navigatorDrawn = true;
        }

        private void DrawNavigationButton(Graphics g, string buttonName, string imagePath, Rectangle buttonRect)
        {
            // Create a temporary button to use its drawing mechanism
            if (!_navigationButtonCache.TryGetValue(buttonName, out BeepButton tempButton))
            {
                tempButton = new BeepButton
                {
                    ImagePath = imagePath,
                    ImageAlign = ContentAlignment.MiddleCenter,
                    HideText = true,
                    IsFrameless = true,
                    Size = buttonRect.Size,
                    Theme = Theme,
                    IsChild = true,
                    MaxImageSize = new Size(buttonRect.Width - 4, buttonRect.Height - 4),
                    ApplyThemeOnImage = true,
                    Name = buttonName,
                    ComponentName = buttonName // Set ComponentName for tooltip support
                };
                _navigationButtonCache[buttonName] = tempButton;
            }
            // Check if mouse is hovering over this button
            bool isHovered = false;
            Point mousePos = PointToClient(MousePosition);
            if (buttonRect.Contains(mousePos))
            {
                isHovered = true;
                tempButton.IsHovered = true;

                // Show tooltip if button is being hovered
                if (!tooltipShown)
                {
                    tempButton.ShowToolTip(buttonName.Replace("Button", ""));
                    tooltipShown = true;
                }
            }
            tempButton.Size = buttonRect.Size;
            // Draw the button with hover effect
            tempButton.Draw(g, buttonRect);

            // Add to the hit test list for click detection
            AddHitArea(buttonName, buttonRect);
        }

        #endregion

        #region Helper Methods

        private void UpdateStickyWidth()
        {
            var selColumn = Columns.FirstOrDefault(c => c.IsSelectionCheckBox);
            if (_showCheckboxes)
            {
                selColumn.Visible = true;
            }
            else
                selColumn.Visible = false;
            var stickyColumns = Columns.Where(c => c.Sticked && c.Visible).ToList();
            int baseStickyWidth = stickyColumns.Sum(c => c.Width);

            // Cap _stickyWidth to prevent overflow within gridRect
            _stickyWidth = Math.Min(baseStickyWidth, gridRect.Width);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Partial class containing navigation controls functionality for BeepSimpleGrid
    /// Handles navigation buttons, paging, and record navigation
    /// </summary>
    public partial class BeepSimpleGrid
    {
        #region Navigation Button Management

        /// <summary>
        /// Hides all navigation buttons
        /// </summary>
        private void HideNavigationButtons()
        {
            foreach (var button in buttons)
            {
                button.Visible = false;
            }
            foreach (var button in pagingButtons)
            {
                button.Visible = false;
            }
        }

        /// <summary>
        /// Shows all navigation buttons
        /// </summary>
        private void ShowNavigationButtons()
        {
            foreach (var button in buttons)
            {
                button.Visible = true;
            }
            foreach (var button in pagingButtons)
            {
                button.Visible = true;
            }
        }

        /// <summary>
        /// Creates all navigation buttons
        /// </summary>
        private void CreateNavigationButtons()
        {
            // Main navigation buttons
            FindButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.079-search.svg", buttonSize, FindpictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            EditButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.062-pencil.svg", buttonSize, EditpictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            PrinterButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.072-printer.svg", buttonSize, PrinterpictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            MessageButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.083-share.svg", buttonSize, MessagepictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            SaveButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.036-floppy disk.svg", buttonSize, SavepictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            NewButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.068-plus.svg", buttonSize, NewButton_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            RemoveButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.035-eraser.svg", buttonSize, RemovepictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            RollbackButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.005-back arrow.svg", buttonSize, RollbackpictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            NextButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-right.svg", buttonSize, NextpictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            PreviousButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-left.svg", buttonSize, PreviouspictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);

            // Page label (as a BeepButton)
            PageLabel = new BeepButton
            {
                Text = "1 of 1",
                Size = new Size(pageLabelWidth, buttonSize.Height),
                HideText = false,
                IsFrameless = true,
                IsChild = true,
                ShowAllBorders = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = true
            };

            // Record number label (as a BeepButton)
            Recordnumberinglabel1 = new BeepButton
            {
                Text = "0 - 0",
                Size = new Size(labelWidth, buttonSize.Height),
                HideText = false,
                IsFrameless = true,
                IsChild = true,
                ShowAllBorders = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = true
            };

            // Paging buttons
            FirstPageButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-double-small-left.svg", buttonSize, FirstPageButton_Click, AnchorStyles.Right | AnchorStyles.Bottom);
            PrevPageButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-left.svg", buttonSize, PrevPageButton_Click, AnchorStyles.Right | AnchorStyles.Bottom);
            NextPageButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-right.svg", buttonSize, NextPageButton_Click, AnchorStyles.Right | AnchorStyles.Bottom);
            LastPageButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-double-small-right.svg", buttonSize, LastPageButton_Click, AnchorStyles.Right | AnchorStyles.Bottom);

            buttons = new List<Control>
            {
                FindButton, PrinterButton, MessageButton, NextButton, NewButton, EditButton, 
                SaveButton, RollbackButton, PreviousButton, Recordnumberinglabel1, NextButton, RemoveButton
            };

            pagingButtons = new List<Control>
            {
                FirstPageButton, PrevPageButton, PageLabel, NextPageButton, LastPageButton
            };

            UpdateRecordNumber();
            UpdatePagingControls();
        }

        /// <summary>
        /// Creates a navigation button with the specified parameters
        /// </summary>
        private BeepButton CreateButton(string imagePath, Size size, EventHandler clickHandler, AnchorStyles anchorStyles)
        {
            var button = new BeepButton
            {
                ImagePath = imagePath,
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true,
                IsFrameless = true,
                Size = size,
                IsChild = true,
                Anchor = anchorStyles,
                Margin = new Padding(0),
                ApplyThemeOnImage = true,
                Padding = new Padding(0),
                IsRounded = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ImageEmbededin = ImageEmbededin.DataGridView,
                MaxImageSize = new Size(size.Width - 4, size.Height - 4),
                Visible = true
            };
            button.Click += clickHandler;
            return button;
        }

        /// <summary>
        /// Positions navigation controls within the specified rectangle
        /// </summary>
        private void PositionControls(Rectangle rect, int spacing)
        {
            int rowHeight = buttonSize.Height;
            int centerY = rect.Top + ((rect.Height - rowHeight) / 2);
            if (centerY < 0) centerY = spacing;

            // Position main buttons on the left
            int startX = rect.X + spacing;
            int currentX = startX;

            foreach (var control in buttons)
            {
                control.Left = currentX;
                control.Top = centerY;
                currentX += control.Width + spacing;
            }

            // Position paging buttons on the right
            int pagingTotalWidth = pagingButtons.Sum(c => c.Width) + spacing * (pagingButtons.Count - 1);
            int pagingStartX = rect.Width - pagingTotalWidth - (2 * spacing);
            if (pagingStartX < currentX) pagingStartX = currentX;
            currentX = pagingStartX;

            foreach (var control in pagingButtons)
            {
                control.Visible = true;
                control.Left = currentX;
                control.Top = centerY;
                currentX += control.Width + spacing;
            }

            UpdatePagingControls();
            UpdateRecordNumber();
        }

        #endregion

        #region Navigation Updates

        /// <summary>
        /// Updates the record number display
        /// </summary>
        private void UpdateRecordNumber()
        {
            if (Recordnumberinglabel1 == null) return;

            if (_fullData != null && _fullData.Any())
            {
                int position = _currentRowIndex + _dataOffset + 1;
                Recordnumberinglabel1.Text = $"{position} - {_fullData.Count}";
            }
            else
            {
                Recordnumberinglabel1.Text = "0 - 0";
            }
        }

        /// <summary>
        /// Updates navigation button enabled states
        /// </summary>
        private void UpdateNavigationButtonState()
        {
            if (_fullData == null || !_fullData.Any())
            {
                // Disable all navigation buttons when no data
                if (PreviousButton != null) PreviousButton.Enabled = false;
                if (NextButton != null) NextButton.Enabled = false;
                if (FirstPageButton != null) FirstPageButton.Enabled = false;
                if (PrevPageButton != null) PrevPageButton.Enabled = false;
                if (NextPageButton != null) NextPageButton.Enabled = false;
                if (LastPageButton != null) LastPageButton.Enabled = false;
                return;
            }

            int currentPosition = _currentRowIndex + _dataOffset;
            bool isFirstRecord = currentPosition <= 0;
            bool isLastRecord = currentPosition >= _fullData.Count - 1;

            // Update record navigation buttons
            if (PreviousButton != null) PreviousButton.Enabled = !isFirstRecord;
            if (NextButton != null) NextButton.Enabled = !isLastRecord;

            // Update paging buttons
            if (FirstPageButton != null) FirstPageButton.Enabled = _currentPage > 1;
            if (PrevPageButton != null) PrevPageButton.Enabled = _currentPage > 1;
            if (NextPageButton != null) NextPageButton.Enabled = _currentPage < _totalPages;
            if (LastPageButton != null) LastPageButton.Enabled = _currentPage < _totalPages;
        }

        /// <summary>
        /// Updates paging controls display
        /// </summary>
        private void UpdatePagingControls()
        {
            if (PageLabel == null || _fullData == null) return;

            int visibleRowCount = GetVisibleRowCount();
            _totalPages = visibleRowCount > 0 ? 
                (int)Math.Ceiling((double)_fullData.Count / visibleRowCount) : 1;
            _currentPage = Math.Max(1, Math.Min(_currentPage, _totalPages));

            PageLabel.Text = $"{_currentPage} of {_totalPages}";
        }

        #endregion

        #region Paging Event Handlers

        private void FirstPageButton_Click(object sender, EventArgs e)
        {
            _currentPage = 1;
            UpdatePage();
        }

        private void PrevPageButton_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                UpdatePage();
            }
        }

        private void NextPageButton_Click(object sender, EventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                UpdatePage();
            }
        }

        private void LastPageButton_Click(object sender, EventArgs e)
        {
            _currentPage = _totalPages;
            UpdatePage();
        }

        private void UpdatePage()
        {
            if (_fullData == null || _fullData.Count == 0) return;

            int visibleRowCount = GetVisibleRowCount();
            int newOffset = (_currentPage - 1) * visibleRowCount;
            int maxOffset = Math.Max(0, _fullData.Count - visibleRowCount);
            newOffset = Math.Min(newOffset, maxOffset);

            if (newOffset != _dataOffset)
            {
                StartSmoothScroll(newOffset);
            }

            UpdatePagingControls();
        }

        #endregion

        #region Navigation Button Event Handlers

        private void SavepictureBox_Click(object sender, EventArgs e)
        {
            SaveToDataSource();
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            if (_dataSource is BindingSource)
            {
                CreateNewRecordForBindingSource();
            }
            else if (_dataSource is System.Collections.IList)
            {
                CreateNewRecordForIList();
            }
        }

        private void RemovepictureBox_Click(object sender, EventArgs e)
        {
            if (_currentRow == null) return;

            int dataIndex = _currentRow.DisplayIndex;
            if (dataIndex < 0 || dataIndex >= _fullData.Count) return;

            var wrapper = _fullData[dataIndex] as DataRowWrapper;
            if (wrapper == null) return;

            if (VerifyDelete)
            {
                var result = MessageBox.Show("Are you sure you want to delete this record?", 
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes) return;
            }

            DeleteFromDataSource(wrapper.OriginalData);
            FillVisibleRows();
            UpdateScrollBars();
            Invalidate();
        }

        private void NextpictureBox_Click(object sender, EventArgs e)
        {
            MoveNextRow();
        }

        private void PreviouspictureBox_Click(object sender, EventArgs e)
        {
            MovePreviousRow();
        }

        private void RollbackpictureBox_Click(object sender, EventArgs e)
        {
            ResetToOriginal();
        }

        private void EditpictureBox_Click(object sender, EventArgs e)
        {
            if (_selectedCell != null)
            {
                BeginEdit();
            }
        }

        private void FindpictureBox_Click(object sender, EventArgs e)
        {
            ShowSearch?.Invoke(this, EventArgs.Empty);
        }

        private void PrinterpictureBox_Click(object sender, EventArgs e)
        {
            CallPrinter?.Invoke(this, EventArgs.Empty);
            if (Rows.Count > 0)
            {
                PrintGridAsReportDocument();
            }
            else
            {
                MessageBox.Show("No data to print.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void MessagepictureBox_Click(object sender, EventArgs e)
        {
            SendMessage?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Prints grid as report document
        /// </summary>
        private void PrintGridAsReportDocument()
        {
            // This method will be implemented in the Print partial class
            // For now, just a placeholder that invokes the event
            GetAllRows();
        }

        #endregion
    }
}

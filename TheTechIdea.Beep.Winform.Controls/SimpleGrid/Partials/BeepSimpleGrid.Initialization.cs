using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.TextFields;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Partial class containing initialization logic for BeepSimpleGrid
    /// Handles creation and initialization of all embedded controls
    /// </summary>
    public partial class BeepSimpleGrid
    {
        /// <summary>
        /// Initializes all embedded controls for the grid
        /// </summary>
        private void InitializeControls()
        {
            InitializeScrollBars();
            InitializeNavigationButtons();
            InitializePagingButtons();
            InitializeFilterControls();
            InitializePanelLabels();
            InitializeSelectionCheckBox();
        }

        /// <summary>
        /// Initializes scrollbar controls
        /// </summary>
        private void InitializeScrollBars()
        {
            if (_verticalScrollBar == null)
            {
                _verticalScrollBar = new BeepScrollBar
                {
                    ScrollOrientation = Orientation.Vertical,
                    Theme = Theme,
                    Visible = false,
                    IsChild = true
                };
                _verticalScrollBar.Scroll += VerticalScrollBar_Scroll;
                Controls.Add(_verticalScrollBar);
            }

            if (_horizontalScrollBar == null)
            {
                _horizontalScrollBar = new BeepScrollBar
                {
                    ScrollOrientation = Orientation.Horizontal,
                    Theme = Theme,
                    Visible = false,
                    IsChild = true
                };
                _horizontalScrollBar.Scroll += HorizontalScrollBar_Scroll;
                Controls.Add(_horizontalScrollBar);
            }
        }

        /// <summary>
        /// Initializes navigation buttons (Find, New, Edit, Previous, Next, Remove, etc.)
        /// </summary>
        private void InitializeNavigationButtons()
        {
            FindButton = CreateNavigationButton("Find");
            NewButton = CreateNavigationButton("New");
            EditButton = CreateNavigationButton("Edit");
            PreviousButton = CreateNavigationButton("Previous");
            NextButton = CreateNavigationButton("Next");
            RemoveButton = CreateNavigationButton("Remove");
            RollbackButton = CreateNavigationButton("Rollback");
            SaveButton = CreateNavigationButton("Save");
            PrinterButton = CreateNavigationButton("Printer");
            MessageButton = CreateNavigationButton("Message");

            buttons = new List<Control>
            {
                FindButton, NewButton, EditButton, PreviousButton, NextButton,
                RemoveButton, RollbackButton, SaveButton, PrinterButton, MessageButton
            };

            foreach (var button in buttons)
            {
                Controls.Add(button);
            }
        }

        /// <summary>
        /// Initializes paging buttons (First, Previous, Next, Last page)
        /// </summary>
        private void InitializePagingButtons()
        {
            FirstPageButton = CreateNavigationButton("FirstPage");
            PrevPageButton = CreateNavigationButton("PrevPage");
            NextPageButton = CreateNavigationButton("NextPage");
            LastPageButton = CreateNavigationButton("LastPage");
            PageLabel = CreateNavigationButton("PageLabel");
            Recordnumberinglabel1 = CreateNavigationButton("RecordNumber");

            pagingButtons = new List<Control>
            {
                FirstPageButton, PrevPageButton, NextPageButton, LastPageButton, PageLabel, Recordnumberinglabel1
            };

            foreach (var button in pagingButtons)
            {
                Controls.Add(button);
            }
        }

        /// <summary>
        /// Initializes filter controls (filter textbox and column selector)
        /// </summary>
        private void InitializeFilterControls()
        {
            if (filterTextBox == null)
            {
                filterTextBox = new BeepTextBox
                {
                    Name = "filterTextBox",
                    Theme = Theme,
                    IsChild = true,
                    Width = 150,
                    Height = 20,
                    Visible = false,
                    PlaceholderText = "Filter ......"
                };
                filterTextBox.TextChanged += FilterTextBox_TextChanged;
                Controls.Add(filterTextBox);
            }

            if (filterColumnComboBox == null)
            {
                filterColumnComboBox = new BeepComboBox
                {
                    Name = "filterColumnComboBox",
                    Theme = Theme,
                    IsChild = true,
                    Width = 120,
                    Visible = false,
                    Height = 20
                };
                filterColumnComboBox.SelectedItemChanged += FilterColumnComboBox_SelectedIndexChanged;
                Controls.Add(filterColumnComboBox);
            }

            if (filterButton == null)
            {
                filterButton = new BeepButton
                {
                    Name = "filterButton",
                    Text = "Filter",
                    Visible = _showFilterButton,
                    Theme = Theme,
                    IsChild = true
                };
                Controls.Add(filterButton);
            }
        }

        /// <summary>
        /// Initializes panel labels (title, percentage)
        /// </summary>
        private void InitializePanelLabels()
        {
            if (titleLabel == null)
            {
                titleLabel = new BeepLabel
                {
                    Name = "titleLabel",
                    Text = _titletext,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Theme = Theme,
                    ImageAlign = ContentAlignment.MiddleLeft,
                    TextImageRelation = TextImageRelation.ImageBeforeText,
                    MaxImageSize = new Size(20, 20),
                    ShowAllBorders = false,
                    ShowShadow = false,
                    IsChild = true,
                    Visible = false,
                    IsShadowAffectedByTheme = false,
                    IsBorderAffectedByTheme = false
                };
                titleLabel.ImagePath = _titleimagestring;
                Controls.Add(titleLabel);
            }

            if (percentageLabel == null)
            {
                percentageLabel = new BeepLabel
                {
                    Name = "percentageLabel",
                    Text = _percentageText,
                    Visible = _showFooter,
                    Theme = Theme,
                    IsChild = true
                };
                Controls.Add(percentageLabel);
            }
        }

        /// <summary>
        /// Initializes the "Select All" checkbox for column headers
        /// </summary>
        private void InitializeSelectionCheckBox()
        {
            if (_selectAllCheckBox == null)
            {
                _selectAllCheckBox = new BeepCheckBoxBool
                {
                    Name = "_selectAllCheckBox",
                    Visible = _showCheckboxes && _showColumnHeaders,
                    Theme = Theme,
                    IsChild = true,
                    HideText = true,
                    Text = string.Empty
                };
                _selectAllCheckBox.StateChanged += SelectAllCheckBox_StateChanged;
                Controls.Add(_selectAllCheckBox);
            }
        }

        /// <summary>
        /// Creates a navigation button with the specified name
        /// </summary>
        private BeepButton CreateNavigationButton(string buttonName)
        {
            return new BeepButton
            {
                Name = buttonName,
                Text = buttonName,
                Size = buttonSize,
                Theme = Theme,
                Visible = _showNavigator
            };
        }
    }
}

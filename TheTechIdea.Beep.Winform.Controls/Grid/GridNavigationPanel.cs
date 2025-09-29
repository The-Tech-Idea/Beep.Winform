using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Grid
{
    /// <summary>
    /// Grid navigation panel - displays navigation buttons and record counter
    /// Modern React-style component with BeepControl HitArea integration
    /// </summary>
    [ToolboxItem(false)]
    public class GridNavigationPanel : BeepControl
    {
        #region Fields
        private BeepGrid _parentGrid;
        private int _currentRecord = 0;
        private int _totalRecords = 0;
        private int _currentPage = 1;
        private int _totalPages = 1;
        private int _navigationHeight = 30;
        private int _buttonSize = 24;
        private int _buttonSpacing = 6;
        private bool _showRecordNavigation = true;
        private bool _showPageNavigation = true;
        private bool _showActionButtons = true;
        private string _hoveredButton = null;
        #endregion

        #region Events
        public event EventHandler<string> NavigationButtonClicked;
        public event EventHandler<NavigationEventArgs> NavigationChanged;
        #endregion

        #region Properties
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(30)]
        public int NavigationHeight
        {
            get => _navigationHeight;
            set
            {
                if (_navigationHeight != value && value > 20)
                {
                    _navigationHeight = value;
                    Height = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool ShowRecordNavigation
        {
            get => _showRecordNavigation;
            set
            {
                if (_showRecordNavigation != value)
                {
                    _showRecordNavigation = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool ShowPageNavigation
        {
            get => _showPageNavigation;
            set
            {
                if (_showPageNavigation != value)
                {
                    _showPageNavigation = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool ShowActionButtons
        {
            get => _showActionButtons;
            set
            {
                if (_showActionButtons != value)
                {
                    _showActionButtons = value;
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        public int CurrentRecord
        {
            get => _currentRecord;
            set
            {
                if (_currentRecord != value)
                {
                    _currentRecord = value;
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        public int TotalRecords
        {
            get => _totalRecords;
            set
            {
                if (_totalRecords != value)
                {
                    _totalRecords = value;
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        public int TotalPages
        {
            get => _totalPages;
            set
            {
                if (_totalPages != value)
                {
                    _totalPages = value;
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        public BeepGrid ParentGrid => _parentGrid;
        #endregion

        #region Constructor
        public GridNavigationPanel(BeepGrid parentGrid = null)
        {
            _parentGrid = parentGrid;
            
            Height = _navigationHeight;
            Dock = DockStyle.Bottom;
            
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | 
                     ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
                     
            // Subscribe to BeepControl HitDetected event
            HitDetected += OnHitDetected;
        }
        #endregion

        #region BeepControl HitArea Integration
        
        /// <summary>
        /// Handle HitDetected events from BeepControl
        /// This automatically handles all mouse interactions
        /// </summary>
        private void OnHitDetected(object sender, ControlHitTestArgs e)
        {
            // The hit action will be automatically executed
            // Optional: Update hover state for visual feedback
            if (e.HitTest != null)
            {
                _hoveredButton = e.HitTest.Name;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Handle navigation button clicks - called via HitArea actions
        /// No more manual coordinate calculations needed!
        /// </summary>
        private void OnNavigationButtonClicked(string buttonName)
        {
            NavigationButtonClicked?.Invoke(this, buttonName);
        }
        
        #endregion

        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            // Clear previous hit areas
            ClearHitList();

            // High-quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Fill background
            using (var brush = new SolidBrush(_currentTheme.GridHeaderBackColor))
            {
                g.FillRectangle(brush, ClientRectangle);
            }

            // Draw top border line
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                g.DrawLine(pen, 0, 0, Width, 0);
            }

            DrawNavigationButtons(g);
        }

        private void DrawNavigationButtons(Graphics g)
        {
            int y = (Height - _buttonSize) / 2;
            int x = _buttonSpacing;

            // Draw action buttons (left side)
            if (_showActionButtons)
            {
                x = DrawActionButtons(g, x, y);
                x += _buttonSpacing * 2; // Extra space after action buttons
            }

            // Draw record counter (center) with navigation buttons
            if (_showRecordNavigation)
            {
                x = DrawRecordNavigation(g, x, y);
                x += _buttonSpacing * 2; // Extra space after record navigation
            }

            // Draw pagination controls (right side)
            if (_showPageNavigation)
            {
                DrawPageNavigation(g, y);
            }
        }

        private int DrawActionButtons(Graphics g, int x, int y)
        {
            var actionButtons = new[]
            {
                new { Name = "FindButton", Icon = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.079-search.svg" },
                new { Name = "EditButton", Icon = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.062-pencil.svg" },
                new { Name = "PrinterButton", Icon = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.072-printer.svg" },
                new { Name = "MessageButton", Icon = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.083-share.svg" },
                new { Name = "SaveButton", Icon = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.036-floppy disk.svg" },
                new { Name = "NewButton", Icon = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.068-plus.svg" },
                new { Name = "RemoveButton", Icon = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.035-eraser.svg" },
                new { Name = "RollbackButton", Icon = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.005-back arrow.svg" }
            };

            foreach (var button in actionButtons)
            {
                var buttonRect = new Rectangle(x, y, _buttonSize, _buttonSize);
                DrawNavigationButton(g, button.Name, button.Icon, buttonRect);
                x += _buttonSize + _buttonSpacing;
            }

            return x;
        }

        private int DrawRecordNavigation(Graphics g, int x, int y)
        {
            string recordCounter = _totalRecords > 0 
                ? $"{Math.Max(1, _currentRecord + 1)} - {_totalRecords}"
                : "0 - 0";

            // Calculate center area for record counter display
            using (var font = new Font(Font.FontFamily, 9f))
            using (var brush = new SolidBrush(_currentTheme.GridHeaderForeColor))
            {
                SizeF textSize = g.MeasureString(recordCounter, font);
                float recordX = x;

                int navButtonWidth = 20;
                int navButtonSpacing = 6;

                // First Record button
                var firstRect = new Rectangle((int)recordX, y, navButtonWidth, _buttonSize);
                DrawNavigationButton(g, "FirstRecordButton", 
                    "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-double-small-left.svg", firstRect);
                recordX += navButtonWidth + navButtonSpacing;

                // Previous Record button
                var prevRect = new Rectangle((int)recordX, y, navButtonWidth, _buttonSize);
                DrawNavigationButton(g, "PreviousRecordButton", 
                    "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-left.svg", prevRect);
                recordX += navButtonWidth + navButtonSpacing;

                // Draw the record counter text
                var textRect = new RectangleF(recordX, y + (_buttonSize - textSize.Height) / 2, 
                    textSize.Width, textSize.Height);
                g.DrawString(recordCounter, font, brush, textRect);
                recordX += textSize.Width + navButtonSpacing;

                // Next Record button
                var nextRect = new Rectangle((int)recordX, y, navButtonWidth, _buttonSize);
                DrawNavigationButton(g, "NextRecordButton", 
                    "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-right.svg", nextRect);
                recordX += navButtonWidth + navButtonSpacing;

                // Last Record button
                var lastRect = new Rectangle((int)recordX, y, navButtonWidth, _buttonSize);
                DrawNavigationButton(g, "LastRecordButton", 
                    "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-double-small-right.svg", lastRect);
                recordX += navButtonWidth;

                return (int)recordX;
            }
        }

        private void DrawPageNavigation(Graphics g, int y)
        {
            int pageButtonX = Width - _buttonSpacing - _buttonSize;

            // Last Page button
            var lastPageRect = new Rectangle(pageButtonX, y, _buttonSize, _buttonSize);
            DrawNavigationButton(g, "LastPageButton", 
                "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-double-small-right.svg", lastPageRect);
            pageButtonX -= _buttonSize + _buttonSpacing;

            // Next Page button
            var nextPageRect = new Rectangle(pageButtonX, y, _buttonSize, _buttonSize);
            DrawNavigationButton(g, "NextPageButton", 
                "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-right.svg", nextPageRect);
            pageButtonX -= _buttonSize + _buttonSpacing;

            // Page counter
            string pageCounter = $"{_currentPage} of {Math.Max(1, _totalPages)}";
            using (var font = new Font(Font.FontFamily, 9f))
            using (var brush = new SolidBrush(_currentTheme.GridHeaderForeColor))
            {
                SizeF textSize = g.MeasureString(pageCounter, font);
                pageButtonX -= (int)textSize.Width + _buttonSpacing;
                var textRect = new RectangleF(pageButtonX, y + (_buttonSize - textSize.Height) / 2, 
                    textSize.Width, textSize.Height);
                g.DrawString(pageCounter, font, brush, textRect);
            }

            pageButtonX -= _buttonSize + _buttonSpacing;

            // Previous Page button
            var prevPageRect = new Rectangle(pageButtonX, y, _buttonSize, _buttonSize);
            DrawNavigationButton(g, "PrevPageButton", 
                "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-left.svg", prevPageRect);
            pageButtonX -= _buttonSize + _buttonSpacing;

            // First Page button
            var firstPageRect = new Rectangle(pageButtonX, y, _buttonSize, _buttonSize);
            DrawNavigationButton(g, "FirstPageButton", 
                "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-double-small-left.svg", firstPageRect);
        }

        private void DrawNavigationButton(Graphics g, string buttonName, string imagePath, Rectangle buttonRect)
        {
            try
            {
                // Draw hover effect if this is the hovered button
                if (_hoveredButton == buttonName)
                {
                    using (var hoverBrush = new SolidBrush(_currentTheme.ButtonHoverBackColor))
                    {
                        g.FillRectangle(hoverBrush, buttonRect);
                    }
                }

                // Create temporary BeepButton for consistent styling
                var tempButton = new BeepButton
                {
                    ImagePath = imagePath,
                    ImageAlign = ContentAlignment.MiddleCenter,
                    HideText = true,
                    IsFrameless = true,
                    Size = buttonRect.Size,
                    IsChild = true,
                    Theme = Theme,
                    MaxImageSize = new Size(buttonRect.Width - 4, buttonRect.Height - 4)
                };

                tempButton.ApplyTheme(_currentTheme);
                tempButton.Draw(g, buttonRect);
                
                // Register the button area as a HitArea in BeepControl
                AddHitArea(
                    name: buttonName,
                    rect: buttonRect,
                    component: null,
                    hitAction: () => OnNavigationButtonClicked(buttonName)
                );
                
                tempButton.Dispose();
            }
            catch (Exception)
            {
                // Fallback: draw a simple rectangle if button creation fails
                using (var brush = new SolidBrush(_currentTheme.ButtonBackColor))
                using (var pen = new Pen(_currentTheme.ButtonBorderColor))
                {
                    g.FillRectangle(brush, buttonRect);
                    g.DrawRectangle(pen, buttonRect);
                }
                
                // Still register the hit area even in fallback case
                AddHitArea(
                    name: buttonName,
                    rect: buttonRect,
                    component: null,
                    hitAction: () => OnNavigationButtonClicked(buttonName)
                );
            }
        }
        #endregion

        #region Public Methods
        public void SetParentGrid(BeepGrid parentGrid)
        {
            _parentGrid = parentGrid;
        }

        public void UpdateNavigationState(int currentRecord, int totalRecords, int currentPage, int totalPages)
        {
            bool changed = false;

            if (_currentRecord != currentRecord)
            {
                _currentRecord = currentRecord;
                changed = true;
            }

            if (_totalRecords != totalRecords)
            {
                _totalRecords = totalRecords;
                changed = true;
            }

            if (_currentPage != currentPage)
            {
                _currentPage = currentPage;
                changed = true;
            }

            if (_totalPages != totalPages)
            {
                _totalPages = totalPages;
                changed = true;
            }

            if (changed)
            {
                NavigationChanged?.Invoke(this, new NavigationEventArgs(currentRecord, totalRecords, currentPage, totalPages));
                Invalidate();
            }
        }

        public void SimulateButtonClick(string buttonName)
        {
            OnNavigationButtonClicked(buttonName);
        }
        #endregion

        #region Theme Support
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            Invalidate();
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Unsubscribe from BeepControl events
                HitDetected -= OnHitDetected;
            }
            base.Dispose(disposing);
        }
        #endregion
    }

    #region Event Args
    public class NavigationEventArgs : EventArgs
    {
        public int CurrentRecord { get; }
        public int TotalRecords { get; }
        public int CurrentPage { get; }
        public int TotalPages { get; }

        public NavigationEventArgs(int currentRecord, int totalRecords, int currentPage, int totalPages)
        {
            CurrentRecord = currentRecord;
            TotalRecords = totalRecords;
            CurrentPage = currentPage;
            TotalPages = totalPages;
        }
    }
    #endregion
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepButtonPopList:BeepControl
    {
        
        private BeepButton _dropDownButton;
        private BeepListBox _beepListBox;
        private bool _isExpanded;

        private int _collapsedHeight = 0;
        private int _buttonWidth = 25;
        private int _maxListHeight = 200;
        private int _padding = 2;
        private int _extraspace = 6; // extra space for the dropdown button

        // If you like, define a min or max width
        private int _minWidth = 80;

        public event EventHandler SelectedIndexChanged;
        protected virtual void OnSelectedIndexChanged(EventArgs e)
            => SelectedIndexChanged?.Invoke(this, e);

        // Delegate beepListBox ListItems
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> ListItems
        {
            get => _beepListBox.ListItems;
            set
            {
                _beepListBox.ListItems = value;
                // Possibly _beepListBox.InitializeMenu();
            }
        }

        [Browsable(false)]
        public int SelectedIndex
        {
            get => _beepListBox.SelectedIndex;
            set
            {
                if (value >= 0 && value < _beepListBox.ListItems.Count)
                {
                    _beepListBox.SelectedIndex = value;
                    OnSelectedIndexChanged(EventArgs.Empty);
                }
            }
        }

        public BeepButtonPopList()
        {
            // If user did not specify size, define default
            if (Width < _minWidth) Width = 150;

            // 1) Create & add beepTextBox
         
            // 2) Compute collapsed height from beepTextBox SingleLineHeight
          

            // If no height specified or too small, set to collapsed
            if (Height < _collapsedHeight)
                Height = _collapsedHeight;

            _isExpanded = false;

            // 3) Create & add dropDownButton
            _dropDownButton = new BeepButton
            {
                IsChild = true,
                IsBorderAffectedByTheme = false,
                IsRoundedAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                HideText = true,
                ImageAlign = ContentAlignment.MiddleCenter,
                TextAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.ImageBeforeText
            };
            _dropDownButton.Click += (s, e) => ToggleMenu();
            Controls.Add(_dropDownButton);
            SetDropDownButtonImage();
            // 4) Create beepListBox
            _beepListBox = new BeepListBox
            {
                IsChild = true,
                Visible = false
            };
            _beepListBox.ItemClicked += (sender, item) =>
            {
            
                SelectedIndex = _beepListBox.ListItems.IndexOf(item);
                Collapse();
            };
          

            // Theming, etc.
            ApplyTheme();
        }

        private void ToggleMenu()
        {
            SetDropDownButtonImage();
            if (_isExpanded)
                Collapse();
            else
                Expand();
        }
        /// <summary>
        /// Sets the dropdown button's image based on the expansion state.
        /// </summary>
        private void SetDropDownButtonImage()
        {
            if (_isExpanded)
            {
                _dropDownButton.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-up.svg";
            }
            else
            {
                _dropDownButton.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-down.svg";
            }
        }
        private void Expand()
        {
            _isExpanded = true;
            int neededHeight = _beepListBox.GetMaxHeight();
            int finalListHeight = Math.Min(neededHeight, _maxListHeight);

            // total height = collapsed area + list portion + some extra padding
            int newHeight = _collapsedHeight + finalListHeight + _padding;
            Height = newHeight; // triggers OnResize => repositions beepListBox
        }

        private void Collapse()
        {
            _isExpanded = false;
            Height = _collapsedHeight; // triggers OnResize
        }

        #region Resizing Logic

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // If user tries to shrink width below _minWidth => revert
            if (Width < _minWidth)
            {
                Width = _minWidth;
            }
          
            // 2) Compute collapsed height from beepTextBox SingleLineHeight
        
            // If not expanded, forcibly revert height to collapsed
            if (!_isExpanded && Height != _collapsedHeight)
            {
                Height = _collapsedHeight;
            }

            UpdateDrawingRect();

            // The top portion is always the collapsed area
            int topSectionHeight = _collapsedHeight;

           

            // Position toggle button
            if (_dropDownButton != null)
            {
                _dropDownButton.Left = DrawingRect.Right - _buttonWidth - _padding;
                _dropDownButton.Top = DrawingRect.Top + _padding;
                _dropDownButton.Width = _buttonWidth;

                int buttonHeight = topSectionHeight - (_padding * 2) - _extraspace;
                if (buttonHeight < 1) buttonHeight = 1;
             

                _dropDownButton.Text = _isExpanded ? "▲" : "▼";
            }

            // beepListBox sits below top portion if expanded
            if (_beepListBox != null)
            {
                // show only if expanded
                _beepListBox.Visible = _isExpanded;

                if (_isExpanded)
                {
                    int listTop = DrawingRect.Top + topSectionHeight + _padding;
                    int listHeight = DrawingRect.Bottom - listTop - _padding;
                    if (listHeight < 1) listHeight = 1;

                    _beepListBox.Left = DrawingRect.Left + _padding;
                    _beepListBox.Top = listTop;
                    _beepListBox.Width = Math.Max(1, DrawingRect.Width - (_padding * 2));
                    _beepListBox.Height = listHeight;
                }
                else
                {
                    // if not expanded, beepListBox is hidden or minimal
                    _beepListBox.Visible = false;
                }
            }
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            // Enforce minWidth
            if ((specified & BoundsSpecified.Width) == BoundsSpecified.Width && width < _minWidth)
            {
                width = _minWidth;
            }

            // If not expanded, forcibly revert height to collapsed
            if (!_isExpanded && (specified & BoundsSpecified.Height) == BoundsSpecified.Height)
            {

                
            }

            base.SetBoundsCore(x, y, width, height, specified);
        }

        #endregion
     
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            //    if (_comboTextBox != null) _comboTextBox.Theme = Theme;
            if (_dropDownButton != null) _dropDownButton.Theme = Theme;
            if (_beepListBox != null) _beepListBox.Theme = Theme;
           
        }
    }

}

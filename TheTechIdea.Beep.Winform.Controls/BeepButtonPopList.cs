using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum PopListButtonShape
    {
        Triangle,
        Circle
    }

    public class BeepButtonPopList : BeepControl
    {
        public BeepButton TriangleButton { get; set; }   // the existing "triangle" style beep button
        public BeepCircularButton CircularButton { get; set; }// a beep "circular" style button

        private BeepPopupForm _popupForm;
        private BeepListBox _beepListBox;
        private bool _isPopupOpen;

        private int _buttonWidth = 15;
        private int _maxListHeight = 100;
        private int _maxListWidth = 100;
        private int _minWidth = 15;
        private int _minheight = 15;

        private TextImageRelation textImageRelation = TextImageRelation.ImageBeforeText;
        private ContentAlignment imageAlign = ContentAlignment.MiddleLeft;
        private ContentAlignment textAlign = ContentAlignment.MiddleCenter;

        // Track which shape to use. The default is Triangle.
        private PopListButtonShape _buttonShape = PopListButtonShape.Triangle;
        private bool _applyThemeOnImage = false;
        // An event to notify about selection
        public event EventHandler SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(EventArgs e)
            => SelectedItemChanged?.Invoke(this, e);

        private Size _maxImageSize = new Size(15, 15); // Default max image size
        [Browsable(true)]
        [Category("Appearance")]
        public Size MaxImageSize
        {
            get => _maxImageSize;
            set
            {
                _maxImageSize = value;
                Invalidate(); // Repaint when the max image size changes
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public TextImageRelation TextImageRelation
        {
            get => textImageRelation;
            set
            {
                textImageRelation = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public ContentAlignment ImageAlign
        {
            get => imageAlign;
            set
            {
                imageAlign = value;
                Invalidate();
            }
        }
        private bool _hideText = false;
        [Browsable(true)]
        [Category("Behavior")]
        public bool HideText
        {
            get => _hideText;
            set
            {
                _hideText = value;
                Invalidate(); // Trigger repaint when the state changes
            }
        }
        // The beepListBox's items
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> ListItems
        {
            get => _beepListBox.ListItems;
            set => _beepListBox.ListItems = value;
        }
        // The item currently chosen by the user
        private SimpleItem _selectedItem;
        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            private set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnSelectedItemChanged(EventArgs.Empty);
                }
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
                    SelectedItem = _beepListBox.ListItems[value];
                }
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Whether to apply the theme on the button's image (e.g. re-color an SVG).")]
        public bool ApplyThemeOnImage
        {
            get => _applyThemeOnImage;
            set
            {
                _applyThemeOnImage = value;
                // If you want to reapply theme to the button's image:
                if (TriangleButton != null)
                    TriangleButton.ApplyThemeOnImage = value;
                if (CircularButton != null)
                    CircularButton.ApplyThemeOnImage = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Select the image file (SVG, PNG, JPG, etc.) to load.")]
        public string ImagePath
        {
            get
            {
                // Return whichever button is visible
                if (_buttonShape == PopListButtonShape.Triangle && TriangleButton != null)
                    return TriangleButton.ImagePath ?? string.Empty;
                if (_buttonShape == PopListButtonShape.Circle && CircularButton != null)
                    return CircularButton.ImagePath ?? string.Empty;

                return string.Empty;
            }
            set
            {
                if (_buttonShape == PopListButtonShape.Triangle && TriangleButton != null)
                {
                    TriangleButton.ImagePath = value;
                    if (ApplyThemeOnImage)
                    {
                        TriangleButton.Theme = Theme;
                        TriangleButton.ApplyThemeToSvg();
                        TriangleButton.ApplyTheme();
                    }
                }
                else if (_buttonShape == PopListButtonShape.Circle && CircularButton != null)
                {
                    CircularButton.ImagePath = value;
                    if (ApplyThemeOnImage)
                    {
                        CircularButton.Theme = Theme;
                        CircularButton.ApplyTheme();
                    }
                }

                Invalidate(); // Repaint to reflect image changes
            }
        }
        // ---------- NEW PROPERTY -----------
        // Switch between triangle or circle shaped toggle button
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Choose which shape of toggle button to use.")]
        public PopListButtonShape ButtonShape
        {
            get => _buttonShape;
            set
            {
                if (_buttonShape != value)
                {
                    _buttonShape = value;
                    // Recreate or switch the toggle button
                    CreateOrSwitchButton();
                }
            }
        }
        // -----------------------------------
        public BeepButtonPopList()
        {
            if (Width < _minWidth)
                Width = 150;
            if (Height < 30)
                Height = 30;

            // 1) Create beepListBox
            _beepListBox = new BeepListBox
            {
                TitleText = "Select an item",
                ShowTitle = false,
                ShowTitleLine = false,
                Width = _maxListWidth,
                Height = _maxListHeight,
                ShowAllBorders = false,
                IsBorderAffectedByTheme = false,
                IsRoundedAffectedByTheme = false,
                IsShadowAffectedByTheme= false,
               
            };
            _beepListBox.ItemClicked += (sender, item) =>
            {
                SelectedItem = item;
                ClosePopup();
            };
            IsChild = true;
            // 2) Create a borderless popup form
            _popupForm = new BeepPopupForm();
            
            _popupForm.Controls.Add(_beepListBox);
            _beepListBox.ShowHilightBox = false;
            _beepListBox.Dock = DockStyle.None;
            //   _popupForm.Deactivate += (s, e) => ClosePopup();

            // 3) Create or switch the toggle button (triangle vs. circle)
            CreateOrSwitchButton();

            ApplyTheme();
        }

        // This method checks which shape we want, and ensures only that button is active/visible.
        private void CreateOrSwitchButton()
        {
            // If we already had a button, remove it from Controls
            if (TriangleButton != null && Controls.Contains(TriangleButton))
                Controls.Remove(TriangleButton);
            if (CircularButton != null && Controls.Contains(CircularButton))
                Controls.Remove(CircularButton);

            if (_buttonShape == PopListButtonShape.Triangle)
            {
                // Create the "triangle" style beep button if necessary
                if (TriangleButton == null)
                {
                    TriangleButton = new BeepButton
                    {
                        IsChild = true,
                        Text = "", // or "▼"
                        HideText = true,
                        ImageAlign = ContentAlignment.MiddleCenter,
                        TextAlign = ContentAlignment.MiddleCenter,
                        TextImageRelation = TextImageRelation.ImageBeforeText,
                        Dock = DockStyle.Fill,
                        Width = _buttonWidth,
                        ShowAllBorders = false,
                        IsBorderAffectedByTheme = false,
                        IsRoundedAffectedByTheme = false,
                        IsShadowAffectedByTheme = false


                    };
                    TriangleButton.Click += (s, e) => TogglePopup();
                }
                Controls.Add(TriangleButton);
                TriangleButton.BringToFront();
                if (CircularButton != null)
                    CircularButton.Visible = false;
            }
            else
            {
                // Create or show the "circular" beep button
                if (CircularButton == null)
                {
                    CircularButton = new BeepCircularButton
                    {
                        IsChild = true,
                        Text = "",
                        Dock = DockStyle.Fill,
                        Width = _buttonWidth,HideText= true,
                        ShowAllBorders = false,
                        IsBorderAffectedByTheme = false,
                        IsRoundedAffectedByTheme = false,
                        IsShadowAffectedByTheme = false
                    };
                    CircularButton.Click += (s, e) => TogglePopup();
                }
                Controls.Add(CircularButton);
                CircularButton.BringToFront();
                if (TriangleButton != null)
                    TriangleButton.Visible = false;
            }

            Invalidate();
        }

        private void TogglePopup()
        {
            if (_isPopupOpen)
                ClosePopup();
            else
                ShowPopup();
        }
        private void ShowPopup()
        {
            if (_isPopupOpen) return;
            _isPopupOpen = true;

            // Rebuild beepListBox's layout
            _beepListBox.InitializeMenu();

            int neededHeight = _beepListBox.GetMaxHeight()+5;
            int finalHeight = Math.Min(neededHeight, _maxListHeight);
            // possibly also compute width
            int finalWidth = Math.Max(Width, _maxListWidth);

            // The popup form is sized to fit beepListBox
            _popupForm.Size = new Size(finalWidth, neededHeight);
            // Position popup just below the main control
            var screenPoint = this.PointToScreen(new Point(0, Height));
            _popupForm.Location = screenPoint;
            _beepListBox.Theme = Theme;
            _beepListBox.ShowAllBorders=false; 
            //_popupForm.BackColor = _currentTheme.BackColor;
            _popupForm.Theme = Theme;
            _beepListBox.Dock = DockStyle.Fill; // Manually size and position
            _popupForm.BorderThickness = 2;
          
            _popupForm.Show();
            _popupForm.BringToFront();
            _popupForm.Invalidate();
        }
        private void ClosePopup()
        {
            if (!_isPopupOpen) return;
            _isPopupOpen = false;
            _popupForm.Hide();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (Width < _minWidth)
                Width = _minWidth;
            if (Height < _minheight)
                Height = _minheight;
            resizeButton();
        }
        private void resizeButton()
        {
            if (_buttonShape == PopListButtonShape.Triangle)
            {
                if (TriangleButton != null)
                {
                    TriangleButton.Width = Width;
                    TriangleButton.Height = Height;
                    TriangleButton.MaxImageSize = new Size(Width - 1, Height - 1);
                }
            }
            else
            {
                if (CircularButton != null)
                {
                    CircularButton.Width = Width;
                    CircularButton.Height = Height;
                    
                }
            }
        }
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if(TriangleButton== null) return;

            // Apply the theme to whichever button is in use
            if (TriangleButton != null) { TriangleButton.Theme = Theme; TriangleButton.IsChild = true; }
              
           
            if (CircularButton != null)
            {
                CircularButton.Theme = Theme;
                CircularButton.IsChild = true;
            }
               
            if (_beepListBox != null)
                _beepListBox.Theme = Theme;
            BackColor = _currentTheme.BackColor ;
        }
    }
}

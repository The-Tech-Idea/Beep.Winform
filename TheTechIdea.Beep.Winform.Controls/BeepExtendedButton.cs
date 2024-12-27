using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepExtendedButton:BeepControl
    {
        private BeepButton extendButton;
        private BeepButton button;
        Panel highlightPanel;
        private int buttonWidth = 200;
        
        private int buttonHeight = 30;
        private int starty = 2;
        private int startx = 2;

        private Size _imagesize = new Size(20, 20);
        bool _applyThemeOnImage = false;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Change Size of main Button inside control")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ButtonWidth
        {
            get => button.Width;
            set
            {
                button.Width = value;
                UpdateDrawingRect();
                _isControlinvalidated = true;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Change Size of  images inside control")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ImageSize
        {
            get => _imagesize.Width;
            set
            {
                _imagesize.Width = value;
                _imagesize.Height = value;
                button.MaxImageSize = _imagesize;
                extendButton.MaxImageSize = _imagesize;
                _isControlinvalidated = true;
                Invalidate();
            }
        }
       
        public bool ApplyThemeOnImage
        {
            get => _applyThemeOnImage;
            set
            {
                _applyThemeOnImage = value;
                button.ApplyThemeOnImage = value;
                if (value)
                {

                    if (ApplyThemeOnImage)
                    {
                        button.Theme = Theme;

                        button.ApplyThemeToSvg();

                    }
                }
                _isControlinvalidated = true;
                Invalidate();
            }
        }
        private string _buttonImagePath;
        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Select the image file (SVG, PNG, JPG, etc.) to load.")]
        public string ImagePath
        {
            get => _buttonImagePath;
            set
            {
                _buttonImagePath = value;
                if (button != null)
                {
                    button.ImagePath = value;
                    if (ApplyThemeOnImage)
                    {
                        button.Theme = Theme;
                        button.ApplyThemeToSvg();
                        button.ApplyTheme();
                    }
                    _isControlinvalidated = true;
                    Invalidate(); // Repaint when the image changes
                                  // UpdateSize();
                  //  _isControlinvalidated = true;
                }
            }
        }
        private string _extendedbuttonImagePath;
        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Select the image file (SVG, PNG, JPG, etc.) to load.")]
        public string ExtendButtonImagePath
        {
            get => _extendedbuttonImagePath;
            set
            {
                _extendedbuttonImagePath=value;
                if (extendButton != null)
                {
                 
                    extendButton.ImagePath = value;
                    if (ApplyThemeOnImage)
                    {
                        extendButton.Theme = Theme;
                        extendButton.ApplyThemeToSvg();
                        extendButton.ApplyTheme();
                    }
                    _isControlinvalidated = true;
                    Invalidate(); // Repaint when the image changes
                                  // UpdateSize();
                  //  _isControlinvalidated = true;
                }
            }
        }
        #region Events
        public event EventHandler<BeepEventDataArgs> ButtonClick;
        public event EventHandler<BeepEventDataArgs> ExtendButtonClick;
        #endregion
        private SimpleItem _menuItem;
        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public SimpleItem MenuItem
        {
            get => _menuItem;
            set
            {
                _menuItem = value;
                if (button != null && value!=null)
                {
                    if (value.Text != null) { button.Text = value.Text; }
                    
                    if (!string.IsNullOrEmpty(value.ImagePath))
                    {
                        button.ImagePath = value.ImagePath;
                    }
                    
                }
                _isControlinvalidated= true;
                Invalidate();
            }
        }
        public BeepExtendedButton()
        {
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 100;
                Height = buttonHeight;
            }
            CreateMenuItemPanel();
            BoundProperty= "Text";
        }
        protected override void InitLayout()
        {
            Console.WriteLine("Control InitLayout");
            base.InitLayout();
            UpdateDrawingRect();
            CreateMenuItemPanel();
            _isControlinvalidated = true;
            Invalidate();
        }
        private void CreateMenuItemPanel()
        {
            

            // Clear existing controls before re-creating the layout
            Controls.Clear();
            UpdateDrawingRect();
            // DrawingRect.Inflate(-2, -10);
            buttonHeight = DrawingRect.Height - 2;
            // Main button setup
            button = new BeepButton
            {
                Size = new Size(DrawingRect.Width - ImageSize -  4, buttonHeight),
                Location = new Point(startx, starty),
                
                MaxImageSize = new Size(ImageSize, ImageSize),
                Text = this.Text,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                TextAlign = ContentAlignment.MiddleCenter,
                ImageAlign = ContentAlignment.MiddleLeft,
                IsBorderAffectedByTheme = false,
                Theme = Theme,
                ShowAllBorders = false,
                ShowShadow = false,
                BorderSize = 0,
                IsRounded = false,
                ApplyThemeOnImage = ApplyThemeOnImage
            };

            if (MenuItem != null)
            {
                button.Text = MenuItem.Text;
                if (!string.IsNullOrEmpty(MenuItem.ImagePath))
                {
                    button.ImagePath = MenuItem.ImagePath;
                }
            }

            // Extend button setup
            extendButton = new BeepButton
            {
                HideText = true,
                Size = new Size(ImageSize + 4, buttonHeight),
                Location = new Point(button.Right + 1 , starty),
               
                MaxImageSize = new Size(ImageSize, ImageSize),
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleCenter,
                Theme = Theme,
                ShowAllBorders = false,
                ShowShadow = false,
                IsRounded = false,
                IsBorderAffectedByTheme = false,
                BorderSize = 0,
                ApplyThemeOnImage = ApplyThemeOnImage
            };

            if (!string.IsNullOrEmpty(_buttonImagePath))
            {
                button.ImagePath = _buttonImagePath;
            }

            if (!string.IsNullOrEmpty(_extendedbuttonImagePath))
            {
                extendButton.ImagePath = _extendedbuttonImagePath;
            }

            // Add highlight panel, main button, and extend button to the panel
            Controls.Add(highlightPanel);
            Controls.Add(button);
            Controls.Add(extendButton);

            // Event handlers
            //button.MouseEnter += (s, e) => { highlightPanel.BackColor = _currentTheme.ButtonHoverBackColor; };
            //button.MouseLeave += (s, e) => { highlightPanel.BackColor = _currentTheme.BackColor; };
            //button.MouseHover += (s, e) => { highlightPanel.BackColor = _currentTheme.ButtonHoverBackColor; };
            button.Click += MenuItemButton_Click;
            extendButton.Click += ExtendButton_Click;

            // Add the panel to the control
           

            _isControlinvalidated = false;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
           // Console.WriteLine("Control Invalidated 1 ");
            if (_isControlinvalidated)
            {
               // Console.WriteLine("Control Invalidated 2");
                Controls.Clear();
                CreateMenuItemPanel();
                _isControlinvalidated = false;
            }

        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateDrawingRect();
            
            if (button != null && extendButton != null)
            {
                button.Size = new Size(DrawingRect.Width - ImageSize -  4, buttonHeight);
                button.Location = new Point(startx, starty);
                extendButton.Size = new Size(ImageSize + 4, buttonHeight);
                extendButton.Location = new Point(button.Right , starty);
            }

            _isControlinvalidated = true;
            Invalidate();
        }
       
        private void ExtendButton_Click(object? sender, EventArgs e)
        {
            ExtendButtonClick?.Invoke(this, new BeepEventDataArgs("ExentededButtonClick", MenuItem));
        }

        private void MenuItemButton_Click(object? sender, EventArgs e)
        {
            ButtonClick?.Invoke(this, new BeepEventDataArgs("ButtonClick", MenuItem));

        }
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls.Template;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepExtendedButton:BeepControl
    {
        private BeepButton extendButton;
        private BeepButton button;
        private int buttonWidth = 200;
        
        private int buttonHeight = 30;

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
        private SimpleMenuItem _menuItem;
        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public SimpleMenuItem MenuItem
        {
            get => _menuItem;
            set
            {
                _menuItem = value;
                if (button != null)
                {
                    button.Text = value.Text;
                    if (!string.IsNullOrEmpty(value.Image))
                    {
                        button.ImagePath = value.Image;
                    }
                    
                }
                _isControlinvalidated= true;
                Invalidate();
            }
        }
        public BeepExtendedButton()
        {
            CreateMenuItemPanel();
            //Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            Console.WriteLine("Control Created");   
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
            UpdateDrawingRect();
           // Controls.Clear();
            var menuItemPanel = new Panel
            {
                Height = DrawingRect.Height,
                Visible = true,
                Dock = DockStyle.Fill,
                Location = new System.Drawing.Point(DrawingRect.X, DrawingRect.Y),
                Size = new System.Drawing.Size(DrawingRect.Width, DrawingRect.Height),
               Anchor= AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom
            };

            // Create the left-side highlight panel
            Panel highlightPanel = new Panel
            {
                Width = 5,
                Location = new System.Drawing.Point(0, 0),
                BackColor = _currentTheme.PanelBackColor,
                Visible = false,
                Size = new System.Drawing.Size(5, DrawingRect.Height),
                 Dock= DockStyle.Left
            };

            // Initialize BeepButton for icon and text
            button = new BeepButton
            {
                Location = new System.Drawing.Point(7, 0),
                Text = this.Text,
                Size = new System.Drawing.Size(DrawingRect.Width- ImageSize-10, ImageSize),
                MaxImageSize = new Size(ImageSize, ImageSize),
                TextImageRelation = TextImageRelation.ImageBeforeText,
                TextAlign = ContentAlignment.MiddleCenter,
                ImageAlign = ContentAlignment.MiddleLeft,
                Theme = Theme,
                IsChild = true,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                BorderSize = 0,
                ApplyThemeOnImage = false,
                Dock= DockStyle.Fill
            };
            if (MenuItem != null)
            {
                button.Text = MenuItem.Text;
                if (!string.IsNullOrEmpty(MenuItem.Image))
                {
                    button.ImagePath = MenuItem.Image;
                }
                
            }
           extendButton = new BeepButton
            {
               
                HideText = true,
                 Location = new System.Drawing.Point(DrawingRect.Width - ImageSize-2, 0),
                Size = new System.Drawing.Size(ImageSize, ImageSize),
                MaxImageSize = new Size(ImageSize, ImageSize),
                TextImageRelation = TextImageRelation.Overlay,
                ImageAlign = ContentAlignment.MiddleCenter,
                Theme = Theme,
                IsChild = true,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                BorderSize = 0,
                ApplyThemeOnImage = false,
               Dock = DockStyle.Right   
           };
            if(_buttonImagePath != null)
            {
                button.ImagePath = _buttonImagePath;
            }
            if (_extendedbuttonImagePath != null)
            {
                extendButton.ImagePath = _extendedbuttonImagePath;
            }
            // Load the icon if specified

            if (_currentTheme != null)
            {
                button.Theme = Theme;
                extendButton.Theme = Theme;
                BackColor = _currentTheme.BackColor;

            }
            // Add BeepButton and highlight panel to the panel
            menuItemPanel.Controls.Add(highlightPanel);
            menuItemPanel.Controls.Add(button);
            menuItemPanel.Controls.Add(extendButton);
          
            //Handle hover effects for the menu item panel

            menuItemPanel.MouseEnter += (s, e) =>
            {
                menuItemPanel.BackColor = _currentTheme.SelectedRowBackColor;
                highlightPanel.Visible = true;
            };
            menuItemPanel.MouseLeave += (s, e) =>
            {
                menuItemPanel.BackColor = _currentTheme.BackColor;
                highlightPanel.Visible = false;
            };

            // Handle button events
            button.MouseEnter += (s, e) =>
            {
                menuItemPanel.BackColor = _currentTheme.SelectedRowBackColor;
                highlightPanel.Visible = true;
            };
            button.MouseLeave += (s, e) =>
            {
                menuItemPanel.BackColor = _currentTheme.BackColor;
                highlightPanel.Visible = false;
            };
            button.Click += MenuItemButton_Click;
            extendButton.Click += ExtendButton_Click;
            Controls.Add(menuItemPanel);
            
            _isControlinvalidated = false;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Console.WriteLine("Control Invalidated 1 ");
            if (_isControlinvalidated)
            {
                Console.WriteLine("Control Invalidated 2");
                Controls.Clear();
                CreateMenuItemPanel();
                _isControlinvalidated = false;
            }

        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _isControlinvalidated = true;
            //if (button != null)
            //{
            //    button.Width = Width - ImageSize;
            //    extendButton.Location = new Point(Width - ImageSize, 0);
            //}
        }
        protected void RearrangeControls()
        {
            if (button != null)
            {
                button.Width = Width - ImageSize;
                extendButton.Location = new Point(Width - ImageSize, 0);
            }
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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Template;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum TextLocation
    {
        Above,
        Right,
        Left,
        Below,
        Inside
    }

    [ToolboxItem(true)]
    [Category("Beep Controls")]
    public class BeepCircularButton : BeepControl
    {
        private BeepImage beepImage;
        private int _borderThickness = 2;
        private bool _isHovered = false;
        private TextLocation _textLocation = TextLocation.Below;
        private ContentAlignment _textAlign = ContentAlignment.MiddleCenter;
        private bool _showBorder = true;
        private string _text;
        private Bitmap _backgroundSnapshot;
        private bool _backgroundchanged=true;
        private Color _forcolor=Color.Black ;
        private bool _isForColorSet = false;
        private bool _hidetext = false;
        private Bitmap _back ;
        bool _applyThemeOnImage = false;
        public bool ApplyThemeOnImage
        {
            get => _applyThemeOnImage;
            set
            {
                _applyThemeOnImage = value;
                if (value)
                {
                  
                    if (ApplyThemeOnImage)
                    {
                        beepImage.ApplyThemeOnImage = true;
                        beepImage.Theme = Theme;

                    }
                }
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool HideText
        {
            get => _hidetext;
            set
            {
                _hidetext = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsForColorSet
        {
            get => _isForColorSet;
            set
            {
                _isForColorSet = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public   Color ForColor
        {
            get => _forcolor;
            set
            {
                _forcolor = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        public new string Text
        {
            get => _text;
            set { _text = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public TextLocation TextLocation
        {
            get => _textLocation;
            set
            {
                _textLocation = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public ContentAlignment TextAlign
        {
            get => _textAlign;
            set
            {
                _textAlign = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Category("Appearance")]
        public string ImagePath
        {
            get => beepImage.ImagePath;
            set
            {
                beepImage.ImagePath = value;
                beepImage.ApplyTheme();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public bool ShowBorder
        {
            get => _showBorder;
            set
            {
                _showBorder = value;
                Invalidate();
            }
        }

        public BeepCircularButton()
        {

         
            beepImage = new BeepImage
            {
                Dock = DockStyle.None,
                Margin = new Padding(0),
            };
            //BorderStyle = BorderStyle.None;
            //BorderRadius = 0;
            //_showAllBorders = false;
            //_showShadow = false;
            _text = "BeepCircularButton";
            ApplyTheme();
            //beepImage.MouseHover += BeepImage_MouseHover;
            //beepImage.MouseLeave += BeepImage_MouseLeave;
            //beepImage.Click += BeepImage_Click;

        }

      

        protected override void OnPaint(PaintEventArgs pevent)
        {
            _isframless = true;

            base.OnPaint(pevent);

            // Calculate text rectangle first to adjust the circle bounds accordingly
            Rectangle textRect = GetTextRectangle();
            Rectangle circleBounds = GetCircleBounds(textRect);
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Calculate the circle bounds based on control size
            int diameter = Math.Min(circleBounds.Width, circleBounds.Height);



            //if (_showBorder)
            //{
            //    using (Pen pen = new Pen(_currentTheme.BorderColor, _borderThickness))
            //    {
            //        pevent.Graphics.DrawEllipse(pen, circleBounds);
            //    }
            //}
            if (IsHovered)
            {
                using (Pen pen = new Pen(_currentTheme.ButtonHoverForeColor, _borderThickness))
                {
                    pevent.Graphics.DrawEllipse(pen, circleBounds);
                }
            }
            //if(IsPressed)
            //{
            //    using (Pen pen = new Pen(_currentTheme.ButtonActiveBackColor, _borderThickness))
            //    {
            //        pevent.Graphics.DrawEllipse(pen, circleBounds);
            //    }
            //}
            using (Brush brush = new SolidBrush( _currentTheme.ButtonBackColor ))
            {
                pevent.Graphics.FillEllipse(brush, circleBounds);
            }
            // Position and set the maximum size for beepImage to fit inside the circle
            if (!string.IsNullOrEmpty(beepImage.ImagePath))
            {
                beepImage.MaximumSize = GetInscribedSquareSize( diameter );  // Constrain to circle diameter
                beepImage.Size = beepImage.MaximumSize;  // Apply the size directly to beepImage
                beepImage.Location = new Point(
                    circleBounds.X + (circleBounds.Width - beepImage.Width) / 2,
                    circleBounds.Y + (circleBounds.Height - beepImage.Height) / 2
                );

                // Render the image within the circular area
                beepImage.DrawImage(pevent.Graphics, new Rectangle(beepImage.Location, beepImage.Size));
            }
            if (!string.IsNullOrEmpty(Text) && !HideText)
            {
                if (IsForColorSet) {
                    using (Brush textBrush = new SolidBrush(ForColor))
                    {
                        TextRenderer.DrawText(pevent.Graphics, Text, Font, textRect, ForColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    }
                }
                else
                {
                    using (Brush textBrush = new SolidBrush(_currentTheme.ButtonForeColor))
                    {
                        TextRenderer.DrawText(pevent.Graphics, Text, Font, textRect, _currentTheme.ButtonForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    }
                }
               
            }
        }
        public Size GetInscribedSquareSize(int circleDiameter)
        {
            // The side length of the largest square that fits within a circle is the circle's diameter divided by √2.
            int squareSideLength = (int)(circleDiameter / Math.Sqrt(2));

            return new Size(squareSideLength, squareSideLength);
        }
        private Rectangle GetCircleBounds(Rectangle textRect)
        {
            // Calculate maximum diameter based on the available space within DrawingRect minus padding
            int maxDiameter = Math.Min(
                DrawingRect.Width - Padding.Horizontal,
                DrawingRect.Height - Padding.Vertical
            );

            // Adjust diameter further if text is displayed above or below the circle
            int diameter = (!HideText && (_textLocation == TextLocation.Above || _textLocation == TextLocation.Below))
                ? Math.Max(0, maxDiameter - textRect.Height)
                : maxDiameter;

            // Calculate the centered X and Y positions for the circle, considering padding
            int x = DrawingRect.X + Padding.Left + (DrawingRect.Width - diameter) / 2;
            int y = DrawingRect.Y + Padding.Top + (DrawingRect.Height - diameter) / 2;

            // Adjust Y position based on text location if text is displayed
            if (!HideText)
            {
                y += _textLocation switch
                {
                    TextLocation.Below => -textRect.Height / 2, // Adjust up if text is below
                    TextLocation.Above => textRect.Height / 2,  // Adjust down if text is above
                    _ => 0 // No adjustment for other locations
                };
            }

            // Ensure diameter is non-negative and return the circle bounds
            return new Rectangle(x, y, Math.Max(0, diameter), Math.Max(0, diameter));
        }
        private Rectangle GetImageRectangle(Rectangle circleBounds)
        {
            // Define the maximum size as the circle's diameter
            int diameter = Math.Min(circleBounds.Width, circleBounds.Height);
            beepImage.MaximumSize = new Size(diameter, diameter);

            // Get the original image size
            Size originalSize = beepImage.GetImageSize();
            if (originalSize.IsEmpty) return Rectangle.Empty;

            // Calculate the scale factor to fit within MaximumSize, keeping aspect ratio
            float scale = Math.Min((float)beepImage.MaximumSize.Width / originalSize.Width,
                                   (float)beepImage.MaximumSize.Height / originalSize.Height);

            // Apply the scale factor to the image dimensions
            int scaledWidth = (int)(originalSize.Width * scale);
            int scaledHeight = (int)(originalSize.Height * scale);

            // Center the image within the circle bounds
            int x = circleBounds.X + (circleBounds.Width - scaledWidth) / 2;
            int y = circleBounds.Y + (circleBounds.Height - scaledHeight) / 2;

            return new Rectangle(x, y, scaledWidth, scaledHeight);
        }
        private Rectangle GetTextRectangle()
        {
            if (HideText) return Rectangle.Empty; // Return an empty rectangle if text is hidden

            Size textSize = TextRenderer.MeasureText(Text, Font);
            Rectangle textRect = new Rectangle();

            switch (_textLocation)
            {
                case TextLocation.Above:
                    textRect = new Rectangle(
                        (Width - textSize.Width) / 2,
                        2,
                        textSize.Width,
                        textSize.Height
                    );
                    break;
                case TextLocation.Below:
                    textRect = new Rectangle(
                        (Width - textSize.Width) / 2,
                        Height - textSize.Height - 2,
                        textSize.Width,
                        textSize.Height
                    );
                    break;
                case TextLocation.Left:
                    textRect = new Rectangle(
                        2,
                        (Height - textSize.Height) / 2,
                        textSize.Width,
                        textSize.Height
                    );
                    break;
                case TextLocation.Right:
                    textRect = new Rectangle(
                        Width - textSize.Width - 2,
                        (Height - textSize.Height) / 2,
                        textSize.Width,
                        textSize.Height
                    );
                    break;
                case TextLocation.Inside:
                    textRect = new Rectangle(
                        (Width - textSize.Width) / 2,
                        (Height - textSize.Height) / 2,
                        textSize.Width,
                        textSize.Height
                    );
                    break;
            }
            return textRect;
        }
        public override void ApplyTheme()
        {
            //base.ApplyTheme();
            Font=BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
            ForeColor = _currentTheme.ButtonForeColor;
           
            if (ApplyThemeOnImage)
            {
                beepImage.ApplyThemeOnImage = true;
                beepImage.Theme = Theme;
                
            }

        }
        //protected override void OnMouseHover(EventArgs e)
        //{
        //    base.OnMouseHover(e);
        //    IsHovered = true;
        //}
        //protected override void OnClick(EventArgs e)
        //{
        //    base.OnClick(e);
        //    IsPressed = true;
        //}
        //protected override void OnMouseLeave(EventArgs e)
        //{
        //    base.OnMouseLeave(e);
        //    IsHovered = false;
        //}
        //private void BeepImage_Click(object? sender, EventArgs e)
        //{
        //    base.OnClick(e);
        //    IsPressed = true;
        //}

        //private void BeepImage_MouseLeave(object? sender, EventArgs e)
        //{
        //    base.OnMouseLeave(e);
        //    IsHovered = false;
        //}

        //private void BeepImage_MouseHover(object? sender, EventArgs e)
        //{
        //    base.OnMouseHover(e);
        //    IsHovered = true;
        //}
    }
}

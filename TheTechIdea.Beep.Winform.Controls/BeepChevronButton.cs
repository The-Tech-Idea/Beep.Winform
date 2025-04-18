using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Chevron Button")]
    [Description("A chevron-shaped button control with text inside, used in breadcrumb steppers.")]
    public class BeepChevronButton : BeepControl
    {
        private string _imagePath;
        private BeepImage beepImage;
        private Font _textFont = new Font("Arial", 10);

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        public Font TextFont
        {
            get => _textFont;
            set
            {
                _textFont = value;
                Font = _textFont;
                UseThemeFont = false;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Path to the image displayed when checked.")]
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                if (beepImage != null)
                {
                    beepImage.ImagePath = value;
                    Invalidate();
                }
            }
        }

        public BeepChevronButton() : base()
        {
            if (Width <= 0 || Height <= 0)
            {
                Width = 100;
                Height = 50;
            }
            beepImage = new BeepImage
            {
                Dock = DockStyle.None,
                Margin = new Padding(0)
            };
            IsChild = true;
            IsShadowAffectedByTheme = false;
            IsRoundedAffectedByTheme = false;
            IsCustomeBorder = true;
            ApplyTheme();
            beepImage.MouseHover += (s, e) => { IsHovered = true; Invalidate(); };
            beepImage.MouseLeave += (s, e) => { IsHovered = false; Invalidate(); };
            beepImage.Click += (s, e) => OnClick(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            base.OnPaint(e);
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            UpdateDrawingRect();
            Draw(g, DrawingRect);
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Define chevron shape
            int arrowWidth = Height / 2; // Width of the arrow part
            Point[] points = new Point[]
            {
new Point(rectangle.Left, rectangle.Top),
new Point(rectangle.Right - arrowWidth, rectangle.Top),
new Point(rectangle.Right, rectangle.Top + rectangle.Height / 2),
new Point(rectangle.Right - arrowWidth, rectangle.Bottom),
new Point(rectangle.Left, rectangle.Bottom)
            };

            using (Brush brush = new SolidBrush(IsHovered ? _currentTheme.ButtonHoverBackColor : _currentTheme.ButtonBackColor))
            {
                graphics.FillPolygon(brush, points);
            }

            using (Pen pen = new Pen(_currentTheme.ShadowColor, _borderThickness))
            {
                graphics.DrawPolygon(pen, points);
            }

            // Draw text centered in the chevron
            if (!string.IsNullOrEmpty(Text))
            {
                TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak;
                Rectangle textRect = new Rectangle(rectangle.Left + 5, rectangle.Top, rectangle.Width - arrowWidth - 10, rectangle.Height);
                TextRenderer.DrawText(graphics, Text, Font, textRect, _currentTheme.PrimaryTextColor, flags);
            }

            // Draw image if present
            if (!string.IsNullOrEmpty(ImagePath))
            {
                int imageSize = Math.Min(rectangle.Height - 10, 16);
                beepImage.MaximumSize = new Size(imageSize, imageSize);
                beepImage.Size = beepImage.MaximumSize;
                beepImage.Location = new Point(
                rectangle.Left + 5,
                rectangle.Top + (rectangle.Height - imageSize) / 2
                );
                beepImage.DrawImage(graphics, new Rectangle(beepImage.Location, beepImage.Size));
            }
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            BackColor = _currentTheme.ButtonBackColor;
            ForeColor = _currentTheme.ButtonForeColor;
            HoverBackColor = _currentTheme.ButtonHoverBackColor;
            HoverForeColor = _currentTheme.ButtonHoverForeColor;
            DisabledBackColor = _currentTheme.DisabledBackColor;
            DisabledForeColor = _currentTheme.DisabledForeColor;
            FocusBackColor = _currentTheme.ButtonSelectedBackColor;
            FocusForeColor = _currentTheme.ButtonSelectedForeColor;
            PressedBackColor = _currentTheme.ButtonPressedBackColor;
            PressedForeColor = _currentTheme.ButtonPressedForeColor;

            if (UseThemeFont)
            {
                _textFont = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
                Font = _textFont;
            }
            beepImage.Theme = Theme;
            Invalidate();
        }
    }
}




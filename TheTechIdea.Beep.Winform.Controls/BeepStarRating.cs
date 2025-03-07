
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Star Rating")]
    [Category("Beep Controls")]
    [Description("A control that allows users to rate items using stars.")]
    public class BeepStarRating : BeepControl
    {
        #region Fields and Properties
        private int _starCount = 5;
        private int _selectedRating = 0;
        private int _starSize = 24; // Default size of stars
        private int _spacing = 5;  // Default spacing between stars
        private Color _filledStarColor = Color.Gold;
        private Color _emptyStarColor = Color.Gray;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Number of stars in the rating control.")]
        public int StarCount
        {
            get => _starCount;
            set
            {
                if (value > 0)
                {
                    _starCount = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Currently selected rating.")]
        public int SelectedRating
        {
            get => _selectedRating;
            set
            {
                if (value >= 0 && value <= _starCount)
                {
                    _selectedRating = value;
                    Invalidate();
                    RatingChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Value of each star.")]
        public int StarSize
        {
            get => _starSize;
            set
            {
                if (value > 0)
                {
                    _starSize = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Spacing between stars.")]
        public int Spacing
        {
            get => _spacing;
            set
            {
                if (value >= 0)
                {
                    _spacing = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color of the filled stars.")]
        public Color FilledStarColor
        {
            get => _filledStarColor;
            set
            {
                _filledStarColor = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color of the empty stars.")]
        public Color EmptyStarColor
        {
            get => _emptyStarColor;
            set
            {
                _emptyStarColor = value;
                Invalidate();
            }
        }
        private Color _starBorderColor = Color.Black;
        private float _starBorderThickness = 1f;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color of the star border.")]
        public Color StarBorderColor
        {
            get => _starBorderColor;
            set
            {
                _starBorderColor = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Thickness of the star border.")]
        public float StarBorderThickness
        {
            get => _starBorderThickness;
            set
            {
                if (value > 0)
                {
                    _starBorderThickness = value;
                    Invalidate();
                }
            }
        }

        public event EventHandler RatingChanged;

        #endregion

        #region Constructor
        public BeepStarRating()
        {
            DoubleBuffered = true;
            MinimumSize = new Size(120, 30); // Enforce a reasonable minimum size
            Cursor = Cursors.Hand;
            BoundProperty = "SelectedRating";
        }
        #endregion

        #region Painting
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            UpdateDrawingRect();
            Draw(e.Graphics, DrawingRect);

        }
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Ensure DrawingRect is valid
            if (DrawingRect.Width <= 0 || DrawingRect.Height <= 0)
                return;

            // Calculate dynamic star size based on DrawingRect size
            int availableWidth = DrawingRect.Width - (Spacing * (_starCount - 1));
            int dynamicStarSize = Math.Max(10, Math.Min(availableWidth / _starCount, DrawingRect.Height - 10));
            int starSize = Math.Min(_starSize, dynamicStarSize); // Use the smaller of fixed or dynamic size

            // Center stars within DrawingRect
            int startX = DrawingRect.Left + (DrawingRect.Width - (starSize * _starCount + Spacing * (_starCount - 1))) / 2;
            int startY = DrawingRect.Top + (DrawingRect.Height - starSize) / 2;

            // Draw stars
            for (int i = 0; i < _starCount; i++)
            {
                DrawStar(graphics, startX + i * (starSize + Spacing), startY, starSize, i < _selectedRating ? _filledStarColor : _emptyStarColor);
            }
        }
        private void DrawStar(Graphics graphics, int x, int y, int size, Color fillColor)
        {
            PointF[] starPoints = CalculateStarPoints(x + size / 2, y + size / 2, size / 2, size / 4, 5);

            using (Brush brush = new SolidBrush(fillColor))
            {
                graphics.FillPolygon(brush, starPoints);
            }

            using (Pen pen = new Pen(StarBorderColor, StarBorderThickness))
            {
                graphics.DrawPolygon(pen, starPoints);
            }
        }
        private PointF[] CalculateStarPoints(float centerX, float centerY, float outerRadius, float innerRadius, int numPoints)
        {
            PointF[] points = new PointF[numPoints * 2];
            double angleStep = Math.PI / numPoints;
            double angle = -Math.PI / 2; // Start at the top

            for (int i = 0; i < points.Length; i++)
            {
                float radius = i % 2 == 0 ? outerRadius : innerRadius;
                points[i] = new PointF(
                    centerX + (float)(Math.Cos(angle) * radius),
                    centerY + (float)(Math.Sin(angle) * radius));
                angle += angleStep;
            }

            return points;
        }
        #endregion

        #region Interaction
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // Calculate dynamic star size based on DrawingRect
            int availableWidth = DrawingRect.Width - (Spacing * (_starCount - 1));
            int dynamicStarSize = Math.Max(10, Math.Min(availableWidth / _starCount, DrawingRect.Height - 10));
            int starSize = Math.Min(_starSize, dynamicStarSize); // Use the smaller of fixed or dynamic size

            int startX = DrawingRect.Left + (DrawingRect.Width - (starSize * _starCount + Spacing * (_starCount - 1))) / 2;

            int starIndex = (e.X - startX) / (starSize + Spacing);
            if (starIndex >= 0 && starIndex < _starCount)
            {
                SelectedRating = starIndex + 1;
            }
        }
        #endregion

        #region "Theme"
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            FilledStarColor = _currentTheme.ButtonForeColor;
            EmptyStarColor = _currentTheme.ButtonBackColor;
            StarBorderColor = _currentTheme.BorderColor;
            StarBorderThickness =BorderThickness    ;
            BackColor = _currentTheme.ButtonBackColor;
            ForeColor = _currentTheme.ButtonForeColor;
        }
        #endregion "Theme"

        public override void SetValue(object value)
        {
            if (value is int)
            {
                SelectedRating = (int)value;
            }
        }
        public override object GetValue()
        {
            return SelectedRating;
        }
    }
}

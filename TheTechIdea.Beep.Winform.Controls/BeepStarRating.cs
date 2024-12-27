using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Star Rating")]
    [Category("Beep Controls")]
    public class BeepStarRating : BeepControl
    {
        #region Fields and Properties
        private int _starCount = 5;
        private int _selectedRating = 0;
        private int _starSize = 24;
        private int _spacing = 5;
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
        [Description("Size of each star.")]
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

        public event EventHandler RatingChanged;

        #endregion

        #region Constructor
        public BeepStarRating()
        {
            DoubleBuffered = true;
            Width = (_starSize + _spacing) * _starCount;
            Height = _starSize + 10;
            Cursor = Cursors.Hand;
            BoundProperty= "SelectedRating";
        }
        #endregion

        #region Painting
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            for (int i = 0; i < _starCount; i++)
            {
                DrawStar(e.Graphics, i, i < _selectedRating ? _filledStarColor : _emptyStarColor);
            }
        }
        private void DrawStar(Graphics graphics, int index, Color color)
        {
            int x = index * (_starSize + _spacing);
            int y = (Height - _starSize) / 2;

            using (Brush brush = new SolidBrush(color))
            {
                PointF[] starPoints = CalculateStarPoints(x, y, _starSize / 2, _starSize / 4, 5);
                graphics.FillPolygon(brush, starPoints);
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
            int starIndex = e.X / (_starSize + _spacing);
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
            FilledStarColor = _currentTheme.ButtonActiveForeColor;
            EmptyStarColor = _currentTheme.ButtonActiveBackColor;
        }
        #endregion "Theme"
    }
}

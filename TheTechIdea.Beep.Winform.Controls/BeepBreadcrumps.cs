using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Provides data for the CrumbClicked event.
    /// </summary>
    public class CrumbClickedEventArgs : EventArgs
    {
        /// <summary>
        /// The index of the breadcrumb item that was clicked.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// The text of the breadcrumb item that was clicked.
        /// </summary>
        public string Crumb { get; }

        public CrumbClickedEventArgs(int index, string crumb)
        {
            Index = index;
            Crumb = crumb;
        }
    }

    /// <summary>
    /// A control that displays breadcrumb navigation items.
    /// All drawing is performed within the DrawingRect.
    /// </summary>
    [ToolboxItem(true)]
    [DisplayName("Beep Breadcrumps")]
    [Category("Beep Controls")]
    [Description("A control that displays breadcrumb navigation items inside a designated drawing area (DrawingRect).")]
    public class BeepBreadcrumps : BeepControl
    {
        #region Fields

        // A designer–editable collection of breadcrumb items.
        private BindingList<string> _items = new BindingList<string>();

        // Stores the bounding rectangles (in absolute coordinates) of each crumb for hit testing.
        private List<Rectangle> _itemRectangles = new List<Rectangle>();

        // The index of the breadcrumb currently under the mouse.
        private int _hoverIndex = -1;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the breadcrumb items.
        /// You can add, remove, or modify items at design time or runtime.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Data")]
        [Description("The breadcrumb items.")]
        public BindingList<string> Items
        {
            get { return _items; }
        }
        private Font _textFont = new Font("Arial", 10);
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TextFont
        {
            get => _textFont;
            set
            {

                _textFont = value;
                UseThemeFont = false;
                Font = _textFont;

                Invalidate();


            }
        }
        #endregion

        #region Events

        /// <summary>
        /// Occurs when a breadcrumb item is clicked.
        /// </summary>
        [Category("Action")]
        [Description("Occurs when a breadcrumb item is clicked.")]
        public event EventHandler<CrumbClickedEventArgs> CrumbClicked;

        #endregion

        #region Constructor

        public BeepBreadcrumps()
        {
            // Enable double buffering and optimized painting.
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            // Set default appearance.
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;
            this.Font = new Font("Segoe UI", 9);

            // Repaint whenever the collection changes.
            _items.ListChanged += (s, e) => { Invalidate(); };
        }

        #endregion

        #region Painting

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Use DrawingRect as the designated drawing area.
            Rectangle drawRect = this.DrawingRect;

            // Set the clipping region and translate the coordinate system so that (0,0) corresponds to drawRect.Location.
            g.SetClip(drawRect);
            g.TranslateTransform(drawRect.X, drawRect.Y);

            // Clear the drawing area.
            //using (SolidBrush backBrush = new SolidBrush(this.BackColor))
            //{
            //    g.FillRectangle(backBrush, 0, 0, drawRect.Width, drawRect.Height);
            //}

            //// Clear any previous crumb bounds.
            //_itemRectangles.Clear();

            // Layout constants (all in local coordinates relative to drawRect).
            int x = 5;                      // Starting x–coordinate
            int padding = 8;                // Padding around each crumb
            string separator = " > ";       // Separator between crumbs (with spaces for readability)
            Size sepSize = TextRenderer.MeasureText(separator, this.Font);

            // Draw each breadcrumb item.
            for (int i = 0; i < _items.Count; i++)
            {
                string crumb = _items[i];
                // Measure the text size.
                Size textSize = TextRenderer.MeasureText(crumb, this.Font);
                // Create a rectangle for the crumb text with added padding.
                Rectangle localRect = new Rectangle(x, (drawRect.Height - textSize.Height) / 2, textSize.Width, textSize.Height);
                localRect.Inflate(padding, padding / 2);

                // Convert local coordinates (relative to drawRect) into absolute coordinates for hit testing.
                Rectangle absoluteRect = new Rectangle(drawRect.X + localRect.X, drawRect.Y + localRect.Y, localRect.Width, localRect.Height);
                _itemRectangles.Add(absoluteRect);

                // If the mouse is over this crumb, fill its background.
                if (i == _hoverIndex)
                {
                    using (SolidBrush brush = new SolidBrush(Color.LightGray))
                    {
                        g.FillRectangle(brush, localRect);
                    }
                }

                // Draw the crumb text.
                TextRenderer.DrawText(g, crumb, this.Font, localRect, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                // Advance the x–coordinate.
                x += localRect.Width;

                // If this is not the last crumb, draw the separator.
                if (i < _items.Count - 1)
                {
                    Point sepLocation = new Point(x, (drawRect.Height - sepSize.Height) / 2);
                    TextRenderer.DrawText(g, separator, this.Font, sepLocation, this.ForeColor);
                    x += sepSize.Width;
                }
            }

            // Reset the transformation and clipping.
            g.ResetTransform();
            g.ResetClip();
        }

        #endregion

        #region Mouse Interaction

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // Determine if a crumb was clicked using absolute coordinates.
            for (int i = 0; i < _itemRectangles.Count; i++)
            {
                if (_itemRectangles[i].Contains(e.Location))
                {
                    CrumbClicked?.Invoke(this, new CrumbClickedEventArgs(i, _items[i]));
                    break;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int newHover = -1;
            for (int i = 0; i < _itemRectangles.Count; i++)
            {
                if (_itemRectangles[i].Contains(e.Location))
                {
                    newHover = i;
                    break;
                }
            }

            if (newHover != _hoverIndex)
            {
                _hoverIndex = newHover;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hoverIndex = -1;
            Invalidate();
        }

        #endregion
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            this.BackColor = _currentTheme.PanelBackColor;
            this.ForeColor = _currentTheme.LabelForeColor;
            if (UseThemeFont)
            {
                _textFont = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
            }

            Font = _textFont;
        }
    }
}

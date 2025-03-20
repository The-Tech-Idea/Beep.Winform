using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Radio Button")]
    [Category("Beep Controls")]
    [Description("A control that displays a list of radio button options with text and images. When one option is selected, it updates the SelectedValue property and fires an event.")]
    public class BeepRadioButton : BeepControl
    {
        #region Fields
        private List<SimpleItem> _options = new List<SimpleItem>();
        private string _selectedValue = string.Empty;
        private List<Rectangle> _optionRectangles = new List<Rectangle>();
        private int _hoverIndex = -1;
        private RadioButtonOrientation _orientation = RadioButtonOrientation.Vertical;
        private readonly BeepImage _imageDrawer = new BeepImage(); // Single instance for drawing
        #endregion

        #region Events
        [Category("Action")]
        [Description("Occurs when the selected value changes.")]
        public event EventHandler<RadioButtonSelectedEventArgs> SelectedValueChanged;
        #endregion

        #region Properties
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Data")]
        [Description("The list of options displayed by the control, each with text and an optional image.")]
        public List<SimpleItem> Options
        {
            get => _options;
            set
            {
                _options = value ?? new List<SimpleItem>();
                Invalidate();
            }
        }

        [Category("Data")]
        [Description("The currently selected option's value (Text property of SimpleItem).")]
        public string SelectedValue
        {
            get => _selectedValue;
            set
            {
                if (_selectedValue != value)
                {
                    _selectedValue = value;
                    OnSelectedValueChanged(new RadioButtonSelectedEventArgs(SelectedIndex, _selectedValue));
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        public int SelectedIndex
        {
            get
            {
                if (string.IsNullOrEmpty(_selectedValue))
                    return -1;
                return _options.FindIndex(item => item.Text == _selectedValue);
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Determines if the options are laid out vertically (stacked) or horizontally (side-by-side).")]
        [DefaultValue(RadioButtonOrientation.Vertical)]
        public RadioButtonOrientation Orientation
        {
            get => _orientation;
            set
            {
                if (_orientation != value)
                {
                    _orientation = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Constructor
        public BeepRadioButton()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            BackColor = Color.White;
            ForeColor = Color.Black;
            Font = new Font("Segoe UI", 9);

            _imageDrawer.Theme = Theme; // Set initial theme for image drawing
        }
        #endregion

        #region Event Raisers
        protected virtual void OnSelectedValueChanged(RadioButtonSelectedEventArgs e)
        {
            SelectedValueChanged?.Invoke(this, e);
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
            var g = graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle drawRect = rectangle;
            g.SetClip(drawRect);
            g.TranslateTransform(drawRect.X, drawRect.Y);

            _optionRectangles.Clear();

            if (_orientation == RadioButtonOrientation.Vertical)
            {
                int itemHeight = 30;
                int indicatorSize = 16;
                int paddingLeft = 10;
                int paddingTop = 5;
                int spacing = 5;
                int imageSize = 20;

                for (int i = 0; i < _options.Count; i++)
                {
                    int top = paddingTop + i * itemHeight;
                    Rectangle localItemRect = new Rectangle(0, top, drawRect.Width, itemHeight);
                    Rectangle absoluteItemRect = new Rectangle(drawRect.X, drawRect.Y + top, drawRect.Width, itemHeight);
                    _optionRectangles.Add(absoluteItemRect);

                    if (i == _hoverIndex)
                    {
                        using (SolidBrush brush = new SolidBrush(Color.LightGray))
                        {
                            g.FillRectangle(brush, localItemRect);
                        }
                    }

                    // Draw radio indicator
                    Rectangle indicatorRect = new Rectangle(paddingLeft, top + (itemHeight - indicatorSize) / 2, indicatorSize, indicatorSize);
                    using (Pen pen = new Pen(ForeColor, 2))
                    {
                        g.DrawEllipse(pen, indicatorRect);
                    }
                    if (_options[i].Text == _selectedValue)
                    {
                        int innerSize = indicatorSize / 2;
                        Rectangle innerRect = new Rectangle(
                            indicatorRect.X + (indicatorSize - innerSize) / 2,
                            indicatorRect.Y + (indicatorSize - innerSize) / 2,
                            innerSize,
                            innerSize);
                        using (SolidBrush brush = new SolidBrush(ForeColor))
                        {
                            g.FillEllipse(brush, innerRect);
                        }
                    }

                    // Draw image if present
                    int xOffset = indicatorRect.Right + spacing;
                    if (!string.IsNullOrEmpty(_options[i].ImagePath))
                    {
                        _imageDrawer.ImagePath = _options[i].ImagePath;
                        Rectangle imageRect = new Rectangle(xOffset, top + (itemHeight - imageSize) / 2, imageSize, imageSize);
                        _imageDrawer.Draw(g, imageRect);
                        xOffset += imageSize + spacing;
                    }

                    // Draw text
                    Rectangle textRect = new Rectangle(xOffset, top, drawRect.Width - xOffset, itemHeight);
                    TextRenderer.DrawText(g, _options[i].Text, Font, textRect, ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
                }
            }
            else // Horizontal
            {
                int indicatorSize = 16;
                int padding = 5;
                int spacing = 5;
                int imageSize = 20;
                int x = padding;

                for (int i = 0; i < _options.Count; i++)
                {
                    string opt = _options[i].Text;
                    Size textSize = TextRenderer.MeasureText(opt, Font);
                    int itemWidth = indicatorSize + spacing + (string.IsNullOrEmpty(_options[i].ImagePath) ? 0 : imageSize + spacing) + textSize.Width + 2 * padding;
                    Rectangle localItemRect = new Rectangle(x, 0, itemWidth, drawRect.Height);
                    Rectangle absoluteItemRect = new Rectangle(drawRect.X + x, drawRect.Y, itemWidth, drawRect.Height);
                    _optionRectangles.Add(absoluteItemRect);

                    if (i == _hoverIndex)
                    {
                        using (SolidBrush brush = new SolidBrush(Color.LightGray))
                        {
                            g.FillRectangle(brush, localItemRect);
                        }
                    }

                    // Draw radio indicator
                    Rectangle indicatorRect = new Rectangle(x + padding, (drawRect.Height - indicatorSize) / 2, indicatorSize, indicatorSize);
                    using (Pen pen = new Pen(ForeColor, 2))
                    {
                        g.DrawEllipse(pen, indicatorRect);
                    }
                    if (_options[i].Text == _selectedValue)
                    {
                        int innerSize = indicatorSize / 2;
                        Rectangle innerRect = new Rectangle(
                            indicatorRect.X + (indicatorSize - innerSize) / 2,
                            indicatorRect.Y + (indicatorSize - innerSize) / 2,
                            innerSize,
                            innerSize);
                        using (SolidBrush brush = new SolidBrush(ForeColor))
                        {
                            g.FillEllipse(brush, innerRect);
                        }
                    }

                    // Draw image if present
                    int xOffset = indicatorRect.Right + spacing;
                    if (!string.IsNullOrEmpty(_options[i].ImagePath))
                    {
                        _imageDrawer.ImagePath = _options[i].ImagePath;
                        Rectangle imageRect = new Rectangle(xOffset, (drawRect.Height - imageSize) / 2, imageSize, imageSize);
                        _imageDrawer.Draw(g, imageRect);
                        xOffset += imageSize + spacing;
                    }

                    // Draw text
                    Rectangle textRect = new Rectangle(xOffset, 0, textSize.Width + 2 * padding, drawRect.Height);
                    TextRenderer.DrawText(g, opt, Font, textRect, ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

                    x += itemWidth + spacing;
                }
            }

            g.ResetTransform();
            g.ResetClip();
        }
        public void Reset()
        {
            _options.Clear();
            SelectedValue = null;
        }
        #endregion

        #region Mouse Interaction
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int newHover = -1;
            for (int i = 0; i < _optionRectangles.Count; i++)
            {
                if (_optionRectangles[i].Contains(e.Location))
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

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            for (int i = 0; i < _optionRectangles.Count; i++)
            {
                if (_optionRectangles[i].Contains(e.Location))
                {
                    SelectedValue = _options[i].Text;
                    break;
                }
            }
        }
        #endregion

        #region Theme and Value Management
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
            _imageDrawer.Theme = Theme; // Update the single image drawer's theme
            Invalidate();
        }

        public override void SetValue(object value)
        {
            if (value is SimpleItem item)
            {
                if (_options.Contains(item))
                {
                    SelectedValue = item.Text;
                }
            }
            else if (value is string str)
            {
                // Fallback for string compatibility
                if (_options.Exists(o => o.Text == str))
                {
                    SelectedValue = str;
                }
            }
        }

        public override object GetValue()
        {
            int index = SelectedIndex;
            return index >= 0 ? _options[index] : null;
        }
        #endregion
    }

    public class RadioButtonSelectedEventArgs : EventArgs
    {
        public int SelectedIndex { get; }
        public string SelectedValue { get; }

        public RadioButtonSelectedEventArgs(int index, string value)
        {
            SelectedIndex = index;
            SelectedValue = value;
        }
    }

    public enum RadioButtonOrientation { Horizontal, Vertical }
}
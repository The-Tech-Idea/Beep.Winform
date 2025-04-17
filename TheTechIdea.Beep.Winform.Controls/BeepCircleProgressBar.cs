using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Circle Progress Bar")]
    [Description("A circular stepper control that displays steps as clickable circles and updates on click.")]
    public class BeepCircleProgressBar : BeepControl
    {
        private List<RectangleF> stepBounds = new List<RectangleF>();
        private int _selectedIndex = -1;

        [Browsable(true)]
        [Category("Data")]
        [Description("The list of steps to display.")]
        public BindingList<SimpleItem> ListItems { get; set; } = new BindingList<SimpleItem>();

        [Browsable(false)]
        public int SelectedIndex => _selectedIndex;

        [Browsable(false)]
        public SimpleItem SelectedItem => (_selectedIndex >= 0 && _selectedIndex < ListItems.Count) ? ListItems[_selectedIndex] : null;

        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

        public BeepCircleProgressBar()
        {
            if (DesignMode)
            {
                ListItems.Add(new SimpleItem { Text = "Step 1", Value = 1 });
                ListItems.Add(new SimpleItem { Text = "Step 2", Value = 2 });
                ListItems.Add(new SimpleItem { Text = "Step 3", Value = 3 });
                ListItems.Add(new SimpleItem { Text = "Step 4", Value = 4 });
            }
            ListItems.ListChanged += (s, e) => Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawSteps(e.Graphics);
        }

        private void DrawSteps(Graphics g)
        {
            stepBounds.Clear();
            if (ListItems == null || ListItems.Count == 0)
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            int count = ListItems.Count;

            // Calculate diameter and spacing
            float diameter = Math.Min(DrawingRect.Height * 0.6f, DrawingRect.Width / (count * 1.5f));
            float totalWidth = count * diameter;
            float spacing = (DrawingRect.Width - totalWidth) / (count + 1);
            float y = DrawingRect.Top + (DrawingRect.Height - diameter) / 2;

            for (int i = 0; i < count; i++)
            {
                float x = DrawingRect.Left + spacing + i * (diameter + spacing);
                var rect = new RectangleF(x, y, diameter, diameter);
                stepBounds.Add(rect);

                // Determine fill color based on state
                Color fill = i < _selectedIndex
                    ? _currentTheme.ButtonPressedBackColor   // completed steps
                    : i == _selectedIndex
                        ? _currentTheme.ButtonBackColor       // active step
                        : _currentTheme.DisabledBackColor;    // pending steps

                using (var brush = new SolidBrush(fill))
                    g.FillEllipse(brush, rect);

                using (var pen = new Pen(_currentTheme.BorderColor, 1))
                    g.DrawEllipse(pen, rect);

                // Draw check mark for completed
                if (i < _selectedIndex)
                {
                    TextRenderer.DrawText(
                        g, "✔",
                        new Font(FontFamily.GenericSansSerif, diameter / 2, FontStyle.Bold),
                        Rectangle.Round(rect),
                        _currentTheme.ButtonForeColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }

                // Draw label underneath
                var label = ListItems[i].Text;
                var sz = TextRenderer.MeasureText(label, Font);
                float lx = rect.X + (rect.Width - sz.Width) / 2;
                float ly = rect.Bottom + 2;
                TextRenderer.DrawText(
                    g, label,
                    Font,
                    new Point((int)lx, (int)ly),
                    _currentTheme.PrimaryTextColor);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            for (int i = 0; i < stepBounds.Count; i++)
            {
                if (stepBounds[i].Contains(e.Location))
                {
                    _selectedIndex = i;
                    Invalidate();
                    SelectedItemChanged?.Invoke(this,
                        new SelectedItemChangedEventArgs(ListItems[i]));
                    break;
                }
            }
        }
    }
}

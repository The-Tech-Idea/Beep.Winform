using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Borderless list box with bottom border on selection
    /// </summary>
    internal class BorderlessListBoxPainter : IListBoxPainter
    {
        private BeepListBox _owner;
        private IBeepTheme _theme;

        public void Initialize(BeepListBox owner, IBeepTheme theme)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _theme = theme;
        }

        public void Paint(Graphics g, BeepListBox owner, Rectangle drawingRect)
        {
            if (g == null || owner == null || drawingRect.IsEmpty) return;

            var items = owner.Items;
            var font = owner.Font;
            var selectedIndex = owner.SelectedIndex;

            for (int i = 0; i < items.Count; i++)
            {
                var itemRect = new Rectangle(drawingRect.X, drawingRect.Y + i * 36, drawingRect.Width, 36);
                bool isHovered = itemRect.Contains(owner.PointToClient(Control.MousePosition));
                bool isSelected = i == selectedIndex;

                DrawItemBackground(g, itemRect, isHovered, isSelected);
                DrawItemText(g, itemRect, items[i].ToString(), font, isSelected);
            }
        }

        private void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty) return;

            // Background color
            Color backgroundColor = Color.White;
            if (isSelected)
            {
                backgroundColor = _theme?.PrimaryColor ?? Color.LightBlue;
            }
            else if (isHovered)
            {
                backgroundColor = Color.FromArgb(240, 240, 240);
            }

            using (var brush = new SolidBrush(backgroundColor))
            {
                g.FillRectangle(brush, itemRect);
            }

            // Bottom border for selected item
            if (isSelected)
            {
                using (var pen = new Pen(_theme?.AccentColor ?? Color.Blue, 2))
                {
                    g.DrawLine(pen, itemRect.Left + 8, itemRect.Bottom - 2, itemRect.Right - 8, itemRect.Bottom - 2);
                }
            }
        }

        private void DrawItemText(Graphics g, Rectangle itemRect, string text, Font font, bool isSelected)
        {
            if (string.IsNullOrEmpty(text)) return;

            var textColor = isSelected ? Color.White : Color.Black;
            var textRect = new Rectangle(itemRect.X + 8, itemRect.Y, itemRect.Width - 16, itemRect.Height);

            TextRenderer.DrawText(g, text, font, textRect, textColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
        }

        // Implemented missing members of IListBoxPainter
        public bool SupportsSearch()
        {
            return false; // BorderlessListBoxPainter does not support search by default
        }

        public bool SupportsCheckboxes()
        {
            return false; // BorderlessListBoxPainter does not support checkboxes by default
        }

        public BeepControlStyle Style
        {
            get { return _owner.ControlStyle; } // Default style for borderless list box
            set { _owner.ControlStyle = value; }
        }

        public System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(8, 4, 8, 4); // Default padding
        }

        public int GetPreferredItemHeight()
        {
            return 36; // Default item height
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;


namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers
{
    /// <summary>
    /// Helper methods for dialog positioning, layout, and calculations
    /// </summary>
    public static class DialogHelpers
    {
        #region Positioning

        /// <summary>
        /// Calculate dialog position based on config
        /// </summary>
        public static Point CalculatePosition(DialogConfig config, Size dialogSize, Form parentForm)
        {
            if (config.Position == DialogPosition.Custom && config.CustomLocation.HasValue)
            {
                return config.CustomLocation.Value;
            }

            Screen screen = Screen.PrimaryScreen;
            Rectangle workingArea = screen.WorkingArea;
            Rectangle parentBounds = Rectangle.Empty;

            if (parentForm != null)
            {
                parentBounds = parentForm.Bounds;
                screen = Screen.FromControl(parentForm);
                workingArea = screen.WorkingArea;
            }

            return config.Position switch
            {
                DialogPosition.CenterScreen => CalculateCenterScreen(dialogSize, workingArea),
                DialogPosition.CenterParent => CalculateCenterParent(dialogSize, parentBounds, workingArea),
                DialogPosition.TopLeft => CalculateTopLeft(parentBounds, workingArea),
                DialogPosition.TopCenter => CalculateTopCenter(dialogSize, parentBounds, workingArea),
                DialogPosition.TopRight => CalculateTopRight(dialogSize, parentBounds, workingArea),
                _ => CalculateCenterScreen(dialogSize, workingArea)
            };
        }

        private static Point CalculateCenterScreen(Size dialogSize, Rectangle workingArea)
        {
            int x = workingArea.Left + (workingArea.Width - dialogSize.Width) / 2;
            int y = workingArea.Top + (workingArea.Height - dialogSize.Height) / 2;
            return new Point(x, y);
        }

        private static Point CalculateCenterParent(Size dialogSize, Rectangle parentBounds, Rectangle workingArea)
        {
            if (parentBounds.IsEmpty)
                return CalculateCenterScreen(dialogSize, workingArea);

            int x = parentBounds.Left + (parentBounds.Width - dialogSize.Width) / 2;
            int y = parentBounds.Top + (parentBounds.Height - dialogSize.Height) / 2;

            // Ensure dialog is within screen bounds
            x = Math.Max(workingArea.Left, Math.Min(x, workingArea.Right - dialogSize.Width));
            y = Math.Max(workingArea.Top, Math.Min(y, workingArea.Bottom - dialogSize.Height));

            return new Point(x, y);
        }

        private static Point CalculateTopLeft(Rectangle parentBounds, Rectangle workingArea)
        {
            if (parentBounds.IsEmpty)
                return new Point(workingArea.Left + 20, workingArea.Top + 20);

            return new Point(parentBounds.Left + 20, parentBounds.Top + 20);
        }

        private static Point CalculateTopCenter(Size dialogSize, Rectangle parentBounds, Rectangle workingArea)
        {
            if (parentBounds.IsEmpty)
            {
                int x = workingArea.Left + (workingArea.Width - dialogSize.Width) / 2;
                return new Point(x, workingArea.Top + 20);
            }

            int xPos = parentBounds.Left + (parentBounds.Width - dialogSize.Width) / 2;
            return new Point(xPos, parentBounds.Top + 20);
        }

        private static Point CalculateTopRight(Size dialogSize, Rectangle parentBounds, Rectangle workingArea)
        {
            if (parentBounds.IsEmpty)
                return new Point(workingArea.Right - dialogSize.Width - 20, workingArea.Top + 20);

            return new Point(parentBounds.Right - dialogSize.Width - 20, parentBounds.Top + 20);
        }

        #endregion

        #region Button Layout

        /// <summary>
        /// Calculate button positions based on layout
        /// </summary>
        public static Rectangle[] CalculateButtonPositions(Rectangle buttonArea, int buttonCount, 
            DialogButtonLayout layout, int buttonWidth, int buttonHeight, int spacing)
        {
            if (buttonCount == 0)
                return Array.Empty<Rectangle>();

            var positions = new Rectangle[buttonCount];

            switch (layout)
            {
                case DialogButtonLayout.Horizontal:
                    CalculateHorizontalButtons(buttonArea, buttonCount, buttonWidth, buttonHeight, spacing, positions);
                    break;

                case DialogButtonLayout.Vertical:
                    CalculateVerticalButtons(buttonArea, buttonCount, buttonWidth, buttonHeight, spacing, positions);
                    break;

                case DialogButtonLayout.Grid:
                    CalculateGridButtons(buttonArea, buttonCount, buttonWidth, buttonHeight, spacing, positions);
                    break;
            }

            return positions;
        }

        private static void CalculateHorizontalButtons(Rectangle buttonArea, int count, 
            int width, int height, int spacing, Rectangle[] positions)
        {
            int totalWidth = (count * width) + ((count - 1) * spacing);
            int startX = buttonArea.Left + (buttonArea.Width - totalWidth) / 2;
            int y = buttonArea.Top + (buttonArea.Height - height) / 2;

            for (int i = 0; i < count; i++)
            {
                int x = startX + (i * (width + spacing));
                positions[i] = new Rectangle(x, y, width, height);
            }
        }

        private static void CalculateVerticalButtons(Rectangle buttonArea, int count, 
            int width, int height, int spacing, Rectangle[] positions)
        {
            int totalHeight = (count * height) + ((count - 1) * spacing);
            int startY = buttonArea.Top + (buttonArea.Height - totalHeight) / 2;
            int x = buttonArea.Left + (buttonArea.Width - width) / 2;

            for (int i = 0; i < count; i++)
            {
                int y = startY + (i * (height + spacing));
                positions[i] = new Rectangle(x, y, width, height);
            }
        }

        private static void CalculateGridButtons(Rectangle buttonArea, int count, 
            int width, int height, int spacing, Rectangle[] positions)
        {
            // Calculate grid dimensions (prefer 2 columns)
            int columns = Math.Min(2, count);
            int rows = (int)Math.Ceiling((double)count / columns);

            int totalWidth = (columns * width) + ((columns - 1) * spacing);
            int totalHeight = (rows * height) + ((rows - 1) * spacing);

            int startX = buttonArea.Left + (buttonArea.Width - totalWidth) / 2;
            int startY = buttonArea.Top + (buttonArea.Height - totalHeight) / 2;

            for (int i = 0; i < count; i++)
            {
                int row = i / columns;
                int col = i % columns;

                int x = startX + (col * (width + spacing));
                int y = startY + (row * (height + spacing));

                positions[i] = new Rectangle(x, y, width, height);
            }
        }

        #endregion

        #region Text Measurement

        /// <summary>
        /// Measure text size for dialog content
        /// </summary>
        public static SizeF MeasureDialogText(Graphics g, string text, Font font, int maxWidth)
        {
            if (string.IsNullOrEmpty(text))
                return SizeF.Empty;

            return TextUtils.MeasureText(g, text, font, maxWidth);
        }

        /// <summary>
        /// Calculate text wrapping for dialog message
        /// </summary>
        public static string[] WrapText(string text, Font font, int maxWidth)
        {
            if (string.IsNullOrEmpty(text))
                return Array.Empty<string>();

            // Simple word wrapping implementation
            var words = text.Split(' ');
            var lines = new System.Collections.Generic.List<string>();
            var currentLine = string.Empty;

            using (var bitmap = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(bitmap))
            {
                foreach (var word in words)
                {
                    var testLine = string.IsNullOrEmpty(currentLine) ? word : $"{currentLine} {word}";
                    var size = g.MeasureString(testLine, font);

                    if (size.Width > maxWidth && !string.IsNullOrEmpty(currentLine))
                    {
                        lines.Add(currentLine);
                        currentLine = word;
                    }
                    else
                    {
                        currentLine = testLine;
                    }
                }

                if (!string.IsNullOrEmpty(currentLine))
                    lines.Add(currentLine);
            }

            return lines.ToArray();
        }

        #endregion

        #region Size Calculation Helpers

        /// <summary>
        /// Calculate minimum button width based on text
        /// </summary>
        public static int CalculateButtonWidth(Graphics g, string text, Font font, int minWidth = 80, int padding = 20)
        {
            if (string.IsNullOrEmpty(text))
                return minWidth;

            var textSize = g.MeasureString(text, font);
            int width = (int)Math.Ceiling(textSize.Width) + padding;

            return Math.Max(width, minWidth);
        }

        /// <summary>
        /// Calculate total button area size
        /// </summary>
        public static Size CalculateButtonAreaSize(int buttonCount, DialogButtonLayout layout, 
            int buttonWidth, int buttonHeight, int spacing)
        {
            if (buttonCount == 0)
                return Size.Empty;

            return layout switch
            {
                DialogButtonLayout.Horizontal => new Size(
                    (buttonCount * buttonWidth) + ((buttonCount - 1) * spacing) + 20,
                    buttonHeight + 20
                ),
                DialogButtonLayout.Vertical => new Size(
                    buttonWidth + 20,
                    (buttonCount * buttonHeight) + ((buttonCount - 1) * spacing) + 20
                ),
                DialogButtonLayout.Grid => CalculateGridButtonAreaSize(buttonCount, buttonWidth, buttonHeight, spacing),
                _ => new Size(buttonWidth + 20, buttonHeight + 20)
            };
        }

        private static Size CalculateGridButtonAreaSize(int count, int width, int height, int spacing)
        {
            int columns = Math.Min(2, count);
            int rows = (int)Math.Ceiling((double)count / columns);

            return new Size(
                (columns * width) + ((columns - 1) * spacing) + 20,
                (rows * height) + ((rows - 1) * spacing) + 20
            );
        }

        #endregion

        #region Validation

        /// <summary>
        /// Ensure size is within bounds
        /// </summary>
        public static Size EnsureSizeWithinBounds(Size size, Size minSize, Size maxSize)
        {
            int width = Math.Max(minSize.Width, Math.Min(size.Width, maxSize.Width));
            int height = Math.Max(minSize.Height, Math.Min(size.Height, maxSize.Height));

            return new Size(width, height);
        }

        /// <summary>
        /// Ensure position is within screen bounds
        /// </summary>
        public static Point EnsurePositionWithinScreen(Point position, Size dialogSize)
        {
            var screen = Screen.FromPoint(position);
            var workingArea = screen.WorkingArea;

            int x = Math.Max(workingArea.Left, Math.Min(position.X, workingArea.Right - dialogSize.Width));
            int y = Math.Max(workingArea.Top, Math.Min(position.Y, workingArea.Bottom - dialogSize.Height));

            return new Point(x, y);
        }

        #endregion
    }
}

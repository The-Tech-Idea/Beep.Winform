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
        #region Title

        /// <summary>
        /// Sets a dialog's visible title and the window's accessible name together.
        /// </summary>
        /// <remarks>
        /// Every dialog's <c>Title</c> setter wrote only to its header label, and each designer set
        /// <c>Text = string.Empty</c>, so the window itself had no accessible name: a screen reader
        /// announcing the dialog said nothing, and the name was empty in the window list. Measured
        /// against a stock <see cref="Form"/>, which reports its <c>Text</c> as the accessible name.
        ///
        /// Assigning <c>Text</c> is safe here only because these dialogs draw their own header and
        /// set <c>ShowCaptionBar = false</c>; with the skinned caption bar on, the form painters draw
        /// <c>owner.Text</c> and the title would appear twice.
        /// </remarks>
        public static void SetTitle(Form form, BeepLabel titleLabel, string? value)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            if (titleLabel == null) throw new ArgumentNullException(nameof(titleLabel));

            string text = value ?? string.Empty;
            titleLabel.Text = text;
            form.Text = text;

            // The title is also the dialog's accessible name, and this is the one place every dialog
            // sets a title — each form's Title setter calls it, whichever of the four construction
            // paths built the form. Putting it in BeepDialogManager.CreateDialog instead would reach
            // only three of the six: the input, list and multi-select dialogs are constructed in
            // BeepDialogManager.Input.cs and never pass through it.
            form.AccessibleRole = AccessibleRole.Dialog;

            // An empty string reads as a named-but-blank window, which is worse than an unnamed one.
            if (!string.IsNullOrWhiteSpace(text))
            {
                form.AccessibleName = text;
            }
        }

        /// <summary>
        /// Sets a dialog's message and the window's accessible description together.
        /// </summary>
        /// <remarks>
        /// The WAI-ARIA APG dialog pattern is the reference: a dialog is labelled by its title and
        /// described by its body. <c>AccessibleName</c> and <c>AccessibleDescription</c> are the
        /// WinForms equivalents of <c>aria-labelledby</c> and <c>aria-describedby</c>. Before this,
        /// the folder had zero occurrences of either across 7,797 lines, so a screen reader announced
        /// that a window had appeared and nothing about what it was asking.
        /// </remarks>
        public static void SetMessage(Form form, BeepLabel messageLabel, string? value)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            if (messageLabel == null) throw new ArgumentNullException(nameof(messageLabel));

            string text = value ?? string.Empty;
            messageLabel.Text = text;

            if (!string.IsNullOrWhiteSpace(text))
            {
                form.AccessibleDescription = text;
            }
        }

        /// <summary>
        /// Gives every action button an accessible name, falling back to its caption.
        /// </summary>
        /// <remarks>
        /// A button captioned only by an icon — the trash glyph in `Example_images/dialog3.png` —
        /// announces nothing at all without this, and those are precisely the destructive ones.
        /// Called from each dialog's constructor once composition has put the buttons in place.
        /// </remarks>
        public static void DescribeActions(Control root)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));

            foreach (Control child in root.Controls)
            {
                if (child is IButtonControl && string.IsNullOrWhiteSpace(child.AccessibleName)
                    && !string.IsNullOrWhiteSpace(child.Text))
                {
                    child.AccessibleName = child.Text;
                }

                DescribeActions(child);
            }
        }

        #endregion

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

            var words = text.Split(' ');
            var lines = new System.Collections.Generic.List<string>();
            var currentLine = string.Empty;

            foreach (var word in words)
            {
                var testLine = string.IsNullOrEmpty(currentLine) ? word : $"{currentLine} {word}";
                var size = TextRenderer.MeasureText(testLine, font, new Size(maxWidth, int.MaxValue), TextFormatFlags.NoPadding);

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

        #region Layout / sizing

        /// <summary>
        /// Sets the form's ClientSize to exactly fit its child controls plus their margins.
        /// No arbitrary minimum — the content determines the size. The form can be resized
        /// smaller by the user; MinimumSize is set to half the computed size as a reasonable
        /// lower bound so the form never collapses to zero.
        /// Call this in the form constructor after InitializeComponent().
        /// </summary>
        /// <summary>
        /// Bounds a form has to respect when it next measures itself.
        /// </summary>
        /// <remarks>
        /// <b>This method is the single sizing authority for dialogs.</b> It used to compete with
        /// <c>BeepDialogManager.FitToContent</c>, which clamped at construction while this re-measured
        /// on <c>Load</c> and won — so <c>MaxContentHeight</c> was ignored, a message row came out
        /// 201px tall, and moving the body into a scroll panel collapsed a dialog to 26px wide. Three
        /// separate fixes failed against that conflict before the ownership was settled here.
        /// <para>
        /// The manager now states the bounds instead of enforcing them, and this honours them.
        /// </para>
        /// </remarks>
        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Form, DialogSizeBounds> _bounds = new();

        internal sealed class DialogSizeBounds
        {
            public int MaxHeight { get; set; }
            public int MinWidth { get; set; }

            /// <summary>
            /// A floor, used when the body scrolls.
            /// </summary>
            /// <remarks>
            /// A scrollable body reports a small preferred height, so a dialog whose content overflows
            /// would otherwise open cramped — 221px for a 4,900-character message — and make the user
            /// scroll far more than necessary. When content is scrolling, the dialog should take the
            /// height it is allowed.
            /// </remarks>
            public int MinHeight { get; set; }
        }

        /// <summary>States the bounds this form must respect. Call before it is shown.</summary>
        public static void SetSizeBounds(Form form, int maxHeight, int minWidth = 0, int minHeight = 0)
        {
            if (form == null) return;
            _bounds.Remove(form);
            _bounds.Add(form, new DialogSizeBounds
            {
                MaxHeight = maxHeight,
                MinWidth = minWidth,
                MinHeight = minHeight,
            });
        }

        /// <summary>The bounds stated for this form, or zeroes when none were.</summary>
        internal static DialogSizeBounds BoundsFor(Form form)
            => form != null && _bounds.TryGetValue(form, out var b) ? b : new DialogSizeBounds();

        public static void FitFormToContent(Form form)
        {
            form.PerformLayout();

            // A form whose content is one docked layout root must be measured by asking that root
            // what it needs. Summing child Right/Bottom — which is what this did — is meaningless
            // for a Dock.Fill child, because its Right and Bottom *are* the form's current client
            // area: the form ends up measured against itself, the content is never consulted, and
            // anything the layout wanted beyond the current size is simply cut off. That is exactly
            // what happened when the dialogs moved onto BeepDialogShell — every footer button was
            // pushed past the bottom edge.
            if (form.Controls.Count == 1 && form.Controls[0].Dock == DockStyle.Fill)
            {
                Control root = form.Controls[0];

                // This is called from the constructor, where the form has no handle yet and its
                // chrome is not established — DisplayRectangle still reports the whole client area,
                // so the caption band measures as zero and the dialog ends up ~73px short, dropping
                // the footer. Defer to Load, when the chrome is real, and measure once there.
                if (!form.IsHandleCreated)
                {
                    void FitOnLoad(object? sender, EventArgs e)
                    {
                        form.Load -= FitOnLoad;
                        FitFormToContent(form);
                    }

                    form.Load += FitOnLoad;
                    return;
                }

                // The host form reserves chrome *inside* its client area — BeepiFormPro draws its
                // caption band there — so a Dock.Fill child is laid into DisplayRectangle, not
                // ClientRectangle. Measuring the child and assigning that straight to ClientSize
                // loses exactly the chrome band, which drops the footer off the bottom.
                int chromeWidth = Math.Max(0, form.ClientSize.Width - form.DisplayRectangle.Width);
                int chromeHeight = Math.Max(0, form.ClientSize.Height - form.DisplayRectangle.Height);

                // Width is a design decision (a dialog has a sensible column measure); height
                // follows the content. Constraining the width and asking for a preferred height is
                // what makes a wrapped message grow the dialog instead of clipping it.
                int width = Math.Max(form.MinimumSize.Width, form.ClientSize.Width);
                Size preferred = root.GetPreferredSize(new Size(Math.Max(1, width - chromeWidth), 0));

                int height = Math.Max(preferred.Height + chromeHeight, form.MinimumSize.Height);

                // The bounds the manager stated, honoured here because this is the last word on size.
                var bounds = BoundsFor(form);
                if (bounds.MinWidth > 0) width = Math.Max(width, bounds.MinWidth);
                if (bounds.MinHeight > 0) height = Math.Max(height, bounds.MinHeight);
                if (bounds.MaxHeight > 0) height = Math.Min(height, bounds.MaxHeight);

                form.ClientSize = new Size(width, height);
                return;
            }

            int maxRight  = 0;
            int maxBottom = 0;
            foreach (Control c in form.Controls)
            {
                int r = c.Right  + c.Margin.Right;
                int b = c.Bottom + c.Margin.Bottom;
                if (r > maxRight)  maxRight  = r;
                if (b > maxBottom) maxBottom = b;
            }

            // Add a reasonable gutter so content doesn't touch the edges.
            int gutter = TheTechIdea.Beep.Winform.Controls.Layouts.Helpers.BeepLayoutMetrics.DialogPadding.Left;
            maxRight  += gutter;
            maxBottom += gutter;

            form.ClientSize = new Size(Math.Max(1, maxRight), Math.Max(1, maxBottom));
            form.MinimumSize = new Size(Math.Max(100, maxRight / 3), Math.Max(80, maxBottom / 3));
        }

        #endregion
    }
}

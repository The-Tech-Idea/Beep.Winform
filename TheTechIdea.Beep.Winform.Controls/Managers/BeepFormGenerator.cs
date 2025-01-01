using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;


namespace TheTechIdea.Beep.Winform.Controls.Managers
{
    public static class BeepFormGenerator
    {
        private const int BorderRadius = 20;
        private const int ButtonSize = 30;

        private static BeepiForm StandardForm { get; set; } = new BeepiForm();
        // Dictionary to store original form properties for restoration
        private static readonly Dictionary<Form, FormState> originalFormStates = new Dictionary<Form, FormState>();

        private static Point lastMousePosition;
        private static bool isResizing = false;


        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        private static extern bool SendMessage(nint hWnd, int Msg, int wParam, int lParam);

        [DllImport("gdi32.dll")]
        private static extern nint CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        public static void ApplyBeepForm(Form form, BeepTheme theme = null)
        {
            // Save the original form properties if not already saved
            if (!originalFormStates.ContainsKey(form))
                originalFormStates[form] = new FormState(form);
            // StandardForm.InitializeForm(form);
            ;
            // Apply custom styling
            form.FormBorderStyle = FormBorderStyle.None;
            form.Padding = new Padding(BorderRadius);

            // Apply rounded corners and add custom controls
            ApplyRoundedCorners(form);

            form.Resize += (s, e) => ApplyRoundedCorners(form);

            //AddCustomControls(form);

            // Apply theme if provided
            if (theme != null)
            {
                ApplyTheme(form, theme);
            }

            // Enable drag-to-move functionality and resizing
            AttachDragMove(form);

            form.MouseDown += BeepForm_MouseDown;
            form.MouseMove += BeepForm_MouseMove;
            form.MouseUp += BeepForm_MouseUp;
            //form.Resize += (s, e) => RepositionControls(form);
        }

        public static void RemoveBeepForm(Form form)
        {
            if (originalFormStates.TryGetValue(form, out var state))
            {
                // Restore the form's original properties
                form.FormBorderStyle = state.FormBorderStyle;
                form.Padding = state.Padding;
                form.BackColor = state.BackColor;
                form.ForeColor = state.ForeColor;
                form.Region = null;

                // form.Controls.Remove(titlePanel);

                // Restore color and border properties
                form.BackColor = state.BackColor;
                form.ForeColor = state.ForeColor;
                form.Padding = state.Padding;

                // Remove event handlers added by BeepFormGenerator
                form.MouseDown -= BeepForm_MouseDown;
                form.MouseMove -= BeepForm_MouseMove;
                form.MouseUp -= BeepForm_MouseUp;
                form.Resize -= (s, e) => ApplyRoundedCorners(form);
                form.Resize -= (s, e) => RepositionControls(form);

                // Reset cursor and clear saved state
                form.Cursor = Cursors.Default;
                originalFormStates.Remove(form);
            }
        }

        private static void ApplyRoundedCorners(Form form)
        {
            nint roundedRegion = CreateRoundRectRgn(0, 0, form.Width, form.Height, BorderRadius, BorderRadius);
            form.Region = Region.FromHrgn(roundedRegion);
        }



        private static void RepositionControls(Form form)
        {
            //closeButton.Location = new Point(titlePanel.Width - _buttonSize - 10, 10);
            //maximizeButton.Location = new Point(closeButton.Left - _buttonSize - 10, 10);
            //minimizeButton.Location = new Point(maximizeButton.Left - _buttonSize - 10, 10);
        }

        private static void AttachDragMove(Form form)
        {
            //titleLabel.MouseDown += (s, e) =>
            //{
            //    if (e.Button == MouseButtons.Left)
            //    {
            //        ReleaseCapture();
            //        SendMessage(form.Handle, 0x112, 0xf012, 0); // Specify all four parameters
            //    }
            //};
        }


        private static void BeepForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && sender is Form form &&
                (e.Location.X >= form.Width - BorderRadius || e.Location.Y >= form.Height - BorderRadius))
            {
                isResizing = true;
                lastMousePosition = e.Location;
            }
        }

        private static void BeepForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isResizing && sender is Form form)
            {
                int dx = e.X - lastMousePosition.X;
                int dy = e.Y - lastMousePosition.Y;

                form.Width = Math.Max(form.MinimumSize.Width, form.Width + dx);
                form.Height = Math.Max(form.MinimumSize.Height, form.Height + dy);

                lastMousePosition = e.Location;
            }
            else if (sender is Form form1)
            {
                form1.Cursor = e.X >= form1.Width - BorderRadius || e.Y >= form1.Height - BorderRadius ? Cursors.SizeNWSE : Cursors.Default;
            }
        }

        private static void BeepForm_MouseUp(object sender, MouseEventArgs e)
        {
            isResizing = false;
        }

        public static void ApplyTheme(Form form, BeepTheme theme)
        {
            form.BackColor = theme.BackgroundColor;
            Console.WriteLine("Form BackColor: " + form.BackColor);

            //closeButton.BackColor = theme.BackgroundColor;
            //closeButton.ForeColor = theme.ButtonForeColor;
            //maximizeButton.BackColor = theme.BackgroundColor;
            //maximizeButton.ForeColor = theme.ButtonForeColor;
            //minimizeButton.BackColor = theme.BackgroundColor;
            //minimizeButton.ForeColor = theme.ButtonForeColor;
            //titleLabel.ForeColor = theme.TextBoxForeColor;
            //titleLabel.BackColor = theme.BackgroundColor;
        }

        // Class to save the original form state before applying BeepForm modifications
        private class FormState
        {
            public FormBorderStyle FormBorderStyle { get; }
            public Padding Padding { get; }
            public Color BackColor { get; }
            public Color ForeColor { get; }
            public Control[] OriginalControls { get; }

            public FormState(Form form)
            {
                FormBorderStyle = form.FormBorderStyle;
                Padding = form.Padding;
                BackColor = form.BackColor;
                ForeColor = form.ForeColor;
                OriginalControls = new Control[form.Controls.Count];
                form.Controls.CopyTo(OriginalControls, 0);
            }
        }
    }
}

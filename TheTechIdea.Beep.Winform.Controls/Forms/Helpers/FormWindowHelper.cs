using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Helper class for managing form window behavior and effects
    /// </summary>
    public class FormWindowHelper
    {
        private readonly BeepFormAdvanced _form;
        private readonly FormComponentsAccessor _components;

        // Win32 API constants
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int CS_DROPSHADOW = 0x00020000;

        public FormWindowHelper(BeepFormAdvanced form, FormComponentsAccessor components)
        {
            _form = form;
            _components = components;
        }

        public void SetupWindowBehavior()
        {
            if (!_form.GetDesignMode())
            {
                _form.FormBorderStyle = FormBorderStyle.None;
                _form.SetFormStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | 
                              ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            }
        }

        public void ApplyWindowEffects(bool enableShadow, bool enableGlow)
        {
            if (!_form.IsHandleCreated || _form.GetDesignMode()) return;

            try
            {
                if (enableShadow)
                {
                    int shadowState = 2;
                    DwmSetWindowAttribute(_form.Handle, DWMWA_WINDOW_CORNER_PREFERENCE, ref shadowState, sizeof(int));
                }

                if (enableGlow)
                {
                    var margins = new MARGINS { Left = 1, Right = 1, Top = 1, Bottom = 1 };
                    DwmExtendFrameIntoClientArea(_form.Handle, ref margins);
                }
            }
            catch { /* Ignore DWM errors on older systems */ }
        }

        public void HandleTitleBarMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !_form.GetDesignMode())
            {
                ReleaseCapture();
                SendMessage(_form.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        public void HandleTitleBarDoubleClick()
        {
            if (!_form.GetDesignMode())
            {
                ToggleMaximizeRestore();
            }
        }

        public void ToggleMaximizeRestore()
        {
            _form.WindowState = _form.WindowState == FormWindowState.Maximized 
                ? FormWindowState.Normal 
                : FormWindowState.Maximized;
        }

        public void DrawFormBorder(PaintEventArgs e, FormUIStyle style, Color borderColor)
        {
            if (_form.GetDesignMode() || _form.WindowState == FormWindowState.Maximized) return;
            
            switch (style)
            {
                case FormUIStyle.Floating:
                    DrawFloatingBorder(e.Graphics, borderColor);
                    break;
                    
                case FormUIStyle.Console:
                    DrawSharpBorder(e.Graphics, borderColor);
                    break;
                    
                case FormUIStyle.Mobile:
                case FormUIStyle.Minimal:
                    // No border for these styles
                    break;
                    
                default:
                    DrawStandardBorder(e.Graphics, borderColor);
                    break;
            }
        }

        public void DrawTitleBarGradient(PaintEventArgs e, FormUIStyle style, Color gradientStart, Color gradientEnd, Color borderColor)
        {
            if (_form.GetDesignMode()) return;
            
            var rect = _components.TitleBar.ClientRectangle;
            
            using (var brush = new LinearGradientBrush(rect, gradientStart, gradientEnd, LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, rect);
            }

            switch (style)
            {
                case FormUIStyle.Ribbon:
                    DrawRibbonElements(e.Graphics, rect, borderColor);
                    break;
                    
                case FormUIStyle.Console:
                    DrawConsoleBorder(e.Graphics, rect, borderColor);
                    break;
                    
                case FormUIStyle.Floating:
                    // No border for floating style
                    break;
                    
                default:
                    if (style != FormUIStyle.Minimal)
                    {
                        DrawTitleBarBorder(e.Graphics, rect, borderColor);
                    }
                    break;
            }
        }

        public CreateParams ModifyCreateParams(CreateParams cp)
        {
            if (!_form.GetDesignMode())
            {
                cp.Style |= WS_MINIMIZEBOX;
                cp.ClassStyle |= CS_DROPSHADOW;
            }
            return cp;
        }

        #region Private Methods
        private void DrawFloatingBorder(Graphics g, Color borderColor)
        {
            using (var pen = new Pen(borderColor, 1))
            {
                var rect = new Rectangle(0, 0, _form.Width - 1, _form.Height - 1);
                g.DrawRectangle(pen, rect);
            }
        }

        private void DrawSharpBorder(Graphics g, Color borderColor)
        {
            using (var pen = new Pen(borderColor, 1))
            {
                var rect = new Rectangle(0, 0, _form.Width - 1, _form.Height - 1);
                g.DrawRectangle(pen, rect);
            }
        }

        private void DrawStandardBorder(Graphics g, Color borderColor)
        {
            using (var pen = new Pen(borderColor, 1))
            {
                var rect = new Rectangle(0, 0, _form.Width - 1, _form.Height - 1);
                g.DrawRectangle(pen, rect);
            }
        }

        private void DrawRibbonElements(Graphics g, Rectangle rect, Color borderColor)
        {
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawLine(pen, 0, 30, rect.Width, 30); // Separator line
                g.DrawLine(pen, 0, rect.Bottom - 1, rect.Width, rect.Bottom - 1);
            }
            
            DrawRibbonTabs(g, rect, borderColor);
        }

        private void DrawRibbonTabs(Graphics g, Rectangle rect, Color borderColor)
        {
            string[] tabs = { "Home", "Insert", "View" };
            int tabWidth = 60;
            int startX = 60;
            
            for (int i = 0; i < tabs.Length; i++)
            {
                Rectangle tabRect = new Rectangle(startX + (i * tabWidth), 30, tabWidth, 30);
                
                if (i == 0)
                {
                    using (var brush = new SolidBrush(Color.White))
                    {
                        g.FillRectangle(brush, tabRect);
                    }
                }
                
                using (var pen = new Pen(borderColor, 1))
                {
                    g.DrawRectangle(pen, tabRect);
                }
                
                using (var textBrush = new SolidBrush(Color.Black))
                {
                    var font = new Font("Segoe UI", 8F, FontStyle.Regular);
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(tabs[i], font, textBrush, tabRect, format);
                }
            }
        }

        private void DrawConsoleBorder(Graphics g, Rectangle rect, Color borderColor)
        {
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawLine(pen, 0, rect.Bottom - 1, rect.Width, rect.Bottom - 1);
            }
        }

        private void DrawTitleBarBorder(Graphics g, Rectangle rect, Color borderColor)
        {
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawLine(pen, 0, rect.Bottom - 1, rect.Width, rect.Bottom - 1);
            }
        }
        #endregion

        #region Win32 API
        [StructLayout(LayoutKind.Sequential)]
        private struct MARGINS
        {
            public int Left, Right, Top, Bottom;
        }

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);
        #endregion
    }
}
using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Helper class for managing button styles and layouts
    /// </summary>
    public class FormButtonStyleHelper
    {
        private readonly BeepFormAdvanced _form;
        private readonly FormComponentsAccessor _components;
        
        public FormButtonStyleHelper(BeepFormAdvanced form, FormComponentsAccessor components)
        {
            _form = form;
            _components = components;
        }

        public void SetupButtons(FormUIStyle style, int titleBarHeight)
        {
            switch (style)
            {
                case FormUIStyle.Mac:
                    SetupMacButtons();
                    break;
                case FormUIStyle.Modern:
                    SetupModernButtons(titleBarHeight);
                    break;
                case FormUIStyle.Classic:
                    SetupClassicButtons(titleBarHeight);
                    break;
                case FormUIStyle.Minimal:
                    SetupMinimalButtons(titleBarHeight);
                    break;
                case FormUIStyle.Material:
                    SetupMaterialButtons(titleBarHeight);
                    break;
                case FormUIStyle.Fluent:
                    SetupFluentButtons(titleBarHeight);
                    break;
                case FormUIStyle.Ribbon:
                    SetupRibbonButtons();
                    break;
                case FormUIStyle.Mobile:
                    SetupMobileButtons();
                    break;
                case FormUIStyle.Console:
                    SetupConsoleButtons();
                    break;
                case FormUIStyle.Floating:
                    SetupFloatingButtons();
                    break;
            }
        }

        public void UpdateButtonsForStyle(FormUIStyle style)
        {
            switch (style)
            {
                case FormUIStyle.Floating:
                    if (_components.BtnMin != null) _components.BtnMin.Visible = false;
                    if (_components.BtnMax != null) _components.BtnMax.Visible = false;
                    if (_components.BtnClose != null) _components.BtnClose.Visible = _form.ShowCloseButton;
                    break;
                    
                default:
                    if (_components.BtnMin != null) _components.BtnMin.Visible = _form.ShowMinimizeButton;
                    if (_components.BtnMax != null) _components.BtnMax.Visible = _form.ShowMaximizeButton;
                    if (_components.BtnClose != null) _components.BtnClose.Visible = _form.ShowCloseButton;
                    break;
            }
        }

        public void UpdateMaximizeButton(FormUIStyle style, FormWindowState windowState)
        {
            if (_components.BtnMax == null) return;

            string restoreText, maximizeText;
            
            switch (style)
            {
                case FormUIStyle.Console:
                    restoreText = "?";
                    maximizeText = "?";
                    break;
                case FormUIStyle.Mobile:
                    restoreText = "?";
                    maximizeText = "?";
                    break;
                case FormUIStyle.Floating:
                    return;
                default:
                    restoreText = "?";
                    maximizeText = "?";
                    break;
            }

            _components.BtnMax.Text = windowState == FormWindowState.Maximized ? restoreText : maximizeText;
        }

        #region Button Setup Methods
        private void SetupMacButtons()
        {
            // Mac traffic lights: Close (red), Minimize (yellow), Maximize (green)
            if (_components.BtnClose != null)
            {
                _components.BtnClose.Text = "?";
                _components.BtnClose.BackColor = Color.FromArgb(255, 95, 86);
                _components.BtnClose.HoverBackColor = Color.FromArgb(255, 95, 86);
                _components.BtnClose.BorderRadius = 6;
                _components.BtnClose.Size = new Size(12, 12);
                _components.BtnClose.Location = new Point(8, 8);
                _components.BtnClose.ForeColor = Color.Transparent;
            }
            if (_components.BtnMin != null)
            {
                _components.BtnMin.Text = "?";
                _components.BtnMin.BackColor = Color.FromArgb(255, 189, 46);
                _components.BtnMin.HoverBackColor = Color.FromArgb(255, 189, 46);
                _components.BtnMin.BorderRadius = 6;
                _components.BtnMin.Size = new Size(12, 12);
                _components.BtnMin.Location = new Point(28, 8);
                _components.BtnMin.ForeColor = Color.Transparent;
            }
            if (_components.BtnMax != null)
            {
                _components.BtnMax.Text = "?";
                _components.BtnMax.BackColor = Color.FromArgb(39, 201, 63);
                _components.BtnMax.HoverBackColor = Color.FromArgb(39, 201, 63);
                _components.BtnMax.BorderRadius = 6;
                _components.BtnMax.Size = new Size(12, 12);
                _components.BtnMax.Location = new Point(48, 8);
                _components.BtnMax.ForeColor = Color.Transparent;
            }
        }

        private void SetupModernButtons(int titleBarHeight)
        {
            ResetButtonsToStandard(titleBarHeight);
          //  SetupDefaultButtons("?", "?", "?");
        }

        private void SetupClassicButtons(int titleBarHeight)
        {
            ResetButtonsToStandard(titleBarHeight);
          //  SetupDefaultButtons("_", "?", "?");
        }

        private void SetupMinimalButtons(int titleBarHeight)
        {
            ResetButtonsToStandard(titleBarHeight);
            if (_components.BtnMin != null)
            {
                _components.BtnMin.Text = "?";
                _components.BtnMin.HoverBackColor = Color.FromArgb(240, 240, 240);
                _components.BtnMin.ForeColor = Color.FromArgb(96, 96, 96);
                _components.BtnMin.BackColor = Color.Transparent;
            }
            if (_components.BtnMax != null)
            {
                _components.BtnMax.Text = "?";
                _components.BtnMax.HoverBackColor = Color.FromArgb(240, 240, 240);
                _components.BtnMax.ForeColor = Color.FromArgb(96, 96, 96);
                _components.BtnMax.BackColor = Color.Transparent;
            }
            if (_components.BtnClose != null)
            {
                _components.BtnClose.Text = "?";
                _components.BtnClose.HoverBackColor = Color.FromArgb(232, 17, 35);
                _components.BtnClose.HoverForeColor = Color.White;
                _components.BtnClose.ForeColor = Color.FromArgb(96, 96, 96);
                _components.BtnClose.BackColor = Color.Transparent;
            }
        }

        private void SetupMaterialButtons(int titleBarHeight)
        {
            ResetButtonsToStandard(titleBarHeight);
            SetupDefaultButtons("_", "?", "?");
            // Material buttons are slightly larger
            if (_components.BtnMin != null) { _components.BtnMin.Size = new Size(56, titleBarHeight); _components.BtnMin.HoverBackColor = Color.FromArgb(30, 255, 255, 255); }
            if (_components.BtnMax != null) { _components.BtnMax.Size = new Size(56, titleBarHeight); _components.BtnMax.HoverBackColor = Color.FromArgb(30, 255, 255, 255); }
            if (_components.BtnClose != null) { _components.BtnClose.Size = new Size(56, titleBarHeight); _components.BtnClose.HoverBackColor = Color.FromArgb(244, 67, 54); }
        }

        private void SetupFluentButtons(int titleBarHeight)
        {
            ResetButtonsToStandard(titleBarHeight);
            SetupDefaultButtons("?", "?", "?");
            if (_components.BtnMin != null) _components.BtnMin.HoverBackColor = Color.FromArgb(237, 235, 233);
            if (_components.BtnMax != null) _components.BtnMax.HoverBackColor = Color.FromArgb(237, 235, 233);
            if (_components.BtnClose != null) _components.BtnClose.HoverBackColor = Color.FromArgb(196, 43, 28);
        }

        private void SetupRibbonButtons()
        {
            ResetButtonsToStandard(30);
            if (_components.BtnMin != null)
            {
                _components.BtnMin.Size = new Size(32, 30);
                _components.BtnMin.Location = new Point(0, 0);
                _components.BtnMin.Text = "?";
            }
            if (_components.BtnMax != null)
            {
                _components.BtnMax.Size = new Size(32, 30);
                _components.BtnMax.Location = new Point(32, 0);
                _components.BtnMax.Text = "?";
            }
            if (_components.BtnClose != null)
            {
                _components.BtnClose.Size = new Size(32, 30);
                _components.BtnClose.Location = new Point(64, 0);
                _components.BtnClose.Text = "?";
                _components.BtnClose.HoverBackColor = Color.FromArgb(232, 17, 35);
            }
        }

        private void SetupMobileButtons()
        {
            if (_components.BtnMin != null)
            {
                _components.BtnMin.Text = "?";
                _components.BtnMin.Size = new Size(64, 64);
                _components.BtnMin.Location = new Point(0, 0);
                _components.BtnMin.Font = new Font("Segoe UI", 16F, FontStyle.Regular);
                _components.BtnMin.HoverBackColor = Color.FromArgb(240, 240, 240);
            }
            if (_components.BtnMax != null)
            {
                _components.BtnMax.Text = "?";
                _components.BtnMax.Size = new Size(64, 64);
                _components.BtnMax.Location = new Point(64, 0);
                _components.BtnMax.Font = new Font("Segoe UI", 16F, FontStyle.Regular);
                _components.BtnMax.HoverBackColor = Color.FromArgb(240, 240, 240);
            }
            if (_components.BtnClose != null)
            {
                _components.BtnClose.Text = "?";
                _components.BtnClose.Size = new Size(64, 64);
                _components.BtnClose.Location = new Point(128, 0);
                _components.BtnClose.Font = new Font("Segoe UI", 16F, FontStyle.Regular);
                _components.BtnClose.HoverBackColor = Color.FromArgb(255, 100, 100);
            }
        }

        private void SetupConsoleButtons()
        {
            if (_components.BtnMin != null)
            {
                _components.BtnMin.Text = "_";
                _components.BtnMin.Size = new Size(20, 20);
                _components.BtnMin.Location = new Point(0, 0);
                _components.BtnMin.Font = new Font("Consolas", 10F, FontStyle.Bold);
                _components.BtnMin.HoverBackColor = Color.FromArgb(64, 64, 64);
                _components.BtnMin.ForeColor = Color.FromArgb(220, 220, 220);
            }
            if (_components.BtnMax != null)
            {
                _components.BtnMax.Text = "?";
                _components.BtnMax.Size = new Size(20, 20);
                _components.BtnMax.Location = new Point(20, 0);
                _components.BtnMax.Font = new Font("Consolas", 8F, FontStyle.Bold);
                _components.BtnMax.HoverBackColor = Color.FromArgb(64, 64, 64);
                _components.BtnMax.ForeColor = Color.FromArgb(220, 220, 220);
            }
            if (_components.BtnClose != null)
            {
                _components.BtnClose.Text = "×";
                _components.BtnClose.Size = new Size(20, 20);
                _components.BtnClose.Location = new Point(40, 0);
                _components.BtnClose.Font = new Font("Consolas", 10F, FontStyle.Bold);
                _components.BtnClose.HoverBackColor = Color.FromArgb(200, 0, 0);
                _components.BtnClose.ForeColor = Color.FromArgb(220, 220, 220);
            }
        }

        private void SetupFloatingButtons()
        {
            if (_components.BtnMin != null) _components.BtnMin.Visible = false;
            if (_components.BtnMax != null) _components.BtnMax.Visible = false;
            
            if (_components.BtnClose != null)
            {
                _components.BtnClose.Text = "?";
                _components.BtnClose.Size = new Size(28, 28);
                _components.BtnClose.Location = new Point(4, 4);
                _components.BtnClose.BorderRadius = 14;
                _components.BtnClose.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                _components.BtnClose.HoverBackColor = Color.FromArgb(255, 100, 100);
                _components.BtnClose.BackColor = Color.FromArgb(240, 240, 240);
            }
        }

        private void ResetButtonsToStandard(int titleBarHeight)
        {
            if (_components.BtnMin != null)
            {
                _components.BtnMin.Size = new Size(48, titleBarHeight);
                _components.BtnMin.Location = new Point(0, 0);
                _components.BtnMin.BorderRadius = 0;
                _components.BtnMin.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                _components.BtnMin.Visible = _form.ShowMinimizeButton;
            }
            if (_components.BtnMax != null)
            {
                _components.BtnMax.Size = new Size(48, titleBarHeight);
                _components.BtnMax.Location = new Point(48, 0);
                _components.BtnMax.BorderRadius = 0;
                _components.BtnMax.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                _components.BtnMax.Visible = _form.ShowMaximizeButton;
            }
            if (_components.BtnClose != null)
            {
                _components.BtnClose.Size = new Size(48, titleBarHeight);
                _components.BtnClose.Location = new Point(96, 0);
                _components.BtnClose.BorderRadius = 0;
                _components.BtnClose.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                _components.BtnClose.Visible = _form.ShowCloseButton;
            }
        }

        private void SetupDefaultButtons(string minText, string maxText, string closeText)
        {
            if (_components.BtnMin != null)
            {
                _components.BtnMin.Text = minText;
                _components.BtnMin.HoverBackColor = Color.FromArgb(60, 60, 60);
                _components.BtnMin.ForeColor = Color.White;
                _components.BtnMin.BackColor = Color.Black;
            }
            if (_components.BtnMax != null)
            {
                _components.BtnMax.Text = maxText;
                _components.BtnMax.HoverBackColor = Color.FromArgb(60, 60, 60);
                _components.BtnMax.ForeColor = Color.White;
                _components.BtnMax.BackColor = Color.Black;
            }
            if (_components.BtnClose != null)
            {
                _components.BtnClose.Text = closeText;
                _components.BtnClose.HoverBackColor = Color.FromArgb(232, 17, 35);
                _components.BtnClose.ForeColor = Color.White;
                _components.BtnClose.BackColor = Color.Black;
            }
        }
        #endregion
    }
}
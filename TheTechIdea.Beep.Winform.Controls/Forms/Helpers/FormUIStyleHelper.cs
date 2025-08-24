using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Helper class for managing UI styles and their layouts
    /// </summary>
    public class FormUIStyleHelper
    {
        private readonly BeepFormAdvanced _form;
        private readonly FormComponentsAccessor _components;
        
        public FormUIStyleHelper(BeepFormAdvanced form, FormComponentsAccessor components)
        {
            _form = form;
            _components = components;
        }

        public void ApplyUIStyle(FormUIStyle style, ref int titleBarHeight)
        {
            switch (style)
            {
                case FormUIStyle.Mac:
                    ApplyMacLayout(ref titleBarHeight);
                    break;
                case FormUIStyle.Modern:
                    ApplyModernLayout(ref titleBarHeight);
                    break;
                case FormUIStyle.Classic:
                    ApplyClassicLayout(ref titleBarHeight);
                    break;
                case FormUIStyle.Minimal:
                    ApplyMinimalLayout(ref titleBarHeight);
                    break;
                case FormUIStyle.Material:
                    ApplyMaterialLayout(ref titleBarHeight);
                    break;
                case FormUIStyle.Fluent:
                    ApplyFluentLayout(ref titleBarHeight);
                    break;
                case FormUIStyle.Ribbon:
                    ApplyRibbonLayout(ref titleBarHeight);
                    break;
                case FormUIStyle.Mobile:
                    ApplyMobileLayout(ref titleBarHeight);
                    break;
                case FormUIStyle.Console:
                    ApplyConsoleLayout(ref titleBarHeight);
                    break;
                case FormUIStyle.Floating:
                    ApplyFloatingLayout(ref titleBarHeight);
                    break;
            }
        }

        #region Layout Methods
        private void ApplyMacLayout(ref int titleBarHeight)
        {
            titleBarHeight = 28;
            if (_components.TitleBar != null) _components.TitleBar.Height = titleBarHeight;

            // Move buttons to LEFT side, Mac-style
            if (_components.TitleRightHost != null)
            {
                _components.TitleRightHost.Width = 68;
                _components.TitleRightHost.Dock = DockStyle.Left;
                _components.TitleRightHost.Location = new Point(0, 0);
            }

            // Center the title
            if (_components.TitleLabel != null)
            {
                _components.TitleLabel.Dock = DockStyle.Fill;
                _components.TitleLabel.TextAlign = ContentAlignment.MiddleCenter;
                _components.TitleLabel.Location = new Point(68, 0);
            }

            // Hide icon in Mac style
            if (_components.AppIcon != null) _components.AppIcon.Visible = false;
        }

        private void ApplyModernLayout(ref int titleBarHeight)
        {
            titleBarHeight = 44;
            if (_components.TitleBar != null) _components.TitleBar.Height = titleBarHeight;
            ResetToStandardLayout(titleBarHeight);
        }

        private void ApplyClassicLayout(ref int titleBarHeight)
        {
            titleBarHeight = 30;
            if (_components.TitleBar != null) _components.TitleBar.Height = titleBarHeight;
            ResetToStandardLayout(titleBarHeight);
        }

        private void ApplyMinimalLayout(ref int titleBarHeight)
        {
            titleBarHeight = 32;
            if (_components.TitleBar != null) _components.TitleBar.Height = titleBarHeight;
            ResetToStandardLayout(titleBarHeight);
            
            // Hide icon for minimal look
            if (_components.AppIcon != null) _components.AppIcon.Visible = false;
            
            // Adjust title to start from left edge
            if (_components.TitleLabel != null)
            {
                _components.TitleLabel.Location = new Point(12, 0);
            }
        }

        private void ApplyMaterialLayout(ref int titleBarHeight)
        {
            titleBarHeight = 56;
            if (_components.TitleBar != null) _components.TitleBar.Height = titleBarHeight;
            ResetToStandardLayout(titleBarHeight);
            
            // Material design typically shows icon
            if (_components.AppIcon != null) _components.AppIcon.Visible = true;
        }

        private void ApplyFluentLayout(ref int titleBarHeight)
        {
            titleBarHeight = 40;
            if (_components.TitleBar != null) _components.TitleBar.Height = titleBarHeight;
            ResetToStandardLayout(titleBarHeight);
        }

        private void ApplyRibbonLayout(ref int titleBarHeight)
        {
            titleBarHeight = 60;
            if (_components.TitleBar != null) _components.TitleBar.Height = titleBarHeight;
            ResetToStandardLayout(titleBarHeight);
            
            // Add ribbon tab area to title bar (simulated)
            if (_components.TitleLabel != null)
            {
                _components.TitleLabel.Location = new Point(45, 30);
                _components.TitleLabel.Size = new Size(300, 30);
                _components.TitleLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            }

            // Adjust button positions to top area
            if (_components.TitleRightHost != null)
            {
                _components.TitleRightHost.Location = new Point(_form.Width - 144, 0);
                _components.TitleRightHost.Height = 30;
            }
        }

        private void ApplyMobileLayout(ref int titleBarHeight)
        {
            titleBarHeight = 64;
            if (_components.TitleBar != null) _components.TitleBar.Height = titleBarHeight;
            ResetToStandardLayout(titleBarHeight);
            
            // Large touch-friendly icon
            if (_components.AppIcon != null)
            {
                _components.AppIcon.Size = new Size(32, 32);
                _components.AppIcon.Location = new Point(16, 16);
            }
            
            // Larger text for mobile
            if (_components.TitleLabel != null)
            {
                _components.TitleLabel.Location = new Point(60, 0);
                _components.TitleLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular);
            }

            // Wide button host for touch
            if (_components.TitleRightHost != null)
            {
                _components.TitleRightHost.Width = 192;
            }
        }

        private void ApplyConsoleLayout(ref int titleBarHeight)
        {
            titleBarHeight = 20;
            if (_components.TitleBar != null) _components.TitleBar.Height = titleBarHeight;
            ResetToStandardLayout(titleBarHeight);
            
            // No icon in console style
            if (_components.AppIcon != null) _components.AppIcon.Visible = false;
            
            // Console-style compact title
            if (_components.TitleLabel != null)
            {
                _components.TitleLabel.Location = new Point(8, 0);
                _components.TitleLabel.Font = new Font("Consolas", 8F, FontStyle.Regular);
                _components.TitleLabel.Size = new Size(200, 20);
            }

            // Compact button host
            if (_components.TitleRightHost != null)
            {
                _components.TitleRightHost.Width = 60;
            }
        }

        private void ApplyFloatingLayout(ref int titleBarHeight)
        {
            titleBarHeight = 36;
            if (_components.TitleBar != null) _components.TitleBar.Height = titleBarHeight;

            // Special floating layout: title in center, close button only
            if (_components.TitleRightHost != null)
            {
                _components.TitleRightHost.Width = 36;
                _components.TitleRightHost.Dock = DockStyle.Right;
            }

            // Hide minimize and maximize for floating panel
            if (_components.BtnMin != null) _components.BtnMin.Visible = false;
            if (_components.BtnMax != null) _components.BtnMax.Visible = false;
            
            // Center title with no icon
            if (_components.AppIcon != null) _components.AppIcon.Visible = false;
            if (_components.TitleLabel != null)
            {
                _components.TitleLabel.Dock = DockStyle.Fill;
                _components.TitleLabel.TextAlign = ContentAlignment.MiddleCenter;
                _components.TitleLabel.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            }
        }

        private void ResetToStandardLayout(int titleBarHeight)
        {
            // Standard Windows layout: Icon left, title left, buttons right
            if (_components.TitleRightHost != null)
            {
                _components.TitleRightHost.Dock = DockStyle.Right;
                _components.TitleRightHost.Width = 144;
            }

            if (_components.AppIcon != null)
            {
                _components.AppIcon.Visible = _form.ShowIcon;
                _components.AppIcon.Size = new Size(24, 24);
                _components.AppIcon.Location = new Point(12, (titleBarHeight - 24) / 2);
            }

            if (_components.TitleLabel != null)
            {
                _components.TitleLabel.Dock = DockStyle.None;
                _components.TitleLabel.TextAlign = _form.TitleAlignment;
                _components.TitleLabel.Location = new Point(_form.ShowIcon ? 45 : 12, 0);
                _components.TitleLabel.Size = new Size(300, titleBarHeight);
                _components.TitleLabel.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            }
        }
        #endregion
    }
}
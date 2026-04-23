using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Toolbar
{
    public class BeepGridToolbarPainter
    {
        private readonly BeepGridPro _grid;

        public BeepGridToolbarPainter(BeepGridPro grid) { _grid = grid; }

        private IBeepTheme Theme => _grid.Theme != null
            ? BeepThemesManager.GetTheme(_grid.Theme)
            : BeepThemesManager.GetDefaultTheme();

        public void Paint(Graphics g, Rectangle bounds, BeepGridToolbarState state)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            using var bgBrush = new SolidBrush(_grid.ToolbarBackColor);
            g.FillRectangle(bgBrush, bounds);

            PaintActionButtons(g, state);
            PaintSearchSection(g, state);
            PaintFilterSection(g, state);
            PaintExportButtons(g, state);
            PaintSeparators(g, state);

            using var borderPen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark, 1);
            g.DrawLine(borderPen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
        }

        private void PaintActionButtons(Graphics g, BeepGridToolbarState state)
        {
            PaintIconButton(g, state.AddButtonRect, SvgsUI.Plus, "add",
                state.HoveredButtonKey == "add", state.PressedButtonKey == "add");
            PaintIconButton(g, state.EditButtonRect, SvgsUI.Edit, "edit",
                state.HoveredButtonKey == "edit", state.PressedButtonKey == "edit");
            PaintIconButton(g, state.DeleteButtonRect, SvgsUI.Trash, "delete",
                state.HoveredButtonKey == "delete", state.PressedButtonKey == "delete");
        }

        private void PaintSearchSection(Graphics g, BeepGridToolbarState state)
        {
            var accentColor = Theme?.AccentColor ?? Color.DeepSkyBlue;
            var iconColor = state.SearchHasFocus ? accentColor : _grid.ToolbarForeColor;
            StyledImagePainter.PaintWithTint(g, state.SearchIconRect, Svgs.Search, iconColor, 0.7f);
            PaintSearchBox(g, state.SearchBoxRect, state.SearchText, state.SearchHasFocus);
        }

        private void PaintSearchBox(Graphics g, Rectangle bounds, string text, bool hasFocus)
        {
            var accentColor = Theme?.AccentColor ?? Color.DeepSkyBlue;
            var bgColor = hasFocus ? _grid.ToolbarSearchFocusBackColor : _grid.ToolbarSearchBackColor;
            using var bgBrush = new SolidBrush(bgColor);
            g.FillRectangle(bgBrush, bounds);

            using var borderPen = new Pen(hasFocus ? accentColor : _grid.ToolbarBorderColor, 1);
            g.DrawRectangle(borderPen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);

            var flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
            if (!string.IsNullOrEmpty(text))
                TextRenderer.DrawText(g, text, _grid.Font, bounds, _grid.ToolbarForeColor, flags);
            else
                TextRenderer.DrawText(g, "Search...", _grid.Font, bounds, _grid.ToolbarPlaceholderColor, flags);
        }

        private void PaintFilterSection(Graphics g, BeepGridToolbarState state)
        {
            var accentColor = Theme?.AccentColor ?? Color.DeepSkyBlue;
            var filterColor = state.IsFilterActive ? accentColor : _grid.ToolbarForeColor;
            StyledImagePainter.PaintWithTint(g, state.FilterButtonRect, SvgsUI.Filter, filterColor,
                state.IsFilterActive ? 1f : 0.6f);
            StyledImagePainter.PaintWithTint(g, state.AdvancedButtonRect, SvgsUI.AdjustmentsHorizontal,
                _grid.ToolbarForeColor, 0.6f);

            if (state.IsFilterActive)
                PaintIconButton(g, state.ClearFilterRect, SvgsUI.X, "clearfilter",
                    state.HoveredButtonKey == "clearfilter", state.PressedButtonKey == "clearfilter");

            if (state.ActiveFilterCount > 0)
                PaintBadge(g, state.BadgeRect, state.ActiveFilterCount);
        }

        private void PaintExportButtons(Graphics g, BeepGridToolbarState state)
        {
            PaintIconButton(g, state.ImportButtonRect, SvgsUI.FileUpload, "import",
                state.HoveredButtonKey == "import", state.PressedButtonKey == "import");
            PaintIconButton(g, state.ExportButtonRect, SvgsUI.Download, "export",
                state.HoveredButtonKey == "export", state.PressedButtonKey == "export");
            PaintIconButton(g, state.PrintButtonRect, Svgs.Print, "print",
                state.HoveredButtonKey == "print", state.PressedButtonKey == "print");
        }

        private void PaintIconButton(Graphics g, Rectangle bounds, string iconPath, string key,
            bool isHovered, bool isPressed)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            if (isHovered || isPressed)
            {
                var hoverColor = isPressed ? _grid.ToolbarButtonPressedBackColor : _grid.ToolbarButtonHoverBackColor;
                using var hoverBrush = new SolidBrush(hoverColor);
                g.FillRectangle(hoverBrush, bounds);
            }

            StyledImagePainter.PaintWithTint(g, bounds, iconPath, _grid.ToolbarForeColor, 0.8f);
        }

        private void PaintSeparators(Graphics g, BeepGridToolbarState state)
        {
            using var pen = new Pen(_grid.ToolbarSeparatorColor, 1);
            int top = state.ActionsSectionRect.Top + 4;
            int bottom = state.ActionsSectionRect.Bottom - 4;

            if (state.Separator1X > 0)
                g.DrawLine(pen, state.Separator1X, top, state.Separator1X, bottom);
            if (state.Separator2X > 0)
                g.DrawLine(pen, state.Separator2X, top, state.Separator2X, bottom);
        }

        private void PaintBadge(Graphics g, Rectangle bounds, int count)
        {
            var accentColor = Theme?.AccentColor ?? Color.DeepSkyBlue;
            using var brush = new SolidBrush(accentColor);
            g.FillEllipse(brush, bounds);

            var text = count > 9 ? "9+" : count.ToString();
            using var font = new Font(_grid.Font.FontFamily, 7f);
            TextRenderer.DrawText(g, text, font, bounds, Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }
}

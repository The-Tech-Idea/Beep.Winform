using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
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

            PaintGridTitle(g, state);
            PaintActionButtons(g, state);
            PaintSearchSection(g, state);
            PaintFilterSection(g, state);
            PaintExportButtons(g, state);
            PaintOverflowButton(g, state);
            PaintSeparators(g, state);

            using var borderPen = new Pen(Theme?.GridLineColor ?? ((Theme?.GridBackColor != null && Theme.GridBackColor != Color.Empty ? Theme.GridBackColor : Color.White).GetBrightness() < 0.5 ? Color.FromArgb(60, 70, 85) : Color.FromArgb(180, 180, 180)), 1);
            g.DrawLine(borderPen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
        }

        private void PaintGridTitle(Graphics g, BeepGridToolbarState state)
        {
            if (!state.ShowGridTitle || string.IsNullOrEmpty(state.GridTitle) || state.TitleSectionRect.IsEmpty) return;

            var titleFont = new Font(SystemFonts.DefaultFont.FontFamily, 9.5f, FontStyle.Bold);
            try
            {
                var titleColor = _grid.ToolbarForeColor;
                var flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
                TextRenderer.DrawText(g, state.GridTitle, titleFont, state.TitleSectionRect, titleColor, flags);
            }
            finally { titleFont.Dispose(); }
        }

        private void PaintActionButtons(Graphics g, BeepGridToolbarState state)
        {
            foreach (var btn in state.ActionButtons)
            {
                if (!btn.IsVisible || btn.IsOverflow || btn.Bounds.IsEmpty) continue;

                bool isHovered = state.HoveredButtonKey == btn.Key;
                bool isPressed = state.PressedButtonKey == btn.Key;

                // Draw background for hover/pressed
                if (isHovered || isPressed)
                {
                    var hoverColor = isPressed ? _grid.ToolbarButtonPressedBackColor : _grid.ToolbarButtonHoverBackColor;
                    using var path = CreateRoundedPath(btn.Bounds, 4);
                    using var brush = new SolidBrush(hoverColor);
                    g.FillPath(brush, path);
                }

                // Draw icon
                string iconPath = ResolveIconPath(btn.IconPath);
                int iconSize = (int)(16 * state.DpiScale);
                var iconRect = new Rectangle(btn.Bounds.X + (int)(4 * state.DpiScale), btn.Bounds.Y + (btn.Bounds.Height - iconSize) / 2, iconSize, iconSize);
                StyledImagePainter.PaintWithTint(g, iconRect, iconPath, _grid.ToolbarForeColor, 0.8f);

                // Draw text label
                if (!string.IsNullOrEmpty(btn.Label))
                {
                    var labelRect = new Rectangle(iconRect.Right + (int)(2 * state.DpiScale), btn.Bounds.Y, btn.Bounds.Right - iconRect.Right - (int)(4 * state.DpiScale), btn.Bounds.Height);
                    var labelFont = new Font(SystemFonts.DefaultFont.FontFamily, 8.5f, FontStyle.Regular);
                    try
                    {
                        TextRenderer.DrawText(g, btn.Label, labelFont, labelRect, _grid.ToolbarForeColor,
                            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                    }
                    finally { labelFont.Dispose(); }
                }
            }
        }

        private void PaintSearchSection(Graphics g, BeepGridToolbarState state)
        {
            var accentColor = Theme?.AccentColor ?? Color.DeepSkyBlue;
            var iconColor = state.SearchHasFocus ? accentColor : _grid.ToolbarForeColor;
            StyledImagePainter.PaintWithTint(g, state.SearchIconRect, Svgs.Search, iconColor, 0.7f);
            PaintSearchBox(g, state.SearchBoxRect, state.SearchText, state.SearchHasFocus, state.DpiScale);
        }

        private void PaintSearchBox(Graphics g, Rectangle bounds, string text, bool hasFocus, float dpiScale)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            var accentColor = Theme?.AccentColor ?? Color.DeepSkyBlue;
            var bgColor = hasFocus ? _grid.ToolbarSearchFocusBackColor : _grid.ToolbarSearchBackColor;
            int radius = 4;

            using var path = CreateRoundedPath(bounds, radius);
            using var bgBrush = new SolidBrush(bgColor);
            g.FillPath(bgBrush, path);

            using var borderPen = new Pen(hasFocus ? accentColor : _grid.ToolbarBorderColor, 1);
            g.DrawPath(borderPen, path);

            var flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
            var textRect = new Rectangle(bounds.X + (int)(24 * dpiScale), bounds.Y, bounds.Width - (int)(24 * dpiScale), bounds.Height);
            if (!string.IsNullOrEmpty(text))
                TextRenderer.DrawText(g, text, _grid.Font, textRect, _grid.ToolbarForeColor, flags);
            else
                TextRenderer.DrawText(g, "Search...", _grid.Font, textRect, _grid.ToolbarPlaceholderColor, flags);
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
            foreach (var btn in state.ExportButtons)
            {
                if (!btn.IsVisible || btn.IsOverflow || btn.Bounds.IsEmpty) continue;

                bool isHovered = state.HoveredButtonKey == btn.Key;
                bool isPressed = state.PressedButtonKey == btn.Key;

                PaintIconButton(g, btn.Bounds, ResolveIconPath(btn.IconPath), btn.Key, isHovered, isPressed);
            }
        }

        private void PaintOverflowButton(Graphics g, BeepGridToolbarState state)
        {
            if (state.OverflowButtonRect.IsEmpty || !state.HasOverflowItems) return;

            bool isHovered = state.HoveredButtonKey == "overflow";
            bool isPressed = state.PressedButtonKey == "overflow";

            if (isHovered || isPressed)
            {
                var hoverColor = isPressed ? _grid.ToolbarButtonPressedBackColor : _grid.ToolbarButtonHoverBackColor;
                using var path = CreateRoundedPath(state.OverflowButtonRect, 4);
                using var brush = new SolidBrush(hoverColor);
                g.FillPath(brush, path);
            }

            // Draw chevron-down icon
            var rect = state.OverflowButtonRect;
            var center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            int sz = (int)(6 * state.DpiScale);
            using var pen = new Pen(_grid.ToolbarForeColor, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            g.DrawLine(pen, center.X - sz, center.Y - sz / 2, center.X, center.Y + sz / 2);
            g.DrawLine(pen, center.X + sz, center.Y - sz / 2, center.X, center.Y + sz / 2);
        }

        private void PaintIconButton(Graphics g, Rectangle bounds, string iconPath, string key,
            bool isHovered, bool isPressed)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            if (isHovered || isPressed)
            {
                var hoverColor = isPressed ? _grid.ToolbarButtonPressedBackColor : _grid.ToolbarButtonHoverBackColor;
                using var path = CreateRoundedPath(bounds, 4);
                using var hoverBrush = new SolidBrush(hoverColor);
                g.FillPath(brush: hoverBrush, path);
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
            if (state.Separator3X > 0)
                g.DrawLine(pen, state.Separator3X, top, state.Separator3X, bottom);
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

        private GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0) return path;

            int r = Math.Min(radius, Math.Min(rect.Width / 2, rect.Height / 2));
            int d = r * 2;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddLine(rect.X + r, rect.Y, rect.Right - r, rect.Y);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddLine(rect.Right, rect.Y + r, rect.Right, rect.Bottom - r);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddLine(rect.Right - r, rect.Bottom, rect.X + r, rect.Bottom);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.AddLine(rect.X, rect.Bottom - r, rect.X, rect.Y + r);
            path.CloseFigure();

            return path;
        }

        private string ResolveIconPath(string iconKey)
        {
            return iconKey switch
            {
                "plus" => SvgsUI.Plus,
                "edit" => SvgsUI.Edit,
                "trash" => SvgsUI.Trash,
                "file_upload" => SvgsUI.FileUpload,
                "download" => SvgsUI.Download,
                "print" => Svgs.Print,
                _ => iconKey
            };
        }
    }
}

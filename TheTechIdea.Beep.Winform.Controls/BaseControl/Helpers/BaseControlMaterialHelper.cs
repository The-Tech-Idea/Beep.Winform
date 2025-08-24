using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers
{
    /// <summary>
    /// Helper that renders Material-style border and icons for BaseControl.
    /// Keeps drawing logic out of the control class.
    /// </summary>
    internal sealed class BaseControlMaterialHelper
    {
        private readonly BaseControl _owner;
        private readonly BaseControlIconsHelper _icons;

        private Rectangle _inputRect;

        public BaseControlMaterialHelper(BaseControl owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _icons = new BaseControlIconsHelper(owner);
            UpdateLayout();
        }

        public void UpdateLayout()
        {
            // Use ControlPaintHelper’s DrawingRect to honor Padding/offsets and unified layout
            _inputRect = _owner.DrawingRect;
            if (_inputRect.Width > 2 && _inputRect.Height > 2)
            {
                _inputRect = new Rectangle(_inputRect.X + 1, _inputRect.Y + 1, _inputRect.Width - 2, _inputRect.Height - 2);
            }
            _icons.UpdateLayout(_inputRect);
        }

        public void Draw(Graphics g)
        {
            if (!_owner.EnableMaterialStyle) return;
            int borderWidth = _owner.Focused ? 2 : 1;
            int radius = Math.Max(0, _owner.MaterialBorderRadius);

            switch (_owner.MaterialVariant)
            {
                case MaterialTextFieldVariant.Filled:
                    DrawUnderline(g, _inputRect, borderWidth); // background already handled by ControlPaintHelper
                    break;
                case MaterialTextFieldVariant.Outlined:
                    DrawOutlined(g, _inputRect, borderWidth, radius);
                    break;
                default:
                    DrawUnderline(g, _inputRect, borderWidth); // Standard = underline only
                    break;
            }

            _icons.Draw(g);
        }

        public void DrawIconsOnly(Graphics g)
        {
            if (!_owner.EnableMaterialStyle) return;
            _icons.Draw(g);
        }

        public Rectangle GetLeadingIconRect() => _icons.LeadingRect;
        public Rectangle GetTrailingIconRect() => _icons.TrailingRect;

        private void DrawUnderline(Graphics g, Rectangle rect, int borderWidth)
        {
            using var pen = new Pen(_owner.MaterialOutlineColor, 1);
            g.DrawLine(pen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
            if (_owner.Focused)
            {
                using var fpen = new Pen(_owner.MaterialPrimaryColor, 2);
                g.DrawLine(fpen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
            }
        }

        private void DrawOutlined(Graphics g, Rectangle rect, int borderWidth, int radius)
        {
            using var pen = new Pen(_owner.MaterialOutlineColor, borderWidth);
            using var path = CreateRoundPath(rect, radius);
            g.DrawPath(pen, path);
            if (_owner.Focused)
            {
                using var fpen = new Pen(Color.FromArgb(180, _owner.MaterialPrimaryColor), borderWidth + 1);
                g.DrawPath(fpen, path);
            }
        }

        private static GraphicsPath CreateRoundPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int r = Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2);
            int d = r * 2;
            if (r > 0)
            {
                path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
            }
            else
            {
                path.AddRectangle(rect);
            }
            return path;
        }
    }
}

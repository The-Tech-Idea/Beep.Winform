using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Base class for all combo box painters
    /// Provides common functionality shared across all variants
    /// </summary>
    internal abstract class ComboBoxPainterBase : IComboBoxPainter
    {
        protected BeepComboBox _owner;
        protected IBeepTheme _theme;
        protected BeepComboBoxHelper _helper;
        
        public virtual void Initialize(BeepComboBox owner, IBeepTheme theme)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _theme = theme;
            _helper = owner.Helper;
        }
        
        public abstract void Paint(Graphics g, BeepComboBox owner, Rectangle drawingRect);
        
        public virtual int GetPreferredButtonWidth() => 32;
        
        public virtual Padding GetPreferredPadding() => new Padding(8, 4, 8, 4);
        
        #region Common Helper Methods
        
        protected GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(rect.Location, size);
            
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            
            path.CloseFigure();
            return path;
        }
        
        protected Rectangle GetCenteredIconRect(Rectangle bounds, int iconWidth, int iconHeight)
        {
            int x = bounds.X + (bounds.Width - iconWidth) / 2;
            int y = bounds.Y + (bounds.Height - iconHeight) / 2;
            return new Rectangle(x, y, iconWidth, iconHeight);
        }
        
        protected void DrawSimpleArrow(Graphics g, Rectangle buttonRect, bool isOpen, Color arrowColor)
        {
            using (var pen = new Pen(arrowColor, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                
                int centerX = buttonRect.X + buttonRect.Width / 2;
                int centerY = buttonRect.Y + buttonRect.Height / 2;
                int arrowSize = 4;
                
                Point[] arrowPoints;
                
                if (isOpen)
                {
                    arrowPoints = new Point[]
                    {
                        new Point(centerX - arrowSize, centerY + arrowSize / 2),
                        new Point(centerX, centerY - arrowSize / 2),
                        new Point(centerX + arrowSize, centerY + arrowSize / 2)
                    };
                }
                else
                {
                    arrowPoints = new Point[]
                    {
                        new Point(centerX - arrowSize, centerY - arrowSize / 2),
                        new Point(centerX, centerY + arrowSize / 2),
                        new Point(centerX + arrowSize, centerY - arrowSize / 2)
                    };
                }
                
                g.DrawLines(pen, arrowPoints);
            }
        }
        
        protected bool IsButtonHovered()
        {
            return _owner.HitTestControl != null && 
                   _owner.HitTestControl.Name == "DropdownButton" && 
                   _owner.HitTestControl.IsHovered;
        }
        
        #endregion
    }
}

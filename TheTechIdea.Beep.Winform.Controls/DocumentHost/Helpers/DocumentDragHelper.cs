using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Helpers
{
    public enum DockAction
    {
        None,
        DockFill,
        DockLeft,
        DockRight,
        DockTop,
        DockBottom,
        DockTab,
        DockSplit,
        Float
    }

    public class DockActionResult
    {
        public DockAction Action { get; set; }
        public Point DropPoint { get; set; }
        public float SplitRatio { get; set; } = 0.5f;
        public int TargetTabIndex { get; set; } = -1;
        public string TargetGroupId { get; set; }
        public bool IsValid { get; set; } = true;
    }

    public class DocumentDragHelper
    {
        private Point _lastPosition;
        private bool _isDragging;
        private int _dragStartTabIndex = -1;
        private Point _dragStartPoint;

        public bool IsDragging => _isDragging;
        public int DragStartTabIndex => _dragStartTabIndex;
        public Point DragStartPoint => _dragStartPoint;
        public Point LastPosition => _lastPosition;

        public void StartDrag(int tabIndex, Point startPoint)
        {
            _dragStartTabIndex = tabIndex;
            _dragStartPoint = startPoint;
            _lastPosition = startPoint;
            _isDragging = true;
        }

        public void UpdateDrag(Point currentPosition)
        {
            _lastPosition = currentPosition;
        }

        public void EndDrag()
        {
            _isDragging = false;
            _dragStartTabIndex = -1;
        }

        public bool HasMovedBeyondThreshold(Point current, int threshold = 10)
        {
            int dx = current.X - _dragStartPoint.X;
            int dy = current.Y - _dragStartPoint.Y;
            return (dx * dx + dy * dy) > (threshold * threshold);
        }

        public bool IsVerticalDrag(Point current, int threshold = 30)
        {
            int dy = System.Math.Abs(current.Y - _dragStartPoint.Y);
            int dx = System.Math.Abs(current.X - _dragStartPoint.X);
            return dy > dx && dy > threshold;
        }

        public bool IsHorizontalDrag(Point current, int threshold = 30)
        {
            int dx = System.Math.Abs(current.X - _dragStartPoint.X);
            int dy = System.Math.Abs(current.Y - _dragStartPoint.Y);
            return dx > dy && dx > threshold;
        }

        public DockActionResult ResolveDockAction(Point dropPoint, Rectangle hostBounds, int tabCount)
        {
            var result = new DockActionResult { DropPoint = dropPoint };

            float relX = (float)(dropPoint.X - hostBounds.Left) / hostBounds.Width;
            float relY = (float)(dropPoint.Y - hostBounds.Top) / hostBounds.Height;

            const float edgeThreshold = 0.15f;
            const float centerThreshold = 0.35f;

            if (relY > centerThreshold && relY < (1 - centerThreshold) && relX > centerThreshold && relX < (1 - centerThreshold))
            {
                result.Action = DockAction.DockFill;
            }
            else if (relX < edgeThreshold)
            {
                result.Action = DockAction.DockLeft;
                result.SplitRatio = System.Math.Max(0.2f, System.Math.Min(0.8f, relX / edgeThreshold * 0.5f));
            }
            else if (relX > (1 - edgeThreshold))
            {
                result.Action = DockAction.DockRight;
                result.SplitRatio = System.Math.Max(0.2f, System.Math.Min(0.8f, (1 - relX) / edgeThreshold * 0.5f));
            }
            else if (relY < edgeThreshold)
            {
                result.Action = DockAction.DockTop;
                result.SplitRatio = System.Math.Max(0.2f, System.Math.Min(0.8f, relY / edgeThreshold * 0.5f));
            }
            else if (relY > (1 - edgeThreshold))
            {
                result.Action = DockAction.DockBottom;
                result.SplitRatio = System.Math.Max(0.2f, System.Math.Min(0.8f, (1 - relY) / edgeThreshold * 0.5f));
            }
            else
            {
                result.Action = DockAction.DockSplit;
                result.SplitRatio = 0.5f;
            }

            return result;
        }
    }
}

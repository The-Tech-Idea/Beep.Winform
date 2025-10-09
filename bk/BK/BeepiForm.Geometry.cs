using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepiForm
    {
        // Centralized cached geometry so all painters can reuse paths
        private void MarkPathsDirty()
        {
            _pathsDirty = true;
        }

        private void EnsurePaths()
        {
            if (!_pathsDirty && _cachedClientPath != null && _cachedWindowPath != null)
                return;

            DisposeCachedPaths();

            // Window path in window coordinates (0,0,Width,Height) using path primitives only
            int wW = Math.Max(0, Width);
            int hW = Math.Max(0, Height);
            if (WindowState == FormWindowState.Maximized || _borderRadius <= 0 || wW <= 0 || hW <= 0)
            {
                _cachedWindowPath = BuildRectPath(0, 0, wW, hW);
            }
            else
            {
                _cachedWindowPath = BuildRoundedRectPathBezier(0, 0, wW, hW, _borderRadius);
                if (_cachedWindowPath.PointCount == 0)
                {
                    _cachedWindowPath = BuildRectPath(0, 0, wW, hW);
                }
            }

            // Client path in client coordinates, path-only
            int wC = Math.Max(0, ClientSize.Width);
            int hC = Math.Max(0, ClientSize.Height);
            if (WindowState == FormWindowState.Maximized || _borderRadius <= 0 || wC <= 0 || hC <= 0)
            {
                _cachedClientPath = BuildRectPath(0, 0, wC, hC);
            }
            else
            {
                // IMPORTANT: Use inner radius adjusted by border thickness so the client fill
                // aligns exactly to the inner edge of the inset-drawn border.
                int innerRadius = Math.Max(0, _borderRadius - _borderThickness);
                _cachedClientPath = BuildRoundedRectPathBezier(0, 0, wC, hC, innerRadius);
                if (_cachedClientPath.PointCount == 0 && wC > 0 && hC > 0)
                    _cachedClientPath = BuildRectPath(0, 0, wC, hC);
            }

            _pathsDirty = false;
        }

        private void DisposeCachedPaths()
        {
            _cachedClientPath?.Dispose();
            _cachedClientPath = null;
            _cachedWindowPath?.Dispose();
            _cachedWindowPath = null;
        }

        // Public-facing geometry helpers for painters
        private GraphicsPath GetFormPath()
        {
            EnsurePaths();
            return (GraphicsPath)(_cachedClientPath?.Clone() ?? new GraphicsPath());
        }

        private GraphicsPath GetWindowPath()
        {
            EnsurePaths();
            return (GraphicsPath)(_cachedWindowPath?.Clone() ?? new GraphicsPath());
        }

        // Build axis-aligned rectangle path without using Rectangle types
        private static GraphicsPath BuildRectPath(int x, int y, int width, int height)
        {
            var p = new GraphicsPath();
            if (width <= 0 || height <= 0) return p;
            var pts = new Point[]
            {
                new Point(x, y),
                new Point(x + width, y),
                new Point(x + width, y + height),
                new Point(x, y + height)
            };
            p.AddPolygon(pts);
            return p;
        }

        // Build rounded rectangle path using cubic Bezier arcs (no Rectangle usage)
        private static GraphicsPath BuildRoundedRectPathBezier(int x, int y, int width, int height, int radius)
        {
            var path = new GraphicsPath();
            if (width <= 0 || height <= 0)
                return path;

            int r = Math.Max(0, Math.Min(radius, Math.Min(width, height) / 2));
            if (r == 0)
                return BuildRectPath(x, y, width, height);

            float k = 0.5522847498307936f; // control point scale for quarter circle
            float rf = r;
            float ox = rf * k; // offset for control points
            float oy = rf * k;

            // Key points
            float left = x;
            float top = y;
            float right = x + width;
            float bottom = y + height;

            float tlx = left + rf;
            float trx = right - rf;
            float tly = top + rf;
            float bry = bottom - rf;

            // Start at top-left corner tangent point
            path.StartFigure();
            path.AddLine(new PointF(tlx, top), new PointF(trx, top)); // top edge
            // top-right corner
            path.AddBezier(new PointF(trx, top), new PointF(trx + ox, top), new PointF(right, top + oy), new PointF(right, tly));
            path.AddLine(new PointF(right, tly), new PointF(right, bry)); // right edge
            // bottom-right corner
            path.AddBezier(new PointF(right, bry), new PointF(right, bry + oy), new PointF(trx + ox, bottom), new PointF(trx, bottom));
            path.AddLine(new PointF(trx, bottom), new PointF(tlx, bottom)); // bottom edge
            // bottom-left corner
            path.AddBezier(new PointF(tlx, bottom), new PointF(tlx - ox, bottom), new PointF(left, bry + oy), new PointF(left, bry));
            path.AddLine(new PointF(left, bry), new PointF(left, tly)); // left edge
            // top-left corner
            path.AddBezier(new PointF(left, tly), new PointF(left, tly - oy), new PointF(tlx - ox, top), new PointF(tlx, top));
            path.CloseFigure();

            return path;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers
{
    // Move public types outside the helper class for accessibility
    public delegate void DrawExternalHandler(Graphics parentGraphics, Rectangle childBounds);

    public class ExternalDrawingFunction : IDisposable
    {
        public Control? ChildControl { get; set; }
        public DrawExternalHandler? Handler { get; set; }
        public DrawingLayer Layer { get; set; }
        public bool Redraw { get; set; } = true;

        public ExternalDrawingFunction(DrawExternalHandler handler, DrawingLayer layer)
        {
            Handler = handler;
            Layer = layer;
        }

        public bool IsValid => Handler is not null;

        public void Invoke(Graphics g, Rectangle childBounds) => Handler?.Invoke(g, childBounds);

        public void Clear() => Handler = null;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) Handler = null;
        }
    }

    internal class ControlExternalDrawingHelper
    {
        private readonly Control _owner;
        // Support multiple external drawing functions per child (e.g., label before, badge after)
        private readonly Dictionary<Control, List<ExternalDrawingFunction>> _childExternalDrawers = new();

        public ControlExternalDrawingHelper(Control owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        #region Properties
        public DrawingLayer ExternalDrawingLayer { get; set; } = DrawingLayer.AfterAll;
        #endregion

        #region External Drawing Management
        public void AddChildExternalDrawing(Control child, DrawExternalHandler handler, DrawingLayer layer = DrawingLayer.AfterAll)
        {
            if (child == null) throw new ArgumentNullException(nameof(child));
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            var drawingFunction = new ExternalDrawingFunction(handler, layer)
            {
                ChildControl = child,
                Redraw = true
            };

            if (!_childExternalDrawers.TryGetValue(child, out var list))
            {
                list = new List<ExternalDrawingFunction>();
                _childExternalDrawers.Add(child, list);
            }

            // Allow multiple functions per layer; avoid exact duplicates of the same handler+layer
            bool exists = list.Exists(f => f.Layer == layer && f.Handler == handler);
            if (!exists)
            {
                list.Add(drawingFunction);
            }
        }

        public void SetChildExternalDrawingRedraw(Control child, bool redraw)
        {
            if (child != null && _childExternalDrawers.TryGetValue(child, out var list))
            {
                foreach (var function in list)
                {
                    function.Redraw = redraw;
                }
            }
        }

        public void ClearChildExternalDrawing(Control child)
        {
            if (child == null) return;

            if (_childExternalDrawers.TryGetValue(child, out var list))
            {
                foreach (var function in list)
                {
                    function.Dispose();
                }
                _childExternalDrawers.Remove(child);
                if (!_owner.IsDisposed && !_owner.Disposing)
                {
                    _owner.Invalidate();
                }
            }
        }

        public void ClearAllChildExternalDrawing()
        {
            foreach (var list in _childExternalDrawers.Values)
            {
                foreach (var function in list)
                {
                    function.Dispose();
                }
            }

            _childExternalDrawers.Clear();
            if (!_owner.IsDisposed && !_owner.Disposing)
            {
                _owner.Invalidate();
            }
        }

        public void PerformExternalDrawing(Graphics g, DrawingLayer layer)
        {
            if (_childExternalDrawers == null) return;

            var staleChildren = new List<Control>();
            foreach (var kvp in _childExternalDrawers)
            {
                Control childControl = kvp.Key;
                var list = kvp.Value;

                if (childControl == null || childControl.IsDisposed || childControl.Disposing)
                {
                    staleChildren.Add(childControl);
                    continue;
                }

                if (!childControl.Visible) continue;

                foreach (var drawingFunction in list)
                {
                    if (drawingFunction.Layer == layer && drawingFunction.Redraw && drawingFunction.IsValid)
                    {
                        try
                        {
                            drawingFunction.Invoke(g, childControl.Bounds);
                        }
                        catch
                        {
                            // Ignore child drawing errors to keep parent painting stable.
                        }
                    }
                }
            }

            if (staleChildren.Count > 0)
            {
                foreach (var stale in staleChildren)
                {
                    if (stale != null)
                    {
                        _childExternalDrawers.Remove(stale);
                    }
                }
            }
        }
        #endregion

        #region Batch Registration
        public void AddExternalDrawingForAll(Func<Control, bool> predicate, DrawExternalHandler handler, DrawingLayer layer = DrawingLayer.AfterAll)
        {
            if (predicate is null || handler is null) return;

            foreach (Control child in _owner.Controls)
            {
                if (child is null || child.IsDisposed || child.Disposing) continue;
                if (predicate(child))
                    AddChildExternalDrawing(child, handler, layer);
            }
        }

        public void AddExternalDrawingForAll<T>(DrawExternalHandler handler, DrawingLayer layer = DrawingLayer.AfterAll) where T : Control
        {
            if (handler is null) return;
            AddExternalDrawingForAll(child => child is T, handler, layer);
        }

        public void AddExternalDrawingForAll(IEnumerable<string> childNames, DrawExternalHandler handler, DrawingLayer layer = DrawingLayer.AfterAll)
        {
            if (childNames is null || handler is null) return;
            var nameSet = new HashSet<string>(childNames, StringComparer.OrdinalIgnoreCase);
            AddExternalDrawingForAll(child => nameSet.Contains(child.Name), handler, layer);
        }

        public void ClearExternalDrawingForAll(Func<Control, bool> predicate)
        {
            if (predicate is null) return;

            var toClear = new List<Control>();
            foreach (var kvp in _childExternalDrawers)
            {
                if (predicate(kvp.Key))
                    toClear.Add(kvp.Key);
            }

            foreach (var child in toClear)
                ClearChildExternalDrawing(child);
        }
        #endregion

        #region Cleanup
        public void Dispose()
        {
            ClearAllChildExternalDrawing();
        }
        #endregion
    }
}
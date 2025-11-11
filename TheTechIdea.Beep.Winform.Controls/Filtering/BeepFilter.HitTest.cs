using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// BeepFilter - HitTest partial class
    /// Contains hit testing logic and mouse interaction handling
    /// </summary>
    public partial class BeepFilter
    {
        #region Hit Testing

        /// <summary>
        /// Performs hit test at the specified point
        /// </summary>
        /// <param name="point">Point in control coordinates</param>
        /// <returns>Hit area information or null if no hit</returns>
        public FilterHitArea? PerformHitTest(Point point)
        {
            if (_activePainter == null || _currentLayout == null)
                return null;

            return _activePainter.HitTest(point, _currentLayout);
        }

        /// <summary>
        /// Gets the currently hovered hit area
        /// </summary>
        public FilterHitArea? HoveredArea => _hoveredArea;

        /// <summary>
        /// Gets the currently pressed hit area
        /// </summary>
        public FilterHitArea? PressedArea => _pressedArea;

        /// <summary>
        /// Checks if a specific hit area is currently hovered
        /// </summary>
        public bool IsAreaHovered(string areaName)
        {
            return _hoveredArea != null && _hoveredArea.Name == areaName;
        }

        /// <summary>
        /// Checks if a specific hit area is currently pressed
        /// </summary>
        public bool IsAreaPressed(string areaName)
        {
            return _pressedArea != null && _pressedArea.Name == areaName;
        }

        /// <summary>
        /// Gets hit area by name
        /// </summary>
        public FilterHitArea? GetHitAreaByName(string name)
        {
            if (_activePainter == null || _currentLayout == null)
                return null;

            // Search through all potential hit areas in the layout
            // This is a helper method for testing/debugging
            
            // Check action buttons
            if (name == "AddFilter" && !_currentLayout.AddFilterButtonRect.IsEmpty)
                return new FilterHitArea { Name = "AddFilter", Bounds = _currentLayout.AddFilterButtonRect, Type = FilterHitAreaType.AddFilterButton };
            
            if (name == "AddGroup" && !_currentLayout.AddGroupButtonRect.IsEmpty)
                return new FilterHitArea { Name = "AddGroup", Bounds = _currentLayout.AddGroupButtonRect, Type = FilterHitAreaType.AddGroupButton };
            
            if (name == "ClearAll" && !_currentLayout.ClearAllButtonRect.IsEmpty)
                return new FilterHitArea { Name = "ClearAll", Bounds = _currentLayout.ClearAllButtonRect, Type = FilterHitAreaType.ClearAllButton };
            
            // Check tags
            for (int i = 0; i < _currentLayout.TagRects.Length; i++)
            {
                if (name == $"Tag_{i}")
                    return new FilterHitArea { Name = name, Bounds = _currentLayout.TagRects[i], Type = FilterHitAreaType.FilterTag, Tag = i };
                
                if (name == $"RemoveTag_{i}" && i < _currentLayout.RemoveButtonRects.Length)
                    return new FilterHitArea { Name = name, Bounds = _currentLayout.RemoveButtonRects[i], Type = FilterHitAreaType.RemoveButton, Tag = i };
            }

            return null;
        }

        /// <summary>
        /// Gets the bounds of a specific hit area type
        /// </summary>
        public Rectangle GetHitAreaBounds(FilterHitAreaType type, int index = 0)
        {
            if (_currentLayout == null)
                return Rectangle.Empty;

            switch (type)
            {
                case FilterHitAreaType.AddFilterButton:
                    return _currentLayout.AddFilterButtonRect;
                
                case FilterHitAreaType.AddGroupButton:
                    return _currentLayout.AddGroupButtonRect;
                
                case FilterHitAreaType.ClearAllButton:
                    return _currentLayout.ClearAllButtonRect;
                
                case FilterHitAreaType.ApplyButton:
                    return _currentLayout.ApplyButtonRect;
                
                case FilterHitAreaType.SaveButton:
                    return _currentLayout.SaveButtonRect;
                
                case FilterHitAreaType.LoadButton:
                    return _currentLayout.LoadButtonRect;
                
                case FilterHitAreaType.SearchInput:
                    return _currentLayout.SearchInputRect;
                
                case FilterHitAreaType.FilterTag:
                    if (index >= 0 && index < _currentLayout.TagRects.Length)
                        return _currentLayout.TagRects[index];
                    break;
                
                case FilterHitAreaType.RemoveButton:
                    if (index >= 0 && index < _currentLayout.RemoveButtonRects.Length)
                        return _currentLayout.RemoveButtonRects[index];
                    break;
                
                case FilterHitAreaType.DragHandle:
                    if (index >= 0 && index < _currentLayout.DragHandleRects.Length)
                        return _currentLayout.DragHandleRects[index];
                    break;
            }

            return Rectangle.Empty;
        }

        #endregion

        #region Hit Test Helpers

        /// <summary>
        /// Checks if a point is inside any filter tag
        /// </summary>
        public bool IsPointInAnyTag(Point point, out int tagIndex)
        {
            tagIndex = -1;
            
            if (_currentLayout == null || _currentLayout.TagRects == null)
                return false;

            for (int i = 0; i < _currentLayout.TagRects.Length; i++)
            {
                if (_currentLayout.TagRects[i].Contains(point))
                {
                    tagIndex = i;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if a point is inside any action button
        /// </summary>
        public bool IsPointInActionButton(Point point, out FilterHitAreaType buttonType)
        {
            buttonType = FilterHitAreaType.None;
            
            if (_currentLayout == null)
                return false;

            if (_currentLayout.AddFilterButtonRect.Contains(point))
            {
                buttonType = FilterHitAreaType.AddFilterButton;
                return true;
            }

            if (_currentLayout.AddGroupButtonRect.Contains(point))
            {
                buttonType = FilterHitAreaType.AddGroupButton;
                return true;
            }

            if (_currentLayout.ClearAllButtonRect.Contains(point))
            {
                buttonType = FilterHitAreaType.ClearAllButton;
                return true;
            }

            if (_currentLayout.ApplyButtonRect.Contains(point))
            {
                buttonType = FilterHitAreaType.ApplyButton;
                return true;
            }

            return false;
        }

        #endregion
    }
}

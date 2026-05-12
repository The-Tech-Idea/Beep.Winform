using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using BeepTreeSortDirection = TheTechIdea.Beep.Winform.Controls.Trees.Models.BeepTreeSortDirection;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Helpers
{
    /// <summary>
    /// Provides layout persistence for BeepTree including column state,
    /// sort state, expanded nodes, and scroll position.
    /// </summary>
    public class BeepTreeLayoutPersistence
    {
        private readonly BeepTree _owner;

        public BeepTreeLayoutPersistence(BeepTree owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        #region Save Layout

        /// <summary>
        /// Saves the current layout to a file.
        /// </summary>
        public void SaveLayoutToFile(string filePath)
        {
            var layout = CaptureLayout();
            var json = JsonSerializer.Serialize(layout, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Saves the current layout to a stream.
        /// </summary>
        public void SaveLayoutToStream(Stream stream)
        {
            var layout = CaptureLayout();
            var json = JsonSerializer.Serialize(layout, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            using var writer = new StreamWriter(stream);
            writer.Write(json);
        }

        /// <summary>
        /// Saves the current layout to a JSON string.
        /// </summary>
        public string SaveLayoutToString()
        {
            var layout = CaptureLayout();
            return JsonSerializer.Serialize(layout, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
        }

        private BeepTreeLayoutData CaptureLayout()
        {
            var layout = new BeepTreeLayoutData
            {
                Version = 1,
                SavedAt = DateTime.Now
            };

            // Column layout
            if (_owner.Columns != null)
            {
                layout.Columns = _owner.Columns.Select(c => new ColumnLayoutData
                {
                    Name = c.Name,
                    Width = c.Width,
                    Visible = c.Visible,
                    SortDirection = c.SortDirection.ToString(),
                    SortOrder = c.SortOrder
                }).ToList();
            }

            // Expanded nodes
            layout.ExpandedNodes = _owner.Nodes
                .SelectMany(n => GetExpandedNodes(n))
                .ToList();

            // Selected node
            layout.SelectedNodeGuid = _owner.SelectedNode?.GuidId;

            // Scroll position
            layout.ScrollPosition = new ScrollPositionData
            {
                X = _owner.XOffset,
                Y = _owner.YOffset
            };

            return layout;
        }

        private IEnumerable<string> GetExpandedNodes(SimpleItem node)
        {
            if (node == null)
                yield break;

            if (node.IsExpanded)
                yield return node.GuidId;

            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    foreach (var expanded in GetExpandedNodes(child))
                    {
                        yield return expanded;
                    }
                }
            }
        }

        #endregion

        #region Load Layout

        /// <summary>
        /// Loads a layout from a file.
        /// </summary>
        public void LoadLayoutFromFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            LoadLayoutFromString(json);
        }

        /// <summary>
        /// Loads a layout from a stream.
        /// </summary>
        public void LoadLayoutFromStream(Stream stream)
        {
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            LoadLayoutFromString(json);
        }

        /// <summary>
        /// Loads a layout from a JSON string.
        /// </summary>
        public void LoadLayoutFromString(string json)
        {
            if (string.IsNullOrEmpty(json))
                return;

            var layout = JsonSerializer.Deserialize<BeepTreeLayoutData>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (layout == null)
                return;

            ApplyLayout(layout);
        }

        private void ApplyLayout(BeepTreeLayoutData layout)
        {
            // Apply column layout
            if (layout.Columns != null && _owner.Columns != null)
            {
                // Reorder columns based on saved order
                var columnMap = _owner.Columns.ToDictionary(c => c.Name, c => c);
                var orderedColumns = new List<BeepTreeColumn>();

                foreach (var colData in layout.Columns)
                {
                    if (columnMap.TryGetValue(colData.Name, out var column))
                    {
                        column.Width = colData.Width;
                        column.Visible = colData.Visible;

                        if (Enum.TryParse<BeepTreeSortDirection>(colData.SortDirection, out var sortDir))
                        {
                            column.SortDirection = sortDir;
                        }
                        column.SortOrder = colData.SortOrder;

                        orderedColumns.Add(column);
                        columnMap.Remove(colData.Name);
                    }
                }

                // Add any remaining columns not in the saved layout
                orderedColumns.AddRange(columnMap.Values);

                // Update the collection order
                _owner.Columns.Clear();
                foreach (var col in orderedColumns)
                {
                    _owner.Columns.Add(col);
                }
            }

            // Apply expanded state
            if (layout.ExpandedNodes != null)
            {
                var expandedSet = new HashSet<string>(layout.ExpandedNodes);
                SetExpandedState(_owner.Nodes, expandedSet);
            }

            // Apply selection
            if (!string.IsNullOrEmpty(layout.SelectedNodeGuid))
            {
                var selectedNode = FindNodeByGuid(_owner.Nodes, layout.SelectedNodeGuid);
                if (selectedNode != null)
                {
                    _owner.SelectedNode = selectedNode;
                }
            }

            // Apply scroll position
            if (layout.ScrollPosition != null)
            {
                // Scroll position will be applied after rebuild
                _owner.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        _owner.ScrollBy(layout.ScrollPosition.X, layout.ScrollPosition.Y);
                    }
                    catch { }
                }));
            }

            _owner.RefreshTree();
        }

        private void SetExpandedState(IEnumerable<SimpleItem> nodes, HashSet<string> expandedSet)
        {
            foreach (var node in nodes)
            {
                if (node == null) continue;

                node.IsExpanded = expandedSet.Contains(node.GuidId);

                if (node.Children != null)
                {
                    SetExpandedState(node.Children, expandedSet);
                }
            }
        }

        private SimpleItem FindNodeByGuid(IEnumerable<SimpleItem> nodes, string guid)
        {
            foreach (var node in nodes)
            {
                if (node == null) continue;

                if (node.GuidId == guid)
                    return node;

                if (node.Children != null)
                {
                    var found = FindNodeByGuid(node.Children, guid);
                    if (found != null)
                        return found;
                }
            }
            return null;
        }

        #endregion
    }

    #region Layout Data Classes

    /// <summary>
    /// Represents the complete layout state of a BeepTree.
    /// </summary>
    public class BeepTreeLayoutData
    {
        public int Version { get; set; }
        public DateTime SavedAt { get; set; }
        public List<ColumnLayoutData> Columns { get; set; }
        public List<string> ExpandedNodes { get; set; }
        public string SelectedNodeGuid { get; set; }
        public ScrollPositionData ScrollPosition { get; set; }
    }

    /// <summary>
    /// Represents the layout state of a single column.
    /// </summary>
    public class ColumnLayoutData
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public bool Visible { get; set; }
        public string SortDirection { get; set; }
        public int SortOrder { get; set; }
    }

    /// <summary>
    /// Represents the scroll position.
    /// </summary>
    public class ScrollPositionData
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    #endregion
}

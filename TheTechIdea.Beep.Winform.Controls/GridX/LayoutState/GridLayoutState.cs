using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.GridX.LayoutState
{
    /// <summary>
    /// Persisted, versioned layout for one grid. Column entries are keyed by
    /// <see cref="GridColumnLayout.ColumnName"/> so a schema change does not
    /// shift meanings onto the wrong column.
    /// </summary>
    public sealed class GridLayoutState
    {
        public const int CurrentVersion = 1;

        public int Version { get; set; } = CurrentVersion;
        public string GridKey { get; set; } = string.Empty;
        public DateTimeOffset CapturedAtUtc { get; set; } = DateTimeOffset.UtcNow;
        public List<GridColumnLayout> Columns { get; set; } = new();
    }

    public sealed class GridColumnLayout
    {
        public string ColumnName { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public int Width { get; set; } = 100;
        public bool Sticked { get; set; }
        public bool Visible { get; set; } = true;
    }

    /// <summary>
    /// Storage abstraction for <see cref="GridLayoutState"/>. A host can back
    /// this with a settings table or a database; the grid never invents a user
    /// identity, so the key is supplied by the caller.
    /// </summary>
    public interface IGridLayoutStateStore
    {
        GridLayoutState? Load(string key);
        bool Save(string key, GridLayoutState state);
        void Delete(string key);
    }
}

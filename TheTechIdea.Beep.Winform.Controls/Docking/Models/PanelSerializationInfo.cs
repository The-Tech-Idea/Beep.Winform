using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Models
{
    /// <summary>
    /// Serializable snapshot of a single panel's state.
    /// Used for layout persistence (save/restore to JSON or other formats).
    /// </summary>
    [Serializable]
    public struct PanelSerializationInfo
    {
        /// <summary>
        /// Unique identifier of the panel.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Display title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Icon file path.
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// Current state (Docked, Floating, AutoHidden, Closed).
        /// </summary>
        public DockPanelState State { get; set; }

        /// <summary>
        /// Docking position.
        /// </summary>
        public DockPosition DockPosition { get; set; }

        /// <summary>
        /// The ID of the group this panel belongs to.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Preferred width.
        /// </summary>
        public int PreferredWidth { get; set; }

        /// <summary>
        /// Preferred height.
        /// </summary>
        public int PreferredHeight { get; set; }

        /// <summary>
        /// Z-order index within the group (0 = topmost).
        /// </summary>
        public int TabIndex { get; set; }

        /// <summary>
        /// Whether this panel is the active panel in its group.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Whether the panel's content is dirty.
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// Cached bounds at time of serialization.
        /// </summary>
        public Rectangle Bounds { get; set; }

        /// <summary>
        /// Timestamp when this snapshot was created (UTC).
        /// </summary>
        public DateTime SnapshotUtc { get; set; }

        /// <summary>
        /// Whether this panel can be closed.
        /// </summary>
        public bool CanClose { get; set; }

        /// <summary>
        /// Whether this panel can be floated.
        /// </summary>
        public bool CanFloat { get; set; }

        /// <summary>
        /// Whether this panel can be auto-hidden.
        /// </summary>
        public bool CanAutoHide { get; set; }

        /// <summary>
        /// Creates a serialization snapshot from a DockPanel.
        /// </summary>
        public static PanelSerializationInfo FromPanel(DockPanel panel, int tabIndex = 0)
        {
            if (panel == null)
                throw new ArgumentNullException(nameof(panel));

            return new PanelSerializationInfo
            {
                Key = panel.Key,
                Title = panel.Title,
                IconPath = panel.IconPath,
                State = panel.State,
                DockPosition = panel.DockPosition,
                GroupId = panel.Group?.Id,
                PreferredWidth = panel.PreferredWidth,
                PreferredHeight = panel.PreferredHeight,
                TabIndex = tabIndex,
                IsActive = panel.Group?.ActivePanel == panel,
                IsDirty = panel.IsDirty,
                Bounds = panel.LayoutBounds,
                SnapshotUtc = DateTime.UtcNow,
                CanClose = panel.CanClose,
                CanFloat = panel.CanFloat,
                CanAutoHide = panel.CanAutoHide
            };
        }

        /// <summary>
        /// Applies this serialization snapshot to a DockPanel.
        /// </summary>
        public void ApplyToPanel(DockPanel panel)
        {
            if (panel == null)
                throw new ArgumentNullException(nameof(panel));

            panel.Title = Title ?? panel.Title;
            panel.IconPath = IconPath ?? panel.IconPath;
            panel.State = State;
            panel.DockPosition = DockPosition;
            panel.PreferredWidth = PreferredWidth;
            panel.PreferredHeight = PreferredHeight;
            panel.IsDirty = IsDirty;
            panel.CanClose = CanClose;
            panel.CanFloat = CanFloat;
            panel.CanAutoHide = CanAutoHide;
        }

        /// <summary>
        /// Gets a diagnostic string for this serialization snapshot.
        /// </summary>
        public override string ToString()
        {
            return $"PanelSerializationInfo[Key={Key}, Title={Title}, State={State}, Group={GroupId}]";
        }
    }
}

using System;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Models
{
    /// <summary>
    /// User status enum
    /// </summary>
    public enum UserStatus
    {
        Online,
        Offline,
        Away,
        Busy
    }

    /// <summary>
    /// Media item for media widgets
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MediaItem
    {
        [Category("Data")]
        [Description("Unique identifier for the media item")]
        public string Id { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Path to the full-size image")]
        public string ImagePath { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Path to the thumbnail image")]
        public string ThumbnailPath { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Caption or description")]
        public string Caption { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Width of the image in pixels")]
        public int Width { get; set; } = 0;

        [Category("Data")]
        [Description("Height of the image in pixels")]
        public int Height { get; set; } = 0;

        public override string ToString() => Caption;
    }

    /// <summary>
    /// Avatar item for avatar lists
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AvatarItem
    {
        [Category("Data")]
        [Description("User ID")]
        public string UserId { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Path to avatar image")]
        public string ImagePath { get; set; } = string.Empty;

        [Category("Data")]
        [Description("User status")]
        public UserStatus Status { get; set; } = UserStatus.Offline;

        [Category("Data")]
        [Description("Display name")]
        public string DisplayName { get; set; } = string.Empty;

        public override string ToString() => DisplayName;
    }
}

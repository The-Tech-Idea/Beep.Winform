using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Models
{
    /// <summary>
    /// Chat message for social widgets
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ChatMessage
    {
        [Category("Data")]
        [Description("User ID of the message sender")]
        public string SenderId { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Display name of the message sender")]
        public string SenderName { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Path to sender avatar image")]
        public string SenderAvatar { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Message content")]
        public string Content { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Timestamp when the message was sent")]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [Category("Behavior")]
        [Description("Message status (Sending, Sent, Delivered, Read, Failed)")]
        public MessageStatus Status { get; set; } = MessageStatus.Sent;

        [Category("Behavior")]
        [Description("Message type (Text, Image, File, System)")]
        public MessageType Type { get; set; } = MessageType.Text;

        public override string ToString() => $"{SenderName}: {Content}";
    }

    /// <summary>
    /// Message status enum
    /// </summary>
    public enum MessageStatus
    {
        Sending,
        Sent,
        Delivered,
        Read,
        Failed
    }

    /// <summary>
    /// Message type enum
    /// </summary>
    public enum MessageType
    {
        Text,
        Image,
        File,
        System
    }

    /// <summary>
    /// Comment for social widgets
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Comment
    {
        [Category("Data")]
        [Description("Unique identifier for the comment")]
        public string Id { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Author name")]
        public string AuthorName { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Path to author avatar image")]
        public string AuthorAvatar { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Comment text content")]
        public string Content { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Timestamp when the comment was created")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Category("Data")]
        [Description("Timestamp when the comment was edited (null if never edited)")]
        public DateTime? EditedAt { get; set; }

        [Category("Data")]
        [Description("Vote score (upvotes - downvotes)")]
        public int VoteScore { get; set; } = 0;

        [Category("Behavior")]
        [Description("User's vote on this comment")]
        public VoteType UserVote { get; set; } = VoteType.None;

        [Category("Data")]
        [Description("Replies to this comment")]
        public List<Comment> Replies { get; set; } = new List<Comment>();

        [Category("Behavior")]
        [Description("Whether current user is the author")]
        public bool IsAuthor { get; set; } = false;

        [Category("Behavior")]
        [Description("Whether current user can edit this comment")]
        public bool CanEdit { get; set; } = false;

        [Category("Behavior")]
        [Description("Whether this comment has been deleted")]
        public bool IsDeleted { get; set; } = false;

        public override string ToString() => $"{AuthorName}: {Content}";
    }

    /// <summary>
    /// Vote type enum
    /// </summary>
    public enum VoteType
    {
        None,
        Upvote,
        Downvote
    }

    /// <summary>
    /// Activity item for activity streams
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ActivityItem
    {
        [Category("Data")]
        [Description("Type of activity")]
        public ActivityType Type { get; set; } = ActivityType.View;

        [Category("Data")]
        [Description("User name who performed the activity")]
        public string UserName { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Path to user avatar image")]
        public string UserAvatar { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Action performed (e.g., 'created', 'updated', 'deleted')")]
        public string Action { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Target of the action (e.g., 'document', 'task')")]
        public string Target { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Additional details about the activity")]
        public string Details { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Timestamp when the activity occurred")]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public override string ToString() => $"{UserName} {Action} {Target}";
    }

    /// <summary>
    /// Activity type enum
    /// </summary>
    public enum ActivityType
    {
        Login,
        Logout,
        Create,
        Update,
        Delete,
        View,
        Download,
        Upload,
        Share,
        Comment
    }

    /// <summary>
    /// Chat participant model.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ChatParticipant
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public DateTime LastSeen { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Contact status for contact-card widgets.
    /// </summary>
    public enum ContactStatus
    {
        Online,
        Away,
        Busy,
        Offline
    }

    /// <summary>
    /// Strongly typed contact info for contact-card widgets.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ContactInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string AvatarPath { get; set; } = string.Empty;
        public ContactStatus Status { get; set; } = ContactStatus.Offline;
        public DateTime? LastContact { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsNewContact { get; set; }
        public string Notes { get; set; } = string.Empty;
        public Color AccentColor { get; set; } = Color.Empty;
    }
}

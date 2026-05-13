using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Controls where a newly opened document should be placed.
    /// </summary>
    public enum DocumentOpenTarget
    {
        /// <summary>
        /// Open in the primary group.
        /// </summary>
        PrimaryGroup,

        /// <summary>
        /// Open in the currently active group.
        /// </summary>
        ActiveGroup,

        /// <summary>
        /// Open in the group identified by <see cref="DocumentOpenOptions.TargetGroupId"/>.
        /// Falls back to the active group when the id is not valid.
        /// </summary>
        SpecificGroup
    }

    /// <summary>
    /// Options applied when opening a document into a <see cref="BeepDocumentHost"/>.
    /// </summary>
    public sealed class DocumentOpenOptions
    {
        /// <summary>
        /// Controls which group receives the newly opened document.
        /// Defaults to the active group for a more natural MDI workflow.
        /// </summary>
        [DefaultValue(DocumentOpenTarget.ActiveGroup)]
        public DocumentOpenTarget Target { get; set; } = DocumentOpenTarget.ActiveGroup;

        /// <summary>
        /// Optional target group id used when <see cref="Target"/> is
        /// <see cref="DocumentOpenTarget.SpecificGroup"/>.
        /// </summary>
        [DefaultValue(null)]
        public string? TargetGroupId { get; set; }

        /// <summary>
        /// When true, the document becomes the active document after it is opened.
        /// </summary>
        [DefaultValue(true)]
        public bool Activate { get; set; } = true;
    }
}
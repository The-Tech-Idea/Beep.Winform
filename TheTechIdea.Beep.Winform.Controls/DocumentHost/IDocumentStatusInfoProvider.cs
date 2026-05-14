using System;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Features;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Optional contract for document content controls that can surface rich
    /// status information such as cursor position, selection size, encoding, or zoom.
    /// </summary>
    public interface IDocumentStatusInfoProvider
    {
        /// <summary>
        /// Raised when the current status payload changes and any connected shell
        /// chrome should refresh.
        /// </summary>
        event EventHandler? StatusInfoChanged;

        /// <summary>
        /// Returns the current status payload for the active document content.
        /// </summary>
        StatusBarInfo GetStatusInfo();
    }
}
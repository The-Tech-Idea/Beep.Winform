using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.FlyoutMenus
{
    /// <summary>
    /// Hosts any <see cref="Control"/> in a non-modal, anchored flyout.
    /// Contrast with <see cref="TheTechIdea.Beep.Vis.Modules.IDialogManager"/>: dialogs are modal
    /// and return a result; flyouts are transient popovers anchored to a trigger that dismiss when
    /// the pointer leaves the anchor/popup cascade or the owner calls <see cref="Close"/>.
    /// </summary>
    public interface IFlyoutManager : IDisposable
    {
        /// <summary>True while a flyout is open.</summary>
        bool IsOpen { get; }

        /// <summary>
        /// Shows <paramref name="content"/> in a flyout anchored to <paramref name="anchor"/>.
        /// A null anchor anchors to the current cursor position. Any already-open flyout is closed
        /// first, so at most one flyout is open at a time.
        /// </summary>
        void Show(Control content, Control? anchor, BeepPopupFormPosition position = BeepPopupFormPosition.Bottom, Size? size = null);

        /// <summary>Closes the active flyout, if any.</summary>
        void Close();
    }
}

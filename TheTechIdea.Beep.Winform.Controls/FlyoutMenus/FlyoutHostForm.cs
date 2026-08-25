using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.FlyoutMenus
{
    /// <summary>
    /// Internal popup form that hosts a single content control for <see cref="FlyoutManager"/>.
    /// Lifetime, anchoring and dismissal are owned by the manager and the <see cref="BeepPopupForm"/>
    /// base (outside-click cascade close).
    /// </summary>
    internal sealed class FlyoutHostForm : BeepPopupForm
    {
        public FlyoutHostForm()
        {
            InPopMode = true;
            CloseOnSelection = false; // flyouts dismiss via outside-click or the manager, not selection
            EnableAnimation = false;  // synchronous close so the manager can dispose the host deterministically
        }

        public void Host(Control content)
        {
            Controls.Clear();
            content.Dock = DockStyle.Fill;
            content.Visible = true;
            Controls.Add(content);
        }
    }
}

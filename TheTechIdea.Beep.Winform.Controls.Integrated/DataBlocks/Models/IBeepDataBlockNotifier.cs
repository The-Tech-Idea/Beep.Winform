using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Models
{
    public interface IBeepDataBlockNotifier
    {
        void ShowInfo(string message, string caption = "Information");
        void ShowWarning(string message, string caption = "Warning");
        void ShowError(string message, string caption = "Error");
    }

    public sealed class MessageBoxBeepDataBlockNotifier : IBeepDataBlockNotifier
    {
        public void ShowInfo(string message, string caption = "Information")
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ShowWarning(string message, string caption = "Warning")
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void ShowError(string message, string caption = "Error")
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

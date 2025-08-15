

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    public static class AnimationExtensions
    {
        // This method helps make animations smoother by properly processing window messages
        public static void DoEvents()
        {
            Application.DoEvents();
        }
    }
}

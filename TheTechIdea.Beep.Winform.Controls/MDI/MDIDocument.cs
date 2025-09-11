using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.MDI
{
    public class MDIDocument
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public Control Content { get; set; }
        public bool CanClose { get; set; } = true;
        public bool IsVisible { get; set; } = true;
        public Rectangle TabBounds { get; set; }
        public bool IsCloseHovered { get; set; }
        public float AnimationProgress { get; set; }
        public float TargetAnimationProgress { get; set; }
        public Image Icon { get; set; }
        public bool IsPinned { get; set; }
        public bool IsDirty { get; set; }
        public object Tag { get; set; }
    }
}

using System.Drawing;
using TheTechIdea.Beep.Addin;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    #region Supporting Classes and Enums

    public class AddinTab
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public IDM_Addin Addin { get; set; }
        public Rectangle Bounds { get; set; }
        public bool IsVisible { get; set; } = true;
        public bool CanClose { get; set; } = true;
        public bool IsCloseHovered { get; set; }
        public float AnimationProgress { get; set; }
        public float TargetAnimationProgress { get; set; }
    }

    public enum ContainerDisplayMode
    {
        Tabbed,
        Tiles,
        List,
        Accordion,
        Stack,
        Single
    }

    public enum TabPosition
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public enum AnimationSpeed
    {
        Slow,
        Normal,
        Fast
    }

    public enum ArrowDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    #endregion
}


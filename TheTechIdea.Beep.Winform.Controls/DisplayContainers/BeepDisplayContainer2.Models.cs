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

        /// <summary>
        /// Optional SVG/image resource path rendered via StyledImagePainter left of the title.
        /// Use SvgsUI constants, e.g. SvgsUI.File, SvgsUI.Database.
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// Short notification badge text (e.g. "3", "!", "NEW") rendered as a pill on the tab.
        /// Null or empty to hide the badge.
        /// </summary>
        public string BadgeText { get; set; }

        /// <summary>
        /// Badge pill background colour.  When not set (Color.Empty), the theme accent colour is used.
        /// </summary>
        public Color BadgeColor { get; set; } = Color.Empty;

        /// <summary>
        /// Optional tooltip description shown in a hover card after a short delay.
        /// </summary>
        public string TooltipText { get; set; }

        /// <summary>
        /// When true the tab is rendered compactly (icon-only), grouped to the left,
        /// and the close button is hidden.
        /// </summary>
        public bool IsPinned { get; set; }
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


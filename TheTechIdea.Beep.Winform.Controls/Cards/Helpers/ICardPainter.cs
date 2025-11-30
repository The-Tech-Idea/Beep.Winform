using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// Interface for card painters. Each CardStyle has its own distinct painter implementation.
    /// No base class - each painter is completely self-contained.
    /// </summary>
    internal interface ICardPainter : IDisposable
    {
        void Initialize(BaseControl owner, IBeepTheme theme);
        LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx);
        void DrawBackground(Graphics g, LayoutContext ctx);
        void DrawForegroundAccents(Graphics g, LayoutContext ctx);
        void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit);
    }

    internal sealed class LayoutContext
    {
        // Basic layout rectangles
        public Rectangle DrawingRect;
        public Rectangle ImageRect;
        public Rectangle HeaderRect;
        public Rectangle ParagraphRect;
        public Rectangle ButtonRect;
        public Rectangle SecondaryButtonRect;
        
        // Additional layout areas for new styles
        public Rectangle SubtitleRect;
        public Rectangle StatusRect;
        public Rectangle RatingRect;
        public Rectangle BadgeRect;
        public Rectangle TagsRect;
        public Rectangle AvatarRect; // For testimonials/profiles
        
        // Display flags
        public bool ShowImage;
        public bool ShowButton;
        public bool ShowSecondaryButton;
        public bool ShowStatus;
        public bool ShowRating;
        
        // Styling properties
        public int Radius;
        public Color AccentColor;
        
        // Content properties
        public List<string> Tags = new List<string>();
        public string BadgeText1 = string.Empty;
        public Color Badge1BackColor;
        public Color Badge1ForeColor;
        public string BadgeText2 = string.Empty;
        public Color Badge2BackColor;
        public Color Badge2ForeColor;
        public string SubtitleText = string.Empty;
        public string StatusText = string.Empty;
        public Color StatusColor;
        public int Rating; // 0-5 stars
    }
}

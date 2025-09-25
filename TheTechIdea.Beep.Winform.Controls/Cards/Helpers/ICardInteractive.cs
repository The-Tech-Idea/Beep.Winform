using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    internal readonly struct CardHitArea
    {
        public string Name { get; }
        public Rectangle Rect { get; }
        public CardHitArea(string name, Rectangle rect) { Name = name; Rect = rect; }
    }

    internal interface ICardInteractive
    {
        IEnumerable<CardHitArea> GetHitAreas(LayoutContext ctx);
    }
}

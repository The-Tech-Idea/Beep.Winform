using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
        internal sealed class BeepiFormProLayoutManager
        {
            private readonly BeepiFormPro _owner;
            public Rectangle CaptionRect { get; private set; }
            public Rectangle ContentRect { get; private set; }
            public Rectangle BottomRect { get; private set; }
            public Rectangle LeftRect { get; private set; }
            public Rectangle RightRect { get; private set; }

            // Caption zones (for flexible alignment)
            public Rectangle LeftZoneRect { get; private set; }
            public Rectangle CenterZoneRect { get; private set; }
            public Rectangle RightZoneRect { get; private set; }

            // System button rects
            public Rectangle IconRect { get; private set; }
            public Rectangle TitleRect { get; private set; }
            public Rectangle ThemeButtonRect { get; private set; }
            public Rectangle StyleButtonRect { get; private set; }
            public Rectangle SearchButtonRect { get; private set; }
            public Rectangle ProfileButtonRect { get; private set; }
            public Rectangle MailButtonRect { get; private set; }
            public Rectangle CustomActionButtonRect { get; private set; }
            public Rectangle MinimizeButtonRect { get; private set; }
            public Rectangle MaximizeButtonRect { get; private set; }
            public Rectangle CloseButtonRect { get; private set; }

            public BeepiFormProLayoutManager(BeepiFormPro owner) { _owner = owner; }

       
        
        }
}

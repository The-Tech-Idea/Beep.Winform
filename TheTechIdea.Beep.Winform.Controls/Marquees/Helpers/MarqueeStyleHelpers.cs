using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Helpers
{
    public static class MarqueeStyleHelpers
    {
        public static int GetBorderRadius(BeepControlStyle controlStyle, int controlHeight)
        {
            return BeepStyling.GetRadius(controlStyle);
        }

        public static Padding GetRecommendedPadding(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => new Padding(8, 4, 8, 4),
                BeepControlStyle.iOS15 => new Padding(10, 6, 10, 6),
                BeepControlStyle.Fluent2 => new Padding(8, 4, 8, 4),
                BeepControlStyle.Minimal => new Padding(6, 3, 6, 3),
                _ => new Padding(8, 4, 8, 4)
            };
        }

        public static int GetRecommendedMinimumHeight(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => 40,
                BeepControlStyle.iOS15 => 44,
                BeepControlStyle.Fluent2 => 42,
                BeepControlStyle.Minimal => 36,
                _ => 40
            };
        }

        public static int GetRecommendedComponentSpacing(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => 20,
                BeepControlStyle.iOS15 => 24,
                BeepControlStyle.Fluent2 => 22,
                BeepControlStyle.Minimal => 16,
                _ => 20
            };
        }

        public static float GetRecommendedScrollSpeed(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => 2.0f,
                BeepControlStyle.iOS15 => 2.5f,
                BeepControlStyle.Fluent2 => 2.2f,
                BeepControlStyle.Minimal => 1.8f,
                _ => 2.0f
            };
        }

        public static int GetRecommendedScrollInterval(BeepControlStyle controlStyle)
        {
            return controlStyle switch
            {
                BeepControlStyle.Material3 => 30,
                BeepControlStyle.iOS15 => 25,
                BeepControlStyle.Fluent2 => 28,
                BeepControlStyle.Minimal => 35,
                _ => 30
            };
        }

        public static bool ShouldShowShadow(BeepControlStyle controlStyle)
        {
            if (SystemInformation.HighContrast)
                return false;

            return controlStyle switch
            {
                BeepControlStyle.Material3 => true,
                BeepControlStyle.Fluent2 => true,
                BeepControlStyle.iOS15 => true,
                BeepControlStyle.Minimal => false,
                BeepControlStyle.Neumorphism => true,
                BeepControlStyle.GlassAcrylic => true,
                _ => false
            };
        }
    }
}

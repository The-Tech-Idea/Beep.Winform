using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars
{
    internal class BottomBarPainterContext
    {
        public Graphics Graphics { get; set; }
        public Rectangle Bounds { get; set; }
        public List<SimpleItem> Items { get; set; }
        public int SelectedIndex { get; set; }
        public int HoverIndex { get; set; }
        public ControlHitTestHelper HitTest { get; set; }
        public BeepBottomBarLayoutHelper LayoutHelper { get; set; }
        public ImagePainter ImagePainter { get; set; }
        public string DefaultImagePath { get; set; }
        public int CTAIndex { get; set; } = -1;
        public Color AccentColor { get; set; }
        public float AnimationPhase { get; set; } = 0f; // 0..1 float used by painters for pulsing/hover effects
        public Action<int, MouseButtons> OnItemClicked { get; set; }
        public float AnimatedIndicatorX { get; set; }
        public float AnimatedIndicatorWidth { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Models
{

public class GradientStop
    {
        public float Position { get; set; } // 0.0 to 1.0
        public Color Color { get; set; }

        public GradientStop(float position, Color color)
        {
            Position = Math.Max(0f, Math.Min(1f, position));
            Color = color;
        }
    }
}

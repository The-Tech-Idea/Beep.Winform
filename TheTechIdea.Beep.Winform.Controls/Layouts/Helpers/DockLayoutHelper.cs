using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    public static class DockLayoutHelper
    {
        public static Control Build(Control parent)
        {
            var main = new Panel { Dock = DockStyle.Fill };
            parent.Controls.Add(main);

            var top = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = SystemColors.ControlLight };
            var bottom = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = SystemColors.ControlDark };
            var left = new Panel { Dock = DockStyle.Left, Width = 100, BackColor = SystemColors.ControlLightLight };
            var right = new Panel { Dock = DockStyle.Right, Width = 100, BackColor = SystemColors.ControlLightLight };
            var fill = new Panel { Dock = DockStyle.Fill, BackColor = SystemColors.Window };

            main.Controls.Add(fill);
            main.Controls.Add(right);
            main.Controls.Add(left);
            main.Controls.Add(bottom);
            main.Controls.Add(top);
            return main;
        }
    }
}

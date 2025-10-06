using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    public static class SplitContainerLayoutHelper
    {
        public static Control Build(Control parent, Orientation orientation)
        {
            var split = new SplitContainer { Orientation = orientation, Dock = DockStyle.Fill };

            var p1 = new Panel { Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle };
            var p2 = new Panel { Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle };
            split.Panel1.Controls.Add(p1);
            split.Panel2.Controls.Add(p2);
            parent.Controls.Add(split);
            return split;
        }
    }
}

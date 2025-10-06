using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    public static class VerticalStackLayoutHelper
    {
        public static Control Build(Control parent)
        {
            var flow = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, WrapContents = false, Dock = DockStyle.Fill, AutoScroll = true };
            for (int i = 1; i <= 3; i++)
            {
                var btn = new Button { Text = $"Button {i}", AutoSize = true, Margin = new Padding(5) };
                flow.Controls.Add(btn);
            }
            parent.Controls.Add(flow);
            return flow;
        }
    }
}

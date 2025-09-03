namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepRibbonGroup : ToolStrip
    {
        public BeepRibbonGroup()
        {
            GripStyle = ToolStripGripStyle.Hidden;
            Dock = DockStyle.Top;
            RenderMode = ToolStripRenderMode.ManagerRenderMode;
            Stretch = true;
            AutoSize = false;
            Height = 48;
        }

        public ToolStripButton AddLargeButton(string text, Image? image = null)
        {
            var btn = new ToolStripButton(text, image)
            {
                ImageScaling = ToolStripItemImageScaling.None,
                TextImageRelation = TextImageRelation.ImageAboveText,
                AutoSize = false,
                Width = 72,
                Height = 48
            };
            Items.Add(btn);
            return btn;
        }

        public ToolStripButton AddSmallButton(string text, Image? image = null)
        {
            var btn = new ToolStripButton(text, image)
            {
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                AutoSize = true
            };
            Items.Add(btn);
            return btn;
        }
    }
}

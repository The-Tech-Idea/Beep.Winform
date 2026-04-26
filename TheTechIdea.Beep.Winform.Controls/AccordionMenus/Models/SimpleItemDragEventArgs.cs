using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.AccordionMenus
{
    public class SimpleItemDragEventArgs : EventArgs
    {
        public SimpleItem DraggedItem { get; }
        public SimpleItem TargetItem { get; }
        public int OldIndex { get; }
        public int NewIndex { get; }

        public SimpleItemDragEventArgs(SimpleItem draggedItem, SimpleItem targetItem, int oldIndex, int newIndex)
        {
            DraggedItem = draggedItem;
            TargetItem = targetItem;
            OldIndex = oldIndex;
            NewIndex = newIndex;
        }
    }

    public class AccordionItemIconEventArgs : EventArgs
    {
        public SimpleItem Item { get; }
        public string IconPath { get; set; }
        public Color IconColor { get; set; }
        public int IconSize { get; set; }

        public AccordionItemIconEventArgs(SimpleItem item)
        {
            Item = item;
            IconSize = 20;
        }
    }
}
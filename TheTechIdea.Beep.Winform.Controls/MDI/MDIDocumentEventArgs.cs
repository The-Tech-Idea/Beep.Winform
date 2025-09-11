using System;

namespace TheTechIdea.Beep.Winform.Controls.MDI
{
    public class MDIDocumentEventArgs : EventArgs
    {
        public MDIDocumentEventArgs(MDIDocument document) => Document = document;
        public MDIDocument Document { get; }
        public bool Cancel { get; set; }
    }

    public class MDIDocumentReorderEventArgs : MDIDocumentEventArgs
    {
        public MDIDocumentReorderEventArgs(MDIDocument document, int oldIndex, int newIndex) : base(document)
        {
            OldIndex = oldIndex;
            NewIndex = newIndex;
        }
        public int OldIndex { get; }
        public int NewIndex { get; }
    }
}

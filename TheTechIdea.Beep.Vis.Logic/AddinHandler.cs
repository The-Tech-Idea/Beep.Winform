using System;
using System.Collections.Generic;
using System.Text;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
namespace TheTechIdea.Beep.Vis.Logic
{
    public class AddinHandler
    {
        public AddinHandler(IDMEEditor dMEEditor)
        {
            DMEEditor = dMEEditor;
        }

        public IDMEEditor DMEEditor { get; }

    }
}

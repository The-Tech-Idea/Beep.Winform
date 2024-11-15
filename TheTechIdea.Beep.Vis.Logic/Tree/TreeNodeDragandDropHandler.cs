using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Editor;



namespace TheTechIdea.Beep.Vis.Tree
{
    public class TreeNodeDragandDropHandler 
    {
        public TreeNodeDragandDropHandler(IBeepService beepService, ITree ptreeControl)
        {
            BeepService = beepService;
            DMEEditor = beepService.DMEEditor;
            VisManager = beepService.vis;
            Tree = ptreeControl;


        }

        public IBeepService BeepService { get; }
        public IDMEEditor DMEEditor { get; set; }
        public IVisManager VisManager { get; }
        public ITree Tree { get; set; }

      
     

    }
}

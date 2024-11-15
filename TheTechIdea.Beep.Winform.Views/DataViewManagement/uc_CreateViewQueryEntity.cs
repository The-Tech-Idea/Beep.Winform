using System.Data;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Basic;
using TheTechIdea.Logger;
using TheTechIdea.Util;

namespace TheTechIdea.Beep.Winform.Views.DataViewManagement
{
    [AddinAttribute(Caption = "Entity in View Editor ", Name = "uc_CreateViewQueryEntity", misc = "VIEW", addinType = AddinType.Control, ObjectType = "Beep")]
    public partial class uc_CreateViewQueryEntity : uc_Addin
    {
        public uc_CreateViewQueryEntity()
        {
            InitializeComponent();
        }
        IDMDataView MyDataView;
        public override void Run(IPassedArgs pPassedarg)
        {
            base.Run(pPassedarg);   

        }
        public override void SetConfig(IDMEEditor pbl, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            base.SetConfig(pbl, plogger, putil, args, e, per);
            if (Passedarg.Objects.Where(i => i.Name == "IDMDataView").Any())
            {
                MyDataView = (IDMDataView)e.Objects.Where(c => c.Name == "IDMDataView").FirstOrDefault()!.obj;

            }
        }


    }
}

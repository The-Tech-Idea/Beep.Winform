using System;
using System.Data;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Basic;
using TheTechIdea.Logger;
using TheTechIdea.Util;



namespace TheTechIdea.Beep.Winform.Views.ImportExport
{
    [AddinAttribute(Caption = "Import Data", Name = "uc_ImportMain", misc = "uc_ImportMain", addinType = AddinType.Control)]
    public partial class uc_ImportMain : uc_Addin
    {
        public uc_ImportMain()
        {
            InitializeComponent();
        }
        public override void SetConfig(IDMEEditor pDMEEditor, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            base.SetConfig(pDMEEditor, plogger, putil, args, e, per);

        }
    }
}

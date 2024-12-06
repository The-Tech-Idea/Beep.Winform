using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.Tree;

using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Winform.Controls.ToolBar
{
    [AddinAttribute(Caption = "Beep", Name = "ToolbarControl", misc = "Control")]
    public class ToolbarControl :  IDM_Addin
    {
        public ToolbarControl(IDMEEditor pDMEEditor, TreeControl ptreeControl)
        {
            DMEEditor = pDMEEditor;
            Treecontrol = ptreeControl;
            vismanager = Treecontrol.VisManager;
            TreeV = ptreeControl.TreeV;
        }
        public ToolStrip ToolStrip { get; set; }
        public TreeView TreeV { get; set; }
        private TreeControl Treecontrol { get; set; }
        public string ParentName { get ; set ; }
        public string ObjectName { get ; set ; }
        public string ObjectType { get; set; } 
        public string AddinName { get ; set ; }
        public string Description { get ; set ; }
        public bool DefaultCreate { get ; set ; }
        public string DllPath { get ; set ; }
        public string DllName { get ; set ; }
        public string NameSpace { get ; set ; }
        public IErrorsInfo ErrorObject { get ; set ; }
        public IDMLogger Logger { get ; set ; }
        public IDMEEditor DMEEditor { get ; set ; }
        public EntityStructure EntityStructure { get ; set ; }
        public string EntityName { get ; set ; }
        public IPassedArgs Passedarg { get ; set ; }
          public bool IsHorizentalBar { get ; set ; }=false;
        public IVisManager vismanager { get; set; }
        public List<ToolStripButton> menuitems { get; set; } = new List<ToolStripButton>();
        public string GuidID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        ImageList imageList;
        public void Run(IPassedArgs pPassedarg)
        {
            throw new NotImplementedException();
        }
         private ImageList GetImageList()
        {
            VisHelper visHelper = (VisHelper)vismanager.visHelper;
          

            return visHelper.ImageList32;
        }
        public void SetConfig(IDMEEditor pbl, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            DMEEditor = pbl;
        }
        #region "MEnu and Tool"
        private bool IsMethodApplicabletoNode(AssemblyClassDefinition cls, IBranch br)
        {
            if (cls.classProperties == null)
            {
                return true;
            }
            if (cls.classProperties.ObjectType != null)
            {
                if (!cls.classProperties.ObjectType.Equals(br.BranchClass, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }
            return true;
        }
        public IErrorsInfo CreateToolbar()
        {
            try
            {
                ToolStrip.ImageScalingSize = new Size(24, 24);
              //  ToolStrip.Dock = System.Windows.Forms.DockStyle.Fill;
                //  toolbarstrip.Location = new System.Drawing.Point(342, 0);
            //    ToolStrip.Name = "TreetoolStrip";
                // toolbarstrip.Size = new System.Drawing.Size(32, 580);
              //  ToolStrip.Text = "toolStrip1";
             //   ToolStrip.Stretch = true;

                //toolbarstrip.TextDirection = System.Windows.Forms.ToolStripTextDirection.;
                ToolStrip.ImageList = GetImageList();
                List< AssemblyClassDefinition > classes = new List< AssemblyClassDefinition >();
                if(!IsHorizentalBar)
                {
                    classes = DMEEditor.ConfigEditor.GlobalFunctions.Where(x => x.componentType == "IFunctionExtension" && x.classProperties != null && x.classProperties.ObjectType != null && (x.classProperties.Showin == ShowinType.Toolbar || x.classProperties.Showin == ShowinType.Both) && x.classProperties.ObjectType.Equals(ObjectType, StringComparison.InvariantCultureIgnoreCase)).OrderBy(p => p.Order).ToList();

                }
                else
                {
                    classes = DMEEditor.ConfigEditor.GlobalFunctions.Where(x => x.componentType == "IFunctionExtension" && x.classProperties != null && x.classProperties.ObjectType != null && (x.classProperties.Showin == ShowinType.HorZToolbar) && x.classProperties.ObjectType.Equals(ObjectType, StringComparison.InvariantCultureIgnoreCase)).OrderBy(p => p.Order).ToList();

                }

                foreach (AssemblyClassDefinition cls in classes)
                {
                  
                    foreach (var item in cls.Methods)
                    {
                        ToolStripButton toolStripButton1 = new ToolStripButton();
                        if (item.iconimage != null)
                        {
                            
                            toolStripButton1.ImageIndex = vismanager.visHelper.GetImageIndex(item.iconimage);
                            toolStripButton1.ImageKey= item.iconimage;
                        }
                        toolStripButton1.Alignment= item.CommandAttr.IsLeftAligned? ToolStripItemAlignment.Left: ToolStripItemAlignment.Right; 
                        toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
                        toolStripButton1.TextAlign = ContentAlignment.BottomLeft;
                        //toolStripButton1.ImageAlign = ContentAlignment.TopRight;
                        toolStripButton1.Name = item.Name;
                        toolStripButton1.Size = new System.Drawing.Size(24, 24);
                        toolStripButton1.Text = item.Caption;
                        toolStripButton1.ToolTipText = item.Caption;
                        toolStripButton1.Click += RunFunction;
                        toolStripButton1.Tag = cls;
                        toolStripButton1.AutoSize = true;
                      
                        toolStripButton1.Width = 32;
                        toolStripButton1.Font = new Font("Arial", 8, FontStyle.Regular);
                        toolStripButton1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.SizeToFit;
                        ToolStrip.Items.Add(toolStripButton1);

                        ToolStripSeparator stripSeparator = new ToolStripSeparator();
                        ToolStrip.Items.Add(stripSeparator);
                    }

                }
                ////-------------------------------------------------------------------------------------------

                return DMEEditor.ErrorObject;
            }
            catch (Exception ex)
            {

                return DMEEditor.ErrorObject;
            }
        }
        private void RunFunction(object sender, EventArgs e)
        {
            
            Treecontrol.RunFunction(sender, e);
        }

        public void Run(params object[] args)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}


using System.ComponentModel;
using System.Data;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls.Basic;
using TheTechIdea.Beep.Winform.Controls.Grid;
using TheTechIdea.Beep.Winform.Controls.Template;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Addin;

namespace TheTechIdea.Beep.Winform.Controls.BindingNavigator
{
    [ToolboxItem(true)]
    //[ToolboxBitmap(typeof(BeepbindingNavigator))] // Optional//"Resources.BeepButtonIcon.bmp"
    [Category("Beep Controls")]
    public partial class BeepbindingNavigator : uc_Addin
    {
        public BeepbindingNavigator()
        {
            InitializeComponent();
            //Loadimages();

            InitPanels();

        }
        public IUnitofWork<Entity> UnitofWork { get; set; }
        public event EventHandler<BindingSource> CallPrinter;
        public event EventHandler<BindingSource> SendMessage;
        public event EventHandler<BindingSource> ShowSearch;
        public event EventHandler<BindingSource> NewRecordCreated;
        public event EventHandler<BindingSource> SaveCalled;
        public event EventHandler<BindingSource> DeleteCalled;
        public event EventHandler<BindingSource> EditCalled;

        public string ParentName { get ; set ; }
        public string ObjectName { get ; set ; }
        public string ObjectType { get ; set ; }
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
       // VisManager visManager;
       // ImportDataManager importDataManager;
        public  BindingSource bindingSource { get; set; }
        public bool VerifyDelete { get; set; } = true;
        public int ButtonBorderSize { get; set; } = 0;
        public int ControlHeight { get; set; } = 30;
        private Color _HightlightColor;
        private Color _SelectedColor;

        private ToolTip searchtooltip;
        private ToolTip addtooltip;
        private ToolTip edittooltip;
        private ToolTip removetooltip;
        private ToolTip nexttooltip;
        private ToolTip previoustooltip;
        private ToolTip canceltooltip;
        private ToolTip savetooltip;
        private ToolTip printtooltip;
        private ToolTip sharetooltip;

     
        public Color HightlightColor { get { return _HightlightColor; } set { _HightlightColor = value; setHiColor(); } }
        public Color SelectedColor { get { return _SelectedColor; } set { _SelectedColor = value; setSelectColor(); } }

        public void Run(IPassedArgs pPassedarg)
        {
           
        }
        public void SetConfig(IDMEEditor pbl, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            DMEEditor = pbl;
            ErrorObject = pbl.ErrorObject;
            Logger = pbl.Logger;

            //if (e.Objects.Where(c => c.Name == "VISUTIL").Any())
            //{
            //    visManager = (VisManager)e.Objects.Where(c => c.Name == "VISUTIL").FirstOrDefault().obj;
            //}
            //if (e.Objects.Where(c => c.Name == "ImportDataManager").Any())
            //{
            //    importDataManager = (ImportDataManager)e.Objects.Where(c => c.Name == "ImportDataManager").FirstOrDefault().obj;
            //}
            if (e.Objects.Where(c => c.Name == "BindingSource").Any())
            {
                bindingSource = (BindingSource)e.Objects.Where(c => c.Name == "BindingSource").FirstOrDefault().obj;
            }
            // UnitofWork<Entity> UnitofWork = new UnitofWork<Entity>(DMEEditor,true,new ObservableBindingList<Entity>(bindingSource.List),"ID");
            //Loadimages();
            // InitPanels();

            // HightlightColor = Color.Yellow;
            //  SelectedColor = Color.Green;

            //    Highlightpanel.BackColor = HightlightColor;
            //  Focuspanel.BackColor = SelectedColor;
          
            this.SizeChanged += Uc_bindingNavigator_SizeChanged;
           

            if (bindingSource != null)
            {
                this.bindingSource.DataSourceChanged += BindingSource_DataSourceChanged;
                this.bindingSource.ListChanged += BindingSource_ListChanged;
                this.bindingSource.CurrentChanged += BindingSource_CurrentChanged;

                this.PrevoiusButton.Click -= PreviouspictureBox_Click;
                this.NextButton.Click -= NextpictureBox_Click;
                this.RemoveButton.Click -= RemovepictureBox_Click;
                this.SaveButton.Click -= SavepictureBox_Click;
                this.RollbackButton.Click -= RollbackpictureBox_Click;
                this.EditButton.Click -= EditpictureBox_Click;
                this.FindButton.Click -= FindpictureBox_Click;
                this.PrinterButton.Click -= PrinterpictureBox_Click;
                this.MessageButton.Click -= MessegepictureBox_Click;
                this.NewButton.Click -= NewButton_Click;

                this.NewButton.Click += NewButton_Click;
                this.PrevoiusButton.Click += PreviouspictureBox_Click;
                this.NextButton.Click += NextpictureBox_Click;
                this.RemoveButton.Click += RemovepictureBox_Click;
                this.SaveButton.Click += SavepictureBox_Click;
                this.RollbackButton.Click += RollbackpictureBox_Click;
                this.EditButton.Click += EditpictureBox_Click;
                this.FindButton.Click += FindpictureBox_Click;
                this.PrinterButton.Click += PrinterpictureBox_Click;
                this.MessageButton.Click += MessegepictureBox_Click;


                this.NewButton.MouseHover-= pictureBox_MouseHover;
                this.PrevoiusButton.MouseHover-= pictureBox_MouseHover;
                this.NextButton.MouseHover -= pictureBox_MouseHover;
                this.RemoveButton.MouseHover -= pictureBox_MouseHover;
                this.SaveButton.MouseHover -= pictureBox_MouseHover;
                this.RollbackButton.MouseHover -= pictureBox_MouseHover;
                this.EditButton.MouseHover -= pictureBox_MouseHover;
                this.FindButton.MouseHover -= pictureBox_MouseHover;
                this.MessageButton.MouseHover -= pictureBox_MouseHover;
                this.PrinterButton.MouseHover -= pictureBox_MouseHover;
                this.Recordnumberinglabel1.MouseHover -= pictureBox_MouseHover;



                this.NewButton.MouseHover += pictureBox_MouseHover;
                this.PrevoiusButton.MouseHover += pictureBox_MouseHover;
                this.NextButton.MouseHover += pictureBox_MouseHover;
                this.RemoveButton.MouseHover += pictureBox_MouseHover;
                this.SaveButton.MouseHover += pictureBox_MouseHover;
                this.RollbackButton.MouseHover += pictureBox_MouseHover;
                this.EditButton.MouseHover += pictureBox_MouseHover;
                this.FindButton.MouseHover += pictureBox_MouseHover;
                this.MessageButton.MouseHover += pictureBox_MouseHover;
                this.PrinterButton.MouseHover += pictureBox_MouseHover;
                this.Recordnumberinglabel1.MouseHover += Recordnumberinglabel1_MouseHover;

                datasourcechanged();
                if (UnitofWork != null)
                {
                    bindingSource.DataSource = UnitofWork.Units;
                }
              
              
            }
        }

     

        private void Recordnumberinglabel1_MouseHover(object sender, EventArgs e)
        {
            Label box = (Label)sender;
            if (box != null)
            {
                Highlightpanel.Left = box.Left - 1;
                Highlightpanel.Top = box.Top - 1;
                Highlightpanel.Height = box.Height + 2;
                Highlightpanel.Width = box.Width + 2;
                Highlightpanel.SendToBack();
            }
        }

        private void pictureBox_MouseHover(object sender, EventArgs e)
        {
            Button box = (Button)sender;
            if (box != null)
            {
                Highlightpanel.Left = box.Left-1 ;
                Highlightpanel.Top = box.Top-1;
                Highlightpanel.Height = box.Height+2;
                Highlightpanel.Width = box.Width+2;
                Highlightpanel.SendToBack();
            }
        }
        private void InitPanels()
        {
            Highlightpanel.Left = Recordnumberinglabel1.Left - 1;
            Highlightpanel.Top = Recordnumberinglabel1.Top-1;
            Highlightpanel.Height = Recordnumberinglabel1.Height+1;
            Highlightpanel.Width = Recordnumberinglabel1.Width + 1;
            foreach (Button item in GetAll(this, typeof(Button)))
            {
                item.FlatAppearance.BorderSize = ButtonBorderSize;
                item.FlatAppearance.MouseDownBackColor = SelectedColor;
                item.FlatAppearance.MouseOverBackColor = HightlightColor;
            }

            searchtooltip = new ToolTip();
            addtooltip = new ToolTip();
            edittooltip = new ToolTip();
            removetooltip = new ToolTip();
            nexttooltip = new ToolTip();
            previoustooltip = new ToolTip();
            canceltooltip =new ToolTip();
            savetooltip = new ToolTip();
            printtooltip = new ToolTip();
            sharetooltip = new ToolTip();

            searchtooltip.SetToolTip(FindButton, "Find");
            addtooltip.SetToolTip(NewButton, "New Record");
            edittooltip.SetToolTip(EditButton, "Edit Current Record");
            removetooltip.SetToolTip(RemoveButton, "Delete Current Record");
            nexttooltip.SetToolTip(NextButton, "Next");
            previoustooltip.SetToolTip(PrevoiusButton, "Previous");
            canceltooltip.SetToolTip(RollbackButton, "Cancel or Rollback changes");
            savetooltip.SetToolTip(SaveButton, "Save Changes");
            printtooltip.SetToolTip(PrinterButton, "Print");
            sharetooltip.SetToolTip(MessageButton, "Share");
            Focuspanel.Visible = false;
        }
        private void setHiColor()
        {
            foreach (Button item in GetAll(this, typeof(Button)))
            {


                item.FlatAppearance.MouseOverBackColor = HightlightColor;
            }
        }
        private void setSelectColor()
        {
            foreach (Button item in GetAll(this, typeof(Button)))
            {

                item.FlatAppearance.MouseDownBackColor = SelectedColor;

            }

        }
        #region "Click Methods for all Buttons"
        //private void FocusPicture(object sender)
        //{
        //    PictureBox box = (PictureBox)sender;
        //    if (box != null)
        //    {
        //        Focuspanel.Visible = true;
        //        Focuspanel.Left = box.Left - 2;
        //        Focuspanel.Top = box.Top - 2;
        //        Focuspanel.Height = box.Height + 4;
        //        Focuspanel.Width = box.Width + 4;
                
                
        //    }
        //}
        private void MessegepictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                //  if (MessageBox.Config(this.ParentNode, "Are you sure you want to cancel Changes?", "Beep", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == BeepDialogResult.OK)
                //  {
                // bindingSource.CancelEdit();
                //FocusPicture(sender);
                SendMessage?.Invoke(this, bindingSource);
                // };

            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Binding Navigator", ex.Message, DateTime.Now, bindingSource.Position, ex.Message, Errors.Failed);
            }
        }
        private void PrinterpictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                //  if (MessageBox.Config(this.ParentNode, "Are you sure you want to cancel Changes?", "Beep", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == BeepDialogResult.OK)
                //  {
                // bindingSource.CancelEdit();
          //      FocusPicture(sender);
                CallPrinter?.Invoke(this, bindingSource);
                // };

            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Binding Navigator", ex.Message, DateTime.Now, bindingSource.Position, ex.Message, Errors.Failed);
            }
        }
        private void FindpictureBox_Click(object sender, EventArgs e)
        {
            try
            {
              //  if (MessageBox.Config(this.ParentNode, "Are you sure you want to cancel Changes?", "Beep", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == BeepDialogResult.OK)
              //  {
                   // bindingSource.CancelEdit();
               //FocusPicture(sender);
                ShowSearch?.Invoke(this, bindingSource);
                // };

            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Binding Navigator", ex.Message, DateTime.Now, bindingSource.Position, ex.Message, Errors.Failed);
            }
        }
        private void EditpictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                //if (MessageBox.Config(this.ParentNode, "Are you sure you want to cancel Changes?", "Beep", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == BeepDialogResult.OK)
                //{
                  //  bindingSource.CancelEdit();
              //      FocusPicture(sender);
               // };
               EditCalled?.Invoke(sender, bindingSource);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Binding Navigator", ex.Message, DateTime.Now, bindingSource.Position, ex.Message, Errors.Failed);
            }
        }
        private void RollbackpictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(this.Parent, "Are you sure you want to cancel Changes?", "Beep", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                  bindingSource.CancelEdit();
                    bindingSource.ResetBindings(false);
                    //      FocusPicture(sender);
                };

            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Binding Navigator", ex.Message, DateTime.Now, bindingSource.Position, ex.Message, Errors.Failed);
            }
        }
        private void SavepictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                bindingSource.EndEdit();
                SaveCalled?.Invoke(sender, bindingSource);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Binding Navigator", ex.Message, DateTime.Now, bindingSource.Position, ex.Message, Errors.Failed);
            }

        }
        private void RemovepictureBox_Click(object sender, EventArgs e)
        {
            try
            { 
                DeleteCalled?.Invoke(sender,bindingSource);
                if (bindingSource.Current != null)
                {
                    if (bindingSource.Count > 0)
                    {
                        if (VerifyDelete)
                        {
                            if (MessageBox.Show(this.Parent, "Are you sure you want to Delete Record?", "Beep", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                            {
                                bindingSource.RemoveCurrent();
                                bindingSource.ResetBindings(false);
                            };
                        }
                        else
                        {
                            bindingSource.RemoveCurrent();
                            bindingSource.ResetBindings(false);
                        }
                    }
                }
               
               
                //FocusPicture(sender);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Binding Navigator", ex.Message, DateTime.Now, bindingSource.Position, ex.Message, Errors.Failed);
            }

        }
        private void NextpictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                if (bindingSource.Count > 0)
                    if (bindingSource[bindingSource.Count - 1] != null)
                        if (bindingSource.Position + 1 < bindingSource.Count)
                            bindingSource.MoveNext();
                ;
                
               // FocusPicture(sender);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Binding Navigator", ex.Message, DateTime.Now, bindingSource.Position, ex.Message, Errors.Failed);
            }

        }
        private void PreviouspictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                if (bindingSource.Count > 0)
                    if (bindingSource[bindingSource.Count - 1] != null)
                        if (bindingSource.Position > 0)
                            bindingSource.MovePrevious();
                ;
              //  FocusPicture(sender);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Binding Navigator", ex.Message, DateTime.Now, bindingSource.Position, ex.Message, Errors.Failed);
            }

        }
        private void NewButton_Click(object sender, EventArgs e)
        {
            try
            {

                bindingSource.AddNew();
                bindingSource.ResetBindings(false);
                NewRecordCreated?.Invoke(this,bindingSource);

            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Binding Navigator", ex.Message, DateTime.Now, bindingSource.Position, ex.Message, Errors.Failed);
            }
        }
       
        #endregion
        #region "BindingSource Events"
        private void BindingSource_CurrentChanged(object sender, EventArgs e)
        {
            datasourcechanged();
            
        }
      
        private void BindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            datasourcechanged();
        }
        private void BindingSource_DataSourceChanged(object sender, EventArgs e)
        {
            datasourcechanged();


        }
        private void datasourcechanged()
        {
            if (this.bindingSource != null)
            {
                if (this.bindingSource.List != null)
                {
                    this.Recordnumberinglabel1.Text = (Convert.ToInt32(bindingSource.Position+1)).ToString() + " From " + bindingSource.List.Count.ToString();
                }
                else
                    this.Recordnumberinglabel1.Text = "-";
            }
        }

        #endregion
        private void Uc_bindingNavigator_SizeChanged(object sender, EventArgs e)
        {
            this.Height = ControlHeight;
        }
        public IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }
        #region "Resource Loaders"
       
       
        #endregion
    }
}

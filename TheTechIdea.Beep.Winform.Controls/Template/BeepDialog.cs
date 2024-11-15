using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;


namespace TheTechIdea.Beep.Winform.Controls.Template
{
    public enum DialogButtons
    {
        YesNo,
        OkCancel,
        Ok,
        None
    }
    public partial class BeepDialog : BeepForm
    {

        bool ok = false;
        bool cancel = false;

        public void SetTheme(BeepTheme theme)
        {
            Toppanel.BackColor=theme.HeaderBackColor;
            BottomPanel.BackColor = theme.HeaderBackColor;
            Insidepanel.BackColor = theme.BackColor;
            this.ForeColor = theme.LatestForColor;
            this.BackColor = theme.BackColor;
            this.Okbutton.BackColor = theme.ButtonBackColor;
            this.Cancelbutton.BackColor = theme.ButtonBackColor;    

        }
        public void SetDialog(bool ok, bool cancel)
        {
            this.ok = ok;
            this.cancel = cancel;
            if (ok)
            {
                this.Okbutton.Visible = true;
            }
            else
            {
                this.Okbutton.Visible = false;
            }
            if (cancel)
            {
                this.Cancelbutton.Visible = true;
            }
            else
            {
                this.Cancelbutton.Visible = false;
            }
        }
        public void PanelColor(System.Drawing.Color color)
        {
            this.Toppanel.BackColor = color;
            this.BottomPanel.BackColor = color;
        }
        public BeepDialog()
        {
            InitializeComponent();
            this.Okbutton.Click += Okbutton_Click;
            this.Cancelbutton.Click += Cancelbutton_Click;
            this.PreClose += BeepDialog_PreClose;
            this.FormClosed += BeepDialog_FormClosed;
            this.AcceptButton = Okbutton; // Set Okbutton as the default button
            SetDialog(true);
        }
        public void ShowDialog(UserControl ctl,string title, string icon, Image image)
        {
            SetTitle(title, icon);
            SetLogo(image);
            AddControl(ctl, title);
            this.ShowDialog();
           
        }
        public void ShowDialog(UserControl ctl, string title, string icon)
        {
            SetTitle(title, icon);
            AddControl(ctl, title);
            this.ShowDialog();
        }
        public void ShowDialog(UserControl ctl, string title)
        {
            SetTitle(title);
            AddControl(ctl, title);
            this.ShowDialog();
        }
        private void BeepDialog_FormClosed(object? sender, FormClosedEventArgs e)
        {
            
        }
        private void BeepDialog_PreClose(object? sender, FormClosingEventArgs e)
        {
            
        }
        private void Cancelbutton_Click(object? sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            bool retval = true;
            Control addin = Insidepanel.Controls[0];
            if (retval)
            {
                if (addin is IAddinExtension)
                {
                    IAddinExtension extension = (IAddinExtension)addin;
                    retval = extension.Cancel();
                }
            }
            if (retval)
            {
                this.Close();
            }

        }
        private void Okbutton_Click(object? sender, EventArgs e)
        {
            bool retval = true;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Control addin = Insidepanel.Controls[0];
            
            retval = ValidateDialog();
            if (retval)
            {
                if (addin is IAddinExtension)
                {
                    IAddinExtension extension = (IAddinExtension)addin;
                    retval= extension.Save();
                }
                
              
            }
            if (retval)
            {
                this.Close();
            }
         
        }
        public bool ValidateDialog()
        {
            bool retval = true;
            if (Insidepanel.Controls.Count > 0)
            {
                Control addin = Insidepanel.Controls[0];
                if(addin  is IAddinExtension)
                {
                    IAddinExtension extension = (IAddinExtension)addin;
                    retval= extension.ValidateAddin();
                }
               
            }
            return retval;
        }
        public void AddControl(UserControl ctl, string title)
        {
            base.AddControl(ctl, title,BottomPanel.Height+10,0);
           
            if (ctl is IAddinExtension)
            {
                IAddinExtension extension = (IAddinExtension)ctl;
                extension.CloseParent += Extension_CloseParent;
            }
            this.Text = title;
        }

        private void Extension_CloseParent(object? sender, PassedArgs e)
        {
            this.Close();
        }


        public void SetButtonOptions(DialogButtons options)
        {
            switch (options)
            {
                case DialogButtons.YesNo:
                    Okbutton.Text = "Yes";
                    Okbutton.Visible = true;
                    Cancelbutton.Text = "No";
                    Cancelbutton.Visible = true;
                    this.AcceptButton = Okbutton;  // Set Okbutton (Yes) as the default
                    break;
                case DialogButtons.OkCancel:
                    Okbutton.Text = "OK";
                    Okbutton.Visible = true;
                    Cancelbutton.Text = "Cancel";
                    Cancelbutton.Visible = true;
                    this.AcceptButton = Okbutton;  // Set Okbutton (OK) as the default
                    break;
                case DialogButtons.Ok:
                case DialogButtons.None:
                    Okbutton.Text = "OK";
                    Okbutton.Visible = true;
                    Okbutton.Location = new System.Drawing.Point((this.Width / 2) - (Okbutton.Width / 2), Okbutton.Location.Y);
                    Cancelbutton.Visible = false;
                    this.AcceptButton = Okbutton;  // Only Okbutton is visible, set as default
                    break;

            }
        }



    }
}

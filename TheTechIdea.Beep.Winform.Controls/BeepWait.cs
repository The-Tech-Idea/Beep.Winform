using TheTechIdea.Beep.Vis.Modules;

using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using System.ComponentModel;
using TheTechIdea.Beep.Desktop.Common;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(false)]
    public partial class BeepWait : BeepiForm, IWaitForm
    {

        public BeepWait()
        {
            InitializeComponent();
            //  LogopictureBox.Visible = false;
            _spinnerImage.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.loading.svg";
            LogopictureBox.ImagePath= "TheTechIdea.Beep.Winform.Controls.GFX.SVG.simpleinfoapps.svg";
          
            StartSpinner();
        }
        private void StartSpinner()
        {
            _spinnerImage.ApplyThemeOnImage = true;
            _spinnerImage.Theme = Theme;
            _spinnerImage.IsSpinning = true; // Start spinning
        }
        private void StopSpinner()
        {
            _spinnerImage.IsSpinning = false; // Stop spinning
        }

        // private ResourceManager resourceManager = new ResourceManager();
        public static void InvokeAction(Control control, MethodInvoker action)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(action);
            }
            else
            {
                action();
            }
        }
        public void CloseForm()
        {

            System.Threading.Thread.Sleep(2000);
            if (this.IsHandleCreated)
            {
                this.Invoke(new Action(Close));
            }
            StopSpinner();
        }

        public void SetImage(string image)
        {
            //          Title.Visible= false;
            LogopictureBox.Visible = true;
            LogopictureBox.ImagePath = ImageListHelper.GetImagePathFromName(image);
        }
        public void SetImage(Image image)
        {
            //         Title.Visible = false;
            LogopictureBox.Visible = true;
            LogopictureBox.Image = image;
        }
        public void SetText(string text)
        {
            messege.BeginInvoke(new Action(() =>
            {
                messege.AppendText(text + Environment.NewLine);
                messege.SelectionStart = messege.Text.Length;
                messege.ScrollToCaret();
            }));
        }

        public void SetTitle(string title)
        {
            Title.Visible = true;
            LogopictureBox.Visible = false;
            Title.Text = title;
        }

        public void SetTitle(string title, string text)
        {

        }

        public IErrorsInfo Show(PassedArgs Passedarguments)
        {
            StartSpinner();
            return new ErrorsInfo();
        }

        public void UpdateProgress(int progress, string text = null)
        {
            messege.BeginInvoke(new Action(() =>
            {
                messege.AppendText(text + Environment.NewLine);
                messege.SelectionStart = messege.Text.Length;
                messege.ScrollToCaret();
            }));

        }

        IErrorsInfo IWaitForm.Close()
        {
            try
            {
                System.Threading.Thread.Sleep(2000);
                if (this.IsHandleCreated)
                {
                    this.Invoke(new Action(Close));
                }
                StopSpinner();
            }
            catch (Exception)
            {


            }
            return null;

        }
    }
}

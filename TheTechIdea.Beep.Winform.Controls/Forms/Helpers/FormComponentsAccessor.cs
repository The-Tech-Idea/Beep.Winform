using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Accessor class to provide controlled access to form components for helpers
    /// </summary>
    public class FormComponentsAccessor
    {
        private readonly BeepFormAdvanced _form;

        public FormComponentsAccessor(BeepFormAdvanced form)
        {
            _form = form;
        }

        // Access through internal properties
        public BeepPanel TitleBar => _form.GetTitleBar();
        public BeepImage AppIcon => _form.GetAppIcon();
        public BeepLabel TitleLabel => _form.GetTitleLabel();
        public BeepPanel TitleRightHost => _form.GetTitleRightHost();
        public BeepButton BtnMin => _form.GetBtnMin();
        public BeepButton BtnMax => _form.GetBtnMax();
        public BeepButton BtnClose => _form.GetBtnClose();
        public BeepPanel ContentHost => _form.GetContentHost();
        public BeepPanel StatusBar => _form.GetStatusBar();
        public BeepLabel StatusLabel => _form.GetStatusLabel();
    }
}
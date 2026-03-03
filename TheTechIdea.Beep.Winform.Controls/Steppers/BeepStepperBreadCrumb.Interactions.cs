using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepStepperBreadCrumb
    {
        private int _focusedStepIndex = -1;

        private int GetNextStepIndex(int current, int direction)
        {
            int next = current + direction;
            if (next < 0 || next >= ListItems.Count)
            {
                return -1;
            }

            return next;
        }

        private void FocusCurrentOrFirstStep()
        {
            if (selectedIndex >= 0)
            {
                return;
            }

            selectedIndex = ListItems.Count > 0 ? 0 : -1;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (ListItems == null || ListItems.Count == 0)
            {
                return base.ProcessDialogKey(keyData);
            }

            FocusCurrentOrFirstStep();
            Keys keyCode = keyData & Keys.KeyCode;
            switch (keyCode)
            {
                case Keys.Left:
                case Keys.Up:
                {
                    int previous = GetNextStepIndex(selectedIndex, -1);
                    if (previous >= 0)
                    {
                        selectedIndex = previous;
                        _focusedStepIndex = previous;
                        Invalidate();
                    }
                    return true;
                }
                case Keys.Right:
                case Keys.Down:
                {
                    int next = GetNextStepIndex(selectedIndex, 1);
                    if (next >= 0)
                    {
                        selectedIndex = next;
                        _focusedStepIndex = next;
                        Invalidate();
                    }
                    return true;
                }
                case Keys.Home:
                    selectedIndex = 0;
                    _focusedStepIndex = 0;
                    Invalidate();
                    return true;
                case Keys.End:
                    selectedIndex = ListItems.Count - 1;
                    _focusedStepIndex = selectedIndex;
                    Invalidate();
                    return true;
                case Keys.Enter:
                case Keys.Space:
                    if (selectedIndex >= 0)
                    {
                        OnStepClicked(selectedIndex);
                        ShowStepNotification(selectedIndex);
                    }
                    return true;
                default:
                    return base.ProcessDialogKey(keyData);
            }
        }

        protected override void OnGotFocus(System.EventArgs e)
        {
            base.OnGotFocus(e);
            _focusedStepIndex = selectedIndex >= 0 ? selectedIndex : (ListItems?.Count > 0 ? 0 : -1);
            Invalidate();
        }

        protected override void OnLostFocus(System.EventArgs e)
        {
            base.OnLostFocus(e);
            _focusedStepIndex = -1;
            Invalidate();
        }
    }
}

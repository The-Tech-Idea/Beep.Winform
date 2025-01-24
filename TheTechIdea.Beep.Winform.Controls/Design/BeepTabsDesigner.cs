using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Design;

public class BeepTabsDesigner : ParentControlDesigner
{
    public override void Initialize(System.ComponentModel.IComponent component)
    {
        base.Initialize(component);

        if (component is BeepTabs beepTabs)
        {
            // Hook into the SelectedIndexChanged event of the TabControl
        //    beepTabs.SelectedIndexChanged += BeepTabs_SelectedIndexChanged;
        }
    }

    private void BeepTabs_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (sender is BeepTabs beepTabs)
        {
            // Refresh the designer when the selected tab changes
            var host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (host != null)
            {
                host.Activate();
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && Control is BeepTabs beepTabs)
        {
            // Unhook the event handler
         //   beepTabs.SelectedIndexChanged -= BeepTabs_SelectedIndexChanged;
        }

        base.Dispose(disposing);
    }
}
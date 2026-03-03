using System.Collections.Generic;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Customization
{
    public sealed class RibbonCustomizationDialogState
    {
        public List<RibbonCustomizationTabModel> Tabs { get; set; } = [];
        public List<RibbonCustomizationCommandModel> AvailableCommands { get; set; } = [];
        public List<string> QuickAccessCommandKeys { get; set; } = [];

        public RibbonCustomizationDialogState DeepClone()
        {
            return new RibbonCustomizationDialogState
            {
                Tabs = Tabs.Select(t => t.DeepClone()).ToList(),
                AvailableCommands = AvailableCommands.Select(c => c.DeepClone()).ToList(),
                QuickAccessCommandKeys = [.. QuickAccessCommandKeys]
            };
        }
    }

    public sealed class RibbonCustomizationTabModel
    {
        public string TabKey { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public bool Visible { get; set; } = true;
        public List<RibbonCustomizationGroupModel> Groups { get; set; } = [];

        public RibbonCustomizationTabModel DeepClone()
        {
            return new RibbonCustomizationTabModel
            {
                TabKey = TabKey,
                Text = Text,
                Visible = Visible,
                Groups = Groups.Select(g => g.DeepClone()).ToList()
            };
        }

        public override string ToString() => Text;
    }

    public sealed class RibbonCustomizationGroupModel
    {
        public string GroupKey { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public bool Visible { get; set; } = true;

        public RibbonCustomizationGroupModel DeepClone()
        {
            return new RibbonCustomizationGroupModel
            {
                GroupKey = GroupKey,
                Text = Text,
                Visible = Visible
            };
        }

        public override string ToString() => Text;
    }

    public sealed class RibbonCustomizationCommandModel
    {
        public string CommandKey { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string TabKey { get; set; } = string.Empty;
        public string GroupKey { get; set; } = string.Empty;
        public string TabText { get; set; } = string.Empty;
        public string GroupText { get; set; } = string.Empty;

        public RibbonCustomizationCommandModel DeepClone()
        {
            return new RibbonCustomizationCommandModel
            {
                CommandKey = CommandKey,
                Text = Text,
                TabKey = TabKey,
                GroupKey = GroupKey,
                TabText = TabText,
                GroupText = GroupText
            };
        }

        public override string ToString() => Text;
    }

    public sealed class RibbonCustomizationStateEventArgs : EventArgs
    {
        public RibbonCustomizationStateEventArgs(RibbonCustomizationDialogState state)
        {
            State = state;
        }

        public RibbonCustomizationDialogState State { get; }
    }
}

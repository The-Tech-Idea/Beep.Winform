using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>How much of a ribbon group's column grid a command occupies.</summary>
    public enum RibbonItemSize
    {
        /// <summary>A whole column: a 32px icon above a centred label, across the full content band.</summary>
        Large,

        /// <summary>Laid out as <see cref="Small"/>. Kept because the designer model has always offered it.</summary>
        Medium,

        /// <summary>One row of a column: a 16px icon with its label beside it.</summary>
        Small
    }

    [ToolboxItem(false)]
    [DesignTimeVisible(false)]
    public abstract class RibbonItemBase : Component
    {
        [Category("Ribbon"), Description("The display text."), DefaultValue("")]
        public string Text { get; set; } = string.Empty;

        [Category("Ribbon"), Description("The command key/value."), DefaultValue("")]
        public string CommandKey { get; set; } = string.Empty;

        [Category("Ribbon"), Description("Path to an icon image."), DefaultValue("")]
        public string ImagePath { get; set; } = string.Empty;

        [Category("Ribbon"), Description("Whether the item is enabled."), DefaultValue(true)]
        public bool Enabled { get; set; } = true;

        [Category("Ribbon"), Description("Whether the item is visible."), DefaultValue(true)]
        public bool Visible { get; set; } = true;

        [Category("Ribbon"), Description("Optional tooltip."), DefaultValue("")]
        public string ToolTip { get; set; } = string.Empty;

        internal SimpleItem ToSimpleItem()
        {
            var item = BuildSimpleItem();
            item.Text = string.IsNullOrWhiteSpace(Text) ? item.Text : Text;
            item.Value = string.IsNullOrWhiteSpace(CommandKey) ? item.Value ?? string.Empty : CommandKey;
            item.ImagePath = string.IsNullOrWhiteSpace(ImagePath) ? item.ImagePath : ImagePath;
            item.IsEnabled = Enabled;
            item.IsVisible = Visible;
            if (!string.IsNullOrWhiteSpace(ToolTip)) item.ToolTip = ToolTip;
            return item;
        }

        protected abstract SimpleItem BuildSimpleItem();
    }

    [ToolboxItem(true)]
    [Category("Beep Ribbon")]
    [DisplayName("Ribbon Tab")]
    [Description("A tab page in the ribbon control.")]
    public class RibbonTabItem : RibbonItemBase
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Ribbon"), Description("Groups within this tab.")]
        public List<RibbonGroupItem> Groups { get; } = new();

        protected override SimpleItem BuildSimpleItem()
        {
            var item = new SimpleItem { Text = Text, Value = CommandKey, IsEnabled = Enabled, IsVisible = Visible };
            foreach (var g in Groups)
                if (g.Visible) item.Children.Add(g.ToSimpleItem());
            return item;
        }
    }

    [ToolboxItem(true)]
    [Category("Beep Ribbon")]
    [DisplayName("Ribbon Group")]
    [Description("A command group within a ribbon tab.")]
    public class RibbonGroupItem : RibbonItemBase
    {
        [Category("Ribbon"), Description("Show dialog launcher in group corner."), DefaultValue(false)]
        public bool ShowDialogLauncher { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Ribbon"), Description("Items within this group.")]
        public List<RibbonItemBase> Items { get; } = new();

        protected override SimpleItem BuildSimpleItem()
        {
            var item = new SimpleItem { Text = Text, Value = CommandKey, IsEnabled = Enabled, IsVisible = Visible };
            if (ShowDialogLauncher) item.ToolTip = "[DLG:" + (string.IsNullOrWhiteSpace(CommandKey) ? Text : CommandKey) + "]";
            foreach (var i in Items)
                if (i.Visible) item.Children.Add(i.ToSimpleItem());
            return item;
        }
    }

    [ToolboxItem(true)]
    [Category("Beep Ribbon")]
    [DisplayName("Ribbon Button")]
    [Description("A clickable button in the ribbon (large or small).")]
    public class RibbonButtonItem : RibbonItemBase
    {
        [Category("Ribbon"), Description("Display size."), DefaultValue(RibbonItemSize.Large)]
        public RibbonItemSize Size { get; set; } = RibbonItemSize.Large;

        [Category("Ribbon"), Description("Can be toggled on/off."), DefaultValue(false)]
        public bool Checkable { get; set; }

        [Category("Ribbon"), Description("Currently checked state."), DefaultValue(false)]
        public bool Checked { get; set; }

        protected override SimpleItem BuildSimpleItem()
        {
            var item = new SimpleItem
            {
                Text = Text,
                Value = CommandKey,
                ImagePath = ImagePath,
                IsEnabled = Enabled,
                IsVisible = Visible,
                IsCheckable = Checkable,
                IsChecked = Checked,
                ToolTip = ToolTip
            };

            // Size had no effect at all: the only thing it decided was "Text = Size == Large ? Text :
            // Text". Carrying it on the command is what lets the group's column layout honour it.
            item.Data[BeepRibbonControl.RibbonItemSizeKey] = Size.ToString();
            return item;
        }
    }

    [ToolboxItem(true)]
    [Category("Beep Ribbon")]
    [DisplayName("Ribbon Split Button")]
    [Description("A split button with dropdown items.")]
    public class RibbonSplitButtonItem : RibbonItemBase
    {
        [Category("Ribbon"), Description("Display size."), DefaultValue(RibbonItemSize.Large)]
        public RibbonItemSize Size { get; set; } = RibbonItemSize.Large;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Ribbon"), Description("Dropdown items for this split button.")]
        public List<RibbonItemBase> DropDownItems { get; } = new();

        protected override SimpleItem BuildSimpleItem()
        {
            var item = new SimpleItem
            {
                Text = Text, Value = CommandKey, ImagePath = ImagePath,
                IsEnabled = Enabled, IsVisible = Visible, ToolTip = ToolTip
            };
            item.Data[BeepRibbonControl.RibbonItemSizeKey] = Size.ToString();
            foreach (var d in DropDownItems)
                if (d.Visible) item.Children.Add(d.ToSimpleItem());
            return item;
        }
    }

    [ToolboxItem(true)]
    [Category("Beep Ribbon")]
    [DisplayName("Ribbon Separator")]
    [Description("A visual separator between ribbon items.")]
    public class RibbonSeparatorItem : RibbonItemBase
    {
        protected override SimpleItem BuildSimpleItem() => new() { IsSeparator = true, IsVisible = true };
    }
}

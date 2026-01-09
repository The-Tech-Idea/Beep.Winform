using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepMenuBar control
    /// Provides smart tags for menu bar configuration and styling
    /// </summary>
    public class BeepMenuBarDesigner : BaseBeepControlDesigner
    {
        public BeepMenuBar? MenuBar => Component as BeepMenuBar;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepMenuBarActionList(this));
            return lists;
        }
    }

    /// <summary>
    /// Action list for BeepMenuBar smart tags
    /// Provides quick configuration presets and common property access
    /// </summary>
    public class BeepMenuBarActionList : DesignerActionList
    {
        private readonly BeepMenuBarDesigner _designer;

        public BeepMenuBarActionList(BeepMenuBarDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepMenuBar? MenuBar => Component as BeepMenuBar;

        #region Properties (for smart tags)

        [Category("Appearance")]
        [Description("Text font displayed in the menu bar")]
        public System.Drawing.Font TextFont
        {
            get => _designer.GetProperty<System.Drawing.Font>("TextFont");
            set => _designer.SetProperty("TextFont", value);
        }

        [Category("Layout")]
        [Description("Height of the menu bar in pixels")]
        public int Height
        {
            get => _designer.GetProperty<int>("Height");
            set => _designer.SetProperty("Height", value);
        }

        #endregion

        #region Quick Configuration Actions

        /// <summary>
        /// Set standard menu bar height (44px)
        /// </summary>
        public void SetStandardHeight()
        {
            Height = 44;
        }

        /// <summary>
        /// Set compact menu bar height (32px)
        /// </summary>
        public void SetCompactHeight()
        {
            Height = 32;
        }

        /// <summary>
        /// Set comfortable menu bar height (56px)
        /// </summary>
        public void SetComfortableHeight()
        {
            Height = 56;
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            // Height presets
            items.Add(new DesignerActionHeaderItem("Height Presets"));
            items.Add(new DesignerActionMethodItem(this, "SetStandardHeight", "Standard (44px)", "Height Presets", true));
            items.Add(new DesignerActionMethodItem(this, "SetCompactHeight", "Compact (32px)", "Height Presets", true));
            items.Add(new DesignerActionMethodItem(this, "SetComfortableHeight", "Comfortable (56px)", "Height Presets", true));

            // Appearance properties
            items.Add(new DesignerActionHeaderItem("Appearance Properties"));
            items.Add(new DesignerActionPropertyItem("TextFont", "Text Font", "Appearance Properties"));
            items.Add(new DesignerActionPropertyItem("Height", "Height", "Appearance Properties"));

            return items;
        }
    }
}

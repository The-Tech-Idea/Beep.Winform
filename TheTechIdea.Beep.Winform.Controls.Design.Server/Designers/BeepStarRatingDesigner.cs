using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepStarRating control
    /// </summary>
    public class BeepStarRatingDesigner : BaseBeepControlDesigner
    {
        public BeepStarRating? StarRating => Component as BeepStarRating;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepStarRatingActionList(this));
            return lists;
        }
    }

    public class BeepStarRatingActionList : DesignerActionList
    {
        private readonly BeepStarRatingDesigner _designer;

        public BeepStarRatingActionList(BeepStarRatingDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        #region Properties

        [Category("Behavior")]
        public int StarCount
        {
            get => _designer.GetProperty<int>("StarCount");
            set => _designer.SetProperty("StarCount", value);
        }

        [Category("Behavior")]
        public int Rating
        {
            get => _designer.GetProperty<int>("Rating");
            set => _designer.SetProperty("Rating", value);
        }

        [Category("Behavior")]
        public bool AllowHalfStars
        {
            get => _designer.GetProperty<bool>("AllowHalfStars");
            set => _designer.SetProperty("AllowHalfStars", value);
        }

        #endregion

        #region Presets

        public void Set5Stars() => StarCount = 5;
        public void Set10Stars() => StarCount = 10;
        public void SetRating3Stars() { StarCount = 5; Rating = 3; }
        public void SetRating5Stars() { StarCount = 5; Rating = 5; }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Configuration"));
            items.Add(new DesignerActionPropertyItem("StarCount", "Star Count", "Configuration"));
            items.Add(new DesignerActionPropertyItem("Rating", "Current Rating", "Configuration"));
            items.Add(new DesignerActionPropertyItem("AllowHalfStars", "Allow Half Stars", "Configuration"));

            items.Add(new DesignerActionHeaderItem("Presets"));
            items.Add(new DesignerActionMethodItem(this, "Set5Stars", "⭐ 5 Stars", "Presets", false));
            items.Add(new DesignerActionMethodItem(this, "Set10Stars", "⭐ 10 Stars", "Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetRating3Stars", "⭐⭐⭐ 3/5 Rating", "Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetRating5Stars", "⭐⭐⭐⭐⭐ 5/5 Rating", "Presets", false));

            return items;
        }
    }
}


using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.ProjectCards;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers.ProjectCards
{
    public class BeepProjectCardDesigner : BaseBeepControlDesigner
    {
        private DesignerVerbCollection? _verbs;

        public BeepProjectCard? ProjectCard => Component as BeepProjectCard;

        public DesignerVerbCollection CustomVerbs
        {
            get
            {
                if (_verbs == null)
                {
                    _verbs = new DesignerVerbCollection
                    {
                        new DesignerVerb("Set CompactProgress Style", OnSetCompactProgress),
                        new DesignerVerb("Set DarkTile Style", OnSetDarkTile),
                        new DesignerVerb("Set AvatarTile Style", OnSetAvatarTile),
                        new DesignerVerb("Set PillBadges Style", OnSetPillBadges),
                        new DesignerVerb("Set Sample Data", OnSetSampleData)
                    };
                }
                return _verbs;
            }
        }

        public override DesignerVerbCollection Verbs => CustomVerbs;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepProjectCardActionList(this));
            return lists;
        }

        private void OnSetCompactProgress(object? sender, EventArgs e) => SetProperty("PainterKind", ProjectCardPainterKind.CompactProgress);
        private void OnSetDarkTile(object? sender, EventArgs e) => SetProperty("PainterKind", ProjectCardPainterKind.DarkTile);
        private void OnSetAvatarTile(object? sender, EventArgs e) => SetProperty("PainterKind", ProjectCardPainterKind.AvatarTile);
        private void OnSetPillBadges(object? sender, EventArgs e) => SetProperty("PainterKind", ProjectCardPainterKind.PillBadges);
        private void OnSetSampleData(object? sender, EventArgs e) => SetSampleData();

        public void SetSampleData()
        {
            SetProperty("Title", "Website Redesign");
            SetProperty("Subtitle", "UI/UX Sprint 4");
            SetProperty("Progress", 65f);
            SetProperty("Status", "In Progress");
            SetProperty("DaysLeft", 12);
            SetProperty("Tags", new[] { "Design", "Frontend", "Sprint" });
        }
    }

    public class BeepProjectCardActionList : DesignerActionList
    {
        private readonly BeepProjectCardDesigner _designer;

        public BeepProjectCardActionList(BeepProjectCardDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepProjectCard? ProjectCard => Component as BeepProjectCard;

        #region Properties

        [Category("Appearance")]
        [Description("Visual style painter for the card")]
        public ProjectCardPainterKind PainterKind
        {
            get => _designer.GetProperty<ProjectCardPainterKind>("PainterKind");
            set => _designer.SetProperty("PainterKind", value);
        }

        [Category("Content")]
        [Description("Card title")]
        public string Title
        {
            get => _designer.GetProperty<string>("Title") ?? "Project Title";
            set => _designer.SetProperty("Title", value);
        }

        [Category("Content")]
        [Description("Card subtitle")]
        public string Subtitle
        {
            get => _designer.GetProperty<string>("Subtitle") ?? "";
            set => _designer.SetProperty("Subtitle", value);
        }

        [Category("Content")]
        [Description("Progress percentage (0-100)")]
        public float Progress
        {
            get => _designer.GetProperty<float>("Progress");
            set => _designer.SetProperty("Progress", value);
        }

        [Category("Content")]
        [Description("Status text")]
        public string Status
        {
            get => _designer.GetProperty<string>("Status") ?? "";
            set => _designer.SetProperty("Status", value);
        }

        [Category("Content")]
        [Description("Days remaining")]
        public int DaysLeft
        {
            get => _designer.GetProperty<int>("DaysLeft");
            set => _designer.SetProperty("DaysLeft", value);
        }

        [Category("Layout")]
        [Description("Controls when text elements are displayed")]
        public CardTextVisibility TextVisibility
        {
            get => _designer.GetProperty<CardTextVisibility>("TextVisibility");
            set => _designer.SetProperty("TextVisibility", value);
        }

        [Category("Layout")]
        [Description("Minimum touch target width in pixels")]
        public int MinTouchTargetWidth
        {
            get => _designer.GetProperty<int>("MinTouchTargetWidth");
            set => _designer.SetProperty("MinTouchTargetWidth", value);
        }

        #endregion

        #region Style Presets

        public void SetCompactProgress() => PainterKind = ProjectCardPainterKind.CompactProgress;
        public void SetDarkTile() => PainterKind = ProjectCardPainterKind.DarkTile;
        public void SetRichCourse() => PainterKind = ProjectCardPainterKind.RichCourse;
        public void SetListKanban() => PainterKind = ProjectCardPainterKind.ListKanban;
        public void SetAvatarTile() => PainterKind = ProjectCardPainterKind.AvatarTile;
        public void SetTeamAvatars() => PainterKind = ProjectCardPainterKind.TeamAvatars;
        public void SetOutlineMeta() => PainterKind = ProjectCardPainterKind.OutlineMeta;
        public void SetCalendarStripe() => PainterKind = ProjectCardPainterKind.CalendarStripe;
        public void SetPillBadges() => PainterKind = ProjectCardPainterKind.PillBadges;

        public void ApplySampleData() => _designer.SetSampleData();

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Content"));
            items.Add(new DesignerActionPropertyItem("Title", "Title:", "Content"));
            items.Add(new DesignerActionPropertyItem("Subtitle", "Subtitle:", "Content"));
            items.Add(new DesignerActionPropertyItem("Progress", "Progress:", "Content"));
            items.Add(new DesignerActionPropertyItem("Status", "Status:", "Content"));
            items.Add(new DesignerActionPropertyItem("DaysLeft", "Days Left:", "Content"));

            items.Add(new DesignerActionHeaderItem("Layout"));
            items.Add(new DesignerActionPropertyItem("PainterKind", "Painter Style:", "Layout"));
            items.Add(new DesignerActionPropertyItem("TextVisibility", "Text Visibility:", "Layout"));
            items.Add(new DesignerActionPropertyItem("MinTouchTargetWidth", "Min Touch Width:", "Layout"));

            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "SetCompactProgress", "Compact Progress", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "SetDarkTile", "Dark Tile", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetRichCourse", "Rich Course", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetListKanban", "List Kanban", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetAvatarTile", "Avatar Tile", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetTeamAvatars", "Team Avatars", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetOutlineMeta", "Outline Meta", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetCalendarStripe", "Calendar Stripe", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetPillBadges", "Pill Badges", "Style Presets", false));

            items.Add(new DesignerActionHeaderItem("Actions"));
            items.Add(new DesignerActionMethodItem(this, "ApplySampleData", "Apply Sample Data", "Actions", true));

            return items;
        }
    }
}

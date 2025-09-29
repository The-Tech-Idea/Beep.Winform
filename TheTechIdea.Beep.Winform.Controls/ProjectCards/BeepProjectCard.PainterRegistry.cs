using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ProjectCards.Painters;

namespace TheTechIdea.Beep.Winform.Controls.ProjectCards
{
    public partial class BeepProjectCard
    {
        private void EnsureDefaultPainters()
        {
            if (_painters.Count > 0) return;
            RegisterPainter(ProjectCardPainterKind.CompactProgress, new CompactProgressCardPainter());
            RegisterPainter(ProjectCardPainterKind.DarkTile, new DarkTileProjectCardPainter());
            RegisterPainter(ProjectCardPainterKind.RichCourse, new RichCourseCardPainter());
            RegisterPainter(ProjectCardPainterKind.ListKanban, new KanbanListProjectCardPainter());
            RegisterPainter(ProjectCardPainterKind.AvatarTile, new AvatarTileProjectCardPainter());
            RegisterPainter(ProjectCardPainterKind.TeamAvatars, new TeamAvatarsProjectCardPainter());
            RegisterPainter(ProjectCardPainterKind.OutlineMeta, new OutlineMetaProjectCardPainter());
            RegisterPainter(ProjectCardPainterKind.CalendarStripe, new CalendarStripeProjectCardPainter());
            RegisterPainter(ProjectCardPainterKind.PillBadges, new PillBadgesProjectCardPainter());
        }

        public void RegisterPainter(ProjectCardPainterKind kind, IProjectCardPainter painter)
        {
            if (painter == null) return;
            _painters[kind] = painter;
        }

        private IProjectCardPainter GetActivePainter()
        {
            EnsureDefaultPainters();
            return _painters.TryGetValue(_painterKind, out var p) ? p : null;
        }
    }
}

using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>A part a card can show. A style is a selection of these in an order.</summary>
    public enum CardPart
    {
        /// <summary>A leading image in the icon gutter — an avatar, a file glyph, a sender.</summary>
        Avatar,

        /// <summary>A full-bleed image above the text — the hero of a media or content card.</summary>
        Hero,

        Header,
        Subtitle,
        Paragraph,

        /// <summary>The one number a card exists to show, at display size. Reads <c>PriceText</c>.</summary>
        Emphasis,

        Badges,
        Tags,
        Rating,
        Status,

        /// <summary>Both action buttons, subject to <c>ShowButton</c> and <c>ShowSecondaryButton</c>.</summary>
        Actions,

        PrimaryAction,
        SecondaryAction,
    }

    /// <summary>
    /// What each <see cref="CardStyle"/> is made of, as data.
    /// </summary>
    /// <remarks>
    /// <para>
    /// 56 styles were 56 painters selecting fonts, measuring text and computing rectangles. Composed,
    /// a style is a list of parts in an order — every one of them drawn from the same set of controls,
    /// so the difference between a <c>BlogCard</c> and a <c>ReviewCard</c> is a row order rather than a
    /// paint routine. That is why this is a table and not 56 methods: a new style is an entry.
    /// </para>
    /// <para>
    /// A part appears only when the property behind it has a value, so a style listing
    /// <see cref="CardPart.Badges"/> with no badge text set adds no control rather than an empty one.
    /// </para>
    /// </remarks>
    public static class CardStyleLayouts
    {
        private static readonly CardPart[] Empty = new CardPart[0];

        public static IReadOnlyList<CardPart> For(CardStyle style) =>
            Layouts.TryGetValue(style, out var parts) ? parts : Default;

        /// <summary>What an unmapped style composes as — a title and its text.</summary>
        private static readonly CardPart[] Default =
        {
            CardPart.Header, CardPart.Paragraph,
        };

        private static readonly Dictionary<CardStyle, CardPart[]> Layouts = new()
        {
            // Profile & user
            [CardStyle.ProfileCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Subtitle, CardPart.Paragraph, CardPart.Actions },
            [CardStyle.CompactProfile] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Subtitle },
            [CardStyle.UserCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Subtitle, CardPart.Badges, CardPart.Actions },
            [CardStyle.TeamMemberCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Subtitle, CardPart.Paragraph, CardPart.Actions },

            // Content & blog
            [CardStyle.ContentCard] = new[] { CardPart.Hero, CardPart.Header, CardPart.Paragraph, CardPart.Actions },
            [CardStyle.BlogCard] = new[] { CardPart.Hero, CardPart.Badges, CardPart.Header, CardPart.Paragraph, CardPart.Tags, CardPart.Actions },
            [CardStyle.NewsCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Subtitle, CardPart.Paragraph },
            [CardStyle.MediaCard] = new[] { CardPart.Hero, CardPart.Header },

            // Feature & service
            [CardStyle.FeatureCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Paragraph },
            [CardStyle.ServiceCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Paragraph, CardPart.Emphasis, CardPart.Actions },
            [CardStyle.IconCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Paragraph },
            [CardStyle.BenefitCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Paragraph },

            // E-commerce
            [CardStyle.ProductCard] = new[] { CardPart.Hero, CardPart.Header, CardPart.Emphasis, CardPart.Rating, CardPart.Actions },
            [CardStyle.PricingCard] = new[] { CardPart.Header, CardPart.Emphasis, CardPart.Paragraph, CardPart.Actions },
            [CardStyle.OfferCard] = new[] { CardPart.Hero, CardPart.Badges, CardPart.Header, CardPart.Paragraph, CardPart.Emphasis, CardPart.Actions },
            [CardStyle.CartItemCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Emphasis, CardPart.Actions },

            // Social & interaction
            [CardStyle.SocialMediaCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Subtitle, CardPart.Paragraph, CardPart.Actions },
            [CardStyle.TestimonialCard] = new[] { CardPart.Avatar, CardPart.Paragraph, CardPart.Header, CardPart.Subtitle, CardPart.Rating },
            [CardStyle.ReviewCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Rating, CardPart.Paragraph },
            [CardStyle.CommentCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Subtitle, CardPart.Paragraph },

            // Dashboard & analytics
            [CardStyle.StatCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Emphasis, CardPart.Badges },
            [CardStyle.ChartCard] = new[] { CardPart.Header, CardPart.Subtitle, CardPart.Paragraph },
            [CardStyle.MetricCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Emphasis, CardPart.Badges },
            [CardStyle.ActivityCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Subtitle, CardPart.Paragraph },

            // Communication
            [CardStyle.NotificationCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Paragraph, CardPart.Subtitle },
            [CardStyle.MessageCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Paragraph, CardPart.Subtitle },
            [CardStyle.AlertCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Paragraph, CardPart.Actions },
            [CardStyle.AnnouncementCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Paragraph, CardPart.Actions },

            // Event & calendar
            [CardStyle.EventCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Subtitle, CardPart.Paragraph, CardPart.Actions },
            [CardStyle.CalendarEventCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Subtitle },
            [CardStyle.ScheduleCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Subtitle, CardPart.Paragraph },
            [CardStyle.TaskCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Paragraph, CardPart.Status, CardPart.Actions },

            // List & data
            [CardStyle.ListCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Subtitle },
            [CardStyle.DataCard] = new[] { CardPart.Header, CardPart.Paragraph },
            [CardStyle.FormCard] = new[] { CardPart.Header, CardPart.Paragraph, CardPart.Actions },
            [CardStyle.SettingsCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Paragraph, CardPart.Status },

            // Specialised
            [CardStyle.DialogCard] = new[] { CardPart.Header, CardPart.Paragraph, CardPart.Actions },
            [CardStyle.BasicCard] = new[] { CardPart.Header, CardPart.Paragraph },
            [CardStyle.HoverCard] = new[] { CardPart.Header, CardPart.Paragraph, CardPart.Actions },
            [CardStyle.InteractiveCard] = new[] { CardPart.Header, CardPart.Paragraph, CardPart.Actions },
            [CardStyle.ImageCard] = new[] { CardPart.Hero, CardPart.Header, CardPart.Paragraph },
            [CardStyle.VideoCard] = new[] { CardPart.Hero, CardPart.Header, CardPart.Subtitle },
            [CardStyle.DownloadCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Subtitle, CardPart.Actions },
            [CardStyle.ContactCard] = new[] { CardPart.Avatar, CardPart.Header, CardPart.Subtitle, CardPart.Paragraph, CardPart.Actions },

            // The *Only styles: one part each, which is what the name promises.
            [CardStyle.BlankCard] = Empty,
            [CardStyle.HeaderOnlyCard] = new[] { CardPart.Header },
            [CardStyle.ParagraphOnlyCard] = new[] { CardPart.Paragraph },
            [CardStyle.ButtonOnlyCard] = new[] { CardPart.PrimaryAction },
            [CardStyle.SecondaryButtonOnlyCard] = new[] { CardPart.SecondaryAction },
            [CardStyle.BadgeOnlyCard] = new[] { CardPart.Badges },
            [CardStyle.RatingOnlyCard] = new[] { CardPart.Rating },
            [CardStyle.StatusOnlyCard] = new[] { CardPart.Status },
            [CardStyle.ImageOnlyCard] = new[] { CardPart.Hero },
            [CardStyle.VideoOnlyCard] = new[] { CardPart.Hero },
            [CardStyle.DownloadOnlyCard] = new[] { CardPart.PrimaryAction },
            [CardStyle.ContactOnlyCard] = new[] { CardPart.Subtitle },
        };
    }
}

using System.Reflection;

namespace TheTechIdea.Beep.Icons
{
    /// <summary>
    /// Static class providing access to embedded streaming sports SVG image paths in the Beep.Winform.Controls assembly.
    /// Constants are generated from actual files under GFX/streamingsports.
    /// Ideal for news banners, live streaming indicators, and sports broadcasts.
    /// </summary>
    public static class SvgsStreamingSports
    {
        private const string BaseNamespace = "TheTechIdea.Beep.Winform.Controls.GFX.streamingsports";

        /// <summary>
        /// Gets the assembly containing the embedded SVG resources.
        /// </summary>
        public static Assembly ResourceAssembly => typeof(SvgsStreamingSports).Assembly;

        // Sports Equipment & Activities
        public static readonly string Skateboard = $"{BaseNamespace}.001-skateboard.svg";
        public static readonly string Boxing = $"{BaseNamespace}.004-boxing.svg";
        public static readonly string Baseball = $"{BaseNamespace}.005-baseball.svg";
        public static readonly string Tennis = $"{BaseNamespace}.007-tennis.svg";
        public static readonly string Football = $"{BaseNamespace}.010-football.svg";
        public static readonly string Basketball = $"{BaseNamespace}.015-basketball.svg";
        public static readonly string Soccer = $"{BaseNamespace}.018-soccer.svg";
        public static readonly string Rugby = $"{BaseNamespace}.025-rugby.svg";
        public static readonly string Golf = $"{BaseNamespace}.026-golf.svg";
        public static readonly string Soccer2 = $"{BaseNamespace}.027-soccer.svg";
        public static readonly string Formula1 = $"{BaseNamespace}.030-formula 1.svg";
        public static readonly string Running = $"{BaseNamespace}.035-running.svg";
        public static readonly string Ball = $"{BaseNamespace}.037-ball.svg";
        public static readonly string PingPong = $"{BaseNamespace}.038-ping pong.svg";
        public static readonly string AmericanFootball = $"{BaseNamespace}.039-american football.svg";
        public static readonly string Football2 = $"{BaseNamespace}.040-football.svg";
        public static readonly string Gym = $"{BaseNamespace}.041-gym.svg";
        public static readonly string Football3 = $"{BaseNamespace}.042-football.svg";
        public static readonly string Basketball2 = $"{BaseNamespace}.044-basketball.svg";
        public static readonly string Football4 = $"{BaseNamespace}.047-football.svg";
        public static readonly string Hockey = $"{BaseNamespace}.048-hockey.svg";
        public static readonly string Sports = $"{BaseNamespace}.050-sports.svg";

        // Streaming & Broadcasting
        public static readonly string Advertising = $"{BaseNamespace}.002-advertising.svg";
        public static readonly string Tv = $"{BaseNamespace}.011-tv.svg";
        public static readonly string Stream = $"{BaseNamespace}.017-stream.svg";
        public static readonly string Live = $"{BaseNamespace}.019-live.svg";
        public static readonly string Video = $"{BaseNamespace}.029-video.svg";
        public static readonly string OnAir = $"{BaseNamespace}.032-on air.svg";
        public static readonly string Camera = $"{BaseNamespace}.036-camera.svg";
        public static readonly string Live2 = $"{BaseNamespace}.043-live.svg";
        public static readonly string Devices = $"{BaseNamespace}.045-devices.svg";

        // News & Social
        public static readonly string Donate = $"{BaseNamespace}.003-donate.svg";
        public static readonly string Time = $"{BaseNamespace}.008-time.svg";
        public static readonly string Commentator = $"{BaseNamespace}.009-commentator.svg";
        public static readonly string Apps = $"{BaseNamespace}.012-apps.svg";
        public static readonly string Notification = $"{BaseNamespace}.013-notification.svg";
        public static readonly string Fans = $"{BaseNamespace}.014-fans.svg";
        public static readonly string Strategy = $"{BaseNamespace}.016-strategy.svg";
        public static readonly string Sport = $"{BaseNamespace}.020-sport.svg";
        public static readonly string Football5 = $"{BaseNamespace}.021-football.svg";
        public static readonly string Selfie = $"{BaseNamespace}.022-selfie.svg";
        public static readonly string Score = $"{BaseNamespace}.031-score.svg";
        public static readonly string Trending = $"{BaseNamespace}.033-trending.svg";
        public static readonly string Chatbot = $"{BaseNamespace}.034-chatbot.svg";
        public static readonly string Comments = $"{BaseNamespace}.046-comments.svg";
        public static readonly string Discussion = $"{BaseNamespace}.049-discussion.svg";

        // Security & Status
        public static readonly string Lock = $"{BaseNamespace}.006-lock.svg";
        public static readonly string PaidContent = $"{BaseNamespace}.023-paid content.svg";
        public static readonly string Cloud = $"{BaseNamespace}.024-cloud.svg";
        public static readonly string Error = $"{BaseNamespace}.028-error.svg";
    }
}

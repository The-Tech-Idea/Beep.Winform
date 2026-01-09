using Svg;
using System.IO;
using System.Reflection;
 
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Icons
{
    /// <summary>
    /// Static class providing easy access to all embedded SVG image paths in the Beep.Winform.Controls assembly.
    /// All paths are formatted as embedded resource names for use with Assembly.GetManifestResourceStream().
    /// UI Icons from Material Design Icons (https://materialdesignicons.com/)
    /// </summary>
    public static class SvgsUI
    {
        private const string BaseNamespace = "TheTechIdea.Beep.Winform.Controls.GFX.Icons.UI";

        /// <summary>
        /// Gets the assembly containing the embedded SVG resources.
        /// </summary>
        public static Assembly ResourceAssembly => Assembly.GetExecutingAssembly();

        #region "Alerts & Notifications"
        public static readonly string AlertCircle = $"{BaseNamespace}.alert-circle.svg";
        public static readonly string AlertOctagon = $"{BaseNamespace}.alert-octagon.svg";
        public static readonly string AlertTriangle = $"{BaseNamespace}.alert-triangle.svg";
        public static readonly string Bell = $"{BaseNamespace}.bell.svg";
        public static readonly string BellOff = $"{BaseNamespace}.bell-off.svg";
        public static readonly string CheckCircle = $"{BaseNamespace}.check-circle.svg";
        public static readonly string Info = $"{BaseNamespace}.info.svg";
        public static readonly string LifeBuoy = $"{BaseNamespace}.life-buoy.svg";
        #endregion

        #region "Arrows & Navigation"
        public static readonly string ArrowDown = $"{BaseNamespace}.arrow-down.svg";
        public static readonly string ArrowDownCircle = $"{BaseNamespace}.arrow-down-circle.svg";
        public static readonly string ArrowDownLeft = $"{BaseNamespace}.arrow-down-left.svg";
        public static readonly string ArrowDownRight = $"{BaseNamespace}.arrow-down-right.svg";
        public static readonly string ArrowLeft = $"{BaseNamespace}.arrow-left.svg";
        public static readonly string ArrowLeftCircle = $"{BaseNamespace}.arrow-left-circle.svg";
        public static readonly string ArrowRight = $"{BaseNamespace}.arrow-right.svg";
        public static readonly string ArrowRightCircle = $"{BaseNamespace}.arrow-right-circle.svg";
        public static readonly string ArrowUp = $"{BaseNamespace}.arrow-up.svg";
        public static readonly string ArrowUpCircle = $"{BaseNamespace}.arrow-up-circle.svg";
        public static readonly string ArrowUpLeft = $"{BaseNamespace}.arrow-up-left.svg";
        public static readonly string ArrowUpRight = $"{BaseNamespace}.arrow-up-right.svg";
        public static readonly string ChevronDown = $"{BaseNamespace}.chevron-down.svg";
        public static readonly string ChevronLeft = $"{BaseNamespace}.chevron-left.svg";
        public static readonly string ChevronRight = $"{BaseNamespace}.chevron-right.svg";
        public static readonly string ChevronUp = $"{BaseNamespace}.chevron-up.svg";
        public static readonly string ChevronsDown = $"{BaseNamespace}.chevrons-down.svg";
        public static readonly string ChevronsLeft = $"{BaseNamespace}.chevrons-left.svg";
        public static readonly string ChevronsRight = $"{BaseNamespace}.chevrons-right.svg";
        public static readonly string ChevronsUp = $"{BaseNamespace}.chevrons-up.svg";
        public static readonly string CornerDownLeft = $"{BaseNamespace}.corner-down-left.svg";
        public static readonly string CornerDownRight = $"{BaseNamespace}.corner-down-right.svg";
        public static readonly string CornerLeftDown = $"{BaseNamespace}.corner-left-down.svg";
        public static readonly string CornerLeftUp = $"{BaseNamespace}.corner-left-up.svg";
        public static readonly string CornerRightDown = $"{BaseNamespace}.corner-right-down.svg";
        public static readonly string CornerRightUp = $"{BaseNamespace}.corner-right-up.svg";
        public static readonly string CornerUpLeft = $"{BaseNamespace}.corner-up-left.svg";
        public static readonly string CornerUpRight = $"{BaseNamespace}.corner-up-right.svg";
        public static readonly string ExternalLink = $"{BaseNamespace}.external-link.svg";
        public static readonly string FastForward = $"{BaseNamespace}.fast-forward.svg";
        public static readonly string Navigation = $"{BaseNamespace}.navigation.svg";
        public static readonly string Navigation2 = $"{BaseNamespace}.navigation-2.svg";
        public static readonly string Rewind = $"{BaseNamespace}.rewind.svg";
        public static readonly string SkipBack = $"{BaseNamespace}.skip-back.svg";
        public static readonly string SkipForward = $"{BaseNamespace}.skip-forward.svg";
        #endregion

        #region "Buttons & Controls"
        public static readonly string Check = $"{BaseNamespace}.check.svg";
        public static readonly string CheckSquare = $"{BaseNamespace}.check-square.svg";
        public static readonly string Circle = $"{BaseNamespace}.circle.svg";
        public static readonly string Minus = $"{BaseNamespace}.minus.svg";
        public static readonly string MinusCircle = $"{BaseNamespace}.minus-circle.svg";
        public static readonly string MinusSquare = $"{BaseNamespace}.minus-square.svg";
        public static readonly string Plus = $"{BaseNamespace}.plus.svg";
        public static readonly string PlusCircle = $"{BaseNamespace}.plus-circle.svg";
        public static readonly string PlusSquare = $"{BaseNamespace}.plus-square.svg";
        public static readonly string Square = $"{BaseNamespace}.square.svg";
        public static readonly string ToggleLeft = $"{BaseNamespace}.toggle-left.svg";
        public static readonly string ToggleRight = $"{BaseNamespace}.toggle-right.svg";
        public static readonly string X = $"{BaseNamespace}.x.svg";
        public static readonly string XCircle = $"{BaseNamespace}.x-circle.svg";
        public static readonly string XOctagon = $"{BaseNamespace}.x-octagon.svg";
        public static readonly string XSquare = $"{BaseNamespace}.x-square.svg";
        #endregion

        #region "Media & Images"
        public static readonly string Activity = $"{BaseNamespace}.activity.svg";
        public static readonly string Airplay = $"{BaseNamespace}.airplay.svg";
        public static readonly string Camera = $"{BaseNamespace}.camera.svg";
        public static readonly string CameraOff = $"{BaseNamespace}.camera-off.svg";
        public static readonly string Cast = $"{BaseNamespace}.cast.svg";
        public static readonly string Disc = $"{BaseNamespace}.disc.svg";
        public static readonly string Dribbble = $"{BaseNamespace}.dribbble.svg";
        public static readonly string Film = $"{BaseNamespace}.film.svg";
        public static readonly string Image = $"{BaseNamespace}.image.svg";
        public static readonly string Instagram = $"{BaseNamespace}.instagram.svg";
        public static readonly string Pause = $"{BaseNamespace}.pause.svg";
        public static readonly string PauseCircle = $"{BaseNamespace}.pause-circle.svg";
        public static readonly string Play = $"{BaseNamespace}.play.svg";
        public static readonly string PlayCircle = $"{BaseNamespace}.play-circle.svg";
        public static readonly string Repeat = $"{BaseNamespace}.repeat.svg";
        public static readonly string Shuffle = $"{BaseNamespace}.shuffle.svg";
        public static readonly string Sound = $"{BaseNamespace}.sound.svg";
        public static readonly string Twitch = $"{BaseNamespace}.twitch.svg";
        public static readonly string Twitter = $"{BaseNamespace}.twitter.svg";
        public static readonly string Video = $"{BaseNamespace}.video.svg";
        public static readonly string VideoOff = $"{BaseNamespace}.video-off.svg";
        public static readonly string Volume = $"{BaseNamespace}.volume.svg";
        public static readonly string Volume1 = $"{BaseNamespace}.volume-1.svg";
        public static readonly string Volume2 = $"{BaseNamespace}.volume-2.svg";
        public static readonly string VolumeX = $"{BaseNamespace}.volume-x.svg";
        public static readonly string Youtube = $"{BaseNamespace}.youtube.svg";
        #endregion

        #region "Text & Editing"
        public static readonly string AlignCenter = $"{BaseNamespace}.align-center.svg";
        public static readonly string AlignJustify = $"{BaseNamespace}.align-justify.svg";
        public static readonly string AlignLeft = $"{BaseNamespace}.align-left.svg";
        public static readonly string AlignRight = $"{BaseNamespace}.align-right.svg";
        public static readonly string Bold = $"{BaseNamespace}.bold.svg";
        public static readonly string Code = $"{BaseNamespace}.code.svg";
        public static readonly string DotsVertical = $"{BaseNamespace}.dotsvertical.svg";
        public static readonly string Edit = $"{BaseNamespace}.edit.svg";
        public static readonly string Edit2 = $"{BaseNamespace}.edit-2.svg";
        public static readonly string Edit3 = $"{BaseNamespace}.edit-3.svg";
        public static readonly string Ellipsis = $"{BaseNamespace}.ellipsis.svg";
        public static readonly string FileText = $"{BaseNamespace}.file-text.svg";
        public static readonly string Hash = $"{BaseNamespace}.hash.svg";
        public static readonly string Italic = $"{BaseNamespace}.italic.svg";
        public static readonly string List = $"{BaseNamespace}.list.svg";
        public static readonly string Slash = $"{BaseNamespace}.slash.svg";
        public static readonly string Sliders = $"{BaseNamespace}.sliders.svg";
        public static readonly string Type = $"{BaseNamespace}.type.svg";
        public static readonly string Underline = $"{BaseNamespace}.underline.svg";
        #endregion

        #region "File & Folder"
        public static readonly string Archive = $"{BaseNamespace}.archive.svg";
        public static readonly string Doc = $"{BaseNamespace}.doc.svg";
        public static readonly string Docx = $"{BaseNamespace}.docx.svg";
        public static readonly string File = $"{BaseNamespace}.file.svg";
        public static readonly string FileMinus = $"{BaseNamespace}.file-minus.svg";
        public static readonly string FilePlus = $"{BaseNamespace}.file-plus.svg";
        public static readonly string Folder = $"{BaseNamespace}.folder.svg";
        public static readonly string FolderMinus = $"{BaseNamespace}.folder-minus.svg";
        public static readonly string FolderPlus = $"{BaseNamespace}.folder-plus.svg";
        public static readonly string Pdf = $"{BaseNamespace}.pdf.svg";
        public static readonly string Ppt = $"{BaseNamespace}.ppt.svg";
        public static readonly string Pptx = $"{BaseNamespace}.pptx.svg";
        public static readonly string Xls = $"{BaseNamespace}.xls.svg";
        public static readonly string Xlsx = $"{BaseNamespace}.xlsx.svg";
        #endregion

        #region "Cloud & Weather"
        public static readonly string Cloud = $"{BaseNamespace}.cloud.svg";
        public static readonly string CloudDrizzle = $"{BaseNamespace}.cloud-drizzle.svg";
        public static readonly string CloudLightning = $"{BaseNamespace}.cloud-lightning.svg";
        public static readonly string CloudOff = $"{BaseNamespace}.cloud-off.svg";
        public static readonly string CloudRain = $"{BaseNamespace}.cloud-rain.svg";
        public static readonly string CloudSnow = $"{BaseNamespace}.cloud-snow.svg";
        public static readonly string Sun = $"{BaseNamespace}.sun.svg";
        public static readonly string Sunrise = $"{BaseNamespace}.sunrise.svg";
        public static readonly string Sunset = $"{BaseNamespace}.sunset.svg";
        public static readonly string Wind = $"{BaseNamespace}.wind.svg";
        #endregion

        #region "User & People"
        public static readonly string Person = $"{BaseNamespace}.person.svg";
        public static readonly string User = $"{BaseNamespace}.user.svg";
        public static readonly string UserCheck = $"{BaseNamespace}.user-check.svg";
        public static readonly string UserCircle = $"{BaseNamespace}.usercircle.svg";
        public static readonly string UserMinus = $"{BaseNamespace}.user-minus.svg";
        public static readonly string UserPlus = $"{BaseNamespace}.user-plus.svg";
        public static readonly string UserX = $"{BaseNamespace}.user-x.svg";
        public static readonly string Users = $"{BaseNamespace}.users.svg";
        #endregion

        #region "Communication"
        public static readonly string AtSign = $"{BaseNamespace}.at-sign.svg";
        public static readonly string Mail = $"{BaseNamespace}.mail.svg";
        public static readonly string MessageCircle = $"{BaseNamespace}.message-circle.svg";
        public static readonly string MessageSquare = $"{BaseNamespace}.message-square.svg";
        public static readonly string Mic = $"{BaseNamespace}.mic.svg";
        public static readonly string MicOff = $"{BaseNamespace}.mic-off.svg";
        public static readonly string Phone = $"{BaseNamespace}.phone.svg";
        public static readonly string PhoneCall = $"{BaseNamespace}.phone-call.svg";
        public static readonly string PhoneForwarded = $"{BaseNamespace}.phone-forwarded.svg";
        public static readonly string PhoneIncoming = $"{BaseNamespace}.phone-incoming.svg";
        public static readonly string PhoneMissed = $"{BaseNamespace}.phone-missed.svg";
        public static readonly string PhoneOff = $"{BaseNamespace}.phone-off.svg";
        public static readonly string PhoneOutgoing = $"{BaseNamespace}.phone-outgoing.svg";
        public static readonly string Send = $"{BaseNamespace}.send.svg";
        public static readonly string Slack = $"{BaseNamespace}.slack.svg";
        public static readonly string Trello = $"{BaseNamespace}.trello.svg";
        #endregion

        #region "Shopping & Commerce"
        public static readonly string Cart = $"{BaseNamespace}.cart.svg";
        public static readonly string CreditCard = $"{BaseNamespace}.credit-card.svg";
        public static readonly string Dollar = $"{BaseNamespace}.dollar.svg";
        public static readonly string DollarSign = $"{BaseNamespace}.dollar-sign.svg";
        public static readonly string Gift = $"{BaseNamespace}.gift.svg";
        public static readonly string Shopping = $"{BaseNamespace}.shopping.svg";
        public static readonly string ShoppingBag = $"{BaseNamespace}.shopping-bag.svg";
        public static readonly string ShoppingCart = $"{BaseNamespace}.shopping-cart.svg";
        public static readonly string Shopify = $"{BaseNamespace}.shopify.svg";
        public static readonly string Stripe = $"{BaseNamespace}.stripe.svg";
        public static readonly string Trello2 = $"{BaseNamespace}.trello.svg";
        #endregion

        #region "Authentication & Login"
        public static readonly string Github = $"{BaseNamespace}.github.svg";
        public static readonly string Google = $"{BaseNamespace}.google.svg";
        public static readonly string LogIn = $"{BaseNamespace}.login.svg";
        public static readonly string Microsoft = $"{BaseNamespace}.microsoft.svg";
        #endregion

        #region "Branding & Logos"
        public static readonly string Brand = $"{BaseNamespace}.trademark.svg";
        public static readonly string Logo = $"{BaseNamespace}.trademark.svg";
        #endregion

        #region "Science & Technology"
        public static readonly string Aperture = $"{BaseNamespace}.aperture.svg";
        public static readonly string Bluetooth = $"{BaseNamespace}.bluetooth.svg";
        public static readonly string Cpu = $"{BaseNamespace}.cpu.svg";
        public static readonly string Database = $"{BaseNamespace}.database.svg";
        public static readonly string HardDrive = $"{BaseNamespace}.hard-drive.svg";
        public static readonly string Monitor = $"{BaseNamespace}.monitor.svg";
        public static readonly string Radio = $"{BaseNamespace}.radio.svg";
        public static readonly string Server = $"{BaseNamespace}.server.svg";
        public static readonly string Signal = $"{BaseNamespace}.signal.svg";
        public static readonly string SignalOff = $"{BaseNamespace}.signaloff.svg";
        public static readonly string Smartphone = $"{BaseNamespace}.smartphone.svg";
        public static readonly string Tablet = $"{BaseNamespace}.tablet.svg";
        public static readonly string Terminal = $"{BaseNamespace}.terminal.svg";
        public static readonly string Tv = $"{BaseNamespace}.tv.svg";
        public static readonly string Watch = $"{BaseNamespace}.watch.svg";
        public static readonly string Wifi = $"{BaseNamespace}.wifi.svg";
        public static readonly string WifiOff = $"{BaseNamespace}.wifi-off.svg";
        #endregion

        #region "Navigation & Layout"
        public static readonly string Apps = $"{BaseNamespace}.apps.svg";
        public static readonly string Columns = $"{BaseNamespace}.columns.svg";
        public static readonly string Command = $"{BaseNamespace}.command.svg";
        public static readonly string Compass = $"{BaseNamespace}.compass.svg";
        public static readonly string Grid = $"{BaseNamespace}.grid.svg";
        public static readonly string Home = $"{BaseNamespace}.home.svg";
        public static readonly string Layout = $"{BaseNamespace}.layout.svg";
        public static readonly string Menu = $"{BaseNamespace}.menu.svg";
        public static readonly string Sidebar = $"{BaseNamespace}.sidebar.svg";
        #endregion

        #region "Search & Find"
        public static readonly string Search = $"{BaseNamespace}.search.svg";
        public static readonly string ZoomIn = $"{BaseNamespace}.zoom-in.svg";
        public static readonly string ZoomOut = $"{BaseNamespace}.zoom-out.svg";
        #endregion

        #region "Maps & Location"
        public static readonly string Compass2 = $"{BaseNamespace}.compass.svg";
        public static readonly string Globe = $"{BaseNamespace}.globe.svg";
        public static readonly string MapPin = $"{BaseNamespace}.map-pin.svg";
        public static readonly string Map = $"{BaseNamespace}.map.svg";
        #endregion

        #region "Time & Calendar"
        public static readonly string Calendar = $"{BaseNamespace}.calendar.svg";
        public static readonly string Clock = $"{BaseNamespace}.clock.svg";
        public static readonly string Loader = $"{BaseNamespace}.loader.svg";
        public static readonly string Moon = $"{BaseNamespace}.moon.svg";
        public static readonly string Watch2 = $"{BaseNamespace}.watch.svg";
        #endregion

        #region "Charts & Analytics"
        public static readonly string BarChart = $"{BaseNamespace}.bar-chart.svg";
        public static readonly string BarChart2 = $"{BaseNamespace}.bar-chart-2.svg";
        public static readonly string Chart = $"{BaseNamespace}.chart.svg";
        public static readonly string PieChart = $"{BaseNamespace}.pie-chart.svg";
        public static readonly string TrendingDown = $"{BaseNamespace}.trending-down.svg";
        public static readonly string TrendingUp = $"{BaseNamespace}.trending-up.svg";
        #endregion

        #region "Drawing & Design"
        public static readonly string Anchor = $"{BaseNamespace}.anchor.svg";
        public static readonly string Award = $"{BaseNamespace}.award.svg";
        public static readonly string Clipboard = $"{BaseNamespace}.clipboard.svg";
        public static readonly string Code2 = $"{BaseNamespace}.code.svg";
        public static readonly string Codepen = $"{BaseNamespace}.codepen.svg";
        public static readonly string Codesandbox = $"{BaseNamespace}.codesandbox.svg";
        public static readonly string Command2 = $"{BaseNamespace}.command.svg";
        public static readonly string Crop = $"{BaseNamespace}.crop.svg";
        public static readonly string Crosshair = $"{BaseNamespace}.crosshair.svg";
        public static readonly string Droplet = $"{BaseNamespace}.droplet.svg";
        public static readonly string Eye = $"{BaseNamespace}.eye.svg";
        public static readonly string EyeOff = $"{BaseNamespace}.eye-off.svg";
        public static readonly string Feather = $"{BaseNamespace}.feather.svg";
        public static readonly string Figma = $"{BaseNamespace}.figma.svg";
        public static readonly string Framer = $"{BaseNamespace}.framer.svg";
        public static readonly string Layers = $"{BaseNamespace}.layers.svg";
        public static readonly string Move = $"{BaseNamespace}.move.svg";
        public static readonly string PenTool = $"{BaseNamespace}.pen-tool.svg";
        public static readonly string RotateCcw = $"{BaseNamespace}.rotate-ccw.svg";
        public static readonly string RotateCw = $"{BaseNamespace}.rotate-cw.svg";
        public static readonly string Scissors = $"{BaseNamespace}.scissors.svg";
        public static readonly string Triangle = $"{BaseNamespace}.triangle.svg";
        #endregion

        #region "Objects & Shapes"
        public static readonly string Box = $"{BaseNamespace}.box.svg";
        public static readonly string Hexagon = $"{BaseNamespace}.hexagon.svg";
        public static readonly string Octagon = $"{BaseNamespace}.octagon.svg";
        public static readonly string Package = $"{BaseNamespace}.package.svg";
        #endregion

        #region "Emotions & Expressions"
        public static readonly string Frown = $"{BaseNamespace}.frown.svg";
        public static readonly string Happy = $"{BaseNamespace}.happy.svg";
        public static readonly string Meh = $"{BaseNamespace}.meh.svg";
        public static readonly string Sad = $"{BaseNamespace}.sad.svg";
        public static readonly string Smile = $"{BaseNamespace}.smile.svg";
        #endregion

        #region "Favorites & Social"
        public static readonly string BookOpen = $"{BaseNamespace}.book-open.svg";
        public static readonly string Book = $"{BaseNamespace}.book.svg";
        public static readonly string Bookmark = $"{BaseNamespace}.bookmark.svg";
        public static readonly string Facebook = $"{BaseNamespace}.facebook.svg";
        public static readonly string Heart = $"{BaseNamespace}.heart.svg";
        public static readonly string HeartOff = $"{BaseNamespace}.heartoff.svg";
        public static readonly string Linkedin = $"{BaseNamespace}.linkedin.svg";
        public static readonly string Star = $"{BaseNamespace}.star.svg";
        public static readonly string ThumbsDown = $"{BaseNamespace}.thumbs-down.svg";
        public static readonly string ThumbsUp = $"{BaseNamespace}.thumbs-up.svg";
        #endregion

        #region "Lock & Security"
        public static readonly string Key = $"{BaseNamespace}.key.svg";
        public static readonly string Lock = $"{BaseNamespace}.lock.svg";
        public static readonly string Unlock = $"{BaseNamespace}.unlock.svg";
        public static readonly string Shield = $"{BaseNamespace}.shield.svg";
        public static readonly string ShieldOff = $"{BaseNamespace}.shield-off.svg";
        #endregion

        #region "Miscellaneous"
        public static readonly string Battery = $"{BaseNamespace}.battery.svg";
        public static readonly string BatteryCharging = $"{BaseNamespace}.battery-charging.svg";
        public static readonly string Briefcase = $"{BaseNamespace}.briefcase.svg";
        public static readonly string Chrome = $"{BaseNamespace}.chrome.svg";
        public static readonly string Coffee = $"{BaseNamespace}.coffee.svg";
        public static readonly string Company = $"{BaseNamespace}.company.svg";
        public static readonly string Copy2 = $"{BaseNamespace}.copy.svg";
        public static readonly string Currency = $"{BaseNamespace}.currency.svg";
        public static readonly string Divide = $"{BaseNamespace}.divide.svg";
        public static readonly string DivideCircle = $"{BaseNamespace}.divide-circle.svg";
        public static readonly string DivideSquare = $"{BaseNamespace}.divide-square.svg";
        public static readonly string Dot = $"{BaseNamespace}.dot.svg";
        public static readonly string Droplet2 = $"{BaseNamespace}.droplet.svg";
        public static readonly string Headphones = $"{BaseNamespace}.headphones.svg";
        public static readonly string HelpCircle = $"{BaseNamespace}.help-circle.svg";
        public static readonly string Inbox = $"{BaseNamespace}.inbox.svg";
        public static readonly string More = $"{BaseNamespace}.more.svg";
        public static readonly string Paperclip = $"{BaseNamespace}.paperclip.svg";
        public static readonly string Percent = $"{BaseNamespace}.percent.svg";
        public static readonly string Pocket = $"{BaseNamespace}.pocket.svg";
        public static readonly string Power = $"{BaseNamespace}.power.svg";
        public static readonly string PowerOff = $"{BaseNamespace}.poweroff.svg";
        public static readonly string Printer = $"{BaseNamespace}.printer.svg";
        public static readonly string RefreshCcw = $"{BaseNamespace}.refresh-ccw.svg";
        public static readonly string RefreshCw = $"{BaseNamespace}.refresh-cw.svg";
        public static readonly string Rss = $"{BaseNamespace}.rss.svg";
        public static readonly string Save = $"{BaseNamespace}.save.svg";
        public static readonly string Settings = $"{BaseNamespace}.settings.svg";
        public static readonly string Share = $"{BaseNamespace}.share.svg";
        public static readonly string Share2 = $"{BaseNamespace}.share-2.svg";
        public static readonly string Speaker = $"{BaseNamespace}.speaker.svg";
        public static readonly string Tag = $"{BaseNamespace}.tag.svg";
        public static readonly string Target = $"{BaseNamespace}.target.svg";
        public static readonly string Thermometer = $"{BaseNamespace}.thermometer.svg";
        public static readonly string Tool = $"{BaseNamespace}.tool.svg";
        public static readonly string Trash = $"{BaseNamespace}.trash.svg";
        public static readonly string Trash2 = $"{BaseNamespace}.trash-2.svg";
        public static readonly string Truck = $"{BaseNamespace}.truck.svg";
        public static readonly string Umbrella = $"{BaseNamespace}.umbrella.svg";
        public static readonly string Zap = $"{BaseNamespace}.zap.svg";
        public static readonly string ZapOff = $"{BaseNamespace}.zap-off.svg";
        #endregion

        #region "Datasources & Databases"
        public static readonly string AnalyticsPlatform = $"{BaseNamespace}.analyticsplatform.svg";
        public static readonly string Asana = $"{BaseNamespace}.asana.svg";
        public static readonly string AwsIot = $"{BaseNamespace}.awsiot.svg";
        public static readonly string AwsIotAnalytics = $"{BaseNamespace}.awsiotanalytics.svg";
        public static readonly string AwsIotEvents = $"{BaseNamespace}.awsiotevents.svg";
        public static readonly string AzureBoards = $"{BaseNamespace}.azureboards.svg";
        public static readonly string Druid = $"{BaseNamespace}.druid.svg";
        public static readonly string Firebird = $"{BaseNamespace}.firebird.svg";
        public static readonly string Grpc = $"{BaseNamespace}.grpc.svg";
        public static readonly string Jira = $"{BaseNamespace}.jira.svg";
        public static readonly string Mailchimp = $"{BaseNamespace}.mailchimp.svg";
        public static readonly string MariaDb = $"{BaseNamespace}.mariadb.svg";
        public static readonly string Mongodb = $"{BaseNamespace}.mongodb.svg";
        public static readonly string Opencart = $"{BaseNamespace}.opencart.svg";
        public static readonly string Pcloud = $"{BaseNamespace}.pcloud.svg";
        public static readonly string Podio = $"{BaseNamespace}.podio.svg";
        public static readonly string Postgresql = $"{BaseNamespace}.postgresql.svg";
        public static readonly string RavenDb = $"{BaseNamespace}.ravendb.svg";
        public static readonly string Redis = $"{BaseNamespace}.redis.svg";
        public static readonly string Shopify2 = $"{BaseNamespace}.shopify.svg";
        public static readonly string Sqlite = $"{BaseNamespace}.sqlite.svg";
        public static readonly string Sqlserver = $"{BaseNamespace}.sqlserver.svg";
        public static readonly string Twist = $"{BaseNamespace}.twist.svg";
        public static readonly string Weaviate = $"{BaseNamespace}.weaviate.svg";
        public static readonly string Zendesk = $"{BaseNamespace}.zendesk.svg";
        #endregion

        /// <summary>
        /// Gets all SVG resource paths as a dictionary for easy enumeration.
        /// </summary>
        public static Dictionary<string, string> GetAllPaths()
        {
            var paths = new Dictionary<string, string>();
            var type = typeof(SvgsUI);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
            {
                if (field.FieldType == typeof(string) && field.IsLiteral == false && field.IsInitOnly)
                {
                    var value = field.GetValue(null) as string;
                    if (!string.IsNullOrEmpty(value) && value.EndsWith(".svg"))
                    {
                        paths[field.Name] = value;
                    }
                }
            }

            return paths;
        }

        /// <summary>
        /// Checks if a resource path exists in the assembly.
        /// </summary>
        /// <param name="resourcePath">The full resource path</param>
        /// <returns>True if the resource exists</returns>
        public static bool ResourceExists(string resourcePath)
        {
            var resourceNames = ResourceAssembly.GetManifestResourceNames();
            return resourceNames.Contains(resourcePath);
        }

        /// <summary>
        /// Gets all available SVG resource names from the assembly.
        /// </summary>
        /// <returns>Array of resource names</returns>
        public static string[] GetAvailableResources()
        {
            return ResourceAssembly.GetManifestResourceNames()
                .Where(name => name.StartsWith(BaseNamespace) && name.EndsWith(".svg"))
                .ToArray();
        }

        /// <summary>
        /// Helper method to get the full file system path (useful for development/debugging).
        /// This assumes the standard project structure.
        /// </summary>
        /// <param name="svgFileName">Just the SVG filename (e.g., "search.svg")</param>
        /// <returns>Full file system path</returns>
        public static string GetFileSystemPath(string svgFileName)
        {
            // This is for development use - gets the actual file path
            string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(baseDirectory, "..", "..", "..", "GFX", "Icons", "UI", svgFileName);
        }
    }

    /// <summary>
    /// Extension methods for easier use of SvgsUI with BeepImage controls.
    /// </summary>
    public static class BeepSvgUIExtensions
    {
        /// <summary>
        /// Sets the image path for a BeepImage control using SvgsUI
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <param name="svgPath">The SVG path from SvgsUI</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetUISvgPath(this BeepImage beepImage, string svgPath)
        {
            if (beepImage != null)
            {
                beepImage.ImagePath = svgPath;
            }
            return beepImage;
        }

        /// <summary>
        /// Sets a search icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetSearchIcon(this BeepImage beepImage)
        {
            return beepImage.SetUISvgPath(SvgsUI.Search);
        }

        /// <summary>
        /// Sets a close icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetCloseIcon(this BeepImage beepImage)
        {
            return beepImage.SetUISvgPath(SvgsUI.XCircle);
        }

        /// <summary>
        /// Sets an edit/pencil icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetEditIcon(this BeepImage beepImage)
        {
            return beepImage.SetUISvgPath(SvgsUI.Edit);
        }

        /// <summary>
        /// Sets a user icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetUserIcon(this BeepImage beepImage)
        {
            return beepImage.SetUISvgPath(SvgsUI.User);
        }

        /// <summary>
        /// Sets a plus/add icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetAddIcon(this BeepImage beepImage)
        {
            return beepImage.SetUISvgPath(SvgsUI.Plus);
        }

        /// <summary>
        /// Sets a delete/trash icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetDeleteIcon(this BeepImage beepImage)
        {
            return beepImage.SetUISvgPath(SvgsUI.Trash);
        }

        /// <summary>
        /// Sets a save icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetSaveIcon(this BeepImage beepImage)
        {
            return beepImage.SetUISvgPath(SvgsUI.Save);
        }

        /// <summary>
        /// Sets a refresh icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetRefreshIcon(this BeepImage beepImage)
        {
            return beepImage.SetUISvgPath(SvgsUI.RefreshCw);
        }

        /// <summary>
        /// Sets a settings/gear icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetSettingsIcon(this BeepImage beepImage)
        {
            return beepImage.SetUISvgPath(SvgsUI.Command);
        }

        /// <summary>
        /// Sets a home icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetHomeIcon(this BeepImage beepImage)
        {
            return beepImage.SetUISvgPath(SvgsUI.Home);
        }

        /// <summary>
        /// Sets a menu icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetMenuIcon(this BeepImage beepImage)
        {
            return beepImage.SetUISvgPath(SvgsUI.Menu);
        }

        /// <summary>
        /// Sets an alert/warning icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetAlertIcon(this BeepImage beepImage)
        {
            return beepImage.SetUISvgPath(SvgsUI.AlertTriangle);
        }

        /// <summary>
        /// Sets a success/check icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetSuccessIcon(this BeepImage beepImage)
        {
            return beepImage.SetUISvgPath(SvgsUI.CheckCircle);
        }

        /// <summary>
        /// Sets a help/question icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetHelpIcon(this BeepImage beepImage)
        {
            return beepImage.SetUISvgPath(SvgsUI.HelpCircle);
        }
    }
}
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Numerics
{
    /// <summary>
    /// Predefined numeric input mask presets for common business scenarios
    /// </summary>
    public enum NumericMaskPreset
    {
        /// <summary>No masking applied - free numeric input</summary>
        [Description("No mask")]
        None,

        /// <summary>US Phone: (###) ###-####</summary>
        [Description("(###) ###-####")]
        PhoneUS,

        /// <summary>International Phone: +## (###) ###-####</summary>
        [Description("+## (###) ###-####")]
        PhoneInternational,

        /// <summary>Social Security: ###-##-####</summary>
        [Description("###-##-####")]
        SSN,

        /// <summary>Credit Card: #### #### #### ####</summary>
        [Description("#### #### #### ####")]
        CreditCard,

        /// <summary>ZIP Code: #####</summary>
        [Description("#####")]
        ZipCode,

        /// <summary>ZIP+4: #####-####</summary>
        [Description("#####-####")]
        ZipCodePlus4,

        /// <summary>Currency: $#,###.##</summary>
        [Description("$#,###.##")]
        Currency,

        /// <summary>Percentage: ##.##%</summary>
        [Description("##.##%")]
        Percentage,

        /// <summary>Time (24h): ##:##</summary>
        [Description("##:##")]
        Time24Hour,

        /// <summary>Time (12h): ##:## AM/PM</summary>
        [Description("##:## AM/PM")]
        Time12Hour,

        /// <summary>Date: ##/##/####</summary>
        [Description("##/##/####")]
        DateMMDDYYYY,

        /// <summary>Date: ####-##-##</summary>
        [Description("####-##-##")]
        DateYYYYMMDD,

        /// <summary>IP Address: ###.###.###.###</summary>
        [Description("###.###.###.###")]
        IPAddress,

        /// <summary>MAC Address: ##:##:##:##:##:##</summary>
        [Description("##:##:##:##:##:##")]
        MACAddress,

        /// <summary>Decimal (2 places): #,###.##</summary>
        [Description("#,###.##")]
        Decimal2Places,

        /// <summary>Decimal (4 places): #,###.####</summary>
        [Description("#,###.####")]
        Decimal4Places,

        /// <summary>Integer with thousands: #,###</summary>
        [Description("#,###")]
        IntegerWithCommas,

        /// <summary>Account Number: ####-####-####</summary>
        [Description("####-####-####")]
        AccountNumber,

        /// <summary>Routing Number: #########</summary>
        [Description("#########")]
        RoutingNumber,

        /// <summary>EIN (Tax ID): ##-#######</summary>
        [Description("##-#######")]
        EIN,

        /// <summary>ISBN-10: #-###-#####-#</summary>
        [Description("#-###-#####-#")]
        ISBN10,

        /// <summary>ISBN-13: ###-#-###-#####-#</summary>
        [Description("###-#-###-#####-#")]
        ISBN13,

        /// <summary>Product Code: ###-###-###</summary>
        [Description("###-###-###")]
        ProductCode,

        /// <summary>Latitude: ##.######</summary>
        [Description("##.######")]
        Latitude,

        /// <summary>Longitude: ###.######</summary>
        [Description("###.######")]
        Longitude,

        /// <summary>Weight (kg): #,###.## kg</summary>
        [Description("#,###.## kg")]
        WeightKg,

        /// <summary>Weight (lbs): #,###.## lbs</summary>
        [Description("#,###.## lbs")]
        WeightLbs,

        /// <summary>Temperature (°C): ###.# °C</summary>
        [Description("###.# °C")]
        TemperatureCelsius,

        /// <summary>Temperature (°F): ###.# °F</summary>
        [Description("###.# °F")]
        TemperatureFahrenheit,

        /// <summary>Distance (km): #,###.## km</summary>
        [Description("#,###.## km")]
        DistanceKm,

        /// <summary>Distance (miles): #,###.## mi</summary>
        [Description("#,###.## mi")]
        DistanceMiles,

        /// <summary>File Size (MB): #,###.## MB</summary>
        [Description("#,###.## MB")]
        FileSizeMB,

        /// <summary>File Size (GB): #,###.## GB</summary>
        [Description("#,###.## GB")]
        FileSizeGB,

        /// <summary>Scientific Notation: #.####E+##</summary>
        [Description("#.####E+##")]
        Scientific,

        /// <summary>Hexadecimal: 0x########</summary>
        [Description("0x########")]
        Hexadecimal,

        /// <summary>Binary: 0b########</summary>
        [Description("0b########")]
        Binary,

        /// <summary>Custom format defined by user</summary>
        [Description("Custom")]
        Custom
    }

    /// <summary>
    /// Configuration for numeric mask presets
    /// </summary>
    public class NumericMaskConfig
    {
        public NumericMaskPreset Preset { get; set; }
        public string MaskPattern { get; set; }
        public string PlaceholderChar { get; set; } = "_";
        public bool AllowNegative { get; set; } = true;
        public int MaxLength { get; set; } = 50;
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public int DecimalPlaces { get; set; } = 2;
        public string IconPath { get; set; }
        public string Unit { get; set; }
        public bool ShowIcon { get; set; } = true;

        public static NumericMaskConfig FromPreset(NumericMaskPreset preset)
        {
            var config = new NumericMaskConfig { Preset = preset };

            switch (preset)
            {
                case NumericMaskPreset.PhoneUS:
                    config.MaskPattern = "(###) ###-####";
                    config.MaxLength = 14;
                    config.AllowNegative = false;
                    config.IconPath = "SvgsUI.Phone";
                    break;

                case NumericMaskPreset.PhoneInternational:
                    config.MaskPattern = "+## (###) ###-####";
                    config.MaxLength = 18;
                    config.AllowNegative = false;
                    config.IconPath = "SvgsUI.Phone";
                    break;

                case NumericMaskPreset.SSN:
                    config.MaskPattern = "###-##-####";
                    config.MaxLength = 11;
                    config.AllowNegative = false;
                    config.IconPath = "SvgsUI.IdCard";
                    break;

                case NumericMaskPreset.CreditCard:
                    config.MaskPattern = "#### #### #### ####";
                    config.MaxLength = 19;
                    config.AllowNegative = false;
                    config.IconPath = "SvgsUI.CreditCard";
                    break;

                case NumericMaskPreset.ZipCode:
                    config.MaskPattern = "#####";
                    config.MaxLength = 5;
                    config.AllowNegative = false;
                    config.IconPath = "SvgsUI.MapPin";
                    break;

                case NumericMaskPreset.ZipCodePlus4:
                    config.MaskPattern = "#####-####";
                    config.MaxLength = 10;
                    config.AllowNegative = false;
                    config.IconPath = "SvgsUI.MapPin";
                    break;

                case NumericMaskPreset.Currency:
                    config.MaskPattern = "$#,###.##";
                    config.DecimalPlaces = 2;
                    config.AllowNegative = true;
                    config.IconPath = "SvgsUI.DollarSign";
                    break;

                case NumericMaskPreset.Percentage:
                    config.MaskPattern = "##.##%";
                    config.DecimalPlaces = 2;
                    config.MinValue = 0;
                    config.MaxValue = 100;
                    config.AllowNegative = false;
                    config.IconPath = "SvgsUI.Percent";
                    break;

                case NumericMaskPreset.Time24Hour:
                    config.MaskPattern = "##:##";
                    config.MaxLength = 5;
                    config.AllowNegative = false;
                    config.IconPath = "SvgsUI.Clock";
                    break;

                case NumericMaskPreset.Time12Hour:
                    config.MaskPattern = "##:## AM";
                    config.MaxLength = 8;
                    config.AllowNegative = false;
                    config.IconPath = "SvgsUI.Clock";
                    break;

                case NumericMaskPreset.DateMMDDYYYY:
                    config.MaskPattern = "##/##/####";
                    config.MaxLength = 10;
                    config.AllowNegative = false;
                    config.IconPath = "SvgsUI.Calendar";
                    break;

                case NumericMaskPreset.DateYYYYMMDD:
                    config.MaskPattern = "####-##-##";
                    config.MaxLength = 10;
                    config.AllowNegative = false;
                    config.IconPath = "SvgsUI.Calendar";
                    break;

                case NumericMaskPreset.IPAddress:
                    config.MaskPattern = "###.###.###.###";
                    config.MaxLength = 15;
                    config.AllowNegative = false;
                    config.IconPath = "SvgsUI.Globe";
                    break;

                case NumericMaskPreset.MACAddress:
                    config.MaskPattern = "##:##:##:##:##:##";
                    config.MaxLength = 17;
                    config.AllowNegative = false;
                    config.IconPath = "SvgsUI.Wifi";
                    break;

                case NumericMaskPreset.Decimal2Places:
                    config.MaskPattern = "#,###.##";
                    config.DecimalPlaces = 2;
                    config.IconPath = "SvgsUI.Hash";
                    break;

                case NumericMaskPreset.Decimal4Places:
                    config.MaskPattern = "#,###.####";
                    config.DecimalPlaces = 4;
                    config.IconPath = "SvgsUI.Hash";
                    break;

                case NumericMaskPreset.IntegerWithCommas:
                    config.MaskPattern = "#,###";
                    config.DecimalPlaces = 0;
                    config.AllowNegative = true;
                    config.IconPath = "SvgsUI.Hash";
                    break;

                case NumericMaskPreset.AccountNumber:
                    config.MaskPattern = "####-####-####";
                    config.MaxLength = 14;
                    config.AllowNegative = false;
                    config.IconPath = "SvgsUI.CreditCard";
                    break;

                case NumericMaskPreset.RoutingNumber:
                    config.MaskPattern = "#########";
                    config.MaxLength = 9;
                    config.AllowNegative = false;
                    config.IconPath = "SvgsUI.Building";
                    break;

                case NumericMaskPreset.EIN:
                    config.MaskPattern = "##-#######";
                    config.MaxLength = 10;
                    config.AllowNegative = false;
                    config.IconPath = "SvgsUI.Building";
                    break;

                case NumericMaskPreset.WeightKg:
                    config.MaskPattern = "#,###.##";
                    config.DecimalPlaces = 2;
                    config.Unit = "kg";
                    config.IconPath = "Svgs.Weight";
                    break;

                case NumericMaskPreset.WeightLbs:
                    config.MaskPattern = "#,###.##";
                    config.DecimalPlaces = 2;
                    config.Unit = "lbs";
                    config.IconPath = "Svgs.Weight";
                    break;

                case NumericMaskPreset.TemperatureCelsius:
                    config.MaskPattern = "###.#";
                    config.DecimalPlaces = 1;
                    config.Unit = "°C";
                    config.IconPath = "SvgsUI.Thermometer";
                    break;

                case NumericMaskPreset.TemperatureFahrenheit:
                    config.MaskPattern = "###.#";
                    config.DecimalPlaces = 1;
                    config.Unit = "°F";
                    config.IconPath = "SvgsUI.Thermometer";
                    break;

                case NumericMaskPreset.DistanceKm:
                    config.MaskPattern = "#,###.##";
                    config.DecimalPlaces = 2;
                    config.Unit = "km";
                    config.IconPath = "SvgsUI.Navigation";
                    break;

                case NumericMaskPreset.DistanceMiles:
                    config.MaskPattern = "#,###.##";
                    config.DecimalPlaces = 2;
                    config.Unit = "mi";
                    config.IconPath = "SvgsUI.Navigation";
                    break;

                case NumericMaskPreset.FileSizeMB:
                    config.MaskPattern = "#,###.##";
                    config.DecimalPlaces = 2;
                    config.Unit = "MB";
                    config.IconPath = "SvgsUI.HardDrive";
                    break;

                case NumericMaskPreset.FileSizeGB:
                    config.MaskPattern = "#,###.##";
                    config.DecimalPlaces = 2;
                    config.Unit = "GB";
                    config.IconPath = "SvgsUI.HardDrive";
                    break;

                case NumericMaskPreset.Scientific:
                    config.MaskPattern = "#.####E+##";
                    config.DecimalPlaces = 4;
                    config.IconPath = "SvgsUI.Atom";
                    break;

                case NumericMaskPreset.Latitude:
                    config.MaskPattern = "##.######";
                    config.DecimalPlaces = 6;
                    config.MinValue = -90;
                    config.MaxValue = 90;
                    config.IconPath = "SvgsUI.MapPin";
                    break;

                case NumericMaskPreset.Longitude:
                    config.MaskPattern = "###.######";
                    config.DecimalPlaces = 6;
                    config.MinValue = -180;
                    config.MaxValue = 180;
                    config.IconPath = "SvgsUI.MapPin";
                    break;

                default:
                    config.MaskPattern = "";
                    break;
            }

            return config;
        }
    }
}

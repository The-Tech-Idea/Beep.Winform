using System.Drawing.Text;
using System.Reflection;
using TheTechIdea.Beep.Vis.Modules.Managers;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    /// <summary>
    /// Static class providing easy access to all embedded font resource paths in the Beep.Winform.Controls assembly.
    /// All paths are formatted as embedded resource names for use with Assembly.GetManifestResourceStream().
    /// </summary>
    public static class BeepFontPaths
    {
        private const string BaseNamespace = "TheTechIdea.Beep.Winform.Controls.Fonts";

        /// <summary>
        /// Gets the assembly containing the embedded font resources.
        /// </summary>
        public static Assembly ResourceAssembly => Assembly.GetExecutingAssembly();

        #region "Cairo Font Family"
        private const string CairoNamespace = BaseNamespace + ".Cairo";
        public static readonly string CairoBlack = $"{CairoNamespace}.Cairo-Black.ttf";
        public static readonly string CairoBold = $"{CairoNamespace}.Cairo-Bold.ttf";
        public static readonly string CairoExtraBold = $"{CairoNamespace}.Cairo-ExtraBold.ttf";
        public static readonly string CairoExtraLight = $"{CairoNamespace}.Cairo-ExtraLight.ttf";
        public static readonly string CairoLight = $"{CairoNamespace}.Cairo-Light.ttf";
        public static readonly string CairoMedium = $"{CairoNamespace}.Cairo-Medium.ttf";
        public static readonly string CairoRegular = $"{CairoNamespace}.Cairo-Regular.ttf";
        public static readonly string CairoSemiBold = $"{CairoNamespace}.Cairo-SemiBold.ttf";
        #endregion

        #region "Comic Neue Font Family"
        private const string ComicNeueNamespace = BaseNamespace + ".Comic_Neue";
        public static readonly string ComicNeueBold = $"{ComicNeueNamespace}.ComicNeue-Bold.ttf";
        public static readonly string ComicNeueBoldItalic = $"{ComicNeueNamespace}.ComicNeue-BoldItalic.ttf";
        public static readonly string ComicNeueItalic = $"{ComicNeueNamespace}.ComicNeue-Italic.ttf";
        public static readonly string ComicNeueLight = $"{ComicNeueNamespace}.ComicNeue-Light.ttf";
        public static readonly string ComicNeueLightItalic = $"{ComicNeueNamespace}.ComicNeue-LightItalic.ttf";
        public static readonly string ComicNeueRegular = $"{ComicNeueNamespace}.ComicNeue-Regular.ttf";
        #endregion

        #region "Roboto Font Family"
        private const string RobotoNamespace = BaseNamespace + ".Roboto";
        public static readonly string RobotoBlack = $"{RobotoNamespace}.Roboto-Black.ttf";
        public static readonly string RobotoBlackItalic = $"{RobotoNamespace}.Roboto-BlackItalic.ttf";
        public static readonly string RobotoBold = $"{RobotoNamespace}.Roboto-Bold.ttf";
        public static readonly string RobotoBoldItalic = $"{RobotoNamespace}.Roboto-BoldItalic.ttf";
        public static readonly string RobotoExtraBold = $"{RobotoNamespace}.Roboto-ExtraBold.ttf";
        public static readonly string RobotoExtraBoldItalic = $"{RobotoNamespace}.Roboto-ExtraBoldItalic.ttf";
        public static readonly string RobotoExtraLight = $"{RobotoNamespace}.Roboto-ExtraLight.ttf";
        public static readonly string RobotoExtraLightItalic = $"{RobotoNamespace}.Roboto-ExtraLightItalic.ttf";
        public static readonly string RobotoItalic = $"{RobotoNamespace}.Roboto-Italic.ttf";
        public static readonly string RobotoLight = $"{RobotoNamespace}.Roboto-Light.ttf";
        public static readonly string RobotoLightItalic = $"{RobotoNamespace}.Roboto-LightItalic.ttf";
        public static readonly string RobotoMedium = $"{RobotoNamespace}.Roboto-Medium.ttf";
        public static readonly string RobotoMediumItalic = $"{RobotoNamespace}.Roboto-MediumItalic.ttf";
        public static readonly string RobotoRegular = $"{RobotoNamespace}.Roboto-Regular.ttf";
        public static readonly string RobotoSemiBold = $"{RobotoNamespace}.Roboto-SemiBold.ttf";
        public static readonly string RobotoSemiBoldItalic = $"{RobotoNamespace}.Roboto-SemiBoldItalic.ttf";
        public static readonly string RobotoThin = $"{RobotoNamespace}.Roboto-Thin.ttf";
        public static readonly string RobotoThinItalic = $"{RobotoNamespace}.Roboto-ThinItalic.ttf";
        #endregion

        #region "Roboto Condensed Font Family"
        public static readonly string RobotoCondensedBlack = $"{RobotoNamespace}.Roboto_Condensed-Black.ttf";
        public static readonly string RobotoCondensedBlackItalic = $"{RobotoNamespace}.Roboto_Condensed-BlackItalic.ttf";
        public static readonly string RobotoCondensedBold = $"{RobotoNamespace}.Roboto_Condensed-Bold.ttf";
        public static readonly string RobotoCondensedBoldItalic = $"{RobotoNamespace}.Roboto_Condensed-BoldItalic.ttf";
        public static readonly string RobotoCondensedExtraBold = $"{RobotoNamespace}.Roboto_Condensed-ExtraBold.ttf";
        public static readonly string RobotoCondensedExtraBoldItalic = $"{RobotoNamespace}.Roboto_Condensed-ExtraBoldItalic.ttf";
        public static readonly string RobotoCondensedExtraLight = $"{RobotoNamespace}.Roboto_Condensed-ExtraLight.ttf";
        public static readonly string RobotoCondensedExtraLightItalic = $"{RobotoNamespace}.Roboto_Condensed-ExtraLightItalic.ttf";
        public static readonly string RobotoCondensedItalic = $"{RobotoNamespace}.Roboto_Condensed-Italic.ttf";
        public static readonly string RobotoCondensedLight = $"{RobotoNamespace}.Roboto_Condensed-Light.ttf";
        public static readonly string RobotoCondensedLightItalic = $"{RobotoNamespace}.Roboto_Condensed-LightItalic.ttf";
        public static readonly string RobotoCondensedMedium = $"{RobotoNamespace}.Roboto_Condensed-Medium.ttf";
        public static readonly string RobotoCondensedMediumItalic = $"{RobotoNamespace}.Roboto_Condensed-MediumItalic.ttf";
        public static readonly string RobotoCondensedRegular = $"{RobotoNamespace}.Roboto_Condensed-Regular.ttf";
        public static readonly string RobotoCondensedSemiBold = $"{RobotoNamespace}.Roboto_Condensed-SemiBold.ttf";
        public static readonly string RobotoCondensedSemiBoldItalic = $"{RobotoNamespace}.Roboto_Condensed-SemiBoldItalic.ttf";
        public static readonly string RobotoCondensedThin = $"{RobotoNamespace}.Roboto_Condensed-Thin.ttf";
        public static readonly string RobotoCondensedThinItalic = $"{RobotoNamespace}.Roboto_Condensed-ThinItalic.ttf";
        #endregion

        #region "Roboto Semi Condensed Font Family"
        public static readonly string RobotoSemiCondensedBlack = $"{RobotoNamespace}.Roboto_SemiCondensed-Black.ttf";
        public static readonly string RobotoSemiCondensedBlackItalic = $"{RobotoNamespace}.Roboto_SemiCondensed-BlackItalic.ttf";
        public static readonly string RobotoSemiCondensedBold = $"{RobotoNamespace}.Roboto_SemiCondensed-Bold.ttf";
        public static readonly string RobotoSemiCondensedBoldItalic = $"{RobotoNamespace}.Roboto_SemiCondensed-BoldItalic.ttf";
        public static readonly string RobotoSemiCondensedExtraBold = $"{RobotoNamespace}.Roboto_SemiCondensed-ExtraBold.ttf";
        public static readonly string RobotoSemiCondensedExtraBoldItalic = $"{RobotoNamespace}.Roboto_SemiCondensed-ExtraBoldItalic.ttf";
        public static readonly string RobotoSemiCondensedExtraLight = $"{RobotoNamespace}.Roboto_SemiCondensed-ExtraLight.ttf";
        public static readonly string RobotoSemiCondensedExtraLightItalic = $"{RobotoNamespace}.Roboto_SemiCondensed-ExtraLightItalic.ttf";
        public static readonly string RobotoSemiCondensedItalic = $"{RobotoNamespace}.Roboto_SemiCondensed-Italic.ttf";
        public static readonly string RobotoSemiCondensedLight = $"{RobotoNamespace}.Roboto_SemiCondensed-Light.ttf";
        public static readonly string RobotoSemiCondensedLightItalic = $"{RobotoNamespace}.Roboto_SemiCondensed-LightItalic.ttf";
        public static readonly string RobotoSemiCondensedMedium = $"{RobotoNamespace}.Roboto_SemiCondensed-Medium.ttf";
        public static readonly string RobotoSemiCondensedMediumItalic = $"{RobotoNamespace}.Roboto_SemiCondensed-MediumItalic.ttf";
        public static readonly string RobotoSemiCondensedRegular = $"{RobotoNamespace}.Roboto_SemiCondensed-Regular.ttf";
        public static readonly string RobotoSemiCondensedSemiBold = $"{RobotoNamespace}.Roboto_SemiCondensed-SemiBold.ttf";
        public static readonly string RobotoSemiCondensedSemiBoldItalic = $"{RobotoNamespace}.Roboto_SemiCondensed-SemiBoldItalic.ttf";
        public static readonly string RobotoSemiCondensedThin = $"{RobotoNamespace}.Roboto_SemiCondensed-Thin.ttf";
        public static readonly string RobotoSemiCondensedThinItalic = $"{RobotoNamespace}.Roboto_SemiCondensed-ThinItalic.ttf";
        #endregion

        #region "Lato Font Family"
        private const string LatoNamespace = BaseNamespace + ".Lato";
        public static readonly string LatoThin = $"{LatoNamespace}.Lato-Thin.ttf";
        public static readonly string LatoThinItalic = $"{LatoNamespace}.Lato-ThinItalic.ttf";
        public static readonly string LatoLight = $"{LatoNamespace}.Lato-Light.ttf";
        public static readonly string LatoLightItalic = $"{LatoNamespace}.Lato-LightItalic.ttf";
        public static readonly string LatoRegular = $"{LatoNamespace}.Lato-Regular.ttf";
        public static readonly string LatoItalic = $"{LatoNamespace}.Lato-Italic.ttf";
        public static readonly string LatoBold = $"{LatoNamespace}.Lato-Bold.ttf";
        public static readonly string LatoBoldItalic = $"{LatoNamespace}.Lato-BoldItalic.ttf";
        public static readonly string LatoBlack = $"{LatoNamespace}.Lato-Black.ttf";
        public static readonly string LatoBlackItalic = $"{LatoNamespace}.Lato-BlackItalic.ttf";
        #endregion

        #region "Ubuntu Font Family"
        private const string UbuntuNamespace = BaseNamespace + ".Ubuntu";
        public static readonly string UbuntuLight = $"{UbuntuNamespace}.Ubuntu-Light.ttf";
        public static readonly string UbuntuLightItalic = $"{UbuntuNamespace}.Ubuntu-LightItalic.ttf";
        public static readonly string UbuntuRegular = $"{UbuntuNamespace}.Ubuntu-Regular.ttf";
        public static readonly string UbuntuItalic = $"{UbuntuNamespace}.Ubuntu-Italic.ttf";
        public static readonly string UbuntuMedium = $"{UbuntuNamespace}.Ubuntu-Medium.ttf";
        public static readonly string UbuntuMediumItalic = $"{UbuntuNamespace}.Ubuntu-MediumItalic.ttf";
        public static readonly string UbuntuBold = $"{UbuntuNamespace}.Ubuntu-Bold.ttf";
        public static readonly string UbuntuBoldItalic = $"{UbuntuNamespace}.Ubuntu-BoldItalic.ttf";
        #endregion

        #region "Noto Sans Font Family"
        private const string NotoSansNamespace = BaseNamespace + ".Noto_Sans";
        public static readonly string NotoSansThin = $"{NotoSansNamespace}.NotoSans-Thin.ttf";
        public static readonly string NotoSansThinItalic = $"{NotoSansNamespace}.NotoSans-ThinItalic.ttf";
        public static readonly string NotoSansExtraLight = $"{NotoSansNamespace}.NotoSans-ExtraLight.ttf";
        public static readonly string NotoSansExtraLightItalic = $"{NotoSansNamespace}.NotoSans-ExtraLightItalic.ttf";
        public static readonly string NotoSansLight = $"{NotoSansNamespace}.NotoSans-Light.ttf";
        public static readonly string NotoSansLightItalic = $"{NotoSansNamespace}.NotoSans-LightItalic.ttf";
        public static readonly string NotoSansRegular = $"{NotoSansNamespace}.NotoSans-Regular.ttf";
        public static readonly string NotoSansItalic = $"{NotoSansNamespace}.NotoSans-Italic.ttf";
        public static readonly string NotoSansMedium = $"{NotoSansNamespace}.NotoSans-Medium.ttf";
        public static readonly string NotoSansMediumItalic = $"{NotoSansNamespace}.NotoSans-MediumItalic.ttf";
        public static readonly string NotoSansSemiBold = $"{NotoSansNamespace}.NotoSans-SemiBold.ttf";
        public static readonly string NotoSansSemiBoldItalic = $"{NotoSansNamespace}.NotoSans-SemiBoldItalic.ttf";
        public static readonly string NotoSansBold = $"{NotoSansNamespace}.NotoSans-Bold.ttf";
        public static readonly string NotoSansBoldItalic = $"{NotoSansNamespace}.NotoSans-BoldItalic.ttf";
        public static readonly string NotoSansExtraBold = $"{NotoSansNamespace}.NotoSans-ExtraBold.ttf";
        public static readonly string NotoSansExtraBoldItalic = $"{NotoSansNamespace}.NotoSans-ExtraBoldItalic.ttf";
        public static readonly string NotoSansBlack = $"{NotoSansNamespace}.NotoSans-Black.ttf";
        public static readonly string NotoSansBlackItalic = $"{NotoSansNamespace}.NotoSans-BlackItalic.ttf";
        #endregion

        #region "Open Sans Font Family"
        private const string OpenSansNamespace = BaseNamespace + ".Open_Sans";
        public static readonly string OpenSansLight = $"{OpenSansNamespace}.OpenSans-Light.ttf";
        public static readonly string OpenSansLightItalic = $"{OpenSansNamespace}.OpenSans-LightItalic.ttf";
        public static readonly string OpenSansRegular = $"{OpenSansNamespace}.OpenSans-Regular.ttf";
        public static readonly string OpenSansItalic = $"{OpenSansNamespace}.OpenSans-Italic.ttf";
        public static readonly string OpenSansMedium = $"{OpenSansNamespace}.OpenSans-Medium.ttf";
        public static readonly string OpenSansMediumItalic = $"{OpenSansNamespace}.OpenSans-MediumItalic.ttf";
        public static readonly string OpenSansSemiBold = $"{OpenSansNamespace}.OpenSans-SemiBold.ttf";
        public static readonly string OpenSansSemiBoldItalic = $"{OpenSansNamespace}.OpenSans-SemiBoldItalic.ttf";
        public static readonly string OpenSansBold = $"{OpenSansNamespace}.OpenSans-Bold.ttf";
        public static readonly string OpenSansBoldItalic = $"{OpenSansNamespace}.OpenSans-BoldItalic.ttf";
        public static readonly string OpenSansExtraBold = $"{OpenSansNamespace}.OpenSans-ExtraBold.ttf";
        public static readonly string OpenSansExtraBoldItalic = $"{OpenSansNamespace}.OpenSans-ExtraBoldItalic.ttf";
        #endregion

        #region "Oswald Font Family"
        private const string OswaldNamespace = BaseNamespace + ".Oswald";
        public static readonly string OswaldExtraLight = $"{OswaldNamespace}.Oswald-ExtraLight.ttf";
        public static readonly string OswaldLight = $"{OswaldNamespace}.Oswald-Light.ttf";
        public static readonly string OswaldRegular = $"{OswaldNamespace}.Oswald-Regular.ttf";
        public static readonly string OswaldMedium = $"{OswaldNamespace}.Oswald-Medium.ttf";
        public static readonly string OswaldSemiBold = $"{OswaldNamespace}.Oswald-SemiBold.ttf";
        public static readonly string OswaldBold = $"{OswaldNamespace}.Oswald-Bold.ttf";
        #endregion

        #region "Poppins Font Family"
        private const string PoppinsNamespace = BaseNamespace + ".Poppins";
        public static readonly string PoppinsThin = $"{PoppinsNamespace}.Poppins-Thin.ttf";
        public static readonly string PoppinsThinItalic = $"{PoppinsNamespace}.Poppins-ThinItalic.ttf";
        public static readonly string PoppinsExtraLight = $"{PoppinsNamespace}.Poppins-ExtraLight.ttf";
        public static readonly string PoppinsExtraLightItalic = $"{PoppinsNamespace}.Poppins-ExtraLightItalic.ttf";
        public static readonly string PoppinsLight = $"{PoppinsNamespace}.Poppins-Light.ttf";
        public static readonly string PoppinsLightItalic = $"{PoppinsNamespace}.Poppins-LightItalic.ttf";
        public static readonly string PoppinsRegular = $"{PoppinsNamespace}.Poppins-Regular.ttf";
        public static readonly string PoppinsItalic = $"{PoppinsNamespace}.Poppins-Italic.ttf";
        public static readonly string PoppinsMedium = $"{PoppinsNamespace}.Poppins-Medium.ttf";
        public static readonly string PoppinsMediumItalic = $"{PoppinsNamespace}.Poppins-MediumItalic.ttf";
        public static readonly string PoppinsSemiBold = $"{PoppinsNamespace}.Poppins-SemiBold.ttf";
        public static readonly string PoppinsSemiBoldItalic = $"{PoppinsNamespace}.Poppins-SemiBoldItalic.ttf";
        public static readonly string PoppinsBold = $"{PoppinsNamespace}.Poppins-Bold.ttf";
        public static readonly string PoppinsBoldItalic = $"{PoppinsNamespace}.Poppins-BoldItalic.ttf";
        public static readonly string PoppinsExtraBold = $"{PoppinsNamespace}.Poppins-ExtraBold.ttf";
        public static readonly string PoppinsExtraBoldItalic = $"{PoppinsNamespace}.Poppins-ExtraBoldItalic.ttf";
        public static readonly string PoppinsBlack = $"{PoppinsNamespace}.Poppins-Black.ttf";
        public static readonly string PoppinsBlackItalic = $"{PoppinsNamespace}.Poppins-BlackItalic.ttf";
        #endregion

        #region "Individual Font Files"
        public static readonly string CaprasimoRegular = $"{BaseNamespace}.Caprasimo-Regular.ttf";
        public static readonly string Consolas = $"{BaseNamespace}.consolas.ttf";
        #endregion

        /// <summary>
        /// Font family groups for easy access to complete font families.
        /// </summary>
        public static class Families
        {
            /// <summary>
            /// Cairo font family paths grouped by weight/style.
            /// </summary>
            public static class Cairo
            {
                public static readonly string Black = CairoBlack;
                public static readonly string Bold = CairoBold;
                public static readonly string ExtraBold = CairoExtraBold;
                public static readonly string ExtraLight = CairoExtraLight;
                public static readonly string Light = CairoLight;
                public static readonly string Medium = CairoMedium;
                public static readonly string Regular = CairoRegular;
                public static readonly string SemiBold = CairoSemiBold;

                public static Dictionary<string, string> GetAllWeights()
                {
                    return new Dictionary<string, string>
                    {
                        { "ExtraLight", ExtraLight },
                        { "Light", Light },
                        { "Regular", Regular },
                        { "Medium", Medium },
                        { "SemiBold", SemiBold },
                        { "Bold", Bold },
                        { "ExtraBold", ExtraBold },
                        { "Black", Black }
                    };
                }
            }

            /// <summary>
            /// Comic Neue font family paths grouped by weight/style.
            /// </summary>
            public static class ComicNeue
            {
                public static readonly string Light = ComicNeueLight;
                public static readonly string LightItalic = ComicNeueLightItalic;
                public static readonly string Regular = ComicNeueRegular;
                public static readonly string Italic = ComicNeueItalic;
                public static readonly string Bold = ComicNeueBold;
                public static readonly string BoldItalic = ComicNeueBoldItalic;

                public static Dictionary<string, string> GetAllStyles()
                {
                    return new Dictionary<string, string>
                    {
                        { "Light", Light },
                        { "LightItalic", LightItalic },
                        { "Regular", Regular },
                        { "Italic", Italic },
                        { "Bold", Bold },
                        { "BoldItalic", BoldItalic }
                    };
                }
            }

            /// <summary>
            /// Roboto font family paths grouped by weight/style.
            /// </summary>
            public static class Roboto
            {
                public static readonly string Thin = RobotoThin;
                public static readonly string ThinItalic = RobotoThinItalic;
                public static readonly string ExtraLight = RobotoExtraLight;
                public static readonly string ExtraLightItalic = RobotoExtraLightItalic;
                public static readonly string Light = RobotoLight;
                public static readonly string LightItalic = RobotoLightItalic;
                public static readonly string Regular = RobotoRegular;
                public static readonly string Italic = RobotoItalic;
                public static readonly string Medium = RobotoMedium;
                public static readonly string MediumItalic = RobotoMediumItalic;
                public static readonly string SemiBold = RobotoSemiBold;
                public static readonly string SemiBoldItalic = RobotoSemiBoldItalic;
                public static readonly string Bold = RobotoBold;
                public static readonly string BoldItalic = RobotoBoldItalic;
                public static readonly string ExtraBold = RobotoExtraBold;
                public static readonly string ExtraBoldItalic = RobotoExtraBoldItalic;
                public static readonly string Black = RobotoBlack;
                public static readonly string BlackItalic = RobotoBlackItalic;

                public static Dictionary<string, string> GetAllStyles()
                {
                    return new Dictionary<string, string>
                    {
                        { "Thin", Thin },
                        { "ThinItalic", ThinItalic },
                        { "ExtraLight", ExtraLight },
                        { "ExtraLightItalic", ExtraLightItalic },
                        { "Light", Light },
                        { "LightItalic", LightItalic },
                        { "Regular", Regular },
                        { "Italic", Italic },
                        { "Medium", Medium },
                        { "MediumItalic", MediumItalic },
                        { "SemiBold", SemiBold },
                        { "SemiBoldItalic", SemiBoldItalic },
                        { "Bold", Bold },
                        { "BoldItalic", BoldItalic },
                        { "ExtraBold", ExtraBold },
                        { "ExtraBoldItalic", ExtraBoldItalic },
                        { "Black", Black },
                        { "BlackItalic", BlackItalic }
                    };
                }
            }

            /// <summary>
            /// Lato font family paths grouped by weight/style.
            /// </summary>
            public static class Lato
            {
                public static readonly string Thin = LatoThin;
                public static readonly string ThinItalic = LatoThinItalic;
                public static readonly string Light = LatoLight;
                public static readonly string LightItalic = LatoLightItalic;
                public static readonly string Regular = LatoRegular;
                public static readonly string Italic = LatoItalic;
                public static readonly string Bold = LatoBold;
                public static readonly string BoldItalic = LatoBoldItalic;
                public static readonly string Black = LatoBlack;
                public static readonly string BlackItalic = LatoBlackItalic;

                public static Dictionary<string, string> GetAllStyles()
                {
                    return new Dictionary<string, string>
                    {
                        { "Thin", Thin },
                        { "ThinItalic", ThinItalic },
                        { "Light", Light },
                        { "LightItalic", LightItalic },
                        { "Regular", Regular },
                        { "Italic", Italic },
                        { "Bold", Bold },
                        { "BoldItalic", BoldItalic },
                        { "Black", Black },
                        { "BlackItalic", BlackItalic }
                    };
                }
            }

            /// <summary>
            /// Ubuntu font family paths grouped by weight/style.
            /// </summary>
            public static class Ubuntu
            {
                public static readonly string Light = UbuntuLight;
                public static readonly string LightItalic = UbuntuLightItalic;
                public static readonly string Regular = UbuntuRegular;
                public static readonly string Italic = UbuntuItalic;
                public static readonly string Medium = UbuntuMedium;
                public static readonly string MediumItalic = UbuntuMediumItalic;
                public static readonly string Bold = UbuntuBold;
                public static readonly string BoldItalic = UbuntuBoldItalic;

                public static Dictionary<string, string> GetAllStyles()
                {
                    return new Dictionary<string, string>
                    {
                        { "Light", Light },
                        { "LightItalic", LightItalic },
                        { "Regular", Regular },
                        { "Italic", Italic },
                        { "Medium", Medium },
                        { "MediumItalic", MediumItalic },
                        { "Bold", Bold },
                        { "BoldItalic", BoldItalic }
                    };
                }
            }

            /// <summary>
            /// Noto Sans font family paths grouped by weight/style.
            /// </summary>
            public static class NotoSans
            {
                public static readonly string Thin = NotoSansThin;
                public static readonly string ThinItalic = NotoSansThinItalic;
                public static readonly string ExtraLight = NotoSansExtraLight;
                public static readonly string ExtraLightItalic = NotoSansExtraLightItalic;
                public static readonly string Light = NotoSansLight;
                public static readonly string LightItalic = NotoSansLightItalic;
                public static readonly string Regular = NotoSansRegular;
                public static readonly string Italic = NotoSansItalic;
                public static readonly string Medium = NotoSansMedium;
                public static readonly string MediumItalic = NotoSansMediumItalic;
                public static readonly string SemiBold = NotoSansSemiBold;
                public static readonly string SemiBoldItalic = NotoSansSemiBoldItalic;
                public static readonly string Bold = NotoSansBold;
                public static readonly string BoldItalic = NotoSansBoldItalic;
                public static readonly string ExtraBold = NotoSansExtraBold;
                public static readonly string ExtraBoldItalic = NotoSansExtraBoldItalic;
                public static readonly string Black = NotoSansBlack;
                public static readonly string BlackItalic = NotoSansBlackItalic;

                public static Dictionary<string, string> GetAllStyles()
                {
                    return new Dictionary<string, string>
                    {
                        { "Thin", Thin },
                        { "ThinItalic", ThinItalic },
                        { "ExtraLight", ExtraLight },
                        { "ExtraLightItalic", ExtraLightItalic },
                        { "Light", Light },
                        { "LightItalic", LightItalic },
                        { "Regular", Regular },
                        { "Italic", Italic },
                        { "Medium", Medium },
                        { "MediumItalic", MediumItalic },
                        { "SemiBold", SemiBold },
                        { "SemiBoldItalic", SemiBoldItalic },
                        { "Bold", Bold },
                        { "BoldItalic", BoldItalic },
                        { "ExtraBold", ExtraBold },
                        { "ExtraBoldItalic", ExtraBoldItalic },
                        { "Black", Black },
                        { "BlackItalic", BlackItalic }
                    };
                }
            }

            /// <summary>
            /// Open Sans font family paths grouped by weight/style.
            /// </summary>
            public static class OpenSans
            {
                public static readonly string Light = OpenSansLight;
                public static readonly string LightItalic = OpenSansLightItalic;
                public static readonly string Regular = OpenSansRegular;
                public static readonly string Italic = OpenSansItalic;
                public static readonly string Medium = OpenSansMedium;
                public static readonly string MediumItalic = OpenSansMediumItalic;
                public static readonly string SemiBold = OpenSansSemiBold;
                public static readonly string SemiBoldItalic = OpenSansSemiBoldItalic;
                public static readonly string Bold = OpenSansBold;
                public static readonly string BoldItalic = OpenSansBoldItalic;
                public static readonly string ExtraBold = OpenSansExtraBold;
                public static readonly string ExtraBoldItalic = OpenSansExtraBoldItalic;

                public static Dictionary<string, string> GetAllStyles()
                {
                    return new Dictionary<string, string>
                    {
                        { "Light", Light },
                        { "LightItalic", LightItalic },
                        { "Regular", Regular },
                        { "Italic", Italic },
                        { "Medium", Medium },
                        { "MediumItalic", MediumItalic },
                        { "SemiBold", SemiBold },
                        { "SemiBoldItalic", SemiBoldItalic },
                        { "Bold", Bold },
                        { "BoldItalic", BoldItalic },
                        { "ExtraBold", ExtraBold },
                        { "ExtraBoldItalic", ExtraBoldItalic }
                    };
                }
            }

            /// <summary>
            /// Oswald font family paths grouped by weight/style.
            /// </summary>
            public static class Oswald
            {
                public static readonly string ExtraLight = OswaldExtraLight;
                public static readonly string Light = OswaldLight;
                public static readonly string Regular = OswaldRegular;
                public static readonly string Medium = OswaldMedium;
                public static readonly string SemiBold = OswaldSemiBold;
                public static readonly string Bold = OswaldBold;

                public static Dictionary<string, string> GetAllStyles()
                {
                    return new Dictionary<string, string>
                    {
                        { "ExtraLight", ExtraLight },
                        { "Light", Light },
                        { "Regular", Regular },
                        { "Medium", Medium },
                        { "SemiBold", SemiBold },
                        { "Bold", Bold }
                    };
                }
            }

            /// <summary>
            /// Poppins font family paths grouped by weight/style.
            /// </summary>
            public static class Poppins
            {
                public static readonly string Thin = PoppinsThin;
                public static readonly string ThinItalic = PoppinsThinItalic;
                public static readonly string ExtraLight = PoppinsExtraLight;
                public static readonly string ExtraLightItalic = PoppinsExtraLightItalic;
                public static readonly string Light = PoppinsLight;
                public static readonly string LightItalic = PoppinsLightItalic;
                public static readonly string Regular = PoppinsRegular;
                public static readonly string Italic = PoppinsItalic;
                public static readonly string Medium = PoppinsMedium;
                public static readonly string MediumItalic = PoppinsMediumItalic;
                public static readonly string SemiBold = PoppinsSemiBold;
                public static readonly string SemiBoldItalic = PoppinsSemiBoldItalic;
                public static readonly string Bold = PoppinsBold;
                public static readonly string BoldItalic = PoppinsBoldItalic;
                public static readonly string ExtraBold = PoppinsExtraBold;
                public static readonly string ExtraBoldItalic = PoppinsExtraBoldItalic;
                public static readonly string Black = PoppinsBlack;
                public static readonly string BlackItalic = PoppinsBlackItalic;

                public static Dictionary<string, string> GetAllStyles()
                {
                    return new Dictionary<string, string>
                    {
                        { "Thin", Thin },
                        { "ThinItalic", ThinItalic },
                        { "ExtraLight", ExtraLight },
                        { "ExtraLightItalic", ExtraLightItalic },
                        { "Light", Light },
                        { "LightItalic", LightItalic },
                        { "Regular", Regular },
                        { "Italic", Italic },
                        { "Medium", Medium },
                        { "MediumItalic", MediumItalic },
                        { "SemiBold", SemiBold },
                        { "SemiBoldItalic", SemiBoldItalic },
                        { "Bold", Bold },
                        { "BoldItalic", BoldItalic },
                        { "ExtraBold", ExtraBold },
                        { "ExtraBoldItalic", ExtraBoldItalic },
                        { "Black", Black },
                        { "BlackItalic", BlackItalic }
                    };
                }
            }

            /// <summary>
            /// Individual fonts that are not part of a family.
            /// </summary>
            public static class Individual
            {
                public static readonly string Caprasimo = CaprasimoRegular;
                public static readonly string Consolas = BeepFontPaths.Consolas;
            }
        }

        /// <summary>
        /// Gets all font resource paths as a dictionary for easy enumeration.
        /// </summary>
        public static Dictionary<string, string> GetAllPaths()
        {
            var paths = new Dictionary<string, string>();
            var type = typeof(BeepFontPaths);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
            {
                if (field.FieldType == typeof(string) && field.IsLiteral == false && field.IsInitOnly)
                {
                    var value = field.GetValue(null) as string;
                    if (!string.IsNullOrEmpty(value) && value.EndsWith(".ttf"))
                    {
                        paths[field.Name] = value;
                    }
                }
            }

            return paths;
        }

        /// <summary>
        /// Gets all font resource paths grouped by font family.
        /// </summary>
        public static Dictionary<string, Dictionary<string, string>> GetAllFamilies()
        {
            return new Dictionary<string, Dictionary<string, string>>
            {
                { "Cairo", Families.Cairo.GetAllWeights() },
                { "ComicNeue", Families.ComicNeue.GetAllStyles() },
                { "Roboto", Families.Roboto.GetAllStyles() },
                { "Lato", Families.Lato.GetAllStyles() },
                { "Ubuntu", Families.Ubuntu.GetAllStyles() },
                { "NotoSans", Families.NotoSans.GetAllStyles() },
                { "OpenSans", Families.OpenSans.GetAllStyles() },
                { "Oswald", Families.Oswald.GetAllStyles() },
                { "Poppins", Families.Poppins.GetAllStyles() },
                { "Individual", new Dictionary<string, string>
                    {
                        { "Caprasimo", Families.Individual.Caprasimo },
                        { "Consolas", Families.Individual.Consolas }
                    }
                }
            };
        }

        /// <summary>
        /// Checks if a font resource path exists in the assembly.
        /// </summary>
        /// <param name="resourcePath">The full resource path</param>
        /// <returns>True if the resource exists</returns>
        public static bool ResourceExists(string resourcePath)
        {
            var resourceNames = ResourceAssembly.GetManifestResourceNames();
            return resourceNames.Contains(resourcePath);
        }

        /// <summary>
        /// Gets all available font resource names from the assembly.
        /// </summary>
        /// <returns>Array of resource names</returns>
        public static string[] GetAvailableFontResources()
        {
            return ResourceAssembly.GetManifestResourceNames()
                .Where(name => name.StartsWith(BaseNamespace) && name.EndsWith(".ttf"))
                .ToArray();
        }

        /// <summary>
        /// Helper method to get the full file system path (useful for development/debugging).
        /// This assumes the standard project structure.
        /// </summary>
        /// <param name="fontFileName">Just the font filename (e.g., "Roboto-Regular.ttf")</param>
        /// <param name="subfolder">Optional subfolder (e.g., "Roboto", "Cairo")</param>
        /// <returns>Full file system path</returns>
        public static string GetFileSystemPath(string fontFileName, string subfolder = null)
        {
            string baseDirectory = AppContext.BaseDirectory;
            if (string.IsNullOrEmpty(subfolder))
            {
                return Path.Combine(baseDirectory, "..", "..", "..", "Fonts", fontFileName);
            }
            else
            {
                return Path.Combine(baseDirectory, "..", "..", "..", "Fonts", subfolder, fontFileName);
            }
        }

        /// <summary>
        /// Gets the best matching font path for a given font family and style.
        /// </summary>
        /// <param name="familyName">Font family name (e.g., "Roboto", "Cairo")</param>
        /// <param name="style">Font style (e.g., "Bold", "Italic", "Regular")</param>
        /// <returns>Font resource path if found, null otherwise</returns>
        public static string GetFontPath(string familyName, string style = "Regular")
        {
            var families = GetAllFamilies();

            if (families.TryGetValue(familyName, out var familyFonts))
            {
                if (familyFonts.TryGetValue(style, out var fontPath))
                {
                    return fontPath;
                }

                // Fallback to Regular if requested style not found
                if (style != "Regular" && familyFonts.TryGetValue("Regular", out var regularPath))
                {
                    return regularPath;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all available font families.
        /// </summary>
        /// <returns>List of font family names</returns>
        public static List<string> GetFontFamilyNames()
        {
            return GetAllFamilies().Keys.ToList();
        }

        /// <summary>
        /// Gets all available styles for a specific font family.
        /// </summary>
        /// <param name="familyName">Font family name</param>
        /// <returns>List of available styles for the family</returns>
        public static List<string> GetFontFamilyStyles(string familyName)
        {
            var families = GetAllFamilies();
            if (families.TryGetValue(familyName, out var familyFonts))
            {
                return familyFonts.Keys.ToList();
            }
            return new List<string>();
        }
    }

    /// <summary>
    /// Extension methods for easier use of BeepFontPaths with font management.
    /// </summary>
    public static class BeepFontPathsExtensions
    {
        /// <summary>
        /// Loads a font from embedded resources and adds it to the FontListHelper.
        /// </summary>
        /// <param name="fontResourcePath">The font resource path from BeepFontPaths</param>
        /// <returns>True if the font was loaded successfully</returns>
        public static bool LoadFontFromEmbeddedResource(string fontResourcePath)
        {
            if (string.IsNullOrEmpty(fontResourcePath))
                throw new ArgumentNullException(nameof(fontResourcePath));

            try
            {
                // Use the existing FontListHelper to load embedded resources
                var fontNamespaces = new[] { "TheTechIdea.Beep.Winform.Controls.Fonts" };
                var loadedFonts = FontListHelper.GetFontResourcesFromEmbedded(fontNamespaces);

                return loadedFonts.Any(f => f.Path == fontResourcePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load font resource: {fontResourcePath}", ex);
            }
        }

        /// <summary>
        /// Creates a Font object from a BeepFontPaths resource.
        /// </summary>
        /// <param name="fontResourcePath">The font resource path from BeepFontPaths</param>
        /// <param name="size">Font size</param>
        /// <param name="style">Font style</param>
        /// <returns>Font object or null if failed</returns>
        public static Font CreateFontFromResource(string fontResourcePath, float size, FontStyle style = FontStyle.Regular)
        {
            if (string.IsNullOrEmpty(fontResourcePath))
                return null;

            try
            {
                // First ensure the font is loaded
                LoadFontFromEmbeddedResource(fontResourcePath);

                // Extract the font name from the path (this is a simplification)
                string fontName = ExtractFontNameFromPath(fontResourcePath);

                // Use FontListHelper to get the font
                return FontListHelper.GetFont(fontName, size, style);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Helper method to extract font name from resource path.
        /// This is a basic implementation - you might need to adjust based on your specific requirements.
        /// </summary>
        /// <param name="fontResourcePath">Font resource path</param>
        /// <returns>Extracted font name</returns>
        private static string ExtractFontNameFromPath(string fontResourcePath)
        {
            if (string.IsNullOrEmpty(fontResourcePath))
                return string.Empty;

            // Extract filename without extension
            string fileName = Path.GetFileNameWithoutExtension(fontResourcePath);

            // Handle common naming conventions
            if (fileName.StartsWith("Cairo-"))
                return "Cairo";
            if (fileName.StartsWith("Roboto"))
                return "Roboto";
            if (fileName.StartsWith("ComicNeue"))
                return "Comic Neue";
            if (fileName.StartsWith("Lato-"))
                return "Lato";
            if (fileName.StartsWith("Ubuntu-"))
                return "Ubuntu";
            if (fileName.StartsWith("NotoSans-"))
                return "Noto Sans";
            if (fileName.StartsWith("OpenSans-"))
                return "Open Sans";
            if (fileName.StartsWith("Oswald-"))
                return "Oswald";
            if (fileName.StartsWith("Poppins-"))
                return "Poppins";
            if (fileName.Contains("Caprasimo"))
                return "Caprasimo";
            if (fileName.Contains("consolas"))
                return "Consolas";

            return fileName;
        }

        /// <summary>
        /// Gets a font using the BeepFontPaths system with fallback.
        /// </summary>
        /// <param name="familyName">Font family name</param>
        /// <param name="style">Font style</param>
        /// <param name="size">Font size</param>
        /// <param name="fallbackFontName">Fallback font name if primary not found</param>
        /// <returns>Font object</returns>
        public static Font GetBeepFont(string familyName, string style, float size, string fallbackFontName = "Arial")
        {
            string fontPath = BeepFontPaths.GetFontPath(familyName, style);
            if (!string.IsNullOrEmpty(fontPath))
            {
                var font = CreateFontFromResource(fontPath, size);
                if (font != null)
                    return font;
            }

            // Fallback to system font
            return FontListHelper.GetFontWithFallback(familyName, fallbackFontName, size);
        }
    }
}
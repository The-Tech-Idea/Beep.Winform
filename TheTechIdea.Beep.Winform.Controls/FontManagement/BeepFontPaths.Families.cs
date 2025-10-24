namespace TheTechIdea.Beep.Winform.Controls.FontManagement
{
    /// <summary>
    /// Font Families partial class containing the nested Families class with organized font collections.
    /// </summary>
    public static partial class BeepFontPaths
    {
        /// <summary>
        /// Font family groups for easy access to complete font families.
        /// </summary>
        public static class Families
        {
            #region "Web Fonts"
            
            /// <summary>
            /// Cairo font family paths grouped by weight/Style.
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
            /// Roboto font family paths grouped by weight/Style.
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
            /// Roboto Condensed font family paths grouped by weight/Style.
            /// </summary>
            public static class RobotoCondensed
            {
                public static readonly string Thin = RobotoCondensedThin;
                public static readonly string ThinItalic = RobotoCondensedThinItalic;
                public static readonly string ExtraLight = RobotoCondensedExtraLight;
                public static readonly string ExtraLightItalic = RobotoCondensedExtraLightItalic;
                public static readonly string Light = RobotoCondensedLight;
                public static readonly string LightItalic = RobotoCondensedLightItalic;
                public static readonly string Regular = RobotoCondensedRegular;
                public static readonly string Italic = RobotoCondensedItalic;
                public static readonly string Medium = RobotoCondensedMedium;
                public static readonly string MediumItalic = RobotoCondensedMediumItalic;
                public static readonly string SemiBold = RobotoCondensedSemiBold;
                public static readonly string SemiBoldItalic = RobotoCondensedSemiBoldItalic;
                public static readonly string Bold = RobotoCondensedBold;
                public static readonly string BoldItalic = RobotoCondensedBoldItalic;
                public static readonly string ExtraBold = RobotoCondensedExtraBold;
                public static readonly string ExtraBoldItalic = RobotoCondensedExtraBoldItalic;
                public static readonly string Black = RobotoCondensedBlack;
                public static readonly string BlackItalic = RobotoCondensedBlackItalic;

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
            /// Open Sans font family paths grouped by weight/Style.
            /// </summary>
            public static class OpenSans
            {
                public static readonly string Light = OpenSansLight;
                public static readonly string LightItalic = OpenSansLightItalic;
                public static readonly string Regular = OpenSansRegular;
                public static readonly string Italic = OpenSansItalic;
                public static readonly string SemiBold = OpenSansSemiBold;
                public static readonly string SemiBoldItalic = OpenSansSemiBoldItalic;
                public static readonly string Bold = OpenSansBold;
                public static readonly string BoldItalic = OpenSansBoldItalic;
                public static readonly string ExtraBold = OpenSansExtraBold;
                public static readonly string ExtraBoldItalic = OpenSansExtraBoldItalic;

                // Condensed variants
                public static readonly string CondensedLight = OpenSansCondensedLight;
                public static readonly string CondensedLightItalic = OpenSansCondensedLightItalic;
                public static readonly string CondensedRegular = OpenSansCondensedRegular;
                public static readonly string CondensedItalic = OpenSansCondensedItalic;
                public static readonly string CondensedSemiBold = OpenSansCondensedSemiBold;
                public static readonly string CondensedSemiBoldItalic = OpenSansCondensedSemiBoldItalic;
                public static readonly string CondensedBold = OpenSansCondensedBold;
                public static readonly string CondensedBoldItalic = OpenSansCondensedBoldItalic;
                public static readonly string CondensedExtraBold = OpenSansCondensedExtraBold;
                public static readonly string CondensedExtraBoldItalic = OpenSansCondensedExtraBoldItalic;

                public static Dictionary<string, string> GetAllStyles()
                {
                    return new Dictionary<string, string>
                    {
                        { "Light", Light },
                        { "LightItalic", LightItalic },
                        { "Regular", Regular },
                        { "Italic", Italic },
                        { "SemiBold", SemiBold },
                        { "SemiBoldItalic", SemiBoldItalic },
                        { "Bold", Bold },
                        { "BoldItalic", BoldItalic },
                        { "ExtraBold", ExtraBold },
                        { "ExtraBoldItalic", ExtraBoldItalic },
                        { "CondensedLight", CondensedLight },
                        { "CondensedLightItalic", CondensedLightItalic },
                        { "CondensedRegular", CondensedRegular },
                        { "CondensedItalic", CondensedItalic },
                        { "CondensedSemiBold", CondensedSemiBold },
                        { "CondensedSemiBoldItalic", CondensedSemiBoldItalic },
                        { "CondensedBold", CondensedBold },
                        { "CondensedBoldItalic", CondensedBoldItalic },
                        { "CondensedExtraBold", CondensedExtraBold },
                        { "CondensedExtraBoldItalic", CondensedExtraBoldItalic }
                    };
                }
            }

            /// <summary>
            /// Montserrat font family paths grouped by weight/Style.
            /// </summary>
            public static class Montserrat
            {
                public static readonly string Thin = MontserratThin;
                public static readonly string ThinItalic = MontserratThinItalic;
                public static readonly string ExtraLight = MontserratExtraLight;
                public static readonly string ExtraLightItalic = MontserratExtraLightItalic;
                public static readonly string Light = MontserratLight;
                public static readonly string LightItalic = MontserratLightItalic;
                public static readonly string Regular = MontserratRegular;
                public static readonly string Italic = MontserratItalic;
                public static readonly string Medium = MontserratMedium;
                public static readonly string MediumItalic = MontserratMediumItalic;
                public static readonly string SemiBold = MontserratSemiBold;
                public static readonly string SemiBoldItalic = MontserratSemiBoldItalic;
                public static readonly string Bold = MontserratBold;
                public static readonly string BoldItalic = MontserratBoldItalic;
                public static readonly string ExtraBold = MontserratExtraBold;
                public static readonly string ExtraBoldItalic = MontserratExtraBoldItalic;
                public static readonly string Black = MontserratBlack;
                public static readonly string BlackItalic = MontserratBlackItalic;

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
            /// Inter font family paths grouped by weight/Style (18pt variants).
            /// </summary>
            public static class Inter
            {
                public static readonly string Thin = Inter18ptThin;
                public static readonly string ThinItalic = Inter18ptThinItalic;
                public static readonly string ExtraLight = Inter18ptExtraLight;
                public static readonly string ExtraLightItalic = Inter18ptExtraLightItalic;
                public static readonly string Light = Inter18ptLight;
                public static readonly string LightItalic = Inter18ptLightItalic;
                public static readonly string Regular = Inter18ptRegular;
                public static readonly string Italic = Inter18ptItalic;
                public static readonly string Medium = Inter18ptMedium;
                public static readonly string MediumItalic = Inter18ptMediumItalic;
                public static readonly string SemiBold = Inter18ptSemiBold;
                public static readonly string SemiBoldItalic = Inter18ptSemiBoldItalic;
                public static readonly string Bold = Inter18ptBold;
                public static readonly string BoldItalic = Inter18ptBoldItalic;
                public static readonly string ExtraBold = Inter18ptExtraBold;
                public static readonly string ExtraBoldItalic = Inter18ptExtraBoldItalic;
                public static readonly string Black = Inter18ptBlack;
                public static readonly string BlackItalic = Inter18ptBlackItalic;

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
            /// Nunito font family paths (variable fonts).
            /// </summary>
            public static class Nunito
            {
                public static readonly string Variable = NunitoVariable;
                public static readonly string ItalicVariable = NunitoItalicVariable;
                public static readonly string FullVariable = NunitoFullVariable;

                public static Dictionary<string, string> GetAllStyles()
                {
                    return new Dictionary<string, string>
                    {
                        { "Variable", Variable },
                        { "ItalicVariable", ItalicVariable },
                        { "FullVariable", FullVariable }
                    };
                }
            }

            /// <summary>
            /// Tajawal font family paths grouped by weight/Style.
            /// </summary>
            public static class Tajawal
            {
                public static readonly string ExtraLight = TajawalExtraLight;
                public static readonly string Light = TajawalLight;
                public static readonly string Regular = TajawalRegular;
                public static readonly string Medium = TajawalMedium;
                public static readonly string Bold = TajawalBold;
                public static readonly string ExtraBold = TajawalExtraBold;
                public static readonly string Black = TajawalBlack;

                public static Dictionary<string, string> GetAllWeights()
                {
                    return new Dictionary<string, string>
                    {
                        { "ExtraLight", ExtraLight },
                        { "Light", Light },
                        { "Regular", Regular },
                        { "Medium", Medium },
                        { "Bold", Bold },
                        { "ExtraBold", ExtraBold },
                        { "Black", Black }
                    };
                }
            }

            /// <summary>
            /// Oxygen font family paths grouped by weight/Style.
            /// </summary>
            public static class Oxygen
            {
                public static readonly string Regular = OxygenRegular;
                public static readonly string Bold = OxygenBold;
                public static readonly string MonoRegular = OxygenMonoRegular;

                public static Dictionary<string, string> GetAllStyles()
                {
                    return new Dictionary<string, string>
                    {
                        { "Regular", Regular },
                        { "Bold", Bold },
                        { "MonoRegular", MonoRegular }
                    };
                }
            }

            #endregion

            #region "Monospace Fonts"

            /// <summary>
            /// Fira Code font family paths grouped by weight/Style.
            /// </summary>
            public static class FiraCode
            {
                public static readonly string Light = FiraCodeLight;
                public static readonly string Regular = FiraCodeRegular;
                public static readonly string Medium = FiraCodeMedium;
                public static readonly string SemiBold = FiraCodeSemiBold;
                public static readonly string Bold = FiraCodeBold;

                public static Dictionary<string, string> GetAllWeights()
                {
                    return new Dictionary<string, string>
                    {
                        { "Light", Light },
                        { "Regular", Regular },
                        { "Medium", Medium },
                        { "SemiBold", SemiBold },
                        { "Bold", Bold }
                    };
                }
            }

            /// <summary>
            /// JetBrains Mono font family paths grouped by weight/Style.
            /// </summary>
            public static class JetBrainsMono
            {
                public static readonly string Thin = JetBrainsMonoThin;
                public static readonly string ThinItalic = JetBrainsMonoThinItalic;
                public static readonly string ExtraLight = JetBrainsMonoExtraLight;
                public static readonly string ExtraLightItalic = JetBrainsMonoExtraLightItalic;
                public static readonly string Light = JetBrainsMonoLight;
                public static readonly string LightItalic = JetBrainsMonoLightItalic;
                public static readonly string Regular = JetBrainsMonoRegular;
                public static readonly string Italic = JetBrainsMonoItalic;
                public static readonly string Medium = JetBrainsMonoMedium;
                public static readonly string MediumItalic = JetBrainsMonoMediumItalic;
                public static readonly string SemiBold = JetBrainsMonoSemiBold;
                public static readonly string SemiBoldItalic = JetBrainsMonoSemiBoldItalic;
                public static readonly string Bold = JetBrainsMonoBold;
                public static readonly string BoldItalic = JetBrainsMonoBoldItalic;
                public static readonly string ExtraBold = JetBrainsMonoExtraBold;
                public static readonly string ExtraBoldItalic = JetBrainsMonoExtraBoldItalic;

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
                        { "ExtraBoldItalic", ExtraBoldItalic }
                    };
                }
            }

            /// <summary>
            /// Cascadia Code font family paths grouped by weight/Style.
            /// </summary>
            public static class CascadiaCode
            {
                public static readonly string ExtraLight = CascadiaCodeExtraLight;
                public static readonly string ExtraLightItalic = CascadiaCodeExtraLightItalic;
                public static readonly string SemiLight = CascadiaCodeSemiLight;
                public static readonly string SemiLightItalic = CascadiaCodeSemiLightItalic;
                public static readonly string Light = CascadiaCodeLight;
                public static readonly string LightItalic = CascadiaCodeLightItalic;
                public static readonly string Regular = CascadiaCodeRegular;
                public static readonly string Italic = CascadiaCodeItalic;
                public static readonly string SemiBold = CascadiaCodeSemiBold;
                public static readonly string SemiBoldItalic = CascadiaCodeSemiBoldItalic;
                public static readonly string Bold = CascadiaCodeBold;
                public static readonly string BoldItalic = CascadiaCodeBoldItalic;

                public static Dictionary<string, string> GetAllStyles()
                {
                    return new Dictionary<string, string>
                    {
                        { "ExtraLight", ExtraLight },
                        { "ExtraLightItalic", ExtraLightItalic },
                        { "SemiLight", SemiLight },
                        { "SemiLightItalic", SemiLightItalic },
                        { "Light", Light },
                        { "LightItalic", LightItalic },
                        { "Regular", Regular },
                        { "Italic", Italic },
                        { "SemiBold", SemiBold },
                        { "SemiBoldItalic", SemiBoldItalic },
                        { "Bold", Bold },
                        { "BoldItalic", BoldItalic }
                    };
                }
            }

            /// <summary>
            /// Space Mono font family paths grouped by weight/Style.
            /// </summary>
            public static class SpaceMono
            {
                public static readonly string Regular = SpaceMonoRegular;
                public static readonly string Italic = SpaceMonoItalic;
                public static readonly string Bold = SpaceMonoBold;
                public static readonly string BoldItalic = SpaceMonoBoldItalic;

                public static Dictionary<string, string> GetAllStyles()
                {
                    return new Dictionary<string, string>
                    {
                        { "Regular", Regular },
                        { "Italic", Italic },
                        { "Bold", Bold },
                        { "BoldItalic", BoldItalic }
                    };
                }
            }

            /// <summary>
            /// DejaVu Sans Mono font family paths grouped by weight/Style.
            /// </summary>
            public static class DejaVuSansMono
            {
                public static readonly string Regular = BeepFontPaths.DejaVuSansMono;
                public static readonly string Oblique = DejaVuSansMonoOblique;
                public static readonly string Bold = DejaVuSansMonoBold;
                public static readonly string BoldOblique = DejaVuSansMonoBoldOblique;

                public static Dictionary<string, string> GetAllStyles()
                {
                    return new Dictionary<string, string>
                    {
                        { "Regular", Regular },
                        { "Oblique", Oblique },
                        { "Bold", Bold },
                        { "BoldOblique", BoldOblique }
                    };
                }
            }

            #endregion

            #region "Display Fonts"

            /// <summary>
            /// Bebas Neue font family paths grouped by weight/Style.
            /// </summary>
            public static class BebasNeue
            {
                public static readonly string Thin = BebasNeueThin;
                public static readonly string Light = BebasNeueLight;
                public static readonly string Book = BebasNeueBook;
                public static readonly string Regular = BebasNeueRegular;
                public static readonly string Bold = BebasNeueBold;

                public static Dictionary<string, string> GetAllWeights()
                {
                    return new Dictionary<string, string>
                    {
                        { "Thin", Thin },
                        { "Light", Light },
                        { "Book", Book },
                        { "Regular", Regular },
                        { "Bold", Bold }
                    };
                }
            }

            /// <summary>
            /// Orbitron font family paths grouped by weight/Style.
            /// </summary>
            public static class Orbitron
            {
                public static readonly string Light = OrbitronLight;
                public static readonly string Medium = OrbitronMedium;
                public static readonly string Bold = OrbitronBold;
                public static readonly string Black = OrbitronBlack;

                public static Dictionary<string, string> GetAllWeights()
                {
                    return new Dictionary<string, string>
                    {
                        { "Light", Light },
                        { "Medium", Medium },
                        { "Bold", Bold },
                        { "Black", Black }
                    };
                }
            }

            /// <summary>
            /// Exo 2 font family paths grouped by weight/Style.
            /// </summary>
            public static class Exo2
            {
                public static readonly string Thin = Exo2Thin;
                public static readonly string ThinItalic = Exo2ThinItalic;
                public static readonly string ExtraLight = Exo2ExtraLight;
                public static readonly string ExtraLightItalic = Exo2ExtraLightItalic;
                public static readonly string Light = Exo2Light;
                public static readonly string LightItalic = Exo2LightItalic;
                public static readonly string Regular = Exo2Regular;
                public static readonly string Italic = Exo2Italic;
                public static readonly string Medium = Exo2Medium;
                public static readonly string MediumItalic = Exo2MediumItalic;
                public static readonly string SemiBold = Exo2SemiBold;
                public static readonly string SemiBoldItalic = Exo2SemiBoldItalic;
                public static readonly string Bold = Exo2Bold;
                public static readonly string BoldItalic = Exo2BoldItalic;
                public static readonly string ExtraBold = Exo2ExtraBold;
                public static readonly string ExtraBoldItalic = Exo2ExtraBoldItalic;
                public static readonly string Black = Exo2Black;
                public static readonly string BlackItalic = Exo2BlackItalic;

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
            /// Rajdhani font family paths grouped by weight/Style.
            /// </summary>
            public static class Rajdhani
            {
                public static readonly string Light = RajdhaniLight;
                public static readonly string Regular = RajdhaniRegular;
                public static readonly string Medium = RajdhaniMedium;
                public static readonly string SemiBold = RajdhaniSemiBold;
                public static readonly string Bold = RajdhaniBold;

                public static Dictionary<string, string> GetAllWeights()
                {
                    return new Dictionary<string, string>
                    {
                        { "Light", Light },
                        { "Regular", Regular },
                        { "Medium", Medium },
                        { "SemiBold", SemiBold },
                        { "Bold", Bold }
                    };
                }
            }

            /// <summary>
            /// Whitney font family paths grouped by weight/Style.
            /// </summary>
            public static class Whitney
            {
                public static readonly string Light = WhitneyLight;
                public static readonly string LightItalic = WhitneyLightItalic;
                public static readonly string LightSC = WhitneyLightSC;
                public static readonly string Book = WhitneyBook;
                public static readonly string BookItalic = WhitneyBookItalic;
                public static readonly string BookSC = WhitneyBookSC;
                public static readonly string Medium = WhitneyMedium;
                public static readonly string MediumItalic = WhitneyMediumItalic;
                public static readonly string MediumSC = WhitneyMediumSC;
                public static readonly string MediumItalicSC = WhitneyMediumItalicSC;
                public static readonly string SemiBold = WhitneySemiBold;
                public static readonly string SemiBoldItalic = WhitneySemiBoldItalic;
                public static readonly string SemiBoldItalicSC = WhitneySemiBoldItalicSC;
                public static readonly string Bold = WhitneyBold;
                public static readonly string BoldSC = WhitneyBoldSC;
                public static readonly string BoldItalicSC = WhitneyBoldItalicSC;
                public static readonly string BlackItalic = WhitneyBlackItalic;
                public static readonly string BlackSC = WhitneyBlackSC;
                public static readonly string TTF = WhitneyTTF;

                public static Dictionary<string, string> GetAllStyles()
                {
                    return new Dictionary<string, string>
                    {
                        { "Light", Light },
                        { "LightItalic", LightItalic },
                        { "LightSC", LightSC },
                        { "Book", Book },
                        { "BookItalic", BookItalic },
                        { "BookSC", BookSC },
                        { "Medium", Medium },
                        { "MediumItalic", MediumItalic },
                        { "MediumSC", MediumSC },
                        { "MediumItalicSC", MediumItalicSC },
                        { "SemiBold", SemiBold },
                        { "SemiBoldItalic", SemiBoldItalic },
                        { "SemiBoldItalicSC", SemiBoldItalicSC },
                        { "Bold", Bold },
                        { "BoldSC", BoldSC },
                        { "BoldItalicSC", BoldItalicSC },
                        { "BlackItalic", BlackItalic },
                        { "BlackSC", BlackSC },
                        { "TTF", TTF }
                    };
                }
            }

            /// <summary>
            /// Noto Color Emoji font family.
            /// </summary>
            public static class NotoColorEmoji
            {
                public static readonly string Regular = NotoColorEmojiRegular;

                public static Dictionary<string, string> GetAllStyles()
                {
                    return new Dictionary<string, string>
                    {
                        { "Regular", Regular }
                    };
                }
            }

            #endregion

            #region "Accessibility Fonts"

            /// <summary>
            /// Atkinson Hyperlegible font family paths grouped by weight/Style.
            /// </summary>
            public static class AtkinsonHyperlegible
            {
                public static readonly string Regular = AtkinsonHyperlegibleRegular;
                public static readonly string Italic = AtkinsonHyperlegibleItalic;
                public static readonly string Bold = AtkinsonHyperlegibleBold;
                public static readonly string BoldItalic = AtkinsonHyperlegibleBoldItalic;

                public static Dictionary<string, string> GetAllStyles()
                {
                    return new Dictionary<string, string>
                    {
                        { "Regular", Regular },
                        { "Italic", Italic },
                        { "Bold", Bold },
                        { "BoldItalic", BoldItalic }
                    };
                }
            }

            /// <summary>
            /// OpenDyslexic font family paths grouped by weight/Style.
            /// </summary>
            public static class OpenDyslexic
            {
                public static readonly string Regular = OpenDyslexicRegular;
                public static readonly string Italic = OpenDyslexicItalic;
                public static readonly string Bold = OpenDyslexicBold;
                public static readonly string BoldItalic = OpenDyslexicBoldItalic;

                public static Dictionary<string, string> GetAllStyles()
                {
                    return new Dictionary<string, string>
                    {
                        { "Regular", Regular },
                        { "Italic", Italic },
                        { "Bold", Bold },
                        { "BoldItalic", BoldItalic }
                    };
                }
            }

            /// <summary>
            /// Lexend font family paths grouped by weight/Style.
            /// </summary>
            public static class Lexend
            {
                public static readonly string Thin = LexendThin;
                public static readonly string ExtraLight = LexendExtraLight;
                public static readonly string Light = LexendLight;
                public static readonly string Regular = LexendRegular;
                public static readonly string Medium = LexendMedium;
                public static readonly string SemiBold = LexendSemiBold;
                public static readonly string Bold = LexendBold;
                public static readonly string ExtraBold = LexendExtraBold;
                public static readonly string Black = LexendBlack;

                public static Dictionary<string, string> GetAllWeights()
                {
                    return new Dictionary<string, string>
                    {
                        { "Thin", Thin },
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

            #endregion

            #region "System Fonts"

            /// <summary>
            /// Source Sans Pro font family paths grouped by weight/Style.
            /// </summary>
            public static class SourceSansPro
            {
                public static readonly string ExtraLight = SourceSans3ExtraLight;
                public static readonly string ExtraLightItalic = SourceSans3ExtraLightIt;
                public static readonly string Light = SourceSans3Light;
                public static readonly string LightItalic = SourceSans3LightIt;
                public static readonly string Regular = SourceSans3Regular;
                public static readonly string Italic = SourceSans3It;
                public static readonly string Semibold = SourceSans3Semibold;
                public static readonly string SemiboldItalic = SourceSans3SemiboldIt;
                public static readonly string Bold = SourceSans3Bold;
                public static readonly string BoldItalic = SourceSans3BoldIt;
                public static readonly string Black = SourceSans3Black;
                public static readonly string BlackItalic = SourceSans3BlackIt;

                public static Dictionary<string, string> GetAllStyles()
                {
                    return new Dictionary<string, string>
                    {
                        { "ExtraLight", ExtraLight },
                        { "ExtraLightItalic", ExtraLightItalic },
                        { "Light", Light },
                        { "LightItalic", LightItalic },
                        { "Regular", Regular },
                        { "Italic", Italic },
                        { "Semibold", Semibold },
                        { "SemiboldItalic", SemiboldItalic },
                        { "Bold", Bold },
                        { "BoldItalic", BoldItalic },
                        { "Black", Black },
                        { "BlackItalic", BlackItalic }
                    };
                }
            }

            /// <summary>
            /// Comic Neue font family paths grouped by weight/Style.
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

            #endregion

            #region "Individual Fonts"

            /// <summary>
            /// Individual fonts that are not part of a family.
            /// </summary>
            public static class Individual
            {
                public static readonly string Caprasimo = CaprasimoRegular;
                public static readonly string Consolas = BeepFontPaths.Consolas;
            }

            #endregion
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace TheTechIdea.Beep.Vis.Modules
{
    public static class BeepThemesManager
    {
        public static Dictionary<EnumBeepThemes, (
            Color LoginPopoverBackgroundColor,
            Color LoginTitleColor,
            Color LoginSubtitleColor,
            Color LoginDescriptionColor,
            Color LoginLinkColor,
            Color LoginButtonBackgroundColor,
            Color LoginButtonTextColor,
            Color LoginDropdownBackgroundColor,
            Color LoginDropdownTextColor,
            Color LoginLogoBackgroundColor
        )> LoginColors =
            new Dictionary<EnumBeepThemes, (Color, Color, Color, Color, Color, Color, Color, Color, Color, Color)>
            {
                { EnumBeepThemes.DefaultTheme, (
                    LoginPopoverBackgroundColor: Color.White,
                    LoginTitleColor: Color.Black,
                    LoginSubtitleColor: Color.DarkBlue,
                    LoginDescriptionColor: Color.Gray,
                    LoginLinkColor: Color.RoyalBlue,
                    LoginButtonBackgroundColor: Color.MediumBlue,
                    LoginButtonTextColor: Color.White,
                    LoginDropdownBackgroundColor: Color.White,
                    LoginDropdownTextColor: Color.Black,
                    LoginLogoBackgroundColor: Color.LightGray
                )},
                { EnumBeepThemes.WinterTheme, (
                    LoginPopoverBackgroundColor: Color.WhiteSmoke,
                    LoginTitleColor: Color.SteelBlue,
                    LoginSubtitleColor: Color.Navy,
                    LoginDescriptionColor: Color.LightSlateGray,
                    LoginLinkColor: Color.CadetBlue,
                    LoginButtonBackgroundColor: Color.DodgerBlue,
                    LoginButtonTextColor: Color.White,
                    LoginDropdownBackgroundColor: Color.WhiteSmoke,
                    LoginDropdownTextColor: Color.DarkSlateGray,
                    LoginLogoBackgroundColor: Color.LightSteelBlue
                )},
                { EnumBeepThemes.CandyTheme, (
                    LoginPopoverBackgroundColor: Color.MistyRose,
                    LoginTitleColor: Color.DeepPink,
                    LoginSubtitleColor: Color.MediumVioletRed,
                    LoginDescriptionColor: Color.HotPink,
                    LoginLinkColor: Color.Magenta,
                    LoginButtonBackgroundColor: Color.HotPink,
                    LoginButtonTextColor: Color.White,
                    LoginDropdownBackgroundColor: Color.MistyRose,
                    LoginDropdownTextColor: Color.DarkMagenta,
                    LoginLogoBackgroundColor: Color.Pink
                )},
                { EnumBeepThemes.ZenTheme, (
                    LoginPopoverBackgroundColor: Color.Beige,
                    LoginTitleColor: Color.DarkSlateGray,
                    LoginSubtitleColor: Color.SeaGreen,
                    LoginDescriptionColor: Color.DarkOliveGreen,
                    LoginLinkColor: Color.MediumSeaGreen,
                    LoginButtonBackgroundColor: Color.SeaGreen,
                    LoginButtonTextColor: Color.White,
                    LoginDropdownBackgroundColor: Color.Beige,
                    LoginDropdownTextColor: Color.DarkOliveGreen,
                    LoginLogoBackgroundColor: Color.PaleGreen
                )},
                { EnumBeepThemes.RetroTheme, (
                    LoginPopoverBackgroundColor: Color.Bisque,
                    LoginTitleColor: Color.Brown,
                    LoginSubtitleColor: Color.SaddleBrown,
                    LoginDescriptionColor: Color.Peru,
                    LoginLinkColor: Color.Chocolate,
                    LoginButtonBackgroundColor: Color.Chocolate,
                    LoginButtonTextColor: Color.White,
                    LoginDropdownBackgroundColor: Color.AntiqueWhite,
                    LoginDropdownTextColor: Color.Brown,
                    LoginLogoBackgroundColor: Color.Tan
                )},
                { EnumBeepThemes.RoyalTheme, (
                    LoginPopoverBackgroundColor: Color.MidnightBlue,
                    LoginTitleColor: Color.Gold,
                    LoginSubtitleColor: Color.LightGoldenrodYellow,
                    LoginDescriptionColor: Color.SlateBlue,
                    LoginLinkColor: Color.RoyalBlue,
                    LoginButtonBackgroundColor: Color.DarkBlue,
                    LoginButtonTextColor: Color.White,
                    LoginDropdownBackgroundColor: Color.MidnightBlue,
                    LoginDropdownTextColor: Color.LightYellow,
                    LoginLogoBackgroundColor: Color.RoyalBlue
                )},
                { EnumBeepThemes.HighlightTheme, (
                    LoginPopoverBackgroundColor: Color.Yellow,
                    LoginTitleColor: Color.Black,
                    LoginSubtitleColor: Color.Red,
                    LoginDescriptionColor: Color.DarkOrange,
                    LoginLinkColor: Color.OrangeRed,
                    LoginButtonBackgroundColor: Color.Orange,
                    LoginButtonTextColor: Color.Black,
                    LoginDropdownBackgroundColor: Color.LightYellow,
                    LoginDropdownTextColor: Color.DarkRed,
                    LoginLogoBackgroundColor: Color.Orange
                )},
                { EnumBeepThemes.DarkTheme, (
                    LoginPopoverBackgroundColor: Color.DarkGray,
                    LoginTitleColor: Color.White,
                    LoginSubtitleColor: Color.LightGray,
                    LoginDescriptionColor: Color.Silver,
                    LoginLinkColor: Color.Cyan,
                    LoginButtonBackgroundColor: Color.DarkSlateGray,
                    LoginButtonTextColor: Color.White,
                    LoginDropdownBackgroundColor: Color.Black,
                    LoginDropdownTextColor: Color.LightGray,
                    LoginLogoBackgroundColor: Color.DimGray
                )},
                { EnumBeepThemes.OceanTheme, (
                    LoginPopoverBackgroundColor: Color.LightBlue,
                    LoginTitleColor: Color.DarkCyan,
                    LoginSubtitleColor: Color.Teal,
                    LoginDescriptionColor: Color.MediumTurquoise,
                    LoginLinkColor: Color.DodgerBlue,
                    LoginButtonBackgroundColor: Color.DarkCyan,
                    LoginButtonTextColor: Color.White,
                    LoginDropdownBackgroundColor: Color.LightBlue,
                    LoginDropdownTextColor: Color.Black,
                    LoginLogoBackgroundColor: Color.LightSeaGreen
                )},
                { EnumBeepThemes.LightTheme, (
                    LoginPopoverBackgroundColor: Color.White,
                    LoginTitleColor: Color.Black,
                    LoginSubtitleColor: Color.DarkBlue,
                    LoginDescriptionColor: Color.DimGray,
                    LoginLinkColor: Color.Blue,
                    LoginButtonBackgroundColor: Color.LightGray,
                    LoginButtonTextColor: Color.Black,
                    LoginDropdownBackgroundColor: Color.White,
                    LoginDropdownTextColor: Color.DarkGray,
                    LoginLogoBackgroundColor: Color.Silver
                )},
                { EnumBeepThemes.PastelTheme, (
                    LoginPopoverBackgroundColor: Color.Lavender,
                    LoginTitleColor: Color.MediumOrchid,
                    LoginSubtitleColor: Color.DarkOrchid,
                    LoginDescriptionColor: Color.Plum,
                    LoginLinkColor: Color.Violet,
                    LoginButtonBackgroundColor: Color.MediumOrchid,
                    LoginButtonTextColor: Color.White,
                    LoginDropdownBackgroundColor: Color.Lavender,
                    LoginDropdownTextColor: Color.DarkSlateGray,
                    LoginLogoBackgroundColor: Color.Thistle
                )},
                { EnumBeepThemes.MidnightTheme, (
                    LoginPopoverBackgroundColor: Color.MidnightBlue,
                    LoginTitleColor: Color.White,
                    LoginSubtitleColor: Color.LightSteelBlue,
                    LoginDescriptionColor: Color.SlateGray,
                    LoginLinkColor: Color.DodgerBlue,
                    LoginButtonBackgroundColor: Color.MidnightBlue,
                    LoginButtonTextColor: Color.White,
                    LoginDropdownBackgroundColor: Color.DarkBlue,
                    LoginDropdownTextColor: Color.LightCyan,
                    LoginLogoBackgroundColor: Color.Navy
                )},
                { EnumBeepThemes.SpringTheme, (
                    LoginPopoverBackgroundColor: Color.PaleGreen,
                    LoginTitleColor: Color.ForestGreen,
                    LoginSubtitleColor: Color.DarkGreen,
                    LoginDescriptionColor: Color.SeaGreen,
                    LoginLinkColor: Color.LimeGreen,
                    LoginButtonBackgroundColor: Color.MediumSpringGreen,
                    LoginButtonTextColor: Color.White,
                    LoginDropdownBackgroundColor: Color.PaleGreen,
                    LoginDropdownTextColor: Color.DarkGreen,
                    LoginLogoBackgroundColor: Color.Chartreuse
                )},
                { EnumBeepThemes.ForestTheme, (
                    LoginPopoverBackgroundColor: Color.DarkOliveGreen,
                    LoginTitleColor: Color.LightGreen,
                    LoginSubtitleColor: Color.OliveDrab,
                    LoginDescriptionColor: Color.ForestGreen,
                    LoginLinkColor: Color.MediumSeaGreen,
                    LoginButtonBackgroundColor: Color.DarkSeaGreen,
                    LoginButtonTextColor: Color.White,
                    LoginDropdownBackgroundColor: Color.MediumSeaGreen,
                    LoginDropdownTextColor: Color.White,
                    LoginLogoBackgroundColor: Color.SeaGreen
                )},
                { EnumBeepThemes.NeonTheme, (
                    LoginPopoverBackgroundColor: Color.Black,
                    LoginTitleColor: Color.Lime,
                    LoginSubtitleColor: Color.Fuchsia,
                    LoginDescriptionColor: Color.Cyan,
                    LoginLinkColor: Color.Yellow,
                    LoginButtonBackgroundColor: Color.Black,
                    LoginButtonTextColor: Color.Lime,
                    LoginDropdownBackgroundColor: Color.DarkSlateGray,
                    LoginDropdownTextColor: Color.White,
                    LoginLogoBackgroundColor: Color.Magenta
                )},
                { EnumBeepThemes.RusticTheme, (
                    LoginPopoverBackgroundColor: Color.SaddleBrown,
                    LoginTitleColor: Color.BurlyWood,
                    LoginSubtitleColor: Color.Tan,
                    LoginDescriptionColor: Color.Sienna,
                    LoginLinkColor: Color.Peru,
                    LoginButtonBackgroundColor: Color.SaddleBrown,
                    LoginButtonTextColor: Color.White,
                    LoginDropdownBackgroundColor: Color.Tan,
                    LoginDropdownTextColor: Color.DarkSlateGray,
                    LoginLogoBackgroundColor: Color.Peru
                )},
                { EnumBeepThemes.GalaxyTheme, (
                    LoginPopoverBackgroundColor: Color.DarkSlateBlue,
                    LoginTitleColor: Color.White,
                    LoginSubtitleColor: Color.MediumPurple,
                    LoginDescriptionColor: Color.SlateBlue,
                    LoginLinkColor: Color.MediumOrchid,
                    LoginButtonBackgroundColor: Color.DarkSlateBlue,
                    LoginButtonTextColor: Color.White,
                    LoginDropdownBackgroundColor: Color.SlateBlue,
                    LoginDropdownTextColor: Color.White,
                    LoginLogoBackgroundColor: Color.MediumPurple
                )},
                { EnumBeepThemes.DesertTheme, (
                    LoginPopoverBackgroundColor: Color.SandyBrown,
                    LoginTitleColor: Color.Sienna,
                    LoginSubtitleColor: Color.Peru,
                    LoginDescriptionColor: Color.Tan,
                    LoginLinkColor: Color.DarkOrange,
                    LoginButtonBackgroundColor: Color.SandyBrown,
                    LoginButtonTextColor: Color.Black,
                    LoginDropdownBackgroundColor: Color.Bisque,
                    LoginDropdownTextColor: Color.Brown,
                    LoginLogoBackgroundColor: Color.DarkOrange
                )},
                { EnumBeepThemes.VintageTheme, (
                    LoginPopoverBackgroundColor: Color.Bisque,
                    LoginTitleColor: Color.Sienna,
                    LoginSubtitleColor: Color.Brown,
                    LoginDescriptionColor: Color.Peru,
                    LoginLinkColor: Color.DarkRed,
                    LoginButtonBackgroundColor: Color.Bisque,
                    LoginButtonTextColor: Color.DarkSlateGray,
                    LoginDropdownBackgroundColor: Color.AntiqueWhite,
                    LoginDropdownTextColor: Color.DarkRed,
                    LoginLogoBackgroundColor: Color.Tan
                )},
                { EnumBeepThemes.ModernDarkTheme, (
                    LoginPopoverBackgroundColor: Color.DarkSlateGray,
                    LoginTitleColor: Color.White,
                    LoginSubtitleColor: Color.LightGray,
                    LoginDescriptionColor: Color.Silver,
                    LoginLinkColor: Color.Cyan,
                    LoginButtonBackgroundColor: Color.DarkSlateGray,
                    LoginButtonTextColor: Color.White,
                    LoginDropdownBackgroundColor: Color.Black,
                    LoginDropdownTextColor: Color.LightGray,
                    LoginLogoBackgroundColor: Color.SlateGray
                )},
                { EnumBeepThemes.MaterialDesignTheme, (
                    LoginPopoverBackgroundColor: Color.LightSlateGray,
                    LoginTitleColor: Color.CornflowerBlue,
                    LoginSubtitleColor: Color.SteelBlue,
                    LoginDescriptionColor: Color.DarkSlateGray,
                    LoginLinkColor: Color.RoyalBlue,
                    LoginButtonBackgroundColor: Color.LightSlateGray,
                    LoginButtonTextColor: Color.White,
                    LoginDropdownBackgroundColor: Color.DarkSlateGray,
                    LoginDropdownTextColor: Color.LightBlue,
                    LoginLogoBackgroundColor: Color.RoyalBlue
                )},
                { EnumBeepThemes.NeumorphismTheme, (
                    LoginPopoverBackgroundColor: Color.WhiteSmoke,
                    LoginTitleColor: Color.DarkGray,
                    LoginSubtitleColor: Color.SlateGray,
                    LoginDescriptionColor: Color.Gainsboro,
                    LoginLinkColor: Color.Silver,
                    LoginButtonBackgroundColor: Color.WhiteSmoke,
                    LoginButtonTextColor: Color.Gray,
                    LoginDropdownBackgroundColor: Color.White,
                    LoginDropdownTextColor: Color.DarkSlateGray,
                    LoginLogoBackgroundColor: Color.Silver
                )},
                { EnumBeepThemes.GlassmorphismTheme, (
                    LoginPopoverBackgroundColor: Color.DarkSlateGray,
                    LoginTitleColor: Color.White,
                    LoginSubtitleColor: Color.LightSteelBlue,
                    LoginDescriptionColor: Color.SteelBlue,
                    LoginLinkColor: Color.DodgerBlue,
                    LoginButtonBackgroundColor: Color.DarkSlateGray,
                    LoginButtonTextColor: Color.White,
                    LoginDropdownBackgroundColor: Color.Black,
                    LoginDropdownTextColor: Color.LightCyan,
                    LoginLogoBackgroundColor: Color.DodgerBlue
                )},
                { EnumBeepThemes.FlatDesignTheme, (
                    LoginPopoverBackgroundColor: Color.Gainsboro,
                    LoginTitleColor: Color.DarkGray,
                    LoginSubtitleColor: Color.SlateGray,
                    LoginDescriptionColor: Color.Gray,
                    LoginLinkColor: Color.Silver,
                    LoginButtonBackgroundColor: Color.Gainsboro,
                    LoginButtonTextColor: Color.Black,
                    LoginDropdownBackgroundColor: Color.WhiteSmoke,
                    LoginDropdownTextColor: Color.DarkGray,
                    LoginLogoBackgroundColor: Color.Silver
                )},
                { EnumBeepThemes.CyberpunkNeonTheme, (
                    LoginPopoverBackgroundColor: Color.Black,
                    LoginTitleColor: Color.Fuchsia,
                    LoginSubtitleColor: Color.Aqua,
                    LoginDescriptionColor: Color.Magenta,
                    LoginLinkColor: Color.Lime,
                    LoginButtonBackgroundColor: Color.Black,
                    LoginButtonTextColor: Color.Lime,
                    LoginDropdownBackgroundColor: Color.DarkSlateGray,
                    LoginDropdownTextColor: Color.Yellow,
                    LoginLogoBackgroundColor: Color.HotPink
                )},
                { EnumBeepThemes.GradientBurstTheme, (
                    LoginPopoverBackgroundColor: Color.DarkMagenta,
                    LoginTitleColor: Color.OrangeRed,
                    LoginSubtitleColor: Color.DeepSkyBlue,
                    LoginDescriptionColor: Color.Crimson,
                    LoginLinkColor: Color.DodgerBlue,
                    LoginButtonBackgroundColor: Color.DarkMagenta,
                    LoginButtonTextColor: Color.White,
                    LoginDropdownBackgroundColor: Color.Purple,
                    LoginDropdownTextColor: Color.Yellow,
                    LoginLogoBackgroundColor: Color.DodgerBlue
                )},
                { EnumBeepThemes.HighContrastTheme, (
                    LoginPopoverBackgroundColor: Color.Black,
                    LoginTitleColor: Color.White,
                    LoginSubtitleColor: Color.Yellow,
                    LoginDescriptionColor: Color.LightGray,
                    LoginLinkColor: Color.Cyan,
                    LoginButtonBackgroundColor: Color.Black,
                    LoginButtonTextColor: Color.Yellow,
                    LoginDropdownBackgroundColor: Color.DarkGray,
                    LoginDropdownTextColor: Color.White,
                    LoginLogoBackgroundColor: Color.Silver
                )},
                { EnumBeepThemes.MonochromeTheme, (
                    LoginPopoverBackgroundColor: Color.Black,
                    LoginTitleColor: Color.White,
                    LoginSubtitleColor: Color.LightGray,
                    LoginDescriptionColor: Color.Silver,
                    LoginLinkColor: Color.Gray,
                    LoginButtonBackgroundColor: Color.Black,
                    LoginButtonTextColor: Color.White,
                    LoginDropdownBackgroundColor: Color.SlateGray,
                    LoginDropdownTextColor: Color.Silver,
                    LoginLogoBackgroundColor: Color.DimGray
                )},
                { EnumBeepThemes.LuxuryGoldTheme, (
                    LoginPopoverBackgroundColor: Color.DarkSlateGray,
                    LoginTitleColor: Color.Gold,
                    LoginSubtitleColor: Color.LightGoldenrodYellow,
                    LoginDescriptionColor: Color.Goldenrod,
                    LoginLinkColor: Color.Yellow,
                    LoginButtonBackgroundColor: Color.DarkSlateGray,
                    LoginButtonTextColor: Color.Gold,
                    LoginDropdownBackgroundColor: Color.Black,
                    LoginDropdownTextColor: Color.Gold,
                    LoginLogoBackgroundColor: Color.DarkGoldenrod
                )},
                { EnumBeepThemes.SunsetTheme, (
                    LoginPopoverBackgroundColor: Color.OrangeRed,
                    LoginTitleColor: Color.Crimson,
                    LoginSubtitleColor: Color.DarkOrange,
                    LoginDescriptionColor: Color.Orange,
                    LoginLinkColor: Color.Coral,
                    LoginButtonBackgroundColor: Color.OrangeRed,
                    LoginButtonTextColor: Color.White,
                    LoginDropdownBackgroundColor: Color.Tomato,
                    LoginDropdownTextColor: Color.DarkRed,
                    LoginLogoBackgroundColor: Color.Coral
                )},
                { EnumBeepThemes.AutumnTheme, (
                    LoginPopoverBackgroundColor: Color.DarkOrange,
                    LoginTitleColor: Color.Brown,
                    LoginSubtitleColor: Color.Orange,
                    LoginDescriptionColor: Color.Peru,
                    LoginLinkColor: Color.Sienna,
                    LoginButtonBackgroundColor: Color.DarkOrange,
                    LoginButtonTextColor: Color.White,
                    LoginDropdownBackgroundColor: Color.Chocolate,
                    LoginDropdownTextColor: Color.Tan,
                    LoginLogoBackgroundColor: Color.Sienna
                )},
                { EnumBeepThemes.EarthyTheme, (
                    LoginPopoverBackgroundColor: Color.Tan,
                    LoginTitleColor: Color.SaddleBrown,
                    LoginSubtitleColor: Color.Peru,
                    LoginDescriptionColor: Color.Sienna,
                    LoginLinkColor: Color.DarkOliveGreen,
                    LoginButtonBackgroundColor: Color.Tan,
                    LoginButtonTextColor: Color.Chocolate,
                    LoginDropdownBackgroundColor: Color.Wheat,
                    LoginDropdownTextColor: Color.DarkSlateGray,
                    LoginLogoBackgroundColor: Color.DarkOliveGreen
                )}
            };


        public static Dictionary<EnumBeepThemes, (
               Color CompanyPopoverBackgroundColor,
               Color CompanyTitleColor,
               Color CompanySubtitleColor,
               Color CompanyDescriptionColor,
               Color CompanyLinkColor,
               Color CompanyButtonBackgroundColor,
               Color CompanyButtonTextColor,
               Color CompanyDropdownBackgroundColor,
               Color CompanyDropdownTextColor,
               Color CompanyLogoBackgroundColor
           )> ThemeCompanyProfileColors =
               new Dictionary<EnumBeepThemes, (Color, Color, Color, Color, Color, Color, Color, Color, Color, Color)>
               {
                { EnumBeepThemes.DefaultTheme, (
                    CompanyPopoverBackgroundColor: Color.White,
                    CompanyTitleColor: Color.Black,
                    CompanySubtitleColor: Color.DarkBlue,
                    CompanyDescriptionColor: Color.Gray,
                    CompanyLinkColor: Color.Blue,
                    CompanyButtonBackgroundColor: Color.Blue,
                    CompanyButtonTextColor: Color.White,
                    CompanyDropdownBackgroundColor: Color.White,
                    CompanyDropdownTextColor: Color.Black,
                    CompanyLogoBackgroundColor: Color.LightGray
                )},
                { EnumBeepThemes.WinterTheme, (
                    CompanyPopoverBackgroundColor: Color.WhiteSmoke,
                    CompanyTitleColor: Color.SteelBlue,
                    CompanySubtitleColor: Color.Navy,
                    CompanyDescriptionColor: Color.LightSlateGray,
                    CompanyLinkColor: Color.CadetBlue,
                    CompanyButtonBackgroundColor: Color.CadetBlue,
                    CompanyButtonTextColor: Color.White,
                    CompanyDropdownBackgroundColor: Color.WhiteSmoke,
                    CompanyDropdownTextColor: Color.DarkSlateGray,
                    CompanyLogoBackgroundColor: Color.LightSteelBlue
                )},
                { EnumBeepThemes.CandyTheme, (
                    CompanyPopoverBackgroundColor: Color.MistyRose,
                    CompanyTitleColor: Color.DeepPink,
                    CompanySubtitleColor: Color.MediumVioletRed,
                    CompanyDescriptionColor: Color.HotPink,
                    CompanyLinkColor: Color.Magenta,
                    CompanyButtonBackgroundColor: Color.HotPink,
                    CompanyButtonTextColor: Color.White,
                    CompanyDropdownBackgroundColor: Color.MistyRose,
                    CompanyDropdownTextColor: Color.DarkMagenta,
                    CompanyLogoBackgroundColor: Color.Pink
                )},
                { EnumBeepThemes.ZenTheme, (
                    CompanyPopoverBackgroundColor: Color.Beige,
                    CompanyTitleColor: Color.DarkSlateGray,
                    CompanySubtitleColor: Color.SeaGreen,
                    CompanyDescriptionColor: Color.DarkOliveGreen,
                    CompanyLinkColor: Color.MediumSeaGreen,
                    CompanyButtonBackgroundColor: Color.SeaGreen,
                    CompanyButtonTextColor: Color.White,
                    CompanyDropdownBackgroundColor: Color.Beige,
                    CompanyDropdownTextColor: Color.DarkOliveGreen,
                    CompanyLogoBackgroundColor: Color.PaleGreen
                )},
                { EnumBeepThemes.RetroTheme, (
                    CompanyPopoverBackgroundColor: Color.Bisque,
                    CompanyTitleColor: Color.Brown,
                    CompanySubtitleColor: Color.SaddleBrown,
                    CompanyDescriptionColor: Color.Peru,
                    CompanyLinkColor: Color.Chocolate,
                    CompanyButtonBackgroundColor: Color.Chocolate,
                    CompanyButtonTextColor: Color.White,
                    CompanyDropdownBackgroundColor: Color.AntiqueWhite,
                    CompanyDropdownTextColor: Color.Brown,
                    CompanyLogoBackgroundColor: Color.Tan
                )},
                { EnumBeepThemes.RoyalTheme, (
                    CompanyPopoverBackgroundColor: Color.MidnightBlue,
                    CompanyTitleColor: Color.Gold,
                    CompanySubtitleColor: Color.LightGoldenrodYellow,
                    CompanyDescriptionColor: Color.SlateBlue,
                    CompanyLinkColor: Color.RoyalBlue,
                    CompanyButtonBackgroundColor: Color.MidnightBlue,
                    CompanyButtonTextColor: Color.White,
                    CompanyDropdownBackgroundColor: Color.DarkBlue,
                    CompanyDropdownTextColor: Color.LightYellow,
                    CompanyLogoBackgroundColor: Color.RoyalBlue
                )},
                { EnumBeepThemes.HighlightTheme, (
                    CompanyPopoverBackgroundColor: Color.Yellow,
                    CompanyTitleColor: Color.Black,
                    CompanySubtitleColor: Color.Red,
                    CompanyDescriptionColor: Color.DarkOrange,
                    CompanyLinkColor: Color.OrangeRed,
                    CompanyButtonBackgroundColor: Color.Orange,
                    CompanyButtonTextColor: Color.Black,
                    CompanyDropdownBackgroundColor: Color.LightYellow,
                    CompanyDropdownTextColor: Color.DarkRed,
                    CompanyLogoBackgroundColor: Color.Orange
                )},
                { EnumBeepThemes.DarkTheme, (
                    CompanyPopoverBackgroundColor: Color.DarkGray,
                    CompanyTitleColor: Color.White,
                    CompanySubtitleColor: Color.LightGray,
                    CompanyDescriptionColor: Color.Silver,
                    CompanyLinkColor: Color.Cyan,
                    CompanyButtonBackgroundColor: Color.DarkSlateGray,
                    CompanyButtonTextColor: Color.White,
                    CompanyDropdownBackgroundColor: Color.Black,
                    CompanyDropdownTextColor: Color.LightGray,
                    CompanyLogoBackgroundColor: Color.DimGray
                )},
                { EnumBeepThemes.OceanTheme, (
                    CompanyPopoverBackgroundColor: Color.LightBlue,
                    CompanyTitleColor: Color.DarkCyan,
                    CompanySubtitleColor: Color.Teal,
                    CompanyDescriptionColor: Color.MediumTurquoise,
                    CompanyLinkColor: Color.DodgerBlue,
                    CompanyButtonBackgroundColor: Color.DarkCyan,
                    CompanyButtonTextColor: Color.White,
                    CompanyDropdownBackgroundColor: Color.LightBlue,
                    CompanyDropdownTextColor: Color.Black,
                    CompanyLogoBackgroundColor: Color.LightSeaGreen
                )},
                { EnumBeepThemes.LightTheme, (
                    CompanyPopoverBackgroundColor: Color.White,
                    CompanyTitleColor: Color.Black,
                    CompanySubtitleColor: Color.DarkBlue,
                    CompanyDescriptionColor: Color.DimGray,
                    CompanyLinkColor: Color.Blue,
                    CompanyButtonBackgroundColor: Color.LightGray,
                    CompanyButtonTextColor: Color.Black,
                    CompanyDropdownBackgroundColor: Color.White,
                    CompanyDropdownTextColor: Color.DarkGray,
                    CompanyLogoBackgroundColor: Color.Silver
                )},
                { EnumBeepThemes.PastelTheme, (
                    CompanyPopoverBackgroundColor: Color.Lavender,
                    CompanyTitleColor: Color.MediumOrchid,
                    CompanySubtitleColor: Color.DarkOrchid,
                    CompanyDescriptionColor: Color.Plum,
                    CompanyLinkColor: Color.Violet,
                    CompanyButtonBackgroundColor: Color.MediumOrchid,
                    CompanyButtonTextColor: Color.White,
                    CompanyDropdownBackgroundColor: Color.Lavender,
                    CompanyDropdownTextColor: Color.DarkSlateGray,
                    CompanyLogoBackgroundColor: Color.Thistle
                )},
                { EnumBeepThemes.MidnightTheme, (
                    CompanyPopoverBackgroundColor: Color.MidnightBlue,
                    CompanyTitleColor: Color.White,
                    CompanySubtitleColor: Color.LightSteelBlue,
                    CompanyDescriptionColor: Color.SlateGray,
                    CompanyLinkColor: Color.DodgerBlue,
                    CompanyButtonBackgroundColor: Color.MidnightBlue,
                    CompanyButtonTextColor: Color.White,
                    CompanyDropdownBackgroundColor: Color.DarkBlue,
                    CompanyDropdownTextColor: Color.LightCyan,
                    CompanyLogoBackgroundColor: Color.Navy
                )},
                { EnumBeepThemes.SpringTheme, (
                    CompanyPopoverBackgroundColor: Color.PaleGreen,
                    CompanyTitleColor: Color.ForestGreen,
                    CompanySubtitleColor: Color.DarkGreen,
                    CompanyDescriptionColor: Color.SeaGreen,
                    CompanyLinkColor: Color.LimeGreen,
                    CompanyButtonBackgroundColor: Color.MediumSpringGreen,
                    CompanyButtonTextColor: Color.White,
                    CompanyDropdownBackgroundColor: Color.PaleGreen,
                    CompanyDropdownTextColor: Color.DarkGreen,
                    CompanyLogoBackgroundColor: Color.Chartreuse
                )},
                { EnumBeepThemes.ForestTheme, (
                    CompanyPopoverBackgroundColor: Color.DarkOliveGreen,
                    CompanyTitleColor: Color.LightGreen,
                    CompanySubtitleColor: Color.OliveDrab,
                    CompanyDescriptionColor: Color.ForestGreen,
                    CompanyLinkColor: Color.MediumSeaGreen,
                    CompanyButtonBackgroundColor: Color.DarkSeaGreen,
                    CompanyButtonTextColor: Color.White,
                    CompanyDropdownBackgroundColor: Color.MediumSeaGreen,
                    CompanyDropdownTextColor: Color.White,
                    CompanyLogoBackgroundColor: Color.SeaGreen
                )},
                { EnumBeepThemes.NeonTheme, (
                    CompanyPopoverBackgroundColor: Color.Black,
                    CompanyTitleColor: Color.Lime,
                    CompanySubtitleColor: Color.Fuchsia,
                    CompanyDescriptionColor: Color.Cyan,
                    CompanyLinkColor: Color.Yellow,
                    CompanyButtonBackgroundColor: Color.Black,
                    CompanyButtonTextColor: Color.Lime,
                    CompanyDropdownBackgroundColor: Color.DarkSlateGray,
                    CompanyDropdownTextColor: Color.White,
                    CompanyLogoBackgroundColor: Color.Magenta
                )},
                { EnumBeepThemes.RusticTheme, (
                    CompanyPopoverBackgroundColor: Color.SaddleBrown,
                    CompanyTitleColor: Color.BurlyWood,
                    CompanySubtitleColor: Color.Tan,
                    CompanyDescriptionColor: Color.Sienna,
                    CompanyLinkColor: Color.Peru,
                    CompanyButtonBackgroundColor: Color.SaddleBrown,
                    CompanyButtonTextColor: Color.White,
                    CompanyDropdownBackgroundColor: Color.Tan,
                    CompanyDropdownTextColor: Color.DarkSlateGray,
                    CompanyLogoBackgroundColor: Color.Peru
                )},
                { EnumBeepThemes.GalaxyTheme, (
                    CompanyPopoverBackgroundColor: Color.DarkSlateBlue,
                    CompanyTitleColor: Color.White,
                    CompanySubtitleColor: Color.MediumPurple,
                    CompanyDescriptionColor: Color.SlateBlue,
                    CompanyLinkColor: Color.MediumOrchid,
                    CompanyButtonBackgroundColor: Color.DarkSlateBlue,
                    CompanyButtonTextColor: Color.White,
                    CompanyDropdownBackgroundColor: Color.SlateBlue,
                    CompanyDropdownTextColor: Color.White,
                    CompanyLogoBackgroundColor: Color.MediumPurple
                )},
                { EnumBeepThemes.DesertTheme, (
                    CompanyPopoverBackgroundColor: Color.SandyBrown,
                    CompanyTitleColor: Color.Sienna,
                    CompanySubtitleColor: Color.Peru,
                    CompanyDescriptionColor: Color.Tan,
                    CompanyLinkColor: Color.DarkOrange,
                    CompanyButtonBackgroundColor: Color.SandyBrown,
                    CompanyButtonTextColor: Color.Black,
                    CompanyDropdownBackgroundColor: Color.Bisque,
                    CompanyDropdownTextColor: Color.Brown,
                    CompanyLogoBackgroundColor: Color.DarkOrange
                )},
                { EnumBeepThemes.VintageTheme, (
                    CompanyPopoverBackgroundColor: Color.Bisque,
                    CompanyTitleColor: Color.Sienna,
                    CompanySubtitleColor: Color.Brown,
                    CompanyDescriptionColor: Color.Peru,
                    CompanyLinkColor: Color.DarkRed,
                    CompanyButtonBackgroundColor: Color.Bisque,
                    CompanyButtonTextColor: Color.DarkSlateGray,
                    CompanyDropdownBackgroundColor: Color.AntiqueWhite,
                    CompanyDropdownTextColor: Color.DarkRed,
                    CompanyLogoBackgroundColor: Color.Tan
                )},
                { EnumBeepThemes.ModernDarkTheme, (
                    CompanyPopoverBackgroundColor: Color.DarkSlateGray,
                    CompanyTitleColor: Color.White,
                    CompanySubtitleColor: Color.LightGray,
                    CompanyDescriptionColor: Color.Silver,
                    CompanyLinkColor: Color.Cyan,
                    CompanyButtonBackgroundColor: Color.DarkSlateGray,
                    CompanyButtonTextColor: Color.White,
                    CompanyDropdownBackgroundColor: Color.Black,
                    CompanyDropdownTextColor: Color.LightGray,
                    CompanyLogoBackgroundColor: Color.SlateGray
                )},
                { EnumBeepThemes.MaterialDesignTheme, (
                    CompanyPopoverBackgroundColor: Color.LightSlateGray,
                    CompanyTitleColor: Color.CornflowerBlue,
                    CompanySubtitleColor: Color.SteelBlue,
                    CompanyDescriptionColor: Color.DarkSlateGray,
                    CompanyLinkColor: Color.RoyalBlue,
                    CompanyButtonBackgroundColor: Color.LightSlateGray,
                    CompanyButtonTextColor: Color.White,
                    CompanyDropdownBackgroundColor: Color.DarkSlateGray,
                    CompanyDropdownTextColor: Color.LightBlue,
                    CompanyLogoBackgroundColor: Color.RoyalBlue
                )},
                { EnumBeepThemes.NeumorphismTheme, (
                    CompanyPopoverBackgroundColor: Color.WhiteSmoke,
                    CompanyTitleColor: Color.DarkGray,
                    CompanySubtitleColor: Color.SlateGray,
                    CompanyDescriptionColor: Color.Gainsboro,
                    CompanyLinkColor: Color.Silver,
                    CompanyButtonBackgroundColor: Color.WhiteSmoke,
                    CompanyButtonTextColor: Color.Gray,
                    CompanyDropdownBackgroundColor: Color.White,
                    CompanyDropdownTextColor: Color.DarkSlateGray,
                    CompanyLogoBackgroundColor: Color.Silver
                )},
                { EnumBeepThemes.GlassmorphismTheme, (
                    CompanyPopoverBackgroundColor: Color.DarkSlateGray,
                    CompanyTitleColor: Color.White,
                    CompanySubtitleColor: Color.LightSteelBlue,
                    CompanyDescriptionColor: Color.SteelBlue,
                    CompanyLinkColor: Color.DodgerBlue,
                    CompanyButtonBackgroundColor: Color.DarkSlateGray,
                    CompanyButtonTextColor: Color.White,
                    CompanyDropdownBackgroundColor: Color.Black,
                    CompanyDropdownTextColor: Color.LightCyan,
                    CompanyLogoBackgroundColor: Color.DodgerBlue
                )},
                { EnumBeepThemes.FlatDesignTheme, (
                    CompanyPopoverBackgroundColor: Color.Gainsboro,
                    CompanyTitleColor: Color.DarkGray,
                    CompanySubtitleColor: Color.SlateGray,
                    CompanyDescriptionColor: Color.Gray,
                    CompanyLinkColor: Color.Silver,
                    CompanyButtonBackgroundColor: Color.Gainsboro,
                    CompanyButtonTextColor: Color.Black,
                    CompanyDropdownBackgroundColor: Color.WhiteSmoke,
                    CompanyDropdownTextColor: Color.DarkGray,
                    CompanyLogoBackgroundColor: Color.Silver
                )},
                { EnumBeepThemes.CyberpunkNeonTheme, (
                    CompanyPopoverBackgroundColor: Color.Black,
                    CompanyTitleColor: Color.Fuchsia,
                    CompanySubtitleColor: Color.Aqua,
                    CompanyDescriptionColor: Color.Magenta,
                    CompanyLinkColor: Color.Lime,
                    CompanyButtonBackgroundColor: Color.Black,
                    CompanyButtonTextColor: Color.Lime,
                    CompanyDropdownBackgroundColor: Color.DarkSlateGray,
                    CompanyDropdownTextColor: Color.Yellow,
                    CompanyLogoBackgroundColor: Color.HotPink
                )},
                { EnumBeepThemes.GradientBurstTheme, (
                    CompanyPopoverBackgroundColor: Color.DarkMagenta,
                    CompanyTitleColor: Color.OrangeRed,
                    CompanySubtitleColor: Color.DeepSkyBlue,
                    CompanyDescriptionColor: Color.Crimson,
                    CompanyLinkColor: Color.DodgerBlue,
                    CompanyButtonBackgroundColor: Color.DarkMagenta,
                    CompanyButtonTextColor: Color.White,
                    CompanyDropdownBackgroundColor: Color.Purple,
                    CompanyDropdownTextColor: Color.Yellow,
                    CompanyLogoBackgroundColor: Color.DodgerBlue
                )},
                { EnumBeepThemes.HighContrastTheme, (
                    CompanyPopoverBackgroundColor: Color.Black,
                    CompanyTitleColor: Color.White,
                    CompanySubtitleColor: Color.Yellow,
                    CompanyDescriptionColor: Color.LightGray,
                    CompanyLinkColor: Color.Cyan,
                    CompanyButtonBackgroundColor: Color.Black,
                    CompanyButtonTextColor: Color.Yellow,
                    CompanyDropdownBackgroundColor: Color.DarkGray,
                    CompanyDropdownTextColor: Color.White,
                    CompanyLogoBackgroundColor: Color.Silver
                )},
                { EnumBeepThemes.MonochromeTheme, (
                    CompanyPopoverBackgroundColor: Color.Black,
                    CompanyTitleColor: Color.White,
                    CompanySubtitleColor: Color.LightGray,
                    CompanyDescriptionColor: Color.Silver,
                    CompanyLinkColor: Color.Gray,
                    CompanyButtonBackgroundColor: Color.Black,
                    CompanyButtonTextColor: Color.White,
                    CompanyDropdownBackgroundColor: Color.SlateGray,
                    CompanyDropdownTextColor: Color.Silver,
                    CompanyLogoBackgroundColor: Color.DimGray
                )},
                { EnumBeepThemes.LuxuryGoldTheme, (
                    CompanyPopoverBackgroundColor: Color.DarkSlateGray,
                    CompanyTitleColor: Color.Gold,
                    CompanySubtitleColor: Color.LightGoldenrodYellow,
                    CompanyDescriptionColor: Color.Goldenrod,
                    CompanyLinkColor: Color.Yellow,
                    CompanyButtonBackgroundColor: Color.DarkSlateGray,
                    CompanyButtonTextColor: Color.Gold,
                    CompanyDropdownBackgroundColor: Color.Black,
                    CompanyDropdownTextColor: Color.Gold,
                    CompanyLogoBackgroundColor: Color.DarkGoldenrod
                )},
                { EnumBeepThemes.SunsetTheme, (
                    CompanyPopoverBackgroundColor: Color.OrangeRed,
                    CompanyTitleColor: Color.Crimson,
                    CompanySubtitleColor: Color.DarkOrange,
                    CompanyDescriptionColor: Color.Orange,
                    CompanyLinkColor: Color.Coral,
                    CompanyButtonBackgroundColor: Color.OrangeRed,
                    CompanyButtonTextColor: Color.White,
                    CompanyDropdownBackgroundColor: Color.Tomato,
                    CompanyDropdownTextColor: Color.DarkRed,
                    CompanyLogoBackgroundColor: Color.Coral
                )},
                { EnumBeepThemes.AutumnTheme, (
                    CompanyPopoverBackgroundColor: Color.DarkOrange,
                    CompanyTitleColor: Color.Brown,
                    CompanySubtitleColor: Color.Orange,
                    CompanyDescriptionColor: Color.Peru,
                    CompanyLinkColor: Color.Sienna,
                    CompanyButtonBackgroundColor: Color.DarkOrange,
                    CompanyButtonTextColor: Color.White,
                    CompanyDropdownBackgroundColor: Color.Chocolate,
                    CompanyDropdownTextColor: Color.Tan,
                    CompanyLogoBackgroundColor: Color.Sienna
                )},
                { EnumBeepThemes.EarthyTheme, (
                    CompanyPopoverBackgroundColor: Color.Tan,
                    CompanyTitleColor: Color.SaddleBrown,
                    CompanySubtitleColor: Color.Peru,
                    CompanyDescriptionColor: Color.Sienna,
                    CompanyLinkColor: Color.DarkOliveGreen,
                    CompanyButtonBackgroundColor: Color.Tan,
                    CompanyButtonTextColor: Color.Chocolate,
                    CompanyDropdownBackgroundColor: Color.Wheat,
                    CompanyDropdownTextColor: Color.DarkSlateGray,
                    CompanyLogoBackgroundColor: Color.DarkOliveGreen
                )}
               };
    
        public static Dictionary<EnumBeepThemes, (Color TestimonialBackColor, Color TestimonialTextColor, Color TestimonialNameColor, Color TestimonialDetailsColor, Color TestimonialDateColor, Color TestimonialRatingColor, Color TestimonialStatusColor)> ThemeTestimonialColors =
    new Dictionary<EnumBeepThemes, (Color, Color, Color, Color, Color, Color, Color)>
    {
        { EnumBeepThemes.DefaultTheme, (Color.White, Color.Black, Color.DarkBlue, Color.Gray, Color.DarkGray, Color.Gold, Color.Green) },
        { EnumBeepThemes.WinterTheme, (Color.WhiteSmoke, Color.SteelBlue, Color.Navy, Color.LightBlue, Color.CadetBlue, Color.Gold, Color.Teal) },
        { EnumBeepThemes.CandyTheme, (Color.MistyRose, Color.DeepPink, Color.MediumVioletRed, Color.Pink, Color.HotPink, Color.Gold, Color.Magenta) },
        { EnumBeepThemes.ZenTheme, (Color.Beige, Color.DarkSlateGray, Color.SeaGreen, Color.LightGray, Color.DarkSeaGreen, Color.Gold, Color.Olive) },
        { EnumBeepThemes.RetroTheme, (Color.Bisque, Color.Brown, Color.SaddleBrown, Color.DarkGoldenrod, Color.Chocolate, Color.Gold, Color.DarkRed) },
        { EnumBeepThemes.RoyalTheme, (Color.MidnightBlue, Color.LightGoldenrodYellow, Color.Gold, Color.DarkSlateBlue, Color.DarkGoldenrod, Color.Gold, Color.DarkBlue) },
        { EnumBeepThemes.HighlightTheme, (Color.Yellow, Color.OrangeRed, Color.Red, Color.DarkOrange, Color.Orange, Color.Gold, Color.OrangeRed) },
        { EnumBeepThemes.DarkTheme, (Color.Black, Color.LightGray, Color.White, Color.SlateGray, Color.Gray, Color.Gold, Color.DarkRed) },
        { EnumBeepThemes.OceanTheme, (Color.DarkCyan, Color.White, Color.Aqua, Color.Teal, Color.LightSeaGreen, Color.Gold, Color.Cyan) },
        { EnumBeepThemes.LightTheme, (Color.White, Color.Black, Color.DarkBlue, Color.Gray, Color.LightGray, Color.Gold, Color.DarkGreen) },
        { EnumBeepThemes.PastelTheme, (Color.LavenderBlush, Color.MediumOrchid, Color.DarkOrchid, Color.Thistle, Color.Plum, Color.Gold, Color.Purple) },
        { EnumBeepThemes.MidnightTheme, (Color.Black, Color.MidnightBlue, Color.White, Color.DarkSlateGray, Color.DarkBlue, Color.Gold, Color.SteelBlue) },
        { EnumBeepThemes.SpringTheme, (Color.MediumSpringGreen, Color.ForestGreen, Color.LimeGreen, Color.SeaGreen, Color.Chartreuse, Color.Gold, Color.Green) },
        { EnumBeepThemes.ForestTheme, (Color.DarkOliveGreen, Color.LightGreen, Color.OliveDrab, Color.ForestGreen, Color.MediumSeaGreen, Color.Gold, Color.DarkGreen) },
        { EnumBeepThemes.NeonTheme, (Color.Black, Color.Lime, Color.HotPink, Color.DeepPink, Color.YellowGreen, Color.Gold, Color.Cyan) },
        { EnumBeepThemes.RusticTheme, (Color.SaddleBrown, Color.Chocolate, Color.Tan, Color.Sienna, Color.DarkRed, Color.Gold, Color.Maroon) },
        { EnumBeepThemes.GalaxyTheme, (Color.DarkSlateBlue, Color.Indigo, Color.MidnightBlue, Color.DarkViolet, Color.MediumPurple, Color.Gold, Color.DeepSkyBlue) },
        { EnumBeepThemes.DesertTheme, (Color.SandyBrown, Color.Peru, Color.Sienna, Color.Tan, Color.Chocolate, Color.Gold, Color.Brown) },
        { EnumBeepThemes.VintageTheme, (Color.Bisque, Color.Sienna, Color.Brown, Color.Peru, Color.DarkSlateGray, Color.Gold, Color.DarkRed) },
        { EnumBeepThemes.ModernDarkTheme, (Color.DarkSlateGray, Color.Gray, Color.White, Color.LightSlateGray, Color.SlateGray, Color.Gold, Color.Black) },
        { EnumBeepThemes.MaterialDesignTheme, (Color.LightSlateGray, Color.CornflowerBlue, Color.SlateGray, Color.SteelBlue, Color.RoyalBlue, Color.Gold, Color.DodgerBlue) },
        { EnumBeepThemes.NeumorphismTheme, (Color.WhiteSmoke, Color.DarkGray, Color.LightGray, Color.Gainsboro, Color.Silver, Color.Gold, Color.DimGray) },
        { EnumBeepThemes.GlassmorphismTheme, (Color.Black, Color.LightSteelBlue, Color.DarkSlateGray, Color.SteelBlue, Color.DodgerBlue, Color.Gold, Color.White) },
        { EnumBeepThemes.FlatDesignTheme, (Color.Gainsboro, Color.Gray, Color.White, Color.LightGray, Color.Silver, Color.Gold, Color.DarkGray) },
        { EnumBeepThemes.CyberpunkNeonTheme, (Color.Black, Color.Fuchsia, Color.Aqua, Color.Magenta, Color.Purple, Color.Gold, Color.Lime) },
        { EnumBeepThemes.GradientBurstTheme, (Color.DarkMagenta, Color.DeepSkyBlue, Color.OrangeRed, Color.Crimson, Color.DodgerBlue, Color.Gold, Color.MediumPurple) },
        { EnumBeepThemes.HighContrastTheme, (Color.Black, Color.White, Color.Gray, Color.LightGray, Color.Silver, Color.Gold, Color.Yellow) },
        { EnumBeepThemes.MonochromeTheme, (Color.Black, Color.DarkGray, Color.LightGray, Color.Gray, Color.SlateGray, Color.Gold, Color.White) },
        { EnumBeepThemes.LuxuryGoldTheme, (Color.DarkSlateGray, Color.Gold, Color.DarkGoldenrod, Color.LightGoldenrodYellow, Color.Goldenrod, Color.Gold, Color.DarkGray) },
        { EnumBeepThemes.SunsetTheme, (Color.OrangeRed, Color.Crimson, Color.DarkOrange, Color.Orange, Color.Red, Color.Gold, Color.Salmon) },
        { EnumBeepThemes.AutumnTheme, (Color.DarkOrange, Color.Brown, Color.Orange, Color.Peru, Color.Sienna, Color.Gold, Color.DarkRed) },
        { EnumBeepThemes.EarthyTheme, (Color.Tan, Color.SaddleBrown, Color.Peru, Color.Sienna, Color.Chocolate, Color.Gold, Color.OliveDrab) }
    };

        public static Dictionary<EnumBeepThemes, (
                Color ChartBackColor,
                Color ChartLineColor,
                Color ChartFillColor,
                Color ChartAxisColor,
                Color ChartTitleColor,
                Color ChartTextColor,
                Color ChartLegendBackColor,
                Color ChartLegendTextColor,
                Color ChartLegendShapeColor,
                Color ChartGridLineColor,
                List<Color> ChartDefaultSeriesColors
            )> ThemeChartColors = new Dictionary<EnumBeepThemes, (Color, Color, Color, Color, Color, Color, Color, Color, Color, Color, List<Color>)>
            {
            { EnumBeepThemes.DefaultTheme, (
                ChartBackColor: Color.White,
                ChartLineColor: Color.Blue,
                ChartFillColor: Color.LightBlue,
                ChartAxisColor: Color.Black,
                ChartTitleColor: Color.Black,
                ChartTextColor: Color.Black,
                ChartLegendBackColor: Color.LightGray,
                ChartLegendTextColor: Color.Black,
                ChartLegendShapeColor: Color.Gray,
                ChartGridLineColor: Color.LightGray,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.Blue, Color.Red, Color.Green, Color.Purple, Color.Orange,
                    Color.Yellow, Color.Cyan, Color.Magenta, Color.Brown, Color.Pink,
                    Color.Teal, Color.Lime
                }
            ) },
            { EnumBeepThemes.WinterTheme, (
                ChartBackColor: Color.WhiteSmoke,
                ChartLineColor: Color.SteelBlue,
                ChartFillColor: Color.LightSteelBlue,
                ChartAxisColor: Color.DarkSlateGray,
                ChartTitleColor: Color.MidnightBlue,
                ChartTextColor: Color.SlateGray,
                ChartLegendBackColor: Color.LightBlue,
                ChartLegendTextColor: Color.DarkBlue,
                ChartLegendShapeColor: Color.CadetBlue,
                ChartGridLineColor: Color.Gainsboro,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.SteelBlue, Color.LightSkyBlue, Color.Teal, Color.PowderBlue, Color.Cyan,
                    Color.SlateBlue, Color.Aqua, Color.DodgerBlue, Color.LightSteelBlue, Color.MediumTurquoise,
                    Color.Navy, Color.AliceBlue
                }
            ) },
            { EnumBeepThemes.CandyTheme, (
                ChartBackColor: Color.Pink,
                ChartLineColor: Color.DeepPink,
                ChartFillColor: Color.LightPink,
                ChartAxisColor: Color.HotPink,
                ChartTitleColor: Color.MediumVioletRed,
                ChartTextColor: Color.DarkMagenta,
                ChartLegendBackColor: Color.MistyRose,
                ChartLegendTextColor: Color.Purple,
                ChartLegendShapeColor: Color.Orchid,
                ChartGridLineColor: Color.Thistle,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.DeepPink, Color.HotPink, Color.Magenta, Color.Plum, Color.Violet,
                    Color.Fuchsia, Color.PaleVioletRed, Color.MediumOrchid, Color.Lavender, Color.Crimson,
                    Color.FromArgb(255, 152, 152), // Rose
                    Color.Salmon
                }
            ) },
            { EnumBeepThemes.ZenTheme, (
                ChartBackColor: Color.Beige,
                ChartLineColor: Color.MediumSeaGreen,
                ChartFillColor: Color.LightGreen,
                ChartAxisColor: Color.DarkOliveGreen,
                ChartTitleColor: Color.ForestGreen,
                ChartTextColor: Color.SeaGreen,
                ChartLegendBackColor: Color.LightGray,
                ChartLegendTextColor: Color.DarkGreen,
                ChartLegendShapeColor: Color.Olive,
                ChartGridLineColor: Color.PaleGreen,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.MediumSeaGreen, Color.LimeGreen, Color.OliveDrab, Color.SpringGreen, Color.Teal,
                    Color.DarkGreen, Color.Chartreuse, Color.SeaGreen, Color.MintCream, Color.ForestGreen,
                    Color.FromArgb(188, 184, 138), // Sage
                    Color.FromArgb(138, 154, 91)   // MossGreen
                }
            ) },
            { EnumBeepThemes.RetroTheme, (
                ChartBackColor: Color.Bisque,
                ChartLineColor: Color.DarkGoldenrod,
                ChartFillColor: Color.LightGoldenrodYellow,
                ChartAxisColor: Color.SaddleBrown,
                ChartTitleColor: Color.Chocolate,
                ChartTextColor: Color.Peru,
                ChartLegendBackColor: Color.AntiqueWhite,
                ChartLegendTextColor: Color.Brown,
                ChartLegendShapeColor: Color.Goldenrod,
                ChartGridLineColor: Color.Tan,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.DarkGoldenrod, Color.Goldenrod, Color.Peru, Color.Sienna, Color.Orange,
                    Color.BurlyWood, Color.Chocolate, Color.Tan, Color.Wheat, Color.SandyBrown,
                    Color.NavajoWhite, Color.Coral
                }
            ) },
            { EnumBeepThemes.RoyalTheme, (
                ChartBackColor: Color.MidnightBlue,
                ChartLineColor: Color.Gold,
                ChartFillColor: Color.LightGoldenrodYellow,
                ChartAxisColor: Color.DarkSlateBlue,
                ChartTitleColor: Color.Goldenrod,
                ChartTextColor: Color.White,
                ChartLegendBackColor: Color.DarkBlue,
                ChartLegendTextColor: Color.LightYellow,
                ChartLegendShapeColor: Color.RoyalBlue,
                ChartGridLineColor: Color.SlateBlue,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.Gold, Color.RoyalBlue, Color.Purple, Color.Crimson, Color.Indigo,
                    Color.Violet, Color.DarkOrchid, Color.MediumPurple, Color.LightGoldenrodYellow, Color.Plum,
                    Color.DarkViolet, Color.MediumSlateBlue
                }
            ) },
            { EnumBeepThemes.HighlightTheme, (
                ChartBackColor: Color.Yellow,
                ChartLineColor: Color.OrangeRed,
                ChartFillColor: Color.LightYellow,
                ChartAxisColor: Color.DarkOrange,
                ChartTitleColor: Color.Red,
                ChartTextColor: Color.Black,
                ChartLegendBackColor: Color.Gold,
                ChartLegendTextColor: Color.DarkRed,
                ChartLegendShapeColor: Color.Orange,
                ChartGridLineColor: Color.LightGoldenrodYellow,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.OrangeRed, Color.Yellow, Color.Red, Color.Orange, Color.Crimson,
                    Color.Gold, Color.DarkRed, Color.Coral, Color.Tomato, Color.Firebrick,
                    Color.Salmon, Color.IndianRed
                }
            ) },
            { EnumBeepThemes.DarkTheme, (
                ChartBackColor: Color.Black,
                ChartLineColor: Color.Gray,
                ChartFillColor: Color.DimGray,
                ChartAxisColor: Color.DarkGray,
                ChartTitleColor: Color.White,
                ChartTextColor: Color.LightGray,
                ChartLegendBackColor: Color.SlateGray,
                ChartLegendTextColor: Color.White,
                ChartLegendShapeColor: Color.Silver,
                ChartGridLineColor: Color.DarkSlateGray,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.Gray, Color.LightGray, Color.Silver, Color.DarkGray, Color.White,
                    Color.SlateGray, Color.LightSlateGray, Color.DimGray, Color.Gainsboro, Color.Black,
                    Color.FromArgb(128, 128, 128), // MediumGray
                    Color.Snow
                }
            ) },
            { EnumBeepThemes.OceanTheme, (
                ChartBackColor: Color.DarkCyan,
                ChartLineColor: Color.Teal,
                ChartFillColor: Color.LightSeaGreen,
                ChartAxisColor: Color.DarkSlateGray,
                ChartTitleColor: Color.Aqua,
                ChartTextColor: Color.White,
                ChartLegendBackColor: Color.SeaGreen,
                ChartLegendTextColor: Color.LightCyan,
                ChartLegendShapeColor: Color.Turquoise,
                ChartGridLineColor: Color.MediumAquamarine,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.Teal, Color.Aqua, Color.Cyan, Color.Turquoise, Color.SeaGreen,
                    Color.DarkTurquoise, Color.LightSeaGreen, Color.MediumTurquoise, Color.Aquamarine, Color.CadetBlue,
                    Color.PaleTurquoise, Color.SkyBlue
                }
            ) },
            { EnumBeepThemes.LightTheme, (
                ChartBackColor: Color.White,
                ChartLineColor: Color.LightGray,
                ChartFillColor: Color.Gainsboro,
                ChartAxisColor: Color.Gray,
                ChartTitleColor: Color.Black,
                ChartTextColor: Color.DarkGray,
                ChartLegendBackColor: Color.WhiteSmoke,
                ChartLegendTextColor: Color.Black,
                ChartLegendShapeColor: Color.Silver,
                ChartGridLineColor: Color.LightGray,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.LightGray, Color.Silver, Color.Gray, Color.DarkGray, Color.Black,
                    Color.SlateGray, Color.LightSlateGray, Color.Gainsboro, Color.WhiteSmoke, Color.Snow,
                    Color.AliceBlue, Color.Lavender
                }
            ) },
            { EnumBeepThemes.PastelTheme, (
                ChartBackColor: Color.LavenderBlush,
                ChartLineColor: Color.Plum,
                ChartFillColor: Color.Thistle,
                ChartAxisColor: Color.MediumOrchid,
                ChartTitleColor: Color.DarkOrchid,
                ChartTextColor: Color.SlateGray,
                ChartLegendBackColor: Color.Lavender,
                ChartLegendTextColor: Color.DarkSlateGray,
                ChartLegendShapeColor: Color.Violet,
                ChartGridLineColor: Color.MistyRose,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.Plum, Color.Lavender, Color.Pink, Color.Violet, Color.Orchid,
                    Color.PaleVioletRed, Color.MediumPurple, Color.Thistle, Color.LightPink, Color.FromArgb(224, 176, 255), // Mauve
                    Color.LavenderBlush, Color.Bisque
                }
            ) },
            { EnumBeepThemes.MidnightTheme, (
                ChartBackColor: Color.Black,
                ChartLineColor: Color.MidnightBlue,
                ChartFillColor: Color.DarkSlateGray,
                ChartAxisColor: Color.SteelBlue,
                ChartTitleColor: Color.White,
                ChartTextColor: Color.LightSteelBlue,
                ChartLegendBackColor: Color.DarkBlue,
                ChartLegendTextColor: Color.White,
                ChartLegendShapeColor: Color.DodgerBlue,
                ChartGridLineColor: Color.SlateGray,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.MidnightBlue, Color.SteelBlue, Color.DodgerBlue, Color.Navy, Color.CornflowerBlue,
                    Color.RoyalBlue, Color.DarkSlateBlue, Color.SlateBlue, Color.LightSlateGray, Color.MediumSlateBlue,
                    Color.Indigo, Color.BlueViolet
                }
            ) },
            { EnumBeepThemes.SpringTheme, (
                ChartBackColor: Color.MediumSpringGreen,
                ChartLineColor: Color.Chartreuse,
                ChartFillColor: Color.LightGreen,
                ChartAxisColor: Color.SeaGreen,
                ChartTitleColor: Color.ForestGreen,
                ChartTextColor: Color.DarkGreen,
                ChartLegendBackColor: Color.PaleGreen,
                ChartLegendTextColor: Color.Green,
                ChartLegendShapeColor: Color.LimeGreen,
                ChartGridLineColor: Color.Honeydew,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.Chartreuse, Color.LimeGreen, Color.SpringGreen, Color.MediumSeaGreen, Color.Teal,
                    Color.DarkGreen, Color.OliveDrab, Color.SeaGreen, Color.LawnGreen, Color.MintCream,
                    Color.LightGreen, Color.YellowGreen
                }
            ) },
            { EnumBeepThemes.ForestTheme, (
                ChartBackColor: Color.DarkOliveGreen,
                ChartLineColor: Color.ForestGreen,
                ChartFillColor: Color.OliveDrab,
                ChartAxisColor: Color.DarkGreen,
                ChartTitleColor: Color.SeaGreen,
                ChartTextColor: Color.LightGreen,
                ChartLegendBackColor: Color.DarkSeaGreen,
                ChartLegendTextColor: Color.White,
                ChartLegendShapeColor: Color.MediumSeaGreen,
                ChartGridLineColor: Color.PaleGreen,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.ForestGreen, Color.OliveDrab, Color.DarkGreen, Color.SeaGreen, Color.MediumSeaGreen,
                    Color.DarkOliveGreen, Color.Olive, Color.LimeGreen, Color.GreenYellow, Color.Chartreuse,
                    Color.FromArgb(188, 184, 138), // Sage
                    Color.FromArgb(138, 154, 91)   // MossGreen
                }
            ) },
            { EnumBeepThemes.NeonTheme, (
                ChartBackColor: Color.Black,
                ChartLineColor: Color.Lime,
                ChartFillColor: Color.Fuchsia,
                ChartAxisColor: Color.DeepPink,
                ChartTitleColor: Color.Yellow,
                ChartTextColor: Color.Cyan,
                ChartLegendBackColor: Color.DarkSlateGray,
                ChartLegendTextColor: Color.White,
                ChartLegendShapeColor: Color.HotPink,
                ChartGridLineColor: Color.DimGray,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.Lime, Color.Fuchsia, Color.Yellow, Color.Cyan, Color.HotPink,
                    Color.FromArgb(125, 249, 255), // ElectricBlue
                    Color.FromArgb(57, 255, 20),   // NeonGreen
                    Color.Magenta, Color.OrangeRed, Color.FromArgb(159, 0, 255), // VividPurple
                    Color.FromArgb(252, 15, 192),  // ShockingPink
                    Color.Turquoise
                }
            ) },
            { EnumBeepThemes.RusticTheme, (
                ChartBackColor: Color.SaddleBrown,
                ChartLineColor: Color.Chocolate,
                ChartFillColor: Color.Tan,
                ChartAxisColor: Color.DarkRed,
                ChartTitleColor: Color.Sienna,
                ChartTextColor: Color.BurlyWood,
                ChartLegendBackColor: Color.Peru,
                ChartLegendTextColor: Color.DarkSlateGray,
                ChartLegendShapeColor: Color.Brown,
                ChartGridLineColor: Color.Wheat,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.Chocolate, Color.Tan, Color.Sienna, Color.Brown, Color.DarkRed,
                    Color.Peru, Color.SaddleBrown, Color.Coral, Color.IndianRed, Color.RosyBrown,
                    Color.Moccasin, Color.Bisque
                }
            ) },
            { EnumBeepThemes.GalaxyTheme, (
                ChartBackColor: Color.DarkSlateBlue,
                ChartLineColor: Color.Indigo,
                ChartFillColor: Color.MidnightBlue,
                ChartAxisColor: Color.DarkViolet,
                ChartTitleColor: Color.MediumPurple,
                ChartTextColor: Color.LightSteelBlue,
                ChartLegendBackColor: Color.SlateBlue,
                ChartLegendTextColor: Color.White,
                ChartLegendShapeColor: Color.BlueViolet,
                ChartGridLineColor: Color.DarkSlateGray,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.Indigo, Color.MidnightBlue, Color.DarkViolet, Color.MediumPurple, Color.BlueViolet,
                    Color.Purple, Color.SlateBlue, Color.DarkOrchid, Color.RoyalBlue, Color.Violet,
                    Color.Plum, Color.Lavender
                }
            ) },
            { EnumBeepThemes.DesertTheme, (
                ChartBackColor: Color.SandyBrown,
                ChartLineColor: Color.Peru,
                ChartFillColor: Color.Tan,
                ChartAxisColor: Color.Sienna,
                ChartTitleColor: Color.Chocolate,
                ChartTextColor: Color.DarkGoldenrod,
                ChartLegendBackColor: Color.Bisque,
                ChartLegendTextColor: Color.Brown,
                ChartLegendShapeColor: Color.DarkOrange,
                ChartGridLineColor: Color.Wheat,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.Peru, Color.Tan, Color.Sienna, Color.Chocolate, Color.DarkOrange,
                    Color.SandyBrown, Color.Coral, Color.BurlyWood, Color.Goldenrod, Color.Orange,
                    Color.NavajoWhite, Color.PeachPuff
                }
            ) },
            { EnumBeepThemes.VintageTheme, (
                ChartBackColor: Color.Bisque,
                ChartLineColor: Color.Sienna,
                ChartFillColor: Color.Brown,
                ChartAxisColor: Color.DarkSlateGray,
                ChartTitleColor: Color.Peru,
                ChartTextColor: Color.Tan,
                ChartLegendBackColor: Color.AntiqueWhite,
                ChartLegendTextColor: Color.DarkRed,
                ChartLegendShapeColor: Color.Chocolate,
                ChartGridLineColor: Color.Wheat,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.Sienna, Color.Brown, Color.Peru, Color.Chocolate, Color.DarkRed,
                    Color.Tan, Color.Coral, Color.RosyBrown, Color.IndianRed, Color.SaddleBrown,
                    Color.BurlyWood, Color.Moccasin
                }
            ) },
            { EnumBeepThemes.ModernDarkTheme, (
                ChartBackColor: Color.DarkSlateGray,
                ChartLineColor: Color.Gray,
                ChartFillColor: Color.DimGray,
                ChartAxisColor: Color.SlateGray,
                ChartTitleColor: Color.White,
                ChartTextColor: Color.LightGray,
                ChartLegendBackColor: Color.LightSlateGray,
                ChartLegendTextColor: Color.White,
                ChartLegendShapeColor: Color.Silver,
                ChartGridLineColor: Color.DarkGray,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.Gray, Color.DimGray, Color.SlateGray, Color.LightSlateGray, Color.Silver,
                    Color.DarkGray, Color.LightGray, Color.Gainsboro, Color.Black, Color.White,
                    Color.FromArgb(128, 128, 128), // MediumGray
                    Color.Snow
                }
            ) },
            { EnumBeepThemes.MaterialDesignTheme, (
                ChartBackColor: Color.LightSlateGray,
                ChartLineColor: Color.CornflowerBlue,
                ChartFillColor: Color.SlateGray,
                ChartAxisColor: Color.RoyalBlue,
                ChartTitleColor: Color.SteelBlue,
                ChartTextColor: Color.White,
                ChartLegendBackColor: Color.DarkSlateGray,
                ChartLegendTextColor: Color.LightBlue,
                ChartLegendShapeColor: Color.DodgerBlue,
                ChartGridLineColor: Color.Gainsboro,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.CornflowerBlue, Color.SlateGray, Color.RoyalBlue, Color.SteelBlue, Color.DodgerBlue,
                    Color.LightBlue, Color.MediumBlue, Color.SkyBlue, Color.LightSteelBlue, Color.Navy,
                    Color.Aqua, Color.Turquoise
                }
            ) },
            { EnumBeepThemes.NeumorphismTheme, (
                ChartBackColor: Color.WhiteSmoke,
                ChartLineColor: Color.LightGray,
                ChartFillColor: Color.Gainsboro,
                ChartAxisColor: Color.DarkGray,
                ChartTitleColor: Color.Silver,
                ChartTextColor: Color.Gray,
                ChartLegendBackColor: Color.White,
                ChartLegendTextColor: Color.DarkSlateGray,
                ChartLegendShapeColor: Color.LightSlateGray,
                ChartGridLineColor: Color.Snow,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.LightGray, Color.Gainsboro, Color.DarkGray, Color.Silver, Color.LightSlateGray,
                    Color.Gray, Color.SlateGray, Color.WhiteSmoke, Color.Snow, Color.Black,
                    Color.FromArgb(128, 128, 128), // MediumGray
                    Color.AliceBlue
                }
            ) },
            { EnumBeepThemes.GlassmorphismTheme, (
                ChartBackColor: Color.Black,
                ChartLineColor: Color.LightSteelBlue,
                ChartFillColor: Color.Transparent,
                ChartAxisColor: Color.SteelBlue,
                ChartTitleColor: Color.DodgerBlue,
                ChartTextColor: Color.White,
                ChartLegendBackColor: Color.FromArgb(50, Color.Gray),
                ChartLegendTextColor: Color.LightCyan,
                ChartLegendShapeColor: Color.Cyan,
                ChartGridLineColor: Color.DarkSlateGray,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.LightSteelBlue, Color.SteelBlue, Color.DodgerBlue, Color.Cyan, Color.Aqua,
                    Color.LightBlue, Color.SkyBlue, Color.Turquoise, Color.MediumTurquoise, Color.PaleTurquoise,
                    Color.Navy, Color.CornflowerBlue
                }
            ) },
            { EnumBeepThemes.FlatDesignTheme, (
                ChartBackColor: Color.Gainsboro,
                ChartLineColor: Color.Gray,
                ChartFillColor: Color.LightGray,
                ChartAxisColor: Color.DarkGray,
                ChartTitleColor: Color.Black,
                ChartTextColor: Color.SlateGray,
                ChartLegendBackColor: Color.WhiteSmoke,
                ChartLegendTextColor: Color.Black,
                ChartLegendShapeColor: Color.Silver,
                ChartGridLineColor: Color.Snow,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.Gray, Color.LightGray, Color.DarkGray, Color.Silver, Color.SlateGray,
                    Color.LightSlateGray, Color.Gainsboro, Color.Black, Color.WhiteSmoke, Color.Snow,
                    Color.FromArgb(128, 128, 128), // MediumGray
                    Color.Lavender
                }
            ) },
            { EnumBeepThemes.CyberpunkNeonTheme, (
                ChartBackColor: Color.Black,
                ChartLineColor: Color.Fuchsia,
                ChartFillColor: Color.Aqua,
                ChartAxisColor: Color.Magenta,
                ChartTitleColor: Color.Purple,
                ChartTextColor: Color.Lime,
                ChartLegendBackColor: Color.DarkSlateGray,
                ChartLegendTextColor: Color.Yellow,
                ChartLegendShapeColor: Color.HotPink,
                ChartGridLineColor: Color.FromArgb(128, 128, 128), // MediumGray
                ChartDefaultSeriesColors: new List<Color> {
                    Color.Fuchsia, Color.Aqua, Color.Magenta, Color.Purple, Color.Lime,
                    Color.Yellow, Color.Cyan, Color.HotPink, Color.FromArgb(125, 249, 255), // ElectricBlue
                    Color.FromArgb(57, 255, 20),   // NeonGreen
                    Color.FromArgb(159, 0, 255),   // VividPurple
                    Color.FromArgb(252, 15, 192)   // ShockingPink
                }
            ) },
            { EnumBeepThemes.GradientBurstTheme, (
                ChartBackColor: Color.DarkMagenta,
                ChartLineColor: Color.OrangeRed,
                ChartFillColor: Color.DeepSkyBlue,
                ChartAxisColor: Color.Crimson,
                ChartTitleColor: Color.DodgerBlue,
                ChartTextColor: Color.White,
                ChartLegendBackColor: Color.Purple,
                ChartLegendTextColor: Color.Yellow,
                ChartLegendShapeColor: Color.Orange,
                ChartGridLineColor: Color.DarkSlateBlue,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.OrangeRed, Color.DeepSkyBlue, Color.Crimson, Color.DodgerBlue, Color.Purple,
                    Color.Yellow, Color.Magenta, Color.Cyan, Color.Lime, Color.HotPink,
                    Color.Turquoise, Color.Violet
                }
            ) },
            { EnumBeepThemes.HighContrastTheme, (
                ChartBackColor: Color.Black,
                ChartLineColor: Color.White,
                ChartFillColor: Color.Gray,
                ChartAxisColor: Color.LightGray,
                ChartTitleColor: Color.Yellow,
                ChartTextColor: Color.White,
                ChartLegendBackColor: Color.DarkGray,
                ChartLegendTextColor: Color.White,
                ChartLegendShapeColor: Color.Silver,
                ChartGridLineColor: Color.SlateGray,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.White, Color.Gray, Color.LightGray, Color.Silver, Color.Yellow,
                    Color.Red, Color.Blue, Color.Green, Color.Purple, Color.Orange,
                    Color.Cyan, Color.Magenta
                }
            ) },
            { EnumBeepThemes.MonochromeTheme, (
                ChartBackColor: Color.Black,
                ChartLineColor: Color.DarkGray,
                ChartFillColor: Color.LightGray,
                ChartAxisColor: Color.Gray,
                ChartTitleColor: Color.White,
                ChartTextColor: Color.Silver,
                ChartLegendBackColor: Color.SlateGray,
                ChartLegendTextColor: Color.White,
                ChartLegendShapeColor: Color.LightSlateGray,
                ChartGridLineColor: Color.DimGray,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.DarkGray, Color.LightGray, Color.Gray, Color.Silver, Color.LightSlateGray,
                    Color.SlateGray, Color.Black, Color.White, Color.Gainsboro, Color.DimGray,
                    Color.FromArgb(128, 128, 128), // MediumGray
                    Color.Snow
                }
            ) },
            { EnumBeepThemes.LuxuryGoldTheme, (
                ChartBackColor: Color.DarkSlateGray,
                ChartLineColor: Color.Gold,
                ChartFillColor: Color.DarkGoldenrod,
                ChartAxisColor: Color.Goldenrod,
                ChartTitleColor: Color.LightGoldenrodYellow,
                ChartTextColor: Color.White,
                ChartLegendBackColor: Color.Black,
                ChartLegendTextColor: Color.Gold,
                ChartLegendShapeColor: Color.Yellow,
                ChartGridLineColor: Color.DimGray,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.Gold, Color.DarkGoldenrod, Color.Goldenrod, Color.LightGoldenrodYellow, Color.Yellow,
                    Color.Orange, Color.Coral, Color.Tan, Color.Wheat, Color.Bisque,
                    Color.NavajoWhite, Color.PeachPuff
                }
            ) },
            { EnumBeepThemes.SunsetTheme, (
                ChartBackColor: Color.OrangeRed,
                ChartLineColor: Color.Crimson,
                ChartFillColor: Color.DarkOrange,
                ChartAxisColor: Color.Red,
                ChartTitleColor: Color.Orange,
                ChartTextColor: Color.Yellow,
                ChartLegendBackColor: Color.Tomato,
                ChartLegendTextColor: Color.DarkRed,
                ChartLegendShapeColor: Color.Coral,
                ChartGridLineColor: Color.LightSalmon,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.Crimson, Color.DarkOrange, Color.Red, Color.Orange, Color.Yellow,
                    Color.Tomato, Color.Coral, Color.Salmon, Color.IndianRed, Color.Firebrick,
                    Color.LightCoral, Color.PeachPuff
                }
            ) },
            { EnumBeepThemes.AutumnTheme, (
                ChartBackColor: Color.DarkOrange,
                ChartLineColor: Color.Brown,
                ChartFillColor: Color.Orange,
                ChartAxisColor: Color.Sienna,
                ChartTitleColor: Color.Peru,
                ChartTextColor: Color.Tan,
                ChartLegendBackColor: Color.Chocolate,
                ChartLegendTextColor: Color.White,
                ChartLegendShapeColor: Color.DarkGoldenrod,
                ChartGridLineColor: Color.Wheat,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.Brown, Color.Orange, Color.Sienna, Color.Peru, Color.DarkGoldenrod,
                    Color.Chocolate, Color.Tan, Color.Coral, Color.IndianRed, Color.RosyBrown,
                    Color.BurlyWood, Color.SandyBrown
                }
            ) },
            { EnumBeepThemes.EarthyTheme, (
                ChartBackColor: Color.Tan,
                ChartLineColor: Color.SaddleBrown,
                ChartFillColor: Color.Peru,
                ChartAxisColor: Color.Sienna,
                ChartTitleColor: Color.Chocolate,
                ChartTextColor: Color.BurlyWood,
                ChartLegendBackColor: Color.Wheat,
                ChartLegendTextColor: Color.DarkSlateGray,
                ChartLegendShapeColor: Color.DarkOliveGreen,
                ChartGridLineColor: Color.Bisque,
                ChartDefaultSeriesColors: new List<Color> {
                    Color.SaddleBrown, Color.Peru, Color.Sienna, Color.Chocolate, Color.DarkOliveGreen,
                    Color.Tan, Color.BurlyWood, Color.OliveDrab, Color.SandyBrown, Color.Brown,
                    Color.Moccasin, Color.NavajoWhite
                }
            ) }
            };
  
   
    public static Dictionary<EnumBeepThemes, (Color ScrollbarBackColor, Color ScrollbarThumbColor, Color ScrollbarTrackColor, Color ScrollbarThumbColorHover, Color ScrollbarThumbColorActive)> ThemeScrollBarColors =
  new Dictionary<EnumBeepThemes, (Color, Color, Color, Color, Color)>
  {
    { EnumBeepThemes.DefaultTheme, (Color.LightGray, Color.Gray, Color.DarkGray, Color.LightSlateGray, Color.SlateGray) },
    { EnumBeepThemes.WinterTheme, (Color.WhiteSmoke, Color.SteelBlue, Color.LightBlue, Color.LightSteelBlue, Color.CadetBlue) },
    { EnumBeepThemes.CandyTheme, (Color.Pink, Color.DeepPink, Color.MistyRose, Color.HotPink, Color.MediumVioletRed) },
    { EnumBeepThemes.ZenTheme, (Color.Beige, Color.MediumSeaGreen, Color.LightGray, Color.SeaGreen, Color.DarkSeaGreen) },
    { EnumBeepThemes.RetroTheme, (Color.Bisque, Color.DarkGoldenrod, Color.SaddleBrown, Color.Goldenrod, Color.Chocolate) },
    { EnumBeepThemes.RoyalTheme, (Color.MidnightBlue, Color.Gold, Color.DarkSlateBlue, Color.LightGoldenrodYellow, Color.DarkGoldenrod) },
    { EnumBeepThemes.HighlightTheme, (Color.Yellow, Color.OrangeRed, Color.DarkOrange, Color.Orange, Color.Red) },
    { EnumBeepThemes.DarkTheme, (Color.Black, Color.DimGray, Color.SlateGray, Color.Gray, Color.DarkGray) },
    { EnumBeepThemes.OceanTheme, (Color.DarkCyan, Color.Teal, Color.LightSeaGreen, Color.Aqua, Color.DarkTurquoise) },
    { EnumBeepThemes.LightTheme, (Color.White, Color.LightGray, Color.Gainsboro, Color.Silver, Color.Gray) },
    { EnumBeepThemes.PastelTheme, (Color.LavenderBlush, Color.Plum, Color.Thistle, Color.MediumOrchid, Color.DarkOrchid) },
    { EnumBeepThemes.MidnightTheme, (Color.Black, Color.MidnightBlue, Color.DarkSlateGray, Color.SteelBlue, Color.DarkBlue) },
    { EnumBeepThemes.SpringTheme, (Color.MediumSpringGreen, Color.Chartreuse, Color.SeaGreen, Color.LimeGreen, Color.ForestGreen) },
    { EnumBeepThemes.ForestTheme, (Color.DarkOliveGreen, Color.ForestGreen, Color.OliveDrab, Color.MediumSeaGreen, Color.DarkGreen) },
    { EnumBeepThemes.NeonTheme, (Color.Black, Color.Lime, Color.DeepPink, Color.YellowGreen, Color.HotPink) },
    { EnumBeepThemes.RusticTheme, (Color.SaddleBrown, Color.Chocolate, Color.Tan, Color.Sienna, Color.DarkRed) },
    { EnumBeepThemes.GalaxyTheme, (Color.DarkSlateBlue, Color.Indigo, Color.MidnightBlue, Color.MediumPurple, Color.DarkViolet) },
    { EnumBeepThemes.DesertTheme, (Color.SandyBrown, Color.Peru, Color.Sienna, Color.Tan, Color.Chocolate) },
    { EnumBeepThemes.VintageTheme, (Color.Bisque, Color.Sienna, Color.Brown, Color.Peru, Color.DarkSlateGray) },
    { EnumBeepThemes.ModernDarkTheme, (Color.DarkSlateGray, Color.Gray, Color.DimGray, Color.LightSlateGray, Color.SlateGray) },
    { EnumBeepThemes.MaterialDesignTheme, (Color.LightSlateGray, Color.CornflowerBlue, Color.SlateGray, Color.SteelBlue, Color.RoyalBlue) },
    { EnumBeepThemes.NeumorphismTheme, (Color.WhiteSmoke, Color.LightGray, Color.Gainsboro, Color.Silver, Color.DarkGray) },
    { EnumBeepThemes.GlassmorphismTheme, (Color.Black, Color.LightSteelBlue, Color.Transparent, Color.SteelBlue, Color.DodgerBlue) },
    { EnumBeepThemes.FlatDesignTheme, (Color.Gainsboro, Color.Gray, Color.LightGray, Color.Silver, Color.DarkGray) },
    { EnumBeepThemes.CyberpunkNeonTheme, (Color.Black, Color.Fuchsia, Color.Aqua, Color.Magenta, Color.Purple) },
    { EnumBeepThemes.GradientBurstTheme, (Color.DarkMagenta, Color.OrangeRed, Color.DeepSkyBlue, Color.Crimson, Color.DodgerBlue) },
    { EnumBeepThemes.HighContrastTheme, (Color.Black, Color.White, Color.Gray, Color.LightGray, Color.Silver) },
    { EnumBeepThemes.MonochromeTheme, (Color.Black, Color.DarkGray, Color.LightGray, Color.Gray, Color.SlateGray) },
    { EnumBeepThemes.LuxuryGoldTheme, (Color.DarkSlateGray, Color.Gold, Color.DarkGoldenrod, Color.LightGoldenrodYellow, Color.Goldenrod) },
    { EnumBeepThemes.SunsetTheme, (Color.OrangeRed, Color.Crimson, Color.DarkOrange, Color.Orange, Color.Red) },
    { EnumBeepThemes.AutumnTheme, (Color.DarkOrange, Color.Brown, Color.Orange, Color.Peru, Color.Sienna) },
    { EnumBeepThemes.EarthyTheme, (Color.Tan, Color.SaddleBrown, Color.Peru, Color.Sienna, Color.Chocolate) }
  };
        static BeepThemesManager()
        {
            try
            {
                

                //  Console.WriteLine("BeepThemesManager initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Initialization error in BeepThemesManager: {ex.Message}");
                throw;
            }
        }
        public static readonly Guid DefaultThemeGuid = new Guid("00000000-0000-0000-0000-000000000001");
        #region "Themes"
        public static BeepTheme ModernDarkTheme => new BeepTheme
        {
            GridBackColor = Color.FromArgb(45, 45, 48), // Dark Gray
            GridForeColor = Color.WhiteSmoke,
            GridHeaderBackColor = Color.FromArgb(51, 51, 55), // Slightly Lighter Gray
            GridHeaderForeColor = Color.White,
            GridHeaderBorderColor = Color.Gray,
            GridHeaderHoverBackColor = Color.FromArgb(63, 63, 70), // Medium Gray
            GridHeaderHoverForeColor = Color.White,
            GridHeaderSelectedBackColor = Color.FromArgb(104, 33, 122), // Purple Accent
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.Gray,
            GridHeaderSelectedBorderColor = Color.Purple,
            GridRowHoverBackColor = Color.FromArgb(55, 55, 58), // Very Dark Gray
            GridRowHoverForeColor = Color.WhiteSmoke,
            GridRowSelectedBackColor = Color.FromArgb(104, 33, 122),
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.Gray,
            GridRowSelectedBorderColor = Color.Purple,
            CardBackColor = Color.FromArgb(45, 45, 48),


            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 255),  // White
            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(200, 200, 200),  // Light Gray
            },

            // UI Elements
            CloseButtonColor = Color.FromArgb(255, 69, 58),               // Bright Red
            MaxButtonColor = Color.FromArgb(97, 218, 251),                // Light Blue
            MinButtonColor = Color.FromArgb(97, 218, 251),                // Light Blue
            TitleBarColor = Color.FromArgb(28, 28, 28),                   // Dark Charcoal
            TitleBarTextColor = Color.FromArgb(255, 255, 255),            // White
            TitleBarIconColor = Color.FromArgb(97, 218, 251),             // Light Blue
            TitleBarHoverColor = Color.FromArgb(42, 42, 42),              // Slightly Lighter Charcoal
            TitleBarHoverTextColor = Color.FromArgb(255, 255, 255),       // White
            TitleBarHoverIconColor = Color.FromArgb(97, 218, 251),        // Light Blue
            TitleBarActiveColor = Color.FromArgb(28, 28, 28),             // Dark Charcoal
            TitleBarActiveTextColor = Color.FromArgb(255, 255, 255),      // White
            TitleBarActiveIconColor = Color.FromArgb(97, 218, 251),       // Light Blue
            TitleBarInactiveColor = Color.FromArgb(42, 42, 42),           // Slightly Lighter Charcoal
            TitleBarInactiveTextColor = Color.FromArgb(169, 169, 169),    // Light Gray
            TitleBarInactiveIconColor = Color.FromArgb(169, 169, 169),    // Light Gray
            TitleBarBorderColor = Color.FromArgb(97, 218, 251),           // Light Blue
            TitleBarBorderHoverColor = Color.FromArgb(97, 218, 251),      // Light Blue
            TitleBarBorderActiveColor = Color.FromArgb(97, 218, 251),     // Light Blue
            TitleBarBorderInactiveColor = Color.FromArgb(42, 42, 42),     // Slightly Lighter Charcoal

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(220, 0, 0),          // Red
            TitleBarCloseHoverTextColor = Color.FromArgb(255, 255, 255),  // White
            TitleBarCloseHoverIconColor = Color.FromArgb(255, 255, 255),  // White
            TitleBarCloseActiveColor = Color.FromArgb(165, 42, 42),       // Dark Red
            TitleBarCloseActiveTextColor = Color.FromArgb(255, 255, 255), // White
            TitleBarCloseActiveIconColor = Color.FromArgb(255, 255, 255), // White
            TitleBarCloseInactiveColor = Color.FromArgb(42, 42, 42),      // Slightly Lighter Charcoal
            TitleBarCloseInactiveTextColor = Color.FromArgb(169, 169, 169), // Light Gray
            TitleBarCloseInactiveIconColor = Color.FromArgb(169, 169, 169), // Light Gray
            TitleBarCloseBorderColor = Color.FromArgb(255, 69, 58),       // Bright Red
            TitleBarCloseBorderHoverColor = Color.FromArgb(220, 0, 0),    // Red
            TitleBarCloseBorderActiveColor = Color.FromArgb(165, 42, 42), // Dark Red
            TitleBarCloseBorderInactiveColor = Color.FromArgb(42, 42, 42),// Slightly Lighter Charcoal

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(42, 42, 42),           // Slightly Lighter Charcoal
            TitleBarMaxHoverTextColor = Color.FromArgb(255, 255, 255),    // White
            TitleBarMaxHoverIconColor = Color.FromArgb(97, 218, 251),     // Light Blue
            TitleBarMaxActiveColor = Color.FromArgb(28, 28, 28),          // Dark Charcoal
            TitleBarMaxActiveTextColor = Color.FromArgb(255, 255, 255),   // White
            TitleBarMaxActiveIconColor = Color.FromArgb(97, 218, 251),    // Light Blue
            TitleBarMaxInactiveColor = Color.FromArgb(42, 42, 42),        // Slightly Lighter Charcoal
            TitleBarMaxInactiveTextColor = Color.FromArgb(169, 169, 169), // Light Gray
            TitleBarMaxInactiveIconColor = Color.FromArgb(169, 169, 169), // Light Gray
            TitleBarMaxBorderColor = Color.FromArgb(97, 218, 251),        // Light Blue
            TitleBarMaxBorderHoverColor = Color.FromArgb(97, 218, 251),   // Light Blue
            TitleBarMaxBorderActiveColor = Color.FromArgb(97, 218, 251),  // Light Blue
            TitleBarMaxBorderInactiveColor = Color.FromArgb(42, 42, 42),  // Slightly Lighter Charcoal

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(42, 42, 42),           // Slightly Lighter Charcoal
            TitleBarMinHoverTextColor = Color.FromArgb(255, 255, 255),    // White
            TitleBarMinHoverIconColor = Color.FromArgb(97, 218, 251),     // Light Blue
            TitleBarMinActiveColor = Color.FromArgb(28, 28, 28),          // Dark Charcoal
            TitleBarMinActiveTextColor = Color.FromArgb(255, 255, 255),   // White
            TitleBarMinActiveIconColor = Color.FromArgb(97, 218, 251),    // Light Blue
            TitleBarMinInactiveColor = Color.FromArgb(42, 42, 42),        // Slightly Lighter Charcoal
            TitleBarMinInactiveTextColor = Color.FromArgb(169, 169, 169), // Light Gray
            TitleBarMinInactiveIconColor = Color.FromArgb(169, 169, 169), // Light Gray
            TitleBarMinBorderColor = Color.FromArgb(97, 218, 251),        // Light Blue
            TitleBarMinBorderHoverColor = Color.FromArgb(97, 218, 251),   // Light Blue
            TitleBarMinBorderActiveColor = Color.FromArgb(97, 218, 251),  // Light Blue
            TitleBarMinBorderInactiveColor = Color.FromArgb(42, 42, 42),  // Slightly Lighter Charcoal

            // Missing TitleBarMinimize properties (set to match Minimize Button)
            TitleBarMinimizeHoverColor = Color.FromArgb(42, 42, 42),
            TitleBarMinimizeHoverTextColor = Color.FromArgb(255, 255, 255),
            TitleBarMinimizeHoverIconColor = Color.FromArgb(97, 218, 251),
            TitleBarMinimizeActiveColor = Color.FromArgb(28, 28, 28),
            TitleBarMinimizeActiveTextColor = Color.FromArgb(255, 255, 255),
            TitleBarMinimizeActiveIconColor = Color.FromArgb(97, 218, 251),
            TitleBarMinimizeInactiveColor = Color.FromArgb(42, 42, 42),
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(169, 169, 169),
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(169, 169, 169),
            TitleBarMinimizeBorderColor = Color.FromArgb(97, 218, 251),
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(97, 218, 251),
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(97, 218, 251),
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(42, 42, 42),

            // Font and Typography properties
            FontName = "Segoe UI",
            FontSize = 14f,
            FontFamily = "Segoe UI",

            // Additional color properties for titles
            TitleForColor = Color.White,
            TitleBarForColor = Color.White, // Added missing property

            // Styles
            TitleStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 255), // White
            },
            SubtitleStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(169, 169, 169), // Light Gray
            },
            BodyStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 255, 255), // White
            },
            CaptionStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(169, 169, 169), // Light Gray
            },
            ButtonStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 255), // White
            },
            LinkStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(97, 218, 251), // Light Blue
            },
            OverlineStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 10f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(97, 218, 251), // Light Blue
            },

            // Additional UI Element Colors
            DescriptionForColor = Color.LightGray,
            BeforeForColor = Color.Gray,
            LatestForColor = Color.White,

            // General Colors
            BackColor = Color.FromArgb(30, 30, 30), // Dark background color

            // Button Colors
            ButtonBackColor = Color.FromArgb(50, 50, 50),
            ButtonForeColor = Color.White,
            ButtonHoverBackColor = Color.FromArgb(70, 70, 70),
            ButtonHoverForeColor = Color.White,
            ButtonActiveBackColor = Color.FromArgb(80, 80, 80),
            ButtonActiveForeColor = Color.White,

            // TextBox Colors
            TextBoxBackColor = Color.FromArgb(55, 55, 55),
            TextBoxForeColor = Color.White,

            // Label Colors
            LabelBackColor = Color.FromArgb(45, 45, 45),   // Dark Gray for background
            LabelForeColor = Color.White,                  // White for text color

            // Panel Colors
            PanelBackColor = Color.FromArgb(40, 40, 40), // Light Gray for subtle contrast against white background


            // Grid Colors
            HeaderBackColor = Color.FromArgb(45, 45, 45),
            HeaderForeColor = Color.White,
            GridLineColor = Color.Gray,
            RowBackColor = Color.FromArgb(30, 30, 30),
            RowForeColor = Color.White,
            AltRowBackColor = Color.FromArgb(40, 40, 40),
            SelectedRowBackColor = Color.FromArgb(60, 60, 60),
            SelectedRowForeColor = Color.White,

            // ComboBox Colors
            ComboBoxBackColor = Color.FromArgb(45, 45, 45),
            ComboBoxForeColor = Color.White,

            // CheckBox Colors
            CheckBoxBackColor = Color.FromArgb(30, 30, 30),
            CheckBoxForeColor = Color.White,

            // RadioButton Colors
            RadioButtonBackColor = Color.FromArgb(30, 30, 30),
            RadioButtonForeColor = Color.White,

            // Border Colors
            BorderColor = Color.Gray,
            ActiveBorderColor = Color.LightGray,
            InactiveBorderColor = Color.Gray,
            BorderSize = 1, // Added missing property

            // Link Colors
            LinkColor = Color.DeepSkyBlue,
            VisitedLinkColor = Color.MediumPurple,
            HoverLinkColor = Color.CornflowerBlue,

            // ToolTip Colors
            ToolTipBackColor = Color.FromArgb(50, 50, 50),
            ToolTipForeColor = Color.White,

            // ScrollBar Colors
            ScrollBarBackColor = Color.FromArgb(45, 45, 45),
            ScrollBarThumbColor = Color.Gray,
            ScrollBarTrackColor = Color.DarkGray,

            // Status Bar Colors
            StatusBarBackColor = Color.FromArgb(45, 45, 45),
            StatusBarForeColor = Color.White,

            // Tab Colors
            TabBackColor = Color.FromArgb(30, 30, 30),
            TabForeColor = Color.White,
            ActiveTabBackColor = Color.FromArgb(50, 50, 50),
            ActiveTabForeColor = Color.White,

            // Dialog Box Colors
            DialogBackColor = Color.FromArgb(40, 40, 40),
            DialogForeColor = Color.White,
            DialogButtonBackColor = Color.FromArgb(50, 50, 50),
            DialogButtonForeColor = Color.White,

            // Gradient Properties
            GradientStartColor = Color.FromArgb(30, 30, 30),
            GradientEndColor = Color.FromArgb(60, 60, 60),
            GradientDirection = LinearGradientMode.Vertical,

            // Side Menu Colors
            SideMenuBackColor = Color.FromArgb(50, 50, 50),           // Darker than the background
            SideMenuHoverBackColor = Color.FromArgb(60, 60, 60),      // Slightly lighter than the back color
            SideMenuSelectedBackColor = Color.FromArgb(80, 80, 80),   // More distinct for selection
            SideMenuForeColor = Color.White,                         // Clear text color
            SideMenuHoverForeColor = Color.White,                    // Same as default for simplicity
            SideMenuSelectedForeColor = Color.LightBlue,             // Highlighted text for selected state
            SideMenuBorderColor = Color.Gray,                        // Subtle borders for separation
            SideMenuIconColor = Color.LightGray,                     // Non-selected icons
            SideMenuSelectedIconColor = Color.DeepSkyBlue,           // Selected icons stand out


            // Title Bar Colors
            TitleBarBackColor = Color.FromArgb(45, 45, 45),
            TitleBarForeColor = Color.White,
            TitleBarHoverBackColor = Color.FromArgb(60, 60, 60),
            TitleBarHoverForeColor = Color.White,

            // Dashboard Colors
            DashboardBackColor = Color.FromArgb(30, 30, 30),
            DashboardCardBackColor = Color.FromArgb(40, 40, 40),
            DashboardCardHoverBackColor = Color.FromArgb(50, 50, 50),
            CardTitleForeColor = Color.White,
            CardTextForeColor = Color.LightGray,

            // Data Visualization (Charts)
            ChartBackColor = Color.FromArgb(30, 30, 30),
            ChartLineColor = Color.DeepSkyBlue,
            ChartFillColor = Color.FromArgb(100, 0, 191, 255), // Semi-transparent DeepSkyBlue
            ChartAxisColor = Color.LightGray,

            // Sidebar and Menu Colors
            SidebarIconColor = Color.LightGray,
            SidebarSelectedIconColor = Color.White,
            SidebarTextColor = Color.LightGray,
            SidebarSelectedTextColor = Color.White,

            // Navigation Colors
            NavigationBackColor = Color.FromArgb(30, 30, 30),
            NavigationForeColor = Color.White,
            NavigationHoverBackColor = Color.FromArgb(45, 45, 45),
            NavigationHoverForeColor = Color.White,

            // Badge and Highlight Colors
            BadgeBackColor = Color.DeepSkyBlue,
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.DarkOrange,

            // Font Sizes
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // Font Styles
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // Text Colors
            PrimaryTextColor = Color.White,
            SecondaryTextColor = Color.LightGray,
            AccentTextColor = Color.DeepSkyBlue,

            // Typography

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.White,
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.White,
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.White,
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.White,
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.White,
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.White,
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.LightGray,
            },

            // Blockquote
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.LightGray,
            },
            BlockquoteBorderColor = Color.Gray,
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,

            // Inline Code
            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.LightGray,
            },
            InlineCodeBackgroundColor = Color.FromArgb(50, 50, 50),
            InlineCodePadding = 4f,

            // Code Block
            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.LightGray,
            },
            CodeBlockBackgroundColor = Color.FromArgb(40, 40, 40),
            CodeBlockBorderColor = Color.Gray,
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            // Lists
            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.LightGray,
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.LightGray,
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            // Link
            Link = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Underline,
                FontWeight = FontWeight.Regular,
                TextColor = Color.DeepSkyBlue,
            },
            LinkHoverColor = Color.CornflowerBlue,
            LinkIsUnderline = true,

            // Additional Typography Styles
            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.LightGray,
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.LightGray,
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.White,
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.White,
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.White,
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.LightGray,
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.LightGray,
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.LightGray,
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.White,
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.White,
            },

            // Color Palette
            PrimaryColor = Color.DeepSkyBlue,
            SecondaryColor = Color.LightGray,
            AccentColor = Color.DeepSkyBlue,
            BackgroundColor = Color.FromArgb(30, 30, 30),
            SurfaceColor = Color.FromArgb(45, 45, 45),
            ErrorColor = Color.IndianRed,
            WarningColor = Color.Orange,
            SuccessColor = Color.LimeGreen,
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.White,

            // Additional Properties
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 4,
            IconSet = "ModernIcons",
            ApplyThemeToIcons = true,
            ShadowColor = Color.FromArgb(255, 0, 0, 0),
            ShadowOpacity = 0.5f,
            AnimationDurationShort = 150,  // in milliseconds
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",
            HighContrastMode = false,
            FocusIndicatorColor = Color.DeepSkyBlue,
            IsDarkTheme = true,
        };
        public static BeepTheme MaterialDesignTheme => new BeepTheme
        {
            // ===== Grid Colors =====
            GridBackColor = Color.FromArgb(250, 250, 250),           // Very Light Gray
            GridForeColor = Color.FromArgb(33, 33, 33),                // Dark Gray
            GridHeaderBackColor = Color.FromArgb(245, 245, 245),       // Light Gray
            GridHeaderForeColor = Color.FromArgb(33, 33, 33),
            GridHeaderBorderColor = Color.FromArgb(224, 224, 224),     // Light divider color
            GridHeaderHoverBackColor = Color.FromArgb(224, 224, 224),
            GridHeaderHoverForeColor = Color.FromArgb(33, 33, 33),
            GridHeaderSelectedBackColor = Color.FromArgb(0, 150, 136), // Material Teal 500 (#009688)
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.Gray,
            GridHeaderSelectedBorderColor = Color.FromArgb(0, 150, 136),
            GridRowHoverBackColor = Color.FromArgb(245, 245, 245),
            GridRowHoverForeColor = Color.FromArgb(33, 33, 33),
            GridRowSelectedBackColor = Color.FromArgb(0, 150, 136),
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.FromArgb(224, 224, 224),
            GridRowSelectedBorderColor = Color.FromArgb(0, 150, 136),

            // ===== Card Styles =====
            CardBackColor = Color.FromArgb(236, 240, 241),
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 20f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(33, 33, 33),  // Dark Gray
            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(117, 117, 117),  // Medium Gray
            },

            // ===== Window Controls & Title Bar =====
            // Window control buttons (Close, Maximize, Minimize)
            CloseButtonColor = Color.FromArgb(244, 67, 54),      // Material Red 500
            MaxButtonColor = Color.FromArgb(76, 175, 80),        // Material Green 500
            MinButtonColor = Color.FromArgb(255, 193, 7),        // Material Amber 500

            // Consolidated Title Bar styling for a light theme
            TitleBarBackColor = Color.White,
            TitleBarForeColor = Color.FromArgb(33, 33, 33),      // Dark text for contrast
            TitleBarIconColor = Color.FromArgb(33, 33, 33),      // Matching icon color
            TitleBarHoverBackColor = Color.FromArgb(245, 245, 245),
            TitleBarHoverForeColor = Color.FromArgb(33, 33, 33),
            TitleBarBorderColor = Color.FromArgb(224, 224, 224),  // Light divider

            // Title Bar – Close Button Specifics
            TitleBarCloseHoverColor = Color.FromArgb(211, 47, 47),   // Darker Red on hover
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(183, 28, 28),  // Even darker Red when active
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(245, 245, 245),
            TitleBarCloseInactiveTextColor = Color.FromArgb(117, 117, 117),
            TitleBarCloseInactiveIconColor = Color.FromArgb(117, 117, 117),
            TitleBarCloseBorderColor = Color.FromArgb(244, 67, 54),
            TitleBarCloseBorderHoverColor = Color.FromArgb(211, 47, 47),
            TitleBarCloseBorderActiveColor = Color.FromArgb(183, 28, 28),
            TitleBarCloseBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // Title Bar – Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(245, 245, 245),
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.FromArgb(76, 175, 80),
            TitleBarMaxActiveColor = Color.White,
            TitleBarMaxActiveTextColor = Color.FromArgb(33, 33, 33),
            TitleBarMaxActiveIconColor = Color.FromArgb(76, 175, 80),
            TitleBarMaxInactiveColor = Color.FromArgb(245, 245, 245),
            TitleBarMaxInactiveTextColor = Color.FromArgb(117, 117, 117),
            TitleBarMaxInactiveIconColor = Color.FromArgb(117, 117, 117),
            TitleBarMaxBorderColor = Color.FromArgb(76, 175, 80),
            TitleBarMaxBorderHoverColor = Color.FromArgb(76, 175, 80),
            TitleBarMaxBorderActiveColor = Color.FromArgb(76, 175, 80),
            TitleBarMaxBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // Title Bar – Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(245, 245, 245),
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.FromArgb(255, 193, 7),
            TitleBarMinActiveColor = Color.White,
            TitleBarMinActiveTextColor = Color.FromArgb(33, 33, 33),
            TitleBarMinActiveIconColor = Color.FromArgb(255, 193, 7),
            TitleBarMinInactiveColor = Color.FromArgb(245, 245, 245),
            TitleBarMinInactiveTextColor = Color.FromArgb(117, 117, 117),
            TitleBarMinInactiveIconColor = Color.FromArgb(117, 117, 117),
            TitleBarMinBorderColor = Color.FromArgb(255, 193, 7),
            TitleBarMinBorderHoverColor = Color.FromArgb(255, 193, 7),
            TitleBarMinBorderActiveColor = Color.FromArgb(255, 193, 7),
            TitleBarMinBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // ===== Typography Styles =====
            TitleStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 24f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(33, 33, 33),
            },
            SubtitleStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 20f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(66, 66, 66),
            },
            BodyStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 16f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(117, 117, 117),
            },
            CaptionStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 12f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(158, 158, 158),
            },
            ButtonStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 14f,
                FontWeight = FontWeight.Medium,
                TextColor = Color.White,
            },
            LinkStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(33, 150, 243), // Material Blue
            },
            OverlineStyle = new TypographyStyle
            {
                FontFamily = "Roboto",
                FontSize = 10f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(33, 33, 33),
            },

            // ===== General Colors =====
            BackColor = Color.White,
            PanelBackColor = Color.FromArgb(240, 240, 240), // Medium Light Gray
            BackgroundColor = Color.White,
            SurfaceColor = Color.White,

            // ===== Primary & Secondary Colors =====
            PrimaryColor = Color.FromArgb(33, 150, 243),    // Material Blue 500
            SecondaryColor = Color.FromArgb(156, 39, 176),  // Material Purple 500
            AccentColor = Color.FromArgb(0, 188, 212),      // Material Cyan 500
            ErrorColor = Color.FromArgb(211, 47, 47),         // Material Red 700
            WarningColor = Color.FromArgb(255, 152, 0),       // Material Orange 500
            SuccessColor = Color.FromArgb(76, 175, 80),       // Material Green 500
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.Black,

            // ===== Button Colors (Updated for Material Buttons) =====
            ButtonBackColor = Color.FromArgb(33, 150, 243),       // Use Primary color
            ButtonForeColor = Color.White,                        // OnPrimary color
            ButtonHoverBackColor = Color.FromArgb(30, 136, 229),   // Slightly darker Primary
            ButtonHoverForeColor = Color.White,
            ButtonActiveBackColor = Color.FromArgb(25, 118, 210),  // Even darker Primary
            ButtonActiveForeColor = Color.White,

            // ===== TextBox & Label Colors =====
            TextBoxBackColor = Color.White,
            TextBoxForeColor = Color.Black,
            LabelBackColor = Color.White,
            LabelForeColor = Color.Black,

            // ===== Additional Grid Colors =====
            HeaderBackColor = Color.FromArgb(245, 245, 245),
            HeaderForeColor = Color.Black,
            GridLineColor = Color.LightGray,
            RowBackColor = Color.White,
            RowForeColor = Color.Black,
            AltRowBackColor = Color.FromArgb(250, 250, 250),
            SelectedRowBackColor = Color.FromArgb(224, 224, 224),
            SelectedRowForeColor = Color.Black,

            // ===== ComboBox, CheckBox, & RadioButton Colors =====
            ComboBoxBackColor = Color.White,
            ComboBoxForeColor = Color.Black,
            CheckBoxBackColor = Color.White,
            CheckBoxForeColor = Color.Black,
            RadioButtonBackColor = Color.White,
            RadioButtonForeColor = Color.Black,

            // ===== Border Colors =====
            BorderColor = Color.LightGray,
            ActiveBorderColor = Color.FromArgb(33, 150, 243),
            InactiveBorderColor = Color.LightGray,
            BorderSize = 1,

            // ===== Link Colors =====
            LinkColor = Color.FromArgb(33, 150, 243),
            VisitedLinkColor = Color.FromArgb(156, 39, 176),
            HoverLinkColor = Color.FromArgb(30, 136, 229),

            // ===== ToolTip Colors =====
            ToolTipBackColor = Color.FromArgb(66, 66, 66),    // Dark Gray
            ToolTipForeColor = Color.White,

            // ===== ScrollBar Colors =====
            ScrollBarBackColor = Color.FromArgb(245, 245, 245),
            ScrollBarThumbColor = Color.Gray,
            ScrollBarTrackColor = Color.LightGray,

            // ===== Status Bar Colors =====
            StatusBarBackColor = Color.FromArgb(33, 150, 243),
            StatusBarForeColor = Color.White,

            // ===== Tab Control Colors =====
            TabBackColor = Color.White,
            TabForeColor = Color.Black,
            ActiveTabBackColor = Color.FromArgb(33, 150, 243),
            ActiveTabForeColor = Color.White,

            // ===== Dialog Box Colors =====
            DialogBackColor = Color.White,
            DialogForeColor = Color.Black,
            DialogButtonBackColor = Color.FromArgb(33, 150, 243),
            DialogButtonForeColor = Color.White,

            // ===== Side Menu Colors =====
            SideMenuBackColor = Color.FromArgb(245, 245, 245),
            SideMenuHoverBackColor = Color.FromArgb(224, 224, 224),
            SideMenuSelectedBackColor = Color.FromArgb(200, 200, 200),
            SideMenuForeColor = Color.FromArgb(33, 33, 33),
            SideMenuHoverForeColor = Color.Black,
            SideMenuSelectedForeColor = Color.FromArgb(33, 150, 243),
            SideMenuBorderColor = Color.LightGray,
            SideMenuIconColor = Color.Gray,
            SideMenuSelectedIconColor = Color.FromArgb(33, 150, 243),

            // ===== Navigation Colors =====
            NavigationBackColor = Color.White,
            NavigationForeColor = Color.Black,
            NavigationHoverBackColor = Color.FromArgb(245, 245, 245),
            NavigationHoverForeColor = Color.Black,

            // ===== Badge & Highlight Colors =====
            BadgeBackColor = Color.FromArgb(33, 150, 243), // Primary
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(255, 234, 0), // Material Yellow A200 (adjust as needed)

            // ===== Font Properties & Sizes =====
            FontFamily = "Roboto",
            FontName = "Roboto",
            FontSize = 14f,
            FontSizeBlockHeader = 96f,  // Display Large
            FontSizeBlockText = 16f,    // Body Large
            FontSizeQuestion = 14f,     // Body Medium
            FontSizeAnswer = 14f,       // Body Medium
            FontSizeCaption = 12f,      // Body Small
            FontSizeButton = 14f,       // Label Large
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // ===== Text Colors =====
            PrimaryTextColor = Color.Black,
            SecondaryTextColor = Color.DarkGray,
            AccentTextColor = Color.FromArgb(33, 150, 243),

            // ===== Additional UI Element Colors =====
            TitleForColor = Color.Black,
            TitleBarForColor = Color.White,
            DescriptionForColor = Color.FromArgb(117, 117, 117),  // Light Gray
            BeforeForColor = Color.FromArgb(117, 117, 117),
            LatestForColor = Color.FromArgb(117, 117, 117),

            // ===== Spacing & Layout =====
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 4,

            // ===== Imagery & Iconography =====
            IconSet = "MaterialIcons",
            ApplyThemeToIcons = true,

            // ===== Effects & Decorations =====
            ShadowColor = Color.FromArgb(100, 0, 0, 0),  // Semi-transparent black
            ShadowOpacity = 0.2f,
            GradientStartColor = Color.White,
            GradientEndColor = Color.White,
            GradientDirection = LinearGradientMode.Vertical,

            // ===== Animation & Transitions =====
            AnimationDurationShort = 150,  // milliseconds
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",

            // ===== Accessibility =====
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(33, 150, 243),

            // ===== Theme Variant =====
            IsDarkTheme = false,
        };
        public static BeepTheme NeumorphismTheme => new BeepTheme
        {
            GridBackColor = Color.White,
            GridForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBackColor = Color.Gainsboro,
            GridHeaderForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBorderColor = Color.LightGray,
            GridHeaderHoverBackColor = Color.WhiteSmoke,
            GridHeaderHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderSelectedBackColor = Color.LightGray,
            GridHeaderSelectedForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderHoverBorderColor = Color.LightGray,
            GridHeaderSelectedBorderColor = Color.Gray,
            GridRowHoverBackColor = Color.WhiteSmoke,
            GridRowHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridRowSelectedBackColor = Color.LightGray,
            GridRowSelectedForeColor = Color.FromArgb(255, 0, 0, 0),
            GridRowHoverBorderColor = Color.LightGray,
            GridRowSelectedBorderColor = Color.Gray,
            CardBackColor = Color.FromArgb(240, 240, 240),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontWeight = FontWeight.Bold,
                TextColor =Color.FromArgb(255, 0, 0, 0),  // Black
            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(128, 128, 128),  // Gray
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(232, 234, 237),
            MaxButtonColor = Color.FromArgb(178, 235, 242),
            MinButtonColor = Color.FromArgb(255, 241, 118),
            TitleBarColor = Color.FromArgb(245, 245, 245),
            TitleBarTextColor = Color.FromArgb(33, 33, 33),
            TitleBarIconColor = Color.FromArgb(38, 198, 218),
            TitleBarHoverColor = Color.FromArgb(250, 250, 250),
            TitleBarHoverTextColor = Color.FromArgb(33, 33, 33),
            TitleBarHoverIconColor = Color.FromArgb(38, 198, 218),
            TitleBarActiveColor = Color.FromArgb(245, 245, 245),
            TitleBarActiveTextColor = Color.FromArgb(33, 33, 33),
            TitleBarActiveIconColor = Color.FromArgb(38, 198, 218),
            TitleBarInactiveColor = Color.FromArgb(250, 250, 250),
            TitleBarInactiveTextColor = Color.FromArgb(189, 189, 189),
            TitleBarInactiveIconColor = Color.FromArgb(189, 189, 189),
            TitleBarBorderColor = Color.FromArgb(224, 224, 224),
            TitleBarBorderHoverColor = Color.FromArgb(38, 198, 218),
            TitleBarBorderActiveColor = Color.FromArgb(38, 198, 218),
            TitleBarBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(255, 82, 82),
            TitleBarCloseHoverTextColor = Color.FromArgb(33, 33, 33),
            TitleBarCloseHoverIconColor = Color.FromArgb(33, 33, 33),
            TitleBarCloseActiveColor = Color.FromArgb(233, 30, 99),
            TitleBarCloseActiveTextColor = Color.FromArgb(33, 33, 33),
            TitleBarCloseActiveIconColor = Color.FromArgb(33, 33, 33),
            TitleBarCloseInactiveColor = Color.FromArgb(224, 224, 224),
            TitleBarCloseInactiveTextColor = Color.FromArgb(189, 189, 189),
            TitleBarCloseInactiveIconColor = Color.FromArgb(189, 189, 189),
            TitleBarCloseBorderColor = Color.FromArgb(232, 234, 237),
            TitleBarCloseBorderHoverColor = Color.FromArgb(255, 82, 82),
            TitleBarCloseBorderActiveColor = Color.FromArgb(233, 30, 99),
            TitleBarCloseBorderInactiveColor = Color.FromArgb(232, 234, 237),

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(250, 250, 250),
            TitleBarMaxHoverTextColor = Color.FromArgb(33, 33, 33),
            TitleBarMaxHoverIconColor = Color.FromArgb(178, 235, 242),
            TitleBarMaxActiveColor = Color.FromArgb(245, 245, 245),
            TitleBarMaxActiveTextColor = Color.FromArgb(33, 33, 33),
            TitleBarMaxActiveIconColor = Color.FromArgb(178, 235, 242),
            TitleBarMaxInactiveColor = Color.FromArgb(250, 250, 250),
            TitleBarMaxInactiveTextColor = Color.FromArgb(189, 189, 189),
            TitleBarMaxInactiveIconColor = Color.FromArgb(189, 189, 189),
            TitleBarMaxBorderColor = Color.FromArgb(178, 235, 242),
            TitleBarMaxBorderHoverColor = Color.FromArgb(178, 235, 242),
            TitleBarMaxBorderActiveColor = Color.FromArgb(178, 235, 242),
            TitleBarMaxBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(250, 250, 250),
            TitleBarMinHoverTextColor = Color.FromArgb(33, 33, 33),
            TitleBarMinHoverIconColor = Color.FromArgb(255, 241, 118),
            TitleBarMinActiveColor = Color.FromArgb(245, 245, 245),
            TitleBarMinActiveTextColor = Color.FromArgb(33, 33, 33),
            TitleBarMinActiveIconColor = Color.FromArgb(255, 241, 118),
            TitleBarMinInactiveColor = Color.FromArgb(250, 250, 250),
            TitleBarMinInactiveTextColor = Color.FromArgb(189, 189, 189),
            TitleBarMinInactiveIconColor = Color.FromArgb(189, 189, 189),
            TitleBarMinBorderColor = Color.FromArgb(255, 241, 118),
            TitleBarMinBorderHoverColor = Color.FromArgb(255, 241, 118),
            TitleBarMinBorderActiveColor = Color.FromArgb(255, 241, 118),
            TitleBarMinBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // TitleBarMinimize properties
            TitleBarMinimizeHoverColor = Color.FromArgb(250, 250, 250),
            TitleBarMinimizeHoverTextColor = Color.FromArgb(33, 33, 33),
            TitleBarMinimizeHoverIconColor = Color.FromArgb(255, 241, 118),
            TitleBarMinimizeActiveColor = Color.FromArgb(245, 245, 245),
            TitleBarMinimizeActiveTextColor = Color.FromArgb(33, 33, 33),
            TitleBarMinimizeActiveIconColor = Color.FromArgb(255, 241, 118),
            TitleBarMinimizeInactiveColor = Color.FromArgb(250, 250, 250),
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(189, 189, 189),
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(189, 189, 189),
            TitleBarMinimizeBorderColor = Color.FromArgb(255, 241, 118),
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(255, 241, 118),
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(255, 241, 118),
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // **Typography Styles**
            TitleStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(33, 33, 33),
            },
            SubtitleStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(97, 97, 97),
            },
            BodyStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(33, 33, 33),
            },
            CaptionStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(189, 189, 189),
            },
            ButtonStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(33, 33, 33),
            },
            LinkStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(38, 198, 218),
            },
            OverlineStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 10f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(189, 189, 189),
            },

            // **General Colors**
            TitleForColor = Color.FromArgb(60, 60, 60),
            TitleBarForColor = Color.FromArgb(33, 33, 33),
            DescriptionForColor = Color.FromArgb(100, 100, 100),
            BeforeForColor = Color.FromArgb(150, 150, 150),
            LatestForColor = Color.FromArgb(200, 200, 200),
            BackColor = Color.FromArgb(240, 240, 240),

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(240, 240, 240),
            ButtonForeColor = Color.FromArgb(33, 33, 33),
            ButtonHoverBackColor = Color.FromArgb(230, 230, 230),
            ButtonHoverForeColor = Color.FromArgb(33, 33, 33),
            ButtonActiveBackColor = Color.FromArgb(220, 220, 220),
            ButtonActiveForeColor = Color.FromArgb(33, 33, 33),

            // **TextBox Colors**
            TextBoxBackColor = Color.FromArgb(245, 245, 245),
            TextBoxForeColor = Color.FromArgb(60, 60, 60),

            // **Label Colors**
            LabelBackColor = Color.White,
            LabelForeColor = Color.FromArgb(60, 60, 60),

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(245, 245, 245),

            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(245, 245, 245),
            HeaderForeColor = Color.FromArgb(60, 60, 60),
            GridLineColor = Color.FromArgb(220, 220, 220),
            RowBackColor = Color.FromArgb(240, 240, 240),
            RowForeColor = Color.FromArgb(60, 60, 60),
            AltRowBackColor = Color.FromArgb(235, 235, 235),
            SelectedRowBackColor = Color.FromArgb(220, 220, 220),
            SelectedRowForeColor = Color.FromArgb(60, 60, 60),

            // **ComboBox Colors**
            ComboBoxBackColor = Color.FromArgb(245, 245, 245),
            ComboBoxForeColor = Color.FromArgb(60, 60, 60),

            // **CheckBox Colors**
            CheckBoxBackColor = Color.White,
            CheckBoxForeColor = Color.FromArgb(60, 60, 60),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.White,
            RadioButtonForeColor = Color.FromArgb(60, 60, 60),

            // **Border Colors**
            BorderColor = Color.FromArgb(220, 220, 220),
            ActiveBorderColor = Color.FromArgb(200, 200, 200),
            InactiveBorderColor = Color.FromArgb(220, 220, 220),
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(38, 198, 218),
            VisitedLinkColor = Color.FromArgb(0, 150, 136),
            HoverLinkColor = Color.FromArgb(0, 172, 193),

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(60, 60, 60),
            ToolTipForeColor = Color.White,

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(245, 245, 245),
            ScrollBarThumbColor = Color.FromArgb(220, 220, 220),
            ScrollBarTrackColor = Color.FromArgb(230, 230, 230),

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(240, 240, 240),
            StatusBarForeColor = Color.FromArgb(60, 60, 60),

            // **Tab Colors**
            TabBackColor = Color.FromArgb(240, 240, 240),
            TabForeColor = Color.FromArgb(60, 60, 60),
            ActiveTabBackColor = Color.FromArgb(220, 220, 220),
            ActiveTabForeColor = Color.FromArgb(38, 198, 218),

            // **Dialog Box Colors**
            DialogBackColor = Color.FromArgb(245, 245, 245),
            DialogForeColor = Color.FromArgb(60, 60, 60),
            DialogButtonBackColor = Color.FromArgb(240, 240, 240),
            DialogButtonForeColor = Color.FromArgb(60, 60, 60),

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(245, 245, 245),
            GradientEndColor = Color.FromArgb(230, 230, 230),
            GradientDirection = LinearGradientMode.Vertical,

            // **Panel and Side Menu Colors**
            SideMenuBackColor =Color.DarkGray,
            SideMenuHoverBackColor = Color.FromArgb(235, 235, 235),
            SideMenuSelectedBackColor = Color.FromArgb(220, 220, 220),
            SideMenuForeColor = Color.FromArgb(33, 33, 33),
            SideMenuHoverForeColor = Color.FromArgb(33, 33, 33),
            SideMenuSelectedForeColor = Color.FromArgb(0, 172, 193),
            SideMenuBorderColor = Color.FromArgb(220, 220, 220),
            SideMenuIconColor = Color.FromArgb(60, 60, 60),
            SideMenuSelectedIconColor = Color.FromArgb(33, 33, 33),

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(245, 245, 245),
            TitleBarForeColor = Color.FromArgb(33, 33, 33),
            TitleBarHoverBackColor = Color.FromArgb(250, 250, 250),
            TitleBarHoverForeColor = Color.FromArgb(33, 33, 33),

            // **Dashboard Colors**
            DashboardBackColor = Color.FromArgb(240, 240, 240),
            DashboardCardBackColor = Color.FromArgb(245, 245, 245),
            DashboardCardHoverBackColor = Color.FromArgb(230, 230, 230),
            CardTitleForeColor = Color.FromArgb(60, 60, 60),
            CardTextForeColor = Color.FromArgb(100, 100, 100),

            // **Data Visualization (Charts)**
            ChartBackColor = Color.FromArgb(245, 245, 245),
            // Removed transparency from ChartFillColor:
            ChartLineColor = Color.FromArgb(38, 198, 218),
            ChartFillColor = Color.FromArgb(38, 198, 218),   // Now fully opaque
            ChartAxisColor = Color.FromArgb(100, 100, 100),

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(60, 60, 60),
            SidebarSelectedIconColor = Color.FromArgb(38, 198, 218),
            SidebarTextColor = Color.FromArgb(60, 60, 60),
            SidebarSelectedTextColor = Color.FromArgb(38, 198, 218),

            // **Navigation Colors**
            NavigationBackColor = Color.FromArgb(240, 240, 240),
            NavigationForeColor = Color.FromArgb(60, 60, 60),
            NavigationHoverBackColor = Color.FromArgb(235, 235, 235),
            NavigationHoverForeColor = Color.FromArgb(60, 60, 60),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(38, 198, 218),
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(255, 241, 118),

            // **Font Properties**
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(60, 60, 60),
            SecondaryTextColor = Color.FromArgb(100, 100, 100),
            AccentTextColor = Color.FromArgb(38, 198, 218),

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(100, 100, 100),
            },
            BlockquoteBorderColor = Color.FromArgb(220, 220, 220),
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(100, 100, 100),
            },
            InlineCodeBackgroundColor = Color.FromArgb(230, 230, 230),
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(100, 100, 100),
            },
            CodeBlockBackgroundColor = Color.FromArgb(235, 235, 235),
            CodeBlockBorderColor = Color.FromArgb(220, 220, 220),
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            LinkHoverColor = Color.FromArgb(0, 172, 193),
            LinkIsUnderline = true,

            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(100, 100, 100),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(60, 60, 60),
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(60, 60, 60),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(100, 100, 100),
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(100, 100, 100),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(100, 100, 100),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(60, 60, 60),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(38, 198, 218),
            SecondaryColor = Color.FromArgb(60, 60, 60),
            AccentColor = Color.FromArgb(0, 172, 193),
            BackgroundColor = Color.FromArgb(240, 240, 240),
            SurfaceColor = Color.FromArgb(250, 250, 250),
            ErrorColor = Color.FromArgb(233, 30, 99),
            WarningColor = Color.FromArgb(255, 241, 118),
            SuccessColor = Color.FromArgb(0, 230, 118),
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.FromArgb(60, 60, 60),

            // **Spacing and Layout**
            PaddingSmall = 8,
            PaddingMedium = 16,
            PaddingLarge = 24,
            BorderRadius = 12,

            // **Imagery and Iconography**
            IconSet = "NeumorphismIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(200, 255, 255, 255),
            ShadowOpacity = 0.5f,
            AnimationDurationShort = 150,
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(38, 198, 218),

            // **Theme Variant**
            IsDarkTheme = false,
        };
        public static BeepTheme OceanTheme => new BeepTheme
        {
            GridBackColor = Color.FromArgb(240, 248, 255), // Alice Blue
            GridForeColor = Color.DarkSlateGray,
            GridHeaderBackColor = Color.FromArgb(70, 130, 180), // Steel Blue
            GridHeaderForeColor = Color.White,
            GridHeaderBorderColor = Color.Teal,
            GridHeaderHoverBackColor = Color.FromArgb(0, 128, 128), // Teal
            GridHeaderHoverForeColor = Color.White,
            GridHeaderSelectedBackColor = Color.FromArgb(0, 105, 148), // Deep Teal
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.DarkSlateGray,
            GridHeaderSelectedBorderColor = Color.DarkSlateGray,
            GridRowHoverBackColor = Color.FromArgb(224, 255, 255), // Light Cyan
            GridRowHoverForeColor = Color.DarkSlateGray,
            GridRowSelectedBackColor = Color.Teal,
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.DarkSlateGray,
            GridRowSelectedBorderColor = Color.DarkSlateGray,
            CardBackColor = Color.FromArgb(176, 224, 230),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 128, 255),  // Ocean Blue
            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(192, 192, 192),  // Light Gray
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(0, 105, 148),             // Deep Ocean Blue
            MaxButtonColor = Color.FromArgb(0, 128, 255),               // Ocean Blue
            MinButtonColor = Color.FromArgb(64, 224, 208),              // Turquoise
            TitleBarColor = Color.FromArgb(0, 105, 148),                // Deep Ocean Blue
            TitleBarTextColor = Color.White,
            TitleBarIconColor = Color.White,
            TitleBarHoverColor = Color.FromArgb(0, 128, 255),           // Ocean Blue
            TitleBarHoverTextColor = Color.White,
            TitleBarHoverIconColor = Color.White,
            TitleBarActiveColor = Color.FromArgb(0, 105, 148),          // Deep Ocean Blue
            TitleBarActiveTextColor = Color.White,
            TitleBarActiveIconColor = Color.White,
            TitleBarInactiveColor = Color.FromArgb(64, 224, 208),       // Turquoise
            TitleBarInactiveTextColor = Color.White,
            TitleBarInactiveIconColor = Color.White,
            TitleBarBorderColor = Color.FromArgb(0, 105, 148),          // Deep Ocean Blue
            TitleBarBorderHoverColor = Color.FromArgb(0, 128, 255),     // Ocean Blue
            TitleBarBorderActiveColor = Color.FromArgb(0, 105, 148),    // Deep Ocean Blue
            TitleBarBorderInactiveColor = Color.FromArgb(64, 224, 208), // Turquoise

            // Missing TitleBarMinimize properties (set to match Minimize Button)
            TitleBarMinimizeHoverColor = Color.FromArgb(64, 224, 208),       // Turquoise
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(0, 105, 148),       // Deep Ocean Blue
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(64, 224, 208),    // Turquoise
            TitleBarMinimizeInactiveTextColor = Color.White,
            TitleBarMinimizeInactiveIconColor = Color.White,
            TitleBarMinimizeBorderColor = Color.FromArgb(0, 105, 148),       // Deep Ocean Blue
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(0, 128, 255),  // Ocean Blue
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(0, 105, 148), // Deep Ocean Blue
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(64, 224, 208), // Turquoise

            // **Typography Styles**
            TitleStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(0, 128, 255), // Ocean Blue
            },
            SubtitleStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(0, 105, 148), // Deep Ocean Blue
            },
            BodyStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(64, 224, 208), // Turquoise
            },
            CaptionStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(0, 128, 255), // Ocean Blue
            },
            ButtonStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.White,
            },
            LinkStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(0, 128, 255), // Ocean Blue
            },
            OverlineStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 10f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(64, 224, 208), // Turquoise
            },

            // **General Colors**
            TitleForColor = Color.FromArgb(0, 128, 255),                // Ocean Blue
            TitleBarForColor = Color.FromArgb(0, 105, 148),             // Deep Ocean Blue
            DescriptionForColor = Color.FromArgb(72, 209, 204),         // Medium Turquoise
            BeforeForColor = Color.FromArgb(175, 238, 238),             // Pale Turquoise
            LatestForColor = Color.FromArgb(0, 128, 128),               // Teal
            BackColor = Color.FromArgb(224, 255, 255),                  // Light Cyan Background

            // **Button Colors**
            ButtonForeColor   = Color.FromArgb(0, 160, 176),              // Blue-Green
            ButtonBackColor = Color.White,
            ButtonHoverBackColor = Color.FromArgb(0, 139, 139),         // Dark Cyan
            ButtonHoverForeColor = Color.White,
            ButtonActiveBackColor = Color.FromArgb(0, 123, 167),        // Deep Blue
            ButtonActiveForeColor = Color.White,

            // **TextBox Colors**
            TextBoxBackColor = Color.White,
            TextBoxForeColor = Color.FromArgb(0, 105, 148),             // Deep Ocean Blue

            // **Label Colors**
            LabelBackColor = Color.White,
            LabelForeColor = Color.FromArgb(0, 105, 148),               // Deep Ocean Blue

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(175, 238, 238),             // Azure

            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(175, 238, 238),            // Pale Turquoise
            HeaderForeColor = Color.FromArgb(0, 105, 148),              // Deep Ocean Blue
            GridLineColor = Color.FromArgb(0, 160, 176),                // Blue-Green
           
            RowForeColor = Color.FromArgb(0, 105, 148),                 // Deep Ocean Blue
                                                                        // **Grid Colors**
            RowBackColor = Color.FromArgb(240, 255, 255),               // Azure
            AltRowBackColor = Color.FromArgb(224, 255, 255),            // Light Cyan
            SelectedRowBackColor = Color.FromArgb(0, 160, 176),         // Blue-Green
            SelectedRowForeColor = Color.White,
            // **ComboBox Colors**
            ComboBoxBackColor = Color.White,
            ComboBoxForeColor = Color.FromArgb(0, 105, 148),            // Deep Ocean Blue

            // **CheckBox Colors**
            CheckBoxBackColor = Color.White,
            CheckBoxForeColor = Color.FromArgb(0, 105, 148),            // Deep Ocean Blue

            // **RadioButton Colors**
            RadioButtonBackColor = Color.White,
            RadioButtonForeColor = Color.FromArgb(0, 105, 148),         // Deep Ocean Blue

            // **Border Colors**
            BorderColor = Color.FromArgb(0, 160, 176),                  // Blue-Green
            ActiveBorderColor = Color.FromArgb(0, 139, 139),            // Dark Cyan
            InactiveBorderColor = Color.FromArgb(175, 238, 238),        // Pale Turquoise
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(0, 123, 167),                    // Deep Blue
            VisitedLinkColor = Color.FromArgb(72, 209, 204),            // Medium Turquoise
            HoverLinkColor = Color.FromArgb(0, 139, 139),               // Dark Cyan

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(0, 105, 148),             // Deep Ocean Blue
            ToolTipForeColor = Color.White,

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(224, 255, 255),         // Light Cyan
            ScrollBarThumbColor = Color.FromArgb(0, 139, 139),          // Dark Cyan
            ScrollBarTrackColor = Color.FromArgb(175, 238, 238),        // Pale Turquoise

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(0, 160, 176),           // Blue-Green
            StatusBarForeColor = Color.White,

            // **Tab Colors**
            TabBackColor = Color.FromArgb(224, 255, 255),               // Light Cyan
            TabForeColor = Color.FromArgb(0, 105, 148),                 // Deep Ocean Blue
            ActiveTabBackColor = Color.FromArgb(0, 160, 176),           // Blue-Green
            ActiveTabForeColor = Color.White,

            // **Dialog Box Colors**
            DialogBackColor = Color.FromArgb(240, 255, 255),            // Azure
            DialogForeColor = Color.FromArgb(0, 105, 148),              // Deep Ocean Blue
            DialogButtonBackColor = Color.FromArgb(0, 160, 176),        // Blue-Green
            DialogButtonForeColor = Color.White,

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(224, 255, 255),         // Light Cyan
            GradientEndColor = Color.FromArgb(175, 238, 238),           // Pale Turquoise
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(240, 255, 255),          // Azure
            SideMenuHoverBackColor = Color.FromArgb(224, 255, 255),     // Light Cyan
            SideMenuSelectedBackColor = Color.FromArgb(0, 160, 176),    // Blue-Green
            SideMenuForeColor = Color.FromArgb(0, 105, 148),            // Deep Ocean Blue
            SideMenuHoverForeColor = Color.FromArgb(0, 105, 148),       // Deep Ocean Blue
            SideMenuSelectedForeColor = Color.White,
            SideMenuIconColor = Color.FromArgb(0, 105, 148),            // Deep Ocean Blue
            SideMenuSelectedIconColor = Color.White,
            SideMenuBorderColor = Color.FromArgb(175, 238, 238),        // Pale Turquoise
        

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(175, 238, 238),            // Blue-Green
            TitleBarForeColor = Color.FromArgb(0, 105, 148),
            TitleBarHoverBackColor = Color.FromArgb(0, 139, 139),       // Dark Cyan
            TitleBarHoverForeColor = Color.White,

            // **Dashboard Colors**
            DashboardBackColor = Color.FromArgb(224, 255, 255),         // Light Cyan
            DashboardCardBackColor = Color.White,
            DashboardCardHoverBackColor = Color.FromArgb(240, 255, 255), // Azure
            CardTitleForeColor = Color.FromArgb(0, 105, 148),           // Deep Ocean Blue
            CardTextForeColor = Color.FromArgb(72, 209, 204),           // Medium Turquoise

            // **Data Visualization (Charts)**
            ChartBackColor = Color.White,
            ChartLineColor = Color.FromArgb(0, 160, 176),               // Blue-Green
            ChartFillColor = Color.FromArgb(100, 0, 160, 176),          // Semi-transparent Blue-Green
            ChartAxisColor = Color.FromArgb(0, 105, 148),               // Deep Ocean Blue

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(0, 105, 148),             // Deep Ocean Blue
            SidebarSelectedIconColor = Color.White,
            SidebarTextColor = Color.FromArgb(0, 105, 148),             // Deep Ocean Blue
            SidebarSelectedTextColor = Color.White,

            // **Navigation Colors**
            NavigationBackColor = Color.FromArgb(224, 255, 255),        // Light Cyan
            NavigationForeColor = Color.FromArgb(0, 105, 148),          // Deep Ocean Blue
            NavigationHoverBackColor = Color.FromArgb(175, 238, 238),   // Pale Turquoise
            NavigationHoverForeColor = Color.FromArgb(0, 105, 148),     // Deep Ocean Blue

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(0, 160, 176),               // Blue-Green
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(0, 139, 139),           // Dark Cyan

            // **Font Properties**
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(0, 105, 148),             // Deep Ocean Blue
            SecondaryTextColor = Color.FromArgb(0, 139, 139),           // Dark Cyan
            AccentTextColor = Color.FromArgb(0, 160, 176),              // Blue-Green

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(0, 105, 148), // Deep Ocean Blue
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(0, 105, 148), // Deep Ocean Blue
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(0, 105, 148), // Deep Ocean Blue
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(0, 105, 148), // Deep Ocean Blue
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(0, 105, 148), // Deep Ocean Blue
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(0, 105, 148), // Deep Ocean Blue
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(0, 105, 148), // Deep Ocean Blue
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(72, 209, 204), // Medium Turquoise
            },
            BlockquoteBorderColor = Color.FromArgb(0, 139, 139), // Dark Cyan
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 105, 148), // Deep Ocean Blue
            },
            InlineCodeBackgroundColor = Color.FromArgb(224, 255, 255), // Light Cyan
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 105, 148), // Deep Ocean Blue
            },
            CodeBlockBackgroundColor = Color.FromArgb(240, 255, 255), // Azure
            CodeBlockBorderColor = Color.FromArgb(0, 139, 139),       // Dark Cyan
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 105, 148), // Deep Ocean Blue
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 105, 148), // Deep Ocean Blue
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            LinkHoverColor = Color.FromArgb(0, 139, 139), // Dark Cyan
            LinkIsUnderline = true,

            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 105, 148), // Deep Ocean Blue
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 105, 148), // Deep Ocean Blue
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 105, 148), // Deep Ocean Blue
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 105, 148), // Deep Ocean Blue
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 105, 148),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 105, 148),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 105, 148),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 105, 148),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 105, 148),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(0, 105, 148),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(0, 105, 148),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(0, 105, 148),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 105, 148), // Deep Ocean Blue
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 105, 148),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 105, 148),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 105, 148),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 105, 148),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(0, 105, 148),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(0, 160, 176),      // Blue-Green
            SecondaryColor = Color.FromArgb(175, 238, 238),  // Pale Turquoise
            AccentColor = Color.FromArgb(0, 123, 167),       // Deep Blue
            BackgroundColor = Color.FromArgb(224, 255, 255), // Light Cyan
            SurfaceColor = Color.White,
            ErrorColor = Color.FromArgb(255, 69, 0),         // Orange-Red
            WarningColor = Color.FromArgb(255, 140, 0),      // Dark Orange
            SuccessColor = Color.FromArgb(60, 179, 113),     // Medium Sea Green
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.FromArgb(0, 105, 148), // Deep Ocean Blue

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 4,

            // **Imagery and Iconography**
            IconSet = "OceanIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(100, 0, 0, 0), // Semi-transparent black
            ShadowOpacity = 0.2f,

            // **Animation and Transitions**
            AnimationDurationShort = 150,  // in milliseconds
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(0, 123, 167), // Deep Blue

            // **Theme Variant**
            IsDarkTheme = false,
        };
        public static BeepTheme SunsetTheme => new BeepTheme
        {
            GridBackColor = Color.FromArgb(255, 228, 196), // Bisque
            GridForeColor = Color.DarkRed,
            GridHeaderBackColor = Color.FromArgb(255, 140, 0), // Dark Orange
            GridHeaderForeColor = Color.White,
            GridHeaderBorderColor = Color.DarkRed,
            GridHeaderHoverBackColor = Color.FromArgb(255, 69, 0), // Red Orange
            GridHeaderHoverForeColor = Color.White,
            GridHeaderSelectedBackColor = Color.FromArgb(255, 99, 71), // Tomato
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.DarkRed,
            GridHeaderSelectedBorderColor = Color.DarkRed,
            GridRowHoverBackColor = Color.FromArgb(255, 160, 122), // Light Salmon
            GridRowHoverForeColor = Color.DarkRed,
            GridRowSelectedBackColor = Color.Tomato,
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.DarkRed,
            GridRowSelectedBorderColor = Color.DarkRed,
            CardBackColor = Color.FromArgb(255, 160, 122),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 94, 77),  // Warm Sunset Orange
            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 165, 0),  // Golden Orange
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(255, 69, 0),               // Red-Orange
            MaxButtonColor = Color.FromArgb(255, 165, 0),                // Orange
            MinButtonColor = Color.FromArgb(255, 140, 0),                // Dark Orange
            TitleBarColor = Color.FromArgb(255, 99, 71),                 // Tomato
            TitleBarTextColor = Color.White,
            TitleBarIconColor = Color.White,
            TitleBarHoverColor = Color.FromArgb(255, 127, 80),           // Coral
            TitleBarHoverTextColor = Color.White,
            TitleBarHoverIconColor = Color.White,
            TitleBarActiveColor = Color.FromArgb(255, 69, 0),            // Red-Orange
            TitleBarActiveTextColor = Color.White,
            TitleBarActiveIconColor = Color.White,
            TitleBarInactiveColor = Color.FromArgb(255, 140, 0),         // Dark Orange
            TitleBarInactiveTextColor = Color.White,
            TitleBarInactiveIconColor = Color.White,
            TitleBarBorderColor = Color.FromArgb(255, 99, 71),           // Tomato
            TitleBarBorderHoverColor = Color.FromArgb(255, 127, 80),     // Coral
            TitleBarBorderActiveColor = Color.FromArgb(255, 69, 0),      // Red-Orange
            TitleBarBorderInactiveColor = Color.FromArgb(255, 140, 0),   // Dark Orange

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(255, 0, 0),         // Red
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(178, 34, 34),      // Firebrick
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(255, 99, 71),    // Tomato
            TitleBarCloseInactiveTextColor = Color.White,
            TitleBarCloseInactiveIconColor = Color.White,
            TitleBarCloseBorderColor = Color.FromArgb(255, 99, 71),      // Tomato
            TitleBarCloseBorderHoverColor = Color.FromArgb(255, 127, 80), // Coral
            TitleBarCloseBorderActiveColor = Color.FromArgb(255, 69, 0), // Red-Orange
            TitleBarCloseBorderInactiveColor = Color.FromArgb(255, 140, 0), // Dark Orange

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(255, 165, 0),         // Orange
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.White,
            TitleBarMaxActiveColor = Color.FromArgb(255, 99, 71),        // Tomato
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(255, 140, 0),      // Dark Orange
            TitleBarMaxInactiveTextColor = Color.White,
            TitleBarMaxInactiveIconColor = Color.White,
            TitleBarMaxBorderColor = Color.FromArgb(255, 99, 71),        // Tomato
            TitleBarMaxBorderHoverColor = Color.FromArgb(255, 127, 80),  // Coral
            TitleBarMaxBorderActiveColor = Color.FromArgb(255, 69, 0),   // Red-Orange
            TitleBarMaxBorderInactiveColor = Color.FromArgb(255, 140, 0), // Dark Orange

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(255, 140, 0),         // Dark Orange
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.White,
            TitleBarMinActiveColor = Color.FromArgb(255, 99, 71),        // Tomato
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(255, 140, 0),      // Dark Orange
            TitleBarMinInactiveTextColor = Color.White,
            TitleBarMinInactiveIconColor = Color.White,
            TitleBarMinBorderColor = Color.FromArgb(255, 99, 71),        // Tomato
            TitleBarMinBorderHoverColor = Color.FromArgb(255, 127, 80),  // Coral
            TitleBarMinBorderActiveColor = Color.FromArgb(255, 69, 0),   // Red-Orange
            TitleBarMinBorderInactiveColor = Color.FromArgb(255, 140, 0), // Dark Orange

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(255, 140, 0),    // Dark Orange
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(255, 99, 71),   // Tomato
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(255, 140, 0), // Dark Orange
            TitleBarMinimizeInactiveTextColor = Color.White,
            TitleBarMinimizeInactiveIconColor = Color.White,
            TitleBarMinimizeBorderColor = Color.FromArgb(255, 99, 71),   // Tomato
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(255, 127, 80), // Coral
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(255, 69, 0), // Red-Orange
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(255, 140, 0), // Dark Orange

            // **Typography Styles**
            TitleStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(255, 69, 0),                   // Red-Orange
            },
            SubtitleStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 99, 71),                  // Tomato
            },
            BodyStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 140, 0),                  // Dark Orange
            },
            CaptionStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 165, 0),                  // Orange
            },
            ButtonStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.White,                                  // White
            },
            LinkStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 165, 0),                  // Orange
            },
            OverlineStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 10f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 99, 71),                  // Tomato
            },

            // **General Colors**
            TitleForColor = Color.FromArgb(255, 69, 0),                  // Red-Orange
            TitleBarForColor = Color.FromArgb(255, 99, 71),              // Tomato
            DescriptionForColor = Color.FromArgb(255, 142, 128),         // Light Salmon
            BeforeForColor = Color.FromArgb(255, 179, 174),              // Light Pink
            LatestForColor = Color.FromArgb(255, 212, 173),              // Peach Puff
            BackColor = Color.FromArgb(255, 110, 102), // Light Salmon                   // Linen Background

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(255, 94, 77),               // Sunset Orange
            ButtonForeColor = Color.White,
            ButtonHoverBackColor = Color.FromArgb(227, 83, 68),          // Darker Sunset Orange
            ButtonHoverForeColor = Color.White,
            ButtonActiveBackColor = Color.FromArgb(199, 73, 60),         // Even Darker Sunset Orange
            ButtonActiveForeColor = Color.White,

            // **TextBox Colors**
            TextBoxBackColor = Color.White,
            TextBoxForeColor = Color.FromArgb(85, 85, 85),               // Dark Gray

            // **Label Colors**
            LabelBackColor = Color.White,
            LabelForeColor = Color.FromArgb(85, 85, 85),                 // Dark Gray

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(255, 223, 211), // Light Coral


            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(255, 212, 173),             // Peach Puff
            HeaderForeColor = Color.FromArgb(85, 85, 85),                // Dark Gray
            GridLineColor = Color.FromArgb(255, 179, 174),               // Light Pink
            RowBackColor = Color.White,
            RowForeColor = Color.FromArgb(85, 85, 85),
            AltRowBackColor = Color.FromArgb(255, 235, 211),             // Light Linen
            SelectedRowBackColor = Color.FromArgb(255, 142, 128),        // Light Salmon
            SelectedRowForeColor = Color.White,

            // **ComboBox Colors**
            ComboBoxBackColor = Color.White,
            ComboBoxForeColor = Color.FromArgb(85, 85, 85),

            // **CheckBox Colors**
            CheckBoxBackColor = Color.White,
            CheckBoxForeColor = Color.FromArgb(85, 85, 85),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.White,
            RadioButtonForeColor = Color.FromArgb(85, 85, 85),

            // **Border Colors**
            BorderColor = Color.FromArgb(255, 179, 174),                 // Light Pink
            ActiveBorderColor = Color.FromArgb(255, 142, 128),           // Light Salmon
            InactiveBorderColor = Color.FromArgb(255, 212, 173),         // Peach Puff
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(199, 73, 60),                     // Dark Sunset Orange
            VisitedLinkColor = Color.FromArgb(150, 60, 130),             // Plum
            HoverLinkColor = Color.FromArgb(227, 83, 68),                // Darker Sunset Orange
            LinkHoverColor = Color.FromArgb(227, 83, 68),                // Darker Sunset Orange
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(255, 94, 77),              // Sunset Orange
            ToolTipForeColor = Color.White,

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(255, 244, 229),          // Linen Background
            ScrollBarThumbColor = Color.FromArgb(255, 179, 174),         // Light Pink
            ScrollBarTrackColor = Color.FromArgb(255, 212, 173),         // Peach Puff

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(255, 94, 77),            // Sunset Orange
            StatusBarForeColor = Color.White,

            // **Tab Colors**
            TabBackColor = Color.FromArgb(255, 244, 229),                // Linen Background
            TabForeColor = Color.FromArgb(85, 85, 85),                   // Dark Gray
            ActiveTabBackColor = Color.FromArgb(255, 94, 77),            // Sunset Orange
            ActiveTabForeColor = Color.White,

            // **Dialog Box Colors**
            DialogBackColor = Color.FromArgb(255, 235, 211),             // Light Linen
            DialogForeColor = Color.FromArgb(85, 85, 85),
            DialogButtonBackColor = Color.FromArgb(255, 94, 77),         // Sunset Orange
            DialogButtonForeColor = Color.White,

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(255, 212, 173),          // Peach Puff
            GradientEndColor = Color.FromArgb(255, 94, 77),              // Sunset Orange
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(255, 160, 122), // Light Salmon

            SideMenuHoverBackColor = Color.FromArgb(255, 235, 211),      // Light Linen
            SideMenuSelectedBackColor = Color.FromArgb(255, 179, 174),   // Light Pink
            SideMenuForeColor = Color.FromArgb(85, 85, 85),
            SideMenuHoverForeColor = Color.FromArgb(85, 85, 85),
            SideMenuSelectedForeColor = Color.White,
            SideMenuBorderColor = Color.FromArgb(255, 212, 173),         // Peach Puff
            SideMenuIconColor = Color.FromArgb(85, 85, 85),
            SideMenuSelectedIconColor = Color.White,

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(255, 94, 77),             // Sunset Orange
            TitleBarForeColor = Color.White,
            TitleBarHoverBackColor = Color.FromArgb(227, 83, 68),        // Darker Sunset Orange
            TitleBarHoverForeColor = Color.White,

            // **Dashboard Colors**
            DashboardBackColor = Color.FromArgb(255, 244, 229),          // Linen Background
            DashboardCardBackColor = Color.White,
            DashboardCardHoverBackColor = Color.FromArgb(255, 235, 211), // Light Linen
            CardTitleForeColor = Color.FromArgb(85, 85, 85),
            CardTextForeColor = Color.FromArgb(150, 60, 130),            // Plum

            // **Data Visualization (Charts)**
            ChartBackColor = Color.White,
            ChartLineColor = Color.FromArgb(255, 94, 77),                // Sunset Orange
            ChartFillColor = Color.FromArgb(100, 255, 94, 77),           // Semi-transparent Sunset Orange
            ChartAxisColor = Color.FromArgb(85, 85, 85),

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(85, 85, 85),
            SidebarSelectedIconColor = Color.White,
            SidebarTextColor = Color.FromArgb(85, 85, 85),
            SidebarSelectedTextColor = Color.White,

            // **Navigation Colors**
            NavigationBackColor = Color.FromArgb(255, 244, 229),         // Linen Background
            NavigationForeColor = Color.FromArgb(85, 85, 85),
            NavigationHoverBackColor = Color.FromArgb(255, 235, 211),    // Light Linen
            NavigationHoverForeColor = Color.FromArgb(85, 85, 85),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(255, 94, 77),                // Sunset Orange
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(150, 60, 130),           // Plum

            // **Font Properties**
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(85, 85, 85),               // Dark Gray
            SecondaryTextColor = Color.FromArgb(130, 130, 130),          // Medium Gray
            AccentTextColor = Color.FromArgb(255, 94, 77),               // Sunset Orange

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(85, 85, 85),                  // Dark Gray
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),                  // Dark Gray
            },
            BlockquoteBorderColor = Color.FromArgb(255, 179, 174),       // Light Pink
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),                  // Dark Gray
            },
            InlineCodeBackgroundColor = Color.FromArgb(255, 235, 211),   // Light Linen
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),                  // Dark Gray
            },
            CodeBlockBackgroundColor = Color.FromArgb(255, 235, 211),    // Light Linen
            CodeBlockBorderColor = Color.FromArgb(255, 179, 174),        // Light Pink
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),                  // Dark Gray
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),                  // Dark Gray
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(255, 94, 77),                 // Sunset Orange
            SecondaryColor = Color.FromArgb(150, 60, 130),              // Plum
            AccentColor = Color.FromArgb(255, 142, 128),                // Light Salmon
            BackgroundColor = Color.FromArgb(255, 130, 122), // Light Salmon            // Linen
            SurfaceColor = Color.White,
            ErrorColor = Color.FromArgb(199, 73, 60),                   // Dark Sunset Orange
            WarningColor = Color.FromArgb(227, 83, 68),                 // Darker Sunset Orange
            SuccessColor = Color.FromArgb(85, 170, 85),                 // Medium Sea Green
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.FromArgb(85, 85, 85),

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 4,

            // **Imagery and Iconography**
            IconSet = "SunsetIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(100, 0, 0, 0),                 // Semi-transparent black
            ShadowOpacity = 0.2f,

            // **Animation and Transitions**
            AnimationDurationShort = 150,   // in milliseconds
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(255, 94, 77),          // Sunset Orange

            // **Theme Variant**
            IsDarkTheme = false,
        };
        public static BeepTheme ForestTheme => new BeepTheme
        {
            // Core Grid Colors
            GridBackColor = Color.FromArgb(34, 139, 34),          // Forest Green
            GridForeColor = Color.White,
            GridHeaderBackColor = Color.FromArgb(0, 100, 0),      // Dark Green
            GridHeaderForeColor = Color.White,
            GridHeaderBorderColor = Color.FromArgb(85, 107, 47),  // Olive Drab
            GridHeaderHoverBackColor = Color.FromArgb(60, 179, 113), // Medium Sea Green
            GridHeaderHoverForeColor = Color.White,
            GridHeaderSelectedBackColor = Color.FromArgb(50, 205, 50), // Lime Green
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.FromArgb(85, 107, 47),  // Olive Drab
            GridHeaderSelectedBorderColor = Color.FromArgb(46, 139, 87), // Sea Green
            GridRowHoverBackColor = Color.FromArgb(143, 188, 143),     // Dark Sea Green
            GridRowHoverForeColor = Color.White,
            GridRowSelectedBackColor = Color.FromArgb(50, 205, 50),    // Lime Green
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.FromArgb(85, 107, 47),     // Olive Drab
            GridRowSelectedBorderColor = Color.FromArgb(46, 139, 87),  // Sea Green

            CardBackColor = Color.FromArgb(107, 142, 35),              // Olive Green

            // Card Typography
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,
            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(245, 245, 220),  // Beige
            },

            // Title Bar and Window Controls
            CloseButtonColor = Color.FromArgb(139, 0, 0),         // Dark Red
            MaxButtonColor = Color.FromArgb(46, 139, 87),         // Sea Green
            MinButtonColor = Color.FromArgb(107, 142, 35),        // Olive Green
            TitleBarColor = Color.FromArgb(34, 139, 34),          // Forest Green
            TitleBarTextColor = Color.White,
            TitleBarIconColor = Color.FromArgb(245, 245, 220),    // Beige
            TitleBarHoverColor = Color.FromArgb(60, 179, 113),    // Medium Sea Green
            TitleBarHoverTextColor = Color.White,
            TitleBarHoverIconColor = Color.White,
            TitleBarActiveColor = Color.FromArgb(0, 100, 0),      // Dark Green
            TitleBarActiveTextColor = Color.White,
            TitleBarActiveIconColor = Color.White,
            TitleBarInactiveColor = Color.FromArgb(85, 107, 47),  // Olive Drab
            TitleBarInactiveTextColor = Color.FromArgb(245, 245, 220), // Beige
            TitleBarInactiveIconColor = Color.FromArgb(245, 245, 220), // Beige
            TitleBarBorderColor = Color.FromArgb(46, 139, 87),         // Sea Green
            TitleBarBorderHoverColor = Color.FromArgb(60, 179, 113),   // Medium Sea Green
            TitleBarBorderActiveColor = Color.FromArgb(0, 100, 0),     // Dark Green
            TitleBarBorderInactiveColor = Color.FromArgb(85, 107, 47), // Olive Drab

            // Close Button States
            TitleBarCloseHoverColor = Color.FromArgb(165, 42, 42),     // Brown
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(139, 0, 0),      // Dark Red
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(85, 107, 47),  // Olive Drab
            TitleBarCloseInactiveTextColor = Color.FromArgb(245, 245, 220), // Beige
            TitleBarCloseInactiveIconColor = Color.FromArgb(245, 245, 220), // Beige
            TitleBarCloseBorderColor = Color.FromArgb(46, 139, 87),    // Sea Green
            TitleBarCloseBorderHoverColor = Color.FromArgb(60, 179, 113), // Medium Sea Green
            TitleBarCloseBorderActiveColor = Color.FromArgb(0, 100, 0),   // Dark Green
            TitleBarCloseBorderInactiveColor = Color.FromArgb(85, 107, 47), // Olive Drab

            // Maximize Button States
            TitleBarMaxHoverColor = Color.FromArgb(60, 179, 113),      // Medium Sea Green
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.White,
            TitleBarMaxActiveColor = Color.FromArgb(0, 128, 0),        // Green
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(85, 107, 47),    // Olive Drab
            TitleBarMaxInactiveTextColor = Color.FromArgb(245, 245, 220), // Beige
            TitleBarMaxInactiveIconColor = Color.FromArgb(245, 245, 220), // Beige
            TitleBarMaxBorderColor = Color.FromArgb(46, 139, 87),      // Sea Green
            TitleBarMaxBorderHoverColor = Color.FromArgb(60, 179, 113), // Medium Sea Green
            TitleBarMaxBorderActiveColor = Color.FromArgb(0, 100, 0),  // Dark Green
            TitleBarMaxBorderInactiveColor = Color.FromArgb(85, 107, 47), // Olive Drab

            // Minimize Button States
            TitleBarMinHoverColor = Color.FromArgb(107, 142, 35),      // Olive Green
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.White,
            TitleBarMinActiveColor = Color.FromArgb(34, 139, 34),      // Forest Green
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(85, 107, 47),    // Olive Drab
            TitleBarMinInactiveTextColor = Color.FromArgb(245, 245, 220), // Beige
            TitleBarMinInactiveIconColor = Color.FromArgb(245, 245, 220), // Beige
            TitleBarMinBorderColor = Color.FromArgb(46, 139, 87),      // Sea Green
            TitleBarMinBorderHoverColor = Color.FromArgb(60, 179, 113), // Medium Sea Green
            TitleBarMinBorderActiveColor = Color.FromArgb(0, 100, 0),  // Dark Green
            TitleBarMinBorderInactiveColor = Color.FromArgb(85, 107, 47), // Olive Drab

            // Alternative Minimize Button States
            TitleBarMinimizeHoverColor = Color.FromArgb(107, 142, 35),  // Olive Green
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(34, 139, 34),  // Forest Green
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(85, 107, 47), // Olive Drab
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(245, 245, 220), // Beige
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(245, 245, 220), // Beige
            TitleBarMinimizeBorderColor = Color.FromArgb(46, 139, 87),  // Sea Green
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(60, 179, 113), // Medium Sea Green
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(0, 100, 0), // Dark Green
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(85, 107, 47), // Olive Drab

            // General Colors
            TitleForColor = Color.White,
            TitleBarForColor = Color.White,
            DescriptionForColor = Color.FromArgb(245, 245, 220),       // Beige
            BeforeForColor = Color.FromArgb(107, 142, 35),             // Olive Green
            LatestForColor = Color.FromArgb(60, 179, 113),             // Medium Sea Green
            BackColor = Color.FromArgb(245, 245, 220),                 // Beige

            // Button Colors
            ButtonBackColor = Color.FromArgb(34, 139, 34),             // Forest Green
            ButtonForeColor = Color.White,
            ButtonHoverBackColor = Color.FromArgb(60, 179, 113),       // Medium Sea Green
            ButtonHoverForeColor = Color.White,
            ButtonActiveBackColor = Color.FromArgb(0, 100, 0),         // Dark Green
            ButtonActiveForeColor = Color.White,

            // TextBox Colors
            TextBoxBackColor = Color.FromArgb(245, 245, 220),          // Beige
            TextBoxForeColor = Color.FromArgb(34, 139, 34),            // Forest Green

            // Label Colors
            LabelBackColor = Color.FromArgb(245, 245, 220),            // Beige
            LabelForeColor = Color.FromArgb(34, 139, 34),              // Forest Green

            // Panel Colors
            PanelBackColor = Color.FromArgb(107, 142, 35),             // Olive Green

            // Grid Colors
            HeaderBackColor = Color.FromArgb(0, 100, 0),               // Dark Green
            HeaderForeColor = Color.White,
            GridLineColor = Color.FromArgb(85, 107, 47),               // Olive Drab
            RowBackColor = Color.FromArgb(245, 245, 220),              // Beige
            RowForeColor = Color.FromArgb(34, 139, 34),                // Forest Green
            AltRowBackColor = Color.FromArgb(240, 230, 140),           // Khaki
            SelectedRowBackColor = Color.FromArgb(60, 179, 113),       // Medium Sea Green
            SelectedRowForeColor = Color.White,

            // ComboBox Colors
            ComboBoxBackColor = Color.FromArgb(245, 245, 220),         // Beige
            ComboBoxForeColor = Color.FromArgb(34, 139, 34),           // Forest Green

            // CheckBox Colors
            CheckBoxBackColor = Color.FromArgb(245, 245, 220),         // Beige
            CheckBoxForeColor = Color.FromArgb(34, 139, 34),           // Forest Green

            // RadioButton Colors
            RadioButtonBackColor = Color.FromArgb(245, 245, 220),      // Beige
            RadioButtonForeColor = Color.FromArgb(34, 139, 34),        // Forest Green

            // Border Colors
            BorderColor = Color.FromArgb(85, 107, 47),                 // Olive Drab
            ActiveBorderColor = Color.FromArgb(34, 139, 34),           // Forest Green
            InactiveBorderColor = Color.FromArgb(107, 142, 35),        // Olive Green
            BorderSize = 1,

            // Link Colors
            LinkColor = Color.FromArgb(34, 139, 34),                   // Forest Green
            VisitedLinkColor = Color.FromArgb(85, 107, 47),            // Olive Drab
            HoverLinkColor = Color.FromArgb(60, 179, 113),             // Medium Sea Green
            LinkHoverColor = Color.FromArgb(60, 179, 113),             // Medium Sea Green
            LinkIsUnderline = true,

            // ToolTip Colors
            ToolTipBackColor = Color.FromArgb(107, 142, 35),           // Olive Green
            ToolTipForeColor = Color.White,

            // ScrollBar Colors
            ScrollBarBackColor = Color.FromArgb(245, 245, 220),        // Beige
            ScrollBarThumbColor = Color.FromArgb(85, 107, 47),         // Olive Drab
            ScrollBarTrackColor = Color.FromArgb(240, 230, 140),       // Khaki

            // Status Bar Colors
            StatusBarBackColor = Color.FromArgb(0, 100, 0),            // Dark Green
            StatusBarForeColor = Color.White,

            // Tab Colors
            TabBackColor = Color.FromArgb(245, 245, 220),              // Beige
            TabForeColor = Color.FromArgb(34, 139, 34),                // Forest Green
            ActiveTabBackColor = Color.FromArgb(60, 179, 113),         // Medium Sea Green
            ActiveTabForeColor = Color.White,

            // Dialog Box Colors
            DialogBackColor = Color.FromArgb(245, 245, 220),           // Beige
            DialogForeColor = Color.FromArgb(34, 139, 34),             // Forest Green
            DialogButtonBackColor = Color.FromArgb(34, 139, 34),       // Forest Green
            DialogButtonForeColor = Color.White,

            // Gradient Properties
            GradientStartColor = Color.FromArgb(245, 245, 220),        // Beige
            GradientEndColor = Color.FromArgb(107, 142, 35),           // Olive Green
            GradientDirection = LinearGradientMode.Vertical,

            // Side Menu Colors
            SideMenuBackColor = Color.FromArgb(34, 139, 34),           // Forest Green
            SideMenuHoverBackColor = Color.FromArgb(60, 179, 113),     // Medium Sea Green
            SideMenuSelectedBackColor = Color.FromArgb(0, 100, 0),     // Dark Green
            SideMenuForeColor = Color.White,
            SideMenuHoverForeColor = Color.White,
            SideMenuSelectedForeColor = Color.White,
            SideMenuBorderColor = Color.FromArgb(85, 107, 47),         // Olive Drab
            SideMenuIconColor = Color.FromArgb(245, 245, 220),         // Beige
            SideMenuSelectedIconColor = Color.White,

            // Title Bar Colors (Repeated Properties)
            TitleBarBackColor = Color.FromArgb(0, 100, 0),             // Dark Green
            TitleBarForeColor = Color.White,
            TitleBarHoverBackColor = Color.FromArgb(60, 179, 113),     // Medium Sea Green
            TitleBarHoverForeColor = Color.White,

            // Dashboard Colors
            DashboardBackColor = Color.FromArgb(245, 245, 220),        // Beige
            DashboardCardBackColor = Color.FromArgb(240, 230, 140),    // Khaki
            DashboardCardHoverBackColor = Color.FromArgb(107, 142, 35), // Olive Green
            CardTitleForeColor = Color.FromArgb(34, 139, 34),          // Forest Green
            CardTextForeColor = Color.FromArgb(245, 245, 220),         // Beige

            // Data Visualization (Charts)
            ChartBackColor = Color.FromArgb(245, 245, 220),            // Beige
            ChartLineColor = Color.FromArgb(34, 139, 34),              // Forest Green
            ChartFillColor = Color.FromArgb(107, 142, 35),             // Olive Green
            ChartAxisColor = Color.FromArgb(85, 107, 47),              // Olive Drab

            // Sidebar and Menu Colors
            SidebarIconColor = Color.FromArgb(34, 139, 34),            // Forest Green
            SidebarSelectedIconColor = Color.White,
            SidebarTextColor = Color.FromArgb(34, 139, 34),            // Forest Green
            SidebarSelectedTextColor = Color.White,

            // Navigation Colors
            NavigationBackColor = Color.FromArgb(245, 245, 220),       // Beige
            NavigationForeColor = Color.FromArgb(34, 139, 34),         // Forest Green
            NavigationHoverBackColor = Color.FromArgb(60, 179, 113),   // Medium Sea Green
            NavigationHoverForeColor = Color.White,

            // Badge and Highlight Colors
            BadgeBackColor = Color.FromArgb(34, 139, 34),              // Forest Green
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(107, 142, 35),         // Olive Green

            // Font Properties
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // Font Sizes
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // Font Styles
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // Text Colors
            PrimaryTextColor = Color.FromArgb(34, 139, 34),            // Forest Green
            SecondaryTextColor = Color.FromArgb(85, 107, 47),          // Olive Drab
            AccentTextColor = Color.FromArgb(60, 179, 113),            // Medium Sea Green

            // Typography Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 107, 47),              // Olive Drab
            },
            BlockquoteBorderColor = Color.FromArgb(107, 142, 35),      // Olive Green
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,
            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            InlineCodeBackgroundColor = Color.FromArgb(240, 230, 140), // Khaki
            InlineCodePadding = 4f,
            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            CodeBlockBackgroundColor = Color.FromArgb(240, 230, 140),  // Khaki
            CodeBlockBorderColor = Color.FromArgb(85, 107, 47),        // Olive Drab
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,
            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,
            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(34, 139, 34),              // Forest Green
            },

            // Color Palette
            PrimaryColor = Color.FromArgb(34, 139, 34),                // Forest Green
            SecondaryColor = Color.FromArgb(85, 107, 47),              // Olive Drab
            AccentColor = Color.FromArgb(60, 179, 113),                // Medium Sea Green
            BackgroundColor = Color.FromArgb(245, 245, 220),           // Beige
            SurfaceColor = Color.FromArgb(240, 230, 140),              // Khaki
            ErrorColor = Color.FromArgb(139, 0, 0),                    // Dark Red
            WarningColor = Color.FromArgb(205, 92, 92),                // Indian Red
            SuccessColor = Color.FromArgb(0, 100, 0),                  // Dark Green
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.FromArgb(34, 139, 34),           // Forest Green

            // Spacing and Layout
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 4,

            // Imagery and Iconography
            IconSet = "ForestIcons",
            ApplyThemeToIcons = true,

            // Effects and Decorations
            ShadowColor = Color.FromArgb(64, 64, 64),                 // Dark Gray
            ShadowOpacity = 0.2f,

            // Animation and Transitions
            AnimationDurationShort = 150,  // in milliseconds
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",

            // Accessibility
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(60, 179, 113),       // Medium Sea Green

            // Theme Variant
            IsDarkTheme = false,
        };
        public static BeepTheme AutumnTheme => new BeepTheme
        {
            GridBackColor = Color.FromArgb(255, 239, 213), // Papaya Whip
            GridForeColor = Color.DarkOrange,
            GridHeaderBackColor = Color.FromArgb(255, 165, 0), // Orange
            GridHeaderForeColor = Color.DarkRed,
            GridHeaderBorderColor = Color.Brown,
            GridHeaderHoverBackColor = Color.FromArgb(255, 140, 0), // Dark Orange
            GridHeaderHoverForeColor = Color.DarkRed,
            GridHeaderSelectedBackColor = Color.FromArgb(139, 69, 19), // SaddleBrown
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.Brown,
            GridHeaderSelectedBorderColor = Color.Brown,
            GridRowHoverBackColor = Color.FromArgb(245, 222, 179), // Wheat
            GridRowHoverForeColor = Color.DarkOrange,
            GridRowSelectedBackColor = Color.SaddleBrown,
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.Brown,
            GridRowSelectedBorderColor = Color.Brown,
            CardBackColor = Color.FromArgb(205, 92, 92),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,  // Indian Red
            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(139, 69, 19),  // Saddle Brown
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(165, 42, 42),               // Brown
            MaxButtonColor = Color.FromArgb(210, 105, 30),                // Chocolate
            MinButtonColor = Color.FromArgb(255, 140, 0),                 // Dark Orange
            TitleBarColor = Color.FromArgb(205, 92, 92),                  // Indian Red
            TitleBarTextColor = Color.FromArgb(139, 69, 19),              // Saddle Brown
            TitleBarIconColor = Color.FromArgb(255, 99, 71),              // Tomato
            TitleBarHoverColor = Color.FromArgb(222, 184, 135),           // Burlywood
            TitleBarHoverTextColor = Color.FromArgb(139, 69, 19),         // Saddle Brown
            TitleBarHoverIconColor = Color.FromArgb(210, 105, 30),        // Chocolate
            TitleBarActiveColor = Color.FromArgb(244, 164, 96),           // Sandy Brown
            TitleBarActiveTextColor = Color.FromArgb(139, 69, 19),        // Saddle Brown
            TitleBarActiveIconColor = Color.FromArgb(255, 127, 80),       // Coral
            TitleBarInactiveColor = Color.FromArgb(205, 133, 63),         // Peru
            TitleBarInactiveTextColor = Color.FromArgb(139, 69, 19),      // Saddle Brown
            TitleBarInactiveIconColor = Color.FromArgb(210, 105, 30),     // Chocolate
            TitleBarBorderColor = Color.FromArgb(160, 82, 45),            // Sienna
            TitleBarBorderHoverColor = Color.FromArgb(205, 133, 63),      // Peru
            TitleBarBorderActiveColor = Color.FromArgb(139, 69, 19),      // Saddle Brown
            TitleBarBorderInactiveColor = Color.FromArgb(160, 82, 45),    // Sienna

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(165, 42, 42),        // Brown
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(139, 0, 0),         // Dark Red
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(165, 42, 42),     // Brown
            TitleBarCloseInactiveTextColor = Color.White,
            TitleBarCloseInactiveIconColor = Color.White,
            TitleBarCloseBorderColor = Color.FromArgb(160, 82, 45),       // Sienna
            TitleBarCloseBorderHoverColor = Color.FromArgb(205, 133, 63), // Peru
            TitleBarCloseBorderActiveColor = Color.FromArgb(139, 69, 19), // Saddle Brown
            TitleBarCloseBorderInactiveColor = Color.FromArgb(160, 82, 45), // Sienna

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(255, 140, 0),          // Dark Orange
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.White,
            TitleBarMaxActiveColor = Color.FromArgb(210, 105, 30),        // Chocolate
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(205, 133, 63),      // Peru
            TitleBarMaxInactiveTextColor = Color.White,
            TitleBarMaxInactiveIconColor = Color.White,
            TitleBarMaxBorderColor = Color.FromArgb(160, 82, 45),         // Sienna
            TitleBarMaxBorderHoverColor = Color.FromArgb(205, 133, 63),   // Peru
            TitleBarMaxBorderActiveColor = Color.FromArgb(139, 69, 19),   // Saddle Brown
            TitleBarMaxBorderInactiveColor = Color.FromArgb(160, 82, 45), // Sienna

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(255, 165, 0),          // Orange
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.White,
            TitleBarMinActiveColor = Color.FromArgb(210, 105, 30),        // Chocolate
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(205, 133, 63),      // Peru
            TitleBarMinInactiveTextColor = Color.White,
            TitleBarMinInactiveIconColor = Color.White,
            TitleBarMinBorderColor = Color.FromArgb(160, 82, 45),         // Sienna
            TitleBarMinBorderHoverColor = Color.FromArgb(205, 133, 63),   // Peru
            TitleBarMinBorderActiveColor = Color.FromArgb(139, 69, 19),   // Saddle Brown
            TitleBarMinBorderInactiveColor = Color.FromArgb(160, 82, 45), // Sienna

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(255, 140, 0),     // Dark Orange
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(210, 105, 30),   // Chocolate
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(205, 133, 63), // Peru
            TitleBarMinimizeInactiveTextColor = Color.White,
            TitleBarMinimizeInactiveIconColor = Color.White,
            TitleBarMinimizeBorderColor = Color.FromArgb(160, 82, 45),    // Sienna
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(205, 133, 63), // Peru
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(139, 69, 19), // Saddle Brown
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(160, 82, 45), // Sienna

            // **General Colors**
            TitleForColor = Color.FromArgb(139, 69, 19),                  // Saddle Brown
            TitleBarForColor = Color.FromArgb(205, 133, 63),              // Peru
            DescriptionForColor = Color.FromArgb(210, 105, 30),           // Chocolate
            BeforeForColor = Color.FromArgb(205, 92, 92),                 // Indian Red
            LatestForColor = Color.FromArgb(218, 165, 32),                // Golden Rod
            BackColor = Color.FromArgb(200, 180, 135),                   // Cornsilk background

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(205, 133, 63),               // Peru
            ButtonForeColor = Color.White,
            ButtonHoverBackColor = Color.FromArgb(160, 82, 45),           // Sienna
            ButtonHoverForeColor = Color.White,
            ButtonActiveBackColor = Color.FromArgb(139, 69, 19),          // Saddle Brown
            ButtonActiveForeColor = Color.White,

            // **TextBox Colors**
            TextBoxBackColor = Color.FromArgb(205, 133, 63),
            TextBoxForeColor = Color.White,                // Dark Gray

            // **Label Colors**
            LabelBackColor = Color.FromArgb(205, 133, 63),
            LabelForeColor = Color.White,

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(250, 200, 200), // Antique White


            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(222, 184, 135),              // Burlywood
            HeaderForeColor = Color.FromArgb(85, 85, 85),
            GridLineColor = Color.FromArgb(210, 105, 30),                 // Chocolate
            RowBackColor = Color.White,
            RowForeColor = Color.FromArgb(85, 85, 85),
            AltRowBackColor = Color.FromArgb(250, 235, 215),              // Antique White
            SelectedRowBackColor = Color.FromArgb(210, 105, 30),          // Chocolate
            SelectedRowForeColor = Color.White,

            // **ComboBox Colors**
            ComboBoxBackColor = Color.White,
            ComboBoxForeColor = Color.FromArgb(85, 85, 85),

            // **CheckBox Colors**
            CheckBoxBackColor = Color.White,
            CheckBoxForeColor = Color.FromArgb(85, 85, 85),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.White,
            RadioButtonForeColor = Color.FromArgb(85, 85, 85),

            // **Border Colors**
            BorderColor = Color.FromArgb(210, 105, 30),                   // Chocolate
            ActiveBorderColor = Color.FromArgb(160, 82, 45),              // Sienna
            InactiveBorderColor = Color.FromArgb(222, 184, 135),          // Burlywood
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(205, 92, 92),                      // Indian Red
            VisitedLinkColor = Color.FromArgb(139, 69, 19),               // Saddle Brown
            HoverLinkColor = Color.FromArgb(160, 82, 45),                 // Sienna
            LinkHoverColor = Color.FromArgb(160, 82, 45),                 // Sienna
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(160, 82, 45),               // Sienna
            ToolTipForeColor = Color.White,

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(255, 248, 220),           // Cornsilk
            ScrollBarThumbColor = Color.FromArgb(210, 105, 30),           // Chocolate
            ScrollBarTrackColor = Color.FromArgb(222, 184, 135),          // Burlywood

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(205, 133, 63),            // Peru
            StatusBarForeColor = Color.White,

            // **Tab Colors**
            TabBackColor = Color.FromArgb(255, 248, 220),                 // Cornsilk
            TabForeColor = Color.FromArgb(85, 85, 85),
            ActiveTabBackColor = Color.FromArgb(205, 92, 92),             // Indian Red
            ActiveTabForeColor = Color.White,

            // **Dialog Box Colors**
            DialogBackColor = Color.FromArgb(250, 235, 215),              // Antique White
            DialogForeColor = Color.FromArgb(85, 85, 85),
            DialogButtonBackColor = Color.FromArgb(205, 133, 63),         // Peru
            DialogButtonForeColor = Color.White,

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(255, 248, 220),           // Cornsilk
            GradientEndColor = Color.FromArgb(210, 105, 30),              // Chocolate
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(222, 184, 135), // Burlywood

            SideMenuHoverBackColor = Color.FromArgb(250, 235, 215),       // Antique White
            SideMenuSelectedBackColor = Color.FromArgb(210, 105, 30),     // Chocolate
            SideMenuForeColor = Color.FromArgb(85, 85, 85),
            SideMenuHoverForeColor = Color.FromArgb(85, 85, 85),
            SideMenuSelectedForeColor = Color.White,
            SideMenuBorderColor = Color.FromArgb(222, 184, 135),          // Burlywood
            SideMenuIconColor = Color.FromArgb(85, 85, 85),
            SideMenuSelectedIconColor = Color.White,

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(139, 69, 19),              // Saddle Brown
            TitleBarForeColor = Color.White,
            TitleBarHoverBackColor = Color.FromArgb(160, 82, 45),         // Sienna
            TitleBarHoverForeColor = Color.White,

            // **Dashboard Colors**
            DashboardBackColor = Color.FromArgb(255, 248, 220),           // Cornsilk
            DashboardCardBackColor = Color.White,
            DashboardCardHoverBackColor = Color.FromArgb(250, 235, 215),  // Antique White
            CardTitleForeColor = Color.FromArgb(85, 85, 85),
            CardTextForeColor = Color.FromArgb(139, 69, 19),              // Saddle Brown

            // **Data Visualization (Charts)**
            ChartBackColor = Color.White,
            ChartLineColor = Color.FromArgb(205, 92, 92),                 // Indian Red
            ChartFillColor = Color.FromArgb(100, 205, 92, 92),            // Semi-transparent Indian Red
            ChartAxisColor = Color.FromArgb(85, 85, 85),

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(85, 85, 85),
            SidebarSelectedIconColor = Color.White,
            SidebarTextColor = Color.FromArgb(85, 85, 85),
            SidebarSelectedTextColor = Color.White,

            // **Navigation Colors**
            NavigationBackColor = Color.FromArgb(255, 248, 220),          // Cornsilk
            NavigationForeColor = Color.FromArgb(85, 85, 85),
            NavigationHoverBackColor = Color.FromArgb(250, 235, 215),     // Antique White
            NavigationHoverForeColor = Color.FromArgb(85, 85, 85),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(205, 92, 92),                 // Indian Red
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(218, 165, 32),            // Golden Rod

            // **Font Properties**
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(85, 85, 85),                // Dark Gray
            SecondaryTextColor = Color.FromArgb(130, 130, 130),           // Medium Gray
            AccentTextColor = Color.FromArgb(205, 92, 92),                // Indian Red

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(139, 69, 19), // Saddle Brown
            },
            BlockquoteBorderColor = Color.FromArgb(160, 82, 45), // Sienna
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            InlineCodeBackgroundColor = Color.FromArgb(250, 235, 215), // Antique White
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            CodeBlockBackgroundColor = Color.FromArgb(250, 235, 215), // Antique White
            CodeBlockBorderColor = Color.FromArgb(160, 82, 45),       // Sienna
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),                // Dark Gray
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(205, 92, 92),          // Indian Red
            SecondaryColor = Color.FromArgb(218, 165, 32),       // Golden Rod
            AccentColor = Color.FromArgb(210, 105, 30),          // Chocolate
            BackgroundColor = Color.FromArgb(222, 210, 155),
            SurfaceColor = Color.White,
            ErrorColor = Color.FromArgb(139, 0, 0),              // Dark Red
            WarningColor = Color.FromArgb(255, 140, 0),          // Dark Orange
            SuccessColor = Color.FromArgb(34, 139, 34),          // Forest Green
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.FromArgb(85, 85, 85),

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 4,

            // **Imagery and Iconography**
            IconSet = "AutumnIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(100, 0, 0, 0), // Semi-transparent black
            ShadowOpacity = 0.2f,

            // **Animation and Transitions**
            AnimationDurationShort = 150,  // in milliseconds
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(205, 92, 92),   // Indian Red

            // **Theme Variant**
            IsDarkTheme = false,
        };
        public static BeepTheme EarthyTheme => new BeepTheme
        {
            GridBackColor = Color.FromArgb(245, 222, 179), // Wheat
            GridForeColor = Color.SaddleBrown,
            GridHeaderBackColor = Color.FromArgb(210, 180, 140), // Tan
            GridHeaderForeColor = Color.SaddleBrown,
            GridHeaderBorderColor = Color.Brown,
            GridHeaderHoverBackColor = Color.FromArgb(139, 69, 19), // SaddleBrown
            GridHeaderHoverForeColor = Color.White,
            GridHeaderSelectedBackColor = Color.FromArgb(160, 82, 45), // Sienna
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.SaddleBrown,
            GridHeaderSelectedBorderColor = Color.Brown,
            GridRowHoverBackColor = Color.FromArgb(238, 232, 170), // Pale Goldenrod
            GridRowHoverForeColor = Color.SaddleBrown,
            GridRowSelectedBackColor = Color.Sienna,
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.Brown,
            GridRowSelectedBorderColor = Color.Sienna,
            CardBackColor = Color.SandyBrown,
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,  // Dark Olive Green
            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(139, 69, 19),  // Saddle Brown
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(139, 69, 19),              // Saddle Brown
            MaxButtonColor = Color.FromArgb(85, 107, 47),                // Dark Olive Green
            MinButtonColor = Color.FromArgb(160, 82, 45),                // Sienna
            TitleBarColor = Color.FromArgb(210, 180, 140),               // Tan
            TitleBarTextColor = Color.FromArgb(34, 139, 34),             // Forest Green
            TitleBarIconColor = Color.FromArgb(139, 69, 19),             // Saddle Brown
            TitleBarHoverColor = Color.FromArgb(222, 184, 135),          // Burlywood
            TitleBarHoverTextColor = Color.FromArgb(85, 107, 47),        // Dark Olive Green
            TitleBarHoverIconColor = Color.FromArgb(160, 82, 45),        // Sienna
            TitleBarActiveColor = Color.FromArgb(210, 180, 140),         // Tan
            TitleBarActiveTextColor = Color.FromArgb(34, 139, 34),       // Forest Green
            TitleBarActiveIconColor = Color.FromArgb(139, 69, 19),       // Saddle Brown
            TitleBarInactiveColor = Color.FromArgb(188, 143, 143),       // Rosy Brown
            TitleBarInactiveTextColor = Color.FromArgb(85, 107, 47),     // Dark Olive Green
            TitleBarInactiveIconColor = Color.FromArgb(160, 82, 45),     // Sienna
            TitleBarBorderColor = Color.FromArgb(85, 107, 47),           // Dark Olive Green
            TitleBarBorderHoverColor = Color.FromArgb(160, 82, 45),      // Sienna
            TitleBarBorderActiveColor = Color.FromArgb(34, 139, 34),     // Forest Green
            TitleBarBorderInactiveColor = Color.FromArgb(188, 143, 143), // Rosy Brown

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(218, 165, 32),      // Goldenrod
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(184, 134, 11),     // Dark Goldenrod
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(188, 143, 143),  // Rosy Brown
            TitleBarCloseInactiveTextColor = Color.White,
            TitleBarCloseInactiveIconColor = Color.White,
            TitleBarCloseBorderColor = Color.FromArgb(85, 107, 47),      // Dark Olive Green
            TitleBarCloseBorderHoverColor = Color.FromArgb(160, 82, 45), // Sienna
            TitleBarCloseBorderActiveColor = Color.FromArgb(34, 139, 34), // Forest Green
            TitleBarCloseBorderInactiveColor = Color.FromArgb(188, 143, 143), // Rosy Brown

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(222, 184, 135),       // Burlywood
            TitleBarMaxHoverTextColor = Color.FromArgb(85, 107, 47),     // Dark Olive Green
            TitleBarMaxHoverIconColor = Color.FromArgb(85, 107, 47),
            TitleBarMaxActiveColor = Color.FromArgb(210, 180, 140),      // Tan
            TitleBarMaxActiveTextColor = Color.FromArgb(34, 139, 34),    // Forest Green
            TitleBarMaxActiveIconColor = Color.FromArgb(34, 139, 34),
            TitleBarMaxInactiveColor = Color.FromArgb(188, 143, 143),    // Rosy Brown
            TitleBarMaxInactiveTextColor = Color.FromArgb(85, 107, 47),
            TitleBarMaxInactiveIconColor = Color.FromArgb(85, 107, 47),
            TitleBarMaxBorderColor = Color.FromArgb(85, 107, 47),        // Dark Olive Green
            TitleBarMaxBorderHoverColor = Color.FromArgb(160, 82, 45),   // Sienna
            TitleBarMaxBorderActiveColor = Color.FromArgb(34, 139, 34),  // Forest Green
            TitleBarMaxBorderInactiveColor = Color.FromArgb(188, 143, 143), // Rosy Brown

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(222, 184, 135),       // Burlywood
            TitleBarMinHoverTextColor = Color.FromArgb(85, 107, 47),     // Dark Olive Green
            TitleBarMinHoverIconColor = Color.FromArgb(85, 107, 47),
            TitleBarMinActiveColor = Color.FromArgb(210, 180, 140),      // Tan
            TitleBarMinActiveTextColor = Color.FromArgb(34, 139, 34),    // Forest Green
            TitleBarMinActiveIconColor = Color.FromArgb(34, 139, 34),
            TitleBarMinInactiveColor = Color.FromArgb(188, 143, 143),    // Rosy Brown
            TitleBarMinInactiveTextColor = Color.FromArgb(85, 107, 47),
            TitleBarMinInactiveIconColor = Color.FromArgb(85, 107, 47),
            TitleBarMinBorderColor = Color.FromArgb(85, 107, 47),        // Dark Olive Green
            TitleBarMinBorderHoverColor = Color.FromArgb(160, 82, 45),   // Sienna
            TitleBarMinBorderActiveColor = Color.FromArgb(34, 139, 34),  // Forest Green
            TitleBarMinBorderInactiveColor = Color.FromArgb(188, 143, 143), // Rosy Brown

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(222, 184, 135),  // Burlywood
            TitleBarMinimizeHoverTextColor = Color.FromArgb(85, 107, 47),
            TitleBarMinimizeHoverIconColor = Color.FromArgb(85, 107, 47),
            TitleBarMinimizeActiveColor = Color.FromArgb(210, 180, 140), // Tan
            TitleBarMinimizeActiveTextColor = Color.FromArgb(34, 139, 34),
            TitleBarMinimizeActiveIconColor = Color.FromArgb(34, 139, 34),
            TitleBarMinimizeInactiveColor = Color.FromArgb(188, 143, 143), // Rosy Brown
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(85, 107, 47),
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(85, 107, 47),
            TitleBarMinimizeBorderColor = Color.FromArgb(85, 107, 47),   // Dark Olive Green
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(160, 82, 45), // Sienna
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(34, 139, 34), // Forest Green
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(188, 143, 143), // Rosy Brown

            // **General Colors**
            TitleForColor = Color.White,                 // Dark Olive Green
            TitleBarForColor = Color.FromArgb(139, 69, 19),              // Saddle Brown
            DescriptionForColor = Color.FromArgb(160, 82, 45),           // Sienna
            BeforeForColor = Color.FromArgb(222, 184, 135),              // Burlywood
            LatestForColor = Color.FromArgb(218, 165, 32),               // Goldenrod
            BackColor = Color.FromArgb(245, 245, 220),                   // Beige

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(139, 69, 19),               // Saddle Brown
            ButtonForeColor = Color.White,
            ButtonHoverBackColor = Color.FromArgb(160, 82, 45),          // Sienna
            ButtonHoverForeColor = Color.White,
            ButtonActiveBackColor = Color.FromArgb(85, 107, 47),         // Dark Olive Green
            ButtonActiveForeColor = Color.White,

            // **TextBox Colors**
            TextBoxBackColor = Color.White,
            TextBoxForeColor = Color.FromArgb(85, 85, 85),               // Dark Gray

            // **Label Colors**
            LabelBackColor = Color.White,
            LabelForeColor = Color.FromArgb(85, 85, 85),

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(222, 184, 135), // Burlywood


            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(210, 180, 140),             // Tan
            HeaderForeColor = Color.FromArgb(85, 85, 85),
            GridLineColor = Color.FromArgb(160, 82, 45),                 // Sienna
            RowBackColor = Color.White,
            RowForeColor = Color.FromArgb(85, 85, 85),
            AltRowBackColor = Color.FromArgb(222, 184, 135),             // Burlywood
            SelectedRowBackColor = Color.FromArgb(139, 69, 19),          // Saddle Brown
            SelectedRowForeColor = Color.White,

            // **ComboBox Colors**
            ComboBoxBackColor = Color.White,
            ComboBoxForeColor = Color.FromArgb(85, 85, 85),

            // **CheckBox Colors**
            CheckBoxBackColor = Color.White,
            CheckBoxForeColor = Color.FromArgb(85, 85, 85),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.White,
            RadioButtonForeColor = Color.FromArgb(85, 85, 85),

            // **Border Colors**
            BorderColor = Color.FromArgb(160, 82, 45),                   // Sienna
            ActiveBorderColor = Color.FromArgb(85, 107, 47),             // Dark Olive Green
            InactiveBorderColor = Color.FromArgb(222, 184, 135),         // Burlywood
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(139, 69, 19),                     // Saddle Brown
            VisitedLinkColor = Color.FromArgb(85, 107, 47),              // Dark Olive Green
            HoverLinkColor = Color.FromArgb(160, 82, 45),                // Sienna
            LinkHoverColor = Color.FromArgb(160, 82, 45),                // Sienna
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(85, 107, 47),              // Dark Olive Green
            ToolTipForeColor = Color.White,

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(245, 245, 220),          // Beige
            ScrollBarThumbColor = Color.FromArgb(160, 82, 45),           // Sienna
            ScrollBarTrackColor = Color.FromArgb(210, 180, 140),         // Tan

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(139, 69, 19),            // Saddle Brown
            StatusBarForeColor = Color.White,

            // **Tab Colors**
            TabBackColor = Color.FromArgb(245, 245, 220),                // Beige
            TabForeColor = Color.FromArgb(85, 85, 85),
            ActiveTabBackColor = Color.FromArgb(85, 107, 47),            // Dark Olive Green
            ActiveTabForeColor = Color.White,

            // **Dialog Box Colors**
            DialogBackColor = Color.FromArgb(222, 184, 135),             // Burlywood
            DialogForeColor = Color.FromArgb(85, 85, 85),
            DialogButtonBackColor = Color.FromArgb(139, 69, 19),         // Saddle Brown
            DialogButtonForeColor = Color.White,

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(245, 245, 220),          // Beige
            GradientEndColor = Color.FromArgb(160, 82, 45),              // Sienna
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(160, 82, 45), // Sienna

            SideMenuHoverBackColor = Color.FromArgb(222, 184, 135),      // Burlywood
            SideMenuSelectedBackColor = Color.FromArgb(160, 82, 45),     // Sienna
            SideMenuForeColor = Color.White,
            SideMenuHoverForeColor = Color.FromArgb(85, 85, 85),
            SideMenuSelectedForeColor = Color.White,
            SideMenuBorderColor = Color.FromArgb(160, 82, 45),           // Sienna
            SideMenuIconColor = Color.FromArgb(85, 85, 85),
            SideMenuSelectedIconColor = Color.White,

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(139, 69, 19),             // Saddle Brown
            TitleBarForeColor = Color.White,
            TitleBarHoverBackColor = Color.FromArgb(160, 82, 45),        // Sienna
            TitleBarHoverForeColor = Color.White,

            // **Dashboard Colors**
            DashboardBackColor = Color.FromArgb(245, 245, 220),          // Beige
            DashboardCardBackColor = Color.White,
            DashboardCardHoverBackColor = Color.FromArgb(222, 184, 135), // Burlywood
            CardTitleForeColor = Color.White,
            CardTextForeColor = Color.White,             // Saddle Brown

            // **Data Visualization (Charts)**
            ChartBackColor = Color.White,
            ChartLineColor = Color.FromArgb(85, 107, 47),                // Dark Olive Green
            ChartFillColor = Color.FromArgb(100, 85, 107, 47),           // Semi-transparent Dark Olive Green
            ChartAxisColor = Color.FromArgb(85, 85, 85),

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(85, 85, 85),
            SidebarSelectedIconColor = Color.White,
            SidebarTextColor = Color.White,
            SidebarSelectedTextColor = Color.White,

            // **Navigation Colors**
            NavigationBackColor = Color.FromArgb(245, 245, 220),         // Beige
            NavigationForeColor = Color.FromArgb(85, 85, 85),
            NavigationHoverBackColor = Color.FromArgb(222, 184, 135),    // Burlywood
            NavigationHoverForeColor = Color.FromArgb(85, 85, 85),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(85, 107, 47),                // Dark Olive Green
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(218, 165, 32),           // Goldenrod

            // **Font Properties**
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(85, 85, 85),
            SecondaryTextColor = Color.FromArgb(130, 130, 130),          // Medium Gray
            AccentTextColor = Color.FromArgb(139, 69, 19),               // Saddle Brown

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(139, 69, 19), // Saddle Brown
            },
            BlockquoteBorderColor = Color.FromArgb(160, 82, 45), // Sienna
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            InlineCodeBackgroundColor = Color.FromArgb(222, 184, 135), // Burlywood
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            CodeBlockBackgroundColor = Color.FromArgb(222, 184, 135), // Burlywood
            CodeBlockBorderColor = Color.FromArgb(160, 82, 45),       // Sienna
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(85, 107, 47),        // Dark Olive Green
            SecondaryColor = Color.FromArgb(139, 69, 19),      // Saddle Brown
            AccentColor = Color.FromArgb(160, 82, 45),         // Sienna
            BackgroundColor = Color.FromArgb(245, 245, 220),   // Beige
            SurfaceColor = Color.White,
            ErrorColor = Color.FromArgb(139, 0, 0),            // Dark Red
            WarningColor = Color.FromArgb(218, 165, 32),       // Goldenrod
            SuccessColor = Color.FromArgb(34, 139, 34),        // Forest Green
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.FromArgb(85, 85, 85),

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 4,

            // **Imagery and Iconography**
            IconSet = "EarthyIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(100, 0, 0, 0), // Semi-transparent black
            ShadowOpacity = 0.2f,

            // **Animation and Transitions**
            AnimationDurationShort = 150,  // in milliseconds
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(139, 69, 19), // Saddle Brown

            // **Theme Variant**
            IsDarkTheme = false,
        };
        public static BeepTheme WinterTheme => new BeepTheme
        {
            GridBackColor = Color.FromArgb(240, 248, 255), // Alice Blue
            GridForeColor = Color.DarkBlue,
            GridHeaderBackColor = Color.FromArgb(176, 224, 230), // Powder Blue
            GridHeaderForeColor = Color.DarkBlue,
            GridHeaderBorderColor = Color.MidnightBlue,
            GridHeaderHoverBackColor = Color.FromArgb(135, 206, 250), // Light Sky Blue
            GridHeaderHoverForeColor = Color.DarkBlue,
            GridHeaderSelectedBackColor = Color.SteelBlue,
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.MidnightBlue,
            GridHeaderSelectedBorderColor = Color.MidnightBlue,
            GridRowHoverBackColor = Color.FromArgb(224, 255, 255), // Light Cyan
            GridRowHoverForeColor = Color.DarkBlue,
            GridRowSelectedBackColor = Color.MidnightBlue,
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.MidnightBlue,
            GridRowSelectedBorderColor = Color.SteelBlue,
            CardBackColor = Color.FromArgb(220, 245, 254),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 102, 204),  // Winter Blue
            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 255, 255),  // Snow White
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(240, 248, 255),        // AliceBlue
            MaxButtonColor = Color.FromArgb(176, 196, 222),          // LightSteelBlue
            MinButtonColor = Color.FromArgb(135, 206, 250),          // LightSkyBlue
            TitleBarColor = Color.FromArgb(173, 216, 230),           // LightBlue
            TitleBarTextColor = Color.FromArgb(0, 0, 139),           // DarkBlue
            TitleBarIconColor = Color.FromArgb(0, 191, 255),         // DeepSkyBlue
            TitleBarHoverColor = Color.FromArgb(70, 130, 180),       // SteelBlue
            TitleBarHoverTextColor = Color.White,
            TitleBarHoverIconColor = Color.White,
            TitleBarActiveColor = Color.FromArgb(0, 191, 255),       // DeepSkyBlue
            TitleBarActiveTextColor = Color.White,
            TitleBarActiveIconColor = Color.White,
            TitleBarInactiveColor = Color.FromArgb(173, 216, 230),   // LightBlue
            TitleBarInactiveTextColor = Color.FromArgb(70, 130, 180),// SteelBlue
            TitleBarInactiveIconColor = Color.FromArgb(70, 130, 180),// SteelBlue
            TitleBarBorderColor = Color.FromArgb(176, 196, 222),     // LightSteelBlue
            TitleBarBorderHoverColor = Color.FromArgb(0, 191, 255),  // DeepSkyBlue
            TitleBarBorderActiveColor = Color.FromArgb(70, 130, 180),// SteelBlue
            TitleBarBorderInactiveColor = Color.FromArgb(176, 196, 222), // LightSteelBlue

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(255, 69, 0),    // Red
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(178, 34, 34),   // Firebrick
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(173, 216, 230),   // LightBlue
            TitleBarCloseInactiveTextColor = Color.FromArgb(70, 130, 180),// SteelBlue
            TitleBarCloseInactiveIconColor = Color.FromArgb(70, 130, 180),// SteelBlue
            TitleBarCloseBorderColor = Color.FromArgb(176, 196, 222),     // LightSteelBlue
            TitleBarCloseBorderHoverColor = Color.FromArgb(255, 69, 0),   // Red
            TitleBarCloseBorderActiveColor = Color.FromArgb(178, 34, 34), // Firebrick
            TitleBarCloseBorderInactiveColor = Color.FromArgb(176, 196, 222), // LightSteelBlue

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(70, 130, 180),       // SteelBlue
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.White,
            TitleBarMaxActiveColor = Color.FromArgb(0, 191, 255),       // DeepSkyBlue
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(173, 216, 230),   // LightBlue
            TitleBarMaxInactiveTextColor = Color.FromArgb(70, 130, 180),// SteelBlue
            TitleBarMaxInactiveIconColor = Color.FromArgb(70, 130, 180),// SteelBlue
            TitleBarMaxBorderColor = Color.FromArgb(176, 196, 222),     // LightSteelBlue
            TitleBarMaxBorderHoverColor = Color.FromArgb(0, 191, 255),  // DeepSkyBlue
            TitleBarMaxBorderActiveColor = Color.FromArgb(70, 130, 180),// SteelBlue
            TitleBarMaxBorderInactiveColor = Color.FromArgb(176, 196, 222), // LightSteelBlue

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(70, 130, 180),       // SteelBlue
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.White,
            TitleBarMinActiveColor = Color.FromArgb(0, 191, 255),       // DeepSkyBlue
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(173, 216, 230),   // LightBlue
            TitleBarMinInactiveTextColor = Color.FromArgb(70, 130, 180),// SteelBlue
            TitleBarMinInactiveIconColor = Color.FromArgb(70, 130, 180),// SteelBlue
            TitleBarMinBorderColor = Color.FromArgb(176, 196, 222),     // LightSteelBlue
            TitleBarMinBorderHoverColor = Color.FromArgb(0, 191, 255),  // DeepSkyBlue
            TitleBarMinBorderActiveColor = Color.FromArgb(70, 130, 180),// SteelBlue
            TitleBarMinBorderInactiveColor = Color.FromArgb(176, 196, 222), // LightSteelBlue

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(70, 130, 180),  // SteelBlue
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(0, 191, 255),  // DeepSkyBlue
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(173, 216, 230), // LightBlue
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(70, 130, 180),// SteelBlue
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(70, 130, 180),// SteelBlue
            TitleBarMinimizeBorderColor = Color.FromArgb(176, 196, 222),   // LightSteelBlue
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(0, 191, 255),// DeepSkyBlue
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(70, 130, 180),// SteelBlue
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(176, 196, 222), // LightSteelBlue

            // **General Colors**
            TitleForColor = Color.FromArgb(70, 130, 180),       // SteelBlue
            TitleBarForColor = Color.FromArgb(176, 196, 222),   // LightSteelBlue
            DescriptionForColor = Color.FromArgb(176, 196, 222), // LightSteelBlue
            BeforeForColor = Color.FromArgb(211, 211, 211),     // Light Gray
            LatestForColor = Color.FromArgb(192, 192, 192),     // Silver
            BackColor = Color.FromArgb(70, 130, 180), // SteelBlue     // White Smoke background

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(135, 206, 250),    // LightSkyBlue
            ButtonForeColor = Color.White,
            ButtonHoverBackColor = Color.FromArgb(70, 130, 180), // SteelBlue
            ButtonHoverForeColor = Color.White,
            ButtonActiveBackColor = Color.FromArgb(65, 105, 225), // RoyalBlue
            ButtonActiveForeColor = Color.White,

            // **TextBox Colors**
            TextBoxBackColor = Color.White,
            TextBoxForeColor = Color.FromArgb(60, 60, 60),      // Dark Gray

            // **Label Colors**
            LabelBackColor = Color.White,
            LabelForeColor = Color.FromArgb(60, 60, 60),

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(230, 240, 255), // Soft Ice Blue


            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(176, 196, 222),    // LightSteelBlue
            HeaderForeColor = Color.FromArgb(60, 60, 60),
            GridLineColor = Color.FromArgb(211, 211, 211),      // Light Gray
            RowBackColor = Color.White,
            RowForeColor = Color.FromArgb(60, 60, 60),
            AltRowBackColor = Color.FromArgb(230, 230, 250),    // Lavender
            SelectedRowBackColor = Color.FromArgb(135, 206, 250), // LightSkyBlue
            SelectedRowForeColor = Color.White,

            // **ComboBox Colors**
            ComboBoxBackColor = Color.White,
            ComboBoxForeColor = Color.FromArgb(60, 60, 60),

            // **CheckBox Colors**
            CheckBoxBackColor = Color.White,
            CheckBoxForeColor = Color.FromArgb(60, 60, 60),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.White,
            RadioButtonForeColor = Color.FromArgb(60, 60, 60),

            // **Border Colors**
            BorderColor = Color.FromArgb(176, 196, 222),        // LightSteelBlue
            ActiveBorderColor = Color.FromArgb(70, 130, 180),   // SteelBlue
            InactiveBorderColor = Color.FromArgb(211, 211, 211), // Light Gray
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(70, 130, 180),           // SteelBlue
            VisitedLinkColor = Color.FromArgb(106, 90, 205),    // SlateBlue
            HoverLinkColor = Color.FromArgb(65, 105, 225),      // RoyalBlue
            LinkHoverColor = Color.FromArgb(65, 105, 225),      // RoyalBlue
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(70, 130, 180),    // SteelBlue
            ToolTipForeColor = Color.White,

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(245, 245, 245),
            ScrollBarThumbColor = Color.FromArgb(176, 196, 222),
            ScrollBarTrackColor = Color.FromArgb(211, 211, 211),

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(70, 130, 180),  // SteelBlue
            StatusBarForeColor = Color.White,

            // **Tab Colors**
            TabBackColor = Color.FromArgb(245, 245, 245),
            TabForeColor = Color.FromArgb(60, 60, 60),
            ActiveTabBackColor = Color.FromArgb(135, 206, 250), // LightSkyBlue
            ActiveTabForeColor = Color.White,

            // **Dialog Box Colors**
            DialogBackColor = Color.FromArgb(230, 230, 250),    // Lavender
            DialogForeColor = Color.FromArgb(60, 60, 60),
            DialogButtonBackColor = Color.FromArgb(135, 206, 250),
            DialogButtonForeColor = Color.White,

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(245, 245, 245), // White Smoke
            GradientEndColor = Color.FromArgb(176, 196, 222),   // LightSteelBlue
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(200, 225, 245), // Frosted Blue

            SideMenuHoverBackColor = Color.FromArgb(230, 230, 250), // Lavender
            SideMenuSelectedBackColor = Color.FromArgb(176, 196, 222),
            SideMenuForeColor = Color.FromArgb(60, 60, 60),
            SideMenuHoverForeColor = Color.FromArgb(60, 60, 60),
            SideMenuSelectedForeColor = Color.White,
            SideMenuBorderColor = Color.FromArgb(211, 211, 211),
            SideMenuIconColor = Color.FromArgb(60, 60, 60),
            SideMenuSelectedIconColor = Color.White,

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(176, 196, 222), // SteelBlue
            TitleBarForeColor = Color.White,
            TitleBarHoverBackColor = Color.FromArgb(65, 105, 225), // RoyalBlue
            TitleBarHoverForeColor = Color.White,

            // **Dashboard Colors**
            DashboardBackColor = Color.FromArgb(245, 245, 245),
            DashboardCardBackColor = Color.White,
            DashboardCardHoverBackColor = Color.FromArgb(230, 230, 250),
            CardTitleForeColor = Color.FromArgb(60, 60, 60),
            CardTextForeColor = Color.FromArgb(70, 130, 180),   // SteelBlue

            // **Data Visualization (Charts)**
            ChartBackColor = Color.White,
            ChartLineColor = Color.FromArgb(70, 130, 180),      // SteelBlue
            ChartFillColor = Color.FromArgb(100, 70, 130, 180), // Semi-transparent SteelBlue
            ChartAxisColor = Color.FromArgb(60, 60, 60),

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(60, 60, 60),
            SidebarSelectedIconColor = Color.White,
            SidebarTextColor = Color.FromArgb(60, 60, 60),
            SidebarSelectedTextColor = Color.White,

            // **Navigation Colors**
            NavigationBackColor = Color.FromArgb(245, 245, 245),
            NavigationForeColor = Color.FromArgb(60, 60, 60),
            NavigationHoverBackColor = Color.FromArgb(230, 230, 250),
            NavigationHoverForeColor = Color.FromArgb(60, 60, 60),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(70, 130, 180),      // SteelBlue
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(176, 196, 222), // LightSteelBlue

            // **Font Properties**
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(60, 60, 60),
            SecondaryTextColor = Color.FromArgb(100, 100, 100), // Medium Gray
            AccentTextColor = Color.FromArgb(70, 130, 180),     // SteelBlue

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(60, 60, 60),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(60, 60, 60),
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(70, 130, 180), // SteelBlue
            },
            BlockquoteBorderColor = Color.FromArgb(176, 196, 222), // LightSteelBlue
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            InlineCodeBackgroundColor = Color.FromArgb(230, 230, 250), // Lavender
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            CodeBlockBackgroundColor = Color.FromArgb(230, 230, 250), // Lavender
            CodeBlockBorderColor = Color.FromArgb(176, 196, 222),     // LightSteelBlue
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(60, 60, 60),
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(60, 60, 60),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(60, 60, 60),                // Dark Gray
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(60, 60, 60),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(60, 60, 60),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(70, 130, 180),        // Steel Blue
            SecondaryColor = Color.FromArgb(176, 196, 222),     // Light Steel Blue
            AccentColor = Color.FromArgb(135, 206, 250),        // Light Sky Blue
            BackgroundColor = Color.FromArgb(176, 196, 222),
            SurfaceColor = Color.White,
            ErrorColor = Color.FromArgb(178, 34, 34),           // Firebrick
            WarningColor = Color.FromArgb(255, 165, 0),         // Orange
            SuccessColor = Color.FromArgb(34, 139, 34),         // Forest Green
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.FromArgb(60, 60, 60),

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 4,

            // **Imagery and Iconography**
            IconSet = "WinterIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(100, 0, 0, 0), // Semi-transparent black
            ShadowOpacity = 0.2f,

            // **Animation and Transitions**
            AnimationDurationShort = 150,  // in milliseconds
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(70, 130, 180), // Steel Blue

            // **Theme Variant**
            IsDarkTheme = false,
        };
        public static BeepTheme SpringTheme => new BeepTheme
        {
            // Core Grid Colors
            GridBackColor = Color.FromArgb(245, 255, 245),        // Light Mint
            GridForeColor = Color.FromArgb(34, 139, 34),          // Forest Green
            GridHeaderBackColor = Color.FromArgb(152, 251, 152),  // Pale Green
            GridHeaderForeColor = Color.FromArgb(34, 139, 34),    // Forest Green
            GridHeaderBorderColor = Color.FromArgb(60, 179, 113), // Medium Sea Green
            GridHeaderHoverBackColor = Color.FromArgb(144, 238, 144), // Light Green
            GridHeaderHoverForeColor = Color.FromArgb(34, 139, 34),   // Forest Green
            GridHeaderSelectedBackColor = Color.FromArgb(60, 179, 113), // Medium Sea Green
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.FromArgb(60, 179, 113),  // Medium Sea Green
            GridHeaderSelectedBorderColor = Color.FromArgb(60, 179, 113), // Medium Sea Green
            GridRowHoverBackColor = Color.FromArgb(144, 238, 144),    // Light Green
            GridRowHoverForeColor = Color.FromArgb(34, 139, 34),      // Forest Green
            GridRowSelectedBackColor = Color.FromArgb(60, 179, 113),  // Medium Sea Green
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.FromArgb(60, 179, 113),   // Medium Sea Green
            GridRowSelectedBorderColor = Color.FromArgb(60, 179, 113), // Medium Sea Green

            CardBackColor = Color.FromArgb(204, 255, 204),           // Pale Mint

            // Card Typography
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 179, 113),  // Medium Sea Green
            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(34, 139, 34),   // Forest Green
            },

            // Title Bar and Window Controls
            CloseButtonColor = Color.FromArgb(255, 182, 193),     // Light Pink
            MaxButtonColor = Color.FromArgb(144, 238, 144),       // Light Green
            MinButtonColor = Color.FromArgb(255, 192, 203),       // Pale Pink
            TitleBarColor = Color.FromArgb(152, 251, 152),        // Pale Green
            TitleBarTextColor = Color.FromArgb(34, 139, 34),      // Forest Green
            TitleBarIconColor = Color.FromArgb(60, 179, 113),     // Medium Sea Green
            TitleBarHoverColor = Color.FromArgb(144, 238, 144),   // Light Green
            TitleBarHoverTextColor = Color.White,
            TitleBarHoverIconColor = Color.White,
            TitleBarActiveColor = Color.FromArgb(60, 179, 113),   // Medium Sea Green
            TitleBarActiveTextColor = Color.White,
            TitleBarActiveIconColor = Color.White,
            TitleBarInactiveColor = Color.FromArgb(245, 255, 245), // Light Mint
            TitleBarInactiveTextColor = Color.FromArgb(60, 179, 113), // Medium Sea Green
            TitleBarInactiveIconColor = Color.FromArgb(60, 179, 113), // Medium Sea Green
            TitleBarBorderColor = Color.FromArgb(144, 238, 144),     // Light Green
            TitleBarBorderHoverColor = Color.FromArgb(102, 205, 170), // Medium Aquamarine
            TitleBarBorderActiveColor = Color.FromArgb(60, 179, 113), // Medium Sea Green
            TitleBarBorderInactiveColor = Color.FromArgb(245, 255, 245), // Light Mint

            // Close Button States
            TitleBarCloseHoverColor = Color.FromArgb(255, 105, 180),    // Hot Pink
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(219, 112, 147),   // Pale Violet Red
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(255, 192, 203), // Pale Pink
            TitleBarCloseInactiveTextColor = Color.FromArgb(219, 112, 147), // Pale Violet Red
            TitleBarCloseInactiveIconColor = Color.FromArgb(219, 112, 147), // Pale Violet Red
            TitleBarCloseBorderColor = Color.FromArgb(255, 182, 193),   // Light Pink
            TitleBarCloseBorderHoverColor = Color.FromArgb(255, 105, 180), // Hot Pink
            TitleBarCloseBorderActiveColor = Color.FromArgb(219, 112, 147), // Pale Violet Red
            TitleBarCloseBorderInactiveColor = Color.FromArgb(255, 192, 203), // Pale Pink

            // Maximize Button States
            TitleBarMaxHoverColor = Color.FromArgb(144, 238, 144),      // Light Green
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.White,
            TitleBarMaxActiveColor = Color.FromArgb(60, 179, 113),      // Medium Sea Green
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(245, 255, 245),   // Light Mint
            TitleBarMaxInactiveTextColor = Color.FromArgb(60, 179, 113), // Medium Sea Green
            TitleBarMaxInactiveIconColor = Color.FromArgb(60, 179, 113), // Medium Sea Green
            TitleBarMaxBorderColor = Color.FromArgb(144, 238, 144),     // Light Green
            TitleBarMaxBorderHoverColor = Color.FromArgb(102, 205, 170), // Medium Aquamarine
            TitleBarMaxBorderActiveColor = Color.FromArgb(60, 179, 113), // Medium Sea Green
            TitleBarMaxBorderInactiveColor = Color.FromArgb(245, 255, 245), // Light Mint

            // Minimize Button States
            TitleBarMinHoverColor = Color.FromArgb(255, 182, 193),      // Light Pink
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.White,
            TitleBarMinActiveColor = Color.FromArgb(255, 105, 180),     // Hot Pink
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(255, 192, 203),   // Pale Pink
            TitleBarMinInactiveTextColor = Color.FromArgb(219, 112, 147), // Pale Violet Red
            TitleBarMinInactiveIconColor = Color.FromArgb(219, 112, 147), // Pale Violet Red
            TitleBarMinBorderColor = Color.FromArgb(255, 182, 193),     // Light Pink
            TitleBarMinBorderHoverColor = Color.FromArgb(255, 105, 180), // Hot Pink
            TitleBarMinBorderActiveColor = Color.FromArgb(219, 112, 147), // Pale Violet Red
            TitleBarMinBorderInactiveColor = Color.FromArgb(255, 192, 203), // Pale Pink

            // Alternative Minimize Button States
            TitleBarMinimizeHoverColor = Color.FromArgb(255, 182, 193),  // Light Pink
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(255, 105, 180), // Hot Pink
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(255, 192, 203), // Pale Pink
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(219, 112, 147), // Pale Violet Red
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(219, 112, 147), // Pale Violet Red
            TitleBarMinimizeBorderColor = Color.FromArgb(255, 182, 193),   // Light Pink
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(255, 105, 180), // Hot Pink
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(219, 112, 147), // Pale Violet Red
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(255, 192, 203), // Pale Pink

            // General Colors
            TitleForColor = Color.FromArgb(60, 179, 113),          // Medium Sea Green
            TitleBarForColor = Color.FromArgb(102, 205, 170),      // Medium Aquamarine
            DescriptionForColor = Color.FromArgb(144, 238, 144),   // Light Green
            BeforeForColor = Color.FromArgb(255, 192, 203),        // Pale Pink
            LatestForColor = Color.FromArgb(255, 182, 193),        // Light Pink
            BackColor = Color.FromArgb(245, 255, 245),             // Light Mint

            // Button Colors
            ButtonBackColor = Color.FromArgb(144, 238, 144),       // Light Green
            ButtonForeColor = Color.FromArgb(34, 139, 34),         // Forest Green
            ButtonHoverBackColor = Color.FromArgb(60, 179, 113),   // Medium Sea Green
            ButtonHoverForeColor = Color.White,
            ButtonActiveBackColor = Color.FromArgb(46, 139, 87),   // Sea Green
            ButtonActiveForeColor = Color.White,

            // TextBox Colors
            TextBoxBackColor = Color.FromArgb(245, 255, 245),      // Light Mint
            TextBoxForeColor = Color.FromArgb(34, 139, 34),        // Forest Green

            // Label Colors
            LabelBackColor = Color.FromArgb(245, 255, 245),        // Light Mint
            LabelForeColor = Color.FromArgb(34, 139, 34),          // Forest Green

            // Panel Colors
            PanelBackColor = Color.FromArgb(144, 238, 144),        // Light Green

            // Grid Colors
            HeaderBackColor = Color.FromArgb(152, 251, 152),       // Pale Green
            HeaderForeColor = Color.FromArgb(34, 139, 34),         // Forest Green
            GridLineColor = Color.FromArgb(144, 238, 144),         // Light Green
            RowBackColor = Color.FromArgb(245, 255, 245),          // Light Mint
            RowForeColor = Color.FromArgb(34, 139, 34),            // Forest Green
            AltRowBackColor = Color.FromArgb(204, 255, 204),       // Pale Mint
            SelectedRowBackColor = Color.FromArgb(60, 179, 113),   // Medium Sea Green
            SelectedRowForeColor = Color.White,

            // ComboBox Colors
            ComboBoxBackColor = Color.FromArgb(245, 255, 245),     // Light Mint
            ComboBoxForeColor = Color.FromArgb(34, 139, 34),       // Forest Green

            // CheckBox Colors
            CheckBoxBackColor = Color.FromArgb(245, 255, 245),     // Light Mint
            CheckBoxForeColor = Color.FromArgb(34, 139, 34),       // Forest Green

            // RadioButton Colors
            RadioButtonBackColor = Color.FromArgb(245, 255, 245),  // Light Mint
            RadioButtonForeColor = Color.FromArgb(34, 139, 34),    // Forest Green

            // Border Colors
            BorderColor = Color.FromArgb(144, 238, 144),           // Light Green
            ActiveBorderColor = Color.FromArgb(60, 179, 113),      // Medium Sea Green
            InactiveBorderColor = Color.FromArgb(152, 251, 152),   // Pale Green
            BorderSize = 1,

            // Link Colors
            LinkColor = Color.FromArgb(34, 139, 34),               // Forest Green
            VisitedLinkColor = Color.FromArgb(85, 107, 47),        // Dark Olive Green
            HoverLinkColor = Color.FromArgb(60, 179, 113),         // Medium Sea Green
            LinkHoverColor = Color.FromArgb(60, 179, 113),         // Medium Sea Green
            LinkIsUnderline = true,

            // ToolTip Colors
            ToolTipBackColor = Color.FromArgb(144, 238, 144),      // Light Green
            ToolTipForeColor = Color.FromArgb(34, 139, 34),        // Forest Green

            // ScrollBar Colors
            ScrollBarBackColor = Color.FromArgb(245, 255, 245),    // Light Mint
            ScrollBarThumbColor = Color.FromArgb(144, 238, 144),   // Light Green
            ScrollBarTrackColor = Color.FromArgb(152, 251, 152),   // Pale Green

            // Status Bar Colors
            StatusBarBackColor = Color.FromArgb(60, 179, 113),     // Medium Sea Green
            StatusBarForeColor = Color.White,

            // Tab Colors
            TabBackColor = Color.FromArgb(245, 255, 245),          // Light Mint
            TabForeColor = Color.FromArgb(34, 139, 34),            // Forest Green
            ActiveTabBackColor = Color.FromArgb(60, 179, 113),     // Medium Sea Green
            ActiveTabForeColor = Color.White,

            // Dialog Box Colors
            DialogBackColor = Color.FromArgb(245, 255, 245),       // Light Mint
            DialogForeColor = Color.FromArgb(34, 139, 34),         // Forest Green
            DialogButtonBackColor = Color.FromArgb(144, 238, 144), // Light Green
            DialogButtonForeColor = Color.White,

            // Gradient Properties
            GradientStartColor = Color.FromArgb(245, 255, 245),    // Light Mint
            GradientEndColor = Color.FromArgb(144, 238, 144),      // Light Green
            GradientDirection = LinearGradientMode.Vertical,

            // Side Menu Colors
            SideMenuBackColor = Color.FromArgb(34, 139, 34),       // Forest Green
            SideMenuHoverBackColor = Color.FromArgb(144, 238, 144), // Light Green
            SideMenuSelectedBackColor = Color.FromArgb(60, 179, 113), // Medium Sea Green
            SideMenuForeColor = Color.White,
            SideMenuHoverForeColor = Color.FromArgb(34, 139, 34),   // Forest Green
            SideMenuSelectedForeColor = Color.White,
            SideMenuBorderColor = Color.FromArgb(152, 251, 152),    // Pale Green
            SideMenuIconColor = Color.FromArgb(245, 255, 245),      // Light Mint
            SideMenuSelectedIconColor = Color.White,

            // Title Bar Colors (Repeated Properties)
            TitleBarBackColor = Color.FromArgb(60, 179, 113),      // Medium Sea Green
            TitleBarForeColor = Color.White,
            TitleBarHoverBackColor = Color.FromArgb(46, 139, 87),  // Sea Green
            TitleBarHoverForeColor = Color.White,

            // Dashboard Colors
            DashboardBackColor = Color.FromArgb(245, 255, 245),    // Light Mint
            DashboardCardBackColor = Color.FromArgb(204, 255, 204), // Pale Mint
            DashboardCardHoverBackColor = Color.FromArgb(144, 238, 144), // Light Green
            CardTitleForeColor = Color.FromArgb(34, 139, 34),      // Forest Green
            CardTextForeColor = Color.FromArgb(60, 179, 113),      // Medium Sea Green

            // Data Visualization (Charts)
            ChartBackColor = Color.FromArgb(245, 255, 245),        // Light Mint
            ChartLineColor = Color.FromArgb(60, 179, 113),         // Medium Sea Green
            ChartFillColor = Color.FromArgb(144, 238, 144),        // Light Green
            ChartAxisColor = Color.FromArgb(34, 139, 34),          // Forest Green

            // Sidebar and Menu Colors
            SidebarIconColor = Color.FromArgb(34, 139, 34),        // Forest Green
            SidebarSelectedIconColor = Color.White,
            SidebarTextColor = Color.FromArgb(34, 139, 34),        // Forest Green
            SidebarSelectedTextColor = Color.White,

            // Navigation Colors
            NavigationBackColor = Color.FromArgb(245, 255, 245),   // Light Mint
            NavigationForeColor = Color.FromArgb(34, 139, 34),     // Forest Green
            NavigationHoverBackColor = Color.FromArgb(144, 238, 144), // Light Green
            NavigationHoverForeColor = Color.FromArgb(34, 139, 34),   // Forest Green

            // Badge and Highlight Colors
            BadgeBackColor = Color.FromArgb(60, 179, 113),         // Medium Sea Green
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(144, 238, 144),    // Light Green

            // Font Properties
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // Font Sizes
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // Font Styles
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // Text Colors
            PrimaryTextColor = Color.FromArgb(34, 139, 34),        // Forest Green
            SecondaryTextColor = Color.FromArgb(85, 107, 47),      // Dark Olive Green
            AccentTextColor = Color.FromArgb(60, 179, 113),        // Medium Sea Green

            // Typography Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 107, 47),          // Dark Olive Green
            },
            BlockquoteBorderColor = Color.FromArgb(144, 238, 144), // Light Green
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,
            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            InlineCodeBackgroundColor = Color.FromArgb(204, 255, 204), // Pale Mint
            InlineCodePadding = 4f,
            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            CodeBlockBackgroundColor = Color.FromArgb(204, 255, 204), // Pale Mint
            CodeBlockBorderColor = Color.FromArgb(144, 238, 144),     // Light Green
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,
            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,
            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(34, 139, 34),          // Forest Green
            },

            // Color Palette
            PrimaryColor = Color.FromArgb(60, 179, 113),          // Medium Sea Green
            SecondaryColor = Color.FromArgb(255, 182, 193),       // Light Pink
            AccentColor = Color.FromArgb(102, 205, 170),          // Medium Aquamarine
            BackgroundColor = Color.FromArgb(245, 255, 245),      // Light Mint
            SurfaceColor = Color.FromArgb(204, 255, 204),         // Pale Mint
            ErrorColor = Color.FromArgb(219, 112, 147),           // Pale Violet Red
            WarningColor = Color.FromArgb(255, 140, 0),           // Dark Orange
            SuccessColor = Color.FromArgb(34, 139, 34),           // Forest Green
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.FromArgb(34, 139, 34),      // Forest Green

            // Spacing and Layout
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 4,

            // Imagery and Iconography
            IconSet = "SpringIcons",
            ApplyThemeToIcons = true,

            // Effects and Decorations
            ShadowColor = Color.FromArgb(64, 64, 64),             // Dark Gray
            ShadowOpacity = 0.2f,

            // Animation and Transitions
            AnimationDurationShort = 150,  // in milliseconds
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",

            // Accessibility
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(60, 179, 113),   // Medium Sea Green

            // Theme Variant
            IsDarkTheme = false,
        };
        public static BeepTheme CandyTheme => new BeepTheme
        {
            GridBackColor = Color.FromArgb(255, 240, 245), // Lavender Blush
            GridForeColor = Color.DarkMagenta,
            GridHeaderBackColor = Color.FromArgb(255, 182, 193), // Light Pink
            GridHeaderForeColor = Color.DarkRed,
            GridHeaderBorderColor = Color.HotPink,
            GridHeaderHoverBackColor = Color.FromArgb(255, 105, 180), // Hot Pink
            GridHeaderHoverForeColor = Color.DarkRed,
            GridHeaderSelectedBackColor = Color.FromArgb(255, 20, 147), // Deep Pink
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.DeepPink,
            GridHeaderSelectedBorderColor = Color.HotPink,
            GridRowHoverBackColor = Color.FromArgb(255, 228, 225), // Misty Rose
            GridRowHoverForeColor = Color.DarkMagenta,
            GridRowSelectedBackColor = Color.DeepPink,
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.HotPink,
            GridRowSelectedBorderColor = Color.DeepPink,
            CardBackColor = Color.FromArgb(255, 182, 193),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 105, 180),  // Hot Pink
            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 182, 193),  // Light Pink
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(255, 160, 122),        // Light Salmon
            MaxButtonColor = Color.FromArgb(255, 182, 193),          // Light Pink
            MinButtonColor = Color.FromArgb(255, 192, 203),          // Pink
            TitleBarColor = Color.FromArgb(255, 182, 193),           // Light Pink
            TitleBarTextColor = Color.FromArgb(219, 112, 147),       // Pale Violet Red
            TitleBarIconColor = Color.FromArgb(238, 130, 238),       // Violet
            TitleBarHoverColor = Color.FromArgb(255, 105, 180),      // Hot Pink
            TitleBarHoverTextColor = Color.White,
            TitleBarHoverIconColor = Color.White,
            TitleBarActiveColor = Color.FromArgb(238, 130, 238),     // Violet
            TitleBarActiveTextColor = Color.White,
            TitleBarActiveIconColor = Color.White,
            TitleBarInactiveColor = Color.FromArgb(255, 240, 245),   // LavenderBlush
            TitleBarInactiveTextColor = Color.FromArgb(219, 112, 147), // Pale Violet Red
            TitleBarInactiveIconColor = Color.FromArgb(219, 112, 147),
            TitleBarBorderColor = Color.FromArgb(255, 160, 122),     // Light Salmon
            TitleBarBorderHoverColor = Color.FromArgb(238, 130, 238),// Violet
            TitleBarBorderActiveColor = Color.FromArgb(255, 105, 180),// Hot Pink
            TitleBarBorderInactiveColor = Color.FromArgb(255, 240, 245), // LavenderBlush

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(255, 69, 0),    // Red Orange
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(220, 20, 60),  // Crimson
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(255, 160, 122),   // Light Salmon
            TitleBarCloseInactiveTextColor = Color.FromArgb(219, 112, 147),
            TitleBarCloseInactiveIconColor = Color.FromArgb(219, 112, 147),
            TitleBarCloseBorderColor = Color.FromArgb(255, 182, 193),     // Light Pink
            TitleBarCloseBorderHoverColor = Color.FromArgb(255, 69, 0),   // Red Orange
            TitleBarCloseBorderActiveColor = Color.FromArgb(220, 20, 60), // Crimson
            TitleBarCloseBorderInactiveColor = Color.FromArgb(255, 160, 122), // Light Salmon

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(238, 130, 238),       // Violet
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.White,
            TitleBarMaxActiveColor = Color.FromArgb(221, 160, 221),       // Plum
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(255, 240, 245),     // LavenderBlush
            TitleBarMaxInactiveTextColor = Color.FromArgb(219, 112, 147),
            TitleBarMaxInactiveIconColor = Color.FromArgb(219, 112, 147),
            TitleBarMaxBorderColor = Color.FromArgb(255, 182, 193),       // Light Pink
            TitleBarMaxBorderHoverColor = Color.FromArgb(238, 130, 238),  // Violet
            TitleBarMaxBorderActiveColor = Color.FromArgb(221, 160, 221), // Plum
            TitleBarMaxBorderInactiveColor = Color.FromArgb(255, 240, 245), // LavenderBlush

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(255, 218, 185),       // Peach Puff
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.White,
            TitleBarMinActiveColor = Color.FromArgb(255, 165, 0),        // Orange
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(255, 240, 245),    // LavenderBlush
            TitleBarMinInactiveTextColor = Color.FromArgb(219, 112, 147),
            TitleBarMinInactiveIconColor = Color.FromArgb(219, 112, 147),
            TitleBarMinBorderColor = Color.FromArgb(255, 182, 193),      // Light Pink
            TitleBarMinBorderHoverColor = Color.FromArgb(255, 218, 185), // Peach Puff
            TitleBarMinBorderActiveColor = Color.FromArgb(255, 165, 0),  // Orange
            TitleBarMinBorderInactiveColor = Color.FromArgb(255, 240, 245), // LavenderBlush

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(255, 218, 185),  // Peach Puff
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(255, 165, 0),   // Orange
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(255, 240, 245), // LavenderBlush
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(219, 112, 147),
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(219, 112, 147),
            TitleBarMinimizeBorderColor = Color.FromArgb(255, 182, 193),   // Light Pink
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(255, 218, 185),// Peach Puff
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(255, 165, 0),// Orange
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(255, 240, 245), // LavenderBlush

            // **General Colors**
            TitleForColor = Color.FromArgb(238, 130, 238),           // Violet
            TitleBarForColor = Color.FromArgb(255, 160, 122),        // Light Salmon
            DescriptionForColor = Color.FromArgb(255, 182, 193),     // Light Pink
            BeforeForColor = Color.FromArgb(255, 105, 180),          // Hot Pink
            LatestForColor = Color.FromArgb(221, 160, 221),          // Plum
            BackColor = Color.FromArgb(255, 250, 250),               // Snow background

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(255, 182, 193),         // Light Pink
            ButtonForeColor = Color.White,
            ButtonHoverBackColor = Color.FromArgb(255, 105, 180),    // Hot Pink
            ButtonHoverForeColor = Color.White,
            ButtonActiveBackColor = Color.FromArgb(219, 112, 147),   // Pale Violet Red
            ButtonActiveForeColor = Color.White,

            // **TextBox Colors**
            TextBoxBackColor = Color.White,
            TextBoxForeColor = Color.FromArgb(139, 0, 139),          // Dark Magenta

            // **Label Colors**
            LabelBackColor = Color.White,
            LabelForeColor = Color.FromArgb(139, 0, 139),

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(230, 230, 250), // Lavender

            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(255, 182, 193),         // Light Pink
            HeaderForeColor = Color.FromArgb(139, 0, 139),
            GridLineColor = Color.FromArgb(255, 160, 122),           // Light Salmon
            RowBackColor = Color.White,
            RowForeColor = Color.FromArgb(139, 0, 139),
            AltRowBackColor = Color.FromArgb(255, 240, 245),         // LavenderBlush
            SelectedRowBackColor = Color.FromArgb(255, 105, 180),    // Hot Pink
            SelectedRowForeColor = Color.White,

            // **ComboBox Colors**
            ComboBoxBackColor = Color.White,
            ComboBoxForeColor = Color.FromArgb(139, 0, 139),

            // **CheckBox Colors**
            CheckBoxBackColor = Color.White,
            CheckBoxForeColor = Color.FromArgb(139, 0, 139),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.White,
            RadioButtonForeColor = Color.FromArgb(139, 0, 139),

            // **Border Colors**
            BorderColor = Color.FromArgb(255, 160, 122),             // Light Salmon
            ActiveBorderColor = Color.FromArgb(255, 105, 180),       // Hot Pink
            InactiveBorderColor = Color.FromArgb(255, 182, 193),     // Light Pink
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(255, 105, 180),               // Hot Pink
            VisitedLinkColor = Color.FromArgb(199, 21, 133),         // Medium Violet Red
            HoverLinkColor = Color.FromArgb(238, 130, 238),          // Violet
            LinkHoverColor = Color.FromArgb(238, 130, 238),          // Violet
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(255, 182, 193),        // Light Pink
            ToolTipForeColor = Color.FromArgb(139, 0, 139),

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(255, 250, 250),      // Snow
            ScrollBarThumbColor = Color.FromArgb(255, 182, 193),     // Light Pink
            ScrollBarTrackColor = Color.FromArgb(255, 240, 245),     // LavenderBlush

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(255, 105, 180),      // Hot Pink
            StatusBarForeColor = Color.White,

            // **Tab Colors**
            TabBackColor = Color.FromArgb(255, 250, 250),            // Snow
            TabForeColor = Color.FromArgb(139, 0, 139),
            ActiveTabBackColor = Color.FromArgb(255, 182, 193),      // Light Pink
            ActiveTabForeColor = Color.White,

            // **Dialog Box Colors**
            DialogBackColor = Color.FromArgb(255, 240, 245),         // LavenderBlush
            DialogForeColor = Color.FromArgb(139, 0, 139),
            DialogButtonBackColor = Color.FromArgb(255, 182, 193),   // Light Pink
            DialogButtonForeColor = Color.White,

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(255, 250, 250),      // Snow
            GradientEndColor = Color.FromArgb(255, 182, 193),        // Light Pink
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(255, 192, 203), // Pink
            SideMenuHoverBackColor = Color.FromArgb(255, 240, 245),  // LavenderBlush
            SideMenuSelectedBackColor = Color.FromArgb(255, 182, 193),
            SideMenuForeColor = Color.FromArgb(139, 0, 139),
            SideMenuHoverForeColor = Color.FromArgb(139, 0, 139),
            SideMenuSelectedForeColor = Color.White,
            SideMenuBorderColor = Color.FromArgb(255, 182, 193),     // Light Pink
            SideMenuIconColor = Color.FromArgb(139, 0, 139),
            SideMenuSelectedIconColor = Color.White,

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(255, 105, 180),       // Hot Pink
            TitleBarForeColor = Color.White,
            TitleBarHoverBackColor = Color.FromArgb(219, 112, 147),  // Pale Violet Red
            TitleBarHoverForeColor = Color.White,

            // **Dashboard Colors**
            DashboardBackColor = Color.FromArgb(255, 250, 250),
            DashboardCardBackColor = Color.White,
            DashboardCardHoverBackColor = Color.FromArgb(255, 240, 245),
            CardTitleForeColor = Color.FromArgb(139, 0, 139),
            CardTextForeColor = Color.FromArgb(238, 130, 238),       // Violet

            // **Data Visualization (Charts)**
            ChartBackColor = Color.White,
            ChartLineColor = Color.FromArgb(255, 105, 180),          // Hot Pink
            ChartFillColor = Color.FromArgb(100, 255, 105, 180),     // Semi-transparent Hot Pink
            ChartAxisColor = Color.FromArgb(139, 0, 139),

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(139, 0, 139),
            SidebarSelectedIconColor = Color.White,
            SidebarTextColor = Color.FromArgb(139, 0, 139),
            SidebarSelectedTextColor = Color.White,

            // **Navigation Colors**
            NavigationBackColor = Color.FromArgb(255, 250, 250),
            NavigationForeColor = Color.FromArgb(139, 0, 139),
            NavigationHoverBackColor = Color.FromArgb(255, 240, 245),
            NavigationHoverForeColor = Color.FromArgb(139, 0, 139),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(255, 105, 180),          // Hot Pink
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(238, 130, 238),      // Violet

            // **Font Properties**
            FontFamily = "Comic Sans MS",
            FontName = "Comic Sans MS",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(139, 0, 139),          // Dark Magenta
            SecondaryTextColor = Color.FromArgb(186, 85, 211),       // Medium Orchid
            AccentTextColor = Color.FromArgb(238, 130, 238),         // Violet

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(139, 0, 139),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(139, 0, 139),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(139, 0, 139),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(139, 0, 139),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(139, 0, 139),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(139, 0, 139),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(139, 0, 139),
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(186, 85, 211), // Medium Orchid
            },
            BlockquoteBorderColor = Color.FromArgb(238, 130, 238), // Violet
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(139, 0, 139),
            },
            InlineCodeBackgroundColor = Color.FromArgb(255, 240, 245), // LavenderBlush
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(139, 0, 139),
            },
            CodeBlockBackgroundColor = Color.FromArgb(255, 240, 245), // LavenderBlush
            CodeBlockBorderColor = Color.FromArgb(238, 130, 238),     // Violet
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(139, 0, 139),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(139, 0, 139),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(139, 0, 139),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(139, 0, 139),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(139, 0, 139),
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(139, 0, 139),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(139, 0, 139),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(139, 0, 139),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(139, 0, 139),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(139, 0, 139),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(139, 0, 139),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(139, 0, 139),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(139, 0, 139),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(139, 0, 139),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(139, 0, 139),                // Dark Magenta
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(139, 0, 139),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(139, 0, 139),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(139, 0, 139),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(139, 0, 139),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Comic Sans MS",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(139, 0, 139),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(255, 105, 180),          // Hot Pink
            SecondaryColor = Color.FromArgb(238, 130, 238),        // Violet
            AccentColor = Color.FromArgb(255, 182, 193),           // Light Pink
            BackgroundColor = Color.FromArgb(255, 250, 250),       // Snow
            SurfaceColor = Color.White,
            ErrorColor = Color.FromArgb(220, 20, 60),              // Crimson
            WarningColor = Color.FromArgb(255, 165, 0),            // Orange
            SuccessColor = Color.FromArgb(50, 205, 50),            // Lime Green
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.FromArgb(139, 0, 139),

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 8,

            // **Imagery and Iconography**
            IconSet = "CandyIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(100, 0, 0, 0), // Semi-transparent black
            ShadowOpacity = 0.2f,

            // **Animation and Transitions**
            AnimationDurationShort = 150,  // in milliseconds
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(255, 105, 180), // Hot Pink

            // **Theme Variant**
            IsDarkTheme = false,
        };
        public static BeepTheme ZenTheme => new BeepTheme
        {
            GridBackColor = Color.FromArgb(245, 245, 245), // Light Gray
            GridForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBackColor = Color.FromArgb(220, 220, 220), // Soft Gray
            GridHeaderForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBorderColor = Color.Silver,
            GridHeaderHoverBackColor = Color.FromArgb(200, 200, 200), // Darker Gray
            GridHeaderHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderSelectedBackColor = Color.FromArgb(180, 180, 180), // Deeper Gray
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.Silver,
            GridHeaderSelectedBorderColor = Color.Gray,
            GridRowHoverBackColor = Color.WhiteSmoke,
            GridRowHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridRowSelectedBackColor = Color.FromArgb(150, 150, 150), // Gray Tone
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.Silver,
            GridRowSelectedBorderColor = Color.Gray,
            CardBackColor = Color.FromArgb(240, 234, 214),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),  // Dark Gray
            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(90, 90, 90),  // Medium Gray
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(220, 220, 220),        // Light Gray
            MaxButtonColor = Color.FromArgb(200, 200, 200),          // Lighter Gray
            MinButtonColor = Color.FromArgb(200, 200, 200),
            TitleBarColor = Color.FromArgb(245, 245, 245),           // Very Light Gray
            TitleBarTextColor = Color.FromArgb(60, 60, 60),          // Dark Gray
            TitleBarIconColor = Color.FromArgb(120, 120, 120),       // Gray
            TitleBarHoverColor = Color.FromArgb(230, 230, 230),      // Light Gray
            TitleBarHoverTextColor = Color.FromArgb(60, 60, 60),
            TitleBarHoverIconColor = Color.FromArgb(60, 60, 60),
            TitleBarActiveColor = Color.FromArgb(240, 240, 240),     // Slightly Darker Light Gray
            TitleBarActiveTextColor = Color.FromArgb(60, 60, 60),
            TitleBarActiveIconColor = Color.FromArgb(60, 60, 60),
            TitleBarInactiveColor = Color.FromArgb(245, 245, 245),
            TitleBarInactiveTextColor = Color.FromArgb(120, 120, 120),
            TitleBarInactiveIconColor = Color.FromArgb(120, 120, 120),
            TitleBarBorderColor = Color.FromArgb(200, 200, 200),
            TitleBarBorderHoverColor = Color.FromArgb(180, 180, 180),
            TitleBarBorderActiveColor = Color.FromArgb(160, 160, 160),
            TitleBarBorderInactiveColor = Color.FromArgb(200, 200, 200),

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(200, 50, 50),    // Soft Red
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(180, 40, 40),
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(220, 220, 220),
            TitleBarCloseInactiveTextColor = Color.FromArgb(120, 120, 120),
            TitleBarCloseInactiveIconColor = Color.FromArgb(120, 120, 120),
            TitleBarCloseBorderColor = Color.FromArgb(200, 200, 200),
            TitleBarCloseBorderHoverColor = Color.FromArgb(200, 50, 50),
            TitleBarCloseBorderActiveColor = Color.FromArgb(180, 40, 40),
            TitleBarCloseBorderInactiveColor = Color.FromArgb(200, 200, 200),

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(230, 230, 230),
            TitleBarMaxHoverTextColor = Color.FromArgb(60, 60, 60),
            TitleBarMaxHoverIconColor = Color.FromArgb(60, 60, 60),
            TitleBarMaxActiveColor = Color.FromArgb(240, 240, 240),
            TitleBarMaxActiveTextColor = Color.FromArgb(60, 60, 60),
            TitleBarMaxActiveIconColor = Color.FromArgb(60, 60, 60),
            TitleBarMaxInactiveColor = Color.FromArgb(245, 245, 245),
            TitleBarMaxInactiveTextColor = Color.FromArgb(120, 120, 120),
            TitleBarMaxInactiveIconColor = Color.FromArgb(120, 120, 120),
            TitleBarMaxBorderColor = Color.FromArgb(200, 200, 200),
            TitleBarMaxBorderHoverColor = Color.FromArgb(180, 180, 180),
            TitleBarMaxBorderActiveColor = Color.FromArgb(160, 160, 160),
            TitleBarMaxBorderInactiveColor = Color.FromArgb(200, 200, 200),

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(230, 230, 230),
            TitleBarMinHoverTextColor = Color.FromArgb(60, 60, 60),
            TitleBarMinHoverIconColor = Color.FromArgb(60, 60, 60),
            TitleBarMinActiveColor = Color.FromArgb(240, 240, 240),
            TitleBarMinActiveTextColor = Color.FromArgb(60, 60, 60),
            TitleBarMinActiveIconColor = Color.FromArgb(60, 60, 60),
            TitleBarMinInactiveColor = Color.FromArgb(245, 245, 245),
            TitleBarMinInactiveTextColor = Color.FromArgb(120, 120, 120),
            TitleBarMinInactiveIconColor = Color.FromArgb(120, 120, 120),
            TitleBarMinBorderColor = Color.FromArgb(200, 200, 200),
            TitleBarMinBorderHoverColor = Color.FromArgb(180, 180, 180),
            TitleBarMinBorderActiveColor = Color.FromArgb(160, 160, 160),
            TitleBarMinBorderInactiveColor = Color.FromArgb(200, 200, 200),

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(230, 230, 230),
            TitleBarMinimizeHoverTextColor = Color.FromArgb(60, 60, 60),
            TitleBarMinimizeHoverIconColor = Color.FromArgb(60, 60, 60),
            TitleBarMinimizeActiveColor = Color.FromArgb(240, 240, 240),
            TitleBarMinimizeActiveTextColor = Color.FromArgb(60, 60, 60),
            TitleBarMinimizeActiveIconColor = Color.FromArgb(60, 60, 60),
            TitleBarMinimizeInactiveColor = Color.FromArgb(245, 245, 245),
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(120, 120, 120),
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(120, 120, 120),
            TitleBarMinimizeBorderColor = Color.FromArgb(200, 200, 200),
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(180, 180, 180),
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(160, 160, 160),
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(200, 200, 200),

            // **General Colors**
            TitleForColor = Color.FromArgb(60, 60, 60),       // Dark Gray
            TitleBarForColor = Color.FromArgb(90, 90, 90),    // Medium Gray
            DescriptionForColor = Color.FromArgb(120, 120, 120), // Gray
            BeforeForColor = Color.FromArgb(200, 200, 200),     // Light Gray
            LatestForColor = Color.FromArgb(180, 180, 180),     // Slightly Darker Gray
            BackColor = Color.FromArgb(250, 250, 250),          // Very Light Gray

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(220, 220, 220),    // Light Gray
            ButtonForeColor = Color.FromArgb(60, 60, 60),
            ButtonHoverBackColor = Color.FromArgb(200, 200, 200),
            ButtonHoverForeColor = Color.FromArgb(60, 60, 60),
            ButtonActiveBackColor = Color.FromArgb(180, 180, 180),
            ButtonActiveForeColor = Color.FromArgb(60, 60, 60),

            // **TextBox Colors**
            TextBoxBackColor = Color.White,
            TextBoxForeColor = Color.FromArgb(60, 60, 60),

            // **Label Colors**
            LabelBackColor = Color.White,
            LabelForeColor = Color.FromArgb(60, 60, 60),

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(240, 240, 240), // Light Gray
        

            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(230, 230, 230),
            HeaderForeColor = Color.FromArgb(60, 60, 60),
            GridLineColor = Color.FromArgb(200, 200, 200),
            RowBackColor = Color.White,
            RowForeColor = Color.FromArgb(60, 60, 60),
            AltRowBackColor = Color.FromArgb(245, 245, 245),
            SelectedRowBackColor = Color.FromArgb(220, 220, 220),
            SelectedRowForeColor = Color.FromArgb(60, 60, 60),

            // **ComboBox Colors**
            ComboBoxBackColor = Color.White,
            ComboBoxForeColor = Color.FromArgb(60, 60, 60),

            // **CheckBox Colors**
            CheckBoxBackColor = Color.White,
            CheckBoxForeColor = Color.FromArgb(60, 60, 60),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.White,
            RadioButtonForeColor = Color.FromArgb(60, 60, 60),

            // **Border Colors**
            BorderColor = Color.FromArgb(200, 200, 200),
            ActiveBorderColor = Color.FromArgb(180, 180, 180),
            InactiveBorderColor = Color.FromArgb(220, 220, 220),
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(60, 60, 60),
            VisitedLinkColor = Color.FromArgb(90, 90, 90),
            HoverLinkColor = Color.FromArgb(30, 30, 30),
            LinkHoverColor = Color.FromArgb(30, 30, 30),
            LinkIsUnderline = false,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(60, 60, 60),
            ToolTipForeColor = Color.White,

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(245, 245, 245),
            ScrollBarThumbColor = Color.FromArgb(200, 200, 200),
            ScrollBarTrackColor = Color.FromArgb(220, 220, 220),

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(230, 230, 230),
            StatusBarForeColor = Color.FromArgb(60, 60, 60),

            // **Tab Colors**
            TabBackColor = Color.FromArgb(245, 245, 245),
            TabForeColor = Color.FromArgb(60, 60, 60),
            ActiveTabBackColor = Color.FromArgb(220, 220, 220),
            ActiveTabForeColor = Color.FromArgb(60, 60, 60),

            // **Dialog Box Colors**
            DialogBackColor = Color.White,
            DialogForeColor = Color.FromArgb(60, 60, 60),
            DialogButtonBackColor = Color.FromArgb(220, 220, 220),
            DialogButtonForeColor = Color.FromArgb(60, 60, 60),

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(250, 250, 250),
            GradientEndColor = Color.FromArgb(220, 220, 220),
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(230, 230, 230), // Medium Gray
            SideMenuHoverBackColor = Color.FromArgb(230, 230, 230),
            SideMenuSelectedBackColor = Color.FromArgb(220, 220, 220),
            SideMenuForeColor = Color.FromArgb(60, 60, 60),
            SideMenuHoverForeColor = Color.FromArgb(60, 60, 60),
            SideMenuSelectedForeColor = Color.FromArgb(60, 60, 60),
            SideMenuBorderColor = Color.FromArgb(200, 200, 200),
            SideMenuIconColor = Color.FromArgb(60, 60, 60),
            SideMenuSelectedIconColor = Color.FromArgb(60, 60, 60),

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(245, 245, 245),
            TitleBarForeColor = Color.FromArgb(60, 60, 60),
            TitleBarHoverBackColor = Color.FromArgb(230, 230, 230),
            TitleBarHoverForeColor = Color.FromArgb(60, 60, 60),

            // **Dashboard Colors**
            DashboardBackColor = Color.FromArgb(250, 250, 250),
            DashboardCardBackColor = Color.White,
            DashboardCardHoverBackColor = Color.FromArgb(245, 245, 245),
            CardTitleForeColor = Color.FromArgb(60, 60, 60),
            CardTextForeColor = Color.FromArgb(90, 90, 90),

            // **Data Visualization (Charts)**
            ChartBackColor = Color.White,
            ChartLineColor = Color.FromArgb(60, 60, 60),
            ChartFillColor = Color.FromArgb(100, 60, 60, 60), // Semi-transparent Dark Gray
            ChartAxisColor = Color.FromArgb(90, 90, 90),

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(60, 60, 60),
            SidebarSelectedIconColor = Color.FromArgb(60, 60, 60),
            SidebarTextColor = Color.FromArgb(60, 60, 60),
            SidebarSelectedTextColor = Color.FromArgb(60, 60, 60),

            // **Navigation Colors**
            NavigationBackColor = Color.FromArgb(245, 245, 245),
            NavigationForeColor = Color.FromArgb(60, 60, 60),
            NavigationHoverBackColor = Color.FromArgb(230, 230, 230),
            NavigationHoverForeColor = Color.FromArgb(60, 60, 60),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(200, 200, 200),
            BadgeForeColor = Color.FromArgb(60, 60, 60),
            HighlightBackColor = Color.FromArgb(220, 220, 220),

            // **Font Properties**
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(60, 60, 60),
            SecondaryTextColor = Color.FromArgb(90, 90, 90),
            AccentTextColor = Color.FromArgb(120, 120, 120),

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(60, 60, 60),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(90, 90, 90),
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(90, 90, 90),
            },
            BlockquoteBorderColor = Color.FromArgb(200, 200, 200),
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(90, 90, 90),
            },
            InlineCodeBackgroundColor = Color.FromArgb(240, 240, 240),
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(90, 90, 90),
            },
            CodeBlockBackgroundColor = Color.FromArgb(240, 240, 240),
            CodeBlockBorderColor = Color.FromArgb(200, 200, 200),
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(90, 90, 90),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(90, 90, 90),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(90, 90, 90),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(90, 90, 90),
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(60, 60, 60),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(90, 90, 90),
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(90, 90, 90),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(90, 90, 90),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(60, 60, 60),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(60, 60, 60),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(60, 60, 60),
            SecondaryColor = Color.FromArgb(90, 90, 90),
            AccentColor = Color.FromArgb(120, 120, 120),
            BackgroundColor = Color.FromArgb(250, 250, 250),
            SurfaceColor = Color.White,
            ErrorColor = Color.FromArgb(200, 50, 50),
            WarningColor = Color.FromArgb(200, 150, 50),
            SuccessColor = Color.FromArgb(50, 150, 50),
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.FromArgb(60, 60, 60),

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 4,

            // **Imagery and Iconography**
            IconSet = "ZenIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(100, 0, 0, 0), // Semi-transparent black
            ShadowOpacity = 0.1f,

            // **Animation and Transitions**
            AnimationDurationShort = 150,  // in milliseconds
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-out",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(60, 60, 60),

            // **Theme Variant**
            IsDarkTheme = false,
        };
        public static BeepTheme RetroTheme => new BeepTheme
        {
            GridBackColor = Color.FromArgb(250, 235, 215), // Antique White
            GridForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBackColor = Color.FromArgb(240, 230, 140), // Khaki
            GridHeaderForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBorderColor = Color.Olive,
            GridHeaderHoverBackColor = Color.FromArgb(218, 165, 32), // Goldenrod
            GridHeaderHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderSelectedBackColor = Color.FromArgb(184, 134, 11), // Dark Goldenrod
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.Olive,
            GridHeaderSelectedBorderColor = Color.DarkGoldenrod,
            GridRowHoverBackColor = Color.FromArgb(245, 222, 179), // Wheat
            GridRowHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridRowSelectedBackColor = Color.DarkGoldenrod,
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.Olive,
            GridRowSelectedBorderColor = Color.DarkGoldenrod,
            CardBackColor = Color.FromArgb(250, 214, 165),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 20, 147),  // Deep Pink
            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 255, 255),  // Aqua
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(255, 105, 180),        // Hot Pink
            MaxButtonColor = Color.FromArgb(0, 255, 127),            // Spring Green
            MinButtonColor = Color.FromArgb(30, 144, 255),           // Dodger Blue
            TitleBarColor = Color.FromArgb(75, 0, 130),              // Indigo
            TitleBarTextColor = Color.FromArgb(255, 255, 0),         // Yellow
            TitleBarIconColor = Color.FromArgb(255, 20, 147),        // Deep Pink
            TitleBarHoverColor = Color.FromArgb(148, 0, 211),        // Dark Violet
            TitleBarHoverTextColor = Color.White,
            TitleBarHoverIconColor = Color.White,
            TitleBarActiveColor = Color.FromArgb(75, 0, 130),        // Indigo
            TitleBarActiveTextColor = Color.White,
            TitleBarActiveIconColor = Color.White,
            TitleBarInactiveColor = Color.FromArgb(123, 104, 238),   // Medium Slate Blue
            TitleBarInactiveTextColor = Color.FromArgb(255, 255, 0), // Yellow
            TitleBarInactiveIconColor = Color.FromArgb(255, 255, 0),
            TitleBarBorderColor = Color.FromArgb(255, 105, 180),     // Hot Pink
            TitleBarBorderHoverColor = Color.FromArgb(0, 255, 127),  // Spring Green
            TitleBarBorderActiveColor = Color.FromArgb(30, 144, 255),// Dodger Blue
            TitleBarBorderInactiveColor = Color.FromArgb(123, 104, 238), // Medium Slate Blue

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(255, 69, 0),    // Orange Red
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(220, 20, 60),  // Crimson
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(255, 105, 180),   // Hot Pink
            TitleBarCloseInactiveTextColor = Color.FromArgb(255, 255, 0),
            TitleBarCloseInactiveIconColor = Color.FromArgb(255, 255, 0),
            TitleBarCloseBorderColor = Color.FromArgb(255, 105, 180),
            TitleBarCloseBorderHoverColor = Color.FromArgb(255, 69, 0),   // Orange Red
            TitleBarCloseBorderActiveColor = Color.FromArgb(220, 20, 60), // Crimson
            TitleBarCloseBorderInactiveColor = Color.FromArgb(255, 105, 180), // Hot Pink

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(0, 255, 127),       // Spring Green
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.White,
            TitleBarMaxActiveColor = Color.FromArgb(60, 179, 113),     // Medium Sea Green
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(123, 104, 238),  // Medium Slate Blue
            TitleBarMaxInactiveTextColor = Color.FromArgb(255, 255, 0),
            TitleBarMaxInactiveIconColor = Color.FromArgb(255, 255, 0),
            TitleBarMaxBorderColor = Color.FromArgb(0, 255, 127),      // Spring Green
            TitleBarMaxBorderHoverColor = Color.FromArgb(60, 179, 113),// Medium Sea Green
            TitleBarMaxBorderActiveColor = Color.FromArgb(0, 255, 127),// Spring Green
            TitleBarMaxBorderInactiveColor = Color.FromArgb(123, 104, 238), // Medium Slate Blue

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(30, 144, 255),       // Dodger Blue
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.White,
            TitleBarMinActiveColor = Color.FromArgb(0, 191, 255),       // Deep Sky Blue
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(123, 104, 238),   // Medium Slate Blue
            TitleBarMinInactiveTextColor = Color.FromArgb(255, 255, 0),
            TitleBarMinInactiveIconColor = Color.FromArgb(255, 255, 0),
            TitleBarMinBorderColor = Color.FromArgb(30, 144, 255),      // Dodger Blue
            TitleBarMinBorderHoverColor = Color.FromArgb(0, 191, 255),  // Deep Sky Blue
            TitleBarMinBorderActiveColor = Color.FromArgb(30, 144, 255),// Dodger Blue
            TitleBarMinBorderInactiveColor = Color.FromArgb(123, 104, 238), // Medium Slate Blue

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(30, 144, 255),  // Dodger Blue
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(0, 191, 255),  // Deep Sky Blue
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(123, 104, 238), // Medium Slate Blue
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(255, 255, 0),
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(255, 255, 0),
            TitleBarMinimizeBorderColor = Color.FromArgb(30, 144, 255),   // Dodger Blue
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(0, 191, 255),// Deep Sky Blue
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(30, 144, 255),// Dodger Blue
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(123, 104, 238), // Medium Slate Blue

            // **General Colors**
            TitleForColor = Color.FromArgb(255, 255, 0),           // Yellow
            TitleBarForColor = Color.FromArgb(0, 255, 127),        // Spring Green
            DescriptionForColor = Color.FromArgb(255, 20, 147),    // Deep Pink
            BeforeForColor = Color.FromArgb(0, 191, 255),          // Deep Sky Blue
            LatestForColor = Color.FromArgb(255, 105, 180),        // Hot Pink
            BackColor =Color.FromArgb(255, 0, 0, 0),                   // Black background
            
            // **Button Colors**
            ButtonBackColor = Color.FromArgb(255, 20, 147),        // Deep Pink
            ButtonForeColor = Color.White,
            ButtonHoverBackColor = Color.FromArgb(0, 255, 255),    // Aqua
            ButtonHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            ButtonActiveBackColor = Color.FromArgb(255, 255, 0),   // Yellow
            ButtonActiveForeColor = Color.FromArgb(255, 0, 0, 0),

            // **TextBox Colors**
            TextBoxBackColor = Color.FromArgb(255, 0, 0, 0),
            TextBoxForeColor = Color.FromArgb(0, 255, 127),        // Spring Green

            // **Label Colors**
            LabelBackColor = Color.FromArgb(255, 0, 0, 0),
            LabelForeColor = Color.FromArgb(255, 255, 0),          // Yellow

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(123, 104, 238), // Medium Slate Blue
            


            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(75, 0, 130),          // Indigo
            HeaderForeColor = Color.FromArgb(255, 255, 0),
            GridLineColor = Color.FromArgb(0, 255, 255),           // Aqua
            RowBackColor = Color.FromArgb(255, 0, 0, 0),
            RowForeColor = Color.FromArgb(255, 255, 0),
            AltRowBackColor = Color.FromArgb(75, 0, 130),
            SelectedRowBackColor = Color.FromArgb(255, 20, 147),   // Deep Pink
            SelectedRowForeColor = Color.White,

            // **ComboBox Colors**
            ComboBoxBackColor = Color.FromArgb(255, 0, 0, 0),
            ComboBoxForeColor = Color.FromArgb(0, 255, 255),       // Aqua

            // **CheckBox Colors**
            CheckBoxBackColor = Color.FromArgb(255, 0, 0, 0),
            CheckBoxForeColor = Color.FromArgb(255, 255, 0),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.FromArgb(255, 0, 0, 0),
            RadioButtonForeColor = Color.FromArgb(255, 20, 147),

            // **Border Colors**
            BorderColor = Color.FromArgb(255, 105, 180),           // Hot Pink
            ActiveBorderColor = Color.FromArgb(0, 255, 127),       // Spring Green
            InactiveBorderColor = Color.FromArgb(75, 0, 130),      // Indigo
            BorderSize = 2,

            // **Link Colors**
            LinkColor = Color.FromArgb(0, 255, 255),               // Aqua
            VisitedLinkColor = Color.FromArgb(255, 20, 147),       // Deep Pink
            HoverLinkColor = Color.FromArgb(255, 255, 0),          // Yellow
            LinkHoverColor = Color.FromArgb(255, 255, 0),          // Yellow
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(75, 0, 130),         // Indigo
            ToolTipForeColor = Color.White,

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(255, 0, 0, 0),
            ScrollBarThumbColor = Color.FromArgb(255, 105, 180),   // Hot Pink
            ScrollBarTrackColor = Color.FromArgb(75, 0, 130),

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(0, 255, 127),      // Spring Green
            StatusBarForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Tab Colors**
            TabBackColor = Color.FromArgb(255, 0, 0, 0),
            TabForeColor = Color.FromArgb(0, 255, 255),            // Aqua
            ActiveTabBackColor = Color.FromArgb(255, 20, 147),     // Deep Pink
            ActiveTabForeColor = Color.White,

            // **Dialog Box Colors**
            DialogBackColor = Color.FromArgb(75, 0, 130),
            DialogForeColor = Color.White,
            DialogButtonBackColor = Color.FromArgb(0, 255, 127),
            DialogButtonForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(255, 0, 0, 0),
            GradientEndColor = Color.FromArgb(75, 0, 130),
            GradientDirection = LinearGradientMode.ForwardDiagonal,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(148, 0, 211), // Dark Violet
            SideMenuHoverBackColor = Color.FromArgb(75, 0, 130),
            SideMenuSelectedBackColor = Color.FromArgb(255, 20, 147),
            SideMenuForeColor = Color.FromArgb(0, 255, 255),       // Aqua
            SideMenuHoverForeColor = Color.FromArgb(0, 255, 255),
            SideMenuSelectedForeColor = Color.White,
            SideMenuBorderColor = Color.FromArgb(255, 105, 180),   // Hot Pink
            SideMenuIconColor = Color.FromArgb(0, 255, 255),
            SideMenuSelectedIconColor = Color.White,

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(75, 0, 130),        // Indigo
            TitleBarForeColor = Color.FromArgb(255, 255, 0),       // Yellow
            TitleBarHoverBackColor = Color.FromArgb(148, 0, 211),  // Dark Violet
            TitleBarHoverForeColor = Color.White,

            // **Dashboard Colors**
            DashboardBackColor = Color.FromArgb(255, 0, 0, 0),
            DashboardCardBackColor = Color.FromArgb(75, 0, 130),
            DashboardCardHoverBackColor = Color.FromArgb(148, 0, 211),
            CardTitleForeColor = Color.FromArgb(255, 255, 0),      // Yellow
            CardTextForeColor = Color.FromArgb(0, 255, 255),       // Aqua

            // **Data Visualization (Charts)**
            ChartBackColor = Color.FromArgb(255, 0, 0, 0),
            ChartLineColor = Color.FromArgb(0, 255, 255),          // Aqua
            ChartFillColor = Color.FromArgb(100, 0, 255, 255),     // Semi-transparent Aqua
            ChartAxisColor = Color.FromArgb(255, 255, 0),          // Yellow

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(0, 255, 255),
            SidebarSelectedIconColor = Color.White,
            SidebarTextColor = Color.FromArgb(0, 255, 255),
            SidebarSelectedTextColor = Color.White,

            // **Navigation Colors**
            NavigationBackColor = Color.FromArgb(255, 0, 0, 0),
            NavigationForeColor = Color.FromArgb(0, 255, 255),
            NavigationHoverBackColor = Color.FromArgb(75, 0, 130),
            NavigationHoverForeColor = Color.FromArgb(0, 255, 255),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(255, 20, 147),         // Deep Pink
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(0, 255, 255),      // Aqua

            // **Font Properties**
            FontFamily = "Arial",
            FontName = "Arial",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(0, 255, 255),        // Aqua
            SecondaryTextColor = Color.FromArgb(255, 20, 147),     // Deep Pink
            AccentTextColor = Color.FromArgb(255, 255, 0),         // Yellow

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(255, 255, 0),  // Yellow
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(0, 255, 255),  // Aqua
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(255, 20, 147), // Deep Pink
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 255, 0),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 20, 147),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(0, 255, 255),  // Aqua
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 255, 0), // Yellow
            },
            BlockquoteBorderColor = Color.FromArgb(255, 105, 180), // Hot Pink
            BlockquoteBorderWidth = 2f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Courier New",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 255, 127), // Spring Green
            },
            InlineCodeBackgroundColor = Color.FromArgb(75, 0, 130), // Indigo
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Courier New",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 255, 127),
            },
            CodeBlockBackgroundColor =Color.FromArgb(255, 0, 0, 0),     // Black
            CodeBlockBorderColor = Color.FromArgb(255, 20, 147),    // Deep Pink
            CodeBlockBorderWidth = 2f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 0), // Yellow
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 20, 147), // Deep Pink
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 0), // Yellow
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 255, 255), // Aqua
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 20, 147), // Deep Pink
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 0),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 20, 147),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(255, 255, 0),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(255, 20, 147),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 255, 255),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 0),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Arial",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(255, 20, 147),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(255, 20, 147),         // Deep Pink
            SecondaryColor = Color.FromArgb(0, 255, 255),        // Aqua
            AccentColor = Color.FromArgb(255, 255, 0),           // Yellow
            BackgroundColor =Color.FromArgb(255, 0, 0, 0),           // Black
            SurfaceColor = Color.FromArgb(255, 0, 0, 0),
            ErrorColor = Color.FromArgb(220, 20, 60),            // Crimson
            WarningColor = Color.FromArgb(255, 69, 0),           // Orange Red
            SuccessColor = Color.FromArgb(0, 255, 127),          // Spring Green
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.FromArgb(0, 255, 255),

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 0,

            // **Imagery and Iconography**
            IconSet = "RetroIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(100, 255, 255, 0), // Semi-transparent Yellow
            ShadowOpacity = 0.3f,

            // **Animation and Transitions**
            AnimationDurationShort = 100,  // in milliseconds
            AnimationDurationMedium = 200,
            AnimationDurationLong = 300,
            AnimationEasingFunction = "linear",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(255, 255, 0), // Yellow

            // **Theme Variant**
            IsDarkTheme = true,
        };
        public static BeepTheme RoyalTheme => new BeepTheme
        {
            GridBackColor = Color.White,
            GridForeColor = Color.FromArgb(0, 0, 128), // Navy
            GridHeaderBackColor = Color.FromArgb(65, 105, 225), // Royal Blue
            GridHeaderForeColor = Color.White,
            GridHeaderBorderColor = Color.Blue,
            GridHeaderHoverBackColor = Color.FromArgb(70, 130, 180), // Steel Blue
            GridHeaderHoverForeColor = Color.White,
            GridHeaderSelectedBackColor = Color.FromArgb(0, 0, 205), // Medium Blue
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.Blue,
            GridHeaderSelectedBorderColor = Color.FromArgb(0, 0, 205),
            GridRowHoverBackColor = Color.LightSteelBlue,
            GridRowHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridRowSelectedBackColor = Color.MediumBlue,
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.Blue,
            GridRowSelectedBorderColor = Color.MediumBlue,
            CardBackColor = Color.FromArgb(250, 230, 190),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(75, 0, 130),  // Indigo
            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(25, 25, 112), // Midnight Blue
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(139, 0, 0),           // Dark Red
            MaxButtonColor = Color.FromArgb(0, 0, 128),             // Navy
            MinButtonColor = Color.FromArgb(0, 0, 128),
            TitleBarColor = Color.FromArgb(75, 0, 130),             // Indigo
            TitleBarTextColor = Color.FromArgb(255, 215, 0),        // Gold
            TitleBarIconColor = Color.FromArgb(218, 165, 32),       // Goldenrod
            TitleBarHoverColor = Color.FromArgb(85, 26, 139),       // Purple
            TitleBarHoverTextColor = Color.FromArgb(255, 215, 0),
            TitleBarHoverIconColor = Color.FromArgb(255, 215, 0),
            TitleBarActiveColor = Color.FromArgb(75, 0, 130),
            TitleBarActiveTextColor = Color.FromArgb(255, 215, 0),
            TitleBarActiveIconColor = Color.FromArgb(255, 215, 0),
            TitleBarInactiveColor = Color.FromArgb(72, 61, 139),    // Dark Slate Blue
            TitleBarInactiveTextColor = Color.FromArgb(218, 165, 32),
            TitleBarInactiveIconColor = Color.FromArgb(218, 165, 32),
            TitleBarBorderColor = Color.FromArgb(218, 165, 32),
            TitleBarBorderHoverColor = Color.FromArgb(184, 134, 11),
            TitleBarBorderActiveColor = Color.FromArgb(184, 134, 11),
            TitleBarBorderInactiveColor = Color.FromArgb(218, 165, 32),

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(165, 42, 42),   // Brown
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(139, 0, 0),    // Dark Red
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(128, 0, 0),
            TitleBarCloseInactiveTextColor = Color.FromArgb(218, 165, 32),
            TitleBarCloseInactiveIconColor = Color.FromArgb(218, 165, 32),
            TitleBarCloseBorderColor = Color.FromArgb(218, 165, 32),
            TitleBarCloseBorderHoverColor = Color.FromArgb(184, 134, 11),
            TitleBarCloseBorderActiveColor = Color.FromArgb(184, 134, 11),
            TitleBarCloseBorderInactiveColor = Color.FromArgb(218, 165, 32),

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(25, 25, 112),     // Midnight Blue
            TitleBarMaxHoverTextColor = Color.FromArgb(255, 215, 0),
            TitleBarMaxHoverIconColor = Color.FromArgb(255, 215, 0),
            TitleBarMaxActiveColor = Color.FromArgb(0, 0, 128),      // Navy
            TitleBarMaxActiveTextColor = Color.FromArgb(255, 215, 0),
            TitleBarMaxActiveIconColor = Color.FromArgb(255, 215, 0),
            TitleBarMaxInactiveColor = Color.FromArgb(72, 61, 139),  // Dark Slate Blue
            TitleBarMaxInactiveTextColor = Color.FromArgb(218, 165, 32),
            TitleBarMaxInactiveIconColor = Color.FromArgb(218, 165, 32),
            TitleBarMaxBorderColor = Color.FromArgb(218, 165, 32),
            TitleBarMaxBorderHoverColor = Color.FromArgb(184, 134, 11),
            TitleBarMaxBorderActiveColor = Color.FromArgb(184, 134, 11),
            TitleBarMaxBorderInactiveColor = Color.FromArgb(218, 165, 32),

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(25, 25, 112),
            TitleBarMinHoverTextColor = Color.FromArgb(255, 215, 0),
            TitleBarMinHoverIconColor = Color.FromArgb(255, 215, 0),
            TitleBarMinActiveColor = Color.FromArgb(0, 0, 128),
            TitleBarMinActiveTextColor = Color.FromArgb(255, 215, 0),
            TitleBarMinActiveIconColor = Color.FromArgb(255, 215, 0),
            TitleBarMinInactiveColor = Color.FromArgb(72, 61, 139),
            TitleBarMinInactiveTextColor = Color.FromArgb(218, 165, 32),
            TitleBarMinInactiveIconColor = Color.FromArgb(218, 165, 32),
            TitleBarMinBorderColor = Color.FromArgb(218, 165, 32),
            TitleBarMinBorderHoverColor = Color.FromArgb(184, 134, 11),
            TitleBarMinBorderActiveColor = Color.FromArgb(184, 134, 11),
            TitleBarMinBorderInactiveColor = Color.FromArgb(218, 165, 32),

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(25, 25, 112),
            TitleBarMinimizeHoverTextColor = Color.FromArgb(255, 215, 0),
            TitleBarMinimizeHoverIconColor = Color.FromArgb(255, 215, 0),
            TitleBarMinimizeActiveColor = Color.FromArgb(0, 0, 128),
            TitleBarMinimizeActiveTextColor = Color.FromArgb(255, 215, 0),
            TitleBarMinimizeActiveIconColor = Color.FromArgb(255, 215, 0),
            TitleBarMinimizeInactiveColor = Color.FromArgb(72, 61, 139),
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(218, 165, 32),
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(218, 165, 32),
            TitleBarMinimizeBorderColor = Color.FromArgb(218, 165, 32),
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(184, 134, 11),
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(184, 134, 11),
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(218, 165, 32),

            // **General Colors**
            TitleForColor = Color.FromArgb(255, 215, 0),             // Gold
            TitleBarForColor = Color.FromArgb(184, 134, 11),         // Dark Goldenrod
            DescriptionForColor = Color.FromArgb(218, 165, 32),      // Goldenrod
            BeforeForColor = Color.FromArgb(72, 61, 139),            // Dark Slate Blue
            LatestForColor = Color.FromArgb(139, 0, 0),              // Dark Red
            BackColor = Color.FromArgb(248, 248, 255),               // Ghost White

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(75, 0, 130),            // Indigo
            ButtonForeColor = Color.FromArgb(255, 215, 0),
            ButtonHoverBackColor = Color.FromArgb(85, 26, 139),      // Purple
            ButtonHoverForeColor = Color.FromArgb(255, 215, 0),
            ButtonActiveBackColor = Color.FromArgb(72, 61, 139),     // Dark Slate Blue
            ButtonActiveForeColor = Color.FromArgb(255, 215, 0),

            // **TextBox Colors**
            TextBoxBackColor = Color.White,
            TextBoxForeColor =Color.FromArgb(255, 0, 0, 0),              // Black

            // **Label Colors**
            LabelBackColor = Color.FromArgb(75, 0, 130),
            LabelForeColor = Color.White,

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(72, 61, 139), // Dark Slate Blue
            


            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(75, 0, 130),
            HeaderForeColor = Color.FromArgb(255, 215, 0),
            GridLineColor = Color.FromArgb(218, 165, 32),
            RowBackColor = Color.White,
            RowForeColor =Color.FromArgb(255, 0, 0, 0),
            AltRowBackColor = Color.FromArgb(248, 248, 255),
            SelectedRowBackColor = Color.FromArgb(85, 26, 139),
            SelectedRowForeColor = Color.FromArgb(255, 215, 0),

            // **ComboBox Colors**
            ComboBoxBackColor = Color.White,
            ComboBoxForeColor =Color.FromArgb(255, 0, 0, 0),

            // **CheckBox Colors**
            CheckBoxBackColor = Color.White,
            CheckBoxForeColor =Color.FromArgb(255, 0, 0, 0),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.White,
            RadioButtonForeColor =Color.FromArgb(255, 0, 0, 0),

            // **Border Colors**
            BorderColor = Color.FromArgb(218, 165, 32),
            ActiveBorderColor = Color.FromArgb(184, 134, 11),
            InactiveBorderColor = Color.FromArgb(218, 165, 32),
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(75, 0, 130),
            VisitedLinkColor = Color.FromArgb(72, 61, 139),
            HoverLinkColor = Color.FromArgb(85, 26, 139),
            LinkHoverColor = Color.FromArgb(85, 26, 139),
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(75, 0, 130),
            ToolTipForeColor = Color.FromArgb(255, 215, 0),

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(248, 248, 255),
            ScrollBarThumbColor = Color.FromArgb(218, 165, 32),
            ScrollBarTrackColor = Color.FromArgb(184, 134, 11),

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(75, 0, 130),
            StatusBarForeColor = Color.FromArgb(255, 215, 0),

            // **Tab Colors**
            TabBackColor = Color.FromArgb(248, 248, 255),
            TabForeColor =Color.FromArgb(255, 0, 0, 0),
            ActiveTabBackColor = Color.FromArgb(75, 0, 130),
            ActiveTabForeColor = Color.FromArgb(255, 215, 0),

            // **Dialog Box Colors**
            DialogBackColor = Color.FromArgb(248, 248, 255),
            DialogForeColor =Color.FromArgb(255, 0, 0, 0),
            DialogButtonBackColor = Color.FromArgb(75, 0, 130),
            DialogButtonForeColor = Color.FromArgb(255, 215, 0),

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(248, 248, 255),      // Ghost White
            GradientEndColor = Color.FromArgb(75, 0, 130),           // Indigo
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(25, 25, 112), // Midnight Blue
            SideMenuHoverBackColor = Color.FromArgb(72, 61, 139),
            SideMenuSelectedBackColor = Color.FromArgb(75, 0, 130),
            SideMenuForeColor = Color.Gold,
            SideMenuHoverForeColor = Color.FromArgb(255, 215, 0),
            SideMenuSelectedForeColor = Color.FromArgb(255, 215, 0),
            SideMenuBorderColor = Color.FromArgb(218, 165, 32),
            SideMenuIconColor =Color.FromArgb(255, 0, 0, 0),
            SideMenuSelectedIconColor = Color.FromArgb(255, 215, 0),

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(75, 0, 130),
            TitleBarForeColor = Color.FromArgb(255, 215, 0),
            TitleBarHoverBackColor = Color.FromArgb(85, 26, 139),
            TitleBarHoverForeColor = Color.FromArgb(255, 215, 0),

            // **Dashboard Colors**
            DashboardBackColor = Color.FromArgb(248, 248, 255),
            DashboardCardBackColor = Color.White,
            DashboardCardHoverBackColor = Color.FromArgb(240, 248, 255),
            CardTitleForeColor = Color.FromArgb(75, 0, 130),
            CardTextForeColor = Color.FromArgb(25, 25, 112),

            // **Data Visualization (Charts)**
            ChartBackColor = Color.White,
            ChartLineColor = Color.FromArgb(75, 0, 130),
            ChartFillColor = Color.FromArgb(100, 75, 0, 130), // Semi-transparent Indigo
            ChartAxisColor =Color.FromArgb(255, 0, 0, 0),

            // **Sidebar and Menu Colors**
            SidebarIconColor =Color.FromArgb(255, 0, 0, 0),
            SidebarSelectedIconColor = Color.FromArgb(255, 215, 0),
            SidebarTextColor =Color.FromArgb(255, 0, 0, 0),
            SidebarSelectedTextColor = Color.FromArgb(255, 215, 0),

            // **Navigation Colors**
            NavigationBackColor = Color.FromArgb(248, 248, 255),
            NavigationForeColor =Color.FromArgb(255, 0, 0, 0),
            NavigationHoverBackColor = Color.FromArgb(240, 248, 255),
            NavigationHoverForeColor =Color.FromArgb(255, 0, 0, 0),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(218, 165, 32),
            BadgeForeColor =Color.FromArgb(255, 0, 0, 0),
            HighlightBackColor = Color.FromArgb(184, 134, 11),

            // **Font Properties**
            FontFamily = "Times New Roman",
            FontName = "Times New Roman",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor =Color.FromArgb(255, 0, 0, 0),
            SecondaryTextColor = Color.FromArgb(25, 25, 112),
            AccentTextColor = Color.FromArgb(75, 0, 130),

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(75, 0, 130),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(75, 0, 130),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(75, 0, 130),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(75, 0, 130),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(75, 0, 130),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(75, 0, 130),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(25, 25, 112), // Midnight Blue
            },
            BlockquoteBorderColor = Color.FromArgb(218, 165, 32),
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },
            InlineCodeBackgroundColor = Color.FromArgb(240, 248, 255), // Alice Blue
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },
            CodeBlockBackgroundColor = Color.FromArgb(240, 248, 255),
            CodeBlockBorderColor = Color.FromArgb(218, 165, 32),
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(75, 0, 130),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(75, 0, 130),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(75, 0, 130),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(75, 0, 130),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(75, 0, 130),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(75, 0, 130),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(75, 0, 130),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(75, 0, 130),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(75, 0, 130),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(75, 0, 130),           // Indigo
            SecondaryColor = Color.FromArgb(184, 134, 11),       // Dark Goldenrod
            AccentColor = Color.FromArgb(255, 215, 0),           // Gold
            BackgroundColor = Color.FromArgb(248, 248, 255),     // Ghost White
            SurfaceColor = Color.White,
            ErrorColor = Color.FromArgb(139, 0, 0),              // Dark Red
            WarningColor = Color.FromArgb(184, 134, 11),         // Dark Goldenrod
            SuccessColor = Color.FromArgb(0, 100, 0),            // Dark Green
            OnPrimaryColor = Color.FromArgb(255, 215, 0),
            OnBackgroundColor =Color.FromArgb(255, 0, 0, 0),

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 6,

            // **Imagery and Iconography**
            IconSet = "RoyalIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(100, 0, 0, 0), // Semi-transparent black
            ShadowOpacity = 0.15f,

            // **Animation and Transitions**
            AnimationDurationShort = 200,  // in milliseconds
            AnimationDurationMedium = 400,
            AnimationDurationLong = 600,
            AnimationEasingFunction = "ease-in-out",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(255, 215, 0), // Gold

            // **Theme Variant**
            IsDarkTheme = false,
        };
        public static BeepTheme HighlightTheme => new BeepTheme
        {
            // **Grid Colors**
            GridBackColor = Color.WhiteSmoke,
            GridForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBackColor = Color.Yellow,                    // Bright Yellow for headers
            GridHeaderForeColor = Color.FromArgb(255, 0, 0, 0),                    // Black text for contrast
            GridHeaderBorderColor = Color.Gray,                    // Changed from DarkOrange to Gray
            GridHeaderHoverBackColor = Color.Gold,                 // Gold for hover effect
            GridHeaderHoverForeColor = Color.FromArgb(255, 0, 0, 0),                // Black text remains for readability
            GridHeaderSelectedBackColor = Color.FromArgb(0, 120, 215), // Highlight Blue instead of DarkOrange
            GridHeaderSelectedForeColor = Color.White,             // White text on blue background
            GridHeaderHoverBorderColor = Color.Gray,               // Changed from DarkOrange to Gray
            GridHeaderSelectedBorderColor = Color.FromArgb(0, 120, 215), // Highlight Blue instead of DarkOrange
            GridRowHoverBackColor = Color.LightGray,               // Light Gray for row hover
            GridRowHoverForeColor = Color.FromArgb(255, 0, 0, 0),                   // Black text for readability
            GridRowSelectedBackColor = Color.FromArgb(0, 120, 215), // Highlight Blue instead of DarkOrange
            GridRowSelectedForeColor = Color.White,                // White text on blue background
            GridRowHoverBorderColor = Color.Gray,                  // Changed from DarkOrange to Gray
            GridRowSelectedBorderColor = Color.FromArgb(0, 120, 215), // Highlight Blue instead of DarkOrange

            // **Card Styles**
            CardBackColor = Color.FromArgb(248, 248, 255),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(33, 33, 33),  // Dark Gray
            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(66, 66, 66),  // Medium Gray
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(229, 57, 53),      // Red
            MaxButtonColor = Color.FromArgb(76, 175, 80),        // Green
            MinButtonColor = Color.FromArgb(33, 150, 243),       // Blue
            TitleBarColor = Color.FromArgb(250, 250, 250),       // Near White
            TitleBarTextColor = Color.FromArgb(33, 33, 33),      // Dark Gray
            TitleBarIconColor = Color.FromArgb(33, 33, 33),
            TitleBarHoverColor = Color.FromArgb(240, 240, 240),  // Light Gray
            TitleBarHoverTextColor = Color.FromArgb(33, 33, 33),
            TitleBarHoverIconColor = Color.FromArgb(33, 33, 33),
            TitleBarActiveColor = Color.FromArgb(240, 240, 240),
            TitleBarActiveTextColor = Color.FromArgb(33, 33, 33),
            TitleBarActiveIconColor = Color.FromArgb(33, 33, 33),
            TitleBarInactiveColor = Color.FromArgb(245, 245, 245),
            TitleBarInactiveTextColor = Color.FromArgb(117, 117, 117),
            TitleBarInactiveIconColor = Color.FromArgb(117, 117, 117),
            TitleBarBorderColor = Color.FromArgb(224, 224, 224),
            TitleBarBorderHoverColor = Color.FromArgb(200, 200, 200),
            TitleBarBorderActiveColor = Color.FromArgb(200, 200, 200),
            TitleBarBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // **Close Button**
            TitleBarCloseHoverColor = Color.FromArgb(211, 47, 47),   // Darker Red
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(229, 57, 53),  // Red
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(245, 245, 245),
            TitleBarCloseInactiveTextColor = Color.FromArgb(117, 117, 117),
            TitleBarCloseInactiveIconColor = Color.FromArgb(117, 117, 117),
            TitleBarCloseBorderColor = Color.FromArgb(224, 224, 224),
            TitleBarCloseBorderHoverColor = Color.FromArgb(200, 200, 200),
            TitleBarCloseBorderActiveColor = Color.FromArgb(200, 200, 200),
            TitleBarCloseBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // **Maximize Button**
            TitleBarMaxHoverColor = Color.FromArgb(67, 160, 71),       // Darker Green
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.White,
            TitleBarMaxActiveColor = Color.FromArgb(76, 175, 80),      // Green
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(245, 245, 245),
            TitleBarMaxInactiveTextColor = Color.FromArgb(117, 117, 117),
            TitleBarMaxInactiveIconColor = Color.FromArgb(117, 117, 117),
            TitleBarMaxBorderColor = Color.FromArgb(224, 224, 224),
            TitleBarMaxBorderHoverColor = Color.FromArgb(200, 200, 200),
            TitleBarMaxBorderActiveColor = Color.FromArgb(200, 200, 200),
            TitleBarMaxBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // **Minimize Button**
            TitleBarMinHoverColor = Color.FromArgb(30, 136, 229),       // Darker Blue
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.White,
            TitleBarMinActiveColor = Color.FromArgb(33, 150, 243),      // Blue
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(245, 245, 245),
            TitleBarMinInactiveTextColor = Color.FromArgb(117, 117, 117),
            TitleBarMinInactiveIconColor = Color.FromArgb(117, 117, 117),
            TitleBarMinBorderColor = Color.FromArgb(224, 224, 224),
            TitleBarMinBorderHoverColor = Color.FromArgb(200, 200, 200),
            TitleBarMinBorderActiveColor = Color.FromArgb(200, 200, 200),
            TitleBarMinBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // **Minimize Button (Alternative Properties)**
            TitleBarMinimizeHoverColor = Color.FromArgb(30, 136, 229),
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(33, 150, 243),
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(245, 245, 245),
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(117, 117, 117),
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(117, 117, 117),
            TitleBarMinimizeBorderColor = Color.FromArgb(224, 224, 224),
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(200, 200, 200),
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(200, 200, 200),
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // **General Colors**
            TitleForColor = Color.FromArgb(33, 33, 33),         // Dark Gray
            TitleBarForColor = Color.FromArgb(33, 33, 33),
            DescriptionForColor = Color.FromArgb(66, 66, 66),   // Medium Gray
            BeforeForColor = Color.FromArgb(33, 150, 243),      // Blue
            LatestForColor = Color.FromArgb(76, 175, 80),       // Green
            BackColor = Color.FromArgb(255, 255, 255),          // White

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(0, 120, 215),        // Highlight Blue
            ButtonForeColor = Color.White,
            ButtonHoverBackColor = Color.FromArgb(30, 136, 229),  // Darker Blue on hover
            ButtonHoverForeColor = Color.White,
            ButtonActiveBackColor = Color.FromArgb(0, 153, 188),  // Cyan
            ButtonActiveForeColor = Color.FromArgb(232, 17, 35),  // Red

            // **TextBox Colors**
            TextBoxBackColor = Color.White,
            TextBoxForeColor =Color.FromArgb(255, 0, 0, 0),           // Black
            BackgroundColor = Color.White,

            // **Label Colors**
            LabelBackColor = Color.White,
            LabelForeColor = Color.FromArgb(33, 33, 33),

            // **Panel Colors**
            PanelBackColor = Color.White,

            // **Grid Colors** (Secondary Grid Properties)
            HeaderBackColor = Color.FromArgb(245, 245, 245),      // Light Gray
            HeaderForeColor = Color.FromArgb(33, 33, 33),         // Dark Gray
            GridLineColor = Color.FromArgb(204, 204, 204),        // Gray Lines
            RowBackColor = Color.White,
            RowForeColor =Color.FromArgb(255, 0, 0, 0),               // Black Text
            AltRowBackColor = Color.FromArgb(240, 240, 240),      // Alternate Row Light Gray
            SelectedRowBackColor = Color.FromArgb(0, 153, 188),   // Cyan for Selected Rows
            SelectedRowForeColor = Color.White,                   // White Text for Selected Rows

            // **ComboBox Colors**
            ComboBoxBackColor = Color.White,
            ComboBoxForeColor =Color.FromArgb(255, 0, 0, 0),

            // **CheckBox Colors**
            CheckBoxBackColor = Color.White,
            CheckBoxForeColor =Color.FromArgb(255, 0, 0, 0),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.White,
            RadioButtonForeColor =Color.FromArgb(255, 0, 0, 0),

            // **Border Colors**
            BorderColor = Color.FromArgb(224, 224, 224),
            ActiveBorderColor = Color.FromArgb(0, 120, 215),
            InactiveBorderColor = Color.FromArgb(224, 224, 224),
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(0, 120, 215),
            VisitedLinkColor = Color.FromArgb(0, 99, 177),
            HoverLinkColor = Color.FromArgb(0, 153, 188),
            LinkHoverColor = Color.FromArgb(0, 153, 188),
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(50, 50, 50),
            ToolTipForeColor = Color.White,

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.White,
            ScrollBarThumbColor = Color.FromArgb(0, 120, 215),
            ScrollBarTrackColor = Color.FromArgb(204, 204, 204),

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(0, 120, 215),
            StatusBarForeColor = Color.White,

            // **Tab Colors**
            TabBackColor = Color.White,
            TabForeColor =Color.FromArgb(255, 0, 0, 0),
            ActiveTabBackColor = Color.FromArgb(0, 120, 215),
            ActiveTabForeColor = Color.White,

            // **Dialog Box Colors**
            DialogBackColor = Color.White,
            DialogForeColor =Color.FromArgb(255, 0, 0, 0),
            DialogButtonBackColor = Color.FromArgb(0, 120, 215),
            DialogButtonForeColor = Color.White,

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(255, 255, 255),
            GradientEndColor = Color.FromArgb(245, 245, 245),
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(0, 120, 215), // Highlight Blue
            SideMenuHoverBackColor = Color.FromArgb(240, 240, 240),
            SideMenuSelectedBackColor = Color.FromArgb(0, 120, 215),
            SideMenuForeColor =Color.FromArgb(255, 0, 0, 0),
            SideMenuHoverForeColor =Color.FromArgb(255, 0, 0, 0),
            SideMenuSelectedForeColor = Color.White,
            SideMenuBorderColor = Color.FromArgb(224, 224, 224),
            SideMenuIconColor =Color.FromArgb(255, 0, 0, 0),
            SideMenuSelectedIconColor = Color.White,

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(0, 120, 215),
            TitleBarForeColor = Color.White,
            TitleBarHoverBackColor = Color.FromArgb(0, 99, 177),
            TitleBarHoverForeColor = Color.White,

            // **Dashboard Colors**
            DashboardBackColor = Color.White,
            DashboardCardBackColor = Color.FromArgb(245, 245, 245),
            DashboardCardHoverBackColor = Color.FromArgb(240, 240, 240),
            CardTitleForeColor = Color.FromArgb(0, 120, 215),
            CardTextForeColor = Color.FromArgb(50, 50, 50),

            // **Data Visualization (Charts)**
            ChartBackColor = Color.White,
            ChartLineColor = Color.FromArgb(0, 120, 215),
            ChartFillColor = Color.FromArgb(100, 0, 120, 215), // Semi-transparent Highlight Blue
            ChartAxisColor = Color.FromArgb(117, 117, 117),

            // **Sidebar and Menu Colors**
            SidebarIconColor =Color.FromArgb(255, 0, 0, 0),
            SidebarSelectedIconColor = Color.White,
            SidebarTextColor =Color.FromArgb(255, 0, 0, 0),
            SidebarSelectedTextColor = Color.White,

            // **Navigation Colors**
            NavigationBackColor = Color.White,
            NavigationForeColor =Color.FromArgb(255, 0, 0, 0),
            NavigationHoverBackColor = Color.FromArgb(240, 240, 240),
            NavigationHoverForeColor =Color.FromArgb(255, 0, 0, 0),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(232, 17, 35),         // Red
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(255, 255, 0),     // Yellow

            // **Font Properties**
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor =Color.FromArgb(255, 0, 0, 0),
            SecondaryTextColor = Color.FromArgb(50, 50, 50),
            AccentTextColor = Color.FromArgb(0, 120, 215),

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(0, 120, 215),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(0, 120, 215),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(0, 120, 215),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(0, 120, 215),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(0, 120, 215),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(0, 120, 215),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(50, 50, 50),
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 120, 215),
            },
            BlockquoteBorderColor = Color.FromArgb(0, 120, 215),
            BlockquoteBorderWidth = 2f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 120, 215),
            },
            InlineCodeBackgroundColor = Color.FromArgb(240, 240, 240),
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },
            CodeBlockBackgroundColor = Color.FromArgb(240, 240, 240),
            CodeBlockBorderColor = Color.FromArgb(204, 204, 204),
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(117, 117, 117),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(50, 50, 50),
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 120, 215),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 120, 215),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 120, 215),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 120, 215),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 120, 215),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 120, 215),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(0, 120, 215),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(0, 120, 215),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(0, 120, 215),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(50, 50, 50),
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(50, 50, 50),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(50, 50, 50),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(0, 120, 215),          // Highlight Blue
            SecondaryColor = Color.FromArgb(0, 153, 188),        // Cyan
            AccentColor = Color.FromArgb(232, 17, 35),           // Red

            SurfaceColor = Color.White,
            ErrorColor = Color.FromArgb(232, 17, 35),            // Red
            WarningColor = Color.FromArgb(255, 185, 0),          // Orange
            SuccessColor = Color.FromArgb(16, 124, 16),          // Green
            OnPrimaryColor = Color.White,
            OnBackgroundColor =Color.FromArgb(255, 0, 0, 0),

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 4,

            // **Imagery and Iconography**
            IconSet = "HighlightIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(100, 0, 0, 0), // Semi-transparent black
            ShadowOpacity = 0.2f,

            // **Animation and Transitions**
            AnimationDurationShort = 150,  // in milliseconds
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(0, 120, 215),

            // **Theme Variant**
            IsDarkTheme = false,
        };
        public static BeepTheme DarkTheme => new BeepTheme
        {
            GridBackColor = Color.FromArgb(255, 0, 0, 0),
            GridForeColor = Color.White,
            GridHeaderBackColor = Color.Gray,
            GridHeaderForeColor = Color.White,
            GridHeaderBorderColor = Color.DarkSlateGray,
            GridHeaderHoverBackColor = Color.DimGray,
            GridHeaderHoverForeColor = Color.White,
            GridHeaderSelectedBackColor = Color.FromArgb(50, 50, 50),
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.DimGray,
            GridHeaderSelectedBorderColor = Color.Silver,
            GridRowHoverBackColor = Color.DarkSlateGray,
            GridRowHoverForeColor = Color.White,
            GridRowSelectedBackColor = Color.Gray,
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.DarkSlateGray,
            GridRowSelectedBorderColor = Color.Gray,
            CardBackColor = Color.FromArgb(34, 34, 34),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 255),   // White

            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(200, 200, 200),   // Light Gray
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(232, 17, 35),       // Red
            MaxButtonColor = Color.FromArgb(28, 151, 234),        // Blue
            MinButtonColor = Color.FromArgb(28, 151, 234),
            TitleBarColor = Color.FromArgb(37, 37, 38),           // Very Dark Gray
            TitleBarTextColor = Color.FromArgb(255, 255, 255),    // White
            TitleBarIconColor = Color.FromArgb(255, 255, 255),
            TitleBarHoverColor = Color.FromArgb(51, 51, 51),      // Darker Gray
            TitleBarHoverTextColor = Color.FromArgb(255, 255, 255),
            TitleBarHoverIconColor = Color.FromArgb(255, 255, 255),
            TitleBarActiveColor = Color.FromArgb(51, 51, 51),
            TitleBarActiveTextColor = Color.FromArgb(255, 255, 255),
            TitleBarActiveIconColor = Color.FromArgb(255, 255, 255),
            TitleBarInactiveColor = Color.FromArgb(45, 45, 48),
            TitleBarInactiveTextColor = Color.FromArgb(153, 153, 153),
            TitleBarInactiveIconColor = Color.FromArgb(153, 153, 153),
            TitleBarBorderColor = Color.FromArgb(28, 28, 28),
            TitleBarBorderHoverColor = Color.FromArgb(51, 51, 51),
            TitleBarBorderActiveColor = Color.FromArgb(51, 51, 51),
            TitleBarBorderInactiveColor = Color.FromArgb(28, 28, 28),

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(170, 0, 0),     // Dark Red
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(232, 17, 35),  // Red
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(45, 45, 48),
            TitleBarCloseInactiveTextColor = Color.FromArgb(153, 153, 153),
            TitleBarCloseInactiveIconColor = Color.FromArgb(153, 153, 153),
            TitleBarCloseBorderColor = Color.FromArgb(28, 28, 28),
            TitleBarCloseBorderHoverColor = Color.FromArgb(51, 51, 51),
            TitleBarCloseBorderActiveColor = Color.FromArgb(51, 51, 51),
            TitleBarCloseBorderInactiveColor = Color.FromArgb(28, 28, 28),

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(51, 51, 51),
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.White,
            TitleBarMaxActiveColor = Color.FromArgb(28, 151, 234),    // Blue
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(45, 45, 48),
            TitleBarMaxInactiveTextColor = Color.FromArgb(153, 153, 153),
            TitleBarMaxInactiveIconColor = Color.FromArgb(153, 153, 153),
            TitleBarMaxBorderColor = Color.FromArgb(28, 28, 28),
            TitleBarMaxBorderHoverColor = Color.FromArgb(51, 51, 51),
            TitleBarMaxBorderActiveColor = Color.FromArgb(51, 51, 51),
            TitleBarMaxBorderInactiveColor = Color.FromArgb(28, 28, 28),

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(51, 51, 51),
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.White,
            TitleBarMinActiveColor = Color.FromArgb(28, 151, 234),
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(45, 45, 48),
            TitleBarMinInactiveTextColor = Color.FromArgb(153, 153, 153),
            TitleBarMinInactiveIconColor = Color.FromArgb(153, 153, 153),
            TitleBarMinBorderColor = Color.FromArgb(28, 28, 28),
            TitleBarMinBorderHoverColor = Color.FromArgb(51, 51, 51),
            TitleBarMinBorderActiveColor = Color.FromArgb(51, 51, 51),
            TitleBarMinBorderInactiveColor = Color.FromArgb(28, 28, 28),

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(51, 51, 51),
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(28, 151, 234),
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(45, 45, 48),
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(153, 153, 153),
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(153, 153, 153),
            TitleBarMinimizeBorderColor = Color.FromArgb(28, 28, 28),
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(51, 51, 51),
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(51, 51, 51),
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(28, 28, 28),

            // **General Colors**
            TitleForColor = Color.FromArgb(255, 255, 255),        // White
            TitleBarForColor = Color.FromArgb(255, 255, 255),
            DescriptionForColor = Color.FromArgb(200, 200, 200),  // Light Gray
            BeforeForColor = Color.FromArgb(28, 151, 234),        // Blue
            LatestForColor = Color.FromArgb(232, 17, 35),         // Red
            BackColor = Color.FromArgb(30, 30, 30),               // Dark Background

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(51, 51, 51),
            ButtonForeColor = Color.FromArgb(255, 255, 255),
            ButtonHoverBackColor = Color.FromArgb(28, 151, 234),  // Blue
            ButtonHoverForeColor = Color.White,
            ButtonActiveBackColor = Color.FromArgb(45, 45, 48),
            ButtonActiveForeColor = Color.White,

            // **TextBox Colors**
            TextBoxBackColor = Color.FromArgb(37, 37, 38),
            TextBoxForeColor = Color.FromArgb(255, 255, 255),


            // **Label Colors**
            LabelBackColor = Color.FromArgb(30, 30, 30),
            LabelForeColor = Color.FromArgb(255, 255, 255),

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(45, 45, 48), // Dark Gray
            


            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(45, 45, 48),
            HeaderForeColor = Color.FromArgb(255, 255, 255),
            GridLineColor = Color.FromArgb(51, 51, 51),
            RowBackColor = Color.FromArgb(37, 37, 38),
            RowForeColor = Color.FromArgb(255, 255, 255),
            AltRowBackColor = Color.FromArgb(30, 30, 30),
            SelectedRowBackColor = Color.FromArgb(28, 151, 234),
            SelectedRowForeColor = Color.White,

            // **ComboBox Colors**
            ComboBoxBackColor = Color.FromArgb(37, 37, 38),
            ComboBoxForeColor = Color.FromArgb(255, 255, 255),


            // **CheckBox Colors**
            CheckBoxBackColor = Color.FromArgb(30, 30, 30),
            CheckBoxForeColor = Color.FromArgb(255, 255, 255),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.FromArgb(30, 30, 30),
            RadioButtonForeColor = Color.FromArgb(255, 255, 255),

            // **Border Colors**
            BorderColor = Color.FromArgb(51, 51, 51),
            ActiveBorderColor = Color.FromArgb(28, 151, 234),
            InactiveBorderColor = Color.FromArgb(51, 51, 51),
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(28, 151, 234),          // Blue
            VisitedLinkColor = Color.FromArgb(0, 122, 204),
            HoverLinkColor = Color.FromArgb(0, 122, 204),
            LinkHoverColor = Color.FromArgb(0, 122, 204),
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(51, 51, 51),
            ToolTipForeColor = Color.FromArgb(255, 255, 255),

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(30, 30, 30),
            ScrollBarThumbColor = Color.FromArgb(51, 51, 51),
            ScrollBarTrackColor = Color.FromArgb(45, 45, 48),

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(37, 37, 38),
            StatusBarForeColor = Color.FromArgb(255, 255, 255),

            // **Tab Colors**
            TabBackColor = Color.FromArgb(30, 30, 30),
            TabForeColor = Color.FromArgb(255, 255, 255),
            ActiveTabBackColor = Color.FromArgb(45, 45, 48),
            ActiveTabForeColor = Color.FromArgb(255, 255, 255),

            // **Dialog Box Colors**
            DialogBackColor = Color.FromArgb(37, 37, 38),
            DialogForeColor = Color.FromArgb(255, 255, 255),
            DialogButtonBackColor = Color.FromArgb(51, 51, 51),
            DialogButtonForeColor = Color.FromArgb(255, 255, 255),

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(30, 30, 30),
            GradientEndColor = Color.FromArgb(45, 45, 48),
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(28, 28, 40), // Deep Blue
            SideMenuHoverBackColor = Color.FromArgb(51, 51, 51),
            SideMenuSelectedBackColor = Color.FromArgb(45, 45, 48),
            SideMenuForeColor = Color.FromArgb(255, 255, 255),
            SideMenuHoverForeColor = Color.FromArgb(255, 255, 255),
            SideMenuSelectedForeColor = Color.FromArgb(255, 255, 255),
            SideMenuBorderColor = Color.FromArgb(51, 51, 51),
            SideMenuIconColor = Color.FromArgb(255, 255, 255),
            SideMenuSelectedIconColor = Color.FromArgb(255, 255, 255),

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(37, 37, 38),
            TitleBarForeColor = Color.FromArgb(255, 255, 255),
            TitleBarHoverBackColor = Color.FromArgb(51, 51, 51),
            TitleBarHoverForeColor = Color.FromArgb(255, 255, 255),

            // **Dashboard Colors**
            DashboardBackColor = Color.FromArgb(30, 30, 30),
            DashboardCardBackColor = Color.FromArgb(37, 37, 38),
            DashboardCardHoverBackColor = Color.FromArgb(45, 45, 48),
            CardTitleForeColor = Color.FromArgb(255, 255, 255),
            CardTextForeColor = Color.FromArgb(200, 200, 200),

            // **Data Visualization (Charts)**
            ChartBackColor = Color.FromArgb(30, 30, 30),
            ChartLineColor = Color.FromArgb(28, 151, 234),
            ChartFillColor = Color.FromArgb(100, 28, 151, 234), // Semi-transparent Blue
            ChartAxisColor = Color.FromArgb(200, 200, 200),

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(255, 255, 255),
            SidebarSelectedIconColor = Color.FromArgb(255, 255, 255),
            SidebarTextColor = Color.FromArgb(255, 255, 255),
            SidebarSelectedTextColor = Color.FromArgb(255, 255, 255),

            // **Navigation Colors**
            NavigationBackColor = Color.FromArgb(30, 30, 30),
            NavigationForeColor = Color.FromArgb(255, 255, 255),
            NavigationHoverBackColor = Color.FromArgb(51, 51, 51),
            NavigationHoverForeColor = Color.FromArgb(255, 255, 255),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(232, 17, 35),      // Red
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(28, 151, 234), // Blue

            // **Font Properties**
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(255, 255, 255),
            SecondaryTextColor = Color.FromArgb(200, 200, 200),
            AccentTextColor = Color.FromArgb(28, 151, 234),

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(255, 255, 255),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(255, 255, 255),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(255, 255, 255),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 255, 255),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 255, 255),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 255, 255),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(200, 200, 200),
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(200, 200, 200),
            },
            BlockquoteBorderColor = Color.FromArgb(51, 51, 51),
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 255, 255),
            },
            InlineCodeBackgroundColor = Color.FromArgb(51, 51, 51),
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 255, 255),
            },
            CodeBlockBackgroundColor = Color.FromArgb(37, 37, 38),
            CodeBlockBorderColor = Color.FromArgb(51, 51, 51),
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(200, 200, 200),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(200, 200, 200),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(153, 153, 153),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 255),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(200, 200, 200),
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 255),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 255),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 255),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 255),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 255),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 255),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(255, 255, 255),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(255, 255, 255),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(255, 255, 255),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(200, 200, 200),
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(200, 200, 200),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(200, 200, 200),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 255),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 255),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor =Color.FromArgb(255, 0, 0, 0),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(28, 151, 234),       // Blue
            SecondaryColor = Color.FromArgb(51, 51, 51),       // Dark Gray
            AccentColor = Color.FromArgb(232, 17, 35),         // Red
            BackgroundColor = Color.FromArgb(30, 30, 30),      // Dark Background
            SurfaceColor = Color.FromArgb(37, 37, 38),
            ErrorColor = Color.FromArgb(232, 17, 35),          // Red
            WarningColor = Color.FromArgb(255, 185, 0),        // Orange
            SuccessColor = Color.FromArgb(16, 124, 16),        // Green
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.FromArgb(255, 255, 255),

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 4,

            // **Imagery and Iconography**
            IconSet = "DarkIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(100, 0, 0, 0), // Semi-transparent black
            ShadowOpacity = 0.3f,

            // **Animation and Transitions**
            AnimationDurationShort = 150,  // in milliseconds
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(28, 151, 234),

            // **Theme Variant**
            IsDarkTheme = true,
        };
        public static BeepTheme LightTheme => new BeepTheme
        {
            // **Grid Colors**
            GridBackColor = Color.WhiteSmoke,
            GridForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBackColor = Color.White,
            GridHeaderForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBorderColor = Color.Silver,
            GridHeaderHoverBackColor = Color.Gainsboro,
            GridHeaderHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderSelectedBackColor = Color.FromArgb(33, 150, 243), // Primary Blue
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.Gray,
            GridHeaderSelectedBorderColor = Color.FromArgb(33, 150, 243), // Primary Blue
            GridRowHoverBackColor = Color.LightGray,
            GridRowHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridRowSelectedBackColor = Color.FromArgb(33, 150, 243), // Primary Blue
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.Gray,
            GridRowSelectedBorderColor = Color.FromArgb(33, 150, 243), // Primary Blue

            // **Card Styles**
            CardBackColor = Color.FromArgb(248, 248, 255),
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(33, 33, 33),  // Dark Gray
            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(66, 66, 66),  // Medium Gray
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(229, 57, 53),      // Red
            MaxButtonColor = Color.FromArgb(76, 175, 80),        // Green
            MinButtonColor = Color.FromArgb(33, 150, 243),       // Blue
            TitleBarColor = Color.FromArgb(250, 250, 250),       // Near White
            TitleBarTextColor = Color.FromArgb(33, 33, 33),      // Dark Gray
            TitleBarIconColor = Color.FromArgb(33, 33, 33),
            TitleBarHoverColor = Color.FromArgb(240, 240, 240),  // Light Gray
            TitleBarHoverTextColor = Color.FromArgb(33, 33, 33),
            TitleBarHoverIconColor = Color.FromArgb(33, 33, 33),
            TitleBarActiveColor = Color.FromArgb(240, 240, 240),
            TitleBarActiveTextColor = Color.FromArgb(33, 33, 33),
            TitleBarActiveIconColor = Color.FromArgb(33, 33, 33),
            TitleBarInactiveColor = Color.FromArgb(245, 245, 245),
            TitleBarInactiveTextColor = Color.FromArgb(117, 117, 117),
            TitleBarInactiveIconColor = Color.FromArgb(117, 117, 117),
            TitleBarBorderColor = Color.FromArgb(224, 224, 224),
            TitleBarBorderHoverColor = Color.FromArgb(200, 200, 200),
            TitleBarBorderActiveColor = Color.FromArgb(200, 200, 200),
            TitleBarBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // **Close Button**
            TitleBarCloseHoverColor = Color.FromArgb(211, 47, 47),   // Darker Red
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(229, 57, 53),  // Red
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(245, 245, 245),
            TitleBarCloseInactiveTextColor = Color.FromArgb(117, 117, 117),
            TitleBarCloseInactiveIconColor = Color.FromArgb(117, 117, 117),
            TitleBarCloseBorderColor = Color.FromArgb(224, 224, 224),
            TitleBarCloseBorderHoverColor = Color.FromArgb(200, 200, 200),
            TitleBarCloseBorderActiveColor = Color.FromArgb(200, 200, 200),
            TitleBarCloseBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // **Maximize Button**
            TitleBarMaxHoverColor = Color.FromArgb(67, 160, 71),       // Darker Green
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.White,
            TitleBarMaxActiveColor = Color.FromArgb(76, 175, 80),      // Green
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(245, 245, 245),
            TitleBarMaxInactiveTextColor = Color.FromArgb(117, 117, 117),
            TitleBarMaxInactiveIconColor = Color.FromArgb(117, 117, 117),
            TitleBarMaxBorderColor = Color.FromArgb(224, 224, 224),
            TitleBarMaxBorderHoverColor = Color.FromArgb(200, 200, 200),
            TitleBarMaxBorderActiveColor = Color.FromArgb(200, 200, 200),
            TitleBarMaxBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // **Minimize Button**
            TitleBarMinHoverColor = Color.FromArgb(30, 136, 229), // Darker Blue
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.White,
            TitleBarMinActiveColor = Color.FromArgb(33, 150, 243), // Blue
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(245, 245, 245),
            TitleBarMinInactiveTextColor = Color.FromArgb(117, 117, 117),
            TitleBarMinInactiveIconColor = Color.FromArgb(117, 117, 117),
            TitleBarMinBorderColor = Color.FromArgb(224, 224, 224),
            TitleBarMinBorderHoverColor = Color.FromArgb(200, 200, 200),
            TitleBarMinBorderActiveColor = Color.FromArgb(200, 200, 200),
            TitleBarMinBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // **Minimize Button (Alternative Properties)**
            TitleBarMinimizeHoverColor = Color.FromArgb(30, 136, 229),
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(33, 150, 243),
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(245, 245, 245),
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(117, 117, 117),
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(117, 117, 117),
            TitleBarMinimizeBorderColor = Color.FromArgb(224, 224, 224),
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(200, 200, 200),
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(200, 200, 200),
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // **General Colors**
            TitleForColor = Color.FromArgb(33, 33, 33), // Fix: actual Dark Gray
            TitleBarForColor = Color.FromArgb(33, 33, 33),
            DescriptionForColor = Color.FromArgb(66, 66, 66), // Medium Gray
            BeforeForColor = Color.FromArgb(33, 150, 243),    // Blue
            LatestForColor = Color.FromArgb(76, 175, 80),     // Fix: actual Green
            BackColor = Color.White,                          // White

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(33, 150, 243), // Blue
            ButtonForeColor = Color.White,
            ButtonHoverBackColor = Color.FromArgb(30, 136, 229), // Darker Blue on hover
            ButtonHoverForeColor = Color.White,
            ButtonActiveBackColor = Color.FromArgb(25, 118, 210), // Even Darker Blue
            ButtonActiveForeColor = Color.White,

            // **TextBox Colors**
            TextBoxBackColor = Color.White,
            TextBoxForeColor = Color.FromArgb(33, 33, 33),
            BackgroundColor = Color.White,

            // **Label Colors**
            LabelBackColor = Color.White,
            LabelForeColor = Color.FromArgb(33, 33, 33),

            // **Panel Colors**
            PanelBackColor = Color.White,

            // **Grid Colors** (Secondary Grid Properties)
            HeaderBackColor = Color.FromArgb(245, 245, 245), // Light Gray
            HeaderForeColor = Color.FromArgb(33, 33, 33),    // Dark Gray
            GridLineColor = Color.FromArgb(224, 224, 224),   // Light Gray
            RowBackColor = Color.White,
            RowForeColor = Color.FromArgb(33, 33, 33),       // Dark Gray
            AltRowBackColor = Color.FromArgb(250, 250, 250), // Very Light Gray
            SelectedRowBackColor = Color.FromArgb(33, 150, 243), // Primary Blue
            SelectedRowForeColor = Color.White,

            // **ComboBox Colors**
            ComboBoxBackColor = Color.White,
            ComboBoxForeColor = Color.FromArgb(33, 33, 33),

            // **CheckBox Colors**
            CheckBoxBackColor = Color.White,
            CheckBoxForeColor = Color.FromArgb(33, 33, 33),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.White,
            RadioButtonForeColor = Color.FromArgb(33, 33, 33),

            // **Border Colors**
            BorderColor = Color.FromArgb(224, 224, 224),
            ActiveBorderColor = Color.FromArgb(33, 150, 243), // Blue
            InactiveBorderColor = Color.FromArgb(224, 224, 224),
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(33, 150, 243), // Blue
            VisitedLinkColor = Color.FromArgb(25, 118, 210), // Darker Blue
            HoverLinkColor = Color.FromArgb(30, 136, 229),   // Slightly Lighter Blue
            LinkHoverColor = Color.FromArgb(30, 136, 229),
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(66, 66, 66), // Medium Gray
            ToolTipForeColor = Color.White,

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.White,
            ScrollBarThumbColor = Color.FromArgb(189, 189, 189), // Gray
            ScrollBarTrackColor = Color.FromArgb(224, 224, 224), // Light Gray

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(245, 245, 245), // Light Gray
            StatusBarForeColor = Color.FromArgb(33, 33, 33),    // Dark Gray

            // **Tab Colors**
            TabBackColor = Color.White,
            TabForeColor = Color.FromArgb(33, 33, 33),
            ActiveTabBackColor = Color.FromArgb(245, 245, 245),
            ActiveTabForeColor = Color.FromArgb(33, 33, 33),

            // **Dialog Box Colors**
            DialogBackColor = Color.White,
            DialogForeColor = Color.FromArgb(33, 33, 33),
            DialogButtonBackColor = Color.FromArgb(33, 150, 243), // Blue
            DialogButtonForeColor = Color.White,

            // **Gradient Properties**
            GradientStartColor = Color.White,
            GradientEndColor = Color.FromArgb(245, 245, 245), // Light Gray
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors** (Fixed for consistency with a Light Theme)
            SideMenuBackColor = Color.White,                     // White background
            SideMenuHoverBackColor = Color.FromArgb(245, 245, 245), // Slight contrast on hover
            SideMenuSelectedBackColor = Color.FromArgb(33, 150, 243), // Primary Blue
            SideMenuForeColor = Color.FromArgb(33, 33, 33),
            SideMenuHoverForeColor = Color.FromArgb(33, 33, 33),
            SideMenuSelectedForeColor = Color.White,
            SideMenuBorderColor = Color.FromArgb(224, 224, 224),
            SideMenuIconColor = Color.FromArgb(33, 33, 33),
            SideMenuSelectedIconColor = Color.White,

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(33, 150, 243), // Blue
            TitleBarForeColor = Color.White,
            TitleBarHoverBackColor = Color.FromArgb(240, 240, 240),
            TitleBarHoverForeColor = Color.FromArgb(33, 33, 33),

            // **Dashboard Colors**
            DashboardBackColor = Color.White,
            DashboardCardBackColor = Color.FromArgb(245, 245, 245),
            DashboardCardHoverBackColor = Color.FromArgb(240, 240, 240),
            CardTitleForeColor = Color.FromArgb(33, 33, 33),
            CardTextForeColor = Color.FromArgb(66, 66, 66),

            // **Data Visualization (Charts)**
            ChartBackColor = Color.White,
            ChartLineColor = Color.FromArgb(33, 150, 243),       // Blue
            ChartFillColor = Color.FromArgb(100, 33, 150, 243),  // Semi-transparent Blue
            ChartAxisColor = Color.FromArgb(117, 117, 117),      // Gray

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(33, 33, 33),
            SidebarSelectedIconColor = Color.White,
            SidebarTextColor = Color.FromArgb(33, 33, 33),
            SidebarSelectedTextColor = Color.White,

            // **Navigation Colors**
            NavigationBackColor = Color.White,
            NavigationForeColor = Color.FromArgb(33, 33, 33),
            NavigationHoverBackColor = Color.FromArgb(245, 245, 245),
            NavigationHoverForeColor = Color.FromArgb(33, 33, 33),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(229, 57, 53), // Red
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(33, 150, 243), // Primary Blue highlight

            // **Font Properties**
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(33, 33, 33),
            SecondaryTextColor = Color.FromArgb(66, 66, 66),
            AccentTextColor = Color.FromArgb(33, 150, 243), // Primary Blue

            // **Theme Variant**
            IsDarkTheme = false,

            // **Animation and Transitions**
            AnimationDurationShort = 150,  // in milliseconds
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(33, 150, 243),
        };
        public static BeepTheme PastelTheme => new BeepTheme
        {
            GridBackColor = Color.MistyRose,
            GridForeColor = Color.DarkSlateGray,
            GridHeaderBackColor = Color.LavenderBlush,
            GridHeaderForeColor = Color.DarkSlateGray,
            GridHeaderBorderColor = Color.RosyBrown,
            GridHeaderHoverBackColor = Color.LightPink,
            GridHeaderHoverForeColor = Color.DarkSlateGray,
            GridHeaderSelectedBackColor = Color.Salmon,
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.RosyBrown,
            GridHeaderSelectedBorderColor = Color.DarkSalmon,
            GridRowHoverBackColor = Color.LightPink,
            GridRowHoverForeColor = Color.DarkSlateGray,
            GridRowSelectedBackColor = Color.Salmon,
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.RosyBrown,
            GridRowSelectedBorderColor = Color.DarkSalmon,
            CardBackColor = Color.FromArgb(255, 223, 211),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),     // Dark Gray

            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(102, 102, 102),  // Medium Gray
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(255, 179, 186),    // Pastel Pink
            MaxButtonColor = Color.FromArgb(186, 225, 255),      // Pastel Blue
            MinButtonColor = Color.FromArgb(186, 225, 255),
            TitleBarColor = Color.FromArgb(255, 250, 240),       // Floral White
            TitleBarTextColor = Color.FromArgb(85, 85, 85),      // Dark Gray
            TitleBarIconColor = Color.FromArgb(85, 85, 85),
            TitleBarHoverColor = Color.FromArgb(255, 240, 245),  // Lavender Blush
            TitleBarHoverTextColor = Color.FromArgb(85, 85, 85),
            TitleBarHoverIconColor = Color.FromArgb(85, 85, 85),
            TitleBarActiveColor = Color.FromArgb(255, 240, 245),
            TitleBarActiveTextColor = Color.FromArgb(85, 85, 85),
            TitleBarActiveIconColor = Color.FromArgb(85, 85, 85),
            TitleBarInactiveColor = Color.FromArgb(255, 245, 238),
            TitleBarInactiveTextColor = Color.FromArgb(136, 136, 136),
            TitleBarInactiveIconColor = Color.FromArgb(136, 136, 136),
            TitleBarBorderColor = Color.FromArgb(224, 224, 224),
            TitleBarBorderHoverColor = Color.FromArgb(200, 200, 200),
            TitleBarBorderActiveColor = Color.FromArgb(200, 200, 200),
            TitleBarBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(255, 204, 204),  // Light Pastel Red
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(255, 179, 186), // Pastel Pink
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(255, 245, 238),
            TitleBarCloseInactiveTextColor = Color.FromArgb(136, 136, 136),
            TitleBarCloseInactiveIconColor = Color.FromArgb(136, 136, 136),
            TitleBarCloseBorderColor = Color.FromArgb(224, 224, 224),
            TitleBarCloseBorderHoverColor = Color.FromArgb(200, 200, 200),
            TitleBarCloseBorderActiveColor = Color.FromArgb(200, 200, 200),
            TitleBarCloseBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(204, 229, 255),    // Light Pastel Blue
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.White,
            TitleBarMaxActiveColor = Color.FromArgb(186, 225, 255),   // Pastel Blue
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(255, 245, 238),
            TitleBarMaxInactiveTextColor = Color.FromArgb(136, 136, 136),
            TitleBarMaxInactiveIconColor = Color.FromArgb(136, 136, 136),
            TitleBarMaxBorderColor = Color.FromArgb(224, 224, 224),
            TitleBarMaxBorderHoverColor = Color.FromArgb(200, 200, 200),
            TitleBarMaxBorderActiveColor = Color.FromArgb(200, 200, 200),
            TitleBarMaxBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(204, 229, 255),    // Light Pastel Blue
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.White,
            TitleBarMinActiveColor = Color.FromArgb(186, 225, 255),   // Pastel Blue
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(255, 245, 238),
            TitleBarMinInactiveTextColor = Color.FromArgb(136, 136, 136),
            TitleBarMinInactiveIconColor = Color.FromArgb(136, 136, 136),
            TitleBarMinBorderColor = Color.FromArgb(224, 224, 224),
            TitleBarMinBorderHoverColor = Color.FromArgb(200, 200, 200),
            TitleBarMinBorderActiveColor = Color.FromArgb(200, 200, 200),
            TitleBarMinBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(204, 229, 255),
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(186, 225, 255),
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(255, 245, 238),
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(136, 136, 136),
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(136, 136, 136),
            TitleBarMinimizeBorderColor = Color.FromArgb(224, 224, 224),
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(200, 200, 200),
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(200, 200, 200),
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(224, 224, 224),

            // **General Colors**
            TitleForColor = Color.FromArgb(85, 85, 85),            // Dark Gray
            TitleBarForColor = Color.FromArgb(85, 85, 85),
            DescriptionForColor = Color.FromArgb(102, 102, 102),   // Medium Gray
            BeforeForColor = Color.FromArgb(255, 223, 186),        // Pastel Orange
            LatestForColor = Color.FromArgb(186, 225, 255),        // Pastel Blue
            BackColor = Color.FromArgb(255, 250, 240),             // Floral White

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(186, 225, 255),       // Pastel Blue
            ButtonForeColor = Color.FromArgb(85, 85, 85),
            ButtonHoverBackColor = Color.FromArgb(204, 229, 255),  // Light Pastel Blue
            ButtonHoverForeColor = Color.FromArgb(85, 85, 85),
            ButtonActiveBackColor = Color.FromArgb(255, 179, 186), // Pastel Pink
            ButtonActiveForeColor = Color.FromArgb(85, 85, 85),

            // **TextBox Colors**
            TextBoxBackColor = Color.White,
            TextBoxForeColor = Color.FromArgb(85, 85, 85),


            // **Label Colors**
            LabelBackColor = Color.White,
            LabelForeColor = Color.FromArgb(85, 85, 85),

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(255, 240, 245),        // Lavender Blush
            


            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(255, 240, 245),       // Lavender Blush
            HeaderForeColor = Color.FromArgb(85, 85, 85),
            GridLineColor = Color.FromArgb(224, 224, 224),
            RowBackColor = Color.White,
            RowForeColor = Color.FromArgb(85, 85, 85),
            AltRowBackColor = Color.FromArgb(255, 245, 238),
            SelectedRowBackColor = Color.FromArgb(204, 229, 255),
            SelectedRowForeColor = Color.FromArgb(85, 85, 85),

            // **ComboBox Colors**
            ComboBoxBackColor = Color.White,
            ComboBoxForeColor = Color.FromArgb(85, 85, 85),


            // **CheckBox Colors**
            CheckBoxBackColor = Color.White,
            CheckBoxForeColor = Color.FromArgb(85, 85, 85),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.White,
            RadioButtonForeColor = Color.FromArgb(85, 85, 85),

            // **Border Colors**
            BorderColor = Color.FromArgb(224, 224, 224),
            ActiveBorderColor = Color.FromArgb(186, 225, 255),
            InactiveBorderColor = Color.FromArgb(224, 224, 224),
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(186, 225, 255),            // Pastel Blue
            VisitedLinkColor = Color.FromArgb(255, 204, 204),     // Light Pastel Red
            HoverLinkColor = Color.FromArgb(204, 229, 255),
            LinkHoverColor = Color.FromArgb(204, 229, 255),
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(102, 102, 102),
            ToolTipForeColor = Color.White,

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.White,
            ScrollBarThumbColor = Color.FromArgb(224, 224, 224),
            ScrollBarTrackColor = Color.FromArgb(245, 245, 245),

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(255, 245, 238),
            StatusBarForeColor = Color.FromArgb(85, 85, 85),

            // **Tab Colors**
            TabBackColor = Color.White,
            TabForeColor = Color.FromArgb(85, 85, 85),
            ActiveTabBackColor = Color.FromArgb(255, 240, 245),
            ActiveTabForeColor = Color.FromArgb(85, 85, 85),

            // **Dialog Box Colors**
            DialogBackColor = Color.White,
            DialogForeColor = Color.FromArgb(85, 85, 85),
            DialogButtonBackColor = Color.FromArgb(186, 225, 255),
            DialogButtonForeColor = Color.FromArgb(85, 85, 85),

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(255, 250, 240),
            GradientEndColor = Color.FromArgb(255, 240, 245),
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(255, 245, 238),     // Peach Puff
            SideMenuHoverBackColor = Color.FromArgb(255, 245, 238),
            SideMenuSelectedBackColor = Color.FromArgb(186, 225, 255),
            SideMenuForeColor = Color.FromArgb(85, 85, 85),
            SideMenuHoverForeColor = Color.FromArgb(85, 85, 85),
            SideMenuSelectedForeColor = Color.FromArgb(85, 85, 85),
            SideMenuBorderColor = Color.FromArgb(224, 224, 224),
            SideMenuIconColor = Color.FromArgb(85, 85, 85),
            SideMenuSelectedIconColor = Color.FromArgb(85, 85, 85),

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(255, 250, 240),
            TitleBarForeColor = Color.FromArgb(85, 85, 85),
            TitleBarHoverBackColor = Color.FromArgb(255, 240, 245),
            TitleBarHoverForeColor = Color.FromArgb(85, 85, 85),

            // **Dashboard Colors**
            DashboardBackColor = Color.FromArgb(255, 250, 240),
            DashboardCardBackColor = Color.White,
            DashboardCardHoverBackColor = Color.FromArgb(255, 245, 238),
            CardTitleForeColor = Color.FromArgb(85, 85, 85),
            CardTextForeColor = Color.FromArgb(102, 102, 102),

            // **Data Visualization (Charts)**
            ChartBackColor = Color.White,
            ChartLineColor = Color.FromArgb(186, 225, 255),
            ChartFillColor = Color.FromArgb(100, 186, 225, 255), // Semi-transparent Pastel Blue
            ChartAxisColor = Color.FromArgb(136, 136, 136),

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(85, 85, 85),
            SidebarSelectedIconColor = Color.FromArgb(85, 85, 85),
            SidebarTextColor = Color.FromArgb(85, 85, 85),
            SidebarSelectedTextColor = Color.FromArgb(85, 85, 85),

            // **Navigation Colors**
            NavigationBackColor = Color.White,
            NavigationForeColor = Color.FromArgb(85, 85, 85),
            NavigationHoverBackColor = Color.FromArgb(255, 245, 238),
            NavigationHoverForeColor = Color.FromArgb(85, 85, 85),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(255, 223, 186),     // Pastel Orange
            BadgeForeColor = Color.FromArgb(85, 85, 85),
            HighlightBackColor = Color.FromArgb(255, 255, 204), // Light Pastel Yellow

            // **Font Properties**
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(85, 85, 85),
            SecondaryTextColor = Color.FromArgb(102, 102, 102),
            AccentTextColor = Color.FromArgb(186, 225, 255),

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(102, 102, 102),
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(102, 102, 102),
            },
            BlockquoteBorderColor = Color.FromArgb(189, 189, 189),
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            InlineCodeBackgroundColor = Color.FromArgb(245, 245, 245),
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            CodeBlockBackgroundColor = Color.FromArgb(245, 245, 245),
            CodeBlockBorderColor = Color.FromArgb(224, 224, 224),
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(136, 136, 136),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(102, 102, 102),
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(102, 102, 102),
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(102, 102, 102),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(102, 102, 102),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(85, 85, 85),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(85, 85, 85),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(186, 225, 255),         // Pastel Blue
            SecondaryColor = Color.FromArgb(255, 179, 186),       // Pastel Pink
            AccentColor = Color.FromArgb(255, 223, 186),          // Pastel Orange
            BackgroundColor = Color.FromArgb(255, 250, 240),      // Floral White
            SurfaceColor = Color.White,
            ErrorColor = Color.FromArgb(255, 179, 186),           // Pastel Pink
            WarningColor = Color.FromArgb(255, 223, 186),         // Pastel Orange
            SuccessColor = Color.FromArgb(186, 225, 255),         // Pastel Blue
            OnPrimaryColor = Color.FromArgb(85, 85, 85),
            OnBackgroundColor = Color.FromArgb(85, 85, 85),

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 8,

            // **Imagery and Iconography**
            IconSet = "PastelIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(50, 0, 0, 0), // Semi-transparent black
            ShadowOpacity = 0.1f,

            // **Animation and Transitions**
            AnimationDurationShort = 150,  // in milliseconds
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(186, 225, 255),

            // **Theme Variant**
            IsDarkTheme = false,
        };
        public static BeepTheme MidnightTheme => new BeepTheme
        {
            GridBackColor = Color.FromArgb(18, 18, 18), // Midnight Black
            GridForeColor = Color.WhiteSmoke,
            GridHeaderBackColor = Color.FromArgb(34, 34, 34),
            GridHeaderForeColor = Color.White,
            GridHeaderBorderColor = Color.Gray,
            GridHeaderHoverBackColor = Color.FromArgb(50, 50, 50),
            GridHeaderHoverForeColor = Color.White,
            GridHeaderSelectedBackColor = Color.FromArgb(0, 120, 215),
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.Gray,
            GridHeaderSelectedBorderColor = Color.Silver,
            GridRowHoverBackColor = Color.FromArgb(34, 34, 34),
            GridRowHoverForeColor = Color.White,
            GridRowSelectedBackColor = Color.FromArgb(0, 120, 215),
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.Gray,
            GridRowSelectedBorderColor = Color.Silver,
            CardBackColor = Color.FromArgb(25, 25, 112),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(230, 230, 250),   // Lavender

            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(211, 211, 211),   // Light Gray
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(178, 34, 34),       // Firebrick
            MaxButtonColor = Color.FromArgb(72, 61, 139),         // Dark Slate Blue
            MinButtonColor = Color.FromArgb(65, 105, 225),        // Royal Blue
            TitleBarColor = Color.FromArgb(15, 15, 30),           // Very Dark Blue
            TitleBarTextColor = Color.FromArgb(230, 230, 250),    // Lavender
            TitleBarIconColor = Color.FromArgb(230, 230, 250),
            TitleBarHoverColor = Color.FromArgb(25, 25, 50),      // Darker Blue
            TitleBarHoverTextColor = Color.FromArgb(230, 230, 250),
            TitleBarHoverIconColor = Color.FromArgb(230, 230, 250),
            TitleBarActiveColor = Color.FromArgb(25, 25, 50),
            TitleBarActiveTextColor = Color.FromArgb(230, 230, 250),
            TitleBarActiveIconColor = Color.FromArgb(230, 230, 250),
            TitleBarInactiveColor = Color.FromArgb(20, 20, 40),
            TitleBarInactiveTextColor = Color.FromArgb(169, 169, 169),
            TitleBarInactiveIconColor = Color.FromArgb(169, 169, 169),
            TitleBarBorderColor = Color.FromArgb(25, 25, 50),
            TitleBarBorderHoverColor = Color.FromArgb(30, 30, 60),
            TitleBarBorderActiveColor = Color.FromArgb(30, 30, 60),
            TitleBarBorderInactiveColor = Color.FromArgb(25, 25, 50),

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(139, 0, 0),     // Dark Red
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(178, 34, 34),  // Firebrick
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(20, 20, 40),
            TitleBarCloseInactiveTextColor = Color.FromArgb(169, 169, 169),
            TitleBarCloseInactiveIconColor = Color.FromArgb(169, 169, 169),
            TitleBarCloseBorderColor = Color.FromArgb(25, 25, 50),
            TitleBarCloseBorderHoverColor = Color.FromArgb(30, 30, 60),
            TitleBarCloseBorderActiveColor = Color.FromArgb(30, 30, 60),
            TitleBarCloseBorderInactiveColor = Color.FromArgb(25, 25, 50),

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(72, 61, 139),
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.White,
            TitleBarMaxActiveColor = Color.FromArgb(72, 61, 139),
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(20, 20, 40),
            TitleBarMaxInactiveTextColor = Color.FromArgb(169, 169, 169),
            TitleBarMaxInactiveIconColor = Color.FromArgb(169, 169, 169),
            TitleBarMaxBorderColor = Color.FromArgb(25, 25, 50),
            TitleBarMaxBorderHoverColor = Color.FromArgb(30, 30, 60),
            TitleBarMaxBorderActiveColor = Color.FromArgb(30, 30, 60),
            TitleBarMaxBorderInactiveColor = Color.FromArgb(25, 25, 50),

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(65, 105, 225),
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.White,
            TitleBarMinActiveColor = Color.FromArgb(65, 105, 225),
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(20, 20, 40),
            TitleBarMinInactiveTextColor = Color.FromArgb(169, 169, 169),
            TitleBarMinInactiveIconColor = Color.FromArgb(169, 169, 169),
            TitleBarMinBorderColor = Color.FromArgb(25, 25, 50),
            TitleBarMinBorderHoverColor = Color.FromArgb(30, 30, 60),
            TitleBarMinBorderActiveColor = Color.FromArgb(30, 30, 60),
            TitleBarMinBorderInactiveColor = Color.FromArgb(25, 25, 50),

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(65, 105, 225),
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(65, 105, 225),
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(20, 20, 40),
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(169, 169, 169),
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(169, 169, 169),
            TitleBarMinimizeBorderColor = Color.FromArgb(25, 25, 50),
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(30, 30, 60),
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(30, 30, 60),
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(25, 25, 50),

            // **General Colors**
            TitleForColor = Color.FromArgb(230, 230, 250),        // Lavender
            TitleBarForColor = Color.FromArgb(230, 230, 250),
            DescriptionForColor = Color.FromArgb(211, 211, 211),  // Light Gray
            BeforeForColor = Color.FromArgb(65, 105, 225),        // Royal Blue
            LatestForColor = Color.FromArgb(72, 61, 139),         // Dark Slate Blue
            BackColor = Color.FromArgb(15, 15, 30),               // Very Dark Blue

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(25, 25, 50),
            ButtonForeColor = Color.FromArgb(230, 230, 250),
            ButtonHoverBackColor = Color.FromArgb(72, 61, 139),
            ButtonHoverForeColor = Color.White,
            ButtonActiveBackColor = Color.FromArgb(65, 105, 225),
            ButtonActiveForeColor = Color.White,

            // **TextBox Colors**
            TextBoxBackColor = Color.FromArgb(25, 25, 50),
            TextBoxForeColor = Color.FromArgb(230, 230, 250),


            // **Label Colors**
            LabelBackColor = Color.FromArgb(15, 15, 30),
            LabelForeColor = Color.FromArgb(230, 230, 250),

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(25, 25, 50), // Darker blue-gray




            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(25, 25, 50),
            HeaderForeColor = Color.FromArgb(230, 230, 250),
            GridLineColor = Color.FromArgb(45, 45, 80),
            RowBackColor = Color.FromArgb(20, 20, 40),
            AltRowBackColor = Color.FromArgb(15, 15, 35),       // Very Dark Navy
            RowForeColor = Color.FromArgb(211, 211, 211),       // Light Gray
            SelectedRowBackColor = Color.FromArgb(65, 105, 225),
            SelectedRowForeColor = Color.White,

            // **ComboBox Colors**
            ComboBoxBackColor = Color.FromArgb(25, 25, 50),
            ComboBoxForeColor = Color.FromArgb(230, 230, 250),


            // **CheckBox Colors**
            CheckBoxBackColor = Color.FromArgb(15, 15, 30),
            CheckBoxForeColor = Color.FromArgb(230, 230, 250),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.FromArgb(15, 15, 30),
            RadioButtonForeColor = Color.FromArgb(230, 230, 250),

            // **Border Colors**
            BorderColor = Color.FromArgb(45, 45, 80),
            ActiveBorderColor = Color.FromArgb(65, 105, 225),
            InactiveBorderColor = Color.FromArgb(45, 45, 80),
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(65, 105, 225),
            VisitedLinkColor = Color.FromArgb(72, 61, 139),
            HoverLinkColor = Color.FromArgb(100, 149, 237),
            LinkHoverColor = Color.FromArgb(100, 149, 237),
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(25, 25, 50),
            ToolTipForeColor = Color.FromArgb(230, 230, 250),

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(15, 15, 30),
            ScrollBarThumbColor = Color.FromArgb(45, 45, 80),
            ScrollBarTrackColor = Color.FromArgb(25, 25, 50),

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(25, 25, 50),
            StatusBarForeColor = Color.FromArgb(230, 230, 250),

            // **Tab Colors**
            TabBackColor = Color.FromArgb(15, 15, 30),
            TabForeColor = Color.FromArgb(230, 230, 250),
            ActiveTabBackColor = Color.FromArgb(25, 25, 50),
            ActiveTabForeColor = Color.FromArgb(230, 230, 250),

            // **Dialog Box Colors**
            DialogBackColor = Color.FromArgb(20, 20, 40),
            DialogForeColor = Color.FromArgb(230, 230, 250),
            DialogButtonBackColor = Color.FromArgb(25, 25, 50),
            DialogButtonForeColor = Color.FromArgb(230, 230, 250),

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(15, 15, 30),
            GradientEndColor = Color.FromArgb(25, 25, 50),
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(20, 20, 40), // Deep blue-gray

            SideMenuHoverBackColor = Color.FromArgb(25, 25, 50),
            SideMenuSelectedBackColor = Color.FromArgb(25, 25, 50),
            SideMenuForeColor = Color.FromArgb(230, 230, 250),
            SideMenuHoverForeColor = Color.FromArgb(230, 230, 250),
            SideMenuSelectedForeColor = Color.FromArgb(230, 230, 250),
            SideMenuBorderColor = Color.FromArgb(45, 45, 80),
            SideMenuIconColor = Color.FromArgb(230, 230, 250),
            SideMenuSelectedIconColor = Color.FromArgb(230, 230, 250),

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(15, 15, 30),
            TitleBarForeColor = Color.FromArgb(230, 230, 250),
            TitleBarHoverBackColor = Color.FromArgb(25, 25, 50),
            TitleBarHoverForeColor = Color.FromArgb(230, 230, 250),

            // **Dashboard Colors**
            DashboardBackColor = Color.FromArgb(15, 15, 30),
            DashboardCardBackColor = Color.FromArgb(20, 20, 40),
            DashboardCardHoverBackColor = Color.FromArgb(25, 25, 50),
            CardTitleForeColor = Color.FromArgb(230, 230, 250),
            CardTextForeColor = Color.FromArgb(211, 211, 211),

            // **Data Visualization (Charts)**
            ChartBackColor = Color.FromArgb(15, 15, 30),
            ChartLineColor = Color.FromArgb(65, 105, 225),
            ChartFillColor = Color.FromArgb(100, 65, 105, 225), // Semi-transparent Royal Blue
            ChartAxisColor = Color.FromArgb(211, 211, 211),

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(230, 230, 250),
            SidebarSelectedIconColor = Color.FromArgb(230, 230, 250),
            SidebarTextColor = Color.FromArgb(230, 230, 250),
            SidebarSelectedTextColor = Color.FromArgb(230, 230, 250),

            // **Navigation Colors**
            NavigationBackColor = Color.FromArgb(15, 15, 30),
            NavigationForeColor = Color.FromArgb(230, 230, 250),
            NavigationHoverBackColor = Color.FromArgb(25, 25, 50),
            NavigationHoverForeColor = Color.FromArgb(230, 230, 250),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(178, 34, 34),      // Firebrick
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(65, 105, 225), // Royal Blue

            // **Font Properties**
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(230, 230, 250),
            SecondaryTextColor = Color.FromArgb(211, 211, 211),
            AccentTextColor = Color.FromArgb(65, 105, 225),

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(230, 230, 250),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(211, 211, 211),
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(211, 211, 211),
            },
            BlockquoteBorderColor = Color.FromArgb(45, 45, 80),
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            InlineCodeBackgroundColor = Color.FromArgb(25, 25, 50),
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            CodeBlockBackgroundColor = Color.FromArgb(20, 20, 40),
            CodeBlockBorderColor = Color.FromArgb(45, 45, 80),
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(211, 211, 211),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(211, 211, 211),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(169, 169, 169),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(211, 211, 211),
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(230, 230, 250),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(211, 211, 211),
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(211, 211, 211),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(211, 211, 211),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(230, 230, 250),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(65, 105, 225),          // Royal Blue
            SecondaryColor = Color.FromArgb(72, 61, 139),         // Dark Slate Blue
            AccentColor = Color.FromArgb(178, 34, 34),            // Firebrick
            BackgroundColor = Color.FromArgb(15, 15, 30),         // Very Dark Blue
            SurfaceColor = Color.FromArgb(20, 20, 40),
            ErrorColor = Color.FromArgb(178, 34, 34),             // Firebrick
            WarningColor = Color.FromArgb(255, 140, 0),           // Dark Orange
            SuccessColor = Color.FromArgb(34, 139, 34),           // Forest Green
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.FromArgb(230, 230, 250),

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 6,

            // **Imagery and Iconography**
            IconSet = "MidnightIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(100, 0, 0, 0), // Semi-transparent black
            ShadowOpacity = 0.3f,

            // **Animation and Transitions**
            AnimationDurationShort = 150,  // in milliseconds
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(65, 105, 225),

            // **Theme Variant**
            IsDarkTheme = true,
        };
        public static BeepTheme NeonTheme => new BeepTheme
        {
            GridBackColor = Color.FromArgb(255, 0, 0, 0),
            GridForeColor = Color.Lime,
            GridHeaderBackColor = Color.Fuchsia,
            GridHeaderForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBorderColor = Color.Lime,
            GridHeaderHoverBackColor = Color.Cyan,
            GridHeaderHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderSelectedBackColor = Color.Magenta,
            GridHeaderSelectedForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderHoverBorderColor = Color.Lime,
            GridHeaderSelectedBorderColor = Color.Lime,
            GridRowHoverBackColor = Color.Cyan,
            GridRowHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridRowSelectedBackColor = Color.Magenta,
            GridRowSelectedForeColor = Color.FromArgb(255, 0, 0, 0),
            GridRowHoverBorderColor = Color.Lime,
            GridRowSelectedBorderColor = Color.Lime,
            CardBackColor = Color.FromArgb(255, 0, 255),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 255, 255),    // Neon Cyan

            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(128, 255, 0),    // Neon Green
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(255, 0, 0),       // Neon Red
            MaxButtonColor = Color.FromArgb(255, 255, 0),       // Neon Yellow
            MinButtonColor = Color.FromArgb(0, 255, 255),       // Neon Cyan
            TitleBarColor =Color.FromArgb(255, 0, 0, 0),            // Black
            TitleBarTextColor = Color.FromArgb(255, 0, 255),    // Neon Magenta
            TitleBarIconColor = Color.FromArgb(255, 0, 255),
            TitleBarHoverColor = Color.FromArgb(30, 30, 30),    // Dark Gray
            TitleBarHoverTextColor = Color.FromArgb(255, 0, 255),
            TitleBarHoverIconColor = Color.FromArgb(255, 0, 255),
            TitleBarActiveColor = Color.FromArgb(30, 30, 30),
            TitleBarActiveTextColor = Color.FromArgb(255, 0, 255),
            TitleBarActiveIconColor = Color.FromArgb(255, 0, 255),
            TitleBarInactiveColor = Color.FromArgb(15, 15, 15),
            TitleBarInactiveTextColor = Color.FromArgb(128, 0, 128),
            TitleBarInactiveIconColor = Color.FromArgb(128, 0, 128),
            TitleBarBorderColor = Color.FromArgb(0, 255, 255),   // Neon Cyan
            TitleBarBorderHoverColor = Color.FromArgb(0, 255, 255),
            TitleBarBorderActiveColor = Color.FromArgb(0, 255, 255),
            TitleBarBorderInactiveColor = Color.FromArgb(128, 128, 128),

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(255, 20, 147),     // Neon Pink
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(255, 0, 0),       // Neon Red
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(15, 15, 15),
            TitleBarCloseInactiveTextColor = Color.FromArgb(128, 0, 0),
            TitleBarCloseInactiveIconColor = Color.FromArgb(128, 0, 0),
            TitleBarCloseBorderColor = Color.FromArgb(255, 0, 0),
            TitleBarCloseBorderHoverColor = Color.FromArgb(255, 20, 147),
            TitleBarCloseBorderActiveColor = Color.FromArgb(255, 0, 0),
            TitleBarCloseBorderInactiveColor = Color.FromArgb(128, 0, 0),

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(173, 255, 47),       // Neon Greenish
            TitleBarMaxHoverTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMaxHoverIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMaxActiveColor = Color.FromArgb(255, 255, 0),       // Neon Yellow
            TitleBarMaxActiveTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMaxActiveIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMaxInactiveColor = Color.FromArgb(15, 15, 15),
            TitleBarMaxInactiveTextColor = Color.FromArgb(128, 128, 0),
            TitleBarMaxInactiveIconColor = Color.FromArgb(128, 128, 0),
            TitleBarMaxBorderColor = Color.FromArgb(255, 255, 0),
            TitleBarMaxBorderHoverColor = Color.FromArgb(173, 255, 47),
            TitleBarMaxBorderActiveColor = Color.FromArgb(255, 255, 0),
            TitleBarMaxBorderInactiveColor = Color.FromArgb(128, 128, 0),

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(0, 255, 127),        // Neon Green
            TitleBarMinHoverTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinHoverIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinActiveColor = Color.FromArgb(0, 255, 255),       // Neon Cyan
            TitleBarMinActiveTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinActiveIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinInactiveColor = Color.FromArgb(15, 15, 15),
            TitleBarMinInactiveTextColor = Color.FromArgb(0, 128, 128),
            TitleBarMinInactiveIconColor = Color.FromArgb(0, 128, 128),
            TitleBarMinBorderColor = Color.FromArgb(0, 255, 255),
            TitleBarMinBorderHoverColor = Color.FromArgb(0, 255, 127),
            TitleBarMinBorderActiveColor = Color.FromArgb(0, 255, 255),
            TitleBarMinBorderInactiveColor = Color.FromArgb(0, 128, 128),

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(0, 255, 127),
            TitleBarMinimizeHoverTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinimizeHoverIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinimizeActiveColor = Color.FromArgb(0, 255, 255),
            TitleBarMinimizeActiveTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinimizeActiveIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinimizeInactiveColor = Color.FromArgb(15, 15, 15),
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(0, 128, 128),
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(0, 128, 128),
            TitleBarMinimizeBorderColor = Color.FromArgb(0, 255, 255),
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(0, 255, 127),
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(0, 255, 255),
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(0, 128, 128),

            // **General Colors**
            TitleForColor = Color.FromArgb(255, 0, 255),         // Neon Magenta
            TitleBarForColor = Color.FromArgb(255, 0, 255),
            DescriptionForColor = Color.FromArgb(128, 255, 0),   // Neon Green
            BeforeForColor = Color.FromArgb(255, 255, 0),        // Neon Yellow
            LatestForColor = Color.FromArgb(0, 255, 255),        // Neon Cyan
            BackColor =Color.FromArgb(255, 0, 0, 0),                 // Black

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(55, 15, 50),      // Neon Cyan
            ButtonForeColor = Color.FromArgb(128, 255, 0),
            ButtonHoverBackColor = Color.FromArgb(0, 255, 127),  // Neon Green
            ButtonHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            ButtonActiveBackColor = Color.FromArgb(255, 20, 147), // Neon Pink
            ButtonActiveForeColor = Color.FromArgb(255, 0, 0, 0),

            // **TextBox Colors**
            TextBoxBackColor = Color.FromArgb(15, 15, 15),
            TextBoxForeColor = Color.FromArgb(128, 255, 0),


            // **Label Colors**
            LabelBackColor =Color.FromArgb(255, 0, 0, 0),
            LabelForeColor = Color.FromArgb(128, 255, 0),

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(15, 15, 50), // Deep Neon Blue Tint


            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(30, 30, 30),
            HeaderForeColor = Color.FromArgb(0, 255, 255),
            GridLineColor = Color.FromArgb(0, 255, 255),
            RowBackColor = Color.FromArgb(15, 15, 15),
            RowForeColor = Color.FromArgb(128, 255, 0),
            AltRowBackColor = Color.FromArgb(30, 30, 30),
            SelectedRowBackColor = Color.FromArgb(255, 0, 255),
            SelectedRowForeColor = Color.FromArgb(255, 0, 0, 0),

            // **ComboBox Colors**
            ComboBoxBackColor = Color.FromArgb(15, 15, 15),
            ComboBoxForeColor = Color.FromArgb(128, 255, 0),


            // **CheckBox Colors**
            CheckBoxBackColor =Color.FromArgb(255, 0, 0, 0),
            CheckBoxForeColor = Color.FromArgb(128, 255, 0),

            // **RadioButton Colors**
            RadioButtonBackColor =Color.FromArgb(255, 0, 0, 0),
            RadioButtonForeColor = Color.FromArgb(128, 255, 0),

            // **Border Colors**
            BorderColor = Color.FromArgb(0, 255, 255),
            ActiveBorderColor = Color.FromArgb(255, 0, 255),
            InactiveBorderColor = Color.FromArgb(128, 128, 128),
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(0, 255, 255),
            VisitedLinkColor = Color.FromArgb(255, 0, 255),
            HoverLinkColor = Color.FromArgb(0, 255, 127),
            LinkHoverColor = Color.FromArgb(0, 255, 127),
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(30, 30, 30),
            ToolTipForeColor = Color.FromArgb(255, 0, 255),

            // **ScrollBar Colors**
            ScrollBarBackColor =Color.FromArgb(255, 0, 0, 0),
            ScrollBarThumbColor = Color.FromArgb(0, 255, 255),
            ScrollBarTrackColor = Color.FromArgb(15, 15, 15),

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(15, 15, 15),
            StatusBarForeColor = Color.FromArgb(0, 255, 255),

            // **Tab Colors**
            TabBackColor =Color.FromArgb(255, 0, 0, 0),
            TabForeColor = Color.FromArgb(128, 255, 0),
            ActiveTabBackColor = Color.FromArgb(30, 30, 30),
            ActiveTabForeColor = Color.FromArgb(255, 0, 255),

            // **Dialog Box Colors**
            DialogBackColor = Color.FromArgb(15, 15, 15),
            DialogForeColor = Color.FromArgb(128, 255, 0),
            DialogButtonBackColor = Color.FromArgb(0, 255, 255),
            DialogButtonForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Gradient Properties**
            GradientStartColor =Color.FromArgb(255, 0, 0, 0),
            GradientEndColor = Color.FromArgb(30, 30, 30),
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(40, 0, 50), // Neon Purple

            SideMenuHoverBackColor = Color.FromArgb(15, 15, 15),
            SideMenuSelectedBackColor = Color.FromArgb(30, 30, 30),
            SideMenuForeColor = Color.FromArgb(128, 255, 0),
            SideMenuHoverForeColor = Color.FromArgb(0, 255, 255),
            SideMenuSelectedForeColor = Color.FromArgb(255, 0, 255),
            SideMenuBorderColor = Color.FromArgb(0, 255, 255),
            SideMenuIconColor = Color.FromArgb(128, 255, 0),
            SideMenuSelectedIconColor = Color.FromArgb(255, 0, 255),

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(25, 25, 15),
            TitleBarForeColor = Color.FromArgb(255, 0, 255),
            TitleBarHoverBackColor = Color.FromArgb(30, 30, 30),
            TitleBarHoverForeColor = Color.FromArgb(255, 0, 255),

            // **Dashboard Colors**
            DashboardBackColor =Color.FromArgb(255, 0, 0, 0),
            DashboardCardBackColor = Color.FromArgb(15, 15, 15),
            DashboardCardHoverBackColor = Color.FromArgb(30, 30, 30),
            CardTitleForeColor = Color.FromArgb(0, 255, 255),
            CardTextForeColor = Color.FromArgb(128, 255, 0),

            // **Data Visualization (Charts)**
            ChartBackColor =Color.FromArgb(255, 0, 0, 0),
            ChartLineColor = Color.FromArgb(255, 0, 255),
            ChartFillColor = Color.FromArgb(100, 255, 0, 255), // Semi-transparent Neon Magenta
            ChartAxisColor = Color.FromArgb(128, 255, 0),

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(128, 255, 0),
            SidebarSelectedIconColor = Color.FromArgb(255, 0, 255),
            SidebarTextColor = Color.FromArgb(128, 255, 0),
            SidebarSelectedTextColor = Color.FromArgb(255, 0, 255),

            // **Navigation Colors**
            NavigationBackColor =Color.FromArgb(255, 0, 0, 0),
            NavigationForeColor = Color.FromArgb(128, 255, 0),
            NavigationHoverBackColor = Color.FromArgb(15, 15, 15),
            NavigationHoverForeColor = Color.FromArgb(0, 255, 255),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(255, 0, 255),       // Neon Magenta
            BadgeForeColor = Color.FromArgb(255, 0, 0, 0),
            HighlightBackColor = Color.FromArgb(255, 255, 0),   // Neon Yellow

            // **Font Properties**
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(128, 255, 0),
            SecondaryTextColor = Color.FromArgb(0, 255, 255),
            AccentTextColor = Color.FromArgb(255, 0, 255),

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(255, 0, 255),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(128, 255, 0),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 255, 0),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 20, 147),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(0, 255, 255),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(128, 255, 0),
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            BlockquoteBorderColor = Color.FromArgb(255, 0, 255),
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 0, 255),
            },
            InlineCodeBackgroundColor = Color.FromArgb(30, 30, 30),
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(128, 255, 0),
            },
            CodeBlockBackgroundColor = Color.FromArgb(15, 15, 15),
            CodeBlockBorderColor = Color.FromArgb(0, 255, 255),
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(128, 255, 0),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(128, 255, 0),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 255),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 255, 255),
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 255),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(128, 255, 0),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 255),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 0),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(255, 0, 255),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(128, 255, 0),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(128, 255, 0),
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(128, 255, 0),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(128, 255, 0),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 255),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(255, 255, 0),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(0, 255, 255),          // Neon Cyan
            SecondaryColor = Color.FromArgb(128, 255, 0),        // Neon Green
            AccentColor = Color.FromArgb(255, 0, 255),           // Neon Magenta
            BackgroundColor =Color.FromArgb(255, 0, 0, 0),           // Black
            SurfaceColor = Color.FromArgb(15, 15, 15),
            ErrorColor = Color.FromArgb(255, 0, 0),              // Neon Red
            WarningColor = Color.FromArgb(255, 255, 0),          // Neon Yellow
            SuccessColor = Color.FromArgb(0, 255, 0),            // Neon Green
            OnPrimaryColor = Color.FromArgb(255, 0, 0, 0),
            OnBackgroundColor = Color.FromArgb(128, 255, 0),

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 0,

            // **Imagery and Iconography**
            IconSet = "NeonIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(100, 0, 255, 255), // Semi-transparent Neon Cyan
            ShadowOpacity = 0.4f,

            // **Animation and Transitions**
            AnimationDurationShort = 100,  // in milliseconds
            AnimationDurationMedium = 200,
            AnimationDurationLong = 300,
            AnimationEasingFunction = "ease-in-out",

            // **Accessibility**
            HighContrastMode = true,
            FocusIndicatorColor = Color.FromArgb(255, 0, 255),

            // **Theme Variant**
            IsDarkTheme = true,
        };
        public static BeepTheme RusticTheme => new BeepTheme
        {
            GridBackColor = Color.FromArgb(222, 184, 135), // Burlywood
            GridForeColor = Color.SaddleBrown,
            GridHeaderBackColor = Color.Chocolate,
            GridHeaderForeColor = Color.White,
            GridHeaderBorderColor = Color.Sienna,
            GridHeaderHoverBackColor = Color.SandyBrown,
            GridHeaderHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderSelectedBackColor = Color.Peru,
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.Sienna,
            GridHeaderSelectedBorderColor = Color.Chocolate,
            GridRowHoverBackColor = Color.SandyBrown,
            GridRowHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridRowSelectedBackColor = Color.Chocolate,
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.Sienna,
            GridRowSelectedBorderColor = Color.Chocolate,
            CardBackColor = Color.FromArgb(210, 180, 140),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(80, 40, 20),     // Dark Brown

            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(100, 60, 40),    // Medium Brown
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(128, 0, 0),        // Maroon
            MaxButtonColor = Color.FromArgb(85, 107, 47),        // Dark Olive Green
            MinButtonColor = Color.FromArgb(139, 69, 19),        // Saddle Brown
            TitleBarColor = Color.FromArgb(205, 133, 63),        // Peru
            TitleBarTextColor = Color.FromArgb(80, 40, 20),      // Dark Brown
            TitleBarIconColor = Color.FromArgb(80, 40, 20),
            TitleBarHoverColor = Color.FromArgb(222, 184, 135),  // Burlywood
            TitleBarHoverTextColor = Color.FromArgb(80, 40, 20),
            TitleBarHoverIconColor = Color.FromArgb(80, 40, 20),
            TitleBarActiveColor = Color.FromArgb(222, 184, 135),
            TitleBarActiveTextColor = Color.FromArgb(80, 40, 20),
            TitleBarActiveIconColor = Color.FromArgb(80, 40, 20),
            TitleBarInactiveColor = Color.FromArgb(210, 180, 140), // Tan
            TitleBarInactiveTextColor = Color.FromArgb(139, 69, 19),
            TitleBarInactiveIconColor = Color.FromArgb(139, 69, 19),
            TitleBarBorderColor = Color.FromArgb(160, 82, 45),     // Sienna
            TitleBarBorderHoverColor = Color.FromArgb(139, 69, 19),
            TitleBarBorderActiveColor = Color.FromArgb(139, 69, 19),
            TitleBarBorderInactiveColor = Color.FromArgb(160, 82, 45),

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(165, 42, 42),   // Brown
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(128, 0, 0),    // Maroon
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(210, 180, 140),
            TitleBarCloseInactiveTextColor = Color.FromArgb(139, 69, 19),
            TitleBarCloseInactiveIconColor = Color.FromArgb(139, 69, 19),
            TitleBarCloseBorderColor = Color.FromArgb(160, 82, 45),
            TitleBarCloseBorderHoverColor = Color.FromArgb(139, 69, 19),
            TitleBarCloseBorderActiveColor = Color.FromArgb(139, 69, 19),
            TitleBarCloseBorderInactiveColor = Color.FromArgb(160, 82, 45),

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(107, 142, 35),    // Olive Drab
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.White,
            TitleBarMaxActiveColor = Color.FromArgb(85, 107, 47),    // Dark Olive Green
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(210, 180, 140),
            TitleBarMaxInactiveTextColor = Color.FromArgb(139, 69, 19),
            TitleBarMaxInactiveIconColor = Color.FromArgb(139, 69, 19),
            TitleBarMaxBorderColor = Color.FromArgb(160, 82, 45),
            TitleBarMaxBorderHoverColor = Color.FromArgb(139, 69, 19),
            TitleBarMaxBorderActiveColor = Color.FromArgb(139, 69, 19),
            TitleBarMaxBorderInactiveColor = Color.FromArgb(160, 82, 45),

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(160, 82, 45),     // Sienna
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.White,
            TitleBarMinActiveColor = Color.FromArgb(139, 69, 19),    // Saddle Brown
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(210, 180, 140),
            TitleBarMinInactiveTextColor = Color.FromArgb(139, 69, 19),
            TitleBarMinInactiveIconColor = Color.FromArgb(139, 69, 19),
            TitleBarMinBorderColor = Color.FromArgb(160, 82, 45),
            TitleBarMinBorderHoverColor = Color.FromArgb(139, 69, 19),
            TitleBarMinBorderActiveColor = Color.FromArgb(139, 69, 19),
            TitleBarMinBorderInactiveColor = Color.FromArgb(160, 82, 45),

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(160, 82, 45),
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(139, 69, 19),
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(210, 180, 140),
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(139, 69, 19),
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(139, 69, 19),
            TitleBarMinimizeBorderColor = Color.FromArgb(160, 82, 45),
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(139, 69, 19),
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(139, 69, 19),
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(160, 82, 45),

            // **General Colors**
            TitleForColor = Color.FromArgb(80, 40, 20),          // Dark Brown
            TitleBarForColor = Color.FromArgb(80, 40, 20),
            DescriptionForColor = Color.FromArgb(100, 60, 40),   // Medium Brown
            BeforeForColor = Color.FromArgb(205, 133, 63),       // Peru
            LatestForColor = Color.FromArgb(139, 69, 19),        // Saddle Brown
            BackColor = Color.FromArgb(245, 222, 179),           // Wheat

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(245, 222, 179),       // Sienna
            ButtonForeColor = Color.FromArgb(80, 40, 20),
            ButtonHoverBackColor = Color.FromArgb(139, 69, 19),  // Saddle Brown
            ButtonHoverForeColor = Color.White,
            ButtonActiveBackColor = Color.FromArgb(128, 0, 0),   // Maroon
            ButtonActiveForeColor = Color.White,

            // **TextBox Colors**
            TextBoxBackColor = Color.FromArgb(245, 222, 179),
            TextBoxForeColor = Color.FromArgb(80, 40, 20),


            // **Label Colors**
            LabelBackColor = Color.FromArgb(245, 222, 179),
            LabelForeColor = Color.FromArgb(80, 40, 20),

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(210, 180, 140),// Tan


            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(222, 184, 135),
            HeaderForeColor = Color.FromArgb(80, 40, 20),
            GridLineColor = Color.FromArgb(160, 82, 45),
            RowBackColor = Color.FromArgb(245, 222, 179),
            RowForeColor = Color.FromArgb(80, 40, 20),
            AltRowBackColor = Color.FromArgb(238, 232, 170),     // Pale Goldenrod
            SelectedRowBackColor = Color.FromArgb(205, 133, 63),
            SelectedRowForeColor = Color.White,

            // **ComboBox Colors**
            ComboBoxBackColor = Color.FromArgb(245, 222, 179),
            ComboBoxForeColor = Color.FromArgb(80, 40, 20),


            // **CheckBox Colors**
            CheckBoxBackColor = Color.FromArgb(245, 222, 179),
            CheckBoxForeColor = Color.FromArgb(80, 40, 20),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.FromArgb(245, 222, 179),
            RadioButtonForeColor = Color.FromArgb(80, 40, 20),

            // **Border Colors**
            BorderColor = Color.FromArgb(160, 82, 45),
            ActiveBorderColor = Color.FromArgb(139, 69, 19),
            InactiveBorderColor = Color.FromArgb(160, 82, 45),
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(139, 69, 19),
            VisitedLinkColor = Color.FromArgb(160, 82, 45),
            HoverLinkColor = Color.FromArgb(205, 133, 63),
            LinkHoverColor = Color.FromArgb(205, 133, 63),
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(80, 40, 20),
            ToolTipForeColor = Color.White,

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(222, 184, 135),
            ScrollBarThumbColor = Color.FromArgb(160, 82, 45),
            ScrollBarTrackColor = Color.FromArgb(210, 180, 140),

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(205, 133, 63),
            StatusBarForeColor = Color.FromArgb(80, 40, 20),

            // **Tab Colors**
            TabBackColor = Color.FromArgb(245, 222, 179),
            TabForeColor = Color.FromArgb(80, 40, 20),
            ActiveTabBackColor = Color.FromArgb(222, 184, 135),
            ActiveTabForeColor = Color.FromArgb(80, 40, 20),

            // **Dialog Box Colors**
            DialogBackColor = Color.FromArgb(245, 222, 179),
            DialogForeColor = Color.FromArgb(80, 40, 20),
            DialogButtonBackColor = Color.FromArgb(160, 82, 45),
            DialogButtonForeColor = Color.White,

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(245, 222, 179),
            GradientEndColor = Color.FromArgb(222, 184, 135),
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(160, 82, 45), // Sienna

            SideMenuHoverBackColor = Color.FromArgb(222, 184, 135),
            SideMenuSelectedBackColor = Color.FromArgb(205, 133, 63),
            SideMenuForeColor = Color.FromArgb(10, 20, 50),
            SideMenuHoverForeColor = Color.FromArgb(80, 40, 20),
            SideMenuSelectedForeColor = Color.White,
            SideMenuBorderColor = Color.FromArgb(160, 82, 45),
            SideMenuIconColor = Color.FromArgb(80, 40, 20),
            SideMenuSelectedIconColor = Color.White,

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(205, 133, 63),
            TitleBarForeColor = Color.FromArgb(80, 40, 20),
            TitleBarHoverBackColor = Color.FromArgb(222, 184, 135),
            TitleBarHoverForeColor = Color.FromArgb(80, 40, 20),

            // **Dashboard Colors**
            DashboardBackColor = Color.FromArgb(245, 222, 179),
            DashboardCardBackColor = Color.FromArgb(222, 184, 135),
            DashboardCardHoverBackColor = Color.FromArgb(210, 180, 140),
            CardTitleForeColor = Color.FromArgb(80, 40, 20),
            CardTextForeColor = Color.FromArgb(100, 60, 40),

            // **Data Visualization (Charts)**
            ChartBackColor = Color.FromArgb(245, 222, 179),
            ChartLineColor = Color.FromArgb(139, 69, 19),
            ChartFillColor = Color.FromArgb(100, 139, 69, 19), // Semi-transparent Saddle Brown
            ChartAxisColor = Color.FromArgb(80, 40, 20),

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(80, 40, 20),
            SidebarSelectedIconColor = Color.White,
            SidebarTextColor = Color.FromArgb(80, 40, 20),
            SidebarSelectedTextColor = Color.White,

            // **Navigation Colors**
            NavigationBackColor = Color.FromArgb(245, 222, 179),
            NavigationForeColor = Color.FromArgb(80, 40, 20),
            NavigationHoverBackColor = Color.FromArgb(222, 184, 135),
            NavigationHoverForeColor = Color.FromArgb(80, 40, 20),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(139, 69, 19),        // Saddle Brown
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(205, 133, 63),   // Peru

            // **Font Properties**
            FontFamily = "Georgia",
            FontName = "Georgia",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(80, 40, 20),
            SecondaryTextColor = Color.FromArgb(100, 60, 40),
            AccentTextColor = Color.FromArgb(139, 69, 19),

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(80, 40, 20),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(80, 40, 20),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(80, 40, 20),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(80, 40, 20),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(80, 40, 20),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(80, 40, 20),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(100, 60, 40),
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(100, 60, 40),
            },
            BlockquoteBorderColor = Color.FromArgb(160, 82, 45),
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Courier New",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(80, 40, 20),
            },
            InlineCodeBackgroundColor = Color.FromArgb(245, 222, 179),
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Courier New",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(80, 40, 20),
            },
            CodeBlockBackgroundColor = Color.FromArgb(245, 222, 179),
            CodeBlockBorderColor = Color.FromArgb(160, 82, 45),
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(80, 40, 20),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(80, 40, 20),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(139, 69, 19),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(80, 40, 20),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(100, 60, 40),
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(80, 40, 20),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(80, 40, 20),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(80, 40, 20),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(80, 40, 20),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(80, 40, 20),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(80, 40, 20),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(80, 40, 20),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(80, 40, 20),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(80, 40, 20),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(100, 60, 40),
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(100, 60, 40),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(100, 60, 40),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(80, 40, 20),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(80, 40, 20),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Georgia",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(80, 40, 20),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(139, 69, 19),            // Saddle Brown
            SecondaryColor = Color.FromArgb(205, 133, 63),         // Peru
            AccentColor = Color.FromArgb(160, 82, 45),             // Sienna
            BackgroundColor = Color.FromArgb(245, 222, 179),       // Wheat
            SurfaceColor = Color.FromArgb(222, 184, 135),
            ErrorColor = Color.FromArgb(128, 0, 0),                // Maroon
            WarningColor = Color.FromArgb(184, 134, 11),           // Dark Goldenrod
            SuccessColor = Color.FromArgb(85, 107, 47),            // Dark Olive Green
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.FromArgb(80, 40, 20),

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 2,

            // **Imagery and Iconography**
            IconSet = "RusticIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(100, 0, 0, 0), // Semi-transparent black
            ShadowOpacity = 0.2f,

            // **Animation and Transitions**
            AnimationDurationShort = 200,  // in milliseconds
            AnimationDurationMedium = 400,
            AnimationDurationLong = 600,
            AnimationEasingFunction = "ease-in-out",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(139, 69, 19),

            // **Theme Variant**
            IsDarkTheme = false,
        };
        public static BeepTheme GalaxyTheme => new BeepTheme
        {
            GridBackColor = Color.FromArgb(255, 0, 0, 0),
            GridForeColor = Color.White,
            GridHeaderBackColor = Color.FromArgb(25, 25, 112), // Midnight Blue
            GridHeaderForeColor = Color.White,
            GridHeaderBorderColor = Color.Gray,
            GridHeaderHoverBackColor = Color.FromArgb(72, 61, 139), // Dark Slate Blue
            GridHeaderHoverForeColor = Color.White,
            GridHeaderSelectedBackColor = Color.FromArgb(0, 0, 128), // Navy
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.Gray,
            GridHeaderSelectedBorderColor = Color.FromArgb(0, 0, 128),
            GridRowHoverBackColor = Color.FromArgb(32, 32, 64), // Deep Space
            GridRowHoverForeColor = Color.White,
            GridRowSelectedBackColor = Color.FromArgb(0, 0, 128),
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.Gray,
            GridRowSelectedBorderColor = Color.FromArgb(0, 0, 128),

            CardBackColor = Color.FromArgb(70, 35, 90),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(173, 216, 230),   // Light Blue

            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(221, 160, 221),   // Plum
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(139, 0, 0),         // Dark Red
            MaxButtonColor = Color.FromArgb(70, 130, 180),        // Steel Blue
            MinButtonColor = Color.FromArgb(138, 43, 226),        // Blue Violet
            TitleBarColor = Color.FromArgb(18, 18, 50),           // Deep Space Blue
            TitleBarTextColor = Color.FromArgb(230, 230, 250),    // Lavender
            TitleBarIconColor = Color.FromArgb(230, 230, 250),
            TitleBarHoverColor = Color.FromArgb(28, 28, 70),      // Slightly Lighter Blue
            TitleBarHoverTextColor = Color.FromArgb(230, 230, 250),
            TitleBarHoverIconColor = Color.FromArgb(230, 230, 250),
            TitleBarActiveColor = Color.FromArgb(28, 28, 70),
            TitleBarActiveTextColor = Color.FromArgb(230, 230, 250),
            TitleBarActiveIconColor = Color.FromArgb(230, 230, 250),
            TitleBarInactiveColor = Color.FromArgb(20, 20, 50),
            TitleBarInactiveTextColor = Color.FromArgb(169, 169, 169),
            TitleBarInactiveIconColor = Color.FromArgb(169, 169, 169),
            TitleBarBorderColor = Color.FromArgb(72, 61, 139),    // Dark Slate Blue
            TitleBarBorderHoverColor = Color.FromArgb(75, 0, 130), // Indigo
            TitleBarBorderActiveColor = Color.FromArgb(75, 0, 130),
            TitleBarBorderInactiveColor = Color.FromArgb(72, 61, 139),

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(178, 34, 34),   // Firebrick
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(139, 0, 0),    // Dark Red
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(20, 20, 50),
            TitleBarCloseInactiveTextColor = Color.FromArgb(169, 169, 169),
            TitleBarCloseInactiveIconColor = Color.FromArgb(169, 169, 169),
            TitleBarCloseBorderColor = Color.FromArgb(72, 61, 139),
            TitleBarCloseBorderHoverColor = Color.FromArgb(75, 0, 130),
            TitleBarCloseBorderActiveColor = Color.FromArgb(75, 0, 130),
            TitleBarCloseBorderInactiveColor = Color.FromArgb(72, 61, 139),

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(95, 158, 160),    // Cadet Blue
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.White,
            TitleBarMaxActiveColor = Color.FromArgb(70, 130, 180),   // Steel Blue
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(20, 20, 50),
            TitleBarMaxInactiveTextColor = Color.FromArgb(169, 169, 169),
            TitleBarMaxInactiveIconColor = Color.FromArgb(169, 169, 169),
            TitleBarMaxBorderColor = Color.FromArgb(72, 61, 139),
            TitleBarMaxBorderHoverColor = Color.FromArgb(75, 0, 130),
            TitleBarMaxBorderActiveColor = Color.FromArgb(75, 0, 130),
            TitleBarMaxBorderInactiveColor = Color.FromArgb(72, 61, 139),

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(153, 50, 204),     // Dark Orchid
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.White,
            TitleBarMinActiveColor = Color.FromArgb(138, 43, 226),    // Blue Violet
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(20, 20, 50),
            TitleBarMinInactiveTextColor = Color.FromArgb(169, 169, 169),
            TitleBarMinInactiveIconColor = Color.FromArgb(169, 169, 169),
            TitleBarMinBorderColor = Color.FromArgb(72, 61, 139),
            TitleBarMinBorderHoverColor = Color.FromArgb(75, 0, 130),
            TitleBarMinBorderActiveColor = Color.FromArgb(75, 0, 130),
            TitleBarMinBorderInactiveColor = Color.FromArgb(72, 61, 139),

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(153, 50, 204),
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(138, 43, 226),
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(20, 20, 50),
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(169, 169, 169),
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(169, 169, 169),
            TitleBarMinimizeBorderColor = Color.FromArgb(72, 61, 139),
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(75, 0, 130),
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(75, 0, 130),
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(72, 61, 139),

            // **General Colors**
            TitleForColor = Color.FromArgb(230, 230, 250),        // Lavender
            TitleBarForColor = Color.FromArgb(230, 230, 250),
            DescriptionForColor = Color.FromArgb(221, 160, 221),  // Plum
            BeforeForColor = Color.FromArgb(138, 43, 226),        // Blue Violet
            LatestForColor = Color.FromArgb(65, 105, 225),        // Royal Blue
            BackColor = Color.FromArgb(18, 18, 50),               // Deep Space Blue

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(25, 25, 112),        // Midnight Blue
            ButtonForeColor = Color.FromArgb(173, 216, 230),      // Light Blue
            ButtonHoverBackColor = Color.FromArgb(72, 61, 139),   // Dark Slate Blue
            ButtonHoverForeColor = Color.White,
            ButtonActiveBackColor = Color.FromArgb(138, 43, 226), // Blue Violet
            ButtonActiveForeColor = Color.White,

            // **TextBox Colors**
            TextBoxBackColor = Color.FromArgb(25, 25, 112),
            TextBoxForeColor = Color.FromArgb(230, 230, 250),


            // **Label Colors**
            LabelBackColor = Color.FromArgb(18, 18, 50),
            LabelForeColor = Color.FromArgb(230, 230, 250),

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(25, 25, 70), // Slightly Lighter Deep Space Blue


            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(25, 25, 112),
            HeaderForeColor = Color.FromArgb(230, 230, 250),
            GridLineColor = Color.FromArgb(72, 61, 139),
            RowBackColor = Color.FromArgb(20, 20, 50),
            RowForeColor = Color.FromArgb(230, 230, 250),
            AltRowBackColor = Color.FromArgb(25, 25, 70),
            SelectedRowBackColor = Color.FromArgb(65, 105, 225),
            SelectedRowForeColor = Color.White,

            // **ComboBox Colors**
            ComboBoxBackColor = Color.FromArgb(25, 25, 112),
            ComboBoxForeColor = Color.FromArgb(230, 230, 250),


            // **CheckBox Colors**
            CheckBoxBackColor = Color.FromArgb(18, 18, 50),
            CheckBoxForeColor = Color.FromArgb(230, 230, 250),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.FromArgb(18, 18, 50),
            RadioButtonForeColor = Color.FromArgb(230, 230, 250),

            // **Border Colors**
            BorderColor = Color.FromArgb(72, 61, 139),
            ActiveBorderColor = Color.FromArgb(138, 43, 226),
            InactiveBorderColor = Color.FromArgb(72, 61, 139),
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(65, 105, 225),
            VisitedLinkColor = Color.FromArgb(138, 43, 226),
            HoverLinkColor = Color.FromArgb(123, 104, 238),       // Medium Slate Blue
            LinkHoverColor = Color.FromArgb(123, 104, 238),
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(25, 25, 70),
            ToolTipForeColor = Color.FromArgb(230, 230, 250),

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(18, 18, 50),
            ScrollBarThumbColor = Color.FromArgb(72, 61, 139),
            ScrollBarTrackColor = Color.FromArgb(25, 25, 70),

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(25, 25, 70),
            StatusBarForeColor = Color.FromArgb(230, 230, 250),

            // **Tab Colors**
            TabBackColor = Color.FromArgb(18, 18, 50),
            TabForeColor = Color.FromArgb(230, 230, 250),
            ActiveTabBackColor = Color.FromArgb(25, 25, 70),
            ActiveTabForeColor = Color.FromArgb(230, 230, 250),

            // **Dialog Box Colors**
            DialogBackColor = Color.FromArgb(20, 20, 50),
            DialogForeColor = Color.FromArgb(230, 230, 250),
            DialogButtonBackColor = Color.FromArgb(25, 25, 70),
            DialogButtonForeColor = Color.FromArgb(230, 230, 250),

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(18, 18, 50),
            GradientEndColor = Color.FromArgb(25, 25, 70),
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.DarkSlateBlue,// Darker Space Blue

            SideMenuHoverBackColor = Color.FromArgb(25, 25, 70),
            SideMenuSelectedBackColor = Color.FromArgb(25, 25, 70),
            SideMenuForeColor = Color.FromArgb(230, 230, 250),
            SideMenuHoverForeColor = Color.FromArgb(230, 230, 250),
            SideMenuSelectedForeColor = Color.FromArgb(230, 230, 250),
            SideMenuBorderColor = Color.FromArgb(72, 61, 139),
            SideMenuIconColor = Color.FromArgb(230, 230, 250),
            SideMenuSelectedIconColor = Color.FromArgb(230, 230, 250),

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(25, 25, 70),
            TitleBarForeColor = Color.FromArgb(230, 230, 250),
            TitleBarHoverBackColor = Color.FromArgb(28, 28, 70),
            TitleBarHoverForeColor = Color.FromArgb(230, 230, 250),

            // **Dashboard Colors**
            DashboardBackColor = Color.FromArgb(18, 18, 50),
            DashboardCardBackColor = Color.FromArgb(20, 20, 50),
            DashboardCardHoverBackColor = Color.FromArgb(25, 25, 70),
            CardTitleForeColor = Color.FromArgb(230, 230, 250),
            CardTextForeColor = Color.FromArgb(221, 160, 221),

            // **Data Visualization (Charts)**
            ChartBackColor = Color.FromArgb(18, 18, 50),
            ChartLineColor = Color.FromArgb(138, 43, 226),
            ChartFillColor = Color.FromArgb(100, 138, 43, 226), // Semi-transparent Blue Violet
            ChartAxisColor = Color.FromArgb(230, 230, 250),

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(230, 230, 250),
            SidebarSelectedIconColor = Color.FromArgb(230, 230, 250),
            SidebarTextColor = Color.FromArgb(230, 230, 250),
            SidebarSelectedTextColor = Color.FromArgb(230, 230, 250),

            // **Navigation Colors**
            NavigationBackColor = Color.FromArgb(18, 18, 50),
            NavigationForeColor = Color.FromArgb(230, 230, 250),
            NavigationHoverBackColor = Color.FromArgb(25, 25, 70),
            NavigationHoverForeColor = Color.FromArgb(230, 230, 250),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(138, 43, 226),      // Blue Violet
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(65, 105, 225),  // Royal Blue

            // **Font Properties**
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(230, 230, 250),
            SecondaryTextColor = Color.FromArgb(221, 160, 221),
            AccentTextColor = Color.FromArgb(138, 43, 226),

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(230, 230, 250),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(221, 160, 221),
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(221, 160, 221),
            },
            BlockquoteBorderColor = Color.FromArgb(72, 61, 139),
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            InlineCodeBackgroundColor = Color.FromArgb(25, 25, 70),
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            CodeBlockBackgroundColor = Color.FromArgb(20, 20, 50),
            CodeBlockBorderColor = Color.FromArgb(72, 61, 139),
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(221, 160, 221),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(221, 160, 221),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(169, 169, 169),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(221, 160, 221),
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(230, 230, 250),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(221, 160, 221),
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(221, 160, 221),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(221, 160, 221),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(230, 230, 250),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(230, 230, 250),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(138, 43, 226),            // Blue Violet
            SecondaryColor = Color.FromArgb(65, 105, 225),          // Royal Blue
            AccentColor = Color.FromArgb(221, 160, 221),            // Plum
            BackgroundColor = Color.FromArgb(18, 18, 50),           // Deep Space Blue
            SurfaceColor = Color.FromArgb(25, 25, 70),
            ErrorColor = Color.FromArgb(139, 0, 0),                 // Dark Red
            WarningColor = Color.FromArgb(255, 140, 0),             // Dark Orange
            SuccessColor = Color.FromArgb(34, 139, 34),             // Forest Green
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.FromArgb(230, 230, 250),

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 5,

            // **Imagery and Iconography**
            IconSet = "GalaxyIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(100, 0, 0, 0), // Semi-transparent black
            ShadowOpacity = 0.3f,

            // **Animation and Transitions**
            AnimationDurationShort = 150,  // in milliseconds
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(138, 43, 226),

            // **Theme Variant**
            IsDarkTheme = true,
        };
        public static BeepTheme DesertTheme => new BeepTheme
        {
            GridBackColor = Color.FromArgb(255, 244, 214), // Sand
            GridForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBackColor = Color.FromArgb(210, 180, 140), // Tan
            GridHeaderForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBorderColor = Color.SandyBrown,
            GridHeaderHoverBackColor = Color.FromArgb(244, 164, 96), // Sandy Brown
            GridHeaderHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderSelectedBackColor = Color.FromArgb(210, 105, 30), // Chocolate
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.SandyBrown,
            GridHeaderSelectedBorderColor = Color.Chocolate,
            GridRowHoverBackColor = Color.FromArgb(244, 164, 96),
            GridRowHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridRowSelectedBackColor = Color.FromArgb(210, 105, 30),
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.SandyBrown,
            GridRowSelectedBorderColor = Color.Chocolate,
            CardBackColor = Color.FromArgb(237, 201, 175),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(102, 51, 0),       // Dark Brown

            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(153, 102, 51),     // Medium Brown
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(179, 89, 0),       // Burnt Orange
            MaxButtonColor = Color.FromArgb(205, 133, 63),       // Peru
            MinButtonColor = Color.FromArgb(189, 183, 107),      // Dark Khaki
            TitleBarColor = Color.FromArgb(210, 180, 140),       // Tan
            TitleBarTextColor = Color.FromArgb(102, 51, 0),      // Dark Brown
            TitleBarIconColor = Color.FromArgb(102, 51, 0),
            TitleBarHoverColor = Color.FromArgb(222, 184, 135),  // Burlywood
            TitleBarHoverTextColor = Color.FromArgb(102, 51, 0),
            TitleBarHoverIconColor = Color.FromArgb(102, 51, 0),
            TitleBarActiveColor = Color.FromArgb(222, 184, 135),
            TitleBarActiveTextColor = Color.FromArgb(102, 51, 0),
            TitleBarActiveIconColor = Color.FromArgb(102, 51, 0),
            TitleBarInactiveColor = Color.FromArgb(210, 180, 140),
            TitleBarInactiveTextColor = Color.FromArgb(153, 102, 51),
            TitleBarInactiveIconColor = Color.FromArgb(153, 102, 51),
            TitleBarBorderColor = Color.FromArgb(160, 82, 45),   // Sienna
            TitleBarBorderHoverColor = Color.FromArgb(139, 69, 19),
            TitleBarBorderActiveColor = Color.FromArgb(139, 69, 19),
            TitleBarBorderInactiveColor = Color.FromArgb(160, 82, 45),

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(205, 92, 92),   // Indian Red
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(179, 89, 0),    // Burnt Orange
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(210, 180, 140),
            TitleBarCloseInactiveTextColor = Color.FromArgb(153, 102, 51),
            TitleBarCloseInactiveIconColor = Color.FromArgb(153, 102, 51),
            TitleBarCloseBorderColor = Color.FromArgb(160, 82, 45),
            TitleBarCloseBorderHoverColor = Color.FromArgb(139, 69, 19),
            TitleBarCloseBorderActiveColor = Color.FromArgb(139, 69, 19),
            TitleBarCloseBorderInactiveColor = Color.FromArgb(160, 82, 45),

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(222, 184, 135),    // Burlywood
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.White,
            TitleBarMaxActiveColor = Color.FromArgb(205, 133, 63),    // Peru
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(210, 180, 140),
            TitleBarMaxInactiveTextColor = Color.FromArgb(153, 102, 51),
            TitleBarMaxInactiveIconColor = Color.FromArgb(153, 102, 51),
            TitleBarMaxBorderColor = Color.FromArgb(160, 82, 45),
            TitleBarMaxBorderHoverColor = Color.FromArgb(139, 69, 19),
            TitleBarMaxBorderActiveColor = Color.FromArgb(139, 69, 19),
            TitleBarMaxBorderInactiveColor = Color.FromArgb(160, 82, 45),

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(218, 165, 32),     // Goldenrod
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.White,
            TitleBarMinActiveColor = Color.FromArgb(189, 183, 107),   // Dark Khaki
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(210, 180, 140),
            TitleBarMinInactiveTextColor = Color.FromArgb(153, 102, 51),
            TitleBarMinInactiveIconColor = Color.FromArgb(153, 102, 51),
            TitleBarMinBorderColor = Color.FromArgb(160, 82, 45),
            TitleBarMinBorderHoverColor = Color.FromArgb(139, 69, 19),
            TitleBarMinBorderActiveColor = Color.FromArgb(139, 69, 19),
            TitleBarMinBorderInactiveColor = Color.FromArgb(160, 82, 45),

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(218, 165, 32),
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(189, 183, 107),
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(210, 180, 140),
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(153, 102, 51),
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(153, 102, 51),
            TitleBarMinimizeBorderColor = Color.FromArgb(160, 82, 45),
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(139, 69, 19),
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(139, 69, 19),
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(160, 82, 45),

            // **General Colors**
            TitleForColor = Color.FromArgb(102, 51, 0),          // Dark Brown
            TitleBarForColor = Color.FromArgb(102, 51, 0),
            DescriptionForColor = Color.FromArgb(153, 102, 51),  // Medium Brown
            BeforeForColor = Color.FromArgb(205, 133, 63),       // Peru
            LatestForColor = Color.FromArgb(189, 183, 107),      // Dark Khaki
            BackColor = Color.FromArgb(237, 201, 175),           // Light Sand

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(210, 180, 140),     // Tan
            ButtonForeColor = Color.FromArgb(102, 51, 0),
            ButtonHoverBackColor = Color.FromArgb(222, 184, 135),// Burlywood
            ButtonHoverForeColor = Color.FromArgb(102, 51, 0),
            ButtonActiveBackColor = Color.FromArgb(205, 133, 63),// Peru
            ButtonActiveForeColor = Color.White,

            // **TextBox Colors**
            TextBoxBackColor = Color.FromArgb(245, 222, 179),    // Wheat
            TextBoxForeColor = Color.FromArgb(102, 51, 0),


            // **Label Colors**
            LabelBackColor = Color.FromArgb(237, 201, 175),
            LabelForeColor = Color.FromArgb(102, 51, 0),

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(220, 180, 140), // Sandy Beige


            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(222, 184, 135),
            HeaderForeColor = Color.FromArgb(102, 51, 0),
            GridLineColor = Color.FromArgb(160, 82, 45),
            RowBackColor = Color.FromArgb(245, 222, 179),
            RowForeColor = Color.FromArgb(102, 51, 0),
            AltRowBackColor = Color.FromArgb(237, 201, 175),
            SelectedRowBackColor = Color.FromArgb(205, 133, 63),
            SelectedRowForeColor = Color.White,

            // **ComboBox Colors**
            ComboBoxBackColor = Color.FromArgb(245, 222, 179),
            ComboBoxForeColor = Color.FromArgb(102, 51, 0),


            // **CheckBox Colors**
            CheckBoxBackColor = Color.FromArgb(237, 201, 175),
            CheckBoxForeColor = Color.FromArgb(102, 51, 0),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.FromArgb(237, 201, 175),
            RadioButtonForeColor = Color.FromArgb(102, 51, 0),

            // **Border Colors**
            BorderColor = Color.FromArgb(160, 82, 45),
            ActiveBorderColor = Color.FromArgb(205, 133, 63),
            InactiveBorderColor = Color.FromArgb(160, 82, 45),
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(139, 69, 19),
            VisitedLinkColor = Color.FromArgb(160, 82, 45),
            HoverLinkColor = Color.FromArgb(205, 133, 63),
            LinkHoverColor = Color.FromArgb(205, 133, 63),
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(102, 51, 0),
            ToolTipForeColor = Color.White,

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(222, 184, 135),
            ScrollBarThumbColor = Color.FromArgb(205, 133, 63),
            ScrollBarTrackColor = Color.FromArgb(210, 180, 140),

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(205, 133, 63),
            StatusBarForeColor = Color.FromArgb(102, 51, 0),

            // **Tab Colors**
            TabBackColor = Color.FromArgb(237, 201, 175),
            TabForeColor = Color.FromArgb(102, 51, 0),
            ActiveTabBackColor = Color.FromArgb(222, 184, 135),
            ActiveTabForeColor = Color.FromArgb(102, 51, 0),

            // **Dialog Box Colors**
            DialogBackColor = Color.FromArgb(237, 201, 175),
            DialogForeColor = Color.FromArgb(102, 51, 0),
            DialogButtonBackColor = Color.FromArgb(205, 133, 63),
            DialogButtonForeColor = Color.White,

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(237, 201, 175),
            GradientEndColor = Color.FromArgb(210, 180, 140),
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(200, 160, 120), // Desert Brown

            SideMenuHoverBackColor = Color.FromArgb(222, 184, 135),
            SideMenuSelectedBackColor = Color.FromArgb(205, 133, 63),
            SideMenuForeColor = Color.FromArgb(102, 51, 0),
            SideMenuHoverForeColor = Color.FromArgb(102, 51, 0),
            SideMenuSelectedForeColor = Color.White,
            SideMenuBorderColor = Color.FromArgb(160, 82, 45),
            SideMenuIconColor = Color.FromArgb(102, 51, 0),
            SideMenuSelectedIconColor = Color.White,

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(210, 180, 140),
            TitleBarForeColor = Color.FromArgb(102, 51, 0),
            TitleBarHoverBackColor = Color.FromArgb(222, 184, 135),
            TitleBarHoverForeColor = Color.FromArgb(102, 51, 0),

            // **Dashboard Colors**
            DashboardBackColor = Color.FromArgb(237, 201, 175),
            DashboardCardBackColor = Color.FromArgb(245, 222, 179),
            DashboardCardHoverBackColor = Color.FromArgb(222, 184, 135),
            CardTitleForeColor = Color.FromArgb(102, 51, 0),
            CardTextForeColor = Color.FromArgb(153, 102, 51),

            // **Data Visualization (Charts)**
            ChartBackColor = Color.FromArgb(237, 201, 175),
            ChartLineColor = Color.FromArgb(205, 133, 63),
            ChartFillColor = Color.FromArgb(100, 205, 133, 63), // Semi-transparent Peru
            ChartAxisColor = Color.FromArgb(102, 51, 0),

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(102, 51, 0),
            SidebarSelectedIconColor = Color.White,
            SidebarTextColor = Color.FromArgb(102, 51, 0),
            SidebarSelectedTextColor = Color.White,

            // **Navigation Colors**
            NavigationBackColor = Color.FromArgb(237, 201, 175),
            NavigationForeColor = Color.FromArgb(102, 51, 0),
            NavigationHoverBackColor = Color.FromArgb(222, 184, 135),
            NavigationHoverForeColor = Color.FromArgb(102, 51, 0),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(205, 133, 63),        // Peru
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(218, 165, 32),    // Goldenrod

            // **Font Properties**
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(102, 51, 0),
            SecondaryTextColor = Color.FromArgb(153, 102, 51),
            AccentTextColor = Color.FromArgb(205, 133, 63),

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(102, 51, 0),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(102, 51, 0),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(102, 51, 0),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(102, 51, 0),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(102, 51, 0),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(102, 51, 0),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(153, 102, 51),
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(153, 102, 51),
            },
            BlockquoteBorderColor = Color.FromArgb(160, 82, 45),
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(102, 51, 0),
            },
            InlineCodeBackgroundColor = Color.FromArgb(245, 222, 179),
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(102, 51, 0),
            },
            CodeBlockBackgroundColor = Color.FromArgb(245, 222, 179),
            CodeBlockBorderColor = Color.FromArgb(160, 82, 45),
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(102, 51, 0),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(102, 51, 0),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(153, 102, 51),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(102, 51, 0),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(153, 102, 51),
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(102, 51, 0),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(102, 51, 0),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(102, 51, 0),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(102, 51, 0),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(102, 51, 0),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(102, 51, 0),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(102, 51, 0),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(102, 51, 0),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(102, 51, 0),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(153, 102, 51),
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(153, 102, 51),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(153, 102, 51),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(102, 51, 0),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(102, 51, 0),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(102, 51, 0),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(205, 133, 63),            // Peru
            SecondaryColor = Color.FromArgb(189, 183, 107),         // Dark Khaki
            AccentColor = Color.FromArgb(179, 89, 0),               // Burnt Orange
            BackgroundColor = Color.FromArgb(237, 201, 175),        // Light Sand
            SurfaceColor = Color.FromArgb(245, 222, 179),
            ErrorColor = Color.FromArgb(179, 89, 0),                // Burnt Orange
            WarningColor = Color.FromArgb(218, 165, 32),            // Goldenrod
            SuccessColor = Color.FromArgb(85, 107, 47),             // Dark Olive Green
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.FromArgb(102, 51, 0),

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 4,

            // **Imagery and Iconography**
            IconSet = "DesertIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(100, 0, 0, 0), // Semi-transparent black
            ShadowOpacity = 0.2f,

            // **Animation and Transitions**
            AnimationDurationShort = 200,  // in milliseconds
            AnimationDurationMedium = 400,
            AnimationDurationLong = 600,
            AnimationEasingFunction = "ease-in-out",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(205, 133, 63),

            // **Theme Variant**
            IsDarkTheme = false,
        };
        public static BeepTheme VintageTheme => new BeepTheme
        {
            GridBackColor = Color.FromArgb(245, 222, 179), // Wheat
            GridForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBackColor = Color.BurlyWood,
            GridHeaderForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBorderColor = Color.SaddleBrown,
            GridHeaderHoverBackColor = Color.Tan,
            GridHeaderHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderSelectedBackColor = Color.Peru,
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.SaddleBrown,
            GridHeaderSelectedBorderColor = Color.Chocolate,
            GridRowHoverBackColor = Color.Tan,
            GridRowHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridRowSelectedBackColor = Color.Peru,
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.SaddleBrown,
            GridRowSelectedBorderColor = Color.Chocolate,
            CardBackColor = Color.FromArgb(238, 232, 170),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(80, 60, 40),     // Dark Brown

            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(100, 80, 60),    // Medium Brown
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(128, 0, 0),        // Maroon
            MaxButtonColor = Color.FromArgb(85, 107, 47),        // Dark Olive Green
            MinButtonColor = Color.FromArgb(139, 69, 19),        // Saddle Brown
            TitleBarColor = Color.FromArgb(210, 180, 140),       // Tan
            TitleBarTextColor = Color.FromArgb(80, 60, 40),      // Dark Brown
            TitleBarIconColor = Color.FromArgb(80, 60, 40),
            TitleBarHoverColor = Color.FromArgb(222, 184, 135),  // Burlywood
            TitleBarHoverTextColor = Color.FromArgb(80, 60, 40),
            TitleBarHoverIconColor = Color.FromArgb(80, 60, 40),
            TitleBarActiveColor = Color.FromArgb(222, 184, 135),
            TitleBarActiveTextColor = Color.FromArgb(80, 60, 40),
            TitleBarActiveIconColor = Color.FromArgb(80, 60, 40),
            TitleBarInactiveColor = Color.FromArgb(210, 180, 140),
            TitleBarInactiveTextColor = Color.FromArgb(139, 69, 19),
            TitleBarInactiveIconColor = Color.FromArgb(139, 69, 19),
            TitleBarBorderColor = Color.FromArgb(160, 82, 45),     // Sienna
            TitleBarBorderHoverColor = Color.FromArgb(139, 69, 19),
            TitleBarBorderActiveColor = Color.FromArgb(139, 69, 19),
            TitleBarBorderInactiveColor = Color.FromArgb(160, 82, 45),

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(165, 42, 42),   // Brown
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(128, 0, 0),    // Maroon
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(210, 180, 140),
            TitleBarCloseInactiveTextColor = Color.FromArgb(139, 69, 19),
            TitleBarCloseInactiveIconColor = Color.FromArgb(139, 69, 19),
            TitleBarCloseBorderColor = Color.FromArgb(160, 82, 45),
            TitleBarCloseBorderHoverColor = Color.FromArgb(139, 69, 19),
            TitleBarCloseBorderActiveColor = Color.FromArgb(139, 69, 19),
            TitleBarCloseBorderInactiveColor = Color.FromArgb(160, 82, 45),

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(107, 142, 35),    // Olive Drab
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.White,
            TitleBarMaxActiveColor = Color.FromArgb(85, 107, 47),    // Dark Olive Green
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(210, 180, 140),
            TitleBarMaxInactiveTextColor = Color.FromArgb(139, 69, 19),
            TitleBarMaxInactiveIconColor = Color.FromArgb(139, 69, 19),
            TitleBarMaxBorderColor = Color.FromArgb(160, 82, 45),
            TitleBarMaxBorderHoverColor = Color.FromArgb(139, 69, 19),
            TitleBarMaxBorderActiveColor = Color.FromArgb(139, 69, 19),
            TitleBarMaxBorderInactiveColor = Color.FromArgb(160, 82, 45),

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(160, 82, 45),     // Sienna
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.White,
            TitleBarMinActiveColor = Color.FromArgb(139, 69, 19),    // Saddle Brown
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(210, 180, 140),
            TitleBarMinInactiveTextColor = Color.FromArgb(139, 69, 19),
            TitleBarMinInactiveIconColor = Color.FromArgb(139, 69, 19),
            TitleBarMinBorderColor = Color.FromArgb(160, 82, 45),
            TitleBarMinBorderHoverColor = Color.FromArgb(139, 69, 19),
            TitleBarMinBorderActiveColor = Color.FromArgb(139, 69, 19),
            TitleBarMinBorderInactiveColor = Color.FromArgb(160, 82, 45),

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(160, 82, 45),
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(139, 69, 19),
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(210, 180, 140),
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(139, 69, 19),
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(139, 69, 19),
            TitleBarMinimizeBorderColor = Color.FromArgb(160, 82, 45),
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(139, 69, 19),
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(139, 69, 19),
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(160, 82, 45),

            // **General Colors**
            TitleForColor = Color.FromArgb(80, 60, 40),          // Dark Brown
            TitleBarForColor = Color.FromArgb(80, 60, 40),
            DescriptionForColor = Color.FromArgb(100, 80, 60),   // Medium Brown
            BeforeForColor = Color.FromArgb(210, 180, 140),      // Tan
            LatestForColor = Color.FromArgb(139, 69, 19),        // Saddle Brown
            BackColor = Color.FromArgb(245, 222, 179),           // Wheat

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(210, 180, 140),     // Tan
            ButtonForeColor = Color.FromArgb(80, 60, 40),
            ButtonHoverBackColor = Color.FromArgb(222, 184, 135),// Burlywood
            ButtonHoverForeColor = Color.FromArgb(80, 60, 40),
            ButtonActiveBackColor = Color.FromArgb(160, 82, 45), // Sienna
            ButtonActiveForeColor = Color.White,

            // **TextBox Colors**
            TextBoxBackColor = Color.FromArgb(245, 222, 179),
            TextBoxForeColor = Color.FromArgb(80, 60, 40),


            // **Label Colors**
            LabelBackColor = Color.FromArgb(245, 222, 179),
            LabelForeColor = Color.FromArgb(80, 60, 40),

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(230, 200, 160), // Muted Tan


            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(222, 184, 135),
            HeaderForeColor = Color.FromArgb(80, 60, 40),
            GridLineColor = Color.FromArgb(160, 82, 45),
            RowBackColor = Color.FromArgb(245, 222, 179),
            RowForeColor = Color.FromArgb(80, 60, 40),
            AltRowBackColor = Color.FromArgb(238, 232, 170),     // Pale Goldenrod
            SelectedRowBackColor = Color.FromArgb(205, 133, 63),
            SelectedRowForeColor = Color.White,

            // **ComboBox Colors**
            ComboBoxBackColor = Color.FromArgb(245, 222, 179),
            ComboBoxForeColor = Color.FromArgb(80, 60, 40),


            // **CheckBox Colors**
            CheckBoxBackColor = Color.FromArgb(245, 222, 179),
            CheckBoxForeColor = Color.FromArgb(80, 60, 40),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.FromArgb(245, 222, 179),
            RadioButtonForeColor = Color.FromArgb(80, 60, 40),

            // **Border Colors**
            BorderColor = Color.FromArgb(160, 82, 45),
            ActiveBorderColor = Color.FromArgb(139, 69, 19),
            InactiveBorderColor = Color.FromArgb(160, 82, 45),
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(139, 69, 19),
            VisitedLinkColor = Color.FromArgb(160, 82, 45),
            HoverLinkColor = Color.FromArgb(205, 133, 63),
            LinkHoverColor = Color.FromArgb(205, 133, 63),
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(80, 60, 40),
            ToolTipForeColor = Color.White,

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(222, 184, 135),
            ScrollBarThumbColor = Color.FromArgb(160, 82, 45),
            ScrollBarTrackColor = Color.FromArgb(210, 180, 140),

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(205, 133, 63),
            StatusBarForeColor = Color.FromArgb(80, 60, 40),

            // **Tab Colors**
            TabBackColor = Color.FromArgb(245, 222, 179),
            TabForeColor = Color.FromArgb(80, 60, 40),
            ActiveTabBackColor = Color.FromArgb(222, 184, 135),
            ActiveTabForeColor = Color.FromArgb(80, 60, 40),

            // **Dialog Box Colors**
            DialogBackColor = Color.FromArgb(245, 222, 179),
            DialogForeColor = Color.FromArgb(80, 60, 40),
            DialogButtonBackColor = Color.FromArgb(160, 82, 45),
            DialogButtonForeColor = Color.White,

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(245, 222, 179),
            GradientEndColor = Color.FromArgb(222, 184, 135),
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(200, 170, 120), // Soft Beige

            SideMenuHoverBackColor = Color.FromArgb(222, 184, 135),
            SideMenuSelectedBackColor = Color.FromArgb(205, 133, 63),
            SideMenuForeColor = Color.FromArgb(80, 60, 40),
            SideMenuHoverForeColor = Color.FromArgb(80, 60, 40),
            SideMenuSelectedForeColor = Color.White,
            SideMenuBorderColor = Color.FromArgb(160, 82, 45),
            SideMenuIconColor = Color.FromArgb(80, 60, 40),
            SideMenuSelectedIconColor = Color.White,

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(210, 180, 140),
            TitleBarForeColor = Color.FromArgb(80, 60, 40),
            TitleBarHoverBackColor = Color.FromArgb(222, 184, 135),
            TitleBarHoverForeColor = Color.FromArgb(80, 60, 40),

            // **Dashboard Colors**
            DashboardBackColor = Color.FromArgb(245, 222, 179),
            DashboardCardBackColor = Color.FromArgb(230, 220, 190),
            DashboardCardHoverBackColor = Color.FromArgb(222, 184, 135),
            CardTitleForeColor = Color.FromArgb(80, 60, 40),
            CardTextForeColor = Color.FromArgb(100, 80, 60),

            // **Data Visualization (Charts)**
            ChartBackColor = Color.FromArgb(245, 222, 179),
            ChartLineColor = Color.FromArgb(139, 69, 19),
            ChartFillColor = Color.FromArgb(100, 139, 69, 19), // Semi-transparent Saddle Brown
            ChartAxisColor = Color.FromArgb(80, 60, 40),

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(80, 60, 40),
            SidebarSelectedIconColor = Color.White,
            SidebarTextColor = Color.FromArgb(80, 60, 40),
            SidebarSelectedTextColor = Color.White,

            // **Navigation Colors**
            NavigationBackColor = Color.FromArgb(245, 222, 179),
            NavigationForeColor = Color.FromArgb(80, 60, 40),
            NavigationHoverBackColor = Color.FromArgb(222, 184, 135),
            NavigationHoverForeColor = Color.FromArgb(80, 60, 40),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(139, 69, 19),        // Saddle Brown
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(205, 133, 63),   // Peru

            // **Font Properties**
            FontFamily = "Times New Roman",
            FontName = "Times New Roman",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(80, 60, 40),
            SecondaryTextColor = Color.FromArgb(100, 80, 60),
            AccentTextColor = Color.FromArgb(139, 69, 19),

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(80, 60, 40),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(80, 60, 40),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(80, 60, 40),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(80, 60, 40),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(80, 60, 40),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(80, 60, 40),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(100, 80, 60),
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(100, 80, 60),
            },
            BlockquoteBorderColor = Color.FromArgb(160, 82, 45),
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Courier New",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(80, 60, 40),
            },
            InlineCodeBackgroundColor = Color.FromArgb(245, 222, 179),
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Courier New",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(80, 60, 40),
            },
            CodeBlockBackgroundColor = Color.FromArgb(245, 222, 179),
            CodeBlockBorderColor = Color.FromArgb(160, 82, 45),
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(80, 60, 40),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(80, 60, 40),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(139, 69, 19),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(80, 60, 40),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(100, 80, 60),
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(80, 60, 40),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(80, 60, 40),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(80, 60, 40),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(80, 60, 40),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(80, 60, 40),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(80, 60, 40),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(80, 60, 40),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(80, 60, 40),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(80, 60, 40),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(100, 80, 60),
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(100, 80, 60),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(100, 80, 60),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(80, 60, 40),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(80, 60, 40),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(80, 60, 40),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(139, 69, 19),            // Saddle Brown
            SecondaryColor = Color.FromArgb(210, 180, 140),        // Tan
            AccentColor = Color.FromArgb(160, 82, 45),             // Sienna
            BackgroundColor = Color.FromArgb(245, 222, 179),       // Wheat
            SurfaceColor = Color.FromArgb(230, 220, 190),
            ErrorColor = Color.FromArgb(128, 0, 0),                // Maroon
            WarningColor = Color.FromArgb(184, 134, 11),           // Dark Goldenrod
            SuccessColor = Color.FromArgb(85, 107, 47),            // Dark Olive Green
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.FromArgb(80, 60, 40),

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 2,

            // **Imagery and Iconography**
            IconSet = "VintageIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(100, 0, 0, 0), // Semi-transparent black
            ShadowOpacity = 0.2f,

            // **Animation and Transitions**
            AnimationDurationShort = 200,  // in milliseconds
            AnimationDurationMedium = 400,
            AnimationDurationLong = 600,
            AnimationEasingFunction = "ease-in-out",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(139, 69, 19),

            // **Theme Variant**
            IsDarkTheme = false,
        };
        public static BeepTheme DefaultTheme => new BeepTheme
        {
            GridBackColor = Color.White,
            GridForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBackColor = Color.LightGray,
            GridHeaderForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBorderColor = Color.Silver,
            GridHeaderHoverBackColor = Color.Gainsboro,
            GridHeaderHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderSelectedBackColor = Color.FromArgb(0, 120, 215),
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.Gray,
            GridHeaderSelectedBorderColor = Color.FromArgb(0, 120, 215),
            GridRowHoverBackColor = Color.Gainsboro,
            GridRowHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridRowSelectedBackColor = Color.FromArgb(0, 120, 215),
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.Gray,
            GridRowSelectedBorderColor = Color.FromArgb(0, 120, 215),
            CardBackColor = Color.FromArgb(255, 69, 58), // Soft reddish tone
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),

            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(232, 17, 35),      // Close Button Red
            MaxButtonColor = Color.FromArgb(0, 120, 215),        // Maximize Button Blue
            MinButtonColor = Color.FromArgb(0, 120, 215),        // Minimize Button Blue
            TitleBarColor = Color.FromArgb(230, 230, 230),       // Light Gray
            TitleBarTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarHoverColor = Color.FromArgb(210, 210, 210),
            TitleBarHoverTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarHoverIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarActiveColor = Color.FromArgb(210, 210, 210),
            TitleBarActiveTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarActiveIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarInactiveColor = Color.FromArgb(230, 230, 230),
            TitleBarInactiveTextColor = Color.Gray,
            TitleBarInactiveIconColor = Color.Gray,
            TitleBarBorderColor = Color.FromArgb(200, 200, 200),
            TitleBarBorderHoverColor = Color.FromArgb(180, 180, 180),
            TitleBarBorderActiveColor = Color.FromArgb(180, 180, 180),
            TitleBarBorderInactiveColor = Color.FromArgb(200, 200, 200),

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(241, 112, 122),
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(232, 17, 35),
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(230, 230, 230),
            TitleBarCloseInactiveTextColor = Color.Gray,
            TitleBarCloseInactiveIconColor = Color.Gray,
            TitleBarCloseBorderColor = Color.FromArgb(200, 200, 200),
            TitleBarCloseBorderHoverColor = Color.FromArgb(180, 180, 180),
            TitleBarCloseBorderActiveColor = Color.FromArgb(180, 180, 180),
            TitleBarCloseBorderInactiveColor = Color.FromArgb(200, 200, 200),

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(204, 228, 247),
            TitleBarMaxHoverTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMaxHoverIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMaxActiveColor = Color.FromArgb(0, 120, 215),
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(230, 230, 230),
            TitleBarMaxInactiveTextColor = Color.Gray,
            TitleBarMaxInactiveIconColor = Color.Gray,
            TitleBarMaxBorderColor = Color.FromArgb(200, 200, 200),
            TitleBarMaxBorderHoverColor = Color.FromArgb(180, 180, 180),
            TitleBarMaxBorderActiveColor = Color.FromArgb(180, 180, 180),
            TitleBarMaxBorderInactiveColor = Color.FromArgb(200, 200, 200),

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(204, 228, 247),
            TitleBarMinHoverTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinHoverIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinActiveColor = Color.FromArgb(0, 120, 215),
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(230, 230, 230),
            TitleBarMinInactiveTextColor = Color.Gray,
            TitleBarMinInactiveIconColor = Color.Gray,
            TitleBarMinBorderColor = Color.FromArgb(200, 200, 200),
            TitleBarMinBorderHoverColor = Color.FromArgb(180, 180, 180),
            TitleBarMinBorderActiveColor = Color.FromArgb(180, 180, 180),
            TitleBarMinBorderInactiveColor = Color.FromArgb(200, 200, 200),

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(204, 228, 247),
            TitleBarMinimizeHoverTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinimizeHoverIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinimizeActiveColor = Color.FromArgb(0, 120, 215),
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(230, 230, 230),
            TitleBarMinimizeInactiveTextColor = Color.Gray,
            TitleBarMinimizeInactiveIconColor = Color.Gray,
            TitleBarMinimizeBorderColor = Color.FromArgb(200, 200, 200),
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(180, 180, 180),
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(180, 180, 180),
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(200, 200, 200),

            // **General Colors**
            TitleForColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarForColor = Color.FromArgb(255, 0, 0, 0),
            DescriptionForColor = Color.Gray,
            BeforeForColor = Color.FromArgb(0, 120, 215),     // Blue Accent
            LatestForColor = Color.FromArgb(0, 120, 215),
            BackColor = Color.White,


            // **Button Colors**
            ButtonBackColor = Color.FromArgb(240, 240, 240),
            ButtonForeColor = Color.FromArgb(255, 0, 0, 0),
            ButtonHoverBackColor = Color.FromArgb(230, 230, 230),
            ButtonHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            ButtonActiveBackColor = Color.FromArgb(0, 120, 215),
            ButtonActiveForeColor = Color.White,

            // **TextBox Colors**
            TextBoxBackColor = Color.White,
            TextBoxForeColor = Color.FromArgb(255, 0, 0, 0),


            // **Label Colors**
            LabelBackColor = Color.White,
            LabelForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(245, 245, 245), // Light Gray


            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(240, 240, 240),
            HeaderForeColor = Color.FromArgb(255, 0, 0, 0),
            GridLineColor = Color.FromArgb(200, 200, 200),
            RowBackColor = Color.White,
            RowForeColor = Color.FromArgb(255, 0, 0, 0),
            AltRowBackColor = Color.FromArgb(248, 248, 248),
            SelectedRowBackColor = Color.FromArgb(204, 228, 247),
            SelectedRowForeColor = Color.FromArgb(255, 0, 0, 0),

            // **ComboBox Colors**
            ComboBoxBackColor = Color.White,
            ComboBoxForeColor = Color.FromArgb(255, 0, 0, 0),


            // **CheckBox Colors**
            CheckBoxBackColor = Color.White,
            CheckBoxForeColor = Color.FromArgb(255, 0, 0, 0),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.White,
            RadioButtonForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Border Colors**
            BorderColor = Color.FromArgb(200, 200, 200),
            ActiveBorderColor = Color.FromArgb(0, 120, 215),
            InactiveBorderColor = Color.FromArgb(200, 200, 200),
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(0, 120, 215),
            VisitedLinkColor = Color.FromArgb(105, 105, 105),
            HoverLinkColor = Color.FromArgb(0, 120, 215),
            LinkHoverColor = Color.FromArgb(0, 120, 215),
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(255, 255, 225),
            ToolTipForeColor = Color.FromArgb(255, 0, 0, 0),

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(240, 240, 240),
            ScrollBarThumbColor = Color.FromArgb(200, 200, 200),
            ScrollBarTrackColor = Color.FromArgb(230, 230, 230),

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(240, 240, 240),
            StatusBarForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Tab Colors**
            TabBackColor = Color.FromArgb(240, 240, 240),
            TabForeColor = Color.FromArgb(255, 0, 0, 0),
            ActiveTabBackColor = Color.White,
            ActiveTabForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Dialog Box Colors**
            DialogBackColor = Color.White,
            DialogForeColor = Color.FromArgb(255, 0, 0, 0),
            DialogButtonBackColor = Color.FromArgb(240, 240, 240),
            DialogButtonForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Gradient Properties**
            GradientStartColor = Color.White,
            GradientEndColor = Color.FromArgb(230, 230, 230),
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(230, 230, 240), // Cool Light Gray

            SideMenuHoverBackColor = Color.FromArgb(230, 230, 230),
            SideMenuSelectedBackColor = Color.FromArgb(0, 120, 215),
            SideMenuForeColor = Color.FromArgb(255, 0, 0, 0),
            SideMenuHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            SideMenuSelectedForeColor = Color.White,
            SideMenuBorderColor = Color.FromArgb(200, 200, 200),
            SideMenuIconColor = Color.FromArgb(255, 0, 0, 0),
            SideMenuSelectedIconColor = Color.White,

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(230, 230, 230),
            TitleBarForeColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarHoverBackColor = Color.FromArgb(210, 210, 210),
            TitleBarHoverForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Dashboard Colors**
            DashboardBackColor = Color.White,
            DashboardCardBackColor = Color.WhiteSmoke,
            DashboardCardHoverBackColor = Color.FromArgb(248, 248, 248),
            CardTitleForeColor = Color.FromArgb(255, 0, 0, 0),
            CardTextForeColor = Color.Gray,

            // **Data Visualization (Charts)**
            ChartBackColor = Color.White,
            ChartLineColor = Color.FromArgb(0, 120, 215),
            ChartFillColor = Color.FromArgb(100, 0, 120, 215), // Semi-transparent Blue
            ChartAxisColor = Color.FromArgb(255, 0, 0, 0),

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(255, 0, 0, 0),
            SidebarSelectedIconColor = Color.White,
            SidebarTextColor = Color.FromArgb(255, 0, 0, 0),
            SidebarSelectedTextColor = Color.White,

            // **Navigation Colors**
            NavigationBackColor = Color.White,
            NavigationForeColor = Color.FromArgb(255, 0, 0, 0),
            NavigationHoverBackColor = Color.FromArgb(230, 230, 230),
            NavigationHoverForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(0, 120, 215),
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(204, 228, 247),

            // **Font Properties**
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(255, 0, 0, 0),
            SecondaryTextColor = Color.Gray,
            AccentTextColor = Color.FromArgb(0, 120, 215),

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.Gray,
            },
            BlockquoteBorderColor = Color.FromArgb(200, 200, 200),
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 120, 215),
            },
            InlineCodeBackgroundColor = Color.FromArgb(248, 248, 248),
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            CodeBlockBackgroundColor = Color.FromArgb(248, 248, 248),
            CodeBlockBorderColor = Color.FromArgb(200, 200, 200),
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.Gray,
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(0, 120, 215),          // Blue Accent
            SecondaryColor = Color.FromArgb(105, 105, 105),      // Dim Gray
            AccentColor = Color.FromArgb(0, 120, 215),           // Blue Accent
            BackgroundColor = Color.White,
            SurfaceColor = Color.FromArgb(248, 248, 248),
            ErrorColor = Color.FromArgb(232, 17, 35),            // Red
            WarningColor = Color.FromArgb(255, 185, 0),          // Yellow
            SuccessColor = Color.FromArgb(16, 124, 16),          // Green
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.FromArgb(255, 0, 0, 0),

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 3,

            // **Imagery and Iconography**
            IconSet = "DefaultIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(100, 0, 0, 0), // Semi-transparent black
            ShadowOpacity = 0.15f,

            // **Animation and Transitions**
            AnimationDurationShort = 150,  // in milliseconds
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(0, 120, 215),

            // **Theme Variant**
            IsDarkTheme = false,
        };
        public static BeepTheme GlassmorphismTheme => new BeepTheme
        {
            // ===== Grid Colors =====
            GridBackColor = Color.FromArgb(255, 255, 255, 255), // Opaque White
            GridForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBackColor = Color.FromArgb(255, 240, 240, 240), // Opaque Light Gray
            GridHeaderForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBorderColor = Color.Gray,
            GridHeaderHoverBackColor = Color.FromArgb(255, 240, 240, 240),
            GridHeaderHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderSelectedBackColor = Color.FromArgb(255, 0, 120, 215), // Opaque Blue
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.Gray,
            GridHeaderSelectedBorderColor = Color.FromArgb(0, 120, 215),
            GridRowHoverBackColor = Color.FromArgb(255, 240, 240, 240),
            GridRowHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridRowSelectedBackColor = Color.FromArgb(255, 0, 120, 215),
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.Gray,
            GridRowSelectedBorderColor = Color.FromArgb(0, 120, 215),

            // ===== Card Styles =====
            CardBackColor = Color.FromArgb(255, 255, 255), // Opaque White
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.DarkGray // Light Gray
            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.DimGray // Light Gray
            },

            // ===== UI Elements =====
            CloseButtonColor = Color.FromArgb(255, 0, 0),      // Fully opaque Red
            MaxButtonColor = Color.FromArgb(0, 255, 0),        // Fully opaque Green
            MinButtonColor = Color.FromArgb(0, 0, 255),        // Fully opaque Blue
            TitleBarColor = Color.FromArgb(255, 255, 255),
            TitleBarTextColor = Color.FromArgb(230, 230, 230),
            TitleBarIconColor = Color.FromArgb(230, 230, 230),
            TitleBarHoverColor = Color.FromArgb(255, 255, 255),
            TitleBarHoverTextColor = Color.FromArgb(230, 230, 230),
            TitleBarHoverIconColor = Color.FromArgb(230, 230, 230),
            TitleBarActiveColor = Color.FromArgb(255, 255, 255),
            TitleBarActiveTextColor = Color.FromArgb(230, 230, 230),
            TitleBarActiveIconColor = Color.FromArgb(230, 230, 230),
            TitleBarInactiveColor = Color.FromArgb(255, 255, 255),
            TitleBarInactiveTextColor = Color.FromArgb(200, 200, 200),
            TitleBarInactiveIconColor = Color.FromArgb(200, 200, 200),
            TitleBarBorderColor = Color.FromArgb(255, 255, 255),
            TitleBarBorderHoverColor = Color.FromArgb(255, 255, 255),
            TitleBarBorderActiveColor = Color.FromArgb(255, 255, 255),
            TitleBarBorderInactiveColor = Color.FromArgb(255, 255, 255),

            // ===== Title Bar – Close Button =====
            TitleBarCloseHoverColor = Color.FromArgb(255, 50, 50),
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(255, 0, 0),
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(255, 255, 255),
            TitleBarCloseInactiveTextColor = Color.FromArgb(200, 200, 200),
            TitleBarCloseInactiveIconColor = Color.FromArgb(200, 200, 200),
            TitleBarCloseBorderColor = Color.FromArgb(255, 255, 255),
            TitleBarCloseBorderHoverColor = Color.FromArgb(255, 255, 255),
            TitleBarCloseBorderActiveColor = Color.FromArgb(255, 255, 255),
            TitleBarCloseBorderInactiveColor = Color.FromArgb(255, 255, 255),

            // ===== Title Bar – Maximize Button =====
            TitleBarMaxHoverColor = Color.FromArgb(50, 255, 50),
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.White,
            TitleBarMaxActiveColor = Color.FromArgb(0, 255, 0),
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(255, 255, 255),
            TitleBarMaxInactiveTextColor = Color.FromArgb(200, 200, 200),
            TitleBarMaxInactiveIconColor = Color.FromArgb(200, 200, 200),
            TitleBarMaxBorderColor = Color.FromArgb(255, 255, 255),
            TitleBarMaxBorderHoverColor = Color.FromArgb(255, 255, 255),
            TitleBarMaxBorderActiveColor = Color.FromArgb(255, 255, 255),
            TitleBarMaxBorderInactiveColor = Color.FromArgb(255, 255, 255),

            // ===== Title Bar – Minimize Button =====
            TitleBarMinHoverColor = Color.FromArgb(50, 50, 255),
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.White,
            TitleBarMinActiveColor = Color.FromArgb(0, 0, 255),
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(255, 255, 255),
            TitleBarMinInactiveTextColor = Color.FromArgb(200, 200, 200),
            TitleBarMinInactiveIconColor = Color.FromArgb(200, 200, 200),
            TitleBarMinBorderColor = Color.FromArgb(255, 255, 255),
            TitleBarMinBorderHoverColor = Color.FromArgb(255, 255, 255),
            TitleBarMinBorderActiveColor = Color.FromArgb(255, 255, 255),
            TitleBarMinBorderInactiveColor = Color.FromArgb(255, 255, 255),

            // ===== Title Bar – Minimize Button (Alternative) =====
            TitleBarMinimizeHoverColor = Color.FromArgb(50, 50, 255),
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(0, 0, 255),
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(255, 255, 255),
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(200, 200, 200),
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(200, 200, 200),
            TitleBarMinimizeBorderColor = Color.FromArgb(255, 255, 255),
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(255, 255, 255),
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(255, 255, 255),
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(255, 255, 255),

            // ===== General Colors =====
            TitleForColor = Color.DarkSlateGray,
            TitleBarForColor = Color.FromArgb(230, 230, 230),
            DescriptionForColor = Color.FromArgb(220, 220, 220),
            BeforeForColor = Color.FromArgb(255, 0, 0, 0), // Opaque Black
            LatestForColor = Color.FromArgb(255, 255, 255), // Opaque White
            BackColor = Color.FromArgb(245, 245, 245),

            // ===== Button Colors =====
            ButtonBackColor = Color.FromArgb(255, 255, 255),
            ButtonForeColor = Color.FromArgb(30, 30, 30),
            ButtonHoverBackColor = Color.FromArgb(255, 255, 255),
            ButtonHoverForeColor = Color.FromArgb(30, 30, 30),
            ButtonActiveBackColor = Color.FromArgb(255, 255, 255),
            ButtonActiveForeColor = Color.FromArgb(30, 30, 30),

            // ===== TextBox Colors =====
            TextBoxBackColor = Color.FromArgb(255, 255, 255),
            TextBoxForeColor = Color.FromArgb(30, 30, 30),

            // ===== Label Colors =====
            LabelBackColor = Color.White, // Opaque White
            LabelForeColor = Color.FromArgb(30, 30, 30),

            // ===== Panel Colors =====
            PanelBackColor = Color.FromArgb(255, 255, 255),

            // ===== Grid Colors =====
            HeaderBackColor = Color.FromArgb(255, 255, 255),
            HeaderForeColor = Color.FromArgb(30, 30, 30),
            GridLineColor = Color.FromArgb(200, 200, 200),
            RowBackColor = Color.FromArgb(255, 255, 255),
            RowForeColor = Color.FromArgb(30, 30, 30),
            AltRowBackColor = Color.FromArgb(255, 255, 255),
            SelectedRowBackColor = Color.FromArgb(30, 144, 255),
            SelectedRowForeColor = Color.White,

            // ===== ComboBox Colors =====
            ComboBoxBackColor = Color.FromArgb(255, 255, 255),
            ComboBoxForeColor = Color.FromArgb(30, 30, 30),

            // ===== CheckBox Colors =====
            CheckBoxBackColor = Color.White,
            CheckBoxForeColor = Color.FromArgb(30, 30, 30),

            // ===== RadioButton Colors =====
            RadioButtonBackColor = Color.White,
            RadioButtonForeColor = Color.FromArgb(30, 30, 30),

            // ===== Border Colors =====
            BorderColor = Color.FromArgb(255, 255, 255),
            ActiveBorderColor = Color.FromArgb(30, 144, 255),
            InactiveBorderColor = Color.FromArgb(255, 255, 255),
            BorderSize = 1,

            // ===== Link Colors =====
            LinkColor = Color.FromArgb(30, 144, 255),         // Dodger Blue
            VisitedLinkColor = Color.FromArgb(100, 149, 237),   // Cornflower Blue
            HoverLinkColor = Color.FromArgb(65, 105, 225),      // Royal Blue
            LinkHoverColor = Color.FromArgb(65, 105, 225),
            LinkIsUnderline = true,

            // ===== ToolTip Colors =====
            ToolTipBackColor = Color.FromArgb(255, 255, 255),
            ToolTipForeColor = Color.FromArgb(30, 30, 30),

            // ===== ScrollBar Colors =====
            ScrollBarBackColor = Color.White,
            ScrollBarThumbColor = Color.FromArgb(200, 200, 200),
            ScrollBarTrackColor = Color.FromArgb(200, 200, 200),

            // ===== Status Bar Colors =====
            StatusBarBackColor = Color.FromArgb(255, 255, 255),
            StatusBarForeColor = Color.FromArgb(30, 30, 30),

            // ===== Tab Colors =====
            TabBackColor = Color.White,
            TabForeColor = Color.FromArgb(30, 30, 30),
            ActiveTabBackColor = Color.FromArgb(255, 255, 255),
            ActiveTabForeColor = Color.FromArgb(30, 30, 30),

            // ===== Dialog Box Colors =====
            DialogBackColor = Color.FromArgb(255, 255, 255),
            DialogForeColor = Color.FromArgb(30, 30, 30),
            DialogButtonBackColor = Color.FromArgb(30, 144, 255),
            DialogButtonForeColor = Color.White,

            // ===== Gradient Properties =====
            GradientStartColor = Color.FromArgb(255, 255, 255),
            GradientEndColor = Color.FromArgb(255, 255, 255),
            GradientDirection = LinearGradientMode.Vertical,

            // ===== Side Menu Colors =====
            SideMenuBackColor = Color.FromArgb(240, 248, 255), // Light Sky Blue Tint
            SideMenuHoverBackColor = Color.FromArgb(255, 255, 255),
            SideMenuSelectedBackColor = Color.FromArgb(255, 255, 255),
            SideMenuForeColor = Color.FromArgb(30, 30, 30),
            SideMenuHoverForeColor = Color.FromArgb(30, 30, 30),
            SideMenuSelectedForeColor = Color.FromArgb(30, 30, 30),
            SideMenuBorderColor = Color.FromArgb(255, 255, 255),
            SideMenuIconColor = Color.FromArgb(30, 30, 30),
            SideMenuSelectedIconColor = Color.FromArgb(30, 30, 30),

            // ===== Title Bar Colors =====
            TitleBarBackColor = Color.FromArgb(255, 255, 255),
            TitleBarForeColor = Color.FromArgb(30, 30, 30),
            TitleBarHoverBackColor = Color.FromArgb(255, 255, 255),
            TitleBarHoverForeColor = Color.FromArgb(30, 30, 30),

            // ===== Dashboard Colors =====
            DashboardBackColor = Color.White,
            DashboardCardBackColor = Color.FromArgb(255, 255, 255),
            DashboardCardHoverBackColor = Color.FromArgb(255, 255, 255),
            CardTitleForeColor = Color.FromArgb(30, 30, 30),
            CardTextForeColor = Color.FromArgb(50, 50, 50),

            // ===== Data Visualization (Charts) =====
            ChartBackColor = Color.White,
            ChartLineColor = Color.FromArgb(30, 144, 255),
            ChartFillColor = Color.FromArgb(30, 144, 255),
            ChartAxisColor = Color.FromArgb(200, 200, 200),

            // ===== Sidebar and Menu Colors =====
            SidebarIconColor = Color.FromArgb(30, 30, 30),
            SidebarSelectedIconColor = Color.FromArgb(30, 30, 30),
            SidebarTextColor = Color.FromArgb(30, 30, 30),
            SidebarSelectedTextColor = Color.FromArgb(30, 30, 30),

            // ===== Navigation Colors =====
            NavigationBackColor = Color.White,
            NavigationForeColor = Color.FromArgb(30, 30, 30),
            NavigationHoverBackColor = Color.FromArgb(255, 255, 255),
            NavigationHoverForeColor = Color.FromArgb(30, 30, 30),

            // ===== Badge and Highlight Colors =====
            BadgeBackColor = Color.FromArgb(255, 69, 0),      // Orange Red
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(255, 215, 0), // Gold

            // ===== Font Properties =====
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // ===== Font Sizes =====
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // ===== Font Styles =====
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // ===== Text Colors =====
            PrimaryTextColor = Color.FromArgb(30, 30, 30),
            SecondaryTextColor = Color.FromArgb(50, 50, 50),
            AccentTextColor = Color.FromArgb(30, 144, 255),

            // ===== Typography Styles =====
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(30, 30, 30),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(30, 30, 30),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(30, 30, 30),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(30, 30, 30),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(30, 30, 30),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(30, 30, 30),
            },

            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(50, 50, 50),
            },

            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(50, 50, 50),
            },
            BlockquoteBorderColor = Color.FromArgb(200, 200, 200),
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(30, 30, 30),
            },
            InlineCodeBackgroundColor = Color.FromArgb(255, 255, 255),
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(30, 30, 30),
            },
            CodeBlockBackgroundColor = Color.FromArgb(255, 255, 255),
            CodeBlockBorderColor = Color.FromArgb(200, 200, 200),
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(50, 50, 50),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(50, 50, 50),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(100, 100, 100),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(30, 30, 30),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(50, 50, 50),
            },

            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(30, 30, 30),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(30, 30, 30),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(30, 30, 30),
            },

            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(30, 30, 30),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(30, 30, 30),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(30, 30, 30),
            },

            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(30, 30, 30),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(30, 30, 30),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(30, 30, 30),
            },

            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(50, 50, 50),
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(50, 50, 50),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(50, 50, 50),
            },

            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(30, 30, 30),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(30, 30, 30),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(30, 30, 30),
            },

            // ===== Color Palette =====
            PrimaryColor = Color.FromArgb(30, 144, 255),      // Dodger Blue
            SecondaryColor = Color.FromArgb(255, 255, 255),     // White
            AccentColor = Color.FromArgb(255, 69, 0),           // Orange Red
            BackgroundColor = Color.FromArgb(245, 245, 245),    // Light Gray
            SurfaceColor = Color.FromArgb(255, 255, 255),       // White
            ErrorColor = Color.FromArgb(255, 0, 0),             // Red
            WarningColor = Color.FromArgb(255, 165, 0),         // Orange
            SuccessColor = Color.FromArgb(0, 255, 0),           // Green
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.FromArgb(30, 30, 30),

            // ===== Spacing and Layout =====
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 10,

            // ===== Imagery and Iconography =====
            IconSet = "GlassIcons",
            ApplyThemeToIcons = true,

            // ===== Effects and Decorations =====
            ShadowColor = Color.FromArgb(255, 0, 0, 0), // Opaque Black
            ShadowOpacity = 1.0f, // Fully opaque shadow

            // ===== Animation and Transitions =====
            AnimationDurationShort = 150,
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",

            // ===== Accessibility =====
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(30, 144, 255), // Dodger Blue

            // ===== Theme Variant =====
            IsDarkTheme = false
        };
        public static BeepTheme FlatDesignTheme => new BeepTheme
        {
            GridBackColor = Color.White,
            GridForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBackColor = Color.FromArgb(0, 120, 215),
            GridHeaderForeColor = Color.White,
            GridHeaderBorderColor = Color.Gray,
            GridHeaderHoverBackColor = Color.FromArgb(28, 151, 234),
            GridHeaderHoverForeColor = Color.White,
            GridHeaderSelectedBackColor = Color.FromArgb(0, 100, 200),
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.Gray,
            GridHeaderSelectedBorderColor = Color.FromArgb(0, 100, 200),
            GridRowHoverBackColor = Color.LightGray,
            GridRowHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridRowSelectedBackColor = Color.FromArgb(0, 120, 215),
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.Gray,
            GridRowSelectedBorderColor = Color.FromArgb(0, 120, 215),
            CardBackColor = Color.FromArgb(250, 250, 250),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.CadetBlue,
            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(44, 62, 80), // Midnight Blue
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(231, 76, 60),    // Alizarin Red
            MaxButtonColor = Color.FromArgb(46, 204, 113),     // Emerald Green
            MinButtonColor = Color.FromArgb(241, 196, 15),     // Sun Flower Yellow
            TitleBarColor = Color.FromArgb(41, 128, 185),      // Belize Hole Blue
            TitleBarTextColor = Color.White,
            TitleBarIconColor = Color.White,
            TitleBarHoverColor = Color.FromArgb(52, 152, 219), // Peter River Blue
            TitleBarHoverTextColor = Color.White,
            TitleBarHoverIconColor = Color.White,
            TitleBarActiveColor = Color.FromArgb(41, 128, 185),
            TitleBarActiveTextColor = Color.White,
            TitleBarActiveIconColor = Color.White,
            TitleBarInactiveColor = Color.FromArgb(127, 140, 141), // Asbestos Gray
            TitleBarInactiveTextColor = Color.White,
            TitleBarInactiveIconColor = Color.White,
            TitleBarBorderColor = Color.FromArgb(127, 140, 141),
            TitleBarBorderHoverColor = Color.FromArgb(149, 165, 166),
            TitleBarBorderActiveColor = Color.FromArgb(149, 165, 166),
            TitleBarBorderInactiveColor = Color.FromArgb(127, 140, 141),

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(192, 57, 43), // Pomegranate Red
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(231, 76, 60),
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(127, 140, 141),
            TitleBarCloseInactiveTextColor = Color.White,
            TitleBarCloseInactiveIconColor = Color.White,
            TitleBarCloseBorderColor = Color.FromArgb(127, 140, 141),
            TitleBarCloseBorderHoverColor = Color.FromArgb(149, 165, 166),
            TitleBarCloseBorderActiveColor = Color.FromArgb(149, 165, 166),
            TitleBarCloseBorderInactiveColor = Color.FromArgb(127, 140, 141),

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(39, 174, 96), // Nephritis Green
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.White,
            TitleBarMaxActiveColor = Color.FromArgb(46, 204, 113),
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(127, 140, 141),
            TitleBarMaxInactiveTextColor = Color.White,
            TitleBarMaxInactiveIconColor = Color.White,
            TitleBarMaxBorderColor = Color.FromArgb(127, 140, 141),
            TitleBarMaxBorderHoverColor = Color.FromArgb(149, 165, 166),
            TitleBarMaxBorderActiveColor = Color.FromArgb(149, 165, 166),
            TitleBarMaxBorderInactiveColor = Color.FromArgb(127, 140, 141),

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(243, 156, 18), // Orange
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.White,
            TitleBarMinActiveColor = Color.FromArgb(241, 196, 15),
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(127, 140, 141),
            TitleBarMinInactiveTextColor = Color.White,
            TitleBarMinInactiveIconColor = Color.White,
            TitleBarMinBorderColor = Color.FromArgb(127, 140, 141),
            TitleBarMinBorderHoverColor = Color.FromArgb(149, 165, 166),
            TitleBarMinBorderActiveColor = Color.FromArgb(149, 165, 166),
            TitleBarMinBorderInactiveColor = Color.FromArgb(127, 140, 141),

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(243, 156, 18),
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(241, 196, 15),
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(127, 140, 141),
            TitleBarMinimizeInactiveTextColor = Color.White,
            TitleBarMinimizeInactiveIconColor = Color.White,
            TitleBarMinimizeBorderColor = Color.FromArgb(127, 140, 141),
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(149, 165, 166),
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(149, 165, 166),
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(127, 140, 141),

            // **General Colors**
            TitleForColor = Color.FromArgb(44, 62, 80), // Midnight Blue
            TitleBarForColor = Color.White,
            DescriptionForColor = Color.FromArgb(127, 140, 141), // Asbestos Gray
            BeforeForColor = Color.FromArgb(52, 152, 219),       // Peter River Blue
            LatestForColor = Color.FromArgb(41, 128, 185),       // Belize Hole Blue
            BackColor = Color.White,

            // **Button Colors**
            ButtonBackColor = Color.LightSkyBlue,
            ButtonForeColor = Color.White,
            ButtonHoverBackColor = Color.FromArgb(41, 128, 185),
            ButtonHoverForeColor = Color.White,
            ButtonActiveBackColor = Color.FromArgb(41, 128, 185),
            ButtonActiveForeColor = Color.White,

            // **TextBox Colors**
            TextBoxBackColor = Color.FromArgb(236, 240, 241), // Clouds
            TextBoxForeColor = Color.FromArgb(44, 62, 80),

            // **Label Colors**
            LabelBackColor = Color.White,
            LabelForeColor = Color.FromArgb(44, 62, 80),

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(245, 245, 245), // Very Light Gray

            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(236, 240, 241),
            HeaderForeColor = Color.FromArgb(44, 62, 80),
            GridLineColor = Color.FromArgb(189, 195, 199),
            RowBackColor = Color.White,
            RowForeColor = Color.FromArgb(44, 62, 80),
            AltRowBackColor = Color.FromArgb(236, 240, 241),
            SelectedRowBackColor = Color.FromArgb(52, 152, 219),
            SelectedRowForeColor = Color.White,

            // **ComboBox Colors**
            ComboBoxBackColor = Color.FromArgb(236, 240, 241),
            ComboBoxForeColor = Color.FromArgb(44, 62, 80),

            // **CheckBox Colors**
            CheckBoxBackColor = Color.White,
            CheckBoxForeColor = Color.FromArgb(44, 62, 80),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.White,
            RadioButtonForeColor = Color.FromArgb(44, 62, 80),

            // **Border Colors**
            BorderColor = Color.FromArgb(189, 195, 199),
            ActiveBorderColor = Color.FromArgb(52, 152, 219),
            InactiveBorderColor = Color.FromArgb(189, 195, 199),
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(41, 128, 185),
            VisitedLinkColor = Color.FromArgb(127, 140, 141),
            HoverLinkColor = Color.FromArgb(52, 152, 219),
            LinkHoverColor = Color.FromArgb(52, 152, 219),
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(236, 240, 241),
            ToolTipForeColor = Color.FromArgb(44, 62, 80),

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(236, 240, 241),
            ScrollBarThumbColor = Color.FromArgb(189, 195, 199),
            ScrollBarTrackColor = Color.FromArgb(236, 240, 241),

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(236, 240, 241),
            StatusBarForeColor = Color.FromArgb(44, 62, 80),

            // **Tab Colors**
            TabBackColor = Color.FromArgb(236, 240, 241),
            TabForeColor = Color.FromArgb(44, 62, 80),
            ActiveTabBackColor = Color.White,
            ActiveTabForeColor = Color.FromArgb(44, 62, 80),

            // **Dialog Box Colors**
            DialogBackColor = Color.White,
            DialogForeColor = Color.FromArgb(44, 62, 80),
            DialogButtonBackColor = Color.FromArgb(52, 152, 219),
            DialogButtonForeColor = Color.White,

            // **Gradient Properties**
            GradientStartColor = Color.White,
            GradientEndColor = Color.White,
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(236, 240, 241), // Clouds (Light Gray)
            SideMenuHoverBackColor = Color.FromArgb(189, 195, 199),
            SideMenuSelectedBackColor = Color.FromArgb(52, 152, 219),
            SideMenuForeColor = Color.CadetBlue,
            SideMenuHoverForeColor = Color.FromArgb(44, 62, 80),
            SideMenuSelectedForeColor = Color.White,
            SideMenuBorderColor = Color.FromArgb(189, 195, 199),
            SideMenuIconColor = Color.FromArgb(44, 62, 80),
            SideMenuSelectedIconColor = Color.White,

            // **Title Bar Colors**
            TitleBarBackColor = Color.LightGray,
            TitleBarForeColor = Color.White,
            TitleBarHoverBackColor = Color.FromArgb(52, 152, 219),
            TitleBarHoverForeColor = Color.White,

            // **Dashboard Colors**
            DashboardBackColor = Color.White,
            DashboardCardBackColor = Color.FromArgb(236, 240, 241),
            DashboardCardHoverBackColor = Color.FromArgb(189, 195, 199),
            CardTitleForeColor = Color.FromArgb(44, 62, 80),
            CardTextForeColor = Color.FromArgb(127, 140, 141),

            // **Data Visualization (Charts)**
            ChartBackColor = Color.White,
            ChartLineColor = Color.FromArgb(41, 128, 185),
            // Updated ChartFillColor to be fully opaque instead of semi-transparent
            ChartFillColor = Color.FromArgb(41, 128, 185),
            ChartAxisColor = Color.FromArgb(44, 62, 80),

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(44, 62, 80),
            SidebarSelectedIconColor = Color.White,
            SidebarTextColor = Color.FromArgb(44, 62, 80),
            SidebarSelectedTextColor = Color.White,

            // **Navigation Colors**
            NavigationBackColor = Color.White,
            NavigationForeColor = Color.FromArgb(44, 62, 80),
            NavigationHoverBackColor = Color.FromArgb(236, 240, 241),
            NavigationHoverForeColor = Color.FromArgb(44, 62, 80),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(231, 76, 60), // Alizarin Red
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(52, 152, 219), // Peter River Blue

            // **Font Properties**
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(44, 62, 80),
            SecondaryTextColor = Color.FromArgb(127, 140, 141),
            AccentTextColor = Color.FromArgb(41, 128, 185),

            // **Typography Styles**
            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(44, 62, 80),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(44, 62, 80),
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(127, 140, 141),
            },
            BlockquoteBorderColor = Color.FromArgb(189, 195, 199),
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,
            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(41, 128, 185),
            },
            InlineCodeBackgroundColor = Color.FromArgb(236, 240, 241),
            InlineCodePadding = 4f,
            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            CodeBlockBackgroundColor = Color.FromArgb(236, 240, 241),
            CodeBlockBorderColor = Color.FromArgb(189, 195, 199),
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,
            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,
            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(127, 140, 141),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(44, 62, 80),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(44, 62, 80),
            },
        };
        public static BeepTheme CyberpunkNeonTheme => new BeepTheme
        {
            GridBackColor = Color.FromArgb(255, 0, 0, 0),
            GridForeColor = Color.Lime,
            GridHeaderBackColor = Color.Fuchsia,
            GridHeaderForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBorderColor = Color.Yellow,
            GridHeaderHoverBackColor = Color.FromArgb(255, 20, 147), // Deep Pink
            GridHeaderHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderSelectedBackColor = Color.Cyan,
            GridHeaderSelectedForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderHoverBorderColor = Color.Yellow,
            GridHeaderSelectedBorderColor = Color.Fuchsia,
            GridRowHoverBackColor = Color.FromArgb(0, 255, 255), // Cyan
            GridRowHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridRowSelectedBackColor = Color.Magenta,
            GridRowSelectedForeColor = Color.FromArgb(255, 0, 0, 0),
            GridRowHoverBorderColor = Color.Yellow,
            GridRowSelectedBorderColor = Color.Magenta,
            CardBackColor = Color.FromArgb(0, 255, 255),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 20f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.Blue,         // Neon Cyan

            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 85, 255),        // Neon Magenta
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(255, 0, 0),        // Bright Red
            MaxButtonColor = Color.FromArgb(0, 255, 0),          // Bright Green
            MinButtonColor = Color.FromArgb(0, 0, 255),          // Bright Blue
            TitleBarColor = Color.FromArgb(15, 15, 15),          // Very Dark Gray
            TitleBarTextColor = Color.FromArgb(0, 255, 255),     // Neon Cyan
            TitleBarIconColor = Color.FromArgb(0, 255, 255),
            TitleBarHoverColor = Color.FromArgb(25, 25, 25),
            TitleBarHoverTextColor = Color.FromArgb(0, 255, 255),
            TitleBarHoverIconColor = Color.FromArgb(0, 255, 255),
            TitleBarActiveColor = Color.FromArgb(25, 25, 25),
            TitleBarActiveTextColor = Color.FromArgb(0, 255, 255),
            TitleBarActiveIconColor = Color.FromArgb(0, 255, 255),
            TitleBarInactiveColor = Color.FromArgb(15, 15, 15),
            TitleBarInactiveTextColor = Color.FromArgb(100, 100, 100),
            TitleBarInactiveIconColor = Color.FromArgb(100, 100, 100),
            TitleBarBorderColor = Color.FromArgb(0, 255, 255),
            TitleBarBorderHoverColor = Color.FromArgb(0, 255, 255),
            TitleBarBorderActiveColor = Color.FromArgb(0, 255, 255),
            TitleBarBorderInactiveColor = Color.FromArgb(100, 100, 100),

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(255, 50, 50),
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(255, 0, 0),
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(15, 15, 15),
            TitleBarCloseInactiveTextColor = Color.FromArgb(100, 100, 100),
            TitleBarCloseInactiveIconColor = Color.FromArgb(100, 100, 100),
            TitleBarCloseBorderColor = Color.FromArgb(255, 0, 0),
            TitleBarCloseBorderHoverColor = Color.FromArgb(255, 0, 0),
            TitleBarCloseBorderActiveColor = Color.FromArgb(255, 0, 0),
            TitleBarCloseBorderInactiveColor = Color.FromArgb(100, 100, 100),

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(50, 255, 50),
            TitleBarMaxHoverTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMaxHoverIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMaxActiveColor = Color.FromArgb(0, 255, 0),
            TitleBarMaxActiveTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMaxActiveIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMaxInactiveColor = Color.FromArgb(15, 15, 15),
            TitleBarMaxInactiveTextColor = Color.FromArgb(100, 100, 100),
            TitleBarMaxInactiveIconColor = Color.FromArgb(100, 100, 100),
            TitleBarMaxBorderColor = Color.FromArgb(0, 255, 0),
            TitleBarMaxBorderHoverColor = Color.FromArgb(0, 255, 0),
            TitleBarMaxBorderActiveColor = Color.FromArgb(0, 255, 0),
            TitleBarMaxBorderInactiveColor = Color.FromArgb(100, 100, 100),

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(50, 50, 255),
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.White,
            TitleBarMinActiveColor = Color.FromArgb(0, 0, 255),
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(15, 15, 15),
            TitleBarMinInactiveTextColor = Color.FromArgb(100, 100, 100),
            TitleBarMinInactiveIconColor = Color.FromArgb(100, 100, 100),
            TitleBarMinBorderColor = Color.FromArgb(0, 0, 255),
            TitleBarMinBorderHoverColor = Color.FromArgb(0, 0, 255),
            TitleBarMinBorderActiveColor = Color.FromArgb(0, 0, 255),
            TitleBarMinBorderInactiveColor = Color.FromArgb(100, 100, 100),

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(50, 50, 255),
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(0, 0, 255),
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(15, 15, 15),
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(100, 100, 100),
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(100, 100, 100),
            TitleBarMinimizeBorderColor = Color.FromArgb(0, 0, 255),
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(0, 0, 255),
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(0, 0, 255),
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(100, 100, 100),

            // **General Colors**
            TitleForColor = Color.FromArgb(0, 255, 255),          // Neon Cyan
            TitleBarForColor = Color.FromArgb(0, 255, 255),
            DescriptionForColor = Color.FromArgb(255, 85, 255),   // Neon Magenta
            BeforeForColor = Color.FromArgb(0, 255, 255),
            LatestForColor = Color.FromArgb(255, 85, 255),
            BackColor = Color.FromArgb(20, 20, 20),               // Dark Background

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(40, 40, 40),
            ButtonForeColor = Color.FromArgb(0, 255, 255),
            ButtonHoverBackColor = Color.FromArgb(60, 60, 60),
            ButtonHoverForeColor = Color.FromArgb(0, 255, 255),
            ButtonActiveBackColor = Color.FromArgb(0, 255, 255),
            ButtonActiveForeColor = Color.FromArgb(255, 0, 0, 0),

            // **TextBox Colors**
            TextBoxBackColor = Color.Gray,
            TextBoxForeColor = Color.FromArgb(0, 255, 255),


            // **Label Colors**
            LabelBackColor = Color.FromArgb(20, 20, 20),
            LabelForeColor = Color.White,

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(35, 35, 35), // Slightly lighter dark gray


            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(30, 30, 30),
            HeaderForeColor = Color.FromArgb(0, 255, 255),
            GridLineColor = Color.FromArgb(0, 255, 255),
            RowBackColor = Color.FromArgb(20, 20, 20),
            RowForeColor = Color.FromArgb(0, 255, 255),
            AltRowBackColor = Color.FromArgb(25, 25, 25),
            SelectedRowBackColor = Color.FromArgb(0, 255, 255),
            SelectedRowForeColor = Color.FromArgb(255, 0, 0, 0),

            // **ComboBox Colors**
            ComboBoxBackColor = Color.FromArgb(30, 30, 30),
            ComboBoxForeColor = Color.FromArgb(0, 255, 255),


            // **CheckBox Colors**
            CheckBoxBackColor = Color.FromArgb(20, 20, 20),
            CheckBoxForeColor = Color.FromArgb(0, 255, 255),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.FromArgb(20, 20, 20),
            RadioButtonForeColor = Color.FromArgb(0, 255, 255),

            // **Border Colors**
            BorderColor = Color.FromArgb(0, 255, 255),
            ActiveBorderColor = Color.FromArgb(255, 85, 255),
            InactiveBorderColor = Color.FromArgb(100, 100, 100),
            BorderSize = 2,

            // **Link Colors**
            LinkColor = Color.FromArgb(0, 255, 255),
            VisitedLinkColor = Color.FromArgb(255, 85, 255),
            HoverLinkColor = Color.FromArgb(0, 255, 255),
            LinkHoverColor = Color.FromArgb(0, 255, 255),
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(30, 30, 30),
            ToolTipForeColor = Color.FromArgb(0, 255, 255),

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(25, 25, 25),
            ScrollBarThumbColor = Color.FromArgb(0, 255, 255),
            ScrollBarTrackColor = Color.FromArgb(40, 40, 40),

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(25, 25, 25),
            StatusBarForeColor = Color.FromArgb(0, 255, 255),

            // **Tab Colors**
            TabBackColor = Color.FromArgb(30, 30, 30),
            TabForeColor = Color.FromArgb(0, 255, 255),
            ActiveTabBackColor = Color.FromArgb(20, 20, 20),
            ActiveTabForeColor = Color.FromArgb(0, 255, 255),

            // **Dialog Box Colors**
            DialogBackColor = Color.FromArgb(20, 20, 20),
            DialogForeColor = Color.FromArgb(0, 255, 255),
            DialogButtonBackColor = Color.FromArgb(0, 255, 255),
            DialogButtonForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(20, 20, 20),
            GradientEndColor = Color.FromArgb(30, 30, 30),
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(40, 40, 40), // Richer dark gray for Side Menu

            SideMenuHoverBackColor = Color.FromArgb(30, 30, 30),
            SideMenuSelectedBackColor = Color.FromArgb(0, 255, 255),
            SideMenuForeColor = Color.FromArgb(0, 255, 255),
            SideMenuHoverForeColor = Color.FromArgb(0, 255, 255),
            SideMenuSelectedForeColor = Color.FromArgb(255, 0, 0, 0),
            SideMenuBorderColor = Color.FromArgb(0, 255, 255),
            SideMenuIconColor = Color.FromArgb(0, 255, 255),
            SideMenuSelectedIconColor = Color.FromArgb(255, 0, 0, 0),

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(15, 15, 15),
            TitleBarForeColor = Color.FromArgb(0, 255, 255),
            TitleBarHoverBackColor = Color.FromArgb(25, 25, 25),
            TitleBarHoverForeColor = Color.FromArgb(0, 255, 255),

            // **Dashboard Colors**
            DashboardBackColor = Color.FromArgb(20, 20, 20),
            DashboardCardBackColor = Color.FromArgb(25, 25, 25),
            DashboardCardHoverBackColor = Color.FromArgb(30, 30, 30),
            CardTitleForeColor = Color.FromArgb(0, 255, 255),
            CardTextForeColor = Color.FromArgb(255, 85, 255),

            // **Data Visualization (Charts)**
            ChartBackColor = Color.FromArgb(20, 20, 20),
            ChartLineColor = Color.FromArgb(0, 255, 255),
            ChartFillColor = Color.FromArgb(100, 0, 255, 255), // Semi-transparent Neon Cyan
            ChartAxisColor = Color.FromArgb(0, 255, 255),

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(0, 255, 255),
            SidebarSelectedIconColor = Color.FromArgb(255, 0, 0, 0),
            SidebarTextColor = Color.FromArgb(0, 255, 255),
            SidebarSelectedTextColor = Color.FromArgb(255, 0, 0, 0),

            // **Navigation Colors**
            NavigationBackColor = Color.FromArgb(25, 25, 25),
            NavigationForeColor = Color.FromArgb(0, 255, 255),
            NavigationHoverBackColor = Color.FromArgb(30, 30, 30),
            NavigationHoverForeColor = Color.FromArgb(0, 255, 255),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(255, 85, 255),        // Neon Magenta
            BadgeForeColor = Color.FromArgb(255, 0, 0, 0),
            HighlightBackColor = Color.FromArgb(0, 255, 255),     // Neon Cyan

            // **Font Properties**
            FontFamily = "Consolas",
            FontName = "Consolas",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(0, 255, 255),
            SecondaryTextColor = Color.FromArgb(255, 85, 255),
            AccentTextColor = Color.FromArgb(255, 255, 0),        // Neon Yellow

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "OCR A Extended",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 1f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "OCR A Extended",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0.8f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(255, 85, 255),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "OCR A Extended",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0.6f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(255, 255, 0),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0.4f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0.2f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 85, 255),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 255, 0),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(0, 255, 255),
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 85, 255),
            },
            BlockquoteBorderColor = Color.FromArgb(0, 255, 255),
            BlockquoteBorderWidth = 2f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 255, 0),
            },
            InlineCodeBackgroundColor = Color.FromArgb(30, 30, 30),
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            CodeBlockBackgroundColor = Color.FromArgb(25, 25, 25),
            CodeBlockBorderColor = Color.FromArgb(0, 255, 255),
            CodeBlockBorderWidth = 2f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(100, 100, 100),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 0),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 85, 255),
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "OCR A Extended",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "OCR A Extended",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 85, 255),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "OCR A Extended",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 0),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 85, 255),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 0),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(255, 85, 255),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(255, 255, 0),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 255, 255),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(0, 255, 255),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 0),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 85, 255),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(0, 255, 255),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(0, 255, 255),            // Neon Cyan
            SecondaryColor = Color.FromArgb(255, 85, 255),         // Neon Magenta
            AccentColor = Color.FromArgb(255, 255, 0),             // Neon Yellow
            BackgroundColor = Color.FromArgb(20, 20, 20),          // Dark Background
            SurfaceColor = Color.FromArgb(30, 30, 30),
            ErrorColor = Color.FromArgb(255, 0, 0),                // Bright Red
            WarningColor = Color.FromArgb(255, 255, 0),            // Neon Yellow
            SuccessColor = Color.FromArgb(0, 255, 0),              // Bright Green
            OnPrimaryColor = Color.FromArgb(255, 0, 0, 0),
            OnBackgroundColor = Color.FromArgb(0, 255, 255),

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 0,

            // **Imagery and Iconography**
            IconSet = "CyberpunkIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(0, 255, 255), // Neon Cyan Shadow
            ShadowOpacity = 0.5f,

            // **Animation and Transitions**
            AnimationDurationShort = 100,  // in milliseconds
            AnimationDurationMedium = 200,
            AnimationDurationLong = 300,
            AnimationEasingFunction = "linear",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(255, 85, 255),

            // **Theme Variant**
            IsDarkTheme = true,
        };
        public static BeepTheme GradientBurstTheme => new BeepTheme
        {
            GridBackColor = Color.FromArgb(255, 255, 240), // Ivory
            GridForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBackColor = Color.FromArgb(255, 182, 193), // Light Pink
            GridHeaderForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBorderColor = Color.Purple,
            GridHeaderHoverBackColor = Color.FromArgb(144, 238, 144), // Light Green
            GridHeaderHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderSelectedBackColor = Color.FromArgb(0, 191, 255), // Deep Sky Blue
            GridHeaderSelectedForeColor = Color.White,
            GridHeaderHoverBorderColor = Color.Purple,
            GridHeaderSelectedBorderColor = Color.Orange,
            GridRowHoverBackColor = Color.FromArgb(255, 222, 173), // Navajo White
            GridRowHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridRowSelectedBackColor = Color.FromArgb(240, 128, 128), // Light Coral
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.Purple,
            GridRowSelectedBorderColor = Color.Orange,
            CardBackColor = Color.FromArgb(255, 105, 180),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,

            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.White,
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(255, 255, 69, 0),       // Orange Red
            MaxButtonColor = Color.FromArgb(255, 60, 179, 113),       // Medium Sea Green
            MinButtonColor = Color.FromArgb(255, 30, 144, 255),       // Dodger Blue
            TitleBarColor = Color.FromArgb(255, 75, 0, 130),          // Indigo
            TitleBarTextColor = Color.White,
            TitleBarIconColor = Color.White,
            TitleBarHoverColor = Color.FromArgb(255, 138, 43, 226),   // Blue Violet
            TitleBarHoverTextColor = Color.White,
            TitleBarHoverIconColor = Color.White,
            TitleBarActiveColor = Color.FromArgb(255, 75, 0, 130),
            TitleBarActiveTextColor = Color.White,
            TitleBarActiveIconColor = Color.White,
            TitleBarInactiveColor = Color.FromArgb(255, 123, 104, 238), // Medium Slate Blue
            TitleBarInactiveTextColor = Color.White,
            TitleBarInactiveIconColor = Color.White,
            TitleBarBorderColor = Color.FromArgb(255, 138, 43, 226),
            TitleBarBorderHoverColor = Color.FromArgb(255, 147, 112, 219),
            TitleBarBorderActiveColor = Color.FromArgb(255, 147, 112, 219),
            TitleBarBorderInactiveColor = Color.FromArgb(255, 123, 104, 238),

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(255, 220, 20, 60), // Crimson
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(255, 255, 69, 0), // Orange Red
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(255, 123, 104, 238),
            TitleBarCloseInactiveTextColor = Color.White,
            TitleBarCloseInactiveIconColor = Color.White,
            TitleBarCloseBorderColor = Color.FromArgb(255, 138, 43, 226),
            TitleBarCloseBorderHoverColor = Color.FromArgb(255, 147, 112, 219),
            TitleBarCloseBorderActiveColor = Color.FromArgb(255, 147, 112, 219),
            TitleBarCloseBorderInactiveColor = Color.FromArgb(255, 123, 104, 238),

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(255, 46, 139, 87), // Sea Green
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.White,
            TitleBarMaxActiveColor = Color.FromArgb(255, 60, 179, 113), // Medium Sea Green
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(255, 123, 104, 238),
            TitleBarMaxInactiveTextColor = Color.White,
            TitleBarMaxInactiveIconColor = Color.White,
            TitleBarMaxBorderColor = Color.FromArgb(255, 138, 43, 226),
            TitleBarMaxBorderHoverColor = Color.FromArgb(255, 147, 112, 219),
            TitleBarMaxBorderActiveColor = Color.FromArgb(255, 147, 112, 219),
            TitleBarMaxBorderInactiveColor = Color.FromArgb(255, 123, 104, 238),

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(255, 65, 105, 225), // Royal Blue
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.White,
            TitleBarMinActiveColor = Color.FromArgb(255, 30, 144, 255), // Dodger Blue
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(255, 123, 104, 238),
            TitleBarMinInactiveTextColor = Color.White,
            TitleBarMinInactiveIconColor = Color.White,
            TitleBarMinBorderColor = Color.FromArgb(255, 138, 43, 226),
            TitleBarMinBorderHoverColor = Color.FromArgb(255, 147, 112, 219),
            TitleBarMinBorderActiveColor = Color.FromArgb(255, 147, 112, 219),
            TitleBarMinBorderInactiveColor = Color.FromArgb(255, 123, 104, 238),

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(255, 65, 105, 225),
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(255, 30, 144, 255),
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(255, 123, 104, 238),
            TitleBarMinimizeInactiveTextColor = Color.White,
            TitleBarMinimizeInactiveIconColor = Color.White,
            TitleBarMinimizeBorderColor = Color.FromArgb(255, 138, 43, 226),
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(255, 147, 112, 219),
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(255, 147, 112, 219),
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(255, 123, 104, 238),

            // **General Colors**
            TitleForColor = Color.FromArgb(255, 75, 0, 130),
            TitleBarForColor = Color.FromArgb(255, 75, 0, 130),
            DescriptionForColor = Color.WhiteSmoke,
            BeforeForColor = Color.FromArgb(255, 255, 140, 0),       // Dark Orange
            LatestForColor = Color.FromArgb(255, 0, 255, 127),       // Spring Green
            BackColor = Color.FromArgb(255, 200, 248, 255), // Alice Blue
            BackgroundColor = Color.FromArgb(255, 040, 248, 255), // Alice Blue

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(255, 255, 105, 180),    // Hot Pink
            ButtonForeColor = Color.FromArgb(255, 75, 0, 130),
            ButtonHoverBackColor = Color.FromArgb(255, 255, 20, 147), // Deep Pink
            ButtonHoverForeColor = Color.White,
            ButtonActiveBackColor = Color.FromArgb(255, 255, 0, 255), // Magenta
            ButtonActiveForeColor = Color.White,

            // **TextBox Colors**
            TextBoxBackColor = Color.FromArgb(255, 240, 248, 255),   // Alice Blue
            TextBoxForeColor = Color.FromArgb(255, 75, 0, 130),      // Indigo


            // **Label Colors**
            LabelBackColor = Color.White,
            LabelForeColor = Color.FromArgb(255, 75, 0, 130),

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(255, 240, 248, 255), // Alice Blue


            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(255, 138, 43, 226),
            HeaderForeColor = Color.White,
            GridLineColor = Color.FromArgb(255, 147, 112, 219),
            RowBackColor = Color.White,
            RowForeColor = Color.FromArgb(255, 75, 0, 130),
            AltRowBackColor = Color.FromArgb(255, 240, 248, 255),
            SelectedRowBackColor = Color.FromArgb(255, 75, 0, 130),
            SelectedRowForeColor = Color.White,

            // **ComboBox Colors**
            ComboBoxBackColor = Color.FromArgb(255, 240, 248, 255),
            ComboBoxForeColor = Color.FromArgb(255, 75, 0, 130),


            // **CheckBox Colors**
            CheckBoxBackColor = Color.White,
            CheckBoxForeColor = Color.FromArgb(255, 75, 0, 130),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.White,
            RadioButtonForeColor = Color.FromArgb(255, 75, 0, 130),

            // **Border Colors**
            BorderColor = Color.FromArgb(255, 138, 43, 226),
            ActiveBorderColor = Color.FromArgb(255, 147, 112, 219),
            InactiveBorderColor = Color.FromArgb(255, 123, 104, 238),
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.FromArgb(255, 30, 144, 255),
            VisitedLinkColor = Color.FromArgb(255, 138, 43, 226),
            HoverLinkColor = Color.FromArgb(255, 75, 0, 130),
            LinkHoverColor = Color.FromArgb(255, 75, 0, 130),
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(255, 75, 0, 130),
            ToolTipForeColor = Color.White,

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(255, 240, 248, 255),
            ScrollBarThumbColor = Color.FromArgb(255, 138, 43, 226),
            ScrollBarTrackColor = Color.FromArgb(255, 240, 248, 255),

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(255, 138, 43, 226),
            StatusBarForeColor = Color.White,

            // **Tab Colors**
            TabBackColor = Color.FromArgb(255, 240, 248, 255),
            TabForeColor = Color.FromArgb(255, 75, 0, 130),
            ActiveTabBackColor = Color.White,
            ActiveTabForeColor = Color.FromArgb(255, 75, 0, 130),

            // **Dialog Box Colors**
            DialogBackColor = Color.White,
            DialogForeColor = Color.FromArgb(255, 75, 0, 130),
            DialogButtonBackColor = Color.FromArgb(255, 138, 43, 226),
            DialogButtonForeColor = Color.White,

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(255, 255, 0, 0),      // Red
            GradientEndColor = Color.FromArgb(255, 0, 0, 255),        // Blue
            GradientDirection = LinearGradientMode.Horizontal,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(255, 010, 208, 255), // Blue Violet

            SideMenuHoverBackColor = Color.FromArgb(255, 138, 43, 226),
            SideMenuSelectedBackColor = Color.FromArgb(255, 75, 0, 130),
            SideMenuForeColor = Color.FromArgb(255, 75, 0, 130),
            SideMenuHoverForeColor = Color.White,
            SideMenuSelectedForeColor = Color.White,
            SideMenuBorderColor = Color.FromArgb(255, 138, 43, 226),
            SideMenuIconColor = Color.FromArgb(255, 75, 0, 130),
            SideMenuSelectedIconColor = Color.White,

            // **Title Bar Colors**
            TitleBarBackColor = Color.CornflowerBlue,
            TitleBarForeColor = Color.White,
            TitleBarHoverBackColor = Color.FromArgb(255, 138, 43, 226),
            TitleBarHoverForeColor = Color.White,

            // **Dashboard Colors**
            DashboardBackColor = Color.White,
            DashboardCardBackColor = Color.FromArgb(255, 240, 248, 255),
            DashboardCardHoverBackColor = Color.FromArgb(255, 138, 43, 226),
            CardTitleForeColor = Color.FromArgb(255, 75, 0, 130),
            CardTextForeColor = Color.FromArgb(255, 75, 0, 130),

            // **Data Visualization (Charts)**
            ChartBackColor = Color.White,
            ChartLineColor = Color.FromArgb(255, 75, 0, 130),
            ChartFillColor = Color.FromArgb(100, 75, 0, 130), // Semi-transparent Indigo
            ChartAxisColor = Color.FromArgb(255, 75, 0, 130),

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(255, 75, 0, 130),
            SidebarSelectedIconColor = Color.White,
            SidebarTextColor = Color.FromArgb(255, 75, 0, 130),
            SidebarSelectedTextColor = Color.White,

            // **Navigation Colors**
            NavigationBackColor = Color.White,
            NavigationForeColor = Color.FromArgb(255, 75, 0, 130),
            NavigationHoverBackColor = Color.FromArgb(255, 240, 248, 255),
            NavigationHoverForeColor = Color.FromArgb(255, 75, 0, 130),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(255, 255, 69, 0),        // Orange Red
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(255, 138, 43, 226),  // Blue Violet

            // **Font Properties**
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(255, 75, 0, 130),
            SecondaryTextColor = Color.FromArgb(255, 138, 43, 226),
            AccentTextColor = Color.FromArgb(255, 255, 69, 0),

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(255, 75, 0, 130),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(255, 138, 43, 226),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(255, 147, 112, 219),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 75, 0, 130),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 138, 43, 226),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 147, 112, 219),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 75, 0, 130),
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 138, 43, 226),
            },
            BlockquoteBorderColor = Color.FromArgb(255, 138, 43, 226),
            BlockquoteBorderWidth = 2f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 255, 69, 0),
            },
            InlineCodeBackgroundColor = Color.FromArgb(255, 240, 248, 255),
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 75, 0, 130),
            },
            CodeBlockBackgroundColor = Color.FromArgb(255, 240, 248, 255),
            CodeBlockBorderColor = Color.FromArgb(255, 138, 43, 226),
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 75, 0, 130),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 75, 0, 130),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 138, 43, 226),
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 75, 0, 130),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 75, 0, 130),
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 75, 0, 130),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 138, 43, 226),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 147, 112, 219),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 75, 0, 130),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 138, 43, 226),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 147, 112, 219),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(255, 75, 0, 130),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(255, 138, 43, 226),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(255, 147, 112, 219),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 75, 0, 130),
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 75, 0, 130),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 75, 0, 130),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 255, 69, 0),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 138, 43, 226),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(255, 75, 0, 130),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(255, 75, 0, 130),          // Indigo
            SecondaryColor = Color.FromArgb(255, 138, 43, 226),      // Blue Violet
            AccentColor = Color.FromArgb(255, 255, 69, 0),           // Orange Red
            SurfaceColor = Color.FromArgb(255, 240, 248, 255),       // Alice Blue
            ErrorColor = Color.FromArgb(255, 220, 20, 60),           // Crimson
            WarningColor = Color.FromArgb(255, 255, 140, 0),         // Dark Orange
            SuccessColor = Color.FromArgb(255, 60, 179, 113),        // Medium Sea Green
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.FromArgb(255, 75, 0, 130),

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 8,

            // **Imagery and Iconography**
            IconSet = "GradientIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(100, 0, 0, 0), // Semi-transparent black
            ShadowOpacity = 0.2f,

            // **Animation and Transitions**
            AnimationDurationShort = 150,  // in milliseconds
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(255, 138, 43, 226),

            // **Theme Variant**
            IsDarkTheme = false,
        };
        public static BeepTheme HighContrastTheme => new BeepTheme
        {
            GridBackColor = Color.FromArgb(255, 0, 0, 0),
            GridForeColor = Color.White,
            GridHeaderBackColor = Color.Gray,
            GridHeaderForeColor = Color.White,
            GridHeaderBorderColor = Color.Yellow,
            GridHeaderHoverBackColor = Color.FromArgb(128, 128, 128), // Dim Gray
            GridHeaderHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderSelectedBackColor = Color.Yellow,
            GridHeaderSelectedForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderHoverBorderColor = Color.Yellow,
            GridHeaderSelectedBorderColor = Color.Yellow,
            GridRowHoverBackColor = Color.FromArgb(64, 64, 64), // Dark Gray
            GridRowHoverForeColor = Color.White,
            GridRowSelectedBackColor = Color.Yellow,
            GridRowSelectedForeColor = Color.FromArgb(255, 0, 0, 0),
            GridRowHoverBorderColor = Color.Yellow,
            GridRowSelectedBorderColor = Color.Yellow,
            CardBackColor = Color.FromArgb(255, 215, 0),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,

            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.White,

            },

            // **UI Elements**
            CloseButtonColor = Color.Red,
            MaxButtonColor = Color.Green,
            MinButtonColor = Color.Yellow,
            TitleBarColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarTextColor = Color.White,
            TitleBarIconColor = Color.White,
            TitleBarHoverColor = Color.FromArgb(255, 30, 30, 30),
            TitleBarHoverTextColor = Color.White,
            TitleBarHoverIconColor = Color.White,
            TitleBarActiveColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarActiveTextColor = Color.White,
            TitleBarActiveIconColor = Color.White,
            TitleBarInactiveColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarInactiveTextColor = Color.Gray,
            TitleBarInactiveIconColor = Color.Gray,
            TitleBarBorderColor = Color.White,
            TitleBarBorderHoverColor = Color.White,
            TitleBarBorderActiveColor = Color.White,
            TitleBarBorderInactiveColor = Color.Gray,

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(255, 200, 0, 0),
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.Red,
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarCloseInactiveTextColor = Color.Gray,
            TitleBarCloseInactiveIconColor = Color.Gray,
            TitleBarCloseBorderColor = Color.White,
            TitleBarCloseBorderHoverColor = Color.White,
            TitleBarCloseBorderActiveColor = Color.White,
            TitleBarCloseBorderInactiveColor = Color.Gray,

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(255, 0, 200, 0),
            TitleBarMaxHoverTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMaxHoverIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMaxActiveColor = Color.Green,
            TitleBarMaxActiveTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMaxActiveIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMaxInactiveColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMaxInactiveTextColor = Color.Gray,
            TitleBarMaxInactiveIconColor = Color.Gray,
            TitleBarMaxBorderColor = Color.White,
            TitleBarMaxBorderHoverColor = Color.White,
            TitleBarMaxBorderActiveColor = Color.White,
            TitleBarMaxBorderInactiveColor = Color.Gray,

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(255, 200, 200, 0),
            TitleBarMinHoverTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinHoverIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinActiveColor = Color.Yellow,
            TitleBarMinActiveTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinActiveIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinInactiveColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinInactiveTextColor = Color.Gray,
            TitleBarMinInactiveIconColor = Color.Gray,
            TitleBarMinBorderColor = Color.White,
            TitleBarMinBorderHoverColor = Color.White,
            TitleBarMinBorderActiveColor = Color.White,
            TitleBarMinBorderInactiveColor = Color.Gray,

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(255, 200, 200, 0),
            TitleBarMinimizeHoverTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinimizeHoverIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinimizeActiveColor = Color.Yellow,
            TitleBarMinimizeActiveTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinimizeActiveIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinimizeInactiveColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinimizeInactiveTextColor = Color.Gray,
            TitleBarMinimizeInactiveIconColor = Color.Gray,
            TitleBarMinimizeBorderColor = Color.White,
            TitleBarMinimizeBorderHoverColor = Color.White,
            TitleBarMinimizeBorderActiveColor = Color.White,
            TitleBarMinimizeBorderInactiveColor = Color.Gray,

            // **General Colors**
            TitleForColor = Color.White,
            TitleBarForColor = Color.White,
            DescriptionForColor = Color.White,
            BeforeForColor = Color.White,
            LatestForColor = Color.White,
            BackColor = Color.FromArgb(255, 0, 0, 0),

            // **Button Colors**
            ButtonBackColor = Color.DimGray ,
            ButtonForeColor = Color.White,
            ButtonHoverBackColor = Color.DimGray,
            ButtonHoverForeColor = Color.White,
            ButtonActiveBackColor = Color.DimGray,
            ButtonActiveForeColor = Color.White,

            // **TextBox Colors**
            TextBoxBackColor = Color.DarkGray,
            TextBoxForeColor = Color.White,


            // **Label Colors**
            LabelBackColor = Color.FromArgb(255, 0, 0, 0),
            LabelForeColor = Color.White,

            // **Panel Colors**
            PanelBackColor = Color.DarkGray,

            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(255, 0, 0, 0),
            HeaderForeColor = Color.White,
            GridLineColor = Color.White,
            RowBackColor = Color.FromArgb(255, 0, 0, 0),
            RowForeColor = Color.White,
            AltRowBackColor = Color.FromArgb(255, 30, 30, 30),
            SelectedRowBackColor = Color.White,
            SelectedRowForeColor = Color.FromArgb(255, 0, 0, 0),

            // **ComboBox Colors**
            ComboBoxBackColor = Color.FromArgb(255, 0, 0, 0),
            ComboBoxForeColor = Color.White,


            // **CheckBox Colors**
            CheckBoxBackColor = Color.FromArgb(255, 0, 0, 0),
            CheckBoxForeColor = Color.White,

            // **RadioButton Colors**
            RadioButtonBackColor = Color.FromArgb(255, 0, 0, 0),
            RadioButtonForeColor = Color.White,

            // **Border Colors**
            BorderColor = Color.White,
            ActiveBorderColor = Color.Yellow,
            InactiveBorderColor = Color.Gray,
            BorderSize = 2,

            // **Link Colors**
            LinkColor = Color.Yellow,
            VisitedLinkColor = Color.Magenta,
            HoverLinkColor = Color.Cyan,
            LinkHoverColor = Color.Cyan,
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(255, 0, 0, 0),
            ToolTipForeColor = Color.White,

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(255, 0, 0, 0),
            ScrollBarThumbColor = Color.White,
            ScrollBarTrackColor = Color.Gray,

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(255, 0, 0, 0),
            StatusBarForeColor = Color.White,

            // **Tab Colors**
            TabBackColor = Color.FromArgb(255, 0, 0, 0),
            TabForeColor = Color.White,
            ActiveTabBackColor = Color.White,
            ActiveTabForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Dialog Box Colors**
            DialogBackColor = Color.Gray,
            DialogForeColor = Color.White,
            DialogButtonBackColor = Color.White,
            DialogButtonForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(255, 0, 0, 0),
            GradientEndColor = Color.FromArgb(255, 0, 0, 0),
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.FromArgb(255, 30, 30, 30), // A dark gray tone to distinguish it from the panel

            SideMenuHoverBackColor = Color.White,
            SideMenuSelectedBackColor = Color.White,
            SideMenuForeColor = Color.White,
            SideMenuHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            SideMenuSelectedForeColor = Color.FromArgb(255, 0, 0, 0),
            SideMenuBorderColor = Color.White,
            SideMenuIconColor = Color.White,
            SideMenuSelectedIconColor = Color.FromArgb(255, 0, 0, 0),

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(255, 35, 35, 30),
            TitleBarForeColor = Color.White,
            TitleBarHoverBackColor = Color.FromArgb(255, 30, 30, 30),
            TitleBarHoverForeColor = Color.White,

            // **Dashboard Colors**
            DashboardBackColor = Color.FromArgb(255, 0, 0, 0),
            DashboardCardBackColor = Color.FromArgb(255, 0, 0, 0),
            DashboardCardHoverBackColor = Color.White,
            CardTitleForeColor = Color.White,
            CardTextForeColor = Color.White,

            // **Data Visualization (Charts)**
            ChartBackColor = Color.FromArgb(255, 0, 0, 0),
            ChartLineColor = Color.White,
            ChartFillColor = Color.White,
            ChartAxisColor = Color.White,

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.White,
            SidebarSelectedIconColor = Color.FromArgb(255, 0, 0, 0),
            SidebarTextColor = Color.White,
            SidebarSelectedTextColor = Color.FromArgb(255, 0, 0, 0),

            // **Navigation Colors**
            NavigationBackColor = Color.FromArgb(255, 0, 0, 0),
            NavigationForeColor = Color.White,
            NavigationHoverBackColor = Color.White,
            NavigationHoverForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.White,
            BadgeForeColor = Color.FromArgb(255, 0, 0, 0),
            HighlightBackColor = Color.Yellow,

            // **Font Properties**
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.White,
            SecondaryTextColor = Color.White,
            AccentTextColor = Color.Yellow,

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.White,

            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.White,

            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.White,

            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.White,

            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.White,

            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.White,

            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.White,

            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.White,

            },
            BlockquoteBorderColor = Color.White,
            BlockquoteBorderWidth = 2f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.White,

            },
            InlineCodeBackgroundColor = Color.FromArgb(255, 0, 0, 0),
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.White,

            },
            CodeBlockBackgroundColor = Color.FromArgb(255, 0, 0, 0),
            CodeBlockBorderColor = Color.White,
            CodeBlockBorderWidth = 2f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.White,

            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.White,

            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.White,

            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,

            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.White,

            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,

            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,

            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,

            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,

            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,

            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,

            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.White,

            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.White,

            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.White,

            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.White,

            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.White,

            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.White,

            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,

            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White,

            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.White,

            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(255, 0, 0, 0),
            SecondaryColor = Color.White,
            AccentColor = Color.Yellow,
            BackgroundColor = Color.DimGray,
            SurfaceColor = Color.FromArgb(255, 0, 0, 0),
            ErrorColor = Color.Red,
            WarningColor = Color.Yellow,
            SuccessColor = Color.Green,
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.White,

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 0,

            // **Imagery and Iconography**
            IconSet = "HighContrastIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.Empty,
            ShadowOpacity = 0f,

            // **Animation and Transitions**
            AnimationDurationShort = 0,  // in milliseconds
            AnimationDurationMedium = 0,
            AnimationDurationLong = 0,
            AnimationEasingFunction = "linear",

            // **Accessibility**
            HighContrastMode = true,
            FocusIndicatorColor = Color.Yellow,

            // **Theme Variant**
            IsDarkTheme = true,
        };
        public static BeepTheme MonochromeTheme => new BeepTheme
        {
            GridBackColor = Color.White,
            GridForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderBackColor = Color.Gray,
            GridHeaderForeColor = Color.White,
            GridHeaderBorderColor = Color.DarkGray,
            GridHeaderHoverBackColor = Color.DimGray,
            GridHeaderHoverForeColor = Color.White,
            GridHeaderSelectedBackColor = Color.Silver,
            GridHeaderSelectedForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderHoverBorderColor = Color.DarkGray,
            GridHeaderSelectedBorderColor = Color.Silver,
            GridRowHoverBackColor = Color.LightGray,
            GridRowHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridRowSelectedBackColor = Color.Gray,
            GridRowSelectedForeColor = Color.White,
            GridRowHoverBorderColor = Color.DarkGray,
            GridRowSelectedBorderColor = Color.Gray,
            CardBackColor = Color.FromArgb(128, 128, 128),
            // **Card Styles**
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),

            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),

            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(64, 64, 64),   // Dark Gray
            MaxButtonColor = Color.FromArgb(128, 128, 128),  // Medium Gray
            MinButtonColor = Color.FromArgb(192, 192, 192),  // Light Gray
            TitleBarColor = Color.FromArgb(230, 230, 230),   // Very Light Gray
            TitleBarTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarHoverColor = Color.FromArgb(210, 210, 210),
            TitleBarHoverTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarHoverIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarActiveColor = Color.FromArgb(220, 220, 220),
            TitleBarActiveTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarActiveIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarInactiveColor = Color.FromArgb(240, 240, 240),
            TitleBarInactiveTextColor = Color.DarkGray,
            TitleBarInactiveIconColor = Color.DarkGray,
            TitleBarBorderColor = Color.FromArgb(200, 200, 200),
            TitleBarBorderHoverColor = Color.FromArgb(180, 180, 180),
            TitleBarBorderActiveColor = Color.FromArgb(180, 180, 180),
            TitleBarBorderInactiveColor = Color.FromArgb(220, 220, 220),

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(100, 100, 100),
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(64, 64, 64),
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(240, 240, 240),
            TitleBarCloseInactiveTextColor = Color.DarkGray,
            TitleBarCloseInactiveIconColor = Color.DarkGray,
            TitleBarCloseBorderColor = Color.FromArgb(200, 200, 200),
            TitleBarCloseBorderHoverColor = Color.FromArgb(180, 180, 180),
            TitleBarCloseBorderActiveColor = Color.FromArgb(180, 180, 180),
            TitleBarCloseBorderInactiveColor = Color.FromArgb(220, 220, 220),

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(160, 160, 160),
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.White,
            TitleBarMaxActiveColor = Color.FromArgb(128, 128, 128),
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(240, 240, 240),
            TitleBarMaxInactiveTextColor = Color.DarkGray,
            TitleBarMaxInactiveIconColor = Color.DarkGray,
            TitleBarMaxBorderColor = Color.FromArgb(200, 200, 200),
            TitleBarMaxBorderHoverColor = Color.FromArgb(180, 180, 180),
            TitleBarMaxBorderActiveColor = Color.FromArgb(180, 180, 180),
            TitleBarMaxBorderInactiveColor = Color.FromArgb(220, 220, 220),

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(210, 210, 210),
            TitleBarMinHoverTextColor = Color.White,
            TitleBarMinHoverIconColor = Color.White,
            TitleBarMinActiveColor = Color.FromArgb(192, 192, 192),
            TitleBarMinActiveTextColor = Color.White,
            TitleBarMinActiveIconColor = Color.White,
            TitleBarMinInactiveColor = Color.FromArgb(240, 240, 240),
            TitleBarMinInactiveTextColor = Color.DarkGray,
            TitleBarMinInactiveIconColor = Color.DarkGray,
            TitleBarMinBorderColor = Color.FromArgb(200, 200, 200),
            TitleBarMinBorderHoverColor = Color.FromArgb(180, 180, 180),
            TitleBarMinBorderActiveColor = Color.FromArgb(180, 180, 180),
            TitleBarMinBorderInactiveColor = Color.FromArgb(220, 220, 220),

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(210, 210, 210),
            TitleBarMinimizeHoverTextColor = Color.White,
            TitleBarMinimizeHoverIconColor = Color.White,
            TitleBarMinimizeActiveColor = Color.FromArgb(192, 192, 192),
            TitleBarMinimizeActiveTextColor = Color.White,
            TitleBarMinimizeActiveIconColor = Color.White,
            TitleBarMinimizeInactiveColor = Color.FromArgb(240, 240, 240),
            TitleBarMinimizeInactiveTextColor = Color.DarkGray,
            TitleBarMinimizeInactiveIconColor = Color.DarkGray,
            TitleBarMinimizeBorderColor = Color.FromArgb(200, 200, 200),
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(180, 180, 180),
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(180, 180, 180),
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(220, 220, 220),

            // **General Colors**
            TitleForColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarForColor = Color.FromArgb(255, 0, 0, 0),
            DescriptionForColor = Color.DarkGray,
            BeforeForColor = Color.Gray,
            LatestForColor = Color.Gray,
            BackColor = Color.White,

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(230, 230, 230),
            ButtonForeColor = Color.FromArgb(255, 0, 0, 0),
            ButtonHoverBackColor = Color.FromArgb(210, 210, 210),
            ButtonHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            ButtonActiveBackColor = Color.FromArgb(200, 200, 200),
            ButtonActiveForeColor = Color.FromArgb(255, 0, 0, 0),

            // **TextBox Colors**
            TextBoxBackColor = Color.White,
            TextBoxForeColor = Color.FromArgb(255, 0, 0, 0),


            // **Label Colors**
            LabelBackColor = Color.White,
            LabelForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(240, 240, 240), // A soft gray tone distinct from white


            // **Grid Colors**
            HeaderBackColor = Color.FromArgb(245, 245, 245),
            HeaderForeColor = Color.FromArgb(255, 0, 0, 0),
            GridLineColor = Color.FromArgb(200, 200, 200),
            RowBackColor = Color.White,
            RowForeColor = Color.FromArgb(255, 0, 0, 0),
            AltRowBackColor = Color.FromArgb(245, 245, 245),
            SelectedRowBackColor = Color.FromArgb(230, 230, 230),
            SelectedRowForeColor = Color.FromArgb(255, 0, 0, 0),

            // **ComboBox Colors**
            ComboBoxBackColor = Color.White,
            ComboBoxForeColor = Color.FromArgb(255, 0, 0, 0),


            // **CheckBox Colors**
            CheckBoxBackColor = Color.White,
            CheckBoxForeColor = Color.FromArgb(255, 0, 0, 0),

            // **RadioButton Colors**
            RadioButtonBackColor = Color.White,
            RadioButtonForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Border Colors**
            BorderColor = Color.FromArgb(200, 200, 200),
            ActiveBorderColor = Color.Gray,
            InactiveBorderColor = Color.LightGray,
            BorderSize = 1,

            // **Link Colors**
            LinkColor = Color.Gray,
            VisitedLinkColor = Color.DarkGray,
            HoverLinkColor = Color.FromArgb(255, 0, 0, 0),
            LinkHoverColor = Color.FromArgb(255, 0, 0, 0),
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(245, 245, 245),
            ToolTipForeColor = Color.FromArgb(255, 0, 0, 0),

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(245, 245, 245),
            ScrollBarThumbColor = Color.Gray,
            ScrollBarTrackColor = Color.LightGray,

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(245, 245, 245),
            StatusBarForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Tab Colors**
            TabBackColor = Color.FromArgb(245, 245, 245),
            TabForeColor = Color.FromArgb(255, 0, 0, 0),
            ActiveTabBackColor = Color.White,
            ActiveTabForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Dialog Box Colors**
            DialogBackColor = Color.White,
            DialogForeColor = Color.FromArgb(255, 0, 0, 0),
            DialogButtonBackColor = Color.FromArgb(230, 230, 230),
            DialogButtonForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Gradient Properties**
            GradientStartColor = Color.White,
            GradientEndColor = Color.White,
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors**
            SideMenuBackColor = Color.DarkGray, // A slightly darker gray than the panel

            SideMenuHoverBackColor = Color.FromArgb(230, 230, 230),
            SideMenuSelectedBackColor = Color.FromArgb(210, 210, 210),
            SideMenuForeColor = Color.FromArgb(255, 0, 0, 0),
            SideMenuHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            SideMenuSelectedForeColor = Color.FromArgb(255, 0, 0, 0),
            SideMenuBorderColor = Color.FromArgb(200, 200, 200),
            SideMenuIconColor = Color.FromArgb(255, 0, 0, 0),
            SideMenuSelectedIconColor = Color.FromArgb(255, 0, 0, 0),

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(230, 230, 230),
            TitleBarForeColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarHoverBackColor = Color.FromArgb(210, 210, 210),
            TitleBarHoverForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Dashboard Colors**
            DashboardBackColor = Color.White,
            DashboardCardBackColor = Color.FromArgb(245, 245, 245),
            DashboardCardHoverBackColor = Color.FromArgb(230, 230, 230),
            CardTitleForeColor = Color.FromArgb(255, 0, 0, 0),
            CardTextForeColor = Color.DarkGray,

            // **Data Visualization (Charts)**
            ChartBackColor = Color.White,
            ChartLineColor = Color.Gray,
            ChartFillColor = Color.FromArgb(100, 128, 128, 128), // Semi-transparent Gray
            ChartAxisColor = Color.FromArgb(255, 0, 0, 0),

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.FromArgb(255, 0, 0, 0),
            SidebarSelectedIconColor = Color.FromArgb(255, 0, 0, 0),
            SidebarTextColor = Color.FromArgb(255, 0, 0, 0),
            SidebarSelectedTextColor = Color.FromArgb(255, 0, 0, 0),

            // **Navigation Colors**
            NavigationBackColor = Color.White,
            NavigationForeColor = Color.FromArgb(255, 0, 0, 0),
            NavigationHoverBackColor = Color.FromArgb(245, 245, 245),
            NavigationHoverForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.Gray,
            BadgeForeColor = Color.White,
            HighlightBackColor = Color.FromArgb(230, 230, 230),

            // **Font Properties**
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.FromArgb(255, 0, 0, 0),
            SecondaryTextColor = Color.DarkGray,
            AccentTextColor = Color.Gray,

            // **Typography Styles**

            // Heading Styles
            Heading1 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },

            // Additional Typography Styles
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.DarkGray,
            },
            BlockquoteBorderColor = Color.Gray,
            BlockquoteBorderWidth = 1f,
            BlockquotePadding = 8f,

            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            InlineCodeBackgroundColor = Color.FromArgb(245, 245, 245),
            InlineCodePadding = 4f,

            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            CodeBlockBackgroundColor = Color.FromArgb(245, 245, 245),
            CodeBlockBorderColor = Color.Gray,
            CodeBlockBorderWidth = 1f,
            CodeBlockPadding = 8f,

            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.DarkGray,
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },

            // Headlines
            HeadlineLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 24f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            HeadlineMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            HeadlineSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },

            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },

            // Body Texts
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },

            // **Color Palette**
            PrimaryColor = Color.FromArgb(255, 0, 0, 0),
            SecondaryColor = Color.Gray,
            AccentColor = Color.DarkGray,
            BackgroundColor = Color.White,
            SurfaceColor = Color.FromArgb(245, 245, 245),
            ErrorColor = Color.DarkRed,
            WarningColor = Color.DarkOrange,
            SuccessColor = Color.DarkGreen,
            OnPrimaryColor = Color.White,
            OnBackgroundColor = Color.FromArgb(255, 0, 0, 0),

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 0,

            // **Imagery and Iconography**
            IconSet = "MonochromeIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.Empty,
            ShadowOpacity = 0f,

            // **Animation and Transitions**
            AnimationDurationShort = 150,  // in milliseconds
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.Gray,

            // **Theme Variant**
            IsDarkTheme = false,
        };
        public static BeepTheme LuxuryGoldTheme => new BeepTheme
        {
            // **Grid Colors**
            GridBackColor = Color.FromArgb(255, 0, 0, 0),
            GridForeColor = Color.Gold,
            GridHeaderBackColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderForeColor = Color.Gold,
            GridHeaderBorderColor = Color.DarkGoldenrod,
            GridHeaderHoverBackColor = Color.Gold,
            GridHeaderHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderSelectedBackColor = Color.FromArgb(128, 128, 0), // Olive
            GridHeaderSelectedForeColor = Color.FromArgb(255, 0, 0, 0),
            GridHeaderHoverBorderColor = Color.Gold,
            GridHeaderSelectedBorderColor = Color.DarkGoldenrod,
            GridRowHoverBackColor = Color.Gold,
            GridRowHoverForeColor = Color.FromArgb(255, 0, 0, 0),
            GridRowSelectedBackColor = Color.FromArgb(128, 128, 0),
            GridRowSelectedForeColor = Color.FromArgb(255, 0, 0, 0),
            GridRowHoverBorderColor = Color.Gold,
            GridRowSelectedBorderColor = Color.DarkGoldenrod,

            // **Card Styles**
            CardBackColor = Color.FromArgb(212, 175, 55),
            CardHeaderStyle = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 22f,
                FontWeight = FontWeight.Bold,
                TextColor = Color.White, // Metallic Gold
            },
            CardparagraphStyle = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontWeight = FontWeight.Regular,
                TextColor = Color.WhiteSmoke,
            },

            // **UI Elements**
            CloseButtonColor = Color.FromArgb(255, 192, 0, 0), // Dark Red
            MaxButtonColor = Color.FromArgb(255, 0, 128, 0),   // Dark Green
            MinButtonColor = Color.FromArgb(255, 218, 165, 32), // Goldenrod
            TitleBarColor = Color.FromArgb(255, 15, 15, 15),    // Very Dark Gray
            TitleBarTextColor = Color.FromArgb(255, 212, 175),  // Metallic Gold
            TitleBarIconColor = Color.FromArgb(255, 212, 175),
            TitleBarHoverColor = Color.FromArgb(255, 25, 25, 25),
            TitleBarHoverTextColor = Color.FromArgb(255, 212, 175),
            TitleBarHoverIconColor = Color.FromArgb(255, 212, 175),
            TitleBarActiveColor = Color.FromArgb(255, 25, 25, 25),
            TitleBarActiveTextColor = Color.FromArgb(255, 212, 175),
            TitleBarActiveIconColor = Color.FromArgb(255, 212, 175),
            TitleBarInactiveColor = Color.FromArgb(255, 15, 15, 15),
            TitleBarInactiveTextColor = Color.FromArgb(255, 128, 128, 128),
            TitleBarInactiveIconColor = Color.FromArgb(255, 128, 128, 128),
            TitleBarBorderColor = Color.FromArgb(255, 212, 175),
            TitleBarBorderHoverColor = Color.FromArgb(255, 212, 175),
            TitleBarBorderActiveColor = Color.FromArgb(255, 212, 175),
            TitleBarBorderInactiveColor = Color.FromArgb(255, 128, 128, 128),

            // Close Button
            TitleBarCloseHoverColor = Color.FromArgb(255, 139, 0, 0),  // Dark Red
            TitleBarCloseHoverTextColor = Color.White,
            TitleBarCloseHoverIconColor = Color.White,
            TitleBarCloseActiveColor = Color.FromArgb(255, 165, 42, 42), // Brown
            TitleBarCloseActiveTextColor = Color.White,
            TitleBarCloseActiveIconColor = Color.White,
            TitleBarCloseInactiveColor = Color.FromArgb(255, 15, 15, 15),
            TitleBarCloseInactiveTextColor = Color.FromArgb(255, 128, 128, 128),
            TitleBarCloseInactiveIconColor = Color.FromArgb(255, 128, 128, 128),
            TitleBarCloseBorderColor = Color.FromArgb(255, 192, 0, 0),
            TitleBarCloseBorderHoverColor = Color.FromArgb(255, 192, 0, 0),
            TitleBarCloseBorderActiveColor = Color.FromArgb(255, 192, 0, 0),
            TitleBarCloseBorderInactiveColor = Color.FromArgb(255, 128, 128, 128),

            // Maximize Button
            TitleBarMaxHoverColor = Color.FromArgb(255, 0, 100, 0),  // Dark Green
            TitleBarMaxHoverTextColor = Color.White,
            TitleBarMaxHoverIconColor = Color.White,
            TitleBarMaxActiveColor = Color.FromArgb(255, 0, 128, 0),
            TitleBarMaxActiveTextColor = Color.White,
            TitleBarMaxActiveIconColor = Color.White,
            TitleBarMaxInactiveColor = Color.FromArgb(255, 15, 15, 15),
            TitleBarMaxInactiveTextColor = Color.FromArgb(255, 128, 128, 128),
            TitleBarMaxInactiveIconColor = Color.FromArgb(255, 128, 128, 128),
            TitleBarMaxBorderColor = Color.FromArgb(255, 0, 128, 0),
            TitleBarMaxBorderHoverColor = Color.FromArgb(255, 0, 128, 0),
            TitleBarMaxBorderActiveColor = Color.FromArgb(255, 0, 128, 0),
            TitleBarMaxBorderInactiveColor = Color.FromArgb(255, 128, 128, 128),

            // Minimize Button
            TitleBarMinHoverColor = Color.FromArgb(255, 184, 134, 11), // Dark Goldenrod
            TitleBarMinHoverTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinHoverIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinActiveColor = Color.FromArgb(255, 218, 165, 32), // Goldenrod
            TitleBarMinActiveTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinActiveIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinInactiveColor = Color.FromArgb(255, 15, 15, 15),
            TitleBarMinInactiveTextColor = Color.FromArgb(255, 128, 128, 128),
            TitleBarMinInactiveIconColor = Color.FromArgb(255, 128, 128, 128),
            TitleBarMinBorderColor = Color.FromArgb(255, 218, 165, 32),
            TitleBarMinBorderHoverColor = Color.FromArgb(255, 218, 165, 32),
            TitleBarMinBorderActiveColor = Color.FromArgb(255, 218, 165, 32),
            TitleBarMinBorderInactiveColor = Color.FromArgb(255, 128, 128, 128),

            // Minimize Button (Alternative Properties)
            TitleBarMinimizeHoverColor = Color.FromArgb(255, 184, 134, 11),
            TitleBarMinimizeHoverTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinimizeHoverIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinimizeActiveColor = Color.FromArgb(255, 218, 165, 32),
            TitleBarMinimizeActiveTextColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinimizeActiveIconColor = Color.FromArgb(255, 0, 0, 0),
            TitleBarMinimizeInactiveColor = Color.FromArgb(255, 15, 15, 15),
            TitleBarMinimizeInactiveTextColor = Color.FromArgb(255, 128, 128, 128),
            TitleBarMinimizeInactiveIconColor = Color.FromArgb(255, 128, 128, 128),
            TitleBarMinimizeBorderColor = Color.FromArgb(255, 218, 165, 32),
            TitleBarMinimizeBorderHoverColor = Color.FromArgb(255, 218, 165, 32),
            TitleBarMinimizeBorderActiveColor = Color.FromArgb(255, 218, 165, 32),
            TitleBarMinimizeBorderInactiveColor = Color.FromArgb(255, 128, 128, 128),

            // **General Colors**
            TitleForColor = Color.FromArgb(255, 212, 175), // Metallic Gold
            TitleBarForColor = Color.FromArgb(255, 212, 175),
            DescriptionForColor = Color.WhiteSmoke,
            BeforeForColor = Color.FromArgb(255, 212, 175),
            LatestForColor = Color.FromArgb(255, 212, 175),
            BackColor = Color.FromArgb(255, 20, 20, 20), // Dark Background

            // **Button Colors**
            ButtonBackColor = Color.FromArgb(255, 35, 35, 35),
            ButtonForeColor = Color.FromArgb(255, 212, 175),
            ButtonHoverBackColor = Color.FromArgb(255, 50, 50, 50),
            ButtonHoverForeColor = Color.FromArgb(255, 212, 175),
            ButtonActiveBackColor = Color.FromArgb(255, 212, 175),
            ButtonActiveForeColor = Color.FromArgb(255, 0, 0, 0),

            // **TextBox Colors**
            TextBoxBackColor = Color.FromArgb(255, 30, 30, 30),
            TextBoxForeColor = Color.WhiteSmoke,

            // **Label Colors**
            LabelBackColor = Color.FromArgb(255, 20, 20, 20),
            LabelForeColor = Color.WhiteSmoke,

            // **Panel Colors**
            PanelBackColor = Color.FromArgb(255, 22, 22, 22), // Slightly lighter than the dark background

            // **Grid Colors (Secondary Grid Properties)**
            HeaderBackColor = Color.FromArgb(255, 35, 35, 35),
            HeaderForeColor = Color.FromArgb(255, 212, 175),
            GridLineColor = Color.FromArgb(255, 212, 175),
            RowBackColor = Color.FromArgb(255, 25, 25, 25),
            RowForeColor = Color.WhiteSmoke,
            AltRowBackColor = Color.FromArgb(255, 30, 30, 30),
            SelectedRowBackColor = Color.FromArgb(255, 212, 175),
            SelectedRowForeColor = Color.FromArgb(255, 0, 0, 0),

            // **ComboBox Colors**
            ComboBoxBackColor = Color.FromArgb(255, 30, 30, 30),
            ComboBoxForeColor = Color.WhiteSmoke,

            // **CheckBox Colors**
            CheckBoxBackColor = Color.FromArgb(255, 20, 20, 20),
            CheckBoxForeColor = Color.WhiteSmoke,

            // **RadioButton Colors**
            RadioButtonBackColor = Color.FromArgb(255, 20, 20, 20),
            RadioButtonForeColor = Color.WhiteSmoke,

            // **Border Colors**
            BorderColor = Color.FromArgb(255, 212, 175),
            ActiveBorderColor = Color.FromArgb(255, 255, 215, 0), // Gold
            InactiveBorderColor = Color.FromArgb(255, 128, 128, 128),
            BorderSize = 2,

            // **Link Colors**
            LinkColor = Color.FromArgb(255, 212, 175),
            VisitedLinkColor = Color.FromArgb(255, 184, 134, 11),   // Dark Goldenrod
            HoverLinkColor = Color.FromArgb(255, 255, 215, 0),      // Gold
            LinkHoverColor = Color.FromArgb(255, 255, 215, 0),
            LinkIsUnderline = true,

            // **ToolTip Colors**
            ToolTipBackColor = Color.FromArgb(255, 30, 30, 30),
            ToolTipForeColor = Color.WhiteSmoke,

            // **ScrollBar Colors**
            ScrollBarBackColor = Color.FromArgb(255, 25, 25, 25),
            ScrollBarThumbColor = Color.FromArgb(255, 212, 175),
            ScrollBarTrackColor = Color.FromArgb(255, 40, 40, 40),

            // **Status Bar Colors**
            StatusBarBackColor = Color.FromArgb(255, 25, 25, 25),
            StatusBarForeColor = Color.WhiteSmoke,

            // **Tab Colors**
            TabBackColor = Color.FromArgb(255, 30, 30, 30),
            TabForeColor = Color.WhiteSmoke,
            ActiveTabBackColor = Color.FromArgb(255, 20, 20, 20),
            ActiveTabForeColor = Color.FromArgb(255, 212, 175),

            // **Dialog Box Colors**
            DialogBackColor = Color.FromArgb(255, 20, 20, 20),
            DialogForeColor = Color.WhiteSmoke,
            DialogButtonBackColor = Color.FromArgb(255, 212, 175),
            DialogButtonForeColor = Color.FromArgb(255, 0, 0, 0),

            // **Gradient Properties**
            GradientStartColor = Color.FromArgb(255, 20, 20, 20),
            GradientEndColor = Color.FromArgb(255, 35, 35, 35),
            GradientDirection = LinearGradientMode.Vertical,

            // **Side Menu Colors** (Revised Hover/Selected)
            SideMenuBackColor = Color.DarkGray,           // Darker side menu background
            SideMenuHoverBackColor = Color.Gray,          // Slightly lighter on hover
            SideMenuSelectedBackColor = Color.FromArgb(255, 212, 175), // Metallic Gold
            SideMenuForeColor = Color.WhiteSmoke,
            SideMenuHoverForeColor = Color.WhiteSmoke,
            SideMenuSelectedForeColor = Color.FromArgb(255, 0, 0, 0),
            SideMenuBorderColor = Color.FromArgb(255, 212, 175),
            SideMenuIconColor = Color.WhiteSmoke,
            SideMenuSelectedIconColor = Color.FromArgb(255, 0, 0, 0),

            // **Title Bar Colors**
            TitleBarBackColor = Color.FromArgb(255, 15, 15, 15),
            TitleBarForeColor = Color.FromArgb(255, 212, 175),
            TitleBarHoverBackColor = Color.FromArgb(255, 25, 25, 25),
            TitleBarHoverForeColor = Color.FromArgb(255, 212, 175),

            // **Dashboard Colors**
            DashboardBackColor = Color.FromArgb(255, 20, 20, 20),
            DashboardCardBackColor = Color.FromArgb(255, 25, 25, 25),
            DashboardCardHoverBackColor = Color.FromArgb(255, 30, 30, 30),
            CardTitleForeColor = Color.WhiteSmoke,
            CardTextForeColor = Color.LightGray,

            // **Data Visualization (Charts)**
            ChartBackColor = Color.FromArgb(255, 20, 20, 20),
            ChartLineColor = Color.FromArgb(255, 212, 175),
            ChartFillColor = Color.FromArgb(100, 212, 175, 55), // Semi-transparent Gold
            ChartAxisColor = Color.WhiteSmoke,

            // **Sidebar and Menu Colors**
            SidebarIconColor = Color.WhiteSmoke,
            SidebarSelectedIconColor = Color.FromArgb(255, 0, 0, 0),
            SidebarTextColor = Color.WhiteSmoke,
            SidebarSelectedTextColor = Color.FromArgb(255, 0, 0, 0),

            // **Navigation Colors**
            NavigationBackColor = Color.FromArgb(255, 25, 25, 25),
            NavigationForeColor = Color.WhiteSmoke,
            NavigationHoverBackColor = Color.FromArgb(255, 30, 30, 30),
            NavigationHoverForeColor = Color.WhiteSmoke,

            // **Badge and Highlight Colors**
            BadgeBackColor = Color.FromArgb(255, 212, 175),      // Metallic Gold
            BadgeForeColor = Color.FromArgb(255, 0, 0, 0),
            HighlightBackColor = Color.FromArgb(255, 255, 215, 0), // Gold

            // **Font Properties**
            FontFamily = "Segoe UI",
            FontName = "Segoe UI",
            FontSize = 14f,

            // **Font Sizes**
            FontSizeBlockHeader = 24f,
            FontSizeBlockText = 14f,
            FontSizeQuestion = 16f,
            FontSizeAnswer = 14f,
            FontSizeCaption = 12f,
            FontSizeButton = 14f,

            // **Font Styles**
            FontStyleRegular = FontStyle.Regular,
            FontStyleBold = FontStyle.Bold,
            FontStyleItalic = FontStyle.Italic,

            // **Text Colors**
            PrimaryTextColor = Color.WhiteSmoke,
            SecondaryTextColor = Color.LightGray,
            AccentTextColor = Color.FromArgb(255, 212, 175),

            // **Additional Typography Styles** -----------------------------
            // The missing multi-level headings, display styles, code blocks, etc.

            // Display Styles
            DisplayLarge = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 40f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 212, 175), // Metallic Gold
            },
            DisplayMedium = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 34f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.WhiteSmoke,
            },
            DisplaySmall = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 28f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.LightGray,
            },

            // Heading Styles (1-6). If you want different fonts for gold headings, adjust accordingly.
            Heading1 = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 32f,
                LineHeight = 1.2f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.FromArgb(255, 212, 175),
            },
            Heading2 = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 28f,
                LineHeight = 1.3f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.WhiteSmoke,
            },
            Heading3 = new TypographyStyle
            {
                FontFamily = "Times New Roman",
                FontSize = 24f,
                LineHeight = 1.4f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Bold,
                FontStyle = FontStyle.Bold,
                TextColor = Color.LightGray,
            },
            Heading4 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 20f,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.WhiteSmoke,
            },
            Heading5 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 18f,
                LineHeight = 1.6f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.WhiteSmoke,
            },
            Heading6 = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                LineHeight = 1.7f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.SemiBold,
                FontStyle = FontStyle.Regular,
                TextColor = Color.WhiteSmoke,
            },

            // Paragraph Style
            Paragraph = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                LineHeight = 1.8f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Regular,
                TextColor = Color.WhiteSmoke,
            },

            // Blockquote Style
            Blockquote = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.LightGray,
            },
            BlockquoteBorderColor = Color.FromArgb(255, 212, 175),
            BlockquoteBorderWidth = 2f,
            BlockquotePadding = 8f,

            // Inline Code Style
            InlineCode = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.FromArgb(255, 255, 215, 0), // Gold
            },
            InlineCodeBackgroundColor = Color.FromArgb(255, 30, 30, 30),
            InlineCodePadding = 4f,

            // Code Block Style
            CodeBlock = new TypographyStyle
            {
                FontFamily = "Consolas",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.WhiteSmoke,
            },
            CodeBlockBackgroundColor = Color.FromArgb(255, 25, 25, 25),
            CodeBlockBorderColor = Color.FromArgb(255, 212, 175),
            CodeBlockBorderWidth = 2f,
            CodeBlockPadding = 8f,

            // List Styles
            UnorderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.WhiteSmoke,
            },
            OrderedList = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.WhiteSmoke,
            },
            ListItemSpacing = 4f,
            ListIndentation = 16f,

            // Additional Text Variants
            SmallText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.Gray,
            },
            StrongText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Bold,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 212, 175),
            },
            EmphasisText = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Italic,
                FontWeight = FontWeight.Regular,
                TextColor = Color.WhiteSmoke,
            },
            // Titles
            TitleLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 22f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            TitleMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            TitleSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.SemiBold,
                TextColor = Color.FromArgb(255, 0, 0, 0),
            },
            // Body Styles
            BodyLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 16f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.WhiteSmoke,
            },
            BodyMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.WhiteSmoke,
            },
            BodySmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Regular,
                TextColor = Color.WhiteSmoke,
            },

            // Labels
            LabelLarge = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 14f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 212, 175),
            },
            LabelMedium = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 12f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Bold,
                TextColor = Color.FromArgb(255, 212, 175),
            },
            LabelSmall = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontStyle = FontStyle.Regular,
                FontWeight = FontWeight.Normal,
                TextColor = Color.FromArgb(255, 212, 175),
            },

            // **Color Palette** (additional properties)
            PrimaryColor = Color.FromArgb(255, 212, 175),  // Metallic Gold
            SecondaryColor = Color.WhiteSmoke,
            AccentColor = Color.FromArgb(255, 255, 215, 0), // Gold
            BackgroundColor = Color.FromArgb(255, 20, 20, 20),
            SurfaceColor = Color.FromArgb(255, 30, 30, 30),
            ErrorColor = Color.FromArgb(255, 192, 0, 0),   // Dark Red
            WarningColor = Color.FromArgb(255, 218, 165, 32), // Goldenrod
            SuccessColor = Color.FromArgb(255, 0, 128, 0), // Dark Green
            OnPrimaryColor = Color.FromArgb(255, 0, 0, 0),
            OnBackgroundColor = Color.WhiteSmoke,

            // **Spacing and Layout**
            PaddingSmall = 4,
            PaddingMedium = 8,
            PaddingLarge = 16,
            BorderRadius = 0,

            // **Imagery and Iconography**
            IconSet = "LuxuryGoldIcons",
            ApplyThemeToIcons = true,

            // **Effects and Decorations**
            ShadowColor = Color.FromArgb(100, 0, 0, 0), // Semi-transparent black
            ShadowOpacity = 0.2f,

            // **Animation and Transitions**
            AnimationDurationShort = 150, // in milliseconds
            AnimationDurationMedium = 300,
            AnimationDurationLong = 500,
            AnimationEasingFunction = "ease-in-out",

            // **Accessibility**
            HighContrastMode = false,
            FocusIndicatorColor = Color.FromArgb(255, 212, 175),

            // **Theme Variant**
            IsDarkTheme = true,
        };
        #endregion "Themes"
        #region "Theme Management"
        public static EnumBeepThemes GetThemeToEnum(BeepTheme theme)
        {
            if (theme == WinterTheme)
                return EnumBeepThemes.WinterTheme;
            else if (theme == CandyTheme)
                return EnumBeepThemes.CandyTheme;
            else if (theme == ZenTheme)
                return EnumBeepThemes.ZenTheme;
            else if (theme == RetroTheme)
                return EnumBeepThemes.RetroTheme;
            else if (theme == RoyalTheme)
                return EnumBeepThemes.RoyalTheme;
            else if (theme == HighlightTheme)
                return EnumBeepThemes.HighlightTheme;
            else if (theme == DarkTheme)
                return EnumBeepThemes.DarkTheme;
            else if (theme == LightTheme)
                return EnumBeepThemes.LightTheme;
            else if (theme == PastelTheme)
                return EnumBeepThemes.PastelTheme;
            else if (theme == MidnightTheme)
                return EnumBeepThemes.MidnightTheme;
            else if (theme == SpringTheme)
                return EnumBeepThemes.SpringTheme;
            else if (theme == NeonTheme)
                return EnumBeepThemes.NeonTheme;
            else if (theme == RusticTheme)
                return EnumBeepThemes.RusticTheme;
            else if (theme == GalaxyTheme)
                return EnumBeepThemes.GalaxyTheme;
            else if (theme == DesertTheme)
                return EnumBeepThemes.DesertTheme;
            else if (theme == VintageTheme)
                return EnumBeepThemes.VintageTheme;
            else if (theme == ModernDarkTheme)
                return EnumBeepThemes.ModernDarkTheme;
            else if (theme == MaterialDesignTheme)
                return EnumBeepThemes.MaterialDesignTheme;
            else if (theme == NeumorphismTheme)
                return EnumBeepThemes.NeumorphismTheme;
            else if (theme == GlassmorphismTheme)
                return EnumBeepThemes.GlassmorphismTheme;
            else if (theme == FlatDesignTheme)
                return EnumBeepThemes.FlatDesignTheme;
            else if (theme == CyberpunkNeonTheme)
                return EnumBeepThemes.CyberpunkNeonTheme;
            else if (theme == GradientBurstTheme)
                return EnumBeepThemes.GradientBurstTheme;
            else if (theme == HighContrastTheme)
                return EnumBeepThemes.HighContrastTheme;
            else if (theme == MonochromeTheme)
                return EnumBeepThemes.MonochromeTheme;
            else if (theme == LuxuryGoldTheme)
                return EnumBeepThemes.LuxuryGoldTheme;
            else if (theme == OceanTheme)
                return EnumBeepThemes.OceanTheme;
            else if (theme == SunsetTheme)
                return EnumBeepThemes.SunsetTheme;
            else if (theme == ForestTheme)
                return EnumBeepThemes.ForestTheme;
            else if (theme == EarthyTheme)
                return EnumBeepThemes.EarthyTheme;
            else if (theme == AutumnTheme)
                return EnumBeepThemes.AutumnTheme;

            return EnumBeepThemes.DefaultTheme;
        }

        public static BeepTheme GetTheme(string themeName)
        {


            if (string.Equals(themeName, "WinterTheme", StringComparison.OrdinalIgnoreCase))
                return WinterTheme;
            else if (string.Equals(themeName, "CandyTheme", StringComparison.OrdinalIgnoreCase))
                return CandyTheme;
            else if (string.Equals(themeName, "ZenTheme", StringComparison.OrdinalIgnoreCase))
                return ZenTheme;
            else if (string.Equals(themeName, "RetroTheme", StringComparison.OrdinalIgnoreCase))
                return RetroTheme;
            else if (string.Equals(themeName, "RoyalTheme", StringComparison.OrdinalIgnoreCase))
                return RoyalTheme;
            else if (string.Equals(themeName, "HighlightTheme", StringComparison.OrdinalIgnoreCase))
                return HighlightTheme;
            else if (string.Equals(themeName, "DarkTheme", StringComparison.OrdinalIgnoreCase))
                return DarkTheme;
            else if (string.Equals(themeName, "LightTheme", StringComparison.OrdinalIgnoreCase))
                return LightTheme;
            else if (string.Equals(themeName, "PastelTheme", StringComparison.OrdinalIgnoreCase))
                return PastelTheme;
            else if (string.Equals(themeName, "MidnightTheme", StringComparison.OrdinalIgnoreCase))
                return MidnightTheme;
            else if (string.Equals(themeName, "SpringTheme", StringComparison.OrdinalIgnoreCase))
                return SpringTheme;
            else if (string.Equals(themeName, "NeonTheme", StringComparison.OrdinalIgnoreCase))
                return NeonTheme;
            else if (string.Equals(themeName, "RusticTheme", StringComparison.OrdinalIgnoreCase))
                return RusticTheme;
            else if (string.Equals(themeName, "GalaxyTheme", StringComparison.OrdinalIgnoreCase))
                return GalaxyTheme;
            else if (string.Equals(themeName, "DesertTheme", StringComparison.OrdinalIgnoreCase))
                return DesertTheme;
            else if (string.Equals(themeName, "VintageTheme", StringComparison.OrdinalIgnoreCase))
                return VintageTheme;
            else if (string.Equals(themeName, "ModernDarkTheme", StringComparison.OrdinalIgnoreCase))
                return ModernDarkTheme;
            else if (string.Equals(themeName, "MaterialDesignTheme", StringComparison.OrdinalIgnoreCase))
                return MaterialDesignTheme;
            else if (string.Equals(themeName, "NeumorphismTheme", StringComparison.OrdinalIgnoreCase))
                return NeumorphismTheme;
            else if (string.Equals(themeName, "GlassmorphismTheme", StringComparison.OrdinalIgnoreCase))
                return GlassmorphismTheme;
            else if (string.Equals(themeName, "FlatDesignTheme", StringComparison.OrdinalIgnoreCase))
                return FlatDesignTheme;
            else if (string.Equals(themeName, "CyberpunkNeonTheme", StringComparison.OrdinalIgnoreCase))
                return CyberpunkNeonTheme;
            else if (string.Equals(themeName, "GradientBurstTheme", StringComparison.OrdinalIgnoreCase))
                return GradientBurstTheme;
            else if (string.Equals(themeName, "HighContrastTheme", StringComparison.OrdinalIgnoreCase))
                return HighContrastTheme;
            else if (string.Equals(themeName, "MonochromeTheme", StringComparison.OrdinalIgnoreCase))
                return MonochromeTheme;
            else if (string.Equals(themeName, "LuxuryGoldTheme", StringComparison.OrdinalIgnoreCase))
                return LuxuryGoldTheme;
            else if (string.Equals(themeName, "OceanTheme", StringComparison.OrdinalIgnoreCase))
                return OceanTheme;
            else if (string.Equals(themeName, "SunsetTheme", StringComparison.OrdinalIgnoreCase))
                return SunsetTheme;
            else if (string.Equals(themeName, "ForestTheme", StringComparison.OrdinalIgnoreCase))
                return ForestTheme;
            else if (string.Equals(themeName, "EarthyTheme", StringComparison.OrdinalIgnoreCase))
                return EarthyTheme;
            else if (string.Equals(themeName, "AutumnTheme", StringComparison.OrdinalIgnoreCase))
                return AutumnTheme;

            return DefaultTheme;
        }

        public static BeepTheme GetTheme(EnumBeepThemes theme)
        {
            if (theme == EnumBeepThemes.WinterTheme)
                return WinterTheme;
            else if (theme == EnumBeepThemes.CandyTheme)
                return CandyTheme;
            else if (theme == EnumBeepThemes.ZenTheme)
                return ZenTheme;
            else if (theme == EnumBeepThemes.RetroTheme)
                return RetroTheme;
            else if (theme == EnumBeepThemes.RoyalTheme)
                return RoyalTheme;
            else if (theme == EnumBeepThemes.HighlightTheme)
                return HighlightTheme;
            else if (theme == EnumBeepThemes.DarkTheme)
                return DarkTheme;
            else if (theme == EnumBeepThemes.LightTheme)
                return LightTheme;
            else if (theme == EnumBeepThemes.PastelTheme)
                return PastelTheme;
            else if (theme == EnumBeepThemes.MidnightTheme)
                return MidnightTheme;
            else if (theme == EnumBeepThemes.SpringTheme)
                return SpringTheme;
            else if (theme == EnumBeepThemes.NeonTheme)
                return NeonTheme;
            else if (theme == EnumBeepThemes.RusticTheme)
                return RusticTheme;
            else if (theme == EnumBeepThemes.GalaxyTheme)
                return GalaxyTheme;
            else if (theme == EnumBeepThemes.DesertTheme)
                return DesertTheme;
            else if (theme == EnumBeepThemes.VintageTheme)
                return VintageTheme;
            else if (theme == EnumBeepThemes.ModernDarkTheme)
                return ModernDarkTheme;
            else if (theme == EnumBeepThemes.MaterialDesignTheme)
                return MaterialDesignTheme;
            else if (theme == EnumBeepThemes.NeumorphismTheme)
                return NeumorphismTheme;
            else if (theme == EnumBeepThemes.GlassmorphismTheme)
                return GlassmorphismTheme;
            else if (theme == EnumBeepThemes.FlatDesignTheme)
                return FlatDesignTheme;
            else if (theme == EnumBeepThemes.CyberpunkNeonTheme)
                return CyberpunkNeonTheme;
            else if (theme == EnumBeepThemes.GradientBurstTheme)
                return GradientBurstTheme;
            else if (theme == EnumBeepThemes.HighContrastTheme)
                return HighContrastTheme;
            else if (theme == EnumBeepThemes.MonochromeTheme)
                return MonochromeTheme;
            else if (theme == EnumBeepThemes.LuxuryGoldTheme)
                return LuxuryGoldTheme;
            else if (theme == EnumBeepThemes.OceanTheme)
                return OceanTheme;
            else if (theme == EnumBeepThemes.SunsetTheme)
                return SunsetTheme;
            else if (theme == EnumBeepThemes.ForestTheme)
                return ForestTheme;
            else if (theme == EnumBeepThemes.EarthyTheme)
                return EarthyTheme;
            else if (theme == EnumBeepThemes.AutumnTheme)
                return AutumnTheme;

            return DefaultTheme;
        }
        public static EnumBeepThemes GetEnumFromTheme(string themeName)
        {


            if (string.Equals(themeName, "WinterTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.WinterTheme;
            else if (string.Equals(themeName, "CandyTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.CandyTheme;
            else if (string.Equals(themeName, "ZenTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.ZenTheme;
            else if (string.Equals(themeName, "RetroTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.RetroTheme;
            else if (string.Equals(themeName, "RoyalTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.RoyalTheme;
            else if (string.Equals(themeName, "HighlightTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.HighlightTheme;
            else if (string.Equals(themeName, "DarkTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.DarkTheme;
            else if (string.Equals(themeName, "LightTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.LightTheme;
            else if (string.Equals(themeName, "PastelTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.PastelTheme;
            else if (string.Equals(themeName, "MidnightTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.MidnightTheme;
            else if (string.Equals(themeName, "SpringTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.SpringTheme;
            else if (string.Equals(themeName, "NeonTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.NeonTheme;
            else if (string.Equals(themeName, "RusticTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.RusticTheme;
            else if (string.Equals(themeName, "GalaxyTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.GalaxyTheme;
            else if (string.Equals(themeName, "DesertTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.DesertTheme;
            else if (string.Equals(themeName, "VintageTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.VintageTheme;
            else if (string.Equals(themeName, "ModernDarkTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.ModernDarkTheme;
            else if (string.Equals(themeName, "MaterialDesignTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.MaterialDesignTheme;
            else if (string.Equals(themeName, "NeumorphismTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.NeumorphismTheme;
            else if (string.Equals(themeName, "GlassmorphismTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.GlassmorphismTheme;
            else if (string.Equals(themeName, "FlatDesignTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.FlatDesignTheme;
            else if (string.Equals(themeName, "CyberpunkNeonTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.CyberpunkNeonTheme;
            else if (string.Equals(themeName, "GradientBurstTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.GradientBurstTheme;
            else if (string.Equals(themeName, "HighContrastTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.HighContrastTheme;
            else if (string.Equals(themeName, "MonochromeTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.MonochromeTheme;
            else if (string.Equals(themeName, "LuxuryGoldTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.LuxuryGoldTheme;
            else if (string.Equals(themeName, "OceanTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.OceanTheme;
            else if (string.Equals(themeName, "SunsetTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.SunsetTheme;
            else if (string.Equals(themeName, "ForestTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.ForestTheme;
            else if (string.Equals(themeName, "EarthyTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.EarthyTheme;
            else if (string.Equals(themeName, "AutumnTheme", StringComparison.OrdinalIgnoreCase))
                return EnumBeepThemes.AutumnTheme;

            return EnumBeepThemes.DefaultTheme;
        }
        public static string GetTheme(BeepTheme theme)
        {
            if (theme == WinterTheme)
                return EnumBeepThemes.WinterTheme.ToString();
            else if (theme == CandyTheme)
                return EnumBeepThemes.CandyTheme.ToString();
            else if (theme == ZenTheme)
                return EnumBeepThemes.ZenTheme.ToString();
            else if (theme == RetroTheme)
                return EnumBeepThemes.RetroTheme.ToString();
            else if (theme == RoyalTheme)
                return EnumBeepThemes.RoyalTheme.ToString();
            else if (theme == HighlightTheme)
                return EnumBeepThemes.HighlightTheme.ToString();
            else if (theme == DarkTheme)
                return EnumBeepThemes.DarkTheme.ToString();
            else if (theme == LightTheme)
                return EnumBeepThemes.LightTheme.ToString();
            else if (theme == PastelTheme)
                return EnumBeepThemes.PastelTheme.ToString();
            else if (theme == MidnightTheme)
                return EnumBeepThemes.MidnightTheme.ToString();
            else if (theme == SpringTheme)
                return EnumBeepThemes.SpringTheme.ToString();
            else if (theme == NeonTheme)
                return EnumBeepThemes.NeonTheme.ToString();
            else if (theme == RusticTheme)
                return EnumBeepThemes.RusticTheme.ToString();
            else if (theme == GalaxyTheme)
                return EnumBeepThemes.GalaxyTheme.ToString();
            else if (theme == DesertTheme)
                return EnumBeepThemes.DesertTheme.ToString();
            else if (theme == VintageTheme)
                return EnumBeepThemes.VintageTheme.ToString();
            else if (theme == ModernDarkTheme)
                return EnumBeepThemes.ModernDarkTheme.ToString();
            else if (theme == MaterialDesignTheme)
                return EnumBeepThemes.MaterialDesignTheme.ToString();
            else if (theme == NeumorphismTheme)
                return EnumBeepThemes.NeumorphismTheme.ToString();
            else if (theme == GlassmorphismTheme)
                return EnumBeepThemes.GlassmorphismTheme.ToString();
            else if (theme == FlatDesignTheme)
                return EnumBeepThemes.FlatDesignTheme.ToString();
            else if (theme == CyberpunkNeonTheme)
                return EnumBeepThemes.CyberpunkNeonTheme.ToString();
            else if (theme == GradientBurstTheme)
                return EnumBeepThemes.GradientBurstTheme.ToString();
            else if (theme == HighContrastTheme)
                return EnumBeepThemes.HighContrastTheme.ToString();
            else if (theme == MonochromeTheme)
                return EnumBeepThemes.MonochromeTheme.ToString();
            else if (theme == LuxuryGoldTheme)
                return EnumBeepThemes.LuxuryGoldTheme.ToString();
            else if (theme == OceanTheme)
                return EnumBeepThemes.OceanTheme.ToString();
            else if (theme == SunsetTheme)
                return EnumBeepThemes.SunsetTheme.ToString();
            else if (theme == ForestTheme)
                return EnumBeepThemes.ForestTheme.ToString();

            else if (theme == EarthyTheme)
                return EnumBeepThemes.EarthyTheme.ToString();
            else
                return EnumBeepThemes.DefaultTheme.ToString();
        }
        public static List<string> GetThemesNames()
        {
            List<string> themes = new List<string>();
            foreach (EnumBeepThemes theme in Enum.GetValues(typeof(EnumBeepThemes)))
            {
                //if(theme== EnumBeepThemes.GlassmorphismTheme)
                //{
                //    continue;
                //}
                themes.Add(theme.ToString());
            }
            return themes;
        }
        public static List<BeepTheme> GetThemes()
        {
            List<BeepTheme> themes = new List<BeepTheme>();
            foreach (EnumBeepThemes theme in Enum.GetValues(typeof(EnumBeepThemes)))
            {
                //if (theme == EnumBeepThemes.GlassmorphismTheme)
                //{
                //    continue;
                //}
                themes.Add(GetTheme(theme));
            }
            return themes;
        }
      
        public static string GetThemeName(EnumBeepThemes theme)
        {
            return theme.ToString();
        }
        #endregion "Theme Management"
        #region "Font Management"
        public static Font ToFont(TypographyStyle style)
        {
            if (style == null)
            {
                // Fallback to default font if style is null
                return new Font("Segoe UI",9f, FontStyle.Regular);
            }

            // Ensure the font family is valid and installed
            string fontFamily = IsFontInstalled(style.FontFamily) ? style.FontFamily : "Segoe UI";

            // Map FontWeight to a proportional font size adjustment
            float adjustedFontSize = style.FontSize;// * GetFontWeightMultiplier(style.FontWeight);

            // Create the font style from provided values
            FontStyle fontStyle = style.FontStyle;

            // Add underline and strikeout styles if applicable
            if (style.IsUnderlined) fontStyle |= FontStyle.Underline;
            if (style.IsStrikeout) fontStyle |= FontStyle.Strikeout;

            try
            {
                // Create and return the font
                return new Font(fontFamily, adjustedFontSize, fontStyle);
            }
            catch (ArgumentException)
            {
                // Fallback to default system font in case of an error
                return new Font("Segoe UI", adjustedFontSize, FontStyle.Regular);
            }
        }
        private static float GetFontWeightMultiplier(FontWeight weight)
        {
            return weight switch
            {
                FontWeight.Thin => 0.9f,       // Slightly smaller
                FontWeight.ExtraLight => 0.95f,
                FontWeight.Light => 0.97f,
                FontWeight.Normal or FontWeight.Regular => 1.0f, // Combine cases
                FontWeight.Medium => 1.05f,
                FontWeight.SemiBold => 1.1f,
                FontWeight.Bold => 1.15f,     // Slightly larger
                FontWeight.ExtraBold => 1.2f,
                FontWeight.Black => 1.25f,    // Heaviest weight, larger size
                _ => 1.0f                     // Default multiplier for unknown values
            };
        }

        private static bool IsFontInstalled(string fontFamily)
        {
            using (InstalledFontCollection fontsCollection = new InstalledFontCollection())
            {
                return fontsCollection.Families.Any(f => f.Name.Equals(fontFamily, StringComparison.InvariantCultureIgnoreCase));
            }
        }
        // Function to calculate a multiplier based on FontWeight
       

        public static Font ToFont(string fontFamily, float fontSize, FontWeight fontWeight, FontStyle fontStyle)
        {
            try
            {
                // Check if the font family is installed
                if (!IsFontInstalled(fontFamily))
                {
                    // Fallback to default font family
                    fontFamily = "Segoe UI";
                }

                // Initialize finalFontStyle with Regular
                FontStyle finalFontStyle = FontStyle.Regular;

                // Map FontWeight to FontStyle
                if (fontWeight == FontWeight.Bold || fontWeight == FontWeight.SemiBold || fontWeight == FontWeight.Medium)
                {
                    finalFontStyle |= FontStyle.Bold;
                }

                // Combine with provided FontStyle
                finalFontStyle |= fontStyle;

                // Create and return the desired font
                return new Font(fontFamily, fontSize, finalFontStyle);
            }
            catch (ArgumentException ex)
            {
                // Optionally log the exception
                // LogException(ex);

                // Fallback to default system font if the specified font is unavailable
                return new Font("Segoe UI", fontSize, FontStyle.Regular);
            }
        }
        #endregion ""Font Management"
        #region "Theme Save/Load"

        public static void SaveTheme(BeepTheme theme, string filePath)
        {
            var serializer = new XmlSerializer(typeof(BeepTheme));
            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, theme);
            }
        }
        //public static void AddTheme(EnumBeepThemes themeEnum, BeepTheme theme)
        //{
        //    if (!EnumToThemeMap.ContainsKey(themeEnum))
        //    {
        //        EnumToThemeMap[themeEnum] = theme;
        //        //ThemeToEnumMap[theme] = themeEnum;
        //    }
        //}

        //public static void RemoveTheme(EnumBeepThemes themeEnum)
        //{
        //    if (EnumToThemeMap.TryGetValue(themeEnum, out BeepTheme theme))
        //    {
        //        EnumToThemeMap.Remove(themeEnum);
        //       // ThemeToEnumMap.Remove(theme);
        //    }
        //}

        public static BeepTheme LoadTheme(string filePath)
        {
            var serializer = new XmlSerializer(typeof(BeepTheme));
            using (var reader = new StreamReader(filePath))
            {
                return (BeepTheme)serializer.Deserialize(reader);
            }
        }
        #endregion "Theme Save/Load"
        #region "Functions"

        public static EnumBeepThemes CurrentTheme
        {
            get
            {
                return _currentTheme;
            }
            set
            {
                if (_currentTheme != value)
                {
                  //  Debug.WriteLine("Theme Changed to 1" + value.ToString());
                    EnumBeepThemes oldTheme = _currentTheme;
                    _currentTheme = value;
                    ThemeChangeEventsArgs x = new() { NewTheme = _currentTheme, OldTheme = oldTheme };
                    ThemeChanged?.Invoke(null, x);
                   
                }
            }
        }
        private static EnumBeepThemes _currentTheme = EnumBeepThemes.DefaultTheme;

        public static EnumBeepThemes GetCurrentTheme()
        {
            return _currentTheme;
        }
        public static event EventHandler<ThemeChangeEventsArgs> ThemeChanged;
        public static void SetCurrentTheme(EnumBeepThemes theme)
        {
            if (_currentTheme != theme)
            {
             //   Debug.WriteLine("Theme Changed to 2" + theme.ToString());
                EnumBeepThemes oldTheme = _currentTheme;
                _currentTheme = theme;
                ThemeChangeEventsArgs x = new() { NewTheme = _currentTheme, OldTheme = oldTheme };
                ThemeChanged?.Invoke(null, x);
            }
        
        }
        public static bool ThemeExists(string themeName)
        {
            return Enum.IsDefined(typeof(EnumBeepThemes), themeName);
        }

      
        public static string GetGuidFromTheme(BeepTheme theme)
        {
            return theme?.ThemeGuid ?? DefaultTheme.ThemeGuid;
        }
        #endregion "Functions"
    }
    public class ThemeChangeEventsArgs : EventArgs
    {
        public EnumBeepThemes OldTheme { get; set; }
        public EnumBeepThemes NewTheme { get; set; }
    }
    public enum EnumBeepThemes
    {
        DefaultTheme,
        WinterTheme,
        CandyTheme,
        ZenTheme,
        RetroTheme,
        RoyalTheme,
        HighlightTheme,
        DarkTheme,
        OceanTheme,
        LightTheme,
        PastelTheme,
        MidnightTheme,
        SpringTheme,
        ForestTheme,
        NeonTheme,
        RusticTheme,
        GalaxyTheme,
        DesertTheme,
        VintageTheme,
        ModernDarkTheme,
        MaterialDesignTheme, NeumorphismTheme,
        GlassmorphismTheme, FlatDesignTheme,
        CyberpunkNeonTheme, GradientBurstTheme,
        HighContrastTheme, MonochromeTheme,
        LuxuryGoldTheme,
        SunsetTheme,
        AutumnTheme,
        EarthyTheme
    }
}

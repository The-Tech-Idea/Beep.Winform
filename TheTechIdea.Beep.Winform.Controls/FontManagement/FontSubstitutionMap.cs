using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.FontManagement
{
    /// <summary>
    /// What a font family is FOR. A substitute must preserve this or the layout breaks:
    /// a proportional stand-in for a monospace family destroys column alignment in code
    /// listings and tables, and a display face where a UI face was asked for reads as noise.
    /// </summary>
    internal enum FontCharacter
    {
        SansUi,
        Serif,
        Monospace,
        Display
    }

    /// <summary>
    /// The single authority for "if this family is not installed, what preserves its character?".
    /// Consulted only from <see cref="FontListHelper"/> so that one component decides substitution
    /// for every caller — painters, themes and controls alike.
    /// </summary>
    /// <remarks>
    /// Keys are <see cref="FontListHelper"/>-normalized: lowercase with spaces, dashes and
    /// underscores removed ("JetBrains Mono" -> "jetbrainsmono").
    /// Deliberately absent: an alias mapping "Segoe UI Italic" -> "Segoe UI". That is a malformed
    /// theme value (a STYLE used as a family name) and is fixed in the theme part files; aliasing
    /// it here would hide the defect and let the next one in.
    /// </remarks>
    internal static class FontSubstitutionMap
    {
        private static readonly Dictionary<string, FontCharacter> Character =
            new Dictionary<string, FontCharacter>(StringComparer.Ordinal)
        {
            // ── Monospace: families that carry code and column alignment ──
            ["jetbrainsmono"]   = FontCharacter.Monospace,
            ["firacode"]        = FontCharacter.Monospace,
            ["firamono"]        = FontCharacter.Monospace,
            ["sourcecodepro"]   = FontCharacter.Monospace,
            ["robotomono"]      = FontCharacter.Monospace,
            ["spacemono"]       = FontCharacter.Monospace,
            ["cascadiacode"]    = FontCharacter.Monospace,
            ["cascadiamono"]    = FontCharacter.Monospace,
            ["dejavusansmono"]  = FontCharacter.Monospace,
            ["ibmplexmono"]     = FontCharacter.Monospace,
            ["sfmono"]          = FontCharacter.Monospace,
            ["menlo"]           = FontCharacter.Monospace,
            ["monaco"]          = FontCharacter.Monospace,

            // ── UI sans: the bulk of the shipped themes ──
            ["roboto"]          = FontCharacter.SansUi,
            ["robotocondensed"] = FontCharacter.SansUi,
            ["inter"]           = FontCharacter.SansUi,
            ["notosans"]        = FontCharacter.SansUi,
            ["sfprotext"]       = FontCharacter.SansUi,   // Apple-licensed: substitution is the ONLY option
            ["sfprodisplay"]    = FontCharacter.SansUi,   // ditto
            ["segoeuivariable"] = FontCharacter.SansUi,   // Windows 11 only; Windows 10 falls to Segoe UI
            ["cantarell"]       = FontCharacter.SansUi,
            ["ubuntu"]          = FontCharacter.SansUi,
            ["nunito"]          = FontCharacter.SansUi,
            ["nunitosans"]      = FontCharacter.SansUi,
            ["poppins"]         = FontCharacter.SansUi,
            ["montserrat"]      = FontCharacter.SansUi,
            ["sourcesans3"]     = FontCharacter.SansUi,
            ["sourcesanspro"]   = FontCharacter.SansUi,
            ["sora"]            = FontCharacter.SansUi,
            ["rajdhani"]        = FontCharacter.SansUi,
            ["opensans"]        = FontCharacter.SansUi,
            ["lato"]            = FontCharacter.SansUi,
            ["worksans"]        = FontCharacter.SansUi,
            ["dmsans"]          = FontCharacter.SansUi,
            ["manrope"]         = FontCharacter.SansUi,
            ["lexend"]          = FontCharacter.SansUi,
            ["cairo"]           = FontCharacter.SansUi,
            ["tajawal"]         = FontCharacter.SansUi,
            ["oxygen"]          = FontCharacter.SansUi,
            ["atkinsonhyperlegible"] = FontCharacter.SansUi,
            ["opendyslexic"]    = FontCharacter.SansUi,

            // ── Serif ──
            ["merriweather"]    = FontCharacter.Serif,
            ["playfairdisplay"] = FontCharacter.Serif,
            ["lora"]            = FontCharacter.Serif,
            ["ptserif"]         = FontCharacter.Serif,
            ["notoserif"]       = FontCharacter.Serif,
            ["sourceserif4"]    = FontCharacter.Serif,
            ["ibmplexserif"]    = FontCharacter.Serif,
            ["robotoslab"]      = FontCharacter.Serif,

            // ── Display / decorative ──
            ["bebasneue"]       = FontCharacter.Display,
            ["orbitron"]        = FontCharacter.Display,
            ["exo2"]            = FontCharacter.Display,
            ["comicneue"]       = FontCharacter.Display,
            ["caprasimo"]       = FontCharacter.Display,
        };

        // Chains, not single names: a stripped Windows install or a Server SKU can lack Consolas.
        private static readonly string[] MonospaceChain = { "Consolas", "Cascadia Mono", "Courier New", "Lucida Console" };
        private static readonly string[] SansChain      = { "Segoe UI", "Tahoma", "Verdana", "Arial", "Microsoft Sans Serif" };
        private static readonly string[] SerifChain     = { "Georgia", "Times New Roman", "Cambria" };
        private static readonly string[] DisplayChain   = { "Segoe UI Semibold", "Segoe UI", "Arial" };

        /// <summary>
        /// Classifies a normalized family name. Families added to a theme after this table was
        /// written fall to a name heuristic rather than silently becoming UI-sans.
        /// </summary>
        internal static FontCharacter Classify(string normalizedName)
        {
            if (string.IsNullOrEmpty(normalizedName)) return FontCharacter.SansUi;
            if (Character.TryGetValue(normalizedName, out var known)) return known;

            // Order matters: "sourcecodepro" must read as mono, and "notosans" must not read as serif.
            if (normalizedName.Contains("mono") || normalizedName.Contains("code") ||
                normalizedName.Contains("consol") || normalizedName.Contains("courier"))
                return FontCharacter.Monospace;
            if (normalizedName.Contains("sans")) return FontCharacter.SansUi;
            if (normalizedName.Contains("serif") || normalizedName.Contains("slab"))
                return FontCharacter.Serif;
            return FontCharacter.SansUi;
        }

        /// <summary>Ordered substitute families for a requested family. Never empty.</summary>
        internal static IReadOnlyList<string> GetSubstituteChain(string normalizedName)
        {
            switch (Classify(normalizedName))
            {
                case FontCharacter.Monospace: return MonospaceChain;
                case FontCharacter.Serif:     return SerifChain;
                case FontCharacter.Display:   return DisplayChain;
                default:                      return SansChain;
            }
        }

        /// <summary>Human-readable character, for the substitution report.</summary>
        internal static string DescribeCharacter(string normalizedName)
        {
            switch (Classify(normalizedName))
            {
                case FontCharacter.Monospace: return "monospace";
                case FontCharacter.Serif:     return "serif";
                case FontCharacter.Display:   return "display";
                default:                      return "ui-sans";
            }
        }
    }
}

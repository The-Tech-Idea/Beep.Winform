using System;

namespace TheTechIdea.Beep.Icons
{
    internal static class SvgResourcePathHelper
    {
        internal static string Build(string baseNamespace, string fileName)
        {
            if (string.IsNullOrWhiteSpace(baseNamespace))
                return Normalize(fileName);

            if (string.IsNullOrWhiteSpace(fileName))
                return Normalize(baseNamespace);

            string ns = Normalize(baseNamespace).TrimEnd('.');
            string name = Normalize(fileName).TrimStart('.');

            if (name.StartsWith(ns + ".", StringComparison.OrdinalIgnoreCase) ||
                name.Equals(ns, StringComparison.OrdinalIgnoreCase))
            {
                return name;
            }

            return $"{ns}.{name}";
        }

        internal static string Normalize(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return path ?? string.Empty;

            string normalized = path.Trim();

            // Support legacy BeepSvgPathsExtensions format.
            if (normalized.StartsWith("resource://", StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized.Substring("resource://".Length);
            }

            normalized = normalized.Replace("\\", ".").Replace("/", ".");

            while (normalized.Contains("..", StringComparison.Ordinal))
            {
                normalized = normalized.Replace("..", ".", StringComparison.Ordinal);
            }

            // Fix known malformed legacy pattern: "...GFX.SVGsend.svg"
            normalized = normalized.Replace(".GFX.SVGsend.svg", ".GFX.SVG.send.svg", StringComparison.OrdinalIgnoreCase);

            return normalized.Trim('.');
        }
    }
}

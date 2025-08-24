using Svg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    public static class BeepSvgPathsExtensions
    {
        /// <summary>
        /// Sets the image path of a BeepImage control using an embedded SVG resource.
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <param name="svgResourcePath">The SVG resource path from Svgs</param>
        public static void SetSvgFromResource(this BeepImage beepImage, string svgResourcePath)
        {
            if (beepImage == null) throw new ArgumentNullException(nameof(beepImage));
            if (string.IsNullOrEmpty(svgResourcePath)) throw new ArgumentNullException(nameof(svgResourcePath));

            // For embedded resources, we use a special format that BeepImage can recognize
            beepImage.ImagePath = $"resource://{svgResourcePath}";
        }

        //    /// <summary>
        //    /// Loads an SVG from embedded resources and sets it on the BeepImage.
        //    /// </summary>
        //    /// <param name="beepImage">The BeepImage control</param>
        //    /// <param name="svgResourcePath">The SVG resource path from Svgs</param>
        public static void LoadSvgFromEmbeddedResource(this BeepImage beepImage, string svgResourcePath)
        {
            if (beepImage == null) throw new ArgumentNullException(nameof(beepImage));
            if (string.IsNullOrEmpty(svgResourcePath)) throw new ArgumentNullException(nameof(svgResourcePath));

            try
            {
                var (isSvg, result) = ImageLoader.LoadFromEmbeddedResource(svgResourcePath, Svgs.ResourceAssembly);
                if (isSvg && result is SvgDocument svgDoc)
                {
                    beepImage.Image = svgDoc.Draw();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load SVG resource: {svgResourcePath}", ex);
            }
        }
    }
}

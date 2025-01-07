using System.Reflection;

using Svg;


namespace TheTechIdea.Beep.Desktop.Common
{
    public enum ImageSourceType
    {
        LocalFile,
        EmbeddedResource
    }

    public static class ImagePreviewer
    {
        /// <summary>
        /// Previews an image in a PictureBox, whether it's from a local file 
        /// or an embedded resource. The logic is determined by 'sourceType'.
        /// </summary>
        /// <param name="pictureBox">Target PictureBox</param>
        /// <param name="pathOrResource">Either a file path (if LocalFile) or resource name (if EmbeddedResource)</param>
        /// <param name="sourceType">Indicates whether 'pathOrResource' is a local file or embedded resource</param>
        /// <param name="assembly">If sourceType is EmbeddedResource, 
        /// you can optionally specify which assembly to look in. Defaults to executing assembly.</param>
        public static void PreviewImage(
            PictureBox pictureBox,
            string pathOrResource,
            ImageSourceType sourceType,
            Assembly assembly = null)
        {
            pictureBox.Image?.Dispose(); // Clear old image

            switch (sourceType)
            {
                case ImageSourceType.LocalFile:
                    PreviewLocalFile(pictureBox, pathOrResource);
                    break;

                case ImageSourceType.EmbeddedResource:
                    PreviewEmbedded(pictureBox, pathOrResource, assembly);
                    break;
            }
        }

        /// <summary>
        /// Overload that defaults to local files, for convenience.
        /// </summary>
        public static void PreviewImage(PictureBox pictureBox, string filePath)
        {
            PreviewImage(pictureBox, filePath, ImageSourceType.LocalFile);
        }

        /// <summary>
        /// Overload that defaults to embedded resources, for convenience.
        /// </summary>
        public static void PreviewImage(PictureBox pictureBox, string resourceName, Assembly assembly)
        {
            PreviewImage(pictureBox, resourceName, ImageSourceType.EmbeddedResource, assembly);
        }

        private static void PreviewLocalFile(PictureBox pictureBox, string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                MessageBox.Show("Invalid or missing file path.", "Load Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string extension = Path.GetExtension(filePath).ToLower();
                if (extension == ".svg")
                {
                    var svgDoc = ImageLoader.LoadSvg(filePath);
                    pictureBox.Image = svgDoc.Draw();
                }
                else
                {
                    pictureBox.Image = ImageLoader.LoadRegularImage(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}",
                                "Load Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private static void PreviewEmbedded(PictureBox pictureBox, string resourceName, Assembly assembly)
        {
            assembly ??= Assembly.GetExecutingAssembly();
            try
            {
                var (isSvg, result) = ImageLoader.LoadFromEmbeddedResource(resourceName, assembly);

                if (isSvg && result is SvgDocument svgDoc)
                {
                    pictureBox.Image = svgDoc.Draw();
                }
                else if (result is Image img)
                {
                    pictureBox.Image = img;
                }
                else
                {
                    MessageBox.Show($"Failed to interpret embedded resource '{resourceName}'.",
                                    "Load Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading embedded resource: {ex.Message}",
                                "Load Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }
    }
}

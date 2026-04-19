using System;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public enum ImageType
    {
        Icon,
        Png,
        Jpg,
        Gif,
        Bmp,
        Tiff,
        Svg,
        Emf,
        Wmf,
        Exif,
        MemoryBmp,
        Tga,
        Dds,
        Jpeg2000,
        Ico,
        Cur,
        Wmp,
        Webp,
        Heif,
        Heic,
        Avif,
        Bpg,
        Flif,
        Jxl,
        Jph,

    }
    public class ImageConfiguration
    {
        public ImageConfiguration() { }
        public int Index { get; set; }
        public string GuidID { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public string Ext { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public string ImageType { get; set; }
        public bool IsResxEmbedded { get; set; }
        public bool IsProjResource { get; set; }
        public bool IsFile { get; set; }
        public bool IsUrl { get; set; }
        public bool IsBase64 { get; set; }
        public bool IsMemoryStream { get; set; }
        public bool IsStream { get; set; }
        public bool IsImage { get; set; }
        public bool IsIcon { get; set; }
        public bool IsSVG { get; set; }

        public Size Size { get; set; }
        /// <summary>
        /// Instead of storing an Assembly object,
        /// we store the full name or path of the assembly.
        /// e.g. "MyAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" 
        /// or "C:\path\to\MyAssembly.dll".
        /// </summary>
        public string AssemblyFullName { get; set; }
        public string AssemblyLocation { get; set; }
    }
}

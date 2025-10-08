using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public enum RegionDock { Caption, Bottom, Left, Right, ContentOverlay }

    public enum FormStyle
    {
        Modern,         // Borderless, custom caption with rounded corners
        Classic,        // Traditional Windows caption and borders
        Minimal,        // Thin border, minimal caption
        MacOS,          // macOS-style traffic lights (red/yellow/green)
        Fluent,         // Microsoft Fluent Design System
        Material        // Material Design 3
    }

    public sealed class FormRegion
    {
        public string Id { get; set; }
        public RegionDock Dock { get; set; }
        public Rectangle Bounds { get; set; }
        public Action<Graphics, Rectangle> OnPaint { get; set; }
        public object Tag { get; set; }
    }

    public sealed class HitArea
    {
        public string Name { get; set; }
        public Rectangle Bounds { get; set; }
        public object Data { get; set; }
    }

    public class RegionEventArgs : EventArgs
    {
        public string RegionName { get; }
        public FormRegion Region { get; }
        public Rectangle Bounds { get; }

        public RegionEventArgs(string regionName, FormRegion region, Rectangle bounds)
        {
            RegionName = regionName;
            Region = region;
            Bounds = bounds;
        }
    }
}
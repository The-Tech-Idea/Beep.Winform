﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Design.UIEditor;

namespace TheTechIdea.Beep.Winform.Controls.Design.Models
{
    public interface IImageSelector
    {
        string PreviewImage(string initialPath);
        DialogResult ShowDialog();
        string SelectedImagePath { get; set; }
    }
    public static class ImageSelector
    {
        public static IImageSelector Selector { get; set; }
        public static string SelectImage(string initialPath)
        {
            Selector = new BeepImageSelectorDialog();
            Selector.PreviewImage(initialPath);
            Selector.ShowDialog();
            return Selector.SelectedImagePath;
        }
        public static void SetSelector(IImageSelector selector)
        {
            Selector = selector;
        }
        public static void SetSelector()
        {
            Selector = new ImageSelectorImporterForm();
        }
    }

}

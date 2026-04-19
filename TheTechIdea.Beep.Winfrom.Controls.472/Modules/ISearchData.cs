using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;


namespace TheTechIdea.Beep.Vis.Modules
{
    public interface ISearchDataBoxSettings
    {
      
        IDM_Addin SearchBoxTarget { get; set; }
        string SelectedSearchText { get; set; }
        int SelectedIndex { get; set; }
        SearchDataItem SelectedsearchDataItem { get; set; }
        List<SearchDataItem> DataItems { get; set; }    
    }
    public class SearchDataBoxSettings : ISearchDataBoxSettings
    {
        public IDM_Addin SearchBoxTarget { get  ; set  ; }
        public string SelectedSearchText { get  ; set  ; }
        public int SelectedIndex { get  ; set  ; }
        public SearchDataItem SelectedsearchDataItem { get  ; set  ; }
        public List<SearchDataItem> DataItems { get; set; } =new List<SearchDataItem>();
    }
    public class SearchDataItem
    {
        public string Text { get; set; } = string.Empty;
        public int Index { get; set; }
        public int Key { get; set; }
        public string Value { get; set; }
        IDM_Addin TargetAddin { get; set; }

    }
}

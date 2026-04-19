using System;
using System.Collections.Generic;
using System.Text;

namespace TheTechIdea.Beep.Vis.Modules
{
    public class BeepProfile : IProfile
    {
        public string HomePageUrl { get  ; set  ; }
        public string LoginPageTitle { get  ; set  ; }
        public string LoginPageDescription { get  ; set  ; }
        public string LoginPageUrl { get  ; set  ; }
        public string ConfigurationPageUrl { get  ; set  ; }
        public string ConfigurationPageTitle { get  ; set  ; }
        public string ConfigurationPageDescription { get  ; set  ; }
        public string ProfilePageUrl { get  ; set  ; }
        public string ProfilePageTitle { get  ; set  ; }
        public string ProfilePageDescription { get  ; set  ; }
        public bool IsFavourite { get  ; set  ; }
    }
    public interface IProfile
    {
        string HomePageUrl { get; set; }
        string LoginPageTitle { get; set; }
        string LoginPageDescription { get; set; }
        string LoginPageUrl { get; set; }

        string ConfigurationPageUrl { get; set; }
        string ConfigurationPageTitle { get; set; }
        string ConfigurationPageDescription { get; set; }

        string ProfilePageUrl { get; set; }
        string ProfilePageTitle { get; set; }
        string ProfilePageDescription { get; set; }
       
        bool IsFavourite { get; set; }


    }
}

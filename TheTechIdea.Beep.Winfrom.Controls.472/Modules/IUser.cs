using System;
using System.Collections.Generic;
using System.Text;

namespace TheTechIdea.Beep.Vis.Modules
{
    public class BeepUser : IBeepUser
    {
        public BeepUser()
        {
                GuidID=Guid.NewGuid().ToString();   
        }
        public string GuidID { get; set; }
        public string FirstName { get ; set ; }
        public string LastName { get ; set ; }
        public string Email { get ; set ; }
        public string Password { get ; set ; }
        public string EmailConfirmed { get ; set ; }
        public string PasswordConfirmed { get ; set ; }
        public string Address { get ; set ; }
        public string City { get ; set ; }
        public string Region { get ; set ; }
        public string PostalCode { get ; set ; }
        public string Country { get ; set ; }
        public string Fax { get ; set ; }
        public string FaxNumber { get ; set ; }
        public string PhoneNumber { get ; set ; }
        public string PhoneNumberConfirmed { get ; set ; }
        public string Company { get ; set ; }
        public string CompanyNumber { get ; set ; }
        public string Department { get ; set ; }
        public string Position { get ; set ; }
        public List<IBeepPrivilege> Privileges { get ; set ; }
        public IProfile Profile { get ; set ; }
        public bool IsLoggedin { get ; set ; }=false;
        public bool IsAdmin { get; set; } = false;
    }
    public interface IBeepUser
    {
        string GuidID { get ; set ; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string EmailConfirmed { get; set; }
        string PasswordConfirmed { get; set; }
        
        string Address { get; set; }
        string City { get; set; }
        string Region { get; set; }
        string PostalCode { get; set; }
        string Country { get; set; }
        string Fax { get; set; }
        string FaxNumber { get; set; }
        string PhoneNumber { get; set; }    
        string PhoneNumberConfirmed { get; set; }

        string Company { get; set; }
        string CompanyNumber { get; set; }
        string Department { get; set; }
        string Position { get; set; }
        List<IBeepPrivilege>   Privileges { get; set; }
        IProfile Profile { get; set; }
        bool IsLoggedin { get; set; }
        bool IsAdmin { get; set; }

    }
}

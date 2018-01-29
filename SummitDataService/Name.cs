using System;
using System.Runtime.Serialization;

namespace CSharpNameParser
{

    public class Name : IComparable<Name>
    {
 
        public string Salutation { get; set; }

 
        public string FirstName { get; set; }


        public string MiddleInitials { get; set; }


        public string LastName { get; set; }


        public string Suffix { get; set; }

        public int CompareTo(Name other)
        {
            // Compare last names.
            var diff = LastName.ToLower().CompareTo(other.LastName.ToLower());
            if (diff != 0) return diff;

            // Compare first names.
            diff = FirstName.ToLower().CompareTo(other.FirstName.ToLower());
            if (diff != 0) return diff;

            // Compare Middle initials
            diff = MiddleInitials.ToLower().CompareTo(other.MiddleInitials.ToLower());
            if (diff != 0) return diff;

            return 0;
        }
    }
}
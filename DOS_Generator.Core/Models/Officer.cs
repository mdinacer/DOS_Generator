using System.Collections.Generic;

namespace DOS_Generator.Core.Models
{
    public class Officer : EntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Title { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        
        public string Initials { get; set; }
        public string TemplatePath { get; set; }
        public virtual ICollection<Declaration> Declarations { get; set; }
    }
}
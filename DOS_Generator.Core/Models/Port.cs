using System.Collections.Generic;

namespace DOS_Generator.Core.Models
{
    public class Port : EntityBase
    {
        public string Name { get; set; }
        public virtual ICollection<Facility> Facilities { get; set; }
    }
}
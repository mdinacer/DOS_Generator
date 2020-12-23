using System.Collections.Generic;

namespace DOS_Generator.Core.Models
{
    public class Agency : EntityBase
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public virtual ICollection<Ship> Ships { get; set; }
    }
}
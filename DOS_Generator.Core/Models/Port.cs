using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DOS_Generator.Core.Models
{
    public class Port : EntityBase
    {
        public string Name { get; set; }
        public virtual ICollection<Facility> Facilities { get; set; }

        public Port()
        {
            Facilities = new ObservableCollection<Facility>();
        }
    }
}
namespace DOS_Generator.Core.Models
{
    public class Facility : EntityBase
    {
        public string Name { get; set; }
        public int PortId { get; set; }
        public virtual Port Port { get; set; }

    }
}
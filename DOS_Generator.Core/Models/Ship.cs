namespace DOS_Generator.Core.Models
{
    public class Ship : EntityBase
    {     
        public string ImoNumber { get; set; }
        public string Name { get; set; }
        public string PortOfRegistry { get; set; }  
        public string Email { get; set; }
        public int? AgencyId { get; set; }
        public virtual Agency Agency { get; set; }
    }
}
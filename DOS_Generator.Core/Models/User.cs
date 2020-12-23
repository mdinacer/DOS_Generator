namespace DOS_Generator.Core.Models
{
    public class User : EntityBase
    {
        public string Name { get; set; }
        public string Hash { get; set; }
        public int OfficerId { get; set; }
        public virtual Officer Officer { get; set; }
    }
}
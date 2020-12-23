namespace DOS_Generator.Core.Models
{
    [AddINotifyPropertyChangedInterface]
    public abstract class EntityBase
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}
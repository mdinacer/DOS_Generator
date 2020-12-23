using System;

namespace DOS_Generator.Core.Models
{
    public class Declaration : EntityBase
    {
        public DateTime Date { get; set; }
        public int ShipId { get; set; }
        public virtual Ship Ship { get; set; }
        public int OfficerId { get; set; }
        public virtual Officer Officer { get; set; }
        public int PortId { get; set; }
        public virtual Port Port { get; set; }
        public int FacilityId { get; set; }
        public virtual Facility Facility { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public OperationType Operation { get; set; }
        public string SecLevel { get; set; }
        public bool IsSent { get; set; }
        public bool IsReceived { get; set; }
        
    }
}
using System;

namespace DOS_Generator.License
{
    [Serializable]
    public class License
    {
        public DateTime PurchaseDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string LicensedTo { get; set; }
        public LicenseType Type { get; set; }
        public string MachineId { get; set; }
    }
}
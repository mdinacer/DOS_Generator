namespace DOS_Generator.Core.Models
{
    public class MailServer : EntityBase
    {
        public string ServiceName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
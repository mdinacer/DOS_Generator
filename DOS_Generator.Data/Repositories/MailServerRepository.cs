using DOS_Generator.Core.Models;
using DOS_Generator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DOS_Generator.Data.Repositories
{
    public class MailServerRepository : Repository<MailServer>, IMailServerRepository
    {
        public MailServerRepository(DbContext context) : base(context)
        {
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using DOS_Generator.Core;
using DOS_Generator.Core.Models;
using DOS_Generator.WPF.Domain;
using BC = BCrypt.Net.BCrypt;

namespace DOS_Generator.WPF.Services
{
    public class AccountService
    {
        public static string HashText(string value)
        {
            return BC.HashPassword(value);
        }

        public static User Register(LoginModel model)
        {
            var user = new User
            {
                Name = model.UserName,
                Hash = BC.HashPassword($"{model.UserName.ToLower()}{model.Password}")
            };
            return user;
        }

        public static async Task<bool> Authenticate(LoginModel model, IUnitOfWork unitOfWork)
        {
            // get account from database
            var account = (await unitOfWork.Users.GetAllAsync())
                .SingleOrDefault(o => o.Name.Equals(model.UserName, StringComparison.CurrentCultureIgnoreCase));

            // check account found and verify password
            return account != null && BC.Verify($"{model.UserName.ToLower()}{model.Password}", account.Hash);
        }
    }
}
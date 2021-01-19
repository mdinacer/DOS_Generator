using System.Text.RegularExpressions;

namespace DOS_Generator.WPF.Helpers
{
    public static class EmailAddressCheck
    {
        public static bool CheckEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email) 
                   && Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                       RegexOptions.IgnoreCase);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using BCrypt.Net;
using System.Text.RegularExpressions;


namespace cSharp_BankSystem
{
    public class User
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public List<Account> Accounts { get; set; }

        public User()
        {
            Accounts = new List<Account>();
        }

        public User(string name, string email, string hashedPassword)
        {
            Name = name;
            Email = email;
            HashedPassword = hashedPassword;
            Accounts = new List<Account>();
        }

        public  bool IsStrongPassword(string password)
        {
            // Define regular expressions for each criterion
            Regex minLengthRegex = new Regex(@".{8,}");
            Regex uppercaseRegex = new Regex(@"[A-Z]");
            Regex lowercaseRegex = new Regex(@"[a-z]");
            Regex digitRegex = new Regex(@"\d");
            Regex symbolRegex = new Regex(@"[!@#$%^&*()_+{}\[\]:;<>,.?~\\-]");

            // Check each criterion
            bool hasMinLength = minLengthRegex.IsMatch(password);
            bool hasUppercase = uppercaseRegex.IsMatch(password);
            bool hasLowercase = lowercaseRegex.IsMatch(password);
            bool hasDigit = digitRegex.IsMatch(password);
            bool hasSymbol = symbolRegex.IsMatch(password);

            // Check if all criteria are met
            return hasMinLength && hasUppercase && hasLowercase && hasDigit && hasSymbol;
        }
    }
}

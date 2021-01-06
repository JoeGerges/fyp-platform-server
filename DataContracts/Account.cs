using System;
using System.Collections.Generic;
using System.Text;

namespace FYPPlatform.DataContracts
{
    public class Account
    {
        public string Name { get; set; } 
        public string Email { get; set; }
        public string Organization { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public List<string> Skills { get; set; }
        public List<string> Interests { get; set; }
        public string State { get; set; }
        public string Token { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Account account &&
                   Email == account.Email;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Email);
        }
    }
}

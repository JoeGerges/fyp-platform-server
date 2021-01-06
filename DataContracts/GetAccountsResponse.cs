using System;
using System.Collections.Generic;
using System.Text;

namespace FYPPlatform.DataContracts
{
    public class GetAccountsResponse
    {
        public List<Account> Accounts { get; set; }
        public string NextUri { get; set; }
    }
}

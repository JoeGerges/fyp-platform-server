using System;
using System.Collections.Generic;
using System.Text;

namespace FYPPlatform.DataContracts
{
    public class GetAccountsResult
    {

        public string ContinuationToken { get; set; }
        public List<Account> Accounts { get; set; }
        
    }
}

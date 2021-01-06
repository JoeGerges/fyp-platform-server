using FYPPlatform.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FYPPlatform.Web.Storage
{
    public interface IAccountStore
    {
        Task AddAccount(Account account);
        Task<Account> GetAccount(string id);
        Task DeleteAccount(string id);
        Task UpdateAccount(Account account);
        Task<GetAccountsResult> GetAccounts(string continuationToken, int limit, string role);
    }
}

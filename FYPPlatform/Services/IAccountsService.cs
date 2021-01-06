using FYPPlatform.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FYPPlatform.Web.Services
{
    public interface IAccountsService
    {
        Task<Account> GetAccount(string email);
        Task<Account> AuthenticateAccount(string email, string password);
        Task AddAccount(Account account);
        Task UpdateAccount(Account account);
        Task DeleteAccount(string email);
        Task<GetAccountsResult> GetAccounts(string continuationToken, int limit, string role);
    }
}

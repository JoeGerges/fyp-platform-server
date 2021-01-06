using FYPPlatform.DataContracts;
using FYPPlatform.Web.Exceptions;
using FYPPlatform.Web.Storage;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FYPPlatform.Web.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly IAccountStore _accountStore;
        private readonly AppSettings _appSettings;
        public AccountsService(IAccountStore accountStore, IOptions<AppSettings> appSettings)
        {
            _accountStore = accountStore;
            _appSettings = appSettings.Value;
        }
        public async Task AddAccount(Account account)
        {
            ThrowBadRequestIfAccountIsInvalid(account);
            await _accountStore.AddAccount(account);
        }

        public async Task DeleteAccount(string email)
        {
            await _accountStore.DeleteAccount(email);
        }

        public async Task<Account> GetAccount(string email)
        {
            var account = await _accountStore.GetAccount(email);
            account.Password = null;
            return account;
        }

        public async Task<Account> AuthenticateAccount(string email, string password)
        {
            var account = await _accountStore.GetAccount(email);
            if (account.Password != password)
                return null;
            var token = generateJwtToken(account);
            account.Token = token;
            account.Password = null;
            return account;
        }

        public async Task UpdateAccount(Account account)
        {
            ThrowBadRequestIfAccountIsInvalid(account);
            if(account.Password == null)
            {
                throw new BadRequestException("Please provide a password for your account");
            }
            var oldAccount = await _accountStore.GetAccount(account.Email);
            await _accountStore.UpdateAccount(account);
        }

        public async Task<GetAccountsResult> GetAccounts(string continuationToken, int limit, string role)
        {
            var accountsResult = await _accountStore.GetAccounts(continuationToken, limit, role);
            return accountsResult;
        }

        private void ThrowBadRequestIfAccountIsInvalid(Account account)
        {
            string error = null;

            if (string.IsNullOrWhiteSpace(account.Name))
            {
                error = "The account's username cannot be empty";
            }
            if (string.IsNullOrWhiteSpace(account.Email))
            {
                error = "The account's email cannot be empty";
            }
            if (string.IsNullOrWhiteSpace(account.Organization))
            {
                error = "The account's organization cannot be empty";
            }
            if (string.IsNullOrWhiteSpace(account.Role))
            {
                error = "The account's role cannot be empty";
            }

            if (error != null)
            {
                throw new BadRequestException(error);
            }
        }

        private string generateJwtToken(Account account)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("email", account.Email) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

using FYPPlatform.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FYPPlatform.DataContracts;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace FYPPlatform.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountsService _accountService;

        public AccountsController(IAccountsService accountService)
        {
            _accountService = accountService;
        }

        
        [HttpPost]
        public async Task<IActionResult> PostAccountAsync([FromBody] Account account)
        {
            await _accountService.AddAccount(account);
            var newAccount = await _accountService.GetAccount(account.Email);
            return StatusCode(201, newAccount);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAccountAsync([FromBody] Account account)
        {
            await _accountService.UpdateAccount(account);
            return Ok();
        }

        [Authorize]
        [HttpGet("{email}")]
        public async Task<IActionResult> GetAccountAsync(string email)
        {
            var account = await _accountService.GetAccount(email);
            return Ok(account);
        }

        [Authorize]
        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteAccountAsync(string email)
        {
            await _accountService.DeleteAccount(email);
            return Ok();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllAccountsAsync(string continuationToken, int limit, string role)
        {
            var result = await _accountService.GetAccounts(continuationToken, limit, role);

            string nextUri = null;

            if (!string.IsNullOrWhiteSpace(result.ContinuationToken))
            {
                nextUri = $"api/accounts?continuationToken={WebUtility.UrlEncode(result.ContinuationToken)}&limit={limit}&role={role}";
            }

            var response = new GetAccountsResponse
            {
                NextUri = nextUri,
                Accounts = result.Accounts
            };
            return Ok(response);
        }
    }
}

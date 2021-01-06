using FYPPlatform.DataContracts;
using FYPPlatform.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FYPPlatform.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IAccountsService _accountService;

        public AuthenticateController(IAccountsService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> AuthenticateAccountAsync([FromBody] AuthenticationRequest request)
        {
            var account = await _accountService.AuthenticateAccount(request.Email, request.Password);

            if (account == null)
                return Unauthorized("Invalid Credentials");

            return Ok(account);
        }
    }
}

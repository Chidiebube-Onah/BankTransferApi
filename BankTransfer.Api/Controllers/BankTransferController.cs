using BankTransfer.BLL.Factories;
using BankTransfer.BLL.Interfaces;
using BankTransfer.Models.Dtos.Requests;
using BankTransfer.Models.Dtos.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BankTransfer.Api.Controllers
{
    [ApiController]
    [Route("api/v{v:apiversion}/core-banking")]
    [Authorize]
    [Produces("application/json")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Client Side Related Errors", Type = typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Resource Doesn't exit", Type = typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
    public class BankTransferController : ControllerBase
    {


        private readonly ILogger<BankTransferController> _logger;
        private readonly IApiKeyCacheService _apiKeyCacheService;
        private readonly BankTransferFactory _factory;

        public BankTransferController(ILogger<BankTransferController> logger, IApiKeyCacheService apiKeyCacheService, BankTransferFactory factory)
        {
            _logger = logger;
            _apiKeyCacheService = apiKeyCacheService;
            _factory = factory;
        }

        [HttpGet("banks", Name = "get-banks")]
        [SwaggerOperation(Summary = "Fetches Banks From A Provide")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Response with List of Banks", Type = typeof(IEnumerable<BankResponse>))]
        public async Task<IActionResult> GetBanks(string? provider)
        {
            IEnumerable<BankResponse> res = await _factory.GetBankTransferService(provider)
                .GetBanks();

            return Ok(res);

        }



        [HttpPost("validateBankAccount", Name = "validateBankAccount")]
        [SwaggerOperation(Summary = "Validates a given Bank Account Number")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Bank Account Details", Type = typeof(ValidateAccountResponse))]
        public async Task<IActionResult> ValidateBankAccount(ValidateAccountRequest request)
        {
            ValidateAccountResponse res = await _factory.GetBankTransferService(request.Provider)
                .ValidateAccount(request);

            return Ok(res);

        }


        [HttpPost("bankTransfer", Name = "bankTransfer")]
        [SwaggerOperation(Summary = "Transfer Money to a Bank Account")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Transfer Status", Type = typeof(BankTransferResponse))]
        public async Task<IActionResult> BankTransfer(BankTransferRequest request)
        {
            BankTransferResponse res = await _factory.GetBankTransferService(request.Provider)
                .Transfer(request);
            return Ok(res);

        }


        [HttpGet("transaction/{referenceId}", Name = "getTransaction")]
        [SwaggerOperation(Summary = "Check Transfer Status")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Transaction Status", Type = typeof(BankTransferResponse))]
        public async Task<IActionResult> GetTransaction(string referenceId, string? provider)
        {
            BankTransferResponse res = await _factory.GetBankTransferService(provider)
                .GetTransactionStatus(referenceId);
            return Ok(res);

        }


        [AllowAnonymous]
        [HttpGet("generate-api-key", Name = "generate-api-key")]
        [SwaggerOperation(Summary = "Generate A New Api Key")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "ApiKey", Type = typeof(string))]
        public async Task<IActionResult> GetApiKey()
        {
            string res = await _apiKeyCacheService.GenerateApiKey();
            return Ok(res);
        }


    }
}
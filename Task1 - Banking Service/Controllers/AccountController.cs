using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Task1___Banking_Service.Data; // Assuming _unitOfWork is inside Data namespace
using Task1___Banking_Service.Models;
using Task1___Banking_Service.Repositories;

[ApiController]
[Route("accounts")]
public class AccountController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("{accountId}/details")]
    public async Task<IActionResult> GetAccountDetails(long accountId, [FromHeader(Name = "Accept-Language")] string language = "en")
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(accountId);
        if (account == null) return NotFound(new { Message = "Account not found" });

        // Ensure NameTranslations is not null before accessing
        string accountName = account.NameTranslations != null && account.NameTranslations.ContainsKey(language)
            ? account.NameTranslations[language]
            : account.NameTranslations?["en"] ?? "Unknown Account";

        var response = new
        {
            AccountId = account.Id,
            AccountName = accountName,
            Balance = account.Balance
        };

        return Ok(response);
    }

}
    
    
    

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UpsideAPI.Models;

namespace UpsideAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

  public class BankAccountController : ControllerBase
  {
    private readonly DatabaseContext _context;

    public BankAccountController(DatabaseContext context)
    {
      _context = context;
    }

    [HttpGet]
    public ActionResult GetUserBankAccounts()
    {
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(_context.BankAccounts.Where(acc => acc.UserID == userId)),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUserBankAccount(BankAccount accountToUpdate)
    {
      _context.Entry(accountToUpdate).State = EntityState.Modified;
      await _context.SaveChangesAsync();

      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(accountToUpdate),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpPost]
    public async Task<ActionResult> AddUserBankAccount(BankAccount newAccount)
    {
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      newAccount.UserID = userId;

      _context.BankAccounts.Add(newAccount);

      await _context.SaveChangesAsync();

      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(newAccount),
        ContentType = "application/json",
        StatusCode = 201
      };
    }

    [HttpDelete("{bankAccountId}")]
    public async Task<ActionResult> DeleteUserBankAccount(int bankAccountId)
    {
      var bankAccountToDelete = await _context.BankAccounts.FirstOrDefaultAsync(acc => acc.ID == bankAccountId);
      _context.BankAccounts.Remove(bankAccountToDelete);
      await _context.SaveChangesAsync();

      return Ok();
    }
  }
}
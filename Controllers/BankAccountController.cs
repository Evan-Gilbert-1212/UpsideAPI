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
      //Get User ID from Claims
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      //Return all Bank Accounts for associated User ID
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(_context.BankAccounts
                                              .Where(acc => acc.UserID == userId)
                                              .OrderBy(acc => acc.AccountType)),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUserBankAccount(BankAccount accountToUpdate)
    {
      //Set state of incoming entry to "Modified"
      _context.Entry(accountToUpdate).State = EntityState.Modified;

      //Save changes
      await _context.SaveChangesAsync();

      //Return updated account
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
      //Get User ID from Claims
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      //Add User ID to new Bank Account
      newAccount.UserID = userId;

      //Add Bank Account to table
      _context.BankAccounts.Add(newAccount);

      //Save changes
      await _context.SaveChangesAsync();

      //Return new Bank Account
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
      //Locate Bank Account to delete by ID
      var bankAccountToDelete = await _context.BankAccounts.FirstOrDefaultAsync(acc => acc.ID == bankAccountId);

      //Remove Bank Account from table
      _context.BankAccounts.Remove(bankAccountToDelete);

      //Save Changes
      await _context.SaveChangesAsync();

      //Return OK
      return Ok();
    }
  }
}
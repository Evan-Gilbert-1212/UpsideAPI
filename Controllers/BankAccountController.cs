using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UpsideAPI.Models;

namespace UpsideAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]

  public class BankAccountController : ControllerBase
  {
    public DatabaseContext UpsideDb = new DatabaseContext();

    [HttpGet("{userId}")]
    public ActionResult GetUserAccounts(int userId)
    {
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(UpsideDb.BankAccounts.Where(acc => acc.UserID == userId)),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpPost("{userId}")]
    public async Task<ActionResult> AddUserAccount(BankAccount newAccount, int userId)
    {
      newAccount.UserID = userId;

      UpsideDb.BankAccounts.Add(newAccount);

      await UpsideDb.SaveChangesAsync();

      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(newAccount),
        ContentType = "application/json",
        StatusCode = 201
      };
    }
  }
}
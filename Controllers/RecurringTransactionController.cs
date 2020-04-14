using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UpsideAPI.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace UpsideAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

  public class RecurringTransactionController : ControllerBase
  {
    private readonly DatabaseContext _context;

    public RecurringTransactionController(DatabaseContext context)
    {
      _context = context;
    }

    //Endpoints
    [HttpGet]
    public ActionResult GetUsersRecurringTransactions()
    {
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(_context.RecurringTransactions
                                                .Where(trans => trans.UserID == userId)
                                                .OrderBy(trans => trans.FirstPaymentDate)),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUsersRecurringTransaction(RecurringTransaction transactionToUpdate)
    {
      _context.Entry(transactionToUpdate).State = EntityState.Modified;
      await _context.SaveChangesAsync();

      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(transactionToUpdate),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpDelete("{recurringTransId}")]
    public async Task<ActionResult> DeleteUsersRecurringTransaction(int recurringTransId)
    {
      var transactionToDelete = await _context.RecurringTransactions.FirstOrDefaultAsync(trans => trans.ID == recurringTransId);
      _context.RecurringTransactions.Remove(transactionToDelete);
      await _context.SaveChangesAsync();

      return Ok();
    }
  }
}
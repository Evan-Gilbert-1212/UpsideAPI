using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UpsideAPI.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System;

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
      //Get User ID from Claims
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      //Return all Recurring Transactions for associated User ID, ordered by First Payment Date
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
      //If Recurring Transaction Amount equals 0, return BadRequest. Recurring Transactions must be greater than 0
      if (transactionToUpdate.TransactionAmount == 0)
      {
        return BadRequest("Amount must be greater than 0.");
      }

      //Else, set state of incoming entry to "Modified"
      _context.Entry(transactionToUpdate).State = EntityState.Modified;

      //Save changes
      await _context.SaveChangesAsync();

      //delete previous projections
      if (transactionToUpdate.TransactionType == "Revenue")
      {
        var transactionsToDelete = _context.Revenues.Where(rev => rev.RecurringTransactionID == transactionToUpdate.ID).ToList();
        _context.Revenues.RemoveRange(transactionsToDelete);
      }
      else
      {
        var transactionsToDelete = _context.Expenses.Where(exp => exp.RecurringTransactionID == transactionToUpdate.ID).ToList();
        _context.Expenses.RemoveRange(transactionsToDelete);
      }

      //Save changes
      await _context.SaveChangesAsync();

      //Re-project based on updated recurring transaction entry
      RecurringTransactionManager.ProjectIndividualPayment(transactionToUpdate);

      //Return updated Recurring Transaction 
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
      //Locate Recurring transaction to delete by ID
      var transactionToDelete = await _context.RecurringTransactions.FirstOrDefaultAsync(trans => trans.ID == recurringTransId);

      //Delete all transactions associated to this Recurring Transaction
      if (transactionToDelete.TransactionType == "Revenue")
      {
        var revenuesToDelete = _context.Revenues.Where(rev => rev.RecurringTransactionID == transactionToDelete.ID);

        _context.Revenues.RemoveRange(revenuesToDelete);
      }
      else
      {
        var expensesToDelete = _context.Expenses.Where(exp => exp.RecurringTransactionID == transactionToDelete.ID);

        _context.Expenses.RemoveRange(expensesToDelete);
      }

      //Remove Recurring Transaction record
      _context.RecurringTransactions.Remove(transactionToDelete);

      //Save changes
      await _context.SaveChangesAsync();

      //Return OK
      return Ok();
    }
  }
}
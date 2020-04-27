using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UpsideAPI.Models;
using Newtonsoft.Json;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using UpsideAPI.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace UpsideAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

  public class RevenueController : ControllerBase
  {
    private readonly DatabaseContext _context;

    public RevenueController(DatabaseContext context)
    {
      _context = context;
    }

    [HttpGet]
    public ActionResult GetUserRevenuesForPeriod(DateTime BeginDate, DateTime EndDate)
    {
      //Get User ID from Claims
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      //Return all Revenues for associated User ID, within the date range, ordered by Revenue Date
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(_context.Revenues
                                                .Where(rev =>
                                                  rev.UserID == userId
                                                  && rev.RevenueDate >= BeginDate
                                                  && rev.RevenueDate <= EndDate)
                                                .OrderBy(rev => rev.RevenueDate)),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpGet("all")]
    public ActionResult GetAllUserRevenues()
    {
      //Get User ID from Claims
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      //Return all Revenues for associated User ID, ordered by Revenue Date
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(_context.Revenues
                                                .Where(rev => rev.UserID == userId)
                                                .OrderBy(rev => rev.RevenueDate)),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUserRevenue(Revenue revenueToUpdate)
    {
      //If Revenue Amount equals 0, return BadRequest. Revenues must be greater than 0.
      if (revenueToUpdate.RevenueAmount == 0)
      {
        return BadRequest("Amount must be greater than 0.");
      }

      //Else, set state of incoming entry to "Modified"
      _context.Entry(revenueToUpdate).State = EntityState.Modified;

      //Save changes
      await _context.SaveChangesAsync();

      //Return updated Revenue
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(revenueToUpdate),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpPost]
    public async Task<ActionResult> AddUserRevenue(IncomingRevenueData incomingRevenue)
    {
      //If Revenue Amount equals 0, return BadRequest. Revenues must be greater than 0.
      if (incomingRevenue.RevenueAmount == 0)
      {
        return BadRequest("Amount must be greater than 0.");
      }

      //Else, get User ID from Claims
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      //Create new Revenue
      var newRevenue = new Revenue
      {
        RevenueCategory = incomingRevenue.RevenueCategory,
        RevenueName = incomingRevenue.RevenueName,
        RevenueDate = incomingRevenue.RevenueDate,
        RevenueAmount = incomingRevenue.RevenueAmount,
        UserID = userId
      };

      //If Revenue is recurring, create new Recurring Transaction and project it.
      if (incomingRevenue.RecurringFrequency != "One Time")
      {
        var newRecurringTransaction = new RecurringTransaction
        {
          TransactionType = "Revenue",
          TransactionCategory = incomingRevenue.RevenueCategory,
          TransactionName = incomingRevenue.RevenueName,
          FirstPaymentDate = incomingRevenue.RevenueDate,
          TransactionAmount = incomingRevenue.RevenueAmount,
          RecurringFrequency = incomingRevenue.RecurringFrequency,
          UserID = userId
        };

        _context.RecurringTransactions.Add(newRecurringTransaction);

        await _context.SaveChangesAsync();

        RecurringTransactionManager.ProjectIndividualPayment(newRecurringTransaction);

        newRevenue.RecurringTransactionID = newRecurringTransaction.ID;
      }
      //Else, save One Time Revenue record
      else
      {
        _context.Revenues.Add(newRevenue);

        await _context.SaveChangesAsync();
      }

      //Return new Revenue
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(newRevenue),
        ContentType = "application/json",
        StatusCode = 201
      };
    }

    [HttpDelete("{revenueId}")]
    public async Task<ActionResult> DeleteUserRevenue(int revenueId)
    {
      //Locate Revenue to delete by ID
      var revenueToDelete = await _context.Revenues.FirstOrDefaultAsync(rev => rev.ID == revenueId);

      //Remove Revenue from table
      _context.Revenues.Remove(revenueToDelete);

      //Save changes
      await _context.SaveChangesAsync();

      //Return OK
      return Ok();
    }
  }
}
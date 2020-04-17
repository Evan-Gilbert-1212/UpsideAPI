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
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

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
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

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
      _context.Entry(revenueToUpdate).State = EntityState.Modified;
      await _context.SaveChangesAsync();

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
      if (incomingRevenue.RevenueDate == DateTime.Parse("01/01/1970"))
      {
        return BadRequest("Receipt Date cannot be blank.");
      }

      if (incomingRevenue.RevenueAmount == 0)
      {
        return BadRequest("Amount must be greater than 0.");
      }

      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      var newRevenue = new Revenue
      {
        RevenueCategory = incomingRevenue.RevenueCategory,
        RevenueName = incomingRevenue.RevenueName,
        RevenueDate = incomingRevenue.RevenueDate,
        RevenueAmount = incomingRevenue.RevenueAmount,
        UserID = userId
      };

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
      else
      {
        _context.Revenues.Add(newRevenue);

        await _context.SaveChangesAsync();
      }

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
      var revenueToDelete = await _context.Revenues.FirstOrDefaultAsync(rev => rev.ID == revenueId);
      _context.Revenues.Remove(revenueToDelete);
      await _context.SaveChangesAsync();

      return Ok();
    }
  }
}
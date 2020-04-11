using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UpsideAPI.Models;
using Newtonsoft.Json;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using UpsideAPI.ViewModels;

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

    [HttpPost]
    public async Task<ActionResult> AddUserRevenue(IncomingRevenueData incomingRevenue)
    {
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      var newRevenue = new Revenue
      {
        RevenueCategory = incomingRevenue.RevenueCategory,
        RevenueName = incomingRevenue.RevenueName,
        RevenueDate = incomingRevenue.RevenueDate,
        RevenueAmount = incomingRevenue.RevenueAmount,
        UserID = userId
      };

      _context.Revenues.Add(newRevenue);

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

        RecurringTransactionManager.ProjectIndividualPayment(newRecurringTransaction);
      }

      await _context.SaveChangesAsync();

      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(newRevenue),
        ContentType = "application/json",
        StatusCode = 201
      };
    }
  }
}
using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UpsideAPI.Models;
using UpsideAPI.ViewModels;

namespace UpsideAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

  public class UserController : ControllerBase
  {
    private readonly DatabaseContext _context;

    public UserController(DatabaseContext context)
    {
      _context = context;
    }

    [HttpGet]
    public ActionResult GetUser()
    {
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(_context.Users.Where(user => user.ID == userId)),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpGet("usersummary")]
    public ActionResult GetUserSummary(DateTime BeginDate, DateTime EndDate)
    {
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      var summary = new UserSummary();

      summary.AccountBalance = _context.BankAccounts.Where(acct => acct.UserID == userId).Sum(acct => acct.AccountBalance);
      summary.CreditCardBalance = _context.CreditCards.Where(card => card.UserID == userId).Sum(card => card.AccountBalance);
      summary.RevenueTotal = _context.Revenues
                             .Where(rev =>
                               rev.UserID == userId
                               && rev.RevenueDate >= BeginDate
                               && rev.RevenueDate <= EndDate)
                             .Sum(rev => rev.RevenueAmount);
      summary.ExpenseTotal = _context.Expenses
                             .Where(exp =>
                               exp.UserID == userId
                               && exp.ExpenseDate >= BeginDate
                               && exp.ExpenseDate <= EndDate).Sum(exp => exp.ExpenseAmount);

      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(summary),
        ContentType = "application/json",
        StatusCode = 200,
      };
    }
  }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UpsideAPI.Models;
using UpsideAPI.ViewModels;

namespace UpsideAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]

  public class UserController : ControllerBase
  {
    private DatabaseContext _context;

    public UserController(DatabaseContext context)
    {
      _context = context;
    }

    [HttpGet("{userId}")]
    public ActionResult GetUser(int userId)
    {
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(_context.Users.Where(user => user.ID == userId)),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpGet("usersummary/{userId}")]
    public ActionResult GetUserSummary(int userId, DateTime BeginDate, DateTime EndDate)
    {
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

    [HttpPost]
    public async Task<ActionResult> AddUser(User newUser)
    {
      await _context.Users.AddAsync(newUser);

      await _context.SaveChangesAsync();

      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(newUser),
        ContentType = "application/json",
        StatusCode = 201
      };
    }
  }
}
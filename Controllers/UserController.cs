using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
      //Get User ID from Claims
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      //Return User for associated User ID
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(_context.Users.Where(user => user.ID == userId).FirstOrDefault()),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpGet("usersummary")]
    public ActionResult GetUserSummary()
    {
      //Get User ID from Claims
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      //get Users Display Period Option
      var displayPeriod = _context.Users.Where(user => user.ID == userId).Select(user => user.DisplayPeriod).FirstOrDefault();

      //Create a new Summary object
      var summary = new UserSummary();

      summary.FirstName = User.Claims.FirstOrDefault(claim => claim.Type == "FirstName").Value;

      //If DisplayPeriod is "Wages" look for Wages records to set PeriodBeginDate and PeriodEndDate
      if (displayPeriod == "Wages")
      {
        if (_context.Revenues.Any(rev => rev.RevenueDate <= DateTime.Now
                                      && rev.RevenueCategory == "Wages"
                                      && rev.UserID == userId))
        {
          summary.PeriodBeginDate = _context.Revenues
                                    .Where(rev => rev.RevenueDate <= DateTime.Now
                                        && rev.RevenueCategory == "Wages"
                                        && rev.UserID == userId)
                                    .Max(rev => rev.RevenueDate);
        }
        else
        {
          summary.PeriodBeginDate = DateTime.Now;
        }

        if (_context.Revenues.Any(rev => rev.RevenueDate > DateTime.Now
                                      && rev.RevenueCategory == "Wages"
                                      && rev.UserID == userId))
        {
          summary.PeriodEndDate = _context.Revenues
                                  .Where(rev => rev.RevenueDate > DateTime.Now
                                      && rev.RevenueCategory == "Wages"
                                      && rev.UserID == userId)
                                  .Min(rev => rev.RevenueDate).AddDays(-1);
        }
        else
        {
          summary.PeriodEndDate = DateTime.Now;
        }
      }
      //Else, set PeriodBeginDate and PeriodEndDate to beginning and end of month, respectively
      else if (displayPeriod == "Monthly")
      {
        summary.PeriodBeginDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        summary.PeriodEndDate = summary.PeriodBeginDate.AddMonths(1).AddDays(-1);
      }

      //Get sum of all Bank Accounts
      summary.AccountBalance = _context.BankAccounts
                               .Where(acct => acct.UserID == userId)
                               .Sum(acct => acct.AccountBalance);

      //Get sum of all Credit Cards
      summary.CreditCardBalance = _context.CreditCards
                                  .Where(card => card.UserID == userId)
                                  .Sum(card => card.AccountBalance);

      //Get sum of all Revenues for the period
      summary.RevenueTotal = _context.Revenues
                             .Where(rev => rev.UserID == userId
                               && rev.RevenueDate >= summary.PeriodBeginDate
                               && rev.RevenueDate <= summary.PeriodEndDate)
                             .Sum(rev => rev.RevenueAmount);

      //Get sum of all Expenses for the period
      summary.ExpenseTotal = _context.Expenses
                             .Where(exp => exp.UserID == userId
                               && exp.ExpenseDate >= summary.PeriodBeginDate
                               && exp.ExpenseDate <= summary.PeriodEndDate)
                             .Sum(exp => exp.ExpenseAmount);

      //Return UserSummary
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(summary),
        ContentType = "application/json",
        StatusCode = 200,
      };
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(User userToUpdate)
    {
      //Set state of incoming entry to "Modified"
      _context.Entry(userToUpdate).State = EntityState.Modified;

      //Save changes
      await _context.SaveChangesAsync();

      //Return updated User
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(userToUpdate),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpPatch("updateperiod")]
    public async Task<ActionResult> UpdateUserDisplayPeriod(User user)
    {
      //Get User ID from Claims
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      //Get User for the associated User ID
      var userToUpdate = await _context.Users.Where(user => user.ID == userId).FirstOrDefaultAsync();

      //Update the DisplayPeriod option
      userToUpdate.DisplayPeriod = user.DisplayPeriod;

      //Save Changes
      await _context.SaveChangesAsync();

      //Return updated User
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(userToUpdate),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpDelete("{userId}")]
    public async Task<ActionResult> DeleteUser(int userId)
    {
      //Locate User to delete by ID
      var userToDelete = await _context.Users.FirstOrDefaultAsync(user => user.ID == userId);

      //Remove User from table
      _context.Users.Remove(userToDelete);

      //Save changes
      await _context.SaveChangesAsync();

      //Return OK
      return Ok();
    }
  }
}
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UpsideAPI.Models;
using Newtonsoft.Json;
using System;

namespace UpsideAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]

  public class ExpenseController : ControllerBase
  {
    public DatabaseContext UpsideDb = new DatabaseContext();

    [HttpGet("{userId}")]
    public ActionResult GetUserExpenses(int userId, DateTime BeginDate, DateTime EndDate)
    {
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(UpsideDb.Expenses
                                                .Where(exp =>
                                                  exp.UserID == userId
                                                  && exp.ExpenseDate >= BeginDate
                                                  && exp.ExpenseDate <= EndDate)
                                                .OrderBy(exp => exp.ExpenseDate)),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpPost("{userId}")]
    public async Task<ActionResult> AddUserExpense(Expense newExpense, int userId)
    {
      newExpense.UserID = userId;

      UpsideDb.Expenses.Add(newExpense);

      await UpsideDb.SaveChangesAsync();

      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(newExpense),
        ContentType = "application/json",
        StatusCode = 201
      };
    }
  }
}
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

  public class ExpenseController : ControllerBase
  {
    private readonly DatabaseContext _context;

    public ExpenseController(DatabaseContext context)
    {
      _context = context;
    }

    [HttpGet]
    public ActionResult GetUserExpensesForPeriod(DateTime BeginDate, DateTime EndDate)
    {
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(_context.Expenses
                                                .Where(exp =>
                                                  exp.UserID == userId
                                                  && exp.ExpenseDate >= BeginDate
                                                  && exp.ExpenseDate <= EndDate)
                                                .OrderBy(exp => exp.ExpenseDate)),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpGet("all")]
    public ActionResult GetAllUserExpenses()
    {
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(_context.Expenses
                                                .Where(exp => exp.UserID == userId)
                                                .OrderBy(exp => exp.ExpenseDate)),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpPost]
    public async Task<ActionResult> AddUserExpense(IncomingExpenseData incomingExpense)
    {
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      var newExpense = new Expense
      {
        ExpenseCategory = incomingExpense.ExpenseCategory,
        ExpenseName = incomingExpense.ExpenseName,
        ExpenseDate = incomingExpense.ExpenseDate,
        ExpenseAmount = incomingExpense.ExpenseAmount,
        UserID = userId
      };

      _context.Expenses.Add(newExpense);

      if (incomingExpense.RecurringFrequency != "One Time")
      {
        var newRecurringTransaction = new RecurringTransaction
        {
          TransactionType = "Expense",
          TransactionCategory = incomingExpense.ExpenseCategory,
          TransactionName = incomingExpense.ExpenseName,
          FirstPaymentDate = incomingExpense.ExpenseDate,
          TransactionAmount = incomingExpense.ExpenseAmount,
          RecurringFrequency = incomingExpense.RecurringFrequency,
          UserID = userId
        };

        _context.RecurringTransactions.Add(newRecurringTransaction);

        RecurringTransactionManager.ProjectIndividualPayment(newRecurringTransaction);
      }

      await _context.SaveChangesAsync();

      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(newExpense),
        ContentType = "application/json",
        StatusCode = 201
      };
    }
  }
}
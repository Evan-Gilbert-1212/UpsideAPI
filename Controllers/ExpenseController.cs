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
      //Get User ID from Claims
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      //Return all Expenses for associated User ID, within the date range, ordered by ExpenseDate 
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
      //Get User ID from Claims
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      //Return all Expenses for associated User ID, ordered by ExpenseDate
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(_context.Expenses
                                                .Where(exp => exp.UserID == userId)
                                                .OrderBy(exp => exp.ExpenseDate)),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUserExpense(Expense expenseToUpdate)
    {
      //If Expense Amount is 0, return BadRequest. Expenses must be greater than 0.
      if (expenseToUpdate.ExpenseAmount == 0)
      {
        return BadRequest("Amount must be greater than 0.");
      }

      //Else, set state of incoming entry to "Modified"
      _context.Entry(expenseToUpdate).State = EntityState.Modified;

      //Save changes
      await _context.SaveChangesAsync();

      //Return updated Expense
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(expenseToUpdate),
        ContentType = "application/json",
        StatusCode = 200
      };
    }

    [HttpPost]
    public async Task<ActionResult> AddUserExpense(IncomingExpenseData incomingExpense)
    {
      //If Expense Amount equals 0, return BadRequest. Expenses must be greater than 0.
      if (incomingExpense.ExpenseAmount == 0)
      {
        return BadRequest("Amount must be greater than 0.");
      }

      //Else, get User ID from Claims
      var userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "ID").Value);

      //Create new expense
      var newExpense = new Expense
      {
        ExpenseCategory = incomingExpense.ExpenseCategory,
        ExpenseName = incomingExpense.ExpenseName,
        ExpenseDate = incomingExpense.ExpenseDate,
        ExpenseAmount = incomingExpense.ExpenseAmount,
        UserID = userId
      };

      //If Expense is recurring, create new Recurring Transaction and project it.
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

        await _context.SaveChangesAsync();

        RecurringTransactionManager.ProjectIndividualPayment(newRecurringTransaction);

        newExpense.RecurringTransactionID = newRecurringTransaction.ID;
      }
      //Else, save One Time Expense record
      else
      {
        _context.Expenses.Add(newExpense);

        await _context.SaveChangesAsync();
      }

      //Return new Expense
      return new ContentResult()
      {
        Content = JsonConvert.SerializeObject(newExpense),
        ContentType = "application/json",
        StatusCode = 201
      };
    }

    [HttpDelete("{expenseId}")]
    public async Task<ActionResult> DeleteUserExpense(int expenseId)
    {
      //Locate Expense to delete by ID
      var expenseToDelete = await _context.Expenses.FirstOrDefaultAsync(exp => exp.ID == expenseId);

      //Remove Expense from table
      _context.Expenses.Remove(expenseToDelete);

      //Save changes
      await _context.SaveChangesAsync();

      //Return OK
      return Ok();
    }
  }
}
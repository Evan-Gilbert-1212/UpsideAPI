using System;
using System.Linq;
using UpsideAPI.Models;

namespace UpsideAPI
{
  public class RecurringTransactionManager
  {
    public static DateTime IncrementDate(DateTime dateToIncrement, string recurringFrequency)
    {
      //Get next recurring payment date
      if (recurringFrequency == "Weekly")
        return dateToIncrement.AddDays(7);
      else if (recurringFrequency == "Bi-Weekly")
        return dateToIncrement.AddDays(14);
      else if (recurringFrequency == "Monthly")
        return dateToIncrement.AddMonths(1);
      else if (recurringFrequency == "Annually")
        return dateToIncrement.AddYears(1);
      else
        return dateToIncrement;
    }

    public static void ProjectPayments(int userId)
    {
      //Establish database connection
      var UpsideDb = new DatabaseContext();

      //Get recurring transactions for user
      var transactionsToProject = UpsideDb.RecurringTransactions.Where(trans => trans.UserID == userId).ToList();

      //Loop through all recurring transactions for a user
      foreach (var trans in transactionsToProject)
      {
        var nextPaymentDate = IncrementDate(trans.FirstPaymentDate, trans.RecurringFrequency);
        var endProjectionDate = DateTime.Now.AddYears(1);

        //loop through each recurring transaction and project them
        while (nextPaymentDate <= endProjectionDate)
        {
          if (trans.TransactionType == "Revenue")
          {
            //check if revenue already exists
            if (!UpsideDb.Revenues.Any(rev => rev.RevenueCategory == trans.TransactionCategory &&
                                              rev.RevenueDate == nextPaymentDate))
            {
              //if not, add revenue to table
              var newRevenue = new Revenue()
              {
                RevenueCategory = trans.TransactionCategory,
                RevenueName = trans.TransactionName,
                RevenueDate = nextPaymentDate,
                RevenueAmount = trans.TransactionAmount,
                UserID = trans.UserID
              };

              UpsideDb.Revenues.Add(newRevenue);
            }
          }
          else if (trans.TransactionType == "Expense")
          {
            //check if expense already exists
            if (!UpsideDb.Expenses.Any(exp => exp.ExpenseCategory == trans.TransactionCategory &&
                                              exp.ExpenseDate == nextPaymentDate))
            {
              //if not, add expense to table
              var newExpense = new Expense()
              {
                ExpenseCategory = trans.TransactionCategory,
                ExpenseName = trans.TransactionName,
                ExpenseDate = nextPaymentDate,
                ExpenseAmount = trans.TransactionAmount,
                UserID = trans.UserID
              };

              UpsideDb.Expenses.Add(newExpense);
            }
          }

          //increment nextPaymentDate
          nextPaymentDate = IncrementDate(nextPaymentDate, trans.RecurringFrequency);
        }
      }

      UpsideDb.SaveChanges();
    }
  }
}
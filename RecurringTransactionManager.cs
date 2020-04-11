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

    public static void ProjectIndividualPayment(RecurringTransaction transaction)
    {
      var UpsideDb = new DatabaseContext();

      var nextPaymentDate = IncrementDate(transaction.FirstPaymentDate, transaction.RecurringFrequency);
      var endProjectionDate = DateTime.Now.AddYears(1);

      //loop through transaction and project
      while (nextPaymentDate <= endProjectionDate)
      {
        if (transaction.TransactionType == "Revenue")
        {
          //check if revenue already exists
          if (!UpsideDb.Revenues.Any(rev => rev.RevenueCategory == transaction.TransactionCategory &&
                                            rev.RevenueDate == nextPaymentDate))
          {
            //if not, add revenue to table
            var newRevenue = new Revenue()
            {
              RevenueCategory = transaction.TransactionCategory,
              RevenueName = transaction.TransactionName,
              RevenueDate = nextPaymentDate,
              RevenueAmount = transaction.TransactionAmount,
              UserID = transaction.UserID
            };

            UpsideDb.Revenues.Add(newRevenue);
          }
        }
        else if (transaction.TransactionType == "Expense")
        {
          //check if expense already exists
          if (!UpsideDb.Expenses.Any(exp => exp.ExpenseCategory == transaction.TransactionCategory &&
                                            exp.ExpenseDate == nextPaymentDate))
          {
            //if not, add expense to table
            var newExpense = new Expense()
            {
              ExpenseCategory = transaction.TransactionCategory,
              ExpenseName = transaction.TransactionName,
              ExpenseDate = nextPaymentDate,
              ExpenseAmount = transaction.TransactionAmount,
              UserID = transaction.UserID
            };

            UpsideDb.Expenses.Add(newExpense);
          }
        }

        //increment nextPaymentDate
        nextPaymentDate = IncrementDate(nextPaymentDate, transaction.RecurringFrequency);
      }

      UpsideDb.SaveChanges();
    }

    public static void ProjectAllPayments(int userId)
    {
      //Establish database connection
      var UpsideDb = new DatabaseContext();

      //Get recurring transactions for user
      var transactionsToProject = UpsideDb.RecurringTransactions.Where(trans => trans.UserID == userId).ToList();

      //Loop through all recurring transactions for a user
      foreach (var trans in transactionsToProject)
      {
        ProjectIndividualPayment(trans);
      }
    }
  }
}
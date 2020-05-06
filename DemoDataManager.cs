using System;
using UpsideAPI.Models;

namespace UpsideAPI
{
  public class DemoDataManager
  {
    public static void CreateDemoData(int userID)
    {
      var UpsideDb = new DatabaseContext();

      //Add a Checking Account
      var checkingAcct = new BankAccount
      {
        AccountType = "Checking",
        AccountBalance = 12495.89,
        UserID = userID
      };

      UpsideDb.BankAccounts.Add(checkingAcct);

      //Add a Savings Account
      var savingsAcct = new BankAccount
      {
        AccountType = "Savings",
        AccountBalance = 23598.45,
        UserID = userID
      };

      UpsideDb.BankAccounts.Add(savingsAcct);

      //Add Bank of America Credit Card
      var bofACard = new CreditCard
      {
        CardIssuer = "Bank of America",
        AccountBalance = 549.70,
        UserID = userID
      };

      UpsideDb.CreditCards.Add(bofACard);

      //Add Citibank Credit Card
      var citiCard = new CreditCard
      {
        CardIssuer = "Citibank",
        AccountBalance = 357.65,
        UserID = userID
      };

      UpsideDb.CreditCards.Add(citiCard);

      //Add Recurring Wages
      var firstSalaryPayment = new RecurringTransaction
      {
        TransactionType = "Revenue",
        TransactionCategory = "Wages",
        TransactionName = "Paycheck on 8th",
        FirstPaymentDate = DateTime.Parse("4/8/2020"),
        TransactionAmount = 2000,
        RecurringFrequency = "Monthly",
        UserID = userID
      };

      UpsideDb.RecurringTransactions.Add(firstSalaryPayment);

      var secondSalaryPayment = new RecurringTransaction
      {
        TransactionType = "Revenue",
        TransactionCategory = "Wages",
        TransactionName = "Paycheck on 22nd",
        FirstPaymentDate = DateTime.Parse("4/22/2020"),
        TransactionAmount = 2000,
        RecurringFrequency = "Monthly",
        UserID = userID
      };

      UpsideDb.RecurringTransactions.Add(secondSalaryPayment);

      //Add 2 one time expenses and 2 recurring expenses for next 2 periods
      var groceries = new Expense
      {
        ExpenseCategory = "Food",
        ExpenseName = "Groceries at Publix",
        ExpenseDate = DateTime.Parse("4/23/2020"),
        ExpenseAmount = 143.76,
        UserID = userID
      };

      UpsideDb.Expenses.Add(groceries);

      var movies = new Expense
      {
        ExpenseCategory = "Entertainment",
        ExpenseName = "1917 Movie Rental",
        ExpenseDate = DateTime.Parse("4/29/2020"),
        ExpenseAmount = 20,
        UserID = userID
      };

      UpsideDb.Expenses.Add(movies);

      var groceriesTwo = new Expense
      {
        ExpenseCategory = "Food",
        ExpenseName = "Groceries at Publix",
        ExpenseDate = DateTime.Parse("5/10/2020"),
        ExpenseAmount = 133.21,
        UserID = userID
      };

      UpsideDb.Expenses.Add(groceriesTwo);

      var dinnerAndDrinks = new Expense
      {
        ExpenseCategory = "Entertainment",
        ExpenseName = "Friday Happy Hour",
        ExpenseDate = DateTime.Parse("5/15/2020"),
        ExpenseAmount = 56.50,
        UserID = userID
      };

      UpsideDb.Expenses.Add(dinnerAndDrinks);

      var rent = new RecurringTransaction
      {
        TransactionType = "Expense",
        TransactionCategory = "Rent",
        TransactionName = "Monthly Rent",
        FirstPaymentDate = DateTime.Parse("4/1/2020"),
        TransactionAmount = 1500,
        RecurringFrequency = "Monthly",
        UserID = userID
      };

      UpsideDb.RecurringTransactions.Add(rent);


      var cellPhoneBill = new RecurringTransaction
      {
        TransactionType = "Expense",
        TransactionCategory = "Cell Phone",
        TransactionName = "Monthly - AT&T",
        FirstPaymentDate = DateTime.Parse("4/5/2020"),
        TransactionAmount = 49.99,
        RecurringFrequency = "Monthly",
        UserID = userID
      };

      UpsideDb.RecurringTransactions.Add(cellPhoneBill);


      var internetBill = new RecurringTransaction
      {
        TransactionType = "Expense",
        TransactionCategory = "Cable & Internet",
        TransactionName = "Monthly - Spectrum",
        FirstPaymentDate = DateTime.Parse("4/10/2020"),
        TransactionAmount = 79.99,
        RecurringFrequency = "Monthly",
        UserID = userID
      };

      UpsideDb.RecurringTransactions.Add(internetBill);


      var carInsurance = new RecurringTransaction
      {
        TransactionType = "Expense",
        TransactionCategory = "Car - Insurance",
        TransactionName = "Monthly - State Farm",
        FirstPaymentDate = DateTime.Parse("4/17/2020"),
        TransactionAmount = 124.77,
        RecurringFrequency = "Monthly",
        UserID = userID
      };

      UpsideDb.RecurringTransactions.Add(carInsurance);

      UpsideDb.SaveChanges();

      RecurringTransactionManager.ProjectIndividualPayment(firstSalaryPayment);
      RecurringTransactionManager.ProjectIndividualPayment(secondSalaryPayment);
      RecurringTransactionManager.ProjectIndividualPayment(rent);
      RecurringTransactionManager.ProjectIndividualPayment(cellPhoneBill);
      RecurringTransactionManager.ProjectIndividualPayment(internetBill);
      RecurringTransactionManager.ProjectIndividualPayment(carInsurance);
    }
  }
}
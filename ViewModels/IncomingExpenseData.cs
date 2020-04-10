using System;

namespace UpsideAPI.ViewModels
{
  public class IncomingExpenseData
  {
    public string ExpenseCategory { get; set; }
    public string ExpenseName { get; set; }
    public DateTime ExpenseDate { get; set; }
    public double ExpenseAmount { get; set; }
    public string RecurringFrequency { get; set; }
  }
}
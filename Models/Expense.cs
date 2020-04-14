using System;
using System.Text.Json.Serialization;

namespace UpsideAPI.Models
{
  public class Expense
  {
    public int ID { get; set; }
    public string ExpenseCategory { get; set; }
    public string ExpenseName { get; set; }
    public DateTime ExpenseDate { get; set; }
    public double ExpenseAmount { get; set; }
    public int UserID { get; set; }
    public int? RecurringTransactionID { get; set; }

    [JsonIgnore]
    public User User { get; set; }
  }
}
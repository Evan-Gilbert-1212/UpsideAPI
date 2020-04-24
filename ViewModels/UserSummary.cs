using System;

namespace UpsideAPI.ViewModels
{
  public class UserSummary
  {
    public string FirstName { get; set; }
    public string TimeOfDay { get; set; }
    public DateTime PeriodBeginDate { get; set; }
    public DateTime PeriodEndDate { get; set; }
    public double AccountBalance { get; set; }
    public double CreditCardBalance { get; set; }
    public double ExpenseTotal { get; set; }
    public double RevenueTotal { get; set; }
  }
}
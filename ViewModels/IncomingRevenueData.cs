using System;

namespace UpsideAPI.ViewModels
{
  public class IncomingRevenueData
  {
    public string RevenueCategory { get; set; }
    public string RevenueName { get; set; }
    public DateTime RevenueDate { get; set; }
    public double RevenueAmount { get; set; }
    public string RecurringFrequency { get; set; }
  }
}
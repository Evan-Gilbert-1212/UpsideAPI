using System;
using System.Text.Json.Serialization;

namespace UpsideAPI.Models
{
  public class RecurringTransaction
  {
    public int ID { get; set; }
    public string TransactionType { get; set; }
    public string TransactionCategory { get; set; }
    public string TransactionName { get; set; }
    public DateTime FirstPaymentDate { get; set; }
    public double TransactionAmount { get; set; }
    public string RecurringFrequency { get; set; }
    public int UserID { get; set; }

    [JsonIgnore]
    public User User { get; set; }
  }
}
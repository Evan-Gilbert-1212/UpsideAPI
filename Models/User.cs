using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UpsideAPI.Models
{
  public class User
  {
    public int ID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string DisplayPeriod { get; set; }
    public bool IsDemoAccount { get; set; }
    public DateTime AccountCreatedTime { get; set; } = DateTime.Now;

    [JsonIgnore]
    public string HashedPassword { get; set; }
    public List<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
    public List<CreditCard> CreditCards { get; set; } = new List<CreditCard>();
    public List<Expense> Expenses { get; set; } = new List<Expense>();
    public List<Revenue> Revenues { get; set; } = new List<Revenue>();
  }
}
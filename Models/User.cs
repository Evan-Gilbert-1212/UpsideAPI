using System.Collections.Generic;

namespace UpsideAPI.Models
{
  public class User
  {
    public int ID { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public List<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
    public List<CreditCard> CreditCards { get; set; } = new List<CreditCard>();
    public List<Expense> Expenses { get; set; } = new List<Expense>();
    public List<Revenue> Revenues { get; set; } = new List<Revenue>();
  }
}
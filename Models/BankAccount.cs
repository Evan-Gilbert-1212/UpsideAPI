using System.Text.Json.Serialization;

namespace UpsideAPI.Models
{
  public class BankAccount
  {
    public int ID { get; set; }
    public string AccountType { get; set; }
    public double AccountBalance { get; set; }
    public int UserID { get; set; }

    [JsonIgnore]
    public User User { get; set; }
  }
}
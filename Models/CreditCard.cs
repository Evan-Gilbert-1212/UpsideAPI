using System.Text.Json.Serialization;

namespace UpsideAPI.Models
{
  public class CreditCard
  {
    public int ID { get; set; }
    public string CardIssuer { get; set; }
    public double AccountBalance { get; set; }
    public int UserID { get; set; }

    [JsonIgnore]
    public User User { get; set; }
  }
}
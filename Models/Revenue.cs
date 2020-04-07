using System;
using System.Text.Json.Serialization;

namespace UpsideAPI.Models
{
  public class Revenue
  {
    public int ID { get; set; }
    public string RevenueCategory { get; set; }
    public string RevenueName { get; set; }
    public DateTime RevenueDate { get; set; }
    public double RevenueAmount { get; set; }
    public int UserID { get; set; }

    [JsonIgnore]
    public User User { get; set; }
  }
}
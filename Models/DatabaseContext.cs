using System;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace UpsideAPI.Models
{
  public partial class DatabaseContext : DbContext
  {
    public DbSet<User> Users { get; set; }
    public DbSet<BankAccount> BankAccounts { get; set; }
    public DbSet<CreditCard> CreditCards { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Revenue> Revenues { get; set; }
    public DbSet<RecurringTransaction> RecurringTransactions { get; set; }

    private string ConvertPostConnectionToConnectionString(string connection)
    {
      var _connection = connection.Replace("postgres://", String.Empty);
      var output = Regex.Split(_connection, ":|@|/");
      return $"server={output[2]};database={output[4]};User Id={output[0]}; password={output[1]}; port={output[3]}";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (!optionsBuilder.IsConfigured)
      {
        var envConn = Environment.GetEnvironmentVariable("DATABASE_URL");
        var conn = "server=localhost;database=UpsideDatabase";
        if (envConn != null)
        {
          conn = ConvertPostConnectionToConnectionString(envConn);
        }
        optionsBuilder.UseNpgsql(conn);
      }
    }



  }
}

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UpsideAPI.Models;

namespace UpsideAPI.Services
{
  internal class UpsideHostedService : IHostedService, IDisposable
  {
    private readonly ILogger _logger;
    private readonly DatabaseContext _context;
    private Timer _timer;

    public UpsideHostedService(ILogger<UpsideHostedService> logger, DatabaseContext context)
    {
      _logger = logger;
      _context = context;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("Background Service is starting.");

      //Execute task once a day
      _timer = new Timer(PerformMaintenance, null, TimeSpan.Zero,
          TimeSpan.FromDays(1));

      return Task.CompletedTask;
    }

    private void PerformMaintenance(object state)
    {
      _logger.LogInformation("Background Service is executing.");

      //Remove expired demo user accounts
      //tokens are valid for 10 hours so only remove accounts for which tokens have expired
      var expirationTime = DateTime.Now.AddHours(-10);

      var accountsToDelete = _context.Users
                               .Where(user => user.IsDemoAccount == true &&
                               user.AccountCreatedTime < expirationTime);

      _context.Users.RemoveRange(accountsToDelete);

      _context.SaveChanges();

      //Project Recurring Payments in the system
      RecurringTransactionManager.ProjectAllPayments();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("Background Service is stopping.");

      _timer?.Change(Timeout.Infinite, 0);

      return Task.CompletedTask;
    }

    public void Dispose()
    {
      _timer?.Dispose();
    }
  }
}
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;

namespace Herb.Services.CronJobs
{
    public class LightSystemService : BackgroundService
    {
        private CrontabSchedule _schedule;
        private DateTime _nextRun;
        private readonly ILogger<LightSystemService> _logger;

        private string Schedule => "0 9,21 * * *";

        public LightSystemService(ILogger<LightSystemService> logger)
        {
            _schedule = CrontabSchedule.Parse(Schedule, new CrontabSchedule.ParseOptions { IncludingSeconds = false });
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var now = DateTime.Now;
                if (now > _nextRun)
                {
                    Process();
                    _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                }
                await Task.Delay(5000, stoppingToken);
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        private void Process()
        {
            var pin = Pi.Gpio[BcmPin.Gpio02];
            pin.PinMode = GpioPinDriveMode.Output;

            _logger.LogInformation($"pin value is {pin.Read()}");

            if (pin.Read())
            {
                pin.Write(false);
                _logger.LogInformation("Process job Lights off");
            }
            else
            {
                pin.Write(true);
                _logger.LogInformation("Process job Lights on");
            }

            _logger.LogInformation("Process hit this is where the work will happen");
        }
    }
}

using Herb.Services.CronJobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace Herb
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private ILogger<Startup> _logger;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHostedService<LightSystemService>();
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IHostApplicationLifetime applicationLifetime,
            ILogger<Startup> logger)
        {

            _logger = logger;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            applicationLifetime.ApplicationStarted.Register(OnStart);
            applicationLifetime.ApplicationStopping.Register(OnStop);

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void OnStart()
        {
            Pi.Init<BootstrapWiringPi>();
            _logger.LogInformation("Inside onstart");

            var pin = Pi.Gpio[BcmPin.Gpio23];
            pin.PinMode = GpioPinDriveMode.Input;
            pin.RegisterInterruptCallback(EdgeDetection.FallingAndRisingEdge, CheckForMoisture);
        }

        public void OnStop()
        {
            _logger.LogInformation("Inside on stop");
            var pin = Pi.Gpio[BcmPin.Gpio02];
            pin.PinMode = GpioPinDriveMode.Output;

            if (pin.Read())
            {
                _logger.LogInformation("Turning lights off");
                pin.Write(false);
            }

            var pin2 = Pi.Gpio[BcmPin.Gpio03];
            pin2.PinMode = GpioPinDriveMode.Output;

            if (pin2.Read())
            {
                _logger.LogInformation("Turning pumps off");
                pin2.Write(false);
            }
        }

        public void CheckForMoisture()
        {
            _logger.LogInformation("Inside low moist");
            var sensor = Pi.Gpio[BcmPin.Gpio23].Read();
            _logger.LogInformation($"Sensor on pin 23: {sensor}");

            // water detected
            if (!sensor)
            {
                _logger.LogInformation("Water dectected");
                _logger.LogInformation($"Pump in is set to: {Pi.Gpio[BcmPin.Gpio03].Read()}");
                if (Pi.Gpio[BcmPin.Gpio03].Read())
                {
                    var pumpPin = Pi.Gpio[BcmPin.Gpio03];
                    pumpPin.PinMode = GpioPinDriveMode.Output;

                    _logger.LogInformation("Stopping water pump");
                    pumpPin.Write(false);
                }
            }
            else
            {
                _logger.LogInformation("no Water dectected");
                _logger.LogInformation($"Pump in is set to: {Pi.Gpio[BcmPin.Gpio03].Read()}");
                if (!Pi.Gpio[BcmPin.Gpio03].Read())
                {
                    var pumpPin = Pi.Gpio[BcmPin.Gpio03];
                    pumpPin.PinMode = GpioPinDriveMode.Output;

                    _logger.LogInformation("Starting water pump");
                    pumpPin.Write(true);
                }
            }

            _logger.LogInformation("leaving method");
            Thread.Sleep(3000);
        }
    }
}

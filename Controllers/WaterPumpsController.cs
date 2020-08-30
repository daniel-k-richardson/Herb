using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;

namespace Herb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WaterPumpsController : ControllerBase
    {
        private ILogger<WaterPumpsController> _logger { get; set; }

        public WaterPumpsController(ILogger<WaterPumpsController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("on")]
        public IActionResult LightsOn()
        {
            var pin = Pi.Gpio[BcmPin.Gpio03];
            pin.PinMode = GpioPinDriveMode.Output;

            if (pin.Read())
            {
                _logger.LogInformation("Pump already on.");
                return Content("Pump already on.");
            }

            pin.Write(true);
            return Content("Pump on");
        }

        [AllowAnonymous]
        [HttpPost("off")]
        public IActionResult LightsOff()
        {
            var pin = Pi.Gpio[BcmPin.Gpio03];
            pin.PinMode = GpioPinDriveMode.Output;

            if (!pin.Read())
            {
                _logger.LogInformation("Pump already off.");
                return Content("Pump already off.");
            }

            pin.Write(false);
            return Content("Pump off");
        }
    }
}

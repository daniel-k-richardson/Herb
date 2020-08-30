using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;

namespace Herb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LightsController : ControllerBase
    {

        private readonly ILogger<LightsController> _logger;

        public LightsController(ILogger<LightsController> logger)
        {
            _logger = logger;
            _logger.LogInformation("Lights controller reached");
        }

        public void Test()
        {

        }

        [AllowAnonymous]
        [HttpPost("on")]
        public IActionResult LightsOn()
        {
            var pin = Pi.Gpio[BcmPin.Gpio02];
            pin.PinMode = GpioPinDriveMode.Output;

            if (pin.Read())
            {
                _logger.LogInformation("Lights already on.");
                return Content("Lights already on.");
            }

            pin.Write(true);
            _logger.LogInformation("Lights on pin.Write(true)");
            return Content("Lights on");
        }

        [AllowAnonymous]
        [HttpPost("off")]
        public IActionResult LightsOff()
        {
            var pin = Pi.Gpio[BcmPin.Gpio02];
            pin.PinMode = GpioPinDriveMode.Output;

            if (!pin.Read())
            {
                _logger.LogInformation("Lights already off.");
                return Content("Lights already off.");
            }

            pin.Write(false);
            _logger.LogInformation("Lights off reached..");
            return Content("Lights off");
        }
    }
}

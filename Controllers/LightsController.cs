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
        }

        [AllowAnonymous]
        [HttpPost("on")]
        public IActionResult LightsOn()
        {
            var pin = Pi.Gpio[BcmPin.Gpio02];
            pin.PinMode = GpioPinDriveMode.Output;
            pin.Write(true);
            return Content("Lights on");
        }

        [AllowAnonymous]
        [HttpPost("off")]
        public IActionResult LightsOff()
        {
            var pin = Pi.Gpio[BcmPin.Gpio02];
            pin.PinMode = GpioPinDriveMode.Output;
            pin.Write(false);
            return Content("Lights off");
        }
    }
}

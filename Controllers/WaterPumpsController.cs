using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;

namespace Herb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WaterPumpsController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("on")]
        public IActionResult LightsOn()
        {
            var pin = Pi.Gpio[BcmPin.Gpio03];
            pin.PinMode = GpioPinDriveMode.Output;
            pin.Write(true);
            return Content("Pump on");
        }

        [AllowAnonymous]
        [HttpPost("off")]
        public IActionResult LightsOff()
        {
            var pin = Pi.Gpio[BcmPin.Gpio03];
            pin.PinMode = GpioPinDriveMode.Output;
            pin.Write(false);
            return Content("Pump off");
        }
    }
}

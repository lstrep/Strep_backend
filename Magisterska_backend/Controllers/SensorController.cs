using Magisterska_backend.Models;
using Magisterska_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Magisterska_backend.Controllers
{
    [ApiController]
    public class SensorController : ControllerBase
    {
        private readonly IInfluxDBService _dbService;
        private readonly IWebSocketService _webSocketService;
        public SensorController(IInfluxDBService dbService, IWebSocketService webSocketService)
        {
            _dbService = dbService;
            _webSocketService = webSocketService;   
        }

        [HttpGet]
        [Route("SensorData")]
        public async Task<IActionResult> GetSensorData()
        {
            var data = await _dbService.GetInfluxDbData();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddFakeData")]
        public async Task<IActionResult> AddFakeData(AddFakeSensorData[] fakeSensorData)
        {
            try
            {
                if (fakeSensorData == null)
                    return BadRequest();

                foreach(var sensor in fakeSensorData)
                {
                    _webSocketService.UpdateData(sensor.Name, sensor.Temperature, sensor.Humidity);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex );
            }
        }
    }
}

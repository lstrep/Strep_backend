using Magisterska_backend.Models;
using Magisterska_backend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Magisterska_backend.Services
{
    public class WebSocketService : IWebSocketService
    {
        private readonly IHubContext<MatrixHub> _hubContext;
        private static Dictionary<string, Sensor> _sensors = new Dictionary<string, Sensor>();

        public WebSocketService(IHubContext<MatrixHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async void UpdateData(string sensorId, double temperature, double humidity)
        {
            if (!_sensors.TryGetValue(sensorId, out var sensor))
            {
                sensor = new Sensor();
                _sensors[sensorId] = sensor;
            }

            sensor.Temperature = temperature;
            sensor.Humidity = humidity;
            sensor.Name = sensorId;

            await _hubContext.Clients.All.SendAsync("UpdateMatrix", sensor);
        }
    }
}

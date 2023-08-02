using Magisterska_backend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Magisterska_backend.Models
{
    public class MatrixHub : Hub
    {
        private readonly IWebSocketService _temperatureService;

        public MatrixHub(IWebSocketService temperatureService)
        {
            _temperatureService = temperatureService;
        }
    }
}

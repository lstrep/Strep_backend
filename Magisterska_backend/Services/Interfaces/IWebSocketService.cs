namespace Magisterska_backend.Services.Interfaces
{
    public interface IWebSocketService
    {
        void UpdateData(string sensorId, double temperature, double humidity);
    }
}

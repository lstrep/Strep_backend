using Magisterska_backend.Models;

namespace Magisterska_backend.Services.Interfaces
{
    public interface IInfluxDBService
    {
        Task<List<Sensor>> GetInfluxDbData();
        void SaveMessageToDb(double temperature, double humidity, string deviceId);
    }
}

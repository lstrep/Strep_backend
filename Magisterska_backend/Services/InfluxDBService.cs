using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core.Flux.Domain;
using InfluxDB.Client.Writes;
using Magisterska_backend.Models;
using Magisterska_backend.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Magisterska_backend.Services
{
    public class InfluxDBService : IInfluxDBService
    {
        private readonly string influxDbUrl;
        private readonly string influxDbToken;
        private readonly string influxDbBucket;
        private readonly string influxDbOrg;
        private readonly InfluxDBClient _influxDbClient;

        public InfluxDBService(IOptions<InfluxDbSettings> influxDbSettings)
        {
            influxDbUrl = influxDbSettings.Value.DbUrl;
            influxDbBucket = influxDbSettings.Value.DbBucket;
            influxDbToken = influxDbSettings.Value.DbToken;
            influxDbOrg = influxDbSettings.Value.DbOrg;

            _influxDbClient = InfluxDBClientFactory.Create(influxDbUrl, influxDbToken);
        }

        public async Task<T> QueryAsync<T>(Func<QueryApi, Task<T>> action)
        {
            var queryApi = _influxDbClient.GetQueryApi();
            return await action(queryApi);
        }

        public async Task<List<Sensor>> GetInfluxDbData()
        {
            var result = new Dictionary<(string, DateTime), Sensor>();

            await QueryAsync<List<FluxTable>>(async queryApi =>
            {
                var fluxQuery = $"from(bucket:\"{influxDbBucket}\") |> range(start: -7d) |> filter(fn: (r) => r._measurement == \"environment\" and (r._field == \"temperature\" or r._field == \"humidity\"))";

                var fluxTables = await queryApi.QueryAsync(fluxQuery, influxDbOrg);

                foreach (var fluxTable in fluxTables)
                {
                    foreach (var fluxRow in fluxTable.Records)
                    {
                        var field = fluxRow.GetValueByKey("_field");
                        var name = fluxRow.GetValueByKey("device_id");
                        var value = fluxRow.GetValueByKey("_value");
                        var time = DateTime.Parse(fluxRow.GetValueByKey("_time").ToString());

                        if (field.Equals("temperature") || field.Equals("humidity"))
                        {
                            var key = (name.ToString(), time);
                            Sensor sensor;

                            if (!result.TryGetValue(key, out sensor))
                            {
                                sensor = new Sensor
                                {
                                    Name = name.ToString(),
                                    TimeStamp = time
                                };
                                result[key] = sensor;
                            }

                            if (field.Equals("temperature"))
                            {
                                sensor.Temperature = Convert.ToDouble(value);
                            }
                            else if (field.Equals("humidity"))
                            {
                                sensor.Humidity = Convert.ToDouble(value);
                            }
                        }
                    }
                }

                return null;
            });

            var sortedResult = result.Values.OrderBy(sensor => sensor.TimeStamp).ToList();
            return sortedResult;
        }

        public void SaveMessageToDb(double temperature, double humidity, string deviceId)
        {
            var point = PointData.Measurement("environment")
                .Tag("device_id", deviceId)
                .Field("temperature", temperature)
                .Field("humidity", humidity)
                .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

            try
            {
                using var writeApi = _influxDbClient.GetWriteApi();
                writeApi.WritePoint(point, influxDbBucket, influxDbOrg);

                Console.WriteLine("Data written to InfluxDB.");
                Console.WriteLine($"Device: {deviceId}, Temperature: {temperature}, Humidity: {humidity}");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}

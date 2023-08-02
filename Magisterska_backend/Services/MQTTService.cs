using Magisterska_backend.Models;
using Magisterska_backend.Services.Interfaces;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System.Text;
using System.Text.Json;

namespace Magisterska_backend.Services
{
    public class MQTTService : IMQTTService
    {
        private readonly IWebSocketService _temperatureService;
        private readonly IMqttClient _mqttClient;
        private readonly IMqttClientOptions _options;
        private readonly IInfluxDBService _dbService;

        public MQTTService(IWebSocketService temperatureService, IInfluxDBService dBService)
        {
            _temperatureService = temperatureService;

            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            _options = new MqttClientOptionsBuilder()
                .WithClientId("Client2")
                .WithTcpServer("eu1.cloud.thethings.industries", 1883)
                .WithCredentials("hodowla@hodowla", "NNSXS.FB3Y3KURH6YBQJNTCZWJCGW2MJVJLANGBQ4YHJY.IJV2TNVWPUK57RQES6W6PSZT6DTHFOTK4MQ7RB27VBFBTB3Q4CNQ")
                .Build();

            _mqttClient.UseApplicationMessageReceivedHandler(HandleReceivedApplicationMessage);
            _dbService = dBService;
        }

        public async Task StartAsync()
        {
            await _mqttClient.ConnectAsync(_options);
            for (int i = 1; i <= 6; i++)
            {
                string topic = $"v3/hodowla@hodowla/devices/sensor{i}/up";
                await _mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic(topic).Build());
            }
        }

        private void HandleReceivedApplicationMessage(MqttApplicationMessageReceivedEventArgs e)
        {
            string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            payload = payload.Trim();

            try
            {
                var receivedMessage = JsonSerializer.Deserialize<Root>(payload);
                var temperature = receivedMessage.uplink_message.decoded_payload.TempC_DS;
                var humidity = receivedMessage.uplink_message.decoded_payload.Hum_SHT;
                var deviceId = receivedMessage.end_device_ids.device_id;

                _dbService.SaveMessageToDb(temperature, humidity, deviceId);

                _temperatureService.UpdateData(deviceId, temperature, humidity);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error MQQTService: {ex.Message}");
            }
        }
    }
}

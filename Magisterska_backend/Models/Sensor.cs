namespace Magisterska_backend.Models
{
    public class Sensor
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public string Name { get; set; }
        public DateTime TimeStamp { get; set; }

        public Sensor()
        {

        }

        public Sensor(double temperature, double humidity, string name)
        {
            Temperature = temperature;
            Humidity = humidity;
            Name = name;
        }
    }
}

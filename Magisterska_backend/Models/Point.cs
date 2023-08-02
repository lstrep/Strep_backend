namespace Magisterska_backend.Models
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double Temperature { get; set; }
        public bool HasSensor { get; set; }

        public Point(int x, int y, double temperature, bool hasSensor)
        {
            X = x;
            Y = y;
            Temperature = temperature;
            HasSensor = hasSensor;
        }
    }
}

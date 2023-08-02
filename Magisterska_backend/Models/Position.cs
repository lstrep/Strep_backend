namespace Magisterska_backend.Models
{
    public class Coordinates
    {
        public Coordinates(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double x { get; set; }
        public double y { get; set; }
    }
}

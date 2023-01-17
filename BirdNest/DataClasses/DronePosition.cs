namespace BirdNest.Structs
{
    public class DronePosition
    {
        public string id { get; }

        public double x { get; }

        public double y { get; }

        public double distance { get; }

        public bool ndz { get; }

        public DronePosition(string id, double x, double y)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            distance = Distance(x - 250000, y - 250000);
            ndz = distance < 100000;

        }

        private double Distance(double x, double y)
        {
            return Math.Sqrt(Math.Pow((x), 2) + Math.Pow((y), 2));
        }
    }
}
